using BaseLib.Helper;
using CommonDialogs.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using VTileEdit.WPF.ViewModels;

namespace VTileEdit.WPF.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        MainWindowViewModel vm;
        DataContext = vm = IoC.GetRequiredService<MainWindowViewModel>();
        vm.ShowFileDlg = ShowFileDialog;
    }

    private bool ShowFileDialog(IFileDialog dialog) => dialog.ShowDialog(this) ?? false;

    private void TileSetExpander_Collapsed(object sender, RoutedEventArgs e)
    {
        if (sender is not Expander expander)
        {
            return;
        }

        if (DataContext is MainWindowViewModel vm && vm.TileSetHasPendingChanges)
        {
            expander.IsExpanded = true;
        }
    }
}
