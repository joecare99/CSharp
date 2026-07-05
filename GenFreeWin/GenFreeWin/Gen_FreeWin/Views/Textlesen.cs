using BaseLib.Helper;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;

public partial class Textlesen : Form
{
    private static List<WeakReference> __ENCList = new();
    private ITextLesenViewModel _viewModel;
    public ITextLesenViewModel ViewModel => _viewModel;

    public void btnShowAsocPeople_Click(object o, EventArgs e) => _viewModel.btnShowAsocPeople_Click(0,e);
    public void btnReenter_Click(object o, EventArgs e) => _viewModel.btnReenter_Click(o,e);
    public void btnMoveToCause_Click(object o, EventArgs e) => _viewModel.btnMoveToCause_Click(o,e);
    public void btnMoveNameToAlias_Click(object o, EventArgs e) => _viewModel.btnMoveNameToAlias_Click(o,e);
    public void Label7_Click(object o, EventArgs e) => _viewModel.Label7_Click(o, e);
    public void Bezeichnung2_TextChanged(object o, EventArgs e) => _viewModel.Bezeichnung2_TextChanged(o, e);
    public void Text4_KeyUp(object o, KeyEventArgs e) => _viewModel.Text4_KeyUp(o, e);
    public void Text2_KeyUp(object o, KeyEventArgs e) => _viewModel.Text2_KeyUp(o, e);
    public void Text3_KeyUp(object o, KeyEventArgs e) => _viewModel.Text3_KeyUp(o, e);
    public void Text1_TextChanged(object o, EventArgs e) => _viewModel.Text1_TextChanged(o, e);
    public void Liste1_DoubleClick(object o, EventArgs e) => _viewModel.Liste1_DoubleClick(o, e);
    public void List1_DoubleClick(object o, EventArgs e) => _viewModel.List1_DoubleClick(o, e);
    public void List3_DoubleClick(object o, EventArgs e) => _viewModel.List3_DoubleClick(o, e);
    public void Command3_Click(object o, EventArgs e) => _viewModel.Command3_Click(o,e);

    public void btnMoveToDateAnot_Click(object o, EventArgs e) => _viewModel.btnMoveToDateAnot_Click(o,e);
    public void btnDeleteEntry_Click(object o, EventArgs e) => _viewModel.btnDeleteEntry_Click(o,e);
    public void btnMoveToLowerDateAnot_Click(object o, EventArgs e) => _viewModel.btnMoveToLowerDateAnot_Click(o,e);

    [DebuggerNonUserCode]
    public Textlesen(ITextLesenViewModel viewModel)
    {
        _viewModel = viewModel;
        Load += _viewModel.Form_Load;
        FormClosing += _viewModel.Form_Closing;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        components = new System.ComponentModel.Container();

        Bef = new ControlArray<Button>();
        Bezeichnung1 = new ControlArray<Label>();
        Check2 = new ControlArray<CheckBox>();
        Command1 = new ControlArray<Button>();
        Label1 = new ControlArray<Label>();

        InitializeComponent();

        Label1.SetIndex(_Label1_0, 0);
        Label1.SetIndex(_Label1_1, 1);
        Label1.SetIndex(_Label1_2, 2);
        Label1.SetIndex(_Label1_3, 3);
        Label1.SetIndex(_Label1_4, 4);

        Bezeichnung1.SetIndex(_Bezeichnung1_0, 0);
        Bezeichnung1.SetIndex(_Bezeichnung1_1, 1);
        
        Check2.SetIndex(_Check2_0, 0);
        Check2.SetIndex(_Check2_1, 1);
        
        Bef.SetIndex(_Bef_0, 0);
        Bef.SetIndex(_Bef_1, 1);
        Bef.SetIndex(_Bef_2, 2);
        Bef.SetIndex(_Bef_3, 3);
        Bef.SetIndex(_Bef_4, 4);
        
        Command1.SetIndex(_Command1_1, 1);
        Command1.SetIndex(_Command1_0, 0);

        Command1.AddClick(_viewModel.Command1_Click);
        Bef.AddClick(_viewModel.Bef_Click); 
 //       Bez.AddClick(_viewModel.Bez_Click);
        TextBindingAttribute.Commit(this, viewModel);
        CheckedBindingAttribute.Commit(this, viewModel);
        CommandBindingAttribute.Commit(this, viewModel);
        VisibilityBindingAttribute.Commit(this, viewModel);
    }

}
