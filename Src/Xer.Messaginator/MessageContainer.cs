using System;

namespace Xer.Messaginator
{
    /// <summary>
    /// Represents a container of message.
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

        public MessageProperties Properties { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message to contain.</param>
        public MessageContainer(TMessage message)
            : this(message, MessageProperties.Empty)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Message to contain.</param>
        /// <param name="properties">Message properties.</param>
        public MessageContainer(TMessage message, MessageProperties properties)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Properties = properties ?? throw new ArgumentNullException(nameof(properties));
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

        #endregion Implicit Operators
    }
}