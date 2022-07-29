namespace ifmIoTCore.Messages
{
    /// <summary>
    /// Specifies the response codes for the response messages
    /// </summary>
    public static class ResponseCodes
    {
        public static bool IsInformation(int code)
        {
            return 100 <= code && code <= 199;
        }

        public static bool IsSuccess(int code)
        {
            return 200 <= code && code <= 299;
        }

        public static bool IsRedirect(int code)
        {
            return 300 <= code && code <= 399;
        }

        public static bool IsClientError(int code)
        {
            return 400 <= code && code <= 499;
        }

        public static bool IsServerError(int code)
        {
            return 500 <= code && code <= 599;
        }

        public static bool IsApplicationError(int code)
        {
            return 900 <= code && code <= 999;
        }

        public static bool IsError(int code)
        {
            return code >= 400;
        }

        /// <summary>
        /// The service execution is successful
        /// </summary>
        public const int Success = 200;

        /// <summary>
        /// The service request is invalid or malformed
        /// </summary>
        public const int BadRequest = 400;

        /// <summary>
        /// The access to a service is denied
        /// </summary>
        public const int AccessDenied = 401;

        /// <summary>
        /// The service request is forbidden
        /// </summary>
        public const int Forbidden = 403;

        /// <summary>
        /// The requested service does not exist
        /// </summary>
        public const int NotFound = 404;

        /// <summary>
        /// The provided service data is invalid
        /// </summary>
        public const int DataInvalid = 422;

        /// <summary>
        /// The service is locked, disabled or busy at the moment
        /// </summary>
        public const int Locked = 423;

        /// <summary>
        /// Too many service requests
        /// </summary>
        public const int TooManyRequests = 429;

        /// <summary>
        /// Internal error; an error condition, that is not considered and handled by IoTCore and therefore raised by the underlying runtime
        /// </summary>
        public const int InternalError = 500;

        /// <summary>
        /// The service is not implemented
        /// </summary>
        public const int NotImplemented = 501;

        /// <summary>
        /// The service is not available
        /// </summary>
        public const int NotAvailable = 503;

        /// <summary>
        /// The service execution timeout
        /// </summary>
        public const int ExecutionTimeout = 504;

        /// <summary>
        /// The service execution failed
        /// </summary>
        public const int ExecutionFailed = 550;

        /// <summary>
        /// The element already exists
        /// </summary>
        public const int AlreadyExists = 901;
    }
}