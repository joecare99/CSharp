using CanvasWPF2_ItemTemplateSelector.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace CanvasWPF2_ItemTemplateSelector.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class CanvasWPFView : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CanvasWPFView"/> class.
    /// </summary>
    public CanvasWPFView()
    {
        InitializeComponent();
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {  
        ShapeData.maxSize = e.NewSize;
    }
}
