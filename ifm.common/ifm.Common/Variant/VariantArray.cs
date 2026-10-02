namespace ifm.Common.Variant;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides a variant class for array data types.
/// </summary>
public class VariantArray : Variant, IEnumerable<Variant>, IEquatable<VariantArray>
{
    private readonly List<Variant> _values = [];

    /// <summary>
    /// Initializes a new class instance that is empty.
    /// </summary>
    public VariantArray()
    {
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<Variant> items)
    {
        foreach (var item in items)
        {
            _values.Add(item);
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<bool> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<char> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<sbyte> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<byte> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<short> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<ushort> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<int> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<uint> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<long> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<ulong> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<float> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<double> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<decimal> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<DateTime> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<TimeSpan> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<Uri> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Initializes a new instance of the VariantArray that contains items copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose items are copied to the new instance.</param>
    public VariantArray(IEnumerable<Guid> items)
    {
        foreach (var item in items)
        {
            _values.Add(new VariantValue(item));
        }
    }

    /// <summary>
    /// Gets the number of items contained in the VariantArray.
    /// </summary>
    public int Count => _values.Count;

    /// <summary>
    /// Adds an item to the end of the VariantArray.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Add(Variant item)
    {
        _values.Add(item);
    }

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to get or set.</param>
    /// <returns>The item at the specified index.</returns>
    public Variant this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    /// <summary>
    /// Gets a string representation of the items in the VariantArray.
    /// </summary>
    /// <returns>The string representation of the items.</returns>
    public override string ToString()
    {
        return string.Join(",", _values);
    }

    /// <summary>
    /// Reverses the order of the items in the VariantArray.
    /// </summary>
    public void Reverse()
    {
        _values.Reverse();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the VariantArray.
    /// </summary>
    /// <returns>The enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _values).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the VariantArray.
    /// </summary>
    /// <returns>The enumerator.</returns>
    public IEnumerator<Variant> GetEnumerator()
    {
        return _values.GetEnumerator();
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
        return Equals((VariantArray)obj);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(VariantArray other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_values.Count != other._values.Count) return false;
        for (var i = 0; i < _values.Count; i++)
        {
            if (!_values[i].Equals(other._values[i]))
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
        return _values.Aggregate(0, (current, item) => NumericHelpers.CombineHashCodes(current, item.GetHashCode()));
    }
}