using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Build.Locator;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using Code.Navigation;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using System.Xml;
using System.Xml.Linq;
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
    private readonly Dictionary<string, string[]> _projectTargetFrameworks = new(System.StringComparer.OrdinalIgnoreCase);
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
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, string>? openDocumentTexts = null,
        string? targetFramework = null)
    {
        if (string.IsNullOrWhiteSpace(filePath) || workspace is null)
        {
            return Analyze(source, filePath, cancellationToken);
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        var candidates = workspace.Projects
            .Select(project => new
            {
                Project = project,
                Directory = Path.GetDirectoryName(project.ProjectPath)!,
            })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .ToArray();
        if (candidates.Length == 0)
        {
            return Analyze(source, filePath, cancellationToken);
        }
        if (candidates.Length > 1 && candidates[0].Directory.Length == candidates[1].Directory.Length)
        {
            throw new InvalidOperationException(
                $"The active file '{normalizedFilePath}' belongs to multiple workspace projects; select a project explicitly.");
        }
        CSharpWorkspaceProject projectDescriptor = candidates[0].Project;

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectDescriptor.ProjectPath, cancellationToken, targetFramework)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project = context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project is null
                ? null
                : CreateDocumentSnapshot(project, normalizedFilePath, source, openDocumentTexts);
            if (document is null)
            {
                return Analyze(source, filePath, cancellationToken);
            }

            Microsoft.CodeAnalysis.Project updatedProject = document.Project;
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
        CancellationToken cancellationToken = default,
        string? targetFramework = null)
    {
        return (await GetCompletionItemsAsync(
            source, filePath, position, workspace, cancellationToken, targetFramework: targetFramework).ConfigureAwait(false))
            .Select(static item => item.DisplayText)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    public async Task<IReadOnlyList<CSharpCompletionSuggestion>> GetCompletionItemsAsync(
        string source,
        string filePath,
        int position,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, string>? openDocumentTexts = null,
        string? targetFramework = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(workspace);
        if (position < 0 || position > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        var candidates = workspace.Projects
            .Select(project => new
            {
                Project = project,
                Directory = Path.GetDirectoryName(project.ProjectPath)!,
            })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .ToArray();
        if (candidates.Length == 0
            || candidates.Length > 1 && candidates[0].Directory.Length == candidates[1].Directory.Length)
        {
            return [];
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(candidates[0].Project.ProjectPath, cancellationToken, targetFramework)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project = context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project is null
                ? null
                : CreateDocumentSnapshot(project, normalizedFilePath, source, openDocumentTexts);
            if (document is null)
            {
                return [];
            }

            Document updatedDocument = document;
            CompletionService completionService = CompletionService.GetService(updatedDocument)
                ?? throw new InvalidOperationException("The C# completion service is unavailable.");
            CompletionList? completionList = await completionService.GetCompletionsAsync(
                updatedDocument,
                position,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            if (completionList is null)
            {
                return [];
            }
            int filterStart = completionList.Span.Start;
            string filter = filterStart <= position ? source[filterStart..position] : string.Empty;
            var items = completionService.FilterItems(updatedDocument, completionList.ItemsList.ToImmutableArray(), filter);
            return items
                .Where(static item => !string.IsNullOrWhiteSpace(item.DisplayText))
                .Select(item => new CSharpCompletionSuggestion(updatedDocument, completionService, item))
                .ToArray();
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<CSharpCompletionEdit> GetCompletionEditAsync(
        CSharpCompletionSuggestion suggestion,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(suggestion);
        CompletionChange change = await suggestion.Service.GetChangeAsync(
            suggestion.Document, suggestion.Item, cancellationToken: cancellationToken).ConfigureAwait(false);
        SourceText source = await suggestion.Document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        SourceText updated = source.WithChanges(change.TextChange);
        int caret = change.NewPosition ?? change.TextChange.Span.Start + (change.TextChange.NewText?.Length ?? 0);
        if (caret < 0 || caret > updated.Length)
        {
            throw new InvalidOperationException("The completion provider returned an invalid caret position.");
        }
        return new CSharpCompletionEdit(
            change.TextChange.Span.Start, change.TextChange.Span.Length, change.TextChange.NewText ?? string.Empty, caret);
    }

    public async Task<CodeLocation?> FindDefinitionAsync(
        string source,
        string filePath,
        int position,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, string>? openDocumentTexts = null,
        string? targetFramework = null)
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
            ProjectContext context = await GetProjectContextAsync(projectDescriptor.ProjectPath, cancellationToken, targetFramework)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project =
                context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project is null
                ? null
                : CreateDocumentSnapshot(project, normalizedFilePath, source, openDocumentTexts);
            if (document is null)
            {
                return null;
            }

            Document updatedDocument = document;
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
                updatedDocument.Project.Solution,
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

    public async Task<string?> GetQuickInfoAsync(
        string source,
        string filePath,
        int position,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, string>? openDocumentTexts = null,
        string? targetFramework = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(workspace);
        if (position < 0 || position > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        var candidates = workspace.Projects
            .Select(project => new { Project = project, Directory = Path.GetDirectoryName(project.ProjectPath)! })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .ToArray();
        if (candidates.Length == 0
            || candidates.Length > 1 && candidates[0].Directory.Length == candidates[1].Directory.Length)
        {
            return null;
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(candidates[0].Project.ProjectPath, cancellationToken, targetFramework)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project =
                context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project is null
                ? null
                : CreateDocumentSnapshot(project, normalizedFilePath, source, openDocumentTexts);
            if (document is null)
            {
                return null;
            }

            Document updatedDocument = document;
            QuickInfoService? service = QuickInfoService.GetService(updatedDocument);
            QuickInfoItem? item = service is null
                ? null
                : await service.GetQuickInfoAsync(updatedDocument, position, cancellationToken).ConfigureAwait(false);
            string[] sections = item?.Sections
                .Select(section => string.Concat(section.TaggedParts.Select(part => part.Text)))
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToArray() ?? [];
            return sections.Length == 0 ? null : string.Join(Environment.NewLine, sections);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<CSharpSignatureHelp?> GetSignatureHelpAsync(
        string source,
        string filePath,
        int position,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default,
        IReadOnlyDictionary<string, string>? openDocumentTexts = null,
        string? targetFramework = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(workspace);
        if (position < 0 || position > source.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(position));
        }

        string normalizedFilePath = Path.GetFullPath(filePath);
        var candidates = workspace.Projects
            .Select(project => new { Project = project, Directory = Path.GetDirectoryName(project.ProjectPath)! })
            .Where(candidate => IsWithinDirectory(normalizedFilePath, candidate.Directory))
            .OrderByDescending(candidate => candidate.Directory.Length)
            .ToArray();
        if (candidates.Length == 0
            || candidates.Length > 1 && candidates[0].Directory.Length == candidates[1].Directory.Length)
        {
            return null;
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(candidates[0].Project.ProjectPath, cancellationToken, targetFramework)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project =
                context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            Document? document = project is null
                ? null
                : CreateDocumentSnapshot(project, normalizedFilePath, source, openDocumentTexts);
            if (document is null)
            {
                return null;
            }

            SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return null;
            }

            ArgumentListSyntax? argumentList = root.DescendantNodes()
                .OfType<ArgumentListSyntax>()
                .Where(node => node.OpenParenToken.SpanStart <= position
                    && (node.CloseParenToken.IsMissing || position <= node.CloseParenToken.SpanStart))
                .OrderBy(node => node.Span.Length)
                .FirstOrDefault();
            if (argumentList is null)
            {
                return null;
            }

            SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken)
                .ConfigureAwait(false);
            if (semanticModel is null)
            {
                return null;
            }
            InvocationExpressionSyntax? invocation = argumentList.Parent as InvocationExpressionSyntax;
            ObjectCreationExpressionSyntax? creation = argumentList.Parent as ObjectCreationExpressionSyntax;
            ImplicitObjectCreationExpressionSyntax? implicitCreation =
                argumentList.Parent as ImplicitObjectCreationExpressionSyntax;
            ConstructorInitializerSyntax? constructorInitializer = argumentList.Parent as ConstructorInitializerSyntax;
            IMethodSymbol? selectedMethod = invocation is not null
                ? semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol
                : creation is not null
                    ? semanticModel.GetSymbolInfo(creation, cancellationToken).Symbol as IMethodSymbol
                    : implicitCreation is not null
                        ? semanticModel.GetSymbolInfo(implicitCreation, cancellationToken).Symbol as IMethodSymbol
                        : constructorInitializer is not null
                            ? semanticModel.GetSymbolInfo(constructorInitializer, cancellationToken).Symbol as IMethodSymbol
                            : null;
            if (selectedMethod is null)
            {
                return null;
            }

            var methods = selectedMethod.ContainingType.GetMembers(selectedMethod.Name)
                .OfType<IMethodSymbol>()
                .Where(method => method.MethodKind == selectedMethod.MethodKind)
                .Concat(new[] { selectedMethod })
                .Distinct<IMethodSymbol>(SymbolEqualityComparer.Default)
                .ToArray();
            if (methods.Length == 0)
            {
                return null;
            }

            int selectedIndex = Array.FindIndex(methods, method =>
                SymbolEqualityComparer.Default.Equals(method, selectedMethod));
            int activeArgumentIndex = argumentList.Arguments.GetSeparators()
                .Count(separator => separator.SpanStart < position);
            ArgumentSyntax? activeArgument = argumentList.Arguments
                .FirstOrDefault(argument => argument.SpanStart <= position && position <= argument.Span.End);
            int activeParameterIndex = activeArgument?.NameColon is { } nameColon
                ? Array.FindIndex(selectedMethod.Parameters.ToArray(),
                    parameter => string.Equals(parameter.Name, nameColon.Name.Identifier.ValueText, StringComparison.Ordinal))
                : Math.Clamp(activeArgumentIndex, 0, Math.Max(0, selectedMethod.Parameters.Length - 1));
            string? activeParameterName = activeParameterIndex >= 0
                && activeParameterIndex < selectedMethod.Parameters.Length
                    ? selectedMethod.Parameters[activeParameterIndex].Name
                    : null;
            return new CSharpSignatureHelp(
                methods.Select(FormatSignature).ToArray(),
                selectedIndex,
                activeParameterIndex,
                activeParameterName);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    private static string FormatSignature(IMethodSymbol method)
    {
        string prefix = method.MethodKind == MethodKind.Constructor
            ? method.ContainingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)
            : $"{method.ContainingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}.{method.Name}";
        string parameters = string.Join(", ", method.Parameters.Select(parameter =>
        {
            string modifier = parameter.RefKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => string.Empty,
            };
            string optional = parameter.IsOptional && parameter.HasExplicitDefaultValue
                ? $" = {Convert.ToString(parameter.ExplicitDefaultValue, System.Globalization.CultureInfo.InvariantCulture) ?? "null"}"
                : string.Empty;
            return $"{modifier}{parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {parameter.Name}{optional}";
        }));
        return $"{prefix}({parameters})";
    }

    public async Task<IReadOnlyList<TestMethodCandidate>> FindTestMethodCandidatesAsync(
        string projectPath,
        string configuration,
        string targetFramework,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetFramework);
        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            RegisterMsBuildLocator();
            HostServices hostServices = CreateHostServices();
            using MSBuildWorkspace workspace = MSBuildWorkspace.Create(new Dictionary<string, string>
            {
                ["Configuration"] = configuration,
                ["TargetFramework"] = targetFramework,
            }, hostServices);
            Microsoft.CodeAnalysis.Project project = await workspace.OpenProjectAsync(
                Path.GetFullPath(projectPath), cancellationToken: cancellationToken).ConfigureAwait(false);
            WorkspaceDiagnostic? failure = workspace.Diagnostics.FirstOrDefault(
                diagnostic => diagnostic.Kind == WorkspaceDiagnosticKind.Failure);
            if (failure is not null)
            {
                throw new InvalidOperationException($"Project loading failed: {failure.Message}");
            }
            Compilation compilation = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException("The selected project could not be compiled for test discovery.");
            List<TestMethodCandidate> candidates = [];
            foreach (Document document in project.Documents)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (string.IsNullOrWhiteSpace(document.FilePath))
                {
                    continue;
                }

                SyntaxTree? tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
                if (tree is null || !compilation.ContainsSyntaxTree(tree))
                {
                    continue;
                }

                SemanticModel model = compilation.GetSemanticModel(tree);
                SyntaxNode root = await tree.GetRootAsync(cancellationToken).ConfigureAwait(false);
                foreach (MethodDeclarationSyntax declaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    if (model.GetDeclaredSymbol(declaration, cancellationToken) is not IMethodSymbol method
                        || method.DeclaredAccessibility != Accessibility.Public
                        || method.IsAbstract || method.IsStatic || method.IsGenericMethod
                        || method.ContainingType.IsAbstract || method.ContainingType.IsGenericType
                        || method.ContainingType.ContainingType is not null
                        || !method.GetAttributes().Any(IsSupportedTestAttribute))
                    {
                        continue;
                    }

                    string name = method.ContainingType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                        + "." + method.Name;
                    FileLinePositionSpan span = declaration.Identifier.GetLocation().GetLineSpan();
                    candidates.Add(new TestMethodCandidate(name, document.FilePath!, span.StartLinePosition.Line + 1));
                }
            }

            return candidates.OrderBy(candidate => candidate.FullyQualifiedName, StringComparer.Ordinal)
                .ThenBy(candidate => candidate.SourcePath, StringComparer.Ordinal)
                .ThenBy(candidate => candidate.LineNumber)
                .ToArray();
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    private static bool IsSupportedTestAttribute(AttributeData attribute)
    {
        for (INamedTypeSymbol? type = attribute.AttributeClass; type is not null; type = type.BaseType)
        {
            if (type.TypeKind != TypeKind.Error
                && (type.ToDisplayString() is "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute"
                or "Xunit.FactAttribute"
                or "NUnit.Framework.TestAttribute"
                or "NUnit.Framework.TestCaseAttribute"
                or "NUnit.Framework.TestCaseSourceAttribute"))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<AxamlMvvmSemanticResolution> AnalyzeAxamlMvvmAsync(
        string markup,
        string filePath,
        CSharpWorkspace workspace,
        CancellationToken cancellationToken = default,
        bool allowExistingRuntimeDataContext = false)
    {
        ArgumentNullException.ThrowIfNull(markup);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(workspace);

        string normalizedFilePath = Path.GetFullPath(filePath);
        CSharpWorkspaceProject[] candidates = workspace.Projects
            .Where(project => IsWithinDirectory(
                normalizedFilePath,
                Path.GetDirectoryName(project.ProjectPath)!))
            .OrderByDescending(project => Path.GetDirectoryName(project.ProjectPath)!.Length)
            .ToArray();
        if (candidates.Length == 0)
        {
            return AxamlMvvmSemanticResolution.Failure(
                null,
                "The AXAML file is not located under a project in the current workspace.");
        }

        int longestProjectDirectory = Path.GetDirectoryName(candidates[0].ProjectPath)!.Length;
        CSharpWorkspaceProject[] bestCandidates = candidates
            .Where(project => Path.GetDirectoryName(project.ProjectPath)!.Length == longestProjectDirectory)
            .ToArray();
        if (bestCandidates.Length != 1)
        {
            return AxamlMvvmSemanticResolution.Failure(
                null,
                "More than one C# project could own this AXAML file. Select a project explicitly before generating MVVM code.");
        }

        string projectPath = bestCandidates[0].ProjectPath;
        XDocument document;
        try
        {
            if (markup.Length > 1_000_000)
            {
                return AxamlMvvmSemanticResolution.Failure(
                    projectPath,
                    "AXAML markup exceeds the semantic analyzer size limit.");
            }

            using StringReader stringReader = new(markup);
            using XmlReader xmlReader = XmlReader.Create(
                stringReader,
                new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    MaxCharactersInDocument = 1_000_000,
                });
            document = XDocument.Load(xmlReader, LoadOptions.SetLineInfo);
        }
        catch (XmlException exception)
        {
            string location = exception.LineNumber > 0
                ? $" at line {exception.LineNumber}, column {exception.LinePosition}"
                : string.Empty;
            return AxamlMvvmSemanticResolution.Failure(
                projectPath,
                $"AXAML could not be parsed{location}: {exception.Message}");
        }

        XNamespace xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        XElement? root = document.Root;
        string? className = (string?)root?.Attribute(xamlNamespace + "Class");
        if (root is null || string.IsNullOrWhiteSpace(className))
        {
            return AxamlMvvmSemanticResolution.Failure(
                projectPath,
                WithAxamlLocation(
                    "The AXAML root must declare x:Class so its C# view type can be resolved.",
                    root));
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project? project =
                context.Workspace.CurrentSolution.GetProject(context.ProjectId);
            if (project is null || !project.SupportsCompilation)
            {
                return AxamlMvvmSemanticResolution.Failure(
                    projectPath,
                    "The selected project does not provide a C# compilation for semantic view-model analysis.");
            }

            Compilation compilation = await project.GetCompilationAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not create a compilation for '{projectPath}'.");
            INamedTypeSymbol? viewType = compilation.GetTypeByMetadataName(className);
            if (viewType is null || !viewType.Locations.Any(static location => location.IsInSource))
            {
                return AxamlMvvmSemanticResolution.Failure(
                    projectPath,
                    WithAxamlLocation(
                        $"The C# view type '{className}' could not be uniquely resolved in the selected project.",
                        root.Attribute(xamlNamespace + "Class")));
            }

            if (viewType.TypeKind != TypeKind.Class)
            {
                return AxamlMvvmSemanticResolution.Failure(
                    projectPath,
                    WithAxamlLocation(
                        $"The x:Class type '{className}' is not a class.",
                        root.Attribute(xamlNamespace + "Class")));
            }

            if (!viewType.DeclaringSyntaxReferences
                    .Select(reference => reference.GetSyntax(cancellationToken))
                    .OfType<ClassDeclarationSyntax>()
                    .Any(declaration => declaration.Modifiers.Any(SyntaxKind.PartialKeyword)))
            {
                return AxamlMvvmSemanticResolution.Failure(
                    projectPath,
                    WithAxamlLocation(
                        $"The view type '{className}' is not partial; Avalonia-generated view code cannot be safely associated with it.",
                        root.Attribute(xamlNamespace + "Class")));
            }

            List<string> diagnostics = [];
            string? dataTypeName = (string?)root.Attribute(xamlNamespace + "DataType");
            INamedTypeSymbol? declaredDataType = null;
            if (!string.IsNullOrWhiteSpace(dataTypeName))
            {
                declaredDataType = ResolveAxamlType(dataTypeName, root, compilation);
                if (declaredDataType is null)
                {
                    diagnostics.Add(WithAxamlLocation(
                        $"The x:DataType '{dataTypeName}' could not be resolved unambiguously in project '{Path.GetFileName(projectPath)}'.",
                        root.Attribute(xamlNamespace + "DataType")));
                }
            }

            bool markupDataContext = root.Attribute("DataContext") is not null
                || root.Elements().Any(element => element.Name.LocalName == "DataContext");
            List<INamedTypeSymbol> runtimeDataContexts = await FindRuntimeDataContextTypesAsync(
                project,
                compilation,
                viewType,
                cancellationToken).ConfigureAwait(false);
            bool runtimeDataContext = markupDataContext || runtimeDataContexts.Count > 0;
            if (markupDataContext)
            {
                XObject? dataContextNode = (XObject?)root.Attribute("DataContext")
                    ?? root.Elements().FirstOrDefault(element => element.Name.LocalName == "DataContext");
                diagnostics.Add(WithAxamlLocation(
                    "The AXAML root already defines DataContext; automatic runtime wiring is unsafe.",
                    dataContextNode));
            }

            if (runtimeDataContexts.Count > 1)
            {
                diagnostics.Add("The project assigns multiple DataContext types to this view; choose the runtime type explicitly.");
            }
            else if (runtimeDataContexts.Count == 1)
            {
                declaredDataType ??= runtimeDataContexts[0];
                if (!allowExistingRuntimeDataContext)
                {
                    diagnostics.Add(
                        $"The view already receives runtime DataContext '{runtimeDataContexts[0].ToDisplayString()}'; it will not be replaced.");
                }

                if (declaredDataType is not null
                    && !SymbolEqualityComparer.Default.Equals(declaredDataType, runtimeDataContexts[0]))
                {
                    diagnostics.Add(WithAxamlLocation(
                        $"The x:DataType '{declaredDataType.ToDisplayString()}' does not match the runtime DataContext type '{runtimeDataContexts[0].ToDisplayString()}'.",
                        root.Attribute(xamlNamespace + "DataType")));
                }
            }

            if (declaredDataType is not null && runtimeDataContexts.Count == 0 && !markupDataContext)
            {
                diagnostics.Add(
                    WithAxamlLocation(
                        $"The view declares x:DataType '{declaredDataType.ToDisplayString()}', but its runtime DataContext cannot be established from the AXAML or project source.",
                        root.Attribute(xamlNamespace + "DataType")));
            }

            AxamlMvvmSemanticMember[] viewModelMembers = declaredDataType is null
                ? []
                : GetViewModelMembers(declaredDataType);
            return new AxamlMvvmSemanticResolution(
                projectPath,
                className,
                declaredDataType?.ToDisplayString(),
                runtimeDataContext,
                viewModelMembers,
                diagnostics);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<bool> SupportsObservablePartialPropertiesAsync(
        string projectPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            RegisterMsBuildLocator();
            if (GetProjectTargetFrameworks(projectPath).Length > 1)
            {
                return false;
            }

            ProjectContext context = await GetProjectContextAsync(projectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project project = context.Workspace.CurrentSolution.GetProject(context.ProjectId)
                ?? throw new InvalidOperationException($"Could not load project '{projectPath}'.");
            return await SupportsObservablePartialPropertiesAsync(project, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    private static async Task<bool> SupportsObservablePartialPropertiesAsync(
        Microsoft.CodeAnalysis.Project project,
        CancellationToken cancellationToken)
    {
        // MSBuildWorkspace opens one target framework; do not infer compatibility for the others.
        if (string.IsNullOrWhiteSpace(project.FilePath))
        {
            return false;
        }

        try
        {
            using Microsoft.Build.Evaluation.ProjectCollection collection = new();
            Microsoft.Build.Evaluation.Project evaluated = collection.LoadProject(project.FilePath);
            if (evaluated.GetPropertyValue("TargetFrameworks")
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length > 1)
            {
                return false;
            }
        }
        catch (Exception exception) when (exception is Microsoft.Build.Exceptions.InvalidProjectFileException
            or IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            return false;
        }

        Compilation baseline = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Could not compile project '{project.FilePath}'.");
        string className = "ObservablePropertyProbe" + Guid.NewGuid().ToString("N");
        string source = $$"""
            namespace AA98GeneratedCapabilityProbe;
            public partial class {{className}} : global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject
            {
                [global::CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
                public partial string Value { get; set; } = string.Empty;
            }
            """;
        Document document = project.AddDocument(className + ".cs", SourceText.From(source));
        Compilation probe = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Could not compile the capability probe for '{project.FilePath}'.");
        IPropertySymbol? property = probe.GetTypeByMetadataName("AA98GeneratedCapabilityProbe." + className)
            ?.GetMembers("Value").OfType<IPropertySymbol>().FirstOrDefault();
        if (property?.PartialImplementationPart is null)
        {
            return false;
        }

        Dictionary<string, int> baselineErrors = baseline.GetDiagnostics(cancellationToken)
            .Where(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .GroupBy(GetCompilerDiagnosticKey, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.Ordinal);
        foreach (Diagnostic diagnostic in probe.GetDiagnostics(cancellationToken)
                     .Where(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
        {
            string key = GetCompilerDiagnosticKey(diagnostic);
            if (!baselineErrors.TryGetValue(key, out int count) || count == 0)
            {
                return false;
            }

            baselineErrors[key] = count - 1;
        }

        return true;
    }

    public async Task<WorkspaceFileChangeRequest> PlanViewModelSourceAdditionAsync(
        string workspaceRoot,
        string projectPath,
        string viewModelTypeName,
        IReadOnlyList<AxamlMvvmMemberPlanEntry> additions,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(viewModelTypeName);
        ArgumentNullException.ThrowIfNull(additions);
        if (additions.Count == 0
            || additions.Any(static entry => entry.Action != AxamlMvvmMemberPlanAction.Create))
        {
            throw new InvalidDataException("Source additions require a non-empty plan containing only explicitly approved Create entries.");
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project project =
                context.Workspace.CurrentSolution.GetProject(context.ProjectId)
                ?? throw new InvalidOperationException($"Could not load project '{projectPath}'.");
            Compilation compilation = await project.GetCompilationAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not compile project '{projectPath}'.");
            INamedTypeSymbol viewModelType = compilation.GetTypeByMetadataName(viewModelTypeName)
                ?? throw new InvalidDataException($"ViewModel type '{viewModelTypeName}' could not be uniquely resolved.");
            bool usePartialProperties = additions.Any(static entry => !entry.Requirement.IsCommand)
                && await SupportsObservablePartialPropertiesAsync(project, cancellationToken).ConfigureAwait(false);
            INamedTypeSymbol? observableObjectType = compilation.GetTypeByMetadataName(
                "CommunityToolkit.Mvvm.ComponentModel.ObservableObject");
            if (observableObjectType is null
                || !InheritsFrom(viewModelType, observableObjectType))
            {
                throw new InvalidDataException(
                    $"ViewModel '{viewModelTypeName}' must derive from CommunityToolkit.Mvvm.ComponentModel.ObservableObject for safe generated properties and commands.");
            }

            Dictionary<string, Document> sourceDocuments = project.Documents
                .Where(static document => !string.IsNullOrWhiteSpace(document.FilePath))
                .ToDictionary(
                    document => Path.GetFullPath(document.FilePath!),
                    document => document,
                    WorkspacePathIdentity.Comparer);
            (ClassDeclarationSyntax Declaration, Document Document)[] declarations = viewModelType.DeclaringSyntaxReferences
                .Select(reference => reference.GetSyntax(cancellationToken))
                .OfType<ClassDeclarationSyntax>()
                .Where(declaration => declaration.SyntaxTree.FilePath is not null
                    && sourceDocuments.ContainsKey(Path.GetFullPath(declaration.SyntaxTree.FilePath)))
                .Select(declaration => (
                    Declaration: declaration,
                    Document: sourceDocuments[Path.GetFullPath(declaration.SyntaxTree.FilePath!)]))
                .ToArray();
            if (declarations.Length == 0
                || declarations.Any(static item =>
                    !item.Declaration.Modifiers.Any(SyntaxKind.PartialKeyword)))
            {
                throw new InvalidDataException(
                    $"ViewModel '{viewModelTypeName}' must have only partial class declarations before it can be extended.");
            }

            HashSet<string> existingNames = new(StringComparer.Ordinal);
            for (INamedTypeSymbol? currentType = viewModelType;
                 currentType is not null && currentType.SpecialType != SpecialType.System_Object;
                 currentType = currentType.BaseType)
            {
                foreach (ISymbol member in currentType.GetMembers())
                {
                    existingNames.Add(member.Name);

                    if (member is IFieldSymbol field
                        && HasAttribute(field, "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute")
                        && TryGetObservablePropertyName(field.Name, out string propertyName))
                    {
                        existingNames.Add(propertyName);
                        existingNames.Add($"On{propertyName}Changing");
                        existingNames.Add($"On{propertyName}Changed");
                    }
                    else if (member is IMethodSymbol method
                        && HasAttribute(method, "CommunityToolkit.Mvvm.Input.RelayCommandAttribute")
                        && TryGetRelayCommandPropertyName(method, out string commandPropertyName))
                    {
                        existingNames.Add(commandPropertyName);
                        existingNames.Add(ToBackingFieldName(commandPropertyName));
                        if (TryGetRelayCommandCancelPropertyName(method, commandPropertyName, out string cancelCommandName))
                        {
                            existingNames.Add(cancelCommandName);
                            existingNames.Add(ToBackingFieldName(cancelCommandName));
                        }
                    }
                }
            }
            List<string> membersToInsert = [];
            foreach (AxamlMvvmMemberPlanEntry entry in additions)
            {
                AxamlMvvmMemberRequirement requirement = entry.Requirement;
                if (requirement.IsCommand)
                {
                    if (!string.Equals(requirement.TypeName, "ICommand", StringComparison.Ordinal)
                        || !requirement.Name.EndsWith("Command", StringComparison.Ordinal))
                    {
                        throw new InvalidDataException(
                            $"Command member '{requirement.Name}' does not match the supported generated command shape.");
                    }

                    string methodName = requirement.Name[..^"Command".Length];
                    if (methodName.Length == 0 || !SyntaxFacts.IsValidIdentifier(methodName))
                    {
                        throw new InvalidDataException(
                            $"Generated command '{requirement.Name}' does not contain a valid method name.");
                    }

                    string commandFieldName = ToBackingFieldName(requirement.Name);
                    if (!existingNames.Add(methodName)
                        || !existingNames.Add(requirement.Name)
                        || !existingNames.Add(commandFieldName))
                    {
                        throw new InvalidDataException(
                            $"Generated command '{requirement.Name}' or its backing field '{commandFieldName}' conflicts with an existing ViewModel member.");
                    }

                    membersToInsert.Add(
                        $"[global::CommunityToolkit.Mvvm.Input.RelayCommand]{Environment.NewLine}"
                        + $"private void {methodName}(){Environment.NewLine}"
                        + "{" + Environment.NewLine
                        + $"    // TODO: Implement the {methodName} action.{Environment.NewLine}"
                        + "}");
                    continue;
                }

                if (!TryGetGeneratedPropertyType(requirement.TypeName, out string fieldType, out string initialValue))
                {
                    throw new InvalidDataException(
                        $"Property '{requirement.Name}' has unsupported generated type '{requirement.TypeName}'.");
                }

                if (!SyntaxFacts.IsValidIdentifier(requirement.Name)
                    || !existingNames.Add(requirement.Name))
                {
                    throw new InvalidDataException(
                        $"Generated property '{requirement.Name}' conflicts with an existing ViewModel member.");
                }

                string fieldName = "_" + char.ToLowerInvariant(requirement.Name[0]) + requirement.Name[1..];
                if (!usePartialProperties && !existingNames.Add(fieldName))
                {
                    throw new InvalidDataException(
                        $"Generated backing field '{fieldName}' conflicts with an existing ViewModel member.");
                }

                string changingHookName = $"On{requirement.Name}Changing";
                string changedHookName = $"On{requirement.Name}Changed";
                if (!existingNames.Add(changingHookName)
                    || !existingNames.Add(changedHookName))
                {
                    throw new InvalidDataException(
                        $"Generated property '{requirement.Name}' conflicts with an existing ObservableProperty change hook.");
                }

                membersToInsert.Add(
                    $"[global::CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]{Environment.NewLine}"
                    + (usePartialProperties
                        ? $"public partial {fieldType} {requirement.Name} {{ get; set; }} = {initialValue};"
                        : $"private {fieldType} {fieldName} = {initialValue};"));
            }

            (ClassDeclarationSyntax targetDeclaration, Document sourceDocument) = declarations
                .OrderByDescending(static item => item.Declaration.Members.Count)
                .First();

            SourceText sourceText = await sourceDocument.GetTextAsync(cancellationToken).ConfigureAwait(false);
            string source = sourceText.ToString();
            int closingBracePosition = targetDeclaration.CloseBraceToken.SpanStart;
            int lineStart = source.LastIndexOfAny(['\r', '\n'], Math.Max(0, closingBracePosition - 1)) + 1;
            if (source.AsSpan(lineStart, closingBracePosition - lineStart)
                .IndexOfAnyExcept(' ', '\t') >= 0)
            {
                throw new InvalidDataException(
                    $"ViewModel '{viewModelTypeName}' has an inline closing brace; source-preserving member insertion is unavailable.");
            }

            string indentation = source[lineStart..closingBracePosition];
            string newline = source.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
            string memberIndentation = GetLineIndentation(source, targetDeclaration.SpanStart) + "    ";
            string insertion = memberIndentation
                + string.Join(
                    newline + memberIndentation,
                    membersToInsert.Select(member =>
                        member.Replace(Environment.NewLine, newline + memberIndentation, StringComparison.Ordinal)))
                + newline;
            string proposedSource = source.Insert(lineStart, insertion);
            string normalizedWorkspaceRoot = Path.GetFullPath(workspaceRoot);
            if (!IsWithinDirectory(sourceDocument.FilePath!, normalizedWorkspaceRoot))
            {
                throw new InvalidDataException(
                    $"The ViewModel source file '{sourceDocument.FilePath}' is outside workspace '{normalizedWorkspaceRoot}'.");
            }

            return WorkspaceFileChangeRequest.ReplaceExisting(
                Path.GetRelativePath(normalizedWorkspaceRoot, sourceDocument.FilePath!),
                source,
                proposedSource);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task RefreshProjectContextAsync(
        string projectPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        string normalizedProjectPath = Path.GetFullPath(projectPath);
        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            string contextPrefix = normalizedProjectPath + "\0";
            foreach (string key in _projectContexts.Keys
                         .Where(key => key.StartsWith(contextPrefix, StringComparison.OrdinalIgnoreCase))
                         .ToArray())
            {
                _projectContexts.Remove(key, out ProjectContext? context);
                context?.Workspace.Dispose();
            }
            _projectTargetFrameworks.Remove(normalizedProjectPath);
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<IReadOnlyList<string>> GetNewCompilerErrorsAsync(
        string projectPath,
        WorkspaceFileChangeRequest request,
        CancellationToken cancellationToken = default,
        string? workspaceRoot = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentNullException.ThrowIfNull(request);
        if (request.Target != WorkspaceFileChangeTarget.MustExist)
        {
            throw new InvalidDataException("Compiler error comparison currently requires a snapshot-checked source replacement.");
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project project =
                context.Workspace.CurrentSolution.GetProject(context.ProjectId)
                ?? throw new InvalidOperationException($"Could not load project '{projectPath}'.");
            string targetPath = Path.GetFullPath(Path.Combine(
                workspaceRoot ?? Path.GetDirectoryName(projectPath)!,
                request.RelativePath));
            Document? document = project.Documents.FirstOrDefault(candidate =>
                candidate.FilePath is not null
                && WorkspacePathIdentity.AreEqual(candidate.FilePath, targetPath));
            if (document is null)
            {
                throw new InvalidDataException(
                    $"The proposed C# file '{request.RelativePath}' is not a source document in project '{projectPath}'.");
            }

            SourceText existingText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            if (!string.Equals(existingText.ToString(), request.ExpectedContent, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    $"The Roslyn snapshot for '{request.RelativePath}' is stale; reanalyze the project before proposing edits.");
            }

            Compilation baselineCompilation = await project.GetCompilationAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not compile project '{projectPath}'.");
            Diagnostic[] baselineErrors = baselineCompilation.GetDiagnostics(cancellationToken)
                .Where(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                .ToArray();

            Solution updatedSolution = context.Workspace.CurrentSolution.WithDocumentText(
                document.Id,
                SourceText.From(request.ProposedContent));
            Microsoft.CodeAnalysis.Project updatedProject = updatedSolution.GetProject(context.ProjectId)
                ?? throw new InvalidOperationException($"Could not create an updated compilation for '{projectPath}'.");
            Compilation updatedCompilation = await updatedProject.GetCompilationAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not compile the proposed changes for '{projectPath}'.");
            Dictionary<string, int> remainingErrors = baselineErrors
                .GroupBy(GetCompilerDiagnosticKey, StringComparer.Ordinal)
                .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.Ordinal);
            List<string> newErrors = [];
            foreach (Diagnostic diagnostic in updatedCompilation.GetDiagnostics(cancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
            {
                string key = GetCompilerDiagnosticKey(diagnostic);
                if (remainingErrors.TryGetValue(key, out int count) && count > 0)
                {
                    remainingErrors[key] = count - 1;
                }
                else
                {
                    newErrors.Add(diagnostic.ToString());
                }
            }

            return newErrors;
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    public async Task<IReadOnlyList<string>> GetNewSourceCompilerErrorsAsync(
        string workspaceRoot,
        string projectPath,
        WorkspaceFileChangeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentNullException.ThrowIfNull(request);
        if (request.Target != WorkspaceFileChangeTarget.MustNotExist)
        {
            throw new InvalidDataException("A new source file is required for compiler validation.");
        }

        string targetPath = Path.GetFullPath(Path.Combine(workspaceRoot, request.RelativePath));
        if (!IsWithinDirectory(targetPath, Path.GetDirectoryName(Path.GetFullPath(projectPath))!)
            || File.Exists(targetPath))
        {
            throw new InvalidDataException("The generated source must be a new file within the target project.");
        }

        await _workspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ProjectContext context = await GetProjectContextAsync(projectPath, cancellationToken)
                .ConfigureAwait(false);
            Microsoft.CodeAnalysis.Project project = context.Workspace.CurrentSolution.GetProject(context.ProjectId)
                ?? throw new InvalidOperationException($"Could not load project '{projectPath}'.");
            if (project.Documents.Any(document => document.FilePath is not null
                && WorkspacePathIdentity.AreEqual(document.FilePath, targetPath)))
            {
                throw new InvalidDataException($"The generated source '{request.RelativePath}' already exists in the project.");
            }

            Compilation baseline = await project.GetCompilationAsync(cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not compile project '{projectPath}'.");
            Document added = project.AddDocument(Path.GetFileName(targetPath),
                SourceText.From(request.ProposedContent), filePath: targetPath);
            Compilation updated = await added.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException($"Could not compile the generated source for '{projectPath}'.");
            Dictionary<string, int> remainingErrors = baseline.GetDiagnostics(cancellationToken)
                .Where(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                .GroupBy(GetCompilerDiagnosticKey, StringComparer.Ordinal)
                .ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.Ordinal);
            List<string> errors = [];
            foreach (Diagnostic diagnostic in updated.GetDiagnostics(cancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))
            {
                string key = GetCompilerDiagnosticKey(diagnostic);
                if (remainingErrors.TryGetValue(key, out int count) && count > 0)
                {
                    remainingErrors[key] = count - 1;
                }
                else
                {
                    errors.Add(diagnostic.ToString());
                }
            }

            return errors;
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    private static string GetCompilerDiagnosticKey(Diagnostic diagnostic)
    {
        string sourcePath = diagnostic.Location.SourceTree?.FilePath ?? string.Empty;
        string normalizedPath = string.IsNullOrWhiteSpace(sourcePath)
            ? string.Empty
            : WorkspacePathIdentity.Normalize(sourcePath);
        return $"{diagnostic.Id}|{normalizedPath}|{diagnostic.GetMessage()}";
    }

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetGeneratedPropertyType(
        string requestedType,
        out string fieldType,
        out string initialValue)
    {
        (fieldType, initialValue) = requestedType switch
        {
            "string" => ("string", "string.Empty"),
            "string?" => ("string?", "null"),
            "bool?" => ("bool?", "null"),
            "decimal?" => ("decimal?", "null"),
            "double" => ("double", "0d"),
            "int" => ("int", "0"),
            _ => (string.Empty, string.Empty),
        };
        return fieldType.Length > 0;
    }

    private static string GetLineIndentation(string source, int position)
    {
        int lineStart = source.LastIndexOfAny(['\r', '\n'], Math.Max(0, position - 1)) + 1;
        int firstNonWhitespace = lineStart;
        while (firstNonWhitespace < source.Length
            && source[firstNonWhitespace] is ' ' or '\t')
        {
            firstNonWhitespace++;
        }

        return source[lineStart..firstNonWhitespace];
    }

    private static string WithAxamlLocation(string message, XObject? node)
    {
        if (node is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            return $"{message} (line {lineInfo.LineNumber}, column {lineInfo.LinePosition})";
        }

        return message;
    }

    private static AxamlMvvmSemanticMember[] GetViewModelMembers(INamedTypeSymbol viewModelType)
    {
        List<AxamlMvvmSemanticMember> members = [];
        HashSet<string> memberNames = new(StringComparer.Ordinal);
        for (INamedTypeSymbol? currentType = viewModelType;
             currentType is not null && currentType.SpecialType != SpecialType.System_Object;
             currentType = currentType.BaseType)
        {
            foreach (IPropertySymbol property in currentType.GetMembers().OfType<IPropertySymbol>()
                         .Where(static property =>
                             !property.IsStatic
                             && property.Parameters.Length == 0
                             && property.GetMethod?.DeclaredAccessibility == Accessibility.Public))
            {
                if (!memberNames.Add(property.Name))
                {
                    continue;
                }

                bool isCommand = property.Type is INamedTypeSymbol namedType
                    && (namedType.ToDisplayString() == "System.Windows.Input.ICommand"
                        || namedType.AllInterfaces.Any(static implementedInterface =>
                            implementedInterface.ToDisplayString() == "System.Windows.Input.ICommand"));
                members.Add(new AxamlMvvmSemanticMember(
                    property.Name,
                    property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    isCommand,
                    !isCommand
                        && property.SetMethod?.DeclaredAccessibility == Accessibility.Public
                        && !property.SetMethod.IsInitOnly));
            }

            foreach (ISymbol member in currentType.GetMembers())
            {
                if (memberNames.Contains(member.Name))
                {
                    continue;
                }
                memberNames.Add(member.Name);

                if (member is IFieldSymbol field
                    && HasAttribute(field, "CommunityToolkit.Mvvm.ComponentModel.ObservablePropertyAttribute")
                    && TryGetObservablePropertyName(field.Name, out string propertyName)
                    && memberNames.Add(propertyName))
                {
                    members.Add(new AxamlMvvmSemanticMember(
                        propertyName,
                        field.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                        IsCommand: false,
                        IsWritable: true));
                }
                else if (member is IMethodSymbol method
                    && HasAttribute(method, "CommunityToolkit.Mvvm.Input.RelayCommandAttribute")
                    && TryGetRelayCommandPropertyName(method, out string commandPropertyName)
                    && memberNames.Add(commandPropertyName))
                {
                    bool isAsync = method.ReturnType.ToDisplayString() is "System.Threading.Tasks.Task"
                        or "System.Threading.Tasks.ValueTask"
                        || method.ReturnType is INamedTypeSymbol returnType
                            && returnType.ConstructedFrom.ToDisplayString() is "System.Threading.Tasks.Task<TResult>"
                                or "System.Threading.Tasks.ValueTask<TResult>";
                    members.Add(new AxamlMvvmSemanticMember(
                        commandPropertyName,
                        isAsync ? "IAsyncRelayCommand" : "IRelayCommand",
                        IsCommand: true,
                        IsWritable: false));
                    if (TryGetRelayCommandCancelPropertyName(method, commandPropertyName, out string cancelCommandName)
                        && memberNames.Add(cancelCommandName))
                    {
                        members.Add(new AxamlMvvmSemanticMember(
                            cancelCommandName,
                            "ICommand",
                            IsCommand: true,
                            IsWritable: false));
                    }
                }
            }
        }

        return members.OrderBy(static member => member.Name, StringComparer.Ordinal).ToArray();
    }

    private static bool HasAttribute(ISymbol symbol, string metadataName)
    {
        return symbol.GetAttributes().Any(attribute =>
            string.Equals(
                attribute.AttributeClass?.ToDisplayString(),
                metadataName,
                StringComparison.Ordinal));
    }

    private static bool TryGetObservablePropertyName(string fieldName, out string propertyName)
    {
        string baseName = fieldName.StartsWith("m_", StringComparison.Ordinal)
            ? fieldName[2..]
            : fieldName.StartsWith('_')
                ? fieldName[1..]
                : fieldName;
        if (baseName.Length == 0 || !SyntaxFacts.IsValidIdentifier(baseName))
        {
            propertyName = string.Empty;
            return false;
        }

        propertyName = char.ToUpperInvariant(baseName[0]) + baseName[1..];
        return SyntaxFacts.IsValidIdentifier(propertyName);
    }

    private static bool TryGetRelayCommandPropertyName(IMethodSymbol method, out string propertyName)
    {
        string methodName = method.Name;
        bool returnsTask = method.ReturnType.ToDisplayString() is "System.Threading.Tasks.Task"
            or "System.Threading.Tasks.ValueTask"
            || method.ReturnType is INamedTypeSymbol returnType
                && returnType.ConstructedFrom.ToDisplayString() is "System.Threading.Tasks.Task<TResult>"
                    or "System.Threading.Tasks.ValueTask<TResult>";
        if (returnsTask && methodName.EndsWith("Async", StringComparison.Ordinal)
            && methodName.Length > "Async".Length)
        {
            methodName = methodName[..^"Async".Length];
        }

        propertyName = methodName + "Command";
        return SyntaxFacts.IsValidIdentifier(propertyName);
    }

    private static bool TryGetRelayCommandCancelPropertyName(
        IMethodSymbol method,
        string commandPropertyName,
        out string cancelCommandName)
    {
        AttributeData? relayCommandAttribute = method.GetAttributes()
            .FirstOrDefault(attribute => string.Equals(
                attribute.AttributeClass?.ToDisplayString(),
                "CommunityToolkit.Mvvm.Input.RelayCommandAttribute",
                StringComparison.Ordinal));
        bool includesCancelCommand = relayCommandAttribute?.NamedArguments.Any(static argument =>
            argument.Key == "IncludeCancelCommand"
            && argument.Value.Value is true) == true;
        bool isAsync = method.ReturnType.ToDisplayString() is "System.Threading.Tasks.Task"
            or "System.Threading.Tasks.ValueTask"
            || method.ReturnType is INamedTypeSymbol returnType
                && returnType.ConstructedFrom.ToDisplayString() is "System.Threading.Tasks.Task<TResult>"
                    or "System.Threading.Tasks.ValueTask<TResult>";
        bool acceptsCancellationToken = method.Parameters.Any(static parameter =>
            parameter.Type.ToDisplayString() == "System.Threading.CancellationToken");
        string commandName = commandPropertyName.EndsWith("Command", StringComparison.Ordinal)
            ? commandPropertyName[..^"Command".Length]
            : string.Empty;
        if (!includesCancelCommand || !isAsync || !acceptsCancellationToken || commandName.Length == 0)
        {
            cancelCommandName = string.Empty;
            return false;
        }

        cancelCommandName = commandName + "CancelCommand";
        return SyntaxFacts.IsValidIdentifier(cancelCommandName);
    }

    private static string ToBackingFieldName(string propertyName)
    {
        return propertyName.Length == 0
            ? string.Empty
            : char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
    }

    private static INamedTypeSymbol? ResolveAxamlType(
        string typeReference,
        XElement element,
        Compilation compilation)
    {
        string reference = typeReference.Trim();
        if (reference.StartsWith('{') && reference.EndsWith('}'))
        {
            string extension = reference[1..^1].Trim();
            int extensionSeparator = extension.IndexOfAny([' ', '\t', '\r', '\n']);
            if (extensionSeparator < 0
                || !string.Equals(extension[..extensionSeparator], "x:Type", StringComparison.Ordinal))
            {
                return null;
            }

            reference = extension[(extensionSeparator + 1)..].Trim();
        }

        int separator = reference.IndexOf(':');
        if (separator <= 0 || separator == reference.Length - 1)
        {
            return null;
        }

        string prefix = reference[..separator];
        string typeName = reference[(separator + 1)..];
        string? xmlNamespace = element.GetNamespaceOfPrefix(prefix)?.NamespaceName;
        if (string.IsNullOrWhiteSpace(xmlNamespace))
        {
            return null;
        }

        List<(string ClrNamespace, string? AssemblyName)> namespaceMappings = [];
        if (xmlNamespace.StartsWith("using:", StringComparison.Ordinal))
        {
            namespaceMappings.Add((xmlNamespace["using:".Length..], null));
        }
        else if (xmlNamespace.StartsWith("clr-namespace:", StringComparison.Ordinal))
        {
            string[] parts = xmlNamespace["clr-namespace:".Length..].Split(';');
            string clrNamespace = parts[0].Trim();
            string? assemblyName = parts
                .Skip(1)
                .Select(static part => part.Trim())
                .Where(static part => part.StartsWith("assembly=", StringComparison.Ordinal))
                .Select(static part => part["assembly=".Length..].Trim())
                .FirstOrDefault();
            namespaceMappings.Add((clrNamespace, assemblyName));
        }
        else
        {
            namespaceMappings.AddRange(FindXmlnsDefinitionMappings(xmlNamespace, compilation));
        }

        INamedTypeSymbol[] matches = namespaceMappings
            .Select(mapping => (
                MetadataName: string.IsNullOrEmpty(mapping.ClrNamespace)
                    ? typeName
                    : $"{mapping.ClrNamespace}.{typeName}",
                mapping.AssemblyName))
            .Select(candidate => ResolveMetadataType(
                candidate.MetadataName,
                candidate.AssemblyName,
                compilation))
            .Where(static symbol => symbol is not null)
            .Cast<INamedTypeSymbol>()
            .DistinctBy(static symbol => (
                symbol.ContainingAssembly?.Identity,
                symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)))
            .ToArray();
        return matches.Length == 1 ? matches[0] : null;
    }

    private static INamedTypeSymbol? ResolveMetadataType(
        string metadataName,
        string? assemblyName,
        Compilation compilation)
    {
        if (assemblyName is null)
        {
            return compilation.GetTypeByMetadataName(metadataName);
        }

        IAssemblySymbol[] matchingAssemblies = compilation.SourceModule.ReferencedAssemblySymbols
            .Append(compilation.Assembly)
            .Where(assembly => string.Equals(assembly.Identity.Name, assemblyName, StringComparison.Ordinal))
            .ToArray();
        if (matchingAssemblies.Length != 1)
        {
            return null;
        }

        return matchingAssemblies[0].GetTypeByMetadataName(metadataName);
    }

    private static IEnumerable<(string ClrNamespace, string? AssemblyName)> FindXmlnsDefinitionMappings(
        string xmlNamespace,
        Compilation compilation)
    {
        foreach (IAssemblySymbol assembly in compilation.SourceModule.ReferencedAssemblySymbols
                     .Append(compilation.Assembly))
        {
            foreach (AttributeData attribute in assembly.GetAttributes())
            {
                if (!string.Equals(
                        attribute.AttributeClass?.ToDisplayString(),
                        "Avalonia.Metadata.XmlnsDefinitionAttribute",
                        StringComparison.Ordinal)
                    || attribute.ConstructorArguments.Length < 2
                    || attribute.ConstructorArguments[0].Value is not string mappedXmlNamespace
                    || attribute.ConstructorArguments[1].Value is not string clrNamespace
                    || !string.Equals(mappedXmlNamespace, xmlNamespace, StringComparison.Ordinal))
                {
                    continue;
                }

                yield return (clrNamespace, assembly.Identity.Name);
            }
        }
    }

    private static async Task<List<INamedTypeSymbol>> FindRuntimeDataContextTypesAsync(
        Microsoft.CodeAnalysis.Project project,
        Compilation compilation,
        INamedTypeSymbol viewType,
        CancellationToken cancellationToken)
    {
        List<INamedTypeSymbol> result = [];
        foreach (Document document in project.Documents)
        {
            SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);
            if (syntaxRoot is null)
            {
                continue;
            }

            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxRoot.SyntaxTree);
            foreach (AssignmentExpressionSyntax assignment in syntaxRoot
                         .DescendantNodes()
                         .OfType<AssignmentExpressionSyntax>()
                         .Where(static assignment => assignment.IsKind(SyntaxKind.SimpleAssignmentExpression)))
            {
                if (!IsDataContextAssignmentForView(assignment.Left, semanticModel, viewType, cancellationToken))
                {
                    continue;
                }

                ITypeSymbol? assignedType = semanticModel.GetTypeInfo(assignment.Right, cancellationToken).Type;
                if (assignedType is INamedTypeSymbol namedType
                    && !result.Any(existing => SymbolEqualityComparer.Default.Equals(existing, namedType)))
                {
                    result.Add(namedType);
                }
            }
        }

        return result;
    }

    private static bool IsDataContextAssignmentForView(
        ExpressionSyntax left,
        SemanticModel semanticModel,
        INamedTypeSymbol viewType,
        CancellationToken cancellationToken)
    {
        if (left is MemberAccessExpressionSyntax memberAccess
            && memberAccess.Name.Identifier.ValueText == "DataContext")
        {
            return SymbolEqualityComparer.Default.Equals(
                semanticModel.GetTypeInfo(memberAccess.Expression, cancellationToken).Type,
                viewType);
        }

        if (left is not IdentifierNameSyntax identifier
            || identifier.Identifier.ValueText != "DataContext")
        {
            return false;
        }

        InitializerExpressionSyntax? initializer = identifier.Ancestors()
            .OfType<InitializerExpressionSyntax>()
            .FirstOrDefault(candidate => candidate.IsKind(SyntaxKind.ObjectInitializerExpression));
        if (initializer?.Parent?.Parent is ObjectCreationExpressionSyntax objectCreation)
        {
            return SymbolEqualityComparer.Default.Equals(
                semanticModel.GetTypeInfo(objectCreation, cancellationToken).Type,
                viewType);
        }

        return SymbolEqualityComparer.Default.Equals(
            semanticModel.GetEnclosingSymbol(identifier.SpanStart, cancellationToken)?.ContainingType,
            viewType);
    }

    private async Task<ProjectContext> GetProjectContextAsync(
        string projectPath,
        CancellationToken cancellationToken,
        string? targetFramework = null)
    {
        string normalizedProjectPath = Path.GetFullPath(projectPath);
        RegisterMsBuildLocator();
        string[] targetFrameworks = GetProjectTargetFrameworks(normalizedProjectPath);
        string normalizedFramework = string.IsNullOrWhiteSpace(targetFramework) ? string.Empty : targetFramework.Trim();
        if (targetFrameworks.Length > 1 && normalizedFramework.Length == 0)
        {
            throw new InvalidOperationException(
                $"Project '{normalizedProjectPath}' targets multiple frameworks "
                + $"({string.Join(", ", targetFrameworks)}); select a target framework before using C# language services.");
        }
        if (normalizedFramework.Length > 0
            && targetFrameworks.Length > 0
            && !targetFrameworks.Contains(normalizedFramework, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Target framework '{normalizedFramework}' is not declared by project '{normalizedProjectPath}'.");
        }

        string contextKey = normalizedProjectPath + "\0" + normalizedFramework;
        if (_projectContexts.TryGetValue(contextKey, out ProjectContext? existing))
        {
            return existing;
        }

        HostServices hostServices = CreateHostServices();
        Dictionary<string, string> globalProperties = normalizedFramework.Length == 0
            ? new(StringComparer.OrdinalIgnoreCase)
            : new(StringComparer.OrdinalIgnoreCase) { ["TargetFramework"] = normalizedFramework };
        MSBuildWorkspace workspace = MSBuildWorkspace.Create(globalProperties, hostServices);
        try
        {
            Microsoft.CodeAnalysis.Project project = await workspace.OpenProjectAsync(normalizedProjectPath, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            ProjectContext context = new(workspace, project.Id);
            _projectContexts.Add(contextKey, context);
            return context;
        }
        catch
        {
            workspace.Dispose();
            throw;
        }
    }

    private static string[] GetTargetFrameworks(string projectPath)
    {
        using Microsoft.Build.Evaluation.ProjectCollection collection = new();
        Microsoft.Build.Evaluation.Project project = collection.LoadProject(projectPath);
        string[] frameworks = project.GetPropertyValue("TargetFrameworks")
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (frameworks.Length > 0)
        {
            return frameworks;
        }

        string framework = project.GetPropertyValue("TargetFramework");
        return string.IsNullOrWhiteSpace(framework) ? [] : [framework.Trim()];
    }

    private string[] GetProjectTargetFrameworks(string projectPath)
    {
        string normalizedProjectPath = Path.GetFullPath(projectPath);
        if (_projectTargetFrameworks.TryGetValue(normalizedProjectPath, out string[]? frameworks))
        {
            return frameworks;
        }

        frameworks = GetTargetFrameworks(normalizedProjectPath);
        _projectTargetFrameworks.Add(normalizedProjectPath, frameworks);
        return frameworks;
    }

    private static Document? CreateDocumentSnapshot(
        Microsoft.CodeAnalysis.Project project,
        string activeFilePath,
        string activeSource,
        IReadOnlyDictionary<string, string>? openDocumentTexts)
    {
        Dictionary<string, string> texts = new(WorkspacePathIdentity.Comparer);
        if (openDocumentTexts is not null)
        {
            foreach ((string path, string text) in openDocumentTexts)
            {
                string normalizedPath = WorkspacePathIdentity.Normalize(path);
                if (texts.TryGetValue(normalizedPath, out string? previous)
                    && !string.Equals(previous, text, StringComparison.Ordinal))
                {
                    throw new InvalidDataException($"More than one open editor buffer maps to '{normalizedPath}'.");
                }

                texts[normalizedPath] = text;
            }
        }

        texts[WorkspacePathIdentity.Normalize(activeFilePath)] = activeSource;
        Solution solution = project.Solution;
        HashSet<ProjectId> reachableProjectIds = new() { project.Id };
        Queue<ProjectId> projectsToVisit = new();
        projectsToVisit.Enqueue(project.Id);
        while (projectsToVisit.TryDequeue(out ProjectId? projectId))
        {
            Microsoft.CodeAnalysis.Project? currentProject = solution.GetProject(projectId);
            if (currentProject is null)
            {
                continue;
            }

            foreach (ProjectReference reference in currentProject.ProjectReferences)
            {
                if (reachableProjectIds.Add(reference.ProjectId))
                {
                    projectsToVisit.Enqueue(reference.ProjectId);
                }
            }
        }

        Dictionary<string, List<Document>> documentsByPath = new(WorkspacePathIdentity.Comparer);
        foreach (ProjectId projectId in reachableProjectIds)
        {
            Microsoft.CodeAnalysis.Project? relatedProject = solution.GetProject(projectId);
            if (relatedProject is null)
            {
                continue;
            }

            foreach (Document candidate in relatedProject.Documents)
            {
                if (candidate.FilePath is not { } path)
                {
                    continue;
                }

                string normalizedPath = WorkspacePathIdentity.Normalize(path);
                if (!documentsByPath.TryGetValue(normalizedPath, out List<Document>? documents))
                {
                    documents = [];
                    documentsByPath.Add(normalizedPath, documents);
                }

                documents.Add(candidate);
            }
        }

        foreach ((string path, string text) in texts)
        {
            if (documentsByPath.TryGetValue(path, out List<Document>? candidates)
                && candidates.Count == 1)
            {
                solution = solution.WithDocumentText(candidates[0].Id, SourceText.From(text));
            }
            else if (WorkspacePathIdentity.AreEqual(path, activeFilePath)
                && documentsByPath.TryGetValue(path, out candidates))
            {
                Document? activeDocument = candidates.FirstOrDefault(candidate => candidate.Project.Id == project.Id);
                if (activeDocument is not null)
                {
                    solution = solution.WithDocumentText(activeDocument.Id, SourceText.From(text));
                }
            }
        }

        return solution.GetProject(project.Id)?.Documents.FirstOrDefault(candidate =>
            candidate.FilePath is not null
            && WorkspacePathIdentity.AreEqual(candidate.FilePath, activeFilePath));
    }

    private static HostServices CreateHostServices()
    {
        Assembly csharpWorkspaceAssembly = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces");
        Assembly csharpFeaturesAssembly = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features");
        return MefHostServices.Create(MefHostServices.DefaultAssemblies
            .Append(csharpWorkspaceAssembly)
            .Append(csharpFeaturesAssembly));
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
        _projectTargetFrameworks.Clear();
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

public sealed class CSharpCompletionSuggestion
{
    internal CSharpCompletionSuggestion(Document document, CompletionService service, CompletionItem item)
    {
        Document = document;
        Service = service;
        Item = item;
    }

    internal Document Document { get; }
    internal CompletionService Service { get; }
    internal CompletionItem Item { get; }
    public string DisplayText => Item.DisplayText;

    public override string ToString()
    {
        string label = Item.DisplayTextPrefix + Item.DisplayText + Item.DisplayTextSuffix;
        return string.IsNullOrWhiteSpace(Item.InlineDescription) ? label : $"{label} ({Item.InlineDescription})";
    }
}

public sealed record CSharpCompletionEdit(int Start, int Length, string NewText, int CaretOffset);

public sealed record CSharpSignatureHelp(
    IReadOnlyList<string> Signatures,
    int SelectedSignatureIndex,
    int ActiveParameterIndex,
    string? ActiveParameterName);
