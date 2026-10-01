namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using System.Collections.Generic;
using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IDeviceElement.SetDataMulti service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="dataToSend">List of addresses of data elements and the values to set.</param>
public class SetDataMultiRequestServiceData(Dictionary<string, Variant> dataToSend)
{
    /// <summary>
    /// List of addresses of data elements and the values to set.
    /// </summary>
    [VariantProperty("datatosend", Required = true)]
    public Dictionary<string, Variant> DataToSend { get; set; } = dataToSend;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public SetDataMultiRequestServiceData() : this(null)
    {
    }
}