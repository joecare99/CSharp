using System.Windows;

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
