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
    internal class AT6 : Form
    {
        private IContainer components;

        public ToolTip ToolTip1;

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

        private string Text1;

        private string Nr;

        private string sterdat2;

        private string Gebdat2;

        private string VNam;

        private string Gebdat;

        private string Gebdat1;

        private string Sterdat;

        private string Sterdat1;

        private string Pername;

        private string Msg;

        private string Famdat;

        private string SchText;

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
                __Command_1 = value;
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
                    _Command1.RemoveClick (obj);
                }
                _Command1 = value;
                if (_Command1 != null)
                {
                    _Command1.AddClick (obj);
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
        public AT6()
        {
            base.Load += AT6_Load;
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
            this.Command_Renamed = new ControlArray<System.Windows.Forms.Button>();
            this.Command1 = new ControlArray<System.Windows.Forms.Button>();
            this.Label1 = new ControlArray<System.Windows.Forms.Label>();
            this.Frame1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
            this.SuspendLayout();
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_1, 1);
            System.Drawing.Point point2 = this._Command1_1.Location = new System.Drawing.Point(688, 547);
            System.Windows.Forms.Padding padding2 = this._Command1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            System.Drawing.Size size2 = this._Command1_1.Size = new System.Drawing.Size(156, 54);
            this._Command1_1.TabIndex = 10;
            this._Command1_1.Text = "Druckmenü";
            this._Command1_1.UseVisualStyleBackColor = false;
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_0, 0);
            point2 = this._Command1_0.Location = new System.Drawing.Point(299, 547);
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
            point2 = this.Frame1.Location = new System.Drawing.Point(296, 285);
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
            this.Controls.Add(this._Command1_1);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Label2);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            padding2 = this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AT6";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Etiketten fürSchmuckahnentafel 6 Generationen";
            this.Frame1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            int lErl = default;
            object CounterResult = default;
            object LoopForResult = default;
            int left = default;
            string nr = default;
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
                            {
                                num = 1;
                                int index = Command_Renamed.GetIndex((Button)eventSender);
                                goto IL_0015;
                            }
                        case 8511:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1b85;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_1b04;
                            }
                        IL_1b04:
                            num = 354;
                            if (Information.Err().Number == 75)
                            {
                                goto IL_1b19;
                            }
                            goto IL_1b36;
                        IL_1b36:
                            num = 358;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_1b5f;
                        IL_0ef3:
                            num = 240;
                            SchText += "Ahnenziffern 8; 9; 10; 11\t";
                            goto IL_0f10;
                        IL_1b5f:
                            num = 361;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_1b89;
                        IL_0f10:
                            num = 242;
                            if (Nr ==  "12")
                            {
                                goto IL_0f2b;
                            }
                            goto IL_0f48;
                        IL_0f2b:
                            num = 243;
                            SchText += "Ahnenziffern 12; 13; 14; 15\t";
                            goto IL_0f48;
                        IL_1b89:
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
                                    goto IL_002a;
                                case 5:
                                    goto IL_003d;
                                case 6:
                                    goto IL_004b;
                                case 7:
                                    goto IL_0064;
                                case 8:
                                    goto IL_007d;
                                case 9:
                                    goto IL_0096;
                                case 10:
                                    goto IL_00b0;
                                case 11:
                                    goto IL_00ca;
                                case 12:
                                    goto IL_00e4;
                                case 13:
                                    goto IL_00fe;
                                case 14:
                                    goto IL_0118;
                                case 15:
                                    goto IL_0132;
                                case 16:
                                    goto IL_014c;
                                case 17:
                                    goto IL_0166;
                                case 18:
                                    goto IL_0180;
                                case 19:
                                    goto IL_019a;
                                case 20:
                                    goto IL_01b4;
                                case 21:
                                    goto IL_01ce;
                                case 22:
                                    goto IL_01e8;
                                case 23:
                                    goto IL_0202;
                                case 24:
                                    goto IL_021c;
                                case 25:
                                    goto IL_0236;
                                case 26:
                                    goto IL_0250;
                                case 27:
                                    goto IL_026a;
                                case 28:
                                    goto IL_0284;
                                case 29:
                                    goto IL_029e;
                                case 30:
                                    goto IL_02b8;
                                case 31:
                                    goto IL_02de;
                                case 32:
                                    goto IL_02ed;
                                case 34:
                                case 35:
                                    goto IL_02f4;
                                case 36:
                                    goto IL_0308;
                                case 38:
                                    goto IL_031c;
                                case 39:
                                    goto IL_0330;
                                case 41:
                                    goto IL_0344;
                                case 42:
                                    goto IL_0358;
                                case 44:
                                    goto IL_036c;
                                case 45:
                                    goto IL_0380;
                                case 47:
                                    goto IL_0394;
                                case 48:
                                    goto IL_03a8;
                                case 50:
                                    goto IL_03bc;
                                case 51:
                                    goto IL_03d0;
                                case 53:
                                    goto IL_03e4;
                                case 54:
                                    goto IL_03f8;
                                case 56:
                                    goto IL_040c;
                                case 57:
                                    goto IL_0420;
                                case 59:
                                    goto IL_0434;
                                case 60:
                                    goto IL_0448;
                                case 62:
                                    goto IL_045c;
                                case 63:
                                    goto IL_0471;
                                case 65:
                                    goto IL_0485;
                                case 66:
                                    goto IL_049a;
                                case 68:
                                    goto IL_04ae;
                                case 69:
                                    goto IL_04c3;
                                case 71:
                                    goto IL_04d7;
                                case 72:
                                    goto IL_04ec;
                                case 74:
                                    goto IL_0500;
                                case 75:
                                    goto IL_0515;
                                case 77:
                                    goto IL_0529;
                                case 78:
                                    goto IL_053e;
                                case 80:
                                    goto IL_0552;
                                case 81:
                                    goto IL_0567;
                                case 83:
                                    goto IL_057b;
                                case 84:
                                    goto IL_0590;
                                case 86:
                                    goto IL_05a4;
                                case 87:
                                    goto IL_05b9;
                                case 89:
                                    goto IL_05cd;
                                case 90:
                                    goto IL_05e2;
                                case 92:
                                    goto IL_05f6;
                                case 93:
                                    goto IL_060b;
                                case 95:
                                    goto IL_061f;
                                case 96:
                                    goto IL_0634;
                                case 98:
                                    goto IL_0648;
                                case 99:
                                    goto IL_065d;
                                case 101:
                                    goto IL_0671;
                                case 102:
                                    goto IL_0686;
                                case 104:
                                    goto IL_069a;
                                case 105:
                                    goto IL_06af;
                                case 107:
                                    goto IL_06c3;
                                case 108:
                                    goto IL_06d8;
                                case 110:
                                    goto IL_06ec;
                                case 111:
                                    goto IL_0701;
                                case 113:
                                    goto IL_0715;
                                case 114:
                                    goto IL_072a;
                                case 116:
                                    goto IL_073e;
                                case 117:
                                    goto IL_0753;
                                case 119:
                                    goto IL_0767;
                                case 120:
                                    goto IL_077c;
                                case 122:
                                    goto IL_0790;
                                case 123:
                                    goto IL_07a5;
                                case 125:
                                    goto IL_07b9;
                                case 126:
                                    goto IL_07ce;
                                case 128:
                                    goto IL_07e2;
                                case 129:
                                    goto IL_07fa;
                                case 131:
                                    goto IL_0811;
                                case 132:
                                    goto IL_0829;
                                case 134:
                                    goto IL_0840;
                                case 135:
                                    goto IL_0858;
                                case 137:
                                    goto IL_086f;
                                case 138:
                                    goto IL_0887;
                                case 140:
                                    goto IL_089e;
                                case 141:
                                    goto IL_08b6;
                                case 143:
                                    goto IL_08cd;
                                case 144:
                                    goto IL_08e5;
                                case 146:
                                    goto IL_08fc;
                                case 147:
                                    goto IL_0914;
                                case 149:
                                    goto IL_092b;
                                case 150:
                                    goto IL_0943;
                                case 152:
                                    goto IL_095a;
                                case 153:
                                    goto IL_0972;
                                case 155:
                                    goto IL_0989;
                                case 156:
                                    goto IL_09a1;
                                case 158:
                                    goto IL_09b8;
                                case 159:
                                    goto IL_09d0;
                                case 161:
                                    goto IL_09e7;
                                case 162:
                                    goto IL_09ff;
                                case 164:
                                    goto IL_0a16;
                                case 165:
                                    goto IL_0a2e;
                                case 167:
                                    goto IL_0a45;
                                case 168:
                                    goto IL_0a5d;
                                case 170:
                                    goto IL_0a74;
                                case 171:
                                    goto IL_0a8c;
                                case 173:
                                    goto IL_0aa3;
                                case 174:
                                    goto IL_0abb;
                                case 176:
                                    goto IL_0ad2;
                                case 177:
                                    goto IL_0aea;
                                case 179:
                                    goto IL_0b01;
                                case 180:
                                    goto IL_0b19;
                                case 182:
                                    goto IL_0b30;
                                case 183:
                                    goto IL_0b48;
                                case 185:
                                    goto IL_0b5f;
                                case 186:
                                    goto IL_0b77;
                                case 188:
                                    goto IL_0b8e;
                                case 189:
                                    goto IL_0ba6;
                                case 191:
                                    goto IL_0bbd;
                                case 192:
                                    goto IL_0bd5;
                                case 194:
                                    goto IL_0bec;
                                case 195:
                                    goto IL_0c04;
                                case 197:
                                    goto IL_0c1b;
                                case 198:
                                    goto IL_0c33;
                                case 200:
                                    goto IL_0c4a;
                                case 201:
                                    goto IL_0c62;
                                case 203:
                                    goto IL_0c79;
                                case 204:
                                    goto IL_0c91;
                                case 206:
                                    goto IL_0ca8;
                                case 207:
                                    goto IL_0cc0;
                                case 209:
                                    goto IL_0cd7;
                                case 210:
                                    goto IL_0cef;
                                case 212:
                                    goto IL_0d06;
                                case 213:
                                    goto IL_0d1e;
                                case 215:
                                    goto IL_0d35;
                                case 216:
                                    goto IL_0d4d;
                                case 218:
                                    goto IL_0d64;
                                case 219:
                                    goto IL_0d7c;
                                case 221:
                                    goto IL_0d90;
                                case 222:
                                    goto IL_0da8;
                                case 224:
                                    goto IL_0dbc;
                                case 225:
                                    goto IL_0dd4;
                                case 33:
                                case 37:
                                case 40:
                                case 43:
                                case 46:
                                case 49:
                                case 52:
                                case 55:
                                case 58:
                                case 61:
                                case 64:
                                case 67:
                                case 70:
                                case 73:
                                case 76:
                                case 79:
                                case 82:
                                case 85:
                                case 88:
                                case 91:
                                case 94:
                                case 97:
                                case 100:
                                case 103:
                                case 106:
                                case 109:
                                case 112:
                                case 115:
                                case 118:
                                case 121:
                                case 124:
                                case 127:
                                case 130:
                                case 133:
                                case 136:
                                case 139:
                                case 142:
                                case 145:
                                case 148:
                                case 151:
                                case 154:
                                case 157:
                                case 160:
                                case 163:
                                case 166:
                                case 169:
                                case 172:
                                case 175:
                                case 178:
                                case 181:
                                case 184:
                                case 187:
                                case 190:
                                case 193:
                                case 196:
                                case 199:
                                case 202:
                                case 205:
                                case 208:
                                case 211:
                                case 214:
                                case 217:
                                case 220:
                                case 223:
                                case 226:
                                case 227:
                                    goto IL_0de6;
                                case 228:
                                    goto IL_0df8;
                                case 229:
                                    goto IL_0e0b;
                                case 231:
                                case 232:
                                    goto IL_0e1a;
                                case 233:
                                    goto IL_0e68;
                                case 234:
                                    goto IL_0e83;
                                case 235:
                                case 236:
                                    goto IL_0ea0;
                                case 237:
                                    goto IL_0ebb;
                                case 238:
                                case 239:
                                    goto IL_0ed8;
                                case 240:
                                    goto IL_0ef3;
                                case 241:
                                case 242:
                                    goto IL_0f10;
                                case 243:
                                    goto IL_0f2b;
                                case 244:
                                case 245:
                                    goto IL_0f48;
                                case 247:
                                    goto IL_0f6b;
                                case 248:
                                    goto IL_0fb6;
                                case 249:
                                    goto IL_0fd4;
                                case 251:
                                    goto IL_0ffc;
                                case 252:
                                    goto IL_1047;
                                case 254:
                                    goto IL_106a;
                                case 255:
                                    goto IL_10b5;
                                case 256:
                                    goto IL_10d3;
                                case 258:
                                    goto IL_10fb;
                                case 259:
                                    goto IL_1149;
                                case 260:
                                    goto IL_1164;
                                case 261:
                                case 262:
                                    goto IL_1181;
                                case 263:
                                    goto IL_119c;
                                case 264:
                                case 265:
                                    goto IL_11b9;
                                case 266:
                                    goto IL_11d4;
                                case 267:
                                case 268:
                                    goto IL_11f1;
                                case 269:
                                    goto IL_120c;
                                case 270:
                                case 271:
                                    goto IL_1229;
                                case 273:
                                    goto IL_124c;
                                case 274:
                                    goto IL_1299;
                                case 275:
                                    goto IL_12b7;
                                case 277:
                                    goto IL_12df;
                                case 278:
                                    goto IL_132c;
                                case 280:
                                    goto IL_134f;
                                case 281:
                                    goto IL_139c;
                                case 282:
                                    goto IL_13ba;
                                case 284:
                                    goto IL_13e2;
                                case 285:
                                    goto IL_1430;
                                case 286:
                                    goto IL_144b;
                                case 287:
                                case 288:
                                    goto IL_1468;
                                case 289:
                                    goto IL_1483;
                                case 290:
                                case 291:
                                    goto IL_14a0;
                                case 292:
                                    goto IL_14bb;
                                case 293:
                                case 294:
                                    goto IL_14d8;
                                case 295:
                                    goto IL_14f3;
                                case 296:
                                case 297:
                                    goto IL_1510;
                                case 299:
                                    goto IL_1533;
                                case 300:
                                    goto IL_1580;
                                case 301:
                                    goto IL_159e;
                                case 303:
                                    goto IL_15c6;
                                case 304:
                                    goto IL_1611;
                                case 306:
                                    goto IL_1634;
                                case 307:
                                    goto IL_167f;
                                case 308:
                                    goto IL_169d;
                                case 310:
                                    goto IL_16c5;
                                case 311:
                                    goto IL_1713;
                                case 312:
                                    goto IL_172e;
                                case 313:
                                case 314:
                                    goto IL_174b;
                                case 315:
                                    goto IL_1766;
                                case 316:
                                case 317:
                                    goto IL_1783;
                                case 318:
                                    goto IL_179e;
                                case 319:
                                case 320:
                                    goto IL_17bb;
                                case 321:
                                    goto IL_17d6;
                                case 322:
                                case 323:
                                    goto IL_17f3;
                                case 325:
                                    goto IL_1816;
                                case 326:
                                    goto IL_1861;
                                case 327:
                                    goto IL_187f;
                                case 329:
                                    goto IL_18a7;
                                case 330:
                                    goto IL_18f2;
                                case 332:
                                    goto IL_1915;
                                case 333:
                                    goto IL_194e;
                                case 334:
                                    goto IL_196c;
                                case 336:
                                    goto IL_1991;
                                case 337:
                                    goto IL_19a8;
                                case 338:
                                    goto IL_19c6;
                                case 340:
                                    num = 340;
                                    lErl = 33;
                                    goto IL_19f6;
                                case 230:
                                case 246:
                                case 250:
                                case 253:
                                case 257:
                                case 272:
                                case 276:
                                case 279:
                                case 283:
                                case 298:
                                case 302:
                                case 305:
                                case 309:
                                case 324:
                                case 328:
                                case 331:
                                case 335:
                                case 341:
                                case 342:
                                    goto IL_19f6;
                                case 343:
                                    goto IL_1a0c;
                                case 344:
                                    goto IL_1a29;
                                case 339:
                                case 345:
                                    goto IL_1a46;
                                case 346:
                                    goto IL_1a51;
                                case 347:
                                    goto IL_1a64;
                                case 348:
                                    goto IL_1a86;
                                case 349:
                                    goto IL_1aa2;
                                case 350:
                                    goto IL_1abb;
                                case 351:
                                    goto IL_1acd;
                                case 352:
                                    goto IL_1aea;
                                case 354:
                                    goto IL_1b04;
                                case 355:
                                    goto IL_1b19;
                                case 356:
                                case 358:
                                    goto IL_1b36;
                                case 359:
                                case 361:
                                    goto IL_1b5f;
                                default:
                                    goto end_IL_0000;
                                case 353:
                                case 362:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_1b19:
                            num = 355;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_1b85;
                        IL_0ed8:
                            num = 239;
                            if (Nr ==  "8")
                            {
                                goto IL_0ef3;
                            }
                            goto IL_0f10;
                        IL_1b85:
                            num4 = num2 + 1;
                            goto IL_1b89;
                        IL_0015:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_001d;
                        IL_001d:
                            num = 3;
                            FileSystem.MkDir("C:\\Temp");
                            goto IL_002a;
                        IL_002a:
                            num = 4;
                            FileSystem.FileOpen(99, "C:\\Temp\\At66.txt", OpenMode.Output);
                            goto IL_003d;
                        IL_003d:
                            num = 5;
                            SchText = "NR1\tName1\tVn1\tgd1\tgo1\tsd1\tso1\t";
                            goto IL_004b;
                        IL_004b:
                            num = 6;
                            SchText += "name2\tVn2\tgd2\tgo2\tsd2\tso2\t";
                            goto IL_0064;
                        IL_0064:
                            num = 7;
                            SchText += "HD1\t";
                            goto IL_007d;
                        IL_007d:
                            num = 8;
                            SchText += "name3\tVn3\tgd3\tg03\tsd3\tso3\t";
                            goto IL_0096;
                        IL_0096:
                            num = 9;
                            SchText += "name4\tVn4\tgd4\tgo4\tsd4\tso4\t";
                            goto IL_00b0;
                        IL_00b0:
                            num = 10;
                            SchText += "HD2\t";
                            goto IL_00ca;
                        IL_00ca:
                            num = 11;
                            SchText += "NR2\tname5\tVn5\tgd5\tgo5\tsd5\tso5\t";
                            goto IL_00e4;
                        IL_00e4:
                            num = 12;
                            SchText += "name6\tVn6\tgd6\tgo6\tsd6\tso6\t";
                            goto IL_00fe;
                        IL_00fe:
                            num = 13;
                            SchText += "HD3\t";
                            goto IL_0118;
                        IL_0118:
                            num = 14;
                            SchText += "name7\tVn7\tgd7\tgo7\tsd7\tso7\t";
                            goto IL_0132;
                        IL_0132:
                            num = 15;
                            SchText += "name8\tVn8\tgd8\tgo8\tsd8\tso8\t";
                            goto IL_014c;
                        IL_014c:
                            num = 16;
                            SchText += "HD4\t";
                            goto IL_0166;
                        IL_0166:
                            num = 17;
                            SchText += "NR3\tname9\tVn9\tgd9\tgo9\tsd9\tso9\t";
                            goto IL_0180;
                        IL_0180:
                            num = 18;
                            SchText += "name10\tVn10\tgd10\tgo10\tsd10\tso10\t";
                            goto IL_019a;
                        IL_019a:
                            num = 19;
                            SchText += "HD5\t";
                            goto IL_01b4;
                        IL_01b4:
                            num = 20;
                            SchText += "name11\tVn11\tgd11\tgo11\tsd11\tso11\t";
                            goto IL_01ce;
                        IL_01ce:
                            num = 21;
                            SchText += "name12\tVn12\tgd12\tgo12\tsd12\tso12\t";
                            goto IL_01e8;
                        IL_01e8:
                            num = 22;
                            SchText += "HD6\t";
                            goto IL_0202;
                        IL_0202:
                            num = 23;
                            SchText += "NR4\tname13\tVn13\tgd13\tgo13\tsd13\tso13\t";
                            goto IL_021c;
                        IL_021c:
                            num = 24;
                            SchText += "name14\tVn14\tgd14\tgo14\tsd14\tso14\t";
                            goto IL_0236;
                        IL_0236:
                            num = 25;
                            SchText += "HD7\t";
                            goto IL_0250;
                        IL_0250:
                            num = 26;
                            SchText += "name15\tVn15\tgd15\tgo15\tsd15\tgso15\t";
                            goto IL_026a;
                        IL_026a:
                            num = 27;
                            SchText += "name16\tVn16\tgd16\tgo16\tsd16\tso16\t";
                            goto IL_0284;
                        IL_0284:
                            num = 28;
                            SchText += "HD8\t";
                            goto IL_029e;
                        IL_029e:
                            num = 29;
                            SchText += "\n";
                            goto IL_02b8;
                        IL_02b8:
                            num = 30;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 63, 1, ref LoopForResult, ref CounterResult))
                            {
                                goto IL_02de;
                            }
                            goto IL_1a0c;
                        IL_02de:
                            num = 31;
                            Nr = "1";
                            goto IL_02ed;
                        IL_02ed:
                            num = 32;
                            left = CounterResult.AsInt();
                            goto IL_02f4;
                        IL_02f4:
                            num = 35;
                            if (left ==  0)
                            {
                                goto IL_0308;
                            }
                            goto IL_031c;
                        IL_0308:
                            num = 36;
                            Nr = "0";
                            goto IL_0de6;
                        IL_031c:
                            num = 38;
                            if (left ==  1)
                            {
                                goto IL_0330;
                            }
                            goto IL_0344;
                        IL_0330:
                            num = 39;
                            Nr = "1";
                            goto IL_0de6;
                        IL_0344:
                            num = 41;
                            if (left ==  2)
                            {
                                goto IL_0358;
                            }
                            goto IL_036c;
                        IL_0358:
                            num = 42;
                            Nr = "2";
                            goto IL_0de6;
                        IL_036c:
                            num = 44;
                            if (left ==  3)
                            {
                                goto IL_0380;
                            }
                            goto IL_0394;
                        IL_0380:
                            num = 45;
                            Nr = "3";
                            goto IL_0de6;
                        IL_0394:
                            num = 47;
                            if (left ==  4)
                            {
                                goto IL_03a8;
                            }
                            goto IL_03bc;
                        IL_03a8:
                            num = 48;
                            Nr = "16";
                            goto IL_0de6;
                        IL_03bc:
                            num = 50;
                            if (left ==  5)
                            {
                                goto IL_03d0;
                            }
                            goto IL_03e4;
                        IL_03d0:
                            num = 51;
                            Nr = "17";
                            goto IL_0de6;
                        IL_03e4:
                            num = 53;
                            if (left ==  6)
                            {
                                goto IL_03f8;
                            }
                            goto IL_040c;
                        IL_03f8:
                            num = 54;
                            Nr = "18";
                            goto IL_0de6;
                        IL_040c:
                            num = 56;
                            if (left ==  7)
                            {
                                goto IL_0420;
                            }
                            goto IL_0434;
                        IL_0420:
                            num = 57;
                            Nr = "19";
                            goto IL_0de6;
                        IL_0434:
                            num = 59;
                            if (left ==  8)
                            {
                                goto IL_0448;
                            }
                            goto IL_045c;
                        IL_0448:
                            num = 60;
                            Nr = "32";
                            goto IL_0de6;
                        IL_045c:
                            num = 62;
                            if (left ==  9)
                            {
                                goto IL_0471;
                            }
                            goto IL_0485;
                        IL_0471:
                            num = 63;
                            Nr = "33";
                            goto IL_0de6;
                        IL_0485:
                            num = 65;
                            if (left ==  10)
                            {
                                goto IL_049a;
                            }
                            goto IL_04ae;
                        IL_049a:
                            num = 66;
                            Nr = "34";
                            goto IL_0de6;
                        IL_04ae:
                            num = 68;
                            if (left ==  11)
                            {
                                goto IL_04c3;
                            }
                            goto IL_04d7;
                        IL_04c3:
                            num = 69;
                            Nr = "35";
                            goto IL_0de6;
                        IL_04d7:
                            num = 71;
                            if (left ==  12)
                            {
                                goto IL_04ec;
                            }
                            goto IL_0500;
                        IL_04ec:
                            num = 72;
                            Nr = "36";
                            goto IL_0de6;
                        IL_0500:
                            num = 74;
                            if (left ==  13)
                            {
                                goto IL_0515;
                            }
                            goto IL_0529;
                        IL_0515:
                            num = 75;
                            Nr = "37";
                            goto IL_0de6;
                        IL_0529:
                            num = 77;
                            if (left ==  14)
                            {
                                goto IL_053e;
                            }
                            goto IL_0552;
                        IL_053e:
                            num = 78;
                            Nr = "38";
                            goto IL_0de6;
                        IL_0552:
                            num = 80;
                            if (left ==  15)
                            {
                                goto IL_0567;
                            }
                            goto IL_057b;
                        IL_0567:
                            num = 81;
                            Nr = "39";
                            goto IL_0de6;
                        IL_057b:
                            num = 83;
                            if (left ==  16)
                            {
                                goto IL_0590;
                            }
                            goto IL_05a4;
                        IL_0590:
                            num = 84;
                            Nr = "4";
                            goto IL_0de6;
                        IL_05a4:
                            num = 86;
                            if (left ==  17)
                            {
                                goto IL_05b9;
                            }
                            goto IL_05cd;
                        IL_05b9:
                            num = 87;
                            Nr = "5";
                            goto IL_0de6;
                        IL_05cd:
                            num = 89;
                            if (left ==  18)
                            {
                                goto IL_05e2;
                            }
                            goto IL_05f6;
                        IL_05e2:
                            num = 90;
                            Nr = "6";
                            goto IL_0de6;
                        IL_05f6:
                            num = 92;
                            if (left ==  19)
                            {
                                goto IL_060b;
                            }
                            goto IL_061f;
                        IL_060b:
                            num = 93;
                            Nr = "7";
                            goto IL_0de6;
                        IL_061f:
                            num = 95;
                            if (left ==  20)
                            {
                                goto IL_0634;
                            }
                            goto IL_0648;
                        IL_0634:
                            num = 96;
                            Nr = "20";
                            goto IL_0de6;
                        IL_0648:
                            num = 98;
                            if (left ==  21)
                            {
                                goto IL_065d;
                            }
                            goto IL_0671;
                        IL_065d:
                            num = 99;
                            Nr = "21";
                            goto IL_0de6;
                        IL_0671:
                            num = 101;
                            if (left ==  22)
                            {
                                goto IL_0686;
                            }
                            goto IL_069a;
                        IL_0686:
                            num = 102;
                            Nr = "22";
                            goto IL_0de6;
                        IL_069a:
                            num = 104;
                            if (left ==  23)
                            {
                                goto IL_06af;
                            }
                            goto IL_06c3;
                        IL_06af:
                            num = 105;
                            Nr = "23";
                            goto IL_0de6;
                        IL_06c3:
                            num = 107;
                            if (left ==  24)
                            {
                                goto IL_06d8;
                            }
                            goto IL_06ec;
                        IL_06d8:
                            num = 108;
                            Nr = "40";
                            goto IL_0de6;
                        IL_06ec:
                            num = 110;
                            if (left ==  25)
                            {
                                goto IL_0701;
                            }
                            goto IL_0715;
                        IL_0701:
                            num = 111;
                            Nr = "41";
                            goto IL_0de6;
                        IL_0715:
                            num = 113;
                            if (left ==  26)
                            {
                                goto IL_072a;
                            }
                            goto IL_073e;
                        IL_072a:
                            num = 114;
                            Nr = "42";
                            goto IL_0de6;
                        IL_073e:
                            num = 116;
                            if (left ==  27)
                            {
                                goto IL_0753;
                            }
                            goto IL_0767;
                        IL_0753:
                            num = 117;
                            Nr = "43";
                            goto IL_0de6;
                        IL_0767:
                            num = 119;
                            if (left ==  28)
                            {
                                goto IL_077c;
                            }
                            goto IL_0790;
                        IL_077c:
                            num = 120;
                            Nr = "44";
                            goto IL_0de6;
                        IL_0790:
                            num = 122;
                            if (left ==  29)
                            {
                                goto IL_07a5;
                            }
                            goto IL_07b9;
                        IL_07a5:
                            num = 123;
                            Nr = "45";
                            goto IL_0de6;
                        IL_07b9:
                            num = 125;
                            if (left ==  30)
                            {
                                goto IL_07ce;
                            }
                            goto IL_07e2;
                        IL_07ce:
                            num = 126;
                            Nr = "46";
                            goto IL_0de6;
                        IL_07e2:
                            num = 128;
                            if (left ==  31)
                            {
                                goto IL_07fa;
                            }
                            goto IL_0811;
                        IL_07fa:
                            num = 129;
                            Nr = "47";
                            goto IL_0de6;
                        IL_0811:
                            num = 131;
                            if (left ==  32)
                            {
                                goto IL_0829;
                            }
                            goto IL_0840;
                        IL_0829:
                            num = 132;
                            Nr = "8";
                            goto IL_0de6;
                        IL_0840:
                            num = 134;
                            if (left ==  33)
                            {
                                goto IL_0858;
                            }
                            goto IL_086f;
                        IL_0858:
                            num = 135;
                            Nr = "9";
                            goto IL_0de6;
                        IL_086f:
                            num = 137;
                            if (left ==  34)
                            {
                                goto IL_0887;
                            }
                            goto IL_089e;
                        IL_0887:
                            num = 138;
                            Nr = "10";
                            goto IL_0de6;
                        IL_089e:
                            num = 140;
                            if (left ==  35)
                            {
                                goto IL_08b6;
                            }
                            goto IL_08cd;
                        IL_08b6:
                            num = 141;
                            Nr = "11";
                            goto IL_0de6;
                        IL_08cd:
                            num = 143;
                            if (left ==  36)
                            {
                                goto IL_08e5;
                            }
                            goto IL_08fc;
                        IL_08e5:
                            num = 144;
                            Nr = "24";
                            goto IL_0de6;
                        IL_08fc:
                            num = 146;
                            if (left ==  37)
                            {
                                goto IL_0914;
                            }
                            goto IL_092b;
                        IL_0914:
                            num = 147;
                            Nr = "25";
                            goto IL_0de6;
                        IL_092b:
                            num = 149;
                            if (left ==  38)
                            {
                                goto IL_0943;
                            }
                            goto IL_095a;
                        IL_0943:
                            num = 150;
                            Nr = "26";
                            goto IL_0de6;
                        IL_095a:
                            num = 152;
                            if (left ==  39)
                            {
                                goto IL_0972;
                            }
                            goto IL_0989;
                        IL_0972:
                            num = 153;
                            Nr = "27";
                            goto IL_0de6;
                        IL_0989:
                            num = 155;
                            if (left ==  40)
                            {
                                goto IL_09a1;
                            }
                            goto IL_09b8;
                        IL_09a1:
                            num = 156;
                            Nr = "48";
                            goto IL_0de6;
                        IL_09b8:
                            num = 158;
                            if (left ==  41)
                            {
                                goto IL_09d0;
                            }
                            goto IL_09e7;
                        IL_09d0:
                            num = 159;
                            Nr = "49";
                            goto IL_0de6;
                        IL_09e7:
                            num = 161;
                            if (left ==  42)
                            {
                                goto IL_09ff;
                            }
                            goto IL_0a16;
                        IL_09ff:
                            num = 162;
                            Nr = "50";
                            goto IL_0de6;
                        IL_0a16:
                            num = 164;
                            if (left ==  43)
                            {
                                goto IL_0a2e;
                            }
                            goto IL_0a45;
                        IL_0a2e:
                            num = 165;
                            Nr = "51";
                            goto IL_0de6;
                        IL_0a45:
                            num = 167;
                            if (left ==  44)
                            {
                                goto IL_0a5d;
                            }
                            goto IL_0a74;
                        IL_0a5d:
                            num = 168;
                            Nr = "52";
                            goto IL_0de6;
                        IL_0a74:
                            num = 170;
                            if (left ==  45)
                            {
                                goto IL_0a8c;
                            }
                            goto IL_0aa3;
                        IL_0a8c:
                            num = 171;
                            Nr = "53";
                            goto IL_0de6;
                        IL_0aa3:
                            num = 173;
                            if (left ==  46)
                            {
                                goto IL_0abb;
                            }
                            goto IL_0ad2;
                        IL_0abb:
                            num = 174;
                            Nr = "54";
                            goto IL_0de6;
                        IL_0ad2:
                            num = 176;
                            if (left ==  47)
                            {
                                goto IL_0aea;
                            }
                            goto IL_0b01;
                        IL_0aea:
                            num = 177;
                            Nr = "55";
                            goto IL_0de6;
                        IL_0b01:
                            num = 179;
                            if (left ==  48)
                            {
                                goto IL_0b19;
                            }
                            goto IL_0b30;
                        IL_0b19:
                            num = 180;
                            Nr = "12";
                            goto IL_0de6;
                        IL_0b30:
                            num = 182;
                            if (left ==  49)
                            {
                                goto IL_0b48;
                            }
                            goto IL_0b5f;
                        IL_0b48:
                            num = 183;
                            Nr = "13";
                            goto IL_0de6;
                        IL_0b5f:
                            num = 185;
                            if (left ==  50)
                            {
                                goto IL_0b77;
                            }
                            goto IL_0b8e;
                        IL_0b77:
                            num = 186;
                            Nr = "14";
                            goto IL_0de6;
                        IL_0b8e:
                            num = 188;
                            if (left ==  51)
                            {
                                goto IL_0ba6;
                            }
                            goto IL_0bbd;
                        IL_0ba6:
                            num = 189;
                            Nr = "15";
                            goto IL_0de6;
                        IL_0bbd:
                            num = 191;
                            if (left ==  52)
                            {
                                goto IL_0bd5;
                            }
                            goto IL_0bec;
                        IL_0bd5:
                            num = 192;
                            Nr = "28";
                            goto IL_0de6;
                        IL_0bec:
                            num = 194;
                            if (left ==  53)
                            {
                                goto IL_0c04;
                            }
                            goto IL_0c1b;
                        IL_0c04:
                            num = 195;
                            Nr = "29";
                            goto IL_0de6;
                        IL_0c1b:
                            num = 197;
                            if (left ==  54)
                            {
                                goto IL_0c33;
                            }
                            goto IL_0c4a;
                        IL_0c33:
                            num = 198;
                            Nr = "30";
                            goto IL_0de6;
                        IL_0c4a:
                            num = 200;
                            if (left ==  55)
                            {
                                goto IL_0c62;
                            }
                            goto IL_0c79;
                        IL_0c62:
                            num = 201;
                            Nr = "31";
                            goto IL_0de6;
                        IL_0c79:
                            num = 203;
                            if (left ==  56)
                            {
                                goto IL_0c91;
                            }
                            goto IL_0ca8;
                        IL_0c91:
                            num = 204;
                            Nr = "56";
                            goto IL_0de6;
                        IL_0ca8:
                            num = 206;
                            if (left ==  57)
                            {
                                goto IL_0cc0;
                            }
                            goto IL_0cd7;
                        IL_0cc0:
                            num = 207;
                            Nr = "57";
                            goto IL_0de6;
                        IL_0cd7:
                            num = 209;
                            if (left ==  58)
                            {
                                goto IL_0cef;
                            }
                            goto IL_0d06;
                        IL_0cef:
                            num = 210;
                            Nr = "58";
                            goto IL_0de6;
                        IL_0d06:
                            num = 212;
                            if (left ==  59)
                            {
                                goto IL_0d1e;
                            }
                            goto IL_0d35;
                        IL_0d1e:
                            num = 213;
                            Nr = "59";
                            goto IL_0de6;
                        IL_0d35:
                            num = 215;
                            if (left ==  60)
                            {
                                goto IL_0d4d;
                            }
                            goto IL_0d64;
                        IL_0d4d:
                            num = 216;
                            Nr = "60";
                            goto IL_0de6;
                        IL_0d64:
                            num = 218;
                            if (left ==  61)
                            {
                                goto IL_0d7c;
                            }
                            goto IL_0d90;
                        IL_0d7c:
                            num = 219;
                            Nr = "61";
                            goto IL_0de6;
                        IL_0d90:
                            num = 221;
                            if (left ==  62)
                            {
                                goto IL_0da8;
                            }
                            goto IL_0dbc;
                        IL_0da8:
                            num = 222;
                            Nr = "62";
                            goto IL_0de6;
                        IL_0dbc:
                            num = 224;
                            if (left ==  63)
                            {
                                goto IL_0dd4;
                            }
                            goto IL_0de6;
                        IL_0dd4:
                            num = 225;
                            Nr = "63";
                            goto IL_0de6;
                        IL_0de6:
                            num = 227;
                            Text1 = " ";
                            goto IL_0df8;
                        IL_0df8:
                            num = 228;
                            AT6perles(ref Famdat);
                            goto IL_0e0b;
                        IL_0e0b:
                            num = 229;
                            nr = Nr;
                            goto IL_0e1a;
                        IL_0e1a:
                            num = 232;
                            switch (nr)
                            {
                                case "0":
                                case "4":
                                case "8":
                                case "12":
                                    break;
                                default:
                                    goto IL_0f6b;
                            }
                            goto IL_0e68;
                        IL_0f6b:
                            num = 247;
                            switch (nr)
                            {
                                case "1":
                                case "5":
                                case "9":
                                case "13":
                                    break;
                                default:
                                    goto IL_0ffc;
                            }
                            goto IL_0fb6;
                        IL_0ffc:
                            num = 251;
                            switch (nr)
                            {
                                case "2":
                                case "6":
                                case "10":
                                case "14":
                                    break;
                                default:
                                    goto IL_106a;
                            }
                            goto IL_1047;
                        IL_106a:
                            num = 254;
                            switch (nr)
                            {
                                case "3":
                                case "7":
                                case "11":
                                case "15":
                                    break;
                                default:
                                    goto IL_10fb;
                            }
                            goto IL_10b5;
                        IL_10fb:
                            num = 258;
                            switch (nr)
                            {
                                case "16":
                                case "20":
                                case "24":
                                case "28":
                                    break;
                                default:
                                    goto IL_124c;
                            }
                            goto IL_1149;
                        IL_124c:
                            num = 273;
                            if (nr ==  "17" || nr ==  "21" || nr ==  25.AsString() || nr ==  "29")
                            {
                                goto IL_1299;
                            }
                            goto IL_12df;
                        IL_0f48:
                            num = 245;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_19f6:
                            num = 342;
                            if (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                            {
                                goto IL_02de;
                            }
                            goto IL_1a0c;
                        IL_1a0c:
                            num = 343;
                            SchText += "\t\t\t\t\t\t\t\t\t\t\t\t";
                            goto IL_1a29;
                        IL_12df:
                            num = 277;
                            if (nr ==  "18" || nr ==  "22" || nr ==  26.AsString() || nr ==  "30")
                            {
                                goto IL_132c;
                            }
                            goto IL_134f;
                        IL_1a29:
                            num = 344;
                            SchText += "\t";
                            goto IL_1a46;
                        IL_1a46:
                            num = 345;
                            lErl = 44;
                            goto IL_1a51;
                        IL_1a51:
                            num = 346;
                            Frame1.Visible = false;
                            goto IL_1a64;
                        IL_134f:
                            num = 280;
                            if (nr ==  "19" || nr ==  "23" || nr ==  27.AsString() || nr ==  "31")
                            {
                                goto IL_139c;
                            }
                            goto IL_13e2;
                        IL_1a64:
                            num = 347;
                            FileSystem.PrintLine(99, SchText);
                            goto IL_1a86;
                        IL_1a86:
                            num = 348;
                            FileSystem.FileClose(99);
                            goto IL_1aa2;
                        IL_1aa2:
                            num = 349;
                            Command1[0].Enabled = true;
                            goto IL_1abb;
                        IL_13e2:
                            num = 284;
                            switch (nr)
                            {
                                case "32":
                                case "40":
                                case "48":
                                case "56":
                                    break;
                                default:
                                    goto IL_1533;
                            }
                            goto IL_1430;
                        IL_1533:
                            num = 299;
                            if (nr ==  "33" || nr ==  "41" || nr ==  49.AsString() || nr ==  "57")
                            {
                                goto IL_1580;
                            }
                            goto IL_15c6;
                        IL_1abb:
                            num = 350;
                            Msg = "Die Steuerdatei wurde gespeichert.";
                            goto IL_1acd;
                        IL_1acd:
                            num = 351;
                            Msg += "\nMit Klick auf Drucken wird der Ausdruck erstellt.";
                            goto IL_1aea;
                        IL_1aea:
                            num = 352;
                            Interaction.MsgBox(Msg);
                            goto end_IL_0000_2;
                        IL_15c6:
                            num = 303;
                            switch (nr)
                            {
                                case "34":
                                case "42":
                                case "50":
                                case "58":
                                    break;
                                default:
                                    goto IL_1634;
                            }
                            goto IL_1611;
                        IL_1634:
                            num = 306;
                            switch (nr)
                            {
                                case "35":
                                case "43":
                                case "51":
                                case "59":
                                    break;
                                default:
                                    goto IL_16c5;
                            }
                            goto IL_167f;
                        IL_16c5:
                            num = 310;
                            switch (nr)
                            {
                                case "36":
                                case "44":
                                case "52":
                                case "60":
                                    break;
                                default:
                                    goto IL_1816;
                            }
                            goto IL_1713;
                        IL_1816:
                            num = 325;
                            switch (nr)
                            {
                                case "37":
                                case "45":
                                case "53":
                                case "61":
                                    break;
                                default:
                                    goto IL_18a7;
                            }
                            goto IL_1861;
                        IL_18a7:
                            num = 329;
                            switch (nr)
                            {
                                case "38":
                                case "46":
                                case "54":
                                case "62":
                                    break;
                                default:
                                    goto IL_1915;
                            }
                            goto IL_18f2;
                        IL_1915:
                            num = 332;
                            switch (nr)
                            {
                                case "39":
                                case "47":
                                case "55":
                                    break;
                                default:
                                    goto IL_1991;
                            }
                            goto IL_194e;
                        IL_1991:
                            num = 336;
                            if (nr ==  "63")
                            {
                                goto IL_19a8;
                            }
                            goto IL_19f6;
                        IL_19a8:
                            num = 337;
                            SchText += Text1;
                            goto IL_19c6;
                        IL_19c6:
                            num = 338;
                            SchText = SchText + Famdat + "\t";
                            goto IL_1a46;
                        IL_194e:
                            num = 333;
                            SchText += Text1;
                            goto IL_196c;
                        IL_196c:
                            num = 334;
                            SchText = SchText + Famdat + "\t\n";
                            goto IL_19f6;
                        IL_18f2:
                            num = 330;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_1861:
                            num = 326;
                            SchText += Text1;
                            goto IL_187f;
                        IL_187f:
                            num = 327;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_1713:
                            num = 311;
                            if (Nr ==  "36")
                            {
                                goto IL_172e;
                            }
                            goto IL_174b;
                        IL_172e:
                            num = 312;
                            SchText += "Ahnenziffern 36; 37; 38; 39\t";
                            goto IL_174b;
                        IL_174b:
                            num = 314;
                            if (Nr ==  "44")
                            {
                                goto IL_1766;
                            }
                            goto IL_1783;
                        IL_1766:
                            num = 315;
                            SchText += "Ahnenziffern 44; 45; 46; 47\t";
                            goto IL_1783;
                        IL_1783:
                            num = 317;
                            if (Nr ==  "52")
                            {
                                goto IL_179e;
                            }
                            goto IL_17bb;
                        IL_179e:
                            num = 318;
                            SchText += "Ahnenziffern 52; 53; 54; 55\t";
                            goto IL_17bb;
                        IL_17bb:
                            num = 320;
                            if (Nr ==  "60")
                            {
                                goto IL_17d6;
                            }
                            goto IL_17f3;
                        IL_17d6:
                            num = 321;
                            SchText += "Ahnenziffern 60; 61; 62; 63\t";
                            goto IL_17f3;
                        IL_17f3:
                            num = 323;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_167f:
                            num = 307;
                            SchText += Text1;
                            goto IL_169d;
                        IL_169d:
                            num = 308;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_1611:
                            num = 304;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_1580:
                            num = 300;
                            SchText += Text1;
                            goto IL_159e;
                        IL_159e:
                            num = 301;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_1430:
                            num = 285;
                            if (Nr ==  "32")
                            {
                                goto IL_144b;
                            }
                            goto IL_1468;
                        IL_144b:
                            num = 286;
                            SchText += "Ahnenziffern 32; 33; 34; 35\t";
                            goto IL_1468;
                        IL_1468:
                            num = 288;
                            if (Nr ==  "40")
                            {
                                goto IL_1483;
                            }
                            goto IL_14a0;
                        IL_1483:
                            num = 289;
                            SchText += "Ahnenziffern 40; 41; 42; 43\t";
                            goto IL_14a0;
                        IL_14a0:
                            num = 291;
                            if (Nr ==  "48")
                            {
                                goto IL_14bb;
                            }
                            goto IL_14d8;
                        IL_14bb:
                            num = 292;
                            SchText += "Ahnenziffern 48; 49; 50; 51\t";
                            goto IL_14d8;
                        IL_14d8:
                            num = 294;
                            if (Nr ==  "56")
                            {
                                goto IL_14f3;
                            }
                            goto IL_1510;
                        IL_14f3:
                            num = 295;
                            SchText += "Ahnenziffern 56; 57; 58; 59\t";
                            goto IL_1510;
                        IL_1510:
                            num = 297;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_139c:
                            num = 281;
                            SchText += Text1;
                            goto IL_13ba;
                        IL_13ba:
                            num = 282;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_132c:
                            num = 278;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_1299:
                            num = 274;
                            SchText += Text1;
                            goto IL_12b7;
                        IL_12b7:
                            num = 275;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_1149:
                            num = 259;
                            if (Nr ==  "16")
                            {
                                goto IL_1164;
                            }
                            goto IL_1181;
                        IL_1164:
                            num = 260;
                            SchText += "Ahnenziffern 16; 17; 18; 19\t";
                            goto IL_1181;
                        IL_1181:
                            num = 262;
                            if (Nr ==  "20")
                            {
                                goto IL_119c;
                            }
                            goto IL_11b9;
                        IL_119c:
                            num = 263;
                            SchText += "Ahnenziffern 20; 21; 22; 23\t";
                            goto IL_11b9;
                        IL_11b9:
                            num = 265;
                            if (Nr ==  "24")
                            {
                                goto IL_11d4;
                            }
                            goto IL_11f1;
                        IL_11d4:
                            num = 266;
                            SchText += "Ahnenziffern 24; 25; 26; 27\t";
                            goto IL_11f1;
                        IL_11f1:
                            num = 268;
                            if (Nr ==  "28")
                            {
                                goto IL_120c;
                            }
                            goto IL_1229;
                        IL_120c:
                            num = 269;
                            SchText += "Ahnenziffern 28; 29; 30; 31\t";
                            goto IL_1229;
                        IL_1229:
                            num = 271;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_10b5:
                            num = 255;
                            SchText += Text1;
                            goto IL_10d3;
                        IL_10d3:
                            num = 256;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_1047:
                            num = 252;
                            SchText += Text1;
                            goto IL_19f6;
                        IL_0fb6:
                            num = 248;
                            SchText += Text1;
                            goto IL_0fd4;
                        IL_0fd4:
                            num = 249;
                            SchText = SchText + Famdat + "\t";
                            goto IL_19f6;
                        IL_0e68:
                            num = 233;
                            if (Nr ==  "0")
                            {
                                goto IL_0e83;
                            }
                            goto IL_0ea0;
                        IL_0e83:
                            num = 234;
                            SchText += "Ahnenziffern 0; 1; 2; 3\t";
                            goto IL_0ea0;
                        IL_0ea0:
                            num = 236;
                            if (Nr ==  "4")
                            {
                                goto IL_0ebb;
                            }
                            goto IL_0ed8;
                        IL_0ebb:
                            num = 237;
                            SchText += "Ahnenziffern 4; 5; 6; 7\t";
                            goto IL_0ed8;
                        end_IL_0000:
                            break;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 8511;
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

        private void Command1_Click(object eventSender, EventArgs eventArgs)
        {
            switch (Command1.GetIndex((Button)eventSender))
            {
                case 0:
                    if (_Modul1.Instance.Aschalt == 4f)
                    {
                        Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\AT4_1.doc", AppWinStyle.MaximizedFocus);
                    }
                    Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Init\\RKN-28082.doc", AppWinStyle.MaximizedFocus);
                    break;
                case 1:
                    Close();
                    break;
            }
        }

        private void AT6_Load(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            int lErl = default;
            int num5 = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                    int num6;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            BackColor = _Modul1.Instance.HintFarb;
                            goto IL_0013;
                        case 2135:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0699;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 75)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0699;
                                }
                                if (Information.Err().Number == 91)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0699;
                                }
                                if (Information.Err().Number == 3420)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0699;
                                }
                                if (Information.Err().Number == 3021)
                                {
                                    goto end_IL_0000_2;
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
                                goto IL_069d;
                            }
                        end_IL_0000:
                            break;
                        IL_0013:
                            num = 2;
                            _Modul1.Instance.Dateienopen();
                            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<Enum>("Windowstate");
                            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
                            ProjectData.ClearProjectError();
                            num3 = 2;
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
                            goto IL_026a;
                        IL_026a: // <========== 12
                            num = 53;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            FileSystem.FileClose(6);
                            Show();
                            num6 = num5;
                            num5 = 0;
                            FileSystem.FileClose(30);
                            Label1[3].Text = "Keine Berechnung vorhanden";
                            Label1[0].Text = "Sie müssen erst die Ahnen berechnen.";
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
                            goto IL_04d2;
                        IL_04d2: // <========== 3
                            num = 81;
                            lErl = 1;
                            Label1[5].Text = _Modul1.Instance.IText[3] + " " + _Modul1.Instance.Kont[11];
                            Label1[2].Text = _Modul1.Instance.IText[5] + " " + _Modul1.Instance.Kont[13];
                            Label1[1].Text = _Modul1.Instance.IText[4] + " " + _Modul1.Instance.Kont[12];
                            Label1[4].Text = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_0699: // <========== 4
                            num4 = num2 + 1;
                            goto IL_069d;
                        IL_069d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 21:
                                case 25:
                                case 28:
                                case 31:
                                case 34:
                                case 37:
                                case 40:
                                case 43:
                                case 46:
                                case 49:
                                case 52:
                                case 53:
                                    goto IL_026a;
                                case 65:
                                case 68:
                                case 81:
                                    goto IL_04d2;
                                case 87:
                                case 101:
                                case 107:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 2135;
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
        public void AT6perles(ref string Famdat)
        {
            string text = " ";
            string text3 = "";
            string text5 = "";
            string text6 = " ";
            string text7 = " ";
            string text8 = " ";
            DataModul.NB_AhnTable.Index = "Ahnen";
            DataModul.NB_AhnTable.Seek("=", new string(' ', 40) + Nr.Right( 40));
            checked
            {
                if (!DataModul.NB_AhnTable.NoMatch)
                {
                    _Modul1.Instance.PersInArb = DataModul.NB_AhnTable.Fields["PerNr"].AsInt();
                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                    _Modul1.Instance.Person.SetFullSurname(_Modul1.Instance.Person_FullSurname(_Modul1.Instance.Person, false));
                    text6 = _Modul1.Instance.Person.Givennames;
                    text = _Modul1.Instance.Person.FullSurName;
                    _Modul1.Instance.Schalt = 200;
                    byte PerPos = 0;
                    _Modul1.Instance.Datles(_Modul1.Instance.PersInArb, out var asPersDates);
                    text5 = _Modul1.Instance.Kont[21];
                    if (text5 ==  "")
                    {
                        text5 = _Modul1.Instance.Kont[22];
                    }
                    text3 = _Modul1.Instance.Kont[23];
                    if (text3 ==  "")
                    {
                        text3 = _Modul1.Instance.Kont[24];
                    }
                    string text4 = _Modul1.Instance.Kont[11];
                    if (text4 ==  "")
                    {
                        text4 = _Modul1.Instance.Kont[12];
                    }
                    text7 = Strings.Mid(text4, text5.Length + 1, text4.Length);
                    string text2 = _Modul1.Instance.Kont[13];
                    if (text2 ==  "")
                    {
                        text2 = _Modul1.Instance.Kont[14];
                    }
                    text8 = Strings.Mid(text2, text3.Length + 1, text2.Length);
                    _Modul1.Instance.FamInArb = DataModul.NB_AhnTable.Fields["Ehe"].AsInt();
                    _Modul1.Instance.Schalt = 6;
                    _Modul1.Instance.Famdatles1(0,out var asFamDate);
                    Famdat = asFamDate[2];
                    if (Famdat.Trim() ==  "")
                    {
                        Famdat = asFamDate[3];
                    }
                }
                Text1 = text + "\t";
                Text1 = Text1 + text6 + "\t";
                Text1 = Text1 + text5 + "\t" + text7 + "\t";
                Text1 = Text1 + text3 + "\t" + text8 + "\t";
            }
        }
    }
}
