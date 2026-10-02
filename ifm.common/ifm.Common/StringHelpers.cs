namespace ifm.Common;

using System;
using System.Text;

/// <summary>
/// Provides string helpers.
/// </summary>
public static class StringHelpers
{
    /// <summary>
    /// Determines if the string instance ends with the specified character.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns>true, if the characters match; otherwise false.</returns>
    public static bool EndsWith(this string str, char value)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(nameof(str));

        return str[str.Length - 1] == value;
    }

    /// <summary>
    /// Determines if the string instance starts with the specified character.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="value">The character to compare.</param>
    /// <returns>true, if the characters match; otherwise false.</returns>
    public static bool StartsWith(this string str, char value)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(nameof(str));

        return str[0] == value;
    }

    /// <summary>
    /// Gets the part of the string left of the first occurrence of the separator character.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator character.</param>
    /// <param name="startIndex">The search start position.</param>
    /// <param name="includeSeparator">If true, the separator is included; otherwise not.</param>
    /// <returns>If the separator character exists, the part of the string left of the separator character; otherwise null.</returns>
    public static string Left(this string str, char separator, int startIndex = 0, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.IndexOf(separator, startIndex);
        if (pos == -1) return null;
        if (includeSeparator) pos++;
        return str.Substring(0, pos);
    }

    /// <summary>
    /// Gets the part of the string left of the first occurrence of the separator string.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator string.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="includeSeparator">If true, the separator string is included; otherwise not.</param>
    /// <returns>If the separator string exists, the part of the string left of the separator character; otherwise null.</returns>
    public static string Left(this string str, string separator, int startIndex = 0, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.IndexOf(separator, startIndex, StringComparison.OrdinalIgnoreCase);
        if (pos == -1) return null;
        if (includeSeparator) pos += separator.Length;
        return str.Substring(0, pos);
    }

    /// <summary>
    /// Gets the part of the string right of the first occurrence of the separator character.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator character.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="includeSeparator">If true, the separator string is included; otherwise not.</param>
    /// <returns>If the separator character exists, the part of the string right of the separator character; otherwise null.</returns>
    public static string Right(this string str, char separator, int startIndex = 0, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.IndexOf(separator, startIndex);
        if (pos == -1) return null;
        if (!includeSeparator) pos++;
        return str.Substring(pos);
    }

    /// <summary>
    /// Gets the part of the string right of the first occurrence of the separator string.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator string.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="includeSeparator">If true, the separator string is included; otherwise not.</param>
    /// <returns>If the separator string exists, the part of the string right of the separator character; otherwise null.</returns>
    public static string Right(this string str, string separator, int startIndex = 0, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.IndexOf(separator, startIndex, StringComparison.OrdinalIgnoreCase);
        if (pos == -1) return null;
        if (!includeSeparator) pos += separator.Length;
        return str.Substring(pos);
    }

    /// <summary>
    /// Gets the first token from a character separated string.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator character.</param>
    /// <param name="includeSeparator">If true, the separator is included in the result; otherwise not.</param>
    /// <returns>If the separator character exists, the part of the string left of the first separator character; otherwise the full string.</returns>
    public static string GetFirstToken(this string str, char separator, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.IndexOf(separator);
        if (pos == -1) return str;
        if (includeSeparator) pos++;
        return str.Substring(0, pos);
    }

    /// <summary>
    /// Gets the last token from a character separated string.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator character.</param>
    /// <param name="includeSeparator">If true, the separator is included in the result; otherwise not.</param>
    /// <returns>If the separator character exists, the part of the string right of the last separator character; otherwise the full string.</returns>
    public static string GetLastToken(this string str, char separator, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.LastIndexOf(separator);
        if (pos == -1) return str;
        if (!includeSeparator) pos++;
        return str.Substring(pos);
    }

    /// <summary>
    /// Removes the first token from a character separated string.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator character.</param>
    /// <param name="includeSeparator">If true, the separator is included in the result; otherwise not.</param>
    /// <returns>If the separator character exists, the part of the string left of the first separator character; otherwise an empty string.</returns>
    public static string RemoveFirstToken(this string str, char separator, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.IndexOf(separator);
        if (pos == -1) return string.Empty;
        if (!includeSeparator) pos++;
        return str.Substring(pos);
    }

    /// <summary>
    /// Removes the last token from a character separated string.
    /// </summary>
    /// <param name="str">The character separated string.</param>
    /// <param name="separator">The separator character.</param>
    /// <param name="includeSeparator">If true, the separator is included in the result; otherwise not.</param>
    /// <returns>If the separator character exists, the part of the string left of the last separator character; otherwise an empty string.</returns>
    public static string RemoveLastToken(this string str, char separator, bool includeSeparator = false)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var pos = str.LastIndexOf(separator);
        if (pos == -1) return string.Empty;
        if (includeSeparator) pos++;
        return str.Substring(0, pos);
    }

    /// <summary>
    /// Replaces all occurrences of old values with new value.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="oldValues">The characters to be replaced.</param>
    /// <param name="newValue">The string to replace the oldChars.</param>
    /// <returns></returns>
    public static string ReplaceAll(this string str, char[] oldValues, string newValue)
    {
        if (string.IsNullOrEmpty(str)) return str;
        var temp = str.Split(oldValues, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(newValue, temp);
    }

    /// <summary>
    /// Replaces all occurrences of old values with new value.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="oldValues">The values to be replaced.</param>
    /// <param name="newValue">The value to replace.</param>
    /// <returns></returns>
    public static string ReplaceAll(this string str, string[] oldValues, string newValue)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentNullException(nameof(str));

        var temp = str.Split(oldValues, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(newValue, temp);
    }


    /// <summary>
    /// Gets the UTF8 encoded string from the bytes.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns>The decoded string.</returns>
    public static string GetUtf8String(this byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Gets the bytes from the UTF8 encoded string.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] GetUtf8Bytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
}