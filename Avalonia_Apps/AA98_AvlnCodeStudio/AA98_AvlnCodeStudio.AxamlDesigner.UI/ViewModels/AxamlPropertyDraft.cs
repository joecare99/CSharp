using CommunityToolkit.Mvvm.ComponentModel;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public sealed partial class AxamlPropertyDraft : ObservableObject
{
    internal AxamlPropertyDraft(
        string targetKey,
        string target,
        string controlType,
        string propertyName,
        string value,
        string sourceMarkup)
    {
        TargetKey = targetKey;
        Target = target;
        ControlType = controlType;
        PropertyName = propertyName;
        Value = value;
        SourceMarkup = sourceMarkup;
    }

    internal string TargetKey { get; }

    internal string ControlType { get; }

    internal string SourceMarkup { get; }

    public string Target { get; }

    public string PropertyName { get; }

    public string Value { get; }

    [ObservableProperty]
    private bool _canRestore;

    [ObservableProperty]
    private string _status = string.Empty;
}
