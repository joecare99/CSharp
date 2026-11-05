using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
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
        bool isCommand = false)
    {
        ControlType = controlType;
        PropertyName = propertyName;
        MemberTypeName = memberTypeName;
        IsCommand = isCommand;
        MemberName = ReadBindingPath(expression) ?? string.Empty;
    }

    public string ControlType { get; }

    public string PropertyName { get; }

    public string MemberTypeName { get; }

    public bool IsCommand { get; }

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
        AvailableMembers.Clear();
        foreach (string member in members)
        {
            AvailableMembers.Add(member);
        }

        MemberSources.Clear();
        if (AvailableMembers.Count > 0)
        {
            MemberSources.Add(ExistingMemberSource);
            MemberSources.Add(NewMemberSource);
        }
        else
        {
            MemberSources.Add(NewMemberSource);
            MemberSource = NewMemberSource;
        }

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
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        string value = expression.Trim();
        const string prefix = "{Binding ";
        if (!value.StartsWith(prefix, StringComparison.Ordinal)
            || !value.EndsWith('}'))
        {
            return null;
        }

        string path = value[prefix.Length..^1].Trim();
        return AxamlMvvmMemberName.IsValid(path) ? path : null;
    }

    partial void OnDiagnosticChanged(string? value)
    {
        OnPropertyChanged(nameof(HasDiagnostic));
    }
}
