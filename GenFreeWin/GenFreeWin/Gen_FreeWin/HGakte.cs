using Gen_FreeWin.ViewModels.Interfaces;
using GenFreeWin.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Gen_FreeWin.Views;

public partial class HGakte : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    private IHGAkteViewModel _viewModel;

    [DebuggerNonUserCode]
    public HGakte(IHGAkteViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        _viewModel.DoClose = Close;
        _viewModel.View_InitView = InitView;
        Load += _viewModel.Form_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
    }

    private void InitView(FormWindowState WiS, float fontSize)
    {

        if (fontSize > 0f)
        {
            Font = new Font("Arial", fontSize, FontStyle.Regular);
            GroupBoxUsage.Font = new Font("Arial", fontSize, FontStyle.Regular);
        }
        BackColor = Menue.Default.BackColor;
        WindowState = WiS == FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
    }

    private void Button8_Click(object sender, EventArgs e) => throw new NotImplementedException();

    private void ListBox1_DoubleClick(object sender, EventArgs e) => throw new NotImplementedException();

    private void Command2_Click(object sender, EventArgs e) => throw new NotImplementedException();

    private void Command1_Click(object sender, EventArgs e) => throw new NotImplementedException();

    private void Label7_Click(object sender, EventArgs e) => throw new NotImplementedException();

    private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e) => throw new NotImplementedException();

}