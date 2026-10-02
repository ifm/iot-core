namespace ifm.Common.UnitTests.Variant;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Variant;
using NUnit.Framework;

[TestFixture]
public class VariantObjectTests
{
    [Test]
    public void ConvertStruct_Success()
    {
        var o1 = new VariantTestStruct { Int1 = 12, Float1 = -5.5f, String1 = "huhu" };
        var v1 = (VariantObject)Variant.FromObject(o1);
        Assert.That(v1.Count == 3);
        Assert.That((int)(VariantValue)v1["int1"] == o1.Int1);
        Assert.That(((float)(VariantValue)v1["float1"]).EqualsWithPrecision(o1.Float1));
        Assert.That((string)(VariantValue)v1["string1"] == o1.String1);
        var o2 = Variant.ToObject<VariantTestStruct>(v1);
        Assert.That(o2.Equals(o1));
    }

    [Test]
    public void ConvertClass_Success()
    {
        var o1 = new VariantTestClass { Int1 = 12, Float1 = -5.5f, String1 = "huhu" };
        var v1 = (VariantObject)Variant.FromObject(o1);
        Assert.That(v1.Count == 3);
        Assert.That((int)(VariantValue)v1["int1"] == o1.Int1);
        Assert.That(((float)(VariantValue)v1["float1"]).EqualsWithPrecision(o1.Float1));
        Assert.That((string)(VariantValue)v1["string1"] == o1.String1);
        var o2 = Variant.ToObject<VariantTestClass>(v1);
        Assert.That(o2.Equals(o1));
    }

    [Test]
    public void ConvertBigClass_Success()
    {
        var o1 = new VariantTestBigClass();
        o1.Init();
        var v1 = (VariantObject)Variant.FromObject(o1);
        Assert.That(v1.Count == 10);
        var o2 = Variant.ToObject<VariantTestBigClass>(v1);
        Assert.That(o2.Equals(o1));
    }

    [Test]
    public void ConvertGenericDictionary_Success()
    {
        var o1 = new Dictionary<int, string> { { 1, "hu" }, { 2, "huhu" }, { 3, "huhuhu" }, { 4, null } };
        var v1 = (VariantObject)Variant.FromObject(o1);
        Assert.That(v1.Count == 4);
        var o2 = Variant.ToObject<Dictionary<int, string>>(v1);
        Assert.That(o2.SequenceEqual(o1));
    }

    [Test]
    public void ModifyVariant_Success()
    {
        var v1 = new VariantObject
        {
            { (VariantValue)"1", (VariantValue)"hu" },
            { "2", (VariantValue)"huhu" },
            { "3", (VariantValue)"huhuhu" }
        };

        Assert.That(v1.Count == 3);
        Assert.That(v1.ContainsKey((VariantValue)"1"));
        Assert.That(v1.ContainsKey("2"));

        Assert.That((string)(VariantValue)v1[(VariantValue)"1"] == "hu");
        Assert.That((string)(VariantValue)v1["2"] == "huhu");
        Assert.That((string)(VariantValue)v1["3"] == "huhuhu");

        v1[(VariantValue)"1"] = (VariantValue)"la";
        v1["2"] = (VariantValue)"lala";
        v1["3"] = (VariantValue)"lalala";

        Assert.That((string)(VariantValue)v1["1"] == "la");
        Assert.That((string)(VariantValue)v1["2"] == "lala");
        Assert.That((string)(VariantValue)v1["3"] == "lalala");

        v1.Add("4", (VariantValue)"hidi");
        Assert.That(v1.Count == 4);
        Assert.That((string)(VariantValue)v1["4"] == "hidi");

        v1.Remove("4");
        Assert.That(v1.Count == 3);
        Assert.That(v1.ContainsKey("4"),Is.False);

        v1.Add((VariantValue)"4", (VariantValue)"hidi");
        Assert.That(v1.Count == 4);
        Assert.That((string)(VariantValue)v1["4"] == "hidi");

        v1.Remove((VariantValue)"4");
        Assert.That(v1.Count == 3);
        Assert.That(v1.ContainsKey("4"),Is.False);
    }

    [Test]
    public void TryGetValue_Success()
    {
        var v1 = new VariantObject
        {
            { "1", (VariantValue)"hu" },
            { "2", (VariantValue)"huhu" },
            { "3", (VariantValue)"huhuhu" }
        };

        Assert.That(v1.TryGetValue((VariantValue)"1", out var value));
        Assert.That((string)(VariantValue)value == "hu");
        Assert.That(v1.TryGetValue((VariantValue)"2", out value));
        Assert.That((string)(VariantValue)value == "huhu");
        Assert.That(v1.TryGetValue((VariantValue)"3", out value));
        Assert.That((string)(VariantValue)value == "huhuhu");

        Assert.That(v1.TryGetValue((VariantValue)"4", out _), Is.False);

        Assert.That(v1.TryGetValue("1", out value));
        Assert.That((string)(VariantValue)value == "hu");
        Assert.That(v1.TryGetValue("2", out value));
        Assert.That((string)(VariantValue)value == "huhu");
        Assert.That(v1.TryGetValue("3", out value));
        Assert.That((string)(VariantValue)value == "huhuhu");

        Assert.That(v1.TryGetValue("4", out _), Is.False);

        Assert.Throws<ArgumentNullException>(() => v1.TryGetValue((IEnumerable<Variant>)null, out _));
        Assert.Throws<ArgumentNullException>(() => v1.TryGetValue((IEnumerable<string>)null, out _));

        Assert.That(v1.TryGetValue([(VariantValue)"5", (VariantValue)"4", (VariantValue)"3"], out value));
        Assert.That((string)(VariantValue)value == "huhuhu");

        Assert.That(v1.TryGetValue(["5", "4", "2"], out value));
        Assert.That((string)(VariantValue)value == "huhu");
    }

