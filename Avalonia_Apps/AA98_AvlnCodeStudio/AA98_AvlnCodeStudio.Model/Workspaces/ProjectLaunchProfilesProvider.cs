using Property.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>Exposes project launch profiles through the shared, read-only property contract.</summary>
public sealed class ProjectLaunchProfilesProvider : IPropertyProvider
{
    private const string ReadOnlyReason = "Inspection only. The workbench debugger does not use launch profiles.";
    private const string ItemReadOnlyReason = "Read-only profile inspection.";
    private static readonly PropertyCategory s_category = new("LaunchProfiles", "Launch profiles");

    private readonly string _projectPath;
    private readonly ProjectLaunchSettingsReader _reader = new();
    private IReadOnlyList<IPropertyItem> _properties = Array.Empty<IPropertyItem>();

    public ProjectLaunchProfilesProvider(string projectPath)
    {
        _projectPath = Path.GetFullPath(projectPath);
        string settingsPath = ProjectLaunchSettingsReader.GetSettingsPath(_projectPath);
        Context = new PropertyProviderContext(
            PropertyScope.Project,
            "Launch profile inspection",
            Path.GetFileName(_projectPath),
            settingsPath,
            ReadOnlyReason);
    }

    public PropertyProviderContext Context { get; }
    public IReadOnlyList<IPropertyItem> Properties => _properties;
    public bool IsReadOnly => true;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        _properties = Array.Empty<IPropertyItem>();
        ProjectLaunchSettingsSnapshot snapshot = await _reader.ReadAsync(_projectPath, cancellationToken);
        List<IPropertyItem> items = [];
        string status = !snapshot.FileExists
            ? "No project launch settings file exists; nothing was created."
            : !snapshot.HasProfilesObject
                ? "The launch settings file has no profiles object."
                : snapshot.Profiles.Count == 0
                    ? "The launch settings file contains no profiles."
                    : "Profiles are shown without argument or environment values.";
        items.Add(CreateItem("Status", "Status", status, snapshot.FilePath, 0));

        for (int index = 0; index < snapshot.Profiles.Count; index++)
        {
            ProjectLaunchProfile profile = snapshot.Profiles[index];
            string value = profile.CommandName is null ? "commandName unavailable" : $"commandName: {profile.CommandName}";
            if (profile.HasCommandLineArguments)
            {
                value += "; arguments specified";
            }
            if (profile.HasEnvironmentVariables)
            {
                value += "; environment specified";
            }
            if (profile.Diagnostic is not null)
            {
                value += $"; {profile.Diagnostic}";
            }

            items.Add(CreateItem($"Profile{index + 1}", profile.Name, value, snapshot.FilePath, index + 1));
        }

        _properties = items.AsReadOnly();
    }

    public Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromException(new InvalidOperationException(ReadOnlyReason));
    }

    public Task DiscardAsync(CancellationToken cancellationToken = default) => LoadAsync(cancellationToken);

    private static PropertyItem CreateItem(string name, string displayName, string value, string origin, int sortOrder) =>
        new(
            s_category,
            name,
            displayName,
            typeof(string),
            value,
            isEditable: false,
            sortOrder: sortOrder,
            valueContext: new PropertyValueContext(
                PropertyScope.Project,
                origin: Path.GetFileName(origin),
                readOnlyReason: ItemReadOnlyReason),
            readOnlyReason: ItemReadOnlyReason);
}
