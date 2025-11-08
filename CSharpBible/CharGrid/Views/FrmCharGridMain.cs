// ***********************************************************************
// Assembly : CharGrid
// Author : Mir
// Created :12-19-2021
//
// Last Modified By : GitHub Copilot
// Last Modified On :11-05-2025
// ***********************************************************************
using CSharpBible.CharGrid.ViewModels.Interfaces;
using System;
using System.Windows.Forms;
using Views;

namespace CSharpBible.CharGrid.Views
{
    /// <summary>
    /// Class FrmCharGridMain.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmCharGridMain : Form
    {
        private readonly ICharGridViewModel _vm;

        // Inject ViewModel
        public FrmCharGridMain(ICharGridViewModel vm)
        {
            _vm = vm;
            InitializeComponent();
            Load += FrmCharGridMain_Load;
            CommandBindingAttribute.Commit(this, vm);
        }

        private void FrmCharGridMain_Load(object sender, EventArgs e)
        {
            var dgv = Controls["dgvChars"] as DataGridView; // expects a DataGridView named dgvChars
            if (dgv == null) return;

            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;

            dgv.ColumnCount = _vm.ColumnCount;
            dgv.RowCount = _vm.RowCount;

            for (int r = 0; r < _vm.RowCount; r++)
            {
                for (int c = 0; c < _vm.ColumnCount; c++)
                {
                    dgv[c, r].Value = _vm.Rows[r][c];
                }
            }
        }
    }
}
