namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents a node that can contain ordered child nodes.
/// </summary>
/// <typeparam name="TNode">The node type stored in the container.</typeparam>
/// <remarks>
/// The container contract is intentionally read-only from the outside so that implementers
/// can control how children are added, validated, and ordered. Consumers should treat the
/// returned list as a snapshot of the current child sequence.
/// </remarks>
public interface IDocumentContainer<out TNode>
    where TNode : IDocumentNode
{
    /// <summary>
    /// Gets the ordered children of the node.
    /// </summary>
    /// <remarks>
    /// The order is significant for document rendering and reconstruction scenarios.
    /// For example, page blocks or document pages should usually be enumerated in source order.
    /// </remarks>
    IReadOnlyList<TNode> Children { get; }
}
