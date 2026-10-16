namespace Document.Base.Models.Interfaces;

/// <summary>
/// Represents an image element in a user document.
/// Confirmed as an independent document element while retaining the existing contract shape for the DOCX tree.
/// </summary>
public interface IDocImage : IDocElement
{
    /// <summary>Gets the encoded image data.</summary>
    byte[] ImageData { get; }

    /// <summary>Gets the image media type.</summary>
    string ContentType { get; }

    /// <summary>Gets the accessible description, if available.</summary>
    string? AltText { get; }
}
