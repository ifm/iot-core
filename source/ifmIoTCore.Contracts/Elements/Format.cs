namespace ifmIoTCore.Elements
{
    using Common.Variant;

    /// <summary>
    /// Represents the format of an IoTCore element\n
    /// Each element can have a format (along with identifier, type, uid, profiles etc.)\n
    /// In case of a data element, the format field describes the format of the data contained in the element
    /// </summary>
    public class Format
    {
        /// <summary>
        /// Format data types
        /// </summary>
        public class Types
        {
            /// <summary>
            /// A number type
            /// </summary>
            public const string Number = "number";

            /// <summary>
            /// A string type
            /// </summary>
            public const string String = "string";

            /// <summary>
            /// A boolean type
            /// </summary>
            public const string Boolean = "boolean";

            /// <summary>
            /// An object type
            /// </summary>
            public const string Object = "object";

            /// <summary>
            /// An array type
            /// </summary>
            public const string Array = "array";

            /// <summary>
            /// An enumeration type
            /// </summary>
            public const string Enum = "enum";
        }

        /// <summary>
        /// Format encodings
        /// </summary>
        public class Encodings
        {
            /// <summary>
            /// An integer encoding
            /// </summary>
            public const string Integer = "integer";

            /// <summary>
            /// A float encoding
            /// </summary>
            public const string Float = "float";

            /// <summary>
            /// An UTF-8 encoding
            /// </summary>
            public const string Utf8 = "utf-8";

            /// <summary>
            /// An ASCII encoding
            /// </summary>
            public const string Ascii = "ascii";

            /// <summary>
            /// A hex-encoded string encoding
            /// </summary>
            public const string HexString = "hexstring";
        }

        /// <summary>
        /// The format type
        /// </summary>
        [VariantProperty("type", Required = true)]
        public string Type { get; set; }

        /// <summary>
        /// The format encoding
        /// </summary>
        [VariantProperty("encoding", IgnoredIfNull = true)]
        public string Encoding { get; set; }

        /// <summary>
        /// The format valuation
        /// </summary>
        [VariantProperty("valuation", IgnoredIfNull = true)]
        public Valuation Valuation { get; set; }

        /// <summary>
        /// The format namespace
        /// </summary>
        [VariantProperty("namespace", IgnoredIfNull = true)]
        public string Namespace { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public Format()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="type">The format type</param>
        /// <param name="encoding">The format encoding</param>
        /// <param name="valuation">The format valuation</param>
        /// <param name="ns">The format namespace</param>
        public Format(string type, string encoding, Valuation valuation, string ns = null)
        {
            Type = type;
            Encoding = encoding;
            Valuation = valuation;
            Namespace = string.IsNullOrEmpty(ns) ? "json" : ns;
        }
    }
}