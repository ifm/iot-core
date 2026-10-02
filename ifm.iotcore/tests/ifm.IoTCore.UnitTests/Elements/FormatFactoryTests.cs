namespace ifm.IoTCore.UnitTests.Elements;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElementManager.Contracts.Elements.Formats;
using ElementManager.Contracts.Elements.Valuations;
using ifm.Common.Variant;
using NUnit.Framework;

internal enum TestEnum
{
    None = 0,
    First = 1,
    Second = 2,
    Third = 3,
    Fourth = 4,
    Reserved,
    Last = 9
}

internal class TestClass
{
    [VariantProperty("int_1", Required = true)]
    public int Int1 { get; set; }

    [VariantProperty("float_1", Required = true)]
    public float Float1 { get; set; }

    [VariantProperty("string_1", Required = true)]
    public string String1 { get; set; }

    [VariantProperty("enum_1", Required = true)]
    public TestEnum Enum1 { get; set; }

    [VariantProperty("int_2")]
    public int Int2 { get; set; }

    [VariantProperty("float_2")]
    public float Float2 { get; set; }
}

internal class DerivedTestClass : TestClass
{
    [VariantProperty("int_3", Required = true)]
    public int Int3 { get; set; }
}

internal class NestedTestClass : DerivedTestClass
{
    [VariantProperty("int_4", Required = true)]
    public int Int4 { get; set; }

    [VariantProperty("test_class_1", Required = true)]
    public TestClass TestClass1 { get; set; }
}

internal class DictionaryTestClass : IDictionary<string, TestClass>
{
    public IEnumerator<KeyValuePair<string, TestClass>> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<string, TestClass> item)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(KeyValuePair<string, TestClass> item)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(KeyValuePair<string, TestClass>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(KeyValuePair<string, TestClass> item)
    {
        throw new System.NotImplementedException();
    }

    public int Count { get; }
    public bool IsReadOnly { get; }
    public void Add(string key, TestClass value)
    {
        throw new System.NotImplementedException();
    }

    public bool ContainsKey(string key)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(string key)
    {
        throw new System.NotImplementedException();
    }

    public bool TryGetValue(string key, out TestClass value)
    {
        throw new System.NotImplementedException();
    }

    public TestClass this[string key]
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public ICollection<string> Keys { get; }
    public ICollection<TestClass> Values { get; }
}

[TestFixture]
internal class FormatFactoryTests
{
    [Test]
    public void TestSimpleFormat()
    {
        var f = FormatFactory.Create(typeof(bool));
        Assert.That(f?.GetType() == typeof(BooleanFormat));

        f = FormatFactory.Create(typeof(bool?));
        Assert.That(f?.GetType() == typeof(BooleanFormat));

        f = FormatFactory.Create(typeof(sbyte));
        Assert.That(f?.GetType() == typeof(Int8Format));

        f = FormatFactory.Create(typeof(sbyte?));
        Assert.That(f?.GetType() == typeof(Int8Format));

        f = FormatFactory.Create(typeof(byte));
        Assert.That(f?.GetType() == typeof(UInt8Format));

        f = FormatFactory.Create(typeof(byte?));
        Assert.That(f?.GetType() == typeof(UInt8Format));

        f = FormatFactory.Create(typeof(short));
        Assert.That(f?.GetType() == typeof(Int16Format));

        f = FormatFactory.Create(typeof(short?));
        Assert.That(f?.GetType() == typeof(Int16Format));

        f = FormatFactory.Create(typeof(ushort));
        Assert.That(f?.GetType() == typeof(UInt16Format));

        f = FormatFactory.Create(typeof(ushort?));
        Assert.That(f?.GetType() == typeof(UInt16Format));

        f = FormatFactory.Create(typeof(int));
        Assert.That(f?.GetType() == typeof(Int32Format));

        f = FormatFactory.Create(typeof(int?));
        Assert.That(f?.GetType() == typeof(Int32Format));

        f = FormatFactory.Create(typeof(uint));
        Assert.That(f?.GetType() == typeof(UInt32Format));

        f = FormatFactory.Create(typeof(uint?));
        Assert.That(f?.GetType() == typeof(UInt32Format));

        f = FormatFactory.Create(typeof(long));
        Assert.That(f?.GetType() == typeof(Int64Format));

        f = FormatFactory.Create(typeof(long?));
        Assert.That(f?.GetType() == typeof(Int64Format));

        f = FormatFactory.Create(typeof(ulong));
        Assert.That(f?.GetType() == typeof(UInt64Format));

        f = FormatFactory.Create(typeof(ulong?));
        Assert.That(f?.GetType() == typeof(UInt64Format));

        f = FormatFactory.Create(typeof(float));
        Assert.That(f?.GetType() == typeof(FloatFormat));

        f = FormatFactory.Create(typeof(float?));
        Assert.That(f?.GetType() == typeof(FloatFormat));

        f = FormatFactory.Create(typeof(double));
        Assert.That(f?.GetType() == typeof(DoubleFormat));

        f = FormatFactory.Create(typeof(double?));
        Assert.That(f?.GetType() == typeof(DoubleFormat));

        f = FormatFactory.Create(typeof(string));
        Assert.That(f?.GetType() == typeof(StringFormat));
    }

