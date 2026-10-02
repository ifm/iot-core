namespace ifm.IoTCore.UnitTests.Elements
{
    using System.Linq;
    using Common.Exceptions;
    using ElementManager.Contracts.Elements;
    using NUnit.Framework;

    [TestFixture]
    public class ElementAddressTests
    {
        [Test]
        public void AddChildTest()
        {
            var ioTCore = Helpers.CreateIoTCore("aasdf");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            Assert.That("aasdf/struct0", Is.EqualTo(struct0.Address));
        }

        [Test]
        public void AddChildTest2()
        {
            var ioTCore = Helpers.CreateIoTCore("aasdf");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(struct0, "struct1");
            Assert.That(struct1,Is.SameAs(struct0.Subs.FirstOrDefault()));
        }

        [Test]
        public void ConstructorTest()
        {
            var ioTCore = Helpers.CreateIoTCore("aasdf");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            Assert.That("aasdf/struct0",Is.EqualTo(struct0.Address));
        }

        [Test]
        public void CreateElementTest()
        {
            var ioTCore = Helpers.CreateIoTCore("aasdf");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            Assert.That("struct0", Is.EqualTo(struct0.Identifier));
            Assert.That("aasdf/struct0", Is.EqualTo(struct0.Address));
        }

        [Test]
        public void GetElementByAddress_IElementManagerApi_QuickerHashLookup()
        {
            var ioTCore = Helpers.CreateIoTCore("testDevice1");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(struct0, "struct1");
            var struct2 = ioTCore.ElementManager.CreateStructureElement(struct1, "struct2");
            var struct3 = ioTCore.ElementManager.CreateStructureElement(struct2, "struct3");
            var struct4 = ioTCore.ElementManager.CreateStructureElement(struct3, "struct4");

            Assert.That(struct1, Is.EqualTo(struct0.Subs.Single(x => x.Identifier == struct1.Identifier)));
            Assert.That(struct2, Is.EqualTo(struct1.Subs.Single(x => x.Identifier == struct2.Identifier)));
            Assert.That(struct3, Is.EqualTo(struct2.Subs.Single(x => x.Identifier == struct3.Identifier)));
            Assert.That(struct4, Is.EqualTo(struct3.Subs.Single(x => x.Identifier == struct4.Identifier)));

            Assert.That(struct4, Is.SameAs(ioTCore.ElementManager.GetElementByAddress("/struct0/struct1/struct2/struct3/struct4")));
        }

        [Test]
        public void GetElementByAddress_IBaseElementApi_SlowerRecursiveSearch()
        {
            var ioTCore = Helpers.CreateIoTCore("testIot1");
            var testElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var subElement = ioTCore.ElementManager.CreateStructureElement(testElement, "struct1");
            Assert.That(ioTCore.ElementManager.GetElementByAddress("testIot1/struct0/struct1") is IStructureElement);
            Assert.That("testIot1/struct0/struct1", Is.EqualTo(ioTCore.ElementManager.GetElementByAddress("testIot1/struct0/struct1").Address));
            Assert.That(subElement, Is.EqualTo(ioTCore.ElementManager.GetElementByAddress("testIot1/struct0/struct1")));
        }

        [Test]
        public void ParentLevel2Test()
        {
            var ioTCore = Helpers.CreateIoTCore("testDevice1");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(struct0, "struct1");
            var struct2 = ioTCore.ElementManager.CreateStructureElement(struct1, "struct2");

            Assert.That("testDevice1/struct0/struct1", Is.EqualTo(struct1.Address));
            Assert.That("testDevice1/struct0/struct1/struct2", Is.EqualTo(struct2.Address));
        }

        [Test]
        public void SetParentTest()
        {
            var ioTCore = Helpers.CreateIoTCore("aasdf");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(struct0, "struct1");

            Assert.That("aasdf/struct0", Is.EqualTo(struct0.Address));
            Assert.That("aasdf/struct0/struct1", Is.EqualTo(struct1.Address));
        }

        [Test]
        public void TryGenerateSameAddress_Throws()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");

            const string identifier = "struct";
            var a = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, identifier);

            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, identifier));
        }

        [Test]
        public void GetInvalidAddress_Throws()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");

            const string identifier = "struct";
            var a = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, identifier);

            // Valid
            Assert.That(a, Is.EqualTo(ioTCore.ElementManager.GetElementByAddress("id0/struct")));

            // Invalid
            Assert.That(ioTCore.ElementManager.GetElementByAddress("id0/struct/") == null);
        }
    }
}