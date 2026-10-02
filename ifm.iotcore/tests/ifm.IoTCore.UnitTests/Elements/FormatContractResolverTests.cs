namespace ifm.IoTCore.UnitTests.Elements
{
    using NUnit.Framework;
    using ifm.Common.Variant;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.Valuations;

    [TestFixture]
    internal class FormatContractResolverTests
    {
        [Test]
        public void TestArrayFormatBaseTypeString()
        {
            var variant = new VariantObject
            {
                {"type", Variant.FromObject("array")},
                { "valuation", new VariantObject
                    {
                        { "baseType", new VariantValue("string") },
                        { "format", new VariantObject
                            {
                                { "type", new VariantValue("string") },
                                { "valuation", new VariantObject
                                    {
                                        { "minlength", Variant.FromObject(1) },
                                        { "maxlength", Variant.FromObject(2) },
                                        { "pattern", Variant.FromObject(".*") }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var formatContractResolver = new FormatContractResolver();
            var result = formatContractResolver.CreateInstance(variant);

            Assert.That(result is ArrayFormat);
            Assert.That(((ArrayFormat) result).Valuation, Is.TypeOf(typeof(ArrayValuation)));
            Assert.That(((ArrayFormat)result).Valuation.Format, Is.TypeOf(typeof(StringFormat)));
            Assert.That(((StringFormat) ((ArrayFormat) result).Valuation.Format).Valuation.MinLength, Is.EqualTo(1));
            Assert.That(((StringFormat) ((ArrayFormat) result).Valuation.Format).Valuation.MaxLength, Is.EqualTo(2));
        }

        [Test]
        public void TestArrayFormatBaseTypeInteger()
        {
            var variant = new VariantObject
            {
                {"type", Variant.FromObject("array")},
                { "valuation", new VariantObject
                    {
                        { "baseType", new VariantValue("number") },
                        { "format", new VariantObject
                            {
                                { "type", new VariantValue("number") },
                                { "encoding", new VariantValue("integer") },
                                { "valuation", new VariantObject
                                    {
                                        { "min", Variant.FromObject(1) },
                                        { "max", Variant.FromObject(2) }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var formatContractResolver = new FormatContractResolver();
            var result = formatContractResolver.CreateInstance(variant);

            Assert.That(result is ArrayFormat);
            Assert.That(((ArrayFormat)result).Valuation, Is.TypeOf(typeof(ArrayValuation)));
            Assert.That(((ArrayFormat)result).Valuation.Format, Is.TypeOf(typeof(IntegerFormat)));
            Assert.That((int)((IntegerFormat)((ArrayFormat)result).Valuation.Format).Valuation.Min , Is.EqualTo(1));
            Assert.That((int)((IntegerFormat)((ArrayFormat)result).Valuation.Format).Valuation.Max, Is.EqualTo(2));
        }
    }
}
