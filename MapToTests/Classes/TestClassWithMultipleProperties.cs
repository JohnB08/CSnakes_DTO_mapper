using ClassImport.Attributes;

namespace ClassImportTests.Classes;

public class TestClassWithMultipleProperties
{
    [PythonPropertyName("testStr")] 
    public string TestString { get; set; }
    
    [PythonPropertyName("testInt")]
    public int TestInt { get; set; }
    
}