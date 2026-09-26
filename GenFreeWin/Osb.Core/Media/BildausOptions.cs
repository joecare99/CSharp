namespace Osb.Core.Media;

/// <summary>
/// Options controlling which picture records are processed by the picture export.
/// Replaces the legacy magic indexes into the global <c>COND.aus</c> settings array.
/// </summary>
public sealed class BildausOptions
{
    public bool IncludePictures { get; }
    public bool IncludeDescriptions { get; }
    public int TargetHeight { get; }

    public BildausOptions(bool IncludePictures, bool IncludeDescriptions, int TargetHeight)
    {
        this.IncludePictures = IncludePictures;
        this.IncludeDescriptions = IncludeDescriptions;
        this.TargetHeight = TargetHeight;
    }
}
