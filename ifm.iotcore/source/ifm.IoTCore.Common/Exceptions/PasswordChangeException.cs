namespace ifm.IoTCore.Common.Exceptions;

/// <summary>
/// The exception that is thrown when the password needs to be changed (Response code: 902).
/// </summary>
public class PasswordChangeException : IoTCoreException
{
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="detailsMessage">The details what caused the error or how to fix it.</param>
    public PasswordChangeException(string message = "The password needs to be changed", string detailsMessage = null) : base(ResponseCodes.PasswordChange, message, detailsMessage)
    {
    }
}