using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GenFreeWin;


public partial class EinzelQuelle : Form
{
    IModul1 Modul1 => _Modul1.Instance;

    [DebuggerNonUserCode]
    public EinzelQuelle()
    {
        Load += EinzelQuelle_Load;

        InitializeComponent();
    }

    private void Command1_Click(object sender, EventArgs e)
    {
        if (Modul1.Typ != DriveType.CDRom)
        {
            if (Modul1.Aus[(int)EOutCfg.o46] == "Y" && edtAus.Text.Trim() == "")
            {
                edtAus.Text = "Seite:";
            }
            DataModul.CitationData.Clear();
            DataModul.CitationData.iLinkType = (short)lbl__QKenn.Tag.AsInt();
            DataModul.CitationData.iQuNr = lbl__QuellNr.Tag.AsInt();
            DataModul.CitationData.sSourceTitle = lblTitel.Tag.AsString();
            DataModul.CitationData.sPage = edtAus.Text;
            DataModul.CitationData.sEntry = edtEntry.Text;
            DataModul.CitationData.sOriginalText = edtOriginalText.Text;
            DataModul.CitationData.sComment = edtComment.Text;
            DataModul.CitationData.Commit(lbl__PerFamNr.Tag.AsInt(), Modul1.Art, Modul1.LfNR);
            edtOriginalText.Text = "";
            edtComment.Text = "";
        }
        Close();
    }

    private void EinzelQuelle_Load(object sender, EventArgs e)
    {
        if (Modul1.FontSize > 0f)
        {
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            edtComment.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            edtOriginalText.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
        Top = MainProject.Forms.Quellen.Top;
        Left = MainProject.Forms.Quellen.Left;
        Visible = true;
        _ = edtEntry.Focus();
    }

    private void RichTextBox1_KeyDown(object sender, KeyEventArgs e) => Modul1_TextEditorKeyTextSubstitution(sender as TextBoxBase, e.KeyCode);

    private void Modul1_TextEditorKeyTextSubstitution(TextBoxBase? textBox, Keys key)
    {
        if ((new int[] {0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
             0x7A, 0x7B }).IndexOf((int)key) >= 0 && textBox != null)
        {
            textBox.SelectedText = Modul1.Te[(short)key - 113];
        }
    }

    private void Text1_GotFocus(object sender, EventArgs e)
    {
        edtEntry.SelectionStart = edtEntry.Text.Length;
        edtEntry.ScrollToCaret();
    }

    private void Text1_TextChanged(object sender, EventArgs e)
    {
        if (edtEntry.Text.Length >= DataModul.DB_SourceLinkTable.Fields[3].Size)
        {
            _ = Interaction.MsgBox("Feldlänge erreicht, nur" + DataModul.DB_SourceLinkTable.Fields[3].Size.AsString() + " Zeichen möglich");
        }
    }

    private void Text1_KeyDown(object sender, KeyEventArgs e)
    {
        Modul1_TextEditorKeyTextSubstitution(sender as TextBoxBase, e.KeyCode);
    }

    private void Text2_GotFocus(object sender, EventArgs e)
    {
        edtEntry.SelectionStart = edtEntry.Text.Length;
        edtEntry.ScrollToCaret();
    }

    private void Text2_KeyDown(object sender, KeyEventArgs e)
    {
        //edtAus.Text = Modul1.Te[num - 113];
        //edtAus.SelectionStart = edtEntry.Text.Length;
        //edtAus.ScrollToCaret();
        Modul1_TextEditorKeyTextSubstitution(sender as TextBoxBase, e.KeyCode);
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        btnFinished.PerformClick();
        MainProject.Forms.Quellen.Button2.PerformClick();
    }

    public DialogResult ShowDialog(short qkenn, int iPerFam, int iQuellNr, string sSourceTitle)
    {
        SetData(qkenn, iPerFam, iQuellNr, sSourceTitle);
        return ShowDialog();
    }

    public void SetData(short qkenn, int iPerFamNr, int iSourceNr, string sSourceTitle)
    {
        lbl__QKenn.Tag = qkenn;
        lbl__QKenn.Text = qkenn.AsString();
        lbl__PerFamNr.Tag = iPerFamNr;
        lbl__PerFamNr.Text = iPerFamNr.AsString();
        lbl__QuellNr.Tag = iSourceNr;
        lbl__QuellNr.Text = iSourceNr.AsString();
        lblTitel.Text = $"Titel der Quelle: {sSourceTitle}";
        lblTitel.Tag = sSourceTitle;
    }
}
