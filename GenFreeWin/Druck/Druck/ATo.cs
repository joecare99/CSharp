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
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Druck
{
    [DesignerGenerated]
    internal class ATo : Form
    {
        private IContainer components;

        public ToolTip ToolTip1;

        [AccessedThroughProperty("_List1_9")]
        private ListBox __List1_9;

        [AccessedThroughProperty("_List1_8")]
        private ListBox __List1_8;

        [AccessedThroughProperty("_List1_7")]
        private ListBox __List1_7;

        [AccessedThroughProperty("_List1_6")]
        private ListBox __List1_6;

        [AccessedThroughProperty("_List1_5")]
        private ListBox __List1_5;

        [AccessedThroughProperty("_List1_4")]
        private ListBox __List1_4;

        [AccessedThroughProperty("_List1_3")]
        private ListBox __List1_3;

        [AccessedThroughProperty("_List1_2")]
        private ListBox __List1_2;

        [AccessedThroughProperty("_List1_1")]
        private ListBox __List1_1;

        [AccessedThroughProperty("_List1_0")]
        private ListBox __List1_0;

        [AccessedThroughProperty("Frame2")]
        private GroupBox _Frame2;

        [AccessedThroughProperty("_Command1_1")]
        private Button __Command1_1;

        [AccessedThroughProperty("_Command1_0")]
        private Button __Command1_0;

        [AccessedThroughProperty("_Command_1")]
        private Button __Command_1;

        [AccessedThroughProperty("_Command_0")]
        private Button __Command_0;

        [AccessedThroughProperty("_Label1_5")]
        private Label __Label1_5;

        [AccessedThroughProperty("_Label1_4")]
        private Label __Label1_4;

        [AccessedThroughProperty("_Label1_3")]
        private Label __Label1_3;

        [AccessedThroughProperty("_Label1_2")]
        private Label __Label1_2;

        [AccessedThroughProperty("_Label1_1")]
        private Label __Label1_1;

        [AccessedThroughProperty("_Label1_0")]
        private Label __Label1_0;

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

        [AccessedThroughProperty("List1")]
        private ListBoxArray _List1;

        private string sterdat2;

        private string Gebdat2;

        private string VNam;

        private string Gebdat;

        private string Gebdat1;

        private string Sterdat;

        private string Sterdat1;

        private string Pername;

        private string Msg;

        private string Famdat2;

        private string FamDat1;

        private string Famdat;

        private string Fz;

        private string Text1;

        private string SchText;

        private string Nr;

        private short A;

        private short B;

        private short I;
        private int M_Start;

        public virtual ListBox _List1_9
        {
            get
            {
                return __List1_9;
            }

            set
            {
                __List1_9 = value;
            }
        }

        public virtual ListBox _List1_8
        {
            get
            {
                return __List1_8;
            }

            set
            {
                __List1_8 = value;
            }
        }

        public virtual ListBox _List1_7
        {
            get
            {
                return __List1_7;
            }

            set
            {
                __List1_7 = value;
            }
        }

        public virtual ListBox _List1_6
        {
            get
            {
                return __List1_6;
            }

            set
            {
                __List1_6 = value;
            }
        }

        public virtual ListBox _List1_5
        {
            get
            {
                return __List1_5;
            }

            set
            {
                __List1_5 = value;
            }
        }

        public virtual ListBox _List1_4
        {
            get
            {
                return __List1_4;
            }

            set
            {
                __List1_4 = value;
            }
        }

        public virtual ListBox _List1_3
        {
            get
            {
                return __List1_3;
            }

            set
            {
                __List1_3 = value;
            }
        }

        public virtual ListBox _List1_2
        {
            get
            {
                return __List1_2;
            }

            set
            {
                __List1_2 = value;
            }
        }

        public virtual ListBox _List1_1
        {
            get
            {
                return __List1_1;
            }

            set
            {
                __List1_1 = value;
            }
        }

        public virtual ListBox _List1_0
        {
            get
            {
                return __List1_0;
            }

            set
            {
                __List1_0 = value;
            }
        }

        public virtual GroupBox Frame2
        {
            get
            {
                return _Frame2;
            }

            set
            {
                _Frame2 = value;
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

        public virtual ListBoxArray List1
        {
            get
            {
                return _List1;
            }

            set
            {
                _List1 = value;
            }
        }

        [DebuggerNonUserCode]
        public ATo()
        {
            base.Load += ATo_Load;
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
            this.Frame2 = new System.Windows.Forms.GroupBox();
            this._List1_9 = new System.Windows.Forms.ListBox();
            this._List1_8 = new System.Windows.Forms.ListBox();
            this._List1_7 = new System.Windows.Forms.ListBox();
            this._List1_6 = new System.Windows.Forms.ListBox();
            this._List1_5 = new System.Windows.Forms.ListBox();
            this._List1_4 = new System.Windows.Forms.ListBox();
            this._List1_3 = new System.Windows.Forms.ListBox();
            this._List1_2 = new System.Windows.Forms.ListBox();
            this._List1_1 = new System.Windows.Forms.ListBox();
            this._List1_0 = new System.Windows.Forms.ListBox();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this._Command_1 = new System.Windows.Forms.Button();
            this._Command_0 = new System.Windows.Forms.Button();
            this._Label1_5 = new System.Windows.Forms.Label();
            this._Label1_4 = new System.Windows.Forms.Label();
            this._Label1_3 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Command_Renamed = new ControlArray<Button>();
            this.Command1 = new ControlArray<Button>();
            this.Label1 = new ControlArray<Label>();
            this.List1 = new Microsoft.VisualBasic.Compatibility.VB6.ListBoxArray(this.components);
            this.Frame2.SuspendLayout();
            this.Frame1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.List1).BeginInit();
            this.SuspendLayout();
            this.Frame2.BackColor = System.Drawing.SystemColors.Control;
            this.Frame2.Controls.Add(this._List1_9);
            this.Frame2.Controls.Add(this._List1_8);
            this.Frame2.Controls.Add(this._List1_7);
            this.Frame2.Controls.Add(this._List1_6);
            this.Frame2.Controls.Add(this._List1_5);
            this.Frame2.Controls.Add(this._List1_4);
            this.Frame2.Controls.Add(this._List1_3);
            this.Frame2.Controls.Add(this._List1_2);
            this.Frame2.Controls.Add(this._List1_1);
            this.Frame2.Controls.Add(this._List1_0);
            this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
            System.Drawing.Point point2 = this.Frame2.Location = new System.Drawing.Point(811, 520);
            System.Windows.Forms.Padding padding2 = this.Frame2.Margin = new System.Windows.Forms.Padding(4);
            this.Frame2.Name = "Frame2";
            padding2 = this.Frame2.Padding = new System.Windows.Forms.Padding(4);
            this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            System.Drawing.Size size2 = this.Frame2.Size = new System.Drawing.Size(249, 116);
            this.Frame2.TabIndex = 12;
            this.Frame2.TabStop = false;
            this.Frame2.Text = "Frame2";
            this.Frame2.Visible = false;
            this._List1_9.BackColor = System.Drawing.SystemColors.Window;
            this._List1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_9.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_9, 9);
            this._List1_9.ItemHeight = 17;
            point2 = this._List1_9.Location = new System.Drawing.Point(901, 13);
            padding2 = this._List1_9.Margin = new System.Windows.Forms.Padding(4);
            this._List1_9.Name = "_List1_9";
            this._List1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_9.Size = new System.Drawing.Size(91, 633);
            this._List1_9.TabIndex = 23;
            this._List1_8.BackColor = System.Drawing.SystemColors.Window;
            this._List1_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_8.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_8, 8);
            this._List1_8.ItemHeight = 17;
            point2 = this._List1_8.Location = new System.Drawing.Point(805, 13);
            padding2 = this._List1_8.Margin = new System.Windows.Forms.Padding(4);
            this._List1_8.Name = "_List1_8";
            this._List1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_8.Size = new System.Drawing.Size(91, 633);
            this._List1_8.TabIndex = 22;
            this._List1_7.BackColor = System.Drawing.SystemColors.Window;
            this._List1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_7.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_7, 7);
            this._List1_7.ItemHeight = 17;
            point2 = this._List1_7.Location = new System.Drawing.Point(709, 13);
            padding2 = this._List1_7.Margin = new System.Windows.Forms.Padding(4);
            this._List1_7.Name = "_List1_7";
            this._List1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_7.Size = new System.Drawing.Size(91, 633);
            this._List1_7.TabIndex = 21;
            this._List1_6.BackColor = System.Drawing.SystemColors.Window;
            this._List1_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_6.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_6, 6);
            this._List1_6.ItemHeight = 17;
            point2 = this._List1_6.Location = new System.Drawing.Point(611, 13);
            padding2 = this._List1_6.Margin = new System.Windows.Forms.Padding(4);
            this._List1_6.Name = "_List1_6";
            this._List1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_6.Size = new System.Drawing.Size(91, 633);
            this._List1_6.TabIndex = 20;
            this._List1_5.BackColor = System.Drawing.SystemColors.Window;
            this._List1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_5.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_5, 5);
            this._List1_5.ItemHeight = 17;
            point2 = this._List1_5.Location = new System.Drawing.Point(512, 13);
            padding2 = this._List1_5.Margin = new System.Windows.Forms.Padding(4);
            this._List1_5.Name = "_List1_5";
            this._List1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_5.Size = new System.Drawing.Size(91, 633);
            this._List1_5.TabIndex = 19;
            this._List1_4.BackColor = System.Drawing.SystemColors.Window;
            this._List1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_4, 4);
            this._List1_4.ItemHeight = 17;
            point2 = this._List1_4.Location = new System.Drawing.Point(411, 13);
            padding2 = this._List1_4.Margin = new System.Windows.Forms.Padding(4);
            this._List1_4.Name = "_List1_4";
            this._List1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_4.Size = new System.Drawing.Size(91, 633);
            this._List1_4.TabIndex = 18;
            this._List1_3.BackColor = System.Drawing.SystemColors.Window;
            this._List1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_3, 3);
            this._List1_3.ItemHeight = 17;
            point2 = this._List1_3.Location = new System.Drawing.Point(312, 13);
            padding2 = this._List1_3.Margin = new System.Windows.Forms.Padding(4);
            this._List1_3.Name = "_List1_3";
            this._List1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_3.Size = new System.Drawing.Size(91, 633);
            this._List1_3.TabIndex = 16;
            this._List1_2.BackColor = System.Drawing.SystemColors.Window;
            this._List1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_2, 2);
            this._List1_2.ItemHeight = 17;
            point2 = this._List1_2.Location = new System.Drawing.Point(213, 13);
            padding2 = this._List1_2.Margin = new System.Windows.Forms.Padding(4);
            this._List1_2.Name = "_List1_2";
            this._List1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_2.Size = new System.Drawing.Size(91, 633);
            this._List1_2.TabIndex = 15;
            this._List1_1.BackColor = System.Drawing.SystemColors.Window;
            this._List1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_1, 1);
            this._List1_1.ItemHeight = 17;
            point2 = this._List1_1.Location = new System.Drawing.Point(120, 13);
            padding2 = this._List1_1.Margin = new System.Windows.Forms.Padding(4);
            this._List1_1.Name = "_List1_1";
            this._List1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_1.Size = new System.Drawing.Size(91, 633);
            this._List1_1.TabIndex = 14;
            this._List1_0.BackColor = System.Drawing.SystemColors.Window;
            this._List1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_0.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.SetIndex(this._List1_0, 0);
            this._List1_0.ItemHeight = 17;
            point2 = this._List1_0.Location = new System.Drawing.Point(27, 13);
            padding2 = this._List1_0.Margin = new System.Windows.Forms.Padding(4);
            this._List1_0.Name = "_List1_0";
            this._List1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._List1_0.Size = new System.Drawing.Size(91, 633);
            this._List1_0.TabIndex = 13;
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_1, 1);
            point2 = this._Command1_1.Location = new System.Drawing.Point(526, 433);
            padding2 = this._Command1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_1.Size = new System.Drawing.Size(156, 54);
            this._Command1_1.TabIndex = 10;
            this._Command1_1.Text = "Druckmenü";
            this._Command1_1.UseVisualStyleBackColor = false;
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_0, 0);
            point2 = this._Command1_0.Location = new System.Drawing.Point(288, 433);
            padding2 = this._Command1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_0.Size = new System.Drawing.Size(156, 54);
            this._Command1_0.TabIndex = 9;
            this._Command1_0.Text = "Drucken";
            this._Command1_0.UseVisualStyleBackColor = false;
            this.Frame1.BackColor = System.Drawing.Color.Red;
            this.Frame1.Controls.Add(this._Command_1);
            this.Frame1.Controls.Add(this._Command_0);
            this.Frame1.Controls.Add(this._Label1_5);
            this.Frame1.Controls.Add(this._Label1_4);
            this.Frame1.Controls.Add(this._Label1_3);
            this.Frame1.Controls.Add(this._Label1_2);
            this.Frame1.Controls.Add(this._Label1_1);
            this.Frame1.Controls.Add(this._Label1_0);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Frame1.Location = new System.Drawing.Point(299, 123);
            padding2 = this.Frame1.Margin = new System.Windows.Forms.Padding(4);
            this.Frame1.Name = "Frame1";
            padding2 = this.Frame1.Padding = new System.Windows.Forms.Padding(4);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Frame1.Size = new System.Drawing.Size(543, 230);
            this.Frame1.TabIndex = 0;
            this.Frame1.TabStop = false;
            this._Command_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command_Renamed.SetIndex(this._Command_1, 1);
            point2 = this._Command_1.Location = new System.Drawing.Point(327, 192);
            padding2 = this._Command_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command_1.Name = "_Command_1";
            this._Command_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command_1.Size = new System.Drawing.Size(141, 26);
            this._Command_1.TabIndex = 2;
            this._Command_1.Text = "OK";
            this._Command_1.UseVisualStyleBackColor = false;
            this._Command_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command_Renamed.SetIndex(this._Command_0, 0);
            point2 = this._Command_0.Location = new System.Drawing.Point(47, 192);
            padding2 = this._Command_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command_0.Name = "_Command_0";
            this._Command_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command_0.Size = new System.Drawing.Size(141, 26);
            this._Command_0.TabIndex = 1;
            this._Command_0.Text = "Abbrechen";
            this._Command_0.UseVisualStyleBackColor = false;
            this._Label1_5.BackColor = System.Drawing.Color.White;
            this._Label1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_5, 5);
            point2 = this._Label1_5.Location = new System.Drawing.Point(9, 73);
            padding2 = this._Label1_5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_5.Name = "_Label1_5";
            this._Label1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_5.Size = new System.Drawing.Size(524, 20);
            this._Label1_5.TabIndex = 8;
            this._Label1_4.BackColor = System.Drawing.Color.White;
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_4, 4);
            point2 = this._Label1_4.Location = new System.Drawing.Point(9, 156);
            padding2 = this._Label1_4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_4.Size = new System.Drawing.Size(524, 20);
            this._Label1_4.TabIndex = 7;
            this._Label1_3.BackColor = System.Drawing.Color.White;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_3, 3);
            point2 = this._Label1_3.Location = new System.Drawing.Point(9, 18);
            padding2 = this._Label1_3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_3.Size = new System.Drawing.Size(524, 20);
            this._Label1_3.TabIndex = 6;
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_2.BackColor = System.Drawing.Color.White;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_2, 2);
            point2 = this._Label1_2.Location = new System.Drawing.Point(9, 128);
            padding2 = this._Label1_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_2.Size = new System.Drawing.Size(524, 20);
            this._Label1_2.TabIndex = 5;
            this._Label1_1.BackColor = System.Drawing.Color.White;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_1, 1);
            point2 = this._Label1_1.Location = new System.Drawing.Point(9, 101);
            padding2 = this._Label1_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_1.Size = new System.Drawing.Size(524, 20);
            this._Label1_1.TabIndex = 4;
            this._Label1_0.BackColor = System.Drawing.Color.White;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.SetIndex(this._Label1_0, 0);
            point2 = this._Label1_0.Location = new System.Drawing.Point(9, 46);
            padding2 = this._Label1_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_0.Size = new System.Drawing.Size(524, 20);
            this._Label1_0.TabIndex = 3;
            this.Label2.AutoSize = true;
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Label2.Location = new System.Drawing.Point(296, 123);
            padding2 = this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Label2.Size = new System.Drawing.Size(0, 17);
            this.Label2.TabIndex = 11;
            this.Label2.Visible = false;
            System.Drawing.SizeF sizeF2 = this.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            size2 = this.ClientSize = new System.Drawing.Size(1016, 723);
            this.Controls.Add(this.Frame2);
            this.Controls.Add(this._Command1_1);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Label2);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            padding2 = this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ATo";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Schmuckahnentafeln";
            this.Frame2.ResumeLayout(false);
            this.Frame1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.List1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            short i = default;
            int lErl = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    checked
                    {
                        int num4;
                        string baseDirectory;
                        string tempPath;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                index = Command_Renamed.GetIndex((Button)eventSender);
                                goto IL_0015;
                            case 13197:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 3:
                                            break;
                                        case 2:
                                        case 4:
                                            goto IL_2cfe;
                                        case 1:
                                            goto IL_2d7f;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    if (Information.Err().Number == 75)
                                    {
                                        Interaction.MsgBox("Der Auftrag kann nicht ausgeführt werden, da noch eine nichtgespeicherte Ahnentafel (MS-Word) offen ist.");
                                        Close();
                                        goto end_IL_0000_2;
                                    }
                                    goto IL_2cfe;
                                }
                            end_IL_0000:
                                break;
                            IL_0015:
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                switch (index)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        goto IL_0058;
                                    default:
                                        goto end_IL_0000_2;
                                }
                                Close();
                                MyProject.Forms.Druck.Show();
                                goto end_IL_0000_2;
                            IL_0058:
                                num = 12;
                                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                tempPath = Path.GetTempPath();
                                FileSystem.FileClose(99);
                                FileSystem.MkDir("C:\\Temp");
                                ProjectData.ClearProjectError();
                                num3 = 3;
                                FileSystem.FileOpen(99, "C:\\Temp\\At99.txt", OpenMode.Output);
                                ProjectData.ClearProjectError();
                                num3 = 4;
                                SchText = "Nur0\tName0\tVn0\tgz0\tgd0\tgo0\tsz0\tsd0\tso0\t";
                                SchText += "Hz0\tHD0\tHD01\tHD02\t";
                                SchText += "Nur1\tName1\tVn1\tgz1\tgd1\tgo1\tsz1\tsd1\tso1\t";
                                SchText += "Nur2\tname2\tVn2\tgz2\tgd2\tgo2\tsz2\tsd2\tso2\t";
                                SchText += "Hz1\tHD1\tHD111\tHD112\t";
                                SchText += "Nur3\tname3\tVn3\tgz3\tgd3\tgo3\tsz3\tsd3\tso3\t";
                                SchText += "Nur4\tname4\tVn4\tgz4\tgd4\tgo4\tsz4\tsd4\tso4\t";
                                SchText += "Hz2\tHD2\tHD121\tHD122\t";
                                SchText += "Nur5\tname5\tVn5\tgz5\tgd5\tgo5\tsz5\tsd5\tso5\t";
                                SchText += "Nur6\tname6\tVn6\tgz6\tgd6\tgo6\tsz6\tsd6\tso6\t";
                                SchText += "Hz3\tHD3\tHD131\tHD132\t";
                                SchText += "Nur7\tname7\tVn7\tgz7\tgd7\tgo7\tsz7\tsd7\tso7\t";
                                SchText += "Nur8\tname8\tVn8\tgz8\tgd8\tgo8\tsz8\tsd8\tso8\t";
                                SchText += "Hz4\tHD4\tHD141\tHD142\t";
                                SchText += "Nur9\tname9\tVn9\tgz9\tgd9\tgo9\tsz9\tsd9\tso9\t";
                                SchText += "Nur10\tname10\tVn10\tgz10\tgd10\tgo10\tsz10\tsd10\tso10\t";
                                SchText += "Hz5\tHD5\tHD151\tHD152\t";
                                SchText += "Nur11\tname11\tVn11\tgz11\tgd11\tgo11\tsz11\tsd11\tso11\t";
                                SchText += "Nur12\tname12\tVn12\tgz12\tgd12\tgo12\tsz12\tsd12\tso12\t";
                                SchText += "Hz6\tHD6\tHD161\tHD162\t";
                                SchText += "Nur13\tname13\tVn13\tgz13\tgd13\tgo13\tsz13\tsd13\tso13\t";
                                SchText += "Nur14\tname14\tVn14\tgz14\tgd14\tgo14\tsz14\tsd14\tso14\t";
                                SchText += "Hz7\tHD7\tHD171\tHD172\t";
                                SchText += "Nur15\tname15\tVn15\tgz15\tgd15\tgo15\tsz15\tsd15\tso15\t";
                                if (!(_Modul1.Instance.Aschalt < 5f))
                                {
                                    SchText += "Nur16\tName16\tVn16\tgz16\tgd16\tgo16\tsz16\tsd16\tso16\t";
                                    SchText += "Hz8\tHD8\tHD181\tHD182\t";
                                    SchText += "Nur17\tName17\tVn17\tgz17\tgd17\tgo17\tsz17\tsd17\tso17\t";
                                    SchText += "Nur18\tname18\tVn18\tgz18\tgd18\tgo18\tsz18\tsd18\tso18\t";
                                    SchText += "Hz9\tHD9\tHD191\tHD192\t";
                                    SchText += "Nur19\tname19\tVn19\tgz19\tgd19\tgo19\tsz19\tsd19\tso19\t";
                                    SchText += "Nur20\tname20\tVn20\tgz20\tgd20\tgo20\tsz20\tsd20\tso20\t";
                                    SchText += "Hz10\tHD10\tHD1001\tHD1002\t";
                                    SchText += "Nur21\tname21\tVn21\tgz21\tgd21\tgo21\tsz21\tsd21\tso21\t";
                                    SchText += "Nur22\tname22\tVn22\tgz22\tgd22\tgo22\tsz22\tsd22\tso22\t";
                                    SchText += "Hz11\tHD11\tHD1011\tHD1012\t";
                                    SchText += "Nur23\tname23\tVn23\tgz23\tgd23\tgo23\tsz23\tsd23\tso23\t";
                                    SchText += "Nur24\tname24\tVn24\tgz24\tgd24\tgo24\tsz24\tsd24\tso24\t";
                                    SchText += "Hz12\tHD12\tHD1021\tHD1022\t";
                                    SchText += "Nur25\tname25\tVn25\tgz25\tgd25\tgo25\tsz25\tsd25\tso25\t";
                                    SchText += "Nur26\tname26\tVn26\tgz26\tgd26\tgo26\tsz26\tsd26\tso26\t";
                                    SchText += "Hz13\tHD13\tHD1031\tHD1032\t";
                                    SchText += "Nur27\tname27\tVn27\tgz27\tgd27\tgo27\tsz27\tsd27\tso27\t";
                                    SchText += "Nur28\tname28\tVn28\tgz28\tgd28\tgo28\tsz28\tsd28\tso28\t";
                                    SchText += "Hz14\tHD14\tHD1041\tHD1042\t";
                                    SchText += "Nur29\tname29\tVn29\tgz29\tgd29\tgo29\tsz29\tsd29\tso29\t";
                                    SchText += "Nur30\tname30\tVn30\tgz30\tgd30\tgo30\tsz30\tsd30\tso30\t";
                                    SchText += "Hz15\tHD15\tHD1051\tHD1052\t";
                                    SchText += "Nur31\tname31\tVn31\tgz31\tgd31\tgo31\tsz31\tsd31\tso31\t";
                                    if (!(_Modul1.Instance.Aschalt < 6f))
                                    {
                                        SchText += "Nur32\tName32\tVn32\tgz32\tgd32\tgo32\tsz32\tsd32\tso32\t";
                                        SchText += "Hz16\tHD16\tHD1611\tHD1621\t";
                                        SchText += "Nur33\tName33\tVn33\tgz33\tgd33\tgo33\tsz33\tsd33\tso33\t";
                                        SchText += "Nur34\tname34\tVn34\tgz34\tgd34\tgo34\tsz34\tsd34\tso34\t";
                                        SchText += "Hz17\tHD17\tHD1711\tHD1721\t";
                                        SchText += "Nur35\tname35\tVn35\tgz35\tgd35\tgo35\tsz35\tsd35\tso35\t";
                                        SchText += "Nur36\tname36\tVn36\tgz36\tgd36\tgo36\tsz36\tsd36\tso36\t";
                                        SchText += "Hz18\tHD18\tHD1811\tHD1821\t";
                                        SchText += "Nur37\tname37\tVn37\tgz37\tgd37\tgo37\tsz37\tsd37\tso37\t";
                                        SchText += "Nur38\tname38\tVn38\tgz38\tgd38\tgo38\tsz38\tsd38\tso38\t";
                                        SchText += "Hz19\tHD19\tHD1911\tHD1921\t";
                                        SchText += "Nur39\tname39\tVn39\tgz39\tgd39\tgo39\tsz39\tsd39\tso39\t";
                                        SchText += "Nur40\tname40\tVn40\tgz40\tgd40\tgo40\tsz40\tsd40\tso40\t";
                                        SchText += "Hz20\tHD20\tHD2011\tHD2021\t";
                                        SchText += "Nur41\tname41\tVn41\tgz41\tgd41\tgo41\tsz41\tsd41\tso41\t";
                                        SchText += "Nur42\tname42\tVn42\tgz42\tgd42\tgo42\tsz42\tsd42\tso42\t";
                                        SchText += "Hz21\tHD21\tHD2111\tHD2121\t";
                                        SchText += "Nur43\tname43\tVn43\tgz43\tgd43\tgo43\tsz43\tsd43\tso43\t";
                                        SchText += "Nur44\tname44\tVn44\tgz44\tgd44\tgo44\tsz44\tsd44\tso44\t";
                                        SchText += "Hz22\tHD22\tHD2211\tHD2221\t";
                                        SchText += "Nur45\tname45\tVn45\tgz45\tgd45\tgo45\tsz45\tsd45\tso45\t";
                                        SchText += "Nur46\tname46\tVn46\tgz46\tgd46\tgo46\tsz46\tsd46\tso46\t";
                                        SchText += "Hz23\tHD23\tHD2311\tHD2321\t";
                                        SchText += "Nur47\tname47\tVn47\tgz47\tgd47\tgo47\tsz47\tsd47\tso47\t";
                                        SchText += "Nur48\tName48\tVn48\tgz48\tgd48\tgo48\tsz48\tsd48\tso48\t";
                                        SchText += "Hz24\tHD24\tHD2411\tHD2421\t";
                                        SchText += "Nur49\tName49\tVn49\tgz49\tgd49\tgo49\tsz49\tsd49\tso49\t";
                                        SchText += "Nur50\tname50\tVn50\tgz50\tgd50\tgo50\tsz50\tsd50\tso50\t";
                                        SchText += "Hz25\tHD25\tHD2511\tHD2521\t";
                                        SchText += "Nur51\tname51\tVn51\tgz51\tgd51\tgo51\tsz51\tsd51\tso51\t";
                                        SchText += "Nur52\tname52\tVn52\tgz52\tgd52\tgo52\tsz52\tsd52\tso52\t";
                                        SchText += "Hz26\tHD26\tHD2611\tHD2621\t";
                                        SchText += "Nur53\tname53\tVn53\tgz53\tgd53\tgo53\tsz53\tsd53\tso53\t";
                                        SchText += "Nur54\tname54\tVn54\tgz54\tgd54\tgo54\tsz54\tsd54\tso54\t";
                                        SchText += "Hz27\tHD27\tHD2711\tHD2721\t";
                                        SchText += "Nur55\tname55\tVn55\tgz55\tgd55\tgo55\tsz55\tsd55\tso55\t";
                                        SchText += "Nur56\tname56\tVn56\tgz56\tgd56\tgo56\tsz56\tsd56\tso56\t";
                                        SchText += "Hz28\tHD28\tHD2811\tHD2821\t";
                                        SchText += "Nur57\tname57\tVn57\tgz57\tgd57\tgo57\tsz57\tsd57\tso57\t";
                                        SchText += "Nur58\tname58\tVn58\tgz58\tgd58\tgo58\tsz58\tsd58\tso58\t";
                                        SchText += "Hz29\tHD29\tHD2911\tHD2921\t";
                                        SchText += "Nur59\tname59\tVn59\tgz59\tgd59\tgo59\tsz59\tsd59\tso59\t";
                                        SchText += "Nur60\tname60\tVn60\tgz60\tgd60\tgo60\tsz60\tsd60\tso60\t";
                                        SchText += "Hz30\tHD30\tHD3011\tHD3021\t";
                                        SchText += "Nur61\tname61\tVn61\tgz61\tgd61\tgo61\tsz61\tsd61\tso61\t";
                                        SchText += "Nur62\tname62\tVn62\tgz62\tgd62\tgo62\tsz62\tsd62\tso62\t";
                                        SchText += "Hz31\tHD31\tHD3111\tHD3121\t";
                                        SchText += "Nur63\tname63\tVn63\tgz63\tgd63\tgo63\tsz63\tsd63\tso63\t";
                                        if (!(_Modul1.Instance.Aschalt < 7f))
                                        {
                                            SchText += "Nur64\tName64\tVn64\tgz64\tgd64\tgo64\tsz64\tsd64\tso64\t";
                                            SchText += "Hz32\tHD32\tHD3211\tHD3221\t";
                                            SchText += "Nur65\tName65\tVn65\tgz65\tgd65\tgo65\tsz65\tsd65\tso65\t";
                                            SchText += "Nur66\tname66\tVn66\tgz66\tgd66\tgo66\tsz66\tsd66\tso66\t";
                                            SchText += "Hz33\tHD33\tHD3311\tHD3321\t";
                                            SchText += "Nur67\tname67\tVn67\tgz67\tgd67\tgo67\tsz67\tsd67\tso67\t";
                                            SchText += "Nur68\tname68\tVn68\tgz68\tgd68\tgo68\tsz68\tsd68\tso68\t";
                                            SchText += "Hz34\tHD34\tHD3411\tHD3421\t";
                                            SchText += "Nur69\tname69\tVn69\tgz69\tgd69\tgo69\tsz69\tsd69\tso69\t";
                                            SchText += "Nur70\tname70\tVn70\tgz70\tgd70\tgo70\tsz70\tsd70\tso70\t";
                                            SchText += "Hz35\tHD35\tHD3511\tHD3521\t";
                                            SchText += "Nur71\tname71\tVn71\tgz71\tgd71\tgo71\tsz71\tsd71\tso71\t";
                                            SchText += "Nur72\tname72\tVn72\tgz72\tgd72\tgo72\tsz72\tsd72\tso72\t";
                                            SchText += "Hz36\tHD36\tHD3611\tHD3621\t";
                                            SchText += "Nur73\tname73\tVn73\tgz73\tgd73\tgo73\tsz73\tsd73\tso73\t";
                                            SchText += "Nur74\tname74\tVn74\tgz74\tgd74\tgo74\tsz74\tsd74\tso74\t";
                                            SchText += "Hz37\tHD37\tHD3711\tHD3721\t";
                                            SchText += "Nur75\tname75\tVn75\tgz75\tgd75\tgo75\tsz75\tsd75\tso75\t";
                                            SchText += "Nur76\tname76\tVn76\tgz76\tgd76\tgo76\tsz76\tsd76\tso76\t";
                                            SchText += "Hz38\tHD38\tHD3811\tHD3821\t";
                                            SchText += "Nur77\tname77\tVn77\tgz77\tgd77\tgo77\tsz77\tsd77\tso77\t";
                                            SchText += "Nur78\tname78\tVn78\tgz78\tgd78\tgo78\tsz78\tsd78\tso78\t";
                                            SchText += "Hz39\tHD39\tHD3911\tHD3921\t";
                                            SchText += "Nur79\tname79\tVn79\tgz79\tgd79\tgo79\tsz79\tsd79\tso79\t";
                                            SchText += "Nur80\tName80\tVn80\tgz80\tgd80\tgo80\tsz80\tsd80\tso80\t";
                                            SchText += "Hz40\tHD40\tHD4011\tHD4021\t";
                                            SchText += "Nur81\tName81\tVn81\tgz81\tgd81\tgo81\tsz81\tsd81\tso81\t";
                                            SchText += "Nur82\tname82\tVn82\tgz82\tgd82\tgo82\tsz82\tsd82\tso82\t";
                                            SchText += "Hz41\tHD41\tHD4111\tHD4121\t";
                                            SchText += "Nur83\tname83\tVn83\tgz83\tgd83\tgo83\tsz83\tsd83\tso83\t";
                                            SchText += "Nur84\tname84\tVn84\tgz84\tgd84\tgo84\tsz84\tsd84\tso84\t";
                                            SchText += "Hz42\tHD42\tHD4211\tHD4221\t";
                                            SchText += "Nur85\tname85\tVn85\tgz85\tgd85\tgo85\tsz85\tsd85\tso85\t";
                                            SchText += "Nur86\tname86\tVn86\tgz86\tgd86\tgo86\tsz86\tsd86\tso86\t";
                                            SchText += "Hz43\tHD43\tHD4311\tHD4321\t";
                                            SchText += "Nur87\tname87\tVn87\tgz87\tgd87\tgo87\tsz87\tsd87\tso87\t";
                                            SchText += "Nur88\tname88\tVn88\tgz88\tgd88\tgo88\tsz88\tsd88\tso88\t";
                                            SchText += "Hz44\tHD44\tHD4411\tHD4421\t";
                                            SchText += "Nur89\tname89\tVn89\tgz89\tgd89\tgo89\tsz89\tsd89\tso89\t";
                                            SchText += "Nur90\tname90\tVn90\tgz90\tgd90\tgo90\tsz90\tsd90\tso90\t";
                                            SchText += "Hz45\tHD45\tHD4511\tHD4521\t";
                                            SchText += "Nur91\tname91\tVn91\tgz91\tgd91\tgo91\tsz91\tsd91\tso91\t";
                                            SchText += "Nur92\tname92\tVn92\tgz92\tgd92\tgo92\tsz92\tsd92\tso92\t";
                                            SchText += "Hz46\tHD46\tHD4611\tHD4621\t";
                                            SchText += "Nur93\tname93\tVn93\tgz93\tgd93\tgo93\tsz93\tsd93\tso93\t";
                                            SchText += "Nur94\tname94\tVn94\tgz94\tgd94\tgo94\tsz94\tsd94\tso94\t";
                                            SchText += "Hz47\tHD47\tHD4711\tHD4721\t";
                                            SchText += "Nur95\tname95\tVn95\tgz95\tgd95\tgo95\tsz95\tsd95\tso95\t";
                                            SchText += "Nur96\tName96\tVn96\tgz96\tgd96\tgo96\tsz96\tsd96\tso96\t";
                                            SchText += "Hz48\tHD48\tHD4811\tHD4821\t";
                                            SchText += "Nur97\tName97\tVn97\tgz97\tgd97\tgo97\tsz97\tsd97\tso97\t";
                                            SchText += "Nur98\tname98\tVn98\tgz98\tgd98\tgo98\tsz98\tsd98\tso98\t";
                                            SchText += "Hz49\tHD49\tHD4911\tHD4921\t";
                                            SchText += "Nur99\tname99\tVn99\tgz99\tgd99\tgo99\tsz99\tsd99\tso99\t";
                                            SchText += "Nur100\tname100\tVn100\tgz100\tgd100\tgo100\tsz100\tsd100\tso100\t";
                                            SchText += "Hz50\tHD50\tHD5011\tHD5021\t";
                                            SchText += "Nur101\tname101\tVn101\tgz101\tgd101\tgo101\tsz101\tsd101\tso101\t";
                                            SchText += "Nur102\tname102\tVn102\tgz102\tgd102\tgo102\tsz102\tsd102\tso102\t";
                                            SchText += "Hz51\tHD51\tHD5111\tHD5121\t";
                                            SchText += "Nur103\tname103\tVn103\tgz103\tgd103\tgo103\tsz103\tsd103\tso103\t";
                                            SchText += "Nur104\tname104\tVn104\tgz104\tgd104\tgo104\tsz104\tsd104\tso104\t";
                                            SchText += "Hz52\tHD52\tHD5211\tHD5221\t";
                                            SchText += "Nur105\tname105\tVn105\tgz105\tgd105\tgo105\tsz105\tsd105\tso105\t";
                                            SchText += "Nur106\tname106\tVn106\tgz106\tgd106\tgo106\tsz106\tsd106\tso106\t";
                                            SchText += "Hz53\tHD53\tHD5311\tHD5321\t";
                                            SchText += "Nur107\tname107\tVn107\tgz107\tgd107\tgo107\tsz107\tsd107\tso107\t";
                                            SchText += "Nur108\tname108\tVn108\tgz108\tgd108\tgo108\tsz108\tsd108\tso108\t";
                                            SchText += "Hz54\tHD54\tHD5411\tHD5421\t";
                                            SchText += "Nur109\tname109\tVn109\tgz109\tgd109\tgo109\tsz109\tsd109\tso109\t";
                                            SchText += "Nur110\tname110\tVn110\tgz110\tgd110\tgo110\tsz110\tsd110\tso110\t";
                                            SchText += "Hz55\tHD55\tHD5511\tHD5521\t";
                                            SchText += "Nur111\tname111\tVn111\tgz111\tgd111\tgo111\tsz111\tsd111\tso111\t";
                                            SchText += "Nur112\tName112\tVn112\tgz112\tgd112\tgo112\tsz112\tsd112\tso112\t";
                                            SchText += "Hz56\tHD56\tHD5611\tHD5621\t";
                                            SchText += "Nur113\tName113\tVn113\tgz113\tgd113\tgo113\tsz113\tsd113\tso113\t";
                                            SchText += "Nur114\tname114\tVn114\tgz114\tgd114\tgo114\tsz114\tsd114\tso114\t";
                                            SchText += "Hz57\tHD57\tHD5711\tHD5721\t";
                                            SchText += "Nur115\tname115\tVn115\tgz115\tgd115\tgo115\tsz115\tsd115\tso115\t";
                                            SchText += "Nur116\tname116\tVn116\tgz116\tgd116\tgo116\tsz116\tsd116\tso116\t";
                                            SchText += "Hz58\tHD58\tHD5811\tHD5821\t";
                                            SchText += "Nur117\tname117\tVn117\tgz117\tgd117\tgo117\tsz117\tsd117\tso117\t";
                                            SchText += "Nur118\tname118\tVn118\tgz118\tgd118\tgo118\tsz118\tsd118\tso118\t";
                                            SchText += "Hz59\tHD59\tHD5911\tHD5921\t";
                                            SchText += "Nur119\tname119\tVn119\tgz119\tgd119\tgo119\tsz119\tsd119\tso119\t";
                                            SchText += "Nur120\tname120\tVn120\tgz120\tgd120\tgo120\tsz120\tsd120\tso120\t";
                                            SchText += "Hz60\tHD60\tHD6011\tHD6021\t";
                                            SchText += "Nur121\tname121\tVn121\tgz121\tgd121\tgo121\tsz121\tsd121\tso121\t";
                                            SchText += "Nur122\tname122\tVn122\tgz122\tgd122\tgo122\tsz122\tsd122\tso122\t";
                                            SchText += "Hz61\tHD61\tHD6111\tHD6121\t";
                                            SchText += "Nur123\tname123\tVn123\tgz123\tgd123\tgo123\tsz123\tsd123\tso123\t";
                                            SchText += "Nur124\tname124\tVn124\tgz124\tgd124\tgo124\tsz124\tsd124\tso124\t";
                                            SchText += "Hz62\tHD62\tHD6231\tHD6241\t";
                                            SchText += "Nur125\tname125\tVn125\tgz125\tgd125\tgo125\tsz125\tsd125\tso125\t";
                                            SchText += "Nur126\tname126\tVn126\tgz126\tgd126\tgo126\tsz126\tsd126\tso126\t";
                                            SchText += "Hz63\tHD63\tHD6311\tHD6321\t";
                                            SchText += "Nur127\tname127\tVn127\tgz127\tgd127\tgo127\tsz127\tsd127\tso127\t";
                                        }
                                        goto IL_1584;
                                    }
                                }
                                goto IL_1584;
                            IL_1584: // <========== 3
                                num = 220;
                                lErl = 11;
                                SchText += "\n";
                                M_Start = (int)Math.Round(Interaction.InputBox("Start mit Ahnennummer", " ", "1").AsDouble());
                                if (M_Start == 0)
                                {
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000_2;
                                }
                                I = 0;
                                goto IL_161e;
                            IL_161e: // <========== 3
                                num = 229;
                                switch (I)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        goto IL_186f;
                                    case 2:
                                        goto IL_1895;
                                    case 3:
                                        goto IL_18bd;
                                    case 4:
                                        goto IL_18e7;
                                    case 5:
                                    case 6:
                                    case 7:
                                        goto IL_1911;
                                    case 8:
                                        goto IL_1944;
                                    case 9:
                                    case 10:
                                    case 11:
                                    case 12:
                                    case 13:
                                    case 14:
                                    case 15:
                                        goto IL_1970;
                                    case 16:
                                        goto IL_19a5;
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
                                        goto IL_19d3;
                                    case 32:
                                    case 33:
                                    case 34:
                                    case 35:
                                    case 36:
                                    case 37:
                                    case 38:
                                    case 39:
                                    case 40:
                                    case 41:
                                    case 42:
                                    case 43:
                                    case 44:
                                    case 45:
                                    case 46:
                                    case 47:
                                    case 48:
                                    case 49:
                                    case 50:
                                    case 51:
                                    case 52:
                                    case 53:
                                    case 54:
                                    case 55:
                                    case 56:
                                    case 57:
                                    case 58:
                                    case 59:
                                    case 60:
                                    case 61:
                                    case 62:
                                    case 63:
                                        goto IL_1a08;
                                    case 64:
                                    case 65:
                                    case 66:
                                    case 67:
                                    case 68:
                                    case 69:
                                    case 70:
                                    case 71:
                                    case 72:
                                    case 73:
                                    case 74:
                                    case 75:
                                    case 76:
                                    case 77:
                                    case 78:
                                    case 79:
                                    case 80:
                                    case 81:
                                    case 82:
                                    case 83:
                                    case 84:
                                    case 85:
                                    case 86:
                                    case 87:
                                    case 88:
                                    case 89:
                                    case 90:
                                    case 91:
                                    case 92:
                                    case 93:
                                    case 94:
                                    case 95:
                                    case 96:
                                    case 97:
                                    case 98:
                                    case 99:
                                    case 100:
                                    case 101:
                                    case 102:
                                    case 103:
                                    case 104:
                                    case 105:
                                    case 106:
                                    case 107:
                                    case 108:
                                    case 109:
                                    case 110:
                                    case 111:
                                    case 112:
                                    case 113:
                                    case 114:
                                    case 115:
                                    case 116:
                                    case 117:
                                    case 118:
                                    case 119:
                                    case 120:
                                    case 121:
                                    case 122:
                                    case 123:
                                    case 124:
                                    case 125:
                                    case 126:
                                    case 127:
                                        goto IL_1a3f;
                                    default:
                                        goto IL_1a76;
                                }
                                Nr = Strings.LTrim((M_Start.AsString().AsDouble() - 1.0).AsString());
                                goto IL_1a76;
                            IL_186f:
                                num = 236;
                                Nr = M_Start.AsString().TrimStart();
                                goto IL_1a76;
                            IL_1895:
                                num = 239;
                                Nr = (M_Start * 2).AsString().TrimStart();
                                goto IL_1a76;
                            IL_18bd:
                                num = 242;
                                Nr = (M_Start * 2 + 1).AsString().TrimStart();
                                goto IL_1a76;
                            IL_18e7:
                                num = 245;
                                Nr = (M_Start * 2 * 2).AsString().TrimStart();
                                goto IL_1a76;
                            IL_1911:
                                num = 248;
                                Nr = (M_Start * 2 * 2 + I - 4).AsString().TrimStart();
                                goto IL_1a76;
                            IL_1944:
                                num = 251;
                                Nr = (M_Start * 2 * 2 * 2).AsString().TrimStart();
                                goto IL_1a76;
                            IL_1970:
                                num = 254;
                                Nr = Strings.LTrim(M_Start * 2 * 2 * 2 + (I - 8).AsString());
                                goto IL_1a76;
                            IL_19a5:
                                num = 257;
                                Nr = (M_Start * 2 * 2 * 2 * 2).AsString().TrimStart();
                                goto IL_1a76;
                            IL_19d3:
                                num = 260;
                                Nr = (M_Start * 2 * 2 * 2 * 2 + I - 16).AsString().TrimStart();
                                goto IL_1a76;
                            IL_1a08:
                                num = 263;
                                Nr = (M_Start * 2 * 2 * 2 * 2 * 2 + I - 32).AsString().TrimStart();
                                goto IL_1a76;
                            IL_1a3f:
                                num = 266;
                                Nr = (M_Start * 2 * 2 * 2 * 2 * 2 * 2 + I - 64).AsString().TrimStart();
                                goto IL_1a76;
                            IL_1a76: // <========== 14
                                num = 268;
                                Text1 = " ";
                                ATperles();
                                i = I;
                                if (i == (short)"0".AsInt() || i == (short)"16".AsInt() || i == (short)"32".AsInt() || i == (short)"48".AsInt() || i == (short)"64".AsInt() || i == (short)"80".AsInt() || i == (short)"96".AsInt() || i == (short)"112".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"1".AsInt() || i == (short)"17".AsInt() || i == (short)"33".AsInt() || i == (short)"49".AsInt() || i == (short)"65".AsInt() || i == (short)"81".AsInt() || i == (short)"97".AsInt() || i == (short)"113".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"2".AsInt() || i == (short)"18".AsInt() || i == (short)"34".AsInt() || i == (short)"50".AsInt() || i == (short)"66".AsInt() || i == (short)"82".AsInt() || i == (short)"98".AsInt() || i == (short)"114".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"3".AsInt() || i == (short)"19".AsInt() || i == (short)"35".AsInt() || i == (short)"51".AsInt() || i == (short)"67".AsInt() || i == (short)"83".AsInt() || i == (short)"99".AsInt() || i == (short)"115".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"4".AsInt() || i == (short)"20".AsInt() || i == (short)"36".AsInt() || i == (short)"52".AsInt() || i == (short)"68".AsInt() || i == (short)"84".AsInt() || i == (short)"100".AsInt() || i == (short)"116".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"5".AsInt() || i == (short)"21".AsInt() || i == (short)"37".AsInt() || i == (short)"53".AsInt() || i == (short)"69".AsInt() || i == (short)"85".AsInt() || i == (short)"101".AsInt() || i == (short)"117".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"6".AsInt() || i == (short)"22".AsInt() || i == (short)"38".AsInt() || i == (short)"54".AsInt() || i == (short)"70".AsInt() || i == (short)"86".AsInt() || i == (short)"102".AsInt() || i == (short)"118".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"7".AsInt() || i == (short)"23".AsInt() || i == (short)"39".AsInt() || i == (short)"55".AsInt() || i == (short)"71".AsInt() || i == (short)"87".AsInt() || i == (short)"103".AsInt() || i == (short)"119".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"8".AsInt() || i == (short)"24".AsInt() || i == (short)"40".AsInt() || i == (short)"56".AsInt() || i == (short)"72".AsInt() || i == (short)"88".AsInt() || i == (short)"104".AsInt() || i == (short)"120".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"9".AsInt() || i == (short)"25".AsInt() || i == (short)"41".AsInt() || i == (short)"57".AsInt() || i == (short)"73".AsInt() || i == (short)"89".AsInt() || i == (short)"105".AsInt() || i == (short)"121".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"10".AsInt() || i == (short)"26".AsInt() || i == (short)"42".AsInt() || i == (short)"58".AsInt() || i == (short)"74".AsInt() || i == (short)"90".AsInt() || i == (short)"106".AsInt() || i == (short)"122".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"11".AsInt() || i == (short)"27".AsInt() || i == (short)"43".AsInt() || i == (short)"59".AsInt() || i == (short)"75".AsInt() || i == (short)"91".AsInt() || i == (short)"107".AsInt() || i == (short)"123".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"12".AsInt() || i == (short)"28".AsInt() || i == (short)"44".AsInt() || i == (short)"60".AsInt() || i == (short)"76".AsInt() || i == (short)"92".AsInt() || i == (short)"108".AsInt() || i == (short)"124".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"13".AsInt() || i == (short)"29".AsInt() || i == (short)"45".AsInt() || i == (short)"61".AsInt() || i == (short)"77".AsInt() || i == (short)"93".AsInt() || i == (short)"109".AsInt() || i == (short)"125".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    goto IL_2be2;
                                }
                                if (i == (short)"14".AsInt() || i == (short)"30".AsInt() || i == (short)"46".AsInt() || i == (short)"62".AsInt() || i == (short)"78".AsInt() || i == (short)"94".AsInt() || i == (short)"110".AsInt() || i == (short)"126".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    SchText = SchText + Fz + "\t" + Famdat + "\t" + FamDat1 + "\t" + Famdat2 + "\t";
                                    goto IL_2be2;
                                }
                                if (i == (short)"15".AsInt() || i == (short)"47".AsInt() || i == (short)"79".AsInt() || i == (short)"95".AsInt() || i == (short)"111".AsInt() || i == (short)"127".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    if (!((Conversion.Int(_Modul1.Instance.Aschalt) == 7f) & (I == 127)))
                                    {
                                        if (!((Conversion.Int(_Modul1.Instance.Aschalt) == 4f) & (I == 15)))
                                        {
                                            if ((Conversion.Int(_Modul1.Instance.Aschalt) == 4f) & (I == 15))
                                            {
                                                SchText += "\n";
                                                goto IL_2c05;
                                            }
                                            goto IL_2be2;
                                        }
                                    }
                                    goto IL_2c05;
                                }
                                if (i == (short)"31".AsInt() || i == (short)"63".AsInt() || i == (short)"113".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                    if (!((Conversion.Int(_Modul1.Instance.Aschalt) == 5f) & (I == 31)))
                                    {
                                        if (!((Conversion.Int(_Modul1.Instance.Aschalt) == 6f) & (I == 63)))
                                        {
                                            if ((Conversion.Int(_Modul1.Instance.Aschalt) == 5f) & (I == 31))
                                            {
                                                SchText += "\n";
                                                goto IL_2c05;
                                            }
                                            if ((Conversion.Int(_Modul1.Instance.Aschalt) == 6f) & (I == 63))
                                            {
                                                SchText += "\n";
                                                goto IL_2c05;
                                            }
                                            goto IL_2be2;
                                        }
                                    }
                                    goto IL_2c05;
                                }
                                if (i == (short)"127".AsInt())
                                {
                                    SchText = SchText + Nr + "\t" + Text1;
                                }
                                goto IL_2be2;
                            IL_2be2: // <========== 19
                                num = 359;
                                I = (short)unchecked(I + 1);
                                if (I <= 127)
                                {
                                    goto IL_161e;
                                }
                                goto IL_2c05;
                            IL_2c05: // <========== 7
                                num = 360;
                                lErl = 13;
                                Frame1.Visible = false;
                                FileSystem.PrintLine(99, SchText);
                                FileSystem.FileClose(99);
                                Command1[0].Enabled = true;
                                Msg = "Die Steuerdatei wurde gespeichert.";
                                Msg += "\nMit Klick auf Drucken erhalten Sie eine Druckvorschau.";
                                Interaction.MsgBox(Msg);
                                goto end_IL_0000_2;
                            IL_2cfe: // <========== 3
                                num = 375;
                                if (Information.Err().Number == 75)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_2d7f;
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
                                goto IL_2d83;
                            IL_2d7f:
                                num4 = unchecked(num2 + 1);
                                goto IL_2d83;
                            IL_2d83:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 44:
                                    case 71:
                                    case 122:
                                    case 220:
                                        goto IL_1584;
                                    case 229:
                                        goto IL_161e;
                                    case 230:
                                    case 234:
                                    case 237:
                                    case 240:
                                    case 243:
                                    case 246:
                                    case 249:
                                    case 252:
                                    case 255:
                                    case 258:
                                    case 261:
                                    case 264:
                                    case 267:
                                    case 268:
                                        goto IL_1a76;
                                    case 271:
                                    case 276:
                                    case 279:
                                    case 283:
                                    case 286:
                                    case 290:
                                    case 293:
                                    case 297:
                                    case 300:
                                    case 304:
                                    case 307:
                                    case 311:
                                    case 314:
                                    case 318:
                                    case 321:
                                    case 325:
                                    case 337:
                                    case 338:
                                    case 354:
                                    case 355:
                                    case 358:
                                    case 359:
                                        goto IL_2be2;
                                    case 329:
                                    case 332:
                                    case 336:
                                    case 342:
                                    case 345:
                                    case 349:
                                    case 353:
                                    case 360:
                                        goto IL_2c05;
                                    case 374:
                                    case 375:
                                        goto IL_2cfe;
                                    case 4:
                                    case 9:
                                    case 10:
                                    case 226:
                                    case 368:
                                    case 369:
                                    case 373:
                                    case 383:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 13197;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 6
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Command1_Click(object eventSender, EventArgs eventArgs)
        {
            switch (Command1.GetIndex((Button)eventSender))
            {
                case 0:
                    {
                        if (Strings.InStr(_Modul1.Instance.Aus[7].ToUpper(), "WINWORD") == 0)
                        {
                            string text = "Ihre eingestellte Textverarbeitung ist nicht MS-Word!";
                            text += "\nEine einwandfreie Funktion der Ahnentafeln kann nur für MS-Word garantiert werden ";
                            Interaction.MsgBox(text, MsgBoxStyle.Exclamation, "");
                        }
                        float aschalt = _Modul1.Instance.Aschalt;
                        if (aschalt == 3f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\2803_5.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 4.1f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT4_01.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 4.2f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT4_02.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 4.3f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT4_011.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 4.4f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT4_022.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_01.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.1f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_02.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.2f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_03.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.3f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_04.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.4f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_022.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.5f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_044.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.6f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_011.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 5.7f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT5_033.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 6.1f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT6_01.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 6.2f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT6_02.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 6.3f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT6_011.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 6.4f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT6_022.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 7.1f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT7_1.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 7.2f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT7_2.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 7.3f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT7_11.doc", AppWinStyle.MaximizedFocus);
                        }
                        else if (aschalt == 7.4f)
                        {
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT7_22.doc", AppWinStyle.MaximizedFocus);
                        }
                        break;
                    }
                case 1:
                    Close();
                    break;
            }
        }

        private void ATo_Load(object eventSender, EventArgs eventArgs)
        {
            BackColor = _Modul1.Instance.HintFarb;
            _Modul1.Instance.Dateienopen();
            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<Enum>("Windowstate");
            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
            ProjectData.ClearProjectError();
            Command1[0].Enabled = false;
            _Modul1.Instance.Verz1 = _Modul1.Instance.Verz.Left(15);
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
            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
            FileSystem.FileClose(6);
            Show();
            FileSystem.FileClose(30);
            Label1[3].Text = "Keine Berechnung vorhanden";
            Label1[0].Text = "Sie müssen erst die Ahnen berechnen.";
            if (DataModul.NB_AhnTable.RecordCount == 0)
            {
                Command_Renamed[1].Enabled = false;
            }
            DataModul.NB_AhnTable.Index = "Ahnen";
            DataModul.NB_AhnTable.Seek("=", 1);
            DataModul.NB_AhnTable.MoveFirst();
            if (!DataModul.NB_AhnTable.EOF)
            {
                if (!DataModul.NB_AhnTable.NoMatch)
                {
                    string text = DataModul.NB_AhnTable.Fields["Ahn"].AsString();
                    DataModul.NB_AhnTable.MoveLast();
                    Label1[3].Text = "Ahnenberechnung " + DataModul.NB_AhnTable.Fields["Gen"].AsString() + " Generationen vorhanden für Ahnenziffer " + text;
                    DataModul.NB_AhnTable.MoveFirst();
                    _Modul1.Instance.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                    if (_Modul1.Instance.Kont[1].Trim() != "")
                    {
                        _Modul1.Instance.Kont[1] = _Modul1.Instance.Kont[1] + " ";
                    }
                    Label1[0].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[1] + _Modul1.Instance.Kont[0].ToUpper();
                    _Modul1.Instance.Datles2();

                }
            }

            Label1[5].Text = _Modul1.Instance.IText[3] + " " + _Modul1.Instance.Kont[11];
            Label1[2].Text = _Modul1.Instance.IText[5] + " " + _Modul1.Instance.Kont[13];
            Label1[1].Text = _Modul1.Instance.IText[4] + " " + _Modul1.Instance.Kont[12];
            Label1[4].Text = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
            MyProject.Forms.Druck.Show();
        }
        public void ATperles()
        {
            Pername = " ";
            Sterdat1 = "";
            Sterdat = "";
            Gebdat1 = "";
            Gebdat = "";
            VNam = " ";
            Famdat = "";
            Gebdat2 = " ";
            sterdat2 = " ";
            Famdat = "";
            FamDat1 = "";
            Famdat2 = "";
            Fz = "";
            DataModul.NB_AhnTable.Index = "Ahnen";
            DataModul.NB_AhnTable.Seek("=", new string(' ', 40) + Nr.Right(40));
            checked
            {
                if (!DataModul.NB_AhnTable.NoMatch)
                {
                    _Modul1.Instance.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                    VNam = _Modul1.Instance.Kont[3];
                    if (_Modul1.Instance.Kont[1] != "")
                    {
                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                    }
                    if (_Modul1.Instance.Kont[2] != "")
                    {
                        _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[0] + " " + _Modul1.Instance.Kont[2];
                    }
                    Pername = _Modul1.Instance.Kont[0];
                    _Modul1.Instance.Schalt = 200;
                    byte PerPos = 0;
                    _Modul1.Instance.Datles(_Modul1.Instance.PersInArb, out var asPersDates);
                    Gebdat = _Modul1.Instance.Kont[21];
                    if (Gebdat == "")
                    {
                        Gebdat = _Modul1.Instance.Kont[22];
                    }
                    Sterdat = _Modul1.Instance.Kont[23];
                    if (Sterdat == "")
                    {
                        Sterdat = _Modul1.Instance.Kont[24];
                    }
                    Gebdat1 = _Modul1.Instance.Kont[11];
                    if (Gebdat1 == "")
                    {
                        Gebdat1 = _Modul1.Instance.Kont[12];
                    }
                    Gebdat2 = Strings.Mid(Gebdat1, Gebdat.Length + 1, Gebdat1.Length);
                    Sterdat1 = _Modul1.Instance.Kont[13];
                    if (Sterdat1 == "")
                    {
                        Sterdat1 = _Modul1.Instance.Kont[14];
                    }
                    sterdat2 = Strings.Mid(Sterdat1, Sterdat.Length + 1, Sterdat1.Length);
                    _Modul1.Instance.FamInArb = DataModul.NB_AhnTable.Fields["Ehe"].AsInt();
                    _Modul1.Instance.Schalt = 6;
                    _Modul1.Instance.Famdatles1(1, out var asFamDate);
                    Famdat = asFamDate[2].Trim();
                    if (asFamDate[52].Trim() != "")
                    {
                        A = (short)Strings.InStr(Famdat, asFamDate[52].Trim());
                        if (A > 0)
                        {
                            FamDat1 = Strings.Trim(Famdat.Left(A - 1));
                        }
                    }
                    else
                    {
                        FamDat1 = Famdat.Trim();
                    }
                    Famdat2 = asFamDate[52].Trim();
                    if (Famdat.Trim() == "")
                    {
                        Famdat = asFamDate[3].Trim();
                        if (asFamDate[53].Trim() != "")
                        {
                            A = (short)Strings.InStr(Famdat, asFamDate[53].Trim());
                            if (A > 0)
                            {
                                FamDat1 = Strings.Trim(Famdat.Left(A - 1));
                            }
                        }
                        else
                        {
                            FamDat1 = Famdat.Trim();
                        }
                        Famdat2 = asFamDate[53].Trim();
                    }
                    if (Famdat.Trim() != "")
                    {
                        Fz = "oo ";
                    }
                    else
                    {
                        Fz = "";
                    }
                }
                Text1 = Pername + "\t";
                Text1 = Text1 + VNam + "\t";
                if ((Gebdat.Trim() != "") | (Gebdat2.Trim() != ""))
                {
                    Text1 += "* \t";
                }
                else
                {
                    Text1 += " \t";
                }
                Text1 = Text1 + Gebdat + "\t" + Gebdat2 + "\t";
                if ((Sterdat.Trim() != "") | (sterdat2.Trim() != ""))
                {
                    Text1 += "+ \t";
                }
                else
                {
                    Text1 += " \t";
                }
                Text1 = Text1 + Sterdat + "\t" + sterdat2 + "\t";
            }
        }

        private void _Command_1_Click(object sender, EventArgs e)
        {
        }

        internal void Show(float v)
        {
            _Modul1.Instance.Aschalt = v;
            base.Show();
        }
    }
}
