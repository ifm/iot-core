namespace ifm.IoTCore.Common;

using ifm.Common.Variant;

/// <summary>
/// Represents a code data pair.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="code">The code of the query.</param>
/// <param name="data">The data of the element.</param>
/// <param name="timeStamp">The time stamp of the value.</param>
public class CodeDataPair(int code, Variant data, long? timeStamp = null)
{
    /// <summary>
    /// The code of the query.
    /// </summary>
    [VariantProperty("code", Required = true)]
    public int Code { get; set; } = code;

    /// <summary>
    /// The value of the element.
    /// </summary>
    [VariantProperty("data", IgnoredIfNull = true)]
    public Variant Data { get; set; } = data;

    /// <summary>
    ///  The time stamp of the value.
    /// </summary>
    [VariantProperty("timestamp", IgnoredIfNull = true)]
    public long? TimeStamp { get; set; } = timeStamp;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public CodeDataPair() : this (0, null)
    {
    }
}