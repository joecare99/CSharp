using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;

public partial class AxamlDesignerPaletteView : UserControl
{
    public AxamlDesignerPaletteView()
    {
        InitializeComponent();
        AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel, true);
        AddHandler(InputElement.GotFocusEvent, OnGotFocus, RoutingStrategies.Bubble, true);
    }

    public event EventHandler? EditorActivated;

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsEditorInteraction(e.Source, includeTree: false))
        {
            EditorActivated?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnGotFocus(object? sender, FocusChangedEventArgs e)
    {
        if (e.NavigationMethod != NavigationMethod.Unspecified && IsEditorInteraction(e.Source, includeTree: true))
        {
            EditorActivated?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool IsEditorInteraction(object? source, bool includeTree)
    {
        for (Visual? visual = source as Visual; visual is not null && !ReferenceEquals(visual, this);
             visual = visual.GetVisualParent())
        {
            if (visual is TreeViewItem
                || visual is Button { Command: not null }
                || includeTree && visual is TreeView { Name: "StructureTree" })
            {
                return true;
            }
        }

        return false;
    }
}
