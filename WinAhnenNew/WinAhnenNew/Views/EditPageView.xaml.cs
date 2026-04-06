using System.Windows.Controls;
using WinAhnenNew.ViewModels;

namespace WinAhnenNew.Views
{
    /// <summary>
    /// Interaktionslogik für EditPageView.xaml
    /// </summary>
    public partial class EditPageView : Page
    {
        public EditPageView()
        {
            InitializeComponent();
            DataContext = new EditPageViewModel();
        }
    }
}
