using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IMandViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    bool Frame1_Visible { get; }

    void Befehl2_Click(object sender, EventArgs e);
    void CmdDeleteMandant_Click(object sender, EventArgs e);
    void CmdNewMandant_Click(object sender, EventArgs e);
    void Command1_Click(object sender, EventArgs e);
    void Form_Load(object sender, EventArgs e);
    void Laufwerk1_SelectedIndexChanged(object sender, EventArgs e);
    void List1_Click(object sender, EventArgs e);
    void List1_DoubleClick(object sender, EventArgs e);
    void Text1_KeyPress(object sender, KeyPressEventArgs e);
}