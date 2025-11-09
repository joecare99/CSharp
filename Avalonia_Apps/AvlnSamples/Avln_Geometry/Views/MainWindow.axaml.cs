using Avalonia.Controls;
using BaseLib.Helper;
using Avln_Geometry.ViewModels.Interfaces;

namespace Avln_Geometry.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Initialize the SampleViewer with DI
        var sampleViewer = IoC.GetRequiredService<SampleViewer>();
        Content = sampleViewer;
    }
}
