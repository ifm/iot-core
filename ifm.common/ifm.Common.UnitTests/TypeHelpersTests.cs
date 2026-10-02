namespace ifm.Common.UnitTests;

using System;
using System.Collections;
using System.Collections.Generic;
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
    public int Int1 { get; set; }
    public float Float1 { get; set; }
    public string String1 { get; set; }
    public TestEnum Enum1 { get; set; }
    public int Int2 { get; set; }
    public float Float2 { get; set; }
}

internal struct TestStruct
{
    public int Int1 { get; set; }
    public float Float1 { get; set; }
    public string String1 { get; set; }
    public TestEnum Enum1 { get; set; }
    public int Int2 { get; set; }
    public float Float2 { get; set; }
}

internal class TestList : List<int>;

internal class TestDictionary : Dictionary<int, string>;

[TestFixture]
public class TypeHelpersTests
{
    [Test]
    public void TestIsNullable()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsNullable(null));
            Assert.That(TypeHelpers.IsNullable(typeof(int)), Is.False);
            Assert.That(TypeHelpers.IsNullable(typeof(int?)), Is.True);
        }
    }

    [Test]
    public void TestIsSimple()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsSimple(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsSimple(typeof(int)), Is.True);
            Assert.That(TypeHelpers.IsSimple(typeof(int?)), Is.True);
        }

        var t = typeof(int);
        var ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.True);
            Assert.That(t, Is.EqualTo(typeof(int)));
        }

        t = typeof(int?);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.True);
            Assert.That(t, Is.EqualTo(typeof(int)));
        }

        t = typeof(TestEnum);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(t, Is.EqualTo(typeof(TestEnum)));
        }

        t = typeof(TestEnum?);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(t, Is.EqualTo(typeof(TestEnum)));
        }

        t = typeof(int[]);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(t, Is.EqualTo(typeof(int[])));
        }

        t = typeof(IEnumerable<int>);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(t, Is.EqualTo(typeof(IEnumerable<int>)));
        }

        t = typeof(IList<int>);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(t, Is.EqualTo(typeof(IList<int>)));
        }

        t = typeof(TestClass);
        ret = TypeHelpers.IsSimple(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.False);
            Assert.That(t, Is.EqualTo(typeof(TestClass)));
        }
    }

    [Test]
    public void TestIsEnum()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsEnum(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsEnum(typeof(TestEnum)), Is.True);
            Assert.That(TypeHelpers.IsEnum(typeof(TestEnum?)), Is.True);
        }

        var t = typeof(TestEnum);
        var ret = TypeHelpers.IsEnum(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.True);
            Assert.That(t, Is.EqualTo(typeof(TestEnum)));
        }

        t = typeof(TestEnum?);
        ret = TypeHelpers.IsEnum(ref t);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(ret, Is.True);
            Assert.That(t, Is.EqualTo(typeof(TestEnum)));
        }
    }

    [Test]
    public void TestIsString()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsString(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsString(typeof(string)), Is.True);
            Assert.That(TypeHelpers.IsString(typeof(char[])), Is.False);
        }
    }

    [Test]
    public void TestIsArray()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsArray(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsArray(typeof(int[])), Is.True);
            Assert.That(TypeHelpers.IsArray(typeof(IList<int>)), Is.False);
        }
    }

    [Test]
    public void TestIsEnumerable()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsNonGenericEnumerable(null));
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsGenericEnumerable(null));

        Assert.That(TypeHelpers.IsNonGenericEnumerable(typeof(IEnumerable)), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(int[])), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(IEnumerable<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(ICollection<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(IList<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(List<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(TestList)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(IDictionary<int, string>)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(Dictionary<int, string>)), Is.True);
            Assert.That(TypeHelpers.IsGenericEnumerable(typeof(TestDictionary)), Is.True);
        }
    }

    [Test]
    public void TestIsCollection()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsNonGenericCollection(null));
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsGenericCollection(null));

        Assert.That(TypeHelpers.IsNonGenericCollection(typeof(ICollection)), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsGenericCollection(typeof(int[])), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(IEnumerable<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(ICollection<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(IList<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(List<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(TestList)), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(IDictionary<int, string>)), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(Dictionary<int, string>)), Is.True);
            Assert.That(TypeHelpers.IsGenericCollection(typeof(TestDictionary)), Is.True);
        }
    }

    [Test]
    public void TestIsList()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsNonGenericList(null));
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsGenericList(null));

        Assert.That(TypeHelpers.IsNonGenericList(typeof(IList)), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsGenericList(typeof(int[])), Is.True);
            Assert.That(TypeHelpers.IsGenericList(typeof(IEnumerable<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericList(typeof(ICollection<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericList(typeof(IList<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericList(typeof(List<int>)), Is.True);
            Assert.That(TypeHelpers.IsGenericList(typeof(TestList)), Is.True);
            Assert.That(TypeHelpers.IsGenericList(typeof(IDictionary<int, string>)), Is.False);
            Assert.That(TypeHelpers.IsGenericList(typeof(Dictionary<int, string>)), Is.False);
            Assert.That(TypeHelpers.IsGenericList(typeof(TestDictionary)), Is.False);
        }
    }

    [Test]
    public void TestIsDictionary()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsNonGenericDictionary(null));
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsGenericDictionary(null));

        Assert.That(TypeHelpers.IsNonGenericDictionary(typeof(IDictionary)), Is.True);
        Assert.That(TypeHelpers.IsNonGenericDictionary(typeof(TestDictionary)), Is.True);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(int[])), Is.False);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(IEnumerable<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(ICollection<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(IList<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(List<int>)), Is.False);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(TestList)), Is.False);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(IDictionary<int, string>)), Is.True);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(Dictionary<int, string>)), Is.True);
            Assert.That(TypeHelpers.IsGenericDictionary(typeof(TestDictionary)), Is.True);
        }
    }

    [Test]
    public void TestIsVariant()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsVariant(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsVariant(typeof(Common.Variant.Variant)), Is.True);
            Assert.That(TypeHelpers.IsVariant(typeof(Common.Variant.VariantValue)), Is.True);
            Assert.That(TypeHelpers.IsVariant(typeof(Common.Variant.VariantArray)), Is.True);
            Assert.That(TypeHelpers.IsVariant(typeof(Common.Variant.VariantObject)), Is.True);
        }
    }

    [Test]
    public void TestIsStruct()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsStruct(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsStruct(typeof(int)), Is.False);
            Assert.That(TypeHelpers.IsStruct(typeof(int[])), Is.False);
            Assert.That(TypeHelpers.IsStruct(typeof(IList<int>)), Is.False);
            Assert.That(TypeHelpers.IsStruct(typeof(List<int>)), Is.False);
            Assert.That(TypeHelpers.IsStruct(typeof(TestClass)), Is.False);
            Assert.That(TypeHelpers.IsStruct(typeof(TestStruct)), Is.True);
        }
    }

    [Test]
    public void TestIsComplex()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.IsComplex(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.IsComplex(typeof(int)), Is.False);
            Assert.That(TypeHelpers.IsComplex(typeof(int[])), Is.True);
            Assert.That(TypeHelpers.IsComplex(typeof(IList<int>)), Is.False);
            Assert.That(TypeHelpers.IsComplex(typeof(List<int>)), Is.True);
            Assert.That(TypeHelpers.IsComplex(typeof(TestClass)), Is.True);
            Assert.That(TypeHelpers.IsComplex(typeof(TestStruct)), Is.True);
        }
    }

    [Test]
    public void TestGetGenericUnderlyingType()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.GetGenericUnderlyingType(null));
        Assert.Throws<ArgumentException>(() => TypeHelpers.GetGenericUnderlyingType(typeof(int?), -1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.GetGenericUnderlyingType(typeof(int)), Is.Null);
            Assert.That(TypeHelpers.GetGenericUnderlyingType(typeof(int[])), Is.Null);
            Assert.That(TypeHelpers.GetGenericUnderlyingType(typeof(int?)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericUnderlyingType(typeof(IList<int>)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericUnderlyingType(typeof(IDictionary<int, string>)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericUnderlyingType(typeof(IDictionary<int, string>), 1), Is.EqualTo(typeof(string)));
        }
    }

    [Test]
    public void TestGetArrayUnderlyingType()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.GetArrayUnderlyingType(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.GetArrayUnderlyingType(typeof(int)), Is.Null);
            Assert.That(TypeHelpers.GetArrayUnderlyingType(typeof(IList<int>)), Is.Null);
            Assert.That(TypeHelpers.GetArrayUnderlyingType(typeof(List<int>)), Is.Null);
            Assert.That(TypeHelpers.GetArrayUnderlyingType(typeof(int[])), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetArrayUnderlyingType(typeof(int[][])), Is.EqualTo(typeof(int[])));
        }
    }

    [Test]
    public void TestGetGenericEnumerableUnderlyingType()
    {
        Assert.Throws<ArgumentNullException>(() => TypeHelpers.GetGenericEnumerableUnderlyingType(null));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(int)), Is.Null);
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(int[])), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(IEnumerable<int>)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(ICollection<int>)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(IList<int>)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(List<int>)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(TestList)), Is.EqualTo(typeof(int)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(IDictionary<int, string>)), Is.EqualTo(typeof(KeyValuePair<int, string>)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(Dictionary<int, string>)), Is.EqualTo(typeof(KeyValuePair<int, string>)));
            Assert.That(TypeHelpers.GetGenericEnumerableUnderlyingType(typeof(TestDictionary)), Is.EqualTo(typeof(KeyValuePair<int, string>)));
        }
    }
}