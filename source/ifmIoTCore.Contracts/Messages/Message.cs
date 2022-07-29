namespace ifmIoTCore.Messages
{
    using Common.Variant;

    /// <summary>
    /// Defines authentication information
    /// </summary>
    public class AuthenticationInfo
    {
        /// <summary>
        /// The user name
        /// </summary>
        public string User { get; }

        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; }

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
        public int Code { get; }

        /// <summary>
        /// The context identifier for the request; the number is returned in the cid field of the corresponding response 
        /// </summary>
        public int Cid { get; set; }

        /// <summary>
        /// The address of the target service that receives the request
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// The message payload
        /// </summary>
        public Variant Data { get; }

        /// <summary>
        /// The optional reply address for the response; if this field is set the response will be sent to this address; 
        /// </summary>
        public string Reply { get; }

        /// <summary>
        /// The authentication information
        /// </summary>
        public AuthenticationInfo AuthenticationInfo { get; }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code">The message code</param>
        /// <param name="cid">The message cid</param>
        /// <param name="address">The service address</param>
        /// <param name="data">The message payload</param>
        /// <param name="reply">The optional response address</param>
        /// <param name="authenticationInfo">The authentication information</param>
        public Message(int code, int cid, string address, Variant data, string reply = null, AuthenticationInfo authenticationInfo = null)
        {
            Code = code;
            Cid = cid;
            Address = address;
            Data = data;
            Reply = reply;
            AuthenticationInfo = authenticationInfo;
        }
    }
}