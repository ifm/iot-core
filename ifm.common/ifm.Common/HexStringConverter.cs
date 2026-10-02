namespace ifm.Common;

using System;
using System.Text;

/// <summary>
/// Provides methods to convert from and to a hexadecimal encoded string.
/// A hexadecimal encoded string is a string in which every two characters represent a hexadecimal number.
/// </summary>
public static class HexStringConverter
{
    /// <summary>
    /// Converts a byte array to a hexadecimal encoded string.
    /// Every byte in bytes is converted to two characters which represent a hexadecimal number with its equivalent value.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The converted string.</returns>
    public static string ByteArrayToHexString(byte[] bytes)
    {
        if (bytes == null) throw new ArgumentNullException(nameof(bytes));

        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            sb.AppendFormat("{0:X2}", b);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Converts a hexadecimal encoded string to a byte array.
    /// Every two characters in str which represent a hexadecimal number are converted to its equivalent byte.
    /// If the number of characters in str is odd, the last character is ignored.
    /// If str contains a character that is not a valid hexadecimal digit the methods throws an exception.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The converted byte array.</returns>
    public static byte[] HexStringToByteArray(string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var bytes = new byte[str.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
        }
        return bytes;
    }


    /// <summary>
    /// Converts a string to a hexadecimal encoded string.
    /// Every character in str is converted to two characters which represent a hexadecimal number with its equivalent ASCII code.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static string StringToHexString(string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var bytes = Encoding.Default.GetBytes(str);
        return ByteArrayToHexString(bytes);
    }

    /// <summary>
    /// Converts a hexadecimal encoded string to a string.
    /// Every two characters in str which represent the ASCII code of a character as a hexadecimal number are converted to an equivalent character.
    /// If the number of characters in str is odd, the last character is ignored.
    /// If str contains a character that is not a valid hexadecimal digit the methods throws an exception.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>The converted string.</returns>
    public static string HexStringToString(string str)
    {
        if (str == null) throw new ArgumentNullException(nameof(str));

        var bytes = HexStringToByteArray(str);
        return Encoding.Default.GetString(bytes);
    }
}