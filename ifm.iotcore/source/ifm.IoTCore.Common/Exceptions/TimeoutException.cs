namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when access to a resource timed out (Response code: 504).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class TimeoutException(string message = "Timeout", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.Timeout, message, detailsMessage);