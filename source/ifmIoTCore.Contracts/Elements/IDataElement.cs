namespace ifmIoTCore.Elements
{
    using System.ComponentModel;
    using Common.Variant;

    /// <summary>
    /// Provides basic functionality to interact with a data element
    /// </summary>
    public interface IDataElement : IBaseElement, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        Variant Value { get; set; }

        /// <summary>
        /// Gets the getdata service element
        /// </summary>
        IServiceElement GetDataServiceElement { get; }

        /// <summary>
        /// Gets the setdata service element
        /// </summary>
        IServiceElement SetDataServiceElement { get; }

        /// <summary>
        /// Gets the datachanged event element
        /// </summary>
        IEventElement DataChangedEventElement { get; }

        /// <summary>
        /// Raises a data changed event
        /// </summary>
        void RaiseDataChanged();
    }
}