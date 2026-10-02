namespace ifm.IoTCore.Common;

using System;
using System.Text.RegularExpressions;
using ifm.Common;

/// <summary>
/// Provides methods to create or process element addresses.
/// </summary>
public static class AddressParser
{
    /// <summary>
    /// The address separator character.
    /// </summary>
    public const char AddressSeparator = '/';

    /// <summary>
    /// The address separator character as string.
    /// </summary>
    public const string AddressSeparatorString = "/";

    /// <summary>
    /// Creates a new address from the parent address and the identifier.
    /// </summary>
    /// <param name="parentAddress">The parent address.</param>
    /// <param name="identifier">The identifier.</param>
    /// <returns>The new address string.</returns>
    public static string CreateAddress(string parentAddress, string identifier)
    {
        return string.IsNullOrEmpty(parentAddress) ? identifier : $"{parentAddress}{AddressSeparator}{identifier}";
    }

    /// <summary>
    /// Creates a new address from the parent address and combination of the given identifiers.
    /// </summary>
    /// <param name="parentAddress">The parent address.</param>
    /// <param name="identifiers">The identifiers to combine.</param>
    /// <returns>The new address string.</returns>
    public static string CreateAddress(string parentAddress, params string[] identifiers)
    {
        return string.IsNullOrEmpty(parentAddress) ?
            string.Join(AddressSeparatorString, identifiers) :
            $"{parentAddress}{AddressSeparator}{string.Join(AddressSeparatorString, identifiers)}";
    }

    /// <summary>
    /// Splits the address into its identifier parts.
    /// </summary>
    /// <param name="address">The address to split.</param>
    /// <param name="options">The split options.</param>
    /// <returns>The array of identifiers.</returns>
    public static string[] SplitAddress(string address, StringSplitOptions options = StringSplitOptions.None)
    {
        return address.Split([AddressSeparator], options);
    }

    /// <summary>
    /// Gets the root identifier from the address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>The root identifier.</returns>
    public static string GetRootIdentifier(string address)
    {
        return address?.GetFirstToken(AddressSeparator);
    }

    /// <summary>
    /// Gets the path from the address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>The path.</returns>
    public static string RemoveRootIdentifier(string address)
    {
        return address.RemoveFirstToken(AddressSeparator, true);
    }

    /// <summary>
    /// Patches the given address with the root identifier.
    /// </summary>
    /// <param name="rootIdentifier">The root identifier.</param>
    /// <param name="address">The address.</param>
    /// <returns>The patched address.</returns>
    public static string PatchAddress(string rootIdentifier, string address)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(rootIdentifier, nameof(rootIdentifier));

        if (!string.IsNullOrEmpty(address))
        {
            address = address.ReplaceAll(["%5B", "%5b"], "[").ReplaceAll(["%5D", "%5d"], "]");
        }
        if (string.IsNullOrEmpty(GetRootIdentifier(address)))
        {
            return rootIdentifier + address;
        }
        return address;
    }

    /// <summary>
    /// Replaces invalid characters in an identifier with the given character.
    /// </summary>
    /// <param name="identifier">The identifier to search.</param>
    /// <param name="replacement">The replacement string.</param>
    /// <returns>A new string with the replacements.</returns>
    public static string ReplaceInvalidCharacters(string identifier, string replacement = "_")
    {
        return Regex.Replace(identifier, @"[^0-9a-zA-Z_\-\]\[]", replacement, RegexOptions.None);
    }

    /// <summary>
    /// Gets the last identifier from the address.
    /// </summary>
    /// <param name="address">The address to search.</param>
    /// <returns>The last identifier from the address.</returns>
    public static string GetLastIdentifier(string address)
    {
        return address.GetLastToken(AddressSeparator);
    }

    /// <summary>
    /// Gets the parent address from the address.
    /// </summary>
    /// <param name="address">The address to search.</param>
    /// <returns>The path from the address.</returns>
    public static string GetParentAddress(string address)
    {
        return address.RemoveLastToken(AddressSeparator);
    }
}