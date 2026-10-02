namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Provides the contract resolver to convert between Variant and Format types.
/// </summary>
public class FormatContractResolver : IVariantContractResolver
{
    /// <summary>
    /// Creates a new Format object from the given Variant object.
    /// </summary>
    /// <param name="data">The Variant object to convert.</param>
    /// <returns>The newly created Format object.</returns>
    public object CreateInstance(Variant data)
    {
        if (data is VariantObject variantObject)
        {
            return CreateFormat(variantObject);
        }
        return null;
    }

    private static Format CreateFormat(VariantObject data)
    {
        string ns = null;
        if (data.TryGetValue("namespace", out var value))
        {
            ns = (string)value.AsVariantValue();
        }


        if (data.ContainsKey("input") || data.ContainsKey("output"))
        {
            return CreateServiceFormat(data, ns);
        }

        return CreateDataFormat(data, ns);
    }

    private static ServiceFormat CreateServiceFormat(VariantObject data, string ns)
    {
        ServiceInputFormat inputFormat = null;
        if (data.TryGetValue("input", out var value))
        {
            inputFormat = value.ToObject<ServiceInputFormat>();
        }
        Format outputFormat = null;
        if (data.TryGetValue("output", out value))
        {
            outputFormat = value.ToObject<Format>();
        }
        return new ServiceFormat(inputFormat, outputFormat, ns);
    }

    private static Format CreateDataFormat(VariantObject data, string ns)
    {
        var type = (string)data["type"].AsVariantValue();

        return type switch
        {
            Format.Types.Binary => CreateBinaryFormat(),
            Format.Types.Boolean => CreateBooleanFormat(data, ns),
            Format.Types.Number => CreateNumberFormat(data, ns),
            Format.Types.String => CreateStringFormat(data, ns),
            Format.Types.Enum => CreateIntegerEnumFormat(data, ns),
            Format.Types.Array => CreateArrayFormat(data, ns),
            Format.Types.Object => CreateObjectFormat(data, ns),
            Format.Types.Map => CreateMapFormat(data, ns),
            Format.Types.Reference => CreateReferenceFormat(data),
            Format.Types.Any => CreateAnyFormat(),
            Format.Types.Self => CreateSelfFormat(),
            _ => null
        };
    }

    private static BooleanFormat CreateBooleanFormat(VariantObject data, string ns)
    {
        BooleanValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<BooleanValuation>();
        }
        return new BooleanFormat(valuation, ns);
    }

    private static Format CreateNumberFormat(VariantObject data, string ns)
    {
        string encoding = null;
        if (data.TryGetValue("encoding", out var value))
        {
            encoding = (string)value.AsVariantValue();
        }

        return encoding switch
        {
            Format.Encodings.Int8 => CreateInt8Format(data, ns),
            Format.Encodings.UInt8 => CreateUInt8Format(data, ns),
            Format.Encodings.Int16 => CreateInt16Format(data, ns),
            Format.Encodings.UInt16 => CreateUInt16Format(data, ns),
            Format.Encodings.Int32 => CreateInt32Format(data, ns),
            Format.Encodings.Integer => CreateIntegerFormat(data, ns),
            Format.Encodings.UInt32 => CreateUInt32Format(data, ns),
            Format.Encodings.Int64 => CreateInt64Format(data, ns),
            Format.Encodings.UInt64 => CreateUInt64Format(data, ns),
            Format.Encodings.Float => CreateFloatFormat(data, ns),
            Format.Encodings.Double => CreateDoubleFormat(data, ns),
            _ => null
        };
    }

    private static Format CreateBinaryFormat()
    {
        return new BinaryFormat();
    }

    private static Int8Format CreateInt8Format(VariantObject data, string ns)
    {
        Int8Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<Int8Valuation>();
        }
        return new Int8Format(valuation, ns);
    }

    private static UInt8Format CreateUInt8Format(VariantObject data, string ns)
    {
        UInt8Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<UInt8Valuation>();
        }
        return new UInt8Format(valuation, ns);
    }

    private static Int16Format CreateInt16Format(VariantObject data, string ns)
    {
        Int16Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<Int16Valuation>();
        }
        return new Int16Format(valuation, ns);
    }

    private static UInt16Format CreateUInt16Format(VariantObject data, string ns)
    {
        UInt16Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<UInt16Valuation>();
        }
        return new UInt16Format(valuation, ns);
    }

    private static Int32Format CreateInt32Format(VariantObject data, string ns)
    {
        Int32Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<Int32Valuation>();
        }
        return new Int32Format(valuation, ns);
    }

    private static IntegerFormat CreateIntegerFormat(VariantObject data, string ns)
    {
        Int64Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<Int64Valuation>();
        }
        return new IntegerFormat(valuation, ns);
    }

    private static UInt32Format CreateUInt32Format(VariantObject data, string ns)
    {
        UInt32Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<UInt32Valuation>();
        }
        return new UInt32Format(valuation, ns);
    }

    private static Int64Format CreateInt64Format(VariantObject data, string ns)
    {
        Int64Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<Int64Valuation>();
        }
        return new Int64Format(valuation, ns);
    }

    private static UInt64Format CreateUInt64Format(VariantObject data, string ns)
    {
        UInt64Valuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<UInt64Valuation>();
        }
        return new UInt64Format(valuation, ns);
    }

    private static FloatFormat CreateFloatFormat(VariantObject data, string ns)
    {
        FloatValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<FloatValuation>();
        }
        return new FloatFormat(valuation, ns);
    }

    private static DoubleFormat CreateDoubleFormat(VariantObject data, string ns)
    {
        DoubleValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<DoubleValuation>();
        }
        return new DoubleFormat(valuation, ns);
    }

    private static StringFormat CreateStringFormat(VariantObject data, string ns)
    {
        string encoding = null;
        if (data.TryGetValue("encoding", out var value))
        {
            encoding = (string)value.AsVariantValue();
        }

        StringValuation valuation = null;
        if (data.TryGetValue("valuation", out value))
        {
            valuation = value.ToObject<StringValuation>();
        }
        return new StringFormat(valuation, encoding, ns);
    }

    private static IntegerEnumFormat CreateIntegerEnumFormat(VariantObject data, string ns)
    {
        IntegerEnumValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<IntegerEnumValuation>();
        }
        return new IntegerEnumFormat(valuation, ns);
    }

    private static ArrayFormat CreateArrayFormat(VariantObject data, string ns)
    {
        ArrayValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<ArrayValuation>();
        }
        return new ArrayFormat(valuation, ns);
    }

    private static ObjectFormat CreateObjectFormat(VariantObject data, string ns)
    {
        ObjectValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<ObjectValuation>();
        }
        return new ObjectFormat(valuation, ns);
    }

    private static MapFormat CreateMapFormat(VariantObject data, string ns)
    {
        MapValuation valuation = null;
        if (data.TryGetValue("valuation", out var value))
        {
            valuation = value.ToObject<MapValuation>();
        }
        return new MapFormat(valuation, ns);
    }

    private static ReferenceFormat CreateReferenceFormat(VariantObject data)
    {
        string reference = null;
        if (data.TryGetValue("ref", out var value))
        {
            reference = (string)value.AsVariantValue();
        }
        return new ReferenceFormat(reference);
    }

    private static Format CreateAnyFormat()
    {
        return new AnyFormat();
    }

    private static Format CreateSelfFormat()
    {
        return new SelfFormat();
    }
}

