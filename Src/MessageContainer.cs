using System;

namespace Xer.Messaginator
{
    /// <summary>
    /// Contains the message.
    /// </summary>
    public class MessageContainer<TMessage> where TMessage : class
    {
        #region Static Declarations

        /// <summary>
        /// Empty message container.
        /// </summary>
        public static readonly MessageContainer<TMessage> Empty = new MessageContainer<TMessage>(default(TMessage));

        #endregion Static Declarations

        #region Properties

        /// <summary>
        /// Checks if message contains a default value.
        /// </summary>
        public bool IsEmpty => Message == default(TMessage);

        /// <summary>
        /// Message.
        /// </summary>
        public TMessage Message { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message to contain.</param>
        public MessageContainer(TMessage message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        #endregion Constructors

        #region Implicit Operators
        
        /// <summary>
        /// Implicit conversion from message container to the message.
        /// </summary>
        /// <param name="messageContainer">Message container that contains the message.</param>
        public static implicit operator TMessage(MessageContainer<TMessage> messageContainer)
        {
            return messageContainer?.Message;
        }

        /// <summary>
        /// Implicit conversion from message to a new instance of a message container.
        /// </summary>
        /// <param name="message">Message that the container will hold.</param>
        public static implicit operator MessageContainer<TMessage>(TMessage message)
        {
            return new MessageContainer<TMessage>(message);
        }

        #endregion Implicit Operators
    }
}