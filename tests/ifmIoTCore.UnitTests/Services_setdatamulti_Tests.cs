namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_setdatamulti_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T71")]
        public void setdatamulti_Invalid_datatosend_400BadRequest_530DataInvalid()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            Assert.DoesNotThrow(() => ioTCore.Root.SetDataMulti(new SetDataMultiRequestServiceData(new Dictionary<string, JToken>())));
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.That(ioTCore.HandleRequest(0, "/setdatamulti").Code, Is.EqualTo(ResponseCodes.DataInvalid));
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.That(ioTCore.HandleRequest(0, "/setdatamulti", JToken.Parse("{'datatosend':null}")).Code, Is.EqualTo(ResponseCodes.DataInvalid));
        }

        [Test, Property("TestCaseKey", "IOTCS-T72")]
        public void setdatamulti_SingleElement_WorksAsSetDataService()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            object setDataValue = null;
            ioTCore.CreateDataElement<long>(ioTCore.Root,
            "int1",
                    (s) => { return 42; }, 
                    (senderElement, i) => 
                    { 
                        setDataValue = i; 
                    }, format: new IntegerFormat(new IntegerValuation(0)));
            var setdatamultiResponse = ioTCore.HandleRequest(0, "/setdatamulti", JToken.Parse("{'datatosend':{'/int1':43}}"));
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
            Assert.AreEqual(43, setDataValue);
        }

        [Test, Property("TestCaseKey", "IOTCS-T73")]
        public void setdatamulti_MultipleDataElements_ExpectedDataIsDelivered_SetDataHandlers_respectively()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var ExpectedSetDataDict = new Dictionary<string, object> {
                { "/int1", 42 }, { "/int2", 43 }, { "/int3", 44 }, { "/int4", 45 }, 
                { "/string1", "everything1" }, { "/string2", "everything2" }, { "/string3", "everything3" }, { "/string4", "everything4" }};
            ExpectedSetDataDict["/complex1"] = new complexData();
            ExpectedSetDataDict["/complex2"] = new complexData(43, 43f, "everything43");

            var ActualSetDataList = new List<Tuple<string,object>>(); 
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "int1",
                    null,
                    (senderElement, incoming) => {
                        ActualSetDataList.Add(Tuple.Create("/int1", (object)incoming)); 
                    }, format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "int2",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/int2", (object)incoming));
                    }, format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "int3",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/int3", (object)incoming));
                    }, format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "int4",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/int4", (object)incoming));
                    }, format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.CreateDataElement<string>(ioTCore.Root,
                "string1",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/string1", (object)incoming));
                    }, format: new StringFormat(new StringValuation("")));
            ioTCore.CreateDataElement<string>(ioTCore.Root,
                "string2",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/string2", (object)incoming));
                    }, format: new StringFormat(new StringValuation("")));
            ioTCore.CreateDataElement<string>(ioTCore.Root,
                "string3",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/string3", (object)incoming));
                    }, format: new StringFormat(new StringValuation("")));
            ioTCore.CreateDataElement<string>(ioTCore.Root,
            "string4",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/string4", (object)incoming));
                    }, format: new StringFormat(new StringValuation("")));

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });

            ioTCore.CreateDataElement<complexData>(ioTCore.Root,
                "complex1",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/complex1", (object)incoming));
                    }, format: new Format("customtype", "customencoding", complextypeValn));

            ioTCore.CreateDataElement<complexData>(ioTCore.Root,
                "complex2",
                    null,
                    (senderElement, incoming) =>
                    {
                        ActualSetDataList.Add(Tuple.Create("/complex2", (object)incoming));
                    }, format: new Format("customtype", "customencoding", complextypeValn));


            var datatosendDict = new Dictionary<string, object>();
            datatosendDict["datatosend"] = ExpectedSetDataDict;
            ioTCore.HandleRequest(0, "/setdatamulti", JToken.FromObject(datatosendDict));
            Assert.That(ActualSetDataList.Count(), Is.EqualTo(10));
            Assert.Multiple(() =>
            {
                foreach (var item in ActualSetDataList)
                    Assert.That(item.Item2, Is.EqualTo(ExpectedSetDataDict[item.Item1]));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T75")]
        public void setdatamulti_IgnoresWithWarningLogs_NonWritableDataElements()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root, "int_nosetter");

            ioTCore.CreateDataElement<int>(ioTCore.Root, "int_nosetter2", null);

            Assert.Multiple(() =>
            {
                using (var directLogger = new TemporaryMemoryAppender())
                {
                        var setdatamultiResponse = ioTCore.HandleRequest(0, "/setdatamulti", JToken.Parse("{'datatosend':{'/int_nosetter':43}}"));
                    Assert.AreEqual(200, ResponseCodes.Success);
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
                    var logMessages = directLogger.PopAllEvents();
                    Assert.IsTrue(logMessages.All(e => e.Level == log4net.Core.Level.Warn));

                    setdatamultiResponse = ioTCore.HandleRequest(0, "/setdatamulti", JToken.Parse("{'datatosend':{'/int_nosetter2':43}}"));
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
                    logMessages = directLogger.PopAllEvents();
                    Assert.IsTrue(logMessages.All(e => e.Level == log4net.Core.Level.Warn));
                }
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T77")]
        public void setdatamulti_IgnoresWithWarningLog_NonDataElementsOrInvalidElements()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateStructureElement(ioTCore.Root, "strcut1");
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.Multiple(() =>
            {
                using (var directLogger = new TemporaryMemoryAppender())
                {
                    var setdatamultiResponse = ioTCore.HandleRequest(0, "/setdatamulti", JToken.Parse("{'datatosend': {'/struct1':42}}"));
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
                    var logMessages = directLogger.PopAllEvents();
                    Assert.IsTrue(logMessages.All(e => e.Level == log4net.Core.Level.Warn));

                    setdatamultiResponse = ioTCore.HandleRequest(0, "/setdatamulti", JToken.Parse("{'datatosend': {'/unknown':42}}"));
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
                    logMessages = directLogger.PopAllEvents();
                    Assert.IsTrue(logMessages.All(e => e.Level == log4net.Core.Level.Warn));
                }
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T70")]
        public void setdatamulti_service_AccessibleThroughNetAdapter_http()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            object setDataValue = null;
            ioTCore.CreateDataElement<long>(ioTCore.Root,
                "int1",
                    (s) => { return 42; }, 
                    (senderElement, incomingJson) => 
                    { 
                        setDataValue = incomingJson; 
                    }, false,
                    format: new IntegerFormat(new IntegerValuation(0)));
            var setdatamultiResponse = new StartNetAdapterServerAndGetResponseHttp().Do( 
                new RequestMessage(1,"/setdatamulti", JToken.Parse(@"{'datatosend':{'/int1':43}}")),
                ioTCore,
                new Uri("http://127.0.0.1:10005"), TestContext.Out);
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
            Assert.AreEqual(43, setDataValue);
        }
    }
}