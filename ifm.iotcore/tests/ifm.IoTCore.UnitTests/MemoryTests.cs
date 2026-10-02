namespace ifm.IoTCore.UnitTests
{
    using System;
    using Contracts;
    using ElementManager.Contracts.Elements;
    using JetBrains.dotMemoryUnit;
    using JetBrains.dotMemoryUnit.Kernel;
    using NUnit.Framework;

    [TestFixture]
    class MemoryTests
    {
        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementAndDestroyElement_Success()
        {
            if (!dotMemoryApi.IsEnabled) return;

            var isolator = new Action(() =>
            {
                var iotCore = Helpers.CreateIoTCore("id0");
                iotCore.ElementManager.CreateStructureElement(null, "id");
            });

            isolator();

            GC.Collect();

            dotMemory.Check(memory =>
            {
                Assert.That(memory.GetObjects(where => where.Type.Is<IBaseElement>()).ObjectsCount, Is.EqualTo(0));
            });
        }

        [DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]
        [Test]
        public void CreateElementsAndDestroyElementsAddDirect_Success()
        {
            if (!dotMemoryApi.IsEnabled) return;

            var isolator = new Action(() =>
            {
                var ioTCore = Helpers.CreateIoTCore("id0");

                var baseElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.ElementManager.CreateStructureElement(baseElement, $"struct{i}");
                    
                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.ElementManager.CreateSimpleDataElement<int>(structureElement, $"data{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.ElementManager.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.ElementManager.CreateActionServiceElement(structureElement, $"service{j}", _ => { });
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
            if (!dotMemoryApi.IsEnabled) return;

            var isolator = new Action(() =>
            {
                var ioTCore = Helpers.CreateIoTCore("id0");

                var baseElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.ElementManager.CreateStructureElement(baseElement, $"struct{i}");

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.ElementManager.CreateSimpleDataElement<int>(structureElement, $"data{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.ElementManager.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.ElementManager.CreateActionServiceElement(structureElement, $"service{j}", _ => { });
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
            if (!dotMemoryApi.IsEnabled) return;

            var isolator = new Action(() =>
            {
                Helpers.CreateIoTCore("0");
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
                var ioTCore = Helpers.CreateIoTCore("id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, $"struct{i}");

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.ElementManager.CreateSimpleDataElement<int>(structureElement, $"data{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.ElementManager.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.ElementManager.CreateActionServiceElement(structureElement, $"service{j}", _ => { });
                    }
                }

                snapShot = dotMemoryApi.GetSnapshot();
                var count2 = snapShot.GetObjects(where => where.Interface.Is(typeof(IBaseElement))).ObjectsCount;
                TestContext.WriteLine($"count2 = {count2}");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.Root.GetElementByIdentifier($"struct{i}");
                    ioTCore.ElementManager.RemoveElement(ioTCore.Root, structureElement);
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
            if (!dotMemoryApi.IsEnabled) return;

            var isolator = new Action(() =>
            {
                var ioTCore = Helpers.CreateIoTCore("id");

                for (var i = 0; i < 100; i++)
                {
                    var structureElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, $"struct{i}");

                    for (var j = 0; j < 100; j++)
                    {
                        var dataElement = ioTCore.ElementManager.CreateSimpleDataElement<int>(structureElement, $"data{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var eventElement = ioTCore.ElementManager.CreateEventElement(structureElement, $"event{j}");
                    }

                    for (var j = 0; j < 100; j++)
                    {
                        var serviceElement = ioTCore.ElementManager.CreateActionServiceElement(structureElement, $"service{j}", _ => { });
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
