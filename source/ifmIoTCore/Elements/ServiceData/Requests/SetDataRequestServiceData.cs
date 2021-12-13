namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Exceptions;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the incoming data for a SetData service call
    /// </summary>
    public class SetDataRequestServiceData
    {
        /// <summary>
        /// The value to set
        /// </summary>
        [JsonProperty("newvalue", Required = Required.Always)]
        private readonly JToken _value;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value to set</param>
        [JsonConstructor]
        public SetDataRequestServiceData(JToken value)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value to set</param>
        public SetDataRequestServiceData(object value)
        {
            _value = JToken.FromObject(value);
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        /// <typeparam name="T">The data type of the value</typeparam>
        /// <returns>The value</returns>
        public T GetValue<T>()
        {
            if (_value == null) throw new IoTCoreException(ResponseCodes.BadRequest, "json is null");
            return _value.ToObject<T>();
        }
    }
}
