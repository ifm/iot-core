namespace ifm.Common.UnitTests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class RingBufferTests
{
    [Test]
    public void CreateRingBuffer_InvalidParameter_Throws()
    {
        // ReSharper disable once ObjectCreationAsStatement
        Assert.Throws<OverflowException>(() => new RingBuffer<int>(-1));
    }

    [Test]
    public void CreateRingBuffer_Success()
    {
        var buffer = new RingBuffer<int>(100);

        Assert.That(buffer.Count == 0);
        Assert.That(buffer.Capacity == 100);
    }

    [Test]
    public void AddItem_Success()
    {
        var buffer = new RingBuffer<int>(1) { 10 };

        Assert.That(buffer.Count == 1);

        var first = buffer.Peek();
        Assert.That(first == 10);

        buffer.Clear();

        Assert.That(buffer.Count == 0);
    }

    [Test]
    public void AddItems1_Success()
    {
        var buffer = new RingBuffer<int>(5) { 10, 20, 30, 40, 50 };

        var item = buffer.Peek();
        Assert.That(item == 10);

        item = buffer.Take();
        Assert.That(item == 10);
        item = buffer.Take();
        Assert.That(item == 20);
        item = buffer.Take();
        Assert.That(item == 30);
        item = buffer.Take();
        Assert.That(item == 40);
        item = buffer.Take();
        Assert.That(item == 50);

        buffer.Add(60);
        item = buffer.Peek();
        Assert.That(item == 60);
        item = buffer.Take();
        Assert.That(item == 60);

        Assert.That(buffer.Count == 0);
    }

    [Test]
    public void ToArray_Success()
    {
        var buffer = new RingBuffer<int>(10) { 10, 20, 30, 40, 50 };

        var array = buffer.ToArray();
        Assert.That(array.SequenceEqual(buffer));
    }

    [Test]
    public void Enumerate_Success()
    {
        var buffer = new RingBuffer<int>(10) { 10, 20, 30, 40, 50 };

        var e1 = ((IEnumerable<int>)buffer).GetEnumerator();
        e1.MoveNext();
        Assert.That(10 == e1.Current);
        e1.MoveNext();
        Assert.That(20 == e1.Current);
        e1.MoveNext();
        Assert.That(30 == e1.Current);
        e1.MoveNext();
        Assert.That(40 == e1.Current);
        e1.MoveNext();
        Assert.That(50 == e1.Current);

        e1.Dispose();

        var e2 = ((IEnumerable)buffer).GetEnumerator();
        e2.MoveNext();
        Assert.That(e2.Current != null && 10 == (int)e2.Current);
        e2.MoveNext();
        Assert.That(e2.Current != null && 20 == (int)e2.Current);
        e2.MoveNext();
        Assert.That(e2.Current != null && 30 == (int)e2.Current);
        e2.MoveNext();
        Assert.That(e2.Current != null && 40 == (int)e2.Current);
        e2.MoveNext();
        Assert.That(e2.Current != null && 50 == (int)e2.Current);
    }
}