using BaseLib.Helper;
using Gen_FreeWin.Main;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFreeWin.Views;
//using DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Gen_FreeWin;

internal class Bemsuch : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    private IContainer _components;
    IModul1 Modul1 => _Modul1.Instance;
    IStrings Strings => Modul1.Strings;
    IProjectData ProjectData => Modul1.ProjectData;

    public ToolTip ToolTip1;

    [AccessedThroughProperty(nameof(Command2))]
    private Button _Command2;

    [AccessedThroughProperty(nameof(List1))]
    private ListBox _List1;

    [AccessedThroughProperty(nameof(Text1))]
    private TextBox _Text1;


#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [AccessedThroughProperty(nameof(Command1))]
    private ControlArray<Button> _Command1;

#pragma warning restore CS0618 // Typ oder Element ist veraltet

    private float _Find;

    private int _I;

    public Button _Command1_2;
    public RichTextBox RichTextBox1;
    public CheckBox _Check1_13;
    public CheckBox _Check1_12;
    public CheckBox _Check1_0;
    public CheckBox _Check1_1;
    public CheckBox _Check1_2;
    public CheckBox _Check1_3;
    public CheckBox _Check1_4;
    public CheckBox _Check1_5;
    public CheckBox _Check1_6;
    public CheckBox _Check1_7;
    public CheckBox _Check1_9;
    public CheckBox _Check1_10;
    public CheckBox _Check1_11;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual Button Command2
    {
        [DebuggerNonUserCode]
        get => _Command2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = _Command2_Click;
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

    public Label Line1;
    public Label _Label2_0;
    public Label _Label2_1;
    public GroupBox Frame1;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ListBox List1
    {
        [DebuggerNonUserCode]
        get => _List1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = _List1_DoubleClick;
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

    public Button _Command1_1;
    public Button _Command1_0;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual TextBox Text1
    {
        [DebuggerNonUserCode]
        get => _Text1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            KeyEventHandler value2 = _Text1_KeyUp;
            if (_Text1 != null)
            {
                _Text1.KeyUp -= value2;
            }
            _Text1 = value;
            if (_Text1 != null)
            {
                _Text1.KeyUp += value2;
            }
        }
    }

    public Label Label3;
    public Label _Label2_2;
    public Label _Label1_0;
    public Label _Label1_1;
    public Label _Label1_2;
    public ControlArray<CheckBox> Check1;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ControlArray<Button> Command1
    {
        [DebuggerNonUserCode]
        get => _Command1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler obj = _Command1_Click;
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

    public ControlArray<Label> Label1;
    public ControlArray<Label> Label2;
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    internal Label Label4;
    internal ProgressBar ProgressBar1;
    public Label Label5;
    public CheckBox CheckBox2;
    public CheckBox CheckBox1;
    private int Modul1_OrtNr;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= 512;
            return createParams;
        }
    }

    public Bemsuch()
    {
        Load += _Bemsuch_Load;
        Resize += _Bemsuch_Resize;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        _InitializeComponent();
    }

    protected override void Dispose(bool Disposing)
    {
        if (Disposing && _components != null)
        {
            _components.Dispose();
        }
        base.Dispose(Disposing);
    }

    private void _InitializeComponent()
    {
        _components = new Container();
        ToolTip1 = new ToolTip(_components);
        _Command1_2 = new Button();
        RichTextBox1 = new RichTextBox();
        Frame1 = new GroupBox();
        Label5 = new Label();
        CheckBox2 = new CheckBox();
        CheckBox1 = new CheckBox();
        _Check1_13 = new CheckBox();
        _Check1_12 = new CheckBox();
        _Check1_0 = new CheckBox();
        _Check1_1 = new CheckBox();
        _Check1_2 = new CheckBox();
        _Check1_3 = new CheckBox();
        _Check1_4 = new CheckBox();
        _Check1_5 = new CheckBox();
        _Check1_6 = new CheckBox();
        _Check1_7 = new CheckBox();
        _Check1_9 = new CheckBox();
        _Check1_10 = new CheckBox();
        _Check1_11 = new CheckBox();
        Command2 = new Button();
        Line1 = new Label();
        _Label2_0 = new Label();
        _Label2_1 = new Label();
        List1 = new ListBox();
        _Command1_1 = new Button();
        _Command1_0 = new Button();
        Text1 = new TextBox();
        Label3 = new Label();
        _Label2_2 = new Label();
        _Label1_0 = new Label();
        _Label1_1 = new Label();
        _Label1_2 = new Label();
        Check1 = new ControlArray<CheckBox>();
        Command1 = new ControlArray<Button>();
        Label1 = new ControlArray<Label>();
        Label2 = new ControlArray<Label>();
        Label4 = new Label();
        ProgressBar1 = new ProgressBar();
        Frame1.SuspendLayout();
        ((ISupportInitialize)Check1).BeginInit();
        ((ISupportInitialize)Command1).BeginInit();
        ((ISupportInitialize)Label1).BeginInit();
        ((ISupportInitialize)Label2).BeginInit();
        SuspendLayout();
        _Command1_2.BackColor = Color.Lime;
        _Command1_2.Cursor = Cursors.Default;
        _Command1_2.DialogResult = DialogResult.Cancel;
        _Command1_2.ForeColor = SystemColors.ControlText;
        _Command1_2.Location = new Point(783, 566);
        Command1.SetIndex(_Command1_2, 2);
        _Command1_2.Name = "_Command1_2";
        _Command1_2.RightToLeft = RightToLeft.No;
        _Command1_2.Size = new Size(145, 37);
        _Command1_2.TabIndex = 28;
        _Command1_2.Text = "Liste drucken";
        _Command1_2.UseVisualStyleBackColor = false;
        RichTextBox1.Font = new Font("Courier New", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        RichTextBox1.Location = new Point(423, 106);
        RichTextBox1.Name = "RichTextBox1";
        RichTextBox1.ScrollBars = RichTextBoxScrollBars.None;
        RichTextBox1.Size = new Size(455, 454);
        RichTextBox1.TabIndex = 27;
        RichTextBox1.Text = "";
        RichTextBox1.Visible = false;
        Frame1.BackColor = Color.FromArgb(192, 192, 255);
        Frame1.Controls.Add(Label5);
        Frame1.Controls.Add(CheckBox2);
        Frame1.Controls.Add(CheckBox1);
        Frame1.Controls.Add(_Check1_13);
        Frame1.Controls.Add(_Check1_12);
        Frame1.Controls.Add(_Check1_0);
        Frame1.Controls.Add(_Check1_1);
        Frame1.Controls.Add(_Check1_2);
        Frame1.Controls.Add(_Check1_3);
        Frame1.Controls.Add(_Check1_4);
        Frame1.Controls.Add(_Check1_5);
        Frame1.Controls.Add(_Check1_6);
        Frame1.Controls.Add(_Check1_7);
        Frame1.Controls.Add(_Check1_9);
        Frame1.Controls.Add(_Check1_10);
        Frame1.Controls.Add(_Check1_11);
        Frame1.Controls.Add(Command2);
        Frame1.Controls.Add(Line1);
        Frame1.Controls.Add(_Label2_0);
        Frame1.Controls.Add(_Label2_1);
        Frame1.ForeColor = SystemColors.ControlText;
        Frame1.Location = new Point(11, 58);
        Frame1.Name = "frmFamilyresidence";
        Frame1.RightToLeft = RightToLeft.No;
        Frame1.Size = new Size(314, 581);
        Frame1.TabIndex = 10;
        Frame1.TabStop = false;
        Label5.BackColor = SystemColors.Window;
        Label5.Location = new Point(0, 447);
        Label5.Name = "lblSorting";
        Label5.Size = new Size(310, 3);
        Label5.TabIndex = 34;
        CheckBox2.BackColor = SystemColors.Control;
        CheckBox2.Cursor = Cursors.Default;
        CheckBox2.ForeColor = SystemColors.ControlText;
        CheckBox2.Location = new Point(21, 485);
        CheckBox2.Name = "CheckBox2";
        CheckBox2.RightToLeft = RightToLeft.No;
        CheckBox2.Size = new Size(274, 21);
        CheckBox2.TabIndex = 33;
        CheckBox2.Text = "In der Quellenverwaltung";
        CheckBox2.UseVisualStyleBackColor = false;
        CheckBox1.BackColor = SystemColors.Control;
        CheckBox1.Cursor = Cursors.Default;
        CheckBox1.ForeColor = SystemColors.ControlText;
        CheckBox1.Location = new Point(21, 458);
        CheckBox1.Name = "cbxIllegitRel";
        CheckBox1.RightToLeft = RightToLeft.No;
        CheckBox1.Size = new Size(274, 21);
        CheckBox1.TabIndex = 32;
        CheckBox1.Text = "In der Ortsverwaltung";
        CheckBox1.UseVisualStyleBackColor = false;
        _Check1_13.BackColor = SystemColors.Control;
        _Check1_13.Cursor = Cursors.Default;
        _Check1_13.ForeColor = SystemColors.ControlText;
        _Check1_13.Location = new Point(21, 423);
        Check1.SetIndex(_Check1_13, 13);
        _Check1_13.Name = "_Check1_13";
        _Check1_13.RightToLeft = RightToLeft.No;
        _Check1_13.Size = new Size(274, 21);
        _Check1_13.TabIndex = 30;
        _Check1_13.Text = "Bei den Datumszeugen";
        _Check1_13.UseVisualStyleBackColor = false;
        _Check1_12.BackColor = SystemColors.Control;
        _Check1_12.Cursor = Cursors.Default;
        _Check1_12.ForeColor = SystemColors.ControlText;
        _Check1_12.Location = new Point(21, 222);
        Check1.SetIndex(_Check1_12, 12);
        _Check1_12.Name = "_Check1_12";
        _Check1_12.RightToLeft = RightToLeft.No;
        _Check1_12.Size = new Size(274, 21);
        _Check1_12.TabIndex = 29;
        _Check1_12.Text = "Bei den Datumszeugen";
        _Check1_12.UseVisualStyleBackColor = false;
        _Check1_0.BackColor = SystemColors.Control;
        _Check1_0.Cursor = Cursors.Default;
        _Check1_0.ForeColor = SystemColors.ControlText;
        _Check1_0.Location = new Point(21, 48);
        Check1.SetIndex(_Check1_0, 0);
        _Check1_0.Name = "_Check1_0";
        _Check1_0.RightToLeft = RightToLeft.No;
        _Check1_0.Size = new Size(274, 21);
        _Check1_0.TabIndex = 23;
        _Check1_0.Text = "In den Personenbemerkungen";
        _Check1_0.UseVisualStyleBackColor = false;
        _Check1_1.BackColor = SystemColors.Control;
        _Check1_1.Cursor = Cursors.Default;
        _Check1_1.ForeColor = SystemColors.ControlText;
        _Check1_1.Location = new Point(21, 77);
        Check1.SetIndex(_Check1_1, 1);
        _Check1_1.Name = "_Check1_1";
        _Check1_1.RightToLeft = RightToLeft.No;
        _Check1_1.Size = new Size(274, 21);
        _Check1_1.TabIndex = 22;
        _Check1_1.Text = "In den Personen-Quellen";
        _Check1_1.UseVisualStyleBackColor = false;
        _Check1_2.BackColor = SystemColors.Control;
        _Check1_2.Cursor = Cursors.Default;
        _Check1_2.ForeColor = SystemColors.ControlText;
        _Check1_2.Location = new Point(21, 106);
        Check1.SetIndex(_Check1_2, 2);
        _Check1_2.Name = "_Check1_2";
        _Check1_2.RightToLeft = RightToLeft.No;
        _Check1_2.Size = new Size(274, 21);
        _Check1_2.TabIndex = 21;
        _Check1_2.Text = "Bei den Paten";
        _Check1_2.UseVisualStyleBackColor = false;
        _Check1_3.BackColor = SystemColors.Control;
        _Check1_3.Cursor = Cursors.Default;
        _Check1_3.ForeColor = SystemColors.ControlText;
        _Check1_3.Location = new Point(21, 135);
        Check1.SetIndex(_Check1_3, 3);
        _Check1_3.Name = "_Check1_3";
        _Check1_3.RightToLeft = RightToLeft.No;
        _Check1_3.Size = new Size(274, 21);
        _Check1_3.TabIndex = 20;
        _Check1_3.Text = "In den oberen Datumsbemerkungen";
        _Check1_3.UseVisualStyleBackColor = false;
        _Check1_4.BackColor = SystemColors.Control;
        _Check1_4.Cursor = Cursors.Default;
        _Check1_4.ForeColor = SystemColors.ControlText;
        _Check1_4.Location = new Point(21, 166);
        Check1.SetIndex(_Check1_4, 4);
        _Check1_4.Name = "_Check1_4";
        _Check1_4.RightToLeft = RightToLeft.No;
        _Check1_4.Size = new Size(274, 21);
        _Check1_4.TabIndex = 19;
        _Check1_4.Text = "In den unteren Datumsbemerkungen";
        _Check1_4.UseVisualStyleBackColor = false;
        _Check1_5.BackColor = SystemColors.Control;
        _Check1_5.Cursor = Cursors.Default;
        _Check1_5.ForeColor = SystemColors.ControlText;
        _Check1_5.Location = new Point(21, 193);
        Check1.SetIndex(_Check1_5, 5);
        _Check1_5.Name = "_Check1_5";
        _Check1_5.RightToLeft = RightToLeft.No;
        _Check1_5.Size = new Size(274, 21);
        _Check1_5.TabIndex = 18;
        _Check1_5.Text = "In den Datumsquellen";
        _Check1_5.UseVisualStyleBackColor = false;
        _Check1_6.BackColor = SystemColors.Control;
        _Check1_6.Cursor = Cursors.Default;
        _Check1_6.ForeColor = SystemColors.ControlText;
        _Check1_6.Location = new Point(21, 280);
        Check1.SetIndex(_Check1_6, 6);
        _Check1_6.Name = "_Check1_6";
        _Check1_6.RightToLeft = RightToLeft.No;
        _Check1_6.Size = new Size(274, 21);
        _Check1_6.TabIndex = 17;
        _Check1_6.Text = "In den Familienbemerkungen";
        _Check1_6.UseVisualStyleBackColor = false;
        _Check1_7.BackColor = SystemColors.Control;
        _Check1_7.Cursor = Cursors.Default;
        _Check1_7.ForeColor = SystemColors.ControlText;
        _Check1_7.Location = new Point(21, 309);
        Check1.SetIndex(_Check1_7, 7);
        _Check1_7.Name = "_Check1_7";
        _Check1_7.RightToLeft = RightToLeft.No;
        _Check1_7.Size = new Size(274, 21);
        _Check1_7.TabIndex = 16;
        _Check1_7.Text = "In den Familien-Quellen";
        _Check1_7.UseVisualStyleBackColor = false;
        _Check1_9.BackColor = SystemColors.Control;
        _Check1_9.Cursor = Cursors.Default;
        _Check1_9.ForeColor = SystemColors.ControlText;
        _Check1_9.Location = new Point(21, 336);
        Check1.SetIndex(_Check1_9, 9);
        _Check1_9.Name = "_Check1_9";
        _Check1_9.RightToLeft = RightToLeft.No;
        _Check1_9.Size = new Size(274, 21);
        _Check1_9.TabIndex = 14;
        _Check1_9.Text = "In den oberen Datumsbemerkungen";
        _Check1_9.UseVisualStyleBackColor = false;
        _Check1_10.BackColor = SystemColors.Control;
        _Check1_10.Cursor = Cursors.Default;
        _Check1_10.ForeColor = SystemColors.ControlText;
        _Check1_10.Location = new Point(21, 365);
        Check1.SetIndex(_Check1_10, 10);
        _Check1_10.Name = "_Check1_10";
        _Check1_10.RightToLeft = RightToLeft.No;
        _Check1_10.Size = new Size(274, 21);
        _Check1_10.TabIndex = 13;
        _Check1_10.Text = "In den unteren Datumsbemerkungen";
        _Check1_10.UseVisualStyleBackColor = false;
        _Check1_11.BackColor = SystemColors.Control;
        _Check1_11.Cursor = Cursors.Default;
        _Check1_11.ForeColor = SystemColors.ControlText;
        _Check1_11.Location = new Point(21, 394);
        Check1.SetIndex(_Check1_11, 11);
        _Check1_11.Name = "_Check1_11";
        _Check1_11.RightToLeft = RightToLeft.No;
        _Check1_11.Size = new Size(274, 21);
        _Check1_11.TabIndex = 12;
        _Check1_11.Text = "In den Datumsquellen";
        _Check1_11.UseVisualStyleBackColor = false;
        Command2.BackColor = SystemColors.Control;
        Command2.Cursor = Cursors.Default;
        Command2.ForeColor = SystemColors.ControlText;
        Command2.Location = new Point(206, 548);
        Command2.Name = "btnNew";
        Command2.RightToLeft = RightToLeft.No;
        Command2.Size = new Size(102, 30);
        Command2.TabIndex = 11;
        Command2.Text = "alle ein";
        Command2.UseVisualStyleBackColor = false;
        Line1.BackColor = SystemColors.Window;
        Line1.Location = new Point(0, 246);
        Line1.Name = "Line1";
        Line1.Size = new Size(310, 3);
        Line1.TabIndex = 31;
        _Label2_0.BackColor = SystemColors.Control;
        _Label2_0.Cursor = Cursors.Default;
        _Label2_0.ForeColor = SystemColors.ControlText;
        _Label2_0.Location = new Point(21, 19);
        Label2.SetIndex(_Label2_0, 0);
        _Label2_0.Name = "lblSep2";
        _Label2_0.RightToLeft = RightToLeft.No;
        _Label2_0.Size = new Size(274, 21);
        _Label2_0.TabIndex = 25;
        _Label2_0.Text = "Suche nach Texten bei Personen";
        _Label2_1.BackColor = SystemColors.Control;
        _Label2_1.Cursor = Cursors.Default;
        _Label2_1.ForeColor = SystemColors.ControlText;
        _Label2_1.Location = new Point(21, 251);
        Label2.SetIndex(_Label2_1, 1);
        _Label2_1.Name = "lblSep1";
        _Label2_1.RightToLeft = RightToLeft.No;
        _Label2_1.Size = new Size(274, 21);
        _Label2_1.TabIndex = 24;
        _Label2_1.Text = "Suche nach Texten bei Familien";
        List1.BackColor = SystemColors.Window;
        List1.Cursor = Cursors.Default;
        List1.ForeColor = SystemColors.WindowText;
        List1.ItemHeight = 17;
        List1.Location = new Point(435, 77);
        List1.Name = "List1";
        List1.RightToLeft = RightToLeft.No;
        List1.Size = new Size(471, 480);
        List1.Sorted = true;
        List1.TabIndex = 6;
        _Command1_1.BackColor = Color.Lime;
        _Command1_1.Cursor = Cursors.Default;
        _Command1_1.ForeColor = SystemColors.ControlText;
        _Command1_1.Location = new Point(264, 683);
        Command1.SetIndex(_Command1_1, 1);
        _Command1_1.Name = "btnCancel";
        _Command1_1.RightToLeft = RightToLeft.No;
        _Command1_1.Size = new Size(127, 22);
        _Command1_1.TabIndex = 5;
        _Command1_1.Text = "Suche starten";
        _Command1_1.UseVisualStyleBackColor = false;
        _Command1_0.BackColor = Color.Lime;
        _Command1_0.Cursor = Cursors.Default;
        _Command1_0.ForeColor = SystemColors.ControlText;
        _Command1_0.Location = new Point(816, 674);
        Command1.SetIndex(_Command1_0, 0);
        _Command1_0.Name = "btnVerify";
        _Command1_0.RightToLeft = RightToLeft.No;
        _Command1_0.Size = new Size(102, 31);
        _Command1_0.TabIndex = 4;
        _Command1_0.UseVisualStyleBackColor = false;
        Text1.AcceptsReturn = true;
        Text1.BackColor = SystemColors.Window;
        Text1.Cursor = Cursors.IBeam;
        Text1.ForeColor = SystemColors.WindowText;
        Text1.Location = new Point(12, 680);
        Text1.MaxLength = 0;
        Text1.Name = "Text1";
        Text1.RightToLeft = RightToLeft.No;
        Text1.Size = new Size(241, 25);
        Text1.TabIndex = 3;
        Label3.AutoSize = true;
        Label3.BackColor = Color.FromArgb(255, 255, 192);
        Label3.Cursor = Cursors.Default;
        Label3.ForeColor = SystemColors.ControlText;
        Label3.Location = new Point(5, 57);
        Label3.Name = "lblDisplayHint";
        Label3.RightToLeft = RightToLeft.No;
        Label3.Size = new Size(0, 17);
        Label3.TabIndex = 26;
        _Label2_2.BackColor = SystemColors.Control;
        _Label2_2.Cursor = Cursors.Default;
        _Label2_2.ForeColor = SystemColors.ControlText;
        _Label2_2.Location = new Point(12, 661);
        Label2.SetIndex(_Label2_2, 2);
        _Label2_2.Name = "_Label2_2";
        _Label2_2.RightToLeft = RightToLeft.No;
        _Label2_2.Size = new Size(259, 16);
        _Label2_2.TabIndex = 7;
        _Label2_2.Text = "Gesuchter Text";
        _Label1_0.BackColor = Color.Red;
        _Label1_0.Cursor = Cursors.Default;
        _Label1_0.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_0.ForeColor = Color.Yellow;
        _Label1_0.Location = new Point(0, 0);
        Label1.SetIndex(_Label1_0, 0);
        _Label1_0.Name = "_Label1_0";
        _Label1_0.RightToLeft = RightToLeft.No;
        _Label1_0.Size = new Size(918, 17);
        _Label1_0.TabIndex = 2;
        _Label1_0.Text = "GEN_PLUS Das Genealogieprogramm mit den Pluspunkten";
        _Label1_0.TextAlign = ContentAlignment.TopCenter;
        _Label1_1.BackColor = Color.Red;
        _Label1_1.Cursor = Cursors.Default;
        _Label1_1.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_1.ForeColor = Color.Yellow;
        _Label1_1.Location = new Point(0, 19);
        Label1.SetIndex(_Label1_1, 1);
        _Label1_1.Name = "_Label1_1";
        _Label1_1.RightToLeft = RightToLeft.No;
        _Label1_1.Size = new Size(918, 17);
        _Label1_1.TabIndex = 1;
        _Label1_1.TextAlign = ContentAlignment.TopCenter;
        _Label1_2.BackColor = Color.Red;
        _Label1_2.Cursor = Cursors.Default;
        _Label1_2.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_2.ForeColor = Color.Yellow;
        _Label1_2.Location = new Point(0, 38);
        Label1.SetIndex(_Label1_2, 2);
        _Label1_2.Name = "_Label1_2";
        _Label1_2.RightToLeft = RightToLeft.No;
        _Label1_2.Size = new Size(918, 17);
        _Label1_2.TabIndex = 0;
        _Label1_2.TextAlign = ContentAlignment.TopCenter;
        Label4.Location = new Point(357, 618);
        Label4.Name = "lblSearch";
        Label4.Size = new Size(406, 21);
        Label4.TabIndex = 29;
        Label4.Text = "lblSearch";
        ProgressBar1.BackColor = Color.FromArgb(255, 255, 192);
        ProgressBar1.ForeColor = Color.Fuchsia;
        ProgressBar1.Location = new Point(11, 642);
        ProgressBar1.Name = "ProgressBar1";
        ProgressBar1.Size = new Size(924, 16);
        ProgressBar1.TabIndex = 31;
        AutoScaleDimensions = new SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        CancelButton = _Command1_2;
        ClientSize = new Size(1018, 725);
        Controls.Add(List1);
        Controls.Add(ProgressBar1);
        Controls.Add(Label4);
        Controls.Add(_Command1_2);
        Controls.Add(RichTextBox1);
        Controls.Add(Frame1);
        Controls.Add(_Command1_1);
        Controls.Add(_Command1_0);
        Controls.Add(Text1);
        Controls.Add(Label3);
        Controls.Add(_Label2_2);
        Controls.Add(_Label1_0);
        Controls.Add(_Label1_1);
        Controls.Add(_Label1_2);
        Cursor = Cursors.Default;
        Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Name = "Bemsuch";
        RightToLeft = RightToLeft.No;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.Manual;
        Text = "Bemerkungen durchsuchen";
        WindowState = FormWindowState.Maximized;
        Frame1.ResumeLayout(false);
        ((ISupportInitialize)Check1).EndInit();
        ((ISupportInitialize)Command1).EndInit();
        ((ISupportInitialize)Label1).EndInit();
        ((ISupportInitialize)Label2).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private void _Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_071e, IL_0d9c, IL_14d4, IL_1bc9, IL_30a0, IL_31d9
        int try0001_dispatch = -1;
        int num = default;
        short index = default;
        int num2 = default;
        int num3 = default;
        int num4 = default;
        int num6 = default;
        int num7 = default;
        string source = default;
        string destination = default;
        int lErl = default;
        string text = default;
        int num10 = default;
        int num5;
        object[] array2;
        object[] arguments;
        object obj;
        object obj9;
        short Schalt;
        object[] array;
        // Field family_sBem1;
        object[] array3;
        bool[] array4;
        switch (try0001_dispatch)
        {
            default:
                num = 1;
                ProjectData.ClearProjectError();
                index = (short)Command1.GetIndex((Button)eventSender);
                num3 = 2;
                switch (index)
                {
                    case 0:
                        Command1_0_Click(eventSender, eventArgs);
                        break;
                    case 1:
                        goto IL_00e0;
                    case 2:
                        goto IL_2e64;
                    default:
                        Command1_3_Click(eventSender, eventArgs);
                        break;
                }
                goto end_IL_0001_2;
            IL_00e0:
                num = 18;
                Command1[0].Enabled = false;
                if ((Text1.Text == "") || (Text1.Text == " "))
                {
                    goto end_IL_0001_2;
                }
                ProjectData.ClearProjectError();
                num3 = 3;
                RichTextBox1.Text = "";
                List1.Items.Clear();
                if (List1.Items.Count == 0)
                {
                    Command1[2].Enabled = false;
                }
                List1.Refresh();
                Refresh();
                source = Modul1.InitDir + "NUMTEMP.mdb";
                destination = Modul1.TempPath + "\\NumTemp.mdb";
                DataModul.ReplaceNBDatafile(destination, source, null, true);
                Modul1.Dateienopen();
                List1.Items.Clear();
                ProjectData.ClearProjectError();
                num3 = 4;
                if ((Check1[0].Checked) || (Check1[1].Checked) || (Check1[2].Checked))
                {
                    ProgressBar1.Minimum = 0;
                    ProgressBar1.Maximum = 0;
                    ProgressBar1.Step = 1;
                    ProgressBar1.Maximum = DataModul.Person.Count;
                    foreach (var cPers in DataModul.Person.ReadAll())
                    {
                        ProgressBar1.PerformStep();
                        _Find = 0f;
                        Modul1.PersInArb = cPers.ID;
                        if (Check1[0].Checked
                            && Strings.InStr(cPers.sBem[1].ToUpper(), Text1.Text.ToUpper()) != 0
                            || Check1[1].Checked
                            && Strings.InStr(cPers.sBem[3].ToUpper(), Text1.Text.ToUpper()) != 0
                            || Check1[2].Checked
                            && Strings.InStr(cPers.sBem[2].ToUpper(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                            lErl = 1;
                            ProjectData.ClearProjectError();
                            num3 = 5;
                            FrauTab_Append(Modul1.PersInArb);
                        }
                        lErl = 11;
                        Application.DoEvents();
                        Label4.Text = "Suche bei den Personen ";
                    }
                }
                if (Check1[3].Checked || Check1[4].Checked || Check1[5].Checked || Check1[12].Checked)
                {
                    ProgressBar1.Minimum = 0;
                    ProgressBar1.Maximum = 0;
                    ProgressBar1.Maximum = DataModul.Event.Count;
                    ProgressBar1.Step = 1;
                    foreach (var cEvt2 in DataModul.Event.ReadAll())
                    {
                        ProgressBar1.PerformStep();
                        _Find = 0f;
                        if (cEvt2.eArt <= EEventArt.eA_499)
                        {
                            Modul1.PersInArb = cEvt2.iPerFamNr;
                            string sUpperTxt = Text1.Text.ToUpper();
                            if ((Check1[3].Checked
                                && cEvt2.sBem[1].ToUpper().Contains(sUpperTxt))
                                || (Check1[4].Checked
                                && cEvt2.sBem[2].ToUpper().Contains(sUpperTxt))
                                || (Check1[5].Checked
                                && cEvt2.sBem[3].ToUpper().Contains(sUpperTxt))
                                || (Check1[12].Checked
                                && cEvt2.sBem[4].ToUpper().Contains(sUpperTxt)))
                            {
                                _Find = 1f;
                                FrauTab_Append(Modul1.PersInArb);
                            }

                            Label4.Text = "Suche in den Datumsfeldern bei Personen";
                            continue;
                        }
                        lErl = 2;
                        ProjectData.ClearProjectError();
                        num3 = 6;
                        if (_Find == 1f)
                            FrauTab_Append(Modul1.PersInArb);
                        lErl = 31;
                        Application.DoEvents();
                        Label4.Text = "Suche in den Datumsfeldern bei Personen";
                    }
                }
                List1.Items.Clear();
                DataModul.NB_FrauTable.MoveFirst();
                Command1[1].Enabled = true;
                if (List1.Items.Count > 0)
                {
                    Command1[2].Enabled = true;
                }
                goto IL_108d;
            IL_108d: // <========== 3
                num = 169;
                if (!DataModul.NB_FrauTable.EOF)
                {
                    Label4.Text = "Aufbau der Ergebnisliste";
                    ProgressBar1.PerformStep();
                    if (DataModul.NB_FrauTable.Fields["PNr"].AsInt() > 0)
                    {
                        Application.DoEvents();
                        Modul1.PersInArb = DataModul.NB_FrauTable.Fields["PNr"].AsInt();
                        Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                        //                                Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.o01_Person.iPerFamNr, out int iAhn, out Modul1.Kont[20]);
                        //                                Modul1.Kont[97] = iAhn.AsString();
                        _ = List1.Items.Add(new ListItem($"  {Modul1.Person.SurName},{Modul1.Person.Givennames}", Modul1.PersInArb));
                    }
                    DataModul.NB_FrauTable.MoveNext();
                    goto IL_108d;
                }
                else
                {
                    List1.Visible = true;
                    destination = Modul1.TempPath + "\\NumTemp.mdb";
                    source = Modul1.InitDir + "NUMTEMP.mdb";
                    DataModul.ReplaceNBDatafile(destination, source, () => Command1[1].Enabled = false, true);
                    if ((Check1[6].Checked) || (Check1[7].Checked))
                    {
                        DataModul.DB_FamilyTable.MoveFirst();
                        ProgressBar1.Minimum = 0;
                        ProgressBar1.Maximum = 0;
                        ProgressBar1.Maximum = DataModul.DB_FamilyTable.RecordCount;
                        ProgressBar1.Step = 1;
                        goto IL_1626;
                    }
                }
                goto IL_1644;
            IL_14d5: // <========== 3
                num = 219;
                lErl = 3;
                ProjectData.ClearProjectError();
                num3 = 7;
                if (_Find == 1f)
                {
                    FrauTab_Append(Modul1.FamInArb);
                }
                goto IL_15e3;
            IL_15e3: // <========== 3
                num = 230;
                lErl = 23;
                DataModul.DB_FamilyTable.MoveNext();
                Application.DoEvents();
                Label4.Text = "Suche bei den Familien ";
                goto IL_1626;
            IL_1626: // <========== 3
                num = 200;
                if (!DataModul.DB_FamilyTable.EOF)
                {
                    string family_sBem1 = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem1].AsString();
                    string Family_sBem3 = DataModul.DB_FamilyTable.Fields[FamilyFields.Bem3].AsString();
                    ProgressBar1.PerformStep();
                    _Find = 0f;
                    Modul1.FamInArb = DataModul.DB_FamilyTable.Fields[FamilyFields.FamNr].AsInt();
                    if (Check1[6].Checked 
                        && family_sBem1.ToUpper().Contains(Text1.Text.ToUpper()))
                    {
                        _Find = 1f;
                    }
                    else if (Check1[7].Checked 
                        && Family_sBem3.Trim() != "" 
                        && Family_sBem3.ToUpper().Contains(Text1.Text.ToUpper()))
                    {
                        _Find = 1f;
                    }
                    else goto IL_15e3;
                        goto IL_14d5;
                }
                goto IL_1644;
            IL_1644: // <========== 3
                num = 236;
                if (((0u - (((Check1[9].Checked) | (Check1[10].Checked) | (Check1[11].Checked)) ? 1u : 0u)) | (uint)Check1[13].CheckState) != 0)
                {
                    ProgressBar1.Minimum = 0;
                    ProgressBar1.Maximum = 0;
                    ProgressBar1.Maximum = DataModul.Event.Count;
                    ProgressBar1.Step = 1;
                    goto IL_1d0e;
                }
                goto IL_1d2d;
            IL_1d0e: // <========== 3
                num = 243;
                while (DataModul.Event.ReadData(0, 0, out var cEvt))
                {
                    ProgressBar1.PerformStep();
                    Application.DoEvents();
                    _Find = 0f;
                    if (cEvt.eArt >= EEventArt.eA_500)
                    {
                        Modul1.FamInArb = cEvt.iPerFamNr;
                        if (Check1[9].Checked 
                            && Strings.InStr(cEvt.sBem[1].AsString().ToUpper(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                        }
                        if ("" != cEvt.sBem[2] && Check1[10].Checked && Strings.InStr(cEvt.sBem[2].ToUpper(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                        }
                        else if ("" != cEvt.sBem[3]
                            && Check1[11].Checked
                            && Strings.InStr(cEvt.sBem[3].ToUpper(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                        }
                        else if ("" != cEvt.sBem[4]
                            && Check1[13].Checked
                            && Strings.InStr(cEvt.sBem[4].ToUpper(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                        }
                        else
                        { 
                            Label4.Text = "Suche in den Datumsfeldern der Familien";
                            continue;
                        }
                    }
                    lErl = 4;
                    ProjectData.ClearProjectError();
                    num3 = 8;
                    if (_Find == 1f)
                    {
                        FrauTab_Append(Modul1.FamInArb);
                        lErl = 44;
                    }
                    Label4.Text = "Suche in den Datumsfeldern der Familien";
                }
                goto IL_1d2d;
            IL_1d2d: // <========== 3
                num = 298;
                lErl = 8;
                if (DataModul.NB_FrauTable.RecordCount > 0)
                {
                    DataModul.NB_FrauTable.MoveFirst();
                    List1.Visible = false;
                    ProgressBar1.Minimum = 0;
                    ProgressBar1.Maximum = 0;
                    ProgressBar1.Maximum = DataModul.NB_FrauTable.RecordCount;
                    ProgressBar1.Step = 1;
                    while (!DataModul.NB_FrauTable.EOF)
                    {
                        Label4.Text = "Aufbau der Ergebnisliste";
                        ProgressBar1.PerformStep();
                        if (DataModul.NB_FrauTable.Fields["pNr"].AsInt() > 0)
                        {
                            Modul1.FamInArb = DataModul.NB_FrauTable.Fields["pNr"].AsInt();
                            DataModul.Link.ReadFamily(Modul1.FamInArb, Modul1.Family);
                            if (Modul1.Family.Mann > 0)
                            {
                                Modul1.PersInArb = Modul1.Family.Mann;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                //                                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.o01_Person.iPerFamNr, out int iAhn, out Modul1.Kont[20]);
                                //                                    Modul1.Kont[97] = iAhn.AsString();
                                //if (Modul1.o01_Person.Prefix != "")
                                //{
                                //    Modul1.o01_Person.Prefix = Modul1.o01_Person.Prefix + " ";
                                //}
                                text = "F  " + Modul1.Person.SurName + " " + Modul1.Person.Givennames + "  / ";
                            }
                            else
                            {
                                text = "F  " + Modul1.IText[EUserText.tUnknown] + " / ";
                            }
                            if (Modul1.Family.Frau > 0)
                            {
                                Modul1.PersInArb = Modul1.Family.Frau;
                                Modul1.Person_ReadNames(Modul1.PersInArb, Modul1.Person);
                                //                                    Modul1.Kont[10] = Modul1.Ancesters_GetPersonData(Modul1.o01_Person.iPerFamNr, out int iAhn, out Modul1.Kont[20]);
                                //                                    Modul1.Kont[97] = iAhn.AsString();
                                //if (Modul1.o01_Person.Prefix != "")
                                //{
                                //    Modul1.o01_Person.Prefix = Modul1.o01_Person.Prefix + " ";
                                //}
                                text = text.TrimEnd() + Modul1.Person.SurName + " " + Modul1.Person.Givennames;
                            }
                            else
                            {
                                text = text.TrimEnd() + Modul1.IText[EUserText.tUnknown];
                            }
                            _ = List1.Items.Add(new ListItem(text, Modul1.FamInArb));
                            text = "";
                        }
                        DataModul.NB_FrauTable.MoveNext();
                    }
                }
                List1.Visible = true;
                source = Modul1.InitDir + "NUMTEMP.mdb";
                destination = Modul1.TempPath + "\\NumTemp.mdb";
                DataModul.ReplaceNBDatafile(destination, source, () => Command1[1].Enabled = false, true);
                if (CheckBox1.Checked)
                {
                    ProgressBar1.Minimum = 0;
                    ProgressBar1.Maximum = 0;
                    ProgressBar1.Step = 1;
                    ProgressBar1.Maximum = DataModul.Place.Count;
                    foreach (var cPlace in DataModul.Place.ReadAll())
                    {
                        ProgressBar1.PerformStep();
                        _Find = 0f;
                        if (Strings.InStr(cPlace.sBem.ToUpper(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                            Application.DoEvents();
                        }
                        ProjectData.ClearProjectError();
                        num3 = 9;
                        if (_Find == 1f)
                        {
                            DataModul_NBFrau_CondAdd(cPlace.iOrt);
                        }
                        Application.DoEvents();
                        Label4.Text = "Suche in den Orten ";
                    }

                    if (DataModul.NB_FrauTable.RecordCount > 0)
                    {
                        DataModul.NB_FrauTable.MoveFirst();
                        List1.Visible = false;
                        ProgressBar1.Minimum = 0;
                        ProgressBar1.Maximum = 0;
                        ProgressBar1.Maximum = DataModul.NB_FrauTable.RecordCount;
                        ProgressBar1.Step = 1;
                        while (!DataModul.NB_FrauTable.EOF)
                        {
                            Label4.Text = "Aufbau der Ergebnisliste";
                            if (DataModul.NB_FrauTable.Fields["pNr"].AsInt() > 0)
                            {
                                Modul1_OrtNr = DataModul.NB_FrauTable.Fields["pNr"].AsInt();
                                if (DataModul.Place.ReadData(Modul1_OrtNr, out var cPlace))
                                {
                                    text = "O  " + DataModul.Place.FullName(cPlace);
                                    _ = List1.Items.Add(new ListItem<int>(text, Modul1_OrtNr));
                                }
                            }
                            DataModul.NB_FrauTable.MoveNext();
                        }
                    }

                }
                List1.Visible = true;
                source = Modul1.InitDir + "NUMTEMP.mdb";
                destination = Modul1.TempPath + "\\NumTemp.mdb";
                DataModul.ReplaceNBDatafile(destination, source, () => Command1[1].Enabled = false, true);
                if (CheckBox2.Checked)
                {
                    DataModul.DB_QuTable.MoveFirst();
                    ProgressBar1.Minimum = 0;
                    ProgressBar1.Maximum = 0;
                    ProgressBar1.Step = 1;
                    ProgressBar1.Maximum = DataModul.DB_QuTable.RecordCount;
                    DataModul.DB_QuTable.Index = "NR";
                    DataModul.DB_QuTable.Seek("=", 1);
                    while (!DataModul.DB_QuTable.EOF)
                    {
                        ProgressBar1.PerformStep();
                        _Find = 0f;
                        num4 = DataModul.DB_QuTable.Fields[QuFields._1].AsInt();

                        array = new object[1];
                        array2 = array;
                        var field = DataModul.DB_QuTable.Fields[QuFields._13];
                        array2[0] = field.Value;
                        array3 = array;
                        arguments = array3;
                        array4 = new bool[1]
                        {
                            true
                        }
                        ;
                        obj = field.AsString().ToUpper();
                        if (array4[0])
                        {
                            field.Value = array3[0];
                        }
                        if (Strings.InStr(obj.AsString(), Text1.Text.ToUpper()) != 0)
                        {
                            _Find = 1f;
                            Application.DoEvents();
                        }
                        ProjectData.ClearProjectError();
                        num3 = 10;
                        if (_Find == 1f)
                        {
                            DataModul.NB_FrauTable.Index = "Nr";
                            DataModul.NB_FrauTable.Seek("=", num4);
                            if (DataModul.NB_FrauTable.NoMatch)
                            {
                                DataModul.NB_FrauTable.AddNew();
                                DataModul.NB_FrauTable.Fields["pNr"].Value = num4;
                                DataModul.NB_FrauTable.Update();
                            }
                        }
                        Application.DoEvents();
                        Label4.Text = "Suche in den Quellen ";
                        DataModul.DB_QuTable.MoveNext();
                    }
                    if (DataModul.NB_FrauTable.RecordCount > 0)
                    {
                        DataModul.NB_FrauTable.MoveFirst();
                        List1.Visible = false;
                        ProgressBar1.Minimum = 0;
                        ProgressBar1.Maximum = 0;
                        ProgressBar1.Maximum = DataModul.NB_FrauTable.RecordCount;
                        ProgressBar1.Step = 1;
                        while (!DataModul.NB_FrauTable.EOF)
                        {
                            Label4.Text = "Aufbau der Ergebnisliste";
                            if (DataModul.NB_FrauTable.Fields["pNr"].AsInt() > 0)
                            {
                                DataModul.DB_QuTable.Index = "NR";
                                DataModul.DB_QuTable.Seek("=", DataModul.NB_FrauTable.Fields["pNr"].Value);
                                text = (("Q  ") + (DataModul.DB_QuTable.Fields[QuFields._2].Value)).AsString();
                                _ = List1.Items.Add(new ListItem(text, DataModul.DB_QuTable.Fields[QuFields._1].AsInt()));
                                text = "";
                            }
                            DataModul.NB_FrauTable.MoveNext();

                        }
                    }
                    else
                    {
                        Label3.Text = "Fertig, " + DataModul.NB_FrauTable.RecordCount.AsString() + " Fundstellen";
                    }

                }
                List1.Visible = true;
                Command1[1].Enabled = true;
                if (List1.Items.Count > 0)
                {
                    Command1[2].Enabled = true;
                }
                Label4.Text = "Fertig, " + List1.Items.Count.AsString() + " Fundstellen";
                Command1[0].Enabled = true;
                DataModul.NB.Close();
                goto end_IL_0001_2;
            //===============================================================
            IL_2e64:
                num = 487;
                RichTextBox1.Visible = true;
                num6 = checked(List1.Items.Count - 1);
                num7 = 0;
                while (num7 <= num6)
                {
                    List1.SelectedIndex = num7;
                    RichTextBox1.SelectedText = "  " + List1.Text.Trim() + new string(' ', 80).Left(55);
                    Modul1.PersInArb = List1.Items.ItemData<int>(List1.SelectedIndex);
                    RichTextBox1.SelectedText = "          " + Modul1.PersInArb.AsString().Right(10) + "\n";
                    num7 = checked(num7 + 1);
                }
                if (Modul1.Typ == DriveType.CDRom)
                {
                    RichTextBox1.SaveFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RichTextBox1.LoadFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                }
                else
                {
                    RichTextBox1.SaveFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RichTextBox1.LoadFile(Modul1.TempPath + "\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RichTextBox1.SaveFile(Modul1.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                    RichTextBox1.LoadFile(Modul1.GenFreeDir + "\\Temp\\Text2.RTF", RichTextBoxStreamType.RichText);
                }
                Modul1.Ausdruck("\\Text2.RTF");
                RichTextBox1.Visible = false;
                DataModul.NB.Close();
                goto end_IL_0001_2;
        }
    end_IL_0001_2: // <========== 3
        return;
    }

    private static void DataModul_NBFrau_CondAdd(int num10)
    {
        DataModul.NB_FrauTable.Index = "Nr";
        DataModul.NB_FrauTable.Seek("=", num10);
        if (DataModul.NB_FrauTable.NoMatch)
        {
            DataModul.NB_FrauTable.AddNew();
            DataModul.NB_FrauTable.Fields["pNr"].Value = num10;
            DataModul.NB_FrauTable.Update();
        }
    }

    private static void Command1_3_Click(object eventSender, EventArgs eventArgs)
    {
        DataModul.NB.Close();
    }

    private void FrauTab_Append(int iPersNr)
    {

        {
            IRecordset nB_FrauTable = DataModul.NB_FrauTable;
            nB_FrauTable.Index = "Nr";
            nB_FrauTable.Seek("=", iPersNr);
            if (nB_FrauTable.NoMatch)
            {
                nB_FrauTable.AddNew();
                nB_FrauTable.Fields["PNr"].Value = iPersNr;
                nB_FrauTable.Update();
            }
        }
    }

    private void Command1_0_Click(object eventSender, EventArgs eventArgs)
    {
        RichTextBox1.Text = "";
        List1.Items.Clear();
        Text1.Text = "";
        Command1[1].Enabled = true;
        DataModul.NB.Close();
        DataModul.WB.Close();
        Hide();
        Close();
        Menue.Default.Show();
        DataModul.NB.Close();
    }

    private void _Command2_Click(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            if (Command2.Text == "alle ein")
            {
                _I = 0;
                int i;
                int num;
                do
                {
                    Check1[(short)_I].CheckState = CheckState.Checked;
                    _I++;
                    i = _I;
                    num = 7;
                }
                while (i <= num);
                _I = 9;
                int i2;
                do
                {
                    Check1[(short)_I].CheckState = CheckState.Checked;
                    _I++;
                    i2 = _I;
                    num = 13;
                }
                while (i2 <= num);
                CheckBox1.CheckState = CheckState.Checked;
                CheckBox2.CheckState = CheckState.Checked;
                Command2.Text = "alle aus";
                _ = Text1.Focus();
            }
            else
            {
                _I = 0;
                int i3;
                int num;
                do
                {
                    Check1[(short)_I].CheckState = CheckState.Unchecked;
                    _I++;
                    i3 = _I;
                    num = 7;
                }
                while (i3 <= num);
                _I = 9;
                int i4;
                do
                {
                    Check1[(short)_I].CheckState = CheckState.Unchecked;
                    _I++;
                    i4 = _I;
                    num = 13;
                }
                while (i4 <= num);
                CheckBox1.CheckState = CheckState.Unchecked;
                CheckBox2.CheckState = CheckState.Unchecked;
                Command2.Text = "alle ein";
                _ = Text1.Focus();
            }
        }
    }

    private void _Bemsuch_Load(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0399, IL_03b8
        int try0001_dispatch = -1;
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
                int farb2 = 0;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 1167:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_03bb;
                                default:
                                    goto end_IL_0001;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            goto IL_03bb;
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        if (Modul1.FontSize > 0f)
                        {
                            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                            _Label1_0.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
                            _Label1_1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
                            _Label1_2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Bold);
                        }
                        Command1[0].Text = Modul1.IText[EUserText.t158];
                        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
                        WindowState = WiS;
                        var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
                        Left = aiPos[0];
                        Top = aiPos[1];
                        Modul1.HintFarb = ColorTranslator.FromOle(12632265);
                        RichTextBox1.Text = "";
                        var cls = Modul1.Persistence.ReadFarbenInit("Farb.dat", 3);
                        Modul1.HintFarb = cls[1];

                        _Label1_1.Text = Modul1.Version1;
                        _Label1_2.Text = "Version 24.09.01 Stand 20.07.2018";
                        Label3.Text = "Mandant: " + Modul1.Verz;
                        if (List1.Items.Count == 0)
                        {
                            Command1[2].Enabled = false;
                        }
                        ProgressBar1.Maximum = 0;
                        Label4.Text = "";
                        Command1[1].Enabled = true;
                        goto end_IL_0001_2;
                    IL_03bb:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1167;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void _Bemsuch_Resize(object eventSender, EventArgs eventArgs)
    {
        _Label1_0.Width = Width;
        _Label1_1.Width = Width;
        _Label1_2.Width = Width;
    }

    private void _List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        Modul1.FamInArb = 0;
        Modul1.PersInArb = 0;
        var sListKenn = List1.SelectedItem.ItemData<(char, int)>().Item1;
        var sListID = List1.SelectedItem.ItemData<(char, int)>().Item2;
        if (sListKenn == 'F')
        {
            Familie.Default.Show(sListID);
        }
        else if (sListKenn == 'O')
        {
            Modul1.Schalt = (byte)List1.Items.ItemData<int>(List1.SelectedIndex);
            MainProject.Forms.Ortsver.Button10.Text = Modul1.IText[EUserText.tNMBack];
            _ = MainProject.Forms.Ortsver.ShowDialog();
        }
        else if (sListKenn == 'Q')
        {
            MainProject.Forms.Quellverw.ACommand1[6].Visible = true;
            _ = MainProject.Forms.Quellverw.ShowDialog(List1.Items.ItemData<int>(List1.SelectedIndex));
        }
        else
        {
            Modul1.PersInArb = List1.Items.ItemData<int>(List1.SelectedIndex);
            Modul1.Aend = 0f;
            Modul1.Ad = false;
            Personen.Default.Show(Modul1.PersInArb, EUserText.tNMBack);
        }
    }

    private void _Text1_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Command1[1].PerformClick();
        }
    }
}
