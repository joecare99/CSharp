using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AddPageWPF
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TabItem lPage = new TabItem();
            lPage.Header = string.Format("Page {0}", tbpPageControl.Items.Count + 1);
            lPage.Content = new DockPanel();
            tbpPageControl.Items.Add(lPage);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CheckBox lBox = new CheckBox();
            TabItem lPage = (TabItem)tbpPageControl.Items[tbpPageControl.SelectedIndex];
            lBox.Content = string.Format("CheckBox {0}_{1}", tbpPageControl.SelectedIndex + 1, ((DockPanel)lPage.Content).Children.Count+2);
            ((DockPanel)lPage.Content).Children.Add(lBox); 
        }

        private void tbpPageControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblTabNumber.Content = string.Format("Tab: {0}", tbpPageControl.SelectedIndex +1);
        }
    }
}
