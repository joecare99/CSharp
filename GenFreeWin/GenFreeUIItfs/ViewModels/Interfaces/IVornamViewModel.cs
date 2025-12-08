using Gen_FreeWin.Views;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces
{
    public interface IVornamViewModel:INotifyPropertyChanged
    {
        Form View { get; set; }

        void Befehl_Click(object sender, EventArgs e);
        void List1_DoubleClick(object sender, EventArgs e);
        void Liste1_DoubleClick(object sender, EventArgs e);
        void Text_Renamed_KeyPress(object sender, KeyPressEventArgs e);
        void Text_Renamed_KeyUp(object sender, KeyEventArgs e);
        void Text_Renamed_TextChanged(object sender, EventArgs e);
        void Form_Load(object sender, EventArgs e);
    }
}