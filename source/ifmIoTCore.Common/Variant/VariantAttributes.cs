namespace ifmIoTCore.Common.Variant
{
    using System;

    [AttributeUsage(AttributeTargets.Constructor)]
    public class VariantConstructorAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class VariantPropertyAttribute : Attribute
    {
        /// <summary>
        /// The property name
        /// </summary>
        public string Name { get; }

        public string[] AlternativeNames { get; set; }

        /// <summary>
        /// If true, the property is required in the variant when creating the object
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// If true, the property in the object is ignored when creating the variant
        /// </summary>
        public bool Ignored { get; set; }

        /// <summary>
        /// If true, the property in the object is ignored if it is null when creating the variant
        /// </summary>
        public bool IgnoredIfNull { get; set; }

        public VariantPropertyAttribute(string name)
        {
            Name = name;
            AlternativeNames = null;
            Required = false;
            Ignored = false;
            IgnoredIfNull = false;
        }
    }
}
