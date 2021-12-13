namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.GetDataMulti service call
    /// </summary>
    public class GetDataMultiRequestServiceData
    {
        /// <summary>
        /// List of addresses of data elements from which the values are requested
        /// </summary>
        [JsonProperty("datatosend", Required = Required.Always)]
        public readonly List<string> DataToSend;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="dataToSend">List of addresses of data elements from which the values are requested</param>
        public GetDataMultiRequestServiceData(List<string> dataToSend)
        {
            DataToSend = dataToSend;
        }
    }
}
