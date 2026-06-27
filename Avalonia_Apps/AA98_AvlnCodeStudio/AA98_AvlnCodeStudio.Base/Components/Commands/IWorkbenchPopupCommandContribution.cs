namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Defines a workbench command contribution that can participate in popup-menu routing.
/// </summary>
public interface IWorkbenchPopupCommandContribution : IWorkbenchCommandContribution
{
    /// <summary>
    /// Determines whether the command applies to the current popup target.
    /// </summary>
    /// <param name="context">The current workbench context.</param>
    /// <param name="popupTarget">The active popup target.</param>
    /// <returns><see langword="true"/> when the command should be considered for the popup target; otherwise <see langword="false"/>.</returns>
    bool AppliesTo(IWorkbenchCommandContext context, WorkbenchPopupTarget popupTarget);
}
