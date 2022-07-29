using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements.ServiceData.Responses;

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
        public void ReadOnlyDataElement_CreatesAndAccepts_getdata_No_setdata()
        {
            // Given: iot core instantiated
            // When: readonlydataelement created with getdatafunc
            var dataelementId = Guid.NewGuid().ToString("N");
            const int everything = 42;
            var readonlyDataElement = testiotcore.Root.AddChild(new ReadOnlyDataElement<int>(dataelementId, getDataFunc: (s)=>everything));
            // Then: dataelement/getdata request works as expected
            Assert.That(testiotcore.HandleRequest(1,$"/{dataelementId}/getdata").Code, Is.EqualTo(ResponseCodes.Success));
            Assert.That(testiotcore.HandleRequest(1, $"/{dataelementId}/getdata").Data.AsVariantObject()["value"].ToObject<int>(), Is.EqualTo(everything));
            // Then: dataelement/setdata request gives error 404 NotFound
            Assert.That(testiotcore.HandleRequest(1,$"/{dataelementId}/setdata", new VariantValue(everything+1)).Code, Is.EqualTo(ResponseCodes.NotFound));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void WriteOnlyDataElement_CreatesAndAccepts_getdata_No_setdata()
        {
            // Given: iot core instaitated 
            // When: writeonlydataelement created with setdatafunc
            var dataelementId = Guid.NewGuid().ToString("N");
            int setvalue = 0; const int everything = 42;
            var writeonlyDataElement = testiotcore.Root.AddChild(new WriteOnlyDataElement<int>(dataelementId, setDataFunc: (s, p) => setvalue = p));

            // Then: dataelement/setdata request works as expected
            var response = testiotcore.HandleRequest(1, $"/{dataelementId}/setdata", data:new VariantObject(){{"value", new VariantValue(everything)}});
            Assert.That(response.Code, Is.EqualTo(ResponseCodes.Success));
            Assert.That(setvalue, Is.EqualTo(everything));
            // Then: dataelement/getdata request gives error 404 NotFound
            Assert.That(testiotcore.HandleRequest(1,$"/{dataelementId}/getdata").Code, Is.EqualTo(ResponseCodes.NotFound));
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void Create_DataChangedEventElement_Using_CreateEventElement()
        {
            // Given: iot core instance 
            
            // When: DataElement created
            string dataelementid = Guid.NewGuid().ToString("N");
            var dataelement = new DataElement<string>(dataelementid, null, null, createDataChangedEventElement: true);
            testiotcore.Root.AddChild(dataelement, true);
            // Then: event element is not created by default 
            Assert.NotNull(testiotcore.GetElementByAddress($"/{dataelementid}/{Identifiers.DataChanged}"));

            // When: datachangedevent element is created
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

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void DataElement_Creation_CanSetValue_getdata_accessible()
        {
            // Given: iot core instance 
            
            // When: DataElement created with default value
            string dataelementid = Guid.NewGuid().ToString("N");
            const int AnswerOfEverything = 42;
            testiotcore.Root.AddChild(new DataElement<int>(dataelementid, value: AnswerOfEverything), true);
            // Then: default value is immediately aviailable to iot core tree, getdata, setdata services
            Assert.That((int)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(1, $"/{dataelementid}/getdata").Data).Value, Is.EqualTo(AnswerOfEverything));
            
            // When: DataElement created with default value for another data type
            string dataelementid2 = Guid.NewGuid().ToString("N");
            const bool Yes = true;
            testiotcore.Root.AddChild(new DataElement<bool>(dataelementid2, value: Yes), true);
            // Then: default value is immediately available to iot core tree, getdata, setdata services
            Assert.That((bool)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(1, $"/{dataelementid2}/getdata").Data).Value, Is.EqualTo(Yes));
        }
    }

    [TestFixture]
    class DataElement_ServiceMethod_Tests
    {
        IIoTCore testiotcore;

        [SetUp]
        public void CreateIotCoreInstance()
        {
            testiotcore = IoTCoreFactory.Create("testiotcore");
        }

        [Test, Property("TestCaseKey", "IOTCS-T41")]
        public void DataElement_GetDataMethod_invokes_getDataFuncHandler_WhichReturnsValue()
        {
            const string stringValue = "hellotest";
            const int intValue = 10;
            var stringDataElement = new ReadOnlyDataElement<string>("string1",
                (sender) => stringValue,
                format: new IntegerFormat(new IntegerValuation(0)));
            testiotcore.Root.AddChild(stringDataElement, true);

            var int2DataElement = new ReadOnlyDataElement<int>("int1",
                (sender) => intValue,
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(int2DataElement, true);

            // When: DataElement has defined Value
            Assert.That((string)stringDataElement.Value.AsVariantValue(), Is.EqualTo(stringValue));
            Assert.That((int)int2DataElement.Value.AsVariantValue(), Is.EqualTo(intValue));

            // Then: getdata request returns the same thing as DataElement Value.
            Assert.That((string)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(1, "/string1/getdata").Data).Value, Is.EqualTo(stringValue));
            Assert.That((int)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(1, "/int1/getdata").Data).Value, Is.EqualTo(intValue));
        }

        [Test, Property("TestCaseKey", "IOTCS-T41")]
        public void DataElement_GetDataMethod_getDataFuncHandler_AnythingReturned_isGivenBack_toDataSide()
        {
            var nullDataElement = new ReadOnlyDataElement<object>("null1",
                (s) => null,
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(nullDataElement, true);

            Assert.AreEqual(nullDataElement.Value, null);

            Assert.That((string)(VariantValue)Variant.ToObject<GetDataResponseServiceData>(testiotcore.HandleRequest(1, "/null1/getdata").Data).Value, Is.EqualTo(null));
        }

        [Test, Property("TestCaseKey", "IOTCS-T46")]
        public void DataElement_SetData_NoHandler_SetValue_Success()
        {
            var testDataElement = new DataElement<string>("string1",
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(testDataElement);

            const string stringValue = "hellotest";
            testDataElement.Value = (VariantValue)stringValue;
            Assert.True((string)(VariantValue)testDataElement.Value == stringValue);
        }

        [Test, Property("TestCaseKey", "IOTCS-T46")]
        public void DataElement_SetDataMethod_invokes_setDataFuncHandler_WhichGetsNewValue()
        {
            object setDataValue = null;
            var testDataElement = new WriteOnlyDataElement<string>("string1",
                (senderElement, incomingValue) =>
                {
                    setDataValue = incomingValue;
                },
                format: new StringFormat(new StringValuation(0, 100)));
            testiotcore.Root.AddChild(testDataElement, true);

            Assert.IsNull(setDataValue);

            const string stringValue = "hellotest";

            testiotcore.HandleRequest(1, "/string1/setdata", new VariantObject() {{"newvalue", new VariantValue($"{stringValue}")}}); 
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
            //Given: Dataelement with caching enabled with specific timeout
            const string elementName = "cached_element";
            var testDataElement = new DataElement<string>(elementName, sender => "getdata_called", 
                null,
                format: new StringFormat(new StringValuation(0, 100)),
                cacheTimeout: TimeSpan.FromMinutes(1));

            //Given: first time cache is loaded 
            testDataElement.Value = (VariantValue)elementName;

            //When: getdata called on caching enabled dataelement before cache timeout
            //Then: cached value is used without using getdata handler
            var beforeTimeout = testDataElement.Value;
            Assert.That((string)beforeTimeout.AsVariantValue(), Is.Not.EqualTo("getdata_called"));
            Assert.That((string)beforeTimeout.AsVariantValue(), Is.EqualTo(elementName)); // extra assert for readability
        }

        [Test, Property("TestCaseKey", "IOTCS-T188")]
        public void DataElement_Caching_getdata_AfterTimeout_returns_RefreshedValue()
        {
            //Given: Dataelement with caching enabled with specific timeout
            const string elementName = "cached_element";
            var testDataElement = new DataElement<string>(elementName, sender => "getdata_called",
                null,
                format: new StringFormat(new StringValuation(0, 100)),
                cacheTimeout: TimeSpan.FromMilliseconds(100));

            //Given: first time cache is loaded 
            testDataElement.Value = (VariantValue)elementName;

            //When: getdata called on caching enabled dataelement after cache timeout
            //Then: refreshed value is returned using getdata handler
            System.Threading.Thread.Sleep(100);
            var afterTimeout = testDataElement.Value;
            Assert.That((string)afterTimeout.AsVariantValue(), Is.EqualTo("getdata_called"));
            Assert.That((string)afterTimeout.AsVariantValue(), Is.Not.EqualTo(elementName)); // extra assert for readability
        }

        [Test, Property("TestCaseKey", "IOTCS-T189"), Property("TestCaseKey", "IOTCS-T190")]
        public void DataElement_Caching_enabled_per_element_null_disables()
        {
            //Given: Dataelement with caching enabled with specific timeout
            const string elementName = "cached_element";
            var cachedElement = new DataElement<string>(elementName, sender => "getdata_called",
                null,
                format: new StringFormat(new StringValuation(0, 100)),
                cacheTimeout: TimeSpan.FromMinutes(1));

            //Given: Dataelement with caching disabled using null
            const string elementName2 = "uncached_element";
            var uncachedelementvalue = elementName2;
            var uncachedElement = new DataElement<string>(elementName2, sender => uncachedelementvalue,
                (sender,value) => { uncachedelementvalue = value; },
                
                format: new StringFormat(new StringValuation(0, 100)));

            //Given: first time cache is loaded for cached element
            cachedElement.Value = (VariantValue)elementName;
            //When: getdata called on cached element before timeout
            //Then: cached value is returned without using getdata handler
            var afterTimeout = cachedElement.Value;
            Assert.That((string)afterTimeout.AsVariantValue(), Is.Not.EqualTo("getdata_called"));

            //When: uncached element value is updated (setdata) and retrieved (getdata)
            //Then: latest value (not the cached value) is returned using getdata handler
            uncachedElement.Value = (VariantValue)"1";
            Assert.That((string)uncachedElement.Value.AsVariantValue(), Is.EqualTo("1"));
        }

    }
}
