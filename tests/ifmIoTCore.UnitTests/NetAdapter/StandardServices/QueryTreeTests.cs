using ifmIoTCore.Converter.Json;
using ifmIoTCore.NetAdapter.Http;

namespace ifmIoTCore.UnitTests.NetAdapter.StandardServices
{
    using System.Linq;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using NUnit.Framework;

    [TestFixture]
    public class QueryTreeTests
    {
        [Test]
        public void TestQueryTreeByName()
        {
            var ioTCore1 = IoTCoreFactory.Create("id0", null);
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore1.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var ioTCore = ioTCore1;
            var structure = ioTCore.CreateStructureElement(ioTCore.Root, "someidentifier");

            var result = ioTCore.Root.QueryTree(new QueryTreeRequestServiceData(name: "someidentifier"));
            var foundItem = result.Addresses.FirstOrDefault();

            Assert.NotNull(foundItem);
            Assert.AreEqual(structure.Address, foundItem);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }
    }
}
