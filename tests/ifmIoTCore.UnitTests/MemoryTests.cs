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
                new BaseElement(null, "type", "id");
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
                using var ioTCore = IoTCoreFactory.Create("id0", null);

                var baseElement = ioTCore.CreateStructureElement(ioTCore.Root, "id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.CreateStructureElement(baseElement, $"struct{i}");
                    
                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.CreateDataElement<int>(structureElement, $"data{j}", null, null, false);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.CreateServiceElement<int, int>(structureElement, $"service{j}", null);
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
                using var ioTCore = IoTCoreFactory.Create("id0", null);

                var baseElement = ioTCore.CreateStructureElement(ioTCore.Root, "id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.CreateStructureElement(baseElement, $"struct{i}");
                    
                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.CreateDataElement<int>(structureElement, $"data{j}", null, null, false);
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.CreateServiceElement<int, int>(structureElement, $"service{j}", null);
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
                Assert.That(memory.GetObjects(where => where.Type.Is<IoTCore>()).ObjectsCount, Is.EqualTo(0));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementsInIoTCoreAndDestroyElements_Success()
        {
            if (!dotMemoryApi.IsEnabled) return;

            using var ioTCore = IoTCoreFactory.Create("id", null);

            var snapShot = dotMemoryApi.GetSnapshot();
            var count1 = snapShot.GetObjects(where => where.Interface.Is(typeof(IBaseElement))).ObjectsCount;
            TestContext.WriteLine($"count1 = {count1}");

            var isolator = new Action(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.CreateStructureElement(ioTCore.Root, $"struct{i}");

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.CreateDataElement<int>(structureElement, $"data{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.CreateServiceElement<int, int>(structureElement, $"service{j}", null);
                    }
                }

                snapShot = dotMemoryApi.GetSnapshot();
                var count2 = snapShot.GetObjects(where => where.Interface.Is(typeof(IBaseElement))).ObjectsCount;
                TestContext.WriteLine($"count2 = {count2}");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.Root.GetElementByIdentifier($"struct{i}");
                    ioTCore.RemoveElement(ioTCore.Root, structureElement);
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
                using var ioTCore = IoTCoreFactory.Create("id", null);

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.CreateStructureElement(ioTCore.Root, $"struct{i}");

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.CreateDataElement<int>(structureElement, $"data{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.CreateServiceElement<int, int>(structureElement, $"service{j}", null);
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
                Assert.That(memory.GetObjects(where => where.Type.Is<IoTCore>()).ObjectsCount, Is.EqualTo(0));
            });
        }
    }
}
