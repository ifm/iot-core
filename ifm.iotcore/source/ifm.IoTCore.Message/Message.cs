namespace ifm.IoTCore.Message;

using Common.Variant;

/// <summary>
/// Defines the message.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="code">The message code.</param>
/// <param name="cid">The message cid.</param>
/// <param name="address">The service address.</param>
/// <param name="data">The message payload.</param>
/// <param name="reply">The optional response address.</param>
/// <param name="authenticationInfo">The authentication information.</param>
public class Message(int code,
    int cid,
    string address,
    Variant data,
    string reply = null,
    Message.AuthenticationInfo authenticationInfo = null)
{
    /// <summary>
    /// Defines authentication information.
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="user">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="token">The session token.</param>
    public class AuthenticationInfo(string user, string password, string token = null)
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="token">The session token.</param>
        public AuthenticationInfo(string token) : this(null, null, token)
        { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="password">The password.</param>
        public AuthenticationInfo(string user, string password) : this(user, password, null)
        { }

        /// <summary>
        /// The username.
        /// </summary>
        public string User { get; set; } = user;

        /// <summary>
        /// The password.
        /// </summary>
        public string Password { get; set; } = password;

        /// <summary>
        /// The session token.
        /// </summary>
        public string Token { get; set; } = token;
    }

    /// <summary>
    /// The message code.
    /// </summary>
    public int Code { get; } = code;

    /// <summary>
    /// The context identifier for the request.
    /// </summary>
    public int Cid { get; set; } = cid;

    /// <summary>
    /// The address of the target service.
    /// </summary>
    public string Address { get; } = address;

    /// <summary>
    /// The message payload.
    /// </summary>
    public Variant Data { get; } = data;

    /// <summary>
    /// The optional reply address for the response.
    /// </summary>
    public string Reply { get; } = reply;

    /// <summary>
    /// The authentication information.
    /// </summary>
    public AuthenticationInfo Authentication { get; } = authenticationInfo;
}
