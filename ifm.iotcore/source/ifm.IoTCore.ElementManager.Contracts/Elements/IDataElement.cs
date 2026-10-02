namespace ifm.IoTCore.ElementManager.Contracts.Elements;

using System.ComponentModel;
using ifm.Common.Variant;
using ServiceData.Requests;
using ServiceData.Responses;

/// <summary>
/// Provides functionality to interact with a data element.
/// </summary>
public interface IDataElement : IBaseElement, INotifyPropertyChanged
{
    /// <summary>
    /// Gets the datachanged event element.
    /// </summary>
    IEventElement DataChangedEventElement { get; }
}

/// <summary>
/// Provides functionality to interact with a readable data element.
/// </summary>
public interface IReadDataElement : IDataElement
{
    /// <summary>
    /// Calls the getdata service handler.
    /// </summary>
    /// <param name="info">Additional service information.</param>
    /// <returns>The getdata service response.</returns>
    GetDataResponseServiceData GetData(ServiceInfo info = null);

    /// <summary>
    /// Gets the value.
    /// </summary>
    Variant Value { get; }
}

/// <summary>
/// Represents a strongly typed, read-only data element that provides access to a value of the specified type.
/// </summary>
/// <typeparam name="T">The type of the value contained by the data element.</typeparam>
public interface IReadDataElement<out T> : IReadDataElement
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    new T Value { get; }
}

/// <summary>
/// Provides functionality to interact with a writable data element.
/// </summary>
public interface IWriteDataElement : IDataElement
{
    /// <summary>
    /// Calls the setdata service handler.
    /// </summary>
    /// <param name="data">The setdata service input data.</param>
    /// <param name="info">Additional service information.</param>
    void SetData(SetDataRequestServiceData data, ServiceInfo info = null);

    /// <summary>
    /// Sets the value.
    /// </summary>
    Variant Value { set; }
}

/// <summary>
/// Defines a writable data element that allows setting a value of the specified type.
/// </summary>
/// <typeparam name="T">The type of value that can be written to the data element.</typeparam>
public interface IWriteDataElement<in T> : IWriteDataElement
{
    /// <summary>
    /// Sets the value.
    /// </summary>
    new T Value { set; }
}

/// <summary>
/// Provides functionality to interact with a readable and writable data element.
/// </summary>
public interface IReadWriteDataElement : IReadDataElement, IWriteDataElement
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    new Variant Value { get; set; }
}

/// <summary>
/// Provides functionality to interact with a readable and writable data element.
/// </summary>
public interface IReadWriteDataElement<T> : IReadWriteDataElement, IReadDataElement<T>, IWriteDataElement<T>
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    new T Value { get; set; }
}
