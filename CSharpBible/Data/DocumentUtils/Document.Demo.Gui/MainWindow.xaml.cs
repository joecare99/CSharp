using System.Windows;
using Microsoft.Win32;
using Document.Demo.Gui.ViewModels;

namespace Document.Demo.Gui;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel(new Services.DocumentDemoExportService());
        DataContext = _viewModel;
    }

    private void ChoosePath_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = BuildFilter(),
            AddExtension = true,
            FileName = _viewModel.OutputFileName,
            InitialDirectory = _viewModel.OutputDirectory
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.ApplyOutputPath(dialog.FileName);
        }
    }

    private string BuildFilter()
    {
        return string.Join("|", _viewModel.OutputFormats.Select(format =>
            $"{format.DisplayName}|*{format.Extension}"));
    }
}
