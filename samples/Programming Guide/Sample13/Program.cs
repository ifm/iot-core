namespace Sample13
{
    using System;
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

                var dataElement = new DataElement<string>("string1");
                struct1.AddChild(dataElement);

                var string1 = ioTCore.GetElementByAddress("/struct1/string1");
                Console.WriteLine(string1.Address);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}