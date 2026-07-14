using BaseLib.Helper;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Gen_FreeWin;

[DesignerGenerated]
internal class FunkT : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    private IContainer _components;
    IModul1 Modul1 => _Modul1.Instance;

    public ToolTip ToolTip1;

    [AccessedThroughProperty(nameof(_Label1_0))]
    private Label __Label1_0;
#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [AccessedThroughProperty(nameof(Command1))]
    private ControlArray<Button> _Command1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet


    private float _I;

    public TextBox _Text1_2;
    public TextBox _Text1_10;
    public TextBox _Text1_9;
    public TextBox _Text1_8;
    public TextBox _Text1_7;
    public Button _Command1_1;
    public Button _Command1_0;
    public TextBox _Text1_6;
    public TextBox _Text1_5;
    public TextBox _Text1_4;
    public TextBox _Text1_3;
    public TextBox _Text1_1;
    public TextBox _Text1_0;
    public Label _Label1_3;
    public Label _Label1_14;
    public Label _Label1_13;
    public Label _Label1_12;
    public Label _Label1_11;
    public Label _Label1_10;
    public Label _Label1_9;
    public Label _Label1_8;
    public Label _Label1_7;
    public Label _Label1_6;
    public Label _Label1_5;
    public Label _Label1_4;
    public Label _Label1_2;
    public Label _Label1_1;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual Label _Label1_0
    {
        [DebuggerNonUserCode]
        get => __Label1_0;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = _Label1_0_Click;
            if (__Label1_0 != null)
            {
                __Label1_0.Click -= value2;
            }
            __Label1_0 = value;
            if (__Label1_0 != null)
            {
                __Label1_0.Click += value2;
            }
        }
    }
