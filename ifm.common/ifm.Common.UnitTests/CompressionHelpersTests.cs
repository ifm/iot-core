namespace ifm.Common.UnitTests;

using System.IO.Compression;
using NUnit.Framework;

[TestFixture]
public class CompressionHelpersTests
{
    private static readonly byte[] UnCompressed = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
    
    private static readonly byte[] GZipOptimalCompression =
    [
        0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0a, 0x63, 0x64, 0x62, 0x66, 0x61, 0x65, 0x03, 0x00,
        0x24, 0x77, 0xf6, 0x81, 0x06, 0x00, 0x00, 0x00
    ];

    private static readonly byte[] GZipFastestCompression =
    [
        0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x0a, 0x63, 0x64, 0x62, 0x66, 0x61, 0x65, 0x03, 0x00,
        0x24, 0x77, 0xf6, 0x81, 0x06, 0x00, 0x00, 0x00
    ];

    private static readonly byte[] GZipNoCompression =
    [
        0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x0a, 0x01, 0x06, 0x00, 0xf9, 0xff, 0x01, 0x02, 0x03,
        0x04, 0x05, 0x06, 0x24, 0x77, 0xf6, 0x81, 0x06, 0x00, 0x00, 0x00
    ];
    
    private static readonly byte[] GZipSmallestSizeCompression =
    [
        0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x0a, 0x63, 0x64, 0x62, 0x66, 0x61, 0x65, 0x03, 0x00,
        0x24, 0x77, 0xf6, 0x81, 0x06, 0x00, 0x00, 0x00
    ];

    private static readonly byte[] ZipCompression =
    [
        0x50, 0x4b, 0x03, 0x04, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2f, 0x77, 0x54, 0x56, 0x24, 0x77, 0xf6, 0x81,
        0x06, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x62, 0x6c, 0x75, 0x62, 0x62, 0x65,
        0x72, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x50, 0x4b, 0x01, 0x02, 0x3f, 0x00, 0x0a, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x2f, 0x77, 0x54, 0x56, 0x24, 0x77, 0xf6, 0x81, 0x06, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x07,
        0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x62,
        0x6c, 0x75, 0x62, 0x62, 0x65, 0x72, 0x0a, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x18, 0x00,
        0x91, 0x23, 0x04, 0x45, 0x33, 0x45, 0xd9, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50, 0x4b, 0x05, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00,
        0x59, 0x00, 0x00, 0x00, 0x2b, 0x00, 0x00, 0x00, 0x00, 0x00
    ];

    [Test]
    public void InValidGZipFileLength()
    {
        System.IO.File.WriteAllBytes("UnCompressed.bin", UnCompressed);
        Assert.That(() =>
        {
            CompressionHelpers.IsGZipFormat([0x1f]);
        }, Throws.InstanceOf<System.ArgumentException>());
    }

    [Test]
    public void InValidZipFileLength()
    {
        Assert.That(() =>
        {
            CompressionHelpers.IsZipFormat([0x50, 0x4b, 0x03]);
        }, Throws.InstanceOf<System.ArgumentException>());
    }

    [Test]
    public void IsValidGZipFormat()
    {
        if (!CompressionHelpers.IsGZipFormat(GZipOptimalCompression))
        {
            Assert.Fail("The given value is not a valid GZip format.");
        }
    }

    [Test]
    public void IsInValidGZipFormat_String()
    {
        if (CompressionHelpers.IsGZipFormat(UnCompressed))
        {
            Assert.Fail("The given data is not an invalid GZip format.");
        }
    }

    [Test]
    public void IsInValidGZipFormat_Zip()
    {
        if (CompressionHelpers.IsGZipFormat(ZipCompression))
        {
            Assert.Fail("The given data is not an invalid GZip format.");
        }
    }

    [Test]
    public void IsValidZipFormat()
    {
        if (!CompressionHelpers.IsZipFormat(ZipCompression))
        {
            Assert.Fail("The given value is not a valid Zip format.");
        }
    }

    [Test]
    public void IsInValidZipFormat_GZip()
    {
        if (CompressionHelpers.IsZipFormat(GZipOptimalCompression))
        {
            Assert.Fail("The given data is not an invalid Zip format.");
        }
    }

    [Test]
    public void IsInValidZipFormat_String()
    {
        if (CompressionHelpers.IsZipFormat(UnCompressed))
        {
            Assert.Fail("The given data is not an invalid Zip format.");
        }
    }
    
    [Test]
    [TestCase(CompressionLevel.Optimal)]
    [TestCase(CompressionLevel.Fastest)]
    [TestCase(CompressionLevel.SmallestSize)]
    [TestCase(CompressionLevel.NoCompression)]
    public void GZipDeCompress(CompressionLevel level)
    {
        switch (level)
        {
            case CompressionLevel.Optimal:
            {
                var decompressed = CompressionHelpers.GZipDecompress(GZipOptimalCompression);
                Assert.That(UnCompressed, Is.EqualTo(decompressed));
                break;
            }
            case CompressionLevel.Fastest:
            {
                var decompressed = CompressionHelpers.GZipDecompress(GZipFastestCompression);
                Assert.That(UnCompressed, Is.EqualTo(decompressed));
                break;
            }
            case CompressionLevel.SmallestSize:
            {
                var decompressed = CompressionHelpers.GZipDecompress(GZipSmallestSizeCompression);
                Assert.That(UnCompressed, Is.EqualTo(decompressed));
                break;
            }
            case CompressionLevel.NoCompression:
            {
                var decompressed = CompressionHelpers.GZipDecompress(GZipNoCompression);
                Assert.That(UnCompressed, Is.EqualTo(decompressed));
                break;
            }
            default:
            {
                Assert.Fail("InValid compression level");
                break;
            }
        }
    }

    [Test]
    [TestCase(CompressionLevel.Optimal)]
    [TestCase(CompressionLevel.Fastest)]
    [TestCase(CompressionLevel.SmallestSize)]
    [TestCase(CompressionLevel.NoCompression)]
    public void GZipCompress(CompressionLevel level)
    {
        switch (level)
        {
            case CompressionLevel.Optimal:
            {
                var compressed = CompressionHelpers.GZipCompress(UnCompressed);
                var uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                compressed = CompressionHelpers.GZipCompress(UnCompressed);
                uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                break;
            }
            case CompressionLevel.Fastest:
            {
                var compressed = CompressionHelpers.GZipCompress(UnCompressed, CompressionLevel.Fastest);
                var uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                compressed = CompressionHelpers.GZipCompress(UnCompressed, CompressionLevel.Fastest);
                uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                break;
            }
            case CompressionLevel.SmallestSize:
            {
                var compressed = CompressionHelpers.GZipCompress(UnCompressed, CompressionLevel.SmallestSize);
                var uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                compressed = CompressionHelpers.GZipCompress(UnCompressed, CompressionLevel.SmallestSize);
                uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                break;
            }
            case CompressionLevel.NoCompression:
            {
                var compressed = CompressionHelpers.GZipCompress(UnCompressed, CompressionLevel.NoCompression);
                var uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                compressed = CompressionHelpers.GZipCompress(UnCompressed, CompressionLevel.NoCompression);
                uncompressed = CompressionHelpers.GZipDecompress(compressed);
                Assert.That(UnCompressed, Is.EqualTo(uncompressed));

                break;
            }
            default:
                Assert.Fail("InValid compression level");
                break;
        }
    }
}