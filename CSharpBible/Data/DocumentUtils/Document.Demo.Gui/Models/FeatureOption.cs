using CommunityToolkit.Mvvm.ComponentModel;

namespace Document.Demo.Gui.Models;

public partial class FeatureOption : ObservableObject
{
    public FeatureOption(DemoFeature feature, string displayName, bool isSelected)
    {
        Feature = feature;
        DisplayName = displayName;
        IsSelected = isSelected;
    }

    public DemoFeature Feature { get; }

    public string DisplayName { get; }

    [ObservableProperty]
    private bool _isSelected;
}
