namespace ifm.Common.UnitTests.Variant;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Variant;
using NUnit.Framework;

internal class VariantConverterTests
{
    [Test]
    public void ConvertSimple_Success()
    {
        // Simple
        var v1 = (VariantValue)Variant.FromObject(true);
        Assert.That(v1.Type == VariantValue.ValueTypes.Boolean);
        Assert.That((bool)v1);
        var bool1 = v1.ToObject<bool>();
        Assert.That(bool1);

        v1 = (VariantValue)Variant.FromObject('A');
        Assert.That(v1.Type == VariantValue.ValueTypes.Character);
        Assert.That((char)v1 == 'A');
        var char1 = v1.ToObject<char>();
        Assert.That(char1 == 'A');

        v1 = (VariantValue)Variant.FromObject((sbyte)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int8);
        Assert.That((sbyte)v1 == 123);
        var sbyte1 = v1.ToObject<sbyte>();
        Assert.That(sbyte1 == 123);

        v1 = (VariantValue)Variant.FromObject((byte)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt8);
        Assert.That((byte)v1 == 123);
        var byte1 = v1.ToObject<byte>();
        Assert.That(byte1 == 123);

        v1 = (VariantValue)Variant.FromObject((short)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int16);
        Assert.That((short)v1 == 123);
        var short1 = v1.ToObject<short>();
        Assert.That(short1 == 123);

        v1 = (VariantValue)Variant.FromObject((ushort)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt16);
        Assert.That((ushort)v1 == 123);
        var ushort1 = v1.ToObject<ushort>();
        Assert.That(ushort1 == 123);

        v1 = (VariantValue)Variant.FromObject(123);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int32);
        Assert.That((int)v1 == 123);
        var int1 = v1.ToObject<int>();
        Assert.That(int1 == 123);

        v1 = (VariantValue)Variant.FromObject((uint)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt32);
        Assert.That((uint)v1 == 123);
        var uint1 = v1.ToObject<uint>();
        Assert.That(uint1 == 123);

        v1 = (VariantValue)Variant.FromObject((long)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int64);
        Assert.That((long)v1 == 123);
        var long1 = v1.ToObject<long>();
        Assert.That(long1 == 123);

