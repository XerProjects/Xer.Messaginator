using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources
{
    /// <summary>
    /// Represents a source of message that does some polling to wait for any messages.
    /// </summary>
    public abstract class PollingMessageSource<TMessage> : IMessageSource<TMessage> where TMessage : class
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

        #region Events
        
        /// <summary>
        /// Received messages are published through this event.
        /// </summary>
        /// <remarks>
        /// It is recommended that subscribers of event handler should not 
        /// let any exceptions propagate because it may not be observed.
        /// </remarks>
        public event MessageReceivedDelegate<TMessage> MessageReceived;

        /// <summary>
        /// Exceptions that occurred while receiving messages are published through this event.
        /// </summary>
        public event EventHandler<Exception> OnError;

        #endregion Events

        #region IMessageSource Implementation
        
        /// <summary>
        /// Start receiving messages from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        public Task StartReceivingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (State != PollingState.Started)
            { 
                State = PollingState.Started;

                OnStart();

                _receiveCancellationToken = cancellationToken;
                _pollingTask = StartPollingAsync(cancellationToken);
            }
            
            return TaskUtility.CompletedTask;
        }

        /// <summary>
        /// Stop receiving messages from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited until the last received message has finished processing.</returns>
        public Task StopReceivingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Only change to Stopped state if message source has been started.
            if (State == PollingState.Started)
            {
                State = PollingState.Stopped;

                OnStop();

                // Remove all subscriptions.
                MessageReceived = null;
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
        public Task ReceiveAsync(MessageContainer<TMessage> message)
        {
            if (message == null)
            {
                return TaskUtility.FromException(new ArgumentNullException(nameof(message)));
            }

            // Publish manually received command.
            publishMessage(message);
            
            return TaskUtility.CompletedTask;
        }

        #endregion IMessageSource Implementation

        #region Abstract Methods
        
        /// <summary>
        /// Try to get a message from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Message container.</returns>
        protected abstract Task<MessageContainer<TMessage>> TryGetMessageAsync(CancellationToken cancellationToken);

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
                Task getAndPublishTask = tryGetAndPublishMessageAsync(cancellationToken);

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
        private async Task tryGetAndPublishMessageAsync(CancellationToken cancellationToken)
        {
            // Asynchronously wait until a message is received.
            MessageContainer<TMessage> receivedMessage = await TryGetMessageAsync(cancellationToken).ConfigureAwait(false);
            if (receivedMessage != null && !receivedMessage.IsEmpty)
            {
                publishMessage(receivedMessage);
            }
        }

        /// <summary>
        /// Publish received message.
        /// </summary>
        /// <param name="receivedMessage">Received message.</param>
        private void publishMessage(MessageContainer<TMessage> receivedMessage)
        {
            if(MessageReceived != null)
            {
                // THis is not awaited.
                MessageReceived(receivedMessage);
            }
        }

        /// <summary>
        /// Publish received message.
        /// </summary>
        /// <param name="receivedMessage">Received message.</param>
        private void publishException(Exception exception)
        {
            if(OnError != null)
            {
                OnError(this, exception);
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