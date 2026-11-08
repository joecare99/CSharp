using Property.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Exposes a project's explicit, unconditional AssemblyName as a reviewed workspace property.
/// </summary>
/// <remarks>
/// Cancellation is honored before applying a confirmed proposal. Once application succeeds,
/// the committed snapshot is retained even if cancellation is subsequently requested.
/// </remarks>
public sealed class ProjectAssemblyNameProvider : IPropertyProvider
{
    private static readonly Regex s_literalElement = new(
        @"<AssemblyName\s*>(?<value>[^<]*)</AssemblyName\s*>",
        RegexOptions.CultureInvariant);
    private static readonly Regex s_validAssemblyName = new(
        @"\A[A-Za-z0-9._-]+\z",
        RegexOptions.CultureInvariant);

    private readonly string _workspaceRoot;
    private readonly string _projectPath;
    private readonly string? _relativeProjectPath;
    private readonly WorkspaceFileChangeService _changeService;
    private readonly Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? _reviewAsync;
    private readonly string? _workspaceReadOnlyReason;
    private string? _source;
    private PropertyItem? _assemblyName;
    private string? _readOnlyReason;
    private PropertyProviderContext _context;

    /// <summary>Creates a reviewed project property provider.</summary>
    /// <param name="workspaceRoot">The workspace containing the project.</param>
    /// <param name="projectPath">The project file to inspect.</param>
    /// <param name="changeService">The service that checks and applies confirmed changes.</param>
    /// <param name="reviewAsync">Reviews the proposal and returns approval without applying it.</param>
    public ProjectAssemblyNameProvider(
        string workspaceRoot,
        string projectPath,
        WorkspaceFileChangeService changeService,
        Func<WorkspaceFileChangeProposal, CancellationToken, Task<bool>>? reviewAsync)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspaceRoot);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        _workspaceRoot = Path.GetFullPath(workspaceRoot);
        _projectPath = Path.GetFullPath(projectPath);
        _changeService = changeService ?? throw new ArgumentNullException(nameof(changeService));
        _reviewAsync = reviewAsync;

        string relativePath = Path.GetRelativePath(_workspaceRoot, _projectPath);
        if (Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
            || relativePath.StartsWith($"..{Path.AltDirectorySeparatorChar}", StringComparison.Ordinal))
        {
            _workspaceReadOnlyReason = "The project file is outside the active workspace and cannot be changed through workspace review.";
        }
        else
        {
            _relativeProjectPath = relativePath;
        }

        _readOnlyReason = _workspaceReadOnlyReason;
        _context = CreateContext(_readOnlyReason);
    }

    /// <inheritdoc/>
    public PropertyProviderContext Context => _context;

    /// <inheritdoc/>
    public IReadOnlyList<IPropertyItem> Properties =>
        _assemblyName is null ? Array.Empty<IPropertyItem>() : [_assemblyName];

    /// <inheritdoc/>
    public bool IsReadOnly => _assemblyName is null || !_assemblyName.IsEditable;

    /// <summary>Gets the most recent load or apply failure.</summary>
    public string? ErrorMessage { get; private set; }

    /// <inheritdoc/>
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ErrorMessage = null;
        string source = await File.ReadAllTextAsync(_projectPath, cancellationToken);
        SetSource(source);
    }

    private void SetSource(string source)
    {
        (string value, string? diagnostic) = InspectAssemblyName(source);
        _readOnlyReason = _workspaceReadOnlyReason ?? diagnostic;
        if (_readOnlyReason is null && _reviewAsync is null)
            _readOnlyReason = "A workspace change review service is required before project settings can be edited.";

        _context = CreateContext(_readOnlyReason);
        _assemblyName = new PropertyItem(
            new PropertyCategory("Project", "Project"),
            "AssemblyName",
            "Assembly name",
            typeof(string),
            value,
            isEditable: _readOnlyReason is null,
            validator: _readOnlyReason is null ? ValidateAssemblyName : null,
            valueContext: new PropertyValueContext(
                PropertyScope.Project,
                origin: _projectPath,
                readOnlyReason: _readOnlyReason),
            readOnlyReason: _readOnlyReason);
        _source = source;
    }

    /// <inheritdoc/>
    public async Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (IsReadOnly || _source is null || _relativeProjectPath is null || _assemblyName is null)
        {
            throw new InvalidOperationException(_readOnlyReason ?? "The project AssemblyName value is not safely editable.");
        }

        if (!_assemblyName.ValidationResult.IsValid)
        {
            throw new InvalidOperationException(_assemblyName.ValidationResult.ErrorMessage ?? "The assembly name is invalid.");
        }

        string proposedContent = ReplaceAssemblyName(_source, (string)_assemblyName.Value!);
        if (string.Equals(_source, proposedContent, StringComparison.Ordinal))
        {
            return;
        }

        WorkspaceFileChangeProposal proposal = _changeService.Prepare(
            _workspaceRoot,
            [WorkspaceFileChangeRequest.ReplaceExisting(_relativeProjectPath, _source, proposedContent)]);

        bool confirmed = await _reviewAsync!(proposal, cancellationToken);
        if (!confirmed)
        {
            ErrorMessage = "The project setting was not changed because workspace review was declined.";
            throw new InvalidOperationException(ErrorMessage);
        }

        cancellationToken.ThrowIfCancellationRequested();
        _changeService.Apply(proposal, confirmed: true);
        SetSource(proposedContent);
        ErrorMessage = null;
    }

    /// <inheritdoc/>
    public Task DiscardAsync(CancellationToken cancellationToken = default) => LoadAsync(cancellationToken);

    private PropertyProviderContext CreateContext(string? readOnlyReason)
    {
        string target = Path.GetFileName(_projectPath);
        return new PropertyProviderContext(
            PropertyScope.Project,
            "Project settings",
            target,
            _projectPath,
            readOnlyReason);
    }

    private static PropertyValidationResult ValidateAssemblyName(object? value)
    {
        return value is string text && s_validAssemblyName.IsMatch(text)
            ? PropertyValidationResult.Valid
            : PropertyValidationResult.Invalid("Use only letters, digits, dot, underscore, or hyphen in the assembly name.");
    }

    private static (string Value, string? Diagnostic) InspectAssemblyName(string source)
    {
        try
        {
            XDocument document = XDocument.Parse(source, LoadOptions.PreserveWhitespace);
            XElement? root = document.Root;
            if (root is null || root.Name.LocalName != "Project")
            {
                return (string.Empty, "The project file does not have a recognized MSBuild Project root.");
            }

            XElement[] properties = root.Descendants()
                .Where(static element => element.Name.LocalName == "AssemblyName")
                .ToArray();
            if (properties.Length != 1)
            {
                return (string.Empty, "AssemblyName must be declared exactly once in the project file.");
            }

            XElement property = properties[0];
            if (property.Name.NamespaceName.Length > 0
                || property.Attributes().Any()
                || property.Parent?.Name.LocalName != "PropertyGroup"
                || property.Parent.Parent != root
                || property.Attribute("Condition") is not null
                || property.Parent.Attribute("Condition") is not null)
            {
                return (property.Value.Trim(), "Conditional or non-standard AssemblyName declarations are read-only.");
            }

            if (root.Descendants().Any(static element => element.Name.LocalName == "Import"))
            {
                return (property.Value.Trim(), "Projects with explicit MSBuild imports require evaluation before AssemblyName can be edited safely.");
            }

            if (property.Nodes().Any(static node => node is not XText || node is XCData))
            {
                return (property.Value.Trim(), "AssemblyName must be a plain literal value.");
            }

            string value = property.Value.Trim();
            if (value.Length == 0
                || value.Contains("$(", StringComparison.Ordinal)
                || value.Contains("@(", StringComparison.Ordinal)
                || value.Contains("%(", StringComparison.Ordinal)
                || !s_validAssemblyName.IsMatch(value))
            {
                return (value, "AssemblyName must be a simple, non-empty literal before it can be edited.");
            }

            if (s_literalElement.Matches(source).Count != 1)
            {
                return (value, "The AssemblyName element cannot be edited without changing unrelated project-file formatting.");
            }

            return (value, null);
        }
        catch (System.Xml.XmlException exception)
        {
            return (string.Empty, $"The project file is not valid XML: {exception.Message}");
        }
    }

    private static string ReplaceAssemblyName(string source, string value)
    {
        Match match = s_literalElement.Match(source);
        if (!match.Success || s_literalElement.Matches(source).Count != 1)
        {
            throw new InvalidOperationException("The project file changed shape; reload its settings before applying.");
        }

        string rawValue = match.Groups["value"].Value;
        int leadingWhitespaceLength = rawValue.TakeWhile(char.IsWhiteSpace).Count();
        int trailingWhitespaceLength = rawValue.Reverse().TakeWhile(char.IsWhiteSpace).Count();
        string leadingWhitespace = rawValue[..leadingWhitespaceLength];
        string trailingWhitespace = rawValue[(rawValue.Length - trailingWhitespaceLength)..];
        string replacement = leadingWhitespace + value + trailingWhitespace;
        return source[..match.Groups["value"].Index]
            + replacement
            + source[(match.Groups["value"].Index + rawValue.Length)..];
    }
}
