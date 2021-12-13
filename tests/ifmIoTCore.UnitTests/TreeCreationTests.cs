namespace ifmIoTCore.UnitTests
{
    using Exceptions;
    using ifmIoTCore.Elements;
    using NUnit.Framework;

    [TestFixture]
    public class TreeCreationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Element_Creation_WithInvalidIdentifier_ThrowsException()
        {
            using var ioTCore = IoTCoreFactory.Create("testIot1", null);
            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.CreateStructureElement(ioTCore.Root, "");
            }); 

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.CreateStructureElement(ioTCore.Root, "/");
            }); 

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.CreateStructureElement(ioTCore.Root, "//");
            });

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.CreateStructureElement(ioTCore.Root, "struct1 ");
            });

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.CreateStructureElement(ioTCore.Root, "struct1/struct2");
            });
        }

        //[Test]
        //public void Element_AddingChild_Self_ThrowsException()
        //{
        //    var testElement = new StructureElement("struct0");
        //    Assert.Throws<IoTCoreException>(() => 
        //    { 
        //        testElement.AddChild(testElement);
        //    });
        //}

        //[Test]
        //public void Element_AddingChild_Parent_ThrowsException()
        //{
        //    var testElement = new StructureElement("struct0");
        //    var testElement1 = new StructureElement("struct1");
        //    testElement.AddChild(testElement1);
        //    Assert.That(() => testElement1.AddChild(testElement), Throws.Exception);
        //}

        [Test]
        public void Element_AddingChild_MatchingAddress_ThrowsException()
        {
            using var ioTCore = IoTCoreFactory.Create("id0", null);
            var testElement = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var subElement = ioTCore.CreateStructureElement(testElement, "struct1");
            Assert.Throws<IoTCoreException>(() => 
            {  // new instance (same type and same identifier)
                ioTCore.CreateStructureElement(testElement, "struct1");
            });
            Assert.Throws<IoTCoreException>(() => 
            {  // new instance (different type and same identifier)
                ioTCore.CreateDataElement<object>(testElement, "struct1"); 
            });
        }

        [Test]
        public void Element_AddingChild_SameIdentifier_ThrowsException()
        {
            using var ioTCore = IoTCoreFactory.Create("id0", null);
            var testElement = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
            var subElement = ioTCore.CreateStructureElement(testElement, "struct0");
            Assert.Throws<IoTCoreException>(() => 
            { 
                ioTCore.CreateDataElement<object>(testElement, "struct0");
            });
        }
    }
}