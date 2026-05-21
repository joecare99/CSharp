namespace RepoMigrator.Providers.Patch;

/// <summary>
/// Defines how a patch collection should be accessed by the patch source provider.
/// </summary>
public enum PatchSourceLocationKind
{
    /// <summary>
    /// Reads patch files from a local directory.
    /// </summary>
    LocalDirectory
}
