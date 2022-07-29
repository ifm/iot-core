namespace Sample10
{
    using System;
    using System.Diagnostics;
    using ifmIoTCore;
    using ifmIoTCore.Elements;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = new StructureElement("struct1");
                ioTCore.Root.AddChild(struct1);
                var struct2 = new StructureElement("struct2");
                struct1.AddChild(struct2);
                var data1 = new DataElement<int>("data1", value:123);
                struct2.AddChild(data1);

                // Create a link from root element to data1 element
                ioTCore.Root.AddLink(data1, "link_data1");

                // Get the element via link
                var element = (IDataElement)ioTCore.GetElementByAddress("/link_data1");
                Console.WriteLine(element.Value);

                // Remove the link from root element to data1 element
                ioTCore.Root.RemoveLink(data1);
                element = (IDataElement)ioTCore.GetElementByAddress("/link_data1");
                Debug.Assert(element == null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}