using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System;

namespace MVVM_Converter_CTImgGrid.ViewModel
{
    public partial class MainWindowViewModel:BaseViewModelCT
    {
        public Func<string, BaseViewModel?>? ShowClient { get; set; }

        public MainWindowViewModel()
        { }

        [RelayCommand]
        private void LoadLevel() => Model.Model.LoadLevel();

        [RelayCommand]
        private void NextLevel() => Model.Model.NextLevel();

        [RelayCommand]
        private void PrevLevel() => Model.Model.PrevLevel();
    }
}
