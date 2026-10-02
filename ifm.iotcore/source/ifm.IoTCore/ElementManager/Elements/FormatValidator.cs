namespace ifm.IoTCore.ElementManager.Elements;

using System.Text.RegularExpressions;
using ifm.Common.Variant;
using Common.Exceptions;
using Contracts.Elements.Formats;

internal static class FormatValidator
{
    public static void ThrowIfInvalid(Format format, Variant data)
    {
        if (format == null || data == null) return;

        switch (format)
        {
            case ObjectFormat f:
                {
                    if (data is VariantObject d)
                    {
                        ThrowIfInvalid(f, d);
                    }
                    else
                    {
                        throw new DataInvalidException(detailsMessage: "Data is not an object");
                    }
                    break;
                }
            case ArrayFormat f:
                {
                    if (data is VariantArray d)
                    {
                        ThrowIfInvalid(f, d);
                    }
                    else
                    {
                        throw new DataInvalidException(detailsMessage: "Data is not an array");
                    }
                    break;
                }
            case ValueFormat f:
                {
                    if (data is VariantValue d)
                    {
                        ThrowIfInvalid(f, d);
                    }
                    else
                    {
                        throw new DataInvalidException(detailsMessage: "Data is not a simple value");
                    }
                    break;
                }
        }
    }

    private static void ThrowIfInvalid(ObjectFormat objectFormat, VariantObject data)
    {
        var itemFields = objectFormat.Valuation?.Fields;
        if (itemFields == null) return;
        foreach (var itemField in itemFields)
        {
            if (data.TryGetValue(itemField.Name, out var itemData))
            {
                ThrowIfInvalid(itemField.Format, itemData);
            }
        }
    }

    private static void ThrowIfInvalid(ArrayFormat arrayFormat, VariantArray data)
    {
        var itemFormat = arrayFormat.Valuation?.Format;
        foreach (var item in data)
        {
            ThrowIfInvalid(itemFormat, item);
        }
    }

    private static void ThrowIfInvalid(ValueFormat valueFormat, VariantValue data)
    {
        switch (valueFormat)
        {
            case Int8Format f:
            {
                if (f.Valuation == null) return;
                var value = (sbyte)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case UInt8Format f:
            {
                if (f.Valuation == null) return;
                var value = (byte)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case Int16Format f:
            {
                if (f.Valuation == null) return;
                var value = (short)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case UInt16Format f:
            {
                if (f.Valuation == null) return;
                var value = (ushort)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case IntegerFormat f:
            {
                if (f.Valuation == null) return;
                var value = (int)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case Int32Format f:
            {
                if (f.Valuation == null) return;
                var value = (int)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case UInt32Format f:
            {
                if (f.Valuation == null) return;
                var value = (uint)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case Int64Format f:
            {
                if (f.Valuation == null) return;
                var value = (long)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case UInt64Format f:
            {
                if (f.Valuation == null) return;
                var value = (ulong)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case FloatFormat f:
            {
                if (f.Valuation == null) return;
                var value = (double)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case DoubleFormat f:
            {
                if (f.Valuation == null) return;
                var value = (double)data;
                if (value < f.Valuation.Min || value > f.Valuation.Max) throw new DataOutOfRangeException();
                return;
            }
            case IntegerEnumFormat f:
            {
                if (f.Valuation == null) return;
                var value = ((int)data).ToString();
                if (!f.Valuation.Values.ContainsKey(value)) throw new DataOutOfRangeException();
                return;
            }
            case StringFormat f:
            {
                if (f.Valuation == null) return;
                var value = (string)data;
                if (value.Length < f.Valuation.MinLength || value.Length > f.Valuation.MaxLength) throw new DataOutOfRangeException();
                if (string.IsNullOrEmpty(f.Valuation.Pattern)) return;
                if (!Regex.IsMatch(value, f.Valuation.Pattern)) throw new DataOutOfRangeException();
                return;
            }
            default: return;
        }
    }
}