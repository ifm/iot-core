namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IDeviceElement.GetTree service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="address">The address of the element that serves as the root element for the requested tree.</param>
/// <param name="level">The number of tree levels that are requested.</param>
/// <param name="expandConstValues">If true the const values will be included in the tree, otherwise not.</param>
/// <param name="expandLinks">If true, linked elements are expanded; otherwise not.</param>
/// <param name="expandServiceFormat">If true, the service format is expanded; otherwise not.</param>
public class GetTreeRequestServiceData(string address,
    int? level,
    bool expandConstValues = false,
    bool expandLinks = false,
    bool expandServiceFormat = false)
{
    /// <summary>
    /// The address of the element that serves as the root element for the requested tree .
    /// </summary>
    [VariantProperty("adr", IgnoredIfNull = true)]
    public string Address { get; set; } = address;

    /// <summary>
    /// The number of tree levels that are requested.\n
    /// 0 = no subelements, 1 = 1 level of subelements, 2 = 2 levels of subelements, ...\n
    /// </summary>
    [VariantProperty("level", IgnoredIfNull = true)]
    public int? Level { get; set; } = level;

    /// <summary>
    /// Determines that values of data elements which have a "const_value" profile should be included in the gettree response.
    /// The value will be on the same level as identifier and will have a property name of "value".
    /// </summary>
    [VariantProperty("expand_const_values", IgnoredIfNull = true)]
    public bool ExpandConstValues { get; set; } = expandConstValues;

    /// <summary>
    /// Expand linked elements.
    /// </summary>
    [VariantProperty("expand_links", IgnoredIfNull = true)]
    public bool ExpandLinks { get; set; } = expandLinks;

    /// <summary>
    /// Expand service format.
    /// </summary>
    [VariantProperty("expand_service_format", IgnoredIfNull = true)]
    public bool ExpandServiceFormat { get; set; } = expandServiceFormat;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public GetTreeRequestServiceData() : this(null, null)
    {
    }
}