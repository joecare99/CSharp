using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IRechTextViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    int FamPerSchalt { get; }
    int PersInArb { get; }

    void Bef_Click(object eventSender, EventArgs eventArgs);
    void Command1_Click(object eventSender, EventArgs eventArgs);
    void Label5_Click(object s, EventArgs e);
    void List3_DoubleClick(object s, EventArgs e);
    void Liste1_DoubleClick(object s, EventArgs e);
    void RechText_FormClosing(object eventSender, FormClosingEventArgs eventArgs);
    void RechText_Load(object eventSender, EventArgs eventArgs);
    void _Bef_3_Click(object s, EventArgs e);
}