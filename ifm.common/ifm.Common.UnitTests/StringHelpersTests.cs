namespace ifm.Common.UnitTests;

using System;
using NUnit.Framework;

[TestFixture]
public class StringHelperTests
{
    [Test]
    public void EndsWith_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.EndsWith(null, '\0'));

        Assert.Throws<ArgumentNullException>(() => StringHelpers.EndsWith(string.Empty, '\0'));
    }

    [Test]
    public void EndsWith_Success()
    {
        var s = "HuHu";
        Assert.That(StringHelpers.EndsWith(s, 'u'));
        Assert.That(StringHelpers.EndsWith(s, 'z'), Is.False);
    }


    [Test]
    public void StartsWith_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.StartsWith(null, '\0'));

        Assert.Throws<ArgumentNullException>(() => StringHelpers.StartsWith(string.Empty, '\0'));
    }

    [Test]
    public void StartsWith_Success()
    {
        var s = "HuHu";
        Assert.That(StringHelpers.StartsWith(s, 'H'));
        Assert.That(StringHelpers.StartsWith(s, 'h'), Is.False);
    }

    [Test]
    public void LeftWithChar_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.Left(null, '\0'));

        Assert.Throws<ArgumentOutOfRangeException>(() => "1,2,3,4,5".Left('\0', 10));
    }

    [Test]
    public void LeftWithChar_Success()
    {
        var s = "1,2,3,4,5";
        var left = s.Left(',');
        Assert.That(left == "1");

        left = s.Left(',', 0, true);
        Assert.That(left == "1,");

        left = s.Left(',', 4);
        Assert.That(left == "1,2,3");

        left = s.Left(',', 4, true);
        Assert.That(left == "1,2,3,");

        left = s.Left(',', 8);
        Assert.That(left == null);

        left = s.Left(',', 8, true);
        Assert.That(left == null);
    }


    [Test]
    public void LeftWithString_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.Left(null, ""));

        Assert.Throws<ArgumentOutOfRangeException>(() => "1,2,3,4,5".Left("", 10));
    }

    [Test]
    public void LeftWithString_Success()
    {
        var s = "1,2,3,4,5";
        var left = s.Left(",");
        Assert.That(left == "1");

        left = s.Left(",", 0, true);
        Assert.That(left == "1,");

        left = s.Left(",", 4);
        Assert.That(left == "1,2,3");

        left = s.Left(",", 4, true);
        Assert.That(left == "1,2,3,");

        left = s.Left(",", 8);
        Assert.That(left == null);

        left = s.Left(",", 8, true);
        Assert.That(left == null);
    }

    [Test]
    public void RightWithChar_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.Right(null, '\0'));

        Assert.Throws<ArgumentOutOfRangeException>(() => "1,2,3,4,5".Right('\0', 10));
    }

    [Test]
    public void RightWithChar_Success()
    {
        var s = "1,2,3,4,5";
        var right = s.Right(',');
        Assert.That(right == "2,3,4,5");

        right = s.Right(',', 0, true);
        Assert.That(right == ",2,3,4,5");

        right = s.Right(',', 4);
        Assert.That(right == "4,5");

        right = s.Right(',', 4, true);
        Assert.That(right == ",4,5");

        right = s.Right(',', 8);
        Assert.That(right == null);

        right = s.Right(',', 8, true);
        Assert.That(right == null);
    }


    [Test]
    public void RightWithString_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.Right(null, ""));

        Assert.Throws<ArgumentOutOfRangeException>(() => "1,2,3,4,5".Right("", 10));
    }

    [Test]
    public void RightWithString_Success()
    {
        var s = "1,2,3,4,5";
        var right = s.Right(",");
        Assert.That(right == "2,3,4,5");

        right = s.Right(",", 0, true);
        Assert.That(right == ",2,3,4,5");

        right = s.Right(",", 4);
        Assert.That(right == "4,5");

        right = s.Right(",", 4, true);
        Assert.That(right == ",4,5");

        right = s.Right(",", 8);
        Assert.That(right == null);

        right = s.Right(",", 8, true);
        Assert.That(right == null);
    }

    [Test]
    public void GetFirstToken_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.GetFirstToken(null, '\0'));
    }

    [Test]
    public void GetFirstToken_Success()
    {
        var s = "1,2,3,4,5";

        var token = s.GetFirstToken(',');
        Assert.That(token == "1");

        token = s.GetFirstToken(',', true);
        Assert.That(token == "1,");

        token = s.GetFirstToken('/');
        Assert.That(token == "1,2,3,4,5");
    }

    [Test]
    public void GetLastToken_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.GetLastToken(null, '\0'));
    }

    [Test]
    public void GetLastToken_Success()
    {
        var s = "1,2,3,4,5";

        var token = s.GetLastToken(',');
        Assert.That(token == "5");

        token = s.GetLastToken(',', true);
        Assert.That(token == ",5");

        token = s.GetLastToken('/');
        Assert.That(token == "1,2,3,4,5");
    }

    [Test]
    public void RemoveFirstToken_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.RemoveFirstToken(null, '\0'));
    }

    [Test]
    public void RemoveFirstToken_Success()
    {
        var s = "1,2,3,4,5";

        var remainder = s.RemoveFirstToken(',');
        Assert.That(remainder == "2,3,4,5");

        remainder = s.RemoveFirstToken(',', true);
        Assert.That(remainder == ",2,3,4,5");

        remainder = s.RemoveFirstToken('/');
        Assert.That(remainder == string.Empty);
    }

    [Test]
    public void RemoveLastToken_InvalidParameter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => StringHelpers.RemoveLastToken(null, '\0'));
    }

    [Test]
    public void RemoveLastToken_Success()
    {
        var s = "1,2,3,4,5";

        var remainder = s.RemoveLastToken(',');
        Assert.That(remainder == "1,2,3,4");

        remainder = s.RemoveLastToken(',', true);
        Assert.That(remainder == "1,2,3,4,");

        remainder = s.RemoveLastToken('/');
        Assert.That(remainder == string.Empty);
    }

    [Test]
    public void GetUtf8String_Success()
    {
        var bytes = new byte[] {65, 66, 67, 195, 182, 195, 188, 88, 89, 90};

        var s = bytes.GetUtf8String();

        Assert.That(s == "ABCöüXYZ");
    }
}