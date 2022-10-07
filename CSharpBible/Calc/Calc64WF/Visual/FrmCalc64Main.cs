// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="FrmCalc64Main.cs" company="HP Inc.">
//     Copyright © HP Inc. 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Calc64Base;
using Calc64WF.ViewModel;
using Calc64WF.Visual.Converter;
using MVVM.ViewModel;

namespace Calc64WF.Visual
{
    /// <summary>
    /// Class FrmCalc64Main.
    /// Implements the <see cref="Form" />
    /// </summary>
    /// <seealso cref="Form" />
    public partial class FrmCalc64Main : Form
    {
        /// <summary>
        /// The vc op to string
        /// </summary>
        private OperationModeToShortString vcOpToString;
        /// <summary>
        /// The i margin
        /// </summary>
        const int iMargin = 2;

        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>The data context.</value>
        public NotificationObject DataContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmCalc64Main" /> class.
        /// </summary>
        public FrmCalc64Main()
        {
            InitializeComponent();
           
            FrmCalc64MainViewModel vm;
            DataContext = vm = new FrmCalc64MainViewModel();
            vm.OnDataChanged += DataChanged;
            vm.CloseForm += DoCloseForm;
            vm.PressNumberKey += DoPressNumberkey;

            foreach (Control c in Controls)
            {
                if (c != pictureBox1 && c != pnlMaster)
                {
                    c.Location = new Point(norm(c.Location.X,24),norm(c.Location.Y,24,10));
                    c.Size = new Size(norm(c.Size.Width,24,-iMargin * 4),norm(c.Size.Height,24, -iMargin * 4));
                }
            }

            vcOpToString = new OperationModeToShortString();
            pnlMaster.Dock = DockStyle.Fill;
            //            pnlMaster_SizeChanged(this, null);

            int norm(int i, int n,int o=0) => 
                (i + n / 2) - (i+n-o + n / 2) % n;
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
        private void DataChanged(object sender, (string prop, object oldVal, object newVal) e)
        {
            if (sender is Calc64Model cc)
            {
                lblResult.Text = cc.Accumulator.ToString();
                lblMemory.Text = cc.Memory.ToString();
                lblOperation.Text = (string)vcOpToString.Convert(cc.OperationMode,typeof(string),lblOperation.Tag,CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// Does the close form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DoCloseForm(object sender,EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the MouseMove event of the FrmCalc32Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void FrmCalc32Main_MouseMove(object sender, MouseEventArgs e)
        {
            Point lMousePnt = e.Location;
            if (sender!= this)
            {
                lMousePnt.X += ((Control)sender).Location.X;
                lMousePnt.Y += ((Control)sender).Location.Y;
            }
            lMousePnt.Offset(-pictureBox1.Size.Width/2, -pictureBox1.Size.Height/2);
            pictureBox1.Location = lMousePnt;
        }

        /// <summary>
        /// Handles the Click event of the btnNummber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnNummber_Click(object sender, EventArgs e)
        {
            (DataContext as FrmCalc64MainViewModel).btnNummber_Click(sender, ((Control)sender).Tag, e);
        }

        /// <summary>
        /// Handles the KeyDown event of the FrmCalc32Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void FrmCalc32Main_KeyDown(object sender, KeyEventArgs e)
        {
            (DataContext as FrmCalc64MainViewModel).frm_KeyDown(sender, ((Control)sender).Tag, e);
        }

        /// <summary>
        /// Handles the Click event of the btnOperator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnOperator_Click(object sender, EventArgs e)
        {
            (DataContext as FrmCalc64MainViewModel).btnOperator_Click(sender, ((Control)sender).Tag, e);
        }

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            (DataContext as FrmCalc64MainViewModel).btnClose_Click(sender, ((Control)sender).Tag, e);
        }

        /// <summary>
        /// Handles the Click event of the btnBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            (DataContext as FrmCalc64MainViewModel).btnBack_Click(sender, ((Control)sender).Tag, e);
        }

        /// <summary>
        /// Handles the SizeChanged event of the pnlMaster control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void pnlMaster_SizeChanged(object sender, EventArgs e)
        {
            GraphicsPath gPath = new GraphicsPath();
            foreach (Control c in Controls)
            {
                if (c != pictureBox1 )
                {
                    var r = new Rectangle(c.Location,c.Size);
                    r.Inflate(iMargin, iMargin);
                    
                    gPath.AddRectangle(r);
                }
            }
            var region = new Region(gPath);
      //      region.Complement(gPath);
            pnlMaster.Region = region;
        }
    }
}
