using BaseLib.Helper;
using GenFree;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.Interfaces.Sys;

namespace Gen_FreeWin.Views
{
    public partial class FraRahSelect : UserControl
    {
        public event EventHandler Cancel;
        public event EventHandler Reenter;
        public event EventHandler EnterNumber;
        public event EventHandler FromFile;
        IModul1 Modul1 => _Modul1.Instance;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EUserText eText { get => frmRahmenSelect.Tag.AsEnum<EUserText>(); 
            set => frmRahmenSelect.Text = Modul1.IText[(frmRahmenSelect.Tag = value).AsEnum<EUserText>()]; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool xEnReenter { get=>btnReenter.Enabled; set=> btnReenter.Enabled=value; }
        public bool xVisReenter { get => btnReenter.Visible; set=> btnReenter.Visible = value; }
        public new string Text { get => frmRahmenSelect.Text; set => frmRahmenSelect.Text = value; }
        public FraRahSelect()
        {
            InitializeComponent();
        }

        
        private void btnCancel_Click(object sender, EventArgs e) => Cancel?.Invoke(this, e);
        private void btnReenter_Click(object sender, EventArgs e) => Reenter?.Invoke(this, e);
        private void btnEnterNumber_Click(object sender, EventArgs e) => EnterNumber?.Invoke(this, e);
        private void btnFromFile_Click(object sender, EventArgs e) => FromFile?.Invoke(this, e);

        private void FraRahSelect_Load(object sender, EventArgs e)
        {
            if (this.DesignMode) return;
            frmRahmenSelect.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            btnReenter.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            btnFromFile.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            btnEnterNumber.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            btnCancel.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
    }
}
