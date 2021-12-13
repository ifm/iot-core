namespace ifmIoTCore.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Exposes static extension methods</summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if the string ends with the provided character
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="value">The value to check</param>
        /// <returns></returns>
        public static bool EndsWith(this string str, char value)
        {
            return str[str.Length - 1] == value;
        }

        /// <summary>
        /// Checks if the string starts with the provided character
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="value">The value to check</param>
        /// <returns></returns>
        public static bool StartsWith(this string str, char value)
        {
            return str[0] == value;
        }

        /// <summary>Gets the first token from a character separated string</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <param name="startIndex">The search starting position</param>
        /// <returns>A string that contains the first token from the original string</returns>
        public static string GetFirstToken(this string str, char separator, int startIndex = 0)
        {
            var pos = str.IndexOf(separator, startIndex);
            return pos == -1 ? str : str.Substring(startIndex, pos);
        }

        /// <summary>Gets the last token from a character separated string</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <param name="startIndex">The search starting position</param>
        /// <returns>A string that contains the last token from the original string</returns>
        public static string GetLastToken(this string str, char separator, int startIndex = 0)
        {
            var pos = str.LastIndexOf(separator, startIndex);
            return pos == -1 ? str : str.Substring(pos);
        }

        /// <summary>Removes the first token from a character separated string</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <returns>A string that contains the original string without the first token</returns>
        public static string RemoveFirstToken(this string str, char separator)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var tokens = str.Split(separator);
            if (tokens.Length > 0 && tokens[0] != string.Empty)
            {
                tokens[0] = string.Empty;
            }
            str = string.Join("/", tokens);
            return str;
        }

        /// <summary>Replace the first token from a character separated string</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <param name="value">The new value</param>
        /// <returns>A string that contains the original string with replaced the first token</returns>
        public static string ReplaceFirstToken(this string str, char separator, string value)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var tokens = str.Split(separator);
            if (tokens.Length > 0 && tokens[0] != string.Empty)
            {
                tokens[0] = value;
            }
            str = string.Join("/", tokens);
            return str;
        }

        /// <summary>Gets the part of the string left of the first occurrence of the separator character</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <returns>If the separator character exists, the part of the string left of the separator character; otherwise null</returns>
        public static string Left(this string str, char separator)
        {
            var pos = str.IndexOf(separator);
            if (pos == -1) return null;
            return str.Substring(0, pos);
        }

        /// <summary>Gets the part of the string left of the first occurrence of the separator character</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <returns>If the separator character exists, the part of the string left of the separator character; otherwise null</returns>
        public static string LeftIncludeSeparator(this string str, char separator)
        {
            var pos = str.IndexOf(separator);
            if (pos == -1) return null;
            return str.Substring(0, pos + 1);
        }

        /// <summary>Gets the part of the string right of the first occurence of the separator character</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <returns>If the separator character exists, the part of the string right of the separator character; otherwise null</returns>
        public static string Right(this string str, char separator)
        {
            var pos = str.IndexOf(separator);
            if (pos == -1) return null;
            if (pos == str.Length) return string.Empty;
            return str.Substring(pos + 1);
        }

        /// <summary>Gets the part of the string right of the first occurrence of the separator character</summary>
        /// <param name="str">The character separated string</param>
        /// <param name="separator">The separator character</param>
        /// <returns>If the separator character exists, the part of the string right of the separator character; otherwise null</returns>
        public static string RightIncludeSeparator(this string str, char separator)
        {
            var pos = str.IndexOf(separator);
            if (pos == -1) return null;
            return str.Substring(pos);
        }

        /// <summary>Compares two values with precision</summary>
        /// <param name="thisValue">The value to compare</param>
        /// <param name="otherValue">The value to compare against</param>
        /// <param name="precision">The precision of the comparison. If the difference between the values is less than the precision, the values are considered equal</param>
        /// <returns>true if the difference between the values is within the precision; otherwise false</returns>
        public static bool EqualsWithPrecision(this float thisValue, float otherValue, float precision = 0.001f)
        {
            return Math.Abs(thisValue - otherValue) < precision;
        }

        /// <summary>Compares two values with precision</summary>
        /// <param name="thisValue">The value to compare</param>
        /// <param name="otherValue">The value to compare against</param>
        /// <param name="precision">The precision of the comparison. If the difference between the values is less than the precision, the values are considered equal/// </param>
        /// <returns>true if the difference between the values is within the precision; otherwise false</returns>
        public static bool EqualsWithPrecision(this double thisValue, double otherValue, double precision = 0.001)
        {
            return Math.Abs(thisValue - otherValue) < precision;
        }

        /// <summary>Removes multiple items from a dictionary that match the predicate</summary>
        /// <typeparam name="TKey">The type of keys in the dictionary</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary</typeparam>
        /// <param name="dic">The dictionary</param>
        /// <param name="predicate">The predicate for the items to remove</param>
        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<TValue, bool> predicate)
        {
            var keys = dic.Keys.Where(x => predicate(dic[x])).ToList();
            foreach (var key in keys)
            {
                dic.Remove(key);
            }
        }

        /// <summary>Adds an item to a list if it is not null</summary>
        /// <typeparam name="T">Type parameter</typeparam>
        /// <param name="list">List item is added to</param>
        /// <param name="item">The item</param>
        public static void AddIfNotNull<T>(this List<T> list, T item)
        {
            if (item != null)
            {
                list.Add(item);
            }
        }

        public static bool Remove<T>(this List<T> list, Predicate<T> predicate)
        {
            var item = list.Find(predicate);
            return item != null && list.Remove(item);
        }
    }
}
