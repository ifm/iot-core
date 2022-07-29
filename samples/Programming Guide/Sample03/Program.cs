namespace Sample03
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Common.Variant;
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

                var service1 = new ActionServiceElement("service1", HandleService1);
                struct1.AddChild(service1);

                var service2 = new GetterServiceElement("service2", HandleService2);
                struct1.AddChild(service2);

                var service3 = new SetterServiceElement("service3", HandleService3);
                struct1.AddChild(service3);

                var service4 = new ServiceElement("service4", HandleService4);
                struct1.AddChild(service4);

                var service5 = new ServiceElement("service5", HandleService5);
                struct1.AddChild(service5);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static void HandleService1(IServiceElement element, int? cid)
        {
            Console.WriteLine("Do something");
        }

        private static Variant HandleService2(IServiceElement element, int? cid)
        {
            Console.WriteLine("Do something and return result");
            return new VariantValue(0);
        }

        private static void HandleService3(IServiceElement element, Variant data, int? cid)
        {
            Console.WriteLine($"Do something with {(string)(VariantValue)data}");
        }

        private static Variant HandleService4(IServiceElement element, Variant data, int? cid)
        {
            Console.WriteLine($"Do something with {data} and return result");

            var str = (string)(VariantValue)data;
            return (VariantValue)$"Received {str}";
        }

        private static Variant HandleService5(IServiceElement element, Variant data, int? cid)
        {
            var value = Variant.ToObject<UserData>(data);
            Console.WriteLine($"Do something with {value} and return result");
            return Variant.FromObject(value);
        }
    }

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
}