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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Druck
{
    [DesignerGenerated]
    internal class Nachlist : Form
    {
        private IContainer components;

        public ToolTip ToolTip1;

        [AccessedThroughProperty("RichTextBox1")]
        private RichTextBox _RichTextBox1;

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

        [AccessedThroughProperty("_Befehl_1")]
        private Button __Befehl_1;

        [AccessedThroughProperty("_Befehl_2")]
        private Button __Befehl_2;

        [AccessedThroughProperty("_Befehl_3")]
        private Button __Befehl_3;

        [AccessedThroughProperty("Liste2")]
        private ListBox _Liste2;

        [AccessedThroughProperty("Bezeichnung1")]
        private Label _Bezeichnung1;

        [AccessedThroughProperty("Bezeichnung2")]
        private Label _Bezeichnung2;

        [AccessedThroughProperty("Bezeichnung6")]
        private Label _Bezeichnung6;

        [AccessedThroughProperty("Bezeichnung7")]
        private Label _Bezeichnung7;

        [AccessedThroughProperty("Bezeichnung3")]
        private Label _Bezeichnung3;

        [AccessedThroughProperty("Befehl")]
        private ControlArray<Button> _Befehl;

        [AccessedThroughProperty("Command_Renamed")]
        private ControlArray<Button> _Command_Renamed;

        [AccessedThroughProperty("Label1")]
        private ControlArray<Label> _Label1;

        private string Gene;

        private string DA;

        public virtual RichTextBox RichTextBox1
        {
            get
            {
                return _RichTextBox1;
            }
     
            set
            {
                _RichTextBox1 = value;
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
                __Command_1 = value;
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

        public virtual Button _Befehl_1
        {
            get
            {
                return __Befehl_1;
            }
     
            set
            {
                __Befehl_1 = value;
            }
        }

        public virtual Button _Befehl_2
        {
            get
            {
                return __Befehl_2;
            }
     
            set
            {
                __Befehl_2 = value;
            }
        }

        public virtual Button _Befehl_3
        {
            get
            {
                return __Befehl_3;
            }
     
            set
            {
                __Befehl_3 = value;
            }
        }

        public virtual ListBox Liste2
        {
            get
            {
                return _Liste2;
            }
     
            set
            {
                _Liste2 = value;
            }
        }

        public virtual Label Bezeichnung1
        {
            get
            {
                return _Bezeichnung1;
            }
     
            set
            {
                _Bezeichnung1 = value;
            }
        }

        public virtual Label Bezeichnung2
        {
            get
            {
                return _Bezeichnung2;
            }
     
            set
            {
                _Bezeichnung2 = value;
            }
        }

        public virtual Label Bezeichnung6
        {
            get
            {
                return _Bezeichnung6;
            }
     
            set
            {
                _Bezeichnung6 = value;
            }
        }

        public virtual Label Bezeichnung7
        {
            get
            {
                return _Bezeichnung7;
            }
     
            set
            {
                _Bezeichnung7 = value;
            }
        }

        public virtual Label Bezeichnung3
        {
            get
            {
                return _Bezeichnung3;
            }
     
            set
            {
                _Bezeichnung3 = value;
            }
        }

        public virtual ControlArray<Button> Befehl
        {
            get
            {
                return _Befehl;
            }
     
            set
            {
                EventHandler obj = Befehl_Click;
                if (_Befehl != null)
                {
                    _Befehl.RemoveClick(obj);
                }
                _Befehl = value;
                if (_Befehl != null)
                {
                    _Befehl.AddClick(obj);
                }
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

        [DebuggerNonUserCode]
        public Nachlist()
        {
            base.Load += Nachlist_Load;
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
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this._Command_0 = new System.Windows.Forms.Button();
            this._Command_1 = new System.Windows.Forms.Button();
            this._Label1_0 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_3 = new System.Windows.Forms.Label();
            this._Label1_4 = new System.Windows.Forms.Label();
            this._Label1_5 = new System.Windows.Forms.Label();
            this._Befehl_1 = new System.Windows.Forms.Button();
            this._Befehl_2 = new System.Windows.Forms.Button();
            this._Befehl_3 = new System.Windows.Forms.Button();
            this.Liste2 = new System.Windows.Forms.ListBox();
            this.Bezeichnung1 = new System.Windows.Forms.Label();
            this.Bezeichnung2 = new System.Windows.Forms.Label();
            this.Bezeichnung6 = new System.Windows.Forms.Label();
            this.Bezeichnung7 = new System.Windows.Forms.Label();
            this.Bezeichnung3 = new System.Windows.Forms.Label();
            this.Befehl = new ControlArray<System.Windows.Forms.Button>();
            this.Command_Renamed = new ControlArray<System.Windows.Forms.Button>();
            this.Label1 = new ControlArray<System.Windows.Forms.Label>();
            this.Frame1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.Befehl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
            this.SuspendLayout();
            System.Drawing.Point point2 = this.RichTextBox1.Location = new System.Drawing.Point(3, 28);
            System.Windows.Forms.Padding padding2 = this.RichTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.RichTextBox1.Name = "RichTextBox1";
            System.Drawing.Size size2 = this.RichTextBox1.Size = new System.Drawing.Size(941, 636);
            this.RichTextBox1.TabIndex = 18;
            this.RichTextBox1.Text = "";
            this.RichTextBox1.Visible = false;
            this.Frame1.BackColor = System.Drawing.Color.Red;
            this.Frame1.Controls.Add(this._Command_0);
            this.Frame1.Controls.Add(this._Command_1);
            this.Frame1.Controls.Add(this._Label1_0);
            this.Frame1.Controls.Add(this._Label1_1);
            this.Frame1.Controls.Add(this._Label1_2);
            this.Frame1.Controls.Add(this._Label1_3);
            this.Frame1.Controls.Add(this._Label1_4);
            this.Frame1.Controls.Add(this._Label1_5);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Frame1.Location = new System.Drawing.Point(149, 118);
            padding2 = this.Frame1.Margin = new System.Windows.Forms.Padding(4);
            this.Frame1.Name = "Frame1";
            padding2 = this.Frame1.Padding = new System.Windows.Forms.Padding(4);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Frame1.Size = new System.Drawing.Size(543, 230);
            this.Frame1.TabIndex = 9;
            this.Frame1.TabStop = false;
            this.Frame1.Visible = false;
            this._Command_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command_Renamed.SetIndex(this._Command_0, 0);
            point2 = this._Command_0.Location = new System.Drawing.Point(47, 192);
            padding2 = this._Command_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command_0.Name = "_Command_0";
            this._Command_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command_0.Size = new System.Drawing.Size(141, 26);
            this._Command_0.TabIndex = 11;
            this._Command_0.Text = "Neue Auswahl";
            this._Command_0.UseVisualStyleBackColor = false;
            this._Command_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command_Renamed.SetIndex(this._Command_1, 1);
            point2 = this._Command_1.Location = new System.Drawing.Point(327, 192);
            padding2 = this._Command_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command_1.Name = "_Command_1";
            this._Command_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command_1.Size = new System.Drawing.Size(141, 26);
            this._Command_1.TabIndex = 10;
            this._Command_1.Text = "OK";
            this._Command_1.UseVisualStyleBackColor = false;
            this._Label1_0.BackColor = System.Drawing.Color.White;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_0, 0);
            point2 = this._Label1_0.Location = new System.Drawing.Point(11, 46);
            padding2 = this._Label1_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_0.Size = new System.Drawing.Size(524, 20);
            this._Label1_0.TabIndex = 17;
            this._Label1_1.BackColor = System.Drawing.Color.White;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_1, 1);
            point2 = this._Label1_1.Location = new System.Drawing.Point(9, 101);
            padding2 = this._Label1_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_1.Size = new System.Drawing.Size(524, 20);
            this._Label1_1.TabIndex = 16;
            this._Label1_2.BackColor = System.Drawing.Color.White;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_2, 2);
            point2 = this._Label1_2.Location = new System.Drawing.Point(9, 128);
            padding2 = this._Label1_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_2.Size = new System.Drawing.Size(524, 20);
            this._Label1_2.TabIndex = 15;
            this._Label1_3.BackColor = System.Drawing.Color.White;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_3, 3);
            point2 = this._Label1_3.Location = new System.Drawing.Point(9, 18);
            padding2 = this._Label1_3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_3.Size = new System.Drawing.Size(524, 20);
            this._Label1_3.TabIndex = 14;
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_4.BackColor = System.Drawing.Color.White;
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_4, 4);
            point2 = this._Label1_4.Location = new System.Drawing.Point(9, 156);
            padding2 = this._Label1_4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_4.Size = new System.Drawing.Size(524, 20);
            this._Label1_4.TabIndex = 13;
            this._Label1_5.BackColor = System.Drawing.Color.White;
            this._Label1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_5, 5);
            point2 = this._Label1_5.Location = new System.Drawing.Point(8, 73);
            padding2 = this._Label1_5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_5.Name = "_Label1_5";
            this._Label1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_5.Size = new System.Drawing.Size(524, 20);
            this._Label1_5.TabIndex = 12;
            this._Befehl_1.BackColor = System.Drawing.SystemColors.Control;
            this._Befehl_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl.SetIndex(this._Befehl_1, 1);
            point2 = this._Befehl_1.Location = new System.Drawing.Point(66, 682);
            padding2 = this._Befehl_1.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl_1.Name = "_Befehl_1";
            this._Befehl_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl_1.Size = new System.Drawing.Size(96, 30);
            this._Befehl_1.TabIndex = 1;
            this._Befehl_1.Text = "&Drucken";
            this._Befehl_1.UseVisualStyleBackColor = false;
            this._Befehl_2.BackColor = System.Drawing.SystemColors.Control;
            this._Befehl_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl_2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._Befehl_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl.SetIndex(this._Befehl_2, 2);
            point2 = this._Befehl_2.Location = new System.Drawing.Point(858, 682);
            padding2 = this._Befehl_2.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl_2.Name = "_Befehl_2";
            this._Befehl_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl_2.Size = new System.Drawing.Size(125, 30);
            this._Befehl_2.TabIndex = 3;
            this._Befehl_2.Text = "Druck&menü";
            this._Befehl_2.UseVisualStyleBackColor = false;
            this._Befehl_3.BackColor = System.Drawing.SystemColors.Control;
            this._Befehl_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl.SetIndex(this._Befehl_3, 3);
            point2 = this._Befehl_3.Location = new System.Drawing.Point(191, 682);
            padding2 = this._Befehl_3.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl_3.Name = "_Befehl_3";
            this._Befehl_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl_3.Size = new System.Drawing.Size(107, 30);
            this._Befehl_3.TabIndex = 2;
            this._Befehl_3.Text = "&speichern";
            this._Befehl_3.UseVisualStyleBackColor = false;
            this.Liste2.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Liste2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Liste2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Liste2.ForeColor = System.Drawing.Color.Black;
            this.Liste2.ItemHeight = 17;
            point2 = this.Liste2.Location = new System.Drawing.Point(693, 146);
            padding2 = this.Liste2.Margin = new System.Windows.Forms.Padding(4);
            this.Liste2.Name = "Liste2";
            this.Liste2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Liste2.Size = new System.Drawing.Size(127, 121);
            this.Liste2.Sorted = true;
            this.Liste2.TabIndex = 6;
            this.Liste2.Visible = false;
            this.Bezeichnung1.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung1.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung1.Location = new System.Drawing.Point(0, 24);
            padding2 = this.Bezeichnung1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung1.Name = "Bezeichnung1";
            this.Bezeichnung1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung1.Size = new System.Drawing.Size(1048, 21);
            this.Bezeichnung1.TabIndex = 0;
            this.Bezeichnung1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Bezeichnung2.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung2.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung2.Location = new System.Drawing.Point(0, 105);
            padding2 = this.Bezeichnung2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung2.Name = "Bezeichnung2";
            this.Bezeichnung2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung2.Size = new System.Drawing.Size(832, 21);
            this.Bezeichnung2.TabIndex = 4;
            this.Bezeichnung2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Bezeichnung6.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung6.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung6.Location = new System.Drawing.Point(0, 126);
            padding2 = this.Bezeichnung6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung6.Name = "Bezeichnung6";
            this.Bezeichnung6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung6.Size = new System.Drawing.Size(213, 21);
            this.Bezeichnung6.TabIndex = 5;
            this.Bezeichnung6.Text = "Personen aufbereiten";
            this.Bezeichnung7.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung7.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung7.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung7.Location = new System.Drawing.Point(0, 167);
            padding2 = this.Bezeichnung7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung7.Name = "Bezeichnung7";
            this.Bezeichnung7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung7.Size = new System.Drawing.Size(832, 21);
            this.Bezeichnung7.TabIndex = 7;
            this.Bezeichnung3.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung3.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung3.Location = new System.Drawing.Point(0, 3);
            padding2 = this.Bezeichnung3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung3.Name = "Bezeichnung3";
            this.Bezeichnung3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung3.Size = new System.Drawing.Size(1048, 21);
            this.Bezeichnung3.TabIndex = 8;
            this.Bezeichnung3.Text = "Nachfahrenliste";
            this.Bezeichnung3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            System.Drawing.SizeF sizeF2 = this.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.CancelButton = this._Befehl_2;
            size2 = this.ClientSize = new System.Drawing.Size(1018, 725);
            this.Controls.Add(this.RichTextBox1);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this._Befehl_1);
            this.Controls.Add(this._Befehl_2);
            this.Controls.Add(this._Befehl_3);
            this.Controls.Add(this.Liste2);
            this.Controls.Add(this.Bezeichnung1);
            this.Controls.Add(this.Bezeichnung2);
            this.Controls.Add(this.Bezeichnung6);
            this.Controls.Add(this.Bezeichnung7);
            this.Controls.Add(this.Bezeichnung3);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            padding2 = this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Nachlist";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Nachfahrenliste";
            this.Frame1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.Befehl).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
            this.ResumeLayout(false);
        }

        private void Befehl_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            int number = default;
            string prompt = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                    string text;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Befehl.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 1043:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0319;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_0248;
                            }
                        IL_0319:
                            num4 = num2 + 1;
                            goto IL_031d;
                        IL_0248:
                            num = 36;
                            number = Information.Err().Number;
                            goto IL_0258;
                        IL_0258:
                            num = 39;
                            if (number == 25)
                            {
                                goto IL_0262;
                            }
                            goto IL_029b;
                        IL_029b:
                            num = 46;
                            if (number == 55)
                            {
                                goto IL_02a5;
                            }
                            goto IL_02d0;
                        IL_02d0:
                            num = 52;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_02f6;
                        IL_00ad:
                            num = 9;
                            RichTextBox1.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            goto IL_00cc;
                        IL_02f6:
                            num = 55;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0315;
                        IL_00cc:
                            num = 10;
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
                            goto end_IL_0000_2;
                        IL_0315:
                            num4 = num2;
                            goto IL_031d;
                        IL_031d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0015;
                                case 3:
                                    goto IL_001d;
                                case 4:
                                    goto IL_0073;
                                case 6:
                                case 8:
                                    goto IL_008f;
                                case 9:
                                    goto IL_00ad;
                                case 10:
                                    goto IL_00cc;
                                case 12:
                                case 13:
                                    goto IL_00f9;
                                case 14:
                                    goto IL_010d;
                                case 15:
                                    goto IL_0117;
                                case 18:
                                case 19:
                                    goto IL_0134;
                                case 20:
                                    goto IL_0151;
                                case 21:
                                    goto IL_0178;
                                case 22:
                                    goto IL_0191;
                                case 23:
                                    goto IL_01aa;
                                case 24:
                                    goto IL_01d0;
                                case 26:
                                case 28:
                                    goto IL_01f9;
                                case 30:
                                case 31:
                                    goto IL_021f;
                                case 36:
                                    goto IL_0248;
                                case 38:
                                case 39:
                                    goto IL_0258;
                                case 40:
                                    goto IL_0262;
                                case 41:
                                    goto IL_026c;
                                case 42:
                                    goto IL_027f;
                                case 46:
                                    goto IL_029b;
                                case 47:
                                    goto IL_02a5;
                                case 48:
                                    goto IL_02b4;
                                case 51:
                                case 52:
                                    goto IL_02d0;
                                case 53:
                                case 55:
                                    goto IL_02f6;
                                default:
                                    goto end_IL_0000;
                                case 5:
                                case 11:
                                case 16:
                                case 17:
                                case 25:
                                case 29:
                                case 32:
                                case 33:
                                case 34:
                                case 35:
                                case 37:
                                case 43:
                                case 45:
                                case 49:
                                case 50:
                                case 56:
                                case 57:
                                case 58:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_02a5:
                            num = 47;
                            FileSystem.FileClose();
                            goto IL_02b4;
                        IL_02b4:
                            num = 48;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0315;
                        IL_008f:
                            num = 8;
                            RichTextBox1.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            goto IL_00ad;
                        IL_0262:
                            num = 40;
                            prompt = "Das angegebene Gerät ist nicht bereit.\rBitte einschalten oder abbrechen.";
                            goto IL_026c;
                        IL_026c:
                            num = 41;
                            if (Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel, "Fehler") == MsgBoxResult.Cancel)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_027f;
                        IL_027f:
                            num = 42;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_0315;
                        IL_0117:
                            num = 15;
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_0015:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_001d;
                        IL_001d:
                            num = 3;
                            text = "Datum " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
                            goto IL_0073;
                        IL_0073:
                            num = 4;
                            switch (index)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_00f9;
                                case 3:
                                    goto IL_0134;
                                default:
                                    goto end_IL_0000_2;
                            }
                            goto IL_008f;
                        IL_0134:
                            num = 19;
                            MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                            goto IL_0151;
                        IL_0151:
                            num = 20;
                            MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
                            goto IL_0178;
                        IL_0178:
                            num = 21;
                            MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                            goto IL_0191;
                        IL_0191:
                            num = 22;
                            MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                            goto IL_01aa;
                        IL_01aa:
                            num = 23;
                            if (MyProject.Forms.Hinter.CommonDialog1Save.FileName ==  "")
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_01d0;
                        IL_01d0:
                            num = 24;
                            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_021f;
                                default:
                                    goto end_IL_0000_2;
                            }
                            goto IL_01f9;
                        IL_021f:
                            num = 31;
                            RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_01f9:
                            num = 28;
                            RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                            goto end_IL_0000_2;
                        IL_00f9:
                            num = 13;
                            RichTextBox1.Text = "";
                            goto IL_010d;
                        IL_010d:
                            num = 14;
                            Close();
                            goto IL_0117;
                        end_IL_0000:
                            break;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 1043;
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

        private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            int number = default;
            string text = default;
            string[] array = default;
            string inputStr = default;
            byte b = default;
            byte b2 = default;
            byte b3 = default;
            byte b4 = default;
            int num5 = default;
            int lErl = default;
            string text4 = default;
            object CounterResult = default;
            object LoopForResult = default;
            object CounterResult2 = default;
            object LoopForResult2 = default;
            ELinkKennz kennz = default;
            object CounterResult3 = default;
            object LoopForResult3 = default;
            ELinkKennz kennz2 = default;
            object LoopForResult4 = default;
            string text5 = default;
            IList<int> aiFams;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    checked
                    {
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                index = Command_Renamed.GetIndex((Button)eventSender);
                                goto IL_0016;
                            case 5063:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 1:
                                            break;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    while (true)
                                    {
                                        int num4 = unchecked(num2 + 1);
                                        while (true)
                                        {
                                            num2 = 0;
                                            switch (num4)
                                            {
                                                case 1:
                                                    break;
                                                case 43:
                                                case 59:
                                                case 64:
                                                    goto IL_037c;
                                                case 65:
                                                    num = 65;
                                                    RichTextBox1.SelectedText = "\n";
                                                    goto IL_0494;
                                                case 66:
                                                case 67:
                                                    goto IL_0494;
                                                case 72:
                                                case 73:
                                                    goto IL_0546;
                                                case 100:
                                                case 103:
                                                case 104:
                                                    goto IL_07e4;
                                                case 108:
                                                    goto IL_083d;
                                                case 114:
                                                case 118:
                                                case 122:
                                                case 126:
                                                case 127:
                                                    goto IL_0a34;
                                                case 133:
                                                case 137:
                                                    goto IL_0aef;
                                                case 146:
                                                    goto IL_0bd6;
                                                case 152:
                                                case 155:
                                                case 158:
                                                case 159:
                                                    goto IL_0d07;
                                                case 161:
                                                case 164:
                                                case 165:
                                                    goto IL_0d49;
                                                case 170:
                                                case 171:
                                                case 174:
                                                case 175:
                                                    goto IL_0daa;
                                                case 176:
                                                case 180:
                                                case 181:
                                                    goto IL_0dee;
                                                case 196:
                                                    num = 196;
                                                    number = Information.Err().Number;
                                                    goto case 198;
                                                case 198:
                                                case 199:
                                                    num = 199;
                                                    if (number == 55)
                                                    {
                                                        ProjectData.ClearProjectError();
                                                        if (num2 == 0)
                                                        {
                                                            throw ProjectData.CreateProjectError(-2146828268);
                                                        }
                                                    }
                                                    goto case 203;
                                                case 203:
                                                    num = 203;
                                                    if (number == 75)
                                                    {
                                                        ProjectData.ClearProjectError();
                                                        if (num2 == 0)
                                                        {
                                                            throw ProjectData.CreateProjectError(-2146828268);
                                                        }
                                                        continue;
                                                    }
                                                    goto case 207;
                                                case 207:
                                                case 208:
                                                    num = 208;
                                                    if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                                    {
                                                        ProjectData.EndApp();
                                                    }
                                                    goto case 209;
                                                case 209:
                                                case 211:
                                                    num = 211;
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                    goto case 55;
                                                case 55:
                                                case 215:
                                                    goto end_IL_0000_2;
                                                case 217:
                                                    num = 217;
                                                    ProjectData.ClearProjectError();
                                                    if (num2 == 0)
                                                    {
                                                        throw ProjectData.CreateProjectError(-2146828268);
                                                    }
                                                    num4 = num2;
                                                    continue;
                                                case 6:
                                                case 11:
                                                case 20:
                                                case 194:
                                                case 195:
                                                case 197:
                                                case 201:
                                                case 202:
                                                case 205:
                                                case 206:
                                                case 212:
                                                case 213:
                                                case 214:
                                                case 216:
                                                case 218:
                                                    goto end_IL_0000_3;
                                            }
                                            break;
                                        }
                                        break;
                                    }
                                    goto default;
                                }
                            end_IL_0000_2:
                                break;
                            IL_0016:
                                num = 2;
                                text = "";
                                array = new string[201];
                                RichTextBox1.SelectionFont = RichTextBox1.SelectionFont.ChangeFName(_Modul1.Instance.Font1);
                                switch (index)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        goto IL_0081;
                                    default:
                                        goto end_IL_0000_3;
                                }
                                Close();
                                Show();
                                goto end_IL_0000_3;
                            IL_0081:
                                num = 13;
                                Frame1.Visible = false;
                                RichTextBox1.Visible = true;
                                inputStr = Interaction.InputBox("Wieviel Generationen", "", "100");
                                b = (byte)Math.Round(inputStr.AsDouble());
                                if (b == 0)
                                {
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000_3;
                                }
                                b2 = (byte)Interaction.MsgBox("Sollen die Linien der weiblichen Nachkommen angezeigt werden?", MsgBoxStyle.YesNo, "Weibliche Linien");
                                Bezeichnung2.Text = b.AsString() + " Generationen";
                                b3 = 1;
                                Gene = Strings.Right("0" + b3.AsString().Trim(), 2);
                                b4 = 0;
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1.SelectedText = _Modul1.Instance.IText[78];
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                RichTextBox1.SelectedText = " " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.TrimEnd()).Trim() + " ";
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = _Modul1.Instance.Kont[1].Trim() + " ";
                                }
                                string sM_Namen = _Modul1.Instance.Kont[0];
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1.SelectedText = sM_Namen + "\n";
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1.SelectedText = "Erstellt am " + DateTime.Today.AsString();
                                RichTextBox1.SelectedText = $" von {_Modul1.Instance.User.Name.Trim()} mit {_Modul1.Instance.AppName} aus Mandant: {_Modul1.Instance.Verz}\n";
                                RichTextBox1.SelectedText = "\n";
                                num5 = 1;
                                goto IL_037c;
                            IL_037c: // <========== 5
                                num = 43;
                                lErl = 1;
                                if (num5 > 1)
                                {
                                    text += ">";
                                    if (text.Length == 70)
                                    {
                                        text = ">";
                                    }
                                    Bezeichnung1.Text = text;
                                    Bezeichnung1.Refresh();
                                    if (b4 == 0)
                                    {
                                        RichTextBox1.SelectedText = "**** Ende der Liste ****";
                                        RichTextBox1.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                                        RichTextBox1.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                                        break;
                                    }
                                    if (b4 == b)
                                    {
                                        b4 = (byte)(unchecked(b4) - 1);
                                        goto IL_037c;
                                    }
                                    if (array[b4].Length < 20)
                                    {
                                        array[b4] = "";
                                        b4 = (byte)(unchecked(b4) - 1);
                                        goto IL_037c;
                                    }
                                    goto IL_0494;
                                }
                                goto IL_0546;
                            IL_0494: // <========== 3
                                num = 67;
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.PersInArb = (int)Math.Round(Strings.Mid(array[b4], 10, 10).AsDouble());
                                b3 = (byte)Math.Round(array[b4].Right(3).AsDouble());
                                Gene = Strings.Right("0" + b3.AsString().Trim(), 2);
                                array[b4] = Strings.Mid(array[b4], 20, array[b4].Length);
                                goto IL_0546;
                            IL_0546: // <========== 3
                                num = 73;
                                _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                DA = "    ";
                                RichTextBox1.SelectionIndent = 10 * unchecked(b3);
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1.SelectedText = Gene + " " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.TrimEnd()).Trim() + " ";
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = _Modul1.Instance.Kont[1] + " ";
                                }
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1.SelectedText = _Modul1.Instance.Kont[0].TrimEnd() + " ";
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = _Modul1.Instance.Kont[2] + " ";
                                }
                                if (_Modul1.Instance.Kont[4].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = "(" + _Modul1.Instance.Kont[4] + ") ";
                                }
                                if (_Modul1.Instance.Kont[5].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = ", Sippe " + _Modul1.Instance.Kont[5] + " ";
                                }
                                LDat();
                                DA = "    ";
                                if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
                                {
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                    goto IL_07e4;
                                }
                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                goto IL_07e4;
                            IL_07e4: // <========== 3
                                num = 104;
                                text4 = "";
                                aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                if (_Modul1.Instance.UbgT.Trim() != "")
                                {
                                    if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 1, 30, 1, ref LoopForResult, ref CounterResult))
                                    {
                                        goto IL_083d;
                                    }
                                    goto IL_0aef;
                                }
                                goto IL_0dee;
                            IL_083d: // <========== 3
                                num = 108;
                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.Left(11).AsDouble());
                                _Modul1.Instance.Schalt = 1;
                                _Modul1.Instance.Famdatles2();
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    Liste2.Items.Add(_Modul1.Instance.Kont[2] + ">" + "                          " + _Modul1.Instance.FamInArb.AsString().Right(20));
                                    _Modul1.Instance.Schalt = 0;
                                    goto IL_0a34;
                                }
                                if (_Modul1.Instance.Kont[3].Trim() != "")
                                {
                                    Liste2.Items.Add(_Modul1.Instance.Kont[3] + ">" + "                          " + _Modul1.Instance.FamInArb.AsString().Right(20));
                                    _Modul1.Instance.Schalt = 0;
                                    goto IL_0a34;
                                }
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    Liste2.Items.Add(_Modul1.Instance.Kont[1] + ">" + "                          " + _Modul1.Instance.FamInArb.AsString().Right(20));
                                    _Modul1.Instance.Schalt = 0;
                                    goto IL_0a34;
                                }
                                if (_Modul1.Instance.Kont[0].Trim() != "")
                                {
                                    Liste2.Items.Add(_Modul1.Instance.Kont[0] + ">" + "                          " + _Modul1.Instance.FamInArb.AsString().Right(20));
                                    _Modul1.Instance.Schalt = 0;
                                }
                                goto IL_0a34;
                            IL_0a34: // <========== 5
                                num = 127;
                                if (_Modul1.Instance.Schalt == 1)
                                {
                                    Liste2.Items.Add("        >" + "           " + _Modul1.Instance.FamInArb.AsString().Right(20));
                                    _Modul1.Instance.Schalt = 0;
                                }
                                if (_Modul1.Instance.UbgT.Length <= 10)
                                {
                                    _Modul1.Instance.UbgT = "";
                                    goto IL_0aef;
                                }
                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                if (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                {
                                    goto IL_083d;
                                }
                                goto IL_0aef;
                            IL_0aef: // <========== 4
                                num = 137;
                                if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, Liste2.Items.Count - 1, 1, ref LoopForResult2, ref CounterResult2))
                                {
                                    while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult2, ref CounterResult2))
                                    {
                                        text4 += Strings.Mid(Liste2.Items[CounterResult2.AsInt()].AsString(), 10, 20);
                                    }
                                }
                                while (Liste2.Items.Count != 0)
                                {
                                    Liste2.Items.RemoveAt(0);
                                }
                                kennz = _Modul1.Instance.eLKennz;
                                if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult3, 1, 300, 1, ref LoopForResult3, ref CounterResult3))
                                {
                                    goto IL_0bd6;
                                }
                                goto IL_0dee;
                            IL_0bd6: // <========== 3
                                num = 146;
                                _Modul1.Instance.Schalt = 0;
                                _Modul1.Instance.FamInArb = (int)Math.Round(text4.Left(20).AsDouble());
                                _Modul1.Instance.Famdatles2();
                                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = Gene + " oo " + _Modul1.Instance.Kont[2] + "\n";
                                    goto IL_0d07;
                                }
                                if (_Modul1.Instance.Kont[3].Trim() != "")
                                {
                                    RichTextBox1.SelectedText = Gene + " oo " + _Modul1.Instance.Kont[3] + "\n";
                                    goto IL_0d07;
                                }
                                RichTextBox1.SelectedText = Gene + " oo \n";
                                goto IL_0d07;
                            IL_0d07: // <========== 4
                                num = 159;
                                if (kennz == ELinkKennz.lkFather)
                                {
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                    goto IL_0d49;
                                }
                                _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                goto IL_0d49;
                            IL_0d49: // <========== 3
                                num = 165;
                                kennz2 = _Modul1.Instance.eLKennz;
                                Persuch(_Modul1.Instance.FamInArb, _Modul1.Instance.eLKennz);
                                if (b2 == 7)
                                {
                                    if (kennz2 != ELinkKennz.lkFather)
                                    {
                                        Kisuch(_Modul1.Instance.FamInArb, Liste2.Items.Add);
                                    }
                                    goto IL_0daa;
                                }
                                Kisuch(_Modul1.Instance.FamInArb, Liste2.Items.Add);
                                goto IL_0daa;
                            IL_0daa: // <========== 3
                                num = 175;
                                if (text4.Length > 21)
                                {
                                    text4 = Strings.Mid(text4, 21, text4.Length);
                                    if (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult3, LoopForResult3, ref CounterResult3))
                                    {
                                        goto IL_0bd6;
                                    }
                                }
                                goto IL_0dee;
                            IL_0dee: // <========== 4
                                num = 181;
                                if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, Liste2.Items.Count - 1, 1, ref LoopForResult4, ref CounterResult2))
                                {
                                    while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult4, ref CounterResult2))
                                    {
                                        array[unchecked(b4) + 1] = array[unchecked(b4) + 1] + Liste2.Items[CounterResult2.AsInt()].AsString();
                                    }
                                }
                                text5 = Strings.Right("   " + Strings.LTrim(unchecked(b3) + 1.AsString()), 3);
                                array[unchecked(b4) + 1] = array[unchecked(b4) + 1] + text5;
                                while (Liste2.Items.Count != 0)
                                {
                                    Liste2.Items.RemoveAt(0);
                                }
                                b4 = (byte)(unchecked(b4) + 1);
                                num5++;
                                if (num5 <= 30000)
                                {
                                    goto IL_037c;
                                }
                                RichTextBox1.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                                RichTextBox1.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text3.RTF", RichTextBoxStreamType.RichText);
                                goto end_IL_0000_3;
                        }
                        num = 215;
                        Bezeichnung7.Visible = false;
                        break;
                    }
                end_IL_0000:
                    ;
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 5063;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_3: // <========== 5
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }

        private void Nachlist_Load(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                    short Listart;
                    EEventArt Art;
                    bool neb;
                    string[] array;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            _Modul1.Instance.Dateienopen();
                            goto IL_0008;
                        case 2101:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_066f;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 55)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_066f;
                                }
                                if (number == 75)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_066f;
                                }
                                if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0673;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            array = new string[301];
                            Bezeichnung3.Text = "Nachfahrenliste";
                            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<Enum>("Windowstate");
                            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
                            _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
                            _Modul1.Instance.DAus[102] = "10";
                            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1.Text = "";
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
                            goto IL_02b8;
                        IL_02b8: // <========== 12
                            num = 55;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1.Text = "";
                            Show();
                            RichTextBox1.Width = checked(Width - 40);
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
                            RichTextBox1.Visible = false;
                            int persInArb = checked((int)Math.Round(Interaction.InputBox("Nummer der Person, Leer/Abbruch = Suche nach Namen").AsDouble()));
                            _Modul1.Instance.PersInArb = persInArb;
                            if (_Modul1.Instance.PersInArb == 0)
                            {
                                _Modul1.Instance.Schalt = 0;
                                _Modul1.Instance.Schalt = 3;
                                MyProject.Forms.Namensuch.ShowDialog();
                                if (_Modul1.Instance.Suchper == 0)
                                {
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000_2;
                                }
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Suchper;
                                _Modul1.Instance.Schalt = 0;
                            }
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            _Modul1.Instance.PerSatzLes(_Modul1.Instance.PersInArb);
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            Frame1.Visible = true;
                            string text = (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.TrimEnd() + " " + _Modul1.Instance.Kont[0].TrimEnd().ToUpper() + " " + _Modul1.Instance.Kont[2].TrimEnd()).Trim();
                            Label1[0].Text = text;
                            Label1[3].Text = "Nachfahrenliste für " + text;
                            Listart = 0;
                            Art = default;
                            neb = false;
                            _Modul1.Instance.Datles3(Listart, default, Art, ref neb);
                            Label1[5].Text = _Modul1.Instance.IText[3] + " " + _Modul1.Instance.Kont[11];
                            Label1[1].Text = _Modul1.Instance.IText[4] + " " + _Modul1.Instance.Kont[12];
                            Label1[2].Text = _Modul1.Instance.IText[5] + " " + _Modul1.Instance.Kont[13];
                            Label1[4].Text = _Modul1.Instance.IText[6] + " " + _Modul1.Instance.Kont[14];
                            goto end_IL_0000_2;
                        IL_066f: // <========== 3
                            num4 = num2 + 1;
                            goto IL_0673;
                        IL_0673:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 23:
                                case 27:
                                case 30:
                                case 33:
                                case 36:
                                case 39:
                                case 42:
                                case 45:
                                case 48:
                                case 51:
                                case 54:
                                case 55:
                                    goto IL_02b8;
                                case 73:
                                case 90:
                                case 92:
                                case 96:
                                case 97:
                                case 100:
                                case 101:
                                case 107:
                                case 108:
                                case 109:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 2101;
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
        public void LDat()
        {
            short Listart = 0;
            EEventArt Art = default;
            bool neb = false;
            _Modul1.Instance.Datles3(Listart, default, Art, ref neb);
            string text = "";
            if (_Modul1.Instance.Kont[11].Trim() !=  "")
            {
                text = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11].Trim();
            }
            else if (_Modul1.Instance.Kont[12].Trim() !=  "")
            {
                text = text + _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12].Trim();
            }
            if (_Modul1.Instance.Kont[13].Trim() !=  "")
            {
                text = text + " " + _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13].Trim();
            }
            else if (_Modul1.Instance.Kont[14].Trim() !=  "")
            {
                text = text + " " + (_Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14]).Trim();
            }
            if (text.Trim() !=  "")
            {
                RichTextBox1.SelectedText = text.Trim();
            }
            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            RichTextBox1.SelectedText = "\n";
            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
        }

        public void Persuch(int famInArb, ELinkKennz eLKennz)
        {
            if (!DataModul.Link.GetFamPerson(famInArb, eLKennz, out var _PersInArb)
)
            {
                RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                RichTextBox1.SelectedText = Gene + " mit unbekanntem Partner\n";
                return;
            }
            _Modul1.Instance.PerSatzLes(_PersInArb);
            _Modul1.Instance.Person_ReadNames(_PersInArb, _Modul1.Instance.Person);
            DA = "    ";
            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            RichTextBox1.SelectedText = Gene + " " + (_Modul1.Instance.Person.Prae.TrimEnd() + " " + _Modul1.Instance.Person.Givennames.TrimEnd()).Trim() + " ";
            if (_Modul1.Instance.Kont[1].Trim() !=  "")
            {
                RichTextBox1.SelectedText = _Modul1.Instance.Kont[1] + " ";
            }
            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
            RichTextBox1.SelectedText = _Modul1.Instance.Kont[0].TrimEnd() + " ";
            RichTextBox1.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            if (_Modul1.Instance.Kont[4].Trim() !=  "")
            {
                RichTextBox1.SelectedText = "(" + _Modul1.Instance.Kont[4] + ") ";
            }
            LDat();
        }

        public void Kisuch(int famInArb,Func<object,int> add)
        {
            foreach (var cLink in DataModul.Link.ReadAllFams(famInArb, ELinkKennz.lkChild))
            {
                _Modul1.Instance.PersInArb = cLink.iPersNr;
                _Modul1.Instance.Datschalt = 1;
                short Listart = 0;
                EEventArt Art = default;
                bool neb = false;
                _Modul1.Instance.Datles3(Listart, default, Art, ref neb);
                _Modul1.Instance.Datschalt = 0;
                DA = "        ";
                if (_Modul1.Instance.Kont[1].Trim() !=  "")
                {
                    DA = _Modul1.Instance.Kont[1];
                }
                else if (_Modul1.Instance.Kont[2] !=  "")
                {
                    DA = _Modul1.Instance.Kont[2];
                }
                if (DA.Trim() ==  "")
                {
                    DA = "        ";
                }
                add(DA + ">" + Strings.Right("          " + cLink.iPersNr.AsString(), 10));
            }
        }
    }
}
