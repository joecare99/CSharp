using System;
using System.Windows;
using Ollama.Wpf.TextAnalysis.ViewModels;

namespace Ollama.Wpf.TextAnalysis;

/// <summary>
/// Hosts the main text analysis window.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        InitializeComponent();
        DataContext = viewModel;
    }
}
