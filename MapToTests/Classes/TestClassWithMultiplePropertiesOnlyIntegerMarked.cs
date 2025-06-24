using ClassImport.Attributes;

namespace ClassImportTests.Classes;

public class TestClassWithMultiplePropertiesOnlyIntegerMarked
{
    public string? TestString { get; set; }
    [PythonPropertyName("testInt")]
    public int TestInt { get; set; }
}