namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    class DataElement_GetDataSetDataService_Tests
    {
        IIoTCore testiotcore;

        [SetUp]
        public void CreateIotCoreInstance()
        {
            testiotcore = IoTCoreFactory.Create("testiotcore", null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T45")]
        public void getdata_Service_WorksThroughCommandInterface_IoTCoreMessaging()
        {
            // simple type data element
            var simpleDataElement = testiotcore.CreateDataElement<string>(testiotcore.Root,
                "string1",
                (sender) => { return "hellotest"; },
                format: new StringFormat(new StringValuation(0, 100)));

            var getdataService = testiotcore.HandleRequest(0, "/string1/getdata");
            Assert.AreEqual("hellotest", getdataService.Data.SelectToken("$.value").ToObject<string>());

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });
            var complex1 = new complexData();
            var complexDataElement = testiotcore.CreateDataElement<complexData>(testiotcore.Root, "complexdatatype",
                (s) => { return complex1; },
                null,
                format: new Format("customtype", "customencoding", complextypeValn, ns: "JSON"));

            var getdataService2 = testiotcore.HandleRequest(0, "/complexdatatype/getdata");
            Assert.AreEqual(42, getdataService2.Data.SelectToken("$.value.int1").ToObject<int>());
            Assert.AreEqual(42f, getdataService2.Data.SelectToken("$.value.float1").ToObject<float>());
            Assert.AreEqual("everything", getdataService2.Data.SelectToken("$.value.string1").ToObject<string>());
        }

        [Test, Property("TestCaseKey", "IOTCS-T48")]
        public void setdata_Service_MandatoryFieldChecked_newvalue()
        {
            JToken setDataValue = null;
            var simpleDataElement = testiotcore.CreateDataElement<JToken>(testiotcore.Root, "string1", null,
                (s, e) =>
                {
                    setDataValue = e; 
                }, false,
                format: new StringFormat(new StringValuation(0, 100)));
            Assert.That(testiotcore.HandleRequest(0, "/string1/setdata", JToken.Parse("{'value':'hellotest'}")).Code, Is.EqualTo(ResponseCodes.DataInvalid), "expect error as mandatory field 'newvalue' not provided");
            Assert.That(testiotcore.HandleRequest(0, "/string1/setdata", JToken.Parse("{'newvalue':'hellotest'}")).Code, Is.EqualTo(ResponseCodes.Success), 
                "expect error as mandatory field 'newvalue' not provided");
            Assert.That(setDataValue.Value<string>, Is.EqualTo("hellotest"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T48")]
        public void setdata_Service_WorksThroughCommandInterface_IoTCoreMessaging()
        {
            string setDataValue = null;
            // simple type data element
            var simpleDataElement = testiotcore.CreateDataElement<string>(testiotcore.Root, "string1", null,
                (s, e) => { setDataValue = e; }, false,
                format: new StringFormat(new StringValuation(0, 100)));
            Assert.IsNull(setDataValue);
            var getdataService = testiotcore.HandleRequest(0, "/string1/setdata", JToken.Parse("{'newvalue':'hellotest'}"));
            Assert.That(setDataValue, Is.EqualTo("hellotest"));

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });
            var complex1 = new complexData();
            JToken setDataValue2 = null;
            var complexDataElement = testiotcore.CreateDataElement<JToken>(testiotcore.Root, "complexdatatype", null,
                (s, e) =>
                {
                    setDataValue2 = e;
                }, false,
                format: new Format("customtype", "customencoding", complextypeValn, ns: "JSON")
                );
            var getdataService2 = testiotcore.HandleRequest(0, "/complexdatatype/setdata", JToken.Parse(@"{newvalue:{'string1':'something', 'int1': 41, 'float1':41.0}}"));
            Assert.AreEqual(41, setDataValue2.SelectToken("$.int1").ToObject<int>());
            Assert.AreEqual(41f, setDataValue2.SelectToken("$.float1").ToObject<float>());
            Assert.AreEqual("something", setDataValue2.SelectToken("$.string1").ToObject<string>());
        }
    }

}
