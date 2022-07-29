namespace Sample02
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Elements;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");
                
                var struct1 = new StructureElement("struct1", 
                    null, 
                    new List<string> { "profile1" });
                ioTCore.Root.AddChild(struct1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }
    }
}