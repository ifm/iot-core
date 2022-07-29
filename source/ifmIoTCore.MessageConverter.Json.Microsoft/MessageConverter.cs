namespace ifmIoTCore.MessageConverter.Json.Microsoft
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Exceptions;
    using Messages;

    public class MessageConverter : IMessageConverter
    {
        public string Type => "json";

        public string ContentType => "application/json";

        public string Serialize(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            try
            {
                // Create authentication info converter from message
                var authenticationInfoConverter = message.AuthenticationInfo != null ?
                    new InnerAuthenticationInfo(message.AuthenticationInfo.User, message.AuthenticationInfo.Password) :
                    null;

                // Create data converter from message
                var data = message.Data != null ?
                    VariantConverter.ToJsonElement(message.Data) :
                    JsonDocument.Parse("null").RootElement;

                // Create message converter from message
                var messageConverter = new InnerMessage(message.Code,
                    message.Cid,
                    message.Address,
                    data,
                    message.Reply,
                    authenticationInfoConverter);

                // Serialize message converter into json string
                var json = JsonSerializer.Serialize(messageConverter, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

                return json;
            }
            catch (Exception e)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, e.Message);
            }
        }

        public Message Deserialize(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            try
            {
                // Deserialize json string into message converter
                var messageConverter = JsonSerializer.Deserialize<InnerMessage>(json);
                if (messageConverter == null)
                {
                    throw new Exception("Convert json failed");
                }

                // Create message data from message converter
                var data = VariantConverter.FromJsonElement(messageConverter.Data);

                // Create authentication info from message converter
                var authenticationInfo = messageConverter.AuthenticationInfoConverter != null
                    ? new AuthenticationInfo(messageConverter.AuthenticationInfoConverter.User, messageConverter.AuthenticationInfoConverter.Password)
                    : null;

                // Create message from message converter
                var message = new Message(messageConverter.Code,
                    messageConverter.Cid,
                    messageConverter.Address,
                    data,
                    messageConverter.Reply,
                    authenticationInfo);

                return message;
            }
            catch (Exception e)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, e.Message);
            }
        }


        private class InnerAuthenticationInfo
        {
            [JsonPropertyName("user" )]
            public string User { get; }

            [JsonPropertyName("passwd" )]
            public string Password { get; }

            public InnerAuthenticationInfo(string user, string password)
            {
                User = user;
                Password = password;
            }
        }

        private class InnerMessage
        {
            [JsonPropertyName("code" )]
            public int Code { get; set; }

            [JsonPropertyName("cid" )]
            public int Cid { get; set; }

            [JsonPropertyName("adr")]
            public string Address { get; set; }

            [JsonPropertyName("data")]
            public JsonElement Data { get; set; }

            [JsonPropertyName("reply")]
            public string Reply { get; set; }

            [JsonPropertyName("auth")]
            public InnerAuthenticationInfo AuthenticationInfoConverter { get; set; }

            public InnerMessage()
            {
            }
            
            public InnerMessage(int code, int cid, string address, JsonElement data, string reply, InnerAuthenticationInfo authenticationInfoConverter)
            {
                Code = code;
                Cid = cid;
                Address = address;
                Data = data;
                Reply = reply;
                AuthenticationInfoConverter = authenticationInfoConverter;
            }

            
            public InnerMessage(JsonElement code, int cid, string address, JsonElement data, string reply, InnerAuthenticationInfo authenticationInfoConverter)
            {
                if (code.ValueKind == JsonValueKind.String)
                {
                    var codeAsString = code.GetString();
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
                else if (code.ValueKind == JsonValueKind.Number)
                {
                    Code = code.GetInt32();
                }
                else
                {
                    throw new IoTCoreException(ResponseCodes.BadRequest, $"Bad code {code}");
                }
                
                Cid = cid;
                Address = address;
                Data = data;
                Reply = reply;
                AuthenticationInfoConverter = authenticationInfoConverter;
            }
        }
    }
}
