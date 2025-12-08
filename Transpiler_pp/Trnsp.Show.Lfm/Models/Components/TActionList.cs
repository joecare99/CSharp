using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TActionList component.
/// </summary>
public partial class TActionList : LfmComponentBase
{
    [ObservableProperty]
    private string _images = string.Empty;

    public List<TAction> Actions { get; } = [];

    public TActionList()
    {
        Width = 28;
        Height = 28;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "images":
                Images = value?.ToString() ?? string.Empty;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TAction component.
/// </summary>
public partial class TAction : LfmComponentBase
{
    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private int _imageIndex = -1;

    [ObservableProperty]
    private string _shortCut = string.Empty;

    [ObservableProperty]
    private string _executeHandler = string.Empty;

    [ObservableProperty]
    private string _updateHandler = string.Empty;

    public TAction()
    {
        Width = 23;
        Height = 22;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "category":
                Category = value?.ToString() ?? string.Empty;
                break;
            case "imageindex":
                ImageIndex = ConvertToInt(value, -1);
                break;
            case "shortcut":
                ShortCut = value?.ToString() ?? string.Empty;
                break;
            case "onexecute":
                ExecuteHandler = value?.ToString() ?? string.Empty;
                break;
            case "onupdate":
                UpdateHandler = value?.ToString() ?? string.Empty;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TFileOpen standard action.
/// </summary>
public partial class TFileOpen : TAction
{
    [ObservableProperty]
    private string _dialogFilter = string.Empty;

    [ObservableProperty]
    private string _dialogDefaultExt = string.Empty;

    [ObservableProperty]
    private string _acceptHandler = string.Empty;

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "dialog.filter":
                DialogFilter = value?.ToString() ?? string.Empty;
                break;
            case "dialog.defaultext":
                DialogDefaultExt = value?.ToString() ?? string.Empty;
                break;
            case "onaccept":
                AcceptHandler = value?.ToString() ?? string.Empty;
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TFileSaveAs standard action.
/// </summary>
public partial class TFileSaveAs : TFileOpen
{
}

/// <summary>
/// Represents a TFileExit standard action.
/// </summary>
public partial class TFileExit : TAction
{
    [ObservableProperty]
    private List<string> _secondaryShortCutList = [];

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "secondaryshortcuts.strings":
                if (value is IEnumerable<object> items)
                {
                    SecondaryShortCutList.Clear();
                    foreach (var item in items)
                    {
                        SecondaryShortCutList.Add(item?.ToString() ?? string.Empty);
                    }
                }
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}

/// <summary>
/// Represents a TImageList component.
/// </summary>
public partial class TImageList : LfmComponentBase
{
    [ObservableProperty]
    private int _imageWidth = 16;

    [ObservableProperty]
    private int _imageHeight = 16;

    public TImageList()
    {
        Width = 28;
        Height = 28;
    }

    protected override void ApplyProperty(string name, object? value)
    {
        switch (name.ToLower())
        {
            case "width":
                ImageWidth = ConvertToInt(value, 16);
                break;
            case "height":
                ImageHeight = ConvertToInt(value, 16);
                break;
            default:
                base.ApplyProperty(name, value);
                break;
        }
    }
}
