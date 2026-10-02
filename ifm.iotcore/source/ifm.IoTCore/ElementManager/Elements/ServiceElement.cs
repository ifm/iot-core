namespace ifm.IoTCore.ElementManager.Elements;

using System;
using System.Collections.Generic;
using ifm.Common.Variant;
using Common;
using Common.Exceptions;
using Contracts.Elements;
using Contracts.Elements.Formats;

internal abstract class ServiceElementBase(string identifier,
    Format format,
    IEnumerable<string> profiles,
    string uid,
    bool isHidden) : BaseElement(Identifiers.Service, 
        identifier, 
        format, 
        profiles, 
        uid, 
        isHidden), IServiceElement
{
    public abstract Variant Invoke(Variant data, ServiceInfo info);

    public AccessControl AccessControl { get; } = new();

    public AccessRestrictions AccessRestrictions { get; set; } = AccessRestrictions.ChangedPassword;

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

internal sealed class ServiceElement<TIn, TOut> : ServiceElementBase, IServiceElement<TIn, TOut>
{
    private readonly Func<IBaseElement, TIn, ServiceInfo, TOut> _func;
    private readonly Func<IBaseElement, TIn, TOut> _func2;

    public ServiceElement(string identifier,
        Func<IBaseElement, TIn, ServiceInfo, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, 
            format ?? new ServiceFormat(new ServiceInputFormat(FormatFactory.Create(typeof(TIn))), FormatFactory.Create(typeof(TOut))), 
            profiles, 
            uid, 
            isHidden)
    {
        _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    public ServiceElement(string identifier,
        Func<IBaseElement, TIn, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier,
            format ?? new ServiceFormat(new ServiceInputFormat(FormatFactory.Create(typeof(TIn))), FormatFactory.Create(typeof(TOut))), 
            profiles, 
            uid, 
            isHidden)
    {
        _func2 = func ?? throw new ArgumentNullException(nameof(func));
    }

    public TOut Invoke(TIn data, ServiceInfo info)
    {
        return _func2 != null ? _func2(this, data) : _func(this, data, info);
    }

    public override Variant Invoke(Variant data, ServiceInfo info)
    {
        return VariantFromObject(Invoke(VariantToObject<TIn>(data), info));
    }
}

internal sealed class ActionServiceElement : ServiceElementBase, IActionServiceElement
{
    private readonly Action<IBaseElement, ServiceInfo> _func;
    private readonly Action<IBaseElement> _func2;

    public ActionServiceElement(string identifier,
        Action<IBaseElement, ServiceInfo> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, 
            format, 
            profiles, 
            uid, 
            isHidden)
    {
        _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    public ActionServiceElement(string identifier,
        Action<IBaseElement> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, 
            format, 
            profiles, 
            uid, 
            isHidden)
    {
        _func2 = func ?? throw new ArgumentNullException(nameof(func));
    }

    public void Invoke(ServiceInfo info)
    {
        if (_func2 != null) _func2(this); else _func(this, info);
    }

    public override Variant Invoke(Variant data, ServiceInfo info)
    {
        Invoke(info);
        return null;
    }
}

internal sealed class GetterServiceElement<TOut> : ServiceElementBase, IGetterServiceElement<TOut>
{
    private readonly Func<IBaseElement, ServiceInfo, TOut> _func;
    private readonly Func<IBaseElement, TOut> _func2;

    public GetterServiceElement(string identifier,
        Func<IBaseElement, ServiceInfo, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier,
            format ?? new ServiceFormat(null, FormatFactory.Create(typeof(TOut))), 
            profiles, 
            uid, 
            isHidden)
    {
        _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    public GetterServiceElement(string identifier,
        Func<IBaseElement, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, 
            format ?? new ServiceFormat(null, FormatFactory.Create(typeof(TOut))), 
            profiles, 
            uid, 
            isHidden)
    {
        _func2 = func ?? throw new ArgumentNullException(nameof(func));
    }

    public TOut Invoke(ServiceInfo info)
    {
        return _func2 != null ? _func2(this) : _func(this, info);
    }

    public override Variant Invoke(Variant data, ServiceInfo info)
    {
        return VariantFromObject(Invoke(info));
    }
}

internal sealed class SetterServiceElement<TIn> : ServiceElementBase, ISetterServiceElement<TIn>
{
    private readonly Action<IBaseElement, TIn, ServiceInfo> _func;
    private readonly Action<IBaseElement, TIn> _func2;

    public SetterServiceElement(string identifier,
        Action<IBaseElement, TIn, ServiceInfo> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier,
            format ?? new ServiceFormat(new ServiceInputFormat(FormatFactory.Create(typeof(TIn))), null), 
            profiles, 
            uid, 
            isHidden)
    {
        _func = func ?? throw new ArgumentNullException(nameof(func));
    }

    public SetterServiceElement(string identifier,
        Action<IBaseElement, TIn> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier,
            format ?? new ServiceFormat(new ServiceInputFormat(FormatFactory.Create(typeof(TIn))), null), 
            profiles, 
            uid, 
            isHidden)
    {
        _func2 = func ?? throw new ArgumentNullException(nameof(func));
    }

    public void Invoke(TIn data, ServiceInfo info)
    {
        if (_func2 != null) _func2(this, data); else _func(this, data, info);
    }

    public override Variant Invoke(Variant data, ServiceInfo info)
    {
        Invoke(VariantToObject<TIn>(data), info);
        return null;
    }
}
