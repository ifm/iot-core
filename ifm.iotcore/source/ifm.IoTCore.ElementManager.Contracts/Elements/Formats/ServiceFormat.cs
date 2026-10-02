namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents the service data format.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="format">The format type.</param>
/// <param name="optional">The optional flag.</param>
public class ServiceInputFormat(Format format, bool optional = false)
{
    /// <summary>
    /// The format.
    /// </summary>
    [VariantProperty("format", Required = true)]
    public Format Format { get; set; } = format;

    /// <summary>
    /// Gets the optional flag.
    /// </summary>
    [VariantProperty("optional", Required = true)]
    public bool Optional { get; set; } = optional;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ServiceInputFormat() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of a reference element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="inputFormat">The service input format.</param>
/// <param name="outputFormat">The service output format.</param>
/// <param name="ns">The namespace.</param>
public class ServiceFormat(ServiceInputFormat inputFormat, Format outputFormat, string ns = null) : Format(null, null, ns)
{
    /// <summary>
    /// Gets or sets the format of the input data to be processed by the service.
    /// </summary>
    [VariantProperty("input", IgnoredIfNull = true)]
    public ServiceInputFormat InputFormat { get; set; } = inputFormat;

    /// <summary>
    /// Gets or sets the format of the output data of the service.
    /// </summary>
    [VariantProperty("output", IgnoredIfNull = true)]
    public Format OutputFormat { get; set; } = outputFormat;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ServiceFormat() : this(null, null)
    {
    }
}