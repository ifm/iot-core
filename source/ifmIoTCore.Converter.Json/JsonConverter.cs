namespace ifmIoTCore.Converter.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Newtonsoft.Json;
    using Messages;
    using NetAdapter;
    using Newtonsoft.Json.Serialization;

    public class JsonConverter : IConverter
    {
        private readonly IContractResolver _contractResolver;
        
        public JsonConverter() : this(new PropertyOrderingContractResolver())
        {
        }

        public JsonConverter(IContractResolver contractResolver)
        {
            this._contractResolver = contractResolver;
        }

        public string Type => "json";
        public string ContentType => "application/json";

        public string Serialize<T>(T message) where T : Message
        {
            try
            {
                return JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = this._contractResolver});
            }
            catch (Exception e)
            {
                throw new ServiceException(ResponseCodes.BadRequest, e.Message);
            }
        }

        public T Deserialize<T>(string message) where T : Message
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(message);
            }
            catch (Exception e)
            {
                throw new ServiceException(ResponseCodes.BadRequest, e.Message);
            }
        }
    }

    /// <summary>
    /// This contract resolver puts an "code" property of the json first on the answer object. This is needed by the codesys ifmiot-lib. This lib can only parse 255 characters deep into the received json.
    /// </summary>
    public class PropertyOrderingContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list =  base.CreateProperties(type, memberSerialization);
            var item = list.FirstOrDefault(x => string.Equals(x.PropertyName, "code", StringComparison.InvariantCultureIgnoreCase));

            if (item != null)
            {
                var index = list.IndexOf(item);

                if (index != -1)
                {
                    list.RemoveAt(index);
                    list.Insert(0, item);
                }
            }

            return list;
        }
    }
}