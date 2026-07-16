using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.Data;

namespace GenFreeWin.Views
{
    public partial class FraHntSrcSelection : UserControl
    {
        #region Properties
        private List<(RadioButton, int, ESearchSelection)> _RadioButtons;
        private ESearchSelection _eSearchSelection = ESearchSelection.eManual;
        private Color _selectionColor = Color.FromArgb(unchecked((int)0xFFC0FFC0));
        private Color _unSelectionColor = Color.FromArgb(unchecked((int)0xFFFFFFC0));

        public FraHntSrcSelection()
        {
            InitializeComponent();
            _RadioButtons = new List<(RadioButton, int, ESearchSelection)>() {
            (RadioButton4, 0, ESearchSelection.eManual),
            (RadioButton5, 0, ESearchSelection.e2),
            (RadioButton6, 0, ESearchSelection.e3),
            (RadioButton7, 0, ESearchSelection.e4),
            (RadioButton8, 0, ESearchSelection.e5),
            (RadioButton9, 0, ESearchSelection.e6),
            (RadioButton10, 0, ESearchSelection.e7),
            (RadioButton11, 0, ESearchSelection.e8),
            (RadioButton12, 0, ESearchSelection.e9)};
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Caption { get => GroupBox4.Text; set => GroupBox4.Text = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ESearchSelection eSearchSelection { get=>_eSearchSelection; set => SetSelection(value); }

        public Color SelectionColor { get => _selectionColor; set => _selectionColor = value; }
        public Color UnSelectionColor { get => _unSelectionColor; set => _unSelectionColor = value; }
        public event EventHandler<ESearchSelection> eSearchSelectionChanged;
        #endregion

        #region Methods
        private void SetSelection(ESearchSelection value)
        {
            if (value == _eSearchSelection) return;
            _eSearchSelection = value;
            foreach (var l in _RadioButtons)
            {
                l.Item1.Checked = _eSearchSelection == l.Item3; 
            }
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var l in _RadioButtons)
            {
                l.Item1.BackColor = l.Item1.Checked ? _selectionColor : _unSelectionColor;
                if (l.Item1.Checked && _eSearchSelection != l.Item3)
                {
                    _eSearchSelection = l.Item3;
                    eSearchSelectionChanged?.Invoke(this, l.Item3);
                }     
            }
        }

        #endregion
    }
}
