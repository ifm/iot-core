namespace Sample06
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, 
                    "struct1", 
                    null,
                    new List<string> { "profile1" });

                ioTCore.CreateDataElement<int>(struct1, 
                    "int1", 
                    GetInt1, 
                    SetInt1, 
                    true, 
                    true, 
                    format: new IntegerFormat(new IntegerValuation(0, 100)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static int GetInt1(IBaseElement element)
        {
            return _int1;
        }

        private static void SetInt1(IBaseElement element, int value)
        {
            _int1 = value;
        }
        private static int _int1 = 10;
    }
}
