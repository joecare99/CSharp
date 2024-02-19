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
using System.Windows.Shapes;

namespace MVVM_39_MultiModelTest.Views
{
    /// <summary>
    /// Interaktionslogik für ScopedModelView.xaml
    /// </summary>
    public partial class ScopedModelView : Window
    {
        public ScopedModelView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            DataContextChanged += (s, e) =>
            {
                SetVMCloseDialog();
            };
            SetVMCloseDialog();

            void SetVMCloseDialog()
            {
                if (DataContext is ViewModels.ScopedModelViewModel vm)
                {
                    vm.DoClose = (o) =>
                    {
                        Close();
                    };
                }
            }

        }
    }
}
