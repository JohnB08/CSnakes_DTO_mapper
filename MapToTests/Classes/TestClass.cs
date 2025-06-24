using ClassImport.Attributes;

namespace ClassImportTests.Classes;

public class TestClass
{
    [PythonPropertyName("name")]
    public string Name { get; set; }
}