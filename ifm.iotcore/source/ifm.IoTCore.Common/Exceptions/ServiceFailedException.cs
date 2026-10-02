namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when a service failed (Response code: 550).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsCode">The details code what caused the error or how to fix it.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class ServiceFailedException(string message, 
    int detailsCode, 
    string detailsMessage) : IoTCoreException(ResponseCodes.ServiceFailed, message, detailsCode, detailsMessage)
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ServiceFailedException(string message = "Service failed") : this(message, 0, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="detailsCode">The details code what caused the error or how to fix it.</param>
    public ServiceFailedException(string message, int detailsCode) : this(message, detailsCode, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
    public ServiceFailedException(string message, string detailsMessage) : this(message, 0, detailsMessage)
    {
    }
}