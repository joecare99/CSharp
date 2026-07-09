namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Identifies the host surface where a workbench command can be presented.
/// </summary>
public enum WorkbenchCommandSurface
{
    /// <summary>
    /// The command is contributed to a menu structure.
    /// </summary>
    Menu,

    /// <summary>
    /// The command is contributed to a toolbar structure.
    /// </summary>
    Toolbar,

    /// <summary>
    /// The command is contributed to a context menu structure.
    /// </summary>
    ContextMenu,

    /// <summary>
    /// The command is contributed to a command palette structure.
    /// </summary>
    CommandPalette
}
