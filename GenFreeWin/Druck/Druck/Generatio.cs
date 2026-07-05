using Druck.My;
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
using GenFree;
using System.Collections.Generic;
using System.Linq;
using BaseLib.Helper;
using GenFree.Interfaces.Sys;
using Druck.Views;

namespace Druck;

[DesignerGenerated]
internal class Generatio : Form
{
    private IContainer components;
    public IModul1 Modul1;

    // Local fields - nur in dieser Klasse verwendet
    private int _iBemZahl;
    private IList<string> _lstKontBem = new List<string>(new string[50]);
    private byte _byHoch = 3;
    private bool _xPat;
    private bool _xTrau;
    private string _sLiText = "";
    private IList<string> _lstKontSP = new List<string>(new string[50]);
    private IList<string> _lstKontSP1 = new List<string>(new string[50]);

    public ToolTip ToolTip1;

    [AccessedThroughProperty("List3")]
    private ListBox _List3;

    [AccessedThroughProperty("_Command_17")]
    private Button __Command_17;

    [AccessedThroughProperty("_Command_16")]
    private Button __Command_16;

    [AccessedThroughProperty("_Command_15")]
    private Button __Command_15;

    [AccessedThroughProperty("_Command_14")]
    private Button __Command_14;

    [AccessedThroughProperty("Frame3")]
    private GroupBox _Frame3;

    [AccessedThroughProperty("_Command2_3")]
    private Button __Command2_3;

    [AccessedThroughProperty("_Command_13")]
    private Button __Command_13;

    [AccessedThroughProperty("_Command_12")]
    private Button __Command_12;

    [AccessedThroughProperty("_Command_8")]
    private Button __Command_8;

    [AccessedThroughProperty("_Command_9")]
    private Button __Command_9;

    [AccessedThroughProperty("_Command_10")]
    private Button __Command_10;

    [AccessedThroughProperty("_Command_11")]
    private Button __Command_11;

    [AccessedThroughProperty("Frame2")]
    private GroupBox _Frame2;

    [AccessedThroughProperty("_Command_7")]
    private Button __Command_7;

    [AccessedThroughProperty("_Command_6")]
    private Button __Command_6;

    [AccessedThroughProperty("_Command_5")]
    private Button __Command_5;

    [AccessedThroughProperty("_Command_4")]
    private Button __Command_4;

    [AccessedThroughProperty("_Command_3")]
    private Button __Command_3;

    [AccessedThroughProperty("_Command_2")]
    private Button __Command_2;

    [AccessedThroughProperty("List2")]
    private ListBox _List2;

    [AccessedThroughProperty("List1")]
    private ListBox _List1;

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

    [AccessedThroughProperty("Frame1")]
    private GroupBox _Frame1;

    [AccessedThroughProperty("_RtB_0")]
    private RichTextBox __RtB_0;

    [AccessedThroughProperty("_RtB_1")]
    private RichTextBox __RtB_1;

    [AccessedThroughProperty("_RtB_2")]
    private RichTextBox __RtB_2;

    [AccessedThroughProperty("Label2")]
    private Label _Label2;

    [AccessedThroughProperty("Command_Renamed")]
    private ControlArray<Button> _Command_Renamed;

    [AccessedThroughProperty("Command2")]
    private ControlArray<Button> _Command2;

    [AccessedThroughProperty("Label1")]
    private ControlArray<Label> _Label1;

    [AccessedThroughProperty("RtB")]
    private RichTextBoxArray _RtB;

    [AccessedThroughProperty("ListBox1")]
    private ListBox _ListBox1;

    [AccessedThroughProperty("Label3")]
    private Label _Label3;

    [AccessedThroughProperty("PictureBox1")]
    private PictureBox _PictureBox1;

    private int PerSp1;

    private int Perstatic;

    private int Tot;

    private bool LSchalt;

    private string PatText;

    private string Datklein;

    private string Datum6;

    private short Abst;

    private short IQ;

    private bool DGSchalt;

    private bool Hei;

    [SpecialName]
    private byte _0024STATIC_0024Weitehen_00242001_0024a;

    [SpecialName]
    private byte _0024STATIC_0024Weitehen_00242001_0024I4;

    [SpecialName]
    private short _0024STATIC_0024Weitehen_00242001_0024D;

    [SpecialName]
    private byte _0024STATIC_0024Weitehen_00242001_0024Fa;
    private bool M1_Ki;
    private double M_Sgg;
    private string M_Namen;
    private int Modul1_J;

    public virtual ListBox List3
    {
        get
        {
            return _List3;
        }

        set
        {
            _List3 = value;
        }
    }

    public virtual Button _Command_17
    {
        get
        {
            return __Command_17;
        }

        set
        {
            __Command_17 = value;
        }
    }

    public virtual Button _Command_16
    {
        get
        {
            return __Command_16;
        }

        set
        {
            __Command_16 = value;
        }
    }

    public virtual Button _Command_15
    {
        get
        {
            return __Command_15;
        }

        set
        {
            __Command_15 = value;
        }
    }

