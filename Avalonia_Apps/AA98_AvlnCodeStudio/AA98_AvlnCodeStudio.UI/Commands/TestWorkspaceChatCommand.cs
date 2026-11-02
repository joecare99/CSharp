using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Workspaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Commands;

/// <summary>
/// Runs tests for one or all projects contained in the explicitly supplied C# workspace.
/// </summary>
public sealed class TestWorkspaceChatCommand(
    CSharpWorkspaceLoader workspaceLoader,
    ITestExecutionService testExecutionService) : IComponentMcpCommand
{
    public ComponentMcpCommandDescriptor Descriptor { get; } = new(
        "workspace.test",
        "Run C# workspace tests",
        "Runs tests for one project in the supplied C# workspace, or all projects when projectPath is omitted. Requires explicit approval.",
        """{"type":"object","properties":{"workspacePath":{"type":"string"},"projectPath":{"type":"string"},"targetFramework":{"type":"string"}},"required":["workspacePath"],"additionalProperties":false}""",
        isReadOnly: false);

    public async Task<string> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken)
    {
        string workspacePath = arguments.GetProperty("workspacePath").GetString()
            ?? throw new ArgumentException("The workspace path cannot be null.", nameof(arguments));
        CSharpWorkspace workspace = workspaceLoader.Load(workspacePath);
        CSharpWorkspaceProject[] projects;
        if (arguments.TryGetProperty("projectPath", out JsonElement projectPathElement))
        {
            string projectPath = projectPathElement.GetString()
                ?? throw new ArgumentException("The project path cannot be null.", nameof(arguments));
            string normalizedProjectPath = Path.GetFullPath(projectPath);
            CSharpWorkspaceProject? project = workspace.Projects.FirstOrDefault(candidate =>
                string.Equals(
                    Path.GetFullPath(candidate.ProjectPath),
                    normalizedProjectPath,
                    OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
            if (project is null)
            {
                throw new ArgumentException("The requested project is not part of the supplied workspace.", nameof(arguments));
            }

            projects = [project];
        }
        else
        {
            projects = workspace.Projects.ToArray();
        }

        List<ProjectTestResponse> results = new(projects.Length);
        foreach (CSharpWorkspaceProject project in projects)
        {
            cancellationToken.ThrowIfCancellationRequested();
            TestRunSummary result = await testExecutionService.RunTestsAsync(
                new TestRunRequest
                {
                    WorkspaceRootPath = workspace.RootPath,
                    ProjectPath = project.ProjectPath,
                    TargetFramework = GetOptionalString(arguments, "targetFramework"),
                },
                cancellationToken).ConfigureAwait(false);
            results.Add(new ProjectTestResponse(
                project.ProjectPath,
                result.Outcome.ToString(),
                result.TotalCount,
                result.PassedCount,
                result.FailedCount,
                result.SkippedCount));
        }

        return results.Count == 1
            ? JsonSerializer.Serialize(results[0])
            : JsonSerializer.Serialize(new
            {
                WorkspacePath = workspace.EntryPath,
                Projects = results,
                Outcome = GetAggregateOutcome(results),
                TotalCount = results.Sum(static result => result.TotalCount),
                PassedCount = results.Sum(static result => result.PassedCount),
                FailedCount = results.Sum(static result => result.FailedCount),
                SkippedCount = results.Sum(static result => result.SkippedCount),
            });
    }

    private static string? GetOptionalString(JsonElement arguments, string propertyName)
    {
        return arguments.TryGetProperty(propertyName, out JsonElement value)
            ? value.GetString()
            : null;
    }

    private static string GetAggregateOutcome(IReadOnlyCollection<ProjectTestResponse> results)
    {
        int failed = results.Sum(static result => result.FailedCount);
        int passed = results.Sum(static result => result.PassedCount);
        int total = results.Sum(static result => result.TotalCount);
        int skipped = results.Sum(static result => result.SkippedCount);
        if (failed > 0)
        {
            return passed > 0 ? "Partial" : "Failed";
        }

        return total == 0 ? "Unknown" : skipped > 0 ? "Partial" : "Passed";
    }

    private sealed record ProjectTestResponse(
        string ProjectPath,
        string Outcome,
        int TotalCount,
        int PassedCount,
        int FailedCount,
        int SkippedCount);
}
