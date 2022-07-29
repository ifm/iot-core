using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Linq;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using NUnit.Framework;
    using Utilities;

    [TestFixture]
    public class QueryTreeTests
    {
        [Test]
        public void TestQueryTree()
        {
            var ioTCore = IoTCoreFactory.Create("id0");

            var structure0 = new StructureElement("structure0");
            ioTCore.Root.AddChild(structure0);
            var data0 = new ReadOnlyDataElement<object>("data0", (element) => { return null; });
            structure0.AddChild(data0);

            var testProfile = Guid.NewGuid().ToString();
            structure0.AddProfile(testProfile);

            var queryTreeService = ioTCore.Root.QueryTreeServiceElement;


            var queryResult1 = queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(profile: testProfile)));
            var queryResult1Data = Variant.ToObject<QueryTreeResponseServiceData>(queryResult1);
            Assert.Contains(structure0.Address, queryResult1Data.Addresses);

            var queryResult2 = queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(profile: "notExisting")));
            var queryResult2Data = Variant.ToObject<QueryTreeResponseServiceData>(queryResult2);
            Assert.AreEqual(0, queryResult2Data.Addresses.Count);

            var queryResult3 = queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(type: "data")));
            var queryResult3Data = Variant.ToObject<QueryTreeResponseServiceData>(queryResult3);
            Assert.Contains(data0.Address, queryResult3Data.Addresses);

            var queryResult4 = queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData()));
            var queryResult4Data = Variant.ToObject<QueryTreeResponseServiceData>(queryResult4);
            foreach (var item in ioTCore.Root.Subs)
            {
                Assert.Contains(item.Address, queryResult4Data.Addresses);
            }
        }

        [Test]
        public void TestQueryTreeByName()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var structure = new StructureElement("someidentifier");
            ioTCore.Root.AddChild(structure);

            var queryTreeService = ioTCore.Root.QueryTreeServiceElement;

            var result = queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(identifier: "someidentifier")));
            var resultData = Variant.ToObject<QueryTreeResponseServiceData>(result);
            var foundItem = resultData.Addresses.FirstOrDefault();

            Assert.NotNull(foundItem);
            Assert.AreEqual(structure.Address, foundItem);
        }
    }
}
