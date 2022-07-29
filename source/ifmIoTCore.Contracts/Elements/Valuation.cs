namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using Common.Variant;

    /// <summary>
    /// Represents a field for an object type valuation
    /// </summary>
    public class Field
    {
        /// <summary>
        /// The field name
        /// </summary>
        [VariantProperty("name", Required = true)]
        public string Name { get; set; }

        /// <summary>
        /// The format of the field item
        /// </summary>
        [VariantProperty("format", Required = true)]
        public Format Format { get; set; }

        /// <summary>
        /// true if field is optional; otherwise false
        /// </summary>
        [VariantProperty("optional", Required = true)]
        public bool Optional { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public Field()
        {
        }

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
        [VariantProperty("min", IgnoredIfNull = true)]
        public object Min { get; set; }

        /// <summary>
        /// The maximum value for the item in an integer or floating point type data element
        /// </summary>
        [VariantProperty("max", IgnoredIfNull = true)]
        public object Max { get; set; }

        /// <summary>
        /// The value to turn off a feature in an integer or floating point type data element
        /// </summary>
        [VariantProperty("off", IgnoredIfNull = true)]
        public object Off { get; set; }

        /// <summary>
        /// The number of decimal places for the item in a floating point type data element
        /// </summary>
        [VariantProperty("decimalplaces", IgnoredIfNull = true)]
        public int? DecimalPlaces { get; set; }

        /// <summary>
        /// The minimum length for the item in a string type data element
        /// </summary>
        [VariantProperty("minlength", IgnoredIfNull = true)]
        public int? MinLength { get; set; }

        /// <summary>
        /// The maximum length for the item in a string type data element
        /// </summary>
        [VariantProperty("maxlength", IgnoredIfNull = true)]
        public int? MaxLength { get; set; }

        /// <summary>
        /// The regular expression to evaluate the item in a string type data element
        /// </summary>
        [VariantProperty("pattern", IgnoredIfNull = true)]
        public string Pattern { get; set; }

        /// <summary>
        /// The list of values for the item in an enumeration type data element
        /// </summary>
        [VariantProperty("valuelist", IgnoredIfNull = true)]
        public Dictionary<string, string> Values { get; set; }

        /// <summary>
        /// The initial value for the item in a data element
        /// </summary>
        [VariantProperty("default", IgnoredIfNull = true)]
        public object DefaultValue { get; set; }

        /// <summary>
        /// The list of fields in an object type data element
        /// </summary>
        [VariantProperty("fields", IgnoredIfNull = true)]
        public List<Field> Fields { get; set; }

        /// <summary>
        /// The base type for an array type data element
        /// </summary>
        [VariantProperty("basetype", IgnoredIfNull = true)]
        public string BaseType { get; set; }

        /// <summary>
        /// The format for an item in an array type data element
        /// </summary>
        [VariantProperty("format", IgnoredIfNull = true)]
        public Format Format { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public Valuation()
        {
        }

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