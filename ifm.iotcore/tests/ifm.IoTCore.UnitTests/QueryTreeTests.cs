namespace ifm.IoTCore.UnitTests
{
    using System;
    using System.Linq;
    using ElementManager.Contracts.Elements;
    using ElementManager.Contracts.Elements.ServiceData.Requests;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using NUnit.Framework;
    
    [TestFixture]
    public class QueryTreeTests
    {
        [Test]
        public void TestQueryTree()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");

            var structure0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "structure0");
            var data0 = ioTCore.ElementManager.CreateSimpleDataElement<object>(structure0, "data0");

            var testProfile = Guid.NewGuid().ToString();
            structure0.AddProfile(testProfile);

            var queryTreeService = ioTCore.Root.Subs.Single(x => x.Identifier == "querytree") as IServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData>;

            var queryResult1 = queryTreeService.Invoke(new QueryTreeRequestServiceData(profile: testProfile));
            Assert.That(queryResult1.Addresses, Does.Contain(structure0.Address));

            var queryResult2 = queryTreeService.Invoke(new QueryTreeRequestServiceData(profile: "notExisting"));
            Assert.That(queryResult2.Addresses.Count, Is.EqualTo(0));

            var queryResult3 = queryTreeService.Invoke(new QueryTreeRequestServiceData(type: "data"));
            Assert.That(queryResult3.Addresses, Does.Contain(data0.Address));

            var queryResult4 = queryTreeService.Invoke(new QueryTreeRequestServiceData());
            foreach (var item in ioTCore.Root.Subs)
            {
                if (item.IsHidden) continue;
                Assert.That(queryResult4.Addresses, Does.Contain(item.Address));
            }
        }

        [Test]
        public void TestQueryTreeByName()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");

            var queryTreeService = ioTCore.Root.Subs.Single(x => x.Identifier == "querytree") as IServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData>;

            var structure = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "someidentifier");

            var result = queryTreeService.Invoke(new QueryTreeRequestServiceData(identifier: "someidentifier"));
            var foundItem = result.Addresses.FirstOrDefault();

            Assert.That(foundItem,Is.Not.Null);
            Assert.That(structure.Address, Is.EqualTo(foundItem));
        }
    }
}
