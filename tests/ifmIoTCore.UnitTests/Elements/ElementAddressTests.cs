namespace ifmIoTCore.UnitTests.Elements
{
    using System.Linq;
    using Exceptions;
    using ifmIoTCore.Elements;
    using NUnit.Framework;

    [TestFixture]
    public class ElementAddressTests
    {
        [Test]
        public void AddChildTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            Assert.AreEqual("/struct0", struct0.Address);
        }

        [Test]
        public void AddChildTest2()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            var struct1 = new StructureElement("struct1");
            struct0.AddChild(struct1);
            Assert.AreSame(struct1, struct0.Subs.FirstOrDefault());
        }

        [Test]
        public void ConstructorTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            Assert.AreEqual("/struct0", struct0.Address);
        }

        [Test]
        public void CreateElementTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            Assert.AreEqual("struct0", struct0.Identifier);
            Assert.AreEqual("/struct0", struct0.Address);
        }

        [Test]
        public void GetElementByAddress_IElementManagerApi_QuickerHashLookup()
        {
            using var ioTCore = IoTCoreFactory.Create("testDevice1");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            var struct1 = new StructureElement("struct1");
            struct0.AddChild(struct1);
            var struct2 = new StructureElement("struct2");
            struct1.AddChild(struct2);
            var struct3 = new StructureElement("struct3");
            struct2.AddChild(struct3);
            var struct4 = new StructureElement("struct4");
            struct3.AddChild(struct4);

            ioTCore.Root.RaiseTreeChanged();

            Assert.AreEqual(struct1, struct0.Subs.Single(x => x.Identifier == struct1.Identifier));
            Assert.AreEqual(struct2, struct1.Subs.Single(x => x.Identifier == struct2.Identifier));
            Assert.AreEqual(struct3, struct2.Subs.Single(x => x.Identifier == struct3.Identifier));
            Assert.AreEqual(struct4, struct3.Subs.Single(x => x.Identifier == struct4.Identifier));

            Assert.AreSame(struct4, ioTCore.GetElementByAddress("/struct0/struct1/struct2/struct3/struct4"));
        }

        [Test]
        public void GetElementByAddress_IBaseElementApi_SlowerRecursiveSearch()
        {
            using var ioTCore = IoTCoreFactory.Create("testIot1");
            var testElement = new StructureElement("struct0");
            ioTCore.Root.AddChild(testElement);
            var subElement = new StructureElement("struct1");
            testElement.AddChild(subElement);
            Assert.That(testElement.GetElementByAddress("/struct0/struct1") is IStructureElement);
            Assert.AreEqual("/struct0/struct1", testElement.GetElementByAddress("/struct0/struct1").Address);
            Assert.AreEqual(subElement, testElement.GetElementByAddress("/struct0/struct1"));
            Assert.AreEqual(subElement, ioTCore.Root.GetElementByAddress("/struct0/struct1"));
        }

        [Test]
        public void ParentLevel2Test()
        {
            using var ioTCore = IoTCoreFactory.Create("testDevice1");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            var struct1 = new StructureElement("struct1");
            struct0.AddChild(struct1);
            var struct2 = new StructureElement("struct2");
            struct1.AddChild(struct2);

            Assert.AreEqual("/struct0/struct1", struct1.Address);
            Assert.AreEqual("/struct0/struct1/struct2", struct2.Address);
        }

        [Test]
        public void SetParentTest()
        {
            using var ioTCore = IoTCoreFactory.Create("aasdf");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            var struct1 = new StructureElement("struct1");
            struct0.AddChild(struct1);

            Assert.AreEqual("/struct0", struct0.Address);
            Assert.AreEqual("/struct0/struct1", struct1.Address);
        }

        [Test]
        public void TryGenerateSameAddress_Throws()
        {
            var ioTCore = IoTCoreFactory.Create("id0");

            const string identifier = "struct";
            var a = new StructureElement(identifier);
            ioTCore.Root.AddChild(a);

            Assert.Throws<IoTCoreException>(() => ioTCore.Root.AddChild(new StructureElement(identifier)));
        }
    }
}