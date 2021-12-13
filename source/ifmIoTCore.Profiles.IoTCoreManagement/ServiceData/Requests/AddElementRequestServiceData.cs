namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System;
    using System.Collections.Generic;
    using Elements.Formats;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.AddElement service call
    /// </summary>
    public class AddElementRequestServiceData
    {
        /// <summary>
        /// The address of the parent element to which the element is added
        /// </summary>
        [JsonProperty("adr", Required = Required.Always)]
        public readonly string Address;

        /// <summary>
        /// The type of the element
        /// </summary>
        [JsonProperty("type", Required = Required.Always)]
        public readonly string Type;

        /// <summary>
        /// The identifier of the element
        /// </summary>
        [JsonProperty("identifier", Required = Required.Always)]
        public readonly string Identifier;

        /// <summary>
        /// The format of the element
        /// </summary>
        [JsonProperty("format", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly Format Format;

        /// <summary>
        /// The unique identifier of the element
        /// </summary>
        [JsonProperty("uid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string UId;

        /// <summary>
        /// The list of profiles the element belongs to
        /// </summary>
        [JsonProperty("profiles", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly List<string> Profiles;

        /// <summary>
        /// Adds a datachanged event element if true and the created element is a data element; otherwise not
        /// </summary>
        [JsonProperty("adddatachanged", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool AddDataChanged;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the parent element to which the element is added</param>
        /// <param name="type">The type of the element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="format">The format of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="profiles">The list of profiles the element belongs to</param>
        public AddElementRequestServiceData(string address, string type, string identifier, Format format = null, string uid = null, List<string> profiles = null, bool addDataChanged = false)
        {
            Address = address;
            Type = type;
            Identifier = identifier;
            Format = format;
            UId = uid;
            Profiles = profiles;
            AddDataChanged = addDataChanged;
        }

        /// <summary>
        /// Creates a new instance of the class from a json object
        /// </summary>
        /// <param name="json">The json object which is converted</param>
        /// <returns>The new instance of the class</returns>
        public static AddElementRequestServiceData FromJson(JToken json)
        {
            try
            {
                return json.ToObject<AddElementRequestServiceData>();
            }
            catch (Exception e)
            {
                throw new ServiceException(ResponseCodes.DataInvalid, e.Message);
            }
        }
    }
}
