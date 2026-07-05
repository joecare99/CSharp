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
    internal class Ort : Form
    {
        private IContainer components;

        public ToolTip ToolTip1;

        [AccessedThroughProperty("_Command1_5")]
        private Button __Command1_5;

        [AccessedThroughProperty("_Check1_3")]
        private CheckBox __Check1_3;

        [AccessedThroughProperty("RTf1")]
        private RichTextBox _RTf1;

        [AccessedThroughProperty("_Command1_4")]
        private Button __Command1_4;

        [AccessedThroughProperty("_Command1_3")]
        private Button __Command1_3;

        [AccessedThroughProperty("_Command1_2")]
        private Button __Command1_2;

        [AccessedThroughProperty("_Command1_1")]
        private Button __Command1_1;

        [AccessedThroughProperty("_Command1_0")]
        private Button __Command1_0;

        [AccessedThroughProperty("_Check1_2")]
        private CheckBox __Check1_2;

        [AccessedThroughProperty("_Check1_1")]
        private CheckBox __Check1_1;

        [AccessedThroughProperty("_Check1_0")]
        private CheckBox __Check1_0;

        [AccessedThroughProperty("_Label1_3")]
        private Label __Label1_3;

        [AccessedThroughProperty("_Label1_1")]
        private Label __Label1_1;

        [AccessedThroughProperty("_Label1_2")]
        private Label __Label1_2;

        [AccessedThroughProperty("_Label1_4")]
        private Label __Label1_4;

        [AccessedThroughProperty("_Label1_0")]
        private Label __Label1_0;

        [AccessedThroughProperty("Check1")]
        private ControlArray<CheckBox> _Check1;

        [AccessedThroughProperty("Command1")]
        private ControlArray<Button> _Command1;

        [AccessedThroughProperty("Label1")]
        private ControlArray<Label> _Label1;

        [AccessedThroughProperty("Label2")]
        private Label _Label2;

        [AccessedThroughProperty("Button1")]
        private Button _Button1;

        public string[] OAus;

        public virtual Button _Command1_5
        {
            get
            {
                return __Command1_5;
            }
     
            set
            {
                __Command1_5 = value;
            }
        }

        public virtual CheckBox _Check1_3
        {
            get
            {
                return __Check1_3;
            }
     
            set
            {
                __Check1_3 = value;
            }
        }

        public virtual RichTextBox RTf1
        {
            get
            {
                return _RTf1;
            }
     
            set
            {
                _RTf1 = value;
            }
        }

        public virtual Button _Command1_4
        {
            get
            {
                return __Command1_4;
            }
     
            set
            {
                __Command1_4 = value;
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
                EventHandler value2 = _Command1_0_Click;
                if (__Command1_0 != null)
                {
                    __Command1_0.Click -= value2;
                }
                __Command1_0 = value;
                if (__Command1_0 != null)
                {
                    __Command1_0.Click += value2;
                }
            }
        }

        public virtual CheckBox _Check1_2
        {
            get
            {
                return __Check1_2;
            }
     
            set
            {
                __Check1_2 = value;
            }
        }

        public virtual CheckBox _Check1_1
        {
            get
            {
                return __Check1_1;
            }
     
            set
            {
                __Check1_1 = value;
            }
        }

        public virtual CheckBox _Check1_0
        {
            get
            {
                return __Check1_0;
            }
     
            set
            {
                __Check1_0 = value;
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

        public virtual ControlArray<CheckBox> Check1
        {
            get
            {
                return _Check1;
            }
     
            set
            {
                _Check1 = value;
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

        internal virtual Label Label2
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

        internal virtual Button Button1
        {
            get
            {
                return _Button1;
            }
     
            set
            {
                EventHandler value2 = Button1_Click;
                if (_Button1 != null)
                {
                    _Button1.Click -= value2;
                }
                _Button1 = value;
                if (_Button1 != null)
                {
                    _Button1.Click += value2;
                }
            }
        }

        [DebuggerNonUserCode]
        public Ort()
        {
            base.Load += Ort_Load;
            base.FormClosing += Ort_FormClosing;
            OAus = new string[7];
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
            this._Command1_5 = new System.Windows.Forms.Button();
            this._Check1_3 = new System.Windows.Forms.CheckBox();
            this.RTf1 = new System.Windows.Forms.RichTextBox();
            this._Command1_4 = new System.Windows.Forms.Button();
            this._Command1_3 = new System.Windows.Forms.Button();
            this._Command1_2 = new System.Windows.Forms.Button();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this._Check1_2 = new System.Windows.Forms.CheckBox();
            this._Check1_1 = new System.Windows.Forms.CheckBox();
            this._Check1_0 = new System.Windows.Forms.CheckBox();
            this._Label1_3 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_4 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.Check1 = new ControlArray<System.Windows.Forms.CheckBox>();
            this.Command1 = new ControlArray<System.Windows.Forms.Button>();
            this.Label1 = new ControlArray<System.Windows.Forms.Label>();
            this.Label2 = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)this.Check1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
            this.SuspendLayout();
            this._Command1_5.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Command1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_5, 5);
            System.Drawing.Point point2 = this._Command1_5.Location = new System.Drawing.Point(67, 476);
            System.Windows.Forms.Padding padding2 = this._Command1_5.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_5.Name = "_Command1_5";
            this._Command1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            System.Drawing.Size size2 = this._Command1_5.Size = new System.Drawing.Size(108, 25);
            this._Command1_5.TabIndex = 15;
            this._Command1_5.Text = "Start für PLZ";
            this._Command1_5.UseVisualStyleBackColor = false;
            this._Command1_5.Visible = false;
            this._Check1_3.BackColor = System.Drawing.Color.White;
            this._Check1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check1.SetIndex(this._Check1_3, 3);
            point2 = this._Check1_3.Location = new System.Drawing.Point(355, 364);
            padding2 = this._Check1_3.Margin = new System.Windows.Forms.Padding(4);
            this._Check1_3.Name = "_Check1_3";
            this._Check1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Check1_3.Size = new System.Drawing.Size(287, 22);
            this._Check1_3.TabIndex = 14;
            this._Check1_3.Text = "separate Ortsteilliste";
            this._Check1_3.UseVisualStyleBackColor = false;
            point2 = this.RTf1.Location = new System.Drawing.Point(13, 534);
            padding2 = this.RTf1.Margin = new System.Windows.Forms.Padding(4);
            this.RTf1.Name = "RTf1";
            this.RTf1.RightMargin = 634;
            size2 = this.RTf1.Size = new System.Drawing.Size(992, 126);
            this.RTf1.TabIndex = 7;
            this.RTf1.Text = "";
            this.RTf1.Visible = false;
            this._Command1_4.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Command1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_4, 4);
            point2 = this._Command1_4.Location = new System.Drawing.Point(43, 680);
            padding2 = this._Command1_4.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_4.Name = "_Command1_4";
            this._Command1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_4.Size = new System.Drawing.Size(132, 25);
            this._Command1_4.TabIndex = 6;
            this._Command1_4.Text = "Drucken";
            this._Command1_4.UseVisualStyleBackColor = false;
            this._Command1_3.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Command1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_3, 3);
            point2 = this._Command1_3.Location = new System.Drawing.Point(187, 680);
            padding2 = this._Command1_3.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_3.Name = "_Command1_3";
            this._Command1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_3.Size = new System.Drawing.Size(161, 25);
            this._Command1_3.TabIndex = 5;
            this._Command1_3.Text = "Speichern in Datei";
            this._Command1_3.UseVisualStyleBackColor = false;
            this._Command1_2.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Command1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_2, 2);
            point2 = this._Command1_2.Location = new System.Drawing.Point(893, 680);
            padding2 = this._Command1_2.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_2.Name = "_Command1_2";
            this._Command1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_2.Size = new System.Drawing.Size(113, 25);
            this._Command1_2.TabIndex = 4;
            this._Command1_2.Text = "Hauptmenue";
            this._Command1_2.UseVisualStyleBackColor = false;
            this._Command1_2.Visible = false;
            this._Command1_1.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_1, 1);
            point2 = this._Command1_1.Location = new System.Drawing.Point(589, 680);
            padding2 = this._Command1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_1.Size = new System.Drawing.Size(127, 25);
            this._Command1_1.TabIndex = 3;
            this._Command1_1.Text = "Druckmenue";
            this._Command1_1.UseVisualStyleBackColor = false;
            this._Command1_0.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_0, 0);
            point2 = this._Command1_0.Location = new System.Drawing.Point(355, 501);
            padding2 = this._Command1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_0.Size = new System.Drawing.Size(108, 25);
            this._Command1_0.TabIndex = 2;
            this._Command1_0.Text = "Start";
            this._Command1_0.UseVisualStyleBackColor = false;
            this._Check1_2.BackColor = System.Drawing.Color.White;
            this._Check1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check1.SetIndex(this._Check1_2, 2);
            point2 = this._Check1_2.Location = new System.Drawing.Point(355, 337);
            padding2 = this._Check1_2.Margin = new System.Windows.Forms.Padding(4);
            this._Check1_2.Name = "_Check1_2";
            this._Check1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Check1_2.Size = new System.Drawing.Size(287, 22);
            this._Check1_2.TabIndex = 10;
            this._Check1_2.Text = "Bemerkungen";
            this._Check1_2.UseVisualStyleBackColor = false;
            this._Check1_1.BackColor = System.Drawing.Color.White;
            this._Check1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check1.SetIndex(this._Check1_1, 1);
            point2 = this._Check1_1.Location = new System.Drawing.Point(355, 311);
            padding2 = this._Check1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Check1_1.Name = "_Check1_1";
            this._Check1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Check1_1.Size = new System.Drawing.Size(287, 22);
            this._Check1_1.TabIndex = 9;
            this._Check1_1.Text = "Angaben für die Forscherkontakte";
            this._Check1_1.UseVisualStyleBackColor = false;
            this._Check1_0.BackColor = System.Drawing.Color.White;
            this._Check1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check1.SetIndex(this._Check1_0, 0);
            point2 = this._Check1_0.Location = new System.Drawing.Point(355, 282);
            padding2 = this._Check1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Check1_0.Name = "_Check1_0";
            this._Check1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Check1_0.Size = new System.Drawing.Size(287, 22);
            this._Check1_0.TabIndex = 8;
            this._Check1_0.Text = "Angaben zur Lage des Ortes";
            this._Check1_0.UseVisualStyleBackColor = false;
            this._Label1_3.BackColor = System.Drawing.Color.Red;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.ForeColor = System.Drawing.Color.White;
            this.Label1.SetIndex(this._Label1_3, 3);
            point2 = this._Label1_3.Location = new System.Drawing.Point(0, 0);
            padding2 = this._Label1_3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_3.Size = new System.Drawing.Size(1072, 21);
            this._Label1_3.TabIndex = 13;
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_1.BackColor = System.Drawing.Color.Red;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.Color.White;
            this.Label1.SetIndex(this._Label1_1, 1);
            point2 = this._Label1_1.Location = new System.Drawing.Point(0, 24);
            padding2 = this._Label1_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_1.Size = new System.Drawing.Size(1072, 21);
            this._Label1_1.TabIndex = 12;
            this._Label1_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_2.BackColor = System.Drawing.Color.Red;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.ForeColor = System.Drawing.Color.White;
            this.Label1.SetIndex(this._Label1_2, 2);
            point2 = this._Label1_2.Location = new System.Drawing.Point(0, 47);
            padding2 = this._Label1_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_2.Size = new System.Drawing.Size(1072, 21);
            this._Label1_2.TabIndex = 11;
            this._Label1_2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_4.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_4, 4);
            point2 = this._Label1_4.Location = new System.Drawing.Point(355, 259);
            padding2 = this._Label1_4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_4.Size = new System.Drawing.Size(247, 22);
            this._Label1_4.TabIndex = 1;
            this._Label1_4.Text = "Ausgabe der";
            this._Label1_4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_0, 0);
            point2 = this._Label1_0.Location = new System.Drawing.Point(0, 89);
            padding2 = this._Label1_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_0.Size = new System.Drawing.Size(1065, 22);
            this._Label1_0.TabIndex = 0;
            this._Label1_0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Label2.AutoSize = true;
            this.Label2.BackColor = System.Drawing.Color.White;
            point2 = this.Label2.Location = new System.Drawing.Point(40, 111);
            this.Label2.Name = "Label2";
            size2 = this.Label2.Size = new System.Drawing.Size(51, 17);
            this.Label2.TabIndex = 16;
            this.Label2.Text = "Label2";
            this.Button1.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            point2 = this.Button1.Location = new System.Drawing.Point(43, 145);
            this.Button1.Name = "Button1";
            size2 = this.Button1.Size = new System.Drawing.Size(118, 31);
            this.Button1.TabIndex = 17;
            this.Button1.Text = "Schrift ändern";
            this.Button1.UseVisualStyleBackColor = false;
            System.Drawing.SizeF sizeF2 = this.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            size2 = this.ClientSize = new System.Drawing.Size(1018, 725);
            this.Controls.Add(this._Command1_5);
            this.Controls.Add(this._Check1_3);
            this.Controls.Add(this.RTf1);
            this.Controls.Add(this._Command1_4);
            this.Controls.Add(this._Command1_3);
            this.Controls.Add(this._Command1_2);
            this.Controls.Add(this._Command1_1);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this._Check1_2);
            this.Controls.Add(this._Check1_1);
            this.Controls.Add(this._Check1_0);
            this.Controls.Add(this._Label1_3);
            this.Controls.Add(this._Label1_1);
            this.Controls.Add(this._Label1_2);
            this.Controls.Add(this._Label1_4);
            this.Controls.Add(this._Label1_0);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Button1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            padding2 = this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Ort";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Ortsliste";
            ((System.ComponentModel.ISupportInitialize)this.Check1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Command1_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            string text = default;
            string text2 = default;
            string text3 = default;
            string text4 = default;
            long num5 = default;
            long num8 = default;
            string text6 = default;
            byte b = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    checked
                    {
                        int num4;
                        long num7;
                        int AAA;
                        int ubg;
                        byte Schalt;
                        string LD;
                        int index;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                index = Command1.GetIndex((Button)eventSender);
                                goto IL_0015;
                            case 8518:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 3:
                                            goto IL_1c32;
                                        case 1:
                                            goto IL_1c6c;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    int number = Information.Err().Number;
                                    if (number is 3022 or 3211 or 3010 or 3376 or 3021)
                                    {
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_1c6c;
                                    }
                                    if (number == 5)
                                    {
                                        ProjectData.ClearProjectError();
                                        if (num2 == 0)
                                        {
                                            throw ProjectData.CreateProjectError(-2146828268);
                                        }
                                        goto IL_1c6c;
                                    }
                                    Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkOnly, Information.Err().Number.AsString());
                                    goto end_IL_0000_2;
                                }
                            end_IL_0000:
                                break;
                            IL_0015:
                                num = 2;
                                text2 = "";
                                text3 = "";
                                text4 = "";
                                Command1[3].Visible = true;
                                Command1[4].Visible = true;
                                switch (index)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        goto IL_1451;
                                    case 2:
                                        goto IL_1479;
                                    case 3:
                                        goto IL_14ee;
                                    case 4:
                                        goto IL_1622;
                                    case 5:
                                        goto IL_16db;
                                    default:
                                        goto end_IL_0000_2;
                                }
                                Command1[4].Enabled = false;
                                Command1[3].Enabled = false;
                                num5 = 0L;
                                while (num5 <= 3)
                                {
                                    Check1[(short)num5].Visible = false;
                                    num5++;
                                }
                                RTf1.Top = 0;
                                RTf1.Visible = true;
                                OAus[1] = unchecked((int)Check1[0].CheckState).AsString();
                                OAus[2] = unchecked((int)Check1[1].CheckState).AsString();
                                OAus[3] = unchecked((int)Check1[2].CheckState).AsString();
                                OAus[4] = unchecked((int)Check1[3].CheckState).AsString();
                                _Modul1.Instance.Persistence.WriteStringsOutput("Ort.DAT", OAus, 7);
                                RTf1.SelectionAlignment = HorizontalAlignment.Center;
                                RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                RTf1.SelectedText = "Ortsliste für Mandant " + _Modul1.Instance.Verz + "\n";
                                RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                RTf1.SelectedText = $"Erstellt von {_Modul1.Instance.User.Name.Trim()} am {DateTime.Today.Date.AsString()}\n";
                                RTf1.SelectionAlignment = HorizontalAlignment.Left;
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                DataModul.TempDB.Execute($"DROP Table {dbTables.OT};");
                                DataModul.TempDB.Execute($"CREATE Table {dbTables.OT} (OT TEXT(240)Not NULL,OrT TEXT(240)Not NULL);");
                                DataModul.TempDB.Execute($"CREATE UNIQUE INDEX Name ON {dbTables.OT} ([OT],[Ort]);");
                                DataModul.DT_OTTable = DataModul.TempDB.OpenRecordset(dbTables.OT, RecordsetTypeEnum.dbOpenTable);
                                if (Check1[3].Checked)
                                {
                                    DataModul.DB_PlaceTable.MoveFirst();
                                    goto IL_04fd;
                                }
                                goto IL_079c;
                            IL_04ef:
                                num = 52;
                                DataModul.DB_PlaceTable.MoveNext();
                                goto IL_04fd;
                            IL_04fd: // <========== 3
                                num = 42;
                                if (!DataModul.DB_PlaceTable.EOF)
                                {
                                    if (DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt() > 0)
                                        {
                                            AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ortsteil].AsInt();
                                        LD = "";
                                        _Modul1.Instance.Kont[1] = DataModul.TextLese1(AAA);
                                        AAA = DataModul.DB_PlaceTable.Fields[PlaceFields.Ort].AsInt();
                                        LD = "";
                                        _Modul1.Instance.Kont[2] = DataModul.TextLese1(AAA);
                                        _Modul1.Instance.Kont[2] = DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString();
                                        DataModul.DT_OTTable.AddNew();
                                        DataModul.DT_OTTable.Fields["OT"].Value = _Modul1.Instance.Kont[1];
                                        DataModul.DT_OTTable.Fields["OrT"].Value = _Modul1.Instance.Kont[2];
                                        DataModul.DT_OTTable.Update();
                                    }
                                    goto IL_04ef;
                                }
                                if (DataModul.DT_OTTable.RecordCount > 0)
                                {
                                    RTf1.SelectionFont = new Font(OAus[5], (float)(OAus[6].AsDouble() + 3.0), FontStyle.Bold);
                                    RTf1.SelectionAlignment = HorizontalAlignment.Center;
                                    RTf1.SelectedText = "\nOrtsteil-Liste\n\n";
                                    RTf1.SelectionAlignment = HorizontalAlignment.Left;
                                    RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                    DataModul.DT_OTTable.Index = "Name";
                                    DataModul.DT_OTTable.MoveFirst();
                                    while (!DataModul.DT_OTTable.EOF)
                                    {
                                        _Modul1.Instance.Kont[1] = DataModul.DT_OTTable.Fields["OT"].AsString();
                                        _Modul1.Instance.Kont[2] = DataModul.DT_OTTable.Fields["OrT"].AsString();
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Bold);
                                        RTf1.SelectedText = _Modul1.Instance.Kont[1];
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                        RTf1.SelectedText = " siehe " + _Modul1.Instance.Kont[2] + "-" + _Modul1.Instance.Kont[1] + "\n\n";
                                        DataModul.DT_OTTable.MoveNext();
                                    }
                                    RTf1.SelectionAlignment = HorizontalAlignment.Center;
                                    RTf1.SelectionFont = new Font(OAus[5], (float)(OAus[6].AsDouble() + 2.0), FontStyle.Bold);
                                    RTf1.SelectedText = "\nOrtsliste\n\n";
                                    RTf1.SelectionAlignment = HorizontalAlignment.Left;
                                    RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                }
                                goto IL_079c;
                            IL_079c: // <========== 3
                                num = 79;
                                DataModul.DOSB_OrtSTable.Index = "Ortsu";
                                DataModul.DOSB_OrtSTable.Seek(">=", " ");
                                goto IL_13a7;
                            IL_0a27:
                                num = 106;
                                if (Strings.Len(DataModul.DB_PlaceTable.Fields["L"].AsString().Trim()) > 1)
                                {
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields["L"].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        LD = DataModul.DB_PlaceTable.Fields["L"].AsString().Trim();
                                        _Modul1.Instance.geoles(ref LD);
                                        if (_Modul1.Instance.UbgT.Trim() != "")
                                        {
                                            if (Operators.CompareString(_Modul1.Instance.UbgT.Trim().Left(1), "-", TextCompare: false) == 0)
                                            {
                                                text4 = text4 + "westliche Länge: " + Strings.Mid(_Modul1.Instance.UbgT, 2, _Modul1.Instance.UbgT.Length) + ";, ";
                                                goto IL_0b90;
                                            }
                                            if (Operators.CompareString(_Modul1.Instance.UbgT.Trim().Left(1), "+", TextCompare: false) == 0)
                                            {
                                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 2, _Modul1.Instance.UbgT.Length);
                                            }
                                            goto IL_0b74;
                                        }

                                    }
                                }
                                goto IL_0b90;
                            IL_0b74:
                                num = 117;
                                text4 = text4 + "östliche Länge: " + _Modul1.Instance.UbgT + "; ";
                                goto IL_0b90;
                            IL_0b90: // <========== 5
                                num = 122;
                                if (Strings.Len(DataModul.DB_PlaceTable.Fields["B"].AsString().Trim()) > 1)
                                {
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields["B"].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        LD = DataModul.DB_PlaceTable.Fields["B"].AsString().Trim();
                                        _Modul1.Instance.geoles(ref LD);
                                        if (_Modul1.Instance.UbgT.Trim() != "")
                                        {
                                            if (Operators.CompareString(_Modul1.Instance.UbgT.Trim().Left(1), "-", TextCompare: false) == 0)
                                            {
                                                text4 = text4 + "südliche Breite: " + Strings.Mid(_Modul1.Instance.UbgT, 2, _Modul1.Instance.UbgT.Length) + "; ";
                                                goto IL_0d05;
                                            }
                                            if (Operators.CompareString(_Modul1.Instance.UbgT.Trim().Left(1), "+", TextCompare: false) == 0)
                                            {
                                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 2, _Modul1.Instance.UbgT.Length);
                                            }
                                            goto IL_0ce6;
                                        }

                                    }
                                }
                                goto IL_0d05;
                            IL_0ce6:
                                num = 133;
                                text4 = text4 + "nördliche Breite: " + _Modul1.Instance.UbgT + "; ";
                                goto IL_0d05;
                            IL_0d05: // <========== 5
                                num = 138;
                                if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        text4 = (text4 +  "Geneal. Ortsverz.: " +  DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].Value +  "; ").AsString();

                                    }
                                }
                                goto IL_0dab;
                            IL_0dab: // <========== 3
                                num = 143;
                                if (Strings.Len(DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim()) > 1)
                                {
                                    text = (text +  "PLZ " +  DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].Value +  "; ").AsString();
                                }
                                goto IL_0e1f;
                            IL_0e1f:
                                num = 146;
                                if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    text = (text +  "Territorium: " +  DataModul.DB_PlaceTable.Fields[PlaceFields.Terr].Value +  "; ").AsString();
                                }
                                goto IL_0e99;
                            IL_0e99:
                                num = 149;
                                if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    text = (text +  "Staatkennzeichen: " +  DataModul.DB_PlaceTable.Fields[PlaceFields.Staatk].Value +  "; ").AsString();
                                }
                                goto IL_0f13;
                            IL_0f13:
                                num = 152;
                                if (!Information.IsDBNull(DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        text = (text +  "GOV: " +  DataModul.DB_PlaceTable.Fields[PlaceFields.GOV].Value +  "; ").AsString();

                                    }
                                }
                                goto IL_0fb9;
                            IL_0fb9: // <========== 3
                                num = 157;
                                if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    text2 = ("Bemerkung: " +  DataModul.DB_PlaceTable.Fields[PlaceFields.Bem].AsString());
                                }
                                goto IL_1022;
                            IL_1022:
                                num = 160;
                                if (Check1[2].Checked)
                                {
                                    if (text2 != "")
                                    {
                                        RTf1.SelectedText = "\n";
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                        RTf1.SelectionIndent = 40;
                                        RTf1.SelectedText = text2;
                                        text2 = "";

                                    }
                                }
                                goto IL_10d1;
                            IL_10d1: // <========== 3
                                num = 169;
                                if (Check1[0].Checked)
                                {
                                    if (text4 != "")
                                    {
                                        RTf1.SelectedText = "\n";
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Bold);
                                        RTf1.SelectionIndent = 35;
                                        RTf1.SelectionHangingIndent = 10;
                                        RTf1.SelectedText = "Lage: ";
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                        text4 = text4.Trim();
                                        if (text4.Right(1) == ";")
                                        {
                                            text4 = text4.Left(text4.Length - 1) + ".";
                                        }
                                        RTf1.SelectedText = text4;
                                        text4 = "";

                                    }
                                }
                                goto IL_122c;
                            IL_122c: // <========== 3
                                num = 185;
                                if (Check1[1].Checked)
                                {
                                    text = text.Trim();
                                    if (text != "")
                                    {
                                        RTf1.SelectedText = "\n";
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Bold);
                                        RTf1.SelectionIndent = 35;
                                        RTf1.SelectedText = "Forscherkontakte: ";
                                        if (text.Right(1) == ";")
                                        {
                                            text = text.Left(text.Length - 1) + ".";
                                        }
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                        RTf1.SelectedText = text;
                                        text = "";

                                    }
                                }
                                goto IL_1373;
                            IL_1373: // <========== 4
                                num = 201;
                                RTf1.SelectedText = "\n";
                                Application.DoEvents();
                                DataModul.DOSB_OrtSTable.MoveNext();
                                goto IL_13a7;
                            IL_13a7: // <========== 3
                                num = 82;
                                if (!DataModul.DOSB_OrtSTable.EOF)
                                {
                                    b = (byte)(unchecked(b) + 1);
                                    text3 += ">";
                                    Label1[0].Text = text3;
                                    Label1[0].Refresh();
                                    if (b == 70)
                                    {
                                        text3 = "";
                                        b = 0;
                                    }
                                    RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                    RTf1.SelectedText = "\n";
                                    RTf1.SelectionHangingIndent = 10;
                                    RTf1.SelectionIndent = 20;
                                    if (DataModul.DOSB_OrtSTable.Fields["Nr"].AsInt() > 0)
                                    {
                                        _Modul1.Instance.Ubg = DataModul.DOSB_OrtSTable.Fields["Nr"].AsInt();
                                        text = "";
                                        ubg = _Modul1.Instance.Ubg;
                                        _Modul1.Instance.UbgT = _Modul1.Instance.ortles(ubg, 21);
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Bold);
                                        RTf1.SelectedText = _Modul1.Instance.UbgT;
                                        _Modul1.Instance.UbgT = "";
                                        RTf1.SelectionFont = new Font(OAus[5], (float)OAus[6].AsDouble(), FontStyle.Regular);
                                        if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.Loc].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            text4 = (text4 +  "Locator: " +  DataModul.DB_PlaceTable.Fields[PlaceFields.Loc].Value +  "; ").AsString();
                                        }
                                        goto IL_0a27;
                                    }
                                    goto IL_1373;
                                }

                                string text5 = "Text2";
                                RTf1.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\" + text5 + ".RTF", RichTextBoxStreamType.RichText);
                                RTf1.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\" + text5 + ".RTF", RichTextBoxStreamType.RichText);
                                Command1[4].Enabled = true;
                                Command1[3].Enabled = true;
                                goto end_IL_0000_2;
                            IL_1451:
                                num = 212;
                                Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            IL_1479:
                                num = 216;
                                DataModul.MandDB.Close();
                                DataModul.DOSB.Close();
                                DataModul.TempDB.Close();
                                DataModul.DSB.Close();
                                ProjectData.ClearProjectError();
                                num3 = 3;
                                Interaction.Shell(_Modul1.Instance.GenFreeDir + "Gen_Plus.exe", AppWinStyle.NormalFocus);
                                ProjectData.EndApp();
                                goto end_IL_0000_2;
                            IL_14ee:
                                num = 225;
                                text6 = FileSystem.CurDir().Left(1);
                                MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                                MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
                                MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                                MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                                switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                                {
                                    case 1:
                                RTf1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                                        break;
                                    case 2:
                                        goto IL_15d9;
                                    default:
                                        break;
                                }
                                goto IL_1600;
                            IL_15d9:
                                num = 237;
                                RTf1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                                goto IL_1600;
                            IL_1600: // <========== 4
                                num = 239;
                                FileSystem.ChDrive(text6 + ":");
                                goto end_IL_0000_2;
                            IL_1622:
                                num = 243;
                                text5 = "Text2";
                                RTf1.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\" + text5 + ".RTF", RichTextBoxStreamType.RichText);
                                RTf1.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\" + text5 + ".RTF", RichTextBoxStreamType.RichText);
                                Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\" + text5 + ".RTF", AppWinStyle.MaximizedFocus);
                                goto end_IL_0000_2;
                            IL_16db:
                                num = 249;
                                Show();
                                num5 = 0L;
                                while (num5 <= 3)
                                {
                                    Check1[(short)num5].Visible = false;
                                    num5++;
                                }
                                Command1[3].Visible = false;
                                Command1[4].Visible = false;
                                int num6 = 0;
                                foreach (var cEv in DataModul.Event.ReadAll(EventIndex.EOrt))
                                {
                                    if (cEv.iOrt != num6)
                                    {
                                        MyProject.Forms.Hinter.List3.Items.Add(new ListItem(cEv.iOrt.ToString(), cEv.iOrt));
                                        num6 = cEv.iOrt;
                                    }
                                }
                                RTf1.Text = "";
                                RTf1.Top = (int)Math.Round(1200.0 / 1440.0 * DeviceDpi);
                                RTf1.Visible = true;
                                DataModul.DOSB_OrtSTable.Index = "Ortsu";
                                num7 = 1L;
                                num8 = MyProject.Forms.Hinter.List3.Items.Count - 1;
                                num5 = num7;
                                goto IL_1a98;
                            IL_1a8c:
                                num = 280;
                                num5++;
                                goto IL_1a98;
                            IL_1a98:
                                if (num5 <= num8)
                                {
                                    DataModul.DB_PlaceTable.Seek("=", MyProject.Forms.Hinter.List3.SelectedItem.ItemData<int>(), num5);
                                    if (Operators.CompareString(DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString().Trim(), "0", TextCompare: false) != 0)
                                    {
                                        RTf1.SelectedText = DataModul.DB_PlaceTable.Fields[PlaceFields.PLZ].AsString() + "; " +
                                            MyProject.Forms.Hinter.List3.Items[(int)num5].ItemData<int>().AsString() + '\r' + '\n';
                                    }
                                    goto IL_1a8c;
                                }
                                MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt";
                                MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
                                MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                                MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                                RTf1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                                goto end_IL_0000_2;
                            IL_1c32:
                                num = 304;
                                if (Information.Err().Number != 5)
                                {
                                    goto end_IL_0000_2;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_1c6c;
                            IL_1c6c: // <========== 4
                                num4 = unchecked(num2 + 1);
                                while (true)
                                {
                                    num2 = 0;
                                    switch (num4)
                                    {
                                        case 1:
                                            break;
                                        case 51:
                                        case 52:
                                            goto IL_04ef;
                                        case 41:
                                        case 42:
                                        case 53:
                                            goto IL_04fd;
                                        case 77:
                                        case 78:
                                        case 79:
                                            goto IL_079c;
                                        case 105:
                                        case 106:
                                            goto IL_0a27;
                                        case 116:
                                        case 117:
                                            goto IL_0b74;
                                        case 112:
                                        case 118:
                                        case 119:
                                        case 120:
                                        case 121:
                                        case 122:
                                            goto IL_0b90;
                                        case 132:
                                        case 133:
                                            goto IL_0ce6;
                                        case 128:
                                        case 134:
                                        case 135:
                                        case 136:
                                        case 137:
                                        case 138:
                                            goto IL_0d05;
                                        case 141:
                                        case 142:
                                        case 143:
                                            goto IL_0dab;
                                        case 145:
                                        case 146:
                                            goto IL_0e1f;
                                        case 148:
                                        case 149:
                                            goto IL_0e99;
                                        case 151:
                                        case 152:
                                            goto IL_0f13;
                                        case 155:
                                        case 157:
                                            goto IL_0fb9;
                                        case 159:
                                        case 160:
                                            goto IL_1022;
                                        case 167:
                                        case 168:
                                        case 169:
                                            goto IL_10d1;
                                        case 183:
                                        case 184:
                                        case 185:
                                            goto IL_122c;
                                        case 198:
                                        case 199:
                                        case 200:
                                        case 201:
                                            goto IL_1373;
                                        case 81:
                                        case 82:
                                        case 204:
                                            goto IL_13a7;
                                        case 231:
                                        case 235:
                                        case 238:
                                        case 239:
                                            goto IL_1600;
                                        case 279:
                                        case 280:
                                            goto IL_1a8c;
                                        case 302:
                                            goto IL_1c15;
                                        case 8:
                                        case 210:
                                        case 214:
                                        case 222:
                                        case 223:
                                        case 240:
                                        case 241:
                                        case 247:
                                        case 286:
                                        case 287:
                                        case 297:
                                        case 298:
                                        case 301:
                                        case 306:
                                            goto end_IL_0000_2;
                                    }
                                    break;
                                IL_1c15:
                                    num = 302;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 8518;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 10
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Ort_Load(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            int lErl = default;
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
                            RTf1.Left = 0;
                            goto IL_000f;
                        case 2412:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                    case 3:
                                        break;
                                    case 1:
                                        goto IL_0772;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 62)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_0491;
                                }
                                if (Information.Err().Number == 75)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0772;
                                }
                                if (Information.Err().Number == 91)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0772;
                                }
                                if (Information.Err().Number == 3376)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0772;
                                }
                                Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkOnly, Information.Err().Number.AsString());
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0776;
                            }
                        end_IL_0000:
                            break;
                        IL_000f:
                            num = 2;
                            RTf1.Width = Width;
                            RTf1.Height = checked(Height - 60);
                            byte b = 1;
                            while (b <= 4u)
                            {
                                Command1[b].Top = checked(RTf1.Height + 10);
                                b = checked((byte)unchecked((uint)(b + 1)));
                            }
                            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<Enum>("Windowstate");
                            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
                            BackColor = _Modul1.Instance.HintFarb;
                            Label1[3].Text = _Modul1.Instance.VersionT;
                            Label1[1].Text = "(c) 1994-2018 Gisbert Berwe 49082 Osnabrück Friedrich-Holthaus-Str. 18 Tel.: 0541-80 00 79 00";
                            Label1[2].Text = "Version 24.09.04 Stand 25.11.2018";
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            DataModul.TempDB.Close();
                            _Modul1.Instance.Dateienopen();
                            RTf1.SelectionFont = RTf1.SelectionFont.ChangeFName(_Modul1.Instance.Font1);
                            Show();
                            Command1[3].Text = _Modul1.Instance.IText[47];
                            _Modul1.Instance.Feg = (short)_Modul1.Instance.Persistence.ReadIntInit("state");
                            _Modul1.Instance.Fs = _Modul1.Instance.Feg switch
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
                                _ => _Modul1.Instance.Fs,
                            };
                            goto IL_0388;
                        IL_0388: // <========== 12
                            num = 67;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Label1[3].Width = Width;
                            Label1[1].Width = Width;
                            Label1[2].Width = Width;
                            _Modul1.Instance.Persistence.ReadStringsOutput( "Ort.dat", OAus, 7);
                            b = 0;
                            while (b <= 6u)
                            {
                                OAus[b] = FileSystem.LineInput(99);
                                b = checked((byte)unchecked((uint)(b + 1)));
                            }
                            goto IL_0491;
                        IL_0491: // <========== 3
                            num = 79;
                            lErl = 6;
                            if (OAus[1] == "")
                            {
                                OAus[1] = "0";
                            }
                            if (OAus[2] == "")
                            {
                                OAus[2] = "0";
                            }
                            if (OAus[3] == "")
                            {
                                OAus[3] = "0";
                            }
                            if (OAus[4] == "")
                            {
                                OAus[4] = "0";
                            }
                            if (OAus[5].Length < 3)
                            {
                                OAus[5] = "Arial";
                            }
                            if (OAus[6].AsDouble() < 8.0)
                            {
                                OAus[6] = "8";
                            }
                            Check1[0].CheckState = (CheckState)OAus[1].AsInt();
                            Check1[1].CheckState = (CheckState)OAus[2].AsInt();
                            Check1[2].CheckState = (CheckState)OAus[3].AsInt();
                            Check1[3].CheckState = (CheckState)OAus[4].AsInt();
                            Label2.Text = "Schriftart und Grösse: " + OAus[5] + " / " + OAus[6] + " Punkt";
                            goto end_IL_0000_2;
                        IL_0772: // <========== 4
                            num4 = num2 + 1;
                            goto IL_0776;
                        IL_0776:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 35:
                                case 39:
                                case 42:
                                case 45:
                                case 48:
                                case 51:
                                case 54:
                                case 57:
                                case 60:
                                case 63:
                                case 66:
                                case 67:
                                    goto IL_0388;
                                case 79:
                                case 106:
                                    goto IL_0491;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 2412;
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

        private void Ort_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
        {
            //Discarded unreachable code: IL_00ef
            int try0000_dispatch = -1;
            int num = default;
            bool cancel = default;
            int num2 = default;
            int num3 = default;
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
                            cancel = eventArgs.Cancel;
                            goto IL_000a;
                        case 341:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_00f3;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number != 91)
                                {
                                    break;
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_00f3;
                            }
                        end_IL_0000_3:
                            break;
                        IL_000a:
                            num = 2;
                            CloseReason closeReason = eventArgs.CloseReason;
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            FileSystem.FileClose(6);
                            FileSystem.FileOpen(6, _Modul1.Instance.GenFreeDir + "\\Init\\Windowstate", OpenMode.Output);
                            FileSystem.PrintLine(6, WindowState);
                            if (closeReason != 0)
                            {
                                }
                            else
                            {
                            DataModul.MandDB.Close();
                            DataModul.DOSB.Close();
                            DataModul.TempDB.Close();
                            DataModul.DSB.Close();
                            ProjectData.EndApp();
                            }
                            goto end_IL_0000_2;
                        IL_00f3:
                            num4 = num2 + 1;
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 17:
                                case 19:
                                    goto end_IL_0000_3;
                                case 12:
                                case 13:
                                case 14:
                                case 20:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                    num = 19;
                    eventArgs.Cancel = cancel;
                    break;
                end_IL_0000:
                    ;
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 341;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 3
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            MyProject.Forms.Hinter.CommonDialog1Font.Font = MyProject.Forms.Hinter.CommonDialog1Font.Font.ChangeFName(OAus[5]);
            MyProject.Forms.Hinter.CommonDialog1Font.Font = MyProject.Forms.Hinter.CommonDialog1Font.Font.ChangeFSize( (float)OAus[6].AsDouble());
            MyProject.Forms.Hinter.CommonDialog1Font.ShowDialog();
            OAus[5] = MyProject.Forms.Hinter.CommonDialog1Font.Font.Name;
            OAus[6] = Conversion.Int(MyProject.Forms.Hinter.CommonDialog1Font.Font.Size).AsString();
            Label2.Text = "Schriftart und Grösse: " + OAus[5] + " / " + OAus[6] + " Punkt";
        }

        private void _Command1_0_Click(object sender, EventArgs e)
        {
        }
    }
}
