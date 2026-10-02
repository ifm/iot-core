namespace ifm.Common;

using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides a base class implementation of the INotifyPropertyChanged and INotifyPropertyChanging interfaces.
/// </summary>
public class NotifyPropertyChangedBase : INotifyPropertyChanged, INotifyPropertyChanging
{
    /// <summary>
    /// The PropertyChanged event handler.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// The PropertyChanging event handler.
    /// </summary>
    public event PropertyChangingEventHandler PropertyChanging;

    /// <summary>
    /// Raises a PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Raises a PropertyChanging event.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
    {
        PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    }
}