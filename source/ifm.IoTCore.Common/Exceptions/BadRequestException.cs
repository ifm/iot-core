namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when a request is invalid or malformed (Response code: 400).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class BadRequestException(string message = "Bad request", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.BadRequest, message, detailsMessage);