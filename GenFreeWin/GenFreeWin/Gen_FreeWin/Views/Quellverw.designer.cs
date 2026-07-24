using BaseLib.Helper;
using GenFreeWin.Main;
using GenFreeWin.ViewModels;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Views;

namespace GenFreeWin;

public partial class Quellverw 
{
    private IContainer components;

    public ToolTip ToolTip1;
    private ListBox _List2;
    private Button __Command1_9;
    private Button _Command2;
    public RadioButton _Option1_0;
    public TextBox Text2;
    public ListBox List1;
    public RichTextBox RTB1;
    public ControlArray<Button> ACommand1;
    public ControlArray<Button> ACommand3;
    public ControlArray<Label> ALabel1;
    public ControlArray<RadioButton> AOption1;
    public ControlArray<RadioButton> AOption2;
    public ControlArray<TextBox> AText1 = new();
    public TextBox TextBox1;
    public Button btnStart;
    internal ComboBox ComboBox1;

    internal TextBox frmSrch_edtSearch;
    internal ListBox frmSrch_ListBox1;
    internal Button frmSrch_btnClose;
    internal Button frmSrch_btnDeleteEntry;
    internal Button frmSrch_btnNewEntry;
    
    internal Label Label7;
   internal Button btnHometown;
    public Button btnClose2;
    internal PictureBox PictureBox1;
    internal ComboBox ComboBox2;
    public RadioButton _Option2_1;
    public RadioButton _Option2_2;
    public RadioButton _Option2_0;
    public Button _Command3_2;
    public Button _Command3_1;
    public Button _Command3_0;
    public RichTextBox RTB2;
    
    public GroupBox Frame3;
    public ListBox List3;
    public Button _Command1_14;
    public ListBox List5;
    public ListBox List4;
    public Button _Command1_11;
    public Button _Command1_10;
    public ListBox List2;

    public GroupBox Frame2;
    public Button _Command1_9;

    public Button Command2;

    public RadioButton _Option1_1;

    [VisibilityBinding(nameof(IQuellVerwViewModel.Frame1_Visible))]
    public GroupBox Frame1;
    public Button _Command1_8;
    public Button _Command1_7;
    public Button _Command1_6;
    public Button _Command1_5;
    public Button _Command1_4;
    public Button _Command1_3;
    public Button _Command1_2;
    public Button _Command1_1;
    

