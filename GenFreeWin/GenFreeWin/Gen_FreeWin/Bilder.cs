using BaseLib.Helper;
using Gen_FreeWin.Main;
using Gen_FreeWin.Models;
using Gen_FreeWin.ViewModels.Interfaces;
using Gen_FreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin;

[DesignerGenerated]
internal class Bilder : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    private IContainer components;
    private readonly IBilderViewModel _viewModel;
    IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction => Menue.Default;
    public ToolTip ToolTip1;

    [AccessedThroughProperty(nameof(Timer1))]
    private Timer _Timer1;

    [VisibilityBinding(nameof(IBilderViewModel.PersonPictureButtonVisible))]
    [EnabledBinding(nameof(IBilderViewModel.PersonPictureButtonEnabled))]
    [AccessedThroughProperty(nameof(Command2))]
    private Button _Command2;

    [VisibilityBinding(nameof(IBilderViewModel.PictureVisible))]
    [AccessedThroughProperty(nameof(Picture1))]
    private PictureBox _Picture1;

    [TextBinding(nameof(IBilderViewModel.PictureTitle))]
    [AccessedThroughProperty(nameof(Text2))]
    private TextBox _Text2;

    [TextBinding(nameof(IBilderViewModel.PictureRemark))]
    [AccessedThroughProperty(nameof(RTB))]
    private RichTextBox _RTB;

    [AccessedThroughProperty(nameof(_Command1_0))]
    private Button __Command1_0;

    [ListBinding(nameof(IBilderViewModel.PictureItems), nameof(IBilderViewModel.SelectedPictureItem))]
    [VisibilityBinding(nameof(IBilderViewModel.PictureListVisible))]
    [AccessedThroughProperty(nameof(List1))]
    private ListBox _List1;

