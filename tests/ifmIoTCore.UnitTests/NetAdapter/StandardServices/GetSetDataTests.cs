using ifmIoTCore.Converter.Json;
using ifmIoTCore.NetAdapter.Http;

namespace ifmIoTCore.UnitTests.NetAdapter.StandardServices
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
                var ioTCore1 = IoTCoreFactory.Create("id0", null);
                using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
                ioTCore1.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
                var ioTCore = ioTCore1;
                
                var dataElement = ioTCore.CreateDataElement<string>(ioTCore.Root, "data0", (b) => "data123");

                var getDataResponse = ioTCore.HandleRequest(0, "/data0/getdata");
                Assert.NotNull(getDataResponse);
                Assert.AreEqual(200, getDataResponse.Code);

                var data = getDataResponse.Data.ToObject<GetDataResponseServiceData>();

                Assert.That(string.Equals("data123", data.GetValue<string>()));
                ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
                clientNetAdapterFactory.Dispose();
                
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
            
            var dataElement = new DataElement<string>(null, "data0",
                (b) => data,
                (b, o) => { data = o;});

            dataElement.Value = "asdf";
            var result = dataElement.Value;
            
            Assert.That(string.Equals("asdf", result));
        }

    }
}