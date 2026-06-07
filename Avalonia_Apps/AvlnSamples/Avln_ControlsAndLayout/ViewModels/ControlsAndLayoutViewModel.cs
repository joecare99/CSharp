using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ViewModels;
using Avln_ControlsAndLayout.Models;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace Avln_ControlsAndLayout.ViewModels;

/// <summary>View model for the Avalonia controls and layout sample browser.</summary>
public partial class ControlsAndLayoutViewModel : BaseViewModelCT
{
    private ControlSample? _selectedSample;
    private string _editableAxamlText = string.Empty;
    private string _errorText = string.Empty;
    private Control _currentPreview = new TextBlock { Text = "No sample selected." };
    private GridLength _previewRowHeight = new(1, GridUnitType.Star);
    private GridLength _codeRowHeight = new(1, GridUnitType.Star);

    public ControlsAndLayoutViewModel()
    {
        SampleGroups = new(CreateGroups());
        SelectedSample = SampleGroups[0].Samples[0];
    }

    public ObservableCollection<ControlSampleGroup> SampleGroups { get; }

    public ControlSample? SelectedSample
    {
        get => _selectedSample;
        set
        {
            if (SetProperty(ref _selectedSample, value, true))
            {
                OnPropertyChanged(nameof(SampleTitle));
                OnPropertyChanged(nameof(SampleDescription));
                EditableAxamlText = value?.AxamlText ?? string.Empty;
            }
        }
    }

    public string SampleTitle => SelectedSample?.Title ?? string.Empty;

    public string SampleDescription => SelectedSample?.Description ?? string.Empty;

    public string EditableAxamlText
    {
        get => _editableAxamlText;
        set
        {
            if (SetProperty(ref _editableAxamlText, value, true))
            {
                RenderPreviewFromText();
            }
        }
    }

    public string ErrorText
    {
        get => _errorText;
        set => SetProperty(ref _errorText, value, true);
    }

    public Control CurrentPreview
    {
        get => _currentPreview;
        private set => SetProperty(ref _currentPreview, value, true);
    }

    public GridLength PreviewRowHeight
    {
        get => _previewRowHeight;
        set => SetProperty(ref _previewRowHeight, value, true);
    }

    public GridLength CodeRowHeight
    {
        get => _codeRowHeight;
        set => SetProperty(ref _codeRowHeight, value, true);
    }

    public IRelayCommand ShowPreviewCommand => new RelayCommand(ShowPreview);
    public IRelayCommand ShowCodeCommand => new RelayCommand(ShowCode);
    public IRelayCommand ShowSplitCommand => new RelayCommand(ShowSplit);

    private void ShowPreview() { PreviewRowHeight = new GridLength(1, GridUnitType.Star); CodeRowHeight = new GridLength(0); }
    private void ShowCode() { PreviewRowHeight = new GridLength(0); CodeRowHeight = new GridLength(1, GridUnitType.Star); }
    private void ShowSplit() { PreviewRowHeight = new GridLength(1, GridUnitType.Star); CodeRowHeight = new GridLength(1, GridUnitType.Star); }

    private void RenderPreviewFromText()
    {
        if (string.IsNullOrWhiteSpace(EditableAxamlText))
        {
            CurrentPreview = new TextBlock { Text = "Enter AXAML to render a preview." };
            ErrorText = string.Empty;
            return;
        }

        try
        {
            var loaded = AvaloniaRuntimeXamlLoader.Parse(EditableAxamlText);
            CurrentPreview = loaded as Control ?? new ContentControl { Content = loaded };
            ErrorText = string.Empty;
        }
        catch (Exception ex)
        {
            CurrentPreview = new TextBlock { Text = "The current AXAML cannot be rendered." };
            ErrorText = ex.Message;
        }
    }

    private static ControlSampleGroup[] CreateGroups() =>
    [
        new("Layout",
        [
            S("Border", "Border draws a border, background, or both around another element.", SampleCode.Border),
            S("Canvas", "Canvas positions child elements by coordinates.", SampleCode.Canvas),
            S("Grid", "Grid defines a flexible area consisting of columns and rows.", SampleCode.Grid),
            S("DockPanel", "DockPanel arranges child elements relative to each other.", SampleCode.DockPanel),
            S("Viewbox", "Viewbox stretches and scales a single child.", SampleCode.ViewBox),
            S("StackPanel", "StackPanel arranges child elements into one line.", SampleCode.StackPanel),
            S("WrapPanel", "WrapPanel wraps controls as available space changes.", SampleCode.WrapPanel),
            S("Grid columns", "Avalonia Grid gives children equal cells when star-sized rows and columns are used.", SampleCode.UniformGrid),
        ]),
        new("Controls",
        [
            S("Button", "Button represents the standard button component.", SampleCode.Button),
            S("CheckBox", "CheckBox can be selected, cleared, or indeterminate.", SampleCode.CheckBox),
            S("ComboBox", "ComboBox provides a drop-down selection list.", SampleCode.ComboBox),
            S("Expander", "Expander reveals or hides additional content.", SampleCode.Expander),
            S("Image", "Image displays bitmap or icon resources.", SampleCode.Image),
            S("ListBox", "ListBox displays selectable items.", SampleCode.ListBox),
            S("Menu", "Menu hierarchically organizes commands.", SampleCode.Menu),
            S("Password input", "Avalonia uses TextBox with PasswordChar for password entry.", SampleCode.PasswordBox),
            S("RadioButton", "RadioButton presents a mutually exclusive choice.", SampleCode.RadioButton),
            S("ScrollViewer", "ScrollViewer represents a scrollable area.", SampleCode.ScrollViewer),
            S("Slider", "Slider lets users select from a range of values.", SampleCode.Slider),
            S("TabControl", "TabControl arranges visual content in tab pages.", SampleCode.TabControl),
            S("TextBlock", "TextBlock displays text.", SampleCode.TextBlock),
            S("TextBox", "TextBox accepts text input.", SampleCode.TextBox),
            S("ToolTip", "ToolTip displays contextual information on hover.", SampleCode.ToolTip),
        ]),
        new("Avalonia only",
        [
            S("SplitView", "SplitView provides pane-and-content navigation.", SampleCode.SplitView),
            S("ToggleSwitch", "ToggleSwitch is a built-in Avalonia on/off control.", SampleCode.ToggleSwitch),
            S("CalendarDatePicker", "CalendarDatePicker combines a date field with a calendar popup.", SampleCode.CalendarDatePicker),
            S("TransitioningContentControl", "TransitioningContentControl animates content changes.", SampleCode.TransitioningContentControl),
            S("NativeMenu", "NativeMenu integrates with platform menu systems.", SampleCode.NativeMenu),
        ]),
    ];

    private static ControlSample S(string title, string description, string axaml) => new(title, description, axaml);
}
