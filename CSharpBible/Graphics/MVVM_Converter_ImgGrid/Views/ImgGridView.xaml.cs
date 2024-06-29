using MVVM.ViewModel;
using MVVM_Converter_ImgGrid.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_ImgGrid.Views
{
    /// <summary>
    /// Interaction logic for ImgGridView.xaml
    /// </summary>
    public partial class ImgGridView : Page
    {
        public ImgGridView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImgGridViewModel vm)
            {
                vm.ShowClient = _ShowClientinFrame;
            }
        }

        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ImgGridViewModel vm)
            {
//                vm.FrameDataContext = e.Source as Frame
            }
        }

        private BaseViewModel? _ShowClientinFrame(string arg)
        {
            try
            {
                this.Client.Source = new Uri(arg);
                return Client.DataContext as BaseViewModel;
            }
            catch(Exception) 
            {
                return null;
            };
        }
    }
}
