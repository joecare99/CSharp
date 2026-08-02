using AA98_AvlnCodeStudio.Planning.Core.Models;
using AppKomponentBaseLib.Commands;
using AppKomponentBaseLib.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Planning.Core.Services;

/// <summary>
/// Provides a provider-neutral planning-summary tool for local planning workflows.
/// </summary>
public sealed class PlanningSummaryToolContribution : IToolCommandContribution
{
    private readonly IPlanningProvider _planningProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanningSummaryToolContribution"/> class.
    /// </summary>
    /// <param name="planningProvider">The planning provider used to read local planning items.</param>
    public PlanningSummaryToolContribution(IPlanningProvider planningProvider)
    {
        _planningProvider = planningProvider;
    }

    /// <inheritdoc />
    public ToolCommandDescriptor Descriptor { get; } = new(
        "Planning.Summarize",
        "Summarize planning",
        new[]
        {
            new ToolCommandParameterDescriptor("repositoryRootPath", "Repository root path", "Path to the repository root that contains the planning folder.", true, valueKind: "path"),
            new ToolCommandParameterDescriptor("planningRootPath", "Planning root path", "Relative planning folder name.", false, defaultValue: "DevOps", valueKind: "path")
        },
        new[]
        {
            new ToolCommandResultDescriptor("summary", "Planning summary", "A grouped planning summary grouped by planning item kind.", "text")
        },
        new[] { "Planning" },
        "Summarizes the local planning hierarchy for the selected repository.",
        requiresConsent: true,
        safetyLevel: ToolCommandSafetyLevel.Low);

    /// <inheritdoc />
    public bool CanExecute(IAppContext context)
    {
        return context.Targets.Any(static target => string.Equals(target.TargetType, "Planning", StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public async Task<object?> ExecuteAsync(IAppContext context, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken = default)
    {
        if (!CanExecute(context))
        {
            return null;
        }

        string? repositoryRootPath = ResolveString(parameters, "repositoryRootPath");
        if (string.IsNullOrWhiteSpace(repositoryRootPath))
        {
            return "Missing repositoryRootPath parameter.";
        }

        string planningRootPath = ResolveString(parameters, "planningRootPath") ?? "DevOps";

        try
        {
            PlanningReadResult readResult = await _planningProvider.ReadAsync(
                new PlanningReadRequest
                {
                    RepositoryRootPath = repositoryRootPath,
                    PlanningRootPath = planningRootPath
                },
                cancellationToken).ConfigureAwait(false);

            IEnumerable<string> summaries = readResult.Items
                .Where(static item => item.Kind != PlanningItemKind.Unknown)
                .GroupBy(static item => item.Kind)
                .Select(static group => $"{group.Key}: {group.Count()}");

            string summary = summaries.Any()
                ? string.Join(Environment.NewLine, summaries)
                : "No planning items found.";

            return summary;
        }
        catch (Exception ex)
        {
            return $"Unable to summarize planning: {ex.Message}";
        }
    }

    private static string? ResolveString(IReadOnlyDictionary<string, object?> parameters, string key)
    {
        if (parameters.TryGetValue(key, out object? value) && value is not null)
        {
            return value.ToString();
        }

        return null;
    }
}
