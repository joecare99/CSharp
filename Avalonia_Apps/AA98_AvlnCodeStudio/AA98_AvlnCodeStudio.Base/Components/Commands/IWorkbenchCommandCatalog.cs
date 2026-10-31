using System.Collections.Generic;
using AppKomponentBaseLib.Context;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Provides the workbench commands available for a host surface and context.
/// </summary>
public interface IWorkbenchCommandCatalog
{
    /// <summary>
    /// Gets executable commands placed on the specified surface and path.
    /// </summary>
    IReadOnlyList<IWorkbenchCommandContribution> GetCommands(
        WorkbenchCommandSurface surface,
        string path,
        IWorkbenchCommandContext context);

    /// <summary>
    /// Gets executable commands that apply to the specified popup target.
    /// </summary>
    IReadOnlyList<IWorkbenchPopupCommandContribution> GetPopupCommands(
        WorkbenchPopupTarget popupTarget,
        IWorkbenchCommandContext context);
}
