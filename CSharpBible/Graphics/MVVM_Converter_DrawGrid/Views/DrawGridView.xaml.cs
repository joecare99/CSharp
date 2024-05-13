using MVVM.ViewModel;
using MVVM_Converter_DrawGrid.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_Converter_DrawGrid.Views
{
    /// <summary>
    /// Interaction logic for DrawGridView.xaml
    /// </summary>
    public partial class DrawGridView : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawGridView"/> class.
        /// </summary>
        public DrawGridView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DrawGridViewModel vm)
            {
                vm.ShowClient = ShowClientinFrame;
            }
        }

        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DrawGridViewModel vm)
            {
//                vm.FrameDataContext = e.Source as Frame
            }
        }

        private BaseViewModel? ShowClientinFrame(string arg)
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
