namespace Avln_Bubbles.View.Services;

/// <summary>
/// Exposes host capabilities to ViewModels without binding them to a specific UI platform API.
/// </summary>
public interface IHostFeatureDescriptor
{
    /// <summary>
    /// Gets the host kind.
    /// </summary>
    HostPlatformKind HostKind { get; }

    /// <summary>
    /// Gets a short host label for the UI.
    /// </summary>
    string HostLabel { get; }

    /// <summary>
    /// Gets a value indicating whether native desktop windowing is available.
    /// </summary>
    bool SupportsDesktopWindowing { get; }

    /// <summary>
    /// Gets a value indicating whether browser hosting is available.
    /// </summary>
    bool SupportsBrowserHosting { get; }

    /// <summary>
    /// Gets a value indicating whether the composition is prepared for future remote hosting.
    /// </summary>
    bool SupportsRemoteHostingPreparation { get; }

    /// <summary>
    /// Gets a text summary describing the remote-hosting preparation.
    /// </summary>
    string RemoteHostingSummary { get; }
}
