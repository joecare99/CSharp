using Druck.My;
using GenFree;
using GenFree.Helper;
using GenFree.Data;
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
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using BaseLib.Helper;
using System.Collections.Generic;

namespace Druck
{
    [DesignerGenerated]
    internal class Ausw : Form
    {
        private IContainer components;

        public ToolTip ToolTip1;

        [AccessedThroughProperty("_Command1_2")]
        private Button __Command1_2;

        [AccessedThroughProperty("Command2")]
        private Button _Command2;

        [AccessedThroughProperty("Text1")]
        private TextBox _Text1;

        [AccessedThroughProperty("List2")]
        private ListBox _List2;

        [AccessedThroughProperty("List1")]
        private ListBox _List1;

        [AccessedThroughProperty("_Command1_1")]
        private Button __Command1_1;

        [AccessedThroughProperty("_Command1_0")]
        private Button __Command1_0;

        [AccessedThroughProperty("Label4")]
        private Label _Label4;

        [AccessedThroughProperty("Label3")]
        private Label _Label3;

        [AccessedThroughProperty("Frame3")]
        private GroupBox _Frame3;

        [AccessedThroughProperty("_Befehl1_32")]
        private Button __Befehl1_32;

        [AccessedThroughProperty("_Befehl1_31")]
        private Button __Befehl1_31;

        [AccessedThroughProperty("_Befehl1_30")]
        private Button __Befehl1_30;

        [AccessedThroughProperty("_Befehl1_29")]
        private Button __Befehl1_29;

        [AccessedThroughProperty("_Befehl1_10")]
        private Button __Befehl1_10;

        [AccessedThroughProperty("_Befehl1_12")]
        private Button __Befehl1_12;

        [AccessedThroughProperty("_Befehl1_11")]
        private Button __Befehl1_11;

        [AccessedThroughProperty("_Befehl1_2")]
        private Button __Befehl1_2;

        [AccessedThroughProperty("_Befehl1_1")]
        private Button __Befehl1_1;

        [AccessedThroughProperty("_Befehl1_14")]
        private Button __Befehl1_14;

        [AccessedThroughProperty("_Befehl1_13")]
        private Button __Befehl1_13;

        [AccessedThroughProperty("_Befehl1_5")]
        private Button __Befehl1_5;

        [AccessedThroughProperty("_Befehl1_6")]
        private Button __Befehl1_6;

        [AccessedThroughProperty("_Befehl1_4")]
        private Button __Befehl1_4;

        [AccessedThroughProperty("_Befehl1_3")]
        private Button __Befehl1_3;

        [AccessedThroughProperty("_Befehl1_0")]
        private Button __Befehl1_0;

        [AccessedThroughProperty("_Befehl1_7")]
        private Button __Befehl1_7;

        [AccessedThroughProperty("_Befehl1_8")]
        private Button __Befehl1_8;

        [AccessedThroughProperty("_Befehl1_9")]
        private Button __Befehl1_9;

        [AccessedThroughProperty("_Befehl1_15")]
        private Button __Befehl1_15;

        [AccessedThroughProperty("_Befehl1_17")]
        private Button __Befehl1_17;

        [AccessedThroughProperty("_Befehl1_18")]
        private Button __Befehl1_18;

        [AccessedThroughProperty("_Befehl1_19")]
        private Button __Befehl1_19;

        [AccessedThroughProperty("_Befehl1_22")]
        private Button __Befehl1_22;

        [AccessedThroughProperty("_Befehl1_23")]
        private Button __Befehl1_23;

        [AccessedThroughProperty("_Befehl1_24")]
        private Button __Befehl1_24;

        [AccessedThroughProperty("_Befehl1_25")]
        private Button __Befehl1_25;

        [AccessedThroughProperty("_Befehl1_28")]
        private Button __Befehl1_28;

        [AccessedThroughProperty("Label5")]
        private Label _Label5;

        [AccessedThroughProperty("Bezeichnung3")]
        private Label _Bezeichnung3;

        [AccessedThroughProperty("Bezeichnung2")]
        private Label _Bezeichnung2;

        [AccessedThroughProperty("Bezeichnung1")]
        private Label _Bezeichnung1;

        [AccessedThroughProperty("_Label1_0")]
        private Label __Label1_0;

        [AccessedThroughProperty("Frame1")]
        private GroupBox _Frame1;

        [AccessedThroughProperty("_Kontroll_11")]
        private CheckBox __Kontroll_11;

        [AccessedThroughProperty("_Kontroll_10")]
        private CheckBox __Kontroll_10;

        [AccessedThroughProperty("_Kontroll_8")]
        private CheckBox __Kontroll_8;

        [AccessedThroughProperty("_Kontroll_7")]
        private CheckBox __Kontroll_7;

        [AccessedThroughProperty("_Kontroll_5")]
        private CheckBox __Kontroll_5;

        [AccessedThroughProperty("_Befehl1_26")]
        private Button __Befehl1_26;

        [AccessedThroughProperty("_Kontroll_0")]
        private CheckBox __Kontroll_0;

        [AccessedThroughProperty("_Befehl1_21")]
        private Button __Befehl1_21;

        [AccessedThroughProperty("_Befehl1_16")]
        private Button __Befehl1_16;

        [AccessedThroughProperty("_Befehl1_20")]
        private Button __Befehl1_20;

        [AccessedThroughProperty("bereich2")]
        private TextBox _bereich2;

        [AccessedThroughProperty("Bereich1")]
        private TextBox _Bereich1;

        [AccessedThroughProperty("bereich3")]
        private TextBox _bereich3;

        [AccessedThroughProperty("bereich4")]
        private TextBox _bereich4;

        [AccessedThroughProperty("Option1")]
        private RadioButton _Option1;

        [AccessedThroughProperty("Option2")]
        private RadioButton _Option2;

        [AccessedThroughProperty("_Kontroll_1")]
        private CheckBox __Kontroll_1;

        [AccessedThroughProperty("_Kontroll_3")]
        private CheckBox __Kontroll_3;

        [AccessedThroughProperty("_Kontroll_2")]
        private CheckBox __Kontroll_2;

        [AccessedThroughProperty("_Kontroll_12")]
        private CheckBox __Kontroll_12;

        [AccessedThroughProperty("_Option3_1")]
        private RadioButton __Option3_1;

        [AccessedThroughProperty("_Option3_0")]
        private RadioButton __Option3_0;

        [AccessedThroughProperty("Frame2")]
        private Panel _Frame2;

        [AccessedThroughProperty("_Kontroll_4")]
        private CheckBox __Kontroll_4;

        [AccessedThroughProperty("_Label2_2")]
        private Label __Label2_2;

        [AccessedThroughProperty("_Label2_1")]
        private Label __Label2_1;

        [AccessedThroughProperty("_Label2_0")]
        private Label __Label2_0;

        [AccessedThroughProperty("_Bez_5")]
        private Label __Bez_5;

        [AccessedThroughProperty("Bezeichnung4")]
        private Label _Bezeichnung4;

        [AccessedThroughProperty("Bezeichnung6")]
        private Label _Bezeichnung6;

        [AccessedThroughProperty("Bezeichnung5")]
        private Label _Bezeichnung5;

        [AccessedThroughProperty("_Bez_1")]
        private Label __Bez_1;

        [AccessedThroughProperty("_Bez_2")]
        private Label __Bez_2;

        [AccessedThroughProperty("_Bez_3")]
        private Label __Bez_3;

        [AccessedThroughProperty("Befehl1")]
        private ControlArray<Button> _Befehl1;

        [AccessedThroughProperty("Bez")]
        private ControlArray<Label> _Bez;

        [AccessedThroughProperty("Command1")]
        private ControlArray<Button> _Command1;

        [AccessedThroughProperty("Kontroll")]
        private ControlArray<CheckBox> _Kontroll;

        [AccessedThroughProperty("Label1")]
        private ControlArray<Label> _Label1;

        [AccessedThroughProperty("Label2")]
        private ControlArray<Label> _Label2;

        [AccessedThroughProperty("Option3")]
        private ControlArray<RadioButton> _Option3;

        [AccessedThroughProperty("CheckBox6")]
        private CheckBox _CheckBox6;

        [AccessedThroughProperty("CheckBox5")]
        private CheckBox _CheckBox5;

        [AccessedThroughProperty("CheckBox4")]
        private CheckBox _CheckBox4;

        [AccessedThroughProperty("CheckBox3")]
        private CheckBox _CheckBox3;

        [AccessedThroughProperty("CheckBox2")]
        private CheckBox _CheckBox2;

        [AccessedThroughProperty("CheckBox1")]
        private CheckBox _CheckBox1;

        [AccessedThroughProperty("Panel1")]
        private Panel _Panel1;

        [AccessedThroughProperty("Label7")]
        private Label _Label7;

        [AccessedThroughProperty("RadioButton3")]
        private RadioButton _RadioButton3;

        [AccessedThroughProperty("RadioButton1")]
        private RadioButton _RadioButton1;

        [AccessedThroughProperty("RadioButton2")]
        private RadioButton _RadioButton2;

        [AccessedThroughProperty("Button1")]
        private Button _Button1;

        [AccessedThroughProperty("RichTextBox1")]
        private RichTextBox _RichTextBox1;

        [AccessedThroughProperty("GroupBox1")]
        private GroupBox _GroupBox1;

        [AccessedThroughProperty("Button3")]
        private Button _Button3;

        [AccessedThroughProperty("Button2")]
        private Button _Button2;

        [AccessedThroughProperty("Label6")]
        private Label _Label6;

        [AccessedThroughProperty("Label8")]
        private Label _Label8;

        [AccessedThroughProperty("Button4")]
        private Button _Button4;

        [AccessedThroughProperty("Button5")]
        private Button _Button5;

        [AccessedThroughProperty("Label9")]
        private Label _Label9;

        [AccessedThroughProperty("Button6")]
        private Button _Button6;

        [AccessedThroughProperty("Button7")]
        private Button _Button7;

        [AccessedThroughProperty("CheckBox7")]
        private CheckBox _CheckBox7;

        [AccessedThroughProperty("CheckBox8")]
        private CheckBox _CheckBox8;

        [AccessedThroughProperty("Button8")]
        private Button _Button8;

        private string Bezeich;
        private int iM_Priv_aus;
        private int M_Ubg1; // Local variable for range calculation
        private int M_Start; // Local variable for start calculation

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

        public virtual Button Command2
        {
            get
            {
                return _Command2;
            }

            set
            {
                EventHandler value2 = Command2_Click;
                if (_Command2 != null)
                {
                    _Command2.Click -= value2;
                }
                _Command2 = value;
                if (_Command2 != null)
                {
                    _Command2.Click += value2;
                }
            }
        }

        public virtual TextBox Text1
        {
            get
            {
                return _Text1;
            }

            set
            {
                _Text1 = value;
            }
        }

        public virtual ListBox List2
        {
            get
            {
                return _List2;
            }

            set
            {
                EventHandler value2 = List2_DoubleClick;
                if (_List2 != null)
                {
                    _List2.DoubleClick -= value2;
                }
                _List2 = value;
                if (_List2 != null)
                {
                    _List2.DoubleClick += value2;
                }
            }
        }