#pragma warning disable CS0618 // Typ oder Element ist veraltet
    [AccessedThroughProperty(nameof(Command1))]
    private ControlArray<Button> _Command1;

    [AccessedThroughProperty(nameof(Button1))]
    private Button _Button1;

    [AccessedThroughProperty(nameof(Dir1))]
    private ListBox _Dir1;

    [AccessedThroughProperty(nameof(Drive1))]
    private ListBox _Drive1;

    [AccessedThroughProperty(nameof(File1))]
    private ListBox _File1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet

    [AccessedThroughProperty(nameof(Button3))]
    private Button _Button3;

    [AccessedThroughProperty(nameof(Button2))]
    private Button _Button2;

    [AccessedThroughProperty(nameof(Button4))]
    private Button _Button4;


    private int I;

    private string Bildname;

    private string A;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual Timer Timer1
    {
        [DebuggerNonUserCode]
        get => _Timer1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Timer1_Tick;
            if (_Timer1 != null)
            {
                _Timer1.Tick -= value2;
            }
            _Timer1 = value;
            if (_Timer1 != null)
            {
                _Timer1.Tick += value2;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual Button Command2
    {
        [DebuggerNonUserCode]
        get => _Command2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual PictureBox Picture1
    {
        [DebuggerNonUserCode]
        get => _Picture1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Picture1_Click;
            if (_Picture1 != null)
            {
                _Picture1.Click -= value2;
            }
            _Picture1 = value;
            if (_Picture1 != null)
            {
                _Picture1.Click += value2;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual TextBox Text2
    {
        [DebuggerNonUserCode]
        get => _Text2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            KeyEventHandler value2 = Text2_KeyDown;
            EventHandler value3 = Text2_Enter;
            if (_Text2 != null)
            {
                _Text2.KeyDown -= value2;
                _Text2.Enter -= value3;
            }
            _Text2 = value;
            if (_Text2 != null)
            {
                _Text2.KeyDown += value2;
                _Text2.Enter += value3;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual RichTextBox RTB
    {
        [DebuggerNonUserCode]
        get => _RTB;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            KeyEventHandler value2 = RTB_KeyDown;
            if (_RTB != null)
            {
                _RTB.KeyDown -= value2;
            }
            _RTB = value;
            if (_RTB != null)
            {
                _RTB.KeyDown += value2;
            }
        }
    }

    public Label _Label2_1;
    [VisibilityBinding(nameof(IBilderViewModel.EditPanelVisible))]
    public GroupBox Frame1;
    [EnabledBinding(nameof(IBilderViewModel.DeleteEnabled))]
    public Button _Command1_4;
    [EnabledBinding(nameof(IBilderViewModel.SaveEnabled))]
    public Button _Command1_3;
    public Button _Command1_2;
    public Button _Command1_1;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual Button _Command1_0
    {
        [DebuggerNonUserCode]
        get => __Command1_0;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ListBox List1
    {
        [DebuggerNonUserCode]
        get => _List1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    [TextBinding(nameof(IBilderViewModel.SelectedRecordNumberText))]
    public Label _Label2_0;
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

    public ControlArray<Label> Label2;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ListBox Dir1
    {
        [DebuggerNonUserCode]
        get => _Dir1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler obj = Dir1_Change;
            if (_Dir1 != null)
            {
                _Dir1.SelectedIndexChanged -= obj;
            }
            _Dir1 = value;
            if (_Dir1 != null)
            {
                _Dir1.SelectedIndexChanged += obj;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ListBox Drive1
    {
        [DebuggerNonUserCode]
        get => _Drive1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Drive1_SelectedIndexChanged;
            if (_Drive1 != null)
            {
                _Drive1.SelectedIndexChanged -= value2;
            }
            _Drive1 = value;
            if (_Drive1 != null)
            {
                _Drive1.SelectedIndexChanged += value2;
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ListBox File1
    {
        [DebuggerNonUserCode]
        get => _File1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = File1_SelectedIndexChanged;
            EventHandler value3 = File1_DoubleClick;
            if (_File1 != null)
            {
                _File1.SelectedIndexChanged -= value2;
                _File1.DoubleClick -= value3;
            }
            _File1 = value;
            if (_File1 != null)
            {
                _File1.SelectedIndexChanged += value2;
                _File1.DoubleClick += value3;
            }
        }
    }

    internal ListBox DirListBox1;
    internal ListBox DriveListBox1;
    internal ListBox FileListBox1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    internal virtual Button Button1
    {
        [DebuggerNonUserCode]
        get => _Button1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    [VisibilityBinding(nameof(IBilderViewModel.FileSelectionVisible))]
    internal GroupBox GroupBox1;
    internal Label Label1;
    internal Label Label3;
    internal Panel Panel1;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual Button Button3
    {
        [DebuggerNonUserCode]
        get => _Button3;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual Button Button2
    {
        [DebuggerNonUserCode]
        get => _Button2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    internal Label Label4;
    internal Label Label6;
    internal Label Label5;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual Button Button4
    {
        [DebuggerNonUserCode]
        get => _Button4;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
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

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassStyle |= 512;
            return createParams;
        }
    }

    [DebuggerNonUserCode]
    public Bilder(IBilderViewModel viewModel)
    {
        Load += Bilder_Load;
        _viewModel = viewModel;
        _viewModel.RequestClose = Close;
        _viewModel.RefreshOwner = RefreshOwner;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        Bildname = "";
        InitializeComponent();
        TextBindingAttribute.Commit(this, _viewModel);
        VisibilityBindingAttribute.Commit(this, _viewModel);
        EnabledBindingAttribute.Commit(this, _viewModel);
        ListBindingAttribute.Commit(this, _viewModel);
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

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
        components = new Container();
        ToolTip1 = new ToolTip(components);
        Timer1 = new Timer(components);
        Command2 = new Button();
        Picture1 = new PictureBox();
        Frame1 = new GroupBox();
        Button4 = new Button();
        Button1 = new Button();
        Text2 = new TextBox();
        RTB = new RichTextBox();
        _Label2_1 = new Label();
        _Command1_4 = new Button();
        _Command1_3 = new Button();
        _Command1_2 = new Button();
        _Command1_1 = new Button();
        _Command1_0 = new Button();
        List1 = new ListBox();
        _Label2_0 = new Label();
        Command1 = new ControlArray<Button>();
        Label2 = new ControlArray<Label>();
        GroupBox1 = new GroupBox();
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        Dir1 = new ListBox();
        Drive1 = new ListBox();
        File1 = new ListBox();
        DirListBox1 = new ListBox();
        DriveListBox1 = new ListBox();
        FileListBox1 = new ListBox();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        Label1 = new Label();
        Label3 = new Label();
        Panel1 = new Panel();
        Label6 = new Label();
        Label5 = new Label();
        Label4 = new Label();
        Button3 = new Button();
        Button2 = new Button();
        ((ISupportInitialize)Picture1).BeginInit();
        Frame1.SuspendLayout();
        ((ISupportInitialize)Command1).BeginInit();
        ((ISupportInitialize)Label2).BeginInit();
        GroupBox1.SuspendLayout();
        Panel1.SuspendLayout();
        SuspendLayout();
        Timer1.Enabled = true;
        Timer1.Interval = 2;
        Command2.BackColor = SystemColors.Control;
        Command2.Cursor = Cursors.Default;
        Command2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Command2.ForeColor = SystemColors.ControlText;
        Command2.Location = new Point(315, 655);
        Command2.Margin = new Padding(4);
        Command2.Name = "btnNew";
        Command2.RightToLeft = RightToLeft.No;
        Command2.Size = new Size(293, 30);
        Command2.TabIndex = 16;
        Command2.Text = "Dieses Bild als Personenbild übernehmen";
        Command2.UseVisualStyleBackColor = false;
        Picture1.BackColor = SystemColors.Control;
        Picture1.BorderStyle = BorderStyle.Fixed3D;
        Picture1.Cursor = Cursors.Default;
        Picture1.ForeColor = SystemColors.ControlText;
        Picture1.Location = new Point(2, 2);
        Picture1.Margin = new Padding(4);
        Picture1.Name = "Picture1";
        Picture1.RightToLeft = RightToLeft.No;
        Picture1.Size = new Size(673, 351);
        Picture1.TabIndex = 15;
        Picture1.TabStop = false;
        Picture1.Visible = false;
        Frame1.BackColor = Color.Red;
        Frame1.Controls.Add(Button4);
        Frame1.Controls.Add(Button1);
        Frame1.Controls.Add(Text2);
        Frame1.Controls.Add(RTB);
        Frame1.Controls.Add(_Label2_1);
        Frame1.ForeColor = SystemColors.ControlText;
        Frame1.Location = new Point(616, 549);
        Frame1.Margin = new Padding(4);
        Frame1.Name = "frmFamilyresidence";
        Frame1.Padding = new Padding(4);
        Frame1.RightToLeft = RightToLeft.No;
        Frame1.Size = new Size(389, 174);
        Frame1.TabIndex = 9;
        Frame1.TabStop = false;
        Frame1.Text = "Bildname (max.40 Zeichen";
        Button4.Location = new Point(116, 56);
        Button4.Name = "btnClose";
        Button4.Size = new Size(262, 23);
        Button4.TabIndex = 16;
        Button4.Text = "Dateiname als Bildname übernehmen";
        Button4.UseVisualStyleBackColor = true;
        Button1.Location = new Point(360, 0);
        Button1.Name = "btnReqHint";
        Button1.Size = new Size(22, 23);
        Button1.TabIndex = 15;
        Button1.Text = "X";
        Button1.UseVisualStyleBackColor = true;
        Text2.AcceptsReturn = true;
        Text2.BackColor = SystemColors.Window;
        Text2.Cursor = Cursors.IBeam;
        Text2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Text2.ForeColor = SystemColors.WindowText;
        Text2.Location = new Point(11, 26);
        Text2.Margin = new Padding(4);
        Text2.MaxLength = 0;
        Text2.Name = "Text2";
        Text2.RightToLeft = RightToLeft.No;
        Text2.Size = new Size(369, 25);
        Text2.TabIndex = 10;
        RTB.BorderStyle = BorderStyle.FixedSingle;
        RTB.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        RTB.Location = new Point(11, 86);
        RTB.Margin = new Padding(4);
        RTB.Name = "RTB";
        RTB.RightMargin = 324;
        RTB.Size = new Size(367, 73);
        RTB.TabIndex = 14;
        RTB.Text = "";
        _Label2_1.BackColor = SystemColors.Control;
        _Label2_1.Cursor = Cursors.Default;
        _Label2_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_1.ForeColor = SystemColors.ControlText;
        _Label2_1.Location = new Point(8, 55);
        Label2.SetIndex(_Label2_1, 1);
        _Label2_1.Margin = new Padding(4, 0, 4, 0);
        _Label2_1.Name = "lblSep1";
        _Label2_1.RightToLeft = RightToLeft.No;
        _Label2_1.Size = new Size(101, 26);
        _Label2_1.TabIndex = 11;
        _Label2_1.Text = "Text zum Bild";
        _Command1_4.BackColor = SystemColors.Control;
        _Command1_4.Cursor = Cursors.Default;
        _Command1_4.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_4.ForeColor = SystemColors.ControlText;
        _Command1_4.Location = new Point(3, 693);
        Command1.SetIndex(_Command1_4, 4);
        _Command1_4.Margin = new Padding(4);
        _Command1_4.Name = "_Command1_4";
        _Command1_4.RightToLeft = RightToLeft.No;
        _Command1_4.Size = new Size(106, 30);
        _Command1_4.TabIndex = 8;
        _Command1_4.Text = "Bild &entfernen";
        _Command1_4.UseVisualStyleBackColor = false;
        _Command1_3.BackColor = SystemColors.Control;
        _Command1_3.Cursor = Cursors.Default;
        _Command1_3.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_3.ForeColor = SystemColors.ControlText;
        _Command1_3.Location = new Point(388, 693);
        Command1.SetIndex(_Command1_3, 3);
        _Command1_3.Margin = new Padding(4);
        _Command1_3.Name = "_Command1_3";
        _Command1_3.RightToLeft = RightToLeft.No;
        _Command1_3.Size = new Size(141, 30);
        _Command1_3.TabIndex = 7;
        _Command1_3.Text = "Bild &übernehmen";
        _Command1_3.UseVisualStyleBackColor = false;
        _Command1_2.BackColor = SystemColors.Control;
        _Command1_2.Cursor = Cursors.Default;
        _Command1_2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_2.ForeColor = SystemColors.ControlText;
        _Command1_2.Location = new Point(117, 693);
        Command1.SetIndex(_Command1_2, 2);
        _Command1_2.Margin = new Padding(4);
        _Command1_2.Name = "_Command1_2";
        _Command1_2.RightToLeft = RightToLeft.No;
        _Command1_2.Size = new Size(120, 30);
        _Command1_2.TabIndex = 6;
        _Command1_2.Text = "Bilder &ansehen";
        _Command1_2.UseVisualStyleBackColor = false;
        _Command1_1.BackColor = SystemColors.Control;
        _Command1_1.Cursor = Cursors.Default;
        _Command1_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_1.ForeColor = SystemColors.ControlText;
        _Command1_1.Location = new Point(245, 693);
        Command1.SetIndex(_Command1_1, 1);
        _Command1_1.Margin = new Padding(4);
        _Command1_1.Name = "btnCancel";
        _Command1_1.RightToLeft = RightToLeft.No;
        _Command1_1.Size = new Size(135, 30);
        _Command1_1.TabIndex = 5;
        _Command1_1.Text = "Bilder &hinzufügen";
        _Command1_1.UseVisualStyleBackColor = false;
        _Command1_0.BackColor = SystemColors.Control;
        _Command1_0.Cursor = Cursors.Default;
        _Command1_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_0.ForeColor = SystemColors.ControlText;
        _Command1_0.Location = new Point(537, 693);
        Command1.SetIndex(_Command1_0, 0);
        _Command1_0.Margin = new Padding(4);
        _Command1_0.Name = "btnVerify";
        _Command1_0.RightToLeft = RightToLeft.No;
        _Command1_0.Size = new Size(71, 30);
        _Command1_0.TabIndex = 4;
        _Command1_0.UseVisualStyleBackColor = false;
        List1.BackColor = SystemColors.Window;
        List1.Cursor = Cursors.Default;
        List1.Font = new Font("Courier New", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        List1.ForeColor = SystemColors.WindowText;
        List1.ItemHeight = 17;
        List1.Location = new Point(665, 13);
        List1.Margin = new Padding(4);
        List1.Name = "List1";
        List1.RightToLeft = RightToLeft.No;
        List1.Size = new Size(342, 412);
        List1.Sorted = true;
        List1.TabIndex = 3;
        List1.Visible = false;
        _Label2_0.BackColor = SystemColors.Control;
        _Label2_0.Cursor = Cursors.Default;
        _Label2_0.ForeColor = SystemColors.ControlText;
        _Label2_0.Location = new Point(245, 282);
        Label2.SetIndex(_Label2_0, 0);
        _Label2_0.Margin = new Padding(4, 0, 4, 0);
        _Label2_0.Name = "lblSep2";
        _Label2_0.RightToLeft = RightToLeft.No;
        _Label2_0.Size = new Size(95, 27);
        _Label2_0.TabIndex = 12;
        _Label2_0.Visible = false;
        GroupBox1.Controls.Add(Dir1);
        GroupBox1.Controls.Add(Drive1);
        GroupBox1.Controls.Add(File1);
        GroupBox1.Location = new Point(665, 2);
        GroupBox1.Margin = new Padding(4);
        GroupBox1.Name = "grpFather";
        GroupBox1.Padding = new Padding(4);
        GroupBox1.Size = new Size(375, 721);
        GroupBox1.TabIndex = 18;
        GroupBox1.TabStop = false;
        GroupBox1.Visible = false;
        Dir1.BackColor = SystemColors.Window;
        Dir1.Cursor = Cursors.Default;
        Dir1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Dir1.ForeColor = SystemColors.WindowText;
        Dir1.FormattingEnabled = true;
        Dir1.IntegralHeight = false;
        Dir1.Location = new Point(19, 43);
        Dir1.Margin = new Padding(4);
        Dir1.Name = "Dir1";
        Dir1.Size = new Size(330, 189);
        Dir1.TabIndex = 2;
        Dir1.Visible = false;
        Drive1.BackColor = SystemColors.Window;
        Drive1.Cursor = Cursors.Default;
        Drive1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Drive1.ForeColor = SystemColors.WindowText;
        Drive1.FormattingEnabled = true;
        Drive1.Location = new Point(19, 11);
        Drive1.Margin = new Padding(4);
        Drive1.Name = "Drive1";
        Drive1.Size = new Size(330, 26);
        Drive1.TabIndex = 1;
        Drive1.Visible = false;
        File1.BackColor = SystemColors.Window;
        File1.Cursor = Cursors.Default;
        File1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        File1.ForeColor = SystemColors.WindowText;
        File1.FormattingEnabled = true;
        File1.Location = new Point(19, 250);
        File1.Margin = new Padding(4);
        File1.Name = "File1";
        File1.Tag = "*.BMP;*.JPG;*.MP4;*.TIF;*.PDF;*.*";
        File1.Size = new Size(330, 412);
        File1.TabIndex = 0;
        File1.Visible = false;
        DirListBox1.BackColor = SystemColors.Window;
        DirListBox1.Cursor = Cursors.Default;
        DirListBox1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        DirListBox1.ForeColor = SystemColors.WindowText;
        DirListBox1.FormattingEnabled = true;
        DirListBox1.IntegralHeight = false;
        DirListBox1.Location = new Point(27, 43);
        DirListBox1.Margin = new Padding(4);
        DirListBox1.Name = "DirListBox1";
        DirListBox1.Size = new Size(307, 189);
        DirListBox1.TabIndex = 2;
        DriveListBox1.BackColor = SystemColors.Window;
        DriveListBox1.Cursor = Cursors.Default;
        DriveListBox1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        DriveListBox1.ForeColor = SystemColors.WindowText;
        DriveListBox1.FormattingEnabled = true;
        DriveListBox1.Location = new Point(19, 11);
        DriveListBox1.Margin = new Padding(4);
        DriveListBox1.Name = "DriveListBox1";
        DriveListBox1.Size = new Size(330, 26);
        DriveListBox1.TabIndex = 1;
        FileListBox1.BackColor = SystemColors.Window;
        FileListBox1.Cursor = Cursors.Default;
        FileListBox1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        FileListBox1.ForeColor = SystemColors.WindowText;
        FileListBox1.FormattingEnabled = true;
        FileListBox1.Location = new Point(28, 240);
        FileListBox1.Margin = new Padding(4);
        FileListBox1.Name = "FileListBox1";
        FileListBox1.Tag = "*.BMP;*.JPG;*.TIF;*.PDF;*.*";
        FileListBox1.Size = new Size(306, 276);
        FileListBox1.TabIndex = 0;
        Label1.AutoEllipsis = true;
        Label1.AutoSize = true;
        Label1.BackColor = Color.White;
        Label1.Location = new Point(0, 668);
        Label1.Name = "Label1";
        Label1.Size = new Size(0, 17);
        Label1.TabIndex = 19;
        Label1.TextAlign = ContentAlignment.MiddleCenter;
        Label3.AutoSize = true;
        Label3.Location = new Point(42, 655);
        Label3.Name = "lblDisplayHint";
        Label3.Size = new Size(254, 17);
        Label3.TabIndex = 20;
        Label3.Text = "Klick in das Bild öffnet Bildbearbeitung";
        Label3.Visible = false;
        Panel1.BackColor = Color.FromArgb(255, 255, 192);
        Panel1.Controls.Add(Label6);
        Panel1.Controls.Add(Label5);
        Panel1.Controls.Add(Label4);
        Panel1.Controls.Add(Button3);
        Panel1.Controls.Add(Button2);
        Panel1.Location = new Point(18, 45);
        Panel1.Name = "Panel1";
        Panel1.Size = new Size(640, 195);
        Panel1.TabIndex = 21;
        Panel1.Visible = false;
        Label6.BackColor = Color.FromArgb(255, 192, 192);
        Label6.Location = new Point(14, 43);
        Label6.Name = "Label6";
        Label6.Size = new Size(618, 23);
        Label6.TabIndex = 4;
        Label6.Text = "Label6";
        Label6.TextAlign = ContentAlignment.MiddleCenter;
        Label5.BackColor = Color.White;
        Label5.Location = new Point(14, 9);
        Label5.Name = "lblSorting";
        Label5.Size = new Size(618, 34);
        Label5.TabIndex = 3;
        Label5.Text = "lblSorting";
        Label5.TextAlign = ContentAlignment.MiddleCenter;
        Label4.BackColor = Color.White;
        Label4.Location = new Point(14, 66);
        Label4.Name = "lblSearch";
        Label4.Size = new Size(618, 74);
        Label4.TabIndex = 2;
        Label4.Text = "lblSearch";
        Label4.TextAlign = ContentAlignment.MiddleCenter;
        Button3.BackColor = Color.FromArgb(192, 255, 255);
        Button3.Location = new Point(314, 157);
        Button3.Name = "btnReenter";
        Button3.Size = new Size(176, 32);
        Button3.TabIndex = 1;
        Button3.Text = "Datei anzeigen";
        Button3.UseVisualStyleBackColor = false;
        Button2.BackColor = Color.FromArgb(192, 255, 255);
        Button2.Location = new Point(17, 157);
        Button2.Name = "btnRegisterSearch";
        Button2.Size = new Size(271, 32);
        Button2.TabIndex = 0;
        Button2.Text = "nur Datei verknüpfen";
        Button2.UseVisualStyleBackColor = false;
        AutoScaleDimensions = new SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        ClientSize = new Size(1018, 725);
        Controls.Add(Panel1);
        Controls.Add(Label3);
        Controls.Add(Label1);
        Controls.Add(Frame1);
        Controls.Add(GroupBox1);
        Controls.Add(Command2);
        Controls.Add(Picture1);
        Controls.Add(_Command1_4);
        Controls.Add(_Command1_3);
        Controls.Add(_Command1_2);
        Controls.Add(_Command1_1);
        Controls.Add(_Command1_0);
        Controls.Add(_Label2_0);
        Controls.Add(List1);
        Cursor = Cursors.Default;
        Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Margin = new Padding(4);
        Name = "Bilder";
        RightToLeft = RightToLeft.No;
        SizeGripStyle = SizeGripStyle.Show;
        StartPosition = FormStartPosition.Manual;
        Text = "Bildverwaltung";
        ((ISupportInitialize)Picture1).EndInit();
        Frame1.ResumeLayout(false);
        Frame1.PerformLayout();
        ((ISupportInitialize)Command1).EndInit();
        ((ISupportInitialize)Label2).EndInit();
        GroupBox1.ResumeLayout(false);
        Panel1.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private void Command1_Click(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0aaf
        int try0001_dispatch = -1;
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
                checked
                {
                    int num4;
                    short index;
                    switch (try0001_dispatch)
                    {
                        default:
                            num = 1;
                            goto IL_0016;
                        case 3809:
                            {
                                num2 = num;
                                switch ((num3 <= -2) ? 1 : num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0c3b;
                                    default:
                                        goto end_IL_0001;
                                }

                                int number = Information.Err().Number;
                                if (number == 3021)
                                {
                                    Modul1.Nr = 0;
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_0016;
                                }
                                else
                                {
                                    if (number is 3315 or 5)
                                    {
                                        _ = Interaction.MsgBox("Kein Bild ausgewählt.");
                                        goto end_IL_0001_2;
                                    }
                                    else
                                    {
                                        if (number == 3163)
                                        {
                                            string text = "Der Dateiname:\n" + File1.SelectedIndex + "\nist zu lang!";
                                            text += "\nBitte ändern Sie den Dateinamen auf höchstens 30 Zeichen";
                                            _ = Interaction.MsgBox(text);
                                            goto end_IL_0001_2;
                                        }
                                        else
                                        {
                                            _ = Interaction.MsgBox(Information.Err().Number.AsString());
                                            _ = Interaction.MsgBox("Bild konnte nicht gespeichert werde.");
                                            _ = Interaction.MsgBox(Information.Err().Number.AsString());
                                            ProjectData.ClearProjectError();
                                            if (num2 == 0)
                                            {
                                                throw ProjectData.CreateProjectError(-2146828268);
                                            }
                                            num4 = num2;
                                            goto IL_0c3f;
                                        }
                                    }
                                }
                            }
                        end_IL_0001:
                            break;
                        IL_0016:
                            num = 2;
                            index = (short)Command1.GetIndex((Button)eventSender);
                            switch (index)
                            {
                                case 0:
                                    Command1_0_Click(eventSender, eventArgs);
                                    break;
                                case 1:
                                    goto IL_0154;
                                case 2:
                                    goto IL_0351;
                                case 3:
                                    goto IL_05a3;
                                case 4:
                                    goto IL_0a0c;
                                default:
                                    break;
                            }
                            goto end_IL_0001_2;
                        IL_0154:
                            num = 25;
                            Command1_1_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_0351:
                            num = 51;
                            Command1_2_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_05a3:
                            ProjectData.ClearProjectError();
                            Command1_3_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_0a0c:
                            num = 132;
                            Command1_4_Click(eventSender, eventArgs);
                            goto end_IL_0001_2;
                        IL_0c3b:
                            num4 = unchecked(num2 + 1);
                            goto IL_0c3f;
                        IL_0c3f:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;


                                case 3:
                                case 10:
                                case 15:
                                case 19:
                                case 22:
                                case 23:
                                case 49:
                                case 66:
                                case 69:
                                case 76:
                                case 80:
                                case 85:
                                case 121:
                                case 130:
                                case 138:
                                case 139:
                                case 140:
                                case 151:
                                case 157:
                                case 165:
                                    goto end_IL_0001_2;
                            }
                            goto default;
                    }
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 3809;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 15
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    private void Command1_4_Click(object eventSender, EventArgs eventArgs)
    {
        if (_viewModel.DeleteEnabled
            && Interaction.MsgBox("Soll die Verbindung des Bildes zur Person/Familie wirklich gelöscht werden?", title: "", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _viewModel.DeleteCurrentPicture();
            _Label2_0.Tag = 0;
        }
    }

    private void Command1_3_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }

        if (string.IsNullOrWhiteSpace(Text2.Text))
        {
            _ = Interaction.MsgBox("Bildname muß angegeben werden!", title: "", icon: MessageBoxIcon.Warning);
            _ = Text2.Focus();
            return;
        }

        _viewModel.SaveCurrentPicture(BuildStoredPath(), GetSelectedFileName());
        _ = Interaction.MsgBox("Verknüpfung / Änderung wurde gespeichert");
    }

    private void Command1_2_Click(object eventSender, EventArgs eventArgs)
    {
        Picture1.Image = null;
        _Label2_0.Tag = 0;
        _viewModel.ShowPictureList();
    }

    private void Command1_1_Click(object eventSender, EventArgs eventArgs)
    {
        Picture1.Image = null;
        _Label2_0.Tag = 0;
        GroupBox1.Top = 0;
        Drive1.Text = Modul1.Verz;
        Dir1.Text = Modul1.PictureDir;
        Drive1.Visible = true;
        Dir1.Visible = true;
        File1.Visible = true;
        _viewModel.StartAddPicture();
    }

    private void Command1_0_Click(object sender, EventArgs e)
    {
        List1.Visible = false;
        Picture1.Image = null;
        _viewModel.CloseAndRefresh();
    }

    private void Command2_Click(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.Typ == DriveType.CDRom)
        {
            _ = Interaction.MsgBox(Modul1.Message_sNoChangesOnCD, title: "", icon: MessageBoxIcon.Information);
            return;
        }

        _viewModel.SaveAsPersonPicture(BuildStoredPath(), GetSelectedFileName());
        Text2.Text = "Personenbild";
        _ = Interaction.MsgBox("Verknüpfung / Änderung wurde gespeichert");
    }
    private void Dir1_Change(object eventSender, EventArgs eventArgs)
    {
        File1.Items.Clear();
        foreach (var item in (IList)Dir1.SelectedItems)
        {
            File1.Items.Add(item);
        }
    }

    private void Drive1_SelectedIndexChanged(object eventSender, EventArgs eventArgs)
    {
        Dir1.Text = Drive1.Text;
    }

    private void File1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_04da, IL_0578
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        string text = default;
        int number = default;
        FileStream fileStream = default;
        PictureBox pictureBox = default;
        string text2 = default;
        PictureBox pictureBox2 = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                Bitmap bitmap;
                switch (try0001_dispatch)
                {
                    default:
                        num = 1;
                        A = Dir1.Text;
                        goto IL_0015;
                    case 1710:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                case 3:
                                    break;
                                case 1:
                                    goto IL_057c;
                                default:
                                    goto end_IL_0001;
                            }
                            number = Information.Err().Number;
                            if (number == 481)
                            {
                                _ = Interaction.MsgBox("Das Bild " + A + " hat ein unbekanntes Format");
                                goto end_IL_0001_2;
                            }
                            else
                            {
                                if (Interaction.MsgBox(Conversion.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                                {
                                    ProjectData.EndApp();
                                }
                                ProjectData.ClearProjectError();
                                if (num2 == 0)
                                {
                                    throw ProjectData.CreateProjectError(-2146828268);
                                }
                                goto IL_057c;
                            }
                        }
                    end_IL_0001:
                        break;
                    IL_0015:
                        num = 2;
                        if (A.Right(1) != "\\")
                        {
                            A += "\\";
                        }
                        A += File1.Items[File1.SelectedIndex].AsString();
                        Bildname = File1.Items[File1.SelectedIndex].AsString();
                        Frame1.Visible = true;
                        fileStream = new FileStream(A, FileMode.Open);
                        text2 = A.Right(3).ToUpper();
                        switch (text2)
                        {
                            case "BMP":
                            case "JPG":
                            case "PEG":
                            case "GIF":
                            case "TIF":
                            case "PNG":
                                break;
                            default:
                                goto IL_026f;
                        }
                        if (true)
                        {
                            Label3.Visible = true;
                            Picture1.Visible = true;
                            Picture1.BackColor = BackColor;
                            Picture1.Width = GroupBox1.Left;
                            Picture1.Height = checked(Command1[0].Top - 10);
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            RTB.Text = "";
                            Text2.Text = "";
                            bitmap = new Bitmap(fileStream);
                            Picture1.Image = new Bitmap(fileStream);
                            pictureBox2 = Picture1;
                            pictureBox2.Image = Modul1.AutoSizeImage(new Bitmap(fileStream), pictureBox2.ClientRectangle.Width, pictureBox2.ClientRectangle.Height, bStretch: true);
                            pictureBox2 = null;
                            fileStream.Close();
                            goto end_IL_0001_2;
                        }
                    IL_026f:
                        num = 29;
                        switch (text2)
                        {
                            case "PDF":
                            case "FLV":
                            case "MP3":
                            case "MP4":
                            case "CSV":
                            case "VOB":
                            case "TXT":
                            case "XLS":
                            case "HTM":
                            case "HTML":
                                break;
                            default:
                                goto IL_03b2;
                        }
                        if (true)
                        {
                            fileStream.Close();
                            Label5.Text = "Die Datei ";
                            Label6.Text = A;
                            text = $"kann von {Modul1.AppName} nicht angezeigt werden.\nDie Datei wird mit dem auf Ihrem System vorhandenen Programm im Vollbildschirm geöffnet.\n";
                            text += "Um die Datei zu verknüpfen, muss die Anzeige wieder geschlossen werden.\n";
                            text += "Alternativ können Sie die Datei auch ohne vorherige Anzeige verknüpfen.";
                            Label4.Text = text;
                            Panel1.Visible = true;
                            goto end_IL_0001_2;
                        }
                    IL_03b2:
                        num = 41;
                        _ = Process.Start(A);
                        goto end_IL_0001_2;
                    IL_057c:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 10:
                            case 28:
                            case 39:
                            case 43:
                            case 44:
                                num = 44;
                                Picture1.Visible = true;
                                Picture1.BackColor = BackColor;
                                Picture1.Width = GroupBox1.Left;
                                Picture1.Height = checked(Command1[0].Top - 10);
                                goto case 48;
                            case 48:
                                ProjectData.ClearProjectError();
                                num3 = 3;
                                RTB.Text = "";
                                Text2.Text = "";
                                bitmap = new Bitmap(fileStream);
                                Picture1.Image = new Bitmap(fileStream);
                                pictureBox = Picture1;
                                pictureBox.Image = Modul1.AutoSizeImage(new Bitmap(fileStream), pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height, bStretch: true);
                                goto case 55;
                            case 55:
                                pictureBox = null;
                                fileStream.Close();
                                goto end_IL_0001_2;
                            case 27:
                            case 38:
                            case 42:
                            case 57:
                            case 59:
                            case 63:
                            case 64:
                            case 70:
                            case 71:
                            case 72:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0001_dispatch = 1710;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 6
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }
    private void Bilder_Load(object eventSender, EventArgs eventArgs)
    {
        if (Modul1.FontSize > 0f)
        {
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Command1[0].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Command1[1].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Command1[2].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Command1[3].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Command1[4].Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Command2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Dir1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Drive1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            File1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            List1.Font = new Font("courier new", Modul1.FontSize, FontStyle.Regular);
            Frame1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Text2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            _Label2_1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            RTB.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }

        Command1[0].Text = Modul1.IText[EUserText.tNMBack];
        Modul1.Persistence.ReadEnumInit<FormWindowState>("Windowstate", out var wiS);
        WindowState = wiS;
        BackColor = Modul1.HintFarb;
        _Label2_0.Tag = 0;
        Drive1.Text = Modul1.Verz;
        Dir1.Text = Modul1.PictureDir;
        _viewModel.Load();
    }

    private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
    {
        //Discarded unreachable code: IL_0486, IL_063c
        int try0001_dispatch = -1;
        int num = default;
        int num2 = default;
        int num3 = default;
        PictureBox pictureBox = default;
        int lErl = default;
        Bitmap original = default;
        string prompt = default;
        string text = default;
        string text2 = default;
        FileStream fileStream = default;

        int num4;

        num = 1;
        Label3.Visible = true;
    //    goto IL_0011;
    //case 2232:
    //    {
    //        num2 = num;
    //        switch ((num3 <= -2) ? 1 : num3)
    //        {
    //            case 3:
    //                break;
    //            case 2:
    //                goto IL_063e;
    //            case 1:
    //                goto IL_0716;
    //            default:
    //                goto end_IL_0001;
    //        }
    //        if (Information.Err().Number == 13)
    //        {
    //            if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
    //            {
    //                ProjectData.EndApp();
    //            }
    //            goto end_IL_0001_2;
    //        }
    //        else
    //        {
    //            if (Information.Err().Number == 53)
    //            {
    //                prompt = "Das Bild " + text + " wurde nicht gefunden.\n\n Soll die Bildverknüpfung gelöscht werden?";
    //                Modul1.Value = (float)Interaction.MsgBox(prompt, mb: MessageBoxButtons.YesNo | MessageBoxButtons.Information, icon: "");
    //                if (Modul1.Value != 6f)
    //                {
    //                    goto end_IL_0001_2;
    //                }
    //                DataModul.DB_PictureTable.Delete();
    //                goto end_IL_0001_2;
    //            }
    //            else
    //            {
    //                if (Information.Err().Number == 71)
    //                {
    //                    _ = Interaction.MsgBox(Conversions.ErrorToString());
    //                    goto end_IL_0001_2;
    //                }
    //                else
    //                {
    //                    if (Information.Err().Number == 76)
    //                    {
    //                        prompt = "Das Bild " + text + " wurde nicht gefunden.\n\n Soll die Bildverknüpfung gelöscht werden?";
    //                        var Modul1_Value = (float)Interaction.MsgBox(prompt, title: "", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Information);
    //                        if (Modul1_Value != 6f)
    //                        {
    //                            goto end_IL_0001_2;
    //                        }
    //                        DataModul.DB_PictureTable.Delete();
    //                        goto end_IL_0001_2;
    //                    }
    //                    else
    //                    {
    //                        if (Interaction.MsgBox(Conversions.ErrorToString(), title: Information.Err().Number.AsString(), mb: MessageBoxButtons.OKCancel) == DialogResult.Cancel)
    //                        {
    //                            ProjectData.EndApp();
    //                        }
    //                        ProjectData.ClearProjectError();
    //                        if (num2 == 0)
    //                        {
    //                            throw ProjectData.CreateProjectError(-2146828268);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        goto IL_0712;
    //    }
    //end_IL_0001:
    //    break;
   // IL_0011:
        num = 2;
        Picture1.BackColor = BackColor;
        Picture1.Width = checked(Width - 10);
        Picture1.Height = checked(Command1[0].Top - 10);
        if (List1.SelectedItem is BilderListItem selectedPicture)
        {
            _viewModel.SelectedPictureItem = selectedPicture;
            Modul1.LfNR = (short)selectedPicture.RecordNumber;
            _Label2_0.Tag = Modul1.LfNR;
            text = selectedPicture.StoredPath + selectedPicture.FileName;
        }
        else
        {
            return;
        }

        ProjectData.ClearProjectError();
        num3 = 2;
        if (text.Left(1) == "#")
        {
            text = Modul1.Verz + Strings.Mid(text, 2, text.Length);
        }
        Label1.Text = text;
        Bildname = text;
        text2 = text.Right(3).ToUpper();
        switch (text2)
        {
            case "PDF":
            case "FLV":
            case "MP3":
            case "MP4":
            case "CSV":
            case "VOB":
            case "TXT":
            case "XLS":
            case "HTM":
            case "HTML":
                _ = Process.Start(text);
                Frame1.Visible = true;
                goto end_IL_0001_2;
            default:
                break;
        }
        goto IL_0304;
    IL_0304:
        ProjectData.ClearProjectError();
        num3 = 3;
        //Br = 10f;
        if (Modul1.Typ != DriveType.CDRom)
        {
            fileStream = new FileStream(text, FileMode.Open);
            original = new Bitmap(fileStream);
            fileStream.Close();
        }
        else
        {
            original = new Bitmap(text);
        }
        pictureBox = Picture1;
        pictureBox.Image = Modul1.AutoSizeImage(new Bitmap(original), pictureBox.ClientRectangle.Width, pictureBox.ClientRectangle.Height, bStretch: true);
        pictureBox = null;
        lErl = 3;
        Picture1.Visible = true;
        Frame1.Visible = true;
        if (null != DataModul.DB_PictureTable.Fields[PictureFields.Bem].Value)
        {
            RTB.Text = DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString();
        }
        goto end_IL_0001_2;
    end_IL_0001_2: // <========== 11
        return;
    }

    private void Text2_Enter(object eventSender, EventArgs eventArgs)
    {
    }

    private void Timer1_Tick(object eventSender, EventArgs eventArgs)
    {
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        Frame1.Visible = false;
    }

    private void RTB_KeyDown(object sender, KeyEventArgs e)
    {
        checked
        {
            short num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
            Modul1.Trans = 1;
            if (num2 == 0)
            {
                switch (num)
                {
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
                        RTB.SelectedText = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    private void Text2_KeyDown(object sender, KeyEventArgs e)
    {
        checked
        {
            short num = (short)e.KeyCode;
            short num2 = (short)unchecked((int)e.KeyData / 65536);
            Modul1.Trans = 1;
            if (num2 == 0)
            {
                switch (num)
                {
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
                        Text2.Text = Modul1.Te[num - 113];
                        break;
                }
            }
        }
    }

    private void Picture1_Click(object sender, EventArgs e)
    {
        _ = Process.Start(Bildname);
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        _ = Process.Start(A);
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        Panel1.Visible = false;
        _ = Text2.Focus();
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        Text2.Text = Bildname;
    }

    private void _Command1_0_Click(object sender, EventArgs e)
    {
    }

    private void File1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    internal DialogResult ShowDialog(int v1, string v2)
    {
        throw new NotImplementedException();
    }

    private string BuildStoredPath()
    {
        string path = Dir1.SelectedItem?.ItemData<string>() ?? Dir1.Text;
        if (Strings.InStr(path.ToUpper(), Modul1.Verz.ToUpper()) != 0)
        {
            path = "#" + Strings.Mid(path, Modul1.Verz.Length + 1, path.Length);
        }

        if (path.Right(1) != "\\")
        {
            path += "\\";
        }

        return path;
    }

    private string GetSelectedFileName()
    {
        if (File1.SelectedItem is null)
        {
            return string.Empty;
        }

        return File1.SelectedItem.AsString();
    }

    private void RefreshOwner()
    {
        string kennz = Modul1.sPKennz;
        if (kennz == "P")
        {
            Modul1.PersInArb = Modul1.Ubg;
            Personen.Default.Perzeig(Modul1.PersInArb);
        }
        else if (kennz == "F")
        {
            Modul1.FamInArb = Modul1.Ubg;
            short rich;
            Familie.Default.Fameinlesen(Modul1.FamInArb, out rich);
        }
        else if (kennz == "Q")
        {
            var quellverw = MainProject.Forms.Quellverw;
            var satznr = (long)MainProject.Forms.Quellverw._Label1_13.Tag.AsInt();
            quellverw.ViewModel.Les1(satznr, Rich: false);
        }
    }
}
