using System.Collections;

namespace ifmIoTCore.Common.Variant
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VariantObject : Variant, IEquatable<VariantObject>, IEnumerable<KeyValuePair<Variant,Variant>>
    {
        private readonly Dictionary<Variant, Variant> _values = new Dictionary<Variant, Variant>();

        public int Count => _values.Count;

        public void Add(Variant key, Variant value)
        {
            _values.Add(key, value);
        }

        public void Add(string key, Variant value)
        {
            _values.Add((VariantValue)key, value);
        }

        public IDictionary<Variant, Variant> GetItems()
        {
            return _values;
        }

        public bool ContainsKey(Variant key)
        {
            return _values.ContainsKey(key);
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey((VariantValue)key);
        }

        public Variant FindFirstKey(IEnumerable<Variant> keys)
        {
            return keys.FirstOrDefault(ContainsKey);
        }

        public string FindFirstKey(IEnumerable<string> keys)
        {
            return keys?.FirstOrDefault(ContainsKey);
        }

        public Variant this[Variant key]
        {
            get => _values[key];
            set => _values[key] = value;
        }

        public Variant this[string key]
        {
            get => _values[(VariantValue)key];
            set => _values[(VariantValue)key] = value;
        }

        public bool TryGetValue(Variant key, out Variant value)
        {
            return _values.TryGetValue(key, out value);
        }

        public bool TryGetValue(string key, out Variant value)
        {
            return _values.TryGetValue((VariantValue)key, out value);
        }

        public override string ToString()
        {
            return string.Join(",", _values);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _values).GetEnumerator();
        }

        public IEnumerator<KeyValuePair<Variant, Variant>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((VariantObject)obj);
        }

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

        public override int GetHashCode()
        {
            return _values != null ? _values.GetHashCode() : 0;
        }
    }
}
