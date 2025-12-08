using BaseLib.Helper;
using GenFree;
using GenFree.Helper;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.Interfaces.UI;
using Views;
using GenFreeWin.Attributes;
using GenFree.ViewModels.Interfaces;

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
