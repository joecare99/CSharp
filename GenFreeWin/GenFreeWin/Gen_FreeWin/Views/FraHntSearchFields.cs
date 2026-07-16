using System;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.Data;

namespace GenFreeWin.Views
{
    public partial class FraHntSearchFields : UserControl
    {
       
        public FraHntSearchFields()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ESearchSelection eSearchSelection1
        {
            get { return fraHntSrcSelection1.eSearchSelection; }
            set { fraHntSrcSelection1.eSearchSelection = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ESearchSelection eSearchSelection2
        {
            get { return fraHntSrcSelection2.eSearchSelection; }
            set { fraHntSrcSelection2.eSearchSelection = value; }
        }
        public ESearchSelection eSearchSelection3
        {
            get { return fraHntSrcSelection3.eSearchSelection; }
            set { fraHntSrcSelection3.eSearchSelection = value; }
        }

        public event EventHandler SaveAndBack;

        private void btnSaveAndBack_Click(object sender, EventArgs e)
        {
            SaveAndBack?.Invoke(this, EventArgs.Empty);
            Hide();
        }

        private void fraHntSrcSelection_eSearchSelectionChanged(object sender, ESearchSelection e)
        {
            
        }
    }
}
