namespace RnzTrauer.Core.Services;

/// <summary>Read-only availability projection for files associated with a notice.</summary>
public sealed record NoticeMediaState(
    bool HasPdf,
    bool HasPng,
    bool HasProfileImage)
{
    public string Summary =>
        HasProfileImage ? "Profile image available"
        : HasPng ? "PNG available"
        : HasPdf ? "PDF available"
        : "No media available";
}
