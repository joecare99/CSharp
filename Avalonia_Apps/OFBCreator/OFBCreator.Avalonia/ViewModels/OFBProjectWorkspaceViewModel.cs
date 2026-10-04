using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OFBCreator.Avalonia.Services;
using OFBCreator.Core.Models;
using OFBCreator.Console.Services.Templates;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Avalonia.ViewModels;

public sealed partial class OFBProjectWorkspaceViewModel : ObservableObject
{
    private readonly OFBProjectStore _projectStore;
    private readonly IOFBWorkspaceService _workspaceService;
    private readonly EntryTemplateStore _templateStore;
    private OFBProject? _loadedProject;

    [ObservableProperty]
    private string _projectFilePath = string.Empty;

    [ObservableProperty]
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

    public OFBProjectWorkspaceViewModel(
        OFBProjectStore projectStore,
        IOFBWorkspaceService workspaceService,
        EntryTemplateStore? templateStore = null)
    {
        _projectStore = projectStore ?? throw new ArgumentNullException(nameof(projectStore));
        _workspaceService = workspaceService ?? throw new ArgumentNullException(nameof(workspaceService));
        _templateStore = templateStore ?? new EntryTemplateStore();
        RefreshRecentProjects();
    }

    public ObservableCollection<string> RecentProjects { get; } = [];

    public ObservableCollection<OFBExportRule> ExportRules { get; } = [];

    public ObservableCollection<OFBGroupingDecision> GroupingDecisions { get; } = [];

    public ObservableCollection<OFBGroupingCandidate> GroupingCandidates { get; } = [];

    public ObservableCollection<string> Diagnostics { get; } = [];

    public string[] RuleTargetKinds { get; } = ["person", "family", "fact"];

    public string[] RuleActions { get; } = ["include", "exclude", "replace", "redact", "generalize"];

    public bool IsNotBusy => !IsBusy;

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
        Diagnostics.Clear();
        StatusMessage = "New project. Choose a .ofbproject path before saving.";
    }

    [RelayCommand]
    private void OpenProject()
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
    private void OpenRecentProject(string path)
    {
        ProjectFilePath = path;
        OpenProject();
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
        StatusMessage = "Loading grouping evidence...";
        try
        {
            var result = await _workspaceService.PreviewGroupingAsync(project, projectPath, cancellationToken)
                .ConfigureAwait(true);
            GroupingCandidates.Clear();
            foreach (var candidate in result.Candidates)
                GroupingCandidates.Add(candidate);
            Diagnostics.Clear();
            foreach (var diagnostic in result.Diagnostics)
                Diagnostics.Add($"{diagnostic.Code}: {diagnostic.Message}");
            var familyCount = result.Groups.Values.Sum(group => group.Count);
            StatusMessage =
                $"Previewed {familyCount} family entries in {result.Groups.Count} groups; {GroupingCandidates.Count} merge candidate(s).";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            StatusMessage = "Grouping review was canceled.";
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
        projectPath = ProjectFilePath;
        if (string.IsNullOrWhiteSpace(projectPath))
        {
            StatusMessage = "Save the project before previewing source-dependent grouping.";
            return false;
        }
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
    }

    private void SaveGroupingDecision(OFBGroupingCandidate candidate, string action, string? groupName)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        if (candidate.LeftFamilyTargetId is null || candidate.RightFamilyTargetId is null)
        {
            StatusMessage = "This candidate cannot be persisted because stable family identifiers are missing.";
            return;
        }

        var existing = GroupingDecisions.FirstOrDefault(decision =>
            SamePair(decision.LeftFamilyTargetId, decision.RightFamilyTargetId,
                candidate.LeftFamilyTargetId, candidate.RightFamilyTargetId));
        var order = existing?.Order
            ?? (GroupingDecisions.Count == 0 ? 1 : GroupingDecisions.Max(decision => decision.Order) + 1);
        if (existing is not null)
            GroupingDecisions.Remove(existing);
        GroupingDecisions.Add(new OFBGroupingDecision
        {
            Order = order,
            LeftFamilyTargetId = candidate.LeftFamilyTargetId,
            RightFamilyTargetId = candidate.RightFamilyTargetId,
            Action = action,
            GroupName = groupName
        });
        StatusMessage = $"Saved '{action}' decision for {candidate.LeftSurname} / {candidate.RightSurname}. Save the project to persist it.";
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
