namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when an element already exists (Response code: 901).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class AlreadyExistsException(string message = "Element already exists", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.AlreadyExists, message, detailsMessage);