#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#pragma warning disable CS0618 // Typ oder Element ist veraltet

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
    public ControlArray<TextBox> Text1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    [DebuggerNonUserCode]
    public FunkT()
    {
        Load += _FunkT_Load;
        FormClosing += _FunkT_FormClosing;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        _InitializeComponent();
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && _components != null)
        {
            _components.Dispose();
        }
        base.Dispose(Disposing);
    }

    [DebuggerStepThrough]
    private void _InitializeComponent()
    {
        _components = new Container();
        ToolTip1 = new ToolTip(_components);
        _Text1_2 = new TextBox();
        _Text1_10 = new TextBox();
        _Text1_9 = new TextBox();
        _Text1_8 = new TextBox();
        _Text1_7 = new TextBox();
        _Command1_1 = new Button();
        _Command1_0 = new Button();
        _Text1_6 = new TextBox();
        _Text1_5 = new TextBox();
        _Text1_4 = new TextBox();
        _Text1_3 = new TextBox();
        _Text1_1 = new TextBox();
        _Text1_0 = new TextBox();
        _Label1_3 = new Label();
        _Label1_14 = new Label();
        _Label1_13 = new Label();
        _Label1_12 = new Label();
        _Label1_11 = new Label();
        _Label1_10 = new Label();
        _Label1_9 = new Label();
        _Label1_8 = new Label();
        _Label1_7 = new Label();
        _Label1_6 = new Label();
        _Label1_5 = new Label();
        _Label1_4 = new Label();
        _Label1_2 = new Label();
        _Label1_1 = new Label();
        _Label1_0 = new Label();
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        Command1 = new ControlArray<Button>();
        Label1 = new ControlArray<Label>();
        Text1 = new ControlArray<TextBox>();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        ((ISupportInitialize)Command1).BeginInit();
        ((ISupportInitialize)Label1).BeginInit();
        ((ISupportInitialize)Text1).BeginInit();
        SuspendLayout();
        _Text1_2.AcceptsReturn = true;
        _Text1_2.BackColor = SystemColors.Window;
        _Text1_2.Cursor = Cursors.IBeam;
        _Text1_2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_2.ForeColor = SystemColors.WindowText;
        _Text1_2.Location = new Point(39, 240);
        Text1.SetIndex(_Text1_2, 2);
        _Text1_2.Margin = new Padding(4);
        _Text1_2.MaxLength = 0;
        _Text1_2.Name = "_Text1_2";
        _Text1_2.RightToLeft = RightToLeft.No;
        _Text1_2.Size = new Size(1044, 25);
        _Text1_2.TabIndex = 26;
        _Text1_10.AcceptsReturn = true;
        _Text1_10.BackColor = SystemColors.Window;
        _Text1_10.Cursor = Cursors.IBeam;
        _Text1_10.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_10.ForeColor = SystemColors.WindowText;
        _Text1_10.Location = new Point(39, 444);
        Text1.SetIndex(_Text1_10, 10);
        _Text1_10.Margin = new Padding(4);
        _Text1_10.MaxLength = 0;
        _Text1_10.Name = "_Text1_10";
        _Text1_10.RightToLeft = RightToLeft.No;
        _Text1_10.Size = new Size(1044, 25);
        _Text1_10.TabIndex = 21;
        _Text1_9.AcceptsReturn = true;
        _Text1_9.BackColor = SystemColors.Window;
        _Text1_9.Cursor = Cursors.IBeam;
        _Text1_9.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_9.ForeColor = SystemColors.WindowText;
        _Text1_9.Location = new Point(39, 419);
        Text1.SetIndex(_Text1_9, 9);
        _Text1_9.Margin = new Padding(4);
        _Text1_9.MaxLength = 0;
        _Text1_9.Name = "_Text1_9";
        _Text1_9.RightToLeft = RightToLeft.No;
        _Text1_9.Size = new Size(1044, 25);
        _Text1_9.TabIndex = 20;
        _Text1_8.AcceptsReturn = true;
        _Text1_8.BackColor = SystemColors.Window;
        _Text1_8.Cursor = Cursors.IBeam;
        _Text1_8.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_8.ForeColor = SystemColors.WindowText;
        _Text1_8.Location = new Point(39, 393);
        Text1.SetIndex(_Text1_8, 8);
        _Text1_8.Margin = new Padding(4);
        _Text1_8.MaxLength = 0;
        _Text1_8.Name = "_Text1_8";
        _Text1_8.RightToLeft = RightToLeft.No;
        _Text1_8.Size = new Size(1044, 25);
        _Text1_8.TabIndex = 19;
        _Text1_7.AcceptsReturn = true;
        _Text1_7.BackColor = SystemColors.Window;
        _Text1_7.Cursor = Cursors.IBeam;
        _Text1_7.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_7.ForeColor = SystemColors.WindowText;
        _Text1_7.Location = new Point(39, 368);
        Text1.SetIndex(_Text1_7, 7);
        _Text1_7.Margin = new Padding(4);
        _Text1_7.MaxLength = 0;
        _Text1_7.Name = "_Text1_7";
        _Text1_7.RightToLeft = RightToLeft.No;
        _Text1_7.Size = new Size(1044, 25);
        _Text1_7.TabIndex = 18;
        _Command1_1.BackColor = SystemColors.Control;
        _Command1_1.Cursor = Cursors.Default;
        _Command1_1.DialogResult = DialogResult.Cancel;
        _Command1_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_1.ForeColor = SystemColors.ControlText;
        _Command1_1.Location = new Point(704, 515);
        Command1.SetIndex(_Command1_1, 1);
        _Command1_1.Margin = new Padding(4);
        _Command1_1.Name = "btnCancel";
        _Command1_1.RightToLeft = RightToLeft.No;
        _Command1_1.Size = new Size(249, 30);
        _Command1_1.TabIndex = 14;
        _Command1_1.Text = "Abbrechen und zum Hauptmenü";
        _Command1_1.UseVisualStyleBackColor = false;
        _Command1_0.BackColor = SystemColors.Control;
        _Command1_0.Cursor = Cursors.Default;
        _Command1_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_0.ForeColor = SystemColors.ControlText;
        _Command1_0.Location = new Point(37, 515);
        Command1.SetIndex(_Command1_0, 0);
        _Command1_0.Margin = new Padding(4);
        _Command1_0.Name = "btnVerify";
        _Command1_0.RightToLeft = RightToLeft.No;
        _Command1_0.Size = new Size(249, 30);
        _Command1_0.TabIndex = 13;
        _Command1_0.Text = "Speichern und zum Hauptmenü";
        _Command1_0.UseVisualStyleBackColor = false;
        _Text1_6.AcceptsReturn = true;
        _Text1_6.BackColor = SystemColors.Window;
        _Text1_6.Cursor = Cursors.IBeam;
        _Text1_6.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_6.ForeColor = SystemColors.WindowText;
        _Text1_6.Location = new Point(39, 342);
        Text1.SetIndex(_Text1_6, 6);
        _Text1_6.Margin = new Padding(4);
        _Text1_6.MaxLength = 0;
        _Text1_6.Name = "_Text1_6";
        _Text1_6.RightToLeft = RightToLeft.No;
        _Text1_6.Size = new Size(1044, 25);
        _Text1_6.TabIndex = 12;
        _Text1_5.AcceptsReturn = true;
        _Text1_5.BackColor = SystemColors.Window;
        _Text1_5.Cursor = Cursors.IBeam;
        _Text1_5.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_5.ForeColor = SystemColors.WindowText;
        _Text1_5.Location = new Point(39, 317);
        Text1.SetIndex(_Text1_5, 5);
        _Text1_5.Margin = new Padding(4);
        _Text1_5.MaxLength = 0;
        _Text1_5.Name = "_Text1_5";
        _Text1_5.RightToLeft = RightToLeft.No;
        _Text1_5.Size = new Size(1044, 25);
        _Text1_5.TabIndex = 11;
        _Text1_4.AcceptsReturn = true;
        _Text1_4.BackColor = SystemColors.Window;
        _Text1_4.Cursor = Cursors.IBeam;
        _Text1_4.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_4.ForeColor = SystemColors.WindowText;
        _Text1_4.Location = new Point(39, 291);
        Text1.SetIndex(_Text1_4, 4);
        _Text1_4.Margin = new Padding(4);
        _Text1_4.MaxLength = 0;
        _Text1_4.Name = "_Text1_4";
        _Text1_4.RightToLeft = RightToLeft.No;
        _Text1_4.Size = new Size(1044, 25);
        _Text1_4.TabIndex = 10;
        _Text1_3.AcceptsReturn = true;
        _Text1_3.BackColor = SystemColors.Window;
        _Text1_3.Cursor = Cursors.IBeam;
        _Text1_3.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_3.ForeColor = SystemColors.WindowText;
        _Text1_3.Location = new Point(39, 266);
        Text1.SetIndex(_Text1_3, 3);
        _Text1_3.Margin = new Padding(4);
        _Text1_3.MaxLength = 0;
        _Text1_3.Name = "txtLicPart3";
        _Text1_3.RightToLeft = RightToLeft.No;
        _Text1_3.Size = new Size(1044, 25);
        _Text1_3.TabIndex = 9;
        _Text1_1.AcceptsReturn = true;
        _Text1_1.BackColor = SystemColors.Window;
        _Text1_1.Cursor = Cursors.IBeam;
        _Text1_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_1.ForeColor = SystemColors.WindowText;
        _Text1_1.Location = new Point(39, 215);
        Text1.SetIndex(_Text1_1, 1);
        _Text1_1.Margin = new Padding(4);
        _Text1_1.MaxLength = 0;
        _Text1_1.Name = "txtLicPart2";
        _Text1_1.RightToLeft = RightToLeft.No;
        _Text1_1.Size = new Size(1044, 25);
        _Text1_1.TabIndex = 8;
        _Text1_0.AcceptsReturn = true;
        _Text1_0.BackColor = SystemColors.Window;
        _Text1_0.Cursor = Cursors.IBeam;
        _Text1_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Text1_0.ForeColor = SystemColors.WindowText;
        _Text1_0.Location = new Point(39, 189);
        Text1.SetIndex(_Text1_0, 0);
        _Text1_0.Margin = new Padding(4);
        _Text1_0.MaxLength = 0;
        _Text1_0.Name = "txtLicPart1";
        _Text1_0.RightToLeft = RightToLeft.No;
        _Text1_0.Size = new Size(1044, 25);
        _Text1_0.TabIndex = 7;
        _Label1_3.BackColor = SystemColors.Control;
        _Label1_3.Cursor = Cursors.Default;
        _Label1_3.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_3.ForeColor = SystemColors.ControlText;
        _Label1_3.Location = new Point(5, 240);
        Label1.SetIndex(_Label1_3, 3);
        _Label1_3.Margin = new Padding(4, 0, 4, 0);
        _Label1_3.Name = "_Label1_3";
        _Label1_3.RightToLeft = RightToLeft.No;
        _Label1_3.Size = new Size(33, 23);
        _Label1_3.TabIndex = 27;
        _Label1_3.Text = "F4:";
        _Label1_3.TextAlign = ContentAlignment.TopCenter;
        _Label1_14.BackColor = SystemColors.Control;
        _Label1_14.Cursor = Cursors.Default;
        _Label1_14.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_14.ForeColor = SystemColors.ControlText;
        _Label1_14.Location = new Point(5, 444);
        Label1.SetIndex(_Label1_14, 14);
        _Label1_14.Margin = new Padding(4, 0, 4, 0);
        _Label1_14.Name = "_Label1_14";
        _Label1_14.RightToLeft = RightToLeft.No;
        _Label1_14.Size = new Size(33, 23);
        _Label1_14.TabIndex = 25;
        _Label1_14.Text = "F12:";
        _Label1_14.TextAlign = ContentAlignment.TopCenter;
        _Label1_13.BackColor = SystemColors.Control;
        _Label1_13.Cursor = Cursors.Default;
        _Label1_13.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_13.ForeColor = SystemColors.ControlText;
        _Label1_13.Location = new Point(5, 419);
        Label1.SetIndex(_Label1_13, 13);
        _Label1_13.Margin = new Padding(4, 0, 4, 0);
        _Label1_13.Name = "_Label1_13";
        _Label1_13.RightToLeft = RightToLeft.No;
        _Label1_13.Size = new Size(33, 23);
        _Label1_13.TabIndex = 24;
        _Label1_13.Text = "F11:";
        _Label1_13.TextAlign = ContentAlignment.TopCenter;
        _Label1_12.BackColor = SystemColors.Control;
        _Label1_12.Cursor = Cursors.Default;
        _Label1_12.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_12.ForeColor = SystemColors.ControlText;
        _Label1_12.Location = new Point(5, 393);
        Label1.SetIndex(_Label1_12, 12);
        _Label1_12.Margin = new Padding(4, 0, 4, 0);
        _Label1_12.Name = "_Label1_12";
        _Label1_12.RightToLeft = RightToLeft.No;
        _Label1_12.Size = new Size(33, 23);
        _Label1_12.TabIndex = 23;
        _Label1_12.Text = "F10:";
        _Label1_12.TextAlign = ContentAlignment.TopCenter;
        _Label1_11.BackColor = SystemColors.Control;
        _Label1_11.Cursor = Cursors.Default;
        _Label1_11.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_11.ForeColor = SystemColors.ControlText;
        _Label1_11.Location = new Point(5, 368);
        Label1.SetIndex(_Label1_11, 11);
        _Label1_11.Margin = new Padding(4, 0, 4, 0);
        _Label1_11.Name = "_Label1_11";
        _Label1_11.RightToLeft = RightToLeft.No;
        _Label1_11.Size = new Size(33, 23);
        _Label1_11.TabIndex = 22;
        _Label1_11.Text = "F9:";
        _Label1_11.TextAlign = ContentAlignment.TopCenter;
        _Label1_10.BackColor = Color.Red;
        _Label1_10.Cursor = Cursors.Default;
        _Label1_10.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_10.ForeColor = Color.Yellow;
        _Label1_10.Location = new Point(3, 0);
        Label1.SetIndex(_Label1_10, 10);
        _Label1_10.Margin = new Padding(4, 0, 4, 0);
        _Label1_10.Name = "_Label1_10";
        _Label1_10.RightToLeft = RightToLeft.No;
        _Label1_10.Size = new Size(1067, 19);
        _Label1_10.TabIndex = 17;
        _Label1_10.TextAlign = ContentAlignment.TopCenter;
        _Label1_9.BackColor = Color.Red;
        _Label1_9.Cursor = Cursors.Default;
        _Label1_9.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_9.ForeColor = Color.Yellow;
        _Label1_9.Location = new Point(0, 22);
        Label1.SetIndex(_Label1_9, 9);
        _Label1_9.Margin = new Padding(4, 0, 4, 0);
        _Label1_9.Name = "_Label1_9";
        _Label1_9.RightToLeft = RightToLeft.No;
        _Label1_9.Size = new Size(1067, 19);
        _Label1_9.TabIndex = 16;
        _Label1_9.TextAlign = ContentAlignment.TopCenter;
        _Label1_8.BackColor = Color.Red;
        _Label1_8.Cursor = Cursors.Default;
        _Label1_8.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_8.ForeColor = Color.Yellow;
        _Label1_8.Location = new Point(0, 44);
        Label1.SetIndex(_Label1_8, 8);
        _Label1_8.Margin = new Padding(4, 0, 4, 0);
        _Label1_8.Name = "_Label1_8";
        _Label1_8.RightToLeft = RightToLeft.No;
        _Label1_8.Size = new Size(1067, 19);
        _Label1_8.TabIndex = 15;
        _Label1_8.TextAlign = ContentAlignment.TopCenter;
        _Label1_7.BackColor = SystemColors.Control;
        _Label1_7.Cursor = Cursors.Default;
        _Label1_7.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_7.ForeColor = SystemColors.ControlText;
        _Label1_7.Location = new Point(5, 342);
        Label1.SetIndex(_Label1_7, 7);
        _Label1_7.Margin = new Padding(4, 0, 4, 0);
        _Label1_7.Name = "_Label1_7";
        _Label1_7.RightToLeft = RightToLeft.No;
        _Label1_7.Size = new Size(33, 23);
        _Label1_7.TabIndex = 6;
        _Label1_7.Text = "F8:";
        _Label1_7.TextAlign = ContentAlignment.TopCenter;
        _Label1_6.BackColor = SystemColors.Control;
        _Label1_6.Cursor = Cursors.Default;
        _Label1_6.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_6.ForeColor = SystemColors.ControlText;
        _Label1_6.Location = new Point(5, 317);
        Label1.SetIndex(_Label1_6, 6);
        _Label1_6.Margin = new Padding(4, 0, 4, 0);
        _Label1_6.Name = "_Label1_6";
        _Label1_6.RightToLeft = RightToLeft.No;
        _Label1_6.Size = new Size(33, 23);
        _Label1_6.TabIndex = 5;
        _Label1_6.Text = "F7:";
        _Label1_6.TextAlign = ContentAlignment.TopCenter;
        _Label1_5.BackColor = SystemColors.Control;
        _Label1_5.Cursor = Cursors.Default;
        _Label1_5.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_5.ForeColor = SystemColors.ControlText;
        _Label1_5.Location = new Point(5, 291);
        Label1.SetIndex(_Label1_5, 5);
        _Label1_5.Margin = new Padding(4, 0, 4, 0);
        _Label1_5.Name = "_Label1_5";
        _Label1_5.RightToLeft = RightToLeft.No;
        _Label1_5.Size = new Size(33, 23);
        _Label1_5.TabIndex = 4;
        _Label1_5.Text = "F6:";
        _Label1_5.TextAlign = ContentAlignment.TopCenter;
        _Label1_4.BackColor = SystemColors.Control;
        _Label1_4.Cursor = Cursors.Default;
        _Label1_4.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_4.ForeColor = SystemColors.ControlText;
        _Label1_4.Location = new Point(5, 266);
        Label1.SetIndex(_Label1_4, 4);
        _Label1_4.Margin = new Padding(4, 0, 4, 0);
        _Label1_4.Name = "_Label1_4";
        _Label1_4.RightToLeft = RightToLeft.No;
        _Label1_4.Size = new Size(33, 23);
        _Label1_4.TabIndex = 3;
        _Label1_4.Text = "F5:";
        _Label1_4.TextAlign = ContentAlignment.TopCenter;
        _Label1_2.BackColor = SystemColors.Control;
        _Label1_2.Cursor = Cursors.Default;
        _Label1_2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_2.ForeColor = SystemColors.ControlText;
        _Label1_2.Location = new Point(5, 215);
        Label1.SetIndex(_Label1_2, 2);
        _Label1_2.Margin = new Padding(4, 0, 4, 0);
        _Label1_2.Name = "_Label1_2";
        _Label1_2.RightToLeft = RightToLeft.No;
        _Label1_2.Size = new Size(33, 23);
        _Label1_2.TabIndex = 2;
        _Label1_2.Text = "F3:";
        _Label1_2.TextAlign = ContentAlignment.TopCenter;
        _Label1_1.BackColor = SystemColors.Control;
        _Label1_1.Cursor = Cursors.Default;
        _Label1_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_1.ForeColor = SystemColors.ControlText;
        _Label1_1.Location = new Point(5, 189);
        Label1.SetIndex(_Label1_1, 1);
        _Label1_1.Margin = new Padding(4, 0, 4, 0);
        _Label1_1.Name = "_Label1_1";
        _Label1_1.RightToLeft = RightToLeft.No;
        _Label1_1.Size = new Size(33, 23);
        _Label1_1.TabIndex = 1;
        _Label1_1.Text = "F2:";
        _Label1_1.TextAlign = ContentAlignment.TopCenter;
        _Label1_0.BackColor = SystemColors.Control;
        _Label1_0.Cursor = Cursors.Default;
        _Label1_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_0.ForeColor = SystemColors.ControlText;
        _Label1_0.Location = new Point(0, 143);
        Label1.SetIndex(_Label1_0, 0);
        _Label1_0.Margin = new Padding(4, 0, 4, 0);
        _Label1_0.Name = "_Label1_0";
        _Label1_0.RightToLeft = RightToLeft.No;
        _Label1_0.Size = new Size(1067, 19);
        _Label1_0.TabIndex = 0;
        _Label1_0.Text = "Belegung der Funktionstasten mit festen Texten";
        _Label1_0.TextAlign = ContentAlignment.TopCenter;
        AutoScaleDimensions = new SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        CancelButton = _Command1_1;
        ClientSize = new Size(1018, 725);
        ControlBox = false;
        Controls.Add(_Text1_2);
        Controls.Add(_Text1_10);
        Controls.Add(_Text1_9);
        Controls.Add(_Text1_8);
        Controls.Add(_Text1_7);
        Controls.Add(_Command1_1);
        Controls.Add(_Command1_0);
        Controls.Add(_Text1_6);
        Controls.Add(_Text1_5);
        Controls.Add(_Text1_4);
        Controls.Add(_Text1_3);
        Controls.Add(_Text1_1);
        Controls.Add(_Text1_0);
        Controls.Add(_Label1_3);
        Controls.Add(_Label1_14);
        Controls.Add(_Label1_13);
        Controls.Add(_Label1_12);
        Controls.Add(_Label1_11);
        Controls.Add(_Label1_10);
        Controls.Add(_Label1_9);
        Controls.Add(_Label1_8);
        Controls.Add(_Label1_7);
        Controls.Add(_Label1_6);
        Controls.Add(_Label1_5);
        Controls.Add(_Label1_4);
        Controls.Add(_Label1_2);
        Controls.Add(_Label1_1);
        Controls.Add(_Label1_0);
        Cursor = Cursors.Default;
        Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Location = new Point(3, 22);
        Margin = new Padding(4);
        Name = "FunkT";
        RightToLeft = RightToLeft.No;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Funktionstastenbelegung";
        WindowState = FormWindowState.Maximized;
        ((ISupportInitialize)Command1).EndInit();
        ((ISupportInitialize)Label1).EndInit();
        ((ISupportInitialize)Text1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private void _Command1_Click(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            switch (Command1.GetIndex((Button)eventSender))
            {
                case 0:
                    {
                        FileSystem.FileClose(99);
                        float num;
                        if (Modul1.cMandDrive.DriveType != DriveType.CDRom)
                        {
                            FileSystem.FileOpen(99, Modul1.InitDir + "Text.Dat", OpenMode.Output);
                            _I = 0f;
                            float i;
                            do
                            {
                                FileSystem.PrintLine(99, Text1[(short)Math.Round(_I)].Text);
                                _I += 1f;
                                i = _I;
                                num = 10f;
                            }
                            while (i <= num);
                        }
                        FileSystem.FileClose(99);
                        FileSystem.FileOpen(99, Modul1.InitDir + "Text.Dat", OpenMode.Input);
                        _I = 0f;
                        float i2;
                        do
                        {
                            Modul1.Te[(int)Math.Round(_I)] = FileSystem.LineInput(99);
                            _I += 1f;
                            i2 = _I;
                            num = 10f;
                        }
                        while (i2 <= num);
                        Close();
                        Menue.Default.Show();
                        break;
                    }
                case 1:
                    Close();
                    Menue.Default.Show();
                    break;
            }
        }
    }

    private void _FunkT_Load(object eventSender, EventArgs eventArgs)
    {
        checked
        {
            if (Modul1.FontSize > 0f)
            {
                Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                short num = 0;
                short num2;
                short num3;
                do
                {
                    Label1[num].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                    num = (short)unchecked(num + 1);
                    num2 = num;
                    num3 = 14;
                }
                while (num2 <= num3);
                num = 0;
                short num4;
                do
                {
                    Text1[num].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                    num = (short)unchecked(num + 1);
                    num4 = num;
                    num3 = 10;
                }
                while (num4 <= num3);
                Command1[0].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
                Command1[1].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            }
            var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
            Left = aiPos[0];
            Top = aiPos[1];
            BackColor = Modul1.HintFarb;
            Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var WiS);
            WindowState = WiS;
            _Label1_0.Width = Width;
            _Label1_8.Width = Width;
            _Label1_9.Width = Width;
            _Label1_10.Width = Width;
            _I = 0f;
            float i;
            float num5;
            do
            {
                Text1[(short)Math.Round(_I)].Width = Width - 100;
                _I += 1f;
                i = _I;
                num5 = 10f;
            }
            while (i <= num5);
            _Label1_0.Text = Modul1.IText[EUserText.t408];
            _Label1_10.Text = Modul1.VersionT;
            _Label1_9.Text = Modul1.Version1;
            _Label1_8.Text = Modul1.Version;
            if (Modul1.System.VerSpecial == 1)
            {
                _Label1_8.Text = "Eingeschränkte Sonderversion";
            }
            Command1[0].Text = Modul1.IText[EUserText.tNMSave];
            Command1[1].Text = Modul1.IText[EUserText.tNMCancel];
            _I = 0f;
            float i2;
            do
            {
                Text1[(short)Math.Round(_I)].Text = Modul1.Te[(int)Math.Round(_I)];
                _I += 1f;
                i2 = _I;
                num5 = 10f;
            }
            while (i2 <= num5);
        }
    }

    private void _FunkT_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
    {
        //Discarded unreachable code: IL_006e, IL_00e6
        int try0001_dispatch = -1;
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
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        cancel = eventArgs.Cancel;
                        goto IL_000b;
                    case 333:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_00e9;
                                default:
                                    goto end_IL_0001;
                            }
                            goto IL_0070;
                        }
                    IL_0029:
                        num = 5;
                        DataModul.MandDB.Close();
                        goto IL_0037;
                    IL_0037:
                        num = 6;
                        DataModul.TempDB.Close();
                        goto IL_0045;
                    IL_001c:
                        num = 4;
                        if (closeReason != 0)
                        {
                            goto end_IL_0001_2;
                        }
                        goto IL_0029;
                    IL_0070:
                        num = 12;
                        if (Information.Err().Number == 91)
                        {
                            goto IL_0088;
                        }
                        else
                        {
                            goto IL_00a1;
                        }
                    IL_00a1:
                        num = 16;
                        if (Information.Err().Number != 3420)
                        {
                            break;
                        }
                        goto IL_00bc;
                    IL_00bc:
                        num = 17;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_00e9;
                    IL_0053:
                        num = 8;
                        DataModul.DSB.Close();
                        ProjectData.EndApp();
                        goto end_IL_0001_2;
                    IL_00e9:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_000b;
                            case 3:
                                goto IL_0015;
                            case 4:
                                goto IL_001c;
                            case 5:
                                goto IL_0029;
                            case 6:
                                goto IL_0037;
                            case 7:
                                goto IL_0045;
                            case 8:
                                goto IL_0053;
                            case 12:
                                goto IL_0070;
                            case 13:
                                goto IL_0088;
                            case 14:
                            case 16:
                                goto IL_00a1;
                            case 17:
                                goto IL_00bc;
                            case 18:
                            case 20:
                                goto end_IL_0001_3;
                            default:
                                goto end_IL_0001;
                            case 9:
                            case 10:
                            case 11:
                            case 21:
                                goto end_IL_0001_2;
                        }
                        goto default;
                    IL_0088:
                        num = 13;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_00e9;
                    IL_0045:
                        num = 7;
                        DataModul.DOSB.Close();
                        goto IL_0053;
                    IL_000b:
                        num = 2;
                        closeReason = eventArgs.CloseReason;
                        goto IL_0015;
                    IL_0015:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_001c;
                    end_IL_0001_3:
                        break;
                }
                num = 20;
                eventArgs.Cancel = cancel;
                break;
            end_IL_0001:
                ;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 333;
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

    private void _Label1_0_Click(object sender, EventArgs e)
    {
    }
}
