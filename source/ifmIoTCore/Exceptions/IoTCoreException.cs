namespace ifmIoTCore.Exceptions
{
    using System;

    internal class IoTCoreException : Exception
    {
        public int Code => HResult;

        public IoTCoreException(int code, string message) : base(message)
        {
            HResult = code;
        }
    }
}