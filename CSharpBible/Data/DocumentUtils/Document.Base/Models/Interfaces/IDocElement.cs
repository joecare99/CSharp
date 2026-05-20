namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a document element in the legacy document tree contract.
/// </summary>
/// <remarks>
/// This interface extends the DOM-style contract with document-specific creation and traversal
/// helpers. It is intentionally more specialized than <see cref="IDOMElement"/> but still serves
/// as a low-level contract rather than a full document model.
/// </remarks>
public interface IDocElement : IDOMElement
{
    /// <summary>
    /// Appends a document element of the specified type.
    /// </summary>
    /// <param name="aType">The element kind or logical type.</param>
    /// <returns>The appended element.</returns>
    /// <remarks>
    /// Implementations usually interpret the enum value as a factory hint for creating a child
    /// document element of the appropriate concrete type.
    /// </remarks>
    IDocElement AppendDocElement(Enum aType);

    /// <summary>
    /// Appends a document element of the specified type and concrete runtime class.
    /// </summary>
    /// <param name="aType">The element kind or logical type.</param>
    /// <param name="aClass">The concrete class to instantiate or associate with the element.</param>
    /// <returns>The appended element.</returns>
    /// <remarks>
    /// This overload is useful when the caller knows both the semantic role and the concrete
    /// implementation type to be created.
    /// </remarks>
    IDocElement AppendDocElement(Enum aType, Type aClass);

    /// <summary>
    /// Appends a document element with an additional attribute and optional identifier.
    /// </summary>
    /// <param name="aType">The element kind or logical type.</param>
    /// <param name="aAttribute">An additional attribute key or classification value.</param>
    /// <param name="value">The attribute value.</param>
    /// <param name="aClass">The concrete class to instantiate or associate with the element.</param>
    /// <param name="Id">An optional element identifier.</param>
    /// <returns>The appended element.</returns>
    /// <remarks>
    /// This overload is typically used when the caller needs to stamp the element with both
    /// classification and a stable identifier for later lookup or linking.
    /// </remarks>
    IDocElement AppendDocElement(Enum aType, Enum aAttribute, string value, Type aClass, string? Id);

    /// <summary>
    /// Enumerates the current element and its descendants.
    /// </summary>
    /// <returns>A traversal sequence over the node and its children.</returns>
    /// <remarks>
    /// The traversal order is implementation-defined, but it should be stable and suitable for
    /// rendering or inspection.
    /// </remarks>
    IEnumerable<IDocElement> Enumerate();
}
