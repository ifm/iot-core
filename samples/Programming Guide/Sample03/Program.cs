namespace Sample03
{
    using System;
    using System.Collections.Generic;
    using ifmIoTCore;
    using ifmIoTCore.Elements;

    internal class UserData : IEquatable<UserData>
    {
        public int Int1;
        public float Float1;
        public string String1;

        public UserData()
        {
            Int1 = 10;
            Float1 = 1.2345f;
            String1 = "Hallo";
        }

        public bool Equals(UserData other)
        {
            if (other == null) return false;
            return Int1 == other.Int1 &&
                   Float1.Equals(other.Float1) &&
                   String1 == other.String1;
        }

        public override string ToString()
        {
            return $"Int1={Int1} Float1={Float1} String1={String1}";
        }
    }

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1", null,
                    new List<string> { "profile1" });

                ioTCore.CreateActionServiceElement(struct1, 
                    "service1", 
                    HandleService1);

                ioTCore.CreateGetterServiceElement<int>(struct1, 
                    "service2", 
                    HandleService2);

                ioTCore.CreateSetterServiceElement<string>(struct1, 
                    "service3", 
                    HandleService3);

                ioTCore.CreateServiceElement<int, string>(struct1, 
                    "service4", 
                    HandleService4);

                ioTCore.CreateServiceElement<UserData, UserData>(struct1, 
                    "service5", 
                    HandleService5);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static void HandleService1(IBaseElement element, int? cid)
        {
            Console.WriteLine("Do something");
        }

        private static int HandleService2(IBaseElement element, int? cid)
        {
            Console.WriteLine("Do something and return result");
            return 0;
        }

        private static void HandleService3(IBaseElement element, string data, int? cid)
        {
            Console.WriteLine($"Do something with {data}");
        }

        private static string HandleService4(IBaseElement element, int data, int? cid)
        {
            Console.WriteLine($"Do something with {data} and return result");
            return $"Received {data}";
        }

        private static UserData HandleService5(IBaseElement element, UserData data, int? cid)
        {
            Console.WriteLine($"Do something with {data} and return result");
            return data;
        }
    }
}