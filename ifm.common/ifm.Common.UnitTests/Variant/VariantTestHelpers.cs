namespace ifm.Common.UnitTests.Variant;

using ifm.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Variant;

internal enum VariantTestEnum
{
    Black,
    Blue,
    Green,
    Yellow,
    Red,
    White
}

internal struct VariantTestStruct : IEquatable<VariantTestStruct>
{
    [VariantProperty("int1", Required = true, AlternativeNames = ["i1, in1"])]
    public int Int1 { get; set; }
    [VariantProperty("float1", Required = true, AlternativeNames = ["f1, fl1"])]
    public float Float1 { get; set; }
    [VariantProperty("string1", Required = true, AlternativeNames = ["s1, st1"])]
    public string String1 { get; set; }

    public bool Equals(VariantTestStruct other)
    {
        return Int1 == other.Int1 && Float1.EqualsWithPrecision(other.Float1) && String1 == other.String1;
    }

    public override bool Equals(object obj)
    {
        return obj is VariantTestStruct other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Int1, Float1, String1);
    }

    public static bool operator ==(VariantTestStruct left, VariantTestStruct right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariantTestStruct left, VariantTestStruct right)
    {
        return !left.Equals(right);
    }
}

internal class VariantTestClass : IEquatable<VariantTestClass>
{
    [VariantProperty("int1", Required = true, AlternativeNames = ["i1, in1"])]
    public int Int1 { get; set; }
    [VariantProperty("float1", Required = true, AlternativeNames = ["f1, fl1"])]
    public float Float1 { get; set; }
    [VariantProperty("string1", Required = true, AlternativeNames = ["s1, st1"])]
    public string String1 { get; set; }

    public bool Equals(VariantTestClass other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Int1 == other.Int1 && Float1.EqualsWithPrecision(other.Float1) && String1 == other.String1;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((VariantTestClass)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Int1, Float1, String1);
    }

    public static bool operator ==(VariantTestClass left, VariantTestClass right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(VariantTestClass left, VariantTestClass right)
    {
        return !Equals(left, right);
    }
}

internal class VariantTestAttributeClass
{
    [VariantProperty("int1", Required = true, AlternativeNames = ["i1", "in1"])]
    public int Int1 { get; set; }

    [VariantProperty("float1", Required = true, AlternativeNames = ["f1", "fl1"])]
    public float Float1 { get; set; }

    [VariantProperty("string1", Required = true, AlternativeNames = ["s1", "st1"])]
    public string String1 { get; set; }

    [VariantProperty("ignored", Ignored = true)]
    public string Ignored { get; set; }

    [VariantProperty("ignored_if_null", IgnoredIfNull = true)]
    public string IgnoredIfNull { get; set; }
}


internal class VariantTestNoSetterClass
{
    [VariantProperty("no_setter", Required = true)]
    public string NoSetter { get; }
}

internal class VariantTestContractResolver : IVariantContractResolver
{
    public object CreateInstance(Variant data)
    {
        if (data is VariantObject variantObject)
        {
            return new VariantTestContractResolverClass
            {
                Int1 = (int)(VariantValue)variantObject["int1"],
                Float1 = (float)(VariantValue)variantObject["float1"],
                String1 = (string)(VariantValue)variantObject["string1"]
            };
        }
        return null;
    }
}

[VariantContractResolver(typeof(VariantTestContractResolver))]
internal class VariantTestContractResolverClass : VariantTestClass
{
}

internal class VariantTestBigClass : IEquatable<VariantTestBigClass>
{
    [VariantProperty("int1")] 
    public int Int1 { get; set; }
    [VariantProperty("float1")] 
    public float Float1 { get; set; }
    [VariantProperty("string1")] 
    public string String1 { get; set; }
    [VariantProperty("struct1")]
    public VariantTestStruct Struct1 { get; set; }
    [VariantProperty("struct_array1")]
    public VariantTestStruct[] StructArray1 { get; set; }
    [VariantProperty("class1")] 
    public VariantTestClass Class1 { get; set; }
    [VariantProperty("class_array1")] 
    public VariantTestClass[] ClassArray1 { get; set; }
    [VariantProperty("class_list1")] 
    public List<VariantTestClass> ClassList1 { get; set; }
    [VariantProperty("string_class_dictionary1")]
    public Dictionary<string, VariantTestClass> StringClassDictionary1 { get; set; }
    [VariantProperty("struct_class_dictionary1")]
    public Dictionary<VariantTestStruct, VariantTestClass> StructClassDictionary1 { get; set; }

    public void Init()
    {
        Int1 = 12;
        Float1 = 123.6f;
        String1 = "huhu";
        Struct1 = new VariantTestStruct { Int1 = 12, Float1 = 12.2f, String1 = "huhu" };
        StructArray1 =
        [
            new VariantTestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
            new VariantTestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
            new VariantTestStruct { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
        ];
        Class1 = new VariantTestClass { Int1 = 12, Float1 = 12.2f, String1 = "huhu" };
        ClassArray1 =
        [
            new VariantTestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
            new VariantTestClass { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
            new VariantTestClass { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
        ];
        ClassList1 =
        [
            new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
            new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
            new() { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
        ];
        StringClassDictionary1 = new Dictionary<string, VariantTestClass>
        {
            { "1", new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" } },
            { "2", new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" } },
            { "3", new() { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" } }
        };
        StructClassDictionary1 = new Dictionary<VariantTestStruct, VariantTestClass>
        {
            { new VariantTestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" }, new VariantTestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" } },
            { new VariantTestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" }, new VariantTestClass { Int1 = 2, Float1 = 2.2f, String1 = "huhu" } },
            { new VariantTestStruct { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }, new VariantTestClass { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" } }
        };
    }

    public bool Equals(VariantTestBigClass other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Int1 == other.Int1 && 
               Float1.Equals(other.Float1) && 
               String1 == other.String1 && 
               Struct1.Equals(other.Struct1) && 
               StructArray1.SequenceEqual(other.StructArray1) && 
               Class1.Equals(other.Class1) && 
               ClassArray1.SequenceEqual(other.ClassArray1) && 
               ClassList1.SequenceEqual(other.ClassList1) && 
               StringClassDictionary1.SequenceEqual(other.StringClassDictionary1) && 
               StructClassDictionary1.SequenceEqual(other.StructClassDictionary1);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((VariantTestBigClass)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Int1);
        hashCode.Add(Float1);
        hashCode.Add(String1);
        hashCode.Add(Struct1);
        hashCode.Add(StructArray1);
        hashCode.Add(Class1);
        hashCode.Add(ClassArray1);
        hashCode.Add(ClassList1);
        hashCode.Add(StringClassDictionary1);
        hashCode.Add(StructClassDictionary1);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(VariantTestBigClass left, VariantTestBigClass right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(VariantTestBigClass left, VariantTestBigClass right)
    {
        return !Equals(left, right);
    }
}

internal class VariantTestGenericEnumerable : IEnumerable<string>
{
    private readonly IList<string> _list;

    public VariantTestGenericEnumerable()
    {
        _list = new List<string>();
    }

    public VariantTestGenericEnumerable(int capacity)
    {
        _list = new List<string>(capacity);
    }

    public void Add(string item)
    {
        _list.Add(item);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }
}