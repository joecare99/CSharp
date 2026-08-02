namespace RnzTrauer.Core.Services;

/// <summary>Converts acquired HTML fragments into normalized notice text.</summary>
public interface IHtmlTextNormalizer
{
    /// <summary>Removes markup, decodes HTML entities, and normalizes whitespace.</summary>
    string Normalize(string html);
}
