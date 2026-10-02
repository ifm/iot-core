namespace ifm.Common;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides extension methods for collection types.
/// </summary>
public static class CollectionHelpers
{
    #region dictionary

    /// <summary>
    /// Removes all items except the ones in keys.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary.</param>
    /// <param name="keys">The keys not to remove.</param>
    public static void RemoveAllExceptByKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));
        if (keys == null)
        {
            dic.Clear();
            return;
        }

        var itemsToRemove = dic.Keys.Except(keys).ToList();
        foreach (var item in itemsToRemove)
        {
            dic.Remove(item);
        }
    }

    /// <summary>
    /// Removes all items from a dictionary with the specified keys.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary.</param>
    /// <param name="keys">The keys to remove.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RemoveAllByKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));
        if (keys == null) return;

        foreach (var key in keys)
        {
            dic.Remove(key);
        }
    }

    /// <summary>
    /// Removes all items from a dictionary where the keys match the specified predicate.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary.</param>
    /// <param name="predicate">The function to test each key for a condition.</param>
    public static void RemoveAllByKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, Predicate<TKey> predicate)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var keys = dic.Keys.Where(x => predicate(x)).ToList();
        foreach (var key in keys)
        {
            dic.Remove(key);
        }
    }

    /// <summary>
    /// Removes all items from a dictionary where the values match the specified predicate.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary.</param>
    /// <param name="predicate">The function to test each value for a condition.</param>
    public static void RemoveAllByValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, Predicate<TValue> predicate)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var itemsToRemove = dic.Keys.Where(x => predicate(dic[x])).ToList();
        foreach (var item in itemsToRemove)
        {
            dic.Remove(item);
        }
    }

    /// <summary>
    /// Clones a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dic">The dictionary to clone.</param>
    /// <returns>The cloned dictionary.</returns>
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dic)
    {
        if (dic == null) throw new ArgumentNullException(nameof(dic));

        return new Dictionary<TKey, TValue>(dic);
    }

    #endregion

    #region list

    /// <summary>
    /// Removes all items from a list except the ones in items.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="items">The items not to remove.</param>
    public static void RemoveAllExcept<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (items == null)
        {
            list.Clear();
            return;
        }

        var itemsToRemove = list.Except(items).ToList();
        foreach (var item in itemsToRemove)
        {
            list.Remove(item);
        }
    }

    /// <summary>
    /// Removes a collection of items from a list.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="items">The items to remove.</param>
    public static void RemoveAll<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (items == null) return;

        foreach (var item in items)
        {
            list.Remove(item);
        }
    }

    /// <summary>
    /// Removes all items from a list which match the specified predicate.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="predicate">The function to test each item for a condition.</param>
    public static void RemoveAll<T>(this IList<T> list, Predicate<T> predicate)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        var itemsToRemove = list.Where(x => predicate(x)).ToList();
        foreach (var item in itemsToRemove)
        {
            list.Remove(item);
        }
    }

    /// <summary>
    /// Adds an item to a list only if the item is not null.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="item">The item to add to the end of the list.</param>
    public static void AddIfNotNull<T>(this IList<T> list, T item)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));

        if (item != null)
        {
            list.Add(item);
        }
    }

    /// <summary>
    /// Checks if a list has duplicate items by using the default equality comparer to compare values.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    /// <param name="list">The list to check.</param>
    /// <returns>true, if the list has duplicates; otherwise false.</returns>
    public static bool HasDuplicates<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));

        return list.Count != list.Distinct().Count();
    }

    /// <summary>
    /// Clones a list.
    /// </summary>
    /// <typeparam name="T">Type of the items in the list.</typeparam>
    /// <param name="list">The list to clone.</param>
    /// <returns>The cloned list.</returns>
    public static List<T> Clone<T>(this IList<T> list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));

        return [..list];
    }

    #endregion
}