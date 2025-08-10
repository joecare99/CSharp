using CommunityToolkit.Mvvm.Input;
using GenFree.Data;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface ITextLesenViewModel : INotifyPropertyChanged
{
    Form View { get; set; }
    string Text1_Text { get; set; }
    string Text2_Text { get; set; }
    string Text3_Text { get; set; }
    string Text4_Text { get; set; }
    bool Check3_Checked { get; set; }
    bool ChangeSex_Visibility { get; set; }
    IRelayCommand ChangeSexToFCommand { get; }
    IRelayCommand ChangeSexToMCommand { get; }
    IRelayCommand MoveToChurchCemetCommand { get; }
    IRelayCommand MoveToEntityAnotCommand { get; }
    (string sText, ETextKennz eTKnz) tTextBez { get; set; }
    bool Check1_Checked { get; set; }
    bool Frame1_Visible { get; }

    void Bef_Click(object sender, EventArgs e);
    void btnMoveNameToAlias_Click(object sender, EventArgs e);
    void btnMoveToCause_Click(object sender, EventArgs e);
    void btnReenter_Click(object sender, EventArgs e);
    void btnShowAsocPeople_Click(object sender, EventArgs e);
    void Command1_Click(object sender, EventArgs e);
    void Form_Load(object sender, EventArgs e);
    void Label7_Click(object sender, EventArgs e);
    void Form_Closing(object sender, FormClosingEventArgs e);
    void Bezeichnung2_TextChanged(object sender, EventArgs e);
    void Text3_KeyUp(object eventSender, KeyEventArgs eventArgs);
    void Text1_TextChanged(object eventSender, EventArgs eventArgs);
    void Liste1_DoubleClick(object eventSender, EventArgs eventArgs);
    void List3_DoubleClick(object eventSender, EventArgs eventArgs);
    void List1_DoubleClick(object eventSender, EventArgs eventArgs);
    void Command3_Click(object eventSender, EventArgs eventArgs);
    void Bez_Click(object eventSender, EventArgs eventArgs);
    void Text4_KeyUp(object sender, KeyEventArgs e);
    void Text2_KeyUp(object sender, KeyEventArgs e);
    void btnMoveToDateAnot_Click(object sender, EventArgs e);
    void btnDeleteEntry_Click(object sender, EventArgs e);
    void btnMoveToLowerDateAnot_Click(object sender, EventArgs e);
}