namespace ifm.IoTCore.UnitTests.Elements
{
    using System;
    using ifm.Common.Variant;
    using Common;
    using Contracts;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ElementManager.Contracts.Elements.Valuations;
    using NUnit.Framework;

    [TestFixture]
    class DataElement_Creation_Tests
    {
        IIoTCore testiotcore;
        [OneTimeSetUp]
        public void CreateTestIotCore()
        {
            testiotcore = Helpers.CreateIoTCore("testiotcore");
        }

        [OneTimeTearDown]
        public void DisposeTestIotCore()
        {
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void ReadOnlyDataElement_CreatesAndAccepts_getdata_No_setdata()
        {
            // Given: iot core instantiated
            // When: readonlydataelement created with getdatafunc
            var dataelementId = Guid.NewGuid().ToString("N");
            const int everything = 42;
            var readonlyDataElement = testiotcore.ElementManager.CreateReadOnlyDataElement(testiotcore.Root, dataelementId, getDataFunc: (s, i) => everything);
            // Then: dataelement/getdata request works as expected
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1,$"/{dataelementId}/getdata").Code, Is.EqualTo((int)ResponseCodes.Success));
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/{dataelementId}/getdata").Data.AsVariantObject()["value"].ToObject<int>(), Is.EqualTo(everything));
            // Then: dataelement/setdata request gives error 404 NotFound
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/{dataelementId}/setdata", new VariantValue(everything + 1)).Code, Is.EqualTo((int)ResponseCodes.NotFound));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void WriteOnlyDataElement_CreatesAndAccepts_getdata_No_setdata()
        {
            // Given: iot core instaitated 
            // When: writeonlydataelement created with setdatafunc
            var dataelementId = Guid.NewGuid().ToString("N");
            var setvalue = 0; const int everything = 42;
            var writeonlyDataElement = testiotcore.ElementManager.CreateWriteOnlyDataElement<int>(testiotcore.Root, dataelementId, setDataFunc: (e, d, i) => setvalue = d);

            // Then: dataelement/setdata request works as expected
            var response = testiotcore.MessageDispatcher.HandleRequest(1, $"/{dataelementId}/setdata", data:new VariantObject {{"value", new VariantValue(everything)}});
            Assert.That(response.Code, Is.EqualTo((int)ResponseCodes.Success));
            Assert.That(setvalue, Is.EqualTo(everything));
            // Then: dataelement/getdata request gives error 404 NotFound
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/{dataelementId}/getdata").Code, Is.EqualTo((int)ResponseCodes.NotFound));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void Create_DataChangedEventElement_Using_CreateEventElement()
        {
            // Given: iot core instance 
            
            // When: DataElement created
            var dataelementid = Guid.NewGuid().ToString("N");

            var dataelement =
                testiotcore.ElementManager.CreateSimpleDataElement<string>(testiotcore.Root, dataelementid,
                    createGetDataServiceElement: false, createSetDataServiceElement: false, createDataChangedEventElement:true);
            
            // Then: event element is not created by default 
            Assert.That(testiotcore.ElementManager.GetElementByAddress($"/{dataelementid}/{Identifiers.DataChanged}"), Is.Not.Null);

            // When: datachangedevent element is created
            // Then: datachangedevent element is accessible. 
            Assert.That(dataelement.GetElementByIdentifier(Identifiers.DataChanged), Is.EqualTo(testiotcore.ElementManager.GetElementByAddress($"/{dataelementid}/{Identifiers.DataChanged}")));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        [Ignore("TODO: create failing test to demonstrate end-user confusions")]
        public void DataElement_Creation_Avoids_Creating_GetDataElement_Without_Handler()
        {
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        [Ignore("TODO: create failing test to demonstrate end-user confusions")]
        public void DataElement_Creation_Avoids_Creating_SetDataElement_Without_Handler()
        {
            Assert.Fail("TODO: create failing test");
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void DataElement_Creation_CanSetValue_getdata_accessible()
        {
            // Given: iot core instance 
            
            // When: DataElement created with default value
            var dataelementid = Guid.NewGuid().ToString("N");
            const int AnswerOfEverything = 42;
            testiotcore.ElementManager.CreateSimpleDataElement(testiotcore.Root, dataelementid, value: AnswerOfEverything);
            // Then: default value is immediately aviailable to iot core tree, getdata, setdata services
            Assert.That((int)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(1, $"/{dataelementid}/getdata").Data).Value, Is.EqualTo(AnswerOfEverything));
            
            // When: DataElement created with default value for another data type
            var dataelementid2 = Guid.NewGuid().ToString("N");
            const bool Yes = true;
            testiotcore.ElementManager.CreateSimpleDataElement(testiotcore.Root, dataelementid2, value: Yes);
            // Then: default value is immediately available to iot core tree, getdata, setdata services
            Assert.That((bool)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(1, $"/{dataelementid2}/getdata").Data).Value, Is.EqualTo(Yes));
        }
    }

    [TestFixture]
    class DataElement_ServiceMethod_Tests
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

        [Test, Property("TestCaseKey", "IOTCS-T41")]
        public void DataElement_GetDataMethod_invokes_getDataFuncHandler_WhichReturnsValue()
        {
            const string stringValue = "hellotest";
            const int intValue = 10;
            var stringDataElement = testiotcore.ElementManager.CreateReadOnlyDataElement(testiotcore.Root, "string1",
                (sender, i) => stringValue,
                format: new Int32Format());

            var int2DataElement = testiotcore.ElementManager.CreateReadOnlyDataElement(testiotcore.Root, "int1",
                (sender, i) => intValue,
                format: new StringFormat(new StringValuation(0, 100)));

            // When: DataElement has defined Value
            Assert.That(stringDataElement.Value, Is.EqualTo(stringValue));
            Assert.That(int2DataElement.Value, Is.EqualTo(intValue));

            // Then: getdata request returns the same thing as DataElement Value.
            Assert.That((string)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(1, "/string1/getdata").Data).Value, Is.EqualTo(stringValue));
            Assert.That((int)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(1, "/int1/getdata").Data).Value, Is.EqualTo(intValue));
        }

