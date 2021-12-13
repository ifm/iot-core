namespace Sample07
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

                ioTCore.CreateDataElement<float>(struct1, 
                    "float1", 
                    GetFloat1, 
                    SetFloat1, 
                    true, 
                    true, 
                    format: new FloatFormat(new FloatValuation(-9.9f, 9.9f, 3)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static float GetFloat1(IBaseElement element)
        {
            return _float1;
        }

        private static void SetFloat1(IBaseElement element, float value)
        {
            if (_float1.EqualsWithPrecision(value)) return;
            _float1 = value;
        }
        private static float _float1 = 1.2345f;
    }
}