namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public class complexData
    {
        public int int1; public float float1; public string string1;
        public complexData(int newint = 42, float newfloat = 42f, string newstring = "everything")
        {
            int1 = newint; float1 = newfloat; string1 = newstring;
        }
        public override bool Equals(object obj)
        {
            var other = obj as complexData;
            if (other == null) return false;
            return (int1.Equals(other.int1) && float1.Equals(other.float1) && string1.Equals(other.string1));
        }
        public override int GetHashCode()
        {
            return ( int1, float1, string1 ).GetHashCode();
        }

    }

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_getdatamulti_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T63")]
        public void getdatamulti_ErrorResponseOnInvalidation_datatosend_missingOrNull()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            Assert.Multiple(() =>
            {
                Assert.Throws<IoTCoreException>(() => ioTCore.Root.GetDataMulti(new GetDataMultiRequestServiceData(null)));
                Assert.AreEqual(422, ResponseCodes.DataInvalid);
                Assert.That(ioTCore.HandleRequest(0, "/getdatamulti").Code,
                    Is.EqualTo(ResponseCodes.DataInvalid));
                Assert.That(ioTCore.HandleRequest(0, "/getdatamulti", JToken.Parse("{'datatosend':null}")).Code,
                    Is.EqualTo(ResponseCodes.DataInvalid));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T63")]
        public void getdatamulti_Ok200ResponseOn_empty_datatosend_list()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            Assert.That(
                ioTCore.Root.GetDataMulti(new GetDataMultiRequestServiceData(new List<string>())),
                Is.Empty);
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.That(ioTCore.HandleRequest(0, "/getdatamulti", JToken.Parse(@"{'datatosend':[]}")).Code, 
                Is.EqualTo(ResponseCodes.Success));
        }

        [Test, Property("TestCaseKey", "IOTCS-T64")]
        public void getdatamulti_Responds_AddressCodeValue_ForDataElement_Single()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "int1", 
                    (s) => { return 42; }, 
                    format: new IntegerFormat(new IntegerValuation(0)));
            Assert.AreEqual(200, ResponseCodes.Success);
            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti", JToken.Parse(@"{'datatosend':['/int1']}"));
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.IsNotNull(getdatamultiResponse.Data.SelectToken("$./int1"));
            Assert.AreEqual(getdatamultiResponse.Data.SelectToken("$./int1.code").ToObject<int>(), 200);
            Assert.AreEqual(getdatamultiResponse.Data.SelectToken("$./int1.data").ToObject<int>(), 42);
        }

        [Test, Property("TestCaseKey", "IOTCS-T65")]
        public void getdatamulti_Responds_AddressCodeValue_ForDataElements_10()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            for (var i = 1; i <= 5; i++)
                ioTCore.CreateDataElement<int>(ioTCore.Root,
                    string.Format("int{0}", i),
                        (s) => { return 42; },
                        format: new IntegerFormat(new IntegerValuation(0)));

            for (var i = 1; i <= 3; i++)
                ioTCore.CreateDataElement<string>(ioTCore.Root,
                    string.Format("string{0}", i), 
                        (s) => { return "everything"; },
                        format: new StringFormat(new StringValuation("")));

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });

            for (var i = 1; i <= 2; i++)
                ioTCore.CreateDataElement<complexData>(ioTCore.Root,
                    string.Format("complex{0}", i), 
                        (s) => { return new complexData(); },
                        null,
                        format: new Format("customtype", "customencoding", complextypeValn));

            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti", 
                JToken.Parse(@"{'datatosend': [ '/int1', '/int2', '/int3', '/int4', '/int5', 
                                                '/string1', '/string2', '/string3', 
                                                '/complex1', '/complex2']
                                }"));
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            var multidata = getdatamultiResponse.Data;
            Assert.Multiple(() =>
            {
                for (var i = 1; i <= 5; i++)
                {
                    Assert.IsNotNull(multidata.SelectToken(string.Format("$./int{0}", i)));
                    Assert.AreEqual(ResponseCodes.Success, multidata.SelectToken(string.Format("$./int{0}.code", i)).ToObject<int>());
                    Assert.AreEqual(42, multidata.SelectToken(string.Format("$./int{0}.data", i)).ToObject<int>());
                }

                for (var i = 1; i <= 3; i++)
                {
                    Assert.IsNotNull(multidata.SelectToken(string.Format("$./string{0}", i)));
                    Assert.AreEqual(ResponseCodes.Success, multidata.SelectToken(string.Format("$./string{0}.code", i)).ToObject<int>());
                    Assert.AreEqual("everything" , multidata.SelectToken(string.Format("$./string{0}.data", i)).ToObject<string>());
                }

                for (var i = 1; i <= 2; i++)
                {
                    Assert.IsNotNull(multidata.SelectToken(string.Format("$./complex{0}", i)));
                    Assert.AreEqual(ResponseCodes.Success, multidata.SelectToken(string.Format("$./complex{0}.code", i)).ToObject<int>());
                    Assert.AreEqual("everything", multidata.SelectToken(string.Format("$./complex{0}.data.string1", i)).ToObject<string>());
                }

            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T66")]
        public void getdatamulti_404NotFound_ForUnknownDataElement()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti", 
                JToken.Parse(@"{'datatosend':['/unknown']}"));
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.AreEqual(404, ResponseCodes.NotFound);

            Assert.IsNotNull(getdatamultiResponse.Data.SelectToken("$./unknown"));
            Assert.AreEqual(ResponseCodes.NotFound, getdatamultiResponse.Data.SelectToken("$./unknown.code").ToObject<int>());
            Assert.AreEqual(JToken.Parse("null"), getdatamultiResponse.Data.SelectToken("$./unknown.data"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T65")]
        public void getdatamulti_ForNoGetterDataElement_Success()
        { // TODO move this test script to getdata service tests where it belongs
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<object>(ioTCore.Root, "int_nogetter");
            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti", 
                JToken.Parse(@"{'datatosend':[ '/int_nogetter']}"));
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.AreEqual(501, ResponseCodes.NotImplemented);

            Assert.IsNotNull(getdatamultiResponse.Data.SelectToken("$./int_nogetter"));
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Data.SelectToken("$./int_nogetter.code").ToObject<int>());
            Assert.AreEqual(JToken.Parse("null"), getdatamultiResponse.Data.SelectToken("$./int_nogetter.data"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T78")]
        public void getdatamulti_400BadRequest_ForNonDataElement()
        { // TODO move this test script to getdata service tests where it belongs
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateStructureElement(ioTCore.Root, "nondataelement");
            var getdatamultiResponse = ioTCore.HandleRequest(0,  "/getdatamulti", 
                JToken.Parse(@"{'datatosend':[ '/nondataelement']}"));
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.AreEqual(400, ResponseCodes.BadRequest);

            Assert.IsNotNull(getdatamultiResponse.Data.SelectToken("$./nondataelement"));
            Assert.AreEqual(ResponseCodes.BadRequest, getdatamultiResponse.Data.SelectToken("$./nondataelement.code").ToObject<int>());
            Assert.AreEqual(JToken.Parse("null"), getdatamultiResponse.Data.SelectToken("$./nondataelement.data"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T69")]
        public void getdatamulti_Service_AccessibleThroughNetAdapter_http()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root, 
                "int1", 
                    (s) => { return 42; });
            Assert.AreEqual(200, ResponseCodes.Success);
            var getdatamultiResponse = new StartNetAdapterServerAndGetResponseHttp().Do( 
                new RequestMessage(1,"/getdatamulti", JToken.Parse(@"{'datatosend':['/int1']}")),
                ioTCore,
                new Uri("http://127.0.0.1:10004"), TestContext.Out);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.IsNotNull(getdatamultiResponse.Data.SelectToken("$./int1"));
            Assert.AreEqual(getdatamultiResponse.Data.SelectToken("$./int1.code").ToObject<int>(), 200);
            Assert.AreEqual(getdatamultiResponse.Data.SelectToken("$./int1.data").ToObject<int>(), 42);
        }

    }
}
