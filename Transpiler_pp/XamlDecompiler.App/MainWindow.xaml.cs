using Microsoft.Win32;
using System.Windows;
using XamlDecompiler.App.ViewModels;
using XamlDecompiler.Core.Services;

namespace XamlDecompiler.App;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel(new GeneratedMauiDecompiler());
        DataContext = _viewModel;
    }

    private async void LoadFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new()
        {
            Title = "Select generated C# source",
            Filter = "C# files (*.cs)|*.cs|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog(this) == true)
        {
            await _viewModel.LoadSourceFileAsync(dialog.FileName);
        }
    }

    private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.CanSave)
        {
            return;
        }

        SaveFileDialog dialog = new()
        {
            Title = "Save reconstructed XAML",
            Filter = "XAML files (*.xaml)|*.xaml|All files (*.*)|*.*",
            FileName = _viewModel.SuggestedXamlFileName
        };

        if (dialog.ShowDialog(this) == true)
        {
            await _viewModel.SaveOutputAsync(dialog.FileName);
        }
    }
}
