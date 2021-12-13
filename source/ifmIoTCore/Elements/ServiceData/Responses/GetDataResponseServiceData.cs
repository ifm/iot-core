namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Exceptions;
    using Messages;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Represents the outgoing data for a GetData service call
    /// </summary>
    public class GetDataResponseServiceData
    {
        /// <summary>
        /// The value returned by the service
        /// </summary>
        [JsonProperty("value", Required = Required.Always)]
        private readonly JToken _value;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value to set</param>
        [JsonConstructor]
        public GetDataResponseServiceData(JToken value)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value returned by the service</param>
        public GetDataResponseServiceData(object value)
        {
            _value = value == null ? JValue.CreateNull() : JToken.FromObject(value);
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
