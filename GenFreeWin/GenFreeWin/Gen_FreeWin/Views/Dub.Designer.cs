using BaseLib.Helper;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

public partial class Dub
{
    private static List<WeakReference> __ENCList = new();

    private IContainer components;

    public ToolTip ToolTip1;

    //[AccessedThroughProperty(nameof(_Option1_0))]
    //private RadioButton __Option1_0;

    //[AccessedThroughProperty(nameof(_Option1_2))]
    //private RadioButton __Option1_2;

    //[AccessedThroughProperty(nameof(_Option1_1))]
    //private RadioButton __Option1_1;

    public ControlArray<Label> ALabel1;
    public ControlArray<Button> ACommand1;
    public ControlArray<GroupBox> AFrame1;
    public ControlArray<ListBox> List1;
    public ControlArray<TextBox> Text1;
    public ControlArray<RadioButton> Option1;

    [CommandBinding("Command1_10Command")]
    public Button _Command1_10;
    [CommandBinding("Command1_7Command")]
    public Button _Command1_7;
    [CommandBinding("Command1_3Command")]
    public Button _Command1_3;
    [CommandBinding("Command1_2Command")]
    public Button _Command1_2;
    public Label _Label1_42;
    public Label _Label1_41;
    public Label _Label1_20;
    public Label _Label1_21;
    public Label _Label1_22;
    public Label _Label1_23;
    public Label _Label1_24;
    public Label _Label1_25;
    public Label _Label1_26;
    public Label _Label1_27;
    public Label _Label1_28;
    public Label _Label1_29;
    public Label _Label1_30;
    public Label _Label1_31;
    public Label _Label1_32;
    public Label _Label1_33;
    public Label _Label1_34;
    public Label _Label1_35;
    public Label _Label1_36;
    public Label _Label1_37;
    public Label _Label1_38;
    public GroupBox _Frame1_3;
    [CommandBinding("Command1_9Command")]
    public Button _Command1_9;
    [CommandBinding("Command1_5Command")]
    public Button _Command1_5;
    [CommandBinding("Command1_1Command")]
    public Button _Command1_1;
    [CommandBinding("Command1_0Command")]
    public Button _Command1_0;
    public Label _Label1_44;
    public Label _Label1_43;
    public Label _Label1_18;
    public Label _Label1_17;
    public Label _Label1_16;
    public Label _Label1_15;
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
    public Label _Label1_3;
    public Label _Label1_2;
    public Label _Label1_1;
    public Label _Label1_0;
    public GroupBox _Frame1_2;

/*
             __Option1_0.CheckedChanged += new EventHandler(_Option1_0_CheckedChanged);
             __Option1_2.CheckedChanged += new EventHandler(_Option1_0_CheckedChanged);
             __Option1_1.CheckedChanged += new EventHandler(_Option1_0_CheckedChanged);
             btnMainmenue.Click += new EventHandler(btnShowAsocPeople_Click);
             edtPlace.TextChanged += new EventHandler(TextBox1_TextChanged);
             edtSuburb.TextChanged += new EventHandler(TextBox1_TextChanged);
             _CheckBox1.Click += new EventHandler(CheckBox1_Click);
             _CheckBox1.CheckedChanged += new EventHandler(CheckBox1_CheckedChanged);
             _CheckBox2.Click += new EventHandler(CheckBox2_Click);
             _CheckBox2.CheckedChanged +=           new  EventHandler (CheckBox2_CheckedChanged);

            btnShowUsage.Click += new EventHandler(btnReenter_Click);

            btnBack.Click += new EventHandler(btnNewEntry_Click);

            btnNextEntry.Click += new EventHandler(btnMoveToCause_Click);

            _RadioButton2.CheckedChanged += new EventHandler(RadioButton2_CheckedChanged);

             */
    public RadioButton _Option1_0;
    public RadioButton _Option1_2;
    public RadioButton _Option1_1;