        [Test, Property("TestCaseKey", "IOTCS-T41")]
        public void DataElement_GetDataMethod_getDataFuncHandler_AnythingReturned_isGivenBack_toDataSide()
        {
            var nullDataElement = testiotcore.ElementManager.CreateSimpleDataElement<object>(testiotcore.Root, "null1");

            Assert.That(nullDataElement.Value, Is.Null);
            Assert.That(nullDataElement.Value, Is.EqualTo(null));

            Assert.That(Variant.ToObject<GetDataResponseServiceData>(testiotcore.MessageDispatcher.HandleRequest(1, "/null1/getdata").Data).Value, Is.EqualTo(null));
        }

        [Test, Property("TestCaseKey", "IOTCS-T46")]
        public void DataElement_SetData_NoHandler_SetValue_Success()
        {
            var testDataElement = testiotcore.ElementManager.CreateSimpleDataElement<string>(testiotcore.Root, "string1",
                format: new StringFormat(new StringValuation(0, 100)));

            const string stringValue = "hellotest";
            testDataElement.Value = stringValue;
            Assert.That((string)(VariantValue)testDataElement.Value == stringValue);
        }

        [Test, Property("TestCaseKey", "IOTCS-T46")]
        public void DataElement_SetDataMethod_invokes_setDataFuncHandler_WhichGetsNewValue()
        {
            object setDataValue = null;
            var testDataElement = testiotcore.ElementManager.CreateWriteOnlyDataElement<string>(testiotcore.Root, "string1",
                (senderElement, incomingValue, i) =>
                {
                    setDataValue = incomingValue;
                },
                format: new StringFormat(new StringValuation(0, 100)));

            Assert.That(setDataValue, Is.Null);

            const string stringValue = "hellotest";

            testiotcore.MessageDispatcher.HandleRequest(1, "/string1/setdata", new VariantObject {{"newvalue", new VariantValue($"{stringValue}")}}); 
            //testDataElement.Value = "hellotest";
            Assert.That(setDataValue, Is.EqualTo(stringValue));
        }

    }

    [TestFixture]
    class DataElement_Caching_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T187")]
        public void DataElement_Caching_getdata_BeforeTimeout_returns_CachedValue()
        {
            var backingValue = "initial_value";

            //Given: Dataelement with caching enabled with specific timeout

            var iotCore = Helpers.CreateIoTCore(Guid.NewGuid().ToString());
            

            var testDataElement = iotCore.ElementManager.CreateReadOnlyDataElement(null, "cached_element", (e, i) => backingValue,
                cacheTimeout: TimeSpan.FromMinutes(1));

            // First time getdata service is always called
            Assert.That(testDataElement.Value, Is.EqualTo("initial_value"));

            backingValue = "new_value";

            //When: getdata called on caching enabled dataelement before cache timeout
            //Then: cached value is used without using getdata handler
            Assert.That(testDataElement.Value, Is.EqualTo("initial_value"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T188")]
        public void DataElement_Caching_getdata_AfterTimeout_returns_RefreshedValue()
        {
            var backingValue = "initial_value";

            var iotCore = Helpers.CreateIoTCore(Guid.NewGuid().ToString());

            //Given: Dataelement with caching enabled with specific timeout
            var testDataElement = iotCore.ElementManager.CreateReadOnlyDataElement(null, "cached_element", (e, i) => backingValue,
                cacheTimeout: TimeSpan.FromMilliseconds(30));

            // First time getdata service is always called
            Assert.That(testDataElement.Value, Is.EqualTo("initial_value"));

            backingValue = "new_value";

            //When: getdata called on caching enabled dataelement after cache timeout
            //Then: new value is used using getdata handler
            System.Threading.Thread.Sleep(100);
            Assert.That(testDataElement.Value, Is.EqualTo("new_value"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T189"), Property("TestCaseKey", "IOTCS-T190")]
        public void DataElement_Caching_null_disables()
        {
            var backingValue = "initial_value";
            var iotCore = Helpers.CreateIoTCore(Guid.NewGuid().ToString());

            //Given: Dataelement with caching enabled with specific timeout
            var testDataElement = iotCore.ElementManager.CreateReadOnlyDataElement(null, "cached_element", (e, i) => backingValue);

            // First time getdata service is always called
            Assert.That(testDataElement.Value, Is.EqualTo("initial_value"));

            backingValue = "new_value";

            //When: getdata called on caching disbaled dataelement
            //Then: new value is used using getdata handler
            System.Threading.Thread.Sleep(100);
            Assert.That(testDataElement.Value, Is.EqualTo("new_value"));
        }
    }
}
