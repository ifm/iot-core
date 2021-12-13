namespace ifmIoTCore.Elements.Formats
{
    using Newtonsoft.Json;
    using Valuations;

    /// <summary>
    /// Represents the format of an IoTCore element\n
    /// Each element can have a format (along with identifier, type, uid, profiles etc.)\n
    /// In case of a data element, the format field describes the format of the data contained in the element
    /// </summary>
    public class Format
    {
        /// <summary>
        /// Predefined format types
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
        /// Predefined format encodings
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
        [JsonProperty("type", Required = Required.Always)]
        public readonly string Type;

        /// <summary>
        /// The format encoding
        /// </summary>
        [JsonProperty("encoding", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Encoding;

        /// <summary>
        /// The format valuation
        /// </summary>
        [JsonProperty("valuation", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly Valuation Valuation;

        /// <summary>
        /// The format namespace
        /// </summary>
        [JsonProperty("namespace", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Namespace;

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