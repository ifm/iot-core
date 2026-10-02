namespace ifm.IoTCore.Common.Exceptions;

using System;

/// <summary>
/// Represents errors that occur during application execution.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="responseCode">The response code.</param>
/// <param name="errorMessage">The error message.</param>
/// <param name="errorCode">The error code.</param>
/// <param name="errorDetails">The details what caused the error or how to fix it.</param>
/// <param name="hexError">The hex-encoded information of the error.</param>
public class IoTCoreException(int responseCode,
    string errorMessage,
    int errorCode,
    string errorDetails,
    string hexError) : Exception(errorMessage)
{
    /// <summary>
    /// Gets the response code.
    /// </summary>
    public int ResponseCode { get; } = responseCode;

    /// <summary>
    /// Gets the error code.
    /// </summary>
    public int? ErrorCode { get; } = errorCode;

    /// <summary>
    /// Gets the error details what caused the error or how to fix the problem.
    /// </summary>
    public string ErrorDetails { get; } = errorDetails;

    /// <summary>
    /// Additional and hex-encoded information of the error.
    /// </summary>
    public string HexError { get; } = hexError;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    public IoTCoreException(int responseCode) : this(responseCode, null, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="errorMessage">The error message.</param>
    public IoTCoreException(int responseCode,
        string errorMessage) : this(responseCode, errorMessage, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    public IoTCoreException(int responseCode,
        string errorMessage,
        int errorCode) : this(responseCode, errorMessage, errorCode, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="errorDetails">The details what caused the error or how to fix it.</param>
    public IoTCoreException(int responseCode,
        string errorMessage,
        string errorDetails) : this(responseCode, errorMessage, 0, errorDetails)
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="responseCode">The response code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorDetails">The details what caused the error or how to fix it.</param>
    public IoTCoreException(int responseCode,
        string errorMessage,
        int errorCode,
        string errorDetails) : this(responseCode, errorMessage, errorCode, errorDetails, null)
    {
    }
}