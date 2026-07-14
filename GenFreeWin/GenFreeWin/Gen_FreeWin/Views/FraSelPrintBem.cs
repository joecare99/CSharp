using BaseLib.Helper;
using GenFree;
using GenFree.Interfaces.Sys;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace GenFreeWin.Views
{

    public partial class FraSelPrintBem : UserControl
    {
        IModul1 Modul1 => _Modul1.Instance;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ControlArray<CheckBox> ChkNotes { get; private set; }

        public FraSelPrintBem()
        {
            InitializeComponent();
        }

        private void FraSelPrintBem_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;
            ChkNotes = new ControlArray<CheckBox>();
            ChkNotes.SetIndex(chbSelNotePerson, EFraSelPrintNotes.o01_Person.AsInt());
            ChkNotes.SetIndex(chbSelNoteUpperPersDate, EFraSelPrintNotes.UpperPersonDate.AsInt());
            ChkNotes.SetIndex(chbSelNoteLowerPersDate, EFraSelPrintNotes.LowerPersonDate.AsInt());
            ChkNotes.SetIndex(chbSelNoteFamily, EOutCfg.o04_Family.AsInt());
            ChkNotes.SetIndex(chbSelNoteUpperFamDate, EFraSelPrintNotes.UpperFamilyDate.AsInt());
            ChkNotes.SetIndex(chbSelNoteLowerFamDate, EFraSelPrintNotes.LowerFamilyDate.AsInt());
            ChkNotes.SetIndex(chbSelNoteKeepFormat, EFraSelPrintNotes.o07_KeepFormat.AsInt());
            lblSelHdrNotesOption.Text = Modul1.IText[EUserText.t218];
            chbSelNotePerson.Text = Modul1.IText[EUserText.t219];
            chbSelNoteUpperPersDate.Text = Modul1.IText[EUserText.t220];
            chbSelNoteLowerPersDate.Text = Modul1.IText[EUserText.t221];
            chbSelNoteFamily.Text = Modul1.IText[EUserText.t222];
            chbSelNoteUpperFamDate.Text = Modul1.IText[EUserText.t223];
            chbSelNoteLowerFamDate.Text = Modul1.IText[EUserText.t224];
            chbSelNoteKeepFormat.Text = Modul1.IText[EUserText.t347];
        }

        public bool[] GetState()
        {
            bool[] result = new bool[ChkNotes.Keys.Max() + 1];
            for (int i = 0; i < result.Count(); i++)
                if (ChkNotes.TryGetValue(i, out var c))
                    result[i] = c.Checked;
            return result;
        }

        public void SetState(bool[] state)
        {
            for (int i = 0; i < state.Count(); i++)
                if (ChkNotes.TryGetValue(i, out var c))
                    c.Checked = state[i];
        }
    }
}
