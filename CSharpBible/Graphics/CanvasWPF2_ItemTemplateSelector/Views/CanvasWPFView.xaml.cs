using CanvasWPF2_ItemTemplateSelector.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
