using Document.Flow.Model;

namespace Document.Flow;

/// <summary>
/// Represents the root of a flow content document.
/// </summary>
public class FlowDocument
{
    /// <summary>
    /// Gets the collection of blocks that form the content of the document.
    /// </summary>
    public BlockCollection Blocks { get; } = new();
}
