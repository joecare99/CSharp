using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.Build.Locator;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Code.Navigation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkbenchDiagnostic = AppKomponentBaseLib.Diagnostics.Diagnostic;

namespace AA98_AvlnCodeStudio.UI.Services;

/// <summary>
/// Produces Roslyn syntax diagnostics for a C# source document.
/// </summary>
public sealed class CSharpSyntaxDiagnosticService : System.IDisposable
{
    private readonly Dictionary<string, ProjectContext> _projectContexts = new(System.StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _workspaceGate = new(1, 1);
    private static readonly object MsBuildLocatorLock = new();
    private static bool _msBuildLocatorRegistered;

    public IReadOnlyList<WorkbenchDiagnostic> Analyze(
        string source,
        string? filePath,
        CancellationToken cancellationToken = default)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            path: filePath ?? string.Empty,
            cancellationToken: cancellationToken);

        return syntaxTree.GetDiagnostics(cancellationToken)
            .Where(diagnostic => diagnostic.Severity != Microsoft.CodeAnalysis.DiagnosticSeverity.Hidden)
            .Select(diagnostic => MapDiagnostic(diagnostic, filePath))
            .ToArray();
    }

    public async Task<IReadOnlyList<WorkbenchDiagnostic>> AnalyzeAsync(
        string source,
        string? filePath,
        CSharpWorkspace? workspace,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath) || workspace is null)
        {
            return Analyze(source, filePath, cancellationToken);
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        CSharpWorkspaceProject? projectDescriptor = workspace.Projects
            .Select(project => new
            {
                Project = project,
                Directory = Path.GetDirectoryName(project.ProjectPath)!,
            })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .Select(candidate => candidate.Project)
            .FirstOrDefault();
        if (projectDescriptor is null)
        {
            return Analyze(source, filePath, cancellationToken);
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectDescriptor.ProjectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project = context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project?.Documents.FirstOrDefault(candidate =>
                string.Equals(candidate.FilePath, normalizedFilePath, StringComparison.OrdinalIgnoreCase));
            if (document is null)
            {
                return Analyze(source, filePath, cancellationToken);
            }

            Solution updatedSolution = context.Workspace.CurrentSolution.WithDocumentText(
                document.Id,
                SourceText.From(source));
            if (!context.Workspace.TryApplyChanges(updatedSolution))
            {
                throw new InvalidOperationException($"Could not update Roslyn document '{normalizedFilePath}'.");
            }

            Microsoft.CodeAnalysis.Project updatedProject = context.Workspace.CurrentSolution.GetProject(context.ProjectId)
                ?? throw new InvalidOperationException($"Roslyn project '{projectDescriptor.ProjectPath}' is no longer available.");
            Compilation compilation = await updatedProject.GetCompilationAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException(
                    $"Could not create a compilation for '{projectDescriptor.ProjectPath}' "
                    + $"(language '{updatedProject.Language}', supports compilation: {updatedProject.SupportsCompilation}, "
                    + $"documents: {updatedProject.Documents.Count()}). "
                    + string.Join(" ", context.Workspace.Diagnostics.Select(static diagnostic => diagnostic.Message)));
            return compilation.GetDiagnostics(cancellationToken)
                .Where(diagnostic => diagnostic.Severity != Microsoft.CodeAnalysis.DiagnosticSeverity.Hidden)
                .Where(diagnostic => diagnostic.Location.SourceTree is not null
                    && string.Equals(
                        diagnostic.Location.SourceTree.FilePath,
                        normalizedFilePath,
                        StringComparison.OrdinalIgnoreCase))
                .Select(diagnostic => MapDiagnostic(diagnostic, filePath))
                .ToArray();
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<IReadOnlyList<string>> GetCompletionsAsync(
        string source,
        string filePath,
        int position,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(workspace);
        if (position < 0 || position > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        CSharpWorkspaceProject? projectDescriptor = workspace.Projects
            .Select(project => new
            {
                Project = project,
                Directory = Path.GetDirectoryName(project.ProjectPath)!,
            })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .Select(candidate => candidate.Project)
            .FirstOrDefault();
        if (projectDescriptor is null)
        {
            return [];
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectDescriptor.ProjectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project = context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project?.Documents.FirstOrDefault(candidate =>
                string.Equals(candidate.FilePath, normalizedFilePath, StringComparison.OrdinalIgnoreCase));
            if (document is null)
            {
                return [];
            }

            Solution updatedSolution = context.Workspace.CurrentSolution.WithDocumentText(
                document.Id,
                SourceText.From(source));
            if (!context.Workspace.TryApplyChanges(updatedSolution))
            {
                throw new InvalidOperationException($"Could not update Roslyn document '{normalizedFilePath}'.");
            }

            Document updatedDocument = context.Workspace.CurrentSolution.GetDocument(document.Id)
                ?? throw new InvalidOperationException($"Roslyn document '{normalizedFilePath}' is no longer available.");
            CompletionService completionService = CompletionService.GetService(updatedDocument)
                ?? throw new InvalidOperationException("The C# completion service is unavailable.");
            CompletionList? completionList = await completionService.GetCompletionsAsync(
                updatedDocument,
                position,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return completionList?.Items
                .Select(static item => item.DisplayText)
                .Where(static item => !string.IsNullOrWhiteSpace(item))
                .Distinct(StringComparer.Ordinal)
                .ToArray() ?? [];
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<CodeLocation?> FindDefinitionAsync(
        string source,
        string filePath,
        int position,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(workspace);
        if (position < 0 || position > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        CSharpWorkspaceProject? projectDescriptor = workspace.Projects
            .Select(project => new
            {
                Project = project,
                Directory = Path.GetDirectoryName(project.ProjectPath)!,
            })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .Select(candidate => candidate.Project)
            .FirstOrDefault();
        if (projectDescriptor is null)
        {
            return null;
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectDescriptor.ProjectPath, cancellationToken)
                .ConfigureAwait(false);
            Document? document = context.Workspace.CurrentSolution
                .GetProject(context.ProjectId)?
                .Documents.FirstOrDefault(candidate =>
                    string.Equals(candidate.FilePath, normalizedFilePath, StringComparison.OrdinalIgnoreCase));
            if (document is null)
            {
                return null;
            }

            Solution updatedSolution = context.Workspace.CurrentSolution.WithDocumentText(
                document.Id,
                SourceText.From(source));
            if (!context.Workspace.TryApplyChanges(updatedSolution))
            {
                throw new InvalidOperationException($"Could not update Roslyn document '{normalizedFilePath}'.");
            }

            Document updatedDocument = context.Workspace.CurrentSolution.GetDocument(document.Id)
                ?? throw new InvalidOperationException($"Roslyn document '{normalizedFilePath}' is no longer available.");
            SemanticModel semanticModel = await updatedDocument.GetSemanticModelAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not create a semantic model for '{normalizedFilePath}'.");
            ISymbol? symbol = await SymbolFinder.FindSymbolAtPositionAsync(
                semanticModel,
                position,
                context.Workspace,
                cancellationToken).ConfigureAwait(false);
            if (symbol is null)
            {
                return null;
            }

            ISymbol? sourceDefinition = await SymbolFinder.FindSourceDefinitionAsync(
                symbol,
                context.Workspace.CurrentSolution,
                cancellationToken).ConfigureAwait(false);
            Location? definitionLocation = sourceDefinition?.Locations
                .FirstOrDefault(static location => location.IsInSource
                    && !string.IsNullOrWhiteSpace(location.SourceTree?.FilePath));
            if (definitionLocation is null)
            {
                return null;
            }

            FileLinePositionSpan lineSpan = definitionLocation.GetLineSpan();
            return new CodeLocation(
                lineSpan.Path,
                lineSpan.StartLinePosition.Line + 1,
                lineSpan.StartLinePosition.Character + 1);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    private async Task<ProjectContext> GetProjectContextAsync(string projectPath, CancellationToken cancellationToken)
    {
        string normalizedProjectPath = Path.GetFullPath(projectPath);
        if (_projectContexts.TryGetValue(normalizedProjectPath, out ProjectContext? existing))
        {
            return existing;
        }

        RegisterMsBuildLocator();
        Assembly csharpWorkspaceAssembly = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces");
        Assembly csharpFeaturesAssembly = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features");
        HostServices hostServices = MefHostServices.Create(
            MefHostServices.DefaultAssemblies
                .Append(csharpWorkspaceAssembly)
                .Append(csharpFeaturesAssembly));
        MSBuildWorkspace workspace = MSBuildWorkspace.Create(hostServices);
        try
        {
            Microsoft.CodeAnalysis.Project project = await workspace.OpenProjectAsync(normalizedProjectPath, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            ProjectContext context = new(workspace, project.Id);
            _projectContexts.Add(normalizedProjectPath, context);
            return context;
        }
        catch
        {
            workspace.Dispose();
            throw;
        }
    }

    private static void RegisterMsBuildLocator()
    {
        lock (MsBuildLocatorLock)
        {
            if (_msBuildLocatorRegistered || MSBuildLocator.IsRegistered)
            {
                _msBuildLocatorRegistered = true;
                return;
            }

            MSBuildLocator.RegisterDefaults();
            _msBuildLocatorRegistered = true;
        }
    }

    private static bool IsWithinDirectory(string filePath, string directoryPath)
    {
        string relativePath = Path.GetRelativePath(directoryPath, filePath);
        return !Path.IsPathRooted(relativePath)
            && relativePath != ".."
            && !relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal);
    }

    public void Dispose()
    {
        foreach (ProjectContext context in _projectContexts.Values)
        {
            context.Workspace.Dispose();
        }

        _projectContexts.Clear();
        _workspaceGate.Dispose();
    }

    private static WorkbenchDiagnostic MapDiagnostic(Microsoft.CodeAnalysis.Diagnostic source, string? filePath)
    {
        FileLinePositionSpan lineSpan = source.Location.GetLineSpan();
        return new WorkbenchDiagnostic
        {
            Code = source.Id,
            Message = source.GetMessage(),
            Severity = source.Severity switch
            {
                Microsoft.CodeAnalysis.DiagnosticSeverity.Error => AppKomponentBaseLib.Diagnostics.DiagnosticSeverity.Error,
                Microsoft.CodeAnalysis.DiagnosticSeverity.Warning => AppKomponentBaseLib.Diagnostics.DiagnosticSeverity.Warning,
                _ => AppKomponentBaseLib.Diagnostics.DiagnosticSeverity.Info,
            },
            SourcePath = string.IsNullOrWhiteSpace(filePath) ? null : filePath,
            LineNumber = source.Location.IsInSource ? lineSpan.StartLinePosition.Line + 1 : null,
            ColumnNumber = source.Location.IsInSource ? lineSpan.StartLinePosition.Character + 1 : null,
        };
    }

    private sealed record ProjectContext(MSBuildWorkspace Workspace, ProjectId ProjectId);
}
