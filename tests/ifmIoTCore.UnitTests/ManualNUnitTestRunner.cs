namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class ManualNUnitTestRunner
    {
        /// <summary>
        /// Runs the nunit tests in the specified class.
        /// </summary>
        /// <param name="testFixtureClassType"></param>
        public void Run(Type testFixtureClassType)
        {
            var instance = Activator.CreateInstance(testFixtureClassType);

            ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "Setup");

            foreach (var testMethod in testFixtureClassType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<TestAttribute>() != null))
            {
                ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "PerTestSetup");
                ReflectionInvokeParameterlessMethod(instance, testMethod);
                ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "PerTestTearDown");
            }

            ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "TearDown");
        }

        public void RunSingleMethod(Type testFixtureClassType, string methodname)
        {
            var instance = Activator.CreateInstance(testFixtureClassType);

            ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "Setup");
            ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "PerTestSetup");
            var testMethod = testFixtureClassType.GetMethod(methodname, BindingFlags.Instance | BindingFlags.Public);
            ReflectionInvokeParameterlessMethod(instance, testMethod);
            ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "PerTestTearDown");

            ReflectionInvokeParameterlessMethod(instance, testFixtureClassType, "TearDown");
        }

        public void RunAllInAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetExportedTypes().Where(x => x.GetCustomAttribute<TestFixtureAttribute>() != null))
            {
                Run(type);
            }
        }

        private void ReflectionInvokeParameterlessMethod(object instance, Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            ReflectionInvokeParameterlessMethod(instance, method);
        }

        private void ReflectionInvokeParameterlessMethod(object instance, MethodInfo method)
        {
            if (method?.ReturnType == typeof(Task))
            {
                try
                {
                    var result = (Task) method.Invoke(instance, new object[] { });
                    result.GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    // Do something here if needed.
                }
            }
            else
            {
                try
                {
                    method?.Invoke(instance, new object[] { });
                }
                catch (Exception)
                {
                    // Do something here if needed.
                }
            }
        }
    }
}
