namespace ifmIoTCore.Messages
{
    /// <summary>
    /// Specifies the request codes for the request messages
    /// </summary>
    public class RequestCodes
    {
        /// <summary>
        /// A request that requires a response
        /// </summary>
        public const int Request = 10;

        /// <summary>
        ///  A request that initiates a transaction
        /// </summary>
        public const int Transaction = 11;

        /// <summary>
        /// A command request
        /// </summary>
        public const int Command = 12;

        /// <summary>
        /// An notification event that does not require a response
        /// </summary>
        public const int Event = 80;
    }
}