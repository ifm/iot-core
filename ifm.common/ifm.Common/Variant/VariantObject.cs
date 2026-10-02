namespace ifm.Common.Variant;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Provides a variant class for complex data types.
/// </summary>
public class VariantObject : Variant, IEnumerable<KeyValuePair<Variant,Variant>>, IEquatable<VariantObject>
{
    private class VariantEqualityComparer : IEqualityComparer<Variant>
    {
        public bool Equals(Variant v1, Variant v2)
        {
            if (ReferenceEquals(v1, v2)) return true;
            if (v1 is null || v2 is null) return false;
            return v1.Equals(v2);
        }

        public int GetHashCode(Variant obj)
        {
            return 0;
        }
    }
    private readonly Dictionary<Variant, Variant> _values = new(new VariantEqualityComparer());

    /// <summary>
    /// Gets the number of items contained in the VariantObject.
    /// </summary>
    public int Count => _values.Count;

    /// <summary>
    /// Adds an item with the specified key and value to the VariantObject.
    /// </summary>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    public void Add(Variant key, Variant value)
    {
        _values.Add(key, value);
    }

    /// <summary>
    /// Adds an item with the specified key and value to the VariantObject.
    /// </summary>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    public void Add(string key, Variant value)
    {
        _values.Add((VariantValue)key, value);
    }

    /// <summary>
    /// Removes the item with the specified key from the VariantObject.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(Variant key)
    {
        return _values.Remove(key);
    }

    /// <summary>
    /// Removes the item with the specified key from the VariantObject.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(string key)
    {
        return _values.Remove((VariantValue)key);
    }

    /// <summary>
    /// Determines whether the VariantObject contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the VariantObject.</param>
    /// <returns>true if the VariantObject contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(Variant key)
    {
        return _values.ContainsKey(key);
    }

    /// <summary>
    /// Determines whether the VariantObject contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the VariantObject.</param>
    /// <returns>true if the VariantObject contains an element with the specified key; otherwise, false.</returns>
    public bool ContainsKey(string key)
    {
        return _values.ContainsKey((VariantValue)key);
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    public Variant this[Variant key]
    {
        get => _values[key];
        set => _values[key] = value;
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    public Variant this[string key]
    {
        get => _values[(VariantValue)key];
        set => _values[(VariantValue)key] = value;
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">The value associated with the specified key, if the key is found; otherwise null.</param>
    /// <returns>true if the VariantObject contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(Variant key, out Variant value)
    {
        return _values.TryGetValue(key, out value);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">The value associated with the specified key, if the key is found; otherwise null.</param>
    /// <returns>true if the VariantObject contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(string key, out Variant value)
    {
        return _values.TryGetValue((VariantValue)key, out value);
    }

    /// <summary>
    /// Gets the first value associated with one of the specified keys.
    /// </summary>
    /// <param name="keys">The keys of the value to get.</param>
    /// <param name="value">The first value associated with one of the specified key, if a key is found; otherwise null.</param>
    /// <returns>true if the VariantObject contains an element with one of the specified keys; otherwise, false.</returns>
    public bool TryGetValue(IEnumerable<Variant> keys, out Variant value)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));

        value = null;
        foreach (var key in keys)
        {
            if (_values.Keys.Contains(key))
            {
                value = _values[key];
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the first value associated with one of the specified keys.
    /// </summary>
    /// <param name="keys">The keys of the value to get.</param>
    /// <param name="value">The first value associated with one of the specified key, if a key is found; otherwise null.</param>
    /// <returns>true if the VariantObject contains an element with one of the specified keys; otherwise, false.</returns>
    public bool TryGetValue(IEnumerable<string> keys, out Variant value)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));

        value = null;
        foreach (var item in keys)
        {
            var key = (VariantValue)item;
            if (_values.Keys.Contains(key))
            {
                value = _values[key];
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the VariantArray.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_values).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the VariantArray.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<KeyValuePair<Variant, Variant>> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    /// <summary>
    /// Gets a string representation of the items in the VariantObject.
    /// </summary>
    /// <returns>The string representation of the items.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("{ ");

        var cnt = 0;
        foreach (var value in _values)
        {
            sb.Append($"{value.Key}: {value.Value}");
            if (++cnt < _values.Count)
            {
                sb.Append(", ");
            }
        }
        sb.Append(" }");
        return sb.ToString();
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="obj">The object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((VariantObject)obj);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(VariantObject other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_values.Count != other._values.Count) return false;
        foreach (var value in _values)
        {
            if (other._values.TryGetValue(value.Key, out var otherValue))
            {
                if (value.Value != null)
                {
                    if (!value.Value.Equals(otherValue))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets a hash code for the current object.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return _values.Aggregate(0, (current, item) =>
            NumericHelpers.CombineHashCodes(current, NumericHelpers.CombineHashCodes(item.Key.GetHashCode(), item.Value.GetHashCode())));
    }
}