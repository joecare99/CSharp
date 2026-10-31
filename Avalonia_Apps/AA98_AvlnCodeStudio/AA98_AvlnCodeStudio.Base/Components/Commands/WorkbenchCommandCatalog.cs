using AppKomponentBaseLib.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Base.Components.Commands;

/// <summary>
/// Resolves registered command contributions for workbench surfaces.
/// </summary>
public sealed class WorkbenchCommandCatalog : IWorkbenchCommandCatalog
{
    private readonly IWorkbenchCommandContribution[] _contributions;

    /// <summary>
    /// Initializes a catalog from the contributions registered by the host.
    /// </summary>
    public WorkbenchCommandCatalog(IEnumerable<IWorkbenchCommandContribution> contributions)
    {
        ArgumentNullException.ThrowIfNull(contributions);

        _contributions = contributions.ToArray();
        if (_contributions.Any(static contribution => contribution is null))
        {
            throw new ArgumentException("Command contributions cannot contain null entries.", nameof(contributions));
        }

        string? duplicateCommandId = _contributions
            .GroupBy(static contribution => contribution.Descriptor.CommandId, StringComparer.Ordinal)
            .Where(static group => group.Count() > 1)
            .Select(static group => group.Key)
            .FirstOrDefault();
        if (duplicateCommandId is not null)
        {
            throw new ArgumentException($"The command identifier '{duplicateCommandId}' is registered more than once.", nameof(contributions));
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<IWorkbenchCommandContribution> GetCommands(
        WorkbenchCommandSurface surface,
        string path,
        IWorkbenchCommandContext context)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(context);

        string normalizedPath = NormalizePath(path);
        return _contributions
            .Where(contribution => contribution.CanExecute(context))
            .SelectMany(contribution => contribution.Descriptor.Placements
                .Where(placement => placement.Surface == surface
                    && string.Equals(placement.Path, normalizedPath, StringComparison.Ordinal))
                .Select(placement => (Contribution: contribution, Placement: placement)))
            .OrderBy(static entry => entry.Placement.Order)
            .ThenBy(static entry => entry.Contribution.Descriptor.CommandId, StringComparer.Ordinal)
            .Select(static entry => entry.Contribution)
            .ToArray();
    }

    /// <inheritdoc/>
    public IReadOnlyList<IWorkbenchPopupCommandContribution> GetPopupCommands(
        WorkbenchPopupTarget popupTarget,
        IWorkbenchCommandContext context)
    {
        ArgumentNullException.ThrowIfNull(popupTarget);
        ArgumentNullException.ThrowIfNull(context);

        return _contributions
            .OfType<IWorkbenchPopupCommandContribution>()
            .Where(contribution => contribution.CanExecute(context)
                && contribution.AppliesTo(context, popupTarget))
            .Select(static contribution => contribution)
            .OrderBy(static contribution => contribution.Descriptor.Placements
                .Where(static placement => placement.Surface == WorkbenchCommandSurface.ContextMenu)
                .Select(static placement => placement.Order)
                .DefaultIfEmpty(0)
                .Min())
            .ThenBy(static contribution => contribution.Descriptor.CommandId, StringComparer.Ordinal)
            .ToArray();
    }

    private static string NormalizePath(string path)
    {
        return string.Join('/', path.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
}
