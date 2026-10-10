using CommunityToolkit.Mvvm.ComponentModel;
using OFBCreator.Projects.Models;

namespace OFBCreator.Avalonia.ViewModels;

public sealed partial class OFBParagraphStyleEditorViewModel : ObservableObject
{
    public string Key { get; }

    public string DisplayName { get; }

    public string[] AlignmentOptions { get; } = ["Left", "Center", "Right", "Justified"];

    [ObservableProperty]
    private string _fontFamily;

    [ObservableProperty]
    private double _fontSizePt;

    [ObservableProperty]
    private int _indentPt;

    [ObservableProperty]
    private int _hangingIndentPt;

    [ObservableProperty]
    private int _spaceBeforePt;

    [ObservableProperty]
    private int _spaceAfterPt;

    [ObservableProperty]
    private string _alignment;

    public OFBParagraphStyleEditorViewModel(string key, string displayName, OFBParagraphStyle style)
    {
        Key = key;
        DisplayName = displayName;
        _fontFamily = style.FontFamily;
        _fontSizePt = style.FontSizePt;
        _indentPt = style.IndentPt;
        _hangingIndentPt = style.HangingIndentPt;
        _spaceBeforePt = style.SpaceBeforePt;
        _spaceAfterPt = style.SpaceAfterPt;
        _alignment = style.Alignment;
    }

    public OFBParagraphStyle ToModel() => new()
    {
        FontFamily = FontFamily,
        FontSizePt = FontSizePt,
        IndentPt = IndentPt,
        HangingIndentPt = HangingIndentPt,
        SpaceBeforePt = SpaceBeforePt,
        SpaceAfterPt = SpaceAfterPt,
        Alignment = Alignment
    };

    public void Load(OFBParagraphStyle style)
    {
        FontFamily = style.FontFamily;
        FontSizePt = style.FontSizePt;
        IndentPt = style.IndentPt;
        HangingIndentPt = style.HangingIndentPt;
        SpaceBeforePt = style.SpaceBeforePt;
        SpaceAfterPt = style.SpaceAfterPt;
        Alignment = style.Alignment;
    }
}
