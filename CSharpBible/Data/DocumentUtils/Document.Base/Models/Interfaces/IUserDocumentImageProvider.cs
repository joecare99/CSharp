namespace Document.Base.Models.Interfaces;

/// <summary>
/// Optional capability for user document providers that can embed images.
/// </summary>
public interface IUserDocumentImageProvider
{
    /// <summary>
    /// Adds an encoded image to the document and returns its document-tree element.
    /// </summary>
    /// <param name="imageData">The encoded image data.</param>
    /// <param name="contentType">The media type of the encoded image.</param>
    /// <param name="altText">An optional accessible description.</param>
    /// <returns>The image element added to the document.</returns>
    IDocImage AddImage(byte[] imageData, string contentType, string? altText = null);
}
