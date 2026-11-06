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
        bool supportsUpdateSourceTrigger = false)
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
        HasUnsupportedBindingExpression = expression?.TrimStart().StartsWith('{') == true
            && MemberName.Length == 0;
    }

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
        bindingMode = AxamlBindingMode.Default;
        updateSourceTrigger = AxamlBindingUpdateSourceTrigger.Default;
        if (string.IsNullOrWhiteSpace(expression))
        {
            return null;
        }

        string value = expression.Trim();
        const string extensionName = "{Binding";
        if (!value.StartsWith(extensionName, StringComparison.Ordinal)
            || !value.EndsWith('}')
            || value.Length > extensionName.Length
                && !char.IsWhiteSpace(value[extensionName.Length])
                && value[extensionName.Length] != ',')
        {
            return null;
        }

        string[] tokens = value[extensionName.Length..^1]
            .Split(',')
            .Select(static token => token.Trim())
            .ToArray();
        string? path = null;
        bool modeSpecified = false;
        bool updateSourceTriggerSpecified = false;
        foreach (string token in tokens)
        {
            if (token.Length == 0)
            {
                continue;
            }

            int separator = token.IndexOf('=');
            if (separator < 0)
            {
                if (path is not null || !AxamlMvvmMemberName.IsValid(token))
                {
                    return null;
                }

                path = token;
                continue;
            }

            string optionName = token[..separator].Trim();
            string optionValue = token[(separator + 1)..].Trim();
            if (string.Equals(optionName, "Path", StringComparison.Ordinal))
            {
                if (path is not null || !AxamlMvvmMemberName.IsValid(optionValue))
                {
                    return null;
                }

                path = optionValue;
            }
            else if (string.Equals(optionName, "Mode", StringComparison.Ordinal))
            {
                if (modeSpecified
                    || !Enum.TryParse(optionValue, ignoreCase: true, out AxamlBindingMode parsedMode)
                    || !Enum.IsDefined(parsedMode))
                {
                    return null;
                }

                bindingMode = parsedMode;
                modeSpecified = true;
            }
            else if (string.Equals(optionName, "UpdateSourceTrigger", StringComparison.Ordinal))
            {
                if (updateSourceTriggerSpecified
                    || !Enum.TryParse(
                        optionValue,
                        ignoreCase: true,
                        out AxamlBindingUpdateSourceTrigger parsedTrigger)
                    || !Enum.IsDefined(parsedTrigger))
                {
                    return null;
                }

                updateSourceTrigger = parsedTrigger;
                updateSourceTriggerSpecified = true;
            }
            else
            {
                return null;
            }
        }

        return path;
    }

    partial void OnDiagnosticChanged(string? value)
    {
        OnPropertyChanged(nameof(HasDiagnostic));
    }
}
