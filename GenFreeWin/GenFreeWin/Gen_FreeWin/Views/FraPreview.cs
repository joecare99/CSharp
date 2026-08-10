using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using GenFree;
using GenFree.Interfaces;
using GenFree.Interfaces.Sys;

namespace GenFreeWin.Views
{
    public partial class FraPreview : UserControl, IDocument
    {
        public bool IsEmpty => edtText.SelectionStart == 0;
        private IModul1 Modul1 => _Modul1.Instance;
        public event EventHandler<EventArgs> OnClose;

        public FraPreview()
        {
            InitializeComponent();
        }

        private void btnSaveText_Click(object sender, EventArgs e)
        {
            CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
            CommonDialog1Save.FilterIndex = 2;
            CommonDialog1Save.InitialDirectory = Modul1.GenFreeDir + "\\list\\";
            _ = CommonDialog1Save.ShowDialog();
            CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
            if (CommonDialog1Save.FileName != "")
            {
                switch (CommonDialog1Save.FilterIndex)
                {
                    case 1:
                        edtText.SaveFile(CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                        break;
                    case 2:
                        edtText.SaveFile(CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                        break;
                }
            }
        }

        private void btn_Commend2_1_Click(object sender, EventArgs e)
        {
            Visible = false;
            OnClose?.Invoke(this, EventArgs.Empty);
            /*          An = 0;
                      ComboBox2.Visible = true;
                      cbxProperty.Visible = true;
                      btnEnterNew[3].Visible = true;
                      if (Modul1.Schalt == 4)
                      {
                          Hide();
                      }
                      if (Modul1.Schalt == 9)
                      {
                          btnEnterNew[0].PerformClick();
                      }
                      Modul1.UbgT1 = "";*/
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DocumentRenew();
            Modul1.Ausdruck("\\Text2.RTF");
        }

        public void DocumentRenew()
        {
            if (Modul1.Typ == DriveType.CDRom)
            {
                edtText.SaveFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                edtText.LoadFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
            }
            else
            {
                edtText.SaveFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                edtText.LoadFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                edtText.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                edtText.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
            }
        }

        public void AdjustLayout()
        {
            edtText.Top = 10;
            edtText.Width = this.Width - 40;
            edtText.Left = 10;
            edtText.Height = tableLayoutPanel1.Top - 20;
            edtText.RightMargin = edtText.Width - 20;
        }
        public void ClearDocument()
        {
            edtText.Clear();
        }

        public void SetFont(Font font)
        {
            edtText.SelectionFont = font;
        }

        public void AppendText(string text)
        {
            edtText.SelectedText = text;
        }

        public void SetAlignment<T>(T eTextAlign) where T : Enum
        {
            edtText.SelectionAlignment = eTextAlign is HorizontalAlignment e ? e : (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), eTextAlign.ToString());
        }

        public bool AppendTextIfNd(string sText = "\n", int iCnt = 1)
        {
            var sTest = "";
            var result = false;
            for (int i = 0; i < iCnt; i++)
            {
                if (result = !edtText.Text.EndsWith(sTest += sText))
                    AppendText(sText);
            }

            return result;
        }

        public void SetIndent(int iIndent)
        {
            edtText.SelectionIndent = iIndent;
        }

        public int GetIndent()
        {
            return edtText.SelectionIndent;
        }

        public bool TrimEnd()
        {
            var result = edtText.SelectionStart > 0 && Strings.Mid(edtText.Text, edtText.SelectionStart, 1) == " ";
            while (edtText.SelectionStart > 0 && Strings.Mid(edtText.Text, edtText.SelectionStart, 1) == " ")
            {
                edtText.SelectionStart = checked(edtText.SelectionStart - 1);
                edtText.SelectionLength = 1;
                edtText.AppendText("");
            }
            return result;
        }

        public bool TrimEnd(string sText)
        {
            bool result;
            if (result = edtText.SelectionStart > 0 && Strings.Mid(edtText.Text, edtText.SelectionStart, sText.Length) == sText)
            {
                edtText.SelectionStart = checked(edtText.SelectionStart - sText.Length);
                edtText.SelectionLength = 1 + sText.Length;
                edtText.AppendText("");
            }
            return result;
        }

        public void AppendImage(Image image)
        {
            Clipboard.SetImage(image);
            edtText.Paste();
            Clipboard.Clear();
        }

        public void SetHangingIndent(int iHIndent)
        {
            edtText.SelectionHangingIndent = iHIndent;
        }

        private void FraPreview_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;
            btnNew.Text = Modul1.IText[EUserText.tNMPrint];
            btnClose.Text = Modul1.IText[EUserText.tNMBack];
            btnSaveText.Text = Modul1.IText[EUserText.tNMSave];
        }

        public void ReplaceLast(string v1, string v2)
        {
            if (TrimEnd(v1))
                AppendText(v2);
        }
    }
}
