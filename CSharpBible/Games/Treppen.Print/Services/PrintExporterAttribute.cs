namespace Treppen.Print.Services;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class PrintExporterAttribute : Attribute
{
    public string Name { get; }
    public string[] Extensions { get; }

    public PrintExporterAttribute(string name, params string[] extensions)
    {
        Name = name;
        Extensions = extensions.Length == 0 ? new[] { ".svg" } : extensions;
    }
}
