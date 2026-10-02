namespace ifm.Common.Variant;

/// <summary>
/// Provides the base interface for a variant contract resolver.
/// </summary>
public interface IVariantContractResolver
{
    /// <summary>
    /// Creates a new class instance from the provided variant.
    /// </summary>
    /// <param name="data">The provided data to create a new class instance.</param>
    /// <returns>The created class instance or null.</returns>
    object CreateInstance(Variant data);
}