using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public interface IOFBViewModel: INotifyPropertyChanged
{
    IContainerControl View { get; set; }

    void Command1_Click(object s, EventArgs e);
    void List1_DoubleClick(object s, EventArgs e);
    void List2_DoubleClick(object s, EventArgs e);
    void List3_DoubleClick(object s, EventArgs e);
    void List4_DoubleClick(object s, EventArgs e);
    void List4_SelectedIndexChanged(object s, EventArgs e);
    void List5_DoubleClick(object sender, EventArgs e);
    void OFB_Load(object sender, EventArgs e);
    void Text1_KeyUp(object s, KeyEventArgs e);
    void Text1_TextChanged(object s, EventArgs e);
    void Text2_KeyUp(object sender, KeyEventArgs e);
    void Text2_TextChanged(object sender, EventArgs e);
}