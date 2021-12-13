namespace ifmIoTCore.UnitTests.Elements
{
using System;
using NUnit.Framework;

using ifmIoTCore.Elements;
using ifmIoTCore.Elements.Formats;
using ifmIoTCore.Elements.Valuations;
    using Newtonsoft.Json.Linq;
    using ifmIoTCore.Messages;

    [TestFixture]
    class DataElement_Creation_Tests
    {
        IIoTCore testiotcore;
        [OneTimeSetUp]
        public void CreateTestIotCore()
        {
            testiotcore = IoTCoreFactory.Create("testiotcore");
        }

        [OneTimeTearDown]
        public void DisposeTestIotCore()
        {
            testiotcore.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void Create_GetData_ServiceElement_UsingFlagIn_CreateDataEventElement()
        {
            // Given: iot core instance and DataElement is being created 

            // When: createGetDataServiceElement flag is set to false
            string dataelementid_NoGetData = Guid.NewGuid().ToString("N");
            testiotcore.CreateDataElement<string>(testiotcore.Root, dataelementid_NoGetData, createGetDataServiceElement: false);
            // Then: getdata service element is not created, dataelement/getdata returns 404 NotFound
            Assert.Null(testiotcore.GetElementByAddress($"/{dataelementid_NoGetData}/getdata"));
            Assert.That(testiotcore.HandleRequest(1,$"/{dataelementid_NoGetData}/getdata").Code, Is.EqualTo(ResponseCodes.NotFound));

            // When: createGetDataServiceElement flag is set to true
            string dataelementid_GetData = Guid.NewGuid().ToString("N");
            testiotcore.CreateDataElement<string>(testiotcore.Root, dataelementid_GetData, createGetDataServiceElement: true);
            // Then: getdata service element is created, dataelement/getdata returns 200 Ok 
            Assert.NotNull(testiotcore.GetElementByAddress($"/{dataelementid_GetData}/getdata"));
            Assert.That(testiotcore.HandleRequest(1, $"/{dataelementid_GetData}/getdata").Code, Is.EqualTo(ResponseCodes.Success));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void Create_SetData_ServiceElement_UsingFlagIn_CreateDataEventElement()
        {
            // Given: iot core instance and DataElement is being created 

            // When: createSetDataServiceElement flag is set to false
            string dataelementid_NoSetData = Guid.NewGuid().ToString("N");
            testiotcore.CreateDataElement<string>(testiotcore.Root, dataelementid_NoSetData, createSetDataServiceElement: false);
            // Then: setdata service element is not created, dataelement/setdata returns 404 NotFound
            Assert.Null(testiotcore.GetElementByAddress($"/{dataelementid_NoSetData}/setdata"));
            Assert.That(testiotcore.HandleRequest(1,$"/{dataelementid_NoSetData}/setdata").Code, Is.EqualTo(ResponseCodes.NotFound));

            // When: createSetDataServiceElement flag is set to true
            string dataelementid_SetData = Guid.NewGuid().ToString("N");
            testiotcore.CreateDataElement<string>(testiotcore.Root, dataelementid_SetData, createSetDataServiceElement: true);
            // Then: setdata service element is created, dataelement/setdata returns 200 Ok 
            Assert.NotNull(testiotcore.GetElementByAddress($"/{dataelementid_SetData}/setdata"));
            Assert.That(testiotcore.HandleRequest(1,$"/{dataelementid_SetData}/setdata", JToken.Parse("{'newvalue':null}")).Code, Is.EqualTo(ResponseCodes.Success));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void Create_DataChangedEventElement_Using_CreateEventElement()
        {
            // Given: iot core instance 
            
            // When: DataElement created
            string dataelementid = Guid.NewGuid().ToString("N");
            var dataelement = testiotcore.CreateDataElement<string>(testiotcore.Root, dataelementid);
            // Then: event element is not created by default 
            Assert.Null(testiotcore.GetElementByAddress($"/{dataelementid}/{Identifiers.DataChanged}"));

            // When: datachangedevent element is created
            dataelement.DataChangedEventElement = testiotcore.CreateEventElement(dataelement, Identifiers.DataChanged);
            // Then: datachangedevent element is accessible. 
            Assert.AreEqual(dataelement.DataChangedEventElement, testiotcore.GetElementByAddress($"/{dataelementid}/{Identifiers.DataChanged}"));
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

    }

    [TestFixture]
    class DataElement_ServiceMethod_Tests
    {
        IIoTCore testiotcore;

        [SetUp]
        public void CreateIotCoreInstance()
        {
            testiotcore = IoTCoreFactory.Create("testiotcore", null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T41")]
        public void DataElement_GetDataMethod_invokes_getDataFuncHandler_WhichReturnsValue()
        {
            const string stringValue = "hellotest";
            const int intValue = 10;
            var stringDataElement = testiotcore.CreateDataElement<string>(testiotcore.Root, "string1",
                (sender) => stringValue,
                format: new IntegerFormat(new IntegerValuation(0)));
            var int2DataElement = testiotcore.CreateDataElement<int>(testiotcore.Root, "int1",
                (sender) => intValue,
                format: new StringFormat(new StringValuation(0, 100)));

            // When: DataElement has defined Value
            Assert.That(stringDataElement.Value, Is.EqualTo(stringValue));
            Assert.That(int2DataElement.Value, Is.EqualTo(intValue));

            // Then: getdata request returns the same thing as DataElement Value.
            Assert.That(testiotcore.HandleRequest(1, "/string1/getdata").Data.Value<string>("value"), Is.EqualTo(stringValue));
            Assert.That(testiotcore.HandleRequest(1, "/int1/getdata").Data.Value<int>("value"), Is.EqualTo(intValue));
        }

        [Test, Property("TestCaseKey", "IOTCS-T41")]
        public void DataElement_GetDataMethod_getDataFuncHandler_AnythingReturned_isGivenBack_toDataSide()
        {
            var nullDataElement = testiotcore.CreateDataElement<object>(testiotcore.Root, "null1",
                (s) => null, 
                format: new StringFormat(new StringValuation(0, 100)));
            Assert.That(nullDataElement.Value, Is.EqualTo(null));
            Assert.That(testiotcore.HandleRequest(1, "/null1/getdata").Data.Value<string>("value"), Is.EqualTo(null));
        }

        [Test, Property("TestCaseKey", "IOTCS-T46")]
        public void DataElement_SetData_NoHandler_SetValue_Success()
        {
            var testDataElement = testiotcore.CreateDataElement<string>(testiotcore.Root, "string1",
                format: new StringFormat(new StringValuation(0, 100)));

            const string stringValue = "hellotest";
            testDataElement.Value = stringValue;
            Assert.True(testDataElement.Value == stringValue);
        }

        [Test, Property("TestCaseKey", "IOTCS-T46")]
        public void DataElement_SetDataMethod_invokes_setDataFuncHandler_WhichGetsNewValue()
        {
            object setDataValue = null;
            var testDataElement = testiotcore.CreateDataElement<object>(testiotcore.Root, "string1",
                null,
                (senderElement, incomingValue) =>
                {
                    setDataValue = incomingValue;
                },
                format: new StringFormat(new StringValuation(0, 100)));
            Assert.IsNull(setDataValue);

            const string stringValue = "hellotest";
            testiotcore.HandleRequest(1, "/string1/setdata",JToken.Parse($"{{'newvalue':'{stringValue}'}}")); //testDataElement.Value = "hellotest";
            Assert.That(setDataValue as string, Is.EqualTo(stringValue));
        }

    }

    [TestFixture]
    class DataElement_Caching_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T187")]
        public void DataElement_Caching_getdata_BeforeTimeout_returns_CachedValue()
        {
            //Given: Dataelement with caching enabled with specific timeout
            const string elementName = "cached_element";
            var testDataElement = new DataElement<string>(null, elementName,
                sender => "getdata_called",
                null,
                format: new StringFormat(new StringValuation(0, 100)),
                cacheTimeout: TimeSpan.FromMinutes(1));

            //Given: first time cache is loaded 
            testDataElement.Value = elementName;

            //When: getdata called on caching enabled dataelement before cache timeout
            //Then: cached value is used without using getdata handler
            var beforeTimeout = testDataElement.Value;
            Assert.That(beforeTimeout, Is.Not.EqualTo("getdata_called"));
            Assert.That(beforeTimeout, Is.EqualTo(elementName)); // extra assert for readability
        }

        [Test, Property("TestCaseKey", "IOTCS-T188")]
        public void DataElement_Caching_getdata_AfterTimeout_returns_RefreshedValue()
        {
            //Given: Dataelement with caching enabled with specific timeout
            const string elementName = "cached_element";
            var testDataElement = new DataElement<string>(null, elementName,
                sender => "getdata_called",
                format: new StringFormat(new StringValuation(0, 100)),
                cacheTimeout: TimeSpan.FromMilliseconds(100));

            //Given: first time cache is loaded 
            testDataElement.Value = elementName;

            //When: getdata called on caching enabled dataelement after cache timeout
            //Then: refreshed value is returned using getdata handler
            System.Threading.Thread.Sleep(100);
            var afterTimeout = testDataElement.Value;
            Assert.That(afterTimeout, Is.EqualTo("getdata_called"));
            Assert.That(afterTimeout, Is.Not.EqualTo(elementName)); // extra assert for readability
        }

        [Test, Property("TestCaseKey", "IOTCS-T189"), Property("TestCaseKey", "IOTCS-T190")]
        public void DataElement_Caching_enabled_per_element_null_disables()
        {
            //Given: Dataelement with caching enabled with specific timeout
            const string elementName = "cached_element";
            var cachedElement = new DataElement<string>(null, elementName,
                sender => "getdata_called",
                format: new StringFormat(new StringValuation(0, 100)),
                cacheTimeout: TimeSpan.FromMinutes(1));

            //Given: Dataelement with caching disabled using null
            const string elementName2 = "uncached_element";
            var uncachedelementvalue = elementName2;
            var uncachedElement = new DataElement<string>(null, elementName2,
                sender => uncachedelementvalue,
                (sender,value) => { uncachedelementvalue = value; },
                format: new StringFormat(new StringValuation(0, 100)));

            //Given: first time cache is loaded for cached element
            cachedElement.Value = elementName;
            //When: getdata called on cached element before timeout
            //Then: cached value is returned without using getdata handler
            var afterTimeout = cachedElement.Value;
            Assert.That(afterTimeout, Is.Not.EqualTo("getdata_called"));

            //When: uncached element value is updated (setdata) and retrieved (getdata)
            //Then: latest value (not the cached value) is returned using getdata handler
            uncachedElement.Value = "1";
            Assert.That(uncachedElement.Value, Is.EqualTo("1"));
        }

    }
}
