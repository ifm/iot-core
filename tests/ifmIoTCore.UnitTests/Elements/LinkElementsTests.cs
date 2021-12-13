namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class LinkElementsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        //[Test]
        //public void AddLink_CheckRelationsAndAddresses_Success()
        //{
        //    // Arrange
        //    var ioTCore = IoTCoreFactory.Create("id0");
        //    var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0");
        //    var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
        //    var struct2 = ioTCore.CreateStructureElement(ioTCore.Root, "struct2");

        //    // Act
        //    ioTCore.AddLink(struct0, struct1);
        //    ioTCore.AddLink(struct1, struct2);

        //    // Assert

        //    // Check references struct0 -> struct1
        //    var reference = struct0.References.ForwardReferences.First(x => x.TargetElement == struct1);
        //    Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Forward);
        //    reference = struct1.References.InverseReferences.First(x => x.SourceElement == struct0);
        //    Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Inverse);

        //    // Check references struct1 -> struct2
        //    reference = struct1.References.ForwardReferences.First(x => x.TargetElement == struct2);
        //    Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Forward);
        //    reference = struct2.References.InverseReferences.First(x => x.SourceElement == struct1);
        //    Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Inverse);

        //    // Check link addresses
        //    var element = ioTCore.GetElementByAddress("/struct0/struct1");
        //    Assert.That(element != null);
        //    element = ioTCore.GetElementByAddress("/struct0/struct1/struct2");
        //    Assert.That(element != null);
        //}

        //[Test]
        //public void AddChild_Twice_Throws()
        //{
        //    var struct1 = new StructureElement("struct1");
        //    var struct2 = new StructureElement("struct2");

        //    var child = new StructureElement("child");
        //    struct1.AddChild(child);

        //    Assert.Throws<IoTCoreException>(() => struct2.AddChild(child));
        //}

        [Test]
        public void AddLink_Twice_Throws()
        {
            //var struct1 = new StructureElement("struct1");

            //var child = new StructureElement("child");
            //struct1.AddLink(child);

            //Assert.Throws<IoTCoreException>(() => struct1.AddLink(child));
        }

        //[Test]
        //public void AddLink_CircularDependency_Throws()
        //{
        //    var struct0 = new StructureElement("struct0");

        //    var struct1 = new StructureElement("struct1");
        //    struct0.AddChild(struct1);

        //    var struct2 = new StructureElement("struct2");
        //    struct1.AddChild(struct2);

        //    Assert.Throws<Exception>(() => struct2.AddLink(struct1));
        //}

        // Add gettree and querytree tests
    }
}
