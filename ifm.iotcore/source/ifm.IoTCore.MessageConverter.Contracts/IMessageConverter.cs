namespace ifm.IoTCore.MessageConverter.Contracts;

using Message;

/// <summary>
/// Provides functionality to interact with a message converter.
/// </summary>
public interface IMessageConverter
{
    /// <summary>
    /// Gets the converter type.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Gets the converter content type.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Serializes a message into a string.
    /// </summary>
    /// <param name="message">The message to be converted.</param>
    /// <returns>The converted string.</returns>
    string Serialize(Message message);

    /// <summary>
    /// Deserializes a string into a message.
    /// </summary>
    /// <param name="message">The string to be converted.</param>
    /// <returns>The converted message.</returns>
    Message Deserialize(string message);
}