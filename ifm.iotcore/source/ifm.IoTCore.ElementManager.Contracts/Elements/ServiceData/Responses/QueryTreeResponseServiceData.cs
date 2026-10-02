namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Responses;

using System.Collections.Generic;
using System.Linq;
using ifm.Common.Variant;

/// <summary>
/// Represents the outgoing data for a IDeviceElement.QueryTree service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="addresses">The list of addresses of the elements retrieved by the query.</param>
public class QueryTreeResponseServiceData(IEnumerable<string> addresses)
{
    /// <summary>
    /// The list of addresses of the elements retrieved by the query.
    /// </summary>
    [VariantProperty("adrlist", Required = true)]
    public List<string> Addresses { get; set; } = addresses?.ToList();

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public QueryTreeResponseServiceData() : this(null)
    {
    }
}