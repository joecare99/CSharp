using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Attributes;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views
{
    public partial class FraPersImpQuerry : UserControl
    {
        private readonly IApplUserTexts _strings;
        private readonly IFraPersImpQueryViewModel _viewModel;

        public FraPersImpQuerry(IFraPersImpQueryViewModel viewModel, IApplUserTexts strings)
        {
            _strings = strings;
            _viewModel = viewModel;
            InitializeComponent();

            CommandBindingAttribute.Commit(this, viewModel);
            ApplTextBindingAttribute.Commit(this, viewModel, _strings);
        }
    }
}
