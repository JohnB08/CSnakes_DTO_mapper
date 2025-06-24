using ClassImport.Attributes;

namespace ClassImportTests.Classes;

public class CollectionClass
{
    [PythonPropertyName("numbers")]
    public IReadOnlyList<long> Collection { get; set; }
}