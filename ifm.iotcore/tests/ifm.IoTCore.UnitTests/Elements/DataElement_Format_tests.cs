namespace ifm.IoTCore.UnitTests.Elements
{
    using System.Collections.Generic;
    using Contracts;
    using ElementManager.Contracts;
    using ElementManager.Contracts.Elements;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.Valuations;
    using NUnit.Framework;

    [TestFixture]
    class DataElement_Format_tests
    {
        private IIoTCore _iotCore;
        private IElementManager _elementManager;

        [SetUp]
        public void Setup()
        {
            _iotCore = Helpers.CreateIoTCore("test");
            _elementManager = _iotCore.ElementManager;
        }

        [TearDown]
        public void TearDown()
        {
            _elementManager = null;
        }

        [Test, Property("TestCaseKey","IOTCS-T10")] 
        public void Format_types_Not_Mandatory()
        {// this test was made NOT mandatory for embedded devices behavior 
            Assert.DoesNotThrow(() =>
            {
                IDataElement testElement = _iotCore.ElementManager.CreateSimpleDataElement<object>(null, "data0"); // Format Base constructor exposed or does not do validation 
            });

        }

        [Test, Property("TestCaseKey", "IOTCS-T10")]
        public void Format_types_Basic_CanBeCreated()
        {
            Assert.Multiple(() =>
            {
                // boolean, number, string
                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "booleanFormat", format: new BooleanFormat()).Format.Type,
                    Is.EqualTo(Format.Types.Boolean));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "IntegerFormat", format: new Int32Format()).Format.Type,
                    Is.EqualTo(Format.Types.Number));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "FloatFormat", format: new FloatFormat()).Format.Type,
                    Is.EqualTo(Format.Types.Number));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "data0", format: new StringFormat()).Format.Type,
                    Is.EqualTo(Format.Types.String));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T10")]
        public void Format_types_Complex_CanBeCreated()
        {
            Assert.Multiple(() =>
            {
                // array, enum, object 
                Assert.That(

                    _elementManager.CreateSimpleDataElement<object>(null, "arrayFormat", 
                        format: new ArrayFormat(new ArrayValuation(new Int32Format(new Int32Valuation(0, 100))))).Format.Type,
                    Is.EqualTo(Format.Types.Array));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "integerEnumFormat",
                        format: new IntegerEnumFormat(
                            new IntegerEnumValuation(new Dictionary<string, string> {
                                    { "firstyear", "1970" },
                                    { "secondyear", "1971" }
                                }))
                            ).Format.Type,
                    Is.EqualTo(Format.Types.Enum));

                var intField = new ObjectValuation.Field("intField1", new Int32Format(new Int32Valuation(-100, 100)));
                var floatField = new ObjectValuation.Field("floatField1", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)));
                var stringField = new ObjectValuation.Field("stringField1", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "object1",
                        format: new ObjectFormat(new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField }))
                        ).Format.Type,
                    Is.EqualTo(Format.Types.Object));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T12")]
        public void Format_encodings_CanBeUsed()
        {
            Assert.Multiple(() =>
            {
                // number, string
                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "IntegerFormat",
                        format: new Int32Format(ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Int32));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "FloatFormat",
                        format: new FloatFormat(ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Float));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "utf8Stringencoding",
                        format: new StringFormat(ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Utf8));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "stringEncoding",
                        format: new StringFormat(encoding: Format.Encodings.Utf8, ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Utf8));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "stringEncoding",
                        format: new StringFormat(encoding: Format.Encodings.Ascii, ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Ascii));

                Assert.That(
                    _elementManager.CreateSimpleDataElement<object>(null, "hexstringEncoding",
                        format: new StringFormat(encoding: Format.Encodings.HexString, ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.HexString));

            });
        }

        [Test, Property("TestCaseKey","IOTCS-T11")] 
        public void Format_namespace_can_be_ignored_AtCreation()
        {
            IDataElement de = _elementManager.CreateSimpleDataElement<object>(null, "data0", 
                format: new Int32Format()); 
            Assert.That(de.Format.Namespace, Is.EqualTo(null));
        }

        [Test, Property("TestCaseKey","IOTCS-T11")] 
        public void Format_namespace_defaultsToJson()
        {
            Assert.Multiple(() => { 

                // expected: null or empty namespace defaults to null
                IDataElement de = _elementManager.CreateSimpleDataElement<object>(null, "data0", 
                    format: new Int32Format());
                Assert.That(de.Format.Namespace, Is.Null);

                IDataElement de2 = _elementManager.CreateSimpleDataElement<object>(null, "data0",
                    format: new StringFormat());
                Assert.That(de2.Format.Namespace, Is.Null);

                //// OR expected: null or empty namespace raises an exception
                //Assert.Throws<Exception>(() => {
                //    new DataElement("data0", getDataFunc:null, 
                //        format:new Format(type:"number", encoding:"integer", valuation:null, ns:null ));
                //});
                //Assert.Throws<Exception>(() => {
                //    new DataElement("data0", getDataFunc: null,
                //        format: new IntegerFormat(new IntegerValuation(0), ns: ""));
                //});
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T11")]
        public void Format_namespace_any_string_accepted()
        {
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    _elementManager.CreateSimpleDataElement<object>(null, "data0",
                        format: new Int32Format());
                });

                Assert.DoesNotThrow(() =>
                {
                    _elementManager.CreateSimpleDataElement<object>(null, "data0",
                        format: new Int32Format(ns: "iolink"));
                });

                Assert.DoesNotThrow(() =>
                {
                    var de1 = _elementManager.CreateSimpleDataElement<object>(null, "data0",
                        format: new Int32Format(ns: "undefined-namespace-1"));
                    Assert.That(de1.Format.Namespace, Is.EqualTo("undefined-namespace-1"));
                });
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_ObjectValuation()
        {
            var intField = new ObjectValuation.Field("intField", new Int32Format(new Int32Valuation(-100, 100)));
            var floatField = new ObjectValuation.Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)),optional:true);
            var stringField = new ObjectValuation.Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var valn = new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField });
            var de1 = _elementManager.CreateSimpleDataElement<object>(null, "valcheck", format: new ObjectFormat(valn));
            var objvn = ((ObjectFormat)de1.Format).Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(objvn, Is.InstanceOf(typeof(ObjectValuation)));
                Assert.That(objvn.Fields.Count, Is.EqualTo(3));

                Assert.That(objvn.Fields[0].Name, Is.EqualTo("intField"));
                Assert.That(objvn.Fields[1].Name, Is.EqualTo("floatField"));
                Assert.That(objvn.Fields[2].Name, Is.EqualTo("stringField"));

                Assert.That(objvn.Fields[0].Optional, Is.Null);
                Assert.That(objvn.Fields[1].Optional, Is.EqualTo(true));
                Assert.That(objvn.Fields[2].Optional, Is.Null);

                Assert.That(objvn.Fields[0].Format.Encoding, Is.EqualTo(Format.Encodings.Int32));
            });
        }
    }
}
