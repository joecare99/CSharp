using CanvasWPF2_CTItemTemplateSelector.ViewModel;
using System.Windows;

namespace CanvasWPF2_CTItemTemplateSelector.Views
{
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
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {  
            ShapeData.maxSize = e.NewSize;
        }
    }
}
