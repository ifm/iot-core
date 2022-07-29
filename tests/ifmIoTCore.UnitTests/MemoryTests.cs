namespace ifmIoTCore.UnitTests
{
    using System;
    using ifmIoTCore.Elements;
    using JetBrains.dotMemoryUnit;
    using JetBrains.dotMemoryUnit.Kernel;
    using NUnit.Framework;

    [TestFixture]
    class MemoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementAndDestroyElement_Success()
        {
            var isolator = new Action(() =>
            {
                new BaseElement("type", "id");
            });

            isolator();

            GC.Collect();

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => where.Type.Is<BaseElement>()).ObjectsCount, Is.EqualTo(0));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementsAndDestroyElementsAddDirect_Success()
        {
            var isolator = new Action(() =>
            {
                using var ioTCore = IoTCoreFactory.Create("id0");

                var baseElement = new StructureElement("id");
                ioTCore.Root.AddChild(baseElement);

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = new StructureElement($"struct{i}");
                    baseElement.AddChild(structureElement);
                    
                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = new DataElement<int>($"data{j}");
                        structureElement.AddChild(dataElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = new EventElement($"event{j}");
                        structureElement.AddChild(eventElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = new ServiceElement($"service{j}", null );
                        structureElement.AddChild(serviceElement);
                    }
                }
            });

            isolator();

            GC.Collect();

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => @where.Interface.Is(typeof(IBaseElement))).ObjectsCount, Is.EqualTo(0));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementsAndDestroyElementsAddTree_Success()
        {
            var isolator = new Action(() =>
            {
                using var ioTCore = IoTCoreFactory.Create("id0");

                var baseElement = new StructureElement("id");
                ioTCore.Root.AddChild(baseElement);

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = new StructureElement($"struct{i}");
                    baseElement.AddChild(structureElement);

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = new DataElement<int>($"data{j}");
                        structureElement.AddChild(dataElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = new EventElement($"event{j}");
                        structureElement.AddChild(eventElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = new ServiceElement($"service{j}", null);
                        structureElement.AddChild(serviceElement);
                    }
                }
            });

            isolator();

            GC.Collect();

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => @where.Interface.Is(typeof(IBaseElement))).ObjectsCount, Is.EqualTo(0));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateIoTCoreAndDestroyIoTCore_Success()
        {
            var isolator = new Action(() =>
            {
                IoTCoreFactory.Create("0", null);
            });

            isolator();

            GC.Collect();

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => @where.Interface.Is(typeof(IBaseElement))).ObjectsCount, Is.EqualTo(0));
            });

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => where.Type.Is<IIoTCore>()).ObjectsCount, Is.EqualTo(0));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementsInIoTCoreAndDestroyElements_Success()
        {
            if (!dotMemoryApi.IsEnabled) return;

            var snapShot = dotMemoryApi.GetSnapshot();
            var count1 = snapShot.GetObjects(where => where.Interface.Is(typeof(IBaseElement))).ObjectsCount;
            TestContext.WriteLine($"count1 = {count1}");

            var isolator = new Action(() =>
            {
                using var ioTCore = IoTCoreFactory.Create("id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = new StructureElement($"struct{i}");
                    ioTCore.Root.AddChild(structureElement);

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = new DataElement<int>($"data{j}");
                        structureElement.AddChild(dataElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = new EventElement($"event{j}");
                        structureElement.AddChild(eventElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = new ServiceElement($"service{j}", null );
                        structureElement.AddChild(serviceElement);
                    }
                }

                snapShot = dotMemoryApi.GetSnapshot();
                var count2 = snapShot.GetObjects(where => where.Interface.Is(typeof(IBaseElement))).ObjectsCount;
                TestContext.WriteLine($"count2 = {count2}");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.Root.GetElementByIdentifier($"struct{i}");
                    ioTCore.Root.RemoveChild(structureElement);
                }
            });

            isolator();

            GC.Collect();

            snapShot = dotMemoryApi.GetSnapshot();
            var count3 = snapShot.GetObjects(where => where.Interface.Is(typeof(IBaseElement))).ObjectsCount;
            TestContext.WriteLine($"count3 = {count3}");

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => @where.Interface.Is(typeof(IBaseElement))).ObjectsCount, Is.EqualTo(count1));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementsInIoTCoreAndDestroyIoTCore_Success()
        {
            var isolator = new Action(() =>
            {
                using var ioTCore = IoTCoreFactory.Create("id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = new StructureElement($"struct{i}");
                    ioTCore.Root.AddChild(structureElement);

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = new DataElement<int>($"data{j}");
                        structureElement.AddChild(dataElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = new EventElement($"event{j}");
                        structureElement.AddChild(eventElement);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = new ServiceElement($"service{j}",  null );
                        structureElement.AddChild(serviceElement);
                    }
                }
            });

            isolator();

            GC.Collect();

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => @where.Interface.Is(typeof(IBaseElement))).ObjectsCount, Is.EqualTo(0));
            });

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => where.Type.Is<IIoTCore>()).ObjectsCount, Is.EqualTo(0));
            });
        }
    }
}
