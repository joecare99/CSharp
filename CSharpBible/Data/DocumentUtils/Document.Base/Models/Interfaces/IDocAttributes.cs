namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a named, provider-neutral document attribute.
/// </summary>
public interface IDocAttributes
{
    /// <summary>
    /// Gets the attribute name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the attribute value.
    /// </summary>
    object? Value { get; }
}

/// <summary>
/// Represents a document provider capability for adding a section with layout options.
/// </summary>
public interface IDocSectionProvider
{
    /// <summary>
    /// Adds a section with an optional column count.
    /// </summary>
    IDocElement AddSection(int? columns = null);
}
