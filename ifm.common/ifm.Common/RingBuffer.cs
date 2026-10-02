namespace ifm.Common;

using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Represents a queue ring buffer.
/// Initializes a new instance of the class.
/// </summary>
/// <typeparam name="T">The type of the items in the buffer.</typeparam>
/// <param name="capacity">The capacity of the buffer.</param>
public class RingBuffer<T>(int capacity) : IEnumerable<T>
{
    private readonly object _lock = new();
    private readonly T[] _buffer = new T[capacity];
    private int _addPos;
    private int _takePos;
    private int _count;

    /// <summary>
    /// Adds a new item to the buffer.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        lock (_lock)
        {
            _buffer[_addPos] = item;
            _addPos = (_addPos + 1) % Capacity;
            if (_count == Capacity)
            {
                _takePos = _addPos;
            }
            else
            {
                _count++;
            }
        }
    }

    /// <summary>
    /// Take an item from the buffer.
    /// </summary>
    /// <returns>The item.</returns>
    public T Take()
    {
        lock (_lock)
        {
            if (_count == 0) throw new InvalidOperationException("Buffer is empty");

            var item = _buffer[_takePos];
            _takePos = (_takePos + 1) % Capacity;
            _count--;
            return item;
        }
    }

    /// <summary>
    /// Get the first item from the buffer.
    /// </summary>
    /// <returns>The item.</returns>
    public T Peek()
    {
        lock (_lock)
        {
            return _count != 0 ? _buffer[_takePos] : throw new Exception("Buffer is empty");
        }
    }

    /// <summary>
    /// Removes all items from the buffer.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _takePos = 0;
        _addPos = 0;
        _count = 0;
    }

    /// <summary>
    /// Gets the number of items in the buffer.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _count;
            }
        }
    }

    /// <summary>
    /// Gets the capacity of the buffer.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Returns an enumerator that iterates through the buffer.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the buffer.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
        {
            for (var i = 0; i < _count; i++)
            {
                yield return _buffer[(_takePos + i) % Capacity];
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the buffer.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the buffer.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
