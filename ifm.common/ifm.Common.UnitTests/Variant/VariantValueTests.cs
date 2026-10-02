namespace ifm.Common.UnitTests.Variant;

using System;
using ifm.Common.Variant;
using NUnit.Framework;

[TestFixture]
public class VariantValueTests
{
    [Test]
    public void ConvertBoolean_Success()
    {
        const bool val = true;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type, Is.EqualTo(VariantValue.ValueTypes.Boolean));

        // Explicit
        Assert.That((bool)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type, Is.EqualTo(VariantValue.ValueTypes.Boolean));
        Assert.That((bool)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Boolean);
        Assert.That((bool)v1, Is.EqualTo(val));

        var b1 = Variant.ToObject<bool>(v1);
        Assert.That(b1, Is.EqualTo(val));

        b1 = v1.ToObject<bool>();
        Assert.That(b1, Is.EqualTo(val));

        var b2 = Variant.ToObject<bool?>(v1);
        Assert.That(b2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertCharacter_Success()
    {
        const char val = 'A';

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Character);

        // Explicit
        Assert.That((char)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Character);
        Assert.That((char)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Character);
        Assert.That((char)v1, Is.EqualTo(val));

        var c1 = Variant.ToObject<char>(v1);
        Assert.That(c1, Is.EqualTo(val));

        var c2 = Variant.ToObject<char?>(v1);
        Assert.That(c2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertInt8_Success()
    {
        const sbyte val = 123;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int8);

        // Explicit
        Assert.That((sbyte)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Int8);
        Assert.That((sbyte)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int8);
        Assert.That((sbyte)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<sbyte>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<sbyte?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertUInt8_Success()
    {
        const byte val = 123;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt8);

        // Explicit
        Assert.That((byte)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt8);
        Assert.That((byte)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt8);
        Assert.That((byte)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<byte>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<byte?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertInt16_Success()
    {
        const short val = 1234;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int16);

        // Explicit
        Assert.That((short)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Int16);
        Assert.That((short)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int16);
        Assert.That((short)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<short>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<short?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertUInt16_Success()
    {
        const ushort val = 1234;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt16);

        // Explicit
        Assert.That((ushort)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt16);
        Assert.That((ushort)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt16);
        Assert.That((ushort)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<ushort>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<ushort?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertInt32_Success()
    {
        const int val = 1234;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int32);

        // Explicit
        Assert.That((int)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Int32);
        Assert.That((int)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int32);
        Assert.That((int)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<int>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<int?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertUInt32_Success()
    {
        const uint val = 1234;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt32);

        // Explicit
        Assert.That((uint)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt32);
        Assert.That((uint)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt32);
        Assert.That((uint)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<uint>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<uint?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertInt64_Success()
    {
        const long val = 1234L;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int64);

        // Explicit
        Assert.That((long)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Int64);
        Assert.That((long)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Int64);
        Assert.That((long)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<long>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<long?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertUInt64_Success()
    {
        const ulong val = 1234L;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt64);

        // Explicit
        Assert.That((ulong)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt64);
        Assert.That((ulong)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.UInt64);
        Assert.That((ulong)v1, Is.EqualTo(val));

        var i1 = Variant.ToObject<ulong>(v1);
        Assert.That(i1, Is.EqualTo(val));

        var i2 = Variant.ToObject<ulong?>(v1);
        Assert.That(i2, Is.EqualTo(val));
    }

    [Test]
    public void ConvertFloat_Success()
    {
        const float val = 12.34f;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Float);

        // Explicit
        Assert.That(((float)v1).EqualsWithPrecision(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Float);
        Assert.That(((float)v1).EqualsWithPrecision(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Float);
        Assert.That(((float)v1).EqualsWithPrecision(val));

        var f1 = Variant.ToObject<float>(v1);
        Assert.That(f1.EqualsWithPrecision(val));

        var f2 = Variant.ToObject<float?>(v1);
        Assert.That(f2 != null && f2.Value.EqualsWithPrecision(val));
    }

    [Test]
    public void ConvertDouble_Success()
    {
        const double val = 12.34;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Double);

        // Explicit
        Assert.That(((double)v1).EqualsWithPrecision(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Double);
        Assert.That(((double)v1).EqualsWithPrecision(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Double);
        Assert.That(((double)v1).EqualsWithPrecision(val));

        var f1 = Variant.ToObject<double>(v1);
        Assert.That(f1.EqualsWithPrecision(12.34));

        var f2 = Variant.ToObject<double?>(v1);
        Assert.That(f2 != null && f2.Value.EqualsWithPrecision(val));
    }

    [Test]
    public void ConvertDecimal_Success()
    {
        const decimal val = 12.34m;

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Decimal);

        // Explicit
        Assert.That(((decimal)v1).EqualsWithPrecision(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Decimal);
        Assert.That(((decimal)v1).EqualsWithPrecision(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Decimal);
        Assert.That(((decimal)v1).EqualsWithPrecision(val));

        var f1 = Variant.ToObject<decimal>(v1);
        Assert.That(f1.EqualsWithPrecision(val));

        var f2 = Variant.ToObject<decimal?>(v1);
        Assert.That(f2 != null && f2.Value.EqualsWithPrecision(val));
    }

    [Test]
    public void ConvertString_Success()
    {
        const string val = "huhu";

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.String);

        // Explicit
        Assert.That((string)v1, Is.EqualTo(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.String);
        Assert.That((string)v1, Is.EqualTo(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.String);
        Assert.That((string)v1, Is.EqualTo(val));

        var s1 = Variant.ToObject<string>(v1);
        Assert.That(s1, Is.EqualTo(val));
    }

    [Test]
    public void ConvertDateTime_Success()
    {
        var val = new DateTime(2023, 2, 7, 14, 22, 30);

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.DateTime);

        // Explicit
        Assert.That(((DateTime)v1).Equals(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.DateTime);
        Assert.That(((DateTime)v1).Equals(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.DateTime);
        Assert.That(((DateTime)v1).Equals(val));

        var date1 = Variant.ToObject<DateTime>(v1);
        Assert.That(date1.Equals(val));

        var date2 = Variant.ToObject<DateTime?>(v1);
        Assert.That(date2 != null && date2.Equals(val));
    }

    [Test]
    public void ConvertTimeSpan_Success()
    {
        var val = new TimeSpan(1000);

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.TimeSpan);

        // Explicit
        Assert.That(((TimeSpan)v1).Equals(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.TimeSpan);
        Assert.That(((TimeSpan)v1).Equals(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.TimeSpan);
        Assert.That(((TimeSpan)v1).Equals(val));

        var ts1 = Variant.ToObject<TimeSpan>(v1);
        Assert.That(ts1.Equals(val));

        var ts2 = Variant.ToObject<TimeSpan?>(v1);
        Assert.That(ts2 != null && ts2.Equals(val));
    }

    [Test]
    public void ConvertUri_Success()
    {
        var val = new Uri("http://127.0.0.1:8000");

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Uri);

        // Explicit
        Assert.That(((Uri)v1).Equals(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Uri);
        Assert.That(((Uri)v1).Equals(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Uri);
        Assert.That(((Uri)v1).Equals(val));

        var uri1 = Variant.ToObject<Uri>(v1);
        Assert.That(uri1.Equals(val));
    }

    [Test]
    public void ConvertGuid_Success()
    {
        var val = Guid.NewGuid();

        // Ctor
        var v1 = new VariantValue(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Guid);

        // Explicit
        Assert.That(((Guid)v1).Equals(val));

        // Implicit
        v1 = val;
        Assert.That(v1.Type == VariantValue.ValueTypes.Guid);
        Assert.That(((Guid)v1).Equals(val));

        // Methods
        v1 = (VariantValue)Variant.FromObject(val);
        Assert.That(v1.Type == VariantValue.ValueTypes.Guid);
        Assert.That(((Guid)v1).Equals(val));

        var guid1 = Variant.ToObject<Guid>(v1);
        Assert.That(guid1.Equals(val));

        var guid2 = Variant.ToObject<Guid?>(v1);
        Assert.That(guid2 != null && guid2.Equals(val));
    }

    [Test]
    public void CopyConstructor_Success()
    {
        var v1 = new VariantValue(123);
        var v2 = new VariantValue(v1);

        Assert.That(v1.Type == v2.Type);
        Assert.That((int)v1 == (int)v2);
    }

    [Test]
    public void ToString_Success()
    {
        var v1 = new VariantValue("huhuhu");
        var s = v1.ToString();
        Assert.That(s == "huhuhu");

        v1 = new VariantValue(-1234);
        s = v1.ToString();
        Assert.That(s == "-1234");
    }

    [Test]
    public void Equals_Success()
    {
        var v1 = new VariantValue(true);
        var v2 = new VariantValue(false);
        var v3 = new VariantValue(true);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue('A');
        v2 = new VariantValue('B');
        v3 = new VariantValue('A');

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((sbyte)127);
        v2 = new VariantValue((sbyte)-127);
        v3 = new VariantValue((sbyte)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((byte)127);
        v2 = new VariantValue((byte)255);
        v3 = new VariantValue((byte)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((short)127);
        v2 = new VariantValue((short)-127);
        v3 = new VariantValue((short)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((ushort)127);
        v2 = new VariantValue((ushort)255);
        v3 = new VariantValue((ushort)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(127);
        v2 = new VariantValue(-127);
        v3 = new VariantValue(127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((uint)127);
        v2 = new VariantValue((uint)255);
        v3 = new VariantValue((uint)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((long)127);
        v2 = new VariantValue((long)-127);
        v3 = new VariantValue((long)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue((ulong)127);
        v2 = new VariantValue((ulong)255);
        v3 = new VariantValue((ulong)127);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(1.27f);
        v2 = new VariantValue(-1.27f);
        v3 = new VariantValue(1.27f);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(1.27);
        v2 = new VariantValue(-1.27);
        v3 = new VariantValue(1.27);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(1.27m);
        v2 = new VariantValue(-1.27m);
        v3 = new VariantValue(1.27m);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue("huhu");
        v2 = new VariantValue("agaga");
        v3 = new VariantValue("huhu");

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(new DateTime(2000, 1, 1, 0, 0, 0));
        v2 = new VariantValue(new DateTime(1999, 12, 31, 23, 59, 59));
        v3 = new VariantValue(new DateTime(2000, 1, 1, 0, 0, 0));

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(new TimeSpan(1000));
        v2 = new VariantValue(new TimeSpan(500));
        v3 = new VariantValue(new TimeSpan(1000));

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        v1 = new VariantValue(new Uri("http://127.0.0.1:8000"));
        v2 = new VariantValue(new Uri("http://0.0.0.0:80"));
        v3 = new VariantValue(new Uri("http://127.0.0.1:8000"));

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        v1 = new VariantValue(guid1);
        v2 = new VariantValue(guid2);
        v3 = new VariantValue(guid1);

        Assert.That(v1.Equals(v2),Is.False);
        Assert.That(v1.Equals(v3));

        Assert.That(v1.Equals((object)null),Is.False);
        Assert.That(v1.Equals((object)v1));
        Assert.That(v1.Equals(new object()),Is.False);

        Assert.That(v1.Equals(null),Is.False);
        Assert.That(v1.Equals(v1));
    }

    [Test]
    public void GetHashCode_Success()
    {
        var v1 = new VariantValue(127);
        var v2 = new VariantValue(-127);
        var v3 = new VariantValue(127);

        Assert.That(v1.GetHashCode() != v2.GetHashCode());
        Assert.That(v1.GetHashCode() == v3.GetHashCode());

        v1 = new VariantValue("huhu");
        v2 = new VariantValue("agagaga");
        v3 = new VariantValue("huhu");

        Assert.That(v1.GetHashCode() != v2.GetHashCode());
        Assert.That(v1.GetHashCode() == v3.GetHashCode());
    }

    [Test]
    public void AsVariantValue_Success()
    {
        Variant v = new VariantValue("hu");
        Assert.That(v.AsVariantValue().GetType() == typeof(VariantValue));
    }
}