        public virtual ListBox List1
        {
            get
            {
                return _List1;
            }

            set
            {
                EventHandler value2 = List1_DoubleClick;
                if (_List1 != null)
                {
                    _List1.DoubleClick -= value2;
                }
                _List1 = value;
                if (_List1 != null)
                {
                    _List1.DoubleClick += value2;
                }
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

        public virtual Label Label4
        {
            get
            {
                return _Label4;
            }

            set
            {
                _Label4 = value;
            }
        }

        public virtual Label Label3
        {
            get
            {
                return _Label3;
            }

            set
            {
                _Label3 = value;
            }
        }

        public virtual GroupBox Frame3
        {
            get
            {
                return _Frame3;
            }

            set
            {
                _Frame3 = value;
            }
        }

        public virtual Button _Befehl1_32
        {
            get
            {
                return __Befehl1_32;
            }

            set
            {
                __Befehl1_32 = value;
            }
        }

        public virtual Button _Befehl1_31
        {
            get
            {
                return __Befehl1_31;
            }

            set
            {
                __Befehl1_31 = value;
            }
        }

        public virtual Button _Befehl1_30
        {
            get
            {
                return __Befehl1_30;
            }

            set
            {
                __Befehl1_30 = value;
            }
        }

        public virtual Button _Befehl1_29
        {
            get
            {
                return __Befehl1_29;
            }

            set
            {
                __Befehl1_29 = value;
            }
        }

        public virtual Button _Befehl1_10
        {
            get
            {
                return __Befehl1_10;
            }

            set
            {
                __Befehl1_10 = value;
            }
        }

        public virtual Button _Befehl1_12
        {
            get
            {
                return __Befehl1_12;
            }

            set
            {
                __Befehl1_12 = value;
            }
        }

        public virtual Button _Befehl1_11
        {
            get
            {
                return __Befehl1_11;
            }

            set
            {
                __Befehl1_11 = value;
            }
        }

        public virtual Button _Befehl1_2
        {
            get
            {
                return __Befehl1_2;
            }

            set
            {
                __Befehl1_2 = value;
            }
        }

        public virtual Button _Befehl1_1
        {
            get
            {
                return __Befehl1_1;
            }

            set
            {
                __Befehl1_1 = value;
            }
        }

        public virtual Button _Befehl1_14
        {
            get
            {
                return __Befehl1_14;
            }

            set
            {
                __Befehl1_14 = value;
            }
        }

        public virtual Button _Befehl1_13
        {
            get
            {
                return __Befehl1_13;
            }

            set
            {
                __Befehl1_13 = value;
            }
        }

        public virtual Button _Befehl1_5
        {
            get
            {
                return __Befehl1_5;
            }

            set
            {
                __Befehl1_5 = value;
            }
        }

        public virtual Button _Befehl1_6
        {
            get
            {
                return __Befehl1_6;
            }

            set
            {
                __Befehl1_6 = value;
            }
        }

        public virtual Button _Befehl1_4
        {
            get
            {
                return __Befehl1_4;
            }

            set
            {
                __Befehl1_4 = value;
            }
        }

        public virtual Button _Befehl1_3
        {
            get
            {
                return __Befehl1_3;
            }

            set
            {
                __Befehl1_3 = value;
            }
        }

        public virtual Button _Befehl1_0
        {
            get
            {
                return __Befehl1_0;
            }

            set
            {
                __Befehl1_0 = value;
            }
        }

        public virtual Button _Befehl1_7
        {
            get
            {
                return __Befehl1_7;
            }

            set
            {
                __Befehl1_7 = value;
            }
        }

        public virtual Button _Befehl1_8
        {
            get
            {
                return __Befehl1_8;
            }

            set
            {
                __Befehl1_8 = value;
            }
        }

        public virtual Button _Befehl1_9
        {
            get
            {
                return __Befehl1_9;
            }

            set
            {
                __Befehl1_9 = value;
            }
        }

        public virtual Button _Befehl1_15
        {
            get
            {
                return __Befehl1_15;
            }

            set
            {
                __Befehl1_15 = value;
            }
        }

        public virtual Button _Befehl1_17
        {
            get
            {
                return __Befehl1_17;
            }

            set
            {
                __Befehl1_17 = value;
            }
        }

        public virtual Button _Befehl1_18
        {
            get
            {
                return __Befehl1_18;
            }

            set
            {
                __Befehl1_18 = value;
            }
        }

        public virtual Button _Befehl1_19
        {
            get
            {
                return __Befehl1_19;
            }

            set
            {
                __Befehl1_19 = value;
            }
        }

        public virtual Button _Befehl1_22
        {
            get
            {
                return __Befehl1_22;
            }

            set
            {
                __Befehl1_22 = value;
            }
        }

        public virtual Button _Befehl1_23
        {
            get
            {
                return __Befehl1_23;
            }

            set
            {
                __Befehl1_23 = value;
            }
        }

        public virtual Button _Befehl1_24
        {
            get
            {
                return __Befehl1_24;
            }

            set
            {
                __Befehl1_24 = value;
            }
        }

        public virtual Button _Befehl1_25
        {
            get
            {
                return __Befehl1_25;
            }

            set
            {
                __Befehl1_25 = value;
            }
        }

        public virtual Button _Befehl1_28
        {
            get
            {
                return __Befehl1_28;
            }

            set
            {
                __Befehl1_28 = value;
            }
        }

        public virtual Label Label5
        {
            get
            {
                return _Label5;
            }

            set
            {
                _Label5 = value;
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

        public virtual CheckBox _Kontroll_11
        {
            get
            {
                return __Kontroll_11;
            }

            set
            {
                __Kontroll_11 = value;
            }
        }

        public virtual CheckBox _Kontroll_10
        {
            get
            {
                return __Kontroll_10;
            }

            set
            {
                __Kontroll_10 = value;
            }
        }

        public virtual CheckBox _Kontroll_8
        {
            get
            {
                return __Kontroll_8;
            }

            set
            {
                __Kontroll_8 = value;
            }
        }

        public virtual CheckBox _Kontroll_7
        {
            get
            {
                return __Kontroll_7;
            }

            set
            {
                __Kontroll_7 = value;
            }
        }

        public virtual CheckBox _Kontroll_5
        {
            get
            {
                return __Kontroll_5;
            }

            set
            {
                __Kontroll_5 = value;
            }
        }

        public virtual Button _Befehl1_26
        {
            get
            {
                return __Befehl1_26;
            }

            set
            {
                __Befehl1_26 = value;
            }
        }

        public virtual CheckBox _Kontroll_0
        {
            get
            {
                return __Kontroll_0;
            }

            set
            {
                __Kontroll_0 = value;
            }
        }

        public virtual Button _Befehl1_21
        {
            get
            {
                return __Befehl1_21;
            }

            set
            {
                __Befehl1_21 = value;
            }
        }

        public virtual Button _Befehl1_16
        {
            get
            {
                return __Befehl1_16;
            }

            set
            {
                __Befehl1_16 = value;
            }
        }

        public virtual Button _Befehl1_20
        {
            get
            {
                return __Befehl1_20;
            }

            set
            {
                __Befehl1_20 = value;
            }
        }

        public virtual TextBox bereich2
        {
            get
            {
                return _bereich2;
            }

            set
            {
                KeyPressEventHandler value2 = bereich2_KeyPress;
                EventHandler value3 = bereich2_TextChanged;
                if (_bereich2 != null)
                {
                    _bereich2.KeyPress -= value2;
                    _bereich2.TextChanged -= value3;
                }
                _bereich2 = value;
                if (_bereich2 != null)
                {
                    _bereich2.KeyPress += value2;
                    _bereich2.TextChanged += value3;
                }
            }
        }

        public virtual TextBox Bereich1
        {
            get
            {
                return _Bereich1;
            }

            set
            {
                KeyPressEventHandler value2 = Bereich1_KeyPress;
                EventHandler value3 = Bereich1_TextChanged;
                if (_Bereich1 != null)
                {
                    _Bereich1.KeyPress -= value2;
                    _Bereich1.TextChanged -= value3;
                }
                _Bereich1 = value;
                if (_Bereich1 != null)
                {
                    _Bereich1.KeyPress += value2;
                    _Bereich1.TextChanged += value3;
                }
            }
        }

        public virtual TextBox bereich3
        {
            get
            {
                return _bereich3;
            }

            set
            {
                KeyPressEventHandler value2 = bereich3_KeyPress;
                if (_bereich3 != null)
                {
                    _bereich3.KeyPress -= value2;
                }
                _bereich3 = value;
                if (_bereich3 != null)
                {
                    _bereich3.KeyPress += value2;
                }
            }
        }

        public virtual TextBox bereich4
        {
            get
            {
                return _bereich4;
            }

            set
            {
                KeyPressEventHandler value2 = bereich4_KeyPress;
                if (_bereich4 != null)
                {
                    _bereich4.KeyPress -= value2;
                }
                _bereich4 = value;
                if (_bereich4 != null)
                {
                    _bereich4.KeyPress += value2;
                }
            }
        }

        public virtual RadioButton Option1
        {
            get
            {
                return _Option1;
            }

            set
            {
                _Option1 = value;
            }
        }

        public virtual RadioButton Option2
        {
            get
            {
                return _Option2;
            }

            set
            {
                _Option2 = value;
            }
        }

        public virtual CheckBox _Kontroll_1
        {
            get
            {
                return __Kontroll_1;
            }

            set
            {
                __Kontroll_1 = value;
            }
        }

        public virtual CheckBox _Kontroll_3
        {
            get
            {
                return __Kontroll_3;
            }

            set
            {
                __Kontroll_3 = value;
            }
        }

        public virtual CheckBox _Kontroll_2
        {
            get
            {
                return __Kontroll_2;
            }

            set
            {
                __Kontroll_2 = value;
            }
        }

        public virtual CheckBox _Kontroll_12
        {
            get
            {
                return __Kontroll_12;
            }

            set
            {
                __Kontroll_12 = value;
            }
        }

        public virtual RadioButton _Option3_1
        {
            get
            {
                return __Option3_1;
            }

            set
            {
                __Option3_1 = value;
            }
        }

        public virtual RadioButton _Option3_0
        {
            get
            {
                return __Option3_0;
            }

            set
            {
                __Option3_0 = value;
            }
        }

        public virtual Panel Frame2
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

        public virtual CheckBox _Kontroll_4
        {
            get
            {
                return __Kontroll_4;
            }

            set
            {
                __Kontroll_4 = value;
            }
        }

        public virtual Label _Label2_2
        {
            get
            {
                return __Label2_2;
            }

            set
            {
                __Label2_2 = value;
            }
        }

        public virtual Label _Label2_1
        {
            get
            {
                return __Label2_1;
            }

            set
            {
                __Label2_1 = value;
            }
        }

        public virtual Label _Label2_0
        {
            get
            {
                return __Label2_0;
            }

            set
            {
                __Label2_0 = value;
            }
        }

        public virtual Label _Bez_5
        {
            get
            {
                return __Bez_5;
            }

            set
            {
                __Bez_5 = value;
            }
        }

        public virtual Label Bezeichnung4
        {
            get
            {
                return _Bezeichnung4;
            }

            set
            {
                _Bezeichnung4 = value;
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

        public virtual Label Bezeichnung5
        {
            get
            {
                return _Bezeichnung5;
            }

            set
            {
                _Bezeichnung5 = value;
            }
        }

        public virtual Label _Bez_1
        {
            get
            {
                return __Bez_1;
            }

            set
            {
                __Bez_1 = value;
            }
        }

        public virtual Label _Bez_2
        {
            get
            {
                return __Bez_2;
            }

            set
            {
                __Bez_2 = value;
            }
        }

        public virtual Label _Bez_3
        {
            get
            {
                return __Bez_3;
            }

            set
            {
                __Bez_3 = value;
            }
        }

        public virtual ControlArray<Button> Befehl1
        {
            get
            {
                return _Befehl1;
            }

            set
            {
                EventHandler obj = Befehl1_Click;
                if (_Befehl1 != null)
                {
                    _Befehl1.RemoveClick(obj);
                }
                _Befehl1 = value;
                if (_Befehl1 != null)
                {
                    _Befehl1.AddClick(obj);
                }
            }
        }

        public virtual ControlArray<Label> Bez
        {
            get
            {
                return _Bez;
            }

            set
            {
                _Bez = value;
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

        public virtual ControlArray<CheckBox> Kontroll
        {
            get
            {
                return _Kontroll;
            }

            set
            {
                KeyPressEventHandler obj = Kontroll_KeyPress;
                EventHandler obj2 = Kontroll_CheckStateChanged;
                if (_Kontroll != null)
                {
                    _Kontroll.RemoveKeyPress(obj);
                    _Kontroll.RemoveCheckedChanged(obj2);
                }
                _Kontroll = value;
                if (_Kontroll != null)
                {
                    _Kontroll.AddKeyPress(obj);
                    _Kontroll.AddCheckedChanged(obj2);
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

        public virtual ControlArray<Label> Label2
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

        public virtual ControlArray<RadioButton> Option3
        {
            get
            {
                return _Option3;
            }

            set
            {
                _Option3 = value;
            }
        }

        internal virtual CheckBox CheckBox6
        {
            get
            {
                return _CheckBox6;
            }

            set
            {
                _CheckBox6 = value;
            }
        }

        internal virtual CheckBox CheckBox5
        {
            get
            {
                return _CheckBox5;
            }

            set
            {
                _CheckBox5 = value;
            }
        }

        internal virtual CheckBox CheckBox4
        {
            get
            {
                return _CheckBox4;
            }

            set
            {
                _CheckBox4 = value;
            }
        }

        internal virtual CheckBox CheckBox3
        {
            get
            {
                return _CheckBox3;
            }

            set
            {
                _CheckBox3 = value;
            }
        }

        internal virtual CheckBox CheckBox2
        {
            get
            {
                return _CheckBox2;
            }

            set
            {
                _CheckBox2 = value;
            }
        }

        internal virtual CheckBox CheckBox1
        {
            get
            {
                return _CheckBox1;
            }

            set
            {
                _CheckBox1 = value;
            }
        }

        internal virtual Panel Panel1
        {
            get
            {
                return _Panel1;
            }

            set
            {
                _Panel1 = value;
            }
        }

        internal virtual Label Label7
        {
            get
            {
                return _Label7;
            }

            set
            {
                _Label7 = value;
            }
        }

        internal virtual RadioButton RadioButton3
        {
            get
            {
                return _RadioButton3;
            }

            set
            {
                _RadioButton3 = value;
            }
        }

        internal virtual RadioButton RadioButton1
        {
            get
            {
                return _RadioButton1;
            }

            set
            {
                _RadioButton1 = value;
            }
        }

        internal virtual RadioButton RadioButton2
        {
            get
            {
                return _RadioButton2;
            }

            set
            {
                _RadioButton2 = value;
            }
        }

        public virtual Button Button1
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

        internal virtual RichTextBox RichTextBox1
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

        internal virtual GroupBox GroupBox1
        {
            get
            {
                return _GroupBox1;
            }

            set
            {
                _GroupBox1 = value;
            }
        }

        internal virtual Button Button3
        {
            get
            {
                return _Button3;
            }

            set
            {
                EventHandler value2 = Button3_Click;
                if (_Button3 != null)
                {
                    _Button3.Click -= value2;
                }
                _Button3 = value;
                if (_Button3 != null)
                {
                    _Button3.Click += value2;
                }
            }
        }

        internal virtual Button Button2
        {
            get
            {
                return _Button2;
            }

            set
            {
                EventHandler value2 = Button2_Click;
                if (_Button2 != null)
                {
                    _Button2.Click -= value2;
                }
                _Button2 = value;
                if (_Button2 != null)
                {
                    _Button2.Click += value2;
                }
            }
        }

        public virtual Label Label6
        {
            get
            {
                return _Label6;
            }

            set
            {
                _Label6 = value;
            }
        }

        internal virtual Label Label8
        {
            get
            {
                return _Label8;
            }

            set
            {
                _Label8 = value;
            }
        }

        public virtual Button Button4
        {
            get
            {
                return _Button4;
            }

            set
            {
                EventHandler value2 = Button4_Click;
                if (_Button4 != null)
                {
                    _Button4.Click -= value2;
                }
                _Button4 = value;
                if (_Button4 != null)
                {
                    _Button4.Click += value2;
                }
            }
        }

        public virtual Button Button5
        {
            get
            {
                return _Button5;
            }

            set
            {
                EventHandler value2 = Button5_Click;
                if (_Button5 != null)
                {
                    _Button5.Click -= value2;
                }
                _Button5 = value;
                if (_Button5 != null)
                {
                    _Button5.Click += value2;
                }
            }
        }

        internal virtual Label Label9
        {
            get
            {
                return _Label9;
            }

            set
            {
                _Label9 = value;
            }
        }

        public virtual Button Button6
        {
            get
            {
                return _Button6;
            }

            set
            {
                EventHandler value2 = Button6_Click;
                if (_Button6 != null)
                {
                    _Button6.Click -= value2;
                }
                _Button6 = value;
                if (_Button6 != null)
                {
                    _Button6.Click += value2;
                }
            }
        }

        internal virtual Button Button7
        {
            get
            {
                return _Button7;
            }

            set
            {
                EventHandler value2 = Button7_Click;
                if (_Button7 != null)
                {
                    _Button7.Click -= value2;
                }
                _Button7 = value;
                if (_Button7 != null)
                {
                    _Button7.Click += value2;
                }
            }
        }

        internal virtual CheckBox CheckBox7
        {
            get
            {
                return _CheckBox7;
            }

            set
            {
                _CheckBox7 = value;
            }
        }

        internal virtual CheckBox CheckBox8
        {
            get
            {
                return _CheckBox8;
            }

            set
            {
                _CheckBox8 = value;
            }
        }

        public virtual Button Button8
        {
            get
            {
                return _Button8;
            }

            set
            {
                EventHandler value2 = Button5_Click;
                if (_Button8 != null)
                {
                    _Button8.Click -= value2;
                }
                _Button8 = value;
                if (_Button8 != null)
                {
                    _Button8.Click += value2;
                }
            }
        }

        [DebuggerNonUserCode]
        public Ausw()
        {
            base.FormClosing += Ausw_FormClosing;
            base.FormClosed += Ausw_FormClosed;
            base.Load += Ausw_Load;
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
            this.Frame3 = new System.Windows.Forms.GroupBox();
            this._Command1_2 = new System.Windows.Forms.Button();
            this.Command2 = new System.Windows.Forms.Button();
            this.Text1 = new System.Windows.Forms.TextBox();
            this.List2 = new System.Windows.Forms.ListBox();
            this.List1 = new System.Windows.Forms.ListBox();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.Button8 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.Button5 = new System.Windows.Forms.Button();
            this.Button4 = new System.Windows.Forms.Button();
            this.Label6 = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Button();
            this._Befehl1_32 = new System.Windows.Forms.Button();
            this._Befehl1_31 = new System.Windows.Forms.Button();
            this._Befehl1_30 = new System.Windows.Forms.Button();
            this._Befehl1_29 = new System.Windows.Forms.Button();
            this._Befehl1_10 = new System.Windows.Forms.Button();
            this._Befehl1_12 = new System.Windows.Forms.Button();
            this._Befehl1_11 = new System.Windows.Forms.Button();
            this._Befehl1_2 = new System.Windows.Forms.Button();
            this._Befehl1_1 = new System.Windows.Forms.Button();
            this._Befehl1_14 = new System.Windows.Forms.Button();
            this._Befehl1_13 = new System.Windows.Forms.Button();
            this._Befehl1_5 = new System.Windows.Forms.Button();
            this._Befehl1_6 = new System.Windows.Forms.Button();
            this._Befehl1_4 = new System.Windows.Forms.Button();
            this._Befehl1_3 = new System.Windows.Forms.Button();
            this._Befehl1_0 = new System.Windows.Forms.Button();
            this._Befehl1_7 = new System.Windows.Forms.Button();
            this._Befehl1_8 = new System.Windows.Forms.Button();
            this._Befehl1_9 = new System.Windows.Forms.Button();
            this._Befehl1_15 = new System.Windows.Forms.Button();
            this._Befehl1_17 = new System.Windows.Forms.Button();
            this._Befehl1_18 = new System.Windows.Forms.Button();
            this._Befehl1_19 = new System.Windows.Forms.Button();
            this._Befehl1_22 = new System.Windows.Forms.Button();
            this._Befehl1_23 = new System.Windows.Forms.Button();
            this._Befehl1_24 = new System.Windows.Forms.Button();
            this._Befehl1_25 = new System.Windows.Forms.Button();
            this._Befehl1_28 = new System.Windows.Forms.Button();
            this.Label5 = new System.Windows.Forms.Label();
            this.Bezeichnung3 = new System.Windows.Forms.Label();
            this.Bezeichnung2 = new System.Windows.Forms.Label();
            this.Bezeichnung1 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.Frame2 = new System.Windows.Forms.Panel();
            this._Option3_1 = new System.Windows.Forms.RadioButton();
            this._Option3_0 = new System.Windows.Forms.RadioButton();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Button7 = new System.Windows.Forms.Button();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Button3 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this._Kontroll_11 = new System.Windows.Forms.CheckBox();
            this._Kontroll_10 = new System.Windows.Forms.CheckBox();
            this._Kontroll_8 = new System.Windows.Forms.CheckBox();
            this._Kontroll_7 = new System.Windows.Forms.CheckBox();
            this._Kontroll_5 = new System.Windows.Forms.CheckBox();
            this._Befehl1_26 = new System.Windows.Forms.Button();
            this._Kontroll_0 = new System.Windows.Forms.CheckBox();
            this._Befehl1_21 = new System.Windows.Forms.Button();
            this._Befehl1_16 = new System.Windows.Forms.Button();
            this._Befehl1_20 = new System.Windows.Forms.Button();
            this.bereich2 = new System.Windows.Forms.TextBox();
            this.Bereich1 = new System.Windows.Forms.TextBox();
            this.bereich3 = new System.Windows.Forms.TextBox();
            this.bereich4 = new System.Windows.Forms.TextBox();
            this.Option1 = new System.Windows.Forms.RadioButton();
            this.Option2 = new System.Windows.Forms.RadioButton();
            this._Kontroll_1 = new System.Windows.Forms.CheckBox();
            this._Kontroll_3 = new System.Windows.Forms.CheckBox();
            this._Kontroll_2 = new System.Windows.Forms.CheckBox();
            this._Kontroll_12 = new System.Windows.Forms.CheckBox();
            this._Kontroll_4 = new System.Windows.Forms.CheckBox();
            this._Label2_2 = new System.Windows.Forms.Label();
            this._Label2_1 = new System.Windows.Forms.Label();
            this._Label2_0 = new System.Windows.Forms.Label();
            this._Bez_5 = new System.Windows.Forms.Label();
            this.Bezeichnung4 = new System.Windows.Forms.Label();
            this.Bezeichnung6 = new System.Windows.Forms.Label();
            this.Bezeichnung5 = new System.Windows.Forms.Label();
            this._Bez_1 = new System.Windows.Forms.Label();
            this._Bez_2 = new System.Windows.Forms.Label();
            this._Bez_3 = new System.Windows.Forms.Label();
            this.Befehl1 = new ControlArray<System.Windows.Forms.Button>();
            this.Bez = new ControlArray<System.Windows.Forms.Label>();
            this.Command1 = new ControlArray<System.Windows.Forms.Button>();
            this.Kontroll = new ControlArray<System.Windows.Forms.CheckBox>();
            this.Label1 = new ControlArray<System.Windows.Forms.Label>();
            this.Label2 = new ControlArray<System.Windows.Forms.Label>();
            this.Option3 = new ControlArray<RadioButton>();
            this.CheckBox6 = new System.Windows.Forms.CheckBox();
            this.CheckBox5 = new System.Windows.Forms.CheckBox();
            this.CheckBox4 = new System.Windows.Forms.CheckBox();
            this.CheckBox3 = new System.Windows.Forms.CheckBox();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Label7 = new System.Windows.Forms.Label();
            this.RadioButton3 = new System.Windows.Forms.RadioButton();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.RadioButton2 = new System.Windows.Forms.RadioButton();
            this.CheckBox7 = new System.Windows.Forms.CheckBox();
            this.CheckBox8 = new System.Windows.Forms.CheckBox();
            this.Frame3.SuspendLayout();
            this.Frame1.SuspendLayout();
            this.Frame2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.Befehl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Bez).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Kontroll).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Label2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.Option3).BeginInit();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            this.Frame3.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
            this.Frame3.Controls.Add(this._Command1_2);
            this.Frame3.Controls.Add(this.Command2);
            this.Frame3.Controls.Add(this.Text1);
            this.Frame3.Controls.Add(this.List2);
            this.Frame3.Controls.Add(this.List1);
            this.Frame3.Controls.Add(this._Command1_1);
            this.Frame3.Controls.Add(this._Command1_0);
            this.Frame3.Controls.Add(this.Label4);
            this.Frame3.Controls.Add(this.Label3);
            this.Frame3.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
            System.Drawing.Point point2 = this.Frame3.Location = new System.Drawing.Point(69, 69);
            System.Windows.Forms.Padding padding2 = this.Frame3.Margin = new System.Windows.Forms.Padding(4);
            this.Frame3.Name = "Frame3";
            padding2 = this.Frame3.Padding = new System.Windows.Forms.Padding(4);
            this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            System.Drawing.Size size2 = this.Frame3.Size = new System.Drawing.Size(1024, 598);
            this.Frame3.TabIndex = 66;
            this.Frame3.TabStop = false;
            this.Frame3.Visible = false;
            this._Command1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_2, 2);
            point2 = this._Command1_2.Location = new System.Drawing.Point(203, 17);
            padding2 = this._Command1_2.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_2.Name = "_Command1_2";
            this._Command1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_2.Size = new System.Drawing.Size(148, 26);
            this._Command1_2.TabIndex = 76;
            this._Command1_2.Text = "Sonstige Ereignisse";
            this._Command1_2.UseVisualStyleBackColor = false;
            this.Command2.BackColor = System.Drawing.SystemColors.Control;
            this.Command2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Command2.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Command2.Location = new System.Drawing.Point(309, 257);
            padding2 = this.Command2.Margin = new System.Windows.Forms.Padding(4);
            this.Command2.Name = "Command2";
            this.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Command2.Size = new System.Drawing.Size(135, 52);
            this.Command2.TabIndex = 74;
            this.Command2.Text = "Start";
            this.Command2.UseVisualStyleBackColor = false;
            this.Text1.AcceptsReturn = true;
            this.Text1.BackColor = System.Drawing.SystemColors.Window;
            this.Text1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text1.ForeColor = System.Drawing.SystemColors.WindowText;
            point2 = this.Text1.Location = new System.Drawing.Point(307, 109);
            padding2 = this.Text1.Margin = new System.Windows.Forms.Padding(4);
            this.Text1.MaxLength = 0;
            this.Text1.Name = "Text1";
            this.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Text1.Size = new System.Drawing.Size(83, 22);
            this.Text1.TabIndex = 71;
            this.List2.BackColor = System.Drawing.SystemColors.Window;
            this.List2.Cursor = System.Windows.Forms.Cursors.Default;
            this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List2.ItemHeight = 16;
            point2 = this.List2.Location = new System.Drawing.Point(469, 78);
            padding2 = this.List2.Margin = new System.Windows.Forms.Padding(4);
            this.List2.Name = "List2";
            this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List2.SelectionMode = System.Windows.Forms.SelectionMode.None;
            size2 = this.List2.Size = new System.Drawing.Size(248, 436);
            this.List2.Sorted = true;
            this.List2.TabIndex = 70;
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 16;
            point2 = this.List1.Location = new System.Drawing.Point(11, 73);
            padding2 = this.List1.Margin = new System.Windows.Forms.Padding(4);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.List1.Size = new System.Drawing.Size(269, 436);
            this.List1.Sorted = true;
            this.List1.TabIndex = 69;
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_1, 1);
            point2 = this._Command1_1.Location = new System.Drawing.Point(112, 15);
            padding2 = this._Command1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_1.Size = new System.Drawing.Size(81, 26);
            this._Command1_1.TabIndex = 68;
            this._Command1_1.Text = "Titel";
            this._Command1_1.UseVisualStyleBackColor = false;
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.SetIndex(this._Command1_0, 0);
            point2 = this._Command1_0.Location = new System.Drawing.Point(21, 15);
            padding2 = this._Command1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Command1_0.Size = new System.Drawing.Size(81, 26);
            this._Command1_0.TabIndex = 67;
            this._Command1_0.Text = "Berufe";
            this._Command1_0.UseVisualStyleBackColor = false;
            this.Label4.BackColor = System.Drawing.SystemColors.Control;
            this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Label4.Location = new System.Drawing.Point(466, 44);
            padding2 = this.Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label4.Name = "Label4";
            this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Label4.Size = new System.Drawing.Size(343, 21);
            this.Label4.TabIndex = 75;
            this.Label3.BackColor = System.Drawing.SystemColors.Control;
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.Font = new System.Drawing.Font("Arial", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Label3.Location = new System.Drawing.Point(304, 85);
            padding2 = this.Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Label3.Size = new System.Drawing.Size(89, 16);
            this.Label3.TabIndex = 72;
            this.Label3.Text = "Start mit";
            this.Frame1.BackColor = System.Drawing.SystemColors.Control;
            this.Frame1.Controls.Add(this.Button8);
            this.Frame1.Controls.Add(this.Button6);
            this.Frame1.Controls.Add(this.Button5);
            this.Frame1.Controls.Add(this.Button4);
            this.Frame1.Controls.Add(this.Label6);
            this.Frame1.Controls.Add(this.Button1);
            this.Frame1.Controls.Add(this._Befehl1_32);
            this.Frame1.Controls.Add(this._Befehl1_31);
            this.Frame1.Controls.Add(this._Befehl1_30);
            this.Frame1.Controls.Add(this._Befehl1_29);
            this.Frame1.Controls.Add(this._Befehl1_10);
            this.Frame1.Controls.Add(this._Befehl1_12);
            this.Frame1.Controls.Add(this._Befehl1_11);
            this.Frame1.Controls.Add(this._Befehl1_2);
            this.Frame1.Controls.Add(this._Befehl1_1);
            this.Frame1.Controls.Add(this._Befehl1_14);
            this.Frame1.Controls.Add(this._Befehl1_13);
            this.Frame1.Controls.Add(this._Befehl1_5);
            this.Frame1.Controls.Add(this._Befehl1_6);
            this.Frame1.Controls.Add(this._Befehl1_4);
            this.Frame1.Controls.Add(this._Befehl1_3);
            this.Frame1.Controls.Add(this._Befehl1_0);
            this.Frame1.Controls.Add(this._Befehl1_7);
            this.Frame1.Controls.Add(this._Befehl1_8);
            this.Frame1.Controls.Add(this._Befehl1_9);
            this.Frame1.Controls.Add(this._Befehl1_15);
            this.Frame1.Controls.Add(this._Befehl1_17);
            this.Frame1.Controls.Add(this._Befehl1_18);
            this.Frame1.Controls.Add(this._Befehl1_19);
            this.Frame1.Controls.Add(this._Befehl1_22);
            this.Frame1.Controls.Add(this._Befehl1_23);
            this.Frame1.Controls.Add(this._Befehl1_24);
            this.Frame1.Controls.Add(this._Befehl1_25);
            this.Frame1.Controls.Add(this._Befehl1_28);
            this.Frame1.Controls.Add(this.Label5);
            this.Frame1.Controls.Add(this.Bezeichnung3);
            this.Frame1.Controls.Add(this.Bezeichnung2);
            this.Frame1.Controls.Add(this.Bezeichnung1);
            this.Frame1.Controls.Add(this._Label1_0);
            this.Frame1.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Frame1.Location = new System.Drawing.Point(80, 66);
            padding2 = this.Frame1.Margin = new System.Windows.Forms.Padding(4);
            this.Frame1.Name = "Frame1";
            padding2 = this.Frame1.Padding = new System.Windows.Forms.Padding(4);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Frame1.Size = new System.Drawing.Size(1013, 596);
            this.Frame1.TabIndex = 20;
            this.Frame1.TabStop = false;
            this.Button8.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Button8.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button8.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Button8.Location = new System.Drawing.Point(730, 452);
            padding2 = this.Button8.Margin = new System.Windows.Forms.Padding(4);
            this.Button8.Name = "Button8";
            this.Button8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Button8.Size = new System.Drawing.Size(216, 44);
            this.Button8.TabIndex = 84;
            this.Button8.Text = "Bemerkungen ohne untere Datumsbemerkungen";
            this.Button8.UseVisualStyleBackColor = false;
            this.Button8.Visible = false;
            this.Button6.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Button6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button6.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Button6.Location = new System.Drawing.Point(730, 300);
            padding2 = this.Button6.Margin = new System.Windows.Forms.Padding(4);
            this.Button6.Name = "Button6";
            this.Button6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Button6.Size = new System.Drawing.Size(216, 44);
            this.Button6.TabIndex = 83;
            this.Button6.Text = "Liste der als Freitext eingegebenen Paten";
            this.Button6.UseVisualStyleBackColor = false;
            this.Button5.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Button5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button5.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Button5.Location = new System.Drawing.Point(730, 407);
            padding2 = this.Button5.Margin = new System.Windows.Forms.Padding(4);
            this.Button5.Name = "Button5";
            this.Button5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Button5.Size = new System.Drawing.Size(215, 44);
            this.Button5.TabIndex = 82;
            this.Button5.Text = "Liste der Bemerkungen";
            this.Button5.UseVisualStyleBackColor = false;
            this.Button4.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Button4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button4.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Button4.Location = new System.Drawing.Point(730, 352);
            padding2 = this.Button4.Margin = new System.Windows.Forms.Padding(4);
            this.Button4.Name = "Button4";
            this.Button4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Button4.Size = new System.Drawing.Size(215, 44);
            this.Button4.TabIndex = 81;
            this.Button4.Text = "Liste der als Freitext eingegebenen Zeugen";
            this.Button4.UseVisualStyleBackColor = false;
            this.Label6.BackColor = System.Drawing.Color.White;
            this.Label6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label6.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Label6.ForeColor = System.Drawing.Color.Black;
            point2 = this.Label6.Location = new System.Drawing.Point(757, 195);
            padding2 = this.Label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label6.Name = "Label6";
            this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Label6.Size = new System.Drawing.Size(180, 30);
            this.Label6.TabIndex = 80;
            this.Label6.Text = "Listen der Freitexte";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Button1.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this.Button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button1.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Button1.Location = new System.Drawing.Point(730, 247);
            padding2 = this.Button1.Margin = new System.Windows.Forms.Padding(4);
            this.Button1.Name = "Button1";
            this.Button1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Button1.Size = new System.Drawing.Size(215, 44);
            this.Button1.TabIndex = 79;
            this.Button1.Text = "Liste der als Freitext eingegebenen Quellen";
            this.Button1.UseVisualStyleBackColor = false;
            this._Befehl1_32.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_32.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_32.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_32, 32);
            point2 = this._Befehl1_32.Location = new System.Drawing.Point(761, 107);
            padding2 = this._Befehl1_32.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_32.Name = "_Befehl1_32";
            this._Befehl1_32.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_32.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_32.TabIndex = 73;
            this._Befehl1_32.Text = "Berufe oder Titel";
            this._Befehl1_32.UseVisualStyleBackColor = false;
            this._Befehl1_31.BackColor = System.Drawing.SystemColors.Control;
            this._Befehl1_31.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_31.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_31, 31);
            point2 = this._Befehl1_31.Location = new System.Drawing.Point(344, 420);
            padding2 = this._Befehl1_31.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_31.Name = "_Befehl1_31";
            this._Befehl1_31.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_31.Size = new System.Drawing.Size(209, 30);
            this._Befehl1_31.TabIndex = 61;
            this._Befehl1_31.Text = "Nachfahrennnummer";
            this._Befehl1_31.UseVisualStyleBackColor = false;
            this._Befehl1_31.Visible = false;
            this._Befehl1_30.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_30.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_30.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_30, 30);
            point2 = this._Befehl1_30.Location = new System.Drawing.Point(761, 70);
            padding2 = this._Befehl1_30.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_30.Name = "_Befehl1_30";
            this._Befehl1_30.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_30.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_30.TabIndex = 60;
            this._Befehl1_30.Text = "sonstiges Datum";
            this._Befehl1_30.UseVisualStyleBackColor = false;
            this._Befehl1_29.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_29.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_29.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_29, 29);
            point2 = this._Befehl1_29.Location = new System.Drawing.Point(40, 106);
            padding2 = this._Befehl1_29.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_29.Name = "_Befehl1_29";
            this._Befehl1_29.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_29.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_29.TabIndex = 56;
            this._Befehl1_29.Text = "Kontaktliste Namen";
            this._Befehl1_29.UseVisualStyleBackColor = false;
            this._Befehl1_10.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_10, 10);
            point2 = this._Befehl1_10.Location = new System.Drawing.Point(244, 70);
            padding2 = this._Befehl1_10.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_10.Name = "_Befehl1_10";
            this._Befehl1_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_10.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_10.TabIndex = 44;
            this._Befehl1_10.Text = "Familien-Nummern";
            this._Befehl1_10.UseVisualStyleBackColor = false;
            this._Befehl1_12.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_12.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_12, 12);
            point2 = this._Befehl1_12.Location = new System.Drawing.Point(244, 141);
            padding2 = this._Befehl1_12.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_12.Name = "_Befehl1_12";
            this._Befehl1_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_12.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_12.TabIndex = 43;
            this._Befehl1_12.Text = "Name der Ehefrau";
            this._Befehl1_12.UseVisualStyleBackColor = false;
            this._Befehl1_11.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_11, 11);
            point2 = this._Befehl1_11.Location = new System.Drawing.Point(244, 106);
            padding2 = this._Befehl1_11.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_11.Name = "_Befehl1_11";
            this._Befehl1_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_11.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_11.TabIndex = 42;
            this._Befehl1_11.Text = "Name des Ehemannes";
            this._Befehl1_11.UseVisualStyleBackColor = false;
            this._Befehl1_2.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_2, 2);
            point2 = this._Befehl1_2.Location = new System.Drawing.Point(40, 176);
            padding2 = this._Befehl1_2.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_2.Name = "_Befehl1_2";
            this._Befehl1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_2.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_2.TabIndex = 41;
            this._Befehl1_2.Text = "Namen";
            this._Befehl1_2.UseVisualStyleBackColor = false;
            this._Befehl1_1.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_1, 1);
            point2 = this._Befehl1_1.Location = new System.Drawing.Point(40, 141);
            padding2 = this._Befehl1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_1.Name = "_Befehl1_1";
            this._Befehl1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_1.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_1.TabIndex = 40;
            this._Befehl1_1.Text = "Personen-Nummern";
            this._Befehl1_1.UseVisualStyleBackColor = false;
            this._Befehl1_14.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_14.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_14, 14);
            point2 = this._Befehl1_14.Location = new System.Drawing.Point(244, 282);
            padding2 = this._Befehl1_14.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_14.Name = "_Befehl1_14";
            this._Befehl1_14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_14.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_14.TabIndex = 39;
            this._Befehl1_14.Text = "kirchl. Heiratsdatum";
            this._Befehl1_14.UseVisualStyleBackColor = false;
            this._Befehl1_13.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_13.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_13.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_13, 13);
            point2 = this._Befehl1_13.Location = new System.Drawing.Point(244, 247);
            padding2 = this._Befehl1_13.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_13.Name = "_Befehl1_13";
            this._Befehl1_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_13.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_13.TabIndex = 38;
            this._Befehl1_13.Text = "Heiratsdatum";
            this._Befehl1_13.UseVisualStyleBackColor = false;
            this._Befehl1_5.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_5, 5);
            point2 = this._Befehl1_5.Location = new System.Drawing.Point(40, 352);
            padding2 = this._Befehl1_5.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_5.Name = "_Befehl1_5";
            this._Befehl1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_5.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_5.TabIndex = 37;
            this._Befehl1_5.Text = "Sterbedatum";
            this._Befehl1_5.UseVisualStyleBackColor = false;
            this._Befehl1_6.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_6, 6);
            point2 = this._Befehl1_6.Location = new System.Drawing.Point(40, 387);
            padding2 = this._Befehl1_6.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_6.Name = "_Befehl1_6";
            this._Befehl1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_6.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_6.TabIndex = 36;
            this._Befehl1_6.Text = "Begräbnisdatum";
            this._Befehl1_6.UseVisualStyleBackColor = false;
            this._Befehl1_4.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_4, 4);
            point2 = this._Befehl1_4.Location = new System.Drawing.Point(40, 317);
            padding2 = this._Befehl1_4.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_4.Name = "_Befehl1_4";
            this._Befehl1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_4.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_4.TabIndex = 35;
            this._Befehl1_4.Text = "Taufdatum";
            this._Befehl1_4.UseVisualStyleBackColor = false;
            this._Befehl1_3.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_3, 3);
            point2 = this._Befehl1_3.Location = new System.Drawing.Point(40, 282);
            padding2 = this._Befehl1_3.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_3.Name = "_Befehl1_3";
            this._Befehl1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_3.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_3.TabIndex = 34;
            this._Befehl1_3.Text = "Geburtsdatum";
            this._Befehl1_3.UseVisualStyleBackColor = false;
            this._Befehl1_0.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_0, 0);
            point2 = this._Befehl1_0.Location = new System.Drawing.Point(244, 176);
            padding2 = this._Befehl1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_0.Name = "_Befehl1_0";
            this._Befehl1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_0.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_0.TabIndex = 33;
            this._Befehl1_0.Text = "Proklamationsdatum";
            this._Befehl1_0.UseVisualStyleBackColor = false;
            this._Befehl1_7.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_7, 7);
            point2 = this._Befehl1_7.Location = new System.Drawing.Point(244, 211);
            padding2 = this._Befehl1_7.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_7.Name = "_Befehl1_7";
            this._Befehl1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_7.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_7.TabIndex = 32;
            this._Befehl1_7.Text = "Verlobungsdatum";
            this._Befehl1_7.UseVisualStyleBackColor = false;
            this._Befehl1_8.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_8, 8);
            point2 = this._Befehl1_8.Location = new System.Drawing.Point(244, 317);
            padding2 = this._Befehl1_8.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_8.Name = "_Befehl1_8";
            this._Befehl1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_8.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_8.TabIndex = 31;
            this._Befehl1_8.Text = "Scheidungsdatum";
            this._Befehl1_8.UseVisualStyleBackColor = false;
            this._Befehl1_9.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_9, 9);
            point2 = this._Befehl1_9.Location = new System.Drawing.Point(496, 70);
            padding2 = this._Befehl1_9.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_9.Name = "_Befehl1_9";
            this._Befehl1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_9.Size = new System.Drawing.Size(207, 30);
            this._Befehl1_9.TabIndex = 30;
            this._Befehl1_9.Text = "Geburtsregister";
            this._Befehl1_9.UseVisualStyleBackColor = false;
            this._Befehl1_15.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_15.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_15.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_15, 15);
            point2 = this._Befehl1_15.Location = new System.Drawing.Point(496, 107);
            padding2 = this._Befehl1_15.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_15.Name = "_Befehl1_15";
            this._Befehl1_15.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_15.Size = new System.Drawing.Size(207, 30);
            this._Befehl1_15.TabIndex = 29;
            this._Befehl1_15.Text = "Taufregister";
            this._Befehl1_15.UseVisualStyleBackColor = false;
            this._Befehl1_17.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_17.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_17, 17);
            point2 = this._Befehl1_17.Location = new System.Drawing.Point(496, 141);
            padding2 = this._Befehl1_17.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_17.Name = "_Befehl1_17";
            this._Befehl1_17.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_17.Size = new System.Drawing.Size(207, 30);
            this._Befehl1_17.TabIndex = 28;
            this._Befehl1_17.Text = "Sterberegister";
            this._Befehl1_17.UseVisualStyleBackColor = false;
            this._Befehl1_18.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_18.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_18.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_18, 18);
            point2 = this._Befehl1_18.Location = new System.Drawing.Point(496, 176);
            padding2 = this._Befehl1_18.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_18.Name = "_Befehl1_18";
            this._Befehl1_18.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_18.Size = new System.Drawing.Size(207, 30);
            this._Befehl1_18.TabIndex = 27;
            this._Befehl1_18.Text = "Begräbnisregister";
            this._Befehl1_18.UseVisualStyleBackColor = false;
            this._Befehl1_19.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_19.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_19.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_19, 19);
            point2 = this._Befehl1_19.Location = new System.Drawing.Point(496, 211);
            padding2 = this._Befehl1_19.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_19.Name = "_Befehl1_19";
            this._Befehl1_19.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_19.Size = new System.Drawing.Size(207, 30);
            this._Befehl1_19.TabIndex = 26;
            this._Befehl1_19.Text = "Heiratsregister";
            this._Befehl1_19.UseVisualStyleBackColor = false;
            this._Befehl1_22.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_22.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_22.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_22, 22);
            point2 = this._Befehl1_22.Location = new System.Drawing.Point(496, 247);
            padding2 = this._Befehl1_22.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_22.Name = "_Befehl1_22";
            this._Befehl1_22.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_22.Size = new System.Drawing.Size(207, 30);
            this._Befehl1_22.TabIndex = 25;
            this._Befehl1_22.Text = "kirchl.Heiratsregister";
            this._Befehl1_22.UseVisualStyleBackColor = false;
            this._Befehl1_23.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_23.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_23.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_23, 23);
            point2 = this._Befehl1_23.Location = new System.Drawing.Point(244, 352);
            padding2 = this._Befehl1_23.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_23.Name = "_Befehl1_23";
            this._Befehl1_23.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_23.Size = new System.Drawing.Size(233, 30);
            this._Befehl1_23.TabIndex = 24;
            this._Befehl1_23.Text = "Eheähnliche Beziehung";
            this._Befehl1_23.UseVisualStyleBackColor = false;
            this._Befehl1_24.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_24.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_24.Enabled = false;
            this._Befehl1_24.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_24, 24);
            point2 = this._Befehl1_24.Location = new System.Drawing.Point(40, 211);
            padding2 = this._Befehl1_24.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_24.Name = "_Befehl1_24";
            this._Befehl1_24.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_24.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_24.TabIndex = 23;
            this._Befehl1_24.Text = "Ahnennummer";
            this._Befehl1_24.UseVisualStyleBackColor = false;
            this._Befehl1_25.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_25.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_25.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_25, 25);
            point2 = this._Befehl1_25.Location = new System.Drawing.Point(40, 247);
            padding2 = this._Befehl1_25.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_25.Name = "_Befehl1_25";
            this._Befehl1_25.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_25.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_25.TabIndex = 22;
            this._Befehl1_25.Text = "Sippe";
            this._Befehl1_25.UseVisualStyleBackColor = false;
            this._Befehl1_28.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_28.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_28.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_28, 28);
            point2 = this._Befehl1_28.Location = new System.Drawing.Point(40, 70);
            padding2 = this._Befehl1_28.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_28.Name = "_Befehl1_28";
            this._Befehl1_28.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_28.Size = new System.Drawing.Size(196, 30);
            this._Befehl1_28.TabIndex = 21;
            this._Befehl1_28.Text = "Nur Namen-Liste";
            this._Befehl1_28.UseVisualStyleBackColor = false;
            this.Label5.BackColor = System.Drawing.Color.White;
            this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label5.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.Label5.ForeColor = System.Drawing.Color.Black;
            point2 = this.Label5.Location = new System.Drawing.Point(765, 19);
            padding2 = this.Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label5.Name = "Label5";
            this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Label5.Size = new System.Drawing.Size(180, 38);
            this.Label5.TabIndex = 78;
            this.Label5.Text = "Auswahllisten Personen  nach";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Bezeichnung3.BackColor = System.Drawing.Color.White;
            this.Bezeichnung3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung3.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung3.Location = new System.Drawing.Point(45, 16);
            padding2 = this.Bezeichnung3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung3.Name = "Bezeichnung3";
            this.Bezeichnung3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung3.Size = new System.Drawing.Size(189, 19);
            this.Bezeichnung3.TabIndex = 48;
            this.Bezeichnung3.Text = "Personenlisten";
            this.Bezeichnung3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Bezeichnung2.BackColor = System.Drawing.Color.White;
            this.Bezeichnung2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung2.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung2.Location = new System.Drawing.Point(40, 44);
            padding2 = this.Bezeichnung2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung2.Name = "Bezeichnung2";
            this.Bezeichnung2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung2.Size = new System.Drawing.Size(437, 24);
            this.Bezeichnung2.TabIndex = 47;
            this.Bezeichnung2.Text = "Liste sortiert nach";
            this.Bezeichnung1.BackColor = System.Drawing.Color.White;
            this.Bezeichnung1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung1.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung1.Location = new System.Drawing.Point(269, 19);
            padding2 = this.Bezeichnung1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung1.Name = "Bezeichnung1";
            this.Bezeichnung1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung1.Size = new System.Drawing.Size(176, 19);
            this.Bezeichnung1.TabIndex = 46;
            this.Bezeichnung1.Text = "Familienlisten";
            this.Bezeichnung1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label1_0.BackColor = System.Drawing.Color.White;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.Color.Black;
            this.Label1.SetIndex(this._Label1_0, 0);
            point2 = this._Label1_0.Location = new System.Drawing.Point(488, 19);
            padding2 = this._Label1_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label1_0.Size = new System.Drawing.Size(193, 24);
            this._Label1_0.TabIndex = 45;
            this._Label1_0.Text = "Registerlisten";
            this._Label1_0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Frame2.BackColor = System.Drawing.SystemColors.Control;
            this.Frame2.Controls.Add(this._Option3_1);
            this.Frame2.Controls.Add(this._Option3_0);
            this.Frame2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
            point2 = this.Frame2.Location = new System.Drawing.Point(3, 438);
            padding2 = this.Frame2.Margin = new System.Windows.Forms.Padding(4);
            this.Frame2.Name = "Frame2";
            this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Frame2.Size = new System.Drawing.Size(327, 79);
            this.Frame2.TabIndex = 57;
            this._Option3_1.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Option3_1.Checked = true;
            this._Option3_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option3_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Option3.SetIndex(this._Option3_1, 1);
            point2 = this._Option3_1.Location = new System.Drawing.Point(8, 46);
            padding2 = this._Option3_1.Margin = new System.Windows.Forms.Padding(4);
            this._Option3_1.Name = "_Option3_1";
            this._Option3_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Option3_1.Size = new System.Drawing.Size(293, 21);
            this._Option3_1.TabIndex = 59;
            this._Option3_1.TabStop = true;
            this._Option3_1.Text = "Fließtext";
            this._Option3_1.UseVisualStyleBackColor = false;
            this._Option3_0.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Option3_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option3_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Option3.SetIndex(this._Option3_0, 0);
            point2 = this._Option3_0.Location = new System.Drawing.Point(8, 19);
            padding2 = this._Option3_0.Margin = new System.Windows.Forms.Padding(4);
            this._Option3_0.Name = "_Option3_0";
            this._Option3_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Option3_0.Size = new System.Drawing.Size(293, 21);
            this._Option3_0.TabIndex = 58;
            this._Option3_0.TabStop = true;
            this._Option3_0.Text = "Blocktext";
            this._Option3_0.UseVisualStyleBackColor = false;
            this.GroupBox1.Controls.Add(this.Button7);
            this.GroupBox1.Controls.Add(this.Label9);
            this.GroupBox1.Controls.Add(this.Label8);
            this.GroupBox1.Controls.Add(this.Button3);
            this.GroupBox1.Controls.Add(this.Button2);
            this.GroupBox1.Controls.Add(this.RichTextBox1);
            point2 = this.GroupBox1.Location = new System.Drawing.Point(237, 685);
            this.GroupBox1.Name = "GroupBox1";
            size2 = this.GroupBox1.Size = new System.Drawing.Size(1003, 436);
            this.GroupBox1.TabIndex = 145;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Visible = false;
            this.Button7.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            point2 = this.Button7.Location = new System.Drawing.Point(740, 252);
            this.Button7.Name = "Button7";
            size2 = this.Button7.Size = new System.Drawing.Size(178, 23);
            this.Button7.TabIndex = 149;
            this.Button7.Text = "CSV (Excel) Ausgabe";
            this.Button7.UseVisualStyleBackColor = false;
            this.Label9.AutoSize = true;
            point2 = this.Label9.Location = new System.Drawing.Point(644, 77);
            this.Label9.Name = "Label9";
            size2 = this.Label9.Size = new System.Drawing.Size(0, 17);
            this.Label9.TabIndex = 148;
            this.Label8.AutoSize = true;
            point2 = this.Label8.Location = new System.Drawing.Point(644, 46);
            this.Label8.Name = "Label8";
            size2 = this.Label8.Size = new System.Drawing.Size(51, 17);
            this.Label8.TabIndex = 147;
            this.Label8.Text = "Label8";
            this.Button3.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            point2 = this.Button3.Location = new System.Drawing.Point(740, 212);
            this.Button3.Name = "Button3";
            size2 = this.Button3.Size = new System.Drawing.Size(178, 23);
            this.Button3.TabIndex = 146;
            this.Button3.Text = "Drucken";
            this.Button3.UseVisualStyleBackColor = false;
            this.Button2.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            point2 = this.Button2.Location = new System.Drawing.Point(740, 171);
            this.Button2.Name = "Button2";
            size2 = this.Button2.Size = new System.Drawing.Size(178, 23);
            this.Button2.TabIndex = 145;
            this.Button2.Text = "Druckmenü";
            this.Button2.UseVisualStyleBackColor = false;
            point2 = this.RichTextBox1.Location = new System.Drawing.Point(14, 16);
            this.RichTextBox1.Name = "RichTextBox1";
            size2 = this.RichTextBox1.Size = new System.Drawing.Size(600, 600);
            this.RichTextBox1.TabIndex = 144;
            this.RichTextBox1.Text = "";
            this.RichTextBox1.Visible = false;
            this._Kontroll_11.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_11.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_11, 11);
            point2 = this._Kontroll_11.Location = new System.Drawing.Point(5, 108);
            padding2 = this._Kontroll_11.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_11.Name = "_Kontroll_11";
            this._Kontroll_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_11.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_11.TabIndex = 54;
            this._Kontroll_11.Text = "Ahnennummern";
            this._Kontroll_11.UseVisualStyleBackColor = false;
            this._Kontroll_10.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_10.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_10, 10);
            point2 = this._Kontroll_10.Location = new System.Drawing.Point(5, 132);
            padding2 = this._Kontroll_10.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_10.Name = "_Kontroll_10";
            this._Kontroll_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_10.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_10.TabIndex = 53;
            this._Kontroll_10.Text = "Nachfahrennummern";
            this._Kontroll_10.UseVisualStyleBackColor = false;
            this._Kontroll_8.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_8.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_8, 8);
            point2 = this._Kontroll_8.Location = new System.Drawing.Point(3, 393);
            padding2 = this._Kontroll_8.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_8.Name = "_Kontroll_8";
            this._Kontroll_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_8.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_8.TabIndex = 52;
            this._Kontroll_8.Text = "Quellen";
            this._Kontroll_8.UseVisualStyleBackColor = false;
            this._Kontroll_7.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_7.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_7, 7);
            point2 = this._Kontroll_7.Location = new System.Drawing.Point(5, 264);
            padding2 = this._Kontroll_7.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_7.Name = "_Kontroll_7";
            this._Kontroll_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_7.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_7.TabIndex = 51;
            this._Kontroll_7.Text = "Quellen";
            this._Kontroll_7.UseVisualStyleBackColor = false;
            this._Kontroll_7.Visible = false;
            this._Kontroll_5.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_5.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_5, 5);
            point2 = this._Kontroll_5.Location = new System.Drawing.Point(5, 156);
            padding2 = this._Kontroll_5.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_5.Name = "_Kontroll_5";
            this._Kontroll_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_5.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_5.TabIndex = 50;
            this._Kontroll_5.Text = "Quellen";
            this._Kontroll_5.UseVisualStyleBackColor = false;
            this._Befehl1_26.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_26.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_26.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._Befehl1_26.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this._Befehl1_26.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_26, 26);
            point2 = this._Befehl1_26.Location = new System.Drawing.Point(344, 670);
            padding2 = this._Befehl1_26.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_26.Name = "_Befehl1_26";
            this._Befehl1_26.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_26.Size = new System.Drawing.Size(176, 33);
            this._Befehl1_26.TabIndex = 19;
            this._Befehl1_26.Text = "Hauptmenue";
            this._Befehl1_26.UseVisualStyleBackColor = false;
            this._Kontroll_0.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_0.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_0, 0);
            point2 = this._Kontroll_0.Location = new System.Drawing.Point(5, 240);
            padding2 = this._Kontroll_0.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_0.Name = "_Kontroll_0";
            this._Kontroll_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_0.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_0.TabIndex = 18;
            this._Kontroll_0.Text = "Daten der Kinder";
            this._Kontroll_0.UseVisualStyleBackColor = false;
            this._Kontroll_0.Visible = false;
            this._Befehl1_21.BackColor = System.Drawing.Color.FromArgb(128, 255, 255);
            this._Befehl1_21.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_21.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this._Befehl1_21.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_21, 21);
            point2 = this._Befehl1_21.Location = new System.Drawing.Point(539, 670);
            padding2 = this._Befehl1_21.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_21.Name = "_Befehl1_21";
            this._Befehl1_21.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_21.Size = new System.Drawing.Size(133, 33);
            this._Befehl1_21.TabIndex = 0;
            this._Befehl1_21.Text = "Druckmenue";
            this._Befehl1_21.UseVisualStyleBackColor = false;
            this._Befehl1_16.BackColor = System.Drawing.SystemColors.Control;
            this._Befehl1_16.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_16.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_16, 16);
            point2 = this._Befehl1_16.Location = new System.Drawing.Point(558, 418);
            padding2 = this._Befehl1_16.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_16.Name = "_Befehl1_16";
            this._Befehl1_16.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_16.Size = new System.Drawing.Size(128, 29);
            this._Befehl1_16.TabIndex = 7;
            this._Befehl1_16.Text = "Start";
            this._Befehl1_16.UseVisualStyleBackColor = false;
            this._Befehl1_16.Visible = false;
            this._Befehl1_20.BackColor = System.Drawing.SystemColors.Control;
            this._Befehl1_20.Cursor = System.Windows.Forms.Cursors.Default;
            this._Befehl1_20.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Befehl1.SetIndex(this._Befehl1_20, 20);
            point2 = this._Befehl1_20.Location = new System.Drawing.Point(694, 418);
            padding2 = this._Befehl1_20.Margin = new System.Windows.Forms.Padding(4);
            this._Befehl1_20.Name = "_Befehl1_20";
            this._Befehl1_20.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Befehl1_20.Size = new System.Drawing.Size(128, 29);
            this._Befehl1_20.TabIndex = 1;
            this._Befehl1_20.Text = "Start";
            this._Befehl1_20.UseVisualStyleBackColor = false;
            this._Befehl1_20.Visible = false;
            this.bereich2.AcceptsReturn = true;
            this.bereich2.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.bereich2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bereich2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.bereich2.ForeColor = System.Drawing.Color.Black;
            point2 = this.bereich2.Location = new System.Drawing.Point(672, 155);
            padding2 = this.bereich2.Margin = new System.Windows.Forms.Padding(4);
            this.bereich2.MaxLength = 0;
            this.bereich2.Name = "bereich2";
            this.bereich2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.bereich2.Size = new System.Drawing.Size(74, 25);
            this.bereich2.TabIndex = 5;
            this.bereich2.Visible = false;
            this.Bereich1.AcceptsReturn = true;
            this.Bereich1.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bereich1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Bereich1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Bereich1.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bereich1.Location = new System.Drawing.Point(672, 129);
            padding2 = this.Bereich1.Margin = new System.Windows.Forms.Padding(4);
            this.Bereich1.MaxLength = 0;
            this.Bereich1.Name = "Bereich1";
            this.Bereich1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bereich1.Size = new System.Drawing.Size(74, 25);
            this.Bereich1.TabIndex = 6;
            this.Bereich1.Visible = false;
            this.bereich3.AcceptsReturn = true;
            this.bereich3.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.bereich3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bereich3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.bereich3.ForeColor = System.Drawing.Color.Black;
            point2 = this.bereich3.Location = new System.Drawing.Point(523, 209);
            padding2 = this.bereich3.Margin = new System.Windows.Forms.Padding(4);
            this.bereich3.MaxLength = 0;
            this.bereich3.Name = "bereich3";
            this.bereich3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.bereich3.Size = new System.Drawing.Size(309, 25);
            this.bereich3.TabIndex = 11;
            this.bereich3.Visible = false;
            this.bereich4.AcceptsReturn = true;
            this.bereich4.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.bereich4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bereich4.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.bereich4.ForeColor = System.Drawing.Color.Black;
            point2 = this.bereich4.Location = new System.Drawing.Point(523, 232);
            padding2 = this.bereich4.Margin = new System.Windows.Forms.Padding(4);
            this.bereich4.MaxLength = 0;
            this.bereich4.Name = "bereich4";
            this.bereich4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.bereich4.Size = new System.Drawing.Size(309, 25);
            this.bereich4.TabIndex = 12;
            this.bereich4.Visible = false;
            this.Option1.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Option1.Checked = true;
            this.Option1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Option1.ForeColor = System.Drawing.Color.Black;
            point2 = this.Option1.Location = new System.Drawing.Point(461, 275);
            padding2 = this.Option1.Margin = new System.Windows.Forms.Padding(4);
            this.Option1.Name = "Option1";
            this.Option1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Option1.Size = new System.Drawing.Size(371, 19);
            this.Option1.TabIndex = 13;
            this.Option1.TabStop = true;
            this.Option1.Text = "Sortiert nach Name, Vorname, Heiratsdatum";
            this.Option1.UseVisualStyleBackColor = false;
            this.Option1.Visible = false;
            this.Option2.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Option2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Option2.ForeColor = System.Drawing.Color.Black;
            point2 = this.Option2.Location = new System.Drawing.Point(461, 305);
            padding2 = this.Option2.Margin = new System.Windows.Forms.Padding(4);
            this.Option2.Name = "Option2";
            this.Option2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Option2.Size = new System.Drawing.Size(371, 19);
            this.Option2.TabIndex = 14;
            this.Option2.Text = "Sortiert nach Name, Heiratsdatum";
            this.Option2.UseVisualStyleBackColor = false;
            this.Option2.Visible = false;
            this._Kontroll_1.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_1.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_1, 1);
            point2 = this._Kontroll_1.Location = new System.Drawing.Point(5, 216);
            padding2 = this._Kontroll_1.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_1.Name = "_Kontroll_1";
            this._Kontroll_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_1.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_1.TabIndex = 15;
            this._Kontroll_1.Text = "Kinder";
            this._Kontroll_1.UseVisualStyleBackColor = false;
            this._Kontroll_1.Visible = false;
            this._Kontroll_3.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_3.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_3, 3);
            point2 = this._Kontroll_3.Location = new System.Drawing.Point(5, 84);
            padding2 = this._Kontroll_3.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_3.Name = "_Kontroll_3";
            this._Kontroll_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_3.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_3.TabIndex = 16;
            this._Kontroll_3.Text = "Personen und Familiennummern";
            this._Kontroll_3.UseVisualStyleBackColor = false;
            this._Kontroll_2.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_2.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_2, 2);
            point2 = this._Kontroll_2.Location = new System.Drawing.Point(3, 366);
            padding2 = this._Kontroll_2.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_2.Name = "_Kontroll_2";
            this._Kontroll_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_2.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_2.TabIndex = 17;
            this._Kontroll_2.Text = "Daten der Eltern und Ehepartner";
            this._Kontroll_2.UseVisualStyleBackColor = false;
            this._Kontroll_2.Visible = false;
            this._Kontroll_12.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_12.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_12.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_12, 12);
            point2 = this._Kontroll_12.Location = new System.Drawing.Point(344, 337);
            padding2 = this._Kontroll_12.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_12.Name = "_Kontroll_12";
            this._Kontroll_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_12.Size = new System.Drawing.Size(293, 57);
            this._Kontroll_12.TabIndex = 55;
            this._Kontroll_12.Text = "Formatierung bei Quellen und Bemerkungen beibehalten";
            this._Kontroll_12.UseVisualStyleBackColor = false;
            this._Kontroll_4.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this._Kontroll_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Kontroll_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._Kontroll_4.ForeColor = System.Drawing.Color.Black;
            this.Kontroll.SetIndex(this._Kontroll_4, 4);
            point2 = this._Kontroll_4.Location = new System.Drawing.Point(5, 181);
            padding2 = this._Kontroll_4.Margin = new System.Windows.Forms.Padding(4);
            this._Kontroll_4.Name = "_Kontroll_4";
            this._Kontroll_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Kontroll_4.Size = new System.Drawing.Size(293, 19);
            this._Kontroll_4.TabIndex = 77;
            this._Kontroll_4.Text = "Paten und Zeugen";
            this._Kontroll_4.UseVisualStyleBackColor = false;
            this._Label2_2.BackColor = System.Drawing.Color.Red;
            this._Label2_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label2_2.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this._Label2_2.ForeColor = System.Drawing.Color.White;
            this.Label2.SetIndex(this._Label2_2, 2);
            point2 = this._Label2_2.Location = new System.Drawing.Point(0, 44);
            padding2 = this._Label2_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label2_2.Name = "_Label2_2";
            this._Label2_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label2_2.Size = new System.Drawing.Size(1016, 21);
            this._Label2_2.TabIndex = 65;
            this._Label2_2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label2_1.BackColor = System.Drawing.Color.Red;
            this._Label2_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label2_1.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this._Label2_1.ForeColor = System.Drawing.Color.White;
            this.Label2.SetIndex(this._Label2_1, 1);
            point2 = this._Label2_1.Location = new System.Drawing.Point(0, 22);
            padding2 = this._Label2_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label2_1.Name = "_Label2_1";
            this._Label2_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label2_1.Size = new System.Drawing.Size(1016, 21);
            this._Label2_1.TabIndex = 64;
            this._Label2_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Label2_0.BackColor = System.Drawing.Color.Red;
            this._Label2_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label2_0.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            this._Label2_0.ForeColor = System.Drawing.Color.White;
            this.Label2.SetIndex(this._Label2_0, 0);
            point2 = this._Label2_0.Location = new System.Drawing.Point(0, 0);
            padding2 = this._Label2_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label2_0.Name = "_Label2_0";
            this._Label2_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Label2_0.Size = new System.Drawing.Size(1016, 21);
            this._Label2_0.TabIndex = 63;
            this._Label2_0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Bez_5.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this._Bez_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bez_5.ForeColor = System.Drawing.Color.Black;
            this.Bez.SetIndex(this._Bez_5, 5);
            point2 = this._Bez_5.Location = new System.Drawing.Point(306, 102);
            padding2 = this._Bez_5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Bez_5.Name = "_Bez_5";
            this._Bez_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Bez_5.Size = new System.Drawing.Size(43, 19);
            this._Bez_5.TabIndex = 62;
            this._Bez_5.Text = "bis";
            this._Bez_5.Visible = false;
            this.Bezeichnung4.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung4.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung4.Location = new System.Drawing.Point(555, 157);
            padding2 = this.Bezeichnung4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung4.Name = "Bezeichnung4";
            this.Bezeichnung4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung4.Size = new System.Drawing.Size(117, 19);
            this.Bezeichnung4.TabIndex = 2;
            this.Bezeichnung4.Text = "bis: 31.12.";
            this.Bezeichnung4.Visible = false;
            this.Bezeichnung6.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung6.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung6.Location = new System.Drawing.Point(555, 109);
            padding2 = this.Bezeichnung6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung6.Name = "Bezeichnung6";
            this.Bezeichnung6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung6.Size = new System.Drawing.Size(256, 19);
            this.Bezeichnung6.TabIndex = 4;
            this.Bezeichnung6.Text = "Zeitraum -leer = alles";
            this.Bezeichnung6.Visible = false;
            this.Bezeichnung5.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.Bezeichnung5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung5.ForeColor = System.Drawing.Color.Black;
            point2 = this.Bezeichnung5.Location = new System.Drawing.Point(555, 129);
            padding2 = this.Bezeichnung5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Bezeichnung5.Name = "Bezeichnung5";
            this.Bezeichnung5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this.Bezeichnung5.Size = new System.Drawing.Size(117, 19);
            this.Bezeichnung5.TabIndex = 3;
            this.Bezeichnung5.Text = "vom: 01.01.";
            this.Bezeichnung5.Visible = false;
            this._Bez_1.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this._Bez_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bez_1.ForeColor = System.Drawing.Color.Black;
            this.Bez.SetIndex(this._Bez_1, 1);
            point2 = this._Bez_1.Location = new System.Drawing.Point(480, 187);
            padding2 = this._Bez_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Bez_1.Name = "_Bez_1";
            this._Bez_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Bez_1.Size = new System.Drawing.Size(341, 19);
            this._Bez_1.TabIndex = 8;
            this._Bez_1.Text = "Nummer von bis leer = alles";
            this._Bez_1.Visible = false;
            this._Bez_2.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this._Bez_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bez_2.ForeColor = System.Drawing.Color.Black;
            this.Bez.SetIndex(this._Bez_2, 2);
            point2 = this._Bez_2.Location = new System.Drawing.Point(480, 206);
            padding2 = this._Bez_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Bez_2.Name = "_Bez_2";
            this._Bez_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Bez_2.Size = new System.Drawing.Size(43, 19);
            this._Bez_2.TabIndex = 9;
            this._Bez_2.Text = "von";
            this._Bez_2.Visible = false;
            this._Bez_3.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this._Bez_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bez_3.ForeColor = System.Drawing.Color.Black;
            this.Bez.SetIndex(this._Bez_3, 3);
            point2 = this._Bez_3.Location = new System.Drawing.Point(480, 234);
            padding2 = this._Bez_3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Bez_3.Name = "_Bez_3";
            this._Bez_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            size2 = this._Bez_3.Size = new System.Drawing.Size(43, 19);
            this._Bez_3.TabIndex = 10;
            this._Bez_3.Text = "bis";
            this._Bez_3.Visible = false;
            this.CheckBox6.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox6.Location = new System.Drawing.Point(370, 627);
            this.CheckBox6.Name = "CheckBox6";
            size2 = this.CheckBox6.Size = new System.Drawing.Size(316, 21);
            this.CheckBox6.TabIndex = 110;
            this.CheckBox6.Text = "untere Bemerkungen zum Familiendatum";
            this.CheckBox6.UseVisualStyleBackColor = false;
            this.CheckBox5.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox5.Location = new System.Drawing.Point(370, 599);
            this.CheckBox5.Name = "CheckBox5";
            size2 = this.CheckBox5.Size = new System.Drawing.Size(316, 21);
            this.CheckBox5.TabIndex = 109;
            this.CheckBox5.Text = "obere Bemerkungen zum Familiendatum";
            this.CheckBox5.UseVisualStyleBackColor = false;
            this.CheckBox4.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox4.Location = new System.Drawing.Point(370, 571);
            this.CheckBox4.Name = "CheckBox4";
            size2 = this.CheckBox4.Size = new System.Drawing.Size(316, 21);
            this.CheckBox4.TabIndex = 108;
            this.CheckBox4.Text = "Familienbemerkungen";
            this.CheckBox4.UseVisualStyleBackColor = false;
            this.CheckBox3.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox3.Location = new System.Drawing.Point(370, 544);
            this.CheckBox3.Name = "CheckBox3";
            size2 = this.CheckBox3.Size = new System.Drawing.Size(316, 21);
            this.CheckBox3.TabIndex = 107;
            this.CheckBox3.Text = "untere Bemerkungen z.  Personendatum";
            this.CheckBox3.UseVisualStyleBackColor = false;
            this.CheckBox2.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox2.Location = new System.Drawing.Point(370, 516);
            this.CheckBox2.Name = "CheckBox2";
            size2 = this.CheckBox2.Size = new System.Drawing.Size(316, 21);
            this.CheckBox2.TabIndex = 106;
            this.CheckBox2.Text = "obere Bemerkungen zum Personendatum";
            this.CheckBox2.UseVisualStyleBackColor = false;
            this.CheckBox1.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox1.Location = new System.Drawing.Point(370, 489);
            this.CheckBox1.Name = "CheckBox1";
            size2 = this.CheckBox1.Size = new System.Drawing.Size(316, 21);
            this.CheckBox1.TabIndex = 105;
            this.CheckBox1.Text = "Personenbemerkungen";
            this.CheckBox1.UseVisualStyleBackColor = false;
            this.Panel1.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            this.Panel1.Controls.Add(this.Label7);
            this.Panel1.Controls.Add(this.RadioButton3);
            this.Panel1.Controls.Add(this.RadioButton1);
            this.Panel1.Controls.Add(this.RadioButton2);
            point2 = this.Panel1.Location = new System.Drawing.Point(729, 489);
            this.Panel1.Name = "Panel1";
            size2 = this.Panel1.Size = new System.Drawing.Size(122, 127);
            this.Panel1.TabIndex = 143;
            point2 = this.Label7.Location = new System.Drawing.Point(3, 6);
            this.Label7.Name = "Label7";
            size2 = this.Label7.Size = new System.Drawing.Size(116, 36);
            this.Label7.TabIndex = 142;
            this.Label7.Text = "Vertraulichkeit der Information";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RadioButton3.AutoSize = true;
            this.RadioButton3.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            point2 = this.RadioButton3.Location = new System.Drawing.Point(6, 95);
            this.RadioButton3.Name = "RadioButton3";
            size2 = this.RadioButton3.Size = new System.Drawing.Size(80, 21);
            this.RadioButton3.TabIndex = 141;
            this.RadioButton3.Text = "gesperrt";
            this.RadioButton3.UseVisualStyleBackColor = false;
            this.RadioButton1.AutoSize = true;
            this.RadioButton1.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            this.RadioButton1.Checked = true;
            point2 = this.RadioButton1.Location = new System.Drawing.Point(6, 51);
            this.RadioButton1.Name = "RadioButton1";
            size2 = this.RadioButton1.Size = new System.Drawing.Size(46, 21);
            this.RadioButton1.TabIndex = 139;
            this.RadioButton1.TabStop = true;
            this.RadioButton1.Text = "frei";
            this.RadioButton1.UseVisualStyleBackColor = false;
            this.RadioButton2.AutoSize = true;
            this.RadioButton2.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            point2 = this.RadioButton2.Location = new System.Drawing.Point(6, 73);
            this.RadioButton2.Name = "RadioButton2";
            size2 = this.RadioButton2.Size = new System.Drawing.Size(61, 21);
            this.RadioButton2.TabIndex = 140;
            this.RadioButton2.Text = "privat";
            this.RadioButton2.UseVisualStyleBackColor = false;
            this.CheckBox7.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox7.Location = new System.Drawing.Point(5, 313);
            this.CheckBox7.Name = "CheckBox7";
            size2 = this.CheckBox7.Size = new System.Drawing.Size(293, 19);
            this.CheckBox7.TabIndex = 146;
            this.CheckBox7.Text = "Eltern";
            this.CheckBox7.UseVisualStyleBackColor = false;
            this.CheckBox8.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            point2 = this.CheckBox8.Location = new System.Drawing.Point(5, 340);
            this.CheckBox8.Name = "CheckBox8";
            size2 = this.CheckBox8.Size = new System.Drawing.Size(293, 19);
            this.CheckBox8.TabIndex = 147;
            this.CheckBox8.Text = "Ehepartner";
            this.CheckBox8.UseVisualStyleBackColor = false;
            System.Drawing.SizeF sizeF2 = this.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(192, 192, 192);
            this.CancelButton = this._Befehl1_26;
            size2 = this.ClientSize = new System.Drawing.Size(1018, 725);
            this.Controls.Add(this.Frame3);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this._Kontroll_10);
            this.Controls.Add(this._Kontroll_8);
            this.Controls.Add(this._Kontroll_7);
            this.Controls.Add(this._Kontroll_5);
            this.Controls.Add(this._Befehl1_26);
            this.Controls.Add(this._Kontroll_0);
            this.Controls.Add(this._Befehl1_21);
            this.Controls.Add(this._Befehl1_16);
            this.Controls.Add(this._Befehl1_20);
            this.Controls.Add(this.bereich2);
            this.Controls.Add(this.Bereich1);
            this.Controls.Add(this.bereich3);
            this.Controls.Add(this.bereich4);
            this.Controls.Add(this.Option1);
            this.Controls.Add(this.Option2);
            this.Controls.Add(this._Kontroll_1);
            this.Controls.Add(this._Kontroll_3);
            this.Controls.Add(this._Kontroll_2);
            this.Controls.Add(this._Kontroll_12);
            this.Controls.Add(this._Kontroll_4);
            this.Controls.Add(this._Label2_2);
            this.Controls.Add(this._Label2_1);
            this.Controls.Add(this._Label2_0);
            this.Controls.Add(this._Bez_5);
            this.Controls.Add(this.Bezeichnung4);
            this.Controls.Add(this.Bezeichnung6);
            this.Controls.Add(this.Bezeichnung5);
            this.Controls.Add(this._Bez_1);
            this.Controls.Add(this._Bez_2);
            this.Controls.Add(this._Bez_3);
            this.Controls.Add(this._Kontroll_11);
            this.Controls.Add(this.CheckBox1);
            this.Controls.Add(this.CheckBox2);
            this.Controls.Add(this.CheckBox3);
            this.Controls.Add(this.CheckBox4);
            this.Controls.Add(this.CheckBox5);
            this.Controls.Add(this.Frame2);
            this.Controls.Add(this.CheckBox6);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.CheckBox7);
            this.Controls.Add(this.CheckBox8);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            padding2 = this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Ausw";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Sortierte Listen";
            this.Frame3.ResumeLayout(false);
            this.Frame3.PerformLayout();
            this.Frame1.ResumeLayout(false);
            this.Frame2.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.Befehl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Bez).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Command1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Kontroll).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Label2).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.Option3).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Befehl1_Click(object eventSender, EventArgs eventArgs)
        {
            //Discarded unreachable code: IL_129d
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            byte b = default;
            int start = default;
            int num5 = default;
            int ubg = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    checked
                    {
                        int num4;
                        string text;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                index = Befehl1.GetIndex((Button)eventSender);
                                goto IL_0016;
                            case 6067:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_12a1;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    if (Information.Err().Number != 5)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_12a1;
                                }
                            end_IL_0000:
                                break;
                            IL_0016:
                                num = 2;
                                Frame1.Visible = false;
                                switch (index)
                                {
                                    case 0:
                                        _Modul1.Instance.Ubg = 500;
                                        weiter(_Modul1.Instance.Ubg);
                                        break;
                                    case 1:
                                        goto IL_00dd;
                                    case 2:
                                        goto IL_00f7;
                                    case 3:
                                        goto IL_0159;
                                    case 4:
                                        goto IL_0173;
                                    case 5:
                                        goto IL_018d;
                                    case 6:
                                        goto IL_01a7;
                                    case 7:
                                        goto IL_01c1;
                                    case 8:
                                        goto IL_01de;
                                    case 9:
                                        goto IL_01fb;
                                    case 10:
                                        goto IL_0218;
                                    case 11:
                                        goto IL_0235;
                                    case 12:
                                        goto IL_029a;
                                    case 13:
                                        goto IL_02ff;
                                    case 14:
                                        goto IL_031c;
                                    case 15:
                                        goto IL_0339;
                                    case 16:
                                        goto IL_0356;
                                    case 17:
                                        goto IL_05b2;
                                    case 18:
                                        goto IL_05cf;
                                    case 19:
                                        goto IL_05ec;
                                    case 20:
                                        goto IL_060f;
                                    case 21:
                                        goto IL_0d77;
                                    case 22:
                                        goto IL_0d89;
                                    case 23:
                                        goto IL_0dac;
                                    case 24:
                                        goto IL_0dcf;
                                    case 25:
                                        goto IL_0def;
                                    case 26:
                                        goto IL_0e0f;
                                    case 28:
                                        goto IL_0e89;
                                    case 29:
                                        goto IL_0ea9;
                                    case 30:
                                        goto IL_0ecc;
                                    case 31:
                                        goto IL_105f;
                                    case 32:
                                        goto IL_107f;
                                    case 27:
                                        break;
                                    default:
                                        goto IL_1247;
                                }
                                goto end_IL_0000_2;
                            IL_00dd:
                                num = 12;
                                _Modul1.Instance.Ubg = 120;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_00f7:
                                num = 16;
                                _Modul1.Instance.Ubg = 121;
                                Option1.Text = "Sortiert nach Name, Vorname, Geburtsdatum";
                                Option2.Text = "Sortiert nach Name, Geburtsdatum";
                                Option1.Visible = true;
                                Option2.Visible = true;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0159:
                                num = 24;
                                _Modul1.Instance.Ubg = 101;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0173:
                                num = 28;
                                _Modul1.Instance.Ubg = 102;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_018d:
                                num = 32;
                                _Modul1.Instance.Ubg = 103;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_01a7:
                                num = 36;
                                _Modul1.Instance.Ubg = 104;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_01c1:
                                num = 40;
                                _Modul1.Instance.Ubg = 501;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_01de:
                                num = 44;
                                _Modul1.Instance.Ubg = 504;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_01fb:
                                num = 48;
                                _Modul1.Instance.Ubg = 1101;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0218:
                                num = 52;
                                _Modul1.Instance.Ubg = 401;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0235:
                                num = 56;
                                _Modul1.Instance.Ubg = 402;
                                Option1.Text = "Sortiert nach Name, Vorname, Heiratsdatum";
                                Option2.Text = "Sortiert nach Name, Heiratsdatum";
                                Option1.Visible = true;
                                Option2.Visible = true;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_029a:
                                num = 64;
                                _Modul1.Instance.Ubg = 403;
                                Option1.Text = "Sortiert nach Name, Vorname, Heiratsdatum";
                                Option2.Text = "Sortiert nach Name, Heiratsdatum";
                                Option1.Visible = true;
                                Option2.Visible = true;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_02ff:
                                num = 72;
                                _Modul1.Instance.Ubg = 502;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_031c:
                                num = 76;
                                _Modul1.Instance.Ubg = 503;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0339:
                                num = 80;
                                _Modul1.Instance.Ubg = 1102;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0356:
                                num = 84;
                                MyProject.Forms.Hinter.List1.Items.Clear();
                                ubg = _Modul1.Instance.Ubg;
                                if (ubg is not 122 and not 119 and not 120 and not 135 and not 401)
                                {
                                    MyProject.Forms.ortw.Top = Label2[2].Top + (Label2[2].Height + 35);
                                    MyProject.Forms.ortw.ShowDialog();
                                }
                                MyProject.Forms.ortw.Close();
                                weiter(_Modul1.Instance.Ubg);
                                if (Kontroll[1].Checked)
                                {
                                }
                                if ((_Modul1.Instance.Ubg == 120) | (_Modul1.Instance.Ubg == 122) | (_Modul1.Instance.Ubg == 125) | (_Modul1.Instance.Ubg == 401))
                                {
                                    if (bereich3.Text.AsDouble() == 0.0)
                                    {
                                        bereich3.Text = "1";
                                    }
                                    text = bereich3.Text.AsDouble().AsString();
                                    M_Ubg1 = (int)Math.Round(bereich4.Text.AsDouble());
                                    goto IL_056b;
                                }
                                if ((_Modul1.Instance.Ubg == 119) | (_Modul1.Instance.Ubg == 121) | (_Modul1.Instance.Ubg == 123) | (_Modul1.Instance.Ubg == 130) | (_Modul1.Instance.Ubg == 402) | (_Modul1.Instance.Ubg == 403))
                                {
                                    if (Option1.Checked)
                                    {
                                    }
                                    _Modul1.Instance.UbgT = bereich3.Text;
                                    _Modul1.Instance.Ubg1T = bereich4.Text;
                                }
                                goto IL_056b;
                            IL_056b: // <========== 3
                                num = 114;
                                Hide();
                                if (_Modul1.Instance.Ubg != 125)
                                {
                                }
                                MyProject.Forms.Anzeige.Show();
                                MyProject.Forms.Anzeige.Button1.PerformClick();
                                goto end_IL_0000_2;
                            IL_05b2:
                                num = 121;
                                _Modul1.Instance.Ubg = 1103;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_05cf:
                                num = 125;
                                _Modul1.Instance.Ubg = 1104;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_05ec:
                                num = 129;
                                _Modul1.Instance.Ubg = 1502;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_060f:
                                num = 133;
                                FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Druck_ini.dat", OpenMode.Output);
                                Read_CheckBoxState(_Modul1.Instance.Aus);
                                b = 1;
                                while (unchecked(b) <= 52u)
                                {
                                    FileSystem.PrintLine(99, _Modul1.Instance.Aus[b]);
                                    b = (byte)unchecked((uint)(b + 1));
                                }
                                if (RadioButton1.Checked)
                                {
                                    iM_Priv_aus = 0;
                                }
                                if (RadioButton2.Checked)
                                {
                                    iM_Priv_aus = 1;
                                }
                                if (RadioButton3.Checked)
                                {
                                    iM_Priv_aus = 2;
                                }
                                FileSystem.FileClose(99);
                                MyProject.Forms.Hinter.List1.Items.Clear();
                                switch (_Modul1.Instance.Ubg)
                                {
                                    case 119:
                                    case 120:
                                    case 122:
                                    case 135:
                                        break;
                                    default:
                                        MyProject.Forms.ortw.Top = Label2[2].Top + (Label2[2].Height + 35);
                                        MyProject.Forms.ortw.ShowDialog();
                                        break;
                                }
                                goto IL_09c0;
                            IL_09c0: // <========== 3
                                num = 191;
                                MyProject.Forms.ortw.Close();
                                if (Kontroll[1].Checked)
                                {
                                }
                                start = 0;
                                if ((Bereich1.Text.AsDouble() > 0.0) & (Bereich1.Text.TrimEnd().Length < 4))
                                {
                                    Interaction.MsgBox("Falsche Eingabe, bei Jahren unter 1000 führende Nullen eingeben");
                                    Bereich1.Text = "";
                                    Bereich1.Focus();
                                    MyProject.Forms.Anzeige.Button1.PerformClick();
                                    goto end_IL_0000_2;
                                }
                                if (Bereich1.Text.AsDouble() > 0.0)
                                {
                                    start = (int)Math.Round((Bereich1.Text + "0000").AsDouble() - 1.0);
                                }
                                num5 = 20090000;
                                if ((bereich2.Text.AsDouble() > 0.0) & (bereich2.Text.TrimEnd().Length < 4))
                                {
                                    Interaction.MsgBox("Bei Jahren unter 1000 führende Nullen eingeben", MsgBoxStyle.OkOnly, "Falsche Eingabe,");
                                    bereich2.Text = "";
                                    bereich2.Focus();
                                    goto end_IL_0000_2;
                                }
                                num5 = (Support.Format(DateTime.Today, "Long Date").Right(4) + "0000").AsInt();
                                bereich2.Text += "0000";
                                if ((bereich2.Text.AsDouble() > 0.0) & (bereich2.Text.AsDouble() < Conversion.Val(num5.AsString())))
                                {
                                    num5 = (int)Math.Round(bereich2.Text.AsDouble());
                                }
                                M_Start = start;
                                M_Ubg1 = num5;
                                if ((Bereich1.Text.AsDouble() > 0.0) & (bereich2.Text.AsDouble() > 0.0))
                                {
                                    if (bereich2.Text.AsDouble() - Bereich1.Text.AsDouble() < 0.0)
                                    {
                                        Interaction.MsgBox("Anfangsjahr muß kleiner wie Endejahr sein", MsgBoxStyle.OkOnly, "Falsche Eingabe,");
                                        Bereich1.Text = "";
                                        bereich2.Text = "";
                                        Bereich1.Focus();
                                    
                                    }
                                }
                                else
                                {
                                    Hide();
                                MyProject.Forms.Anzeige.Show();
                                MyProject.Forms.Anzeige.Button1.PerformClick();
                                }
                                goto end_IL_0000_2;
                            IL_0d77:
                                num = 235;
                                Close();
                                goto end_IL_0000_2;
                            IL_0d89:
                                num = 238;
                                _Modul1.Instance.Ubg = 1503;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0dac:
                                num = 242;
                                _Modul1.Instance.Ubg = 505;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0dcf:
                                num = 246;
                                _Modul1.Instance.Ubg = 122;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0def:
                                num = 250;
                                _Modul1.Instance.Ubg = 123;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0e0f:
                                num = 254;
                                DataModul.MandDB.Close();
                                DataModul.DOSB.Close();
                                DataModul.TempDB.Close();
                                DataModul.DSB.Close();
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                Interaction.Shell(_Modul1.Instance.GenFreeDir + "Gen_Plus.exe", AppWinStyle.NormalFocus);
                                ProjectData.EndApp();
                                goto end_IL_0000_2;
                            IL_0e89:
                                num = 265;
                                _Modul1.Instance.Ubg = 119;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0ea9:
                                num = 269;
                                _Modul1.Instance.Ubg = 130;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_0ecc:
                                num = 273;
                                bereich3.Text = "";
                                bereich4.Text = "";
                                Command1[0].Visible = false;
                                Command1[1].Visible = false;
                                Command1[2].Visible = true;
                                Frame3.Top = 86;
                                Frame3.Left = 1;
                                Frame3.BackColor = _Modul1.Instance.HintFarb;
                                Kontroll[1].Text = "Kinder";
                                Kontroll[0].Text = "Daten der Kinder";
                                Kontroll[1].Visible = true;
                                Kontroll[3].Text = "Personen- und Familiennummern";
                                Kontroll[2].Visible = true;
                                Kontroll[3].Visible = true;
                                Kontroll[2].Text = "Daten der Eltern und Ehepartner";
                                Frame3.Visible = true;
                                goto end_IL_0000_2;
                            IL_105f:
                                num = 291;
                                _Modul1.Instance.Ubg = 124;
                                weiter(_Modul1.Instance.Ubg);
                                goto end_IL_0000_2;
                            IL_107f:
                                num = 295;
                                bereich3.Text = "";
                                bereich4.Text = "";
                                Command1[0].Visible = true;
                                Command1[1].Visible = true;
                                Command1[2].Visible = false;
                                Frame3.Top = 86;
                                Frame3.Left = 1;
                                Frame3.BackColor = _Modul1.Instance.HintFarb;
                                Frame3.Top = Label2[2].Top + 25;
                                Kontroll[1].Text = "Kinder";
                                Kontroll[0].Text = "Daten der Kinder";
                                Kontroll[1].Visible = true;
                                Kontroll[3].Text = "Personen- und Familiennummern";
                                Kontroll[2].Visible = true;
                                Kontroll[3].Visible = true;
                                Kontroll[2].Text = "Daten der Eltern und Ehepartner";
                                Frame3.Visible = true;
                                Frame3.Refresh();
                                goto end_IL_0000_2;
                            IL_1247:
                                num = 315;
                                Interaction.MsgBox(index.AsString());
                                goto end_IL_0000_2;
                            IL_12a1:
                                num4 = unchecked(num2 + 1);
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 106:
                                    case 113:
                                    case 114:
                                        goto IL_056b;
                                    case 183:
                                    case 184:
                                    case 186:
                                    case 190:
                                    case 191:
                                        goto IL_09c0;
                                    case 5:
                                    case 10:
                                    case 14:
                                    case 22:
                                    case 26:
                                    case 30:
                                    case 34:
                                    case 38:
                                    case 42:
                                    case 46:
                                    case 50:
                                    case 54:
                                    case 62:
                                    case 70:
                                    case 74:
                                    case 78:
                                    case 82:
                                    case 119:
                                    case 123:
                                    case 127:
                                    case 131:
                                    case 202:
                                    case 212:
                                    case 227:
                                    case 233:
                                    case 236:
                                    case 240:
                                    case 244:
                                    case 248:
                                    case 252:
                                    case 260:
                                    case 261:
                                    case 262:
                                    case 263:
                                    case 267:
                                    case 271:
                                    case 289:
                                    case 293:
                                    case 313:
                                    case 316:
                                    case 317:
                                    case 320:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 6067;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 39
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Bereich1_TextChanged(object eventSender, EventArgs eventArgs)
        {
            if (Bereich1.Text.TrimEnd().Length == 4)
            {
                bereich2.Focus();
            }
        }

        private void Bereich1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
        {
            short num = checked((short)Strings.Asc(eventArgs.KeyChar));
            if (num == 13)
            {
                bereich2.Focus();
            }
            eventArgs.KeyChar = Strings.Chr(num);
            if (num == 0)
            {
                eventArgs.Handled = true;
            }
        }

        private void bereich2_TextChanged(object eventSender, EventArgs eventArgs)
        {
            if (bereich2.Text.TrimEnd().Length == 4)
            {
                Befehl1[20].Focus();
            }
        }

        private void bereich2_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
        {
            short num = checked((short)Strings.Asc(eventArgs.KeyChar));
            if (num == 13)
            {
                Kontroll[1].Focus();
            }
            eventArgs.KeyChar = Strings.Chr(num);
            if (num == 0)
            {
                eventArgs.Handled = true;
            }
        }

        private void bereich3_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
        {
            short num = checked((short)Strings.Asc(eventArgs.KeyChar));
            if (num == 13)
            {
                bereich4.Focus();
            }
            eventArgs.KeyChar = Strings.Chr(num);
            if (num == 0)
            {
                eventArgs.Handled = true;
            }
        }

        private void bereich4_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
        {
            short num = checked((short)Strings.Asc(eventArgs.KeyChar));
            if (Kontroll[1].Visible && num == 13)
            {
                Kontroll[1].Focus();
            }
            eventArgs.KeyChar = Strings.Chr(num);
            if (num == 0)
            {
                eventArgs.Handled = true;
            }
        }

        private void Command1_Click(object eventSender, EventArgs eventArgs)
        {
            //Discarded unreachable code: IL_0248, IL_024d
            int index = Command1.GetIndex((Button)eventSender);
            string text = "";
            string text2 = "";
            switch (index)
            {
                case 0:
                    {
                        text2 = "E";
                        string text4 = _Modul1.Instance.IText[185] + new string(' ', 100) + "E";
                        Label4.Text = _Modul1.Instance.IText[185];
                        break;
                    }
                case 1:
                    {
                        text2 = "G";
                        string text3 = _Modul1.Instance.IText[70] + " >" + Text1.Text.Trim() + "<";
                        Label4.Text = _Modul1.Instance.IText[70];
                        break;
                    }
                case 2:
                    text2 = "T";
                    Label4.Text = "Sonstiges Ereignis";
                    break;
            }
            List1.Items.Clear();
            if (DataModul.DB_TexteTable.RecordCount <= 0)
            {
                Interaction.MsgBox("Keine Daten");
                return;
            }
            DataModul.DB_TexteTable.Index = "KText";
            DataModul.DB_TexteTable.MoveFirst();
            DataModul.DB_TexteTable.Seek("=", text2);
            if (Text1.Text != "")
            {
                _Modul1.Instance.UbgT = Text1.Text.ToUpper();
                DataModul.DB_TexteTable.Index = "STexte";
                DataModul.DB_TexteTable.Seek(">=", text2, _Modul1.Instance.UbgT);
            }
            while (!DataModul.DB_TexteTable.EOF && !DataModul.DB_TexteTable.NoMatch)
            {
                if (Text1.Text != "")
                {
                    if (DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsString() != text2)
                    {
                        break;
                    }
                    Type typeFromHandle = typeof(Strings);
                    object[] array = new object[1];
                    IField field = DataModul.DB_TexteTable.Fields[TexteFields.Txt];
                    array[0] = field.Value;
                    object[] array2 = array;
                    bool[] array3 = new bool[1] { true };
                    object obj = NewLateBinding.LateGet(null, typeFromHandle, "UCase", array2, null, null, array3);
                    if (array3[0])
                    {
                        field.Value = array2[0];
                    }
                    if (obj.AsString().Left(1) != _Modul1.Instance.UbgT.Left(1))
                    {
                        break;
                    }
                }
                else if (DataModul.DB_TexteTable.Fields[TexteFields.Kennz].AsString() != text2)
                {
                    break;
                }
                string expression = DataModul.DB_TexteTable.Fields[TexteFields.Txt].AsString();
                var LiText = Strings.Left(expression.Replace("ssss", "ß").TrimStart() + new string(' ', 240), 240) + DataModul.DB_TexteTable.Fields[TexteFields.TxNr].AsString().TrimStart();
                List1.Items.Add(LiText);
                DataModul.DB_TexteTable.MoveNext();
                text += ">";
                if (text.Length == 30)
                {
                    text = "";
                }
                Bezeichnung6.Text = text;
                Bezeichnung6.Refresh();
            }
        }

        private void Command2_Click(object eventSender, EventArgs eventArgs)
        {
            if (List2.Items.Count == 0)
            {
                Interaction.MsgBox("Kein Begriff ausgewählt");
                return;
            }
            _Modul1.Instance.Ubg = 135;
            Frame3.Visible = false;
            Befehl1[16].Visible = true;
        }

        private void Ausw_Load(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num3 = default;
            int num2 = default;
            int num = default;
            int number = default;
            int num5 = default;
            string source = default;
            string destination = default;
            string name = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                //    Hinter hinter;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 3621:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 3:
                                        break;
                                    case 2:
                                    case 4:
                                        goto IL_0a15;
                                    case 1:
                                        goto IL_0b5f;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 340)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0b5f;
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
                                goto IL_0b5b;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            Label2[0].Text = _Modul1.Instance.VersionT;
                            Label2[1].Text = "(c) 1994-2018 Gisbert Berwe 49082 Osnabrück Friedrich-Holthaus-Str. 18 Tel.: 0541-80 00 79 00";
                            Label2[2].Text = "Version 24.09.04 Stand 25.11.2018";
                            Kontroll[7].Enabled = true;
                            Befehl1[24].Enabled = false;
                            BackColor = _Modul1.Instance.HintFarb;
                            Frame1.BackColor = _Modul1.Instance.HintFarb;
                            Frame1.Top = checked(Label2[2].Top + 25);
                            Frame1.Width = 1024;
                            Frame1.Height = 600;
                            _Modul1.Instance.eWindowState = _Modul1.Instance.Persistence.ReadEnumInit<FormWindowState>("Windowstate");
                            WindowState = _Modul1.Instance.eWindowState.AsEnum<FormWindowState>();
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
                                _ => 18.0f,
                            };
                            goto IL_0328;
                        IL_0328: // <========== 12
                            num = 59;
                            FileSystem.FileClose(99);
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            FileSystem.FileClose(6);
                            Show();
                            Label2[0].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Bold);
                            Label2[1].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Bold);
                            Label2[2].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Bold);
                            ProjectData.ClearProjectError();
                            num3 = 3;
                            num5 = 0;
                            while (num5 <= 32)
                            {
                                Befehl1[checked((short)num5)].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                                num5 = checked(num5 + 1);
                            }
                            Label1[0].Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Bezeichnung1.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Bezeichnung2.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Bezeichnung3.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Label5.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            RichTextBox1.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Button1.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Button4.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Button5.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Button6.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Button8.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Label6.Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            ProjectData.ClearProjectError();
                            num3 = 4;
                            Frame1.Left = 0;
                            DataModul.DSB_SortTable.Close();
                            DataModul.DSB_NamIdxTable.Close();
                            DataModul.DSB_OrtIdxTable.Close();
                            DataModul.DSB_PerStatTable.Close();
                            DataModul.DSB_FamStatTable.Close();
                            DataModul.DSB_QuellIdxTable.Close();
                            DataModul.TempSort_DB.Close();
                            var hinter = MyProject.Forms.Hinter;
                            hinter.Att(_Modul1.Instance.Verz + "Temp");
                            FileSystem.Kill(_Modul1.Instance.GenFreeDir + "TEMP\\*.*");
                            source = _Modul1.Instance.GenFreeDir + "INIT\\SortTem.mdb";
                            destination = _Modul1.Instance.GenFreeDir + "TEMP\\SortTem.mdb";
                            FileSystem.FileCopy(source, destination);
                            name = _Modul1.Instance.GenFreeDir + "TEMP\\SortTem.mdb";
                            DataModul.TempSort_DB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                            DataModul.TempSort_DB.Execute($"CREATE UNIQUE INDEX Nr ON {dbTables.Sort} (Nr);");
                            DataModul.DSB_SortTable = DataModul.TempSort_DB.OpenRecordset(dbTables.Sort, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_NamIdxTable = DataModul.TempSort_DB.OpenRecordset(dbTables.NamInd, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_OrtIdxTable = DataModul.TempSort_DB.OpenRecordset(dbTables.OrtIndex, RecordsetTypeEnum.dbOpenTable);
                            DataModul.DSB_SortTable.Index = "Sort";
                            _Modul1.Instance.Dateienopen();
                            CheckBox1.CheckState = CheckState.Unchecked;
                            CheckBox2.CheckState = CheckState.Unchecked;
                            CheckBox3.CheckState = CheckState.Unchecked;
                            CheckBox4.CheckState = CheckState.Unchecked;
                            CheckBox5.CheckState = CheckState.Unchecked;
                            CheckBox6.CheckState = CheckState.Unchecked;
                            if (_Modul1.Instance.Aus[1] == "Y")
                            {
                                CheckBox1.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[2] == "Y")
                            {
                                CheckBox2.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[3] == "Y")
                            {
                                CheckBox3.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[4] == "Y")
                            {
                                CheckBox4.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[5] == "Y")
                            {
                                CheckBox5.CheckState = CheckState.Checked;
                            }
                            if (_Modul1.Instance.Aus[6] == "Y")
                            {
                                CheckBox6.CheckState = CheckState.Checked;
                            }
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            DataModul.DT_AncesterTable.MoveLast();
                            if (!DataModul.DT_AncesterTable.NoMatch)
                            {
                                if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() > 0)
                                {
                                    Befehl1[24].Enabled = true;

                                }
                            }
                            goto IL_0950;
                        IL_0950: // <========== 3
                            num = 135;
                            Show();
                            Kontroll[2].CheckState = CheckState.Checked;
                            Kontroll[2].CheckState = CheckState.Unchecked;
                            goto end_IL_0000_2;
                        IL_0a15:
                            num = 148;
                            number = Information.Err().Number;
                            if (number == 75)
                            {
                                DataModul.DSB_NamIdxTable.Close();
                                DataModul.DSB_OrtIdxTable.Close();
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0b5b;
                            }
                            if (number is 53 or 55 or 91 or 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0b5f;
                            }
                            if (number == 3021)
                            {
                                Befehl1[24].Enabled = false;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0b5f;
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
                            goto IL_0b5b;
                        IL_0b5b: // <========== 3
                            num4 = num2;
                            goto IL_0b63;
                        IL_0b5f: // <========== 4
                            num4 = num2 + 1;
                            goto IL_0b63;
                        IL_0b63:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 27:
                                case 31:
                                case 34:
                                case 37:
                                case 40:
                                case 43:
                                case 46:
                                case 49:
                                case 52:
                                case 55:
                                case 58:
                                case 59:
                                    goto IL_0328;
                                case 133:
                                case 134:
                                case 135:
                                    goto IL_0950;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 3621;
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
        private void Ausw_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
        {
            //Discarded unreachable code: IL_009b
            int try0000_dispatch = -1;
            int num = default;
            bool cancel = default;
            int num2 = default;
            int num3 = default;
            CloseReason closeReason = default;
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
                        case 242:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_009e;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_0060;
                            }
                        IL_0060:
                            num = 12;
                            if (Information.Err().Number != 91)
                            {
                                break;
                            }
                            goto IL_0072;
                        IL_0072:
                            num = 13;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_009e;
                        IL_0049:
                            num = 8;
                            DataModul.DSB.Close();
                            ProjectData.EndApp();
                            goto end_IL_0000_2;
                        IL_009e:
                            num4 = num2 + 1;
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_000a;
                                case 3:
                                    goto IL_0014;
                                case 4:
                                    goto IL_001b;
                                case 5:
                                    goto IL_0022;
                                case 6:
                                    goto IL_002f;
                                case 7:
                                    goto IL_003c;
                                case 8:
                                    goto IL_0049;
                                case 12:
                                    goto IL_0060;
                                case 13:
                                    goto IL_0072;
                                case 14:
                                case 16:
                                    goto end_IL_0000_3;
                                default:
                                    goto end_IL_0000;
                                case 9:
                                case 10:
                                case 11:
                                case 17:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_000a:
                            num = 2;
                            closeReason = eventArgs.CloseReason;
                            goto IL_0014;
                        IL_0014:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_001b;
                        IL_001b:
                            num = 4;
                            if (closeReason != 0)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_0022;
                        IL_0022:
                            num = 5;
                            DataModul.MandDB.Close();
                            goto IL_002f;
                        IL_002f:
                            num = 6;
                            DataModul.DOSB.Close();
                            goto IL_003c;
                        IL_003c:
                            num = 7;
                            DataModul.TempDB.Close();
                            goto IL_0049;
                        end_IL_0000_3:
                            break;
                    }
                    num = 16;
                    eventArgs.Cancel = cancel;
                    break;
                end_IL_0000:;
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 242;
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

        private void Ausw_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
        {
            MyProject.Forms.Druck.Show();
        }

        private void Kontroll_CheckStateChanged(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            short num5 = default;
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
                            index = Kontroll.GetIndex((CheckBox)eventSender);
                            goto IL_0015;
                        case 511:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0173;
                                    default:
                                        goto end_IL_0000;
                                }
                                goto IL_0100;
                            }
                        IL_0173:
                            num4 = num2 + 1;
                            goto IL_0176;
                        IL_0100:
                            num = 23;
                            if (Information.Err().Number == 5)
                            {
                                goto IL_0111;
                            }
                            goto IL_012c;
                        IL_012c:
                            num = 27;
                            if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, Information.Err().Number.AsString()) == MsgBoxResult.Cancel)
                            {
                                ProjectData.EndApp();
                            }
                            goto IL_0152;
                        IL_009c:
                            num = 14;
                            Kontroll[0].Visible = false;
                            goto IL_00b2;
                        IL_0152:
                            num = 30;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num4 = num2;
                            goto IL_0176;
                        IL_00b2:
                            num = 15;
                            if (!Kontroll[7].Enabled)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_00c9;
                        IL_00c9:
                            num = 16;
                            Kontroll[7].Visible = false;
                            goto end_IL_0000_2;
                        IL_0176:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0015;
                                case 3:
                                    goto IL_001c;
                                case 5:
                                case 6:
                                    goto IL_0021;
                                case 7:
                                    goto IL_002b;
                                case 8:
                                    goto IL_0042;
                                case 9:
                                    goto IL_0058;
                                case 10:
                                    goto IL_006e;
                                case 11:
                                case 12:
                                case 13:
                                    goto IL_0084;
                                case 14:
                                    goto IL_009c;
                                case 15:
                                    goto IL_00b2;
                                case 16:
                                    goto IL_00c9;
                                case 21:
                                case 25:
                                    goto IL_00e4;
                                case 23:
                                    goto IL_0100;
                                case 24:
                                    goto IL_0111;
                                case 26:
                                case 27:
                                    goto IL_012c;
                                case 28:
                                case 30:
                                    goto IL_0152;
                                default:
                                    goto end_IL_0000;
                                case 4:
                                case 17:
                                case 18:
                                case 19:
                                case 20:
                                case 22:
                                case 31:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                        IL_0111:
                            num = 24;
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num2 = 0;
                            goto IL_00e4;
                        IL_006e:
                            num = 10;
                            Kontroll[7].Visible = true;
                            goto IL_0084;
                        IL_0084:
                            num = 13;
                            if (Kontroll[1].CheckState != 0)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_009c;
                        IL_00e4:
                            num = 21;
                            Befehl1[20].Focus();
                            goto end_IL_0000_2;
                        IL_0015:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_001c;
                        IL_001c:
                            num = 3;
                            num5 = (short)index;
                            goto IL_0021;
                        IL_0021:
                            num = 6;
                            if (num5 != 1)
                            {
                                goto end_IL_0000_2;
                            }
                            goto IL_002b;
                        IL_002b:
                            num = 7;
                            if (Kontroll[1].Checked)
                            {
                                goto IL_0042;
                            }
                            goto IL_0084;
                        IL_0042:
                            num = 8;
                            if (Kontroll[7].Enabled)
                            {
                                goto IL_0058;
                            }
                            goto IL_0084;
                        IL_0058:
                            num = 9;
                            Kontroll[0].Visible = true;
                            goto IL_006e;
                        end_IL_0000:
                            break;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 511;
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

        private void Kontroll_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
        {
            var num5 = checked((short)Strings.Asc(eventArgs.KeyChar));

            ProjectData.ClearProjectError();
            if (num5 == 13)
            {
                Befehl1[16].Focus();
            }
            eventArgs.KeyChar = Strings.Chr(num5);
            eventArgs.Handled = true;
        }
        private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            List2.Items.Add(List1.Items[List1.SelectedIndex].AsString());
        }

        private void List2_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            List2.Items.RemoveAt(List2.SelectedIndex);
        }

        public void weiter(int ubg)
        {
            Read_CheckBoxState(_Modul1.Instance.Aus);
            var b = 1;
            FileSystem.FileOpen(99, _Modul1.Instance.GenFreeDir + "\\Init\\Druck_ini.dat", OpenMode.Output);
            while (b <= 52u)
            {
                FileSystem.PrintLine(99, _Modul1.Instance.Aus[b]);
                b++;
            }
            FileSystem.FileClose(99);

            if (RadioButton1.Checked)
            {
                iM_Priv_aus = 0;
            }
            if (RadioButton2.Checked)
            {
                iM_Priv_aus = 1;
            }
            if (RadioButton3.Checked)
            {
                iM_Priv_aus = 2;
            }
            b = 0;
            while (b <= 19u)
            {
                Befehl1[b].Visible = false;
                b++;
            }
            Befehl1[28].Visible = false;
            Label1[0].Visible = false;
            Befehl1[22].Visible = false;
            Befehl1[23].Visible = false;
            Befehl1[24].Visible = false;
            Befehl1[25].Visible = false;
            Kontroll[1].Text = "Kinder";
            Kontroll[0].Text = "Daten der Kinder";
            Kontroll[1].Visible = true;
            Kontroll[3].Text = "Personen- und Familiennummern";
            Kontroll[2].Visible = true;
            Kontroll[3].Visible = true;
            Kontroll[2].Text = "Daten der Eltern und Ehepartner";
            if (ubg == 119 || ubg == 130)
            {
                Kontroll[1].Visible = false;
                Kontroll[2].Visible = false;
                Kontroll[3].Visible = false;
                Kontroll[10].Visible = false;
                Kontroll[5].Visible = false;
                Kontroll[11].Visible = false;
                Kontroll[7].Visible = false;
                Kontroll[8].Visible = false;
                Kontroll[12].Visible = false;
                Frame2.Visible = false;
            }
            else if (ubg == 120 || ubg == 121)
                Kontroll[11].Visible = false;

            Bezeich = ubg switch
            {
                101 => " Geburtsdatum",
                102 => " Taufdatum",
                103 => " Sterbedatum",
                104 => " Begräbnisdatum",
                105 => " sonstiges Datum",
                119 => " Namen",
                120 => " Personennummern",
                121 => " Namen",
                122 => " Ahnennummern",
                124 => " Nachfahrennummern",
                130 => " Namen",
                401 => " Familiennummern",
                402 => " Name des Ehemannes",
                403 => " Name der Ehefrau",
                503 => " kirchl. Heiratsdatum",
                504 => " Scheidungsdatum",
                1101 => " Geburtsregister",
                1102 => " Taufregister",
                1103 => " Sterberegister",
                1104 => " Begräbnissregister",
                1502 => " Heiratsregister",
                1503 => " kirchl. Heiratsregister",
                _ => Bezeich,
            };
            Bezeichnung1.Visible = false;
            Bezeichnung3.Visible = false;
            if (ubg > 110 & ubg < 500)
            {
                if (ubg == 120 | ubg == 401)
                {
                    Bez[1].Text = "Nummern von bis. Leer = alle";
                }
                if (ubg == 119 | ubg == 121 | ubg == 130 | ubg > 401)
                {
                    Bez[1].Text = "Namen von bis. Leer = alle";
                }
                Bez[1].Visible = true;
                Bez[2].Visible = true;
                Bez[3].Visible = true;
                bereich3.Visible = true;
                bereich3.Focus();
                bereich4.Visible = true;
                Befehl1[16].Visible = true;
            }
            else
            {
                Bezeichnung5.Visible = true;
                Bezeichnung4.Visible = true;
                Bezeichnung6.Visible = true;
                Bereich1.Visible = true;
                bereich2.Visible = true;
                Befehl1[20].Visible = true;
                Bereich1.Focus();
            }
            _Modul1.Instance.Kek = (long)ubg;
        }

        private void Read_CheckBoxState(IList<string> aus)
        {
            aus[1] = CheckBox1.Checked ? "Y" : "";
            aus[2] = CheckBox2.Checked ? "Y" : "";
            aus[3] = CheckBox3.Checked ? "Y" : "";
            aus[4] = CheckBox4.Checked ? "Y" : "";
            aus[5] = CheckBox5.Checked ? "Y" : "";
            aus[6] = CheckBox6.Checked ? "Y" : "";
            aus[51] = CheckBox7.Checked ? "Y" : "";
            aus[52] = CheckBox8.Checked ? "Y" : "";
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            int try0000_dispatch = -1;
            int num = default;
            bool flag = default;
            int num2 = default;
            int num3 = default;
            long num5 = default;
            string text = default;
            long num6 = default;
            long num8 = default;
            int num9 = default;
            long num11 = default;
            string text2 = default;
            long num12 = default;
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
                        long num7;
                        long num10;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                flag = false;
                                goto IL_0005;
                            case 9061:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_1e9b;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    if (Information.Err().Number != 75)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    Interaction.MsgBox("Datei " + _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv schließen");
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_1e9f;
                                }
                            end_IL_0000:
                                break;
                            IL_0005:
                                num = 2;
                                text = "";
                                Frame1.Visible = false;
                                GroupBox1.Width = Width;
                                GroupBox1.Height = Height - 80;
                                GroupBox1.Top = 80;
                                GroupBox1.Left = 0;
                                GroupBox1.Visible = true;
                                RichTextBox1.Left = 20;
                                RichTextBox1.Top = 80;
                                RichTextBox1.Visible = true;
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                FileSystem.FileClose(2);
                                FileSystem.FileOpen(2, _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv", OpenMode.Output, OpenAccess.Write);
                                ProjectData.ClearProjectError();
                                num3 = 0;
                                DataModul.DB_PersonTable.Index = "PerNr";
                                DataModul.DB_PersonTable.MoveLast();
                                num6 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsLong();
                                num7 = 1L;
                                num8 = num6;
                                num5 = num7;
                                goto IL_118d;
                            IL_0388: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 38;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch && !Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    if (num12 == 101)
                                    {
                                        text = "(Geb.)";
                                    }
                                    else if (num12 == 102)
                                    {
                                        text = "(Tauf.)";
                                    }
                                    else if (num12 == 103)
                                    {
                                        text = "(Tod)";
                                    }
                                    else if (num12 == 104)
                                    {
                                        text = "(Begr.)";
                                    }
                                    if (num12 == 105)
                                    {
                                        text = "(Sonst.)";
                                        goto IL_06b6;
                                    }
                                    else if (num12 == 106)
                                    {
                                        text = "(Heimat.)";
                                        goto IL_08be;
                                    }
                                    else
                                    {
                                        flag = true;
                                        RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                        text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                        text2 = text2.Replace("\n", " ");
                                        FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumquelle" + text + ";" + text2);
                                    }
                                }
                                goto IL_0a03;
                            IL_06b6: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 58;
                                while (!DataModul.DB_EventTable.EOF && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        flag = true;
                                        RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                        text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                        text2 = text2.Replace("\n", " ");
                                        FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                    }
                                    DataModul.DB_EventTable.MoveNext();
                                }
                                goto IL_0a03;
                            IL_08be: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 78;
                                while (!DataModul.DB_EventTable.EOF && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        flag = true;
                                        RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                        text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                        text2 = text2.Replace("\n", " ");
                                        FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                    }
                                    DataModul.DB_EventTable.MoveNext();
                                }
                                goto IL_0a03;
                            IL_0a03: // <========== 4
                                     // <========== 5
                                     // <========== 5
                                num = 103;
                                lErl = 88;
                                num12++;
                                if (num12 <= 106)
                                {
                                    goto IL_0388;
                                }
                                num12 = 300L;
                                goto IL_0a28;
                            IL_0a28: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 106;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (num12 == 300)
                                    {
                                        text = "(Beruf)";
                                        while (!DataModul.DB_EventTable.EOF
                                                   && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                        goto IL_1114;
                                    }
                                    if (num12 == 301)
                                    {
                                        text = "(Titel)";
                                        while (!DataModul.DB_EventTable.EOF
                                                   && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                        goto IL_1114;
                                    }
                                    if (num12 == 302)
                                    {
                                        text = "(Wohnort)";
                                        while (!DataModul.DB_EventTable.EOF
                                                   && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                    }
                                }
                                goto IL_1114;
                            IL_1114: // <========== 5
                                     // <========== 5
                                     // <========== 5
                                num = 170;
                                lErl = 99;
                                num12++;
                                if (num12 <= 302)
                                {
                                    goto IL_0a28;
                                }
                                goto IL_1137;
                            IL_1137: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 172;
                                lErl = 46;
                                if (flag)
                                {
                                    RichTextBox1.SelectedText = "**** Ende Person " + num5.AsString() + "****\n";
                                    flag = false;
                                }
                                num5++;
                                goto IL_118d;
                            IL_118d:
                                if (num5 <= num8)
                                {
                                    flag = false;
                                    _Modul1.Instance.PersInArb = (int)num5;
                                    Label8.Text = "Person " + num5.AsString() + " von" + num6.AsString() + " in Arbeit";
                                    Application.DoEvents();
                                    DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(string.Concat("Personenquelle Person" + num5.AsString(), "\n") + DataModul.DB_PersonTable.Fields[PersonFields.Bem3].Value + '\n', '\n'));
                                            text2 = Strings.Replace(DataModul.DB_PersonTable.Fields[PersonFields.Bem3].AsString(), ";", ",");
                                            text2 = text2.Replace("\n", " ");
                                            FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Personenquelle;" + text2);
                                            flag = true;
                                        }
                                        num12 = 101L;
                                        goto IL_0388;
                                    }
                                    goto IL_1137;
                                }
                                RichTextBox1.SelectedText = "\nEnde der Personenprüfung\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\nBeginn der Familienprüfung\n\n";
                                Label8.Text = "Personen fertig";
                                DataModul.DB_FamilyTable.Index = "Fam";
                                DataModul.DB_FamilyTable.MoveLast();
                                num9 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                num10 = 1L;
                                num11 = num9;
                                num5 = num10;
                                goto IL_1e02;
                            IL_189f: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 242;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (num12 == 602)
                                    {
                                        text = "(Wohnung)";
                                        while (!DataModul.DB_EventTable.EOF && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb))
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                    }
                                    else if (num12 == 603)
                                    {
                                        text = "(Sonst. Datum)";
                                        while (!DataModul.DB_EventTable.EOF && !(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb))
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                            }
                                            DataModul.DB_EventTable.MoveNext();
                                        }
                                    }
                                }
                                goto IL_1d89;
                            IL_1d89: // <========== 4
                                     // <========== 4
                                     // <========== 4
                                num = 286;
                                lErl = 19;
                                num12++;
                                if (num12 <= 603)
                                {
                                    goto IL_189f;
                                }
                                goto IL_1dac;
                            IL_1dac: // <========== 3
                                     // <========== 3
                                     // <========== 3
                                num = 288;
                                lErl = 47;
                                if (flag)
                                {
                                    RichTextBox1.SelectedText = "****Ende Familie " + num5.AsString() + "****\n";
                                    flag = false;
                                }
                                num5++;
                                goto IL_1e02;
                            IL_1e02:
                                if (num5 <= num11)
                                {
                                    flag = false;
                                    _Modul1.Instance.FamInArb = (int)num5;
                                    Label9.Text = "Familie " + num5.AsString() + " von" + num9.AsString() + " in Arbeit";
                                    Application.DoEvents();
                                    DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb.AsString());
                                    if (!DataModul.DB_FamilyTable.NoMatch)
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value) && Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(string.Concat("Familienquelle Familie" + num5.AsString(), "\n") + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].Value + '\n', '\n'));
                                            text2 = Strings.Replace(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString(), ";", ",");
                                            text2 = text2.Replace("\n", " ");
                                            FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Familienquelle " + text + ";" + text2);
                                            flag = true;
                                        }
                                        num12 = 500L;
                                        while (num12 <= 507)
                                        {
                                            DataModul.DB_EventTable.Index = "Besu";
                                            DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                            if (!DataModul.DB_EventTable.NoMatch && !Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem3].Value) && Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                if (num12 == 500)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[5] + ")";
                                                }
                                                if (num12 == 501)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[6] + ")";
                                                }
                                                if (num12 == 502)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[7] + ")";
                                                }
                                                if (num12 == 503)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[8] + ")";
                                                }
                                                if (num12 == 504)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[9] + ")";
                                                }
                                                if (num12 == 505)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[10] + ")";
                                                }
                                                if (num12 == 506)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[12] + ")";
                                                }
                                                if (num12 == 507)
                                                {
                                                    text = "(" + _Modul1.Instance.DTxt[15] + ")";
                                                }
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumsquelle " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem3].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem3].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Datumsquelle " + text + ";" + text2);
                                            }
                                            lErl = 18;
                                            num12++;
                                        }
                                        num12 = 602L;
                                        goto IL_189f;
                                    }
                                    goto IL_1dac;
                                }
                                RichTextBox1.SelectedText = "Ende der Liste";
                                Label9.Text = "Familien fertig";
                                goto end_IL_0000_2;
                            IL_1e9b:
                                num4 = unchecked(num2 + 1);
                                goto IL_1e9f;
                            IL_1e9f:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 38:
                                        goto IL_0388;
                                    case 57:
                                    case 58:
                                    case 72:
                                        goto IL_06b6;
                                    case 77:
                                    case 78:
                                    case 92:
                                        goto IL_08be;
                                    case 60:
                                    case 73:
                                    case 80:
                                    case 93:
                                    case 100:
                                    case 101:
                                    case 102:
                                    case 103:
                                        goto IL_0a03;
                                    case 106:
                                        goto IL_0a28;
                                    case 114:
                                    case 127:
                                    case 134:
                                    case 147:
                                    case 154:
                                    case 167:
                                    case 168:
                                    case 169:
                                    case 170:
                                        goto IL_1114;
                                    case 26:
                                    case 172:
                                        goto IL_1137;
                                    case 242:
                                        goto IL_189f;
                                    case 250:
                                    case 263:
                                    case 270:
                                    case 283:
                                    case 284:
                                    case 285:
                                    case 286:
                                        goto IL_1d89;
                                    case 190:
                                    case 288:
                                        goto IL_1dac;
                                    case 296:
                                    case 300:
                                    case 301:
                                    case 302:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 9061;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 3
                           // <========== 3
                           // <========== 3
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            RichTextBox1.SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
            RichTextBox1.LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            int try0000_dispatch = -1;
            int num = default;
            bool flag = default;
            int num2 = default;
            int num3 = default;
            long num5 = default;
            string text = default;
            long num6 = default;
            long num8 = default;
            int num9 = default;
            long num11 = default;
            long num12 = default;
            string text2 = default;
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
                        long num7;
                        long num10;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                flag = false;
                                goto IL_0005;
                            case 8226:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_1ba0;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    if (Information.Err().Number != 75)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    Interaction.MsgBox("Datei " + _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv schließen");
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_1ba4;
                                }
                            end_IL_0000:
                                break;
                            IL_0005:
                                num = 2;
                                text = "";
                                Frame1.Visible = false;
                                GroupBox1.Width = Width;
                                GroupBox1.Height = Height - 80;
                                GroupBox1.Top = 80;
                                GroupBox1.Left = 0;
                                GroupBox1.Visible = true;
                                RichTextBox1.Left = 20;
                                RichTextBox1.Top = 80;
                                RichTextBox1.Visible = true;
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                FileSystem.FileClose(2);
                                FileSystem.FileOpen(2, _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv", OpenMode.Output, OpenAccess.Write);
                                ProjectData.ClearProjectError();
                                num3 = 0;
                                DataModul.DB_PersonTable.Index = "PerNr";
                                DataModul.DB_PersonTable.MoveLast();
                                num6 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsLong();
                                num7 = 1L;
                                num8 = num6;
                                num5 = num7;
                                goto IL_1027;
                            IL_0231: // <========== 3
                                num = 29;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                    {
                                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            if (num12 == 101)
                                            {
                                                text = "(Geb.)";
                                            }
                                            if (num12 == 102)
                                            {
                                                text = "(Tauf.)";
                                            }
                                            if (num12 == 103)
                                            {
                                                text = "(Tod)";
                                            }
                                            if (num12 == 104)
                                            {
                                                text = "(Begr.)";
                                            }
                                            if (num12 == 105)
                                            {
                                                text = "(Sonst.)";
                                                goto IL_055f;
                                            }
                                            if (num12 == 106)
                                            {
                                                text = "(Heimat.)";
                                                goto IL_0767;
                                            }
                                            flag = true;
                                            RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                            text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                            text2 = text2.Replace("\n", " ");
                                            FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                        }
                                    }
                                }
                                goto IL_08ac;
                            IL_0551: // <========== 3
                                num = 62;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_055f;
                            IL_055f: // <========== 3
                                num = 49;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                        }
                                        goto IL_0551;
                                    }
                                }
                                goto IL_08ac;
                            IL_0759: // <========== 3
                                num = 82;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_0767;
                            IL_0767: // <========== 3
                                num = 69;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                        }
                                        goto IL_0759;
                                    }
                                }
                                goto IL_08ac;
                            IL_08ac: // <========== 5
                                num = 94;
                                lErl = 88;
                                num12++;
                                if (num12 <= 106)
                                {
                                    goto IL_0231;
                                }
                                num12 = 300L;
                                goto IL_08d1;
                            IL_08d1: // <========== 3
                                num = 97;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (num12 == 300)
                                    {
                                        text = "(Beruf)";
                                        goto IL_0b4a;
                                    }
                                    if (num12 == 301)
                                    {
                                        text = "(Titel)";
                                        goto IL_0d6a;
                                    }
                                    if (num12 == 302)
                                    {
                                        text = "(Wohnort)";
                                        goto IL_0f96;
                                    }
                                }
                                goto IL_0fae;
                            IL_0b3c: // <========== 3
                                num = 116;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_0b4a;
                            IL_0b4a: // <========== 3
                                num = 103;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                        }
                                        goto IL_0b3c;
                                    }
                                }
                                goto IL_0fae;
                            IL_0d59: // <========== 3
                                num = 136;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_0d6a;
                            IL_0d6a: // <========== 3
                                num = 123;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                        }
                                        goto IL_0d59;
                                    }
                                }
                                goto IL_0fae;
                            IL_0f85: // <========== 3
                                num = 156;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_0f96;
                            IL_0f96: // <========== 3
                                num = 143;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                        }
                                        goto IL_0f85;
                                    }
                                }
                                goto IL_0fae;
                            IL_0fae: // <========== 5
                                num = 161;
                                lErl = 99;
                                num12++;
                                if (num12 <= 302)
                                {
                                    goto IL_08d1;
                                }
                                goto IL_0fd1;
                            IL_0fd1: // <========== 3
                                num = 163;
                                lErl = 46;
                                if (flag)
                                {
                                    RichTextBox1.SelectedText = "**** Ende Person " + num5.AsString() + "****\n";
                                    flag = false;
                                }
                                goto IL_101b;
                            IL_101b:
                                num = 168;
                                num5++;
                                goto IL_1027;
                            IL_1027:
                                if (num5 <= num8)
                                {
                                    flag = false;
                                    _Modul1.Instance.PersInArb = (int)num5;
                                    Label8.Text = "Person " + num5.AsString() + " von" + num6.AsString() + " in Arbeit";
                                    Application.DoEvents();
                                    DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        num12 = 101L;
                                        goto IL_0231;
                                    }
                                    goto IL_0fd1;
                                }
                                RichTextBox1.SelectedText = "\nEnde der Personenprüfung\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\nBeginn der Familienprüfung\n\n";
                                Label8.Text = "Personen fertig";
                                DataModul.DB_FamilyTable.Index = "Fam";
                                DataModul.DB_FamilyTable.MoveLast();
                                num9 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                num10 = 1L;
                                num11 = num9;
                                num5 = num10;
                                goto IL_1b07;
                            IL_11c2: // <========== 3
                                num = 184;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                    {
                                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            if (num12 == 500)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[5] + ")";
                                            }
                                            if (num12 == 501)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[6] + ")";
                                            }
                                            if (num12 == 502)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[7] + ")";
                                            }
                                            if (num12 == 503)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[8] + ")";
                                            }
                                            if (num12 == 504)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[9] + ")";
                                            }
                                            if (num12 == 505)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[10] + ")";
                                            }
                                            if (num12 == 506)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[12] + ")";
                                            }
                                            if (num12 == 507)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[15] + ")";
                                            }
                                            flag = true;
                                            RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                            text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                            text2 = text2.Replace("\n", " ");
                                            FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                        }
                                    }
                                }
                                goto IL_1573;
                            IL_1573: // <========== 3
                                num = 221;
                                lErl = 18;
                                num12++;
                                if (num12 <= 507)
                                {
                                    goto IL_11c2;
                                }
                                num12 = 602L;
                                goto IL_15a4;
                            IL_15a4: // <========== 3
                                num = 224;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (num12 == 602)
                                    {
                                        text = "(Wohnung)";
                                        goto IL_1847;
                                    }
                                    if (num12 == 603)
                                    {
                                        text = "(Sonst. Datum)";
                                        goto IL_1a76;
                                    }
                                }
                                goto IL_1a8e;
                            IL_1836: // <========== 3
                                num = 243;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_1847;
                            IL_1847: // <========== 3
                                num = 230;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                    {
                                        if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                            goto IL_1836;
                                        }
                                        goto IL_1a8e;
                                    }
                                    goto IL_1836;
                                }
                                goto IL_1a8e;
                            IL_1a65: // <========== 3
                                num = 263;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_1a76;
                            IL_1a76: // <========== 3
                                num = 250;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem4].Value))
                                    {
                                        if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("Datumszeugen " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem4].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem4].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Datumszeugen " + text + ";" + text2);
                                            }
                                            goto IL_1a65;
                                        }
                                        goto IL_1a8e;
                                    }
                                    goto IL_1a65;
                                }
                                goto IL_1a8e;
                            IL_1a8e: // <========== 6
                                num = 268;
                                lErl = 19;
                                num12++;
                                if (num12 <= 603)
                                {
                                    goto IL_15a4;
                                }
                                goto IL_1ab1;
                            IL_1ab1: // <========== 3
                                num = 270;
                                lErl = 47;
                                if (flag)
                                {
                                    RichTextBox1.SelectedText = "**** Ende Familie " + num5.AsString() + "****\n";
                                    flag = false;
                                }
                                goto IL_1afb;
                            IL_1afb:
                                num = 275;
                                num5++;
                                goto IL_1b07;
                            IL_1b07:
                                if (num5 <= num11)
                                {
                                    flag = false;
                                    _Modul1.Instance.FamInArb = (int)num5;
                                    Label9.Text = "Familie " + num5.AsString() + " von" + num9.AsString() + " in Arbeit";
                                    Application.DoEvents();
                                    DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb.AsString());
                                    if (!DataModul.DB_FamilyTable.NoMatch)
                                    {
                                        num12 = 500L;
                                        goto IL_11c2;
                                    }
                                    goto IL_1ab1;
                                }
                                RichTextBox1.SelectedText = "Ende der Liste";
                                Label9.Text = "Familien fertig";
                                goto end_IL_0000_2;
                            IL_1ba0:
                                num4 = unchecked(num2 + 1);
                                goto IL_1ba4;
                            IL_1ba4:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 29:
                                        goto IL_0231;
                                    case 60:
                                    case 61:
                                    case 62:
                                        goto IL_0551;
                                    case 48:
                                    case 49:
                                    case 63:
                                        goto IL_055f;
                                    case 80:
                                    case 81:
                                    case 82:
                                        goto IL_0759;
                                    case 68:
                                    case 69:
                                    case 83:
                                        goto IL_0767;
                                    case 51:
                                    case 64:
                                    case 71:
                                    case 84:
                                    case 91:
                                    case 92:
                                    case 93:
                                    case 94:
                                        goto IL_08ac;
                                    case 97:
                                        goto IL_08d1;
                                    case 114:
                                    case 115:
                                    case 116:
                                        goto IL_0b3c;
                                    case 102:
                                    case 103:
                                    case 117:
                                        goto IL_0b4a;
                                    case 134:
                                    case 135:
                                    case 136:
                                        goto IL_0d59;
                                    case 122:
                                    case 123:
                                    case 137:
                                        goto IL_0d6a;
                                    case 154:
                                    case 155:
                                    case 156:
                                        goto IL_0f85;
                                    case 142:
                                    case 143:
                                    case 157:
                                        goto IL_0f96;
                                    case 105:
                                    case 118:
                                    case 125:
                                    case 138:
                                    case 145:
                                    case 158:
                                    case 159:
                                    case 160:
                                    case 161:
                                        goto IL_0fae;
                                    case 26:
                                    case 163:
                                        goto IL_0fd1;
                                    case 167:
                                    case 168:
                                        goto IL_101b;
                                    case 184:
                                        goto IL_11c2;
                                    case 218:
                                    case 219:
                                    case 220:
                                    case 221:
                                        goto IL_1573;
                                    case 224:
                                        goto IL_15a4;
                                    case 241:
                                    case 242:
                                    case 243:
                                        goto IL_1836;
                                    case 229:
                                    case 230:
                                    case 244:
                                        goto IL_1847;
                                    case 261:
                                    case 262:
                                    case 263:
                                        goto IL_1a65;
                                    case 249:
                                    case 250:
                                    case 264:
                                        goto IL_1a76;
                                    case 233:
                                    case 245:
                                    case 253:
                                    case 265:
                                    case 266:
                                    case 267:
                                    case 268:
                                        goto IL_1a8e;
                                    case 181:
                                    case 270:
                                        goto IL_1ab1;
                                    case 274:
                                    case 275:
                                        goto IL_1afb;
                                    case 278:
                                    case 282:
                                    case 283:
                                    case 284:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 8226;
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
        private void Button5_Click(object sender, EventArgs e)
        {
            int try0000_dispatch = -1;
            int num = default;
            bool flag = default;
            int num2 = default;
            int num3 = default;
            long num5 = default;
            string text = default;
            long num6 = default;
            long num8 = default;
            int num9 = default;
            long num11 = default;
            string text2 = default;
            long num12 = default;
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
                        long num7;
                        long num10;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                flag = false;
                                goto IL_0005;
                            case 13669:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_2f37;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    if (Information.Err().Number != 75)
                                    {
                                        goto end_IL_0000_2;
                                    }
                                    Interaction.MsgBox("Datei " + _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv schließen");
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num4 = num2;
                                    goto IL_2f3b;
                                }
                            end_IL_0000:
                                break;
                            IL_0005:
                                num = 2;
                                text = "";
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                FileSystem.FileClose(2);
                                FileSystem.FileOpen(2, _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv", OpenMode.Output, OpenAccess.Write);
                                ProjectData.ClearProjectError();
                                num3 = 0;
                                Frame1.Visible = false;
                                GroupBox1.Width = Width;
                                GroupBox1.Height = Height - 80;
                                GroupBox1.Top = 80;
                                GroupBox1.Left = 0;
                                GroupBox1.Visible = true;
                                RichTextBox1.Left = 20;
                                RichTextBox1.Top = 80;
                                RichTextBox1.Visible = true;
                                DataModul.DB_PersonTable.Index = "PerNr";
                                DataModul.DB_PersonTable.MoveLast();
                                num6 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsLong();
                                num7 = 1L;
                                num8 = num6;
                                num5 = num7;
                                goto IL_1c55;
                            IL_038a: // <========== 3
                                     // <========== 3
                                num = 38;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem1].Value))
                                    {
                                        if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            if (num12 == 101)
                                            {
                                                text = "(Geb.)";
                                            }
                                            if (num12 == 102)
                                            {
                                                text = "(Tauf.)";
                                            }
                                            if (num12 == 103)
                                            {
                                                text = "(Tod)";
                                            }
                                            if (num12 == 104)
                                            {
                                                text = "(Begr.)";
                                            }
                                            if (num12 == 105)
                                            {
                                                text = "(Sonst.)";
                                                goto IL_083b;
                                            }
                                            if (num12 == 106)
                                            {
                                                text = "(Heimat.)";
                                                goto IL_0bfe;
                                            }
                                            flag = true;
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                            }
                                            if (sender == Button5)
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                }
                                            }
                                        }
                                    }
                                }
                                goto IL_0eea;
                            IL_083b: // <========== 3
                                     // <========== 3
                                num = 58;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                        {
                                            flag = true;
                                            RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                            text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                            text2 = text2.Replace("\n", " ");
                                            FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                        }
                                        if (sender == Button5)
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                            }
                                        }
                                        DataModul.DB_EventTable.MoveNext();
                                        goto IL_083b;
                                    }
                                }
                                goto IL_0eea;
                            IL_0bf0:
                                // <========== 4
                                num = 107;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_0bfe;
                            IL_0bfe: // <========== 3
                                     // <========== 3
                                num = 85;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung5 " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                            }
                                            if (sender == Button5)
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                }
                                            }
                                        }
                                        goto IL_0bf0;
                                    }
                                }
                                goto IL_0eea;
                            IL_0eea: // <========== 5
                                     // <========== 7
                                num = 129;
                                lErl = 88;
                                num12++;
                                if (num12 <= 106)
                                {
                                    goto IL_038a;
                                }
                                num12 = 300L;
                                goto IL_0f18;
                            IL_0f18: // <========== 3
                                     // <========== 3
                                num = 132;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (num12 == 300)
                                    {
                                        text = "(Beruf)";
                                        goto IL_13ba;
                                    }
                                    if (num12 == 301)
                                    {
                                        text = "(Titel)";
                                        goto IL_17b9;
                                    }
                                    if (num12 == 302)
                                    {
                                        text = "(Wohnort)";
                                        goto IL_1bc4;
                                    }
                                }
                                goto IL_1bdc;
                            IL_13a9: // <========== 3
                                     // <========== 5
                                num = 162;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_13ba;
                            IL_13ba: // <========== 3
                                     // <========== 3
                                num = 138;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem1].Value))
                                    {
                                        if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                        {
                                            if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                                }
                                                if (sender == Button5)
                                                {
                                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                    {
                                                        flag = true;
                                                        RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                        text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                        text2 = text2.Replace("\n", " ");
                                                        FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                            goto IL_1bdc;
                                    }
                                    goto IL_13a9;
                                }
                                goto IL_1bdc;
                            IL_17a8:
                                // <========== 4
                                num = 191;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_17b9;
                            IL_17b9: // <========== 3
                                     // <========== 3
                                num = 169;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                            }
                                            if (sender == Button5)
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                }
                                            }
                                        }
                                        goto IL_17a8;
                                    }
                                }
                                goto IL_1bdc;
                            IL_1bb3:
                                // <========== 4
                                num = 221;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_1bc4;
                            IL_1bc4: // <========== 3
                                     // <========== 3
                                num = 198;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    Application.DoEvents();
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb))
                                    {
                                        if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                            }
                                            if (sender == Button5)
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Person"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                }
                                            }
                                        }
                                        goto IL_1bb3;
                                    }
                                }
                                goto IL_1bdc;
                            IL_1bdc: // <========== 6
                                     // <========== 6
                                num = 226;
                                lErl = 99;
                                num12++;
                                if (num12 <= 302)
                                {
                                    goto IL_0f18;
                                }
                                goto IL_1bff;
                            IL_1bff: // <========== 3
                                     // <========== 3
                                num = 228;
                                lErl = 46;
                                if (flag)
                                {
                                    RichTextBox1.SelectedText = "**** Ende Person " + num5.AsString() + "****\n";
                                    flag = false;
                                }
                                num5++;
                                goto IL_1c55;
                            IL_1c55:
                                if (num5 <= num8)
                                {
                                    flag = false;
                                    _Modul1.Instance.PersInArb = (int)num5;
                                    Label8.Text = "Person " + num5.AsString() + " von" + num6.AsString() + " in Arbeit";
                                    Application.DoEvents();
                                    DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                                    if (!DataModul.DB_PersonTable.NoMatch)
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(string.Concat("Personenbemerkung Person" + num5.AsString(), "\n") + DataModul.DB_PersonTable.Fields[PersonFields.Bem1].Value + '\n', '\n'));
                                                text2 = Strings.Replace(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Personenbemerkung ;" + text2);
                                                flag = true;
                                            }
                                        }
                                        num12 = 101L;
                                        goto IL_038a;
                                    }
                                    goto IL_1bff;
                                }
                                Label8.Text = "Personen fertig";
                                RichTextBox1.SelectedText = "\nEnde der Personenprüfung\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\nBeginn der Familienprüfung\n\n";
                                DataModul.DB_FamilyTable.Index = "Fam";
                                DataModul.DB_FamilyTable.MoveLast();
                                num9 = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                                num10 = 1L;
                                num11 = num9;
                                num5 = num10;
                                goto IL_2e9e;
                            IL_1f5c: // <========== 3
                                     // <========== 3
                                num = 258;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem1].Value))
                                    {
                                        if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            if (num12 == 500)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[5] + ")";
                                            }
                                            if (num12 == 501)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[6] + ")";
                                            }
                                            if (num12 == 502)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[7] + ")";
                                            }
                                            if (num12 == 503)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[8] + ")";
                                            }
                                            if (num12 == 504)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[9] + ")";
                                            }
                                            if (num12 == 505)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[10] + ")";
                                            }
                                            if (num12 == 506)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[12] + ")";
                                            }
                                            if (num12 == 507)
                                            {
                                                text = "(" + _Modul1.Instance.DTxt[15] + ")";
                                            }
                                            if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                flag = true;
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                            }
                                            if (sender == Button5)
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                }
                                            }
                                        }
                                    }
                                }
                                goto IL_250c;
                            IL_250c: // <========== 3
                                     // <========== 5
                                num = 306;
                                lErl = 18;
                                num12++;
                                if (num12 <= 507)
                                {
                                    goto IL_1f5c;
                                }
                                num12 = 602L;
                                goto IL_253d;
                            IL_253d: // <========== 3
                                     // <========== 3
                                num = 309;
                                DataModul.DB_EventTable.Index = "Besu";
                                DataModul.DB_EventTable.Seek("=", num12.AsString(), num5.AsString());
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    if (num12 == 602)
                                    {
                                        text = "(Wohnung)";
                                        goto IL_29df;
                                    }
                                    if (num12 == 603)
                                    {
                                        text = "(Sonst. Datum)";
                                        goto IL_2e0d;
                                    }
                                }
                                goto IL_2e25;
                            IL_29ce: // <========== 3
                                     // <========== 4
                                num = 339;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_29df;
                            IL_29df: // <========== 3
                                     // <========== 3
                                num = 315;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb))
                                    {
                                        if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                        {
                                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem1].Value))
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                                }
                                                if (sender == Button5)
                                                {
                                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                    {
                                                        flag = true;
                                                        RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                        text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                        text2 = text2.Replace("\n", " ");
                                                        FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                    }
                                                }
                                            }
                                        }
                                        goto IL_29ce;
                                    }
                                }
                                goto IL_2e25;
                            IL_2dfc: // <========== 3
                                     // <========== 4
                                num = 370;
                                DataModul.DB_EventTable.MoveNext();
                                goto IL_2e0d;
                            IL_2e0d: // <========== 3
                                     // <========== 3
                                num = 346;
                                if (!DataModul.DB_EventTable.EOF)
                                {
                                    if (!(DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.FamInArb))
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Bem1].Value))
                                        {
                                            if ((Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0) | (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0))
                                            {
                                                if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                                {
                                                    flag = true;
                                                    RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("obere Datumsbemerkung " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem1].Value), '\n'), '\n'));
                                                    text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString(), ";", ",");
                                                    text2 = text2.Replace("\n", " ");
                                                    FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Obere Datumsbemerkung " + text + ";" + text2);
                                                }
                                                if (sender == Button5)
                                                {
                                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                                    {
                                                        flag = true;
                                                        RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(Operators.AddObject(Operators.AddObject(string.Concat(string.Concat(string.Concat("untere Datumsbemerkung " + text, " Familie"), num5.AsString()), "\n"), DataModul.DB_EventTable.Fields[EventFields.Bem2].Value), '\n'), '\n'));
                                                        text2 = Strings.Replace(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString(), ";", ",");
                                                        text2 = text2.Replace("\n", " ");
                                                        FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Untere Datumsbemerkung " + text + ";" + text2);
                                                    }
                                                }
                                            }
                                        }
                                        goto IL_2dfc;
                                    }
                                }
                                goto IL_2e25;
                            IL_2e25: // <========== 4
                                     // <========== 4
                                num = 375;
                                lErl = 19;
                                num12++;
                                if (num12 <= 603)
                                {
                                    goto IL_253d;
                                }
                                goto IL_2e48;
                            IL_2e48: // <========== 3
                                     // <========== 3
                                num = 377;
                                lErl = 47;
                                if (flag)
                                {
                                    RichTextBox1.SelectedText = "****Ende Familie " + num5.AsString() + "****\"\n";
                                    flag = false;
                                }
                                num5++;
                                goto IL_2e9e;
                            IL_2e9e:
                                if (num5 <= num11)
                                {
                                    flag = false;
                                    _Modul1.Instance.FamInArb = (int)num5;
                                    Label9.Text = "Familie " + num5.AsString() + " von" + num9.AsString() + " in Arbeit";
                                    Application.DoEvents();
                                    DataModul.DB_FamilyTable.Seek("=", _Modul1.Instance.FamInArb.AsString());
                                    if (!DataModul.DB_FamilyTable.NoMatch)
                                    {
                                        if (!Information.IsDBNull(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].Value))
                                        {
                                            if (Operators.CompareString(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                            {
                                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(string.Concat("Familienbemerkung Familie" + num5.AsString(), "\n") + DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].Value + '\n', '\n'));
                                                text2 = Strings.Replace(DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString(), ";", ",");
                                                text2 = text2.Replace("\n", " ");
                                                FileSystem.PrintLine(2, "Familie" + num5.AsString() + ";Familienbemerkung ;" + text2);
                                                flag = true;
                                            }
                                        }
                                        num12 = 500L;
                                        goto IL_1f5c;
                                    }
                                    goto IL_2e48;
                                }
                                RichTextBox1.SelectedText = "Ende der Liste";
                                Label9.Text = "Familien fertig";
                                goto end_IL_0000_2;
                            IL_2f37:
                                num4 = unchecked(num2 + 1);
                                goto IL_2f3b;
                            IL_2f3b:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 38:
                                        goto IL_038a;
                                    case 57:
                                    case 58:
                                    case 79:
                                        goto IL_083b;
                                    case 104:
                                    case 105:
                                    case 106:
                                    case 107:
                                        goto IL_0bf0;
                                    case 84:
                                    case 85:
                                    case 108:
                                        goto IL_0bfe;
                                    case 60:
                                    case 80:
                                    case 87:
                                    case 109:
                                    case 124:
                                    case 125:
                                    case 126:
                                    case 127:
                                    case 128:
                                    case 129:
                                        goto IL_0eea;
                                    case 132:
                                        goto IL_0f18;
                                    case 158:
                                    case 159:
                                    case 160:
                                    case 161:
                                    case 162:
                                        goto IL_13a9;
                                    case 137:
                                    case 138:
                                    case 163:
                                        goto IL_13ba;
                                    case 188:
                                    case 189:
                                    case 190:
                                    case 191:
                                        goto IL_17a8;
                                    case 168:
                                    case 169:
                                    case 192:
                                        goto IL_17b9;
                                    case 218:
                                    case 219:
                                    case 220:
                                    case 221:
                                        goto IL_1bb3;
                                    case 197:
                                    case 198:
                                    case 222:
                                        goto IL_1bc4;
                                    case 141:
                                    case 164:
                                    case 171:
                                    case 193:
                                    case 201:
                                    case 223:
                                    case 224:
                                    case 225:
                                    case 226:
                                        goto IL_1bdc;
                                    case 26:
                                    case 228:
                                        goto IL_1bff;
                                    case 258:
                                        goto IL_1f5c;
                                    case 301:
                                    case 302:
                                    case 303:
                                    case 304:
                                    case 305:
                                    case 306:
                                        goto IL_250c;
                                    case 309:
                                        goto IL_253d;
                                    case 335:
                                    case 336:
                                    case 337:
                                    case 338:
                                    case 339:
                                        goto IL_29ce;
                                    case 314:
                                    case 315:
                                    case 340:
                                        goto IL_29df;
                                    case 366:
                                    case 367:
                                    case 368:
                                    case 369:
                                    case 370:
                                        goto IL_2dfc;
                                    case 345:
                                    case 346:
                                    case 371:
                                        goto IL_2e0d;
                                    case 317:
                                    case 341:
                                    case 348:
                                    case 372:
                                    case 373:
                                    case 374:
                                    case 375:
                                        goto IL_2e25;
                                    case 246:
                                    case 377:
                                        goto IL_2e48;
                                    case 385:
                                    case 389:
                                    case 390:
                                    case 391:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 13669;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 3
                           // <========== 3
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            int try0000_dispatch = -1;
            int num = default;
            bool flag = default;
            int num2 = default;
            int num3 = default;
            int lErl = default;
            long num5 = default;
            long num6 = default;
            long num8 = default;
            string text = default;
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
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                flag = false;
                                goto IL_0005;
                            case 1309:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_0443;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    goto IL_03ee;
                                }
                            IL_0443:
                                num4 = unchecked(num2 + 1);
                                goto IL_0447;
                            IL_03ee:
                                num = 45;
                                if (Information.Err().Number != 75)
                                {
                                    goto end_IL_0000_2;
                                }
                                goto IL_0400;
                            IL_0400:
                                num = 46;
                                Interaction.MsgBox("Datei " + _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv schließen");
                                goto IL_0420;
                            IL_0420:
                                num = 47;
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                num4 = num2;
                                goto IL_0447;
                            IL_03bc:
                                num = 41;
                                lErl = 46;
                                goto IL_03c4;
                            IL_03c4:
                                num = 42;
                                num5++;
                                goto IL_03cd;
                            IL_0447:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 2:
                                        goto IL_0005;
                                    case 3:
                                        goto IL_0014;
                                    case 4:
                                        goto IL_0028;
                                    case 5:
                                        goto IL_003f;
                                    case 6:
                                        goto IL_004f;
                                    case 7:
                                        goto IL_005e;
                                    case 8:
                                        goto IL_006d;
                                    case 9:
                                        goto IL_007d;
                                    case 10:
                                        goto IL_008e;
                                    case 11:
                                        goto IL_009e;
                                    case 12:
                                        goto IL_00a6;
                                    case 13:
                                        goto IL_00be;
                                    case 14:
                                        goto IL_00db;
                                    case 15:
                                        goto IL_00e3;
                                    case 16:
                                        goto IL_00f6;
                                    case 17:
                                        goto IL_0105;
                                    case 18:
                                        goto IL_0128;
                                    case 19:
                                        goto IL_013c;
                                    case 20:
                                        goto IL_014b;
                                    case 21:
                                        goto IL_0151;
                                    case 22:
                                        goto IL_015c;
                                    case 23:
                                        goto IL_01b3;
                                    case 24:
                                        goto IL_01bc;
                                    case 25:
                                        goto IL_021a;
                                    case 27:
                                    case 28:
                                        goto IL_022f;
                                    case 29:
                                        goto IL_025b;
                                    case 30:
                                        goto IL_0293;
                                    case 31:
                                        goto IL_02fc;
                                    case 32:
                                        goto IL_0332;
                                    case 33:
                                        goto IL_034c;
                                    case 34:
                                        goto IL_0380;
                                    case 35:
                                    case 36:
                                    case 37:
                                        goto IL_0386;
                                    case 38:
                                        goto IL_038d;
                                    case 39:
                                        goto IL_03b6;
                                    case 26:
                                    case 40:
                                    case 41:
                                        goto IL_03bc;
                                    case 42:
                                        goto IL_03c4;
                                    case 43:
                                        goto IL_03d5;
                                    case 45:
                                        goto IL_03ee;
                                    case 46:
                                        goto IL_0400;
                                    case 47:
                                        goto IL_0420;
                                    default:
                                        goto end_IL_0000;
                                    case 44:
                                    case 48:
                                    case 49:
                                    case 50:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                            IL_0005:
                                num = 2;
                                Frame1.Visible = false;
                                goto IL_0014;
                            IL_0014:
                                num = 3;
                                GroupBox1.Width = Width;
                                goto IL_0028;
                            IL_0028:
                                num = 4;
                                GroupBox1.Height = Height - 80;
                                goto IL_003f;
                            IL_003f:
                                num = 5;
                                GroupBox1.Top = 80;
                                goto IL_004f;
                            IL_004f:
                                num = 6;
                                GroupBox1.Left = 0;
                                goto IL_005e;
                            IL_005e:
                                num = 7;
                                GroupBox1.Visible = true;
                                goto IL_006d;
                            IL_006d:
                                num = 8;
                                RichTextBox1.Left = 20;
                                goto IL_007d;
                            IL_007d:
                                num = 9;
                                RichTextBox1.Top = 80;
                                goto IL_008e;
                            IL_008e:
                                num = 10;
                                RichTextBox1.Visible = true;
                                goto IL_009e;
                            IL_009e:
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                goto IL_00a6;
                            IL_00a6:
                                num = 12;
                                FileSystem.FileClose(2);
                                goto IL_00be;
                            IL_00be:
                                num = 13;
                                FileSystem.FileOpen(2, _Modul1.Instance.GenFreeDir + "Temp\\Neu1.csv", OpenMode.Output, OpenAccess.Write);
                                goto IL_00db;
                            IL_00db:
                                ProjectData.ClearProjectError();
                                num3 = 0;
                                goto IL_00e3;
                            IL_00e3:
                                num = 15;
                                DataModul.DB_PersonTable.Index = "PerNr";
                                goto IL_00f6;
                            IL_00f6:
                                num = 16;
                                DataModul.DB_PersonTable.MoveLast();
                                goto IL_0105;
                            IL_0105:
                                num = 17;
                                num6 = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsLong();
                                goto IL_0128;
                            IL_0128:
                                num = 18;
                                Label9.Text = "";
                                goto IL_013c;
                            IL_013c:
                                num = 19;
                                num7 = 1L;
                                num8 = num6;
                                num5 = num7;
                                goto IL_03cd;
                            IL_03cd:
                                if (num5 <= num8)
                                {
                                    goto IL_014b;
                                }
                                goto IL_03d5;
                            IL_03d5:
                                num = 43;
                                Label8.Text = "Personen fertig";
                                goto end_IL_0000_2;
                            IL_014b:
                                num = 20;
                                flag = false;
                                goto IL_0151;
                            IL_0151:
                                num = 21;
                                _Modul1.Instance.PersInArb = (int)num5;
                                goto IL_015c;
                            IL_015c:
                                num = 22;
                                Label8.Text = "Person " + num5.AsString() + " von" + num6.AsString() + " in Arbeit";
                                goto IL_01b3;
                            IL_01b3:
                                num = 23;
                                Application.DoEvents();
                                goto IL_01bc;
                            IL_01bc:
                                num = 24;
                                DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                                goto IL_021a;
                            IL_021a:
                                num = 25;
                                if (!DataModul.DB_PersonTable.NoMatch)
                                {
                                    goto IL_022f;
                                }
                                goto IL_03bc;
                            IL_022f:
                                num = 28;
                                if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value))
                                {
                                    goto IL_025b;
                                }
                                goto IL_0386;
                            IL_025b:
                                num = 29;
                                if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                {
                                    goto IL_0293;
                                }
                                goto IL_0386;
                            IL_0293:
                                num = 30;
                                RichTextBox1.SelectedText = Conversions.ToString(Operators.AddObject(string.Concat("Paten zu Person" + num5.AsString(), "\n") + DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value + '\n', '\n'));
                                goto IL_02fc;
                            IL_02fc:
                                num = 31;
                                text = Strings.Replace(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString(), ";", ",");
                                goto IL_0332;
                            IL_0332:
                                num = 32;
                                text = text.Replace("\n", " ");
                                goto IL_034c;
                            IL_034c:
                                num = 33;
                                FileSystem.PrintLine(2, "Person" + num5.AsString() + ";Paten ;" + text);
                                goto IL_0380;
                            IL_0380:
                                num = 34;
                                flag = true;
                                goto IL_0386;
                            IL_0386:
                                num = 37;
                                if (flag)
                                {
                                    goto IL_038d;
                                }
                                goto IL_03bc;
                            IL_038d:
                                num = 38;
                                RichTextBox1.SelectedText = "**** Ende Person " + num5.AsString() + "****\n";
                                goto IL_03b6;
                            IL_03b6:
                                num = 39;
                                flag = false;
                                goto IL_03bc;
                            end_IL_0000:
                                break;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 1309;
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

        private void Button7_Click(object sender, EventArgs e)
        {
            FileSystem.FileClose(2);
            Process.Start(_Modul1.Instance.GenFreeDir + "\\Temp\\Neu1.csv");
        }
    }
}
