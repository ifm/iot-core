namespace ifm.Common;

using System.Threading;


/// <summary>
/// Represents a blocking ring buffer.
/// </summary>
/// <typeparam name="T">The type of the items in the buffer.</typeparam>
public class BlockingRingBuffer<T>
{
    private readonly RingBuffer<T> _buffer;
    private readonly SemaphoreSlim _semaphore = new(0);

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="capacity">The capacity of the buffer.</param>
    public BlockingRingBuffer(int capacity)
    {
        _buffer = new RingBuffer<T>(capacity);
    }

    /// <summary>
    /// Adds a new item to the buffer.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(T item)
    {
        _buffer.Add(item);
        if (_semaphore.CurrentCount < _buffer.Count) _semaphore.Release();
    }

    /// <summary>
    /// Take an item from the buffer.
    /// </summary>
    /// <returns>The item.</returns>
    public T Take()
    {
        _semaphore.Wait();
        return _buffer.Take();
    }

    /// <summary>
    /// Take an item from the buffer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The item.</returns>
    public T Take(CancellationToken cancellationToken)
    {
        _semaphore.Wait(cancellationToken);
        return _buffer.Take();
    }

    /// <summary>
    /// Try to take an item from the buffer.
    /// </summary>
    /// <param name="item">The item taken.</param>
    /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public bool TryTake(out T item, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        item = default;
        if (!_semaphore.Wait(millisecondsTimeout, cancellationToken)) return false;
        item = _buffer.Take();
        return true;
    }
}