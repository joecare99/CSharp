using CommunityToolkit.Mvvm.ComponentModel;

namespace MdbBrowser.ViewModels
{
    public partial class MainWindowViewModel : ObservableValidator
    {

        public MainWindowViewModel()
        {
            View = "DBView.xaml";
        }

        [ObservableProperty]
        private string _view;

    }
}
