namespace ifm.Common.UnitTests;

using NUnit.Framework;

[TestFixture]
public class NumericHelpersTests
{
    [Test]
    public void EqualsWithPrecision_Success()
    {
        const float f1 = 1.2345f;
        Assert.That(f1.EqualsWithPrecision(1.234f));
        Assert.That(f1.EqualsWithPrecision(1.234f, 0.0001f), Is.Not.True);


        const double d1 = 1.2345;
        Assert.That(d1.EqualsWithPrecision(1.234));
        Assert.That(d1.EqualsWithPrecision(1.234, 0.0001), Is.Not.True);

        const decimal dec1 = 1.2345m;
        Assert.That(dec1.EqualsWithPrecision(1.234m));
        Assert.That(dec1.EqualsWithPrecision(1.234m, 0.0001m), Is.Not.True);
    }
}