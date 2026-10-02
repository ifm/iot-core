namespace ifm.Common.UnitTests.Variant;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Variant;
using NUnit.Framework;
using ifm.Common;


[TestFixture]
public class VariantArrayTests
{
    [Test]
    public void ConvertVariantArray_Success()
    {
        var originalValues = new[] { new VariantValue(12), new VariantValue(2.2f), new VariantValue("huhuhu") };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 3);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((VariantValue)item.Variant).Equals(item.OriginalValue));
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 3);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((VariantValue)item.Variant).Equals(item.OriginalValue));
        }

        var values = Variant.ToObject<VariantValue[]>(v1);
        Assert.That(values.Length == 3);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value.Equals(item.OriginalValue));
        }
    }

    [Test]
    public void ConvertBooleanArray_Success()
    {
        var originalValues = new[] { true, false, true, false, true };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((bool)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((bool)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<bool[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertCharacterArray_Success()
    {
        var originalValues = new[] { 'A', 'B', 'C', 'D', 'E' };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((char)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((char)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<char[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertInt8Array_Success()
    {
        var originalValues = new sbyte[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((sbyte)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((sbyte)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<sbyte[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertUInt8Array_Success()
    {
        var originalValues = new byte[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((byte)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((byte)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<byte[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertInt16Array_Success()
    {
        var originalValues = new short[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((short)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((short)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<short[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertUInt16Array_Success()
    {
        var originalValues = new ushort[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((ushort)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((ushort)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<ushort[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertInt32Array_Success()
    {
        var originalValues = new[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((int)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((int)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<int[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertUInt32Array_Success()
    {
        var originalValues = new uint[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((uint)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((uint)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<uint[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertInt64Array_Success()
    {
        var originalValues = new long[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((long)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((long)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<long[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertUInt64Array_Success()
    {
        var originalValues = new ulong[] { 1, 2, 3, 4, 5 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((ulong)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((ulong)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<ulong[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertFloatArray_Success()
    {
        var originalValues = new[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((float)(VariantValue)item.Variant).EqualsWithPrecision(item.OriginalValue));
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((float)(VariantValue)item.Variant).EqualsWithPrecision(item.OriginalValue));
        }

        var values = Variant.ToObject<float[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value.EqualsWithPrecision(item.OriginalValue));
        }
    }

    [Test]
    public void ConvertDoubleArray_Success()
    {
        var originalValues = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((double)(VariantValue)item.Variant).EqualsWithPrecision(item.OriginalValue));
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((double)(VariantValue)item.Variant).EqualsWithPrecision(item.OriginalValue));
        }

        var values = Variant.ToObject<double[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value.EqualsWithPrecision(item.OriginalValue));
        }
    }

    [Test]
    public void ConvertDecimalArray_Success()
    {
        var originalValues = new[] { 1.0m, 2.0m, 3.0m, 4.0m, 5.0m };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((decimal)(VariantValue)item.Variant).EqualsWithPrecision(item.OriginalValue));
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That(((decimal)(VariantValue)item.Variant).EqualsWithPrecision(item.OriginalValue));
        }

        var values = Variant.ToObject<decimal[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value.EqualsWithPrecision(item.OriginalValue));
        }
    }

    [Test]
    public void ConvertStringArray_Success()
    {
        var originalValues = new[] { "one", "two", "three", "four", "five" };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((string)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((string)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<string[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertDataTimeArray_Success()
    {
        var originalValues = new[]
        {
            new DateTime(1999, 12, 31, 23, 59, 59),
            new DateTime(2000, 1, 1, 0, 0, 0),
            new DateTime(2010, 6, 30, 12, 0, 0),
            new DateTime(2020, 12, 31, 0, 0, 0),
            new DateTime(2023, 2, 8, 16, 16, 30)
        };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((DateTime)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((DateTime)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<DateTime[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertTimeSpanArray_Success()
    {
        var originalValues = new[]
        {
            new TimeSpan(100),
            new TimeSpan(200),
            new TimeSpan(300),
            new TimeSpan(400),
            new TimeSpan(500),
        };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((TimeSpan)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((TimeSpan)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<TimeSpan[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertUriArray_Success()
    {
        var originalValues = new[]
        {
            new Uri("http://127.0.0.1"),
            new Uri("http://127.0.0.2"),
            new Uri("http://127.0.0.3"),
            new Uri("http://127.0.0.4"),
            new Uri("http://127.0.0.5"),
        };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((Uri)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((Uri)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<Uri[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertGuidArray_Success()
    {
        var originalValues = new[]
        {
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
        };

        // Ctor
        var v1 = new VariantArray(originalValues);
        Assert.That(v1.Count == 5);
        var items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((Guid)(VariantValue)item.Variant == item.OriginalValue);
        }

        // Methods
        v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);
        items = v1.Zip(originalValues, (variant, originalValue) => new { Variant = variant, OriginalValue = originalValue });
        foreach (var item in items)
        {
            Assert.That((Guid)(VariantValue)item.Variant == item.OriginalValue);
        }

        var values = Variant.ToObject<Guid[]>(v1);
        Assert.That(values.Length == 5);
        var items2 = values.Zip(originalValues, (value, originalValue) => new { Value = value, OriginalValue = originalValue });
        foreach (var item in items2)
        {
            Assert.That(item.Value == item.OriginalValue);
        }
    }

    [Test]
    public void ConvertStructArray_Success()
    {
        var originalValues = new[]
        {
            new VariantTestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
            new VariantTestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
            new VariantTestStruct { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
        };

        // Methods
        var v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 3);

        var convertedValues = Variant.ToObject<VariantTestStruct[]>(v1);
        Assert.That(convertedValues.SequenceEqual(originalValues));
    }

    [Test]
    public void ConvertGenericList_Success()
    {
        var originalValues = new List<string> { "one", "two", "three", "four", "five" };

        // Methods
        var v1 = (VariantArray)Variant.FromObject(originalValues);
        Assert.That(v1.Count == 5);

        var convertedValues = Variant.ToObject<List<string>>(v1);
        Assert.That(convertedValues.SequenceEqual(originalValues));
    }

    [Test]
    public void Enumerate_Success()
    {
        var v1 = new VariantArray(new[] { 'A', 'B', 'C' });

        using var e1 = ((IEnumerable<Variant>)v1).GetEnumerator();
        e1.MoveNext();
        Assert.That((char)(VariantValue)e1.Current == 'A');
        e1.MoveNext();
        Assert.That((char)(VariantValue)e1.Current == 'B');
        e1.MoveNext();
        Assert.That((char)(VariantValue)e1.Current == 'C');

        var e2 = ((IEnumerable)v1).GetEnumerator();
        e2.MoveNext();
        Assert.That(e2.Current != null && (char)(VariantValue)e2.Current == 'A');
        e2.MoveNext();
        Assert.That(e2.Current != null && (char)(VariantValue)e2.Current == 'B');
        e2.MoveNext();
        Assert.That(e2.Current != null && (char)(VariantValue)e2.Current == 'C');
    }

    [Test]
    public void AddGetSetItem_Success()
    {
        var v1 = new VariantArray(new[] { 'A', 'B', 'C' });
        Assert.That(v1.Count == 3);
        Assert.That((char)(VariantValue)v1[0] == 'A');
        Assert.That((char)(VariantValue)v1[1] == 'B');
        Assert.That((char)(VariantValue)v1[2] == 'C');

        v1.Add((VariantValue)'D');
        Assert.That(v1.Count == 4);
        Assert.That((char)(VariantValue)v1[3] == 'D');

        v1[0] = (VariantValue)'E';
        Assert.That((char)(VariantValue)v1[0] == 'E');
    }

    [Test]
    public void Reverse_Success()
    {

        var v1 = new VariantArray(new[] { 'A', 'B', 'C', 'D', 'E' });
        v1.Reverse();
        Assert.That((char)(VariantValue)v1[0] == 'E');
        Assert.That((char)(VariantValue)v1[1] == 'D');
        Assert.That((char)(VariantValue)v1[2] == 'C');
        Assert.That((char)(VariantValue)v1[3] == 'B');
        Assert.That((char)(VariantValue)v1[4] == 'A');
    }

    [Test]
    public void ToString_Success()
    {
        var v1 = new VariantArray(new[] { 'A', 'B', 'C' });

        var s = v1.ToString();
        Assert.That(s == "A,B,C");
    }

    [Test]
    public void Equals_Success()
    {
        var v1 = new VariantArray(new[] { 'A', 'B', 'C' });
        var v2 = new VariantArray(new[] { 'A', 'B', 'C' });
        var v3 = new VariantArray(new[] { 'D', 'E', 'F' });
        var v4 = new VariantArray(new[] { 'C', 'B', 'A' });
        var v5 = new VariantArray(new[] { 'A', 'B', 'C', 'D' });

        Assert.That(v1.Equals((object)null), Is.False);
        Assert.That(v1.Equals((object)v1));
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(v1.Equals(string.Empty), Is.False);
        Assert.That(v1.Equals((object)v2));

        Assert.That(v1.Equals(null), Is.False);
        Assert.That(v1.Equals(v1));
        Assert.That(v1.Equals(v2));
        Assert.That(v1.Equals(v3), Is.False);
        Assert.That(v1.Equals(v4), Is.False);
        Assert.That(v1.Equals(v5), Is.False);
    }

    [Test]
    public void GetHashCode_Success()
    {
        var v1 = new VariantArray(new[] { 'A', 'B', 'C' });
        var v2 = new VariantArray(new[] { 'A', 'B', 'C' });
        var v3 = new VariantArray(new[] { 'D', 'E', 'F' });
        var v4 = new VariantArray(new[] { 'C', 'B', 'A' });
        var v5 = new VariantArray(new[] { 'A', 'B', 'C', 'D' });

        Assert.That(v1.GetHashCode(), Is.EqualTo(v2.GetHashCode()));
        Assert.That(v1.GetHashCode(), Is.Not.EqualTo(v3.GetHashCode()));
        Assert.That(v1.GetHashCode(), Is.Not.EqualTo(v4.GetHashCode()));
        Assert.That(v1.GetHashCode(), Is.Not.EqualTo(v5.GetHashCode()));
    }

    [Test]
    public void AsVariantArray_Success()
    {
        Variant v = new VariantArray();
        Assert.That(v.AsVariantArray().GetType() == typeof(VariantArray));
    }
}