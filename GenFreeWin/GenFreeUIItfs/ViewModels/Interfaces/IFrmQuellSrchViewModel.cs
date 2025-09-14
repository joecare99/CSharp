using System;

namespace GenFree.ViewModels.Interfaces;

public interface IFrmQuellSrchViewModel
{
    void btnClose_Click(object sender, EventArgs e);
    void btnDeleteEntry_Click(object sender, EventArgs e);
    void edtSearch_TextChanged(object sender, EventArgs e);
    void ListBox1_DoubleClick(object sender, EventArgs e);
}