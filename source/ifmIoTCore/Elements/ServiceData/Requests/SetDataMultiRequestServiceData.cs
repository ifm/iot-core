namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.SetDataMulti service call
    /// </summary>
    public class SetDataMultiRequestServiceData
    {
        /// <summary>
        /// List of addresses of data elements and the values to set
        /// </summary>
        [JsonProperty("datatosend", Required = Required.Always)]
        public readonly Dictionary<string, JToken> DataToSend;

        public SetDataMultiRequestServiceData(Dictionary<string, JToken> dataToSend)
        {
            DataToSend = dataToSend;
        }
    }
}
