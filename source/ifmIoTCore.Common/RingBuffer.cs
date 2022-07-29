namespace ifmIoTCore.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class RingBuffer<T> : IEnumerable<T>
    {
        public RingBuffer(int capacity)
        {
            _buffer = new T[capacity];
        }

        public void Add(T item)
        {
            _buffer[_pos] = item;
            _pos = (_pos + 1) % _buffer.Length;
            if (_size < _buffer.Length) _size++;
        }

        public void Clear()
        {
            _pos = 0;
            _size = 0;
        }

        public T GetFirst()
        {
            return _size < _buffer.Length ? _buffer[0] : _buffer[_pos];
        }

        public T GetLast()
        {
            return _buffer[(_pos + _buffer.Length - 1) % _buffer.Length];
        }

        public T GetAt(int pos)
        {
            return _size < _buffer.Length ? _buffer[pos] : _buffer[(_pos + pos) % _buffer.Length];
        }

        public bool IsEmpty => _size == 0;

        public int GetSize()
        {
            return _size;
        }

        //public T this[int pos]
        //{
        //    get { return _buffer[(_pos + pos) % _buffer.Length]; }
        //    set { _buffer[(_pos + pos) % _buffer.Length] = value; }
        //}

        public T[] ToArray()
        {
            var buf = new T[_size];
            for (var i = 0; i < _size; i++)
            {
                buf[i] = GetAt(i);
            }
            return buf;
        }

        private IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _size; i++)
            {
                yield return GetAt(i);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly T[] _buffer;
        private int _pos;
        private int _size;
    }

    public class ObservableRingBuffer<T> : RingBuffer<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableRingBuffer(int size) : base(size)
        {
        }

        public new void Add(T item)
        {
            base.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
    }
}
