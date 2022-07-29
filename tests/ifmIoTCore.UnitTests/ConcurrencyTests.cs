using System;

namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using ifmIoTCore.Elements;
    using NUnit.Framework;

    [TestFixture]
    public class ConcurrencyTests
    {
        [Test]
        public void MultiThreadedAddElement_AllElementsExist_Success()
        {
            IIoTCore ioTCore = IoTCoreFactory.Create("id0");

            // Arrange
            var stopWatch = new Stopwatch();

            try
            {

                stopWatch.Start();
                var tasks = new List<Task>();
                for (var i = 0; i < 100; i++)
                {
                    var i1 = i;
                    var task = Task.Run(() =>
                    {
                        for (var j = 0; j < 100; j++)
                        {
                            try
                            {
                                ioTCore.Root.AddChild(new StructureElement($"structure{i1}-{j}"), true);
                            }
                            catch 
                            {
                                // Ignore
                            }
                            //var dataElement = new DataElement($"data{i1}-{j}", null, null, null);
                            //structureElement.AddChild(dataElement);
                            //var serviceElement = new ServiceElement($"service{i1}-{j}", null, null, null);
                            //structureElement.AddChild(serviceElement);
                            //var eventElement = new EventElement($"event{i1}-{j}", null, null, null);
                            //structureElement.AddChild(eventElement);
                        }
                    });
                    tasks.Add(task);
                }

                
                Task.WaitAll(tasks.ToArray());
                TestContext.WriteLine($"Elements created takes {stopWatch.ElapsedMilliseconds} ms");

                // Act
                stopWatch.Restart();
                var elements = new List<IBaseElement>();
                for (var i = 0; i < 100; i++)
                {
                    for (var j = 0; j < 100; j++)
                    {
                        var element = ioTCore.Root.GetElementByIdentifier($"structure{i}-{j}");
                        if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"data{i}-{j}");
                        //if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"service{i}-{j}");
                        //if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"event{i}-{j}");
                        //if (element != null) elements.Add(element);
                    }
                }

                TestContext.WriteLine($"Elements collected by identifier takes {stopWatch.ElapsedMilliseconds} ms");

                // Assert
                Assert.That(elements.Count == 100 * 100);
                //Assert.That(elements.Count == 100 * 100 * 4);

                // Act
                elements.Clear();
                stopWatch.Restart();
                for (var i = 0; i < 100; i++)
                {
                    for (var j = 0; j < 100; j++)
                    {
                        var element = ioTCore.Root.GetElementByAddress($"/structure{i}-{j}");
                        if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"data{i}-{j}");
                        //if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"service{i}-{j}");
                        //if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"event{i}-{j}");
                        //if (element != null) elements.Add(element);
                    }
                }

                TestContext.WriteLine($"Elements collected by address takes {stopWatch.ElapsedMilliseconds} ms");

                Assert.That(elements.Count == 100 * 100);

                // Act
                elements.Clear();
                stopWatch.Restart();
                for (var i = 0; i < 100; i++)
                {
                    for (var j = 0; j < 100; j++)
                    {
                        var element = ioTCore.GetElementByAddress($"/structure{i}-{j}");
                        if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"data{i}-{j}");
                        //if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"service{i}-{j}");
                        //if (element != null) elements.Add(element);
                        //element = _ioTCore.Root.GetElementByIdentifier($"event{i}-{j}");
                        //if (element != null) elements.Add(element);
                    }
                }

                TestContext.WriteLine(
                    $"Elements collected by address from element manager takes {stopWatch.ElapsedMilliseconds} ms");

                Assert.That(elements.Count == 100 * 100);
            }
            finally
            {
                stopWatch.Stop();
                ioTCore.Dispose();
            }
        }
    }
}
