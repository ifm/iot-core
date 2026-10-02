namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using System.Collections.Generic;
using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IDeviceElement.GetDataMulti service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="dataToSend">List of addresses of data elements from which the values are requested.</param>
public class GetDataMultiRequestServiceData(List<string> dataToSend)
{
    /// <summary>
    /// List of addresses of data elements from which the values are requested.
    /// </summary>
    [VariantProperty("datatosend", Required = true)]
    public List<string> DataToSend { get; set; } = dataToSend;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public GetDataMultiRequestServiceData() : this(null)
    {
    }
}