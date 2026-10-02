namespace ifm.Common.Variant;

/// <summary>
/// Provides extension methods for variant types.
/// </summary>
public static class VariantExtensions
{
    /// <summary>
    /// Returns the specified variant as a VariantObject.
    /// </summary>
    /// <param name="variant">The variant object to convert.</param>
    /// <returns>The converted object, if the conversion is successful; otherwise null.</returns>
    public static VariantObject AsVariantObject(this Variant variant)
    {
        return variant as VariantObject;
    }

    /// <summary>
    /// Returns the specified variant as a VariantArray.
    /// </summary>
    /// <param name="variant">The variant object to convert.</param>
    /// <returns>The converted object, if the conversion is successful; otherwise null.</returns>
    public static VariantArray AsVariantArray(this Variant variant)
    {
        return variant as VariantArray;
    }

    /// <summary>
    /// Returns the specified variant as a VariantValue.
    /// </summary>
    /// <param name="variant">The variant object to convert.</param>
    /// <returns>The converted object, if the conversion is successful; otherwise null.</returns>
    public static VariantValue AsVariantValue(this Variant variant)
    {
        return variant as VariantValue;
    }
}