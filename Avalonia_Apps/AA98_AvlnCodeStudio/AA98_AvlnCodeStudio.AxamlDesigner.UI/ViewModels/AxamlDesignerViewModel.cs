using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public partial class AxamlDesignerViewModel : ObservableObject
{
    private readonly AxamlDesignerSession _session;
    private bool _synchronizing;

    public AxamlDesignerViewModel(AxamlPreviewService? previewService = null)
    {
        _session = new AxamlDesignerSession(previewService);
    }

    public event EventHandler<AxamlDesignerMarkupChangedEventArgs>? MarkupChanged;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private AxamlPreviewNode? _previewRoot;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedElementName))]
    private AxamlPreviewNode? _selectedNode;

    [ObservableProperty]
    private string? _selectedProperty;

    [ObservableProperty]
    private string _selectedValue = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private string _errorText = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public ObservableCollection<string> EditableProperties { get; } = [];

    public bool HasPreview => PreviewRoot is not null || !string.IsNullOrWhiteSpace(ErrorText);

    public string SelectedElementName => SelectedNode?.TypeName ?? "Select a control in the preview.";

    public string PreviewTitle => "AXAML Preview (safe subset)";

    public bool CanUndo => _session.CanUndo;

    public bool CanRedo => _session.CanRedo;

    public bool ResizeElement(AxamlPreviewNode node, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!double.IsFinite(width) || !double.IsFinite(height))
        {
            StatusText = "AXAML resize failed: dimensions must be finite numbers.";
            return false;
        }

        try
        {
            _session.SelectNode(node);
            Dictionary<string, string> values = new(StringComparer.Ordinal)
            {
                ["Width"] = FormatLayoutSize(width),
                ["Height"] = FormatLayoutSize(height),
            };
            if (!_session.ApplyValues(values, "Width"))
            {
                return false;
            }

            SynchronizeState();
            StatusText = "Designer size applied to AXAML.";
            RaiseMarkupChanged();
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"AXAML resize failed: {exception.Message}";
            return false;
        }
    }

    public void LoadMarkup(string? markup)
    {
        if (string.IsNullOrWhiteSpace(markup))
        {
            _session.Reset();
            ErrorText = string.Empty;
            SynchronizeState();
            return;
        }

        try
        {
            _session.Load(markup);
            ErrorText = string.Empty;
            SynchronizeState();
        }
        catch (Exception exception) when (exception is XmlException or InvalidDataException)
        {
            _session.Reset();
            SynchronizeState();
            ErrorText = exception.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplySelectedValue))]
    private void ApplySelectedValue()
    {
        if (SelectedProperty is null || SelectedNode is null)
        {
            return;
        }

        try
        {
            _session.SelectProperty(SelectedProperty);
            if (_session.ApplySelectedValue(SelectedValue))
            {
                SynchronizeState();
                StatusText = "Designer property applied to AXAML.";
                RaiseMarkupChanged();
            }
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"AXAML designer edit failed: {exception.Message}";
        }
    }

    private bool CanApplySelectedValue()
    {
        return SelectedNode is not null
            && SelectedProperty is not null
            && _session.EditableProperties.Contains(SelectedProperty, StringComparer.Ordinal);
    }

    partial void OnSelectedNodeChanged(AxamlPreviewNode? value)
    {
        if (_synchronizing)
        {
            return;
        }

        try
        {
            _session.SelectNode(value);
            SynchronizeState();
        }
        catch (InvalidOperationException exception)
        {
            StatusText = $"AXAML selection failed: {exception.Message}";
        }

        ApplySelectedValueCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedPropertyChanged(string? value)
    {
        if (!_synchronizing && !string.IsNullOrWhiteSpace(value))
        {
            try
            {
                _session.SelectProperty(value);
                SelectedValue = _session.SelectedValue;
            }
            catch (InvalidOperationException exception)
            {
                StatusText = $"AXAML property selection failed: {exception.Message}";
                SelectedProperty = null;
            }
        }

        ApplySelectedValueCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanUndoDesigner))]
    private void Undo()
    {
        if (_session.Undo())
        {
            SynchronizeState();
            RaiseMarkupChanged();
        }
    }

    private bool CanUndoDesigner() => _session.CanUndo;

    [RelayCommand(CanExecute = nameof(CanRedoDesigner))]
    private void Redo()
    {
        if (_session.Redo())
        {
            SynchronizeState();
            RaiseMarkupChanged();
        }
    }

    private bool CanRedoDesigner() => _session.CanRedo;

    private void SynchronizeState()
    {
        _synchronizing = true;
        try
        {
            PreviewRoot = _session.Root;
            SelectedNode = _session.SelectedNode;
            EditableProperties.Clear();
            foreach (string property in _session.EditableProperties)
            {
                EditableProperties.Add(property);
            }

            SelectedProperty = _session.SelectedProperty;
            SelectedValue = _session.SelectedValue;
        }
        finally
        {
            _synchronizing = false;
        }

        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        ApplySelectedValueCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    private void RaiseMarkupChanged()
    {
        MarkupChanged?.Invoke(this, new AxamlDesignerMarkupChangedEventArgs(_session.Markup));
    }

    private static string FormatLayoutSize(double value)
    {
        return Math.Clamp(value, 16, 4096).ToString("0.#", CultureInfo.InvariantCulture);
    }
}

public sealed class AxamlDesignerMarkupChangedEventArgs(string markup) : EventArgs
{
    public string Markup { get; } = markup;
}
