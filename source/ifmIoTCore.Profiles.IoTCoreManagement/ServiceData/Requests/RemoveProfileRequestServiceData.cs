namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.RemoveProfile service call
    /// </summary>
    public class RemoveProfileRequestServiceData
    {
        /// <summary>
        /// The list of element addresses, that specify the elements from which the specified profiles are removed
        /// </summary>
        [JsonProperty("adrlist", Required = Required.Always)]
        public readonly List<string> Addresses;

        /// <summary>
        /// The list of profiles to remove
        /// </summary>
        [JsonProperty("profiles", Required = Required.Always)]
        public readonly List<string> Profiles;

        /// <summary>
        /// true, if the request is persisted; otherwise false. false is default
        /// </summary>
        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="addresses">The list of element addresses, that specify the elements from which the specified profiles are removed</param>
        /// <param name="profiles">The list of profiles to remove</param>
        /// <param name="persist">true, if the request is persisted; otherwise false. false is default</param>
        public RemoveProfileRequestServiceData(List<string> addresses, List<string> profiles, bool persist = false)
        {
            this.Addresses = addresses;
            Profiles = profiles;
            Persist = persist;
        }

        /// <summary>
        /// Creates a new instance of the class from a json object
        /// </summary>
        /// <param name="json">The json object which is converted</param>
        /// <returns>The new instance of the class</returns>
        public static RemoveProfileRequestServiceData FromJson(JToken json)
        {
            try
            {
                return json.ToObject<RemoveProfileRequestServiceData>();
            }
            catch (Exception e)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, e.Message);
            }
        }
    }
}
