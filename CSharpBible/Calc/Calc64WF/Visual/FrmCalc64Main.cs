// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="FrmCalc64Main.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Calc64WF.ViewModels.Interfaces;
using Calc64WF.Visual.Converter;

namespace Calc64WF.Visual
{
    /// <summary>
    /// Class FrmCalc64Main.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmCalc64Main : Form
    {
        private readonly object[][] btnDef = new object[][]
        {
            new object[]{"0",  2,2,3,2},
            new object[]{"+/-",5,2,3,2},
            new object[]{"1",2,4,2,2},
            new object[]{"2",4,4,2,2},
            new object[]{"3",6,4,2,2},
            new object[]{"4",2,6,2,2},
            new object[]{"5",4,6,2,2},
            new object[]{"6",6,6,2,2},
            new object[]{"7",2,8,2,2},
            new object[]{"8",4,8,2,2},
            new object[]{"9",6,8,2,2},
            new object[]{"10",2,10,2,2},
            new object[]{"11",4,10,2,2},
            new object[]{"12",6,10,2,2},
            new object[]{"13",2,12,2,2},
            new object[]{"14",4,12,2,2},
            new object[]{"15",6,12,2,2},
            new object[]{"-1",0,4,2,4},
            new object[]{"-2",0,6,2,2},
            new object[]{"-3",0,8,2,2},
            new object[]{"-4",0,10,2,2},
            new object[]{"-5",0,12,2,2},
            new object[]{"-6",8,12,3,3},
            new object[]{"-7",8,9,3,3},
            new object[]{"-8",8,6,3,3},
            new object[]{"-9",8,3,3,3},
            new object[]{"+0",-14,4,8,4},
            new object[]{"+1",-4,12,4,2},
            new object[]{"+2",-14,12,4,2},
            new object[]{"+3",-4,10,4,2},
        };


        #region Properties
        /// <summary>
        /// The vc op to string
        /// </summary>
        private OperationModeToShortString vcOpToString;
        /// <summary>
        /// The i margin
        /// </summary>
        const int iMargin = 2;
#if NET6_0_OR_GREATER
        const int iRaster = 24;
#else
        const int iRaster = 22;
#endif
        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if NET7_0_OR_GREATER
        public new IFrmCalc64MainViewModel DataContext { get; private set; }
#else
        public IFrmCalc64MainViewModel DataContext { get; private set; }
#endif
#endregion
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FrmCalc64Main" /> class.
        /// </summary>
        public FrmCalc64Main(IFrmCalc64MainViewModel vm)
        {
            InitializeComponent();

#if NET6_0_OR_GREATER
            this.Text += " NET6.0";
#else
            this.Text += " FW4.8";
#endif
            DataContext = vm;
            vm.OnDataChanged += DataChanged;
            vm.CloseForm += DoCloseForm;

            CommandBindingAttribute.Commit(this, vm);

            foreach (Control c in Controls)
            {
                if (c != pictureBox1 && c != pnlMaster)
                {
                    var xFlag = true;
                    for (int i = 0; i < btnDef.Length; i++)
                        if (c.Tag == btnDef[i][0])
                        {
                            xFlag = false;
                            c.Location = new Point(ClientRectangle.Width / 2 + (((int)btnDef[i][1] + 2) * iRaster), ClientRectangle.Height - (((int)btnDef[i][2] + 1) * iRaster));
                            c.Size = new Size(((int)btnDef[i][3] * iRaster) - iMargin * 4, ((int)btnDef[i][4] * iRaster) - iMargin * 4);
                        }
                    if (xFlag)
                    {
                        c.Location = new Point(norm(c.Location.X, iRaster), norm(c.Location.Y, iRaster, 10));
                        c.Size = new Size(norm(c.Size.Width, iRaster, -iMargin * 4), norm(c.Size.Height, iRaster, -iMargin * 4));
                    }
                }
            }

            vcOpToString = new OperationModeToShortString();
            pnlMaster.Dock = DockStyle.Fill;
            //            pnlMaster_SizeChanged(this, null);

            int norm(int i, int n, int o = 0) =>
                (i + n / 2) - (i + n - o + n / 2) % n;
        }


        /// <summary>
        /// Does the press numberkey.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DoPressNumberkey(object sender, char e)
        {
            foreach (Control c in Controls)
            {
                if ((c.GetType() == typeof(Button)) && (c.Text[0] == e))
                {
                    ((Button)c).Select();
                    ((Button)c).PerformClick();
                }
            }
        }

        /// <summary>
        /// Change event of Calculators class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void DataChanged(object sender, (string prop, object? oldVal, object? newVal) e)
        {
            switch (e.prop)
            {
                case nameof(IFrmCalc64MainViewModel.Accumulator):
                    lblResult.Text = e.newVal?.ToString() ?? "";
                    break;
                case nameof(IFrmCalc64MainViewModel.OperationText):
                    lblOperation.Text = e.newVal?.ToString() ?? "";
                    break;
                case nameof(IFrmCalc64MainViewModel.Memory):
                    lblMemory.Text = e.newVal?.ToString() ?? "";
                    break;

            }
        }

        /// <summary>
        /// Does the close form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DoCloseForm(object? sender, EventArgs e) => Close();

        /// <summary>
        /// Handles the MouseMove event of the FrmCalc32Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void FrmCalc32Main_MouseMove(object sender, MouseEventArgs e)
        {
            Point lMousePnt = e.Location;
            if (sender != this)
            {
                lMousePnt.X += ((Control)sender).Location.X;
                lMousePnt.Y += ((Control)sender).Location.Y;
            }
            lMousePnt.Offset(-pictureBox1.Size.Width / 2, -pictureBox1.Size.Height / 2);
            pictureBox1.Location = lMousePnt;
        }

        #region Relay-Events    
        /// <summary>
        /// Handles the KeyDown event of the FrmCalc32Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void FrmCalc32Main_KeyDown(object sender, KeyEventArgs e)
            => DataContext.frm_KeyDown(sender, ((Control)sender)?.Tag, e);

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnClose_Click(object sender, EventArgs e)
            => DataContext.btnClose_Click(sender, ((Control)sender)?.Tag, e);

        private void btnDefault_Click(object sender, EventArgs e)
            => DataContext.OperationCommand?.Execute(((Control)sender)?.Tag);

        #endregion

        /// <summary>
        /// Handles the SizeChanged event of the pnlMaster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void pnlMaster_SizeChanged(object sender, EventArgs e)
        {
            Application.Idle += SetMasterRegionWhenIdle;
            SetMasterRegion();
        }

        private void SetMasterRegionWhenIdle(object? sender, EventArgs e)
        {
            Application.Idle -= SetMasterRegionWhenIdle;
            SetMasterRegion();
        }

        private void SetMasterRegion()
        {
            GraphicsPath gPath = new GraphicsPath();
            foreach (Control c in Controls)
            {
                if (c != pictureBox1)
                {
                    var r = new Rectangle(c.Location, c.Size);
                    r.Inflate(iMargin, iMargin);

                    gPath.AddRectangle(r);
                }
            }
            var region = new Region(gPath);
            //      region.Complement(gPath);
            pnlMaster.Region = region;
        }
        #endregion
    }
}
