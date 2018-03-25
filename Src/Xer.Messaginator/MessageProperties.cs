using System;
using System.Collections.Generic;

namespace Xer.Messaginator
{
    /// <summary>
    /// Represents properties for a message.
    /// </summary>
    public class MessageProperties
    {
        #region Static Declarations

        /// <summary>
        /// Empty message properties.
        /// </summary>
        public static readonly MessageProperties Empty = new MessageProperties();

        #endregion Static Declarations

        #region Declarations
            
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        #endregion Declarations

        #region Methods
        
        /// <summary>
        /// Add a property.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <param name="property">Value of property.</param>
        public MessageProperties Add<T>(string name, T property)
        {
            _properties.Add(name, new Property(property, typeof(T)));
            return this;
        }

        /// <summary>
        /// Get a property value of default if property was not found.
        /// </summary>
        /// <param name="name">Name of property.</param>
        /// <param name="defaultValue">Default property value that is used if property was not found.</param>
        /// <returns>Value of property.</returns>
        public T GetOrDefault<T>(string name, T defaultValue = default(T))
        {
            if (_properties.TryGetValue(name, out Property property))
            {
                if (property.IsOfType<T>())
                {
                    return (T)property.Value;
                }
            }

            return defaultValue;
        }

        #endregion Methods

        #region Inner Class

        /// <summary>
        /// Represents a property.
        /// </summary>
        private struct Property
        {
            /// <summary>
            /// Value of property.
            /// </summary>
            public object Value { get; private set; }

            /// <summary>
            /// Type of property.
            /// </summary>
            public Type Type { get; private set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="value">Value of property.</param>
            /// <param name="type">Type of property.</param>
            public Property(object value, Type type)
            {
                Value = value;
                Type = type;
            }

            /// <summary>
            /// Check if property is of the given type.
            /// </summary>
            /// <returns>True if property is of the given type. Otherwise, false.</returns>
            public bool IsOfType<T>()
            {
                return typeof(T) == Type;
            }
        }

        #endregion Inner Class
    }
}