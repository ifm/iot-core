namespace ifmIoTCore.Elements.Valuations
{
    using System.Collections.Generic;
    using Formats;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a field for an object type valuation
    /// </summary>
    public class Field
    {
        /// <summary>
        /// The field name
        /// </summary>
        [JsonProperty("name", Required = Required.Always)]
        public readonly string Name;

        /// <summary>
        /// The format of the field item
        /// </summary>
        [JsonProperty("format", Required = Required.Always)]
        public readonly Format Format;

        /// <summary>
        /// true if field is optional; otherwise false
        /// </summary>
        [JsonProperty("optional", Required = Required.Always)]
        public readonly bool Optional;

        /// <summary>
        /// Initializes a new instance of the Field class
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="format">The format for the field</param>
        /// <param name="optional">Flag if field item optional or mandatory; default is false</param>
        public Field(string name, Format format, bool optional = false)
        {
            Name = name;
            Format = format;
            Optional = optional;
        }
    }

    /// <summary>
    /// Represents the valuation parameters for a data element
    /// </summary>
    public class Valuation
    {
        /// <summary>
        /// The minimum value for the item in an integer or floating point type data element
        /// </summary>
        [JsonProperty("min", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly object Min;

        /// <summary>
        /// The maximum value for the item in an integer or floating point type data element
        /// </summary>
        [JsonProperty("max", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly object Max;

        /// <summary>
        /// The value to turn off a feature in an integer or floating point type data element
        /// </summary>
        [JsonProperty("off", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly object Off;

        /// <summary>
        /// The number of decimal places for the item in a floating point type data element
        /// </summary>
        [JsonProperty("decimalplaces", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly int? DecimalPlaces;

        /// <summary>
        /// The minimum length for the item in a string type data element
        /// </summary>
        [JsonProperty("minlength", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly int? MinLength;

        /// <summary>
        /// The maximum length for the item in a string type data element
        /// </summary>
        [JsonProperty("maxlength", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly int? MaxLength;

        /// <summary>
        /// The regular expression to evaluate the item in a string type data element
        /// </summary>
        [JsonProperty("pattern", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Pattern;

        /// <summary>
        /// The list of values for the item in an enumeration type data element
        /// </summary>
        [JsonProperty("valuelist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly Dictionary<string, string> Values;

        /// <summary>
        /// The initial value for the item in a data element
        /// </summary>
        [JsonProperty("default", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly object DefaultValue;

        /// <summary>
        /// The list of fields in an object type data element
        /// </summary>
        [JsonProperty("fields", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly List<Field> Fields;

        /// <summary>
        /// The base type for an array type data element
        /// </summary>
        [JsonProperty("basetype", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string BaseType;

        /// <summary>
        /// The format for an item in an array type data element
        /// </summary>
        [JsonProperty("format", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly Format Format;

        /// <summary>
        /// Initializes a new instance of the Valuation class 
        /// </summary>
        /// <param name="min">The minimum value for the item</param>
        /// <param name="max">The maximum value for the item</param>
        /// <param name="off">The value to turn off the item</param>
        /// <param name="decimalPlaces">The decimal places value for the item</param>
        /// <param name="minLength">The minimum string length for the item</param>
        /// <param name="maxLength">The maximum string length for the item</param>
        /// <param name="pattern">The evaluation pattern for the item</param>
        /// <param name="values">The list of values for the item</param>
        /// <param name="defaultValue">The default value for the item</param>
        /// <param name="fields">The list of fields in the object type data element</param>
        /// <param name="baseType">The base type for item in the the array type data element</param>
        /// <param name="format">The format for an item in the array type data element</param>
        public Valuation(object min, object max, object off, int? decimalPlaces, int? minLength, int? maxLength, string pattern, Dictionary<string, string> values, object defaultValue, List<Field> fields, string baseType, Format format)
        {
            Min = min;
            Max = max;
            Off = off;
            DecimalPlaces = decimalPlaces;
            MinLength = minLength;
            MaxLength = maxLength;
            Pattern = pattern;
            Values = values;
            DefaultValue = defaultValue;
            Fields = fields;
            BaseType = baseType;
            Format = format;
        }
    }
}