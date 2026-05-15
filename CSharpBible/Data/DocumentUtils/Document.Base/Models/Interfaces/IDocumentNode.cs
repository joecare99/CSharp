using Document.Base.Models.Enums;

namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a node in the shared document graph.
/// </summary>
/// <remarks>
/// This is the minimal contract that all higher-level document objects should implement.
/// The interface does not prescribe storage, mutability, or parent-child navigation.
/// It only provides the information needed for identification and basic classification.
/// </remarks>
public interface IDocumentNode
{
    /// <summary>
    /// Gets the optional stable identifier of the node.
    /// </summary>
    /// <remarks>
    /// The identifier can be used to correlate a node with a source object, a rendered
    /// artifact, or a downstream analysis result. An identifier is not required to be
    /// globally unique, but it should be stable within the context of a single document.
    /// </remarks>
    string? Id { get; }

    /// <summary>
    /// Gets the coarse-grained node kind.
    /// </summary>
    /// <remarks>
    /// The kind is meant for routing and traversal decisions. Detailed semantics belong
    /// to the concrete model type or to attached metadata.
    /// </remarks>
    DocumentElementKind Kind { get; }
}
