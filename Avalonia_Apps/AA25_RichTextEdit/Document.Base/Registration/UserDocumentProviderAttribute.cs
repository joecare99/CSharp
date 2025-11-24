using System;

namespace Document.Base.Registration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class UserDocumentProviderAttribute : Attribute
{
    /// <summary>
    /// Primäre Schlüssel (z. B. "html", "pdf", "odf"), unter denen das Dokument ansprechbar ist.
    /// </summary>
    public string[] Keys { get; }

    /// <summary>
    /// Unterstützte Dateiendungen inkl. Punkt (z. B. ".html", ".htm").
    /// </summary>
    public string[] Extensions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Optionaler MIME-Content-Type (z. B. "text/html").
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Anzeigename (optional).
    /// </summary>
    public string? DisplayName { get; init; }

    public UserDocumentProviderAttribute(params string[] keys)
    {
        Keys = keys ?? Array.Empty<string>();
    }
}