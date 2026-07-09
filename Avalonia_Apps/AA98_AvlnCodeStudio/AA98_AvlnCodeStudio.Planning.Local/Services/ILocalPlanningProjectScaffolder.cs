namespace AA98_AvlnCodeStudio.Planning.Local.Services;

/// <summary>
/// Creates local planning project structures for the markdown-backed driver.
/// </summary>
public interface ILocalPlanningProjectScaffolder
{
    LocalPlanningProjectScaffoldResult Create(string planningProjectRootPath);
}