    [Test]
    public void TryGetValueCaseMismatch_Success()
    {
        var v1 = new VariantObject
        {
            { "Int1", (VariantValue)1 },
            { "Float1", (VariantValue)1.5 },
            { "String1", (VariantValue)"huhuhu" }
        };

        Assert.That(v1.TryGetValue("int1", out var value));
        Assert.That((int)(VariantValue)value == 1);
        Assert.That(v1.TryGetValue("float1", out value));
        Assert.That(((float)(VariantValue)value).EqualsWithPrecision(1.5f));
        Assert.That(v1.TryGetValue("string1", out value));
        Assert.That((string)(VariantValue)value == "huhuhu");

        var o1 = Variant.ToObject<VariantTestStruct>(v1);
        Assert.That(o1.Int1 == 1);
        Assert.That(o1.Float1.EqualsWithPrecision(1.5f));
        Assert.That(o1.String1 == "huhuhu");
    }

    [Test]
    public void Enumerate_Success()
    {
        var v1 = new VariantObject
        {
            { "1", (VariantValue)"hu" },
            { "2", (VariantValue)"huhu" },
            { "3", (VariantValue)"huhuhu" }
        };

        using var e1 = ((IEnumerable<KeyValuePair<Variant, Variant>>)v1).GetEnumerator();
        e1.MoveNext();
        Assert.That((string)(VariantValue)e1.Current.Key == "1");
        Assert.That((string)(VariantValue)e1.Current.Value == "hu");
        e1.MoveNext();
        Assert.That((string)(VariantValue)e1.Current.Key == "2");
        Assert.That((string)(VariantValue)e1.Current.Value == "huhu");
        e1.MoveNext();
        Assert.That((string)(VariantValue)e1.Current.Key == "3");
        Assert.That((string)(VariantValue)e1.Current.Value == "huhuhu");

        var e2 = ((IEnumerable)v1).GetEnumerator();
        e2.MoveNext();
        Assert.That(e2.Current != null && (string)(VariantValue)((KeyValuePair<Variant, Variant>)e2.Current).Key == "1");
        Assert.That(e2.Current != null && (string)(VariantValue)((KeyValuePair<Variant, Variant>)e2.Current).Value == "hu");
    }

    [Test]
    public void ToString_Success()
    {
        var o1 = new Dictionary<int, string> { { 1, "hu" }, { 2, "huhu" }, { 3, "huhuhu" } };
        var v1 = Variant.FromObject(o1);
        var s = v1.ToString();
        Assert.That(s == "{ 1: hu, 2: huhu, 3: huhuhu }");
    }

    [Test]
    public void Equals_Success()
    {
        var v1 = new VariantObject { { "1", (VariantValue)"hu" }, { "2", (VariantValue)"huhu" }, { "3", (VariantValue)"huhuhu" } };
        var v2 = new VariantObject { { "1", (VariantValue)"hu" }, { "2", (VariantValue)"huhu" }, { "3", (VariantValue)"huhuhu" } };
        var v3 = new VariantObject { { "4", (VariantValue)"hu" }, { "5", (VariantValue)"huhu" }, { "6", (VariantValue)"huhuhu" } };
        var v4 = new VariantObject { { "1", (VariantValue)"la" }, { "2", (VariantValue)"lala" }, { "3", (VariantValue)"lalala" } };
        var v5 = new VariantObject { { "1", (VariantValue)"hu" }, { "2", (VariantValue)"huhu" }, { "3", (VariantValue)"huhuhu" }, { "4", (VariantValue)"huhuhuhu" } };

        Assert.That(v1.Equals((object)null),Is.False);
        Assert.That(v1.Equals((object)v1));
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(v1.Equals(string.Empty),Is.False);
        Assert.That(v1.Equals((object)v2));

        Assert.That(v1.Equals(null),Is.False);
        Assert.That(v1.Equals(v1));
        Assert.That(v1.Equals(v2));
        Assert.That(v1.Equals(v3),Is.False);
        Assert.That(v1.Equals(v4),Is.False);
        Assert.That(v1.Equals(v5),Is.False);
    }

    [Test]
    public void GetHashCode_Success()
    {
        var v1 = new VariantObject { { "1", (VariantValue)"hu" }, { "2", (VariantValue)"huhu" }, { "3", (VariantValue)"huhuhu" } };
        var v2 = new VariantObject { { "1", (VariantValue)"hu" }, { "2", (VariantValue)"huhu" }, { "3", (VariantValue)"huhuhu" } };
        var v3 = new VariantObject { { "4", (VariantValue)"hu" }, { "5", (VariantValue)"huhu" }, { "6", (VariantValue)"huhuhu" } };
        var v4 = new VariantObject { { "1", (VariantValue)"la" }, { "2", (VariantValue)"lala" }, { "3", (VariantValue)"lalala" } };
        var v5 = new VariantObject { { "1", (VariantValue)"hu" }, { "2", (VariantValue)"huhu" }, { "3", (VariantValue)"huhuhu" }, { "4", (VariantValue)"huhuhuhu" } };

        
        Assert.That(v1.GetHashCode(), Is.EqualTo(v2.GetHashCode()));
        Assert.That(v1.GetHashCode(), Is.Not.EqualTo(v3.GetHashCode()));
        Assert.That(v1.GetHashCode(), Is.Not.EqualTo(v4.GetHashCode()));
        Assert.That(v1.GetHashCode(), Is.Not.EqualTo(v5.GetHashCode()));
    }

    [Test]
    public void AsVariantObject_Success()
    {
        Variant v = new VariantObject();
        Assert.That(v.AsVariantObject().GetType() == typeof(VariantObject));
    }
}