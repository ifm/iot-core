namespace ifm.IoTCore.UnitTests.Elements
{
    using System.Collections.Generic;
    using ifm.Common.Variant;
    using Common;
    using Contracts;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ElementManager.Contracts.Elements.Valuations;
    using Message;
    using MessageConverter.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public class setdataMessages
    {
        public static IEnumerable<TestCaseData> samples_alternativenames
        {
            get
            {
                yield return new TestCaseData(
                new MessageConverter().Deserialize(
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
                new MessageConverter().Deserialize(
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
            testiotcore = Helpers.CreateIoTCore("testiotcore");
        }

        [TearDown]
        public void DeleteIotCoreInstance()
        {
        }

        [Test, Property("TestCaseKey", "IOTCS-T45")]
        public void getdata_Service_WorksThroughCommandInterface_IoTCoreMessaging()
        {
            // simple type data element

            var simpleDataElement =
                testiotcore.ElementManager.CreateReadOnlyDataElement(testiotcore.Root, "string1", (_, i) => "hellotest", format: new StringFormat(new StringValuation(0, 100)));
            
            var getdataService = Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(0, "/string1/getdata").Data);
            Assert.That((string)(VariantValue)getdataService.Value, Is.EqualTo("hellotest"));

            // complex type data element
            var intField = new ObjectValuation.Field("intField", new Int32Format(new Int32Valuation(-100, 100)));
            var floatField = new ObjectValuation.Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new ObjectValuation.Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField });
            var complex1 = new complexData { float1 = 42f, int1 = 42, string1 = "everything" };

            var complexDataElement = testiotcore.ElementManager.CreateReadOnlyDataElement(testiotcore.Root, "complexdatatype", (e, i) => complex1, format: new ObjectFormat(complextypeValn));
            var getdataService2 = Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(0, "/complexdatatype/getdata").Data);


            Assert.That((int)(VariantValue)getdataService2.Value.AsVariantObject()["int1"], Is.EqualTo(42));
            Assert.That((float)(VariantValue)getdataService2.Value.AsVariantObject()["float1"], Is.EqualTo(42f));
            Assert.That((string)(VariantValue)getdataService2.Value.AsVariantObject()["string1"], Is.EqualTo("everything"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T48")]
        [TestCaseSource(typeof(setdataMessages), nameof(setdataMessages.samples_alternativenames))]
        public void setdata_Service_Message_Works(Message setdataMessage)
        {
            Variant setDataValue = null;
            var simpleDataElement =
                testiotcore.ElementManager.CreateWriteOnlyDataElement<Variant>(testiotcore.Root, "string1",
                    (s, e, i) =>
                    {
                        setDataValue = e;
                    },
                    format: new StringFormat(new StringValuation(0, 100)));
            
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(setdataMessage).Code, Is.EqualTo((int)ResponseCodes.Success)); 
            Assert.That((string)setDataValue.AsVariantValue(), Is.EqualTo("hallotest"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T48")]
        public void setdata_Service_WorksThroughCommandInterface_IoTCoreMessaging()
        {
            // simple type data element
            string simpleValue = null;
            var simpleDataElement =
                testiotcore.ElementManager.CreateWriteOnlyDataElement<string>(testiotcore.Root, "simple",
                    (_, e, i) => { simpleValue = e; });
            
            testiotcore.MessageDispatcher.HandleRequest(0, "/simple/setdata", 
                new VariantObject
                {
                    { "value", new VariantValue("hallotest") }
                });
            Assert.That(simpleValue, Is.EqualTo("hallotest"));

            // complex type data element
            complexData complexValue = null;
            var complexDataElement =

                testiotcore.ElementManager.CreateWriteOnlyDataElement<complexData>(testiotcore.Root, "complex",
                    (e, d, i) => { complexValue = d; });
            
            testiotcore.MessageDispatcher.HandleRequest(0, "/complex/setdata", 
                new VariantObject
                {
                    {"value", new VariantObject
                    {
                        { "string1", new VariantValue("something") }, 
                        { "int1", new VariantValue(41) }, 
                        { "float1", new VariantValue(41.0f)}
                    }}
                });

            Assert.That(41, Is.EqualTo(complexValue.int1));
            Assert.That(41f, Is.EqualTo(complexValue.float1));
            Assert.That("something", Is.EqualTo(complexValue.string1));
        }
    }

}
