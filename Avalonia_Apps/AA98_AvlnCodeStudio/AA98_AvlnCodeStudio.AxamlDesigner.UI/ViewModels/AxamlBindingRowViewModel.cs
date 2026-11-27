using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public partial class AxamlBindingRowViewModel : ObservableObject
{
    private const string ExistingMemberSource = "Use existing member";
    private const string NewMemberSource = "Create new member";

    public AxamlBindingRowViewModel(
        string controlType,
        string propertyName,
        string? expression,
        string memberTypeName = "object",
        bool isCommand = false,
        AxamlBindingMode defaultBindingMode = AxamlBindingMode.Default,
        bool supportsUpdateSourceTrigger = false,
        bool hasUnsafeBindingScope = false)
    {
        ControlType = controlType;
        PropertyName = propertyName;
        MemberTypeName = memberTypeName;
        IsCommand = isCommand;
        MemberName = ReadBindingPath(
            expression,
            out AxamlBindingMode parsedMode,
            out AxamlBindingUpdateSourceTrigger parsedUpdateSourceTrigger) ?? string.Empty;
        BindingMode = MemberName.Length == 0 ? defaultBindingMode : parsedMode;
        UpdateSourceTrigger = MemberName.Length == 0
            ? AxamlBindingUpdateSourceTrigger.Default
            : parsedUpdateSourceTrigger;
        SupportsUpdateSourceTrigger = supportsUpdateSourceTrigger;
        SourceExpression = expression;
        HasUnsupportedBindingExpression = hasUnsafeBindingScope
            || expression?.TrimStart().StartsWith('{') == true && MemberName.Length == 0;
    }

    internal string? SourceExpression { get; }

    public string ControlType { get; }

    public string PropertyName { get; }

    public string MemberTypeName { get; }

    public bool IsCommand { get; }

    public bool SupportsBindingMode => !IsCommand;

    public bool SupportsUpdateSourceTrigger { get; }

    public bool CanEditBinding => !HasUnsupportedBindingExpression;

    public bool HasUnsupportedBindingExpression { get; }

    public string? BindingExpressionDiagnostic => HasUnsupportedBindingExpression
        ? "This binding uses a path or option not yet supported by the safe binding editor; it is preserved unchanged."
        : null;

    public AxamlBindingMode[] BindingModes { get; } =
    [
        AxamlBindingMode.Default,
        AxamlBindingMode.OneTime,
        AxamlBindingMode.OneWay,
        AxamlBindingMode.OneWayToSource,
        AxamlBindingMode.TwoWay,
    ];

    public AxamlBindingUpdateSourceTrigger[] UpdateSourceTriggers { get; } =
    [
        AxamlBindingUpdateSourceTrigger.Default,
        AxamlBindingUpdateSourceTrigger.PropertyChanged,
        AxamlBindingUpdateSourceTrigger.LostFocus,
        AxamlBindingUpdateSourceTrigger.Explicit,
        AxamlBindingUpdateSourceTrigger.DataValidation,
    ];

    public string MemberKindLabel => IsCommand
        ? $"Command ({MemberTypeName})"
        : $"Property ({MemberTypeName})";

    public string NewMemberPlaceholder => IsCommand
        ? "New command member name"
        : $"New {MemberTypeName} property name";

    public ObservableCollection<string> AvailableMembers { get; } = [];

    public ObservableCollection<string> MemberSources { get; } = [];

    [ObservableProperty]
    private string _memberName;

    [ObservableProperty]
    private string? _selectedMember;

    [ObservableProperty]
    private string _newMemberName = string.Empty;

    [ObservableProperty]
    private string _memberSource = NewMemberSource;

    [ObservableProperty]
    private AxamlBindingMode _bindingMode;

    [ObservableProperty]
    private AxamlBindingUpdateSourceTrigger _updateSourceTrigger;

    [ObservableProperty]
    private string? _diagnostic;

    public bool HasDiagnostic => !string.IsNullOrWhiteSpace(Diagnostic);

    public bool IsAvailable => AvailableMembers.Count > 0;

    public bool IsCreatingNewMember =>
        string.Equals(MemberSource, NewMemberSource, StringComparison.Ordinal);

    public bool IsSelectingExistingMember =>
        string.Equals(MemberSource, ExistingMemberSource, StringComparison.Ordinal);

    public string? GetRequestedMember()
    {
        string? memberName = IsSelectingExistingMember
            ? SelectedMember
            : IsCreatingNewMember
                ? NewMemberName.Trim()
                : null;
        if (string.IsNullOrWhiteSpace(memberName))
        {
            Diagnostic = IsSelectingExistingMember
                ? "Select an existing ViewModel member."
                : "Enter a new ViewModel member name.";
            return null;
        }

        if (!AxamlMvvmMemberName.IsValid(memberName))
        {
            Diagnostic = "Use a simple C# member name without a binding path or expression.";
            return null;
        }

        Diagnostic = null;
        return memberName;
    }

    partial void OnNewMemberNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            MemberSource = NewMemberSource;
        }
    }

    partial void OnSelectedMemberChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            MemberSource = ExistingMemberSource;
        }
    }

    partial void OnMemberSourceChanged(string value)
    {
        OnPropertyChanged(nameof(IsCreatingNewMember));
        OnPropertyChanged(nameof(IsSelectingExistingMember));
    }

    public void SetAvailableMembers(System.Collections.Generic.IEnumerable<string> members)
    {
        ArgumentNullException.ThrowIfNull(members);
        string[] updatedMembers = members.ToArray();
        string? selectedMember = SelectedMember;
        string memberSource = MemberSource;
        if (!AvailableMembers.SequenceEqual(updatedMembers, StringComparer.Ordinal))
        {
            AvailableMembers.Clear();
            foreach (string member in updatedMembers)
            {
                AvailableMembers.Add(member);
            }
        }

        string[] sources = updatedMembers.Length > 0
            ? [ExistingMemberSource, NewMemberSource]
            : [NewMemberSource];
        if (!MemberSources.SequenceEqual(sources, StringComparer.Ordinal))
        {
            MemberSources.Clear();
            foreach (string source in sources)
            {
                MemberSources.Add(source);
            }
        }

        SelectedMember = updatedMembers.Contains(selectedMember, StringComparer.Ordinal) ? selectedMember : null;
        MemberSource = sources.Contains(memberSource, StringComparer.Ordinal) ? memberSource : NewMemberSource;

        OnPropertyChanged(nameof(IsAvailable));
    }

    public void SelectExistingMemberSource()
    {
        if (AvailableMembers.Count > 0)
        {
            MemberSource = ExistingMemberSource;
        }
    }

    public static string? ReadBindingPath(string? expression)
    {
        return ReadBindingPath(expression, out _);
    }

    public static string? ReadBindingPath(string? expression, out AxamlBindingMode bindingMode)
    {
        return ReadBindingPath(expression, out bindingMode, out _);
    }

    public static string? ReadBindingPath(
        string? expression,
        out AxamlBindingMode bindingMode,
        out AxamlBindingUpdateSourceTrigger updateSourceTrigger)
    {
        return AxamlEditableBinding.ReadPath(expression, out bindingMode, out updateSourceTrigger);
    }
    partial void OnDiagnosticChanged(string? value)
    {
        OnPropertyChanged(nameof(HasDiagnostic));
    }
}
