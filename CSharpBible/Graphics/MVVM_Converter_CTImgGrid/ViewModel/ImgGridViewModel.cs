using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System;
using System.Reflection;

namespace MVVM_Converter_CTImgGrid.ViewModel
{
    public partial class ImgGridViewModel:BaseViewModelCT
    {
        public Func<string, BaseViewModel?>? ShowClient { get; set; }

        public ImgGridViewModel()
        { }

        [RelayCommand]
        private void LoadLevel() => Model.Model.LoadLevel();

        [RelayCommand]
        private void NextLevel() => Model.Model.NextLevel();

        [RelayCommand]
        private void PrevLevel() => Model.Model.PrevLevel();

        /// <summary>Gets the plot frame source.</summary>
        /// <value>The plot frame source.</value>
        public string PlotFrameSource => $"/{Assembly.GetExecutingAssembly().GetName().Name};component/Views/PlotFrame.xaml";

    }
}
