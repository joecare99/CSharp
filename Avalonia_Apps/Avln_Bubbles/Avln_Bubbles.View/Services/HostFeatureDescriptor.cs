namespace Avln_Bubbles.View.Services;

/// <summary>
/// Default host capability descriptor for the shared Bubbles application.
/// </summary>
public sealed class HostFeatureDescriptor : IHostFeatureDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HostFeatureDescriptor"/> class.
    /// </summary>
    /// <param name="hostKind">The active host kind.</param>
    public HostFeatureDescriptor(HostPlatformKind hostKind)
    {
        HostKind = hostKind;
    }

    /// <inheritdoc/>
    public HostPlatformKind HostKind { get; }

    /// <inheritdoc/>
    public string HostLabel => HostKind switch
    {
        HostPlatformKind.Desktop => "Desktop host",
        HostPlatformKind.Browser => "Browser host",
        HostPlatformKind.Remote => "Remote host",
        _ => "Unknown host"
    };

    /// <inheritdoc/>
    public bool SupportsDesktopWindowing => HostKind == HostPlatformKind.Desktop;

    /// <inheritdoc/>
    public bool SupportsBrowserHosting => HostKind == HostPlatformKind.Browser;

    /// <inheritdoc/>
    public bool SupportsRemoteHostingPreparation => true;

    /// <inheritdoc/>
    public string RemoteHostingSummary => "The game model, shared Avalonia view layer, and thin host projects are separated so future remote or streamed hosts can reuse the same application composition.";
}
