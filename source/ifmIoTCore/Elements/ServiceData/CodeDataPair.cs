namespace ifmIoTCore.Elements.ServiceData
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CodeDataPair
    {
        /// <summary>
        /// The code of the query
        /// </summary>
        [JsonProperty("code", Required = Required.Always)]
        public readonly int Code;

        /// <summary>
        /// The value of the element
        /// </summary>
        [JsonProperty("data", Required = Required.Default)]
        public readonly JToken Data;

        public CodeDataPair(int code, JToken data)
        {
            this.Code = code;
            this.Data = data;
        }
    }
}