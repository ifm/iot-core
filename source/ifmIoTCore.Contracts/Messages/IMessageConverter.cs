namespace ifmIoTCore.Messages
{
    /// <summary>
    /// Provides functionality to interact with a data converter
    /// </summary>
    public interface IMessageConverter
    {
        /// <summary>
        /// Gets the converter type
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the converter content type
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Serializes the message object into a message string
        /// </summary>
        /// <param name="message">
        /// The message object to be converted
        /// </param>
        /// <returns>
        /// A string object which represents the message
        /// </returns>
        string Serialize(Message message);

        /// <summary>
        /// Deserializes the message string into a message object
        /// </summary>
        /// <param name="message">
        /// The message string to be converted
        /// </param>
        /// <returns>
        /// A Message object which represents the message
        /// </returns>
        Message Deserialize(string message);
    }
}
