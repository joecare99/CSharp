using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents a provider-neutral request to persist one or more planning documents.
/// </summary>
public sealed class PlanningWriteRequest
{
    public string RepositoryRootPath { get; set; } = string.Empty;

    public string PlanningRootPath { get; set; } = "DevOps";

    public IList<PlanningItem> Items { get; } = new List<PlanningItem>();
}
