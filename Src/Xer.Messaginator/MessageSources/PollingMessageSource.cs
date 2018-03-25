using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources
{
    /// <summary>
    /// Represents a source of message that does some polling to wait for any messages.
    /// </summary>
    public abstract class PollingMessageSource<TMessage> : MessageSource<TMessage> where TMessage : class
    {
        #region Declarations
        
        private CancellationToken _receiveCancellationToken;
        private Task _pollingTask;

        #endregion Declarations

        #region Properties
        
        /// <summary>
        /// Message source's polling state.
        /// </summary>
        protected PollingState State { get; private set; } = PollingState.Unstarted;

        /// <summary>
        /// Determine if polling should stop.
        /// </summary>
        protected virtual bool IsTimeToStop => State == PollingState.Stopped;

        /// <summary>
        /// Polling interval.
        /// </summary>
        protected abstract TimeSpan PollingInterval { get; }

        #endregion Properties

        #region IMessageSource Implementation
        
        /// <summary>
        /// Start receiving messages from the source. This does not block.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        public override Task StartReceivingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (State != PollingState.Started)
            { 
                State = PollingState.Started;
                
                try
                {
                    OnStart();
                }
                catch(Exception ex)
                {
                    return TaskUtility.FromException(ex);
                }

                _receiveCancellationToken = cancellationToken;
                _pollingTask = StartPollingAsync(cancellationToken);
            }
            
            return TaskUtility.CompletedTask;
        }

        /// <summary>
        /// Stop receiving messages from the source. This will block until last received message has finished processing.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited until the last received message has finished processing.</returns>
        public override Task StopReceivingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Only change to Stopped state if message source has been started.
            if (State == PollingState.Started)
            {
                State = PollingState.Stopped;

                try
                {
                    OnStop();
                }
                catch(Exception ex)
                {
                    return TaskUtility.FromException(ex);
                }
            }
                        
            // Return polling task so that caller can await
            // until the last received message has finished processing.
            return _pollingTask;
        }

        /// <summary>
        /// Receive a message in-process and publish for processing.
        /// </summary>
        /// <param name="message">Message to receive.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        public override Task ReceiveAsync(MessageContainer<TMessage> message, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (message == null)
            {
                return TaskUtility.FromException(new ArgumentNullException(nameof(message)));
            }

            // Publish manually received command.
            PublishMessage(message);
            return TaskUtility.CompletedTask;
        }

        #endregion IMessageSource Implementation

        #region Abstract Methods
        
        /// <summary>
        /// Try to get a message from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Message container.</returns>
        protected abstract Task<MessageContainer<TMessage>> GetNextMessageAsync(CancellationToken cancellationToken);

        #endregion Abstract Methods

        #region Protected Methods

        /// <summary>
        /// Start polling the source for messages.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task.</returns>
        protected virtual async Task StartPollingAsync(CancellationToken cancellationToken)
        {
            while (!IsTimeToStop && !cancellationToken.IsCancellationRequested)
            {
                // Not awaited. Store so that compiler won't complain.
                Task task = ProcessNextMessage(cancellationToken);

                if (!IsTimeToStop && !cancellationToken.IsCancellationRequested)
                {
                    // Only delay if not stopped or cancelled at this point.
                    await Task.Delay(PollingInterval, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Hook that is executed before message source starts receiving any messages.
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Hook that is executed before message source stops receiving any messages.
        /// </summary>
        protected virtual void OnStop()
        {
        }

        #endregion Protected Methods

        #region Functions
        
        /// <summary>
        /// Try to get a message and publish is message container was received that is not null or empty.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that can be awaited for completion.</returns>
        private async Task ProcessNextMessage(CancellationToken cancellationToken)
        {
            try
            {
                // Asynchronously wait until a message is received.
                MessageContainer<TMessage> receivedMessage = await GetNextMessageAsync(cancellationToken).ConfigureAwait(false);

                // Publish message. This checks for nulls/empty message containers.
                PublishMessage(receivedMessage);
            }
            catch(Exception ex)
            {
                PublishException(ex);
            }
        }

        #endregion Functions

        #region Nested Enum
        
        protected enum PollingState
        {
            Unstarted,
            Started,
            Stopped
        }

        #endregion Nested Enum
    }
}