using BaseLib.Helper;
using GenFreeWin.ViewModels;
using GenFree;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views
{
    public partial class FraParentView : UserControl
    {
        IModul1 Modul1 => _Modul1.Instance;
        private IPersonRedViewModel _viewModel;

        public FraParentView()
        {
            InitializeComponent();
            if (DesignMode)
            {
                _viewModel = new PersonRedViewModel();
            }
        }

        public void SetViewmodel(IPersonRedViewModel viewModel)
        {
            _viewModel = viewModel;
            TextBindingAttribute.Commit(this, _viewModel);
            CommandBindingAttribute.Commit(this, _viewModel);
        }

        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // public EUserText iText { get => frmParent.Tag.AsEnum<EUserText>(); set => frmParent.Text = Modul1.IText[frmParent.Tag = value]; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int iPNr { get => edtParentPNr.Tag.AsInt(); set => edtParentPNr.Text = value.ToString(); }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersName { get => lblParentName.Text; set => lblParentName.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersGivn { get => lblParentName.Text; set => lblParentName.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersTitle { get => lblParentTitle.Text; set => lblParentTitle.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersText8 { get => lblParent_8.Text; set => lblParent_8.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersAka { get => lblParentAka.Text; set => lblParentAka.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersText10 { get => lblParentResidence.Text; set => lblParentResidence.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersText12 { get => lblParent_12.Text; set => lblParent_12.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersNrMarr { get => lblParentNrMarr.Text; set => lblParentNrMarr.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int iFamNr { get; internal set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool PNr_Visible { get => edtParentPNr.Visible; set => edtParentPNr.Visible = value; }
        public event EventHandler<KeyPressEventArgs> onPNr_KeyPress;
        public event EventHandler<KeyEventArgs> onPNr_KeyUp;
        public event EventHandler<EventArgs> onLabel_Click;
        public event EventHandler<EventArgs> onParentNrMarr_Click;
        public event EventHandler<EventArgs> onDelete_Click;
        public event EventHandler<EventArgs> onGrandparent_Click;
        internal void Clear(EUserText eUser)
        {
            //iText = eUser;
            iPNr = 0;
            PersName = Modul1.IText[EUserText.tName];
            PersGivn = Modul1.IText[EUserText.tGivenname];
            PersAka = Modul1.IText[EUserText.tAKA];
            PersTitle = Modul1.IText[EUserText.tTitle] + ": ";
            PersText8 = Modul1.IText[EUserText.t257];
            PersText10 = Modul1.IText[EUserText.tResidence] + ": ";
            PersText12 = "";
            PersNrMarr = Modul1.IText[EUserText.tMarrCount];

        }

        private void edtParentPNr_KeyUp(object sender, KeyEventArgs e)
        {
            onPNr_KeyUp?.Invoke(sender, e);
        }

        private void edtParentPNr_KeyPress(object sender, KeyPressEventArgs e)
        {
            onPNr_KeyPress?.Invoke(sender, e);
        }

        private void Label_Click(object sender, EventArgs e)
        {
            onLabel_Click?.Invoke(sender, e);
        }

        private void lblParentNrMarr_Click(object sender, EventArgs e)
        {
            onParentNrMarr_Click?.Invoke(sender, e);
        }

        private void lblDeleteParent_Click(object sender, EventArgs e)
        {
            onDelete_Click?.Invoke(sender, e);
        }

        private void lblGrandparent_Click(object sender, EventArgs e)
        {
            onGrandparent_Click?.Invoke(sender, e);
        }
    }
}
