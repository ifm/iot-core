namespace ifm.IoTCore.UnitTests
{
    using System.Linq;
    using Common;
    using ElementManager.Contracts.Elements;
    using ElementManager.Contracts.Elements.ServiceData.Requests;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using NUnit.Framework;

    [TestFixture]
    public class SubTreeTests
    {
        [Test]
        public void TestSubTreeLevel4()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var structureElementLevel1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "level1");
            var structureElementLevel2 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel1, "level2"); 
            var structureElementLevel3 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel2, "level3");
            var structureElementLevel4 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel3, "level4");

            var getTreeService =
                ioTCore.Root.Subs.Single(x => x.Identifier.ToLower() == Identifiers.GetTree.ToLowerInvariant()) as
                    IServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData>;

            var result = getTreeService.Invoke(new GetTreeRequestServiceData("id0", null));

            var element4 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2").Subs
                .First(x => x.Identifier == "level3").Subs.First(x => x.Identifier == "level4");
            
            Assert.That(element4,Is.Not.Null);
            Assert.That(structureElementLevel4.Address, Is.EqualTo(element4.Address));
        }

        [Test]
        public void TestSubTreeLevel3()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var structureElementLevel1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "level1");
            var structureElementLevel2 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel1, "level2");
            var structureElementLevel3 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel2, "level3");
            var structureElementLevel4 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel3, "level4");

            var getTreeService =
                ioTCore.Root.Subs.Single(x => x.Identifier.ToLower() == Identifiers.GetTree.ToLowerInvariant()) as
                    IServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData>;

            var result = getTreeService.Invoke(new GetTreeRequestServiceData("id0", 3));

            var element3 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2")
                .Subs
                .FirstOrDefault(x => x.Identifier == "level3");
            Assert.That(element3,Is.Not.Null);

            var element4 = element3.Subs?.FirstOrDefault(x => x.Identifier == "level4");
            Assert.That(element4,Is.Null);
            Assert.That(structureElementLevel3.Address, Is.EqualTo(element3.Address));
        }

        [Test]
        public void TestSubTreeLevel2()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var structureElementLevel1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "level1");
            var structureElementLevel2 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel1, "level2");
            var structureElementLevel3 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel2, "level3");
            var structureElementLevel4 = ioTCore.ElementManager.CreateStructureElement(structureElementLevel3, "level4");

            var getTreeService =
                ioTCore.Root.Subs.Single(x => x.Identifier.ToLower() == Identifiers.GetTree.ToLowerInvariant()) as
                    IServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData>;

            var result = getTreeService.Invoke(new GetTreeRequestServiceData("id0", 2));

            var element2 = result.Subs.First(x => x.Identifier == "level1").Subs.First(x => x.Identifier == "level2");
            Assert.That(element2,Is.Not.Null);
            if (element2.Subs != null)
            {
                Assert.That(element2.Subs.FirstOrDefault(x=>x.Identifier == "level3"),Is.Null);
            }
            Assert.That(structureElementLevel2.Address, Is.EqualTo(element2.Address));
        }
    }
}
