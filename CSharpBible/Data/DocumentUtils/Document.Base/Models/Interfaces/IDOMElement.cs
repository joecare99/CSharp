namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents the minimal DOM-style contract used by the document object model.
/// </summary>
/// <remarks>
/// This interface is intentionally generic and low-level. It provides a small common surface
/// for storing attributes and child nodes without forcing a specific rendering or persistence
/// strategy. Higher-level document objects can use this contract to expose tree-like structure.
/// </remarks>
public interface IDOMElement
{
    /// <summary>
    /// Gets the attribute collection for the node.
    /// </summary>
    /// <remarks>
    /// Attributes are represented as simple string key/value pairs so that implementations can
    /// store structural, semantic, or rendering-related metadata without taking a dependency on a
    /// particular schema.
    /// </remarks>
    IDictionary<string, string> Attributes { get; }

    /// <summary>
    /// Retrieves a single attribute value by name.
    /// </summary>
    /// <param name="name">The attribute name.</param>
    /// <returns>The attribute value, or <c>null</c> if the attribute is not present.</returns>
    /// <remarks>
    /// Implementations should typically treat attribute names in a stable and predictable way,
    /// for example by using a case-sensitive or case-insensitive lookup consistently.
    /// </remarks>
    string? GetAttribute(string name);

    /// <summary>
    /// Gets the ordered child nodes.
    /// </summary>
    /// <remarks>
    /// The order of the nodes is significant for document reconstruction and rendering.
    /// Consumers should preserve the sequence when traversing or serializing the tree.
    /// </remarks>
    IList<IDOMElement> Nodes { get; }

    /// <summary>
    /// Adds a child node to the current node.
    /// </summary>
    /// <param name="element">The node to add.</param>
    /// <returns>The added node.</returns>
    /// <remarks>
    /// Implementations may normalize, validate, or wrap the supplied node before storing it.
    /// </remarks>
    IDOMElement AddChild(IDOMElement element);
}
