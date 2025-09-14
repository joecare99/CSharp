using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Gen_FreeWin.Views
{
    public interface IPartnerRechercheViewModel: INotifyPropertyChanged
    {
        IContainerControl View { get; set; }

        void Command1_Click(object s, EventArgs e);
        void Command2_Click(object s, EventArgs e);
        void Command3_Click(object s, EventArgs e);
        void List1_DoubleClick(object s, EventArgs e);
        void List2_DoubleClick(object s, EventArgs e);
        void Partnerrecherche_Load(object sender, EventArgs e);
        void Partnerrecherche_Resize(object sender, EventArgs e);
        void Text1_KeyPress(object sender, KeyPressEventArgs e);
        void Text1_TextChanged(object sender, EventArgs e);
        void _Text1_0_TextChanged(object s, EventArgs e);
    }
}