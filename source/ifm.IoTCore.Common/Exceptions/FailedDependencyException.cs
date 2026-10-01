namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when a failed dependency is detected (Response code: 424).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class FailedDependencyException(string message = "Failed dependency", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.FailedDependency, message, detailsMessage);