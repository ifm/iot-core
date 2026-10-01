namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when access to an element is denied (Response code: 401).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class AccessDeniedException(string message = "Access denied", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.AccessDenied, message, detailsMessage);