        v1 = (VariantValue)Variant.FromObject((ulong)123);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt64);
        Assert.That((ulong)v1 == 123);
        var ulong1 = v1.ToObject<ulong>();
        Assert.That(ulong1 == 123);

        v1 = (VariantValue)Variant.FromObject(123.0f);
        Assert.That(v1.Type == VariantValue.ValueTypes.Float);
        Assert.That(((float)v1).EqualsWithPrecision(123.0f));
        var float1 = v1.ToObject<float>();
        Assert.That(float1.EqualsWithPrecision(123.0f));

        v1 = (VariantValue)Variant.FromObject(123.0);
        Assert.That(v1.Type == VariantValue.ValueTypes.Double);
        Assert.That(((double)v1).EqualsWithPrecision(123.0));
        var double1 = v1.ToObject<double>();
        Assert.That(double1.EqualsWithPrecision(123.0));

        v1 = (VariantValue)Variant.FromObject((decimal)123.0);
        Assert.That(v1.Type == VariantValue.ValueTypes.Decimal);
        Assert.That(((decimal)v1).EqualsWithPrecision(123.0m));
        var decimal1 = v1.ToObject<decimal>();
        Assert.That(decimal1.EqualsWithPrecision(123.0m));

        v1 = (VariantValue)Variant.FromObject("huhu");
        Assert.That(v1.Type == VariantValue.ValueTypes.String);
        Assert.That((string)v1 == "huhu");
        var string1 = v1.ToObject<string>();
        Assert.That(string1 == "huhu");

        var dateTime = new DateTime(2000, 1, 1);
        v1 = (VariantValue)Variant.FromObject(dateTime);
        Assert.That(v1.Type == VariantValue.ValueTypes.DateTime);
        Assert.That((DateTime)v1 == dateTime);
        var dateTime1 = v1.ToObject<DateTime>();
        Assert.That(dateTime1.Equals(dateTime));

        var timeSpan = new TimeSpan(1000);
        v1 = (VariantValue)Variant.FromObject(timeSpan);
        Assert.That(v1.Type == VariantValue.ValueTypes.TimeSpan);
        Assert.That((TimeSpan)v1 == timeSpan);
        var timeSpan1 = v1.ToObject<TimeSpan>();
        Assert.That(timeSpan1.Equals(timeSpan));

        var uri = new Uri("http://127.0.0.1:80");
        v1 = (VariantValue)Variant.FromObject(uri);
        Assert.That(v1.Type == VariantValue.ValueTypes.Uri);
        Assert.That((Uri)v1 == uri);
        var uri1 = v1.ToObject<Uri>();
        Assert.That(uri1.Equals(uri));

        var guid = Guid.NewGuid();
        v1 = (VariantValue)Variant.FromObject(guid);
        Assert.That(v1.Type == VariantValue.ValueTypes.Guid);
        Assert.That((Guid)v1 == guid);
        var guid1 = v1.ToObject<Guid>();
        Assert.That(guid1.Equals(guid));

        // Enum
        v1 = (VariantValue)Variant.FromObject(VariantTestEnum.Red);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int32);
        Assert.That((int)v1 == (int)VariantTestEnum.Red);
        var enum1 = v1.ToObject<int>();
        Assert.That(enum1 == (int)VariantTestEnum.Red);
    }

    [Test]
    public void ConvertArray_Success()
    {
        // Array
        var array1 = new[] { "one", "two", "three" };
        var v1 = (VariantArray)Variant.FromObject(array1);
        var array11 = v1.ToObject<string[]>();
        Assert.That(array11.SequenceEqual(array1));

        // Generic list
        var list1 = new List<string> { "one", "two", "three" };
        v1 = (VariantArray)Variant.FromObject(list1);
        var list11 = v1.ToObject<List<string>>();
        Assert.That(list11.SequenceEqual(list1));

        // Generic enumerable
        var enu1 = new VariantTestGenericEnumerable { "one", "two", "three" };
        v1 = (VariantArray)Variant.FromObject(enu1);
        var enu11 = v1.ToObject<VariantTestGenericEnumerable>();
        Assert.That(enu11.SequenceEqual(enu1));
    }

    [Test]
    public void ConvertDictionary_Success()
    {
        var dic1 = new Dictionary<int, string> { { 1, "hu" }, { 2, "huhu" }, { 3, "huhuhu" }, { 4, null } };
        var v1 = (VariantObject)Variant.FromObject(dic1);
        var dic11 = v1.ToObject<Dictionary<int, string>>();
        Assert.That(dic11.SequenceEqual(dic1));
    }

    [Test]
    public void ConvertComplex_Success()
    {
        // Struct
        var struct1 = new VariantTestStruct { Int1 = 12, Float1 = -5.5f, String1 = "huhu" };
        var v1 = (VariantObject)Variant.FromObject(struct1);
        var struct2 = v1.ToObject<VariantTestStruct>();
        Assert.That(struct2.Equals(struct1));

        // Class
        var class1 = new VariantTestClass { Int1 = 12, Float1 = -5.5f, String1 = "huhu" };
        v1 = (VariantObject)Variant.FromObject(class1);
        var class11 = v1.ToObject<VariantTestClass>();
        Assert.That(class11.Equals(class1));

        var class2 = new VariantTestBigClass();
        class2.Init();
        v1 = (VariantObject)Variant.FromObject(class2);
        var class21 = v1.ToObject<VariantTestBigClass>();
        Assert.That(class21.Equals(class2));
    }

    [Test]
    public void ConvertWithContractResolver_Success()
    {
        var class1 = new VariantTestContractResolverClass { Int1 = 12, Float1 = -5.5f, String1 = "huhu" };
        var v1 = (VariantObject)Variant.FromObject(class1);
        var class11 = v1.ToObject<VariantTestContractResolverClass>();
        Assert.That(class11.Equals(class1));
    }

    [Test]
    public void CheckAttributes_Success()
    {
        var class1 = new VariantTestAttributeClass
        {
            Int1 = 12, 
            Float1 = -5.5f, 
            String1 = "huhu",
            // Check that ignored property is excluded
            Ignored = "ignored"
        };

        var v1 = (VariantObject)Variant.FromObject(class1);
        Assert.That(v1.TryGetValue("ignored", out _), Is.False);

        // Check that ignored_if_null property is included if not null
        class1.IgnoredIfNull = "not ignored";
        v1 = (VariantObject)Variant.FromObject(class1);
        Assert.That(v1.TryGetValue("ignored_if_null", out _));

        // Check that ignored_if_null property is excluded if null
        class1.IgnoredIfNull = null;
        v1 = (VariantObject)Variant.FromObject(class1);
        Assert.That(v1.TryGetValue("ignored_if_null", out _), Is.False);

        // Check conversion fails if required property is missing 
        v1.Remove("string1");
        Assert.Throws<Exception>(() => v1.ToObject<VariantTestAttributeClass>());

        // Check conversion succeeds if required property is provided 
        v1["string1"] = (VariantValue)"huhu";
        Assert.DoesNotThrow(() => class1 = v1.ToObject<VariantTestAttributeClass>());
        Assert.That(class1.String1 == "huhu");

        // Check that alternative names can be used to identify property
        v1 = new VariantObject
        {
            {"i1", (VariantValue)10}, 
            { "f1", (VariantValue)10.5 }, 
            { "s1", (VariantValue)"hiho" }, 
            { "required", (VariantValue)"required" }
        };
        class1 = v1.ToObject<VariantTestAttributeClass>();
        Assert.That(class1.Int1 == 10);
        Assert.That(class1.Float1.EqualsWithPrecision(10.5f));
        Assert.That(class1.String1 == "hiho");
    }

    [Test]
    public void ConvertTypeNoSetter_Throws()
    {
        var v1 = new VariantObject
        {
            { "no_setter", (VariantValue)"no_setter" }
        };
        Assert.Throws<Exception>(() => Variant.ToObject<VariantTestNoSetterClass>(v1));
    }
}
