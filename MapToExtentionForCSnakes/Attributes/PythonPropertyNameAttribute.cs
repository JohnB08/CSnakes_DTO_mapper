namespace ClassImport.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PythonPropertyName(string name = "") : Attribute
{
    public string Name { get; set; } = name;
}