using ifmIoTCore.Converter.Json;
using ifmIoTCore.NetAdapter.Http;

namespace ifmIoTCore.UnitTests
{
    using System.Linq;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using NUnit.Framework;
  
    [TestFixture]
    public class SubTreeTests
    {
        [Test]
        public void TestSubTreeLevel4()
        {
            var ioTCore1 = IoTCoreFactory.Create("id0", null);
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore1.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var ioTCore = ioTCore1;
            var structureElementLevel1 = ioTCore.CreateStructureElement(ioTCore.Root, "level1");
            var structureElementLevel2 = ioTCore.CreateStructureElement(structureElementLevel1, "level2");
            var structureElementLevel3 = ioTCore.CreateStructureElement(structureElementLevel2, "level3");
            var structureElementLevel4 = ioTCore.CreateStructureElement(structureElementLevel3, "level4");

            var result = ioTCore.Root.GetTree(new GetTreeRequestServiceData("/", null));

            var element4 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2").Subs
                .First(x => x.Identifier == "level3").Subs.First(x => x.Identifier == "level4");
            
            Assert.NotNull(element4);

            Assert.AreEqual(structureElementLevel4.Address, element4.Address);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test]
        public void TestSubTreeLevel3()
        {
            var ioTCore1 = IoTCoreFactory.Create("id0", null);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore1.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var ioTCore = ioTCore1;
            var structureElementLevel1 = ioTCore.CreateStructureElement(ioTCore.Root, "level1");
            var structureElementLevel2 = ioTCore.CreateStructureElement(structureElementLevel1, "level2");
            var structureElementLevel3 = ioTCore.CreateStructureElement(structureElementLevel2, "level3");
            var structureElementLevel4 = ioTCore.CreateStructureElement(structureElementLevel3, "level4");

            var result = ioTCore.Root.GetTree(new GetTreeRequestServiceData("/", 3));

            var element3 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2")
                .Subs
                .FirstOrDefault(x => x.Identifier == "level3");

            Assert.NotNull(element3);

            var element4 = element3.Subs?.FirstOrDefault(x => x.Identifier == "level4");

            Assert.Null(element4);

            Assert.AreEqual(structureElementLevel3.Address, element3.Address);

            clientNetAdapterFactory.Dispose();
        }

        [Test]
        public void TestSubTreeLevel2()
        {
            var ioTCore1 = IoTCoreFactory.Create("id0", null);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore1.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var ioTCore = ioTCore1;
            var structureElementLevel1 = ioTCore.CreateStructureElement(ioTCore.Root, "level1");
            var structureElementLevel2 = ioTCore.CreateStructureElement(structureElementLevel1, "level2");
            var structureElementLevel3 = ioTCore.CreateStructureElement(structureElementLevel2, "level3");
            var structureElementLevel4 = ioTCore.CreateStructureElement(structureElementLevel3, "level4");

            var result = ioTCore.Root.GetTree(new GetTreeRequestServiceData("/", 2));

            var element2 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2");

            Assert.NotNull(element2);

            if (element2.Subs != null)
            {
                Assert.Null(element2.Subs.FirstOrDefault(x=>x.Identifier == "level3"));
            }
            
            Assert.AreEqual(structureElementLevel2.Address, element2.Address);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }
    }
}
