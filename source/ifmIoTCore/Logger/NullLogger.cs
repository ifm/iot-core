namespace ifmIoTCore.Logger
{
    /// <summary>
    /// Implements a default logger that does nothing.
    /// </summary>
    public class NullLogger : ILogger
    {
        public void Info(string message)
        {
            // This instance doesnt do nothing here.
        }

        public void Warning(string message)
        {
            // This instance doesnt do nothing here.
        }

        public void Error(string message)
        {
            // This instance doesnt do nothing here.
        }

        public void Debug(string message)
        {
            // This instance doesnt do nothing here.
        }
    }
}
