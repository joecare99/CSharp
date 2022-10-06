using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpBible.Calc32.NonVisual;

namespace CSharpBible.Calc32.Visual
{
    public partial class FrmCalc32Main : Form
    {
        public FrmCalc32Main()
        {
            InitializeComponent();
        }

        private void calculatorClassChange(object sender, EventArgs e)
        {
            using (CalculatorClass cc = (CalculatorClass)sender)
            {
                lblResult.Text = cc.Akkumulator.ToString();
                lblMemory.Text = cc.Memory.ToString();
                lblOperation.Text = cc.OperationText;

            }
        }

        private void btnNummber_Click(object sender, EventArgs e)
        {
            if (int.TryParse(((Control)sender).Tag.ToString(), out int aNumber))
            {
                calculatorClass1.Button(aNumber);
            }
        }

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

        private void FrmCalc32Main_KeyDown(object sender, KeyEventArgs e)
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
                    btnPlus.PerformClick();
                    break;
                case Keys.OemMinus:
                case Keys.Subtract:
                    btnMinus.PerformClick();
                    break;
                case Keys.Escape:
                    break;
                case Keys.Back:
                    btnBack.PerformClick();
                    break;
                default:
                    if (ActKey != (char)0)
                    {
                        foreach (Control c in Controls)
                        {
                            if ((c.GetType() == typeof(Button)) && (c.Text[0] == ActKey))
                            {
                                ((Button)c).Select();
                                ((Button)c).PerformClick();
                            }
                        }

                    }
                    break;
            }
        }

        private void btnOperator_Click(object sender, EventArgs e)
        {
            if (int.TryParse(((Control)sender).Tag.ToString(), out int aNumber))
            {
                calculatorClass1.Operation(-aNumber);
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            calculatorClass1.BackSpace();
        }
    }
}
