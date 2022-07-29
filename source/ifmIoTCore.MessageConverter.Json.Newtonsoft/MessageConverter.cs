namespace ifmIoTCore.MessageConverter.Json.Newtonsoft
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Exceptions;
    using Messages;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Bson;
    using global::Newtonsoft.Json.Linq;
    using global::Newtonsoft.Json.Serialization;

    internal class InnerAuthenticationInfoConverter
    {
        [JsonProperty("user", Required = Required.Always)]
        public string User { get; }

        [JsonProperty("passwd", Required = Required.Always)]
        public string Password { get; }

        public InnerAuthenticationInfoConverter(string user, string password)
        {
            User = user;
            Password = password;
        }
    }

    internal class InnerMessageConverter
    {
        [JsonProperty("code", Required = Required.Always)]
        public int Code { get; }

        [JsonProperty("cid", Required = Required.Always)]
        public int Cid { get; }

        [JsonProperty("adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; }

        [JsonProperty("data", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public JToken Data { get; }

        [JsonProperty("reply", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Reply { get; }

        [JsonProperty("auth", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public InnerAuthenticationInfoConverter AuthenticationInfoConverter { get; }

        public InnerMessageConverter(int code, int cid, string address, JToken data, string reply, InnerAuthenticationInfoConverter authenticationInfoConverter)
        {
            Code = code;
            Cid = cid;
            Address = address;
            Data = data;
            Reply = reply;
            AuthenticationInfoConverter = authenticationInfoConverter;
        }

        [JsonConstructor]
        public InnerMessageConverter(JValue code, int cid, string address, JToken data, string reply, InnerAuthenticationInfoConverter authenticationInfoConverter)
        {
            if (code != null)
            {
                if (code.Type == JTokenType.String)
                {
                    var codeAsString = code.ToObject<string>();
                    switch (codeAsString?.ToLower())
                    {
                        case "event":
                            Code = RequestCodes.Event;
                            break;
                        case "request":
                            Code = RequestCodes.Request;
                            break;
                        case "response":
                            Code = ResponseCodes.Success;
                            break;
                        default:
                            throw new IoTCoreException(ResponseCodes.BadRequest, $"Bad code {codeAsString}");
                    }
                }
                else if (code.Type == JTokenType.Integer)
                {
                    Code = code.ToObject<int>();
                }
                else
                {
                    throw new IoTCoreException(ResponseCodes.BadRequest, $"Bad code {code}");
                }
            }
            Cid = cid;
            Address = address;
            Data = data;
            Reply = reply;
            AuthenticationInfoConverter = authenticationInfoConverter;
        }
    }

    public class MessageConverter : IMessageConverter
    {
        public const string JsonSerializerType = "json";
        public const string BsonSerializerType = "bson";

        private readonly Func<InnerMessageConverter, string> _serializeFunc;
        private readonly Func<string, InnerMessageConverter> _deserializeFunc;

        private readonly IContractResolver _contractResolver;
        
        public MessageConverter(string type = "json") : this(type, new PropertyOrderingContractResolver())
        {
        }

        public MessageConverter(string type, IContractResolver contractResolver)
        {
            if (type == JsonSerializerType)
            {
                Type = type;
                _serializeFunc = SerializeToJson;
                _deserializeFunc = DeserializeFromJson;
            }
            else if (type == BsonSerializerType)
            {
                Type = type;
                _serializeFunc = SerializeToBson;
                _deserializeFunc = DeserializeFromBson;
            }
            else
            {
                throw new Exception("Unsupported type");
            }
            _contractResolver = contractResolver;
        }

        public string Type { get; }

        public string ContentType => $"application/{Type}";

        public string Serialize(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            try
            {
                // Create authentication info converter from message
                var authenticationInfoConverter = message.AuthenticationInfo != null ? 
                    new InnerAuthenticationInfoConverter(message.AuthenticationInfo.User, message.AuthenticationInfo.Password) : 
                    null;

                // Create data converter from message
                var data = message.Data != null ? 
                    VariantConverter.ToJToken(message.Data) : 
                    null;

                // Create message converter from message
                var messageConverter = new InnerMessageConverter(message.Code, 
                    message.Cid, 
                    message.Address, 
                    data, 
                    message.Reply, 
                    authenticationInfoConverter);

                // Serialize message converter into string
                return _serializeFunc(messageConverter);
            }
            catch (Exception e)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, e.Message);
            }
        }

        public Message Deserialize(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            try
            {
                // Deserialize string into message converter
                var messageConverter = _deserializeFunc(message);
                if (messageConverter == null)
                {
                    throw new Exception("Convert json failed");
                }

                // Create message data from message converter
                var data = messageConverter.Data != null ? 
                    VariantConverter.FromJToken(messageConverter.Data) : 
                    null;

                // Create authentication info from message converter
                var authenticationInfo = messageConverter.AuthenticationInfoConverter != null
                    ? new AuthenticationInfo(messageConverter.AuthenticationInfoConverter.User, messageConverter.AuthenticationInfoConverter.Password)
                    : null;

                // Create message from message converter
                return new Message(messageConverter.Code, 
                    messageConverter.Cid, 
                    messageConverter.Address, 
                    data,
                    messageConverter.Reply, 
                    authenticationInfo);
            }
            catch (Exception e)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, e.Message);
            }
        }

        private string SerializeToJson(InnerMessageConverter value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = _contractResolver });
        }

        private static InnerMessageConverter DeserializeFromJson(string json)
        {
            return JsonConvert.DeserializeObject<InnerMessageConverter>(json);
        }

        private static string SerializeToBson(InnerMessageConverter value)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BsonDataWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, value);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static InnerMessageConverter DeserializeFromBson(string bson)
        {
            var data = Convert.FromBase64String(bson);

            using (var ms = new MemoryStream(data))
            using (var reader = new BsonDataReader(ms))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<InnerMessageConverter>(reader);
            }
        }
    }

    /// <summary>
    /// This contract resolver puts an "code" property of the json first on the answer object. This is needed by the codesys ifmiot-lib. This lib can only parse 255 characters deep into the received json.
    /// </summary>
    internal class PropertyOrderingContractResolver : DefaultContractResolver
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