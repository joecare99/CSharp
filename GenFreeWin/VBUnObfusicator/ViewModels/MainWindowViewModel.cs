using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;

namespace VBUnObfusicator.ViewModels
{
    public partial class MainWindowViewModel : BaseViewModelCT    
    {
        [ObservableProperty]
        private string _frameSource = "/Views/CodeUnObFusView.xaml";
    }
}
