using BaseLib.Helper;
using GenFree;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Ortsver : Form
{
    public static Ortsver Instance { get=>field ??= IoC.GetRequiredService<Ortsver>(); private set; }

    private static List<WeakReference> __ENCList = new();

    private IOrtsVerViewModel _viewModel;

    public Keys ModifierKeys => Form.ModifierKeys;
    public Ortsver(IOrtsVerViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;
        Load += _viewModel.Form_Load;
        Load += Ortsver_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
        CommandBindingAttribute.Commit(this, _viewModel);
        Instance = this;
    }

    private void Ortsver_Load(object sender, EventArgs e)
    {
        RTB1.AddContextMenu();
        if (_Modul1.Instance.FontSize > 0f)
        {
            var Font = new Font("Arial", _Modul1.Instance.FontSize, FontStyle.Regular);
            ListBox2.Font = new Font("Courier New", _Modul1.Instance.FontSize, FontStyle.Regular);
            Frame1.Font    = Font;
            Label19.Font   = Font;
            Label20.Font   = Font;
            Label21.Font   = Font;
            Button13.Font  = Font;
            Button14.Font  = Font;
            Button15.Font  = Font;
            TextBox22.Font = Font;
            TextBox23.Font = Font;
            TextBox24.Font = Font;
            TextBox25.Font = Font;
            TextBox26.Font = Font;
            TextBox27.Font = Font;
            TextBox28.Font = Font;
            TextBox29.Font = Font;
        }

        var aiPos = _Modul1.Instance.Persistence.ReadIntsProg("maspos.dat", 2);
        Left = aiPos[0];
        Top = aiPos[1];
        Text = $"{_Modul1.Instance.AppName} Ortsverwaltung für Mandant {_Modul1.Instance.Mandant}";
        BackColor = _Modul1.Instance.HintFarb;
        _Modul1.Instance.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
        WindowState = WiS;
    }

    private void TextBox1_KeyUp(object s, KeyEventArgs e) => _viewModel.TextBox1_KeyUp(s, e);
    private void Textbox1_KeyPress(object s, KeyPressEventArgs e) => _viewModel.Textbox1_KeyPress(s, e);
    private void TextBox13_TextChanged(object s, EventArgs e) => _viewModel.TextBox13_TextChanged(s, e);
    private void TextBox30_TextChanged(object s, EventArgs e) => _viewModel.TextBox30_TextChanged(s, e);
    private void TextBox31_TextChanged(object s, EventArgs e) => _viewModel.TextBox31_TextChanged(s, e);
    private void TextBox32_TextChanged(object s, EventArgs e) => _viewModel.TextBox32_TextChanged(s, e);

    private void ListBox1_DoubleClick(object s, EventArgs e) => _viewModel.ListBox1_DoubleClick(s, e);
    private void ListBox2_DoubleClick(object s, EventArgs e) => _viewModel.ListBox2_DoubleClick(s, e);
    private void ListBox3_DoubleClick(object s, EventArgs e) => _viewModel.ListBox3_DoubleClick(s, e);
    private void ListBox4_DoubleClick(object s, EventArgs e) => _viewModel.ListBox4_DoubleClick(s, e);

    private void Label27_TextChanged(object s, EventArgs e) => _viewModel.Label27_TextChanged(s, e);
    private void Label32_TextChanged(object s, EventArgs e) => _viewModel.Label32_TextChanged(s, e);

    private void Button24_Click(object s, EventArgs e) => _viewModel.Button24_Click(s, e);
    private void Button23_Click(object s, EventArgs e) => _viewModel.Button23_Click(s, e);
    private void Button22_Click(object s, EventArgs e) => _viewModel.Button22_Click(s, e);
    private void Button21_Click(object s, EventArgs e) => _viewModel.Button21_Click(s, e);
    private void Button20_Click(object s, EventArgs e) => _viewModel.Button20_Click(s, e);
    private void Button19_Click(object s, EventArgs e) => _viewModel.Button19_Click(s, e);
    private void Button18_Click(object s, EventArgs e) => _viewModel.Button18_Click(s, e);
    private void Button17_Click(object s, EventArgs e) => _viewModel.Button17_Click(s, e);
    private void Button16_Click(object s, EventArgs e) => _viewModel.Button16_Click(s, e);
    private void Button15_Click(object s, EventArgs e) => _viewModel.Button15_Click(s, e);
    private void Button14_Click(object s, EventArgs e) => _viewModel.Button14_Click(s, e);
    private void Button13_Click(object s, EventArgs e) => _viewModel.Button13_Click(s, e);
    private void Button12_Click(object s, EventArgs e) => _viewModel.Button12_Click(s, e);
    private void Button11_Click(object s, EventArgs e) => _viewModel.Button11_Click(s, e);
    private void Button10_Click(object s, EventArgs e) => _viewModel.Button10_Click(s, e);
    private void RTB1_KeyUp(object s, KeyEventArgs e) => _viewModel.RTB1_KeyUp(s, e);
    
    
}
