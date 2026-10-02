namespace ifm.IoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using ElementManager.Contracts.Elements;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.ServiceData.Requests;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ElementManager.Contracts.Elements.Valuations;
    using ifm.Common.Variant;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_setdatamulti_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T71")]
        public void setdatamulti_Invalid_datatosend_400BadRequest_530DataInvalid()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");

            var setDataMultiService = (IServiceElement<SetDataMultiRequestServiceData, SetDataMultiResponseServiceData>)ioTCore.Root.Subs.Single(x => x.Identifier == "setdatamulti");

            Assert.DoesNotThrow(() => setDataMultiService.Invoke(new SetDataMultiRequestServiceData(new Dictionary<string, Variant>())));
            Assert.That(ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti").Code, Is.EqualTo((int)ResponseCodes.DataInvalid));
            Assert.That(ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", new VariantObject { { "datatosend", null } }).Code, Is.EqualTo((int)ResponseCodes.DataInvalid));
        }

        [Test, Property("TestCaseKey", "IOTCS-T72")]
        public void setdatamulti_SingleElement_WorksAsSetDataService()
        { 
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            object setDataValue = null;
            ioTCore.ElementManager.CreateDataElement(ioTCore.Root, "int1",
                    (s, i) => { return 42; }, 
                    (senderElement, d, i) => 
                    { 
                        setDataValue = d; 
                    },
                     format: new Int32Format());

            var setdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", new VariantObject { { "datatosend", new VariantObject { { "/int1", new VariantValue(43) } } } });
            Assert.That(setdatamultiResponse.Code, Is.EqualTo((int)ResponseCodes.Success));
            Assert.That(setDataValue, Is.EqualTo(43));
        }

        [Test, Property("TestCaseKey", "IOTCS-T73")]
        public void setdatamulti_MultipleDataElements_ExpectedDataIsDelivered_SetDataHandlers_respectively()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var ExpectedSetDataDict = new Dictionary<string, object> {
                { "/int1", 42 }, { "/int2", 43 }, { "/int3", 44 }, { "/int4", 45 }, 
                { "/string1", "everything1" }, { "/string2", "everything2" }, { "/string3", "everything3" }, { "/string4", "everything4" }};
            ExpectedSetDataDict["/complex1"] = new complexData();
            ExpectedSetDataDict["/complex2"] = new complexData(43, 43f, "everything43");

            var actualSetDataList = new List<Tuple<string,object>>(); 
            ioTCore.ElementManager.CreateWriteOnlyDataElement<int>(ioTCore.Root, "int1", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/int1", (object)incoming));  }, format: new Int32Format());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<int>(ioTCore.Root, "int2", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/int2", (object)incoming)); }, format: new Int32Format());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<int>(ioTCore.Root, "int3", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/int3", (object)incoming)); }, format: new Int32Format());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<int>(ioTCore.Root, "int4", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/int4", (object)incoming)); }, format: new Int32Format());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<string>(ioTCore.Root, "string1", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/string1", (object)incoming)); }, format: new StringFormat());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<string>(ioTCore.Root, "string2", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/string2", (object)incoming)); }, format: new StringFormat());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<string>(ioTCore.Root, "string3", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/string3", (object)incoming)); }, format: new StringFormat());
            ioTCore.ElementManager.CreateWriteOnlyDataElement<string>(ioTCore.Root, "string4", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/string4", (object)incoming)); }, format: new StringFormat());

            // complex type data element
            var intField = new ObjectValuation.Field("intField", new Int32Format(new Int32Valuation(-100, 100)));
            var floatField = new ObjectValuation.Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new ObjectValuation.Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField });

            ioTCore.ElementManager.CreateWriteOnlyDataElement<complexData>(ioTCore.Root, "complex1", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/complex1", (object)incoming)); }, format: new ObjectFormat(complextypeValn)); 
            ioTCore.ElementManager.CreateWriteOnlyDataElement<complexData>(ioTCore.Root, "complex2", (senderElement, incoming, i) => { actualSetDataList.Add(Tuple.Create("/complex2", (object)incoming)); }, format: new ObjectFormat(complextypeValn));

            var datatosendDict = new Dictionary<string, object>();
            datatosendDict["datatosend"] = ExpectedSetDataDict;

            ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", Variant.FromObject(datatosendDict));
            Assert.That(actualSetDataList.Count(), Is.EqualTo(10));
            Assert.Multiple(() =>
            {
                foreach (var item in actualSetDataList)
                    Assert.That(item.Item2, Is.EqualTo(ExpectedSetDataDict[item.Item1]));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T75")]
        public void setdatamulti_IgnoresWithWarningLogs_NonWritableDataElements()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement<int>(ioTCore.Root, "int_nosetter");

            ioTCore.ElementManager.CreateSimpleDataElement<int>(ioTCore.Root, "int_nosetter2");

            Assert.Multiple(() =>
            {
                using (var directLogger = new TemporaryMemoryAppender())
                {
                        var setdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", new VariantObject { { "datatosend", new VariantObject { { "/int_nosetter", new VariantValue(43) } } } });
                    Assert.That(ResponseCodes.Success, Is.EqualTo(200));
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo((int)ResponseCodes.Success));
                    var logMessages = directLogger.PopAllEvents();
                    Assert.That(logMessages.All(e => e.Level == log4net.Core.Level.Warn));

                    setdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", new VariantObject { { "datatosend", new VariantObject { { "/int_nosetter2", new VariantValue(43) } } } });
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo((int)ResponseCodes.Success));
                    logMessages = directLogger.PopAllEvents();
                    Assert.That(logMessages.All(e => e.Level == log4net.Core.Level.Warn));
                }
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T77")]
        public void setdatamulti_IgnoresWithWarningLog_NonDataElementsOrInvalidElements()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "strcut1");
            Assert.That(ResponseCodes.Success, Is.EqualTo(200));

            Assert.Multiple(() =>
            {
                using (var directLogger = new TemporaryMemoryAppender())
                {
                    var setdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", new VariantObject { { "datatosend", new VariantObject { { "/struct1", new VariantValue(42) } } } });
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo((int)ResponseCodes.Success));
                    var logMessages = directLogger.PopAllEvents();
                    Assert.That(logMessages.All(e => e.Level == log4net.Core.Level.Warn));

                    setdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", new VariantObject { { "datatosend", new VariantObject { { "/unknown", new VariantValue(42) } } } });
                    Assert.That(setdatamultiResponse.Code, Is.EqualTo((int)ResponseCodes.Success));
                    logMessages = directLogger.PopAllEvents();
                    Assert.That(logMessages.All(e => e.Level == log4net.Core.Level.Warn));
                }
            });
        }

        [Test]
        public void SetDataMultiResponseTests()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            object setDataValue = null;
            ioTCore.ElementManager.CreateDataElement(ioTCore.Root, "int1",
                (s, i) => { return 42; },
                (senderElement, d, i) =>
                {
                    setDataValue = d;
                },
                format: new Int32Format());

            var setdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/setdatamulti", 
                new VariantObject
                {
                    { "datatosend", new VariantObject
                        {
                            { "/int1", new VariantValue(43) }
                        }
                    }
                });

            Assert.That(setdatamultiResponse.Code, Is.EqualTo((int)ResponseCodes.Success));

            var setDataResponseData = setdatamultiResponse.Data?.ToObject<SetDataMultiResponseServiceData>();

            Assert.That(setDataResponseData,Is.Not.Null);

            Assert.That(setDataValue, Is.EqualTo(43));
        }

    }
}