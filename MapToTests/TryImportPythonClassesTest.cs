using ClassImport.Extensions;
using ClassImportTests.Classes;
using CSnakes.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace ClassImportTests;

public class ClassImportTests
{
    private IPythonEnvironment _env;
    private IHost _app;

    public ClassImportTests(ITestOutputHelper output)
    {
        var builder = Host.CreateApplicationBuilder();
        var home = Path.Join(Environment.CurrentDirectory, "./PythonModules");
        
        builder.Services
            .WithPython()
            .WithHome(home)
            .FromRedistributable();
        
        _app = builder.Build();

        _env = _app.Services.GetRequiredService<IPythonEnvironment>();
        
    }
    [Fact]
    public void ImportModuleAndFetchSimpleClassWithStringFromMethodCall()
    {
        var classModule = _env.PythonTestClass();
        var testObj = classModule.CreateGreeter("John");
        var testGreeter = testObj.MapTo<TestClass>();
        Assert.Equal("John", testGreeter.Name);
    }

    [Fact]
    public void ImportCollectionFromPythonModule()
    {
        var env = _env.ClassWithCollections();
        IReadOnlyList<long> collection = [1, 2, 3, 4, 5, 6, 8, 7, 9, 10];
        
        var testObj = env.CreateCollection(collection);
        var collClass = testObj.MapTo<CollectionClass>();
        
        Assert.Equivalent(collection, collClass.Collection);
    }

    [Fact]
    public void ImportClassWithMultipleProperties()
    {
        var env = _env.ClassWithMultipleProperties();
        var testObj = env.GetTestClassWithMultipleProperties("testString", 123);
        var testMultiplePropertiesObj = testObj.MapTo<TestClassWithMultipleProperties>();
        
        Assert.Equal("testString", testMultiplePropertiesObj.TestString);
        Assert.Equal(123, testMultiplePropertiesObj.TestInt);
    }

    [Fact]
    public void ImportOnlySelectedPropertiesFromClass()
    {
        var env = _env.ClassWithMultipleProperties();
        var testObj = env.GetTestClassWithMultipleProperties("testString", 123);
        var testOnlyIntMappedObj = testObj.MapTo<TestClassWithMultiplePropertiesOnlyIntegerMarked>();
        
        Assert.True(string.IsNullOrWhiteSpace(testOnlyIntMappedObj.TestString));
        Assert.Equal(123, testOnlyIntMappedObj.TestInt);
        
    }
}