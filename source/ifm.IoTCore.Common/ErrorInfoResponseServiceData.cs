namespace ifm.IoTCore.Common;

using ifm.Common.Variant;

/// <summary>
/// Represents error information returned from failing service requests.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="code">The error code.</param>
/// <param name="details">The details what caused the error or how to fix the problem.</param>
/// <param name="hexError">The string with additional hex-encoded information of the error.</param>
public class ErrorInfoResponseServiceData(string message,
    int? code = null,
    string details = null,
    string hexError = null)
{
    /// <summary>
    /// Gets the error message.
    /// </summary>
    [VariantProperty("msg", IgnoredIfNull = true)]
    public string Message { get; set; } = message;

    /// <summary>
    /// Gets the error code.
    /// </summary>
    [VariantProperty("code", IgnoredIfNull = true)]
    public int? Code { get; set; } = code;

    /// <summary>
    /// Gets the details what caused the error or how to fix it.
    /// </summary>
    [VariantProperty("details", IgnoredIfNull = true)]
    public string Details { get; set; } = details;

    /// <summary>
    /// Gets the string with additional hex-encoded information of the error.
    /// </summary>
    [VariantProperty("error", IgnoredIfNull = true)]
    public string HexError { get; set; } = hexError;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ErrorInfoResponseServiceData() : this(null)
    {
    }
}