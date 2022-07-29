namespace ifmIoTCore.Common.Variant
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class VariantArray : Variant, IEquatable<VariantArray>, IEnumerable<Variant>
    {
        private readonly List<Variant> _values = new List<Variant>();

        public VariantArray()
        {
        }

        public void Reverse()
        {
            _values.Reverse();
        }

        public VariantArray(IEnumerable<Variant> items)
        {
            foreach (var item in items)
            {
                this._values.Add(item);
            }
        }

        public VariantArray(IEnumerable<bool> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<char> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<sbyte> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<byte> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<short> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<ushort> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<int> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<uint> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<long> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<ulong> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<float> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<double> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<decimal> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public VariantArray(IEnumerable<byte[]> items)
        {
            foreach (var item in items)
            {
                _values.Add(new VariantValue(item));
            }
        }

        public int Count => _values.Count;

        public void Add(Variant item)
        {
            _values.Add(item);
        }

        public IEnumerable<Variant> GetItems()
        {
            return _values;
        }

        public Variant this[int index]
        {
            get => _values[index];
            set => _values[index] = value;
        }

        public override string ToString()
        {
            return string.Join(",", _values);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _values).GetEnumerator();
        }

        public IEnumerator<Variant> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((VariantArray)obj);
        }

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

        public override int GetHashCode()
        {
            return _values != null ? _values.GetHashCode() : 0;
        }
    }
}
