namespace ifm.Common.UnitTests;

using System;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class HexStringConverterTests
{
    [Test]
    public void ConvertByteArrayToHexStringNull_Fail()
    {
        Assert.Throws<ArgumentNullException>(() => HexStringConverter.ByteArrayToHexString(null));
    }

    [Test]
    public void ConvertHexStringToByteArrayNull_Fail()
    {
        Assert.Throws<ArgumentNullException>(() => HexStringConverter.HexStringToByteArray(null));
    }

    [Test]
    public void ConvertByteArrayToHexString_Success()
    {
        var str = HexStringConverter.ByteArrayToHexString([0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
        ]);

        Assert.That(str == "000102030405060708090A0B0C0D0E0F1011121314");
    }

    [Test]
    public void ConvertHexStringToByteArray_Success()
    {
        const string str = "000102030405060708090A0B0C0D0E0F1011121314";
        var bytes = HexStringConverter.HexStringToByteArray(str);

        Assert.That(bytes.SequenceEqual(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }));
    }

    [Test]
    public void ConvertHexStringToByteArrayOddCount_LastDigitDropped()
    {
        const string str = "00012";
        var bytes = HexStringConverter.HexStringToByteArray(str);

        Assert.That(bytes.SequenceEqual(new byte[] { 0, 1 }));
    }
}