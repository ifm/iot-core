namespace Sample08
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;
    using ifmIoTCore.Utilities;

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

                ioTCore.CreateDataElement<string>(struct1, 
                    "string1", 
                    GetString1, 
                    SetString1, 
                    true, 
                    true, 
                    format: new StringFormat(new StringValuation(0, 100)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static string GetString1(IBaseElement element)
        {
            return _string1;
        }

        private static void SetString1(IBaseElement element, string value)
        {
            _string1 = value;
        }
        private static string _string1 = "Hallo";
    }
}