    public Button _Command1_0;
    public TextBox _Text1_10;
    public TextBox _Text1_9;
    public TextBox _Text1_8;
    public TextBox _Text1_7;
    public TextBox _Text1_6;
    public TextBox _Text1_5;
    public TextBox _Text1_3;
    public TextBox _Text1_2;
    public TextBox _Text1_1;
    public TextBox _Text1_0;
    public Button _Command1_12;
    public Label _Label1_13;
    public Label _Label1_12;
    public Label _Label1_11;
    public Label _Label1_9;
    public Label _Label1_8;
    public Label _Label1_7;
    public Label _Label1_6;
    public Label _Label1_5;
    public Label _Label1_4;
    public Label _Label1_3;
    public Label _Label1_2;
    public Label _Label1_1;
    public Label _Label1_0;
    internal ProgressBar ProgressBar1;
    internal Label Label2;
    internal CheckBox CheckBox1;
    internal Label Label4;
    internal Label Label3;
    internal GroupBox LagerFrame;
    internal Label frmSrch_Label5;
    public RadioButton RadioButton1;
    internal Label Label6;
    internal Label Label8;

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
            this.Text2 = new System.Windows.Forms.TextBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Frame3 = new System.Windows.Forms.GroupBox();
            this._Option2_1 = new System.Windows.Forms.RadioButton();
            this._Option2_2 = new System.Windows.Forms.RadioButton();
            this._Option2_0 = new System.Windows.Forms.RadioButton();
            this._Command3_2 = new System.Windows.Forms.Button();
            this._Command3_1 = new System.Windows.Forms.Button();
            this._Command3_0 = new System.Windows.Forms.Button();
            this.RTB2 = new System.Windows.Forms.RichTextBox();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Command2 = new System.Windows.Forms.Button();
            this._Option1_1 = new System.Windows.Forms.RadioButton();
            this._Option1_0 = new System.Windows.Forms.RadioButton();
            this.List1 = new System.Windows.Forms.ListBox();
            this.Frame2 = new System.Windows.Forms.GroupBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.List3 = new System.Windows.Forms.ListBox();
            this._Command1_14 = new System.Windows.Forms.Button();
            this.List5 = new System.Windows.Forms.ListBox();
            this.List4 = new System.Windows.Forms.ListBox();
            this._Command1_11 = new System.Windows.Forms.Button();
            this._Command1_10 = new System.Windows.Forms.Button();
            this.List2 = new System.Windows.Forms.ListBox();
            this._Command1_9 = new System.Windows.Forms.Button();
            this._Command1_8 = new System.Windows.Forms.Button();
            this._Command1_7 = new System.Windows.Forms.Button();
            this._Command1_6 = new System.Windows.Forms.Button();
            this._Command1_5 = new System.Windows.Forms.Button();
            this._Command1_4 = new System.Windows.Forms.Button();
            this._Command1_3 = new System.Windows.Forms.Button();
            this._Command1_2 = new System.Windows.Forms.Button();
            this._Command1_1 = new System.Windows.Forms.Button();
            this.RTB1 = new System.Windows.Forms.RichTextBox();
            this._Command1_0 = new System.Windows.Forms.Button();
            this._Text1_10 = new System.Windows.Forms.TextBox();
            this._Text1_9 = new System.Windows.Forms.TextBox();
            this._Text1_8 = new System.Windows.Forms.TextBox();
            this._Text1_7 = new System.Windows.Forms.TextBox();
            this._Text1_6 = new System.Windows.Forms.TextBox();
            this._Text1_5 = new System.Windows.Forms.TextBox();
            this._Text1_3 = new System.Windows.Forms.TextBox();
            this._Text1_2 = new System.Windows.Forms.TextBox();
            this._Text1_1 = new System.Windows.Forms.TextBox();
            this._Text1_0 = new System.Windows.Forms.TextBox();
            this._Command1_12 = new System.Windows.Forms.Button();
            this._Label1_13 = new System.Windows.Forms.Label();
            this._Label1_12 = new System.Windows.Forms.Label();
            this._Label1_11 = new System.Windows.Forms.Label();
            this._Label1_9 = new System.Windows.Forms.Label();
            this._Label1_8 = new System.Windows.Forms.Label();
            this._Label1_7 = new System.Windows.Forms.Label();
            this._Label1_6 = new System.Windows.Forms.Label();
            this._Label1_5 = new System.Windows.Forms.Label();
            this._Label1_4 = new System.Windows.Forms.Label();
            this._Label1_3 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.LagerFrame = new System.Windows.Forms.GroupBox();
            this.frmSrch_Label5 = new System.Windows.Forms.Label();
            this.frmSrch_btnClose = new System.Windows.Forms.Button();
            this.frmSrch_btnDeleteEntry = new System.Windows.Forms.Button();
            this.frmSrch_btnNewEntry = new System.Windows.Forms.Button();
            this.frmSrch_edtSearch = new System.Windows.Forms.TextBox();
            this.frmSrch_ListBox1 = new System.Windows.Forms.ListBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.btnHometown = new System.Windows.Forms.Button();
            this.btnClose2 = new System.Windows.Forms.Button();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.ComboBox2 = new System.Windows.Forms.ComboBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.Frame3.SuspendLayout();
            this.Frame1.SuspendLayout();
            this.Frame2.SuspendLayout();
            this.LagerFrame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Text2
            // 
            this.Text2.AcceptsReturn = true;
            this.Text2.BackColor = System.Drawing.SystemColors.Window;
            this.Text2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text2.Location = new System.Drawing.Point(86, 58);
            this.Text2.Margin = new System.Windows.Forms.Padding(4);
            this.Text2.MaxLength = 0;
            this.Text2.Name = "Text2";
            this.Text2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text2.Size = new System.Drawing.Size(368, 27);
            this.Text2.TabIndex = 37;
            this.ToolTip1.SetToolTip(this.Text2, "Eingabe des Suchbegriffs");
            this.Text2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text2_KeyDown);
            // 
            // edtPlace
            // 
            this.TextBox1.AcceptsReturn = true;
            this.TextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.TextBox1.Location = new System.Drawing.Point(86, 88);
            this.TextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.TextBox1.MaxLength = 0;
            this.TextBox1.Name = "edtPlace";
            this.TextBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TextBox1.Size = new System.Drawing.Size(368, 27);
            this.TextBox1.TabIndex = 41;
            this.ToolTip1.SetToolTip(this.TextBox1, "Eingabe des Suchbegriffs");
            this.TextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            // 
            // Frame3
            // 
            this.Frame3.BackColor = System.Drawing.SystemColors.Control;
            this.Frame3.Controls.Add(this._Option2_1);
            this.Frame3.Controls.Add(this._Option2_2);
            this.Frame3.Controls.Add(this.LagerFrame);
            this.Frame3.Controls.Add(this._Option2_0);
            this.Frame3.Controls.Add(this._Command3_2);
            this.Frame3.Controls.Add(this._Command3_1);
            this.Frame3.Controls.Add(this._Command3_0);
            this.Frame3.Controls.Add(this.RTB2);
            this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame3.Location = new System.Drawing.Point(35, 719);
            this.Frame3.Margin = new System.Windows.Forms.Padding(4);
            this.Frame3.Name = "Frame3";
            this.Frame3.Padding = new System.Windows.Forms.Padding(4);
            this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame3.Size = new System.Drawing.Size(795, 596);
            this.Frame3.TabIndex = 49;
            this.Frame3.TabStop = false;
            this.Frame3.Text = "frmPicture";
            this.Frame3.Visible = false;
            // 
            // _Option2_1
            // 
            this._Option2_1.BackColor = System.Drawing.SystemColors.Control;
            this._Option2_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option2_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option2_1.Location = new System.Drawing.Point(714, 665);
            this._Option2_1.Margin = new System.Windows.Forms.Padding(4);
            this._Option2_1.Name = "_Option2_1";
            this._Option2_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option2_1.Size = new System.Drawing.Size(111, 22);
            this._Option2_1.TabIndex = 57;
            this._Option2_1.TabStop = true;
            this._Option2_1.Text = "Nach Autor";
            this._Option2_1.UseVisualStyleBackColor = false;
            // 
            // _Option2_2
            // 
            this._Option2_2.BackColor = System.Drawing.SystemColors.Control;
            this._Option2_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option2_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option2_2.Location = new System.Drawing.Point(409, 665);
            this._Option2_2.Margin = new System.Windows.Forms.Padding(4);
            this._Option2_2.Name = "_Option2_2";
            this._Option2_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option2_2.Size = new System.Drawing.Size(137, 22);
            this._Option2_2.TabIndex = 56;
            this._Option2_2.TabStop = true;
            this._Option2_2.Text = "Nach \"Zitiert als\"";
            this._Option2_2.UseVisualStyleBackColor = false;
            // 
            // _Option2_0
            // 
            this._Option2_0.BackColor = System.Drawing.SystemColors.Control;
            this._Option2_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option2_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option2_0.Location = new System.Drawing.Point(579, 665);
            this._Option2_0.Margin = new System.Windows.Forms.Padding(4);
            this._Option2_0.Name = "_Option2_0";
            this._Option2_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option2_0.Size = new System.Drawing.Size(97, 22);
            this._Option2_0.TabIndex = 55;
            this._Option2_0.TabStop = true;
            this._Option2_0.Text = "Nach Titel";
            this._Option2_0.UseVisualStyleBackColor = false;
            // 
            // _Command3_2
            // 
            this._Command3_2.BackColor = System.Drawing.SystemColors.Control;
            this._Command3_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command3_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command3_2.Location = new System.Drawing.Point(298, 661);
            this._Command3_2.Margin = new System.Windows.Forms.Padding(4);
            this._Command3_2.Name = "_Command3_2";
            this._Command3_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command3_2.Size = new System.Drawing.Size(65, 30);
            this._Command3_2.TabIndex = 54;
            this._Command3_2.Text = "Start";
            this._Command3_2.UseVisualStyleBackColor = false;
            // 
            // _Command3_1
            // 
            this._Command3_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command3_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command3_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command3_1.Location = new System.Drawing.Point(199, 661);
            this._Command3_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command3_1.Name = "_Command3_1";
            this._Command3_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command3_1.Size = new System.Drawing.Size(71, 30);
            this._Command3_1.TabIndex = 53;
            this._Command3_1.UseVisualStyleBackColor = false;
            // 
            // _Command3_0
            // 
            this._Command3_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command3_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command3_0.Enabled = false;
            this._Command3_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command3_0.Location = new System.Drawing.Point(66, 661);
            this._Command3_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command3_0.Name = "_Command3_0";
            this._Command3_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command3_0.Size = new System.Drawing.Size(100, 30);
            this._Command3_0.TabIndex = 52;
            this._Command3_0.Text = "Ausdrucken";
            this._Command3_0.UseVisualStyleBackColor = false;
            // 
            // RTB2
            // 
            this.RTB2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTB2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.RTB2.Location = new System.Drawing.Point(18, 24);
            this.RTB2.Margin = new System.Windows.Forms.Padding(4);
            this.RTB2.Name = "RTB2";
            this.RTB2.Size = new System.Drawing.Size(916, 629);
            this.RTB2.TabIndex = 50;
            this.RTB2.Text = "";
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.SystemColors.Control;
            this.Frame1.Controls.Add(this.RadioButton1);
            this.Frame1.Controls.Add(this.btnStart);
            this.Frame1.Controls.Add(this.Label4);
            this.Frame1.Controls.Add(this.Label3);
            this.Frame1.Controls.Add(this.TextBox1);
            this.Frame1.Controls.Add(this.Command2);
            this.Frame1.Controls.Add(this._Option1_1);
            this.Frame1.Controls.Add(this._Option1_0);
            this.Frame1.Controls.Add(this.Text2);
            this.Frame1.Controls.Add(this.List1);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1.Location = new System.Drawing.Point(812, 37);
            this.Frame1.Margin = new System.Windows.Forms.Padding(4);
            this.Frame1.Name = "Frame1";
            this.Frame1.Padding = new System.Windows.Forms.Padding(4);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(464, 632);
            this.Frame1.TabIndex = 35;
            this.Frame1.TabStop = false;
            this.Frame1.Visible = false;
            // 
            // RadioButton1
            // 
            this.RadioButton1.BackColor = System.Drawing.SystemColors.Control;
            this.RadioButton1.Cursor = System.Windows.Forms.Cursors.Default;
            this.RadioButton1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton1.Location = new System.Drawing.Point(24, 34);
            this.RadioButton1.Margin = new System.Windows.Forms.Padding(4);
            this.RadioButton1.Name = "RadioButton1";
            this.RadioButton1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RadioButton1.Size = new System.Drawing.Size(140, 22);
            this.RadioButton1.TabIndex = 45;
            this.RadioButton1.TabStop = true;
            this.RadioButton1.Text = "Nach Autor";
            this.RadioButton1.UseVisualStyleBackColor = false;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Lime;
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStart.Location = new System.Drawing.Point(276, 10);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnStart.Size = new System.Drawing.Size(60, 22);
            this.btnStart.TabIndex = 44;
            this.btnStart.Text = "Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(7, 61);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(90, 19);
            this.Label4.TabIndex = 43;
            this.Label4.Text = "Beginnt mit";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(21, 90);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(58, 19);
            this.Label3.TabIndex = 42;
            this.Label3.Text = "Enthält";
            // 
            // btnEdit
            // 
            this.Command2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Command2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Command2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command2.Location = new System.Drawing.Point(357, 10);
            this.Command2.Margin = new System.Windows.Forms.Padding(4);
            this.Command2.Name = "Command2";
            this.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Command2.Size = new System.Drawing.Size(104, 22);
            this.Command2.TabIndex = 40;
            this.Command2.Text = "Schliessen";
            this.Command2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Command2.UseVisualStyleBackColor = false;
            // 
            // _Option1_1
            // 
            this._Option1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_1.Location = new System.Drawing.Point(128, 13);
            this._Option1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Option1_1.Name = "_Option1_1";
            this._Option1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_1.Size = new System.Drawing.Size(140, 22);
            this._Option1_1.TabIndex = 39;
            this._Option1_1.TabStop = true;
            this._Option1_1.Text = "Nach \"Zitiert als\"";
            this._Option1_1.UseVisualStyleBackColor = false;
            // 
            // _Option1_0
            // 
            this._Option1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_0.Checked = true;
            this._Option1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_0.Location = new System.Drawing.Point(24, 13);
            this._Option1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Option1_0.Name = "_Option1_0";
            this._Option1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_0.Size = new System.Drawing.Size(105, 22);
            this._Option1_0.TabIndex = 38;
            this._Option1_0.TabStop = true;
            this._Option1_0.Text = "Nach Titel";
            this._Option1_0.UseVisualStyleBackColor = false;
            this._Option1_0.CheckedChanged += new System.EventHandler(this._Option1_0_CheckedChanged);
            // 
            // List1
            // 
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 19;
            this.List1.Location = new System.Drawing.Point(37, 115);
            this.List1.Margin = new System.Windows.Forms.Padding(4);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List1.Size = new System.Drawing.Size(419, 479);
            this.List1.TabIndex = 36;
            this.List1.DoubleClick += new System.EventHandler(this.List1_DoubleClick);
            // 
            // Frame2
            // 
            this.Frame2.BackColor = System.Drawing.SystemColors.Control;
            this.Frame2.Controls.Add(this.Label2);
            this.Frame2.Controls.Add(this.CheckBox1);
            this.Frame2.Controls.Add(this.ProgressBar1);
            this.Frame2.Controls.Add(this.List3);
            this.Frame2.Controls.Add(this._Command1_14);
            this.Frame2.Controls.Add(this.List5);
            this.Frame2.Controls.Add(this.List4);
            this.Frame2.Controls.Add(this._Command1_11);
            this.Frame2.Controls.Add(this._Command1_10);
            this.Frame2.Controls.Add(this.List2);
            this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame2.Location = new System.Drawing.Point(992, 633);
            this.Frame2.Margin = new System.Windows.Forms.Padding(4);
            this.Frame2.Name = "Frame2";
            this.Frame2.Padding = new System.Windows.Forms.Padding(4);
            this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame2.Size = new System.Drawing.Size(292, 398);
            this.Frame2.TabIndex = 42;
            this.Frame2.TabStop = false;
            this.Frame2.Text = "Frame2";
            this.Frame2.Visible = false;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(575, 668);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(0, 19);
            this.Label2.TabIndex = 63;
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.Location = new System.Drawing.Point(763, 661);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(183, 23);
            this.CheckBox1.TabIndex = 62;
            this.CheckBox1.Text = "Auswahl beibehalten";
            this.CheckBox1.UseVisualStyleBackColor = true;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(18, 696);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(873, 17);
            this.ProgressBar1.TabIndex = 61;
            // 
            // List3
            // 
            this.List3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.List3.Cursor = System.Windows.Forms.Cursors.Default;
            this.List3.ForeColor = System.Drawing.Color.Black;
            this.List3.ItemHeight = 19;
            this.List3.Location = new System.Drawing.Point(27, 332);
            this.List3.Margin = new System.Windows.Forms.Padding(4);
            this.List3.Name = "List3";
            this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List3.Size = new System.Drawing.Size(248, 42);
            this.List3.Sorted = true;
            this.List3.TabIndex = 44;
            this.List3.Visible = false;
            // 
            // _Command1_14
            // 
            this._Command1_14.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_14.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_14.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_14.Location = new System.Drawing.Point(18, 662);
            this._Command1_14.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_14.Name = "_Command1_14";
            this._Command1_14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_14.Size = new System.Drawing.Size(235, 27);
            this._Command1_14.TabIndex = 60;
            this._Command1_14.Text = "Speichern für Gedcom-Ausgabe";
            this._Command1_14.UseVisualStyleBackColor = false;
            // 
            // List5
            // 
            this.List5.BackColor = System.Drawing.SystemColors.Window;
            this.List5.Cursor = System.Windows.Forms.Cursors.Default;
            this.List5.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List5.ItemHeight = 19;
            this.List5.Location = new System.Drawing.Point(387, 562);
            this.List5.Margin = new System.Windows.Forms.Padding(4);
            this.List5.Name = "List5";
            this.List5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List5.Size = new System.Drawing.Size(93, 4);
            this.List5.Sorted = true;
            this.List5.TabIndex = 48;
            this.List5.Visible = false;
            // 
            // List4
            // 
            this.List4.BackColor = System.Drawing.SystemColors.Window;
            this.List4.Cursor = System.Windows.Forms.Cursors.Default;
            this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List4.ItemHeight = 19;
            this.List4.Location = new System.Drawing.Point(491, 552);
            this.List4.Margin = new System.Windows.Forms.Padding(4);
            this.List4.Name = "List4";
            this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List4.Size = new System.Drawing.Size(272, 42);
            this.List4.Sorted = true;
            this.List4.TabIndex = 47;
            this.List4.Visible = false;
            // 
            // _Command1_11
            // 
            this._Command1_11.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_11.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_11.Location = new System.Drawing.Point(268, 662);
            this._Command1_11.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_11.Name = "_Command1_11";
            this._Command1_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_11.Size = new System.Drawing.Size(108, 27);
            this._Command1_11.TabIndex = 46;
            this._Command1_11.Text = "Drucken";
            this._Command1_11.UseVisualStyleBackColor = false;
            // 
            // _Command1_10
            // 
            this._Command1_10.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_10.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_10.Location = new System.Drawing.Point(408, 662);
            this._Command1_10.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_10.Name = "_Command1_10";
            this._Command1_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_10.Size = new System.Drawing.Size(108, 27);
            this._Command1_10.TabIndex = 45;
            this._Command1_10.UseVisualStyleBackColor = false;
            // 
            // List2
            // 
            this.List2.BackColor = System.Drawing.SystemColors.Window;
            this.List2.Cursor = System.Windows.Forms.Cursors.Default;
            this.List2.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List2.ItemHeight = 20;
            this.List2.Location = new System.Drawing.Point(27, 26);
            this.List2.Margin = new System.Windows.Forms.Padding(4);
            this.List2.Name = "List2";
            this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List2.Size = new System.Drawing.Size(868, 604);
            this.List2.TabIndex = 43;
            // 
            // _Command1_9
            // 
            this._Command1_9.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_9.Location = new System.Drawing.Point(4, 649);
            this._Command1_9.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_9.Name = "_Command1_9";
            this._Command1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_9.Size = new System.Drawing.Size(108, 27);
            this._Command1_9.TabIndex = 41;
            this._Command1_9.Text = "Verwendung";
            this._Command1_9.UseVisualStyleBackColor = false;
            // 
            // _Command1_8
            // 
            this._Command1_8.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_8.Location = new System.Drawing.Point(335, 649);
            this._Command1_8.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_8.Name = "_Command1_8";
            this._Command1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_8.Size = new System.Drawing.Size(124, 27);
            this._Command1_8.TabIndex = 34;
            this._Command1_8.Text = "Eintrag kopieren";
            this._Command1_8.UseVisualStyleBackColor = false;
            // 
            // _Command1_7
            // 
            this._Command1_7.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_7.Enabled = false;
            this._Command1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_7.Location = new System.Drawing.Point(234, 650);
            this._Command1_7.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_7.Name = "_Command1_7";
            this._Command1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_7.Size = new System.Drawing.Size(86, 27);
            this._Command1_7.TabIndex = 33;
            this._Command1_7.Text = "Einfügen";
            this._Command1_7.UseVisualStyleBackColor = false;
            // 
            // _Command1_6
            // 
            this._Command1_6.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_6.Location = new System.Drawing.Point(852, 702);
            this._Command1_6.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_6.Name = "_Command1_6";
            this._Command1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_6.Size = new System.Drawing.Size(108, 27);
            this._Command1_6.TabIndex = 32;
            this._Command1_6.UseVisualStyleBackColor = false;
            this._Command1_6.Visible = false;
            // 
            // _Command1_5
            // 
            this._Command1_5.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_5.Location = new System.Drawing.Point(350, 614);
            this._Command1_5.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_5.Name = "_Command1_5";
            this._Command1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_5.Size = new System.Drawing.Size(108, 27);
            this._Command1_5.TabIndex = 31;
            this._Command1_5.Text = "Rückblättern";
            this._Command1_5.UseVisualStyleBackColor = false;
            // 
            // _Command1_4
            // 
            this._Command1_4.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_4.Location = new System.Drawing.Point(234, 614);
            this._Command1_4.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_4.Name = "_Command1_4";
            this._Command1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_4.Size = new System.Drawing.Size(108, 27);
            this._Command1_4.TabIndex = 30;
            this._Command1_4.Text = "Vorblättern";
            this._Command1_4.UseVisualStyleBackColor = false;
            // 
            // _Command1_3
            // 
            this._Command1_3.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_3.Location = new System.Drawing.Point(2, 614);
            this._Command1_3.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_3.Name = "_Command1_3";
            this._Command1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_3.Size = new System.Drawing.Size(108, 27);
            this._Command1_3.TabIndex = 29;
            this._Command1_3.Text = "Suchen";
            this._Command1_3.UseVisualStyleBackColor = false;
            // 
            // _Command1_2
            // 
            this._Command1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_2.Location = new System.Drawing.Point(118, 650);
            this._Command1_2.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_2.Name = "_Command1_2";
            this._Command1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_2.Size = new System.Drawing.Size(108, 27);
            this._Command1_2.TabIndex = 28;
            this._Command1_2.Text = "löschen";
            this._Command1_2.UseVisualStyleBackColor = false;
            // 
            // _Command1_1
            // 
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_1.Location = new System.Drawing.Point(118, 614);
            this._Command1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_1.Size = new System.Drawing.Size(108, 27);
            this._Command1_1.TabIndex = 27;
            this._Command1_1.Text = "Neu eingeben";
            this._Command1_1.UseVisualStyleBackColor = false;
            // 
            // RTB1
            // 
            this.RTB1.Location = new System.Drawing.Point(11, 266);
            this.RTB1.Margin = new System.Windows.Forms.Padding(4);
            this.RTB1.Name = "RTB1";
            this.RTB1.Size = new System.Drawing.Size(618, 313);
            this.RTB1.TabIndex = 25;
            this.RTB1.Text = "";
            this.RTB1.GotFocus += new System.EventHandler(this.RTB1_GotFocus);
            // 
            // _Command1_0
            // 
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_0.Location = new System.Drawing.Point(416, 684);
            this._Command1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_0.Size = new System.Drawing.Size(108, 27);
            this._Command1_0.TabIndex = 24;
            this._Command1_0.UseVisualStyleBackColor = false;
            // 
            // _Text1_10
            // 
            this._Text1_10.AcceptsReturn = true;
            this._Text1_10.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_10.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_10.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_10.Location = new System.Drawing.Point(144, 214);
            this._Text1_10.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_10.MaxLength = 0;
            this._Text1_10.Name = "_Text1_10";
            this._Text1_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_10.Size = new System.Drawing.Size(819, 20);
            this._Text1_10.TabIndex = 23;
            this._Text1_10.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_9
            // 
            this._Text1_9.AcceptsReturn = true;
            this._Text1_9.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_9.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_9.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_9.Location = new System.Drawing.Point(144, 194);
            this._Text1_9.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_9.MaxLength = 0;
            this._Text1_9.Name = "_Text1_9";
            this._Text1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_9.Size = new System.Drawing.Size(819, 20);
            this._Text1_9.TabIndex = 22;
            this._Text1_9.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_8
            // 
            this._Text1_8.AcceptsReturn = true;
            this._Text1_8.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_8.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_8.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_8.Location = new System.Drawing.Point(144, 174);
            this._Text1_8.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_8.MaxLength = 0;
            this._Text1_8.Name = "_Text1_8";
            this._Text1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_8.Size = new System.Drawing.Size(819, 20);
            this._Text1_8.TabIndex = 21;
            this._Text1_8.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_7
            // 
            this._Text1_7.AcceptsReturn = true;
            this._Text1_7.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_7.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_7.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_7.Location = new System.Drawing.Point(144, 154);
            this._Text1_7.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_7.MaxLength = 0;
            this._Text1_7.Name = "_Text1_7";
            this._Text1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_7.Size = new System.Drawing.Size(819, 20);
            this._Text1_7.TabIndex = 20;
            this._Text1_7.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_6
            // 
            this._Text1_6.AcceptsReturn = true;
            this._Text1_6.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_6.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_6.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_6.Location = new System.Drawing.Point(144, 134);
            this._Text1_6.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_6.MaxLength = 0;
            this._Text1_6.Name = "_Text1_6";
            this._Text1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_6.Size = new System.Drawing.Size(819, 20);
            this._Text1_6.TabIndex = 19;
            this._Text1_6.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_5
            // 
            this._Text1_5.AcceptsReturn = true;
            this._Text1_5.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_5.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_5.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_5.Location = new System.Drawing.Point(144, 114);
            this._Text1_5.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_5.MaxLength = 0;
            this._Text1_5.Name = "_Text1_5";
            this._Text1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_5.Size = new System.Drawing.Size(819, 20);
            this._Text1_5.TabIndex = 18;
            this._Text1_5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_3
            // 
            this._Text1_3.AcceptsReturn = true;
            this._Text1_3.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_3.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_3.Location = new System.Drawing.Point(144, 94);
            this._Text1_3.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_3.MaxLength = 0;
            this._Text1_3.Name = "_Text1_3";
            this._Text1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_3.Size = new System.Drawing.Size(819, 20);
            this._Text1_3.TabIndex = 16;
            this._Text1_3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_2
            // 
            this._Text1_2.AcceptsReturn = true;
            this._Text1_2.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_2.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_2.Location = new System.Drawing.Point(144, 74);
            this._Text1_2.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_2.MaxLength = 0;
            this._Text1_2.Name = "_Text1_2";
            this._Text1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_2.Size = new System.Drawing.Size(819, 20);
            this._Text1_2.TabIndex = 15;
            this._Text1_2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_1
            // 
            this._Text1_1.AcceptsReturn = true;
            this._Text1_1.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_1.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_1.Location = new System.Drawing.Point(157, 302);
            this._Text1_1.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_1.MaxLength = 0;
            this._Text1_1.Name = "_Text1_1";
            this._Text1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_1.Size = new System.Drawing.Size(489, 20);
            this._Text1_1.TabIndex = 14;
            this._Text1_1.Visible = false;
            this._Text1_1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Text1_0
            // 
            this._Text1_0.AcceptsReturn = true;
            this._Text1_0.BackColor = System.Drawing.SystemColors.Window;
            this._Text1_0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._Text1_0.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text1_0.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Text1_0.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text1_0.Location = new System.Drawing.Point(144, 54);
            this._Text1_0.Margin = new System.Windows.Forms.Padding(4);
            this._Text1_0.MaxLength = 0;
            this._Text1_0.Name = "_Text1_0";
            this._Text1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text1_0.Size = new System.Drawing.Size(819, 20);
            this._Text1_0.TabIndex = 13;
            this._Text1_0.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // _Command1_12
            // 
            this._Command1_12.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_12.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_12.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_12.Location = new System.Drawing.Point(466, 614);
            this._Command1_12.Margin = new System.Windows.Forms.Padding(4);
            this._Command1_12.Name = "_Command1_12";
            this._Command1_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_12.Size = new System.Drawing.Size(149, 27);
            this._Command1_12.TabIndex = 51;
            this._Command1_12.Text = "Quellenliste drucken";
            this._Command1_12.UseVisualStyleBackColor = false;
            // 
            // _Label1_13
            // 
            this._Label1_13.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_13.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_13.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_13.Location = new System.Drawing.Point(141, 29);
            this._Label1_13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_13.Name = "_Label1_13";
            this._Label1_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_13.Size = new System.Drawing.Size(135, 20);
            this._Label1_13.TabIndex = 26;
            // 
            // _Label1_12
            // 
            this._Label1_12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_12.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_12.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_12.Location = new System.Drawing.Point(0, 74);
            this._Label1_12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_12.Name = "_Label1_12";
            this._Label1_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_12.Size = new System.Drawing.Size(144, 18);
            this._Label1_12.TabIndex = 12;
            this._Label1_12.Text = "Zitiert als:";
            this._Label1_12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_11
            // 
            this._Label1_11.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_11.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_11.Location = new System.Drawing.Point(3, 29);
            this._Label1_11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_11.Name = "_Label1_11";
            this._Label1_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_11.Size = new System.Drawing.Size(137, 20);
            this._Label1_11.TabIndex = 11;
            this._Label1_11.Text = "Quellen-Nr.:";
            this._Label1_11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_9
            // 
            this._Label1_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_9.Location = new System.Drawing.Point(0, 174);
            this._Label1_9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_9.Name = "_Label1_9";
            this._Label1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_9.Size = new System.Drawing.Size(144, 18);
            this._Label1_9.TabIndex = 9;
            this._Label1_9.Text = "In:";
            this._Label1_9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_8
            // 
            this._Label1_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_8.Location = new System.Drawing.Point(0, 194);
            this._Label1_8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_8.Name = "_Label1_8";
            this._Label1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_8.Size = new System.Drawing.Size(144, 18);
            this._Label1_8.TabIndex = 8;
            this._Label1_8.Text = "Jahrgang:";
            this._Label1_8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_7
            // 
            this._Label1_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_7.Location = new System.Drawing.Point(0, 214);
            this._Label1_7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_7.Name = "_Label1_7";
            this._Label1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_7.Size = new System.Drawing.Size(144, 18);
            this._Label1_7.TabIndex = 7;
            this._Label1_7.Text = "Nr.:";
            this._Label1_7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_6
            // 
            this._Label1_6.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_6.Location = new System.Drawing.Point(13, 302);
            this._Label1_6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_6.Name = "_Label1_6";
            this._Label1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_6.Size = new System.Drawing.Size(137, 22);
            this._Label1_6.TabIndex = 6;
            this._Label1_6.Text = "Kurztitel:";
            this._Label1_6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this._Label1_6.Visible = false;
            // 
            // _Label1_5
            // 
            this._Label1_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_5.Location = new System.Drawing.Point(0, 94);
            this._Label1_5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_5.Name = "_Label1_5";
            this._Label1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_5.Size = new System.Drawing.Size(144, 18);
            this._Label1_5.TabIndex = 5;
            this._Label1_5.Text = "Autor:";
            this._Label1_5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_4
            // 
            this._Label1_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_4.Location = new System.Drawing.Point(0, 114);
            this._Label1_4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_4.Size = new System.Drawing.Size(144, 18);
            this._Label1_4.TabIndex = 4;
            this._Label1_4.Text = "Herausgeber:";
            this._Label1_4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_3
            // 
            this._Label1_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_3.Location = new System.Drawing.Point(0, 134);
            this._Label1_3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_3.Size = new System.Drawing.Size(144, 18);
            this._Label1_3.TabIndex = 3;
            this._Label1_3.Text = "Erscheinungsort:";
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_2
            // 
            this._Label1_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_2.Location = new System.Drawing.Point(0, 154);
            this._Label1_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_2.Size = new System.Drawing.Size(144, 18);
            this._Label1_2.TabIndex = 2;
            this._Label1_2.Text = "Erscheinungsdatum:";
            this._Label1_2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_1
            // 
            this._Label1_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_1.Location = new System.Drawing.Point(0, 54);
            this._Label1_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_1.Size = new System.Drawing.Size(144, 18);
            this._Label1_1.TabIndex = 1;
            this._Label1_1.Text = "Titel:";
            this._Label1_1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_0
            // 
            this._Label1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_0.Location = new System.Drawing.Point(5, 3);
            this._Label1_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_0.Size = new System.Drawing.Size(1068, 20);
            this._Label1_0.TabIndex = 0;
            this._Label1_0.Text = "Quellen- und Literaturverwaltung";
            this._Label1_0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ComboBox1
            // 
            this.ComboBox1.BackColor = System.Drawing.Color.White;
            this.ComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComboBox1.FormattingEnabled = true;
            this.ComboBox1.Location = new System.Drawing.Point(144, 234);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(813, 27);
            this.ComboBox1.TabIndex = 59;
            this.ComboBox1.DoubleClick += new System.EventHandler(this.ComboBox1_DoubleClick);
            this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            this.ComboBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ComboBox1_KeyUp);
            // 
            // LagerFrame
            // 
            this.LagerFrame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.LagerFrame.Controls.Add(this.frmSrch_Label5);
            this.LagerFrame.Controls.Add(this.frmSrch_btnClose);
            this.LagerFrame.Controls.Add(this.frmSrch_btnDeleteEntry);
            this.LagerFrame.Controls.Add(this.frmSrch_btnNewEntry);
            this.LagerFrame.Controls.Add(this.frmSrch_edtSearch);
            this.LagerFrame.Controls.Add(this.frmSrch_ListBox1);
            this.LagerFrame.Location = new System.Drawing.Point(534, 27);
            this.LagerFrame.Name = "LagerFrame";
            this.LagerFrame.Size = new System.Drawing.Size(973, 440);
            this.LagerFrame.TabIndex = 60;
            this.LagerFrame.TabStop = false;
            this.LagerFrame.Text = "Suchbegriff:";
            this.LagerFrame.Visible = false;
            // 
            // frmSrch_Label5
            // 
            this.frmSrch_Label5.BackColor = System.Drawing.Color.White;
            this.frmSrch_Label5.Location = new System.Drawing.Point(384, 293);
            this.frmSrch_Label5.Name = "frmSrch_Label5";
            this.frmSrch_Label5.Size = new System.Drawing.Size(541, 21);
            this.frmSrch_Label5.TabIndex = 5;
            // 
            // frmSrch_btnClose
            // 
            this.frmSrch_btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.frmSrch_btnClose.Location = new System.Drawing.Point(805, 397);
            this.frmSrch_btnClose.Name = "btnClose";
            this.frmSrch_btnClose.Size = new System.Drawing.Size(117, 31);
            this.frmSrch_btnClose.TabIndex = 4;
            this.frmSrch_btnClose.Text = "schließen";
            this.frmSrch_btnClose.UseVisualStyleBackColor = false;
            this.frmSrch_btnClose.Click += new System.EventHandler(this.frmSrch_btnClose_Click);
            // 
            // frmSrch_btnDeleteEntry
            // 
            this.frmSrch_btnDeleteEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.frmSrch_btnDeleteEntry.Location = new System.Drawing.Point(545, 259);
            this.frmSrch_btnDeleteEntry.Name = "frmSrch_btnDeleteEntry";
            this.frmSrch_btnDeleteEntry.Size = new System.Drawing.Size(196, 31);
            this.frmSrch_btnDeleteEntry.TabIndex = 3;
            this.frmSrch_btnDeleteEntry.Text = "Diesen Eintrag löschen";
            this.frmSrch_btnDeleteEntry.UseVisualStyleBackColor = false;
            this.frmSrch_btnDeleteEntry.Click += new System.EventHandler(this.frmSrch_btnDeleteEntry_Click);
            // 
            // frmSrch_btnNewEntry
            // 
            this.frmSrch_btnNewEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.frmSrch_btnNewEntry.Location = new System.Drawing.Point(261, 20);
            this.frmSrch_btnNewEntry.Name = "frmSrch_btnNewEntry";
            this.frmSrch_btnNewEntry.Size = new System.Drawing.Size(117, 31);
            this.frmSrch_btnNewEntry.TabIndex = 2;
            this.frmSrch_btnNewEntry.Text = "Neuer Eintrag";
            this.frmSrch_btnNewEntry.UseVisualStyleBackColor = false;
            this.frmSrch_btnNewEntry.Click += new System.EventHandler(this.btnNewEntry_Click);
            // 
            // frmSrch_edtSearch
            // 
            this.frmSrch_edtSearch.Location = new System.Drawing.Point(5, 20);
            this.frmSrch_edtSearch.Name = "frmSrch_edtSearch";
            this.frmSrch_edtSearch.Size = new System.Drawing.Size(225, 27);
            this.frmSrch_edtSearch.TabIndex = 1;
            this.frmSrch_edtSearch.TextChanged += new System.EventHandler(this.frmSrch_edtSearch_TextChanged);
            // 
            // frmSrch_ListBox1
            // 
            this.frmSrch_ListBox1.FormattingEnabled = true;
            this.frmSrch_ListBox1.ItemHeight = 19;
            this.frmSrch_ListBox1.Location = new System.Drawing.Point(3, 65);
            this.frmSrch_ListBox1.Name = "frmSrch_ListBox1";
            this.frmSrch_ListBox1.Size = new System.Drawing.Size(375, 346);
            this.frmSrch_ListBox1.TabIndex = 0;
            this.frmSrch_ListBox1.DoubleClick += new System.EventHandler(this.frmSrch_ListBox1_DoubleClick);
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label7.Location = new System.Drawing.Point(0, 234);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(144, 25);
            this.Label7.TabIndex = 45;
            this.Label7.Text = "Standort:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label7.Click += new System.EventHandler(this.Label7_Click);
            // 
            // btnHometown
            // 
            this.btnHometown.BackColor = System.Drawing.SystemColors.Control;
            this.btnHometown.Location = new System.Drawing.Point(466, 648);
            this.btnHometown.Name = "btnHometown";
            this.btnHometown.Size = new System.Drawing.Size(156, 27);
            this.btnHometown.TabIndex = 61;
            this.btnHometown.Text = "Standorte bearbeiten";
            this.btnHometown.UseVisualStyleBackColor = false;
            this.btnHometown.Click += new System.EventHandler(this.btnHometown_Click);
            // 
            // btnClose2
            // 
            this.btnClose2.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose2.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnClose2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClose2.Location = new System.Drawing.Point(532, 684);
            this.btnClose2.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose2.Name = "btnClose2";
            this.btnClose2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnClose2.Size = new System.Drawing.Size(108, 27);
            this.btnClose2.TabIndex = 62;
            this.btnClose2.Text = "Zurück";
            this.btnClose2.UseVisualStyleBackColor = false;
            this.btnClose2.Visible = false;
            this.btnClose2.Click += new System.EventHandler(this.Button6_Click);
            // 
            // PictureBox1
            // 
            this.PictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.PictureBox1.Location = new System.Drawing.Point(651, 265);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(353, 350);
            this.PictureBox1.TabIndex = 63;
            this.PictureBox1.TabStop = false;
            this.PictureBox1.Click += new System.EventHandler(this.PictureBox1_Click);
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(648, 621);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(348, 111);
            this.Label6.TabIndex = 64;
            // 
            // ComboBox2
            // 
            this.ComboBox2.FormattingEnabled = true;
            this.ComboBox2.Location = new System.Drawing.Point(3, 683);
            this.ComboBox2.Name = "ComboBox2";
            this.ComboBox2.Size = new System.Drawing.Size(312, 27);
            this.ComboBox2.TabIndex = 65;
            this.ComboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBox2_SelectedIndexChanged);
            this.ComboBox2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ComboBox2_MouseDoubleClick);
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Label8.Location = new System.Drawing.Point(638, 650);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(76, 19);
            this.Label8.TabIndex = 66;
            this.Label8.Text = "Bildname";
            this.Label8.Visible = false;
            // 
            // Quellverw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1016, 742);
            this.ControlBox = false;
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Frame3);
            this.Controls.Add(this.Frame2);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.ComboBox1);
            this.Controls.Add(this._Command1_9);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this._Command1_5);
            this.Controls.Add(this._Command1_8);
            this.Controls.Add(this._Command1_7);
            this.Controls.Add(this._Command1_12);
            this.Controls.Add(this._Command1_4);
            this.Controls.Add(this._Command1_3);
            this.Controls.Add(this._Command1_2);
            this.Controls.Add(this.RTB1);
            this.Controls.Add(this._Command1_1);
            this.Controls.Add(this._Text1_10);
            this.Controls.Add(this._Text1_9);
            this.Controls.Add(this._Text1_8);
            this.Controls.Add(this._Text1_7);
            this.Controls.Add(this._Text1_6);
            this.Controls.Add(this._Text1_5);
            this.Controls.Add(this._Text1_3);
            this.Controls.Add(this._Text1_2);
            this.Controls.Add(this._Text1_1);
            this.Controls.Add(this._Text1_0);
            this.Controls.Add(this._Label1_13);
            this.Controls.Add(this._Label1_12);
            this.Controls.Add(this._Label1_11);
            this.Controls.Add(this._Label1_9);
            this.Controls.Add(this._Label1_8);
            this.Controls.Add(this._Label1_7);
            this.Controls.Add(this._Label1_6);
            this.Controls.Add(this._Label1_5);
            this.Controls.Add(this._Label1_4);
            this.Controls.Add(this._Label1_3);
            this.Controls.Add(this._Label1_2);
            this.Controls.Add(this._Label1_1);
            this.Controls.Add(this._Label1_0);
            this.Controls.Add(this.btnHometown);
            this.Controls.Add(this.btnClose2);
            this.Controls.Add(this._Command1_6);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.ComboBox2);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Location = new System.Drawing.Point(1, 1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Quellverw";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Quellen und Literaturverwaltung";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Frame3.ResumeLayout(false);
            this.Frame1.ResumeLayout(false);
            this.Frame1.PerformLayout();
            this.Frame2.ResumeLayout(false);
            this.Frame2.PerformLayout();
            this.LagerFrame.ResumeLayout(false);
            this.LagerFrame.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    }
