using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using NUnit.Framework;

    [ExcludeFromCodeCoverage]
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class GetSetDataTests
    {
        [Test]
        public void LocalGetDataTest()
        {
            try
            {
                var ioTCore1 = IoTCoreFactory.Create("id0");
                var ioTCore = ioTCore1;
                
                var dataElement = new ReadOnlyDataElement<string>("data0", (b) => "data123");
                ioTCore.Root.AddChild(dataElement, true); 

                var getDataResponse = ioTCore.HandleRequest(0, "/data0/getdata");
                Assert.NotNull(getDataResponse);
                Assert.AreEqual(200, getDataResponse.Code);

                var data = ifmIoTCore.Common.Variant.Variant.ToObject<GetDataResponseServiceData>(getDataResponse.Data);

                Assert.That(string.Equals("data123", (string)(VariantValue)data.Value));
            }
            catch (Exception exception)
            {
                Assert.Fail($"An exeption occured. The message was: {exception.Message}");
            }
        }

        [Test]
        public void LocalSetDataTest()
        {
            string data = null;
            
            var dataElement = new DataElement<string>("data0",
                (b) => data,
                (b, o) => { data = o;});

            dataElement.Value = (VariantValue)"asdf";
            var result = dataElement.Value;
            
            Assert.That(string.Equals("asdf", (string)result.AsVariantValue()));
        }

    }
}