using MVVM.View.Extension;
using System.Windows.Controls;

namespace MVVM_39_MultiModelTest.Views
{
    /// <summary>
    /// Interaktionslogik für TemplateView.xaml
    /// </summary>
    public partial class MultiModelMainView : Page
    {
        public MultiModelMainView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            DataContextChanged += (s, e) =>
            {
                SetVMShowDialog();
            };
            SetVMShowDialog();

            void SetVMShowDialog()
            {
                if (DataContext is ViewModels.MultiModelMainViewModel vm)
                {
                    vm.showModel = (model) =>
                    {
                        IoC.SetCurrentScope(model.Scope);
                        var view = new ScopedModelView();
                        view.Show();
                    };
                }
            }

        }
    }
}
