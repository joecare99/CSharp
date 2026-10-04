namespace OFBCreator.Projects.Models;

/// <summary>
/// Describes a loaded project and whether it needs explicit migration saving.
/// </summary>
public sealed record OFBProjectLoadResult(
    OFBProject Project,
    string ProjectPath,
    bool RequiresMigrationSave,
    int SourceSchemaVersion);
