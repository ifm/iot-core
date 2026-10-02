namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents the format of an element.
/// </summary>
[VariantContractResolver(typeof(FormatContractResolver))]
public abstract class Format
{
    /// <summary>
    /// Defines the format data types.
    /// </summary>
    public static class Types
    {
        /// <summary>
        /// A binary type.
        /// </summary>
        public const string Binary = "binary";

        /// <summary>
        /// A boolean type.
        /// </summary>
        public const string Boolean = "boolean";

        /// <summary>
        /// A number type.
        /// </summary>
        public const string Number = "number";

        /// <summary>
        /// A string type.
        /// </summary>
        public const string String = "string";

        /// <summary>
        /// An enumeration type.
        /// </summary>
        public const string Enum = "enum";

        /// <summary>
        /// A value type.
        /// </summary>
        public const string Value = "value";

        /// <summary>
        /// An array type.
        /// </summary>
        public const string Array = "array";

        /// <summary>
        /// An object type.
        /// </summary>
        public const string Object = "object";

        /// <summary>
        /// A map type.
        /// </summary>
        public const string Map = "map";

        /// <summary>
        /// A reference type.
        /// </summary>
        public const string Reference = "ref";

        /// <summary>
        /// An any type.
        /// </summary>
        public const string Any = "any";

        /// <summary>
        /// A self referencing type.
        /// </summary>
        public const string Self = "self";
    }

    /// <summary>
    /// Defines the format encodings.
    /// </summary>
    public static class Encodings
    {
        /// <summary>
        /// An integer (signed 32 bit) encoding.
        /// </summary>
        public const string Integer = "integer";

        /// <summary>
        /// An integer (signed 8 bit) encoding.
        /// </summary>
        public const string Int8 = "int8";

        /// <summary>
        /// An integer (unsigned 8 bit) encoding.
        /// </summary>
        public const string UInt8 = "uint8";

        /// <summary>
        /// An integer (signed 16 bit) encoding.
        /// </summary>
        public const string Int16 = "int16";

        /// <summary>
        /// An integer (unsigned 16 bit) encoding.
        /// </summary>
        public const string UInt16 = "uint16";

        /// <summary>
        /// An integer (signed 32 bit) encoding.
        /// </summary>
        public const string Int32 = "int32";

        /// <summary>
        /// An integer (unsigned 32 bit) encoding.
        /// </summary>
        public const string UInt32 = "uint32";

        /// <summary>
        /// An integer (signed 64 bit) encoding.
        /// </summary>
        public const string Int64 = "int64";

        /// <summary>
        /// An integer (unsigned 64 bit) encoding.
        /// </summary>
        public const string UInt64 = "uint64";

        /// <summary>
        /// A float encoding.
        /// </summary>
        public const string Float = "float";

        /// <summary>
        /// A double encoding.
        /// </summary>
        public const string Double = "double";

        /// <summary>
        /// An UTF-8 string encoding.
        /// </summary>
        public const string Utf8 = "utf-8";

        /// <summary>
        /// An ASCII string encoding.
        /// </summary>
        public const string Ascii = "ascii";

        /// <summary>
        /// A hex-encoded string encoding.
        /// </summary>
        public const string HexString = "hexstring";

        /// <summary>
        /// A base64 binary encoding.
        /// </summary>
        public const string Base64 = "base64";
    }

    /// <summary>
    /// Defines format namespaces.
    /// </summary>
    public class NameSpaces
    {
        /// <summary>
        /// A JSON namespace (default).
        /// </summary>
        public const string Json = null;
    }

    /// <summary>
    /// Gets the format type.
    /// </summary>
    [VariantProperty("type", IgnoredIfNull = true)]
    public string Type { get; set; }

    /// <summary>
    /// Gets the format encoding.
    /// </summary>
    [VariantProperty("encoding", IgnoredIfNull = true)]
    public string Encoding { get; set; }

    /// <summary>
    /// Gets the format namespace.
    /// </summary>
    [VariantProperty("namespace", IgnoredIfNull = true)]
    public string Namespace { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    protected Format()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="type">The format type.</param>
    /// <param name="encoding">The format encoding.</param>
    /// <param name="ns">The format namespace.</param>
    protected Format(string type, string encoding, string ns)
    {
        Type = type;
        Encoding = encoding;
        Namespace = ns;
    }
}