using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Gen_FreeWin.Views
{
    public partial class FraNameSrchSelection : UserControl
    {
        private const string sHelpOrgSize = "Diese Funktion zeigt die Bilder im Originalformat. Hierdurch kann die Erstellung des Ausdrucks sehr lange dauern.\n Ebenso kann es vorkommen, das Bilder wegen Überschreitung der für RTF-Text vorgesehnen Größe nicht angezeigt werden.";
        IModul1 Modul1 => _Modul1.Instance;

        public ControlArray<CheckBox> Check1 { get; } = new();
        public ControlArray<CheckBox> Check2 { get; } = new();
        public List<int> PersonIDs { get; } = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EUserText eChb43Txt { set => chbPersonBaseDatesOnly.Text = Modul1.IText[value]; }

        public EExportPrivacy ePrintPrivacy => fraSelPrintPrivacy1.ePrintPrivacy;

        public string NumbersText { get => TextBox1.Text; }

        public FraNameSrchSelection()
        {
            InitializeComponent();

            Check1.SetIndex(chbSelEmitIDs, 0);
            Check1.SetIndex(chbSelEmitAncestNo, 1);
            Check1.SetIndex(chbSelEmitDescNo, 2);
            Check1.SetIndex(chbPicturePath, 3);
            Check1.SetIndex(chbStructured, 4);

            Check2.SetIndex(chbGodparents, 30);
            Check2.SetIndex(chbGodpWithoutData, 31);
            Check2.SetIndex(chbWitnesses, 32);
            Check2.SetIndex(chbWitnWithoutData, 33);
            Check2.SetIndex(chbEmitDocumentNo, 34);
            Check2.SetIndex(chbShortenPlaces, 35);
            Check2.SetIndex(chbWitnessOf, 37);
            Check2.SetIndex(chbGodparentOf, 38);
            Check2.SetIndex(chbEmitSources, 39);
            Check2.SetIndex(chbEmitPictures, 41);
            Check2.SetIndex(chbPersonPictOnly, 42);
            Check2.SetIndex(chbPersonBaseDatesOnly, 43);

            //           btnEnterNew.SetIndex(btnSelStart1, 5);
            //           btnEnterNew.SetIndex(btnSelStart2, 6);

        }

        public void ReadCheckBoxState(IList<bool> axOpt)
        {
            var bools = fraSelPrintBem1.GetState();
            for (int num4 = 1; num4 <= 7; num4++)
            {
                axOpt[num4] = bools[num4];
            }
            for (int num4 = 0; num4 <= 10; num4++)
                if (Check1.TryGetValue(num4, out var c))
                    axOpt[num4 + 10] = c.Checked;

            for (int num4 = 30; num4 <= 44; num4++)
                if (Check2.TryGetValue(num4, out var c))
                    axOpt[num4] = c.Checked;

            axOpt[40] = rbtBirthDeathOccuEtc.Checked;
        }

        public void SetCheckBoxState(IList<bool> axOpt)
        {
            for (int num4 = 0; num4 <= 10; num4++)
                if (Check2.TryGetValue(num4, out var c))
                    c.Checked = axOpt[num4 + 10];

            bool[] bools = new bool[8];
            for (int num4 = 1; num4 <= 7; num4++)
            {
                bools[num4] = axOpt[num4];
            }
            fraSelPrintBem1.SetState(bools);

            for (int num4 = 30; num4 <= 44; num4++)
                if (Check2.TryGetValue(num4, out var c))
                    c.Checked = axOpt[num4];

            rbtBirthOccuEtcDeath.Checked = true;
            rbtBirthDeathOccuEtc.Checked = axOpt[40];

        }

        private void FraNameSrchSelection_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;
            if (Modul1.FontSize > 0f)
                lblHelp.Font = new Font("Courier New", Modul1.FontSize, FontStyle.Bold);

            chbGodparents.Text = Modul1.IText[EUserText.tGodparents];
            chbGodpWithoutData.Text = Modul1.IText[EUserText.t338];
            chbWitnesses.Text = Modul1.IText[EUserText.t301];
            chbWitnWithoutData.Text = Modul1.IText[EUserText.t338];
            chbEmitDocumentNo.Text = Modul1.IText[EUserText.t333];
            chbShortenPlaces.Text = Modul1.IText[EUserText.t335];
            chbWitnessOf.Text = Modul1.IText[EUserText.t302];
            chbGodparentOf.Text = Modul1.IText[EUserText.tGodparentOf];
            chbEmitSources.Text = Modul1.IText[EUserText.t332];
            chbPersonBaseDatesOnly.Text = Modul1.IText[EUserText.t336];
            chbNoCauseOfDeath.Text = Modul1.IText[EUserText.t337];
            lblSelHdrDataorder.Text = Modul1.IText[EUserText.t342];
            lblSelHdrNumEnt.Text = Modul1.IText[EUserText.t349];
            rbtBirthDeathOccuEtc.Text = Modul1.IText[EUserText.t339];
            rbtBirthOccuEtcDeath.Text = Modul1.IText[EUserText.t340];
            Check1[0].Text = Modul1.IText[EUserText.t234];
            Check1[1].Text = Modul1.IText[EUserText.t329];
            Check1[2].Text = Modul1.IText[EUserText.t330];
            Check1[3].Text = Modul1.IText[EUserText.t331];
            Check1[4].Text = Modul1.IText[EUserText.t334];
        }
        private void lblHelp_Click(object sender, EventArgs e)
        {
            _ = Interaction.MsgBox(sHelpOrgSize);
        }
        private void chbEmitPictures_CheckStateChanged(object sender, EventArgs e)
        {
            if (chbEmitPictures.Checked)
            {
                chbPersonPictOnly.CheckState = CheckState.Unchecked;
            }
        }

        private void chbPersonPictOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (chbPersonPictOnly.Checked)
            {
                chbEmitPictures.CheckState = CheckState.Unchecked;
            }
        }

        private void chbPictOrginalSize_CheckedChanged(object sender, EventArgs e)
        {
            lblHelp.Visible = chbPictOrginalSize.Checked;
        }

    }
}
