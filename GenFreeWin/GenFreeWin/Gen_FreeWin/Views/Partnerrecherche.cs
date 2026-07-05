using BaseLib.Helper;
using GenFree.Helper;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

[DesignerGenerated]
public partial class Partnerrecherche : Form
{
	private static readonly List<WeakReference> __ENCList = new();


    private IPartnerRechercheViewModel _viewModel;

    [DebuggerNonUserCode]
	public Partnerrecherche(IPartnerRechercheViewModel viewModel)
	{
		_viewModel = viewModel;
		_viewModel.View = this;
		Resize += _viewModel.Partnerrecherche_Resize;
		Load += _viewModel.Partnerrecherche_Load;
		lock (__ENCList)
		{
			__ENCList.Add(new WeakReference(this));
		}
		Label2 = new ControlArray<System.Windows.Forms.Label>();
		Text1 = new ControlArray<System.Windows.Forms.TextBox>();
		InitializeComponent();
		Text1.SetIndex(_Text1_2, 2);
		Text1.SetIndex(_Text1_1, 1);
		Text1.SetIndex(_Text1_0, 0);
		Label2.SetIndex(_Label2_1, 1);
		Label2.SetIndex(_Label2_0, 0);
		Text1.AddKeyPress(_viewModel.Text1_KeyPress);
		Text1.AddTextChanged(_viewModel.Text1_TextChanged);

	}

    private void Command1_Click(object s, EventArgs e) => _viewModel.Command1_Click(s, e);
    private void Command2_Click(object s, EventArgs e) => _viewModel.Command2_Click(s, e);
    private void Command3_Click(object s, EventArgs e) => _viewModel.Command3_Click(s, e);
    private void List2_DoubleClick(object s, EventArgs e) => _viewModel.List2_DoubleClick(s, e);
    private void _Text1_0_TextChanged(object s, EventArgs e) => _viewModel._Text1_0_TextChanged(s, e);
    private void List1_DoubleClick(object s, EventArgs e) => _viewModel.List1_DoubleClick(s, e);
}
