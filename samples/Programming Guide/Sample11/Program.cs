namespace Sample11
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

                ioTCore.CreateDataElement<int[]>(struct1, 
                    "array1", 
                    GetArray1, 
                    SetArray1, 
                    true, 
                    true, 
                    format: new ArrayFormat(new ArrayValuation(Format.Types.Number, 
                        new IntegerFormat(new IntegerValuation(0, 100)))));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static int[] GetArray1(IBaseElement element)
        {
            return _array1;
        }

        private static void SetArray1(IBaseElement element, int[] value)
        {
            if (_array1.SequenceEqual(value)) return;
            _array1 = value;
        }
        private static int[] _array1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    }
}
