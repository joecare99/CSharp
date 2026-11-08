using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AvlnCodeStudio.Model.Documents;
using Avalonia.Controls;
using Property.Editor;
using System;
using System.ComponentModel;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.Components;

/// <summary>Exports the reusable palette and extended property editor for one designer view.</summary>
public sealed class AxamlDesignerContributions : IDocumentEditorContributions<Control>
{
    private readonly AxamlDesignerViewModel _designer;
    private Control? _paletteView;
    private Control? _propertiesView;

    public AxamlDesignerContributions(AxamlDesignerViewModel designer)
    {
        _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        _designer.PropertyChanged += OnDesignerPropertyChanged;
    }

    public Control PaletteView => _paletteView ??= new AxamlDesignerPaletteView { DataContext = _designer };

    public Control PropertiesView => _propertiesView ??= new AxamlDesignerPropertiesView { DataContext = _designer };

    public IPropertyProvider SelectedProperties => _designer.PropertyProvider;

    public bool HasUnappliedDrafts => _designer.HasUnappliedPropertyDrafts;

    public event EventHandler? SelectionChanged;

    public event EventHandler? DraftStateChanged;

    public void Dispose() => _designer.PropertyChanged -= OnDesignerPropertyChanged;

    private void OnDesignerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AxamlDesignerViewModel.SelectedNode))
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        else if (e.PropertyName == nameof(AxamlDesignerViewModel.HasUnappliedPropertyDrafts))
        {
            DraftStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
