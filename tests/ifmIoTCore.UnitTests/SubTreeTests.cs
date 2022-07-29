using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using System.Linq;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using NUnit.Framework;
    using Utilities;

    [TestFixture]
    public class SubTreeTests
    {
        [Test]
        public void TestSubTreeLevel4()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var structureElementLevel1 = new StructureElement("level1");
            ioTCore.Root.AddChild(structureElementLevel1, true);
            var structureElementLevel2 = new StructureElement("level2");
            structureElementLevel1.AddChild(structureElementLevel2, true); 
            var structureElementLevel3 = new StructureElement("level3");
            structureElementLevel2.AddChild(structureElementLevel3, true);
            var structureElementLevel4 = new StructureElement("level4");
            structureElementLevel3.AddChild(structureElementLevel4, true);

            var getTreeService = ioTCore.Root.GetTreeServiceElement;

            var result = Variant.ToObject<GetTreeResponseServiceData>(getTreeService.Invoke(Variant.FromObject(new GetTreeRequestServiceData("/", null))));

            var element4 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2").Subs
                .First(x => x.Identifier == "level3").Subs.First(x => x.Identifier == "level4");
            
            Assert.NotNull(element4);
            Assert.AreEqual(structureElementLevel4.Address, element4.Address);
        }

        [Test]
        public void TestSubTreeLevel3()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var structureElementLevel1 = new StructureElement("level1");
            ioTCore.Root.AddChild(structureElementLevel1, true);
            var structureElementLevel2 = new StructureElement("level2");
            structureElementLevel1.AddChild(structureElementLevel2, true);
            var structureElementLevel3 = new StructureElement("level3");
            structureElementLevel2.AddChild(structureElementLevel3, true);
            var structureElementLevel4 = new StructureElement("level4");
            structureElementLevel3.AddChild(structureElementLevel4, true);

            var getTreeService = ioTCore.Root.GetTreeServiceElement;

            var result = Variant.ToObject<GetTreeResponseServiceData>(getTreeService.Invoke(Variant.FromObject(new GetTreeRequestServiceData("/", 3))));

            var element3 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2")
                .Subs
                .FirstOrDefault(x => x.Identifier == "level3");
            Assert.NotNull(element3);

            var element4 = element3.Subs?.FirstOrDefault(x => x.Identifier == "level4");
            Assert.Null(element4);
            Assert.AreEqual(structureElementLevel3.Address, element3.Address);
        }

        [Test]
        public void TestSubTreeLevel2()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var structureElementLevel1 = new StructureElement("level1");
            ioTCore.Root.AddChild(structureElementLevel1, true);
            var structureElementLevel2 = new StructureElement("level2");
            structureElementLevel1.AddChild(structureElementLevel2, true);
            var structureElementLevel3 = new StructureElement("level3");
            structureElementLevel2.AddChild(structureElementLevel3, true);
            var structureElementLevel4 = new StructureElement("level4");
            structureElementLevel3.AddChild(structureElementLevel4, true);

            var getTreeService = ioTCore.Root.GetTreeServiceElement;

            var result = Variant.ToObject<GetTreeResponseServiceData>(getTreeService.Invoke(Variant.FromObject(new GetTreeRequestServiceData("/", 2))));

            var element2 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2");
            Assert.NotNull(element2);
            if (element2.Subs != null)
            {
                Assert.Null(element2.Subs.FirstOrDefault(x=>x.Identifier == "level3"));
            }
            Assert.AreEqual(structureElementLevel2.Address, element2.Address);
        }
    }
}
