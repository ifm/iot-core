namespace ifm.IoTCore.ElementManager.Elements;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ifm.Common.Variant;
using Common;
using Common.Exceptions;
using Contracts.Elements;
using Contracts.Elements.Formats;

internal abstract class DataElementBase(string identifier,
    Format format,
    IEnumerable<string> profiles,
    string uid,
    bool isHidden) : BaseElement(Identifiers.Data, identifier, format, profiles, uid, isHidden), IDataElement
{
    public IEventElement DataChangedEventElement { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaiseDataChanged()
    {
        DataChangedEventElement?.Raise();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Address));
    }

    protected static long TimeStamp => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    protected static T VariantToObject<T>(Variant data)
    {
        try
        {
            return data != null ? Variant.ToObject<T>(data) : default;
        }
        catch (OverflowException e)
        {
            throw new DataOutOfRangeException(e.Message, e.InnerException?.Message);
        }
        catch (Exception e)
        {
            throw new DataInvalidException(e.Message, e.InnerException?.Message);
        }
    }

    protected static Variant VariantFromObject<T>(T data)
    {
        try
        {
            return data != null ? Variant.FromObject(data) : null;
        }
        catch (Exception e)
        {
            throw new DataInvalidException(e.Message, e.InnerException?.Message);
        }
    }
}
