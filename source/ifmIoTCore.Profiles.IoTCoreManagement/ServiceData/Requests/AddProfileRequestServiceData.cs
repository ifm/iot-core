namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.AddProfile service call
    /// </summary>
    public class AddProfileRequestServiceData
    {
        /// <summary>
        /// The list of element addresses, that specify the elements to which the specified profiles are added
        /// </summary>
        [JsonProperty("adrlist", Required = Required.Always)]
        public readonly List<string> Addresses;

        /// <summary>
        /// The list of profiles to add
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
        /// <param name="addresses">The list of element addresses, that specify the elements to which the specified profiles are added</param>
        /// <param name="profiles">The list of profiles to add</param>
        /// <param name="persist">true, if the request is persisted; otherwise false. false is default</param>
        public AddProfileRequestServiceData(List<string> addresses, List<string> profiles, bool persist = false)
        {
            Addresses = addresses;
            Profiles = profiles;
            Persist = persist;
        }

        /// <summary>
        /// Creates a new instance of the class from a json object
        /// </summary>
        /// <param name="json">The json object which is converted</param>
        /// <returns>The new instance of the class</returns>
        public static AddProfileRequestServiceData FromJson(JToken json)
        {
            try
            {
                return json.ToObject<AddProfileRequestServiceData>();
            }
            catch (Exception e)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, e.Message);
            }
        }
    }
}