    public ListBox _List1_0;
    public GroupBox _Frame1_0;
    [CommandBinding("Command1_4Command")]
    public Button _Command1_4;
    public ListBox _List1_1;
    public GroupBox _Frame1_1;
    public Button Button1;
    public TextBox TextBox1;
    public TextBox TextBox2;
    public CheckBox CheckBox1;
    public CheckBox CheckBox2;
    public Button Button3;
    public Button Button2;
    public Button Button4;
    public RadioButton RadioButton2;
    internal RadioButton RadioButton1;

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
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._Frame1_3 = new System.Windows.Forms.GroupBox();
            this.Button3 = new System.Windows.Forms.Button();
            this._Command1_10 = new System.Windows.Forms.Button();
            this._Command1_7 = new System.Windows.Forms.Button();
            this._Command1_3 = new System.Windows.Forms.Button();
            this._Command1_2 = new System.Windows.Forms.Button();
            this._Label1_42 = new System.Windows.Forms.Label();
            this._Label1_41 = new System.Windows.Forms.Label();
            this._Label1_20 = new System.Windows.Forms.Label();
            this._Label1_21 = new System.Windows.Forms.Label();
            this._Label1_22 = new System.Windows.Forms.Label();
            this._Label1_23 = new System.Windows.Forms.Label();
            this._Label1_24 = new System.Windows.Forms.Label();
            this._Label1_25 = new System.Windows.Forms.Label();
            this._Label1_26 = new System.Windows.Forms.Label();
            this._Label1_27 = new System.Windows.Forms.Label();
            this._Label1_28 = new System.Windows.Forms.Label();
            this._Label1_29 = new System.Windows.Forms.Label();
            this._Label1_30 = new System.Windows.Forms.Label();
            this._Label1_31 = new System.Windows.Forms.Label();
            this._Label1_32 = new System.Windows.Forms.Label();
            this._Label1_33 = new System.Windows.Forms.Label();
            this._Label1_34 = new System.Windows.Forms.Label();
            this._Label1_35 = new System.Windows.Forms.Label();
            this._Label1_36 = new System.Windows.Forms.Label();
            this._Label1_37 = new System.Windows.Forms.Label();
            this._Label1_38 = new System.Windows.Forms.Label();
            this._Frame1_2 = new System.Windows.Forms.GroupBox();
            this.Button2 = new System.Windows.Forms.Button();
            this._Command1_9 = new System.Windows.Forms.Button();
            this._Command1_5 = new System.Windows.Forms.Button();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this._Label1_44 = new System.Windows.Forms.Label();
            this._Label1_43 = new System.Windows.Forms.Label();
            this._Label1_18 = new System.Windows.Forms.Label();
            this._Label1_17 = new System.Windows.Forms.Label();
            this._Label1_16 = new System.Windows.Forms.Label();
            this._Label1_15 = new System.Windows.Forms.Label();
            this._Label1_14 = new System.Windows.Forms.Label();
            this._Label1_13 = new System.Windows.Forms.Label();
            this._Label1_12 = new System.Windows.Forms.Label();
            this._Label1_11 = new System.Windows.Forms.Label();
            this._Label1_10 = new System.Windows.Forms.Label();
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
            this._Frame1_0 = new System.Windows.Forms.GroupBox();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this._Option1_0 = new System.Windows.Forms.RadioButton();
            this._Option1_2 = new System.Windows.Forms.RadioButton();
            this._Option1_1 = new System.Windows.Forms.RadioButton();
            this._List1_0 = new System.Windows.Forms.ListBox();
            this._Frame1_1 = new System.Windows.Forms.GroupBox();
            this.RadioButton2 = new System.Windows.Forms.RadioButton();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.Button4 = new System.Windows.Forms.Button();
            this._List1_1 = new System.Windows.Forms.ListBox();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this._Command1_4 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this._Frame1_3.SuspendLayout();
            this._Frame1_2.SuspendLayout();
            this._Frame1_0.SuspendLayout();
            this._Frame1_1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _Frame1_3
            // 
            this._Frame1_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._Frame1_3.Controls.Add(this.Button3);
            this._Frame1_3.Controls.Add(this._Command1_10);
            this._Frame1_3.Controls.Add(this._Command1_7);
            this._Frame1_3.Controls.Add(this._Command1_3);
            this._Frame1_3.Controls.Add(this._Command1_2);
            this._Frame1_3.Controls.Add(this._Label1_42);
            this._Frame1_3.Controls.Add(this._Label1_41);
            this._Frame1_3.Controls.Add(this._Label1_20);
            this._Frame1_3.Controls.Add(this._Label1_21);
            this._Frame1_3.Controls.Add(this._Label1_22);
            this._Frame1_3.Controls.Add(this._Label1_23);
            this._Frame1_3.Controls.Add(this._Label1_24);
            this._Frame1_3.Controls.Add(this._Label1_25);
            this._Frame1_3.Controls.Add(this._Label1_26);
            this._Frame1_3.Controls.Add(this._Label1_27);
            this._Frame1_3.Controls.Add(this._Label1_28);
            this._Frame1_3.Controls.Add(this._Label1_29);
            this._Frame1_3.Controls.Add(this._Label1_30);
            this._Frame1_3.Controls.Add(this._Label1_31);
            this._Frame1_3.Controls.Add(this._Label1_32);
            this._Frame1_3.Controls.Add(this._Label1_33);
            this._Frame1_3.Controls.Add(this._Label1_34);
            this._Frame1_3.Controls.Add(this._Label1_35);
            this._Frame1_3.Controls.Add(this._Label1_36);
            this._Frame1_3.Controls.Add(this._Label1_37);
            this._Frame1_3.Controls.Add(this._Label1_38);
            this._Frame1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Frame1_3.Location = new System.Drawing.Point(523, 90);
            this._Frame1_3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_3.Name = "_Frame1_3";
            this._Frame1_3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Frame1_3.Size = new System.Drawing.Size(488, 645);
            this._Frame1_3.TabIndex = 28;
            this._Frame1_3.TabStop = false;
            this._Frame1_3.Text = "Person";
            this._Frame1_3.Visible = false;
            // 
            // Button3
            // 
            this.Button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button3.Location = new System.Drawing.Point(294, 600);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(111, 26);
            this.Button3.TabIndex = 62;
            this.Button3.Text = "zur Familie";
            this.Button3.UseVisualStyleBackColor = false;
            // 
            // _Command1_10
            // 
            this._Command1_10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this._Command1_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_10.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_10.Location = new System.Drawing.Point(168, 599);
            this._Command1_10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_10.Name = "_Command1_10";
            this._Command1_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_10.Size = new System.Drawing.Size(99, 24);
            this._Command1_10.TabIndex = 56;
            this._Command1_10.Text = "Zur Person";
            this._Command1_10.UseVisualStyleBackColor = false;
            // 
            // _Command1_7
            // 
            this._Command1_7.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_7.Location = new System.Drawing.Point(245, 489);
            this._Command1_7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_7.Name = "_Command1_7";
            this._Command1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_7.Size = new System.Drawing.Size(243, 24);
            this._Command1_7.TabIndex = 51;
            this._Command1_7.Text = "btnCancel4";
            this._Command1_7.UseVisualStyleBackColor = false;
            this._Command1_7.Visible = false;
            // 
            // _Command1_3
            // 
            this._Command1_3.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_3.Location = new System.Drawing.Point(6, 510);
            this._Command1_3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_3.Name = "_Command1_3";
            this._Command1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_3.Size = new System.Drawing.Size(97, 24);
            this._Command1_3.TabIndex = 30;
            this._Command1_3.Text = "Liste";
            this._Command1_3.UseVisualStyleBackColor = false;
            // 
            // _Command1_2
            // 
            this._Command1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_2.Location = new System.Drawing.Point(137, 512);
            this._Command1_2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_2.Name = "_Command1_2";
            this._Command1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_2.Size = new System.Drawing.Size(351, 83);
            this._Command1_2.TabIndex = 29;
            this._Command1_2.Text = "btnCancel4";
            this._Command1_2.UseVisualStyleBackColor = false;
            this._Command1_2.Visible = false;
            // 
            // _Label1_42
            // 
            this._Label1_42.BackColor = System.Drawing.Color.White;
            this._Label1_42.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_42.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_42.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_42.Location = new System.Drawing.Point(8, 176);
            this._Label1_42.Name = "_Label1_42";
            this._Label1_42.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_42.Size = new System.Drawing.Size(487, 19);
            this._Label1_42.TabIndex = 58;
            // 
            // _Label1_41
            // 
            this._Label1_41.BackColor = System.Drawing.Color.White;
            this._Label1_41.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_41.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_41.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_41.Location = new System.Drawing.Point(8, 154);
            this._Label1_41.Name = "_Label1_41";
            this._Label1_41.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_41.Size = new System.Drawing.Size(487, 19);
            this._Label1_41.TabIndex = 57;
            // 
            // _Label1_20
            // 
            this._Label1_20.BackColor = System.Drawing.Color.White;
            this._Label1_20.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_20.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_20.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_20.Location = new System.Drawing.Point(8, 21);
            this._Label1_20.Name = "_Label1_20";
            this._Label1_20.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_20.Size = new System.Drawing.Size(487, 19);
            this._Label1_20.TabIndex = 49;
            // 
            // _Label1_21
            // 
            this._Label1_21.BackColor = System.Drawing.Color.White;
            this._Label1_21.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_21.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_21.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_21.Location = new System.Drawing.Point(8, 44);
            this._Label1_21.Name = "_Label1_21";
            this._Label1_21.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_21.Size = new System.Drawing.Size(487, 19);
            this._Label1_21.TabIndex = 48;
            // 
            // _Label1_22
            // 
            this._Label1_22.BackColor = System.Drawing.Color.White;
            this._Label1_22.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_22.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_22.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_22.Location = new System.Drawing.Point(8, 66);
            this._Label1_22.Name = "_Label1_22";
            this._Label1_22.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_22.Size = new System.Drawing.Size(487, 19);
            this._Label1_22.TabIndex = 47;
            // 
            // _Label1_23
            // 
            this._Label1_23.BackColor = System.Drawing.Color.White;
            this._Label1_23.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_23.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_23.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_23.Location = new System.Drawing.Point(8, 87);
            this._Label1_23.Name = "_Label1_23";
            this._Label1_23.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_23.Size = new System.Drawing.Size(487, 19);
            this._Label1_23.TabIndex = 46;
            // 
            // _Label1_24
            // 
            this._Label1_24.BackColor = System.Drawing.Color.White;
            this._Label1_24.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_24.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_24.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_24.Location = new System.Drawing.Point(8, 109);
            this._Label1_24.Name = "_Label1_24";
            this._Label1_24.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_24.Size = new System.Drawing.Size(487, 19);
            this._Label1_24.TabIndex = 45;
            // 
            // _Label1_25
            // 
            this._Label1_25.BackColor = System.Drawing.Color.White;
            this._Label1_25.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_25.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_25.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_25.Location = new System.Drawing.Point(8, 132);
            this._Label1_25.Name = "_Label1_25";
            this._Label1_25.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_25.Size = new System.Drawing.Size(487, 19);
            this._Label1_25.TabIndex = 44;
            // 
            // _Label1_26
            // 
            this._Label1_26.BackColor = System.Drawing.Color.White;
            this._Label1_26.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_26.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_26.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_26.Location = new System.Drawing.Point(8, 199);
            this._Label1_26.Name = "_Label1_26";
            this._Label1_26.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_26.Size = new System.Drawing.Size(487, 19);
            this._Label1_26.TabIndex = 43;
            // 
            // _Label1_27
            // 
            this._Label1_27.BackColor = System.Drawing.Color.White;
            this._Label1_27.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_27.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_27.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_27.Location = new System.Drawing.Point(8, 221);
            this._Label1_27.Name = "_Label1_27";
            this._Label1_27.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_27.Size = new System.Drawing.Size(487, 19);
            this._Label1_27.TabIndex = 42;
            // 
            // _Label1_28
            // 
            this._Label1_28.BackColor = System.Drawing.Color.White;
            this._Label1_28.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_28.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_28.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_28.Location = new System.Drawing.Point(8, 243);
            this._Label1_28.Name = "_Label1_28";
            this._Label1_28.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_28.Size = new System.Drawing.Size(487, 19);
            this._Label1_28.TabIndex = 41;
            // 
            // _Label1_29
            // 
            this._Label1_29.BackColor = System.Drawing.Color.White;
            this._Label1_29.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_29.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_29.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_29.Location = new System.Drawing.Point(8, 266);
            this._Label1_29.Name = "_Label1_29";
            this._Label1_29.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_29.Size = new System.Drawing.Size(487, 19);
            this._Label1_29.TabIndex = 40;
            // 
            // _Label1_30
            // 
            this._Label1_30.BackColor = System.Drawing.Color.White;
            this._Label1_30.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_30.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_30.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_30.Location = new System.Drawing.Point(8, 288);
            this._Label1_30.Name = "_Label1_30";
            this._Label1_30.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_30.Size = new System.Drawing.Size(487, 19);
            this._Label1_30.TabIndex = 39;
            // 
            // _Label1_31
            // 
            this._Label1_31.BackColor = System.Drawing.Color.White;
            this._Label1_31.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_31.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_31.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_31.Location = new System.Drawing.Point(8, 310);
            this._Label1_31.Name = "_Label1_31";
            this._Label1_31.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_31.Size = new System.Drawing.Size(487, 19);
            this._Label1_31.TabIndex = 38;
            // 
            // _Label1_32
            // 
            this._Label1_32.BackColor = System.Drawing.Color.White;
            this._Label1_32.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_32.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_32.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_32.Location = new System.Drawing.Point(8, 333);
            this._Label1_32.Name = "_Label1_32";
            this._Label1_32.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_32.Size = new System.Drawing.Size(487, 19);
            this._Label1_32.TabIndex = 37;
            // 
            // _Label1_33
            // 
            this._Label1_33.BackColor = System.Drawing.Color.White;
            this._Label1_33.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_33.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_33.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_33.Location = new System.Drawing.Point(8, 355);
            this._Label1_33.Name = "_Label1_33";
            this._Label1_33.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_33.Size = new System.Drawing.Size(487, 19);
            this._Label1_33.TabIndex = 36;
            // 
            // _Label1_34
            // 
            this._Label1_34.BackColor = System.Drawing.Color.White;
            this._Label1_34.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_34.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_34.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_34.Location = new System.Drawing.Point(8, 376);
            this._Label1_34.Name = "_Label1_34";
            this._Label1_34.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_34.Size = new System.Drawing.Size(487, 19);
            this._Label1_34.TabIndex = 35;
            // 
            // _Label1_35
            // 
            this._Label1_35.BackColor = System.Drawing.Color.White;
            this._Label1_35.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_35.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_35.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_35.Location = new System.Drawing.Point(8, 398);
            this._Label1_35.Name = "_Label1_35";
            this._Label1_35.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_35.Size = new System.Drawing.Size(487, 19);
            this._Label1_35.TabIndex = 34;
            // 
            // _Label1_36
            // 
            this._Label1_36.BackColor = System.Drawing.Color.White;
            this._Label1_36.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_36.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_36.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_36.Location = new System.Drawing.Point(8, 421);
            this._Label1_36.Name = "_Label1_36";
            this._Label1_36.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_36.Size = new System.Drawing.Size(487, 19);
            this._Label1_36.TabIndex = 33;
            // 
            // _Label1_37
            // 
            this._Label1_37.BackColor = System.Drawing.Color.White;
            this._Label1_37.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_37.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_37.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_37.Location = new System.Drawing.Point(8, 443);
            this._Label1_37.Name = "_Label1_37";
            this._Label1_37.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_37.Size = new System.Drawing.Size(487, 19);
            this._Label1_37.TabIndex = 32;
            // 
            // _Label1_38
            // 
            this._Label1_38.BackColor = System.Drawing.Color.White;
            this._Label1_38.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_38.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_38.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_38.Location = new System.Drawing.Point(8, 465);
            this._Label1_38.Name = "_Label1_38";
            this._Label1_38.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_38.Size = new System.Drawing.Size(487, 19);
            this._Label1_38.TabIndex = 31;
            // 
            // _Frame1_2
            // 
            this._Frame1_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._Frame1_2.Controls.Add(this.Button2);
            this._Frame1_2.Controls.Add(this._Command1_9);
            this._Frame1_2.Controls.Add(this._Command1_5);
            this._Frame1_2.Controls.Add(this._Command1_1);
            this._Frame1_2.Controls.Add(this._Command1_0);
            this._Frame1_2.Controls.Add(this._Label1_44);
            this._Frame1_2.Controls.Add(this._Label1_43);
            this._Frame1_2.Controls.Add(this._Label1_18);
            this._Frame1_2.Controls.Add(this._Label1_17);
            this._Frame1_2.Controls.Add(this._Label1_16);
            this._Frame1_2.Controls.Add(this._Label1_15);
            this._Frame1_2.Controls.Add(this._Label1_14);
            this._Frame1_2.Controls.Add(this._Label1_13);
            this._Frame1_2.Controls.Add(this._Label1_12);
            this._Frame1_2.Controls.Add(this._Label1_11);
            this._Frame1_2.Controls.Add(this._Label1_10);
            this._Frame1_2.Controls.Add(this._Label1_9);
            this._Frame1_2.Controls.Add(this._Label1_8);
            this._Frame1_2.Controls.Add(this._Label1_7);
            this._Frame1_2.Controls.Add(this._Label1_6);
            this._Frame1_2.Controls.Add(this._Label1_5);
            this._Frame1_2.Controls.Add(this._Label1_4);
            this._Frame1_2.Controls.Add(this._Label1_3);
            this._Frame1_2.Controls.Add(this._Label1_2);
            this._Frame1_2.Controls.Add(this._Label1_1);
            this._Frame1_2.Controls.Add(this._Label1_0);
            this._Frame1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Frame1_2.Location = new System.Drawing.Point(2, 90);
            this._Frame1_2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_2.Name = "_Frame1_2";
            this._Frame1_2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Frame1_2.Size = new System.Drawing.Size(499, 645);
            this._Frame1_2.TabIndex = 6;
            this._Frame1_2.TabStop = false;
            this._Frame1_2.Text = "Person";
            this._Frame1_2.Visible = false;
            // 
            // Button2
            // 
            this.Button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button2.Location = new System.Drawing.Point(294, 600);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(111, 26);
            this.Button2.TabIndex = 61;
            this.Button2.Text = "zur Familie";
            this.Button2.UseVisualStyleBackColor = false;
            // 
            // _Command1_9
            // 
            this._Command1_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this._Command1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_9.Location = new System.Drawing.Point(158, 601);
            this._Command1_9.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_9.Name = "_Command1_9";
            this._Command1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_9.Size = new System.Drawing.Size(99, 24);
            this._Command1_9.TabIndex = 55;
            this._Command1_9.Text = "Zur Person";
            this._Command1_9.UseVisualStyleBackColor = false;
            // 
            // _Command1_5
            // 
            this._Command1_5.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_5.Location = new System.Drawing.Point(248, 489);
            this._Command1_5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_5.Name = "_Command1_5";
            this._Command1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_5.Size = new System.Drawing.Size(247, 24);
            this._Command1_5.TabIndex = 50;
            this._Command1_5.Text = "btnCancel4";
            this._Command1_5.UseVisualStyleBackColor = false;
            this._Command1_5.Visible = false;
            // 
            // _Command1_1
            // 
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_1.Location = new System.Drawing.Point(152, 512);
            this._Command1_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_1.Size = new System.Drawing.Size(351, 83);
            this._Command1_1.TabIndex = 27;
            this._Command1_1.Text = "btnCancel4";
            this._Command1_1.UseVisualStyleBackColor = false;
            this._Command1_1.Visible = false;
            // 
            // _Command1_0
            // 
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_0.Location = new System.Drawing.Point(6, 512);
            this._Command1_0.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_0.Size = new System.Drawing.Size(97, 24);
            this._Command1_0.TabIndex = 26;
            this._Command1_0.Text = "Liste";
            this._Command1_0.UseVisualStyleBackColor = false;
            // 
            // _Label1_44
            // 
            this._Label1_44.BackColor = System.Drawing.Color.White;
            this._Label1_44.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_44.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_44.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_44.Location = new System.Drawing.Point(8, 176);
            this._Label1_44.Name = "_Label1_44";
            this._Label1_44.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_44.Size = new System.Drawing.Size(495, 19);
            this._Label1_44.TabIndex = 60;
            // 
            // _Label1_43
            // 
            this._Label1_43.BackColor = System.Drawing.Color.White;
            this._Label1_43.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_43.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_43.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_43.Location = new System.Drawing.Point(8, 154);
            this._Label1_43.Name = "_Label1_43";
            this._Label1_43.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_43.Size = new System.Drawing.Size(495, 19);
            this._Label1_43.TabIndex = 59;
            // 
            // _Label1_18
            // 
            this._Label1_18.BackColor = System.Drawing.Color.White;
            this._Label1_18.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_18.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_18.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_18.Location = new System.Drawing.Point(8, 465);
            this._Label1_18.Name = "_Label1_18";
            this._Label1_18.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_18.Size = new System.Drawing.Size(495, 19);
            this._Label1_18.TabIndex = 25;
            // 
            // _Label1_17
            // 
            this._Label1_17.BackColor = System.Drawing.Color.White;
            this._Label1_17.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_17.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_17.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_17.Location = new System.Drawing.Point(8, 443);
            this._Label1_17.Name = "_Label1_17";
            this._Label1_17.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_17.Size = new System.Drawing.Size(495, 19);
            this._Label1_17.TabIndex = 24;
            // 
            // _Label1_16
            // 
            this._Label1_16.BackColor = System.Drawing.Color.White;
            this._Label1_16.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_16.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_16.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_16.Location = new System.Drawing.Point(8, 421);
            this._Label1_16.Name = "_Label1_16";
            this._Label1_16.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_16.Size = new System.Drawing.Size(495, 19);
            this._Label1_16.TabIndex = 23;
            // 
            // _Label1_15
            // 
            this._Label1_15.BackColor = System.Drawing.Color.White;
            this._Label1_15.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_15.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_15.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_15.Location = new System.Drawing.Point(8, 398);
            this._Label1_15.Name = "_Label1_15";
            this._Label1_15.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_15.Size = new System.Drawing.Size(495, 19);
            this._Label1_15.TabIndex = 22;
            // 
            // _Label1_14
            // 
            this._Label1_14.BackColor = System.Drawing.Color.White;
            this._Label1_14.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_14.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_14.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_14.Location = new System.Drawing.Point(8, 376);
            this._Label1_14.Name = "_Label1_14";
            this._Label1_14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_14.Size = new System.Drawing.Size(495, 19);
            this._Label1_14.TabIndex = 21;
            // 
            // _Label1_13
            // 
            this._Label1_13.BackColor = System.Drawing.Color.White;
            this._Label1_13.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_13.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_13.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_13.Location = new System.Drawing.Point(8, 355);
            this._Label1_13.Name = "_Label1_13";
            this._Label1_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_13.Size = new System.Drawing.Size(495, 19);
            this._Label1_13.TabIndex = 20;
            // 
            // _Label1_12
            // 
            this._Label1_12.BackColor = System.Drawing.Color.White;
            this._Label1_12.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_12.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_12.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_12.Location = new System.Drawing.Point(8, 333);
            this._Label1_12.Name = "_Label1_12";
            this._Label1_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_12.Size = new System.Drawing.Size(495, 19);
            this._Label1_12.TabIndex = 19;
            // 
            // _Label1_11
            // 
            this._Label1_11.BackColor = System.Drawing.Color.White;
            this._Label1_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_11.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_11.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_11.Location = new System.Drawing.Point(8, 310);
            this._Label1_11.Name = "_Label1_11";
            this._Label1_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_11.Size = new System.Drawing.Size(495, 19);
            this._Label1_11.TabIndex = 18;
            // 
            // _Label1_10
            // 
            this._Label1_10.BackColor = System.Drawing.Color.White;
            this._Label1_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_10.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_10.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_10.Location = new System.Drawing.Point(8, 288);
            this._Label1_10.Name = "_Label1_10";
            this._Label1_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_10.Size = new System.Drawing.Size(495, 19);
            this._Label1_10.TabIndex = 17;
            // 
            // _Label1_9
            // 
            this._Label1_9.BackColor = System.Drawing.Color.White;
            this._Label1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_9.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_9.Location = new System.Drawing.Point(8, 266);
            this._Label1_9.Name = "_Label1_9";
            this._Label1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_9.Size = new System.Drawing.Size(495, 19);
            this._Label1_9.TabIndex = 16;
            // 
            // _Label1_8
            // 
            this._Label1_8.BackColor = System.Drawing.Color.White;
            this._Label1_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_8.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_8.Location = new System.Drawing.Point(8, 243);
            this._Label1_8.Name = "_Label1_8";
            this._Label1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_8.Size = new System.Drawing.Size(495, 19);
            this._Label1_8.TabIndex = 15;
            // 
            // _Label1_7
            // 
            this._Label1_7.BackColor = System.Drawing.Color.White;
            this._Label1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_7.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_7.Location = new System.Drawing.Point(8, 221);
            this._Label1_7.Name = "_Label1_7";
            this._Label1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_7.Size = new System.Drawing.Size(495, 19);
            this._Label1_7.TabIndex = 14;
            // 
            // _Label1_6
            // 
            this._Label1_6.BackColor = System.Drawing.Color.White;
            this._Label1_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_6.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_6.Location = new System.Drawing.Point(8, 199);
            this._Label1_6.Name = "_Label1_6";
            this._Label1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_6.Size = new System.Drawing.Size(495, 19);
            this._Label1_6.TabIndex = 13;
            // 
            // _Label1_5
            // 
            this._Label1_5.BackColor = System.Drawing.Color.White;
            this._Label1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_5.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_5.Location = new System.Drawing.Point(8, 132);
            this._Label1_5.Name = "_Label1_5";
            this._Label1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_5.Size = new System.Drawing.Size(495, 19);
            this._Label1_5.TabIndex = 12;
            // 
            // _Label1_4
            // 
            this._Label1_4.BackColor = System.Drawing.Color.White;
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_4.Location = new System.Drawing.Point(8, 109);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_4.Size = new System.Drawing.Size(495, 19);
            this._Label1_4.TabIndex = 11;
            // 
            // _Label1_3
            // 
            this._Label1_3.BackColor = System.Drawing.Color.White;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_3.Location = new System.Drawing.Point(8, 87);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_3.Size = new System.Drawing.Size(495, 19);
            this._Label1_3.TabIndex = 10;
            // 
            // _Label1_2
            // 
            this._Label1_2.BackColor = System.Drawing.Color.White;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_2.Location = new System.Drawing.Point(8, 66);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_2.Size = new System.Drawing.Size(495, 19);
            this._Label1_2.TabIndex = 9;
            // 
            // _Label1_1
            // 
            this._Label1_1.BackColor = System.Drawing.Color.White;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_1.Location = new System.Drawing.Point(8, 44);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_1.Size = new System.Drawing.Size(495, 19);
            this._Label1_1.TabIndex = 8;
            // 
            // _Label1_0
            // 
            this._Label1_0.BackColor = System.Drawing.Color.White;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_0.Location = new System.Drawing.Point(8, 21);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_0.Size = new System.Drawing.Size(495, 19);
            this._Label1_0.TabIndex = 7;
            // 
            // _Frame1_0
            // 
            this._Frame1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Frame1_0.Controls.Add(this.CheckBox2);
            this._Frame1_0.Controls.Add(this.CheckBox1);
            this._Frame1_0.Controls.Add(this.TextBox1);
            this._Frame1_0.Controls.Add(this._Option1_0);
            this._Frame1_0.Controls.Add(this._Option1_2);
            this._Frame1_0.Controls.Add(this._Option1_1);
            this._Frame1_0.Controls.Add(this._List1_0);
            this._Frame1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Frame1_0.Location = new System.Drawing.Point(2, 1);
            this._Frame1_0.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_0.Name = "_Frame1_0";
            this._Frame1_0.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Frame1_0.Size = new System.Drawing.Size(500, 715);
            this._Frame1_0.TabIndex = 0;
            this._Frame1_0.TabStop = false;
            this._Frame1_0.Text = "Auswahl";
            // 
            // CheckBox2
            // 
            this.CheckBox2.AutoSize = true;
            this.CheckBox2.Location = new System.Drawing.Point(227, 672);
            this.CheckBox2.Name = "CheckBox2";
            this.CheckBox2.Size = new System.Drawing.Size(197, 23);
            this.CheckBox2.TabIndex = 67;
            this.CheckBox2.Text = "Elternnamen anzeigen";
            this.CheckBox2.UseVisualStyleBackColor = true;
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.Location = new System.Drawing.Point(227, 655);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(209, 23);
            this.CheckBox1.TabIndex = 66;
            this.CheckBox1.Text = "Partnernamen anzeigen";
            this.CheckBox1.UseVisualStyleBackColor = true;
            // 
            // TextBox1
            // 
            this.TextBox1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox1.Location = new System.Drawing.Point(14, 31);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(482, 27);
            this.TextBox1.TabIndex = 65;
            // 
            // _Option1_0
            // 
            this._Option1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_0.Checked = true;
            this._Option1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_0.Location = new System.Drawing.Point(14, 653);
            this._Option1_0.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Option1_0.Name = "_Option1_0";
            this._Option1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_0.Size = new System.Drawing.Size(195, 22);
            this._Option1_0.TabIndex = 64;
            this._Option1_0.TabStop = true;
            this._Option1_0.Text = "Alle Personen";
            this._Option1_0.UseVisualStyleBackColor = false;
            // 
            // _Option1_2
            // 
            this._Option1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_2.Location = new System.Drawing.Point(14, 690);
            this._Option1_2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Option1_2.Name = "_Option1_2";
            this._Option1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_2.Size = new System.Drawing.Size(195, 22);
            this._Option1_2.TabIndex = 63;
            this._Option1_2.TabStop = true;
            this._Option1_2.Text = "nur weibliche Personen";
            this._Option1_2.UseVisualStyleBackColor = false;
            // 
            // _Option1_1
            // 
            this._Option1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_1.Location = new System.Drawing.Point(14, 670);
            this._Option1_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Option1_1.Name = "_Option1_1";
            this._Option1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_1.Size = new System.Drawing.Size(195, 22);
            this._Option1_1.TabIndex = 62;
            this._Option1_1.TabStop = true;
            this._Option1_1.Text = "nur männliche Personen";
            this._Option1_1.UseVisualStyleBackColor = false;
            // 
            // _List1_0
            // 
            this._List1_0.BackColor = System.Drawing.SystemColors.Window;
            this._List1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_0.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._List1_0.ForeColor = System.Drawing.SystemColors.WindowText;
            this._List1_0.ItemHeight = 20;
            this._List1_0.Location = new System.Drawing.Point(14, 63);
            this._List1_0.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._List1_0.Name = "_List1_0";
            this._List1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._List1_0.Size = new System.Drawing.Size(468, 564);
            this._List1_0.TabIndex = 2;
            // 
            // _Frame1_1
            // 
            this._Frame1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Frame1_1.Controls.Add(this.RadioButton2);
            this._Frame1_1.Controls.Add(this.RadioButton1);
            this._Frame1_1.Controls.Add(this.Button4);
            this._Frame1_1.Controls.Add(this._List1_1);
            this._Frame1_1.Controls.Add(this.TextBox2);
            this._Frame1_1.Controls.Add(this._Command1_4);
            this._Frame1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Frame1_1.Location = new System.Drawing.Point(510, 2);
            this._Frame1_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_1.Name = "_Frame1_1";
            this._Frame1_1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Frame1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Frame1_1.Size = new System.Drawing.Size(500, 715);
            this._Frame1_1.TabIndex = 1;
            this._Frame1_1.TabStop = false;
            this._Frame1_1.Text = "Auswahl";
            // 
            // RadioButton2
            // 
            this.RadioButton2.AutoSize = true;
            this.RadioButton2.Location = new System.Drawing.Point(180, 681);
            this.RadioButton2.Name = "RadioButton2";
            this.RadioButton2.Size = new System.Drawing.Size(103, 23);
            this.RadioButton2.TabIndex = 65;
            this.RadioButton2.Text = "nach UID";
            this.RadioButton2.UseVisualStyleBackColor = true;
            // 
            // RadioButton1
            // 
            this.RadioButton1.AutoSize = true;
            this.RadioButton1.Checked = true;
            this.RadioButton1.Location = new System.Drawing.Point(180, 654);
            this.RadioButton1.Name = "RadioButton1";
            this.RadioButton1.Size = new System.Drawing.Size(130, 26);
            this.RadioButton1.TabIndex = 64;
            this.RadioButton1.TabStop = true;
            this.RadioButton1.Text = "nach Namen";
            this.RadioButton1.UseCompatibleTextRendering = true;
            this.RadioButton1.UseMnemonic = false;
            this.RadioButton1.UseVisualStyleBackColor = true;
            // 
            // Button4
            // 
            this.Button4.Enabled = false;
            this.Button4.Location = new System.Drawing.Point(12, 655);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(162, 53);
            this.Button4.TabIndex = 63;
            this.Button4.Text = "Nur doppelte Personen nach UID";
            this.Button4.UseVisualStyleBackColor = true;
            // 
            // _List1_1
            // 
            this._List1_1.BackColor = System.Drawing.SystemColors.Window;
            this._List1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._List1_1.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._List1_1.ForeColor = System.Drawing.SystemColors.WindowText;
            this._List1_1.ItemHeight = 20;
            this._List1_1.Location = new System.Drawing.Point(15, 63);
            this._List1_1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._List1_1.Name = "_List1_1";
            this._List1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._List1_1.Size = new System.Drawing.Size(445, 564);
            this._List1_1.TabIndex = 5;
            // 
            // TextBox2
            // 
            this.TextBox2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox2.Location = new System.Drawing.Point(13, 29);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(486, 27);
            this.TextBox2.TabIndex = 62;
            // 
            // _Command1_4
            // 
            this._Command1_4.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_4.Location = new System.Drawing.Point(361, 668);
            this._Command1_4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._Command1_4.Name = "_Command1_4";
            this._Command1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_4.Size = new System.Drawing.Size(97, 24);
            this._Command1_4.TabIndex = 61;
            this._Command1_4.UseVisualStyleBackColor = false;
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(31, 744);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(86, 24);
            this.Button1.TabIndex = 55;
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Visible = false;
            // 
            // Dub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1018, 725);
            this.Controls.Add(this._Frame1_3);
            this.Controls.Add(this._Frame1_2);
            this.Controls.Add(this._Frame1_1);
            this.Controls.Add(this._Frame1_0);
            this.Controls.Add(this.Button1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Dub";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Dubletten bearbeiten";
            this._Frame1_3.ResumeLayout(false);
            this._Frame1_2.ResumeLayout(false);
            this._Frame1_0.ResumeLayout(false);
            this._Frame1_0.PerformLayout();
            this._Frame1_1.ResumeLayout(false);
            this._Frame1_1.PerformLayout();
            this.ResumeLayout(false);

    }
}
