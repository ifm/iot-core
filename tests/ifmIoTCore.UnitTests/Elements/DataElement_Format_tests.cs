namespace ifmIoTCore.UnitTests.Elements
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;

    [TestFixture]
    class DataElement_Format_tests
    {
        [Test, Property("TestCaseKey","IOTCS-T10")] 
        public void Format_types_Not_Mandatory()
        {// this test was made NOT mandatory for embedded devices behavior 
            Assert.DoesNotThrow(() =>
            {
                IDataElement testElement = new DataElement<object>("data0", 
                    null
                    ,null,false,null,null,
                    new Format("", "", null, ""),
                    null,null,false); // Format Base constructor exposed or does not do validation 
            });

        }

        [Test, Property("TestCaseKey", "IOTCS-T10")]
        public void Format_types_Basic_CanBeCreated()
        {
            Assert.Multiple(() =>
            {
                // boolean, number, string
                Assert.That(
                    new DataElement<object>("booleanFormat",
                        null,
                        null,
                        false,
                        null,
                        null,
                        new BooleanFormat(ns: "JSON"),null,null,false).Format.Type,
                    Is.EqualTo(Format.Types.Boolean));

                Assert.That(
                    new DataElement<object>("IntegerFormat",
                        format: new IntegerFormat(new IntegerValuation(0), ns: "JSON")).Format.Type,
                    Is.EqualTo(Format.Types.Number));

                Assert.That(
                    new DataElement<object>("FloatFormat",
                        format: new FloatFormat(new FloatValuation(0.0f), ns: "JSON")).Format.Type,
                    Is.EqualTo(Format.Types.Number));

                Assert.That(
                    new DataElement<object>("data0",
                        format: new StringFormat(new StringValuation("-"), ns: "JSON")).Format.Type,
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
                    new DataElement<object>("arrayFormat", 
                        format: new ArrayFormat(new ArrayValuation(Format.Types.Number, new IntegerFormat(new IntegerValuation(0, 100))))).Format.Type,
                    Is.EqualTo(Format.Types.Array));

                Assert.That(
                    new DataElement<object>("integerEnumFormat",
                        format: new IntegerEnumFormat(
                            new IntegerEnumValuation(new Dictionary<string, string> {
                                    { "firstyear", "1970" },
                                    { "secondyear", "1971" }
                                }))
                            ).Format.Type,
                    Is.EqualTo(Format.Types.Enum));

                var intField = new Field("intField1", new IntegerFormat(new IntegerValuation(-100, 100)));
                var floatField = new Field("floatField1", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)));
                var stringField = new Field("stringField1", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
                Assert.That(
                    new DataElement<object>("object1",
                        format: new ObjectFormat(new ObjectValuation(new List<Field> { intField, floatField, stringField }))
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
                    new DataElement<object>("IntegerFormat",
                        format: new IntegerFormat(new IntegerValuation(0), ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Integer));

                Assert.That(
                    new DataElement<object>("FloatFormat",
                        format: new FloatFormat(new FloatValuation(0.0f), ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Float));

                Assert.That(
                    new DataElement<object>("utf8Stringencoding",
                        format: new StringFormat(new StringValuation("-"), ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Utf8));

                Assert.That(
                    new DataElement<object>("stringEncoding",
                        format: new StringFormat(new StringValuation("-"), encoding: Format.Encodings.Utf8, ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Utf8));

                Assert.That(
                    new DataElement<object>("stringEncoding",
                        format: new StringFormat(new StringValuation("-"), encoding: Format.Encodings.Ascii, ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.Ascii));

                Assert.That(
                    new DataElement<object>("hexstringEncoding",
                        format: new StringFormat(new StringValuation("-"), encoding: Format.Encodings.HexString, ns: "JSON")).Format.Encoding,
                    Is.EqualTo(Format.Encodings.HexString));

            });
        }

        [Test, Property("TestCaseKey","IOTCS-T11")] 
        public void Format_namespace_can_be_ignored_AtCreation()
        {
            IDataElement de = new DataElement<object>("data0", 
                format: new Format(type: "number", encoding: "integer", valuation: null)); 
            Assert.That(de.Format.Namespace, Is.EqualTo("json"));
        }

        [Test, Property("TestCaseKey","IOTCS-T11")] 
        public void Format_namespace_defaultsToJson()
        {
            Assert.Multiple(() => { 

                // expected: null or empty namespace defaults to "json"
                IDataElement de = new DataElement<object>("data0", 
                    format: new Format(type: "number", encoding: "integer", valuation: null, ns: null));
                Assert.That(de.Format.Namespace, Is.Not.Null);

                IDataElement de2 = new DataElement<object>("data0",
                    format: new StringFormat(new StringValuation(""), ns: ""));
                Assert.That(de2.Format.Namespace, Is.Not.Empty);

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
                    new DataElement<object>("data0",
                        format: new IntegerFormat(new IntegerValuation(0), ns: "json"));
                });

                Assert.DoesNotThrow(() =>
                {
                    new DataElement<object>("data0",
                        format: new IntegerFormat(new IntegerValuation(0), ns: "iolink"));
                });

                Assert.DoesNotThrow(() =>
                {
                    var de1 = new DataElement<object>("data0",
                        format: new IntegerFormat(new IntegerValuation(0), ns: "undefined-namespace-1"));
                    Assert.That(de1.Format.Namespace, Is.EqualTo("undefined-namespace-1"));
                });
            });

        }

            readonly Dictionary<string,Type> ValuationProperties = new Dictionary<string, Type> {
                {"Min", typeof(object)},
                {"Max", typeof(object)},
                {"DecimalPlaces",typeof(int?)},
                {"MinLength", typeof(int?)},
                {"MaxLength", typeof(int?)},
                {"Pattern",  typeof(string)},
                {"Values", typeof(Dictionary<string,string>)},
                {"DefaultValue",  typeof(object)},
                {"Fields", typeof(List<Field>)},
                {"BaseType",  typeof(string)},
                {"Format",  typeof(Format)}
            };


        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuations_all_members_defined_in_baseClassValuation()
        {
            var valuationInstance = new Valuation(null, null, null, null, null, null, null, null, null, null, null, null);
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valuationInstance, ns:"JSON"));
            Assert.Multiple(() =>
            {
                foreach (var name in ValuationProperties.Keys)
                {
                    var exptype = ValuationProperties[name];
                    var actualProperty = de1.Format.Valuation.GetType().GetProperty(name);
                    Assert.NotNull(actualProperty, "property not found in Valuation: '{0}' ", name);
                    Assert.AreEqual(exptype, actualProperty.PropertyType, "type mismatched for property '{0}'", name);
                }
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_IntegerValuation()
        {
            var valn = new IntegerValuation(int.MinValue, int.MaxValue, 0);
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valn, ns:"JSON"));
            var ivn = de1.Format.Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(ivn, Is.InstanceOf(typeof(Valuation)));
                Assert.AreEqual(ivn.Min, int.MinValue);
                Assert.AreEqual(ivn.Max, int.MaxValue);
                Assert.AreEqual(ivn.DefaultValue, null);
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_FloatValuation()
        {
            var valn = new FloatValuation(float.MinValue, float.MaxValue, 2, 0.0f);
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valn, ns:"JSON"));
            var fvn = de1.Format.Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(fvn, Is.InstanceOf(typeof(Valuation)));
                Assert.LessOrEqual(Math.Abs((float)fvn.Min - float.MinValue), 0.0000001f);
                Assert.LessOrEqual(Math.Abs((float)fvn.Max - float.MaxValue), 0.0000001f);
                Assert.LessOrEqual(Math.Abs((float)fvn.DefaultValue - 0.0f), 0.0000001f);
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_StringValuation()
        {
            var valn = new StringValuation(int.MinValue, int.MaxValue, pattern: @"\$*()[a-zA-Z0-9]", defaultValue: "stringdefaultValue");
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valn, ns:"JSON"));
            var svn = de1.Format.Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(svn, Is.InstanceOf(typeof(Valuation)));
                Assert.AreEqual(svn.MinLength, int.MinValue);
                Assert.AreEqual(svn.MaxLength, int.MaxValue);
                Assert.AreEqual(svn.Pattern, @"\$*()[a-zA-Z0-9]");
                Assert.AreEqual(svn.DefaultValue, "stringdefaultValue");
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_IntegerEnumValuation()
        {
            var valn = new IntegerEnumValuation(new Dictionary<string, string> { { "1", "one" }, { "2", "two" } }, 1);
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valn, ns:"JSON"));
            var ievn = de1.Format.Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(ievn, Is.InstanceOf(typeof(Valuation)));
                Assert.That(ievn.Values, Contains.Key("1"));
                Assert.That(ievn.Values, Contains.Value("one"));
                Assert.That(ievn.Values, Contains.Key("2"));
                Assert.That(ievn.Values, Contains.Value("two"));
                Assert.AreEqual(ievn.DefaultValue, 1);
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_ArrayValuation()
        {
            var valn = new ArrayValuation(Format.Types.String, new StringFormat(new StringValuation("noname")));
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valn, ns:"JSON"));
            var avn = de1.Format.Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(avn.BaseType, Is.EqualTo(Format.Types.String));
                Assert.That(avn.Format.Encoding, Is.EqualTo(Format.Encodings.Utf8));
                Assert.That(avn.Format.Valuation.DefaultValue, Is.EqualTo("noname"));

                var avni = new ArrayValuation(Format.Types.Array, new IntegerFormat(new IntegerValuation(0)));
                Assert.That(avni.BaseType, Is.EqualTo(Format.Types.Array));
                Assert.That(avni.Format.Encoding, Is.EqualTo(Format.Encodings.Integer));
                Assert.That(avni.Format.Valuation.DefaultValue, Is.EqualTo(0));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T13")]
        public void Format_valuation_DerivedTypeMembersAreUsable_ObjectValuation()
        {
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)),optional:true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var valn = new ObjectValuation(new List<Field> { intField, floatField, stringField });
            var de1 = new DataElement<object>("valcheck", format: new Format("customtype", "customencoding", valn, ns:"JSON"));
            var objvn = de1.Format.Valuation;
            Assert.Multiple(() =>
            {
                Assert.That(objvn, Is.InstanceOf(typeof(Valuation)));
                Assert.That(objvn.Fields.Count, Is.EqualTo(3));

                Assert.That(objvn.Fields[0].Name, Is.EqualTo("intField"));
                Assert.That(objvn.Fields[1].Name, Is.EqualTo("floatField"));
                Assert.That(objvn.Fields[2].Name, Is.EqualTo("stringField"));

                Assert.That(objvn.Fields[0].Optional, Is.EqualTo(false));
                Assert.That(objvn.Fields[1].Optional, Is.EqualTo(true));
                Assert.That(objvn.Fields[2].Optional, Is.EqualTo(false));

                Assert.That(objvn.Fields[0].Format.Encoding, Is.EqualTo(Format.Encodings.Integer));
            });
        }
    }
}
