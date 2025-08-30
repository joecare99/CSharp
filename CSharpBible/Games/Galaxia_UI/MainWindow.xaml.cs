using Galaxia.UI.ViewModels;
using System.Windows;

namespace Galaxia.UI
{
    /// <summary>
    /// Hauptfenster; DataContext via DI injiziert.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}