using MVVM.ViewModel;
using System;
using System.Reflection;

namespace MVVM_Converter_ImgGrid.ViewModel
{
    public class ImgGridViewModel:BaseViewModel
    {
        public Func<string, BaseViewModel?>? ShowClient { get; set; }

        public DelegateCommand LoadLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.LoadLevel());
        public DelegateCommand NextLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.NextLevel());
        public DelegateCommand PrevLevelCommand { get; set; } = new DelegateCommand((o) => Model.Model.PrevLevel());

        public string PlotFrameSource => $"/{Assembly.GetExecutingAssembly().GetName().Name};component/Views/PlotFrame.xaml";
        public ImgGridViewModel()
        {

        }



    }
}
