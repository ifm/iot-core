using System.Collections;

namespace ifmIoTCore.Common.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using NUnit.Framework;
    using Variant;

    public enum TestColorEnum
    {
        Black,
        Blue,
        Green,
        Yellow,
        Red,
        White
    }

    public struct TestStruct
    {
        [VariantProperty("int1")]
        public int Int1 { get; set; }
        [VariantProperty("float1")]
        public float Float1 { get; set; }
        [VariantProperty("string1")]
        public string String1 { get; set; }
    }

    public class TestClass : IEquatable<TestClass>
    {
        [VariantProperty("int1")]
        public int Int1 { get; set; }
        [VariantProperty("float1")]
        public float Float1 { get; set; }
        [VariantProperty("string1")]
        public string String1 { get; set; }

        public bool Equals(TestClass other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Int1 == other.Int1 &&
                   Float1.EqualsWithPrecision(other.Float1) &&
                   String1 == other.String1;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((TestClass)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Int1, Float1, String1);
        }
    }

    public class BigTestClass : IEquatable<BigTestClass>
    {
        [VariantProperty("int1")]
        public int Int1 { get; set; }
        [VariantProperty("float1")]
        public float Float1 { get; set; }
        [VariantProperty("string1")]
        public string String1 { get; set; }
        [VariantProperty("class1")]
        public TestClass Class1 { get; set; }
        [VariantProperty("class_array")]
        public TestClass[] ClassArray { get; set; }
        [VariantProperty("class_list")]
        public List<TestClass> ClassList { get; set; }
        [VariantProperty("string_class_dictionary")]
        public Dictionary<string, TestClass> StringClassDictionary { get; set; }
        [VariantProperty("struct_class_dictionary")]
        public Dictionary<TestStruct, TestClass> StructClassDictionary { get; set; }

        public BigTestClass()
        {
            Int1 = 12;
            Float1 = 123.6f;
            String1 = "hallohallo";
            Class1 = new TestClass { Int1 = 12, Float1 = 12.2f, String1 = "huhu" };
            ClassArray = new[]
            {
                new TestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new TestClass { Int1 = 2, Float1 = 2.2f, String1 = "huhu" }
            };
            ClassList = new List<TestClass>
            {
                new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
                new() { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" },
            };
            StringClassDictionary = new Dictionary<string, TestClass>
            {
                { "1", new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" } },
                { "2", new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" }},
                { "3", new() { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }}
            };
            StructClassDictionary = new Dictionary<TestStruct, TestClass>
            {
                {
                    new TestStruct {Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                    new TestClass {Int1 = 1, Float1 = 1.1f, String1 = "hu" }
                },
                {
                    new TestStruct {Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
                    new TestClass {Int1 = 2, Float1 = 2.2f, String1 = "huhu" }
                },
                {
                    new TestStruct {Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" },
                    new TestClass {Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
                }
            };
        }

        public bool Equals(BigTestClass other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Int1 == other.Int1 && 
                   Float1.EqualsWithPrecision(other.Float1) && 
                   String1 == other.String1 && 
                   Class1.Equals(other.Class1) && 
                   ClassArray.Equals(other.ClassArray) && 
                   ClassList.Equals(other.ClassList) &&
                   StringClassDictionary.Equals(other.StringClassDictionary) &&
                   StructClassDictionary.Equals(other.StructClassDictionary);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((BigTestClass)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Int1, Float1, String1, Class1, ClassArray, ClassList, StringClassDictionary, StructClassDictionary);
        }
    }


    public class TestStringList : IList<string>
    {
        private readonly IList<string> _list = new List<string>();

        public IEnumerator<string> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public void Add(string item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(string item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public int IndexOf(string item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public string this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }

    public class TestStructList : IList<TestStruct>
    {
        private readonly IList<TestStruct> _list = new List<TestStruct>();

        public IEnumerator<TestStruct> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public void Add(TestStruct item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(TestStruct item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(TestStruct[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TestStruct item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public int IndexOf(TestStruct item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TestStruct item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public TestStruct this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }

    public class TestStringStructDictionary : IDictionary<string, TestStruct>
    {
        private readonly IDictionary<string, TestStruct> _dictionary = new Dictionary<string, TestStruct>();

        public IEnumerator<KeyValuePair<string, TestStruct>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }

        public void Add(KeyValuePair<string, TestStruct> item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, TestStruct> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, TestStruct>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, TestStruct> item)
        {
            return _dictionary.Remove(item);
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public void Add(string key, TestStruct value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out TestStruct value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TestStruct this[string key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<TestStruct> Values => _dictionary.Values;
    }

    public class Variant_TypeConversions
    {
        [Test]
        public void VariantValue_ToFrom_ValueTypes()
        {
            Variant v;
            object o;

            bool bo = true;
            v = Variant.FromObject(bo);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Boolean);
            Assert.That((bool)(VariantValue)v == bo);
            o = Variant.ToObject<bool>(v);
            Assert.That((bool)o == bo);

            char ch = 'A';
            v = Variant.FromObject(ch);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Character);
            Assert.That((char)(VariantValue)v == ch);
            o = Variant.ToObject<char>(v);
            Assert.That((char)o == ch);

            sbyte sby = -127;
            v = Variant.FromObject(sby);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.SignedByte);
            Assert.That((sbyte)(VariantValue)v == sby);
            o = Variant.ToObject<sbyte>(v);
            Assert.That((sbyte)o == sby);

            byte by = 255;
            v = Variant.FromObject(by);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Byte);
            Assert.That((byte)(VariantValue)v == by);
            o = Variant.ToObject<byte>(v);
            Assert.That((byte)o == by);

            short sh = 32000;
            v = Variant.FromObject(sh);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Short);
            Assert.That((short)(VariantValue)v == sh);
            o = Variant.ToObject<short>(v);
            Assert.That((short)o == sh);

            ushort ush = 64000;
            v = Variant.FromObject(ush);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.UnsignedShort);
            Assert.That((ushort)(VariantValue)v == ush);
            o = Variant.ToObject<ushort>(v);
            Assert.That((ushort)o == ush);

            int i = -12345;
            v = Variant.FromObject(i);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Integer);
            Assert.That((int)(VariantValue)v == i);
            o = Variant.ToObject<int>(v);
            Assert.That((int)o == i);

            uint ui = 0xFFFFFFFFU;
            v = Variant.FromObject(ui);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.UnsignedInteger);
            Assert.That((uint)(VariantValue)v == ui);
            o = Variant.ToObject<uint>(v);
            Assert.That((uint)o == ui);

            long l = -321L;
            v = Variant.FromObject(l);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Long);
            Assert.That((long)(VariantValue)v == l);
            o = Variant.ToObject<long>(v);
            Assert.That((long)o == l);

            ulong ul = 123UL;
            v = Variant.FromObject(ul);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.UnsignedLong);
            Assert.That((ulong)(VariantValue)v == ul);
            o = Variant.ToObject<ulong>(v);
            Assert.That((ulong)o == ul);

            float fl = 4.6f;
            v = Variant.FromObject(fl);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Float);
            Assert.That(((float)(VariantValue)v).EqualsWithPrecision(fl));
            o = Variant.ToObject<float>(v);
            Assert.That(((float)o).EqualsWithPrecision(fl));

            double dou = 4.6;
            v = Variant.FromObject(dou);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Double);
            Assert.That(((double)(VariantValue)v).EqualsWithPrecision(dou));
            o = Variant.ToObject<double>(v);
            Assert.That(((double)o).EqualsWithPrecision(dou));

            decimal dec = 4.6M;
            v = Variant.FromObject(dec);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Decimal);
            Assert.That(((decimal)(VariantValue)v).EqualsWithPrecision(dec));
            o = Variant.ToObject<decimal>(v);
            Assert.That(((decimal)o).EqualsWithPrecision(dec));

            // String is a value type in variant
            string s = "huhu";
            v = Variant.FromObject(s);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.String);
            Assert.That((string)(VariantValue)v == s);
            o = Variant.ToObject<string>(v);
            Assert.That((string)o == s);

            // byte[] is a value type in variant
            byte[] baa = { 0x1, 0x2, 0x3, 0x4, 0x5, 0x65, 0x66, 0x67 };
            v = Variant.FromObject(baa);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Bytes);
            Assert.That(((byte[])(VariantValue)v).SequenceEqual(baa));
            o = Variant.ToObject<byte[]>(v);
            Assert.That(((byte[])o).SequenceEqual(baa));

            // enum is a value type in variant
            TestColorEnum e = TestColorEnum.Red;
            v = Variant.FromObject(e);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Integer);
            Assert.That((TestColorEnum)(int)(VariantValue)v == e);
            o = Variant.ToObject<int>(v);
            Assert.That((TestColorEnum)(int)o == e);

        }

        [Test]
        public void VariantValue_ToFrom_NullableValues()
        {
            Variant v;
            object o;

            // Nullable values ************************************************

            bool? nbo = true;
            v = Variant.FromObject(nbo);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Boolean);
            Assert.That((bool)(VariantValue)v == nbo);
            o = Variant.ToObject<bool>(v);
            Assert.That((bool)o == nbo);

            nbo = null;
            v = Variant.FromObject(nbo);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            char? nch = 'A';
            v = Variant.FromObject(nch);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Character);
            Assert.That((char)(VariantValue)v == nch);
            o = Variant.ToObject<char>(v);
            Assert.That((char)o == nch);

            nch = null;
            v = Variant.FromObject(nch);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            sbyte? nsby = -127;
            v = Variant.FromObject(nsby);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.SignedByte);
            Assert.That((sbyte)(VariantValue)v == nsby);
            o = Variant.ToObject<sbyte>(v);
            Assert.That((sbyte)o == nsby);

            nsby = null;
            v = Variant.FromObject(nsby);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            byte? nby = 255;
            v = Variant.FromObject(nby);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Byte);
            Assert.That((byte)(VariantValue)v == nby);
            o = Variant.ToObject<byte>(v);
            Assert.That((byte)o == nby);

            nby = null;
            v = Variant.FromObject(nby);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            short? nsh = 32000;
            v = Variant.FromObject(nsh);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Short);
            Assert.That((short)(VariantValue)v == nsh);
            o = Variant.ToObject<short>(v);
            Assert.That((short)o == nsh);

            nsh = null;
            v = Variant.FromObject(nsh);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            ushort? nush = 64000;
            v = Variant.FromObject(nush);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.UnsignedShort);
            Assert.That((ushort)(VariantValue)v == nush);
            o = Variant.ToObject<ushort>(v);
            Assert.That((ushort)o == nush);

            nush = null;
            v = Variant.FromObject(nush);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            int? ni = -12345;
            v = Variant.FromObject(ni);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Integer);
            Assert.That((int)(VariantValue)v == ni);
            o = Variant.ToObject<int>(v);
            Assert.That((int)o == ni);

            ni = null;
            v = Variant.FromObject(ni);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            uint? nui = 0xFFFFFFFFU;
            v = Variant.FromObject(nui);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.UnsignedInteger);
            Assert.That((uint)(VariantValue)v == nui);
            o = Variant.ToObject<uint>(v);
            Assert.That((uint)o == nui);

            nui = null;
            v = Variant.FromObject(nui);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            long? nl = -321L;
            v = Variant.FromObject(nl);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Long);
            Assert.That((long)(VariantValue)v == nl);
            o = Variant.ToObject<long>(v);
            Assert.That((long)o == nl);

            nl = null;
            v = Variant.FromObject(nl);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            ulong? nul = 123UL;
            v = Variant.FromObject(nul);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.UnsignedLong);
            Assert.That((ulong)(VariantValue)v == nul);
            o = Variant.ToObject<ulong>(v);
            Assert.That((ulong)o == nul);

            nul = null;
            v = Variant.FromObject(nul);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            float? nfl = 4.6f;
            v = Variant.FromObject(nfl);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Float);
            Assert.That(((float)(VariantValue)v).EqualsWithPrecision(nfl.Value));
            o = Variant.ToObject<float>(v);
            Assert.That(((float)o).EqualsWithPrecision(nfl.Value));

            nfl = null;
            v = Variant.FromObject(nfl);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            double? ndou = 4.6;
            v = Variant.FromObject(ndou);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Double);
            Assert.That(((double)(VariantValue)v).EqualsWithPrecision(ndou.Value));
            o = Variant.ToObject<double>(v);
            Assert.That(((double)o).EqualsWithPrecision(ndou.Value));

            ndou = null;
            v = Variant.FromObject(ndou);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            decimal? ndec = 4.6M;
            v = Variant.FromObject(ndec);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Decimal);
            Assert.That(((decimal)(VariantValue)v).EqualsWithPrecision(ndec.Value));
            o = Variant.ToObject<decimal>(v);
            Assert.That(((decimal)o).EqualsWithPrecision(ndec.Value));

            ndec = null;
            v = Variant.FromObject(ndec);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            string? ns = "huhu";
            v = Variant.FromObject(ns);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.String);
            Assert.That((string)(VariantValue)v == ns);
            o = Variant.ToObject<string>(v);
            Assert.That((string)o == ns);

            ns = null;
            v = Variant.FromObject(ns);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);

            TestColorEnum? ne = TestColorEnum.Red;
            v = Variant.FromObject(ne);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Integer);
            Assert.That((TestColorEnum)(int)(VariantValue)v == ne);
            o = Variant.ToObject<int>(v);
            Assert.That((TestColorEnum)(int)o == ne);

            ne = null;
            v = Variant.FromObject(ne);
            Assert.That(v is VariantValue);
            Assert.That(((VariantValue)v).Type == VariantValue.ValueType.Null);
        }

        [Test]
        public void Variant_ToFrom_Variant_ValuesArraysObjects()
        {
            Variant v;
            VariantValue vv;
            VariantArray va;
            VariantObject vo;
            object o;
            // Variants *******************************************************

            var v1 = new VariantValue(7);
            v = Variant.FromObject(new VariantValue(7));
            Assert.That(v.Equals(v1));

            var v2 = new VariantValue("huhu");
            v = Variant.FromObject(new VariantValue("huhu"));
            Assert.That(v.Equals(v2));

            // TODO VariantArray
            // TODO VariantObject
        }


        [Test]
        public void VariantObject_ToFrom_StructsClasses()
        {
            Variant v;
            VariantObject vo;
            object o;

            var testStruct = new TestStruct { Int1 = 12, Float1 = -5.5f, String1 = "suppenhuhn" };
            v = Variant.FromObject(testStruct);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 3);
            Assert.That((int)(VariantValue)vo["int1"] == 12);
            Assert.That(((float)(VariantValue)vo["float1"]).EqualsWithPrecision(-5.5f));
            Assert.That((string)(VariantValue)vo["string1"] == "suppenhuhn");
            o = Variant.ToObject<TestStruct>(v);
            Assert.That(((TestStruct)o).Equals(testStruct));

            var testClass = new TestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" };
            v = Variant.FromObject(testClass);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 3);
            Assert.That((int)(VariantValue)vo["int1"] == 1);
            Assert.That(((float)(VariantValue)vo["float1"]).EqualsWithPrecision(1.1f));
            Assert.That((string)(VariantValue)vo["string1"] == "hu");
            o = Variant.ToObject<TestClass>(v);
            Assert.That(((TestClass)o).Equals(testClass));

            var bigTestClass = new BigTestClass();
            v = Variant.FromObject(bigTestClass);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 8);
            o = Variant.ToObject<BigTestClass>(v);
            //Assert.That(((BigTestClass)o).Equals(bigTestClass));
        }

        [Test]
        public void VariantArray_ToFrom_ListsArrays()
        {
            Variant v;
            VariantArray va;
            object o;

            var intArray = new[] { 1, 2, 3, 4, 5 };
            v = Variant.FromObject(intArray);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 5);
            o = Variant.ToObject<int[]>(v);
            Assert.That(((int[])o).SequenceEqual(intArray));

            var intList = new List<int> { 1, 2, 3, 4, 5 };
            v = Variant.FromObject(intList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 5);
            o = Variant.ToObject<List<int>>(v);
            Assert.That(((List<int>)o).SequenceEqual(intList));

            var doubleArray = new[] { 1.1, 2.1, 3.1, 4.1, 5.1 };
            v = Variant.FromObject(doubleArray);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 5);
            o = Variant.ToObject<double[]>(v);
            Assert.That(((double[])o).SequenceEqual(doubleArray));

            var doubleList = new List<double> { 1.1, 2.1, 3.1, 4.1, 5.1 };
            v = Variant.FromObject(doubleList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 5);
            o = Variant.ToObject<List<double>>(v);
            Assert.That(((List<double>)o).SequenceEqual(doubleList));

            var stringArray = new[] { null, "hu", "huhu", "huhuhu", "huhuhuhu", "huhuhuhuhu" };
            v = Variant.FromObject(stringArray);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 6);
            o = Variant.ToObject<string[]>(v);
            Assert.That(((string[])o).SequenceEqual(stringArray));

            var stringList = new List<string> { "hu", null, "huhu", "huhuhu", "huhuhuhu", "huhuhuhuhu" };
            v = Variant.FromObject(stringList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 6);
            o = Variant.ToObject<List<string>>(v);
            Assert.That(((List<string>)o).SequenceEqual(stringList));

            var structArray = new[]
            {
                new TestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new TestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" }
            };
            v = Variant.FromObject(structArray);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 2);
            foreach (var item in va.GetItems())
            {
                Assert.That(item is VariantObject);
                var inner = (VariantObject)item;
                Assert.That(inner.Count == 3);
                // Check values
            }

            o = Variant.ToObject<TestStruct[]>(v);
            Assert.That(((TestStruct[])o).SequenceEqual(structArray));

            var structList = new List<TestStruct>
            {
                new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" }
            };
            v = Variant.FromObject(structList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 2);
            foreach (var item in va.GetItems())
            {
                Assert.That(item is VariantObject);
                var inner = (VariantObject)item;
                Assert.That(inner.Count == 3);
                // Check values
            }

            o = Variant.ToObject<List<TestStruct>>(v);
            Assert.That(((List<TestStruct>)o).SequenceEqual(structList));


            var classArray = new[]
            {
                new TestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new TestClass { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
                null
            };
            v = Variant.FromObject(classArray);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 3);
            o = Variant.ToObject<TestClass[]>(v);
            Assert.That(((TestClass[])o).SequenceEqual(classArray));

            var classList = new List<TestClass>
            {
                new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
                null
            };
            v = Variant.FromObject(classList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 3);
            o = Variant.ToObject<List<TestClass>>(v);
            Assert.That(((List<TestClass>)o).SequenceEqual(classList));

            var testStringList = new TestStringList
            {
                "hu", "huhu", "huhuhu"
            };
            v = Variant.FromObject(testStringList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 3);
            o = Variant.ToObject<TestStringList>(v);
            Assert.That(((TestStringList)o).SequenceEqual(testStringList));

            var testStructList = new TestStructList
            {
                new() { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                new() { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
                new() { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
            };
            v = Variant.FromObject(testStructList);
            Assert.That(v is VariantArray);
            va = (VariantArray)v;
            Assert.That(va.Count == 3);
            o = Variant.ToObject<TestStructList>(v);
            Assert.That(((TestStructList)o).SequenceEqual(testStructList));
        }


        [Test]
        public void VariantObject_ToFrom_Dictionaries()
        {
            Variant v;
            VariantObject vo;
            object o;

            var dic0 = new Dictionary<int, string>
            {
                { 1, "hu" },
                { 2, "huhu" },
                { 3, "huhuhu" },
                { 4, null }
            };
            v = Variant.FromObject(dic0);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 4);
            foreach (var item in vo.GetItems())
            {
                var key = (int)(VariantValue)item.Key;
                var value = (string)(VariantValue)item.Value;
                Assert.That(value == dic0[key]);
            }
            o = Variant.ToObject<Dictionary<int, string>>(v);
            Assert.That(((Dictionary<int, string>)o).SequenceEqual(dic0));

            var dic1 = new Dictionary<string, string>
            {
                { "1", "hu" },
                { "2", "huhu" },
                { "3", "huhuhu" },
                { "4", null }
            };
            v = Variant.FromObject(dic1);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 4);
            foreach (var item in vo.GetItems())
            {
                var key = (string)(VariantValue)item.Key;
                var value = (string)(VariantValue)item.Value;
                Assert.That(value == dic1[key]);
            }
            o = Variant.ToObject<Dictionary<string, string>>(v);
            Assert.That(((Dictionary<string, string>)o).SequenceEqual(dic1));

            var dic2 = new Dictionary<int, TestStruct>
            {
                { 1, new TestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" } },
                { 2, new TestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" } },
                { 3, new TestStruct { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" } }
            };
            v = Variant.FromObject(dic2);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 3);
            foreach (var item in vo.GetItems())
            {
                var key = (int)(VariantValue)item.Key;
                var value = Variant.ToObject<TestStruct>(item.Value);
                Assert.That(value.Equals(dic2[key]));
            }
            o = Variant.ToObject<Dictionary<int, TestStruct>>(v);
            Assert.That(((Dictionary<int, TestStruct>)o).SequenceEqual(dic2));

            var dic3 = new Dictionary<int, TestClass>
            {
                { 1, new TestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" } },
                { 2, new TestClass { Int1 = 2, Float1 = 2.2f, String1 = "huhu" } },
                { 3, new TestClass { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" } }
            };
            v = Variant.FromObject(dic3);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 3);
            foreach (var item in vo.GetItems())
            {
                var key = (int)(VariantValue)item.Key;
                var value = Variant.ToObject<TestClass>(item.Value);
                Assert.That(value.Equals(dic3[key]));
            }
            o = Variant.ToObject<Dictionary<int, TestClass>>(v);
            Assert.That(((Dictionary<int, TestClass>)o).SequenceEqual(dic3));

            var dic4 = new Dictionary<TestStruct, TestClass>
            {
                {
                    new TestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" },
                    new TestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" }
                },
                {
                    new TestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" },
                    new TestClass { Int1 = 2, Float1 = 2.2f, String1 = "huhu" }
                },
                {
                    new TestStruct { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" },
                    new TestClass { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" }
                }
            };
            v = Variant.FromObject(dic4);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 3);
            foreach (var item in vo.GetItems())
            {
                var key = Variant.ToObject<TestStruct>(item.Key);
                var value = Variant.ToObject<TestClass>(item.Value);
                Assert.That(value.Equals(dic4[key]));
            }
            o = Variant.ToObject<Dictionary<TestStruct, TestClass>>(v);
            Assert.That(((Dictionary<TestStruct, TestClass>)o).SequenceEqual(dic4));

            var dic5 = new TestStringStructDictionary
            {
                { "1", new TestStruct { Int1 = 1, Float1 = 1.1f, String1 = "hu" } },
                { "2", new TestStruct { Int1 = 2, Float1 = 2.2f, String1 = "huhu" } },
                { "3", new TestStruct { Int1 = 3, Float1 = 3.3f, String1 = "huhuhu" } }
            };
            v = Variant.FromObject(dic5);
            Assert.That(v is VariantObject);
            vo = (VariantObject)v;
            Assert.That(vo.Count == 3);
            o = Variant.ToObject<TestStringStructDictionary>(v);
            Assert.That(((TestStringStructDictionary)o).SequenceEqual(dic5));

            //var ssa = new[]
            //{
            //    new[] { "1.1", "1.2" }, 
            //    new[] { "2.1", "2.2", "2.3", "2.4" }, 
            //    new[] { "3.1", "3.2", "3.3" }
            //};
            //v = Variant.FromObject(ssa);
            //Assert.That(v is VariantObject);
            //va = (VariantArray)v;
            //Assert.That(va.Count == 3);
            //foreach (var item in va.GetItems())
            //{
            //    Assert.That(item is VariantArray);
            //    var sa = (VariantArray)item;
            //    foreach (var subItem in sa.GetItems())
            //    {
            //        Assert.That(subItem is VariantValue);
            //        // Check all values
            //    }
            //}
            //o = Variant.ToObject<string[][]>(v);
            //Assert.That(((string[][])o).SequenceEqual(ssa));
        }

        [Test]
        public void Variant_CastToValueTypes()
        {
            Variant v;

            v = new VariantValue("true");
            bool bo = (bool)(VariantValue)v;
            Assert.That(bo);

            v = new VariantValue("1234");
            int i = (int)(VariantValue)v;
            Assert.That(i == 1234);

            v = new VariantValue(1234);
            string s = (string)(VariantValue)v;
            Assert.That(s == "1234");

            v = new VariantValue("true");
            s = (string)(VariantValue)v;
            Assert.That(s == "true");
        }

        [Test]
        public void Variant_As_IntermediateType_BetweenConversions()
        {
            Variant v;
            var testStruct = new TestStruct { Int1 = 12, Float1 = -5.5f, String1 = "suppenhuhn" };
            v = Variant.FromObject(testStruct);
            TestClass testClass = Variant.ToObject<TestClass>(v);
            Assert.That(testStruct.Int1 == testClass.Int1);
            Assert.That(testStruct.Float1.EqualsWithPrecision(testClass.Float1));
            Assert.That(testStruct.String1 == testClass.String1);

            testClass = new TestClass { Int1 = 1, Float1 = 1.1f, String1 = "hu" };
            v = Variant.FromObject(testClass);
            testStruct = Variant.ToObject<TestStruct>(v);
            Assert.That(testClass.Int1 == testStruct.Int1);
            Assert.That(testClass.Float1.EqualsWithPrecision(testStruct.Float1));
            Assert.That(testClass.String1 == testStruct.String1);
        }
    }
}