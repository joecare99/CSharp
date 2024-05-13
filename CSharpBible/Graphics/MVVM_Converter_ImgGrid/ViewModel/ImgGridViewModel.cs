using MVVM.ViewModel;
using System;

namespace MVVM_Converter_ImgGrid.ViewModel
{
    public class MainWindowViewModel:BaseViewModel
    {
        public Func<string, BaseViewModel?>? ShowClient { get; set; }

        public DelegateCommand LoadLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.LoadLevel());
        public DelegateCommand NextLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.NextLevel());
        public DelegateCommand PrevLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.PrevLevel());

        public MainWindowViewModel()
        {

        }



    }
}
