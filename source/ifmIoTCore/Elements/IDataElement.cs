namespace ifmIoTCore.Elements
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides functionality to interact with a data element through JSON
    /// </summary>
    public interface IDataElement : IBaseElement
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        JToken Value { get; set; }

        /// <summary>
        /// Raises a data changed event; this is required to raise the event, if the data is modified from outside of the element object
        /// </summary>
        void RaiseDataChanged();

        /// <summary>
        /// Gets or sets the datachanged event element
        /// </summary>
        IEventElement DataChangedEventElement { get; set; }
    }

    /// <summary>
    /// Provides functionality to interact with a data element
    /// </summary>
    public interface IDataElement<T> : IDataElement
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        new T Value { get; set; }
    }
}