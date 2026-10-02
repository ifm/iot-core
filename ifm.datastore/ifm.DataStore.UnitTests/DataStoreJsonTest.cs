namespace ifm.DataStore.UnitTests;

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using NUnit.Framework;

public class DataStoreJsonTest
{
    private const string NameSpace = "ifm.DataStore.UnitTests.Resources.";
    private const string BaseFile = "BaseFile.json";
    private const string TempFile = "temp.json";
    private const string WriteLockFile = "WriteLock.json";
    private const string ReadLockFile = "ReadLock.json";
    private const string ReadWriteLockFile = "ReadWriteLock.json";
    private string _baseFile;
    private static readonly object BaseFileLock = new();

    private IDataStore Prepare()
    {
        lock (BaseFileLock)
        {
            _baseFile = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\" + TempFile;
            if (File.Exists(_baseFile))
            {
                File.Delete(_baseFile);
            }
        }

        var stream = GetType().Assembly.GetManifestResourceStream(NameSpace + BaseFile);
        if (stream == null)
        {
            Assert.Fail("Missing file 'BaseFile.json'");
        }

        var buffer = new byte[stream.Length];
        var _ = stream.Read(buffer, 0, buffer.Length);
        File.WriteAllBytes(_baseFile, buffer);

        return new DataStore(_baseFile);
    }

