namespace ifm.IoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using ifm.Common.Variant;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ElementManager.Contracts.Elements.Valuations;
    using NUnit.Framework;

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
            if (obj.GetType() != GetType()) return false;
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
        [Test, Property("TestCaseKey", "IOTCS-T64")]
        public void getdatamulti_Responds_AddressCodeValue_ForDataElement_Single()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "int1", 42, format: new Int32Format());
            var getdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/getdatamulti", new VariantObject { { "datatosend", new VariantArray { new VariantValue("/int1") } } });
            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.That(getdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));

            Assert.That(getdatamultiResponseData.FirstOrDefault(x=>x.Key == "/int1"),Is.Not.Null);

            Assert.That(getdatamultiResponseData["/int1"].Code, Is.EqualTo(200));
            Assert.That((int)(VariantValue)getdatamultiResponseData["/int1"].Data, Is.EqualTo(42));
        }

        [Test, Property("TestCaseKey", "IOTCS-T65")]
        public void getdatamulti_Responds_AddressCodeValue_ForDataElements_10()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            for (var i = 1; i <= 5; i++)
                ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, string.Format("int{0}", i), 42, format: new Int32Format());

            for (var i = 1; i <= 3; i++)
                ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, string.Format("string{0}", i), "everything", format: new StringFormat());

            // complex type data element
            var intField = new ObjectValuation.Field("intField", new Int32Format(new Int32Valuation(-100, 100)));
            var floatField = new ObjectValuation.Field("floatField", new FloatFormat(new FloatValuation(-100.0f, 100.0f, 3)), optional: true);
            var stringField = new ObjectValuation.Field("stringField", new StringFormat(new StringValuation(10, 10, "dd-mm-yyyy")));
            var complextypeValn = new ObjectValuation(new List<ObjectValuation.Field> { intField, floatField, stringField });

            for (var i = 1; i <= 2; i++)
                ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, string.Format("complex{0}", i), new complexData(), format: new ObjectFormat(complextypeValn));

            
            var getdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/getdatamulti",
                new VariantObject
                {
                    {"datatosend" , new VariantArray
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
            Assert.That(getdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
            var multidata = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);
            Assert.Multiple(() =>
            {
                for (var i = 1; i <= 5; i++)
                {
                    Assert.That(multidata.FirstOrDefault(x=>x.Key == string.Format("$./int{0}", i)), Is.Not.Null);
                    Assert.That(multidata[string.Format("/int{0}", i)].Code, Is.EqualTo(ResponseCodes.Success));
                    Assert.That((int)(VariantValue)multidata[string.Format("/int{0}", i)].Data, Is.EqualTo(42));
                }

                for (var i = 1; i <= 3; i++)
                {
                    Assert.That(multidata.FirstOrDefault(x=>x.Key == string.Format("/string{0}", i)), Is.Not.Null);
                    Assert.That(multidata[string.Format("/string{0}", i)].Code, Is.EqualTo(ResponseCodes.Success));
                    Assert.That((string)(VariantValue)multidata[string.Format("/string{0}", i)].Data, Is.EqualTo("everything"));
                }

                for (var i = 1; i <= 2; i++)
                {
                    Assert.That(multidata.FirstOrDefault(x=>x.Key == string.Format("/complex{0}", i)), Is.Not.Null);
                    Assert.That(multidata[string.Format("/complex{0}", i)].Code, Is.EqualTo(ResponseCodes.Success));
                    Assert.That(multidata[string.Format("/complex{0}", i)].Data.AsVariantObject(), Is.EqualTo(Variant.FromObject(new complexData())));
                }

            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T66")]
        public void getdatamulti_404NotFound_ForUnknownDataElement()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var getdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/getdatamulti", new VariantObject { { "datatosend", new VariantArray { new VariantValue("/unknown") } } });
            Assert.That(getdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));

            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.That(getdatamultiResponseData.FirstOrDefault(x=>x.Key == "/unknown"),Is.Not.Null);
            
            Assert.That(getdatamultiResponseData["/unknown"].Code, Is.EqualTo(ResponseCodes.NotFound));
            Assert.That(getdatamultiResponseData["/unknown"].Data, Is.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T65")]
        public void getdatamulti_ForNoGetterDataElement_Success()
        { // TODO move this test script to getdata service tests where it belongs
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement<object>(ioTCore.Root, "int_nogetter");
            var getdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/getdatamulti",
                new VariantObject
                {
                    { "datatosend", new VariantArray { new VariantValue("/int_nogetter") } }
                });
            Assert.That(getdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));

            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.That(getdatamultiResponseData.FirstOrDefault(x=>x.Key  == "/int_nogetter"),Is.Not.Null);

            Assert.That(getdatamultiResponseData["/int_nogetter"].Code, Is.EqualTo(ResponseCodes.Success));
            Assert.That(getdatamultiResponseData["/int_nogetter"].Data, Is.Null);
        }

        [Test, Property("TestCaseKey", "IOTCS-T78")]
        public void getdatamulti_400BadRequest_ForNonDataElement()
        { // TODO move this test script to getdata service tests where it belongs

            

            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "nondataelement");
            var getdatamultiResponse = ioTCore.MessageDispatcher.HandleRequest(0,  "/getdatamulti",
                new VariantObject
                {
                    {"datatosend", new VariantArray { new VariantValue("/nondataelement") }}
                });
            Assert.That(getdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));

            var getdatamultiResponseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.That(getdatamultiResponseData.FirstOrDefault(x=>x.Key == "/nondataelement"),Is.Not.Null);
            Assert.That(getdatamultiResponseData["/nondataelement"].Code, Is.EqualTo(ResponseCodes.NotFound));
            Assert.That(getdatamultiResponseData["/nondataelement"].Data, Is.Null);
        }
    }
}

