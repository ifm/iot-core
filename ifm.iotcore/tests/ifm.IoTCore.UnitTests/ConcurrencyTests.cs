namespace ifm.IoTCore.UnitTests;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using ElementManager.Contracts.Elements;
using NUnit.Framework;

[TestFixture]
public class ConcurrencyTests
{
    [Test]
    public void MultiThreadedAddElement_AllElementsExist_Success()
    {
        var ioTCore = Helpers.CreateIoTCore("id0");

        const int taskCount = 10;
        const int loopCount = 200;

        // Arrange
        var stopWatch = new Stopwatch();

        try
        {
            stopWatch.Start();
            var tasks = new List<Task>();

            TestContext.WriteLine($"Create elements in {taskCount} tasks. Each task loops {loopCount} times. Each loop creates 5 elements");

            CreateElementsInternalLock(ioTCore, taskCount, loopCount, tasks);
            //CreateElementsExternalLock(ioTCore, taskCount, loopCount, tasks);
            //CreateElementsNoLock(ioTCore, taskCount, loopCount, tasks);
            //CreateElementsNoTask(ioTCore, taskCount, loopCount);

            Task.WaitAll(tasks.ToArray());
            TestContext.WriteLine($"Elements created takes {stopWatch.ElapsedMilliseconds} ms");

            TestContext.Write("Get elements by identifier from element: ");
            stopWatch.Restart();
            var elements = new List<IBaseElement>();
            for (var i = 0; i < taskCount; i++)
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var parent = ioTCore.Root.GetElementByIdentifier($"parent[{i}-{j}]");
                    if (parent != null)
                    {
                        elements.Add(parent);
                        var child = parent.GetElementByIdentifier($"child1[{i}-{j}]");
                        if (child != null) elements.Add(child);
                        child = parent.GetElementByIdentifier($"child2[{i}-{j}]");
                        if (child != null) elements.Add(child);
                        child = parent.GetElementByIdentifier($"child3[{i}-{j}]");
                        if (child != null) elements.Add(child);
                        child = parent.GetElementByIdentifier($"child4[{i}-{j}]");
                        if (child != null) elements.Add(child);
                    }
                }
            }
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount * 5);

            TestContext.Write("Get elements by predicate from root element: ");
            elements.Clear();
            stopWatch.Restart();
            for (var i = 0; i < taskCount; i++)
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var i1 = i;
                    var j1 = j;
                    var element = ioTCore.Root.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.Root.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child1[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.Root.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child2[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.Root.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child3[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.Root.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child4[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                }
            }
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount * 5);

            TestContext.Write("Get elements by predicate from element manager: ");
            elements.Clear();
            stopWatch.Restart();
            for (var i = 0; i < taskCount; i++)
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var i1 = i;
                    var j1 = j;
                    var element = ioTCore.ElementManager.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child1[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child2[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child3[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByPredicate(x => x.Address == $"id0/parent[{i1}-{j1}]/child4[{i1}-{j1}]");
                    if (element != null) elements.Add(element);
                }
            }
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount * 5);

            TestContext.Write("Get elements by address from element manager: ");
            elements.Clear();
            stopWatch.Restart();
            for (var i = 0; i < taskCount; i++)
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child1[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child2[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child3[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child4[{i}-{j}]");
                    if (element != null) elements.Add(element);
                }
            }
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount * 5);

            TestContext.Write("Get elements by address from element manager cache: ");
            elements.Clear();
            stopWatch.Restart();
            for (var i = 0; i < taskCount; i++)
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child1[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child2[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child3[{i}-{j}]");
                    if (element != null) elements.Add(element);
                    element = ioTCore.ElementManager.GetElementByAddress($"/parent[{i}-{j}]/child4[{i}-{j}]");
                    if (element != null) elements.Add(element);
                }
            }
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount * 5);

            TestContext.Write("Get elements by predicate (profile=parent) from element manager: ");

            stopWatch.Restart();
            elements = ioTCore.ElementManager.GetElementsByPredicate(x => x.HasProfile("parent")).ToList();
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount);

            TestContext.Write("Get elements by predicate (profile=child) from element manager: ");

            stopWatch.Restart();
            elements = ioTCore.ElementManager.GetElementsByPredicate(x => x.HasProfile("child")).ToList();
            TestContext.WriteLine($"{elements.Count} elements takes {stopWatch.ElapsedMilliseconds} ms");

            Assert.That(elements.Count == taskCount * loopCount * 4);
        }
        finally
        {
            stopWatch.Stop();
        }
    }

    private void CreateElementsInternalLock(IIoTCore ioTCore, int taskCount, int loopCount, ICollection<Task> tasks)
    {
        for (var i = 0; i < taskCount; i++)
        {
            var i1 = i;
            var task = Task.Run(() =>
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var parent = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, $"parent[{i1}-{j}]", profiles: ["parent"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child1[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child2[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child3[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child4[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                }
            });
            tasks.Add(task);
        }
    }

    private void CreateElementsExternalLock(IIoTCore ioTCore, int taskCount, int loopCount, ICollection<Task> tasks)
    {
        for (var i = 0; i < taskCount; i++)
        {
            var i1 = i;
            var task = Task.Run(() =>
            {
                ioTCore.ElementManager.EnterWriteLock();
                try
                {
                    for (var j = 0; j < loopCount; j++)
                    {
                        var parent = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, $"parent[{i1}-{j}]", profiles: ["parent"], acquireLock: true);
                        ioTCore.ElementManager.CreateStructureElement(parent, $"child1[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                        ioTCore.ElementManager.CreateStructureElement(parent, $"child2[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                        ioTCore.ElementManager.CreateStructureElement(parent, $"child3[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                        ioTCore.ElementManager.CreateStructureElement(parent, $"child4[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    }
                }
                finally
                {
                    ioTCore.ElementManager.ExitWriteLock();
                }
            });
            tasks.Add(task);
        }
    }

    private void CreateElementsNoLock(IIoTCore ioTCore, int taskCount, int loopCount, ICollection<Task> tasks)
    {
        for (var i = 0; i < taskCount; i++)
        {
            var i1 = i;
            var task = Task.Run(() =>
            {
                for (var j = 0; j < loopCount; j++)
                {
                    var parent = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, $"parent[{i1}-{j}]", profiles: ["parent"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child1[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child2[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child3[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                    ioTCore.ElementManager.CreateStructureElement(parent, $"child4[{i1}-{j}]", profiles: ["child"], acquireLock: true);
                }
            });
            tasks.Add(task);
        }
    }

    private void CreateElementsNoTask(IIoTCore ioTCore, int taskCount, int loopCount)
    {
        for (var i = 0; i < taskCount; i++)
        {
            for (var j = 0; j < loopCount; j++)
            {
                var parent = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, $"parent[{i}-{j}]", profiles: ["parent"], acquireLock: true);
                ioTCore.ElementManager.CreateStructureElement(parent, $"child1[{i}-{j}]", profiles: ["child"], acquireLock: true);
                ioTCore.ElementManager.CreateStructureElement(parent, $"child2[{i}-{j}]", profiles: ["child"], acquireLock: true);
                ioTCore.ElementManager.CreateStructureElement(parent, $"child3[{i}-{j}]", profiles: ["child"], acquireLock: true);
                ioTCore.ElementManager.CreateStructureElement(parent, $"child4[{i}-{j}]", profiles: ["child"], acquireLock: true);
            }
        }
    }
}