using System.Windows.Controls;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaction logic for TextPageView.xaml.
    /// </summary>
    public partial class TextPageView : Page
    {
        public TextPageView()
        {
            InitializeComponent();
            DataContext = new TextPageViewModel();
        }
    }
}
