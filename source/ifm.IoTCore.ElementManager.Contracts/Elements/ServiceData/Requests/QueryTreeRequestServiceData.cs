namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IDeviceElement.QueryTree service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="profile">The profile filter.</param>
/// <param name="type">The type filter.</param>
/// <param name="identifier">The name filter.</param>
/// <param name="link">The link filter.</param>
public class QueryTreeRequestServiceData(string profile = null,
    string type = null,
    string identifier = null,
    bool? link = null)
{
    /// <summary>
    /// The profile filter for the query.
    /// </summary>
    [VariantProperty("profile", IgnoredIfNull = true)]
    public string Profile { get; set; } = profile;

    /// <summary>
    /// The type filter for the query.
    /// </summary>
    [VariantProperty("type", IgnoredIfNull = true)]
    public string Type { get; set; } = type;

    /// <summary>
    /// The identifier (name) filter for the query.
    /// </summary>
    [VariantProperty("identifier", IgnoredIfNull = true, AlternativeNames = ["name"])]
    public string Identifier { get; set; } = identifier;

    /// <summary>
    /// The link filter for the query.
    /// </summary>
    [VariantProperty("link", IgnoredIfNull = true)]
    public bool? Link { get; set; } = link;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public QueryTreeRequestServiceData() : this(null)
    {
    }
}