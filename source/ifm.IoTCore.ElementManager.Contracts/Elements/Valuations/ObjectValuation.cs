namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using System.Collections.Generic;
using ifm.Common.Variant;
using Formats;

/// <summary>
/// Represents the valuation for an object type data element.
/// </summary>
public class ObjectValuation
{
    /// <summary>
    /// Represents a field for an object type valuation.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Gets the field name.
        /// </summary>
        [VariantProperty("name", Required = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets the format of a field.
        /// </summary>
        [VariantProperty("format", IgnoredIfNull = true)]
        public Format Format { get; set; }

        /// <summary>
        /// Gets the optional flag.
        /// </summary>
        [VariantProperty("optional", IgnoredIfNull = true)]
        public bool? Optional { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter.
        /// </summary>
        [VariantConstructor]
        public Field()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The field name.</param>
        /// <param name="format">The format for the field.</param>
        /// <param name="optional">The optional flag.</param>
        public Field(string name, Format format, bool? optional = null)
        {
            Name = name;
            Format = format;
            Optional = optional;
        }
    }

    /// <summary>
    /// Gets the list of fields.
    /// </summary>
    [VariantProperty("fields", IgnoredIfNull = true)]
    public List<Field> Fields { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ObjectValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="fields">The list of fields.</param>
    public ObjectValuation(List<Field> fields)
    {
        Fields = fields;
    }
}