// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="FrmCalc64MainViewModel.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using Calc64Base;
using MVVM.ViewModel;
using System;
using System.Windows.Forms;

namespace Calc64WF.ViewModel
{
    /// <summary>
    /// Class FrmCalc64MainViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class FrmCalc64MainViewModel :BaseViewModel
    {
        /// <summary>
        /// The calc64model
        /// </summary>
        private Calc64Model _calc64model = new Calc64Model();

        /// <summary>
        /// Occurs when [on data changed].
        /// </summary>
        public event EventHandler<(string prop, object oldVal, object newVal)> OnDataChanged;
        /// <summary>
        /// Occurs when [close form].
        /// </summary>
        public event EventHandler CloseForm;
        /// <summary>
        /// Occurs when [press number key].
        /// </summary>
        public event EventHandler<char> PressNumberKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmCalc64MainViewModel"/> class.
        /// </summary>
        public FrmCalc64MainViewModel()
        {
            _calc64model.CalcOperationChanged += (s, e) => OnDataChanged?.Invoke(s, e);
        }

        /// <summary>
        /// BTNs the close click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        internal void btnClose_Click(object sender, object tag, EventArgs e)
        {
            CloseForm?.Invoke(this,e);
        }

        /// <summary>
        /// BTNs the nummber click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        internal void btnNummber_Click(object sender,object tag, EventArgs e)
        {
            if (int.TryParse(tag.ToString(), out int aNumber))
            {
                _calc64model.Button(aNumber);
            }
        }

        /// <summary>
        /// BTNs the operator click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        internal void btnOperator_Click(object sender, object tag, EventArgs e)
        {
            if (int.TryParse(tag.ToString(), out int aNumber))
            {
                _calc64model.Operation(-aNumber);
            }
        }

        /// <summary>
        /// FRMs the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        internal void frm_KeyDown(object sender, object tag, KeyEventArgs e)
        {
            Char ActKey = (char)0;
            if ((char)e.KeyValue >= "0"[0] && (char)e.KeyValue <= "9"[0])
            {
                ActKey = (char)e.KeyValue;
            }
            else if ((char)e.KeyValue >= 96 && (char)e.KeyValue <= 105)
            {
                ActKey = (char)(e.KeyValue - 96 + (int)("0"[0]));
            }

            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    btnOperator_Click(sender,"-2",e);
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    btnOperator_Click(sender, "-3", e);
                    break;
                case Keys.Escape:
                    break;
                case Keys.Back:
                    btnOperator_Click(sender, "-2", e);
                    break;
                default:
                    if (ActKey != (char)0)
                        PressNumberKey?.Invoke(sender, ActKey);
                    break;
            }
        }

        /// <summary>
        /// BTNs the back click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        internal void btnBack_Click(object sender, object tag, EventArgs e)
        {
            _calc64model.BackSpace();
        }
    }
}
