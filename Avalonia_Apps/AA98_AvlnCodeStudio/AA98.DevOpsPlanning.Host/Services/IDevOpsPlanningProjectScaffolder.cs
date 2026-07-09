namespace AA98.DevOpsPlanning.Host.Services;

/// <summary>
/// Creates and verifies DevOps planning project structures.
/// </summary>
public interface IDevOpsPlanningProjectScaffolder
{
    /// <summary>
    /// Creates or updates a planning project structure under the provided path.
    /// </summary>
    /// <param name="planningProjectRootPath">The planning project root path.</param>
    /// <returns>The scaffolding result.</returns>
    DevOpsPlanningProjectScaffoldResult Create(string planningProjectRootPath);
}
