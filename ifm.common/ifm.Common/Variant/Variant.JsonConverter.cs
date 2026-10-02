namespace ifm.Common.Variant;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

/// <summary>
/// Provides a variant base class and variant converters.
/// </summary>
public abstract partial class Variant
{
    /// <summary>
    /// Converts a variant to a json string.
    /// </summary>
    /// <param name="variant">The variant to convert.</param>
    /// <returns>The converted string.</returns>
    public static string ToJsonString(Variant variant)
    {
        return ToJsonElement(variant).ToString();
    }

    /// <summary>
    /// Converts a json string to a variant.
    /// </summary>
    /// <param name="json">The string to convert.</param>
    /// <returns>The converted variant.</returns>
    public static Variant FromJsonString(string json)
    {
        return FromJsonElement(JsonDocument.Parse(json).RootElement, false);
    }

    /// <summary>
    /// Converts a variant to a json element.
    /// </summary>
    /// <param name="variant">The variant to convert.</param>
    /// <returns>The converted element.</returns>
    public static JsonElement ToJsonElement(Variant variant)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new Utf8JsonWriter(memoryStream);
        WriteToUtf8Writer(writer, variant);

        writer.Flush();
        return JsonDocument.Parse(Encoding.UTF8.GetString(memoryStream.ToArray())).RootElement;
    }

    /// <summary>
    /// Converts a json element to a variant.
    /// </summary>
    /// <param name="jsonElement">The string to convert.</param>
    /// <param name="throwOnReadingNull">If true an exception is thrown on reading a null value; otherwise not.</param>
    /// <returns>The converted variant.</returns>
    public static Variant FromJsonElement(JsonElement jsonElement, bool throwOnReadingNull)
    {
        if (jsonElement.ValueKind == JsonValueKind.Undefined)
        {
            return null;
        }

        return FromUtf8Reader(new Utf8JsonReader(Encoding.UTF8.GetBytes(jsonElement.GetRawText())), throwOnReadingNull);
    }

    private static void WriteToUtf8Writer(Utf8JsonWriter writer, VariantObject variantObject)
    {
        writer.WriteStartObject();

        foreach (var item in variantObject)
        {
            writer.WritePropertyName((string)(VariantValue)item.Key);
            WriteToUtf8Writer(writer, item.Value);
        }

        writer.WriteEndObject();
    }

    private static void WriteToUtf8Writer(Utf8JsonWriter writer, VariantArray variantArray)
    {
        writer.WriteStartArray();

        foreach (var item in variantArray)
        {
            WriteToUtf8Writer(writer, item);
        }

        writer.WriteEndArray();
    }

    private static void WriteToUtf8Writer(Utf8JsonWriter writer, VariantValue data)
    {
        switch (data.Type)
        {
            case VariantValue.ValueTypes.Boolean:
                writer.WriteBooleanValue((bool)data);
                break;
            case VariantValue.ValueTypes.Character:
                writer.WriteNumberValue((char)data);
                break;
            case VariantValue.ValueTypes.Int8:
                writer.WriteNumberValue((sbyte)data);
                break;
            case VariantValue.ValueTypes.UInt8:
                writer.WriteNumberValue((byte)data);
                break;
            case VariantValue.ValueTypes.Int16:
                writer.WriteNumberValue((short)data);
                break;
            case VariantValue.ValueTypes.UInt16:
                writer.WriteNumberValue((ushort)data);
                break;
            case VariantValue.ValueTypes.Int32:
                writer.WriteNumberValue((int)data);
                break;
            case VariantValue.ValueTypes.UInt32:
                writer.WriteNumberValue((uint)data);
                break;
            case VariantValue.ValueTypes.Int64:
                writer.WriteNumberValue((long)data);
                break;
            case VariantValue.ValueTypes.UInt64:
                writer.WriteNumberValue((ulong)data);
                break;
            case VariantValue.ValueTypes.Float:
                writer.WriteNumberValue((float)data);
                break;
            case VariantValue.ValueTypes.Double:
                writer.WriteNumberValue((double)data);
                break;
            case VariantValue.ValueTypes.Decimal:
                writer.WriteNumberValue((decimal)data);
                break;
            case VariantValue.ValueTypes.String:
                writer.WriteStringValue((string)data);
                break;
            case VariantValue.ValueTypes.DateTime:
                writer.WriteStringValue((DateTime)data);
                break;
            case VariantValue.ValueTypes.TimeSpan:
                writer.WriteStringValue(((TimeSpan)data).ToString());
                break;
            case VariantValue.ValueTypes.Uri:
                writer.WriteStringValue(((Uri)data).ToString());
                break;
            case VariantValue.ValueTypes.Guid:
                writer.WriteStringValue(((Guid)data).ToString());
                break;
            default:
                throw new Exception($"Unsupported variant value type {data.Type}");
        }
    }

    private static void WriteToUtf8Writer(Utf8JsonWriter writer, Variant variant)
    {
        if (variant is VariantObject variantObject)
        {
            WriteToUtf8Writer(writer, variantObject);
        }
        else if (variant is VariantArray variantArray)
        {
            WriteToUtf8Writer(writer, variantArray);
        }
        else if (variant is VariantValue variantValue)
        {
            WriteToUtf8Writer(writer, variantValue);
        }
        else if (variant == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            throw new Exception("Unknown variant type");
        }
    }
        

    private static Variant FromUtf8Reader(Utf8JsonReader jsonReader, bool throwOnReadingNull)
    {
        var stack = new Stack<Variant>();
        var arrayStack = new Stack<VariantArray>();
        var objectStack = new Stack<VariantObject>();

        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.None:
                    break;

                case JsonTokenType.StartObject:

                    var obj = new VariantObject();
                    objectStack.Push(obj);
                    stack.Push(obj);

                    break;

                case JsonTokenType.EndObject:

                    var objectResult = objectStack.Pop();
                    var objectItem = stack.Pop();

                    Dictionary<string, Variant> variantDictionary = new();

                    // ReSharper disable once PossibleUnintendedReferenceComparison
                    while (objectItem != objectResult)
                    {
                        var propertyName = (string)(VariantValue)stack.Pop();

                        if (variantDictionary != null && variantDictionary.ContainsKey(propertyName))
                        {
                            throw new DuplicateKeyException($"The key '{propertyName}' has already been used in this object. Did you use duplicate keys in your json object?");
                        }

                        variantDictionary.Add(propertyName, objectItem);
                        objectItem = stack.Pop();
                    }

                    // The items need to be reversed, because they come in opposite order of the stack. (Last in first out).
                    foreach (var item in variantDictionary.Reverse())
                    {
                        objectResult.Add(item.Key, item.Value);
                    }

                    if (stack.Count == 0)
                    {
                        return objectResult;
                    }
                    else
                    {
                        stack.Push(objectResult);
                    }

                    break;

                case JsonTokenType.StartArray:

                    var array = new VariantArray();
                    arrayStack.Push(array);
                    stack.Push(array);

                    break;

                case JsonTokenType.EndArray:

                    var arrayResult = arrayStack.Pop();
                    var arrayItem = stack.Pop();

                    // ReSharper disable once PossibleUnintendedReferenceComparison
                    while (arrayItem != arrayResult)
                    {
                        arrayResult.Add(arrayItem);
                        arrayItem = stack.Pop();
                    }

                    // The items need to be reversed, because they come in opposite order of the stack. (Last in first out).
                    arrayResult.Reverse();

                    if (stack.Count == 0)
                    {
                        return arrayResult;
                    }
                    else
                    {
                        stack.Push(arrayResult);
                    }

                    break;

                case JsonTokenType.PropertyName:
                    stack.Push(new VariantValue(jsonReader.GetString()));
                    break;

                case JsonTokenType.Comment:
                    // Comments will be ignored.
                    break;

                case JsonTokenType.String:
                    stack.Push(new VariantValue(jsonReader.GetString()));
                    break;

                case JsonTokenType.Number:
                    stack.Push(new VariantValue(jsonReader.GetDouble()));
                    break;

                case JsonTokenType.True:
                case JsonTokenType.False:
                    stack.Push(new VariantValue(jsonReader.GetBoolean()));
                    break;

                case JsonTokenType.Null:
                    if (throwOnReadingNull)
                    {
                        throw new NullReadException("A null value has been read and throwOnReadingNull is set to true.");
                    }
                    stack.Push(null);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (stack.Count == 1)
        {
            return stack.Pop();
        }

        throw new Exception("Unhandled objects.");

    }

    /// <summary>
    /// Represents a null read error.
    /// </summary>
    public class NullReadException : Exception
    {
        /// <summary>
        /// Initializes an instance of the class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public NullReadException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Represents a duplicate key error.
    /// </summary>
    public class DuplicateKeyException : Exception
    {
        /// <summary>
        /// Initializes an instance of the class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public DuplicateKeyException(string message) : base(message)
        {
        }
    }
}