    public virtual Button _Command_14
    {
        get
        {
            return __Command_14;
        }

        set
        {
            __Command_14 = value;
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

    public virtual Button _Command2_3
    {
        get
        {
            return __Command2_3;
        }

        set
        {
            __Command2_3 = value;
        }
    }

    public virtual Button _Command_13
    {
        get
        {
            return __Command_13;
        }

        set
        {
            __Command_13 = value;
        }
    }

    public virtual Button _Command_12
    {
        get
        {
            return __Command_12;
        }

        set
        {
            __Command_12 = value;
        }
    }

    public virtual Button _Command_8
    {
        get
        {
            return __Command_8;
        }

        set
        {
            __Command_8 = value;
        }
    }

    public virtual Button _Command_9
    {
        get
        {
            return __Command_9;
        }

        set
        {
            __Command_9 = value;
        }
    }

    public virtual Button _Command_10
    {
        get
        {
            return __Command_10;
        }

        set
        {
            __Command_10 = value;
        }
    }

    public virtual Button _Command_11
    {
        get
        {
            return __Command_11;
        }

        set
        {
            __Command_11 = value;
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

    public virtual Button _Command_7
    {
        get
        {
            return __Command_7;
        }

        set
        {
            __Command_7 = value;
        }
    }

    public virtual Button _Command_6
    {
        get
        {
            return __Command_6;
        }

        set
        {
            __Command_6 = value;
        }
    }

    public virtual Button _Command_5
    {
        get
        {
            return __Command_5;
        }

        set
        {
            __Command_5 = value;
        }
    }

    public virtual Button _Command_4
    {
        get
        {
            return __Command_4;
        }

        set
        {
            __Command_4 = value;
        }
    }

    public virtual Button _Command_3
    {
        get
        {
            return __Command_3;
        }

        set
        {
            __Command_3 = value;
        }
    }

    public virtual Button _Command_2
    {
        get
        {
            return __Command_2;
        }

        set
        {
            __Command_2 = value;
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
            _List2 = value;
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
            _List1 = value;
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

    public virtual RichTextBox _RtB_0
    {
        get
        {
            return __RtB_0;
        }

        set
        {
            __RtB_0 = value;
        }
    }

    public virtual RichTextBox _RtB_1
    {
        get
        {
            return __RtB_1;
        }

        set
        {
            __RtB_1 = value;
        }
    }

    public virtual RichTextBox _RtB_2
    {
        get
        {
            return __RtB_2;
        }

        set
        {
            __RtB_2 = value;
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

    public virtual ControlArray<Button> Command2
    {
        get
        {
            return _Command2;
        }

        set
        {
            _Command2 = value;
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

    public virtual RichTextBoxArray RtB
    {
        get
        {
            return _RtB;
        }

        set
        {
            _RtB = value;
        }
    }

    internal virtual ListBox ListBox1
    {
        get
        {
            return _ListBox1;
        }

        set
        {
            _ListBox1 = value;
        }
    }

    internal virtual Label Label3
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

    internal virtual PictureBox PictureBox1
    {
        get
        {
            return _PictureBox1;
        }

        set
        {
            _PictureBox1 = value;
        }
    }

    [DebuggerNonUserCode]
    public Generatio()
    {
        base.Load += Sippenlist_Load;
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
        this.List3 = new System.Windows.Forms.ListBox();
        this.Frame3 = new System.Windows.Forms.GroupBox();
        this._Command_17 = new System.Windows.Forms.Button();
        this._Command_16 = new System.Windows.Forms.Button();
        this._Command_15 = new System.Windows.Forms.Button();
        this._Command_14 = new System.Windows.Forms.Button();
        this.Frame2 = new System.Windows.Forms.GroupBox();
        this._Command2_3 = new System.Windows.Forms.Button();
        this._Command_13 = new System.Windows.Forms.Button();
        this._Command_12 = new System.Windows.Forms.Button();
        this._Command_8 = new System.Windows.Forms.Button();
        this._Command_9 = new System.Windows.Forms.Button();
        this._Command_10 = new System.Windows.Forms.Button();
        this._Command_11 = new System.Windows.Forms.Button();
        this._Command_7 = new System.Windows.Forms.Button();
        this._Command_6 = new System.Windows.Forms.Button();
        this._Command_5 = new System.Windows.Forms.Button();
        this._Command_4 = new System.Windows.Forms.Button();
        this._Command_3 = new System.Windows.Forms.Button();
        this._Command_2 = new System.Windows.Forms.Button();
        this.List2 = new System.Windows.Forms.ListBox();
        this.List1 = new System.Windows.Forms.ListBox();
        this.Frame1 = new System.Windows.Forms.GroupBox();
        this._Command_0 = new System.Windows.Forms.Button();
        this._Command_1 = new System.Windows.Forms.Button();
        this._Label1_0 = new System.Windows.Forms.Label();
        this._Label1_1 = new System.Windows.Forms.Label();
        this._Label1_2 = new System.Windows.Forms.Label();
        this._Label1_3 = new System.Windows.Forms.Label();
        this._RtB_0 = new System.Windows.Forms.RichTextBox();
        this._RtB_1 = new System.Windows.Forms.RichTextBox();
        this._RtB_2 = new System.Windows.Forms.RichTextBox();
        this.Label2 = new System.Windows.Forms.Label();
        this.Command_Renamed = new ControlArray<System.Windows.Forms.Button>();
        this.Command2 = new ControlArray<System.Windows.Forms.Button>();
        this.Label1 = new ControlArray<System.Windows.Forms.Label>();
        this.RtB = new Microsoft.VisualBasic.Compatibility.VB6.RichTextBoxArray(this.components);
        this.ListBox1 = new System.Windows.Forms.ListBox();
        this.Label3 = new System.Windows.Forms.Label();
        this.PictureBox1 = new System.Windows.Forms.PictureBox();
        this.Frame3.SuspendLayout();
        this.Frame2.SuspendLayout();
        this.Frame1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.Command2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.Label1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.RtB).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
        this.SuspendLayout();
        this.List3.BackColor = System.Drawing.SystemColors.Window;
        this.List3.Cursor = System.Windows.Forms.Cursors.Default;
        this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List3.ItemHeight = 17;
        System.Drawing.Point point2 = this.List3.Location = new System.Drawing.Point(5, 10);
        System.Windows.Forms.Padding padding2 = this.List3.Margin = new System.Windows.Forms.Padding(4);
        this.List3.Name = "List3";
        this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        System.Drawing.Size size2 = this.List3.Size = new System.Drawing.Size(240, 412);
        this.List3.Sorted = true;
        this.List3.TabIndex = 32;
        this.List3.Visible = false;
        this.Frame3.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
        this.Frame3.Controls.Add(this._Command_17);
        this.Frame3.Controls.Add(this._Command_16);
        this.Frame3.Controls.Add(this._Command_15);
        this.Frame3.Controls.Add(this._Command_14);
        this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
        point2 = this.Frame3.Location = new System.Drawing.Point(336, 186);
        padding2 = this.Frame3.Margin = new System.Windows.Forms.Padding(4);
        this.Frame3.Name = "Frame3";
        padding2 = this.Frame3.Padding = new System.Windows.Forms.Padding(4);
        this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this.Frame3.Size = new System.Drawing.Size(233, 190);
        this.Frame3.TabIndex = 26;
        this.Frame3.TabStop = false;
        this.Frame3.Visible = false;
        this._Command_17.BackColor = System.Drawing.SystemColors.Control;
        this._Command_17.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_17.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_17, 17);
        point2 = this._Command_17.Location = new System.Drawing.Point(19, 139);
        padding2 = this._Command_17.Margin = new System.Windows.Forms.Padding(4);
        this._Command_17.Name = "_Command_17";
        this._Command_17.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_17.Size = new System.Drawing.Size(196, 25);
        this._Command_17.TabIndex = 30;
        this._Command_17.Text = "Namensliste";
        this._Command_17.UseVisualStyleBackColor = false;
        this._Command_17.Visible = false;
        this._Command_16.BackColor = System.Drawing.SystemColors.Control;
        this._Command_16.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_16.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_16, 16);
        point2 = this._Command_16.Location = new System.Drawing.Point(19, 107);
        padding2 = this._Command_16.Margin = new System.Windows.Forms.Padding(4);
        this._Command_16.Name = "_Command_16";
        this._Command_16.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_16.Size = new System.Drawing.Size(196, 25);
        this._Command_16.TabIndex = 29;
        this._Command_16.Text = "Namensindex Kurzform";
        this._Command_16.UseVisualStyleBackColor = false;
        this._Command_16.Visible = false;
        this._Command_15.BackColor = System.Drawing.SystemColors.Control;
        this._Command_15.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_15.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_15, 15);
        point2 = this._Command_15.Location = new System.Drawing.Point(19, 60);
        padding2 = this._Command_15.Margin = new System.Windows.Forms.Padding(4);
        this._Command_15.Name = "_Command_15";
        this._Command_15.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_15.Size = new System.Drawing.Size(196, 25);
        this._Command_15.TabIndex = 28;
        this._Command_15.Text = "Index Namen Vornamen";
        this._Command_15.UseVisualStyleBackColor = false;
        this._Command_14.BackColor = System.Drawing.SystemColors.Control;
        this._Command_14.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_14.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_14, 14);
        point2 = this._Command_14.Location = new System.Drawing.Point(19, 29);
        padding2 = this._Command_14.Margin = new System.Windows.Forms.Padding(4);
        this._Command_14.Name = "_Command_14";
        this._Command_14.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_14.Size = new System.Drawing.Size(196, 25);
        this._Command_14.TabIndex = 27;
        this._Command_14.Text = "Namensindex";
        this._Command_14.UseVisualStyleBackColor = false;
        this.Frame2.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
        this.Frame2.Controls.Add(this._Command2_3);
        this.Frame2.Controls.Add(this._Command_13);
        this.Frame2.Controls.Add(this._Command_12);
        this.Frame2.Controls.Add(this._Command_8);
        this.Frame2.Controls.Add(this._Command_9);
        this.Frame2.Controls.Add(this._Command_10);
        this.Frame2.Controls.Add(this._Command_11);
        this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
        point2 = this.Frame2.Location = new System.Drawing.Point(336, 144);
        padding2 = this.Frame2.Margin = new System.Windows.Forms.Padding(4);
        this.Frame2.Name = "Frame2";
        padding2 = this.Frame2.Padding = new System.Windows.Forms.Padding(4);
        this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this.Frame2.Size = new System.Drawing.Size(231, 247);
        this.Frame2.TabIndex = 18;
        this.Frame2.TabStop = false;
        this.Frame2.Visible = false;
        this._Command2_3.BackColor = System.Drawing.SystemColors.Control;
        this._Command2_3.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command2_3.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command2.SetIndex(this._Command2_3, 3);
        point2 = this._Command2_3.Location = new System.Drawing.Point(75, 13);
        padding2 = this._Command2_3.Margin = new System.Windows.Forms.Padding(4);
        this._Command2_3.Name = "_Command2_3";
        this._Command2_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command2_3.Size = new System.Drawing.Size(169, 51);
        this._Command2_3.TabIndex = 22;
        this._Command2_3.Text = "Indizierungsdatei Orte für WinWord";
        this._Command2_3.UseVisualStyleBackColor = false;
        this._Command2_3.Visible = false;
        this._Command_13.BackColor = System.Drawing.SystemColors.Control;
        this._Command_13.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_13.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_13, 13);
        point2 = this._Command_13.Location = new System.Drawing.Point(21, 169);
        padding2 = this._Command_13.Margin = new System.Windows.Forms.Padding(4);
        this._Command_13.Name = "_Command_13";
        this._Command_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_13.Size = new System.Drawing.Size(196, 25);
        this._Command_13.TabIndex = 25;
        this._Command_13.Text = "Verzeichnis Namen-Orte";
        this._Command_13.UseVisualStyleBackColor = false;
        this._Command_12.BackColor = System.Drawing.SystemColors.Control;
        this._Command_12.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_12.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_12, 12);
        point2 = this._Command_12.Location = new System.Drawing.Point(21, 201);
        padding2 = this._Command_12.Margin = new System.Windows.Forms.Padding(4);
        this._Command_12.Name = "_Command_12";
        this._Command_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_12.Size = new System.Drawing.Size(196, 25);
        this._Command_12.TabIndex = 24;
        this._Command_12.Text = "Verzeichnis Orte-Namen";
        this._Command_12.UseVisualStyleBackColor = false;
        this._Command_8.BackColor = System.Drawing.SystemColors.Control;
        this._Command_8.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_8.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_8, 8);
        point2 = this._Command_8.Location = new System.Drawing.Point(21, 136);
        padding2 = this._Command_8.Margin = new System.Windows.Forms.Padding(4);
        this._Command_8.Name = "_Command_8";
        this._Command_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_8.Size = new System.Drawing.Size(196, 25);
        this._Command_8.TabIndex = 23;
        this._Command_8.Text = "Ortsverzeichnis";
        this._Command_8.UseVisualStyleBackColor = false;
        this._Command_9.BackColor = System.Drawing.SystemColors.Control;
        this._Command_9.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_9.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_9, 9);
        point2 = this._Command_9.Location = new System.Drawing.Point(21, 26);
        padding2 = this._Command_9.Margin = new System.Windows.Forms.Padding(4);
        this._Command_9.Name = "_Command_9";
        this._Command_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_9.Size = new System.Drawing.Size(196, 25);
        this._Command_9.TabIndex = 21;
        this._Command_9.Text = "Ortsindex";
        this._Command_9.UseVisualStyleBackColor = false;
        this._Command_10.BackColor = System.Drawing.SystemColors.Control;
        this._Command_10.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_10.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_10, 10);
        point2 = this._Command_10.Location = new System.Drawing.Point(21, 94);
        padding2 = this._Command_10.Margin = new System.Windows.Forms.Padding(4);
        this._Command_10.Name = "_Command_10";
        this._Command_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_10.Size = new System.Drawing.Size(196, 25);
        this._Command_10.TabIndex = 20;
        this._Command_10.Text = "Index Orte-Namen";
        this._Command_10.UseVisualStyleBackColor = false;
        this._Command_11.BackColor = System.Drawing.SystemColors.Control;
        this._Command_11.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_11.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_11, 11);
        point2 = this._Command_11.Location = new System.Drawing.Point(21, 60);
        padding2 = this._Command_11.Margin = new System.Windows.Forms.Padding(4);
        this._Command_11.Name = "_Command_11";
        this._Command_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_11.Size = new System.Drawing.Size(196, 25);
        this._Command_11.TabIndex = 19;
        this._Command_11.Text = "Index Namen-Orte";
        this._Command_11.UseVisualStyleBackColor = false;
        this._Command_7.BackColor = System.Drawing.SystemColors.Control;
        this._Command_7.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_7.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_7, 7);
        point2 = this._Command_7.Location = new System.Drawing.Point(449, 693);
        padding2 = this._Command_7.Margin = new System.Windows.Forms.Padding(4);
        this._Command_7.Name = "_Command_7";
        this._Command_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_7.Size = new System.Drawing.Size(131, 26);
        this._Command_7.TabIndex = 17;
        this._Command_7.Text = "Personenindex";
        this._Command_7.UseVisualStyleBackColor = false;
        this._Command_6.BackColor = System.Drawing.SystemColors.Control;
        this._Command_6.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_6.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_6, 6);
        point2 = this._Command_6.Location = new System.Drawing.Point(878, 693);
        padding2 = this._Command_6.Margin = new System.Windows.Forms.Padding(4);
        this._Command_6.Name = "_Command_6";
        this._Command_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_6.Size = new System.Drawing.Size(131, 26);
        this._Command_6.TabIndex = 16;
        this._Command_6.Text = "Druckmenü";
        this._Command_6.UseVisualStyleBackColor = false;
        this._Command_5.BackColor = System.Drawing.SystemColors.Control;
        this._Command_5.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_5.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_5, 5);
        point2 = this._Command_5.Location = new System.Drawing.Point(588, 693);
        padding2 = this._Command_5.Margin = new System.Windows.Forms.Padding(4);
        this._Command_5.Name = "_Command_5";
        this._Command_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_5.Size = new System.Drawing.Size(131, 26);
        this._Command_5.TabIndex = 15;
        this._Command_5.Text = "Ortsindex";
        this._Command_5.UseVisualStyleBackColor = false;
        this._Command_4.BackColor = System.Drawing.SystemColors.Control;
        this._Command_4.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_4.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_4, 4);
        point2 = this._Command_4.Location = new System.Drawing.Point(21, 693);
        padding2 = this._Command_4.Margin = new System.Windows.Forms.Padding(4);
        this._Command_4.Name = "_Command_4";
        this._Command_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_4.Size = new System.Drawing.Size(131, 26);
        this._Command_4.TabIndex = 14;
        this._Command_4.Text = "Drucken";
        this._Command_4.UseVisualStyleBackColor = false;
        this._Command_3.BackColor = System.Drawing.SystemColors.Control;
        this._Command_3.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_3.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_3, 3);
        point2 = this._Command_3.Location = new System.Drawing.Point(160, 693);
        padding2 = this._Command_3.Margin = new System.Windows.Forms.Padding(4);
        this._Command_3.Name = "_Command_3";
        this._Command_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_3.Size = new System.Drawing.Size(131, 26);
        this._Command_3.TabIndex = 13;
        this._Command_3.Text = "in Datei";
        this._Command_3.UseVisualStyleBackColor = false;
        this._Command_2.BackColor = System.Drawing.SystemColors.Control;
        this._Command_2.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_2.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_2, 2);
        point2 = this._Command_2.Location = new System.Drawing.Point(309, 693);
        padding2 = this._Command_2.Margin = new System.Windows.Forms.Padding(4);
        this._Command_2.Name = "_Command_2";
        this._Command_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_2.Size = new System.Drawing.Size(131, 26);
        this._Command_2.TabIndex = 12;
        this._Command_2.Text = "Generationenliste";
        this._Command_2.UseVisualStyleBackColor = false;
        this.List2.BackColor = System.Drawing.SystemColors.Window;
        this.List2.Cursor = System.Windows.Forms.Cursors.Default;
        this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List2.ItemHeight = 17;
        point2 = this.List2.Location = new System.Drawing.Point(21, 175);
        padding2 = this.List2.Margin = new System.Windows.Forms.Padding(4);
        this.List2.Name = "List2";
        this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this.List2.Size = new System.Drawing.Size(64, 21);
        this.List2.Sorted = true;
        this.List2.TabIndex = 11;
        this.List2.Visible = false;
        this.List1.BackColor = System.Drawing.SystemColors.Window;
        this.List1.Cursor = System.Windows.Forms.Cursors.Default;
        this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List1.ItemHeight = 17;
        point2 = this.List1.Location = new System.Drawing.Point(45, 207);
        padding2 = this.List1.Margin = new System.Windows.Forms.Padding(4);
        this.List1.Name = "List1";
        this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this.List1.Size = new System.Drawing.Size(61, 89);
        this.List1.Sorted = true;
        this.List1.TabIndex = 10;
        this.List1.Visible = false;
        this.Frame1.BackColor = System.Drawing.Color.Red;
        this.Frame1.Controls.Add(this._Command_0);
        this.Frame1.Controls.Add(this._Command_1);
        this.Frame1.Controls.Add(this._Label1_0);
        this.Frame1.Controls.Add(this._Label1_1);
        this.Frame1.Controls.Add(this._Label1_2);
        this.Frame1.Controls.Add(this._Label1_3);
        this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
        point2 = this.Frame1.Location = new System.Drawing.Point(224, 288);
        padding2 = this.Frame1.Margin = new System.Windows.Forms.Padding(4);
        this.Frame1.Name = "Frame1";
        padding2 = this.Frame1.Padding = new System.Windows.Forms.Padding(4);
        this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this.Frame1.Size = new System.Drawing.Size(543, 175);
        this.Frame1.TabIndex = 3;
        this.Frame1.TabStop = false;
        this._Command_0.BackColor = System.Drawing.SystemColors.Control;
        this._Command_0.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_0.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_0, 0);
        point2 = this._Command_0.Location = new System.Drawing.Point(47, 127);
        padding2 = this._Command_0.Margin = new System.Windows.Forms.Padding(4);
        this._Command_0.Name = "_Command_0";
        this._Command_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_0.Size = new System.Drawing.Size(141, 26);
        this._Command_0.TabIndex = 5;
        this._Command_0.Text = "Abbrechen";
        this._Command_0.UseVisualStyleBackColor = false;
        this._Command_1.BackColor = System.Drawing.SystemColors.Control;
        this._Command_1.Cursor = System.Windows.Forms.Cursors.Default;
        this._Command_1.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Command_Renamed.SetIndex(this._Command_1, 1);
        point2 = this._Command_1.Location = new System.Drawing.Point(327, 127);
        padding2 = this._Command_1.Margin = new System.Windows.Forms.Padding(4);
        this._Command_1.Name = "_Command_1";
        this._Command_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Command_1.Size = new System.Drawing.Size(141, 26);
        this._Command_1.TabIndex = 4;
        this._Command_1.Text = "OK";
        this._Command_1.UseVisualStyleBackColor = false;
        this._Label1_0.BackColor = System.Drawing.Color.White;
        this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label1.SetIndex(this._Label1_0, 0);
        point2 = this._Label1_0.Location = new System.Drawing.Point(9, 41);
        padding2 = this._Label1_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this._Label1_0.Name = "_Label1_0";
        this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Label1_0.Size = new System.Drawing.Size(524, 20);
        this._Label1_0.TabIndex = 9;
        this._Label1_1.BackColor = System.Drawing.Color.White;
        this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label1.SetIndex(this._Label1_1, 1);
        point2 = this._Label1_1.Location = new System.Drawing.Point(9, 63);
        padding2 = this._Label1_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this._Label1_1.Name = "_Label1_1";
        this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Label1_1.Size = new System.Drawing.Size(524, 20);
        this._Label1_1.TabIndex = 8;
        this._Label1_2.BackColor = System.Drawing.Color.White;
        this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label1.SetIndex(this._Label1_2, 2);
        point2 = this._Label1_2.Location = new System.Drawing.Point(9, 85);
        padding2 = this._Label1_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this._Label1_2.Name = "_Label1_2";
        this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this._Label1_2.Size = new System.Drawing.Size(524, 20);
        this._Label1_2.TabIndex = 7;
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
        this._Label1_3.Text = "Nachfahren von";
        this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        this.RtB.SetIndex(this._RtB_0, 0);
        point2 = this._RtB_0.Location = new System.Drawing.Point(0, 3);
        padding2 = this._RtB_0.Margin = new System.Windows.Forms.Padding(4);
        this._RtB_0.Name = "_RtB_0";
        this._RtB_0.RightMargin = 734;
        size2 = this._RtB_0.Size = new System.Drawing.Size(88, 73);
        this._RtB_0.TabIndex = 0;
        this._RtB_0.Text = "";
        this._RtB_0.Visible = false;
        this.RtB.SetIndex(this._RtB_1, 1);
        point2 = this._RtB_1.Location = new System.Drawing.Point(0, 5);
        padding2 = this._RtB_1.Margin = new System.Windows.Forms.Padding(4);
        this._RtB_1.Name = "_RtB_1";
        this._RtB_1.RightMargin = 600;
        size2 = this._RtB_1.Size = new System.Drawing.Size(88, 73);
        this._RtB_1.TabIndex = 1;
        this._RtB_1.Text = "1";
        this._RtB_1.Visible = false;
        this.RtB.SetIndex(this._RtB_2, 2);
        point2 = this._RtB_2.Location = new System.Drawing.Point(0, 3);
        padding2 = this._RtB_2.Margin = new System.Windows.Forms.Padding(4);
        this._RtB_2.Name = "_RtB_2";
        this._RtB_2.RightMargin = 600;
        size2 = this._RtB_2.Size = new System.Drawing.Size(88, 73);
        this._RtB_2.TabIndex = 2;
        this._RtB_2.Text = "2";
        this._RtB_2.Visible = false;
        this.Label2.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
        this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
        this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
        point2 = this.Label2.Location = new System.Drawing.Point(112, 662);
        padding2 = this.Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.Label2.Name = "Label2";
        this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        size2 = this.Label2.Size = new System.Drawing.Size(809, 25);
        this.Label2.TabIndex = 31;
        this.ListBox1.FormattingEnabled = true;
        this.ListBox1.ItemHeight = 17;
        point2 = this.ListBox1.Location = new System.Drawing.Point(658, 109);
        this.ListBox1.Name = "ListBox1";
        size2 = this.ListBox1.Size = new System.Drawing.Size(120, 89);
        this.ListBox1.TabIndex = 33;
        this.ListBox1.Visible = false;
        this.Label3.AutoSize = true;
        point2 = this.Label3.Location = new System.Drawing.Point(850, 408);
        this.Label3.Name = "Label3";
        size2 = this.Label3.Size = new System.Drawing.Size(51, 17);
        this.Label3.TabIndex = 34;
        this.Label3.Text = "Label3";
        this.Label3.Visible = false;
        point2 = this.PictureBox1.Location = new System.Drawing.Point(856, 299);
        this.PictureBox1.Name = "PictureBox1";
        size2 = this.PictureBox1.Size = new System.Drawing.Size(100, 50);
        this.PictureBox1.TabIndex = 35;
        this.PictureBox1.TabStop = false;
        this.PictureBox1.Visible = false;
        System.Drawing.SizeF sizeF2 = this.AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.SystemColors.Control;
        size2 = this.ClientSize = new System.Drawing.Size(1022, 748);
        this.ControlBox = false;
        this.Controls.Add(this.PictureBox1);
        this.Controls.Add(this.Label3);
        this.Controls.Add(this.ListBox1);
        this.Controls.Add(this.List3);
        this.Controls.Add(this.Frame3);
        this.Controls.Add(this.Frame2);
        this.Controls.Add(this._Command_7);
        this.Controls.Add(this._Command_6);
        this.Controls.Add(this._Command_5);
        this.Controls.Add(this._Command_4);
        this.Controls.Add(this._Command_3);
        this.Controls.Add(this._Command_2);
        this.Controls.Add(this.List2);
        this.Controls.Add(this.List1);
        this.Controls.Add(this.Frame1);
        this.Controls.Add(this._RtB_0);
        this.Controls.Add(this._RtB_1);
        this.Controls.Add(this._RtB_2);
        this.Controls.Add(this.Label2);
        this.Cursor = System.Windows.Forms.Cursors.Default;
        this.Font = new System.Drawing.Font("Arial", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        padding2 = this.Margin = new System.Windows.Forms.Padding(4);
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "Generatio";
        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Frame3.ResumeLayout(false);
        this.Frame2.ResumeLayout(false);
        this.Frame1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.Command_Renamed).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.Command2).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.Label1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.RtB).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_2646, IL_2814
        int index = Command_Renamed.GetIndex((Button)eventSender);
        string text = "";
        string text2 = "";
        string right = "";
        Modul1.Schalt = 0;
        checked
        {
            switch (index)
            {
                case 0:
                    Close();
                    MyProject.Forms.Druck.Show();
                    break;
                case 1:
                    {
                        MyProject.Forms.AW.Close();
                        MyProject.Forms.AW.ShowDialog("SGF");
                        RtB[0].Font = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[1].Font = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[2].Font = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        Modul1.PersSp = Modul1.PersInArb;
                        DataModul.DB_PersonTable.Index = "PerNr";
                        DataModul.DB_PersonTable.MoveFirst();
                        byte Tot = default;
                        for (; !DataModul.DB_PersonTable.EOF; DataModul.DB_PersonTable.MoveNext())
                        {
                            Modul1.PersInArb = DataModul.DB_PersonTable.Fields[PersonFields.PersNr].AsInt();
                            if (Modul1.DAus[122].AsDouble() == 1.0)
                            {
                                Module2.TotPrüf1(ref Tot);
                                if (Tot == 2)
                                {
                                    continue;
                                }
                            }
                            if (Modul1.DAus[120] == "1")
                            {
                                Datcheck();
                                if (Datum6.AsDouble() > Modul1.DAus[121].AsDouble())
                                {
                                    DataModul.DT_SperrTable.AddNew();
                                    DataModul.DT_SperrTable.Fields["Pernr"].Value = Modul1.PersInArb;
                                    DataModul.DT_SperrTable.Update();
                                }
                            }
                        }
                        Modul1.PersInArb = Modul1.PersSp;
                        List3.Items.Clear();
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        if (Modul1.DAus[89] == "1")
                        {
                            Modul1.Kont[0] = Modul1.Kont[0].ToUpper();
                        }
                        if (Modul1.Kont[1] != "")
                        {
                            Modul1.Kont[1] = Modul1.Kont[1] + " ";
                        }
                        if (Modul1.Kont[2] != "")
                        {
                            Modul1.Kont[2] = " " + Modul1.Kont[2];
                        }
                        RtB[0].SelectionAlignment = HorizontalAlignment.Center;
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 1.5), FontStyle.Bold);
                        RtB[0].SelectedText = "Generationenliste für " + Modul1.Kont[3].TrimEnd() + " " + Modul1.Kont[1] + Modul1.Kont[0] + Modul1.Kont[2] + "\n";
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                        RtB[0].SelectedText = $" von {Modul1.User.Name.Trim()} mit {Modul1.AppName} aus Mandant: {Modul1.Verz}\n";
                        RtB[0].SelectionAlignment = HorizontalAlignment.Left;
                        RtB[0].SelectedText = "\n";
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[0].Visible = true;
                        Frame1.Visible = false;
                        byte b2 = 0;
                        do
                        {
                            RtB[b2].Width = Width - 20;
                            RtB[b2].RightMargin = Width - 60;
                            RtB[b2].Height = Height - 130;
                            b2 = (byte)unchecked((uint)(b2 + 1));
                        }
                        while (unchecked(b2) <= 2u);
                        DataModul.DT_DescendentTable.Index = "GNR";
                        DataModul.DT_DescendentTable.MoveFirst();
                        DataModul.DT_DescendentTable.Index = "GNR";
                        DataModul.DT_DescendentTable.MoveFirst();
                        string text6 = 0.AsString();
                        byte b4 = default;
                        bool flag = default;
                        while (!DataModul.DT_DescendentTable.EOF)
                        {
                            text6 += ">";
                            Label2.Text = text6;
                            Label2.Refresh();
                            if (text6.Length == 70)
                            {
                                text6 = "";
                            }
                            byte b3 = (byte)Math.Round(DataModul.DT_DescendentTable.Fields["gen"].AsDouble());
                            RtB[0].SelectionIndent = 0;
                            if (unchecked(b3 > (uint)b4))
                            {
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() + 4.0), FontStyle.Bold);
                                RtB[0].SelectedText = "\n" + Strings.Right("  " + b3.AsString().Trim(), 2) + ".Generation\n";
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                b4 = b3;
                                if (b3 < 3)
                                {
                                    RtB[0].SelectedText = "\n";
                                }
                            }
                            string text9;
                            if (b3 > 2)
                            {
                                string key = DataModul.DT_DescendentTable.Fields["Nr"].AsString();
                                string text7 = DataModul.DT_DescendentTable.Fields["Nr"].AsString();
                                int num = DataModul.DT_DescendentTable.Fields["LFNr"].AsInt();
                                short num2 = 1;
                                while (true)
                                {
                                    text7 = text7.Left(text7.Length - 1);
                                    string text8;
                                    byte b5;
                                    if (text7.Right(1) == ".")
                                    {
                                        text8 = text7;
                                        if (text8 == right)
                                        {
                                            break;
                                        }
                                        right = text8;
                                        DataModul.DT_DescendentTable.Index = "Nr";
                                        DataModul.DT_DescendentTable.Seek("=", text8);
                                        if (DataModul.DT_DescendentTable.NoMatch)
                                        {
                                            break;
                                        }
                                        Modul1.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                                        string text3 = " des ";
                                        b5 = 1;
                                        Modul1.PerSatzLes(Modul1.PersInArb);
                                        if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                                        {
                                            text3 = " der ";
                                            b5 = 2;
                                        }
                                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                        Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
                                        RtB[0].SelectedText = "\n";
                                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                        RtB[0].SelectionIndent = 0;
                                        if (Modul1.DAus[89] == "1")
                                        {
                                            Modul1.Kont[0] = Modul1.Kont[0].ToUpper();
                                        }
                                        if (Modul1.DAus[120] == "1")
                                        {
                                            DataModul.DT_SperrTable.Index = "Nr";
                                            DataModul.DT_SperrTable.Seek("=", Modul1.PersInArb);
                                            if (!DataModul.DT_SperrTable.NoMatch)
                                            {
                                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                                RtB[0].SelectedText = "Kinder von";
                                                RtB[0].SelectedText = " " + Modul1.Datschuname + " ";
                                                goto IL_0be4;
                                            }
                                        }
                                        if (Modul1.DAus[113].AsDouble() == 0.0)
                                        {
                                            RtB[0].SelectedText = "Kinder" + text3 + Modul1.Kont[3] + " ";
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                            RtB[0].SelectedText = Modul1.Kont[0];
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                            RtB[0].SelectedText = " (" + Operators.SubtractObject(DataModul.DT_DescendentTable.Fields["Gen"].Value, 1).AsString() + "- " + text8.Trim() + ") ";
                                        }
                                        else
                                        {
                                            RtB[0].SelectedText = "Kinder" + text3 + Modul1.Kont[3] + " ";
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                            RtB[0].SelectedText = Modul1.Kont[0];
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                            RtB[0].SelectedText = " (" + Operators.SubtractObject(DataModul.DT_DescendentTable.Fields["Gen"].Value, 1).AsString() + "-" + DataModul.DT_DescendentTable.Fields["LFNR"].AsString().Trim() + ") ";
                                        }
                                        goto IL_0be4;
                                    }
                                    num2 = (short)unchecked(num2 + 1);
                                    if (num2 > 100)
                                    {
                                        break;
                                    }
                                    continue;
                                IL_0be4:
                                    Modul1.Eltq = text8.Trim();
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    RtB[0].SelectedText = "";
                                    if (b5 == 2)
                                    {
                                        List1.Items.Clear();
                                        Modul1.eLKennz = ELinkKennz.lkMother;
                                        var aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                                        text9 = Modul1.UbgT;
                                        flag = false;
                                        byte b6 = 1;
                                        do
                                        {
                                            if (text9.Length > 10)
                                            {
                                                flag = true;
                                            }
                                            if (text9.Length > 0)
                                            {
                                                Modul1.FamInArb = (int)Math.Round(text9.Left(10).AsDouble());
                                                text9 = Strings.Mid(text9, 11, text9.Length);
                                                DataModul.DB_EventTable.Index = "ArtNr";
                                                byte b7 = 0;
                                                int num3 = 502;
                                                do
                                                {
                                                    DataModul.DB_EventTable.Seek("=", num3.AsString(), Modul1.FamInArb.AsString(), "0");
                                                    if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                    {
                                                        List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + Modul1.FamInArb.AsString().Right(10));
                                                        b7 = 1;
                                                        break;
                                                    }
                                                    num3++;
                                                }
                                                while (num3 <= 505);
                                                if (b7 == 0)
                                                {
                                                    List1.Items.Add("        " + "          " + Modul1.FamInArb.AsString().Right(10));
                                                }
                                            }
                                            if (text9.Length < 10)
                                            {
                                                break;
                                            }
                                            b6 = (byte)unchecked((uint)(b6 + 1));
                                        }
                                        while (unchecked(b6) <= 10u);
                                        int count = List1.Items.Count;
                                        for (int num3 = 0; num3 <= count; num3++)
                                        {
                                            Modul1.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num3].AsString(), 9, 10)));
                                            Modul1.Famles();
                                            if (Modul1.FamInArb == 0)
                                            {
                                                break;
                                            }
                                            int mann = Modul1.Family.Mann;
                                            if (flag)
                                            {
                                                RtB[0].SelectedText = " (" + Strings.Chr(97 + num3).AsString() + ") ";
                                            }
                                            bool neb = true;
                                            Heidat(ref neb);
                                            RtB[0].SelectionIndent = 0;
                                            if ((Modul1.Family.Mann == 0) & (Modul1.Family.Frau > 0))
                                            {
                                                RtB[0].SelectedText = " mit Unbekannt";
                                            }
                                            Modul1.PersInArb = mann;
                                            if (mann <= 0)
                                            {
                                                continue;
                                            }
                                            Modul1.Schalt = 0;
                                            if (Modul1.DAus[120] == "1")
                                            {
                                                DataModul.DT_SperrTable.Index = "Nr";
                                                DataModul.DT_SperrTable.Seek("=", Modul1.PersInArb);
                                                if (!DataModul.DT_SperrTable.NoMatch)
                                                {
                                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                                    RtB[0].SelectedText = " mit " + Modul1.Datschuname;
                                                    continue;
                                                }
                                            }
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
                                            RtB[0].SelectedText = " mit " + Modul1.Kont[3] + " ";
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                            RtB[0].SelectedText = Modul1.Kont[0];
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                            Modul1.Ahnles(Modul1.PersInArb, out var asAhnData);
                                            if (Modul1.Kont[13].Trim() != "")
                                            {
                                                RtB[0].SelectedText = " (" + Modul1.Kont[13] + ")";
                                                List3.Items.Add("          " + Modul1.Family.Mann.AsString().Right(10) + "          " + Modul1.Family.Frau.AsString().Right(10));
                                            }
                                        }
                                    }
                                    List1.Items.Clear();
                                    if (b5 == 1)
                                    {
                                        List1.Items.Clear();
                                        Modul1.eLKennz = ELinkKennz.lkFather;
                                        var aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                                        flag = false;
                                        byte b6 = 1;
                                        if (aiFams.Count > 1)
                                        {
                                            flag = true;
                                        }
                                        foreach (var iFam in aiFams)
                                        {
                                            Modul1.FamInArb = iFam;
                                            DataModul.DB_EventTable.Index = "ArtNr";
                                            byte b7 = 0;
                                            int num3 = 502;
                                            do
                                            {
                                                if (num3 != 504)
                                                {
                                                    DataModul.DB_EventTable.Seek("=", num3.AsString(), Modul1.FamInArb.AsString(), "0");
                                                    if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                    {
                                                        List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + Modul1.FamInArb.AsString().Right(10));
                                                        b7 = 1;
                                                        break;
                                                    }
                                                }
                                                num3++;
                                            }
                                            while (num3 <= 506);
                                            if (b7 == 0)
                                            {
                                                DataModul.DB_EventTable.Seek("=", 601, Modul1.FamInArb.AsString(), "0");
                                                if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                                {
                                                    List1.Items.Add(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim() + "          " + Modul1.FamInArb.AsString().Right(10));
                                                    b7 = 1;
                                                    break;
                                                }
                                                if (b7 == 0)
                                                {
                                                    List1.Items.Add("        " + "          " + Modul1.FamInArb.AsString().Right(10));
                                                }
                                            }
                                            if (b6++ > 10) break;
                                        }
                                        int num4 = List1.Items.Count - 1;
                                        for (int num3 = 0; num3 <= num4; num3++)
                                        {
                                            Modul1.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num3].AsString(), 9, 10)));
                                            Modul1.Famles();
                                            int num5 = Modul1.PersInArb = Modul1.Family.Frau;
                                            Modul1.Schalt = 0;
                                            if (flag)
                                            {
                                                RtB[0].SelectedText = " (" + Strings.Chr(97 + num3).AsString() + ") ";
                                            }
                                            bool neb = true;
                                            Heidat(ref neb);
                                            RtB[0].SelectionIndent = 0;
                                            DataModul.DT_SperrTable.Index = "Nr";
                                            DataModul.DT_SperrTable.Seek("=", Modul1.PersInArb);
                                            if (!DataModul.DT_SperrTable.NoMatch)
                                            {
                                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                                RtB[0].SelectedText = " " + Modul1.Datschuname + " ";
                                                continue;
                                            }
                                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                            if (Modul1.Kont[3] == "")
                                            {
                                                Modul1.Kont[3] = "NN";
                                            }
                                            RtB[0].SelectedText = " mit " + Modul1.Kont[3] + " ";
                                            if (Modul1.Kont[1].Trim() != "")
                                            {
                                                Modul1.Kont[0] = Modul1.Kont[1].Trim() + " " + Modul1.Kont[0].Trim();
                                            }
                                            if (Modul1.Kont[2].Trim() != "")
                                            {
                                                Modul1.Kont[0] = Modul1.Kont[0].Trim() + " " + Modul1.Kont[2].Trim();
                                            }
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                            if (Modul1.DAus[89] == "1")
                                            {
                                                Modul1.Kont[0] = Modul1.Kont[0].ToUpper();
                                            }
                                            RtB[0].SelectedText = Modul1.Kont[0];
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                            if (Modul1.Kont[4].Trim() != "")
                                            {
                                                RtB[0].SelectedText = " (" + Modul1.Kont[4].Trim() + ") ";
                                            }
                                            Modul1.Ahnles(Modul1.PersInArb, out var asAhnData);
                                            if (Modul1.Kont[13].Trim() != "")
                                            {
                                                RtB[0].SelectedText = " (" + Modul1.Kont[13] + ")";
                                                List3.Items.Add("          " + Modul1.Family.Mann.AsString().Right(10) + "          " + Modul1.Family.Frau.AsString().Right(10));
                                            }
                                        }
                                    }
                                    List1.Items.Clear();
                                    RtB[0].SelectedText = "\n";
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    DataModul.DT_DescendentTable.Index = "Nr";
                                    DataModul.DT_DescendentTable.Seek("=", key);
                                    break;
                                }
                            }
                            RtB[0].SelectionIndent = 0;
                            RtB[0].SelectionHangingIndent = 0;
                            Label3.Font = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Label3.Text = "";
                            Abst = 50;
                            string text10 = "";
                            if (flag)
                            {
                                text10 = " (aus " + Conversions.ToString(Strings.Chr(96 +  DataModul.DT_DescendentTable.Fields["kia"].AsInt())) + ") ";
                            }
                            if (Modul1.DAus[113].AsDouble() == 0.0)
                            {
                                RtB[0].SelectedText = (DataModul.DT_DescendentTable.Fields["Gen"].AsString().Trim() +  "-" +  DataModul.DT_DescendentTable.Fields["Nr"].Value +  text10 +  '\n').AsString();
                                Modul1.Ind1 = (DataModul.DT_DescendentTable.Fields["Gen"].AsString() +  "-" +  DataModul.DT_DescendentTable.Fields["Nr"].AsString());
                                Label3.Text = (DataModul.DT_DescendentTable.Fields["Gen"].AsString() +  "-" +  DataModul.DT_DescendentTable.Fields["Nr"].AsString());
                            }
                            else
                            {
                                RtB[0].SelectedText = DataModul.DT_DescendentTable.Fields["Gen"].AsString().Trim() + "-" + DataModul.DT_DescendentTable.Fields["LFNr"].AsString().Trim() + text10 + " ";
                                Modul1.Ind1 = DataModul.DT_DescendentTable.Fields["Gen"].AsString() + "-" + DataModul.DT_DescendentTable.Fields["LFNr"].AsString().Trim();
                            }
                            Label3.Text = Modul1.Ind1 + " ";
                            Modul1.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                            if (Modul1.DAus[76] == "1")
                            {
                                RtB[0].SelectedText = "<" + Modul1.PersInArb.AsString().Trim() + ">";
                            }
                            Perstatic = Modul1.PersInArb;
                            Personlesen();
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")
                            {
                                Modul1.eLKennz = ELinkKennz.lkFather;
                            }
                            else
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                            }

                            RtB[0].SelectionHangingIndent = Label3.Width;
                            var aiData = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                            if (aiData.Count != 0)
                            {
                                List2.Items.Clear();
                                byte b8 = 1;
                                foreach (var iFam in aiData)
                                {
                                    Modul1.FamInArb = iFam;
                                    if (Modul1.FamInArb == 0)
                                    {
                                        break;
                                    }
                                    Modul1.Schalt = 1;
                                    Modul1.Famdatles2();
                                    if (Modul1.Kont[2].Trim() != "")
                                    {
                                        List2.Items.Add(Modul1.Kont[2] + ">" + "                          " + Modul1.FamInArb.AsString().Right(20));
                                        Modul1.Schalt = 0;
                                    }
                                    else if (Modul1.Kont[3].Trim() != "")
                                    {
                                        List2.Items.Add(Modul1.Kont[3] + ">" + "                          " + Modul1.FamInArb.AsString().Right(20));
                                        Modul1.Schalt = 0;
                                    }
                                    else if (Modul1.Kont[1].Trim() != "")
                                    {
                                        List2.Items.Add(Modul1.Kont[1] + ">" + "                          " + Modul1.FamInArb.AsString().Right(20));
                                        Modul1.Schalt = 0;
                                    }
                                    else if (Modul1.Kont[0].Trim() != "")
                                    {
                                        List2.Items.Add(Modul1.Kont[0] + ">" + "                          " + Modul1.FamInArb.AsString().Right(20));
                                        Modul1.Schalt = 0;
                                    }
                                    else if (Modul1.Kont[5].Trim() != "")
                                    {
                                        List2.Items.Add(Modul1.Kont[5] + ">" + "                          " + Modul1.FamInArb.AsString().Right(20));
                                        Modul1.Schalt = 0;
                                    }
                                    if (Modul1.Schalt == 1)
                                    {
                                        List2.Items.Add("        >" + "                          " + Modul1.FamInArb.AsString().Right(20));
                                    }
                                    b8 = (byte)unchecked((uint)(b8 + 1));
                                }
                                while (unchecked(b8) <= 30u) ;
                            }
                            ELinkKennz b9 = (!(DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "M")) ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                            short num6 = (short)(List2.Items.Count - 1);
                            for (short num2 = 0; num2 <= num6; num2 = (short)unchecked(num2 + 1))
                            {
                                Modul1.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List2.Items[num2].AsString(), 10, 20)));
                                Modul1.Famles();
                                Modul1.Schalt = 0;
                                RtB[0].SelectedText = "\n";
                                RtB[0].SelectionIndent = Abst;
                                bool neb = true;
                                Heidat(ref neb);
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                RtB[0].SelectedText = " mit ";
                                Modul1.eLKennz = b9;
                                if (Modul1.eLKennz == ELinkKennz.lkFather)
                                {
                                    b9 = Modul1.eLKennz;
                                    Modul1.PersInArb = Modul1.Family.Frau;
                                }
                                else
                                {
                                    Modul1.PersInArb = Modul1.Family.Mann;
                                }
                                if (Modul1.DAus[76] == "1")
                                {
                                    RtB[0].SelectedText = "<" + Modul1.PersInArb.AsString().Trim() + ">";
                                }
                                Personlesen();
                                Modul1.eLKennz = b9;
                            }
                            RtB[0].SelectionHangingIndent = 0;
                            List2.Items.Clear();
                            RtB[0].SelectedText = "\n";
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.3), FontStyle.Regular);
                            RtB[0].SelectedText = "\n";
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DT_DescendentTable.MoveNext();
                        }
                        if (_iBemZahl > 0)
                        {
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                            RtB[0].SelectedText = "\nQuellen:";
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            int bemZahl = _iBemZahl;
                            for (int i = 1; i <= bemZahl; i++)
                            {
                                if ((Modul1.DAus[71] == "1") & (i <= _iBemZahl))
                                {
                                    RtB[0].SelectedText = "\n";
                                }
                                b2 = (byte)Strings.InStr(_lstKontBem[i], ".)");
                                string text11 = _lstKontBem[i].Left(b2);
                                Modul1.UbgT1 = Strings.Mid(_lstKontBem[i], unchecked(b2) + 2, _lstKontBem[i].Length);
                                Modul1.UbgT1 = ((_Modul1)Modul1).Text_Retweg(Modul1.UbgT1);
                                Modul1.UbgT1 = Modul1.UbgT1.Left(Modul1.UbgT1.Length - 1) + Modul1.UbgT1.Right(1).Replace("\n", "");
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                RtB[0].SelectedText = " " + text11;
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                RtB[0].SelectedText = " " + Modul1.UbgT1;
                                Modul1.UbgT1 = "";
                            }
                            _iBemZahl = 0;
                            RtB[0].SelectedText = "\n";
                        }
                        RtB[0].Height = Height - 130;
                        RtB[0].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        RtB[0].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        break;
                    }
                case 2:
                    RtB[1].Visible = false;
                    RtB[2].Visible = false;
                    RtB[0].Visible = true;
                    break;
                case 3:
                    {
                        MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                        byte b = 0;
                        while (!RtB[b].Visible)
                        {
                            b = (byte)unchecked((uint)(b + 1));
                            if (unchecked(b) > 2u)
                            {
                                break;
                            }
                        }
                        MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = Modul1.GenFreeDir + "\\list\\";
                        MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                        MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                        if (MyProject.Forms.Hinter.CommonDialog1Save.FileName != "")
                        {
                            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                            {
                                case 1:
                                    RtB[b].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                                    break;
                                case 2:
                                    RtB[b].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                                    break;
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        byte b = 0;
                        while (!RtB[b].Visible)
                        {
                            b = (byte)unchecked((uint)(b + 1));
                            if (unchecked(b) > 2u)
                            {
                                break;
                            }
                        }
                        RtB[b].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        RtB[b].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        Interaction.Shell(Modul1.Aus[7] + " " + Modul1.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
                        break;
                    }
                case 5:
                    {
                        Frame2.Visible = false;
                        byte b = 0;
                        do
                        {
                            RtB[b].Visible = false;
                            b = (byte)unchecked((uint)(b + 1));
                        }
                        while (unchecked(b) <= 2u);
                        RtB[2].Visible = true;
                        RtB[2].Text = "";
                        Frame2.Visible = true;
                        RtB[2].Width = Width - 20;
                        RtB[2].RightMargin = Width - 60;
                        RtB[2].Height = Height - 130;
                        break;
                    }
                case 6:
                    Close();
                    DataModul.NB.Close();
                    MyProject.Forms.Druck.Show();
                    break;
                case 7:
                    {
                        byte b = 0;
                        do
                        {
                            RtB[b].Visible = false;
                            b = (byte)unchecked((uint)(b + 1));
                        }
                        while (unchecked(b) <= 2u);
                        RtB[1].Visible = true;
                        RtB[1].Text = "";
                        Frame3.Visible = true;
                        break;
                    }
                case 8:
                case 9:
                    Frame2.Visible = false;
                    RtB[2].SelectionAlignment = HorizontalAlignment.Center;
                    RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                    RtB[2].SelectedText = "Ortsindex";
                    RtB[2].SelectedText = "\n";
                    DataModul.DSB_OrtIdxTable.Index = "Ort";
                    DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                    while (!DataModul.DSB_OrtIdxTable.EOF)
                    {
                        if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                        {
                            RtB[2].SelectionAlignment = HorizontalAlignment.Left;
                            RtB[2].SelectedText = "\n";
                            RtB[2].SelectionIndent = 0;
                            Modul1.UbgT = Modul1.ortles(DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 0);
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = Modul1.UbgT;
                            Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            RtB[2].SelectedText = "\n";
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[2].SelectionIndent = 40;
                        }
                        if (index == 9)
                        {
                            RtB[2].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim();
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = "/ ";
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        DataModul.DSB_OrtIdxTable.MoveNext();
                    }
                    RtB[2].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RtB[2].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                    break;
                case 10:
                case 12:
                    RtB[2].Text = "";
                    Frame2.Visible = false;
                    RtB[2].SelectionAlignment = HorizontalAlignment.Center;
                    RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                    RtB[2].SelectedText = "Index Orte-Namen";
                    RtB[2].SelectedText = "\n";
                    RtB[2].SelectionHangingIndent = 0;
                    DataModul.DSB_OrtIdxTable.Index = "ortnam";
                    DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                    while (!DataModul.DSB_OrtIdxTable.EOF)
                    {
                        if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                        {
                            RtB[2].SelectedText = "\n";
                            RtB[2].SelectionAlignment = HorizontalAlignment.Left;
                            RtB[2].SelectionIndent = 0;
                            Modul1.AltName = "";
                            Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                            Modul1.Ind1 = "";
                            Modul1.UbgT = Modul1.ortles(OrtNr: DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt(), 0);
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = Modul1.UbgT;
                            RtB[2].SelectedText = "\n";
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[2].SelectionIndent = 40;
                        }
                        if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), Modul1.AltName.Trim(), TextCompare: false) != 0)
                        {
                            if (Modul1.AltName != "")
                            {
                                RtB[2].SelectedText = "\n";
                            }
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Name"].Value +  "  ").AsString();
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                        }
                        if (index == 10)
                        {
                            RtB[2].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim();
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = "/ ";
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        DataModul.DSB_OrtIdxTable.MoveNext();
                    }
                    RtB[2].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RtB[2].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                    break;
                case 11:
                case 13:
                    RtB[2].SelectedText = "";
                    Frame2.Visible = false;
                    RtB[2].SelectionAlignment = HorizontalAlignment.Center;
                    RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                    RtB[2].SelectedText = "Index Namen-Orte";
                    RtB[2].SelectedText = "\n";
                    DataModul.DSB_OrtIdxTable.Index = "NameOrt";
                    DataModul.DSB_OrtIdxTable.Seek(">=", " ");
                    while (!DataModul.DSB_OrtIdxTable.EOF)
                    {
                        if (Operators.CompareString(DataModul.DSB_OrtIdxTable.Fields["Name"].AsString().Trim(), Modul1.AltName.Trim(), TextCompare: false) != 0)
                        {
                            RtB[2].SelectionAlignment = HorizontalAlignment.Left;
                            RtB[2].SelectedText = "\n";
                            RtB[2].SelectionIndent = 0;
                            Modul1.AltNr = 0;
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                            Modul1.AltName = DataModul.DSB_OrtIdxTable.Fields["Name"].AsString();
                            RtB[2].SelectedText = "\n";
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[2].SelectionIndent = 40;
                        }
                        if (DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt() != Modul1.AltNr)
                        {
                            if (Modul1.AltNr > 0)
                            {
                                RtB[2].SelectedText = "\n";
                            }
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = (DataModul.DSB_OrtIdxTable.Fields["Ort"].Value +  "  ").AsString();
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.AltNr = DataModul.DSB_OrtIdxTable.Fields["OrtNr"].AsInt();
                        }
                        if (index == 11)
                        {
                            RtB[2].SelectedText = DataModul.DSB_OrtIdxTable.Fields["Ind"].AsString().Trim();
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                            RtB[2].SelectedText = "/ ";
                            RtB[2].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        DataModul.DSB_OrtIdxTable.MoveNext();
                    }
                    RtB[2].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RtB[2].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                    break;
                case 14:
                    {
                        string right3 = "";
                        string text5 = "";
                        Frame3.Visible = false;
                        RtB[1].Text = "";
                        DataModul.DSB_NamIdxTable.Index = "Kurzname";
                        M_Namen = "";
                        RtB[1].SelectionIndent = 50;
                        RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        RtB[1].SelectedText = "Namen-Index (Kurzform)\n\n";
                        RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        DataModul.DSB_NamIdxTable.Seek(">=", " ", 0);
                        while (!DataModul.DSB_NamIdxTable.EOF)
                        {
                            if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                            {
                                if (M_Namen != "")
                                {
                                    RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                    RtB[1].SelectedText = M_Namen;
                                    RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    RtB[1].SelectedText = Strings.Mid(text5, 2, text5.Length) + "\n\n";
                                    Modul1.AltNr = 0;
                                    text5 = "";
                                    right3 = 0.AsString();
                                }
                                M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                            }
                            if (DataModul.DSB_NamIdxTable.Fields["Ind"].AsString() != right3)
                            {
                                text5 = text5 + "/ " + DataModul.DSB_NamIdxTable.Fields["Ind"].AsString().Trim();
                                right3 = DataModul.DSB_NamIdxTable.Fields["Ind"].AsString();
                            }
                            DataModul.DSB_NamIdxTable.MoveNext();
                        }
                        RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        RtB[1].SelectedText = M_Namen;
                        RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[1].SelectedText = text5 + "\n\n";
                        RtB[1].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        RtB[1].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        break;
                    }
                case 15:
                    {
                        string right2 = "";
                        string text4 = "";
                        Frame3.Visible = false;
                        DataModul.DSB_NamIdxTable.Index = "Langi";
                        RtB[1].SelectionIndent = 50;
                        RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        RtB[1].SelectedText = "Namen-Index (Langform)\n\n";
                        RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        DataModul.DSB_NamIdxTable.Seek(">=", " ", " ", 0);
                        M_Namen = "";
                        while (!DataModul.DSB_NamIdxTable.EOF)
                        {
                            RtB[1].SelectionIndent = 50;
                            if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                            {
                                if (text != "")
                                {
                                    RtB[1].SelectionIndent = 70;
                                    RtB[1].SelectedText = text + "; " + Strings.Mid(text4, 2, text4.Length) + ".\n";
                                    text = "";
                                }
                                RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                                RtB[1].SelectionIndent = 50;
                                RtB[1].SelectedText = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString() + '\n';
                                RtB[1].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                text4 = "";
                                right2 = "";
                                M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                            }
                            if (text == "")
                            {
                                text = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                            }
                            if (text != DataModul.DSB_NamIdxTable.Fields["Name"].AsString())
                            {
                                RtB[1].SelectionIndent = 70;
                                RtB[1].SelectedText = text + ": " + Strings.Mid(text4, 2, text4.Length) + ".\n";
                                text4 = "";
                                right2 = "";
                                text = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                            }
                            if (DataModul.DSB_NamIdxTable.Fields["Ind"].AsString() != right2)
                            {
                                text4 = text4 + "/ " + DataModul.DSB_NamIdxTable.Fields["Ind"].AsString().Trim();
                                right2 = DataModul.DSB_NamIdxTable.Fields["Ind"].AsString();
                            }
                            DataModul.DSB_NamIdxTable.MoveNext();
                        }
                        if (text != "")
                        {
                            RtB[1].SelectionIndent = 70;
                            RtB[1].SelectedText = text + "; " + Strings.Mid(text2, 2, text2.Length) + ".\n";
                            Modul1.AltNr = 0;
                        }
                        RtB[1].SaveFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        RtB[1].LoadFile(Modul1.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        break;
                    }
                case 16:
                case 17:
                    break;
                default:
                    Interaction.MsgBox(index);
                    break;
            }
        }
    }

    private void Sippenlist_Load(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
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
                Hinter hinter; // Hinter form from Gen_FreeWin.Views - not available in Druck
                short Listart;
                EEventArt Art;
                bool neb;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        Modul1.DAus[101] = Modul1.Font1;
                        goto IL_0010;
                    case 2072:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 3:
                                    break;
                                case 2:
                                    goto IL_05c9;
                                case 1:
                                    goto IL_066a;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0565;
                        }
                    end_IL_0000:
                        break;
                    IL_0010:
                        num = 2;
                        Modul1.DAus[102] = "10";
                        Modul1.Dateienopen();
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        BackColor = Modul1.HintFarb;
                        FileSystem.MkDir(Modul1.Verz + "Nachkommen");
                        hinter = MyProject.Forms.Hinter;
                        hinter.Att(Modul1.Verz + "Nachkommen");
                        FileSystem.Kill(Modul1.Verz + "Nachkommen\\*.*");
                        Modul1.Verz1 = Modul1.Verz.Left(15);
                        source = Modul1.GenFreeDir + "INIT\\GedAUS.mdb";
                        destination = Modul1.Verz + "Nachkommen\\GEDAUS.mdb";
                        FileSystem.FileCopy(source, destination);
                        name = Modul1.Verz + "Nachkommen\\GEDAUS.mdb";
                        DataModul.NB = UpgradeSupport.DAODBEngine_definst.OpenDatabase(name, false, false, "");
                        DataModul.NB_PersonTable = DataModul.NB.OpenRecordset(dbTables.Personen1, RecordsetTypeEnum.dbOpenTable);
                        DataModul.NB_FamilyTable = DataModul.NB.OpenRecordset(dbTables.Familie1, RecordsetTypeEnum.dbOpenTable);
                        ProjectData.ClearProjectError();
                        num3 = 3;
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
                        goto IL_02fd;
                    IL_02fd: // <========== 12
                        num = 58;
                        Font = new Font("Arial", Modul1.Fs, FontStyle.Regular);
                        DataModul.DT_DescendentTable.Index = "NR";
                        DataModul.DT_DescendentTable.MoveFirst();
                        Modul1.PersInArb = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        if (Modul1.Kont[1].Trim() != "")
                        {
                            Modul1.Kont[0] = Modul1.Kont[1].Trim() + " " + Modul1.Kont[0].Trim();
                        }
                        if (Modul1.Kont[2].Trim() != "")
                        {
                            Modul1.Kont[0] = Modul1.Kont[0].Trim() + " " + Modul1.Kont[2].Trim();
                        }
                        Label1[0].Text = Modul1.Kont[3] + " " + Modul1.Kont[0];
                        Listart = 0;
                        Art = default;
                        neb = false;
                        Modul1.Datles3(Listart, 0, Art, ref neb);
                        if (Modul1.Kont[11] != "")
                        {
                            Modul1.Kont[11] = "Geb.: " + Modul1.Kont[11];
                        }
                        if (Modul1.Kont[11] == "")
                        {
                            Modul1.Kont[11] = "Get.: " + Modul1.Kont[12];
                        }
                        if (Modul1.Kont[13] != "")
                        {
                            Modul1.Kont[13] = "Gest.: " + Modul1.Kont[13];
                        }
                        if (Modul1.Kont[13] == "")
                        {
                            Modul1.Kont[13] = "Begr.: " + Modul1.Kont[14];
                        }
                        Label1[1].Text = Modul1.Kont[11];
                        Label1[2].Text = Modul1.Kont[13];
                        goto IL_0565;
                    IL_0565: // <========== 3
                        num = 85;
                        if (Information.Err().Number != 3021)
                        {
                        }
                        else
                        {
                            Label1[3].Text = "Keine Berechnung vorhanden";
                            Label1[0].Text = "Sie müssen erst die Nachfahren berechnen.";
                            Command_Renamed[1].Enabled = false;
                        }
                        goto end_IL_0000_2;
                    IL_05c9:
                        num = 91;
                        if (Information.Err().Number == 75)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_066a;
                        }
                        if (Information.Err().Number == 53)
                        {
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_066a;
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
                        goto IL_066e;
                    IL_066a: // <========== 3
                        num4 = num2 + 1;
                        goto IL_066e;
                    IL_066e:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 26:
                            case 30:
                            case 33:
                            case 36:
                            case 39:
                            case 42:
                            case 45:
                            case 48:
                            case 51:
                            case 54:
                            case 57:
                            case 58:
                                goto IL_02fd;
                            case 85:
                                goto IL_0565;
                            case 89:
                            case 90:
                            case 103:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 2072;
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
    public void Personlesen()
    {
        bool neb = false;
        EPerles(ref neb);
    }

    public void Namenindex()
    {
        if (Modul1.Kont[0] != "" && Modul1.Kont[0] != "NN" && Modul1.Kont[0] != "N.N.")
        {
            DataModul.DSB_NamIdxTable.AddNew();
            if (Modul1.Kont[3] == "")
            {
                DataModul.DSB_NamIdxTable.Fields["Name"].Value = "?";
            }
            else
            {
                DataModul.DSB_NamIdxTable.Fields["Name"].Value = Modul1.Kont[99];
            }
            DataModul.DSB_NamIdxTable.Fields["Name1"].Value = Modul1.Kont[0];
            DataModul.DSB_NamIdxTable.Fields["Ind"].Value = Modul1.Ind1;
            M_Namen = Modul1.Kont[0];
            DataModul.DSB_NamIdxTable.Update();
        }
    }
    public void EPerles(ref bool neb)
    {
        neb = false;
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int famInArb = default;
        int famInArb2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int FamPer;
                string LD;
                short Listart;
                EEventArt _eArt;
                bool neb2;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 3976:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0d06;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Information.Err().Number == 3420)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0d06;
                            }
                            if (Information.Err().Number == 5)
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
                            goto IL_0d0a;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        DataModul.DT_SperrTable.Index = "Nr";
                        DataModul.DT_SperrTable.Seek("=", Modul1.PersInArb);
                        if (!DataModul.DT_SperrTable.NoMatch)
                        {
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[0].SelectedText = Modul1.Datschuname;
                            goto end_IL_0000_2;
                        }
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        Namenindex();
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        RtB[0].SelectedText = (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].TrimEnd()).Trim() + " ";
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Bold);
                        RtB[0].SelectedText = Modul1.Kont[0].Trim();
                        leerweg();
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        if ((Modul1.DAus[115] == "1") | (Modul1.DAus[116] == "1"))
                        {
                            Module2.Bildaus("P", "generatio");
                        }
                        Modul1.PerSatzLes(Modul1.PersInArb);
                        if (Modul1.Kont[4] != "")
                        {
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Italic);
                            RtB[0].SelectedText = " (" + Modul1.Kont[4].TrimEnd() + ")";
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[0].SelectedText = " ";
                        }
                        leerweg();
                        if (Modul1.DAus[62] == "1")
                        {
                            FamPer = 1;
                            ((_Modul1)Modul1).PerQu(ref FamPer);
                        }
                        if (Modul1.Kont[30].Trim() != "")
                        {
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[0].SelectionCharOffset = _byHoch;
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            RtB[0].SelectedText = Modul1.Kont[30];
                            RtB[0].SelectionCharOffset = 0;
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        leerweg();
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields["religi"].Value))
                        {
                            if (Operators.CompareString(DataModul.DB_PersonTable.Fields["religi"].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                Modul1.Ubg = DataModul.DB_PersonTable.Fields["religi"].AsInt();
                                LD = "";
                                Modul1.UbgT = DataModul.TextLese1(Modul1.Ubg);
                                if (Modul1.UbgT.Trim() != "")
                                {
                                    RtB[0].SelectedText = " " + Modul1.UbgT;
                                }
                            }
                        }
                        goto IL_0564;
                    IL_0564: // <========== 3
                        num = 52;
                        if ((Modul1.DAus[99].AsDouble() == 1.0) & (Modul1.Kont[6].Trim() != ""))
                        {
                            RtB[0].SelectedText = " " + Modul1.Kont[6].TrimEnd() + " ";
                        }
                        Modul1.Datschalt = 1;
                        Listart = 1;
                        LD = 0.AsString();
                        _eArt = 0;
                        neb2 = false;
                        Modul1.Datles3(Listart, 0, _eArt, ref neb2);
                        Modul1.Datschalt = 0;
                        if (Modul1.DAus[66] == "1")
                        {
                            if (Modul1.Kont[25] != "")
                            {
                                RtB[0].SelectedText = " " + Modul1.Kont[25].Trim();
                            }
                        }
                        goto IL_0665;
                    IL_0665: // <========== 3
                        num = 63;
                        if (Modul1.DAus[106].AsDouble() != 1.0)
                        {
                        }
                        Modul1.Aschalt = Modul1.Datschalt;
                        PerSp1 = Modul1.PersInArb;
                        if (!LSchalt)
                        {
                            if (Modul1.DAus[36] == "1")
                            {
                                Paten2();
                            }
                        }
                        goto IL_06cf;
                    IL_06cf: // <========== 3
                        num = 72;
                        Modul1.Datschalt = checked((byte)Math.Round(Modul1.Aschalt));
                        M1_Ki = false;
                        _eArt = default;
                        LD = 0.AsString();
                        Listart = 1;
                        neb2 = true;
                        Modul1.Datles3(Listart, 0, _eArt, ref neb2);
                        neb2 = true;
                        Datschreib(ref PatText, 0, ref neb2);
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        if (!LSchalt)
                        {
                            if ((Modul1.DAus[38] == "1") | (Modul1.DAus[39] == "1"))
                            {
                                Sonst();
                            }
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if ((Modul1.DAus[0] == "1") | (Modul1.DAus[13] == "1"))
                            {
                                _eArt = EEventArt.eA_300;
                                neb2 = false;
                                Berufe(_eArt, ref neb2);
                            }
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if ((Modul1.DAus[16] == "1") | (Modul1.DAus[17] == "1"))
                            {
                                Modul1.Ubg = 301;
                                _eArt = EEventArt.eA_301;
                                neb2 = false;
                                Berufe(_eArt, ref neb2);
                            }
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            if ((Modul1.DAus[20] == "1") | (Modul1.DAus[21] == "1"))
                            {
                                Modul1.Ubg = 302;
                                _eArt = EEventArt.eA_302;
                                neb2 = false;
                                Berufe(_eArt, ref neb2);
                            }
                            famInArb = Modul1.FamInArb;
                            PerSp1 = Modul1.PersInArb;
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        PerSp1 = Modul1.PersInArb;
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        if (!LSchalt)
                        {
                            if (Modul1.DAus[37] == "1")
                            {
                                Pate_bei2(Modul1.PersInArb);
                            }
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            Modul1.PersInArb = PerSp1;
                            PerSp1 = Modul1.PersInArb;
                            famInArb2 = Modul1.FamInArb;
                            _xTrau = false;
                            _xPat = false;
                            _xTrau = false;
                            Modul1.FamInArb = famInArb2;
                            Modul1.PersInArb = PerSp1;
                        }
                        goto IL_0a77;
                    IL_0a77:
                        num = 115;
                        if (Modul1.DAus[88] == "1")
                        {
                            Bild("P", Modul1.PersInArb);
                        }
                        goto IL_0aa5;
                    IL_0aa5:
                        num = 118;
                        if (Modul1.DAus[1] == "1")
                        {
                            if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                            {
                                Modul1.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                                if ((Modul1.DAus[106].AsDouble() == 1.0) & !neb)
                                {
                                    RtB[0].SelectedText = "\n";
                                
                                }
                                else
                                {
                                    RtB[0].SelectedText = " ";
                                }
                                goto IL_0b7e;
                            }
                        }
                        goto IL_0b89;
                    IL_0b7e: // <========== 3
                        num = 127;
                        Bemaus(ref neb);
                        goto IL_0b89;
                    IL_0b89: // <========== 3
                        num = 130;
                        if (Modul1.DAus[79] == "1")
                        {
                            if (!LSchalt & (Modul1.PersInArb != Perstatic))
                            {
                                Weitehen();
                            }
                        }
                        goto IL_0bd6;
                    IL_0bd6: // <========== 3
                        num = 135;
                        if (Perstatic == Modul1.PersInArb)
                        {
                            goto end_IL_0000_2;
                        }
                        DataModul.Link.GetPersonFam(Modul1.PersInArb, ELinkKennz.lkChild, out var _iUbg);
                        Modul1.Ubg = _iUbg;
                        if (Modul1.Ubg > 0)
                        {
                            Eltles();
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        }
                        Modul1.FamInArb = famInArb;
                        Modul1.PersInArb = PerSp1;
                        goto end_IL_0000_2;
                    IL_0d06:
                        num4 = num2 + 1;
                        goto IL_0d0a;
                    IL_0d0a:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 49:
                            case 50:
                            case 51:
                            case 52:
                                goto IL_0564;
                            case 61:
                            case 62:
                            case 63:
                                goto IL_0665;
                            case 70:
                            case 71:
                            case 72:
                                goto IL_06cf;
                            case 114:
                            case 115:
                                goto IL_0a77;
                            case 117:
                            case 118:
                                goto IL_0aa5;
                            case 123:
                            case 126:
                            case 127:
                                goto IL_0b7e;
                            case 128:
                            case 129:
                            case 130:
                                goto IL_0b89;
                            case 133:
                            case 134:
                            case 135:
                                goto IL_0bd6;
                            case 7:
                            case 143:
                            case 144:
                            case 150:
                            case 156:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 3976;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void leerweg()
    {
        while (RtB[0].SelectionStart > 0 && Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == " ")
        {
            RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
            RtB[0].SelectionLength = 1;
            RtB[0].SelectedText = "";
        }
        RtB[0].SelectionCharOffset = 0;
    }


    public void Berufe(EEventArt Beruf, ref bool neb, double M_Sgg = 1f)
    {
        neb = false;
        int try0000_dispatch = -1;
        int num = default;
        string left = default;
        int num2 = default;
        int num3 = default;
        string Job = default;
        int lErl = default;
        short num5 = default;
        short num6 = default;
        string text2 = default;
        string value = default;
        int persInArb = default;
        string text3 = default;
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
                    int ortNr;
                    int ortNr2;
                    byte Schalt;
                    int Nr;
                    short LfNR;
                    bool neb2;
                    short Listart;
                    int AAA;
                    string LD;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            left = "";
                            goto IL_0009;
                        case 8048:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_1afe;
                                    default:
                                        goto end_IL_0000;
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
                                goto IL_1b02;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            List3.Items.Clear();
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            DataModul.DB_EventTable.Index = "Besu";
                            DataModul.DB_EventTable.Seek("=", Beruf.AsString(), Modul1.PersInArb.AsString());
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                DataModul.DB_EventTable.Index = "ArtNr";
                                goto end_IL_0000_2;
                            }
                            goto IL_04e7;
                        IL_0336: // <========== 3
                            num = 35;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                    //bBB6 = ref COND.Kont[0];
                                    LD = "";
                                    DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont[10] = " " + Modul1.Kont[0].Trim() + ": ";
                                    }
                                }
                            }
                            goto IL_0420;
                        IL_0420: // <========== 3
                            num = 43;
                            Job = Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString();
                            if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                            {
                                Job = "+" + Job;
                            }
                            goto IL_04ba;
                        IL_04ba:
                            num = 47;
                            List3.Items.Add(Job);
                            goto IL_04d1;
                        IL_04d1: // <========== 3
                            num = 48;
                            lErl = 12;
                            DataModul.DB_EventTable.MoveNext();
                            goto IL_04e7;
                        IL_04e7: // <========== 3
                            num = 11;
                            if (!DataModul.DB_EventTable.EOF)
                            {
                                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Modul1_J = 0;
                                    while (unchecked(Modul1_J) <= 15u)
                                    {
                                        Modul1.Kont1[Modul1_J] = "";
                                        Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                                    }
                                    Modul1.sDatu = "";
                                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                                 | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb)
                                 | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)))
                                    {
                                        DataModul.DB_EventTable.Index = "ArtNr";
                                        goto IL_04fa;
                                    }
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                        Modul1.Kont1[1] = Modul1.sDatu;
                                    }
                                    Modul1.UbgT = "";
                                    if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                        //bBB5 = ref COND.Kont[0];
                                        LD = "";
                                        DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + " ";
                                        }
                                    }
                                    goto IL_0336;
                                }
                                goto IL_04d1;
                            }
                            goto IL_04fa;
                        IL_04fa: // <========== 3
                            num = 51;
                            lErl = 13;
                            leerweg();
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            if (List3.Items.Count == 0)
                            {
                                goto end_IL_0000_2;
                            }
                            if ((Modul1.DAus[106].AsDouble() == 1.0) & !neb)
                            {
                                RtB[0].SelectedText = "\n";
                            
                            }
                            else
                            {
                                RtB[0].SelectedText = " ";
                            }
                            goto IL_05be;
                        IL_05be: // <========== 3
                            num = 63;
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Underline);
                            switch (Beruf)
                            {
                                case EEventArt.eA_300:
                                    goto IL_0618;
                                case EEventArt.eA_301:
                                    goto IL_067c;
                                case EEventArt.eA_302:
                                    goto IL_06a0;
                                default:
                                    break;
                            }
                            goto IL_06c1;
                        IL_0618:
                            num = 68;
                            if (List3.Items.Count == 1)
                            {
                                RtB[0].SelectedText = "Beruf:";
                            }
                            goto IL_0649;
                        IL_0649:
                            num = 71;
                            if (List3.Items.Count > 1)
                            {
                                RtB[0].SelectedText = "Berufe:";
                            }
                            goto IL_06c1;
                        IL_067c:
                            num = 76;
                            RtB[0].SelectedText = Modul1.IText[70].Trim();
                            goto IL_06c1;
                        IL_06a0:
                            num = 79;
                            RtB[0].SelectedText = Modul1.IText[8].Trim();
                            goto IL_06c1;
                        IL_06c1: // <========== 5
                            num = 81;
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            RtB[0].SelectedText = " ";
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * M_Sgg), FontStyle.Regular);
                            num5 = (short)(List3.Items.Count - 1);
                            num6 = 0;
                            goto IL_19f7;
                        IL_0a2e: // <========== 3
                            num = 111;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
                            M_Namen = Modul1.Kont[0];
                            if (Modul1.Kont[99] == "")
                            {
                                text = "?";
                            
                            }
                            else
                            {
                                text = Modul1.Kont[99].Trim();
                            }
                            goto IL_0a8e;
                        IL_0a8e: // <========== 3
                            num = 120;
                            Modul1.Kont1[1] = "";
                            Modul1.Kont1[3] = "";
                            if (left == "1")
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString().Trim(), 8);
                                    text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                    Modul1.Kont1[1] = Modul1.sDatu;
                                }
                                goto IL_0b74;
                            }
                            goto IL_0f40;
                        IL_0b74:
                            num = 129;
                            if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                            {
                                if (Modul1.Kont1[1] != "")
                                {
                                    Modul1.Kont1[1] = "von " + Modul1.Kont1[1];
                                }
                                Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                if ((Modul1.sDatu != "") & (text2.Trim() == ""))
                                {
                                    Modul1.sDatu = " bis " + Modul1.sDatu.Trim();
                                }
                                goto IL_0cb7;
                            }
                            goto IL_0cca;
                        IL_0cb7:
                            num = 140;
                            Modul1.Kont1[3] = Modul1.sDatu;
                            goto IL_0cca;
                        IL_0cca: // <========== 3
                            num = 142;
                            if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                {
                                    value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                    LD = "";
                                    Modul1.UbgT = DataModul.TextLese1(value.AsInt());
                                    if (Modul1.UbgT.Trim() != "")
                                    {
                                        Modul1.Kont1[3] = Modul1.Kont1[3] + " (" + Modul1.UbgT.Trim() + ")";
                                        Modul1.UbgT = "";
                                    }
                                }
                            }
                            goto IL_0dda;
                        IL_0dda: // <========== 3
                            num = 152;
                            if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                            {
                                ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                Modul1.UbgT = Modul1.ortles1(ortNr, 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                Modul1.UbgT = "";
                            }
                            goto IL_0e7c;
                        IL_0e7c:
                            num = 157;
                            if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[6] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_0f40;
                        IL_0f40: // <========== 4
                            num = 164;
                            Modul1.UbgT = "";
                            if (Beruf == EEventArt.eA_302)
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                {
                                    ortNr2 = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                    Modul1.UbgT = Modul1.ortles1(ortNr2, 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                    Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                    Modul1.UbgT = "";
                                }
                                goto IL_1006;
                            }
                            if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                            {
                                AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                LD = "";
                                DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                if (Modul1.Kont[0] != "")
                                {
                                    Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                }
                            }
                            goto IL_11b3;
                        IL_1006:
                            num = 171;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                    }
                                }
                            }
                            goto IL_11b3;
                        IL_11b3: // <========== 5
                            num = 188;
                            if (Beruf == EEventArt.eA_302)
                            {
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.Hausnr].Value))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.Hausnr].AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                        Modul1.Kont1[7] = Modul1.Kont1[7] + " " + Modul1.Kont[0].Trim() + " ";
                                        Modul1.Kont[0] = "";
                                    }
                                }
                            }
                            goto IL_12bf;
                        IL_12bf: // <========== 3
                            num = 197;
                            left = "0";
                            Job = (Modul1.Kont1[1] + Modul1.Kont1[3] + Modul1.Kont1[7] + Modul1.Kont1[6] + Modul1.Kont1[5]).Trim();
                            Job = Module2.Jobdreh(Job);
                            if (Job.Trim() != "")
                            {
                                if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == ";")
                                {
                                    RtB[0].SelectedText = " ";
                                }
                                goto IL_13a4;
                            }
                            goto IL_13c3;
                        IL_13a4:
                            num = 204;
                            RtB[0].SelectedText = Job.TrimEnd();
                            goto IL_13c3;
                        IL_13c3: // <========== 3
                            num = 206;
                            Nr = Modul1.PersInArb;
                            LfNR = Modul1.LfNR;
                            ((_Modul1)Modul1).QuellenDatum(ref Nr, Beruf, ref LfNR);
                            Modul1.LfNR = Conversions.ToByte(LfNR);
                            Modul1.PersInArb = Nr.AsInt();
                            leerweg();
                            if (Modul1.Kont1[9].Trim() != "")
                            {
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                RtB[0].SelectionCharOffset = _byHoch;
                                RtB[0].SelectedText = " " + Modul1.Kont1[9];
                                RtB[0].SelectionCharOffset = 0;
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            left = "0";
                            if (unchecked(0 - (M1_Ki ? 1 : 0)) == 1)
                            {
                                if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[26] == "1")) | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[30] == "1")) | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[34] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_1620;
                            }
                            if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[14] == "1")) | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[20] == "1")) | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[22] == "1")))
                            {
                                left = "1";
                            }
                            goto IL_1620;
                        IL_1620: // <========== 3
                            num = 226;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString() != " ")
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                    neb2 = false;
                                    Bemaus(ref neb2);
                                }
                            }
                            goto IL_16a4;
                        IL_16a4: // <========== 3
                            num = 232;
                            left = "0";
                            if (unchecked(0 - (M1_Ki ? 1 : 0)) == 1)
                            {
                                if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[27] == "1")) | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[31] == "1")) | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[35] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_17bd;
                            }
                            if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[15] == "1")) | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[19] == "1")) | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[23] == "1")))
                            {
                                left = "1";
                            }
                            goto IL_17bd;
                        IL_17bd: // <========== 3
                            num = 243;
                            if (left == "1")
                            {
                                if (DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString() != " ")
                                {
                                    Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                    neb2 = false;
                                    Bemaus(ref neb2);
                                }
                            }
                            goto IL_1841;
                        IL_1841: // <========== 3
                            num = 249;
                            persInArb = Modul1.PersInArb;
                            Listart = 1;
                            LD = "";
                            Modul1.Zeugsu(Beruf, Modul1.LfNR, Listart, 0L);
                            text3 = Modul1.Kont1[20];
                            Modul1.Kont1[20] = "";
                            Modul1.PersInArb = persInArb;
                            if (Modul1.DAus[96].AsDouble() == 1.0)
                            {
                                if (text3 != "")
                                {
                                    leerweg();
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    }
                                    RtB[0].SelectedText = " Zeugen: " + text3.Trim();
                                    text3 = "";
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                            }
                            goto IL_19b0;
                        IL_19b0: // <========== 3
                            num = 265;
                            left = "0";
                            RtB[0].SelectedText = ";";
                            DataModul.DB_EventTable.MoveNext();
                            num6 = (short)unchecked(num6 + 1);
                            goto IL_19f7;
                        IL_19f7:
                            if (num6 <= num5)
                            {
                                Modul1.LfNR = (byte)Math.Round(Conversion.Val(Strings.Mid(List3.Items[num6].AsString(), 240, 10)));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", Beruf.AsString(), Modul1.PersInArb.AsString(), Modul1.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("7");
                                    Debugger.Break();
                                }
                                Modul1_J = 0;
                                while (unchecked(Modul1_J) <= 15u)
                                {
                                    Modul1.Kont1[Modul1_J] = "";
                                    Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                                }
                                Modul1.Ubg = num6;
                                Modul1.sDatu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                                 | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb)
                                 | (DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != Beruf)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    goto end_IL_0000_2;
                                }
                                if (unchecked(0 - (M1_Ki ? 1 : 0)) == 1)
                                {
                                    if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[25] == "1")) | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[29] == "1")) | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[33] == "1")))
                                    {
                                        left = "1";
                                    }
                                    goto IL_0a2e;
                                }
                                if (((Beruf == EEventArt.eA_300) & (Modul1.DAus[13] == "1")) | ((Beruf == EEventArt.eA_301) & (Modul1.DAus[17] == "1")) | ((Beruf == EEventArt.eA_302) & (Modul1.DAus[21] == "1")))
                                {
                                    left = "1";
                                }
                                goto IL_0a2e;
                            }
                            if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == ";")
                            {
                                RtB[0].SelectionStart = RtB[0].SelectionStart - 1;
                                RtB[0].SelectionLength = 1;
                                RtB[0].SelectedText = ".";
                            }
                            goto IL_1a9d;
                        IL_1a9d:
                            num = 274;
                            left = "0";
                            goto end_IL_0000_2;
                        IL_1afe:
                            num4 = unchecked(num2 + 1);
                            goto IL_1b02;
                        IL_1b02:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 33:
                                case 34:
                                case 35:
                                    goto IL_0336;
                                case 40:
                                case 41:
                                case 42:
                                case 43:
                                    goto IL_0420;
                                case 46:
                                case 47:
                                    goto IL_04ba;
                                case 13:
                                case 48:
                                    goto IL_04d1;
                                case 9:
                                case 10:
                                case 11:
                                case 50:
                                    goto IL_04e7;
                                case 21:
                                case 51:
                                    goto IL_04fa;
                                case 59:
                                case 62:
                                case 63:
                                    goto IL_05be;
                                case 70:
                                case 71:
                                    goto IL_0649;
                                case 65:
                                case 73:
                                case 74:
                                case 77:
                                case 80:
                                case 81:
                                    goto IL_06c1;
                                case 104:
                                case 105:
                                case 109:
                                case 110:
                                case 111:
                                    goto IL_0a2e;
                                case 116:
                                case 119:
                                case 120:
                                    goto IL_0a8e;
                                case 128:
                                case 129:
                                    goto IL_0b74;
                                case 139:
                                case 140:
                                    goto IL_0cb7;
                                case 141:
                                case 142:
                                    goto IL_0cca;
                                case 149:
                                case 150:
                                case 151:
                                case 152:
                                    goto IL_0dda;
                                case 156:
                                case 157:
                                    goto IL_0e7c;
                                case 161:
                                case 162:
                                case 163:
                                case 164:
                                    goto IL_0f40;
                                case 170:
                                case 171:
                                    goto IL_1006;
                                case 176:
                                case 177:
                                case 178:
                                case 179:
                                case 185:
                                case 186:
                                case 187:
                                case 188:
                                    goto IL_11b3;
                                case 194:
                                case 195:
                                case 196:
                                case 197:
                                    goto IL_12bf;
                                case 203:
                                case 204:
                                    goto IL_13a4;
                                case 205:
                                case 206:
                                    goto IL_13c3;
                                case 219:
                                case 220:
                                case 224:
                                case 225:
                                case 226:
                                    goto IL_1620;
                                case 230:
                                case 231:
                                case 232:
                                    goto IL_16a4;
                                case 236:
                                case 237:
                                case 241:
                                case 242:
                                case 243:
                                    goto IL_17bd;
                                case 247:
                                case 248:
                                case 249:
                                    goto IL_1841;
                                case 263:
                                case 264:
                                case 265:
                                    goto IL_19b0;
                                case 273:
                                case 274:
                                    goto IL_1a9d;
                                case 8:
                                case 55:
                                case 99:
                                case 275:
                                case 280:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 8048;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 5
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Bemaus(ref bool neb)
    {
        neb = false;

        while (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == "\n")
        {
            RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
            RtB[0].SelectionLength = 1;
            RtB[0].SelectedText = "";
        }
        leerweg();
        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        if (Modul1.DAus[72] == "1")
        {
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
        }
        RtB[0].SelectedText = " {";
        if (Modul1.DAus[70] == "0")
        {
            Modul1.UbgT1 = ((_Modul1)Modul1).Text_Retweg(Modul1.UbgT1);
        }
        RtB[0].SelectedText = Modul1.UbgT1;
        RtB[0].SelectedText = "}";
        if (neb)
        {
            RtB[0].SelectedText = "\n";
        }
        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
    }

    public void Eltles()
    {
        int try0000_dispatch = -1;
        int num = default;
        string Pattext = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int famInArb = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                object obj;
                string Ahne;
                short Listart;
                EEventArt Art;
                bool neb;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        Pattext = "";
                        goto IL_0009;
                    case 3439:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0b55;
                                default:
                                    goto end_IL_0000;
                            }
                            if (Information.Err().Number == 5)
                            {
                                goto end_IL_0000_2;
                            }
                            if (Information.Err().Number == 3022)
                            {
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_0b55;
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
                            goto IL_0b59;
                        }
                    end_IL_0000:
                        break;
                    IL_0009:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                        Modul1.PerSatzLes(Modul1.PersInArb);
                        Modul1.eLKennz = ELinkKennz.lkMother;
                        if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                        {
                            Modul1.eLKennz = ELinkKennz.lkFather;
                        }
                        if (Modul1.eLKennz.AsDouble() == 2.0)
                        {
                            RtB[0].SelectedText = " (Sohn von ";
                        }
                        if (Modul1.eLKennz.AsDouble() == 1.0)
                        {
                            RtB[0].SelectedText = " (Tochter von ";
                        }
                        Modul1.FamInArb = Modul1.Ubg;
                        DataModul.NB_FamilyTable.AddNew();
                        DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = Modul1.FamInArb;
                        DataModul.NB_FamilyTable.Update();
                        if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkFather, out var _iPersInArb1))
                        {
                            Modul1.PersInArb = _iPersInArb1;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Namenindex();
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
                            Modul1.Kont[0] = Modul1.Kont[0].TrimEnd();
                            RtB[0].SelectedText = Modul1.Kont[3] + " ";
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Bold);
                            RtB[0].SelectedText = Modul1.Kont[0];
                            leerweg();
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            RtB[0].SelectedText = " ";
                            if (Modul1.Kont[4] != "")
                            {
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Italic);
                                RtB[0].SelectedText = "(" + Modul1.Kont[4].TrimEnd() + ")";
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                RtB[0].SelectedText = " ";
                            }
                            leerweg();
                            if (Modul1.DAus[78] == "1")
                            {
                                Listart = 1;
                                Ahne = 0.AsString();
                                Art = default;
                                neb = true;
                                Modul1.Datles3(Listart, default, Art, ref neb);
                                neb = true;
                                Datschreib(ref Pattext, 1, ref neb);
                                leerweg();
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble()), FontStyle.Regular);
                                Art = EEventArt.eA_300;
                                neb = true;
                                Berufe(Art, ref neb, 0f);
                                Art = EEventArt.eA_301;
                                neb = true;
                                Berufe(Art, ref neb, 0f);
                            }
                            Modul1.Aschalt = Modul1.Datschalt;
                            Modul1.Datschalt = checked((byte)Math.Round(Modul1.Aschalt));
                            obj = 0;
                            DGSchalt = false;
                        
                        }
                        else
                        {
                            RtB[0].SelectedText = "unbekanntem Vater ";
                        }
                        goto IL_0539;
                    IL_0539: // <========== 3
                        num = 60;
                        lErl = 33;
                        DataModul.NB_FamilyTable.AddNew();
                        DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = Modul1.FamInArb;
                        DataModul.NB_FamilyTable.Update();
                        if (DataModul.Link.GetFamPerson(Modul1.FamInArb, ELinkKennz.lkMother, out var _iPersInArb2))
                        {
                            Modul1.PersInArb = _iPersInArb2;
                            Modul1.Family.Frau = Modul1.PersInArb;
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Namenindex();
                            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                            Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
                            Modul1.Kont[0] = Modul1.Kont[0].TrimEnd();
                            leerweg();
                            RtB[0].SelectedText = " ";
                            if ((Modul1.Kont[0].Trim() == "") & (Modul1.Kont[3].Trim() == ""))
                            {
                                RtB[0].SelectedText = "und unbekannter Mutter";
                                goto IL_0a2d;
                            }
                            leerweg();
                            if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == ".")
                            {
                                RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
                                RtB[0].SelectionLength = 1;
                                RtB[0].SelectedText = ",";
                            }
                            RtB[0].SelectedText = " und " + (Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3]).Trim();
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Bold);
                            RtB[0].SelectedText = " " + Modul1.Kont[0];
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                            if (Modul1.DAus[78] == "1")
                            {
                                Art = default;
                                Ahne = 0.AsString();
                                Listart = 1;
                                neb = true;
                                Modul1.Datles3(Listart, default, Art, ref neb);
                                neb = true;
                                Datschreib(ref Pattext, 0, ref neb);
                                leerweg();
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                Art = EEventArt.eA_300;
                                neb = true;
                                Berufe(Art, ref neb);
                                Art = EEventArt.eA_301;
                                neb = true;
                                Berufe(Art, ref neb);
                            }
                            goto IL_0923;
                        }
                        leerweg();
                        RtB[0].SelectedText = " ";
                        RtB[0].SelectedText = "und unbekannter Mutter";
                        Modul1.FamInArb = famInArb;
                        Modul1.PersInArb = PerSp1;
                        if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == ".")
                        {
                            RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
                            RtB[0].SelectedText = "";
                        }
                        goto IL_0a0e;
                    IL_0923:
                        num = 97;
                        Modul1.Aschalt = Modul1.Datschalt;
                        goto IL_0a2d;
                    IL_0a0e:
                        num = 110;
                        RtB[0].SelectedText = ".)";
                        goto end_IL_0000_2;
                    IL_0a2d: // <========== 3
                        num = 113;
                        if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == ".")
                        {
                            RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
                            RtB[0].SelectionLength = 2;
                        }
                        goto IL_0aa5;
                    IL_0aa5:
                        num = 117;
                        RtB[0].SelectedText = ".)";
                        goto end_IL_0000_2;
                    IL_0b55:
                        num4 = num2 + 1;
                        goto IL_0b59;
                    IL_0b59:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 56:
                            case 59:
                            case 60:
                                goto IL_0539;
                            case 96:
                            case 97:
                                goto IL_0923;
                            case 109:
                            case 110:
                                goto IL_0a0e;
                            case 77:
                            case 98:
                            case 99:
                            case 112:
                            case 113:
                                goto IL_0a2d;
                            case 116:
                            case 117:
                                goto IL_0aa5;
                            case 111:
                            case 118:
                            case 120:
                            case 130:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj2) when (obj2 is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj2, lErl);
                try0000_dispatch = 3439;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Datschreib(ref bool neb)
    {
        neb = false;
        if (Modul1.DAus[63] == "1")
        {
            int FamPer = 1;
            ((_Modul1)Modul1).PerQu(ref FamPer);
            RtB[0].SelectionCharOffset = _byHoch;
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            RtB[0].SelectedText = Modul1.Kont[30];
            RtB[0].SelectionCharOffset = 0;
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        M1_Ki = true;
        Modul1.Datschalt = 0;
        short Listart = 1;
        string Ahne = 0.AsString();
        EEventArt Art = default;
        bool neb2 = false;
        Modul1.Datles3(Listart, default, Art, ref neb2);
        ref string patText = ref PatText;
        neb2 = false;
        Datschreib(ref patText, 0, ref neb2);
        if (Modul1.DAus[7] == "1")
        {
            Modul1.PerSatzLes(Modul1.PersInArb);
            if (Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
            {
                if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) != "\n")
                {
                    RtB[0].SelectedText = "\n";
                }
                Modul1.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem1].AsString();
                neb2 = false;
                Bemaus(ref neb2);
            }
        }
        leerweg();
    }
    public void Datschreib(ref string Pattext, byte umb, ref bool neb)
    {
        neb = false;
        bool neb2;

        if ((Modul1.Kont[11].Trim() != "") | (Modul1.Kont[16].Trim() != "") | (Modul1.Kont[21].Trim() != ""))
        {
            if ((Modul1.DAus[106].AsDouble() == 1.0) & !neb)
            {
                if (umb == 0)
                {
                    RtB[0].SelectedText = "\n";
                }
            }
            else
                RtB[0].SelectedText = " ";
            RtB[0].SelectedText = Modul1.DTxt[1];
            if (Modul1.Kont[11].Trim() != "")
            {
                RtB[0].SelectedText = " " + Modul1.Kont[11].Trim() + ".";
            }
            if (Modul1.Kont[31].Trim() != "")
            {
                RtB[0].SelectionCharOffset = _byHoch;
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                RtB[0].SelectedText = Modul1.Kont[31].Trim();
                RtB[0].SelectionCharOffset = 0;
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85] == "1" && Modul1.Kont[41].Trim() != "")
            {
                RtB[0].SelectedText = " Urkunde: " + Modul1.Kont[41].Trim();
            }
            if (Modul1.DAus[2] == "1" && Modul1.Kont[16].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[16];
                neb2 = false;
                Bemaus(ref neb2);
            }
            if (Modul1.DAus[3] == "1" && Modul1.Kont[21].Trim() != "")
            {
                Modul1.UbgT1 = Modul1.Kont[21];
                neb2 = false;
                Bemaus(ref neb2);
            }
        }
        if (Modul1.DAus[96].AsBool() && Modul1.Kont[51].Trim() != "")
        {
            leerweg();
            RtB[0].SelectedText = " ";
            if (Modul1.DAus[100] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            RtB[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[100] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            RtB[0].SelectedText = " " + Modul1.Kont[51].Trim() + ".";
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if ((Modul1.Kont[12].Trim() != "") | (Modul1.Kont[17].Trim() != "") | (Modul1.Kont[22].Trim() != ""))
        {
            if ((Modul1.DAus[106].AsDouble() == 1.0) & !neb)
            {
                if (umb == 0)
                {
                    RtB[0].SelectedText = "\n";
                }
            }
            else
                RtB[0].SelectedText = " ";
            RtB[0].SelectedText = Modul1.DTxt[2];
            if (Modul1.Kont[12].Trim() != "")
            {
                RtB[0].SelectedText = " " + Modul1.Kont[12].Trim() + ".";
            }
            if (Modul1.Kont[32].Trim() != "")
            {
                RtB[0].SelectionCharOffset = _byHoch;
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                RtB[0].SelectedText = Modul1.Kont[32].Trim();
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85].AsDouble() == 1.0 && Modul1.Kont[42].Trim() != "")
            {
                RtB[0].SelectedText = " Urkunde: " + Modul1.Kont[42].Trim();
            }
            if (Modul1.DAus[2] == "1" && Modul1.Kont[17].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[17];
                neb2 = false;
                Bemaus(ref neb2);
            }
            if (Modul1.DAus[3] == "1" && Modul1.Kont[22].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[22];
                neb2 = false;
                Bemaus(ref neb2);
            }
        }
        leerweg();
        if (Pattext != "")
        {
            RtB[0].SelectedText = " ";
            if (Modul1.DAus[100].AsDouble() == 1.0)
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            RtB[0].SelectedText = "Paten:";
            if (Modul1.DAus[100].AsDouble() == 1.0)
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            RtB[0].SelectedText = " " + Pattext.Trim() + ".";
            Pattext = "";
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if (Modul1.DAus[96].AsBool() && Modul1.Kont[52].Trim() != "")
        {
            leerweg();
            RtB[0].SelectedText = " ";
            if (Modul1.DAus[100] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            RtB[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[100] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            RtB[0].SelectedText = " " + Modul1.Kont[52].Trim() + ".";
        }
        if ((Modul1.Kont[13].Trim() != "") | (Modul1.Kont[18].Trim() != "") | (Modul1.Kont[23].Trim() != ""))
        {
            if ((Modul1.DAus[106].AsDouble() == 1.0) & !neb)
            {
                if (umb == 0)
                {
                    RtB[0].SelectedText = "\n";
                }
            }
            else
                RtB[0].SelectedText = " ";
            RtB[0].SelectedText = Modul1.DTxt[3];
            if (Modul1.Kont[13].Trim() != "")
            {
                RtB[0].SelectedText = " " + Modul1.Kont[13].Trim() + ".";
            }
            if (Modul1.Kont[33].Trim() != "")
            {
                RtB[0].SelectionCharOffset = _byHoch;
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                RtB[0].SelectedText = Modul1.Kont[33].Trim();
                RtB[0].SelectionCharOffset = 0;
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85].AsDouble() == 1.0 && Modul1.Kont[43].Trim() != "")
            {
                RtB[0].SelectedText = " Urkunde: " + Modul1.Kont[43].Trim();
            }
            if (Modul1.DAus[2] == "1" && Modul1.Kont[18].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[18];
                neb2 = false;
                Bemaus(ref neb2);
            }
            if (Modul1.DAus[3] == "1" && Modul1.Kont[23].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[23];
                neb2 = false;
                Bemaus(ref neb2);
            }
        }
        if (Modul1.DAus[96].AsBool() && Modul1.Kont[53].Trim() != "")
        {
            leerweg();
            RtB[0].SelectedText = " ";
            if (Modul1.DAus[72] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            RtB[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[72] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            RtB[0].SelectedText = " " + Modul1.Kont[53].Trim() + ".";
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        if ((Modul1.Kont[14].Trim() != "") | (Modul1.Kont[19].Trim() != "") | (Modul1.Kont[24].Trim() != ""))
        {
            if ((Modul1.DAus[106].AsDouble() == 1.0) & !neb)
            {
                if (umb == 0)
                {
                    RtB[0].SelectedText = "\n";
                }
            }
            else
                RtB[0].SelectedText = " ";
            RtB[0].SelectedText = Modul1.DTxt[4];
            if (Modul1.Kont[14].Trim() != "")
            {
                RtB[0].SelectedText = " " + Modul1.Kont[14].Trim() + ".";
            }
            if (Modul1.Kont[34].Trim() != "")
            {
                RtB[0].SelectionCharOffset = _byHoch;
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                RtB[0].SelectedText = Modul1.Kont[34].Trim() + " ";
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            }
            if (Modul1.DAus[85].AsDouble() == 1.0 && Modul1.Kont[44].Trim() != "")
            {
                RtB[0].SelectedText = " Urkunde: " + Modul1.Kont[44].Trim();
            }
            if (Modul1.DAus[2] == "1" && Modul1.Kont[19].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[19];
                neb2 = false;
                Bemaus(ref neb2);
            }
            if (Modul1.DAus[3] == "1" && Modul1.Kont[24].Length > 0)
            {
                Modul1.UbgT1 = Modul1.Kont[24];
                neb2 = false;
                Bemaus(ref neb2);
            }
        }
        if (Modul1.DAus[96].AsBool() && Modul1.Kont[54].Trim() != "")
        {
            leerweg();
            RtB[0].SelectedText = " ";
            if (Modul1.DAus[72] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Underline);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
            RtB[0].SelectedText = "Zeugen:";
            if (Modul1.DAus[72] == "1")
            {
                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
            }
            else RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            RtB[0].SelectedText = " " + Modul1.Kont[54].Trim() + ".";
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
        }
        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
    }
    public void Paten2()
    {
        _xPat = false;
        PerSp1 = Modul1.PersInArb;
        foreach (var cLink in DataModul.Link.ReadAllFams(Modul1.FamInArb, ELinkKennz.lkGodparent))
        {
            Modul1.PersInArb = cLink.iPersNr;
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            Namenindex();
            Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
            Modul1.Person_FullSurname(Modul1.Kont, Modul1.DAus[89] == "1");
            if (PatText == "")
            {
                PatText = Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim());
            }
            else
            {
                PatText = PatText + "; " + Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim());
            }
            Modul1.PersInArb = PerSp1;
        }
        Modul1.PersInArb = PerSp1;
        Modul1.PerSatzLes(Modul1.PersInArb);
        if (!Information.IsDBNull(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].Value) && Operators.CompareString(DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
        {
            Modul1.UbgT1 = DataModul.DB_PersonTable.Fields[PersonFields.Bem2].AsString();
            Modul1.UbgT1 = ((_Modul1)Modul1).Text_Retweg(Modul1.UbgT1);
            if (PatText == "")
            {
                PatText = Modul1.UbgT1.Trim();
                Modul1.UbgT1 = "";
            }
            else
            {
                PatText = PatText.Trim() + "; " + Modul1.UbgT1.Trim();
                Modul1.UbgT1 = "";
            }
        }
    }

    public void Sonst()
    {
        List3.Items.Clear();
        DataModul.DB_EventTable.Index = "Besu";
        DataModul.DB_EventTable.Seek("=", "105", Modul1.PersInArb.AsString());
        if (DataModul.DB_EventTable.NoMatch)
        {
            DataModul.DB_EventTable.Index = "ArtNr";
            return;
        }
        short num = 1;
        checked
        {
            while (!DataModul.DB_EventTable.EOF)
            {
                if (DataModul.DB_EventTable.NoMatch)
                {
                    DataModul.DB_EventTable.Index = "ArtNr";
                    return;
                }
                if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                {
                    Modul1_J = 0;
                    do
                    {
                        Modul1.Kont1[Modul1_J] = "";
                        Modul1_J = (byte)unchecked((uint)(Modul1_J + 1));
                    }
                    while (unchecked(Modul1_J) <= 15u);
                    Modul1.Ubg = num;
                    Modul1.sDatu = "";
                    if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                    {
                        DataModul.DB_EventTable.Index = "ArtNr";
                        break;
                    }
                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                    {
                        Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        Modul1.Kont1[1] = Modul1.sDatu;
                    }
                    Modul1.UbgT = "";
                    if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value) && DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                    {
                        int AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                        string LD;
                        DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                        if (Modul1.Kont[0] != "")
                        {
                            Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim() + ": ";
                        }
                    }
                    string text = (!Modul1.DAus[103].AsBool()) ? (Modul1.Kont1[7] + Modul1.Kont1[1] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString()) : ("!!!1" + Modul1.Kont1[1] + Modul1.Kont1[7] + new string(' ', 240).Left(240) + DataModul.DB_EventTable.Fields[EventFields.LfNr].AsString());
                    if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                    {
                        text = "+" + text;
                    }
                    if (text.Trim() != "")
                    {
                        List3.Items.Add(text);
                    }
                }
                DataModul.DB_EventTable.MoveNext();
                num = (short)unchecked(num + 1);
                if (num > 70)
                {
                    break;
                }
            }
            Sonstdat();
        }
    }
    public void Sonstdat()
    {
        int try0000_dispatch = -1;
        int num = default;
        string text = default;
        int num2 = default;
        int num3 = default;
        int lErl = default;
        int num5 = default;
        string Job = default;
        string text2 = default;
        string value = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    object[] array;
                    int ortNr;
                    byte Schalt;
                    int Nr;
                    EEventArt Art;
                    short LfNR;
                    int AAA;
                    string LD;
                    bool neb;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            text = "";
                            goto IL_0009;
                        case 4183:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0e19;
                                    default:
                                        goto end_IL_0000;
                                }
                                lErl = 200;
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
                                goto IL_0e1d;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            num = 2;
                            Job = "";
                            array = new object[6];
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num5 = 0;
                            while (num5 <= List3.Items.Count - 1)
                            {
                                if (Modul1.DAus[106].AsDouble() == 1.0)
                                {
                                    RtB[0].SelectedText = "\n";
                                }
                                else
                                    RtB[0].SelectedText = " ";

                                num = 12;
                                Modul1.LfNR = Conversions.ToByte(List3.Items[num5].AsString().Right(10));
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", "105", Modul1.PersInArb.AsString(), Modul1.LfNR);
                                if (Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                                {
                                    Interaction.MsgBox("Stop 14");
                                }
                                var Modul1_J = 0;
                                while (unchecked(Modul1_J) <= 15u)
                                {
                                    Modul1.Kont1[Modul1_J] = "";
                                    Modul1_J++;
                                }
                                Modul1.sDatu = "";
                                if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch | (DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != Modul1.PersInArb) | (DataModul.DB_EventTable.Fields[EventFields.Art].AsInt() != 105)))
                                {
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    break;
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                    }
                                }
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                    Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                    text2 = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    if ((text2.Trim() == "") & DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate() != default)
                                    {
                                        text2 = " von";
                                    }
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                    Modul1.Kont1[1] = Modul1.sDatu;
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                {
                                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                    Modul1.sDatu = "00000000" + Modul1.sDatu.Right(8);
                                    text2 = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, text2);
                                    if (Modul1.sDatu.Trim() != "")
                                    {
                                        Modul1.sDatu = " bis " + Modul1.sDatu.Trim();
                                    }
                                    if (Modul1.Kont1[1].Trim() != "")
                                    {
                                        Modul1.Kont1[1] = " von " + Modul1.Kont1[1].Trim();
                                    }
                                    Modul1.Kont1[3] = Modul1.sDatu;
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                    {
                                        value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                        AAA = value.AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out var _sUbgT, out LD); Modul1.UbgT = _sUbgT;
                                        value = AAA.AsString();
                                        if (Modul1.UbgT.Trim() != "")
                                        {
                                            Modul1.Kont1[3] = Modul1.Kont1[3] + " (" + Modul1.UbgT.Trim() + ")";
                                            Modul1.UbgT = "";
                                        }
                                    }
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                {
                                    ortNr = (int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble());
                                    Modul1.UbgT = Modul1.ortles(ortNr, 1);
                                    Modul1.Kont1[5] = " " + Modul1.UbgT.Trim();
                                    Modul1.UbgT = "";
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[6] = " " + Modul1.Kont[0].Trim();
                                    }
                                }
                                Nr = Modul1.PersInArb;
                                Art = EEventArt.eA_105;
                                LfNR = Modul1.LfNR;
                                ((_Modul1)Modul1).QuellenDatum(ref Nr, Art, ref LfNR);
                                Modul1.LfNR = Conversions.ToByte(LfNR);
                                Modul1.PersInArb = Nr.AsInt();
                                if (((Modul1.DAus[38] == "1") & !M1_Ki) | ((Modul1.DAus[42] == "1") & M1_Ki))
                                {
                                    Job = Modul1.Kont1[7].TrimEnd();
                                }
                                else if (((Modul1.DAus[39] == "1") & !M1_Ki) | ((Modul1.DAus[43] == "1") & M1_Ki))
                                {
                                    Job = "";
                                    Job = Module2.Jobdreh(Job);
                                    Job += text;
                                    text = "";
                                }
                                leerweg();
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                RtB[0].SelectionCharOffset = 0;
                                if (Modul1.DAus[106].AsDouble() != 1.0) RtB[0].SelectedText = " ";
                                else if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) != "\n")
                                {
                                    RtB[0].SelectedText = "\n";
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.ArtText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.ArtText].AsDouble() > 0.0)
                                    {
                                        AAA = DataModul.DB_EventTable.Fields[EventFields.ArtText].AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                        if (Modul1.Kont[0] != "")
                                        {
                                            Modul1.Kont1[10] = " " + Modul1.Kont[0].Trim() + ": ";
                                        }
                                    }
                                }
                                if (Modul1.Kont1[10].Trim() != "")
                                {
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Underline);
                                    RtB[0].SelectedText = Modul1.Kont1[10].Trim();
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                RtB[0].SelectedText = " " + Job.Trim() + ". ";
                                if (((Modul1.DAus[40] == "1") & !M1_Ki) | ((Modul1.DAus[44] == "1") & M1_Ki))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim();
                                        neb = false;
                                        Bemaus(ref neb);
                                    }
                                }
                                if (((Modul1.DAus[41] == "1") & !M1_Ki) | ((Modul1.DAus[45] == "1") & M1_Ki))
                                {
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim(), "", TextCompare: false) != 0)
                                    {
                                        Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim();
                                        neb = false;
                                        Bemaus(ref neb);
                                    }
                                }
                                if (Modul1.Kont1[9].Trim() != "")
                                {
                                    RtB[0].SelectionCharOffset = _byHoch;
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    RtB[0].SelectedText = Modul1.Kont1[9];
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                RtB[0].SelectionCharOffset = 0;
                                RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                DataModul.DB_EventTable.MoveNext();
                                num5++;
                            }

                            goto end_IL_0000_2;

                        IL_0e19:
                            num4 = unchecked(num2 + 1);
                            goto IL_0e1d;
                        IL_0e1d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 8:
                                case 11:
                                case 12:
                                case 24:
                                case 133:
                                case 139:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 4183;
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
    public void Weitehen()
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int persInArb = default;
        int famInArb = default;
        ELinkKennz eLKnz = default;
        byte b2 = default;
        byte b3 = default;
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
                    bool neb;
                    switch (try0000_dispatch)
                    {
                        default:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            goto IL_0008;
                        case 2353:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_079f;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 3022)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_079f;
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
                                goto IL_07a3;
                            }
                        end_IL_0000:
                            break;
                        IL_0008:
                            num = 2;
                            persInArb = Modul1.PersInArb;
                            famInArb = Modul1.FamInArb;
                            Modul1.eLKennz = ELinkKennz.lkFather;
                            eLKnz = ELinkKennz.lkMother;
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            if (DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F")
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                                eLKnz = ELinkKennz.lkFather;
                            }
                            ListBox1.Items.Clear();
                            var aiFams = DataModul.Link.GetPersonFams(Modul1.PersInArb, Modul1.eLKennz);
                            if (aiFams.Count > 1)
                            {
                                _0024STATIC_0024Weitehen_00242001_0024a = (byte)Math.Round(Modul1.UbgT.Length / 10.0);
                                b2 = _0024STATIC_0024Weitehen_00242001_0024a;
                                _0024STATIC_0024Weitehen_00242001_0024I4 = 1;
                                goto IL_033b;
                            }
                            goto IL_06c9;
                        IL_013c: // <========== 3
                            num = 22;
                            DataModul.DB_EventTable.Seek("=", _0024STATIC_0024Weitehen_00242001_0024D, Modul1.FamInArb.AsString(), "0");
                            if (DataModul.DB_EventTable.NoMatch)
                            {
                                Modul1.sDatu = "        ";
                                _0024STATIC_0024Weitehen_00242001_0024D = (short)unchecked(_0024STATIC_0024Weitehen_00242001_0024D + 1);
                                if (_0024STATIC_0024Weitehen_00242001_0024D <= 505)
                                {
                                    goto IL_013c;
                                }
                            
                            }
                            else
                            {
                                Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                            }
                            goto IL_022a;
                        IL_022a: // <========== 3
                            num = 31;
                            if (Modul1.sDatu.AsDouble() == 0.0)
                            {
                                DataModul.DB_EventTable.Seek("=", 601, Modul1.FamInArb.AsString(), "0");
                                if (!DataModul.DB_EventTable.NoMatch)
                                {
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);

                                }
                            }
                            goto IL_02fa;
                        IL_02fa: // <========== 3
                            num = 37;
                            ListBox1.Items.Add(Modul1.sDatu + Modul1.FamInArb.AsString());
                            _0024STATIC_0024Weitehen_00242001_0024I4 = (byte)unchecked((uint)(_0024STATIC_0024Weitehen_00242001_0024I4 + 1));
                            goto IL_033b;
                        IL_033b:
                            if (unchecked(_0024STATIC_0024Weitehen_00242001_0024I4 <= (uint)b2))
                            {
                                if (Modul1.UbgT.Length != 0)
                                {
                                    Modul1.FamInArb = Modul1.UbgT.Left(10).AsInt();
                                    Modul1.UbgT = Strings.Mid(Modul1.UbgT, 11, Modul1.UbgT.Length);
                                    _0024STATIC_0024Weitehen_00242001_0024D = 502;
                                    goto IL_013c;
                                }
                            }
                            _0024STATIC_0024Weitehen_00242001_0024Fa = 0;
                            b3 = (byte)(ListBox1.Items.Count - 1);
                            _0024STATIC_0024Weitehen_00242001_0024I4 = 0;
                            goto IL_06bc;
                        IL_05b6: // <========== 4
                            num = 69;
                            lErl = 3;
                            if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == "\n")
                            {
                                RtB[0].SelectionStart = RtB[0].SelectionStart - 1;
                                RtB[0].SelectionLength = 1;
                                RtB[0].SelectedText = "";
                                goto IL_05b6;
                            }
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            RtB[0].SelectedText = ")\n";
                            goto IL_069e;
                        IL_069e: // <========== 3
                            num = 78;
                            lErl = 211;
                            _0024STATIC_0024Weitehen_00242001_0024I4 = (byte)unchecked((uint)(_0024STATIC_0024Weitehen_00242001_0024I4 + 1));
                            goto IL_06bc;
                        IL_06bc:
                            if (unchecked(_0024STATIC_0024Weitehen_00242001_0024I4 <= (uint)b3))
                            {
                                _0024STATIC_0024Weitehen_00242001_0024Fa++;
                                Modul1.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(ListBox1.Items[_0024STATIC_0024Weitehen_00242001_0024I4].AsString(), 9, 10)));
                                if (Modul1.FamInArb != famInArb)
                                {
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (Modul1.eLKennz.AsDouble() == 1.0)
                                    {
                                        RtB[0].SelectedText = " (Er in anderer Verbindung";
                                    }
                                    if (Modul1.eLKennz.AsDouble() == 2.0)
                                    {
                                        RtB[0].SelectedText = " (Sie in anderer Verbindung";
                                    }
                                    neb = true;
                                    Heidat(ref neb);
                                    DataModul.NB_FamilyTable.AddNew();
                                    DataModul.NB_FamilyTable.Fields[IndexFields.Fam].Value = Modul1.FamInArb;
                                    DataModul.NB_FamilyTable.Update();
                                    RtB[0].SelectedText = " mit ";
                                    if (DataModul.Link.GetFamPerson(Modul1.FamInArb, eLKnz, out var _iPersInArb3))
                                    {
                                        Modul1.PersInArb = _iPersInArb3;
                                        LSchalt = true;
                                        neb = true;
                                        EPerles(ref neb);
                                        LSchalt = false;
                                    }
                                    else
                                        RtB[0].SelectedText = "unbekannt.";
                                    goto IL_05b6;
                                }
                                goto IL_069e;
                            }
                            goto IL_06c9;
                        IL_06c9: // <========== 3
                            num = 81;
                            Modul1.PersInArb = persInArb;
                            Modul1.eLKennz = ELinkKennz.lkFather;
                            Modul1.PerSatzLes(Modul1.PersInArb);
                            if (!(DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString() == "F"))
                            {
                            }
                            else
                            {
                                Modul1.eLKennz = ELinkKennz.lkMother;
                            }
                            goto end_IL_0000_2;
                        IL_079f:
                            num4 = unchecked(num2 + 1);
                            goto IL_07a3;
                        IL_07a3:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 22:
                                    goto IL_013c;
                                case 28:
                                case 31:
                                    goto IL_022a;
                                case 35:
                                case 36:
                                case 37:
                                    goto IL_02fa;
                                case 65:
                                case 68:
                                case 69:
                                case 74:
                                    goto IL_05b6;
                                case 44:
                                case 78:
                                    goto IL_069e;
                                case 80:
                                case 81:
                                    goto IL_06c9;
                                case 86:
                                case 87:
                                case 96:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 2353;
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

    public void Heidat(ref bool neb)
    {
        neb = false;
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        EEventArt num5 = default;
        string Job = default;
        EEventArt _eArt = default;
        short num6 = default;
        string ds = default;
        string value = default;
        string text = default;
        byte b = default;
        string text2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                int ortNr;
                int AAA;
                bool neb2;
                int Nr;
                short LfNR;
                byte Schalt;
                short Listart;
                string LD;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 6194:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_145c;
                                default:
                                    goto end_IL_0000;
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
                            goto IL_1460;
                        }
                    end_IL_0000:
                        break;
                    IL_0008:
                        num = 2;
                        Job = "";
                        if (Modul1.FamInArb == 0)
                        {
                            goto end_IL_0000_2;
                        }
                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                        if (Modul1.DAus[76] == "1")
                        {
                            RtB[0].SelectedText = "[" + Modul1.FamInArb.AsString() + "]";
                        }
                        DataModul.DB_FamilyTable.Index = "Fam";
                        DataModul.DB_FamilyTable.Seek("=", Modul1.FamInArb);
                        if (DataModul.DB_FamilyTable.Fields[FamilyFields.Aeb].AsInt() == -1)
                        {
                            if (!(((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[82].AsDouble() == 1.0)) | ((0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[83].AsDouble() == 1.0))))
                            {
                            
                            }
                            else
                            {
                                RtB[0].SelectedText = " " + Modul1.DTxt[12] + " ";
                            }
                            goto end_IL_0000_2;
                        }
                        num5 = EEventArt.eA_500;
                        while (num5 <= EEventArt.eA_507)
                        {
                            num = 19;
                            _eArt = num5 switch
                            {
                                EEventArt.eA_500 => EEventArt.eA_501,
                                EEventArt.eA_501 => EEventArt.eA_500,
                                _ => num5
                            };
                            Modul1.sDatu = "";
                            num6 = 1;
                            while (num6 <= 8)
                            {
                                Modul1.Kont1[num6] = "";
                                num6 = checked((short)unchecked(num6 + 1));
                            }
                            DataModul.DB_EventTable.Index = "ArtNr";
                            DataModul.DB_EventTable.Seek("=", _eArt, Modul1.FamInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    if (Modul1.DAus[124].AsDouble() == 1.0)
                                    {
                                        if (Operators.CompareString(Modul1.sDatu, Modul1.DAus[125], TextCompare: false) > 0)
                                        {
                                            goto end_IL_0000_2;
                                        }
                                    }
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, ds);
                                    Modul1.Kont1[1] = Modul1.sDatu;
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.DatumB].AsInt() > 0)
                                {
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString();
                                    Modul1.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumB].AsDate().AsString().Trim(), 8);
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, ds);
                                    if (Operators.CompareString(DataModul.DB_EventTable.Fields[EventFields.DatumB_S].AsString().Trim(), "", TextCompare: false) != 0)
                                        Modul1.Kont1[3] = " " + Modul1.sDatu;
                                    else
                                    {
                                        Modul1.Kont1[3] = "/" + Modul1.sDatu;
                                    }
                                }
                                if (!Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.DatumText].Value))
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.DatumText].AsInt() > 0)
                                    {
                                        value = DataModul.DB_EventTable.Fields[EventFields.DatumText].AsString();
                                        AAA = value.AsInt();
                                        LD = "";
                                        DataModul.Textlese(AAA, out var _sUbgT, out LD); Modul1.UbgT = _sUbgT;
                                        value = AAA.AsString();
                                        if (Modul1.UbgT.Trim() != "")
                                        {
                                            Modul1.Kont1[3] = Modul1.Kont1[3] + " (" + Modul1.UbgT.Trim() + ")";
                                            Modul1.UbgT = "";
                                        }
                                    }
                                }
                                Modul1.UbgT = "";
                                if (DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble() > 0.0)
                                {
                                    ortNr = checked((int)Math.Round(DataModul.DB_EventTable.Fields[EventFields.Ort].AsDouble()));
                                    Modul1.UbgT = Modul1.ortles1(ortNr, 1, (i, s) => Modul1.ExportPlace(i, s, Modul1.Ind1, M_Namen));
                                    Modul1.Kont1[5] = Modul1.UbgT;
                                    Modul1.UbgT = "";
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.Platz].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.Platz].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[6] = " " + Modul1.Kont[0].Trim();
                                    }
                                }
                                if (DataModul.DB_EventTable.Fields[EventFields.KBem].AsDouble() > 0.0)
                                {
                                    AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                                    LD = "";
                                    DataModul.Textlese(AAA, out var _sKont0, out LD); Modul1.Kont[0] = _sKont0;
                                    if (Modul1.Kont[0] != "")
                                    {
                                        Modul1.Kont1[7] = " " + Modul1.Kont[0].Trim();
                                    }
                                }
                                num6 = 1;
                                while (num6 <= 6)
                                {
                                    if (Modul1.Kont1[num6] == "0")
                                    {
                                        Modul1.Kont1[num6] = "";
                                    }
                                    num6 = checked((short)unchecked(num6 + 1));
                                }
                                if ((Modul1.Kont1[1].Trim() != "") | (Modul1.Kont1[2].Trim() != "") | (Modul1.Kont1[3].Trim() != "") | (Modul1.Kont1[5].Trim() != "") | (Modul1.Kont1[6].Trim() != "") | (Modul1.UbgT.Trim() != "") | ((Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim()) > 0) & (Modul1.DAus[5] == "1")) | ((Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim()) > 0) & (Modul1.DAus[6] == "1")))
                                {
                                    text = "";
                                    switch (_eArt)
                                    {
                                        case EEventArt.eA_500:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[52] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[57] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[5];
                                            break;
                                        case EEventArt.eA_501:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[53] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[58] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[6];
                                            break;
                                        case EEventArt.eA_Marriage:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[64] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[65] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[7];
                                            break;
                                        case EEventArt.eA_MarrReligious:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[54] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[59] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[8];
                                            break;
                                        case EEventArt.eA_504:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[55] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[60] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[9];
                                            break;
                                        case EEventArt.eA_505:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[56] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[61] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[10];
                                            break;
                                        case EEventArt.eA_507:
                                            if ((0 - (M1_Ki ? 1 : 0) == 0) & (Modul1.DAus[84] == "0") || (0 - (M1_Ki ? 1 : 0) == 1) & (Modul1.DAus[86] == "0"))
                                            {
                                                num5++;
                                                continue;
                                            }
                                            text = Modul1.DTxt[15];
                                            break;
                                        default:
                                            break;
                                    }
                                    if ((Modul1.Kont1[1].Trim() != "") | (Modul1.Kont1[3].Trim() != "") | (Modul1.Kont1[5].Trim() != "") | (Modul1.Kont1[6].Trim() != "") | (Modul1.Kont1[7].Trim() != "") | (Modul1.Kont1[8].Trim() != "") | (Modul1.UbgT.Trim() != ""))
                                    {
                                        leerweg();
                                        if (Modul1.DAus[106].AsDouble() != 1.0 | neb)
                                            RtB[0].SelectedText = " ";
                                        else
                                        {
                                            RtB[0].SelectedText = "\n";
                                        }
                                        Job = "";
                                        Job = Module2.Jobdreh(Job);
                                        Job = (text + " " + Job).Trim();
                                        RtB[0].SelectedText = Job;
                                        Hei = true;
                                    }
                                }
                                if (Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString().Trim()) > 0)
                                {
                                    if (Modul1.DAus[5] == "1")
                                    {
                                        Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem1].AsString();
                                        neb2 = false;
                                        Bemaus(ref neb2);
                                    }
                                }
                                if (Strings.Len(DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString().Trim()) > 0)
                                {
                                    if (Modul1.DAus[6] == "1")
                                    {
                                        Modul1.UbgT1 = DataModul.DB_EventTable.Fields[EventFields.Bem2].AsString();
                                        neb2 = false;
                                        Bemaus(ref neb2);
                                    }
                                }
                                Nr = Modul1.FamInArb;
                                LfNR = 0;
                                ((_Modul1)Modul1).QuellenDatum(ref Nr, _eArt, ref LfNR);
                                Modul1.FamInArb = Nr.AsInt();
                                if (Modul1.Kont1[9].Trim() != "")
                                {
                                    leerweg();
                                    RtB[0].SelectionCharOffset = _byHoch;
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    RtB[0].SelectedText = " " + Modul1.Kont1[9].Trim() + " ";
                                    RtB[0].SelectionCharOffset = 0;
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                if (Modul1.DAus[96].AsBool())
                                {
                                    Modul1.PersSp = Modul1.PersInArb;
                                    b = 1;
                                    while (b <= 100u)
                                    {
                                        _lstKontSP1[b] = Modul1.Kont1[b];
                                        _lstKontSP[b] = Modul1.Kont[b];
                                        Modul1.Kont[b] = "";
                                        Modul1.Kont1[b] = "";
                                        b = checked((byte)unchecked((uint)(b + 1)));
                                    }
                                    Schalt = 0;
                                    Listart = 1;
                                    LD = "";
                                    Modul1.Zeugsu(_eArt, Schalt, Listart, 0L);
                                    Modul1.PersInArb = Modul1.PersSp;
                                    text2 = Modul1.Kont1[20];
                                    b = 1;
                                    while (b <= 100u)
                                    {
                                        Modul1.Kont1[b] = _lstKontSP1[b];
                                        Modul1.Kont[b] = _lstKontSP[b];
                                        _lstKontSP[b] = "";
                                        _lstKontSP1[b] = "";
                                        b = checked((byte)unchecked((uint)(b + 1)));
                                    }
                                    if (text2 != "")
                                    {
                                        leerweg();
                                        if (text2.Trim().Right(1) != ";")
                                        {
                                            text2 = text2.Trim() + ";";
                                        }
                                        if (Modul1.DAus[100].AsDouble() == 1.0)
                                        {
                                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                        }
                                        RtB[0].SelectedText = ", Zeugen: " + text2.Trim();
                                        text2 = "";
                                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    }
                                }
                                if (Modul1.DAus[85] == "1")
                                {
                                    if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                                    {
                                        RtB[0].SelectedText = " Urkunde: " + DataModul.DB_EventTable.Fields[EventFields.Reg].AsString().Trim();
                                    }
                                }
                                leerweg();
                            }
                            num5++;
                        }
                        leerweg();
                        goto end_IL_0000_2;
                    IL_145c:
                        num4 = num2 + 1;
                        goto IL_1460;
                    IL_1460:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 19:
                            case 21:
                            case 24:
                            case 27:
                            case 28:
                            case 55:
                            case 58:
                            case 59:
                            case 60:
                            case 96:
                            case 106:
                            case 115:
                            case 124:
                            case 133:
                            case 142:
                            case 151:
                            case 160:
                            case 161:
                            case 165:
                            case 168:
                            case 169:
                            case 174:
                            case 175:
                            case 176:
                            case 35:
                            case 100:
                            case 103:
                            case 109:
                            case 112:
                            case 118:
                            case 121:
                            case 127:
                            case 130:
                            case 136:
                            case 139:
                            case 145:
                            case 148:
                            case 154:
                            case 157:
                            case 233:
                            case 4:
                            case 15:
                            case 16:
                            case 41:
                            case 236:
                            case 241:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0000_dispatch = 6194;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 6
            // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    public void Bild(string BKennz, int Nr)
    {
        if (Modul1.DAus[88] != "1")
        {
            return;
        }

        DataModul.DB_PictureTable.Index = "Perkenn  ";
        DataModul.DB_PictureTable.Seek("=", BKennz, Nr);
        while (!DataModul.DB_PictureTable.EOF && !DataModul.DB_PictureTable.NoMatch && !(DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() != Nr)
            && !(DataModul.DB_PictureTable.Fields[PictureFields.Kennz].AsString() != BKennz))
        {
            string text = (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Left(1) != "#") ? (DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString() + DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString()) : Conversions.ToString(Modul1.Verz +  Strings.Mid(DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(), 2, DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString().Length) +  DataModul.DB_PictureTable.Fields[PictureFields.Datei].Value);
            while (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == "\n")
            {
                RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
                RtB[0].SelectionLength = 1;
                RtB[0].SelectedText = "";
            }
            if (Operators.CompareString(RtB[0].Text.Trim().Right(1), "\n", TextCompare: false) != 0)
            {
                RtB[0].SelectedText = "\n";
            }
            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
            Modul1.UbgT1 = ("Bild: " + text +  " " +  DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString());
            if (Modul1.DAus[70] == "0")
            {
                Modul1.UbgT1 = ((_Modul1)Modul1).Text_Retweg(Modul1.UbgT1);
            }
            RtB[0].SelectedText = Modul1.UbgT1 + "\n";
            Modul1.UbgT1 = "";
            DataModul.DB_PictureTable.MoveNext();
        }
    }

    public void Pate_bei2(int persInArb)
    {
        int try0000_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        List<(int, ELinkKennz)> text2 = new();
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                checked
                {
                    int num4;
                    string ds;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            ds = "";
                            goto IL_0009;
                        case 5674:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_12f4;
                                    default:
                                        goto end_IL_0000;
                                }
                                if (Information.Err().Number == 94)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_12f4;
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
                                goto IL_12f8;
                            }
                        end_IL_0000:
                            break;
                        IL_0009:
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            MyProject.Forms.Hinter.List3.Items.Clear();
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkGodparent))
                            {
                                persInArb = cLink.iFamNr;
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb, 0);
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb, 0);
                                }
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Modul1.sDatu = "          ";
                                }
                                else
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                }
                                else
                                    Modul1.sDatu = "          ";
                                MyProject.Forms.Hinter.List3.Items.Add(new ListItem(Modul1.sDatu, persInArb));
                            }

                            int num6;
                            if (MyProject.Forms.Hinter.List3.Items.Count > 0)
                            {
                                int num5 = MyProject.Forms.Hinter.List3.Items.Count - 1;
                                num6 = 0;
                                while (num6 <= num5)
                                {
                                    persInArb = MyProject.Forms.Hinter.List3.Items[num6].ItemData<int>();
                                    Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                    DataModul.DB_EventTable.Index = "ArtNr";
                                    DataModul.DB_EventTable.Seek("=", 102.AsString(), persInArb, 0);
                                    if (DataModul.DB_EventTable.NoMatch)
                                    {
                                        DataModul.DB_EventTable.Seek("=", 101.AsString(), persInArb, 0);
                                    }
                                    if (DataModul.DB_EventTable.NoMatch)
                                    {
                                        Modul1.sDatu = "          ";
                                    }
                                    else
                                    if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                        ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                        Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, ds);
                                    }
                                    else
                                        Modul1.sDatu = "          ";
                                    RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                                    if (Modul1.DAus[100] == "1")
                                    {
                                        RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)(Modul1.DAus[102].AsDouble() * 0.8), FontStyle.Regular);
                                    }
                                    if (num6 == 0)
                                    {
                                        RtB[0].SelectedText = " Pate: " + Modul1.sDatu + " bei " + Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim());
                                    }
                                    else
                                    if (MyProject.Forms.Ausw.Option3[0].Checked)
                                    {
                                        RtB[0].SelectedText = " Pate: " + Modul1.sDatu + " bei " + Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim());
                                    }
                                    else
                                        RtB[0].SelectedText = " ; " + Modul1.sDatu + " bei " + Strings.Trim(Modul1.Person.Prae.TrimEnd() + " " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim());
                                    num6++;
                                }
                            }
                            text2.Clear();
                            int num7 = 5;
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkMarrWitness))
                            {
                                text2.Add((cLink.iFamNr, Modul1.eLKennz));
                            }
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkWitnOfEngage))
                            {
                                text2.Add((cLink.iFamNr, Modul1.eLKennz));
                            }
                            foreach (var cLink in DataModul.Link.ReadAllPers(persInArb, ELinkKennz.lkWitnOfMarr))
                            {
                                text2.Add((cLink.iFamNr, Modul1.eLKennz));
                            }
                            num7 = text2.Count;
                            int num8 = text2.Count;
                            num6 = 1;
                            while (num6 <= num8)
                            {
                                Modul1.FamInArb = text2.First().Item1;
                                Modul1.eLKennz = text2.First().Item2;
                                text2.RemoveAt(0);
                                DataModul.DB_EventTable.Index = "ArtNr";
                                DataModul.DB_EventTable.Seek("=", 502, Modul1.FamInArb.AsString(), "0");
                                Modul1.Art = EEventArt.eA_Marriage;
                                if (DataModul.DB_EventTable.NoMatch)
                                {
                                    Modul1.sDatu = "    ";
                                }
                                else
                                if (Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                {
                                    Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                }
                                if (Modul1.sDatu.Trim() == "")
                                {
                                    DataModul.DB_EventTable.Seek("=", 503, Modul1.FamInArb.AsString(), "0");
                                    if (!DataModul.DB_EventTable.NoMatch && Conversion.Val(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate()) > 0.0)
                                    {
                                        Modul1.sDatu = Strings.Right("00000000" + DataModul.DB_EventTable.Fields[EventFields.DatumV].AsDate().AsString().Trim(), 8);
                                    }
                                }
                                if (Modul1.sDatu.Trim() != "")
                                {
                                    ds = DataModul.DB_EventTable.Fields[EventFields.DatumV_S].AsString();
                                }
                                if (Modul1.sDatu.Trim() != "")
                                {
                                    Modul1.sDatu = Modul1.Datwand1(Modul1.sDatu, ds);
                                }
                                _sLiText = " " + Modul1.sDatu + " bei ";
                                Modul1.sDatu = "";
                                Modul1.Famles();
                                persInArb = Modul1.Family.Mann;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                string text3 = Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim();
                                _sLiText += text3;
                                persInArb = Modul1.Family.Frau;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                text3 = " " + Modul1.Kont[3].Trim() + " " + Modul1.Kont[0].Trim();
                                if (MyProject.Forms.Ausw.Option3[0].Checked)
                                {
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "\nTrauzeuge#alt#:";
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectionFont = MyProject.Forms.Anzeige.RichTextBox1[0].SelectionFont.ChangeFBold(false);
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = _sLiText + " und " + text3 + ".";
                                }
                                else
                                {
                                    if (num6 == 1)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "Trauzeuge#alt#:";
                                    }
                                    else
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = "; ";
                                    MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = _sLiText + " und " + text3;
                                    if (num7 == num6)
                                    {
                                        MyProject.Forms.Anzeige.RichTextBox1[0].SelectedText = ".";
                                    }
                                }
                                _sLiText = "";
                                num6++;
                            }
                            RtB[0].SelectionFont = new Font(Modul1.DAus[101], (float)Modul1.DAus[102].AsDouble(), FontStyle.Regular);
                            goto end_IL_0000_2;
                        IL_12f4:
                            num4 = unchecked(num2 + 1);
                            goto IL_12f8;
                        IL_12f8:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 5674;
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
    public void Retweg2()
    {
        byte b = default;

        int num4;


        if (RtB[0].Text.Length == 0)
            return;

        b = 1;
        while (b == 1)
        {
            leerweg();
            b = 0;
            if (RtB[0].Text.Length == 0)
            {
                return;
            }
            if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == "\n")
            {
                RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
                RtB[0].SelectionLength = 1;
                RtB[0].SelectedText = "";
                b = 1;
            }
            if (RtB[0].Text.Length == 0)
            {
                return;
            }
            if (Strings.Mid(RtB[0].Text, RtB[0].SelectionStart, 1) == "\r")
            {
                RtB[0].SelectionStart = checked(RtB[0].SelectionStart - 1);
                RtB[0].SelectionLength = 1;
                RtB[0].SelectedText = "";
                b = 1;
            }
            leerweg();
        }
        RtB[0].SelectionCharOffset = 0;
    }
    public void Datcheck()
    {
        //Discarded unreachable code: IL_010e
        Datklein = 0.AsString();
        DataModul.DB_EventTable.Index = "ArtNr";
        Modul1.Ubg = 101;
        DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
        if (!DataModul.DB_EventTable.NoMatch)
        {
            Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
            if (!(DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() == 0))
            {
                goto IL_0186;
            }
        }
        Modul1.Ubg = 102;
        DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
        if (!DataModul.DB_EventTable.NoMatch)
        {
            Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
        }
        goto IL_0186;
    IL_0186:
        if (Datklein.AsDouble() == 30000000.0)
        {
            Datklein = 0.AsString();
        }
        Datum6 = Datklein;
    }

    public void TotPrüf3(ref byte Tot)
    {
        Tot = 1;
        if (Modul1.Aus[172] == "0")
        {
            Modul1.Aus[171] = Conversions.ToString(Conversion.Val(Strings.Mid(DateTime.Now.AsString(), 7, 4) + Strings.Mid(DateTime.Now.AsString(), 4, 2) + DateTime.Now.AsString().Left(2)));
        }
        Datklein = 30000000.AsString();
        DataModul.DB_EventTable.Index = "ArtNr";
        Modul1.Ubg = 103;
        DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
        if (DataModul.DB_EventTable.NoMatch)
        {
            Modul1.Ubg = 104;
            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
            if (!DataModul.DB_EventTable.NoMatch)
            {
                Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
            }
        }
        else
        {
            if (DataModul.DB_EventTable.Fields[EventFields.DatumV].AsInt() > 0)
            {
                Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
            }
            if (Information.IsDBNull(DataModul.DB_EventTable.Fields[EventFields.tot].Value))
            {
            }
        }
        if (Datklein.AsDouble() < Modul1.Aus[171].AsDouble())
        {
            Tot = 2;
        }
        if ((Datklein.AsDouble() != 30000000.0) & (Datklein.AsDouble() > Modul1.Aus[171].AsDouble()))
        {
            Tot = 3;
        }
    }

    public void TotPrüf(ref byte Tot)
    {
        int try0000_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
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
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0007;
                    case 646:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_020a;
                                default:
                                    goto end_IL_0000;
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
                            goto IL_020d;
                        }
                    end_IL_0000:
                        break;
                    IL_0007:
                        num = 2;
                        Tot = 1;
                        if (Modul1.DAus[122] != "1")
                        {
                            goto end_IL_0000_2;
                        }
                        Datklein = 30000000.AsString();
                        DataModul.DB_EventTable.Index = "ArtNr";
                        Modul1.Ubg = 103;
                        DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
                        if (DataModul.DB_EventTable.NoMatch)
                        {
                            Modul1.Ubg = 104;
                            DataModul.DB_EventTable.Seek("=", Modul1.Ubg.AsString(), Modul1.PersInArb.AsString(), "0");
                            if (!DataModul.DB_EventTable.NoMatch)
                            {
                                Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                            }
                        
                        }
                        else
                        {
                            Datklein = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        }
                        goto IL_019d;
                    IL_019d: // <========== 3
                        num = 18;
                        if (!(Datklein.AsDouble() < Modul1.DAus[123].AsDouble()))
                        {
                        
                        }
                        else
                        {
                            Tot = 2;
                        }
                        goto end_IL_0000_2;
                    IL_020a:
                        num4 = num2 + 1;
                        goto IL_020d;
                    IL_020d:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 13:
                            case 14:
                            case 17:
                            case 18:
                                goto IL_019d;
                            case 20:
                            case 21:
                            case 22:
                            case 27:
                                goto end_IL_0000_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 646;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0000_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void _Command_1_Click(object sender, EventArgs e)
    {
    }
}
