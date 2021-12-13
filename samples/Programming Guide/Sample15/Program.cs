namespace Sample15
{
    using System;
    using ifmIoTCore;
    using ifmIoTCore.Elements.EventArguments;

    internal class Program
    {
        static void Main()
        {
            try
            {
                var ioTCore = IoTCoreFactory.Create("MyIoTCore");

                // Register treechanged event handler
                ioTCore.TreeChanged += HandleTreeChangedEvent;

                // Create an element and do not raise a treechanged event
                var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, 
                    "struct1");

                // Raise a treechanged event on demand
                ioTCore.RaiseTreeChanged(ioTCore.Root, 
                    struct1, 
                    TreeChangedAction.ElementAdded);

                // Remove an element and raise a treechanged event
                ioTCore.RemoveElement(ioTCore.Root, 
                    struct1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

        private static void HandleTreeChangedEvent(object sender, EventArgs e)
        {
            var treeChangedEventArgs = (TreeChangedEventArgs)e;

            // Handle event
            switch (treeChangedEventArgs.Action)
            {
                case TreeChangedAction.ElementAdded:
                    Console.WriteLine("Element added");
                    break;
                case TreeChangedAction.ElementRemoved:
                    Console.WriteLine("Element removed");
                    break;
                case TreeChangedAction.TreeChanged:
                    Console.WriteLine("Tree changed");
                    break;
            }
        }
    }
}