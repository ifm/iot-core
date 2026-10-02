namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when data provided with a request is out of range (Response code: 416).
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
public class DataOutOfRangeException(string message = "Data is out of range", 
    string detailsMessage = null) : IoTCoreException(ResponseCodes.DataOutOfRange, message, detailsMessage);