    [Test]
    public void TestEnumFormat()
    {
        var f = FormatFactory.Create(typeof(TestEnum));
        Assert.That(f?.GetType() == typeof(IntegerEnumFormat));
        Assert.That(((IntegerEnumFormat)f)?.Valuation != null);
        // ReSharper disable once PossibleNullReferenceException
        var ev = ((IntegerEnumFormat)f).Valuation;
        Assert.That(ev.Values.Count == 7);
        Assert.That(ev.Values.TryGetValue("0", out var value));
        Assert.That(value == "None");
        Assert.That(ev.Values.TryGetValue("1", out value));
        Assert.That(value == "First");
        Assert.That(ev.Values.TryGetValue("2", out value));
        Assert.That(value == "Second");
        Assert.That(ev.Values.TryGetValue("3", out value));
        Assert.That(value == "Third");
        Assert.That(ev.Values.TryGetValue("4", out value));
        Assert.That(value == "Fourth");
        Assert.That(ev.Values.TryGetValue("5", out value));
        Assert.That(value == "Reserved");
        Assert.That(ev.Values.TryGetValue("9", out value));
        Assert.That(value == "Last");

        f = FormatFactory.Create(typeof(TestEnum?));
        Assert.That(f?.GetType() == typeof(IntegerEnumFormat));
        Assert.That(((IntegerEnumFormat)f)?.Valuation != null);
        // ReSharper disable once PossibleNullReferenceException
        ev = ((IntegerEnumFormat)f).Valuation;
        Assert.That(ev.Values.Count == 7);
        Assert.That(ev.Values.TryGetValue("0", out value));
        Assert.That(value == "None");
        Assert.That(ev.Values.TryGetValue("1", out value));
        Assert.That(value == "First");
        Assert.That(ev.Values.TryGetValue("2", out value));
        Assert.That(value == "Second");
        Assert.That(ev.Values.TryGetValue("3", out value));
        Assert.That(value == "Third");
        Assert.That(ev.Values.TryGetValue("4", out value));
        Assert.That(value == "Fourth");
        Assert.That(ev.Values.TryGetValue("5", out value));
        Assert.That(value == "Reserved");
        Assert.That(ev.Values.TryGetValue("9", out value));
        Assert.That(value == "Last");
    }

    [Test]
    public void TestSimpleArrayFormat()
    {
        var f = FormatFactory.Create(typeof(int[]));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(Int32Format));

