namespace Sample09
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

                ioTCore.CreateDataElement<int>(struct1, 
                    "enum1", 
                    GetEnum1, 
                    SetEnum1, 
                    true, 
                    true, 
                    format: new IntegerEnumFormat(new IntegerEnumValuation(
                        new Dictionary<string, string>
                        {
                            {"0", "null"}, 
                            {"1", "one"}, 
                            {"2", "two"}, 
                            {"3", "three"}
                        }, _enum1)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static int GetEnum1(IBaseElement element)
        {
            return _enum1;
        }

        private static void SetEnum1(IBaseElement element, int value)
        {
            _enum1 = value;
        }
        private static int _enum1 = 1;
    }
}