namespace ifm.Common.Variant;

using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// Represents a variant constructor attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
public class VariantConstructorAttribute : Attribute
{

}

/// <summary>
/// Represents a variant property, field, or parameter attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class VariantPropertyAttribute : Attribute
{
    /// <summary>
    /// The property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A collection of alternative property names.
    /// </summary>
    public string[] AlternativeNames { get; set; }

    /// <summary>
    /// If true, the property is required in the variant when creating the object.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// If true, the property in the object is ignored when creating the variant.
    /// </summary>
    public bool Ignored { get; set; }

    /// <summary>
    /// If true, the property in the object is ignored if it is null when creating the variant.
    /// </summary>
    public bool IgnoredIfNull { get; set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="name">The property name.</param>
    public VariantPropertyAttribute(string name)
    {
        Name = name;
        AlternativeNames = null;
        Required = false;
        Ignored = false;
        IgnoredIfNull = false;
    }

    /// <summary>
    /// Gets the attribute from the provided property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The attribute, if it exists; otherwise null.</returns>
    public static VariantPropertyAttribute Get(ICustomAttributeProvider property)
    {
        var attributes = property.GetCustomAttributes(false);
        return attributes.Where(attribute => attribute.GetType() == typeof(VariantPropertyAttribute)).Cast<VariantPropertyAttribute>().FirstOrDefault();
    }
}

/// <summary>
/// Represents a variant contract resolver attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class VariantContractResolverAttribute : Attribute
{
    /// <summary>
    /// Gets the resolver type.
    /// </summary>
    public Type ResolverType { get; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="resolverType">The type of the resolver.</param>
    public VariantContractResolverAttribute(Type resolverType)
    {
        ResolverType = resolverType;
    }
}