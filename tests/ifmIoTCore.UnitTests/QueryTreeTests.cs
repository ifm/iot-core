using ifmIoTCore.Converter.Json;
using ifmIoTCore.NetAdapter.Http;

namespace ifmIoTCore.UnitTests
{
    using System;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using NUnit.Framework;

    [TestFixture]
    public class QueryTreeTests
    {
        [Test]
        public void TestQueryTree()
        {
            var ioTCore1 = IoTCoreFactory.Create("id0", null);
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore1.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var ioTCore = ioTCore1;

            var structure0 = ioTCore.CreateStructureElement(ioTCore.Root, "structure0");
            var data0 = ioTCore.CreateDataElement<object>(structure0, "data0", (element) => { return null; });

            var testProfile = Guid.NewGuid().ToString();
            structure0.AddProfile(testProfile);

            var result = ioTCore.Root.QueryTree(new QueryTreeRequestServiceData(profile:testProfile));
            Assert.Contains(structure0.Address, result.Addresses);

            var result2 = ioTCore.Root.QueryTree(new QueryTreeRequestServiceData(profile: "notExisting"));

            Assert.AreEqual(0, result2.Addresses.Count);

            var result3 = ioTCore.Root.QueryTree(new QueryTreeRequestServiceData(type: "data"));
            Assert.Contains(data0.Address, result3.Addresses);

            var result4 = ioTCore.Root.QueryTree(new QueryTreeRequestServiceData());

            foreach (var item in ioTCore.Root.Subs)
            {
                Assert.Contains(item.Address, result4.Addresses);
            }
            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }
    }
}
