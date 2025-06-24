using ClassImport.Attributes;

namespace ClassImportTests.Classes;

public class TestNestedObject
{
    [PythonPropertyName("id")]
    public int Id { get; set; }
    
    [PythonPropertyName("name")]
    public string Name { get; set; }
    
    [PythonPropertyName("nested_obj")]
    public NestedObject NestedObject { get; set; }
}
public class NestedObject
{
    [PythonPropertyName("number")]
    public int Number { get; set; }
        
    [PythonPropertyName("word")]
    public string Word { get; set; }
}

