namespace Workbench.Builder.Host;

/// <summary>
/// Defines supported verbosity levels for the builder compile host.
/// </summary>
public enum HostVerbosity
{
    /// <summary>
    /// Writes only the standard host output.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Writes additional progress details.
    /// </summary>
    Detailed = 1,
}