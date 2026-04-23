using GenFree;
using GenFree.Data;
using System;
using System.Windows.Forms;
using System.ComponentModel;
using Views;
using GenFreeWin.Attributes;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;

namespace Gen_FreeWin.Views
{
    public partial class FraEventShowEdit : UserControl
    {
        private IEventShowEditViewModel _viewModel;

        public FraEventShowEdit(IEventShowEditViewModel viewModel,IApplUserTexts strings)
        {
            _viewModel = viewModel;
            Load += FraEventShowEdit_Load;
            InitializeComponent();
            _viewModel.DoClick = () => lbl_Click(this, null);
            CommandBindingAttribute.Commit(this, viewModel);
            TextBindingAttribute.Commit(this, viewModel);
            ApplTextBindingAttribute.Commit(this, viewModel,strings);
        }
        private void FraEventShowEdit_Load(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                lblEvent.Text = $"{_viewModel.EEvtArt}:";
                return;
            };

            UpdateHdr();

        }

        private void UpdateHdr()
        {
            if (DesignMode)
            {
                switch (_viewModel.EEvtArt)
                {
                    case EEventArt.eA_Birth:
                        lblEvent.Text = "Bth:";
                        lblBirthDisp.Text = "<Birthday>";
                        break;
                    case EEventArt.eA_Baptism:
                        lblEvent.Text = "Bpt:";
                        lblBirthDisp.Text = "<Baptism>";
                        break;
                    case EEventArt.eA_Death:
                        lblEvent.Text = "Dth:";
                        lblBirthDisp.Text = "<Death>";
                        break;
                    case EEventArt.eA_Burial:
                        lblEvent.Text = "Bur:";
                        lblBirthDisp.Text = "<Burial>";
                        break;
                    default:
                        break;
                }
                return;
            } 
                
        }
      
        private void lbl_Click(object sender, EventArgs e)
        {
            OnClick(EventArgs.Empty);
        }
    }
}
