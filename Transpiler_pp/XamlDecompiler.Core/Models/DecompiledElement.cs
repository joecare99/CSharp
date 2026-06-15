namespace XamlDecompiler.Core.Models;

/// <summary>
/// Represents a reconstructed XAML object node.
/// </summary>
public sealed class DecompiledElement
{
    public DecompiledElement(string typeName, string variableName)
    {
        TypeName = typeName;
        VariableName = variableName;
    }

    public string TypeName { get; }

    public string VariableName { get; }

    public string? XamlName { get; set; }

    public string? ResourceKey { get; set; }

    public IDictionary<string, string> Attributes { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

    public IDictionary<string, string> Events { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

    public IList<DecompiledElement> Children { get; } = new List<DecompiledElement>();

    public IDictionary<string, IList<DecompiledElement>> PropertyElements { get; } = new Dictionary<string, IList<DecompiledElement>>(StringComparer.Ordinal);

    public IList<string> Comments { get; } = new List<string>();

    public DecompiledElement? Content { get; set; }

    public string? TextValue { get; set; }

    public void AddChild(DecompiledElement child)
    {
        if (!Children.Contains(child))
        {
            Children.Add(child);
        }
    }

    public void AddPropertyElement(string propertyName, DecompiledElement child)
    {
        if (!PropertyElements.TryGetValue(propertyName, out IList<DecompiledElement>? elements))
        {
            elements = new List<DecompiledElement>();
            PropertyElements[propertyName] = elements;
        }

        if (!elements.Contains(child))
        {
            elements.Add(child);
        }
    }
}
