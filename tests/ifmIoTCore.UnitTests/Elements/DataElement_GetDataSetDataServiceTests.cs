using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements;
using ifmIoTCore.Elements.ServiceData.Responses;

namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class setdataMessages
    {
        public static IEnumerable<TestCaseData> samples_alternativenames
        {
            get
            {
                yield return new TestCaseData(
                new MessageConverter.Json.Newtonsoft.MessageConverter().Deserialize(
                    JObject.Parse(@"{
                        'cid': 1,
                        'code': 10,
                        'adr': '/string1/setdata',
                        'data': { 
                                'value': 'hallotest'
                                }
                         }").ToString()
                        )
                ).SetName("{m}_value");

                yield return new TestCaseData(
                new MessageConverter.Json.Newtonsoft.MessageConverter().Deserialize(
                    JObject.Parse(@"{
                        'cid': 1,
                        'code': 10,
                        'adr': '/string1/setdata',
                        'data': { 
                                'newvalue': 'hallotest'
                                }
                         }").ToString()
                        )
                ).SetName("{m}_alternativeName_newvalue");
            }

        }
    }

    [TestFixture]
    class DataElement_GetDataSetDataService_Tests
    {
        IIoTCore testiotcore;

        [SetUp]
        public void CreateIotCoreInstance()
        {
            testiotcore = IoTCoreFactory.Create("testiotcore");
        }

        [Test, Property("TestCaseKey", "IOTCS-T45")]
        public void getdata_Service_WorksThroughCommandInterface_IoTCoreMessaging()
        {
            // simple type data element
            var simpleDataElement = new ReadOnlyDataElement<string>("string1",
                (sender) => { return "hellotest"; },
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(simpleDataElement, true);

            var getdataService = Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(0, "/string1/getdata").Data);
            Assert.AreEqual("hellotest", (string)(VariantValue)getdataService.Value);

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });
            var complex1 = new complexData() { float1 = 42f, int1 = 42, string1 = "everything" };

            var complexDataElement = new ReadOnlyDataElement<complexData>("complexdatatype",
                (s) => { return complex1; }, 
                format: new Format("customtype", "customencoding", complextypeValn, ns: "JSON"));
            testiotcore.Root.AddChild(complexDataElement, true);

            var getdataService2 = Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(0, "/complexdatatype/getdata").Data);


            Assert.AreEqual(42, (int)(VariantValue)getdataService2.Value.AsVariantObject()["int1"]);
            Assert.AreEqual(42f, (float)(VariantValue)getdataService2.Value.AsVariantObject()["float1"]);
            Assert.AreEqual("everything", (string)(VariantValue)getdataService2.Value.AsVariantObject()["string1"]);
        }

        [Test, Property("TestCaseKey", "IOTCS-T48")]
        [TestCaseSource(typeof(setdataMessages), nameof(setdataMessages.samples_alternativenames))]
        public void setdata_Service_Message_Works(Message setdataMessage)
        {
            Variant setDataValue = null;
            var simpleDataElement = new WriteOnlyDataElement<Variant>("string1",
                (s, e) =>
                {
                    setDataValue = e; 
                },
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(simpleDataElement, true);

            Assert.That(testiotcore.HandleRequest(setdataMessage).Code, Is.EqualTo(ResponseCodes.Success)); 
            Assert.That((string)setDataValue.AsVariantValue(), Is.EqualTo("hallotest"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T48")]
        public void setdata_Service_WorksThroughCommandInterface_IoTCoreMessaging()
        {
            string setDataValue = null;
            // simple type data element
            var simpleDataElement = new WriteOnlyDataElement<string>("string1",
                (s, e) => { setDataValue = e; }, 
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(simpleDataElement, true);

            Assert.IsNull(setDataValue);
            var getdataService = testiotcore.HandleRequest(0, "/string1/setdata", new VariantObject() { { "newvalue", new VariantValue("hallotest") } });
            Assert.That(setDataValue, Is.EqualTo("hallotest"));

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });
            var complex1 = new complexData();
            complexData setDataValue2 = null;
            var complexDataElement = new WriteOnlyDataElement<complexData>("complexdatatype",
                (s, e) =>
                {
                    setDataValue2 = e;
                },
                format: new Format("customtype", "customencoding", complextypeValn, ns: "JSON"));
            testiotcore.Root.AddChild(complexDataElement, true);

            var getdataService2 = Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(0, "/complexdatatype/setdata", 
                new VariantObject()
                {{
                    "newvalue", new VariantObject() { { "string1", new VariantValue("something") }, { "int1", new VariantValue(41) }, { "float1", new VariantValue(41.0f) } }
                }}).Data);
            Assert.AreEqual(41, setDataValue2.int1);
            Assert.AreEqual(41f, setDataValue2.float1);
            Assert.AreEqual("something", setDataValue2.string1);
        }
    }

}
