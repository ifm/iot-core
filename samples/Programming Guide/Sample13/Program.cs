namespace Sample13
{
    using System;
    using ifmIoTCore;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, 
                    "struct1");
                ioTCore.CreateDataElement<string>(struct1, "string1");

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