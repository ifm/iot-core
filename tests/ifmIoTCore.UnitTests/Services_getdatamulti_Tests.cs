using System.Linq;
using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Utilities;

    public class complexData: IEquatable<complexData>
    {
        public int int1 { get; set; }
        public float float1 { get; set; }
        public string string1 { get; set; }

        public complexData()
        {
            
        }

        public complexData(int newint = 42, float newfloat = 42f, string newstring = "everything")
        {
            int1 = newint; float1 = newfloat; string1 = newstring;
        }
        
        public bool Equals(complexData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return int1 == other.int1 && float1.Equals(other.float1) && string1 == other.string1;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((complexData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = int1;
                hashCode = (hashCode * 397) ^ float1.GetHashCode();
                hashCode = (hashCode * 397) ^ (string1 != null ? string1.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_getdatamulti_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T63")]
        public void getdatamulti_ErrorResponseOnInvalidation_datatosend_missingOrNull()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore");

            var getDataMultiService = ioTCore.Root.GetDataMultiServiceElement;

            Assert.Throws<IoTCoreException>(() => getDataMultiService.Invoke(Variant.FromObject(new GetDataMultiRequestServiceData(null))));
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.That(ioTCore.HandleRequest(0, "/getdatamulti").Code, Is.EqualTo(ResponseCodes.DataInvalid));
            Assert.That(ioTCore.HandleRequest(0, "/getdatamulti", new VariantObject { { "datatosend", VariantValue.CreateNull() } }).Code, Is.EqualTo(ResponseCodes.DataInvalid));
        }

        [Test, Property("TestCaseKey", "IOTCS-T63")]
        public void getdatamulti_Ok200ResponseOn_empty_datatosend_list()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore");

            var getDataMultiService = ioTCore.Root.GetDataMultiServiceElement;

            var result = getDataMultiService.Invoke(Variant.FromObject(new GetDataMultiRequestServiceData(new List<string>())));
            var resultData = Variant.ToObject<GetDataMultiResponseServiceData>(result);

            Assert.That(resultData, Is.Empty);
            Assert.AreEqual(200, ResponseCodes.Success);

            var response = ioTCore.HandleRequest(0, "/getdatamulti", new VariantObject() { { "datatosend", new VariantArray() } });
            Assert.That(response.Code, Is.EqualTo(ResponseCodes.Success));
        }

        [Test, Property("TestCaseKey", "IOTCS-T64")]
        public void getdatamulti_Responds_AddressCodeValue_ForDataElement_Single()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new ReadOnlyDataElement<int>("int1",
                    (s) => { return 42; },
                    format: new IntegerFormat(new IntegerValuation(0))), true);
            Assert.AreEqual(200, ResponseCodes.Success);
            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti", new VariantObject() { { "datatosend", new VariantArray() { new VariantValue("/int1") } } });
            var getdatamultiResponseData = VariantObject.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);

            Assert.NotNull(getdatamultiResponseData.FirstOrDefault(x=>x.Key == "/int1"));

            Assert.AreEqual(200, getdatamultiResponseData["/int1"].Code);
            Assert.AreEqual(42, (int)(VariantValue)getdatamultiResponseData["/int1"].Data);
        }

        [Test, Property("TestCaseKey", "IOTCS-T65")]
        public void getdatamulti_Responds_AddressCodeValue_ForDataElements_10()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            for (var i = 1; i <= 5; i++)
                ioTCore.Root.AddChild(new ReadOnlyDataElement<int>(string.Format("int{0}", i),
                        (s) => { return 42; },
                        format: new IntegerFormat(new IntegerValuation(0))), true);

            for (var i = 1; i <= 3; i++)
                ioTCore.Root.AddChild(new ReadOnlyDataElement<string>(string.Format("string{0}", i),
                        (s) => { return "everything"; },
                        format: new StringFormat(new StringValuation(""))), true);

            // complex type data element
            var intField = new Field("intField", new IntegerFormat(new IntegerValuation(-100, 100)));
            var floatField = new Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<Field> { intField, floatField, stringField });

            for (var i = 1; i <= 2; i++)
                ioTCore.Root.AddChild(new ReadOnlyDataElement<complexData>(string.Format("complex{0}", i),
                        (s) => { return new complexData(); },
                        
                        format: new Format("customtype", "customencoding", complextypeValn)), true);

            

            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti",
                new VariantObject()
                {
                    {"datatosend" , new VariantArray()
                    {
                        new VariantValue("/int1"),
                        new VariantValue("/int2"),
                        new VariantValue("/int3"),
                        new VariantValue("/int4"),
                        new VariantValue("/int5"),
                        new VariantValue("/string1"),
                        new VariantValue("/string2"),
                        new VariantValue("/string3"),
                        new VariantValue("/complex1"),
                        new VariantValue("/complex2")

                    }}
                });
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            var multidata = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);
            Assert.Multiple(() =>
            {
                for (var i = 1; i <= 5; i++)
                {
                    Assert.IsNotNull(multidata.FirstOrDefault(x=>x.Key == string.Format("$./int{0}", i)));
                    Assert.AreEqual(ResponseCodes.Success, multidata[string.Format("/int{0}", i)].Code);
                    Assert.AreEqual(42, (int)(VariantValue)multidata[string.Format("/int{0}", i)].Data);
                }

                for (var i = 1; i <= 3; i++)
                {
                    Assert.IsNotNull(multidata.FirstOrDefault(x=>x.Key == string.Format("/string{0}", i)));
                    Assert.AreEqual(ResponseCodes.Success, multidata[string.Format("/string{0}", i)].Code);
                    Assert.AreEqual("everything" , (string)(VariantValue)multidata[string.Format("/string{0}", i)].Data);
                }

                for (var i = 1; i <= 2; i++)
                {
                    Assert.IsNotNull(multidata.FirstOrDefault(x=>x.Key == string.Format("/complex{0}", i)));
                    Assert.AreEqual(ResponseCodes.Success, multidata[string.Format("/complex{0}", i)].Code);
                    Assert.AreEqual(Variant.FromObject(new complexData()), multidata[string.Format("/complex{0}", i)].Data.AsVariantObject());
                }

            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T66")]
        public void getdatamulti_404NotFound_ForUnknownDataElement()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti", new VariantObject() { { "datatosend", new VariantArray() { new VariantValue("/unknown") } } });
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.AreEqual(404, ResponseCodes.NotFound);

            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.NotNull(getdatamultiResponseData.FirstOrDefault(x=>x.Key == "/unknown"));
            
            Assert.AreEqual(ResponseCodes.NotFound, getdatamultiResponseData["/unknown"].Code);
            Assert.AreEqual(VariantValue.CreateNull(), getdatamultiResponseData["/unknown"].Data);
        }

        [Test, Property("TestCaseKey", "IOTCS-T65")]
        public void getdatamulti_ForNoGetterDataElement_Success()
        { // TODO move this test script to getdata service tests where it belongs
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new DataElement<object>("int_nogetter"), true);
            var getdatamultiResponse = ioTCore.HandleRequest(0, "/getdatamulti",
                new VariantObject()
                {
                    { "datatosend", new VariantArray() { new VariantValue("/int_nogetter") } }
                });
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.AreEqual(501, ResponseCodes.NotImplemented);

            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.NotNull(getdatamultiResponseData.FirstOrDefault(x=>x.Key  == "/int_nogetter"));

            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponseData["/int_nogetter"].Code);
            Assert.AreEqual(VariantValue.CreateNull(), getdatamultiResponseData["/int_nogetter"].Data);
        }

        [Test, Property("TestCaseKey", "IOTCS-T78")]
        public void getdatamulti_400BadRequest_ForNonDataElement()
        { // TODO move this test script to getdata service tests where it belongs

            

            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new StructureElement("nondataelement"), true);
            var getdatamultiResponse = ioTCore.HandleRequest(0,  "/getdatamulti",
                new VariantObject()
                {
                    {"datatosend", new VariantArray() { new VariantValue("/nondataelement") }}
                });
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);
            Assert.AreEqual(400, ResponseCodes.BadRequest);

            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.NotNull(getdatamultiResponseData.FirstOrDefault(x=>x.Key == "/nondataelement"));
            Assert.AreEqual(ResponseCodes.BadRequest, getdatamultiResponseData["/nondataelement"].Code);
            Assert.AreEqual(VariantValue.CreateNull(), getdatamultiResponseData["/nondataelement"].Data);
        }
    }
}