    [TestCase(TestName = "ReadWriteLock"), NonParallelizable]
    public void ReadWriteLock()
    {
        var baseFile = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "/Resources/" + ReadWriteLockFile;

        IDataStore store = new DataStore(baseFile);

        var tokenSource = new CancellationTokenSource();
        var watch = new Stopwatch();
        Exception exception = null;
        watch.Start();

        Thread[] threads =
        {
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out exception)) { Priority = ThreadPriority.Lowest }
        };

        Parallel.ForEach(threads, t => t.Start());
        foreach (var thread in threads) thread.Join();
            
        tokenSource.Cancel();
        watch.Stop();

        if (exception != null)
            Assert.Fail(exception.Message);
    }

    [TestCase(TestName = "ReadLock"), NonParallelizable]
    public void ReadLock()
    {
        var baseFile = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "/Resources/" + ReadLockFile;

        IDataStore store = new DataStore(baseFile);

        var tokenSource = new CancellationTokenSource();
        var watch = new Stopwatch();
        Exception exception = null;
        watch.Start();

        Thread[] threads =
        {
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => ReadDataStore(store, watch, tokenSource, out exception)) { Priority = ThreadPriority.Lowest }
        };

        Parallel.ForEach(threads, t => t.Start());
        foreach (var thread in threads) thread.Join();

        tokenSource.Cancel();
        watch.Stop();

        if (exception != null)
            Assert.Fail(exception.Message);
    }

    [TestCase(TestName = "WriteLock"), NonParallelizable]
    public void WriteLock()
    {
        var baseFile = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "/Resources/" + WriteLockFile;

        IDataStore store = new DataStore(baseFile);

        var tokenSource = new CancellationTokenSource();
        var watch = new Stopwatch();
        Exception exception = null;
        watch.Start();

        Thread[] threads =
        {
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out _)) { Priority = ThreadPriority.Highest },
            new(() => WriteDataStore(store, watch, tokenSource, out exception)) { Priority = ThreadPriority.Lowest }
        };

        Parallel.ForEach(threads, t => t.Start());
        foreach (var thread in threads) thread.Join();
            
        tokenSource.Cancel();
        watch.Stop();

        if (exception != null)
            Assert.Fail(exception.Message);

        //if (exception == null)
        //    Assert.Fail("No exception available but it was expected.");

        //if (exception.GetType() != typeof(FieldAccessException))
        //    Assert.Fail($"Type of exception was '{exception.GetType()}' but '{typeof(FieldAccessException)}' is expected.");
    }

    [TestCase("valid", "complex", true, TestName = "Get Valid Complex Data Type = not null"), NonParallelizable]
    public void GetComplexConfiguration(string sectionKey, string configKey, bool expectedTestResult)
    {
        try
        {
            var store = Prepare();

            var response = GetValue(store, typeof(ComplexType), sectionKey, configKey);

            // If we get no exception but we expect an invalid test result,
            // we have to throw an exception.
            if (!expectedTestResult)
                Assert.Fail($@"The Test '{TestContext.CurrentContext.Test.FullName}' was valid, but an invalid test result was expected.");

            var expectedValue = new ComplexType();
            if (expectedValue == null)
            {
                throw new InvalidDataException("the parameter 'expectedValue' is null");
            }

            CheckProperties(expectedValue, (ComplexType)response);
        }
        catch (Exception)
        {
            // If we expect an invalid test, all is fine (test successful)
            if (!expectedTestResult)
                return;

            // if not rethrow the exception (test failed).
            throw;
        }
    }

    private void ReadDataStore(IDataStore store, Stopwatch watch, CancellationTokenSource tokenSource, out Exception exception)
    {
        try
        {
            exception = null;
            while (watch.ElapsedMilliseconds < 200 && !tokenSource.IsCancellationRequested)
            {
                store.Get<string>("valid", "string1");
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }
    }

    private void WriteDataStore(IDataStore store, Stopwatch watch, CancellationTokenSource tokenSource, out Exception exception)
    {
        try
        {
            exception = null;
            while (watch.ElapsedMilliseconds < 200 && !tokenSource.IsCancellationRequested)
            {
                store.Set("valid", "string1", "23");
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }
    }

    private static void CheckProperties(ComplexType expectedValue, ComplexType actualValue)
    {
        var properties = expectedValue.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType.Name.Equals(nameof(ComplexType)) && actualValue.MyComplexType != null)
                CheckProperties(expectedValue, actualValue.MyComplexType);
                    
            var expected = property.GetValue(expectedValue, null);
            if (expected == null)
                return;
            var actual = property.GetValue(actualValue, null);

            if (actual is IList list)
            {
                AssertListsAreEquals(property, list, (IList)expected);
            }
            else if (!Equals(expected, actual))
            {
                Assert.Fail($"Property {property.DeclaringType?.Name}.{property.Name} does not match. Expected: {expectedValue} but was: {actualValue}");
            }
        }
    }

    private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
    {
        if (actualList.Count != expectedList.Count)
            Assert.Fail($"Property {property.PropertyType.Name}.{property.Name} does not match. Expected IList containing {expectedList.Count} elements but was IList containing {actualList.Count} elements");

        for (var i = 0; i < actualList.Count; i++)
            if (!Equals(actualList[i], expectedList[i]))
                Assert.Fail($"Property {property.PropertyType.Name}.{property.Name} does not match. Expected IList with element {property.Name} equals to {expectedList[i]} but was IList with element {property.Name} equals to {actualList[i]}");
    }

    [NonParallelizable]
    // These tests will fail because newton soft and microsoft json parser converts a "true" to a valid boolean ...
    //[TestCase(typeof(bool), "invalid", "boolean1", true, false, TestName = "Get Invalid Boolean = \"true\"")]
    //[TestCase(typeof(bool), "invalid", "boolean2", true, false, TestName = "Get Invalid Boolean = 23")]
    //[TestCase(typeof(string), "invalid", "string1", "true", false, TestName = "Get Invalid String = true")]
    //[TestCase(typeof(string), "invalid", "string2", "23", false, TestName = "Get Invalid String = 23")]

    [TestCase(typeof(bool), "valid", "boolean", true, true, TestName = "Get Valid Boolean = true")]
    [TestCase(typeof(string), "valid", "string1", "23", true, TestName = "Get Valid String = \"23\"")]
    [TestCase(typeof(string), "valid", "string2", "true", true, TestName = "Get Valid String = \"true\"")]
    [TestCase(typeof(uint), "valid", "uint", 652000, true, TestName = "Get Valid Unsigned Integer = 652000")]
    [TestCase(typeof(int), "valid", "int", -652000, true, TestName = "Get Valid Integer = -652000")]
    [TestCase(typeof(ushort), "valid", "ushort", 65200, true, TestName = "Get Valid Unsigned Short = 65200")]
    [TestCase(typeof(short), "valid", "short", -32200, true, TestName = "Get Valid Short = -32200")]
    [TestCase(typeof(double), "valid", "double", 65.200, true, TestName = "Get Valid Double = 65,200")]
    [TestCase(typeof(double), "valid", "double1", 65.234, true, TestName = "Get Valid positive Double = 65,234")]
    [TestCase(typeof(double), "valid", "double2", -65.234, true, TestName = "Get Valid negative Double = -65,234")]
    [TestCase(typeof(byte), "valid", "byte", 255, true, TestName = "Get Valid Byte = 255")]
    [TestCase(typeof(float), "valid", "float", (float)65.200, true, TestName = "Get Valid Float = 65,200")]
    [TestCase(typeof(float), "valid", "float1", (float)65.234, true, TestName = "Get Valid positive Float = 65,234")]
    [TestCase(typeof(float), "valid", "float2", (float)-65.234, true, TestName = "Get Valid negative Float = -65,234")]
    [TestCase(typeof(int[]), "valid", "intarray", new int[] { 1 }, true, TestName = "Get Valid Intarray with one value = [ 1 ]")]
    [TestCase(typeof(int[]), "valid", "intarrayempty", new int[] { }, true, TestName = "Get Valid Intarray empty = []")]
    [TestCase(typeof(int[]), "valid", "intarraynull", null, true, TestName = "Get Valid Intarray null = null")]
    [TestCase(typeof(string[]), "valid", "stringarray", new string[]{""}, true, TestName = "Get Valid Stringarray with one empty string = [\"\"]")]
    [TestCase(typeof(string[]), "valid", "stringarrayempty", new string[]{ }, true, TestName = "Get Valid Stringarray empty = []")]
    [TestCase(typeof(string[]), "valid", "stringarraynull", null, true, TestName = "Get Valid Stringarray null = null")]
    [TestCase(typeof(uint), "invalid", "uint", 652000, false, TestName = "Get Invalid Unsigned Integer = 65,200")]
    [TestCase(typeof(int), "invalid", "int", -652000, false, TestName = "Get Invalid Integer = 65,200")]
    [TestCase(typeof(ushort), "invalid", "ushort1", 65200, false, TestName = "Get Invalid Unsigned Short = 652000")]
    [TestCase(typeof(ushort), "invalid", "ushort2", 65200, false, TestName = "Get Invalid Unsigned Short = -32200")]
    [TestCase(typeof(short), "invalid", "short", -32200, false, TestName = "Get Invalid Short = -3222000")]
    [TestCase(typeof(double), "invalid", "double", 65.200, false, TestName = "Get Invalid Double = true")]
    [TestCase(typeof(double), "invalid", "double1", 65.234, false, TestName = "Get Invalid positive Double = true")]
    [TestCase(typeof(double), "invalid", "double2", -65.234, false, TestName = "Get Invalid negative Double = true")]
    [TestCase(typeof(byte), "invalid", "byte", 255, false, TestName = "Get Invalid Byte = 257")]
    [TestCase(typeof(float), "invalid", "double", (float)65.200, false, TestName = "Get Invalid Float = true")]
    [TestCase(typeof(float), "invalid", "float1", (float)65.234, false, TestName = "Get Invalid positive Float = true")]
    [TestCase(typeof(float), "invalid", "float2", (float)-65.234, false, TestName = "Get Invalid negative Float = true")]
    public void GetConfiguration(Type type, string sectionKey, string configKey, object expectedValue, bool expectedTestResult)
    {
        try
        {
            var store = Prepare();
            var response = GetValue(store, type, sectionKey, configKey);
                
            // If we get no exception but we expect an invalid test result,
            // we have to throw an exception.
            if (!expectedTestResult)
                Assert.Fail($@"The Test '{TestContext.CurrentContext.Test.FullName}' was valid, but an invalid test result was expected.");

            //if (expectedValue == null)
            //{
            //    throw new InvalidDataException("the parameter 'expectedValue' is null");
            //}

            Assert.That(response, Is.EqualTo(expectedValue));
        }
        catch (Exception)
        {
            // If we expect an invalid test, all is fine (test successful)
            if (!expectedTestResult)
                return;

            // if not rethrow the exception (test failed).
            throw;
        }
    }

    [NonParallelizable]
    [TestCase(typeof(string), "valid", "string1", "23", true, TestName = "Set Valid String = \"23\"")]
    [TestCase(typeof(string), "valid", "string2", "true", true, TestName = "Set Valid String = \"true\"")]
    [TestCase(typeof(bool), "valid", "boolean", false, true, TestName = "Set Valid Boolean = false")]
    [TestCase(typeof(uint), "valid", "uint", (uint)450000, true, TestName = "Set Valid Unsigned Integer = 450000")]
    [TestCase(typeof(int), "valid", "int", -452000, true, TestName = "Set Valid Integer = -452000")]
    [TestCase(typeof(ushort), "valid", "ushort", (ushort)45200, true, TestName = "Set Valid Unsigned Short = 45200")]
    [TestCase(typeof(short), "valid", "short", (short)-22200, true, TestName = "Set Valid Unsigned Short = -22200")]
    [TestCase(typeof(double), "valid", "double", 45.200, true, TestName = "Set Valid Double = 45,200")]
    [TestCase(typeof(double), "valid", "double1", 45.234, true, TestName = "Set Valid Double = 45,234")]
    [TestCase(typeof(double), "valid", "double2", -45.234, true, TestName = "Get Valid Double = -45,234")]
    [TestCase(typeof(byte), "valid", "byte", (byte)235, true, TestName = "Set Valid Byte = 235")]
    [TestCase(typeof(float), "valid", "float", (float)45.200, true, TestName = "Set Valid Float = 45,200")]
    [TestCase(typeof(float), "valid", "float1", (float)45.234, true, TestName = "Set Valid Float = 45,234")]
    [TestCase(typeof(float), "valid", "float2", (float)-45.234, true, TestName = "Get Valid Float = -45,234")]
    [TestCase(typeof(int[]), "valid", "intarray", new int[] { 1 }, true, TestName = "Set Valid Intarray with one value = [ 1 ] ")]
    [TestCase(typeof(int[]), "valid", "intarrayempty", new int[] { }, true, TestName = "Set Valid Intarray empty = [] ")]
    [TestCase(typeof(int[]), "valid", "intarraynull", null, true, TestName = "Set Valid Intarray null = null ")]
    [TestCase(typeof(string[]), "valid", "stringarray", new string[] { "" }, true, TestName = "Set Valid Stringarray with one empty string = [\"\"] ")]
    [TestCase(typeof(string[]), "valid", "stringarrayempty", new string[] { }, true, TestName = "Set Valid Stringarray empty = [] ")]
    [TestCase(typeof(string[]), "valid", "stringarraynull", null, true, TestName = "Set Valid Stringarray null = null ")]
    [TestCase(typeof(bool), "invalid", "boolean1", "false", false, TestName = "Set InValid Boolean = \"false\"")]
    [TestCase(typeof(bool), "invalid", "boolean2", 23, false, TestName = "Set InValid Boolean = 23")]
    [TestCase(typeof(string), "invalid", "string1", true, false, TestName = "Set InValid String = true")]
    [TestCase(typeof(string), "invalid", "string2", 23, false, TestName = "Set InValid String = 23")]
    [TestCase(typeof(uint), "invalid", "uint", 65.200, false, TestName = "Set InValid Unsigned Integer = 65,200")]
    [TestCase(typeof(int), "invalid", "int", 65.200, false, TestName = "Set InValid Integer = 65,200")]
    [TestCase(typeof(ushort), "invalid", "ushort1", 652000, false, TestName = "Set InValid Unsigned Short = 652000")]
    [TestCase(typeof(ushort), "invalid", "ushort2", -32200, false, TestName = "Set InValid Unsigned Short = -32200")]
    [TestCase(typeof(short), "invalid", "short", 652000, false, TestName = "Set InValid Short = 652000")]
    [TestCase(typeof(double), "invalid", "double", true, false, TestName = "Set InValid Double = true")]
    [TestCase(typeof(byte), "invalid", "byte", 257, false, TestName = "Set InValid Byte = 257")]
    [TestCase(typeof(float), "invalid", "float", true, false, TestName = "Set InValid Float = true")]
    public void SetConfiguration(Type type, string sectionKey, string configKey, object value, bool expectedTestResult)
    {
        try
        {
            var store = Prepare();

            // Write the value into section and config position.
            // The file is the local TempFile (temp.json) a copy of the BaseFile.json
            var fileStream = SetValue(store, type, sectionKey, configKey, value);
            if (fileStream == null)
                Assert.Fail("Missing comparable file");

            var test = new byte[fileStream.Length];
            var _ = fileStream.Read(test, 0, test.Length);

            var str1 = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(_baseFile));
            var str2 = System.Text.Encoding.UTF8.GetString(test);

            Assert.That(str1, Is.EqualTo(str2));
        }
        catch (InvalidCastException)
        {
            // If we expect an invalid test, all is fine (test successful)
            if (!expectedTestResult)
                return;

            // if not rethrow the exception (test failed).
            throw;
        }
    }

    [TestCase(typeof(string), "valid", "string1", "23", true, TestName = "Roundtrip Valid String = \"23\"")]
    [TestCase(typeof(string), "valid", "string2", "true", true, TestName = "Roundtrip Valid String = \"true\"")]
    [TestCase(typeof(bool), "valid", "boolean", false, true, TestName = "Roundtrip Valid Boolean = false")]
    [TestCase(typeof(uint), "valid", "uint", (uint)450000, true, TestName = "Roundtrip Valid Unsigned Integer = 450000")]
    [TestCase(typeof(int), "valid", "int", -452000, true, TestName = "Roundtrip Valid Integer = -452000")]
    [TestCase(typeof(ushort), "valid", "ushort", (ushort)45200, true, TestName = "Roundtrip Valid Unsigned Short = 45200")]
    [TestCase(typeof(short), "valid", "short", (short)-22200, true, TestName = "Roundtrip Valid Unsigned Short = -22200")]
    [TestCase(typeof(double), "valid", "double", 45.200, true, TestName = "Roundtrip Valid Double = 45,200")]
    [TestCase(typeof(double), "valid", "double1", 45.234, true, TestName = "Roundtrip Valid Double = 45,234")]
    [TestCase(typeof(double), "valid", "double2", -45.234, true, TestName = "Get Valid Double = -45.234")]
    [TestCase(typeof(byte), "valid", "byte", (byte)235, true, TestName = "Roundtrip Valid Byte = 235")]
    [TestCase(typeof(float), "valid", "float", (float)45.200, true, TestName = "Roundtrip Valid Float = 45,200")]
    [TestCase(typeof(float), "valid", "float1", (float)45.234, true, TestName = "Roundtrip Valid Float = 45,234")]
    [TestCase(typeof(float), "valid", "float2", (float)-45.234, true, TestName = "Get Valid Float = -45.234")]
    [TestCase(typeof(int[]), "valid", "intarray", new int[] { 1 }, true, TestName = "Roundtrip Valid Intarray with one value = [ 1 ] ")]
    //[TestCase(typeof(int[]), "valid", "intarrayempty", new int[] { }, true, TestName = "Roundtrip Valid Intarray empty = [] ")]
    [TestCase(typeof(int[]), "valid", "intarraynull", null, true, TestName = "Roundtrip Valid Intarray null = null ")]
    [TestCase(typeof(string[]), "valid", "stringarray", new string[] { "" }, true, TestName = "Roundtrip Valid Stringarray with one empty string = [\"\"] ")]
    //[TestCase(typeof(string[]), "valid", "stringarrayempty", new string[] { }, true, TestName = "Roundtrip Valid Stringarray empty = [] ")]
    [TestCase(typeof(string[]), "valid", "stringarraynull", null, true, TestName = "Roundtrip Valid Stringarray null = null ")]
    public void RoundTripConfiguration(Type type, string sectionKey, string configKey, object value, bool expectedTestResult)
    {
        var store = Prepare();
        using var fileStream = SetValue(store, type, sectionKey, configKey, value);
        var response = GetValue(store, type, sectionKey, configKey);

        Assert.That(response, Is.EqualTo(value));
    }

    private Stream SetValue(IDataStore store, Type type, string sectionKey, string configKey, object value)
    {
        if (type == typeof(bool))
        {
            store.Set(sectionKey, configKey, (bool)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "BoolTest.json");
        }
        if (type == typeof(string))
        {
            store.Set(sectionKey, configKey, (string)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "StringTest.json");
        }
        if (type == typeof(uint))
        {
            store.Set(sectionKey, configKey, (uint)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "UnsignedIntegerTest.json");
        }
        if (type == typeof(int))
        {
            store.Set(sectionKey, configKey, (int)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "IntegerTest.json");
        }
        if (type == typeof(ushort))
        {
            store.Set(sectionKey, configKey, (ushort)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "UnsignedShortTest.json");
        }
        if (type == typeof(short))
        {
            store.Set(sectionKey, configKey, (short)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "ShortTest.json");
        }
        if (type == typeof(double))
        {
            store.Set(sectionKey, configKey, (double)value);

            switch (configKey)
            {
                case "double":
                    return GetType().Assembly.GetManifestResourceStream(NameSpace + "DoubleTest.json");
                case "double1":
                    return GetType().Assembly.GetManifestResourceStream(NameSpace + "Double1Test.json");
                case "double2":
                    return GetType().Assembly.GetManifestResourceStream(NameSpace + "Double2Test.json");
            }
        }
        if (type == typeof(float))
        {
            store.Set(sectionKey, configKey, (float)value);

            switch (configKey)
            {
                case "float":
                    return GetType().Assembly.GetManifestResourceStream(NameSpace + "FloatTest.json");
                case "float1":
                    return GetType().Assembly.GetManifestResourceStream(NameSpace + "Float1Test.json");
                case "float2":
                    return GetType().Assembly.GetManifestResourceStream(NameSpace + "Float2Test.json");
            }
        }
        if (type == typeof(byte))
        {
            store.Set(sectionKey, configKey, (byte)value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "ByteTest.json");
        }

        if (type == typeof(string[]))
        {
            store.Set(sectionKey, configKey, (string[])value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "StringArray.json");
        }

        if (type == typeof(int[]))
        {
            store.Set(sectionKey, configKey, (int[])value);
            return GetType().Assembly.GetManifestResourceStream(NameSpace + "IntArray.json");
        }

        throw new Exception($@"Type '{type}' not given");
    }

    private object GetValue(IDataStore store, Type type, string sectionKey, string configKey)
    {
        if (type == typeof(bool))
            return store.Get<bool>(sectionKey, configKey);
        if (type == typeof(string))
            return store.Get<string>(sectionKey, configKey);
        if (type == typeof(uint))
            return store.Get<uint>(sectionKey, configKey);
        if (type == typeof(int))
            return store.Get<int>(sectionKey, configKey);
        if (type == typeof(ushort))
            return store.Get<ushort>(sectionKey, configKey);
        if (type == typeof(short))
            return store.Get<short>(sectionKey, configKey);
        if (type == typeof(double))
            return store.Get<double>(sectionKey, configKey);
        if (type == typeof(byte))
            return store.Get<byte>(sectionKey, configKey);
        if (type == typeof(float))
            return store.Get<float>(sectionKey, configKey);
        if (type == typeof(ComplexType))
            return store.Get<ComplexType>(sectionKey, configKey);
        if (type == typeof(string[]))
            return store.Get<string[]>(sectionKey, configKey);
        if (type == typeof(int[]))
            return store.Get<int[]>(sectionKey, configKey);
        
        throw new Exception($@"Type '{type}' not given");
    }
}