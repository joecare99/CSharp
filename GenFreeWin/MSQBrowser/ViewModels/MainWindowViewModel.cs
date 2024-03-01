using CommunityToolkit.Mvvm.ComponentModel;

namespace MSQBrowser.ViewModels
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
