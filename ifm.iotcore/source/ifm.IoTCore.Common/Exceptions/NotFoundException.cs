namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when an element can't be found (Response code: 404).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class NotFoundException(string message = "Not found", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.NotFound, message, detailsMessage);