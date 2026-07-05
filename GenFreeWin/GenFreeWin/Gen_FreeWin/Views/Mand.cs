using GenFree.ViewModels.Interfaces;
using System;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public partial class Mand : Form
{
    private IMandViewModel _viewModel;

    private void Text1_KeyPress(object sender, KeyPressEventArgs e) => _viewModel.Text1_KeyPress(sender,e);

    private void Command1_Click(object sender, EventArgs e) => _viewModel.Command1_Click(sender, e);

    private void Befehl2_Click(object sender, EventArgs e) => _viewModel.Befehl2_Click(sender, e);

    private void _CmdDeleteMandant_Click(object sender, EventArgs e) => _viewModel.CmdDeleteMandant_Click(sender, e);

    private void _CmdNewMandant_Click(object sender, EventArgs e) => _viewModel.CmdNewMandant_Click(sender, e);

    private void List1_DoubleClick(object sender, EventArgs e) 
        => _viewModel.List1_DoubleClick(sender, e);

    private void List1_Click(object sender, EventArgs e)
        => _viewModel.List1_Click(sender, e);

    private void Laufwerk1_SelectedIndexChanged(object sender, EventArgs e) 
        => _viewModel.Laufwerk1_SelectedIndexChanged(sender, e);

    public Mand(IMandViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Form_Load;
        InitializeComponent();
    }

}
