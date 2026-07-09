using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Druck
{
    [DesignerGenerated]
    internal class Ahnent : Form
    {
        private IContainer components;
        public IModul1 Modul1;

        public ToolTip ToolTip1;

        [AccessedThroughProperty("l1")]
        private TextBox _l1;

        [AccessedThroughProperty("_Command1_3")]
        private Button __Command1_3;

        [AccessedThroughProperty("_Command1_2")]
        private Button __Command1_2;

        [AccessedThroughProperty("_Command1_1")]
        private Button __Command1_1;

        [AccessedThroughProperty("_Command1_0")]
        private Button __Command1_0;

        [AccessedThroughProperty("_Command_0")]
        private Button __Command_0;

        [AccessedThroughProperty("_Command_1")]
        private Button __Command_1;

        [AccessedThroughProperty("_Label1_0")]
        private Label __Label1_0;

        [AccessedThroughProperty("_Label1_1")]
        private Label __Label1_1;

        [AccessedThroughProperty("_Label1_2")]
        private Label __Label1_2;

        [AccessedThroughProperty("_Label1_3")]
        private Label __Label1_3;

        [AccessedThroughProperty("_Label1_4")]
        private Label __Label1_4;

        [AccessedThroughProperty("_Label1_5")]
        private Label __Label1_5;

        [AccessedThroughProperty("Frame1")]
        private GroupBox _Frame1;

        [AccessedThroughProperty("Label2")]
        private Label _Label2;

        [AccessedThroughProperty("Command_Renamed")]
        private ControlArray<Button> _Command_Renamed;

        [AccessedThroughProperty("Command1")]
        private ControlArray<Button> _Command1;

        [AccessedThroughProperty("Label1")]
        private ControlArray<Label> _Label1;

        [AccessedThroughProperty("RTB1")]
        private RichTextBox _RTB1;

        private string[] Text1;

        private string Famdat;

        private string sterdat2;

        private string Sterdat;

        private string Gebdat2;

        private string Gebdat;

        private string Sterdat1;

        private string Gebdat1;

        private string VNam;

        private string Pername;

        private int S;

        private string Nr;

        private int I;
        private int M_Start;
        private byte Modul1_J; // Iterator for Kont1 array

        public virtual TextBox l1
        {
            get
            {
                return _l1;
            }

            set
            {
                _l1 = value;
            }
        }

        public virtual Button _Command1_3
        {
            get
            {
                return __Command1_3;
            }

            set
            {
                __Command1_3 = value;
            }
        }

        public virtual Button _Command1_2
        {
            get
            {
                return __Command1_2;
            }

            set
            {
                __Command1_2 = value;
            }
        }

        public virtual Button _Command1_1
        {
            get
            {
                return __Command1_1;
            }

            set
            {
                __Command1_1 = value;
            }
        }

        public virtual Button _Command1_0
        {
            get
            {
                return __Command1_0;
            }

            set
            {
                __Command1_0 = value;
            }
        }

        public virtual Button _Command_0
        {
            get
            {
                return __Command_0;
            }

            set
            {
                __Command_0 = value;
            }
        }

        public virtual Button _Command_1
        {
            get
            {
                return __Command_1;
            }

            set
            {
                EventHandler value2 = _Command_1_Click;
                if (__Command_1 != null)
                {
                    __Command_1.Click -= value2;
                }
                __Command_1 = value;
                if (__Command_1 != null)
                {
                    __Command_1.Click += value2;
                }
            }
        }

        public virtual Label _Label1_0
        {
            get
            {
                return __Label1_0;
            }

            set
            {
                __Label1_0 = value;
            }
        }

        public virtual Label _Label1_1
        {
            get
            {
                return __Label1_1;
            }

            set
            {
                __Label1_1 = value;
            }
        }

        public virtual Label _Label1_2
        {
            get
            {
                return __Label1_2;
            }

            set
            {
                __Label1_2 = value;
            }
        }

        public virtual Label _Label1_3
        {
            get
            {
                return __Label1_3;
            }

            set
            {
                __Label1_3 = value;
            }
        }

        public virtual Label _Label1_4
        {
            get
            {
                return __Label1_4;
            }

            set
            {
                __Label1_4 = value;
            }
        }

        public virtual Label _Label1_5
        {
            get
            {
                return __Label1_5;
            }

            set
            {
                __Label1_5 = value;
            }
        }

        public virtual GroupBox Frame1
        {
            get
            {
                return _Frame1;
            }

            set
            {
                _Frame1 = value;
            }
        }

        public virtual Label Label2
        {
            get
            {
                return _Label2;
            }

            set
            {
                _Label2 = value;
            }
        }

        public virtual ControlArray<Button> Command_Renamed
        {
            get
            {
                return _Command_Renamed;
            }

            set
            {
                EventHandler obj = Command_Renamed_Click;
                if (_Command_Renamed != null)
                {
                    _Command_Renamed.RemoveClick(obj);
                }
                _Command_Renamed = value;
                if (_Command_Renamed != null)
                {
                    _Command_Renamed.AddClick(obj);
                }
            }
        }

        public virtual ControlArray<Button> Command1
        {
            get
            {
                return _Command1;
            }

            set
            {
                EventHandler obj = Command1_Click;
                if (_Command1 != null)
                {
                    _Command1.RemoveClick(obj);
                }
                _Command1 = value;
                if (_Command1 != null)
                {
                    _Command1.AddClick(obj);
                }
            }
        }

        public virtual ControlArray<Label> Label1
        {
            get
            {
                return _Label1;
            }

            set
            {
                _Label1 = value;
            }
        }

        internal virtual RichTextBox RTB1
        {
            get
            {
                return _RTB1;
            }

            set
            {
                _RTB1 = value;
            }
        }

        [DebuggerNonUserCode]
        public Ahnent()
        {
            base.Load += Ahnent_Load;
            Text1 = new string[1301];
            InitializeComponent();
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool Disposing)
        {
            if (Disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(Disposing);
        }

        [System.Diagnostics.DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.l1 = new System.Windows.Forms.TextBox();
            this._Command1_3 = new System.Windows.Forms.Button();
            this._Command1_2 = new System.Windows.Forms.Button();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this._Command_0 = new System.Windows.Forms.Button();
            this._Command_1 = new System.Windows.Forms.Button();
            this._Label1_0 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_3 = new System.Windows.Forms.Label();
            this._Label1_4 = new System.Windows.Forms.Label();
            this._Label1_5 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Command_Renamed = new ControlArray<Button>();
            this.Command1 = new ControlArray<Button>();
            this.Label1 = new ControlArray<Label>();
            this.RTB1 = new System.Windows.Forms.RichTextBox();
            this.Frame1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
            this.SuspendLayout();
            this.l1.AcceptsReturn = true;
            this.l1.BackColor = System.Drawing.SystemColors.Window;
            this.l1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.l1.ForeColor = System.Drawing.SystemColors.WindowText;
            System.Drawing.Point point2 = this.l1.Location = new System.Drawing.Point(575, 678);
            this.l1.MaxLength = 0;
            this.l1.Name = "l1";
            this.l1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            System.Drawing.Size size2 = this.l1.Size = new System.Drawing.Size(167, 22);
            this.l1.TabIndex = 19;
            this.l1.Text = "Text1";
            this.l1.Visible = false;
            this._Command1_3.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_3, 3);
            point2 = this._Command1_3.Location = new System.Drawing.Point(223, 706);
            this._Command1_3.Name = "_Command1_3";
            this._Command1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_3.Size = new System.Drawing.Size(191, 25);
            this._Command1_3.TabIndex = 17;
            this._Command1_3.Text = "Drucken A4 Hochformat";
            this._Command1_3.UseVisualStyleBackColor = false;
            this._Command1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_2, 2);
            point2 = this._Command1_2.Location = new System.Drawing.Point(437, 706);
            this._Command1_2.Name = "_Command1_2";
            this._Command1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_2.Size = new System.Drawing.Size(83, 25);
            this._Command1_2.TabIndex = 11;
            this._Command1_2.Text = "&In Datei";
            this._Command1_2.UseVisualStyleBackColor = false;
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_1, 1);
            point2 = this._Command1_1.Location = new System.Drawing.Point(10, 706);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_1.Size = new System.Drawing.Size(207, 32);
            this._Command1_1.TabIndex = 10;
            this._Command1_1.Text = "Drucken A4 Querformat";
            this._Command1_1.UseVisualStyleBackColor = false;
            this._Command1_1.Visible = false;
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_0, 0);
            point2 = this._Command1_0.Location = new System.Drawing.Point(536, 706);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_0.Size = new System.Drawing.Size(91, 25);
            this._Command1_0.TabIndex = 9;
            this._Command1_0.Text = "Druck&menü";
            this._Command1_0.UseVisualStyleBackColor = false;
            this.Frame1.BackColor = System.Drawing.Color.Red;
            this.Frame1.Controls.Add(this._Command_0);
            this.Frame1.Controls.Add(this._Command_1);
            this.Frame1.Controls.Add(this._Label1_0);
            this.Frame1.Controls.Add(this._Label1_1);
            this.Frame1.Controls.Add(this._Label1_2);
            this.Frame1.Controls.Add(this._Label1_3);
            this.Frame1.Controls.Add(this._Label1_4);
            this.Frame1.Controls.Add(this._Label1_5);
            this.Frame1.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Frame1.Location = new System.Drawing.Point(282, 289);
            this.Frame1.Name = "Frame1";
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Frame1.Size = new System.Drawing.Size(407, 176);
            this.Frame1.TabIndex = 0;
            this.Frame1.TabStop = false;
            this._Command_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command_Renamed.SetIndex(this._Command_0, 0);
            point2 = this._Command_0.Location = new System.Drawing.Point(35, 137);
            this._Command_0.Name = "_Command_0";
            this._Command_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command_0.Size = new System.Drawing.Size(106, 30);
            this._Command_0.TabIndex = 2;
            this._Command_0.Text = "Abbrechen";
            this._Command_0.UseVisualStyleBackColor = false;
            this._Command_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command_Renamed.SetIndex(this._Command_1, 1);
            point2 = this._Command_1.Location = new System.Drawing.Point(245, 137);
            this._Command_1.Name = "_Command_1";
            this._Command_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command_1.Size = new System.Drawing.Size(106, 30);
            this._Command_1.TabIndex = 1;
            this._Command_1.Text = "OK";
            this._Command_1.UseVisualStyleBackColor = false;
            this._Label1_0.BackColor = System.Drawing.Color.White;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_0, 0);
            point2 = this._Label1_0.Location = new System.Drawing.Point(7, 35);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_0.Size = new System.Drawing.Size(393, 15);
            this._Label1_0.TabIndex = 8;
            this._Label1_1.BackColor = System.Drawing.Color.White;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_1, 1);
            point2 = this._Label1_1.Location = new System.Drawing.Point(7, 77);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_1.Size = new System.Drawing.Size(393, 15);
            this._Label1_1.TabIndex = 7;
            this._Label1_2.BackColor = System.Drawing.Color.White;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_2, 2);
            point2 = this._Label1_2.Location = new System.Drawing.Point(7, 98);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_2.Size = new System.Drawing.Size(393, 15);
            this._Label1_2.TabIndex = 6;
            this._Label1_3.BackColor = System.Drawing.Color.White;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_3, 3);
            point2 = this._Label1_3.Location = new System.Drawing.Point(7, 14);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_3.Size = new System.Drawing.Size(393, 15);
            this._Label1_3.TabIndex = 5;
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_4.BackColor = System.Drawing.Color.White;
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_4, 4);
            point2 = this._Label1_4.Location = new System.Drawing.Point(7, 119);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_4.Size = new System.Drawing.Size(393, 15);
            this._Label1_4.TabIndex = 4;
            this._Label1_5.BackColor = System.Drawing.Color.White;
            this._Label1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_5, 5);
            point2 = this._Label1_5.Location = new System.Drawing.Point(7, 56);
            this._Label1_5.Name = "_Label1_5";
            this._Label1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_5.Size = new System.Drawing.Size(393, 15);
            this._Label1_5.TabIndex = 3;
            this.Label2.AutoSize = true;
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Label2.Location = new System.Drawing.Point(7, 662);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Label2.Size = new System.Drawing.Size(46, 16);
            this.Label2.TabIndex = 18;
            this.Label2.Text = "Label2";
            this.RTB1.Font = new System.Drawing.Font("Arial", 8f);
            point2 = this.RTB1.Location = new System.Drawing.Point(25, 12);
            this.RTB1.Name = "RTB1";
            size2 = this.RTB1.Size = new System.Drawing.Size(936, 682);
            this.RTB1.TabIndex = 20;
            this.RTB1.Text = "";
            System.Drawing.SizeF sizeF2 = this.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            size2 = this.ClientSize = new System.Drawing.Size(1024, 750);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.RTB1);
            this.Controls.Add(this.l1);
            this.Controls.Add(this._Command1_3);
            this.Controls.Add(this._Command1_2);
            this.Controls.Add(this._Command1_1);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this.Label2);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Ahnent";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Frame1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
        {
            //Discarded unreachable code: IL_004e
            switch (Command_Renamed.GetIndex((Button)eventSender))
            {
                case 0:
                    Close();
                    MyProject.Forms.ATMenue.Close();
                    MyProject.Forms.Druck.Show();
                    return;
                case 1:
                    if (Strings.Mid(Label1[3].Text, 16, 10).AsDouble() < 1.0)
                    {
                        MyProject.Forms.ATMenue.Close();
                        Close();
                        MyProject.Forms.Druck.Show();
                        return;
                    }
                    Frame1.Visible = false;
                    break;
            }
            if (Modul1.Aschalt != 1f)
            {
                return;
            }
            Grundform3();
            RTB1.Text = "";
            string[] array = new string[81];
            short num = 10;
            checked
            {
                do
                {
                    short num2 = num;
                    if (num2 != 1 && num2 != 11 && num2 != 21 && num2 != 31 && num2 != 41 && num2 != 51 && num2 != 61 && num2 != 71 && num2 != 81 && num2 != 91 && num2 != 101 && num2 != 111 && num2 != 121 && num2 != 131 && num2 != 141 && num2 != 151 && num2 != 161 && num2 != 171 && num2 != 181 && num2 != 191 && num2 != 201 && num2 != 211 && num2 != 221 && num2 != 231 && num2 != 241 && num2 != 251 && num2 != 271 && num2 != 281 && num2 != 291 && num2 != 301 && num2 != 311 && num2 != 321 && Text1[num].Trim().Length < 3)
                    {
                        Text1[num] = "";
                    }
                    num = (short)unchecked(num + 1);
                }
                while (num <= 350);
                num = 1;
                do
                {
                    short num3 = num;
                    if (num3 is >= 10 and <= 19)
                    {
                        Modul1.UbgT = Text1[num];
                        while (true)
                        {
                            Label2.Text = Modul1.UbgT;
                            if (Label2.Width <= 350)
                            {
                                break;
                            }
                            Modul1.UbgT = Modul1.UbgT.Left(Modul1.UbgT.Length - 1);
                        }
                        if (Label2.Width < 14.5 * Modul1.Fs)
                        {
                            Modul1.UbgT += "\t";
                        }
                        if (Label2.Width < 8.2 * Modul1.Fs)
                        {
                            Modul1.UbgT += "\t";
                        }
                        Text1[num] = Modul1.UbgT;
                    }
                    else if (num3 is >= 20 and <= 39)
                    {
                        Modul1.UbgT = Text1[num];
                        while (true)
                        {
                            Label2.Text = Modul1.UbgT;
                            if (Label2.Width <= 260)
                            {
                                break;
                            }
                            Modul1.UbgT = Modul1.UbgT.Left(Modul1.UbgT.Length - 1);
                        }
                        if (Label2.Width < 8.5 * Modul1.Fs)
                        {
                            Modul1.UbgT += "\t";
                        }
                        Text1[num] = Modul1.UbgT;
                    }
                    else if (num3 is >= 40 and <= 79)
                    {
                        Modul1.UbgT = Text1[num];
                        while (true)
                        {
                            Label2.Text = Modul1.UbgT;
                            if (Label2.Width <= 170)
                            {
                                break;
                            }
                            Modul1.UbgT = Modul1.UbgT.Left(Modul1.UbgT.Length - 1);
                        }
                        Text1[num] = Modul1.UbgT;
                    }
                    else if (num3 is >= 80 and <= 159)
                    {
                        Modul1.UbgT = Text1[num];
                        while (true)
                        {
                            Label2.Text = Modul1.UbgT;
                            if (Label2.Width <= 170)
                            {
                                break;
                            }
                            Modul1.UbgT = Modul1.UbgT.Left(Modul1.UbgT.Length - 1);
                        }
                        Text1[num] = Modul1.UbgT;
                    }
                    else if (num3 > 160)
                    {
                        Modul1.UbgT = Text1[num];
                        while (true)
                        {
                            Label2.Text = Modul1.UbgT;
                            if (Label2.Width <= 170)
                            {
                                break;
                            }
                            Modul1.UbgT = Modul1.UbgT.Left(Modul1.UbgT.Length - 1);
                        }
                        Text1[num] = Modul1.UbgT;
                    }
                    num = (short)unchecked(num + 1);
                }
                while (num <= 350);
                RTB1.SelectionTabs = new int[6] { 1, 20, 80, 150, 300, 450 };
                array[0] = "\tGeneration " + Text1[1].AsDouble().AsString() + "\tGeneration " + Conversion.Str(Conversion.Val(Text1[1].AsDouble() + 1.0)) + "\tGeneration " + Conversion.Str(Conversion.Val(Text1[1].AsDouble() + 2.0)) + "\tGeneration " + Conversion.Str(Conversion.Val(Text1[1].AsDouble() + 3.0)) + "\tGeneration " + Conversion.Str(Conversion.Val(Text1[1].AsDouble() + 4.0)) + "\n";
                array[1] = "\t\t\t\t\t\t" + Text1[161] + "\n";
                array[2] = "\t\t\t\t\t" + Text1[81] + "\t" + Text1[162] + "\n";
                array[3] = "\t\t\t\t" + Text1[41] + "\t" + Text1[82] + "\t" + Text1[165] + " " + Text1[167] + "\n";
                array[4] = "\t\t\t\t" + Text1[42] + "\t" + Text1[85] + "\t" + Text1[169] + "\n";
                array[5] = "";
                array[6] = "\t\t\t\t" + Text1[45] + "\t" + Text1[86] + "\t" + Text1[171] + "\n";
                array[7] = "\t\t\t\t" + Text1[46] + "\t" + Text1[87] + "\t" + Text1[172] + "\n";
                array[8] = "\t\t\t\t" + Text1[47] + "\t" + Text1[88] + "\t" + Text1[175] + " " + Text1[177] + "\n";
                array[9] = "\t\t\t\t" + Text1[48] + "\t" + Text1[89] + "\n";
                array[10] = "\t\t\t\t" + Text1[49] + "\t\t" + Text1[181] + "\n";
                array[11] = "\t\t\t\t\t" + Text1[91] + "\t" + Text1[182] + "\n";
                array[12] = "\t\t\t" + Text1[21] + "\t" + Text1[92] + "\t" + Text1[185] + " " + Text1[187] + "\n";
                array[13] = "\t\t\t" + Text1[22] + "\t" + Text1[95] + "\t" + Text1[189] + "\n";
                array[14] = "";
                array[15] = "\t\t\t" + Text1[23] + "\t" + Text1[96] + "\t" + Text1[191] + "\n";
                array[16] = "\t\t\t" + Text1[24] + "\t" + Text1[97] + "\t" + Text1[192] + "\n";
                array[17] = "\t\t\t" + Text1[29] + "\t" + Text1[98] + "\t" + Text1[195] + " " + Text1[197] + "\n";
                array[18] = "\t\t\t\t\t\n";
                array[19] = "\t\t\t\t\t\t" + Text1[201] + "\n";
                array[20] = "\t\t\t\t\t" + Text1[101] + "\t" + Text1[202] + "\n";
                array[21] = "\t\t\t\t" + Text1[51] + "\t" + Text1[102] + "\t" + Text1[205] + " " + Text1[207] + "\n";
                array[22] = "\t\t\t\t" + Text1[52] + "\t" + Text1[105] + "\t" + Text1[209] + "\n";
                array[23] = "";
                array[24] = "\t\t\t\t" + Text1[55] + "\t" + Text1[106] + "\t" + Text1[211] + "\n";
                array[25] = "\t\t\t\t" + Text1[56] + "\t" + Text1[107] + "\t" + Text1[212] + "\n";
                array[26] = "\t\t\t\t" + Text1[57] + "\t" + Text1[108] + "\t" + Text1[215] + " " + Text1[217] + "\n";
                array[27] = "\t\t\t\t" + Text1[58] + "\t" + Text1[109] + "\n";
                array[28] = "\t\t\t\t\t\t" + Text1[221] + "\n";
                array[29] = "\t\t\t\t\t" + Text1[111] + "\t" + Text1[222] + "\n";
                array[30] = "\t\t\t\t\t" + Text1[112] + "\t" + Text1[225] + " " + Text1[227] + "\n";
                array[31] = "\t\t" + Text1[11] + "\t" + Text1[115] + "\t" + Text1[229] + "\n";
                array[32] = "";
                array[33] = "\t\t" + Text1[12] + "\t" + Text1[116] + "\t" + Text1[231] + "\n";
                array[34] = "\t\t" + Text1[15] + "\t" + Text1[117] + "\t" + Text1[232] + "\n";
                array[35] = "\t\t" + Text1[16] + "\t" + Text1[118] + "\t" + Text1[235] + " " + Text1[237] + "\n";
                array[36] = "\t\t" + Text1[17] + "\t\t\n";
                array[37] = "";
                array[38] = "\t\t" + Text1[18] + "\t\t" + Text1[241] + "\n";
                array[39] = "\t\t\t\t\t" + Text1[121] + "\t" + Text1[242] + "\n";
                array[40] = "\t\t\t\t" + Text1[61] + "\t" + Text1[122] + "\t" + Text1[245] + " " + Text1[247] + "\n";
                array[41] = "\t\t\t\t" + Text1[62] + "\t" + Text1[125] + "\t" + Text1[249] + "\n";
                array[42] = "";
                array[43] = "\t\t\t\t" + Text1[65] + "\t" + Text1[126] + "\t" + Text1[251] + "\n";
                array[44] = "\t\t\t\t" + Text1[66] + "\t" + Text1[127] + "\t" + Text1[252] + "\n";
                array[45] = "\t\t\t\t" + Text1[67] + "\t" + Text1[128] + "\t" + Text1[255] + " " + Text1[257] + "\n";
                array[46] = "\t\t\t\t" + Text1[68] + "\t" + Text1[129] + "\t\n";
                array[47] = "\t\t\t\t" + Text1[69] + "\t\t" + Text1[261] + "\n";
                array[48] = "\t\t\t\t\t" + Text1[131] + "\t" + Text1[262] + "\n";
                array[49] = "\t\t\t" + Text1[31] + "\t" + Text1[132] + "\t" + Text1[265] + " " + Text1[267] + "\n";
                array[50] = "\t\t\t" + Text1[32] + "\t" + Text1[135] + "\t" + Text1[269] + "\n";
                array[51] = "";
                array[52] = "\t\t\t" + Text1[33] + "\t" + Text1[136] + "\t" + Text1[271] + "\n";
                array[53] = "\t\t\t" + Text1[34] + "\t" + Text1[137] + "\t" + Text1[272] + "\n";
                array[54] = "\t\t\t\t\t" + Text1[138] + "\t" + Text1[275] + " " + Text1[277] + "\n";
                array[55] = "\t\t\t\t\t\t\n";
                array[56] = "\t\t\t\t\t\t" + Text1[281] + "\n";
                array[57] = "\t\t\t\t\t" + Text1[141] + "\t" + Text1[282] + "\n";
                array[58] = "\t\t\t\t" + Text1[71] + "\t" + Text1[142] + "\t" + Text1[285] + " " + Text1[287] + "\n";
                array[59] = "\t\t\t\t" + Text1[72] + "\t" + Text1[145] + "\t" + Text1[289] + "\n";
                array[60] = "\t\t\t\t" + Text1[75] + "\t" + Text1[146] + "\t" + Text1[291] + "\n";
                array[61] = "\t\t\t\t" + Text1[76] + "\t" + Text1[147] + "\t" + Text1[292] + "\n";
                array[62] = "\t\t\t\t" + Text1[77] + "\t" + Text1[148] + "\t" + Text1[295] + " " + Text1[297] + "\n";
                array[63] = "\t\t\t\t" + Text1[78] + "\t" + Text1[149] + "\t\n";
                array[64] = "\t\t\t\t\t\t" + Text1[301] + "\n";
                array[65] = "\t\t\t\t\t" + Text1[151] + "\t" + Text1[302] + "\n";
                array[66] = "\t\t\t\t\t" + Text1[152] + "\t" + Text1[305] + " " + Text1[307] + "\n";
                array[67] = "\t\t\t\t\t" + Text1[155] + "\t" + Text1[309] + "\n";
                array[68] = "";
                array[69] = "\t\t\t\t\t" + Text1[156] + "\t" + Text1[311] + "\n";
                array[70] = "\t\t\t\t\t" + Text1[157] + "\t" + Text1[312] + "\n";
                array[71] = "\t\t\t\t\t" + Text1[158] + "\t" + Text1[315] + " " + Text1[317] + "\n";
                num = 0;
                do
                {
                    RTB1.SelectedText = array[num];
                    num = (short)unchecked(num + 1);
                }
                while (num <= 71);
            }
        }

        private void Command1_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            int number = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Command1.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 904:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0296;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_0205;
                            }
                        IL_0296:
                            num4 = num2 + 1;
                            goto IL_029a;
                        IL_0205:
                            num = 40;
                            number = Information.Err().Number;
                            goto IL_0214;
                        IL_0214:
                            num = 43;
                            if (number == 55)
                            {
                                goto IL_021d;
                            }
                            goto IL_0248;
                        IL_0248:
                            num = 49;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_026e;
                        IL_026e:
                            num = 52;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0292;
                        IL_005a:
                            num = 11;
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_0292:
                            num4 = num2;
                            goto IL_029a;
                        IL_029a:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0015;
                                case 8:
                                case 9:
                                    goto IL_003d;
                                case 10:
                                    goto IL_0047;
                                case 11:
                                    goto IL_005a;
                                case 13:
                                case 14:
                                    goto IL_0072;
                                case 15:
                                    goto IL_008f;
                                case 16:
                                    goto IL_00b6;
                                case 17:
                                    goto IL_00cf;
                                case 18:
                                    goto IL_00e8;
                                case 20:
                                case 21:
                                    goto IL_0113;
                                case 23:
                                case 25:
                                    goto IL_013c;
                                case 27:
                                case 28:
                                    goto IL_0162;
                                case 32:
                                case 33:
                                    goto IL_018d;
                                case 34:
                                    goto IL_0195;
                                case 35:
                                    goto IL_01b4;
                                case 36:
                                    goto IL_01d3;
                                case 40:
                                    goto IL_0205;
                                case 42:
                                case 43:
                                    goto IL_0214;
                                case 44:
                                    goto IL_021d;
                                case 45:
                                    goto IL_022c;
                                case 48:
                                case 49:
                                    goto IL_0248;
                                case 51:
                                case 52:
                                    goto IL_026e;
                                default:
                                    goto end_IL_0000;
                                case 3:
                                case 4:
                                case 6:
                                case 7:
                                case 12:
                                case 19:
                                case 22:
                                case 26:
                                case 29:
                                case 30:
                                case 31:
                                case 37:
                                case 38:
                                case 39:
                                case 41:
                                case 46:
                                case 47:
                                case 50:
                                case 53:
                                case 54:
                                case 55:
                                case 56:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_021d:
                            num = 44;
                            FileSystem.FileClose();
                            goto IL_022c;
                        IL_022c:
                            num = 45;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0292;
                        IL_0047:
                            num = 10;
                            MyProject.Forms.ATMenue.Close();
                            goto IL_005a;
                        IL_0015:
                            num = 2;
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 2:
                                    goto IL_0072;
                                case 3:
                                    goto IL_018d;
                                default:
                                    goto end_IL_0000_2;
                            }
                            goto IL_003d;
                        IL_018d:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0195;
                        IL_0195:
                            num = 34;
                            RTB1.SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            goto IL_01b4;
                        IL_01b4:
                            num = 35;
                            RTB1.LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            goto IL_01d3;
                        IL_01d3:
                            num = 36;
                            Interaction.Shell(Modul1.Aus[7] + " " + Modul1.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
                            goto end_IL_0000_2;
                        IL_0072:
                            num = 14;
                            MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                            goto IL_008f;
                        IL_008f:
                            num = 15;
                            MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = Modul1.GenFreeDir + "list\\";
                            goto IL_00b6;
                        IL_00b6:
                            num = 16;
                            MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                            goto IL_00cf;
                        IL_00cf:
                            num = 17;
                            MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                            goto IL_00e8;
                        IL_00e8:
                            num = 18;
                            if (MyProject.Forms.Hinter.CommonDialog1Save.FileName == "")
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_0113;
                        IL_0113:
                            num = 21;
                            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0162;
                                default:
                                    goto end_IL_0000_2;
                            }
                            goto IL_013c;
                        IL_0162:
                            num = 28;
                            RTB1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_013c:
                            num = 25;
                            RTB1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                            goto end_IL_0000_2;
                        IL_003d:
                            num = 9;
                            Close();
                            goto IL_0047;
                        end_IL_0000:
                            break;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 904;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2:
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        private void Ahnent_Load(object eventSender, EventArgs eventArgs)
        {
            Modul1.Dateienopen();
            BackColor = Modul1.HintFarb;
            Modul1.Feg = (short)Modul1.Persistence.ReadIntInit("state");
            Modul1.Fs = Modul1.Feg switch
            {
                0 => 7.8f,
                1 => 8.7f,
                2 => 9.5f,
                3 => 10.3f,
                4 => 11f,
                5 => 11.7f,
                6 => 12.4f,
                7 => 13.2f,
                8 => 14.9f,
                9 => 16.5f,
                _ => Modul1.Fs,
            };
            Font = new Font("Arial", Modul1.Fs, FontStyle.Regular);
            FileSystem.FileClose(6);
            Show();
            ProjectData.ClearProjectError();
            Frame1.Visible = true;
            Label2.Font = new Font("Arial", Modul1.Fs, FontStyle.Regular);
            Label1[3].Text = "Keine Berechnung vorhanden! Zuerst Ahnen berechnen!";
            DataModul.NB_AhnTable.Index = "Ahnen";
            DataModul.NB_AhnTable.MoveFirst();
            if (!DataModul.NB_AhnTable.EOF)
            {
                if (!DataModul.NB_AhnTable.NoMatch)
                {
                    string text = DataModul.NB_AhnTable.Fields["Ahn"].AsString();
                    DataModul.NB_AhnTable.MoveLast();
                    Label1[3].Text = "Ahnenberechnung " + DataModul.NB_AhnTable.Fields["Gen"].AsString() + " Generationen vorhanden für Ahnenziffer " + text;
                    DataModul.NB_AhnTable.MoveFirst();
                    Modul1.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    Label1[0].Text = Modul1.Kont[3] + " " + Modul1.Kont[0].ToUpper();
                    Datles();

                }
            }

            Label1[5].Text = Modul1.IText[3] + " " + Modul1.Kont[11];
            Label1[2].Text = Modul1.IText[5] + " " + Modul1.Kont[13];
            Label1[1].Text = Modul1.IText[4] + " " + Modul1.Kont[12];
            Label1[4].Text = Modul1.IText[6] + " " + Modul1.Kont[14];
        }
        public void Grundform3()
        {
            checked
            {
                M_Start = (int)Math.Round(Interaction.InputBox("Start mit Ahnennummer", " ", "1").AsDouble());
                if (M_Start == 0)
                {
                    Close();
                    MyProject.Forms.ATMenue.Close();
                    MyProject.Forms.Druck.Show();
                    return;
                }
                I = 1;
                do
                {
                    Nr = "1";
                    switch (I)
                    {
                        case 1:
                            Nr = M_Start.AsString().TrimStart();
                            break;
                        case 2:
                            Nr = (M_Start * 2).AsString().TrimStart();
                            break;
                        case 3:
                            Nr = (M_Start * 2 + 1).AsString().TrimStart();
                            break;
                        case 4:
                            Nr = (M_Start * 2 * 2).AsString().TrimStart();
                            break;
                        case 5:
                        case 6:
                        case 7:
                            Nr = (M_Start * 2 * 2 + I - 4).AsString().TrimStart();
                            break;
                        case 8:
                            Nr = (M_Start * 2 * 2 * 2).AsString().TrimStart();
                            break;
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 15:
                            Nr = Strings.LTrim((M_Start * 2 * 2 * 2 + (I - 8)).AsString());
                            break;
                        case 16:
                            Nr = (M_Start * 2 * 2 * 2 * 2).AsString().TrimStart();
                            break;
                        case 17:
                        case 18:
                        case 19:
                        case 20:
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                        case 25:
                        case 26:
                        case 27:
                        case 28:
                        case 29:
                        case 30:
                        case 31:
                            Nr = (M_Start * 2 * 2 * 2 * 2 + I - 16).AsString().TrimStart();
                            break;
                    }
                    Perles1();
                    if (I == 1)
                    {
                        Text1[1] = DataModul.NB_AhnTable.Fields["Gen"].AsString();
                    }
                    S = I * 10;
                    Text1[S + 1] = Nr + " " + Pername;
                    Text1[S + 2] = VNam;
                    Text1[S + 3] = " * " + Gebdat1;
                    Text1[S + 4] = "+" + Sterdat1;
                    Text1[S + 5] = "* " + Gebdat;
                    Text1[S + 6] = Gebdat2.Trim();
                    Text1[S + 7] = "+ " + Sterdat;
                    Text1[S + 8] = sterdat2.Trim();
                    if (Nr.AsDouble() > 9.0)
                    {
                        Text1[S + 9] = "oo " + Famdat.Trim();
                    }
                    else
                    {
                        Text1[S + 9] = "oo " + Famdat.Trim();
                    }
                    I++;
                }
                while (I <= 32);
            }
        }

        public void Perles1()
        {
            Pername = "";
            Sterdat1 = "";
            Sterdat = "          ";
            Gebdat1 = "";
            Gebdat = "          ";
            VNam = "";
            Famdat = "";
            Gebdat2 = "";
            sterdat2 = "";
            DataModul.NB_AhnTable.Index = "Ahnen";
            DataModul.NB_AhnTable.Seek("=", new string(' ', 40) + Nr.Right(40));
            DataModul.NB_AhnTable.Seek("=", new string(' ', 40) + Nr.Right(40));
            checked
            {
                if (!DataModul.NB_AhnTable.NoMatch)
                {
                    Modul1.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                    VNam = Modul1.Kont[3];
                    if (Modul1.Kont[1] != "")
                    {
                        Modul1.Kont[0] = Modul1.Kont[1] + " " + Modul1.Kont[0];
                    }
                    if (Modul1.Kont[2] != "")
                    {
                        Modul1.Kont[0] = Modul1.Kont[0] + " " + Modul1.Kont[2];
                    }
                    Pername = Modul1.Kont[0];
                    Modul1.Schalt = 200;
                    Datles();
                    Gebdat = Modul1.Kont[21];
                    if (Gebdat == "")
                    {
                        Gebdat = Modul1.Kont[22];
                    }
                    Sterdat = Modul1.Kont[23];
                    if (Sterdat == "")
                    {
                        Sterdat = Modul1.Kont[24];
                    }
                    Gebdat1 = Modul1.Kont[11];
                    if (Gebdat1 == "")
                    {
                        Gebdat1 = Modul1.Kont[12];
                    }
                    Gebdat2 = Strings.Mid(Gebdat1, Gebdat.Length + 1, Gebdat1.Length);
                    Sterdat1 = Modul1.Kont[13];
                    if (Sterdat1 == "")
                    {
                        Sterdat1 = Modul1.Kont[14];
                    }
                    sterdat2 = Strings.Mid(Sterdat1, Sterdat.Length + 1, Sterdat1.Length);
                    Modul1.FamInArb = DataModul.NB_AhnTable.Fields["Ehe"].AsInt();
                    Modul1.Schalt = 200;
                    Famdatles();
                    Famdat = Modul1.Kont[2];
                    if (Famdat == "")
                    {
                        Famdat = Modul1.Kont[3];
                    }
                }
            }
        }

        public void Datles()
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            int number = default;
            int lErl = default;
            short num5 = default;
            string ds = default;
            string ds2 = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    checked
                    {
                        int num4;
                        int ortNr;
                        byte Schalt;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                Modul1.Datum1 = "";
                                goto IL_000d;
                            case 1315:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_042d;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    goto IL_03ae;
                                }
                            IL_03ae:
                                num = 43;
                                number = Information.Err().Number;
                                goto IL_03be;
                            IL_03be:
                                num = 46;
                                if (number == 94)
                                {
                                    goto IL_03c8;
                                }
                                goto IL_03e4;
                            IL_03e4:
                                num = 51;
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                goto IL_040a;
                            IL_0378:
                                num = 39;
                                Modul1.Kont[Modul1.Ubg - 80] = Modul1.Kont1[1];
                                goto IL_0391;
                            IL_040a:
                                num = 54;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0431;
                            IL_0391:
                                num = 40;
                                lErl = 2;
                                goto IL_0398;
                            IL_0398:
                                num = 41;
                                num5 = (short)unchecked(num5 + 1);
                                if (num5 > 104)
                                {
                                    goto end_IL_0000_2;
                                }
                                goto IL_0049;
                            IL_0431:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 2:
                                        goto IL_000d;
                                    case 3:
                                        goto IL_001a;
                                    case 4:
                                        goto IL_0022;
                                    case 5:
                                        goto IL_0027;
                                    case 6:
                                        goto IL_0036;
                                    case 7:
                                        goto IL_0043;
                                    case 8:
                                        goto IL_0049;
                                    case 9:
                                        goto IL_0052;
                                    case 10:
                                        goto IL_0066;
                                    case 11:
                                        goto IL_0080;
                                    case 12:
                                        goto IL_008a;
                                    case 13:
                                        goto IL_009d;
                                    case 14:
                                        goto IL_0105;
                                    case 16:
                                    case 17:
                                        goto IL_011a;
                                    case 18:
                                        goto IL_014f;
                                    case 19:
                                        goto IL_018b;
                                    case 20:
                                        goto IL_0197;
                                    case 22:
                                    case 23:
                                        goto IL_01e1;
                                    case 24:
                                        goto IL_0204;
                                    case 25:
                                        goto IL_0213;
                                    case 26:
                                    case 27:
                                        goto IL_0223;
                                    case 28:
                                        goto IL_024e;
                                    case 29:
                                        goto IL_028a;
                                    case 30:
                                        goto IL_02ad;
                                    case 31:
                                        goto IL_02bc;
                                    case 32:
                                    case 33:
                                        goto IL_02cc;
                                    case 34:
                                        goto IL_02da;
                                    case 35:
                                        goto IL_030c;
                                    case 36:
                                    case 37:
                                        goto IL_0342;
                                    case 38:
                                        goto IL_036a;
                                    case 39:
                                        goto IL_0378;
                                    case 15:
                                    case 21:
                                    case 40:
                                        goto IL_0391;
                                    case 41:
                                        goto IL_0398;
                                    case 43:
                                        goto IL_03ae;
                                    case 45:
                                    case 46:
                                        goto IL_03be;
                                    case 47:
                                        goto IL_03c8;
                                    case 50:
                                    case 51:
                                        goto IL_03e4;
                                    case 52:
                                    case 54:
                                        goto IL_040a;
                                    default:
                                        goto end_IL_0000;
                                    case 42:
                                    case 44:
                                    case 48:
                                    case 49:
                                    case 55:
                                    case 56:
                                    case 57:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                            IL_03c8:
                                num = 47;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_042d;
                            IL_036a:
                                num = 38;
                                Modul1.UbgT = "";
                                goto IL_0378;
                            IL_042d:
                                num4 = unchecked(num2 + 1);
                                goto IL_0431;
                            IL_000d:
                                num = 2;
                                Modul1.Datum2 = "";
                                goto IL_001a;
                            IL_001a:
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                goto IL_0022;
                            IL_0022:
                                num = 4;
                                num5 = 1;
                                goto IL_0027;
                            IL_0027:
                                num = 5;
                                Modul1.Kont[num5] = "";
                                goto IL_0036;
                            IL_0036:
                                num = 6;
                                num5 = (short)unchecked(num5 + 1);
                                if (num5 <= 25)
                                {
                                    goto IL_0027;
                                }
                                goto IL_0043;
                            IL_0043:
                                num = 7;
                                num5 = 101;
                                goto IL_0049;
                            IL_0049:
                                num = 8;
                                Modul1_J = 0;
                                goto IL_0052;
                            IL_0052:
                                num = 9;
                                Modul1.Kont1[Modul1_J] = "";
                                goto IL_0066;
                            IL_0066:
                                num = 10;
                                Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                                if (unchecked(Modul1_J) <= 15u)
                                {
                                    goto IL_0052;
                                }
                                goto IL_0080;
                            IL_0080:
                                num = 11;
                                Modul1.Ubg = num5;
                                goto IL_008a;
                            IL_008a:
                                num = 12;
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto IL_009d;
                            IL_009d:
                                num = 13;
                                DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
                                goto IL_0105;
                            IL_0105:
                                num = 14;
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    goto IL_011a;
                                }
                                goto IL_0391;
                            IL_011a:
                                num = 17;
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    goto IL_014f;
                                }
                                goto IL_0223;
                            IL_014f:
                                num = 18;
                                Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                goto IL_018b;
                            IL_018b:
                                num = 19;
                                if (Modul1.Datschalt == 1)
                                {
                                    goto IL_0197;
                                }
                                goto IL_01e1;
                            IL_0197:
                                num = 20;
                                Modul1.Kont[Modul1.Ubg - 100] = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                goto IL_0391;
                            IL_01e1:
                                num = 23;
                                ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                goto IL_0204;
                            IL_0204:
                                num = 24;
                                { var tempDatu = Modul1.sDatu; Modul1.sDatu = Modul1.Datwand1(tempDatu, ds); }
                                goto IL_0213;
                            IL_0213:
                                num = 25;
                                Modul1.Kont1[1] = Modul1.sDatu;
                                goto IL_0223;
                            IL_0223:
                                num = 27;
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                {
                                    goto IL_024e;
                                }
                                goto IL_02cc;
                            IL_024e:
                                num = 28;
                                Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString().Trim(), 8);
                                goto IL_028a;
                            IL_028a:
                                num = 29;
                                ds2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                goto IL_02ad;
                            IL_02ad:
                                num = 30;
                                { var tempDatu = Modul1.sDatu; Modul1.sDatu = Modul1.Datwand1(tempDatu, ds2); }
                                goto IL_02bc;
                            IL_02bc:
                                num = 31;
                                Modul1.Kont1[3] = Modul1.sDatu;
                                goto IL_02cc;
                            IL_02cc:
                                num = 33;
                                Modul1.UbgT = "";
                                goto IL_02da;
                            IL_02da:
                                num = 34;
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                                {
                                    goto IL_030c;
                                }
                                goto IL_0342;
                            IL_030c:
                                num = 35;
                                Modul1.UbgT = Modul1.ortles((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble()), 1);
                                goto IL_0342;
                            IL_0342:
                                num = 37;
                                Modul1.Kont[Modul1.Ubg - 90] = Modul1.Kont1[1] + " " + Modul1.UbgT;
                                goto IL_036a;
                            end_IL_0000:
                                break;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 1315;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2:
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        public void Famdatles()
        {
            string ind = "";
            short num = 0;
            checked
            {
                do
                {
                    Modul1.Kont[num] = "";
                    num = (short)unchecked(num + 1);
                }
                while (num <= 30);
                num = 500;
                do
                {
                    Modul1.Ubg = num;
                    Modul1.Art = (EEventArt)Modul1.Ubg;
                    DataModul.DB_EventTable.Index = "ArtNr";
                    DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.FamInArb.AsString(), "0");
                    if (!DataModul.DB_EventTable.NoMatch)
                    {
                        Modul1_J = 0;
                        do
                        {
                            Modul1.Kont1[Modul1_J] = "";
                            Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                        }
                        while (unchecked(Modul1_J) <= 15u);
                        if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                        {
                            Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                            if (Modul1.Schalt == 1)
                            {
                                Modul1.Kont[Modul1.Ubg - 500] = Modul1.sDatu;
                                goto IL_02a2;
                            }
                            Modul1.Datschalt = 14;
                            { var tempDatu = Modul1.sDatu; Modul1.sDatu = Modul1.Datwand1(tempDatu, DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString()); }
                            Modul1.Kont1[1] = Modul1.sDatu;
                        }
                        Modul1.UbgT = "";
                        if (DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble() > 0.0)
                        {
                            if (Modul1.Schalt == 10)
                            {
                                Modul1.Ind1 = ind;
                            }
                            if (Modul1.Schalt == 200)
                            {
                                Modul1.UbgT = Modul1.ortles((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble()), 200);
                            }
                            else
                            {
                                Modul1.UbgT = Modul1.ortles((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].Value.AsDouble()), 1);
                            }
                        }
                        Modul1.Kont[Modul1.Ubg - 500] = Modul1.Kont1[1].Trim() + " " + Modul1.UbgT;
                        Modul1.UbgT = "";
                    }
                    goto IL_02a2;
                IL_02a2:
                    num = (short)unchecked(num + 1);
                }
                while (num <= 504);
            }
        }

        private void _Command_1_Click(object sender, EventArgs e)
        {
        }

        public void Show(float v)
        {
            _Modul1.Instance.Aschalt = v;
            base.Show();
        }
    }
}
