namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.RemoveElement service call
    /// </summary>
    public class RemoveElementRequestServiceData
    {
        /// <summary>
        /// The address of the element to remove
        /// </summary>
        [JsonProperty("adr", Required = Required.Always)]
        public readonly string Address;

        /// <summary>
        /// true, if the request is persisted; otherwise false. false is default
        /// </summary>
        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the element to remove</param>
        /// <param name="persist">true, if the request is persisted; otherwise false. false is default</param>
        public RemoveElementRequestServiceData(string address, bool persist = false)
        {
            this.Address = address;
            Persist = persist;
        }

        /// <summary>
        /// Creates a new instance of the class from a json object
        /// </summary>
        /// <param name="json">The json object which is converted</param>
        /// <returns>The new instance of the class</returns>
        public static RemoveElementRequestServiceData FromJson(JToken json)
        {
            try
            {
                return json.ToObject<RemoveElementRequestServiceData>();
            }
            catch (Exception e)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, e.Message);
            }
        }
    }
}
