namespace ifmIoTCore.UnitTests.Elements
{
    using System.Linq;
    using ifmIoTCore.Elements;
    using NUnit.Framework;

    [TestFixture]
    public class ElementAddressTests
    {
        [Test]
        public void AddChildTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            Assert.AreEqual("/struct1", struct1.Address);
        }

        [Test]
        public void AddChildTest2()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf", null);
            var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.CreateStructureElement(struct0, "struct1");
            Assert.AreSame(struct1, struct0.Subs.FirstOrDefault());
        }

        [Test]
        public void ConstructorTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf", null);
            var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            Assert.AreEqual("/struct0", struct0.Address);
        }

        [Test]
        public void CreateElementTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf", null);
            var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            Assert.AreEqual("struct0", struct0.Identifier);
            Assert.AreEqual("/struct0", struct0.Address);
        }

        [Test]
        public void GetElementByAddress_IElementManagerApi_QuickerHashLookup()
        {
            using var ioTCore = IoTCoreFactory.Create("testDevice1", null);
            var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.CreateStructureElement(struct0, "struct1");
            var struct2 = ioTCore.CreateStructureElement(struct1, "struct2");
            var struct3 = ioTCore.CreateStructureElement(struct2, "struct3");
            var struct4 = ioTCore.CreateStructureElement(struct3, "struct4");

            Assert.AreEqual(struct1, struct0.Subs.Single(x => x.Identifier == struct1.Identifier));
            Assert.AreEqual(struct2, struct1.Subs.Single(x => x.Identifier == struct2.Identifier));
            Assert.AreEqual(struct3, struct2.Subs.Single(x => x.Identifier == struct3.Identifier));
            Assert.AreEqual(struct4, struct3.Subs.Single(x => x.Identifier == struct4.Identifier));

            Assert.AreSame(struct4, ioTCore.GetElementByAddress("/struct0/struct1/struct2/struct3/struct4"));
        }

        [Test]
        public void GetElementByAddress_IBaseElementApi_SlowerRecursiveSearch()
        {
            using var ioTCore = IoTCoreFactory.Create("testIot1", null);
            var testElement = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var subElement = ioTCore.CreateStructureElement(testElement, "struct1");
            Assert.That(testElement.GetElementByAddress("/struct0/struct1") is IStructureElement);
            Assert.AreEqual("/struct0/struct1", testElement.GetElementByAddress("/struct0/struct1").Address);
            Assert.AreEqual(subElement, testElement.GetElementByAddress("/struct0/struct1"));
            Assert.AreEqual(subElement, ioTCore.Root.GetElementByAddress("/struct0/struct1"));
        }

        [Test]
        public void ParentLevel2Test()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf", null);
            var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.CreateStructureElement(struct0, "struct1");
            var struct2 = ioTCore.CreateStructureElement(struct1, "struct2");

            Assert.AreEqual("/struct0/struct1", struct1.Address);
            Assert.AreEqual("/struct0/struct1/struct2", struct2.Address);
        }

        [Test]
        public void SetParentTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf", null);
            var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.CreateStructureElement(struct0, "struct1");

            Assert.AreEqual("/struct0", struct0.Address);
            Assert.AreEqual("/struct0/struct1", struct1.Address);
        }
    }
}