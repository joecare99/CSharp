using GenFree.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenFree.ViewModels.Interfaces;

public interface IRahmenViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }
    IList<int>? _aiPers { get; set; }
    IList<(EEventArt eArt, short iKnz, int iFamily)>? _atEKFam { get; set; }

    void btnAppend_Click(object s, EventArgs e);
    void btnClose_Click(object s, EventArgs e);
    void btnEnterNumber_Click(object s, EventArgs e);
    void btnSelCancel_Click(object s, EventArgs e);
    void btnSelFromFile_Click(object s, EventArgs e);
    void btnSelReenter_Click(object s, EventArgs e);
    void Command1_Click(object s, EventArgs e);
    void Form_Load(object sender, EventArgs e);
}