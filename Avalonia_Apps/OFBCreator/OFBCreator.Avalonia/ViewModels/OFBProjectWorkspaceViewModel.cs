using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OFBCreator.Avalonia.Services;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;
using OFBCreator.Publishing.Services.Templates;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Avalonia.ViewModels;

public sealed partial class OFBProjectWorkspaceViewModel : ObservableObject
{
    private readonly OFBProjectStore _projectStore;
    private readonly IOFBWorkspaceService _workspaceService;
    private readonly IOFBFileDialogService? _fileDialogService;
    private readonly EntryTemplateStore _templateStore;
    private OFBProject? _loadedProject;
    private readonly Dictionary<string, string> _groupingTargetBySurname = new(StringComparer.OrdinalIgnoreCase);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DefaultProjectPath))]
    [NotifyPropertyChangedFor(nameof(DefaultOutputPath))]
    private string _projectFilePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DefaultProjectPath))]
    [NotifyPropertyChangedFor(nameof(DefaultOutputPath))]
    private string _name = "New OFB";

    [ObservableProperty]
    private string _title = "New OFB";

    [ObservableProperty]
    private string _inputPath = string.Empty;

    [ObservableProperty]
    private string _outputPath = string.Empty;

    [ObservableProperty]
    private string _entryTemplate = "gc";

    [ObservableProperty]
    private string? _placeId;

    [ObservableProperty]
    private bool _includeDescendants;

    [ObservableProperty]
    private string? _preface;

    [ObservableProperty]
    private string? _legend;

    [ObservableProperty]
    private int _autoAcceptThreshold = 85;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = "Create or open a portable .ofbproject to begin.";

    [ObservableProperty]
    private OFBExportRule? _selectedRule;

    [ObservableProperty]
    private string _ruleTargetKind = "person";

    [ObservableProperty]
    private string _ruleTargetId = string.Empty;

    [ObservableProperty]
    private string _ruleAction = "exclude";

    [ObservableProperty]
    private string _ruleField = string.Empty;

    [ObservableProperty]
    private string _ruleValue = string.Empty;

    [ObservableProperty]
    private int? _ruleOccurrence;

    [ObservableProperty]
    private string _manualGroupName = string.Empty;

    private string? _leftGroupingChoice;
    private string? _rightGroupingChoice;

    public string? LeftGroupingChoice
    {
        get => _leftGroupingChoice;
        set => SetProperty(ref _leftGroupingChoice, value);
    }

    public string? RightGroupingChoice
    {
        get => _rightGroupingChoice;
        set => SetProperty(ref _rightGroupingChoice, value);
    }

    public OFBProjectWorkspaceViewModel(
        OFBProjectStore projectStore,
        IOFBWorkspaceService workspaceService,
        EntryTemplateStore? templateStore = null,
        IOFBFileDialogService? fileDialogService = null)
    {
        _projectStore = projectStore ?? throw new ArgumentNullException(nameof(projectStore));
        _workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        _templateStore = templateStore ?? new EntryTemplateStore();
        _fileDialogService = fileDialogService;
        RefreshRecentProjects();
    }

    public ObservableCollection<string> RecentProjects { get; } = [];

    public ObservableCollection<OFBExportRule> ExportRules { get; } = [];

    public ObservableCollection<OFBGroupingDecision> GroupingDecisions { get; } = [];

    public ObservableCollection<OFBGroupingCandidate> GroupingCandidates { get; } = [];

    public ObservableCollection<string> GroupingSummaries { get; } = [];

    public ObservableCollection<string> GroupingChoices { get; } = [];

    public ObservableCollection<OFBPersonPrivacyPreview> PrivacyPeople { get; } = [];

    public ObservableCollection<string> Diagnostics { get; } = [];

    public string[] RuleTargetKinds { get; } = ["person", "family", "fact"];

    public string[] RuleActions { get; } = ["include", "exclude", "replace", "redact", "generalize"];

    public bool IsNotBusy => !IsBusy;

    public string DefaultProjectPath => string.IsNullOrWhiteSpace(ProjectFilePath)
        ? Path.Combine(GetDefaultProjectDirectory(), GetSafeFileName(Name) + OFBProjectStore.ProjectExtension)
        : ProjectFilePath;

    public string DefaultOutputPath => Path.Combine(
        string.IsNullOrWhiteSpace(ProjectFilePath)
            ? GetDefaultProjectDirectory()
            : Path.GetDirectoryName(Path.GetFullPath(ProjectFilePath)) ?? GetDefaultProjectDirectory(),
        GetSafeFileName(Name) + ".docx");

    partial void OnSelectedRuleChanged(OFBExportRule? value)
    {
        if (value is null)
            return;
        RuleTargetKind = value.TargetKind;
        RuleTargetId = value.TargetId;
        RuleAction = value.Action;
        RuleField = value.Field ?? string.Empty;
        RuleValue = value.Value ?? string.Empty;
        RuleOccurrence = value.Occurrence;
    }

    [RelayCommand]
    private async Task BrowseProjectAsync()
    {
        if (_fileDialogService is null)
            return;

        var path = await _fileDialogService.OpenProjectAsync(ProjectFilePath).ConfigureAwait(true);
        if (string.IsNullOrWhiteSpace(path))
            return;

        ProjectFilePath = path;
        await OpenProjectAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private async Task SaveProjectAsAsync()
    {
        if (_fileDialogService is null)
            return;

        var path = await _fileDialogService.SaveProjectAsAsync(DefaultProjectPath).ConfigureAwait(true);
        if (string.IsNullOrWhiteSpace(path))
            return;

        ProjectFilePath = path;
        if (TrySaveProject())
            StatusMessage = $"Saved project '{ProjectFilePath}'.";
    }

    [RelayCommand]
    private async Task SelectProjectFolderAsync()
    {
        if (_fileDialogService is null)
            return;

        var directory = await _fileDialogService.PickDirectoryAsync(
            string.IsNullOrWhiteSpace(ProjectFilePath) ? GetDefaultProjectDirectory() : ProjectFilePath)
            .ConfigureAwait(true);
        if (!string.IsNullOrWhiteSpace(directory))
            ProjectFilePath = Path.Combine(directory, GetSafeFileName(Name) + OFBProjectStore.ProjectExtension);
    }

    [RelayCommand]
    private async Task OpenGedcomFileAsync()
    {
        if (_fileDialogService is null)
            return;

        var path = await _fileDialogService.OpenGedcomAsync(GetResolvedInputPath()).ConfigureAwait(true);
        if (!string.IsNullOrWhiteSpace(path))
        {
            InputPath = path;
            await RefreshGroupingCandidatesAsync(CancellationToken.None).ConfigureAwait(true);
        }
    }

    [RelayCommand]
    private async Task SaveDocxAsAsync()
    {
        if (_fileDialogService is null)
            return;

        var path = await _fileDialogService.SaveDocxAsAsync(GetResolvedOutputPath()).ConfigureAwait(true);
        if (!string.IsNullOrWhiteSpace(path))
            OutputPath = path;
    }

    [RelayCommand]
    private async Task SelectOutputFolderAsync()
    {
        if (_fileDialogService is null)
            return;

        var directory = await _fileDialogService.PickDirectoryAsync(GetResolvedOutputPath()).ConfigureAwait(true);
        if (!string.IsNullOrWhiteSpace(directory))
            OutputPath = Path.Combine(directory, GetSafeFileName(Name) + ".docx");
    }

    [RelayCommand]
    private void NewProject()
    {
        _loadedProject = null;
        ProjectFilePath = string.Empty;
        Name = "New OFB";
        Title = "New OFB";
        InputPath = string.Empty;
        OutputPath = string.Empty;
        EntryTemplate = "gc";
        PlaceId = null;
        IncludeDescendants = false;
        Preface = null;
        Legend = null;
        AutoAcceptThreshold = 85;
        ExportRules.Clear();
        GroupingDecisions.Clear();
        GroupingCandidates.Clear();
        GroupingSummaries.Clear();
        PrivacyPeople.Clear();
        Diagnostics.Clear();
        StatusMessage = "New project. Choose a .ofbproject path before saving.";
    }

    private string GetResolvedInputPath() => string.IsNullOrWhiteSpace(InputPath)
        ? string.IsNullOrWhiteSpace(ProjectFilePath) ? GetDefaultProjectDirectory() : Path.GetDirectoryName(Path.GetFullPath(ProjectFilePath)) ?? GetDefaultProjectDirectory()
        : ResolveProjectRelativePath(InputPath);

    private string GetResolvedOutputPath() => string.IsNullOrWhiteSpace(OutputPath)
        ? DefaultOutputPath
        : ResolveProjectRelativePath(OutputPath);

    private string ResolveProjectRelativePath(string path)
    {
        if (Path.IsPathRooted(path))
            return Path.GetFullPath(path);

        var projectDirectory = string.IsNullOrWhiteSpace(ProjectFilePath)
            ? GetDefaultProjectDirectory()
            : Path.GetDirectoryName(Path.GetFullPath(ProjectFilePath)) ?? GetDefaultProjectDirectory();
        return Path.GetFullPath(Path.Combine(projectDirectory, path));
    }

    private static string GetDefaultProjectDirectory()
    {
        var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (string.IsNullOrWhiteSpace(documentsDirectory))
            documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(documentsDirectory, "OFBCreator");
    }

    private static string GetSafeFileName(string? fileName)
    {
        var candidate = string.IsNullOrWhiteSpace(fileName) ? "New OFB" : fileName;
        foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            candidate = candidate.Replace(invalidCharacter, '_');
        return candidate;
    }

    [RelayCommand]
    private async Task OpenProjectAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectFilePath))
        {
            StatusMessage = "Enter a project path first.";
            return;
        }

        try
        {
            var loaded = _projectStore.Open(ProjectFilePath);
            ProjectFilePath = loaded.ProjectPath;
            LoadProject(loaded.Project);
            Diagnostics.Clear();
            if (loaded.RequiresMigrationSave)
                Diagnostics.Add(
                    $"Project schema {loaded.SourceSchemaVersion} is loaded in memory; save it to write the current schema.");
            StatusMessage = $"Opened project '{Name}'.";
            RefreshRecentProjects();
            if (!string.IsNullOrWhiteSpace(InputPath) && File.Exists(GetResolvedInputPath()))
                await RefreshGroupingCandidatesAsync(CancellationToken.None).ConfigureAwait(true);
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (IOException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (ArgumentException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    [RelayCommand]
    private async Task OpenRecentProjectAsync(string path)
    {
        ProjectFilePath = path;
        await OpenProjectAsync().ConfigureAwait(true);
    }

    [RelayCommand]
    private void SaveProject()
    {
        if (TrySaveProject())
            StatusMessage = $"Saved project '{ProjectFilePath}'.";
    }

    [RelayCommand]
    private void AddRule()
    {
        var isDecisionRule = RuleTargetKind != "fact" && (RuleAction is "include" or "exclude");
        var rule = new OFBExportRule
        {
            Order = ExportRules.Count == 0 ? 1 : ExportRules.Max(existing => existing.Order) + 1,
            TargetKind = RuleTargetKind,
            TargetId = RuleTargetId.Trim(),
            Action = RuleAction,
            Field = isDecisionRule ? null : NullIfWhiteSpace(RuleField),
            Value = isDecisionRule || RuleTargetKind == "fact" || RuleAction == "redact"
                ? null
                : NullIfWhiteSpace(RuleValue),
            Occurrence = RuleTargetKind == "fact" ? RuleOccurrence : null
        };

        try
        {
            OFBExportRuleValidator.Validate(ExportRules.Append(rule).ToArray());
            ExportRules.Add(rule);
            SelectedRule = rule;
            StatusMessage = "Export rule added. Save the project to persist it.";
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    [RelayCommand]
    private void RemoveRule()
    {
        if (SelectedRule is null)
            return;
        ExportRules.Remove(SelectedRule);
        SelectedRule = null;
        StatusMessage = "Export rule removed. Save the project to persist the change.";
    }

    [RelayCommand]
    private void UpdateSelectedRule()
    {
        if (SelectedRule is null)
            return;
        var isDecisionRule = RuleTargetKind != "fact" && (RuleAction is "include" or "exclude");
        var replacement = new OFBExportRule
        {
            Id = SelectedRule.Id,
            Order = SelectedRule.Order,
            Enabled = SelectedRule.Enabled,
            TargetKind = RuleTargetKind,
            TargetId = RuleTargetId.Trim(),
            Action = RuleAction,
            Field = isDecisionRule ? null : NullIfWhiteSpace(RuleField),
            Value = isDecisionRule || RuleTargetKind == "fact" || RuleAction == "redact"
                ? null
                : NullIfWhiteSpace(RuleValue),
            Occurrence = RuleTargetKind == "fact" ? RuleOccurrence : null
        };

        var updatedRules = ExportRules.Select(rule => ReferenceEquals(rule, SelectedRule) ? replacement : rule).ToArray();
        try
        {
            OFBExportRuleValidator.Validate(updatedRules);
            var index = ExportRules.IndexOf(SelectedRule);
            ExportRules[index] = replacement;
            SelectedRule = replacement;
            StatusMessage = "Selected rule updated. Save the project to persist the change.";
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    [RelayCommand]
    private void MoveSelectedRuleUp() => MoveSelectedRule(-1);

    [RelayCommand]
    private void MoveSelectedRuleDown() => MoveSelectedRule(1);

    [RelayCommand]
    private void ValidateTemplate()
    {
        try
        {
            var templatePath = string.IsNullOrWhiteSpace(ProjectFilePath)
                ? EntryTemplate
                : OFBProjectStore.ResolveEntryTemplate(ProjectFilePath, EntryTemplate);
            var template = _templateStore.Load(templatePath);
            StatusMessage = $"Template '{template.Id}' is valid for {template.EntryRoot} entries.";
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (IOException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (ArgumentException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task RefreshGroupingCandidatesAsync(CancellationToken cancellationToken)
    {
        if (!TryBuildProject(out var project, out var projectPath))
            return;

        IsBusy = true;
        StatusMessage = "Loading privacy and grouping preview...";
        GroupingCandidates.Clear();
        GroupingSummaries.Clear();
        PrivacyPeople.Clear();
        GroupingChoices.Clear();
        _groupingTargetBySurname.Clear();
        try
        {
            var result = await _workspaceService.PreviewWorkspaceAsync(project, projectPath, cancellationToken)
                .ConfigureAwait(true);
            var grouping = result.Grouping;
            foreach (var group in grouping.Groups.OrderBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase))
            {
                var surnames = group.Value
                    .Select(OFBFamilyGroupingService.SelectFamilySurname)
                    .Where(surname => !string.IsNullOrWhiteSpace(surname))
                    .Distinct(StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(surname => surname, StringComparer.CurrentCultureIgnoreCase);
                GroupingSummaries.Add($"{group.Key} ({group.Value.Count} families: {string.Join(", ", surnames)})");
                foreach (var surname in surnames)
                {
                    var target = group.Value
                        .Where(family => string.Equals(
                            OFBFamilyGroupingService.SelectFamilySurname(family), surname, StringComparison.OrdinalIgnoreCase))
                        .Where(family => !string.IsNullOrWhiteSpace(family.FamilyRefID))
                        .OrderBy(family => family.FamilyRefID, StringComparer.Ordinal)
                        .Select(family => OFBExportRuleTarget.Family("gedcom", family.FamilyRefID!))
                        .FirstOrDefault();
                    if (target is not null)
                        _groupingTargetBySurname[surname] = target;
                }
            }
            GroupingChoices.Clear();
            foreach (var surname in _groupingTargetBySurname.Keys.OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase))
                GroupingChoices.Add(surname);
            if (!GroupingChoices.Contains(LeftGroupingChoice))
                LeftGroupingChoice = GroupingChoices.FirstOrDefault();
            if (!GroupingChoices.Contains(RightGroupingChoice) || string.Equals(LeftGroupingChoice, RightGroupingChoice, StringComparison.OrdinalIgnoreCase))
                RightGroupingChoice = GroupingChoices.FirstOrDefault(surname => !string.Equals(surname, LeftGroupingChoice, StringComparison.OrdinalIgnoreCase));
            foreach (var person in result.People.OrderBy(person => person.DisplayName, StringComparer.CurrentCultureIgnoreCase))
                PrivacyPeople.Add(person);
            foreach (var candidate in grouping.Candidates)
                GroupingCandidates.Add(candidate);
            Diagnostics.Clear();
            foreach (var diagnostic in grouping.Diagnostics)
                Diagnostics.Add($"{diagnostic.Code}: {diagnostic.Message}");
            var familyCount = grouping.Groups.Values.Sum(group => group.Count);
            StatusMessage =
                $"{PrivacyPeople.Count} people pass the filters; {familyCount} family entries in {grouping.Groups.Count} groups; {GroupingCandidates.Count} merge candidate(s).";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            StatusMessage = "Privacy and grouping preview was canceled.";
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (FileNotFoundException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (IOException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (NotSupportedException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (ArgumentException exception)
        {
            StatusMessage = exception.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void AcceptCandidate(OFBGroupingCandidate candidate) =>
        SaveGroupingDecision(candidate, "acceptMerge", null);

    [RelayCommand]
    private void RejectCandidate(OFBGroupingCandidate candidate) =>
        SaveGroupingDecision(candidate, "rejectMerge", null);

    [RelayCommand]
    private void ManualMergeCandidate(OFBGroupingCandidate candidate)
    {
        if (string.IsNullOrWhiteSpace(ManualGroupName))
        {
            StatusMessage = "Enter a manual group label before merging.";
            return;
        }
        SaveGroupingDecision(candidate, "manualMerge", ManualGroupName.Trim());
    }

    [RelayCommand]
    private void ManualMergeSelected()
    {
        if (string.IsNullOrWhiteSpace(LeftGroupingChoice)
            || string.IsNullOrWhiteSpace(RightGroupingChoice)
            || string.Equals(LeftGroupingChoice, RightGroupingChoice, StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "Select two different family groups before merging.";
            return;
        }

        if (!_groupingTargetBySurname.TryGetValue(LeftGroupingChoice, out var leftTarget)
            || !_groupingTargetBySurname.TryGetValue(RightGroupingChoice, out var rightTarget))
        {
            StatusMessage = "Refresh the grouping preview before creating a manual merge.";
            return;
        }

        SaveGroupingDecision(
            leftTarget,
            rightTarget,
            "manualMerge",
            string.IsNullOrWhiteSpace(ManualGroupName) ? LeftGroupingChoice : ManualGroupName.Trim(),
            LeftGroupingChoice,
            RightGroupingChoice);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task ExportAsync(CancellationToken cancellationToken)
    {
        if (!TrySaveProject() || _loadedProject is null)
            return;

        IsBusy = true;
        StatusMessage = "Exporting DOCX...";
        try
        {
            await _workspaceService.ExportAsync(_loadedProject, ProjectFilePath, cancellationToken)
                .ConfigureAwait(true);
            StatusMessage = $"Exported '{OutputPath}'.";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            StatusMessage = "DOCX export was canceled.";
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (IOException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (InvalidOperationException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (NotSupportedException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (ArgumentException exception)
        {
            StatusMessage = exception.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool TrySaveProject()
    {
        if (string.IsNullOrWhiteSpace(ProjectFilePath))
        {
            StatusMessage = "Enter a destination path ending in .ofbproject.";
            return false;
        }

        var project = BuildProject();
        try
        {
            ProjectFilePath = _projectStore.Save(project, ProjectFilePath);
            _loadedProject = project;
            RefreshRecentProjects();
            return true;
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (IOException exception)
        {
            StatusMessage = exception.Message;
        }
        catch (ArgumentException exception)
        {
            StatusMessage = exception.Message;
        }
        return false;
    }

    private bool TryBuildProject(out OFBProject project, out string projectPath)
    {
        project = BuildProject();
        projectPath = string.IsNullOrWhiteSpace(ProjectFilePath) ? DefaultProjectPath : ProjectFilePath;
        try
        {
            OFBExportRuleValidator.Validate(project.ExportRules);
            OFBGroupingPolicyValidator.Validate(project.GroupingPolicy, project.GroupingDecisions);
            return true;
        }
        catch (InvalidDataException exception)
        {
            StatusMessage = exception.Message;
            return false;
        }
    }

    private OFBProject BuildProject()
    {
        var project = _loadedProject ?? new OFBProject();
        project.SchemaVersion = OFBProject.CurrentSchemaVersion;
        project.Name = Name;
        project.Title = Title;
        project.InputPath = NullIfWhiteSpace(InputPath);
        project.OutputPath = NullIfWhiteSpace(OutputPath);
        project.EntryTemplate = EntryTemplate;
        project.PlaceId = NullIfWhiteSpace(PlaceId);
        project.IncludeDescendants = IncludeDescendants;
        project.Preface = NullIfWhiteSpace(Preface);
        project.Legend = NullIfWhiteSpace(Legend);
        project.ExportRules = ExportRules.ToList();
        project.GroupingPolicy = new OFBGroupingPolicy { AutoAcceptThreshold = AutoAcceptThreshold };
        project.GroupingDecisions = GroupingDecisions.ToList();
        return project;
    }

    private void LoadProject(OFBProject project)
    {
        _loadedProject = project;
        Name = project.Name;
        Title = project.Title;
        InputPath = project.InputPath ?? string.Empty;
        OutputPath = project.OutputPath ?? string.Empty;
        EntryTemplate = project.EntryTemplate;
        PlaceId = project.PlaceId;
        IncludeDescendants = project.IncludeDescendants;
        Preface = project.Preface;
        Legend = project.Legend;
        AutoAcceptThreshold = project.GroupingPolicy.AutoAcceptThreshold;
        ExportRules.Clear();
        foreach (var rule in project.ExportRules)
            ExportRules.Add(rule);
        GroupingDecisions.Clear();
        foreach (var decision in project.GroupingDecisions)
            GroupingDecisions.Add(decision);
        GroupingCandidates.Clear();
        PrivacyPeople.Clear();
        GroupingSummaries.Clear();
        GroupingChoices.Clear();
        _groupingTargetBySurname.Clear();
    }

    private void SaveGroupingDecision(OFBGroupingCandidate candidate, string action, string? groupName)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        if (candidate.LeftFamilyTargetId is null || candidate.RightFamilyTargetId is null)
        {
            StatusMessage = "This candidate cannot be persisted because stable family identifiers are missing.";
            return;
        }

        SaveGroupingDecision(
            candidate.LeftFamilyTargetId,
            candidate.RightFamilyTargetId,
            action,
            groupName,
            candidate.LeftSurname,
            candidate.RightSurname);
    }

    private void SaveGroupingDecision(
        string leftFamilyTargetId,
        string rightFamilyTargetId,
        string action,
        string? groupName,
        string leftLabel,
        string rightLabel)
    {
        var existing = GroupingDecisions.FirstOrDefault(decision =>
            SamePair(decision.LeftFamilyTargetId, decision.RightFamilyTargetId,
                leftFamilyTargetId, rightFamilyTargetId));
        var order = existing?.Order
            ?? (GroupingDecisions.Count == 0 ? 1 : GroupingDecisions.Max(decision => decision.Order) + 1);
        if (existing is not null)
            GroupingDecisions.Remove(existing);
        GroupingDecisions.Add(new OFBGroupingDecision
        {
            Order = order,
            LeftFamilyTargetId = leftFamilyTargetId,
            RightFamilyTargetId = rightFamilyTargetId,
            Action = action,
            GroupName = groupName
        });
        StatusMessage = $"Saved '{action}' decision for {leftLabel} / {rightLabel}. Save the project to persist it.";
    }

    private void MoveSelectedRule(int offset)
    {
        if (SelectedRule is null)
            return;
        var selectedIndex = ExportRules.IndexOf(SelectedRule);
        var targetIndex = selectedIndex + offset;
        if (selectedIndex < 0 || targetIndex < 0 || targetIndex >= ExportRules.Count)
            return;

        var orderedRules = ExportRules.ToList();
        (orderedRules[selectedIndex], orderedRules[targetIndex]) =
            (orderedRules[targetIndex], orderedRules[selectedIndex]);
        var selectedId = SelectedRule.Id;
        ExportRules.Clear();
        OFBExportRule? movedSelection = null;
        for (var index = 0; index < orderedRules.Count; index++)
        {
            var rule = CopyRule(orderedRules[index], index + 1);
            ExportRules.Add(rule);
            if (string.Equals(rule.Id, selectedId, StringComparison.Ordinal))
                movedSelection = rule;
        }
        SelectedRule = movedSelection;
        StatusMessage = "Rule order changed. Save the project to persist it.";
    }

    private static OFBExportRule CopyRule(OFBExportRule rule, int order) => new()
    {
        Id = rule.Id,
        Order = order,
        Enabled = rule.Enabled,
        TargetKind = rule.TargetKind,
        TargetId = rule.TargetId,
        Action = rule.Action,
        Field = rule.Field,
        Value = rule.Value,
        Occurrence = rule.Occurrence
    };

    private void RefreshRecentProjects()
    {
        RecentProjects.Clear();
        foreach (var path in _projectStore.GetRecentProjects())
            RecentProjects.Add(path);
    }

    private static bool SamePair(string firstLeft, string firstRight, string secondLeft, string secondRight) =>
        string.Equals(firstLeft, secondLeft, StringComparison.Ordinal)
        && string.Equals(firstRight, secondRight, StringComparison.Ordinal)
        || string.Equals(firstLeft, secondRight, StringComparison.Ordinal)
        && string.Equals(firstRight, secondLeft, StringComparison.Ordinal);

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