        f = FormatFactory.Create(typeof(List<int>));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(Int32Format));

        f = FormatFactory.Create(typeof(IList<int>));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(Int32Format));

        f = FormatFactory.Create(typeof(string[]));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(StringFormat));

        f = FormatFactory.Create(typeof(List<string>));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(StringFormat));

        f = FormatFactory.Create(typeof(IList<string>));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(StringFormat));

        f = FormatFactory.Create(typeof(TestEnum[]));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format?.GetType() == typeof(IntegerEnumFormat));
        // ReSharper disable once PossibleNullReferenceException
        var v = ((IntegerEnumFormat)((ArrayFormat)f).Valuation.Format).Valuation;
        Assert.That(v.Values.Count == 7);
        Assert.That(v.Values.TryGetValue("0", out var value));
        Assert.That(value == "None");
        Assert.That(v.Values.TryGetValue("1", out value));
        Assert.That(value == "First");
        Assert.That(v.Values.TryGetValue("2", out value));
        Assert.That(value == "Second");
        Assert.That(v.Values.TryGetValue("3", out value));
        Assert.That(value == "Third");
        Assert.That(v.Values.TryGetValue("4", out value));
        Assert.That(value == "Fourth");
        Assert.That(v.Values.TryGetValue("5", out value));
        Assert.That(value == "Reserved");
        Assert.That(v.Values.TryGetValue("9", out value));
        Assert.That(value == "Last");
    }

    [Test]
    public void TestDictionaryFormat()
    {
        var f = FormatFactory.Create(typeof(Dictionary<int, int>));
        Assert.That(f?.GetType() == typeof(MapFormat));
        Assert.That(((MapFormat)f)?.Valuation.GetType() == typeof(MapValuation));
        Assert.That(((MapFormat)f)?.Valuation?.Format?.GetType() == typeof(Int32Format));

        f = FormatFactory.Create(typeof(Dictionary<string, string>));
        Assert.That(f?.GetType() == typeof(MapFormat));
        Assert.That(((MapFormat)f)?.Valuation.GetType() == typeof(MapValuation));
        Assert.That(((MapFormat)f)?.Valuation?.Format?.GetType() == typeof(StringFormat));

        f = FormatFactory.Create(typeof(Dictionary<string, TestClass>));
        Assert.That(f?.GetType() == typeof(MapFormat));
        Assert.That(((MapFormat)f)?.Valuation.GetType() == typeof(MapValuation));
        Assert.That(((MapFormat)f)?.Valuation?.Format?.GetType() == typeof(ObjectFormat));

        f = FormatFactory.Create(typeof(DictionaryTestClass));
        Assert.That(f?.GetType() == typeof(MapFormat));
        Assert.That(((MapFormat)f)?.Valuation.GetType() == typeof(MapValuation));
        Assert.That(((MapFormat)f)?.Valuation?.Format?.GetType() == typeof(ObjectFormat));
    }

    [Test]
    public void TestVariantFormat()
    {
        var f = FormatFactory.Create(typeof(Variant));
        Assert.That(f.GetType(), Is.EqualTo(typeof(AnyFormat)));
        f = FormatFactory.Create(typeof(VariantValue));
        Assert.That(f.GetType(), Is.EqualTo(typeof(ValueFormat)));
        f = FormatFactory.Create(typeof(VariantArray));
        Assert.That(f.GetType(), Is.EqualTo(typeof(ArrayFormat)));
        f = FormatFactory.Create(typeof(VariantObject));
        Assert.That(f.GetType(), Is.EqualTo(typeof(ObjectFormat)));
    }

    [Test]
    public void TestComplexFormat()
    {
        var f = FormatFactory.Create(typeof(TestClass));
        Assert.That(f?.GetType() == typeof(ObjectFormat));
        Assert.That(((ObjectFormat)f)?.Valuation?.Fields?.Count == 6);
        // ReSharper disable once PossibleNullReferenceException
        var field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "float_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "string_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(StringFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "enum_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(IntegerEnumFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "float_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.True);

        f = FormatFactory.Create(typeof(DerivedTestClass));
        Assert.That(f?.GetType() == typeof(ObjectFormat));
        Assert.That(((ObjectFormat)f)?.Valuation?.Fields?.Count == 7);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "float_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "string_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(StringFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "enum_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(IntegerEnumFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "float_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_3");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);

        f = FormatFactory.Create(typeof(NestedTestClass));
        Assert.That(f?.GetType() == typeof(ObjectFormat));
        Assert.That(((ObjectFormat)f)?.Valuation?.Fields?.Count == 9);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "float_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "string_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(StringFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "enum_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(IntegerEnumFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "float_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_3");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "int_4");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);

        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)f).Valuation.Fields.FirstOrDefault(x => x.Name == "test_class_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format?.GetType() == typeof(ObjectFormat));
        Assert.That(field.Optional, Is.Null);
        Assert.That(((ObjectFormat)field.Format)?.Valuation?.Fields?.Count == 6);
        // ReSharper disable once PossibleNullReferenceException
        var field1 = ((ObjectFormat)field.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "int_1");
        Assert.That(field1 != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field1.Format.GetType() == typeof(Int32Format));
        Assert.That(field1.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field1 = ((ObjectFormat)field.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "float_1");
        Assert.That(field1 != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field1.Format.GetType() == typeof(FloatFormat));
        Assert.That(field1.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field1 = ((ObjectFormat)field.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "string_1");
        Assert.That(field1 != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field1.Format.GetType() == typeof(StringFormat));
        Assert.That(field1.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field1 = ((ObjectFormat)field.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "enum_1");
        Assert.That(field1 != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field1.Format.GetType() == typeof(IntegerEnumFormat));
        Assert.That(field1.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field1 = ((ObjectFormat)field.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "int_2");
        Assert.That(field1 != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field1.Format.GetType() == typeof(Int32Format));
        Assert.That(field1.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field1 = ((ObjectFormat)field.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "float_2");
        Assert.That(field1 != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field1.Format.GetType() == typeof(FloatFormat));
        Assert.That(field1.Optional, Is.True);
    }

    [Test]
    public void TestComplexArrayFormat()
    {
        var f = FormatFactory.Create(typeof(TestClass[]));
        Assert.That(f?.GetType() == typeof(ArrayFormat));
        Assert.That(((ArrayFormat)f)?.Valuation?.Format.GetType() == typeof(ObjectFormat));

        // ReSharper disable once PossibleNullReferenceException
        Assert.That(((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation?.Fields?.Count == 6);

        // ReSharper disable once PossibleNullReferenceException
        var field = ((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "int_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "float_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "string_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(StringFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "enum_1");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(IntegerEnumFormat));
        Assert.That(field.Optional, Is.Null);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "int_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(Int32Format));
        Assert.That(field.Optional, Is.True);
        // ReSharper disable once PossibleNullReferenceException
        field = ((ObjectFormat)((ArrayFormat)f).Valuation.Format).Valuation.Fields.FirstOrDefault(x => x.Name == "float_2");
        Assert.That(field != null);
        // ReSharper disable once PossibleNullReferenceException
        Assert.That(field.Format.GetType() == typeof(FloatFormat));
        Assert.That(field.Optional, Is.True);
    }
}