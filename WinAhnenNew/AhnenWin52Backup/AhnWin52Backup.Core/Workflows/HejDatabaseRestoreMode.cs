namespace AhnWin52Backup.Core.Workflows;

/// <summary>Destination handling strategy for a HEJ database restore.</summary>
public enum HejDatabaseRestoreMode
{
    ExistingTemplate,
    FromScratch,
    Override
}
