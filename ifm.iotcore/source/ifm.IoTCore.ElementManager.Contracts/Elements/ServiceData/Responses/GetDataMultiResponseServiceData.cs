namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Responses;

using System.Collections.Generic;
using Common;

/// <summary>
/// Represents the outgoing data for a IDeviceElement.GetDataMulti service call.
/// </summary>
public class GetDataMultiResponseServiceData : Dictionary<string, CodeDataPair>
{
}