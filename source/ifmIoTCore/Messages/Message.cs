namespace ifmIoTCore.Messages
{
    using Exceptions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Resources;

    /// <summary>
    /// Specifies the message structure for the protocol command interface\n
    /// The class is used for both request and response messages
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The message code\n
        /// For requests it specifies the request type; the possible request codes are defined in RequestCodes\n
        /// For responses it specifies the result of the service execution; the possible response codes are defined in ResponseCodes
        /// </summary>
        [JsonProperty("code", Required = Required.Always)]
        public readonly int Code;

        /// <summary>
        /// The context identifier for the request; the number is returned in the cid field of the corresponding response 
        /// </summary>
        [JsonProperty("cid", Required = Required.Always)]
        public int Cid;

        /// <summary>
        /// The address of the target service that receives the request
        /// </summary>
        [JsonProperty("adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Address;

        /// <summary>
        /// The data which is sent to the service
        /// </summary>
        [JsonProperty("data", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly JToken Data;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code">The message code</param>
        /// <param name="cid">The message cid</param>
        /// <param name="address">The service address</param>
        /// <param name="data">The optional service data</param>
        public Message(int code, int cid, string address, JToken data)
        {
            Code = code;
            Cid = cid;
            Address = address;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code">The message code</param>
        /// <param name="cid">The message cid</param>
        /// <param name="address">The service address</param>
        /// <param name="data">The optional service data</param>
        [JsonConstructor]
        public Message(JValue code, int cid, string address, JToken data)
        {
            if (code != null)
            {
                if (code.Type == JTokenType.String)
                {
                    var codeAsString = code.ToObject<string>();
                    switch (codeAsString.ToLower())
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
                            throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidMessageCode, codeAsString));
                    }
                }
                else if (code.Type == JTokenType.Integer)
                {
                    Code = code.ToObject<int>();
                }
                else
                {
                    throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidMessageCode, code));
                }
            }
            Cid = cid;
            Address = address;
            Data = data;
        }
    }

    /// <summary>
    /// Defines authentication information
    /// </summary>
    public class AuthenticationInfo
    {
        /// <summary>
        /// The user name
        /// </summary>
        [JsonProperty("user", Required = Required.Always)]
        public readonly string User;

        /// <summary>
        /// The password
        /// </summary>
        [JsonProperty("passwd", Required = Required.Always)]
        public readonly string Password;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="user">The user name</param>
        /// <param name="password">The password</param>
        public AuthenticationInfo(string user, string password)
        {
            User = user;
            Password = password;
        }
    }

    public class RequestMessage : Message
    {
        /// <summary>
        /// The optional reply address for the response; if this field is set the response will be sent to this address; 
        /// </summary>
        [JsonProperty("reply", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Reply;

        /// <summary>
        /// The authentication information
        /// </summary>
        [JsonProperty("auth", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly AuthenticationInfo Authentication;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="cid">The message cid</param>
        /// <param name="address">The service address</param>
        /// <param name="data">The optional service data</param>
        /// <param name="reply">The optional response address</param>
        /// <param name="authentication">The authentication information</param>
        public RequestMessage(int cid, string address, JToken data, string reply = null, AuthenticationInfo authentication = null) : 
            base(RequestCodes.Request, cid, address, data)
        {
            Reply = reply;
            Authentication = authentication;
        }

        public RequestMessage(Message message, string reply = null, AuthenticationInfo authentication = null) : 
            base(message.Code, message.Cid, message.Address, message.Data)
        {
            Reply = reply;
            Authentication = authentication;
        }

        [JsonConstructor]
        public RequestMessage(JValue code, int cid, string address, JToken data, string reply = null, AuthenticationInfo authentication = null) 
            : base(code, cid, address, data)
        {
            Reply = reply;
            Authentication = authentication;
        }
    }

    public class EventMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="cid">The message cid</param>
        /// <param name="address">The service address</param>
        /// <param name="data">The optional service data</param>
        public EventMessage(int cid, string address, JToken data) : 
            base(RequestCodes.Event, cid, address, data)
        {
        }

        public EventMessage(Message message) : 
            base(RequestCodes.Event, message.Cid, message.Address, message.Data)
        {
        }

        [JsonConstructor]
        public EventMessage(JValue code, int cid, string address, JToken data) : 
            base(code, cid, address, data)
        {
        }
    }
    public class ResponseMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code">The message code</param>
        /// <param name="cid">The message cid</param>
        /// <param name="address">The service address</param>
        /// <param name="data">The optional service data</param>
        public ResponseMessage(int code, int cid, string address, JToken data) : 
            base(code, cid, address, data)
        {
        }

        [JsonConstructor]
        public ResponseMessage(JValue code, int cid, string address, JToken data) : 
            base(code, cid, address, data)
        {
        }
    }
}