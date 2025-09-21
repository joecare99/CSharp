namespace Document.Base.Models.Interfaces;

public interface IDocHeadline: IDocContent
{
    int Level { get; }
    /// <summary>
    /// Gets the unique identifier for this instance. also helps for creating anchors and links.
    /// </summary>
    string Id { get; }
}