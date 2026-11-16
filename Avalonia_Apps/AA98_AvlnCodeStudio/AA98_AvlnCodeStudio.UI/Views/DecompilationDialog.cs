using System;
using AA98_AvlnCodeStudio.Decompilation;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Code.Navigation;

namespace AA98_AvlnCodeStudio.UI.Views;

public sealed class DecompilationDialog : Window
{
    private readonly DecompilationWorkbench _workbench;
    private readonly TextBlock _assembly = new() { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
    private readonly TextBlock _status = new() { TextWrapping = Avalonia.Media.TextWrapping.Wrap };
    private readonly ListBox _types = new();
    private readonly Button _open = new() { Content = "Open source", IsEnabled = false };
    private bool _closed;

    public DecompilationDialog(IAssemblyDecompilationService service, IDecompilationAssemblyPicker picker)
    {
        _workbench = new DecompilationWorkbench(service);
        ArgumentNullException.ThrowIfNull(picker);
        Title = "Decompile assembly";
        Width = 700;
        Height = 520;
        MinWidth = 480;
        MinHeight = 320;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        Button choose = new() { Content = "Choose assembly..." };
        Button cancel = new() { Content = "Close" };
        choose.Click += async (_, _) =>
        {
            choose.IsEnabled = false;
            _workbench.Reset();
            try
            {
                string? path = await picker.PickAsync(this);
                if (!_closed && path is not null)
                {
                    await _workbench.InspectAsync(path);
                }
            }
            catch (Exception exception)
            {
                if (!_closed) _status.Text = $"Cannot choose assembly: {exception.Message}";
            }
            finally
            {
                if (!_closed) choose.IsEnabled = true;
            }
        };
        _types.SelectionChanged += (_, _) => _workbench.SelectType(_types.SelectedItem as AssemblyTypeInfo);
        _open.Click += async (_, _) =>
        {
            CodeLocation? location = await _workbench.DecompileSelectedAsync();
            if (!_closed && location is not null) Close(location);
        };
        cancel.Click += (_, _) => Close();
        Closed += (_, _) =>
        {
            _closed = true;
            _workbench.Dispose();
        };
        _workbench.Changed += (_, _) => Refresh();
        Content = new Grid
        {
            Margin = new Thickness(14),
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto,Auto"),
            RowSpacing = 10,
            Children =
            {
                choose,
                AtRow(_assembly, 1),
                AtRow(_types, 2),
                AtRow(_status, 3),
                AtRow(new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Spacing = 8,
                    Children = { _open, cancel },
                }, 4),
            },
        };
        Refresh();
    }

    private void Refresh()
    {
        _assembly.Text = _workbench.Assembly is { } assembly
            ? $"{assembly.Name} {assembly.Version} — {assembly.AssemblyPath}"
            : "No assembly selected.";
        if (!ReferenceEquals(_types.ItemsSource, _workbench.Types))
        {
            _types.ItemsSource = _workbench.Types;
            _types.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<AssemblyTypeInfo>(
                (type, _) => new TextBlock { Text = type?.FullName });
        }
        _types.IsEnabled = !_workbench.IsBusy;
        _open.IsEnabled = !_workbench.IsBusy && _workbench.SelectedType is not null;
        _status.Text = _workbench.Status;
    }

    private static T AtRow<T>(T control, int row) where T : Control
    {
        Grid.SetRow(control, row);
        return control;
    }
}
