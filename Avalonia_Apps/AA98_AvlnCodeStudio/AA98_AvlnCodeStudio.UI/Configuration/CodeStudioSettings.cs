namespace AA98_AvlnCodeStudio.UI.Configuration;

/// <summary>
/// Stores host-owned settings for the CodeStudio workbench configuration section.
/// </summary>
public sealed class CodeStudioSettings
{
    /// <summary>Gets or sets the initial workspace path shown by the workbench.</summary>
    public string? WorkspacePath { get; set; }

    /// <summary>Gets or sets whether files reopen when the workbench starts.</summary>
    public bool ReopenLastDocument { get; set; } = true;
}
