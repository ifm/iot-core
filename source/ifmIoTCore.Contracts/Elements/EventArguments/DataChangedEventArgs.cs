namespace ifmIoTCore.Elements.EventArguments
{
    using System;

    public class DataChangedEventArgs : EventArgs
    {
        public object Value { get; }

        public DataChangedEventArgs(object value)
        {
            Value = value;
        }
    }
}
