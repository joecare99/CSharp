using BaseLib.Helper;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GenFreeWin.Views;
partial class Partnerrecherche
{
     /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        ToolTip1 = new System.Windows.Forms.ToolTip(components);
        _Text1_2 = new System.Windows.Forms.TextBox();
        Command3 = new System.Windows.Forms.Button();
        _Text1_1 = new System.Windows.Forms.TextBox();
        Command2 = new System.Windows.Forms.Button();
        List2 = new System.Windows.Forms.ListBox();
        Command1 = new System.Windows.Forms.Button();
        _Text1_0 = new System.Windows.Forms.TextBox();
        List1 = new System.Windows.Forms.ListBox();
        _Label2_1 = new System.Windows.Forms.Label();
        _Label2_0 = new System.Windows.Forms.Label();
        Label1 = new System.Windows.Forms.Label();
        CheckBox1 = new System.Windows.Forms.CheckBox();
        SuspendLayout();
        _Text1_2.AcceptsReturn = true;
        _Text1_2.BackColor = SystemColors.Window;
        _Text1_2.Cursor = Cursors.IBeam;
        _Text1_2.ForeColor = SystemColors.WindowText;
        _Text1_2.Location = new System.Drawing.Point(513, 29);
        _Text1_2.Margin = new System.Windows.Forms.Padding(4);
        _Text1_2.MaxLength = 0;
        _Text1_2.Name = "_Text1_2";
        _Text1_2.RightToLeft = RightToLeft.No;
        _Text1_2.Size = new System.Drawing.Size(232, 25);
        _Text1_2.TabIndex = 10;
        _Text1_2.Tag = "0";
        Command3.BackColor = Color.FromArgb(192, 192, 255);
        Command3.Cursor = Cursors.Default;
        Command3.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Command3.ForeColor = SystemColors.ControlText;
        Command3.Location = new System.Drawing.Point(630, 645);
        Command3.Margin = new System.Windows.Forms.Padding(4);
        Command3.Name = "btnEdit";
        Command3.RightToLeft = RightToLeft.No;
        Command3.Size = new System.Drawing.Size(149, 37);
        Command3.TabIndex = 8;
        Command3.Text = "Abbrechen";
        Command3.UseVisualStyleBackColor = false;
        _Text1_1.AcceptsReturn = true;
        _Text1_1.BackColor = SystemColors.Window;
        _Text1_1.Cursor = Cursors.IBeam;
        _Text1_1.ForeColor = SystemColors.WindowText;
        _Text1_1.Location = new System.Drawing.Point(267, 29);
        _Text1_1.Margin = new System.Windows.Forms.Padding(4);
        _Text1_1.MaxLength = 0;
        _Text1_1.Name = "txtLicPart2";
        _Text1_1.RightToLeft = RightToLeft.No;
        _Text1_1.Size = new System.Drawing.Size(232, 25);
        _Text1_1.TabIndex = 7;
        _Text1_1.Tag = "0";
        Command2.BackColor = Color.FromArgb(192, 192, 255);
        Command2.Cursor = Cursors.Default;
        Command2.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Command2.ForeColor = SystemColors.ControlText;
        Command2.Location = new System.Drawing.Point(214, 645);
        Command2.Margin = new System.Windows.Forms.Padding(4);
        Command2.Name = "btnNew";
        Command2.RightToLeft = RightToLeft.No;
        Command2.Size = new System.Drawing.Size(149, 37);
        Command2.TabIndex = 6;
        Command2.Text = "Namen tauschen";
        Command2.UseVisualStyleBackColor = false;
        List2.BackColor = SystemColors.Window;
        List2.Cursor = Cursors.Default;
        List2.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        List2.ForeColor = SystemColors.WindowText;
        List2.ItemHeight = 17;
        List2.Location = new System.Drawing.Point(787, 42);
        List2.Margin = new System.Windows.Forms.Padding(4);
        List2.Name = "List2";
        List2.RightToLeft = RightToLeft.No;
        List2.Size = new System.Drawing.Size(218, 582);
        List2.TabIndex = 3;
        Command1.BackColor = Color.FromArgb(192, 192, 255);
        Command1.Cursor = Cursors.Default;
        Command1.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Command1.ForeColor = SystemColors.ControlText;
        Command1.Location = new System.Drawing.Point(39, 645);
        Command1.Margin = new System.Windows.Forms.Padding(4);
        Command1.Name = "btnCancel4";
        Command1.RightToLeft = RightToLeft.No;
        Command1.Size = new System.Drawing.Size(149, 37);
        Command1.TabIndex = 2;
        Command1.Text = "Suche starten";
        Command1.UseVisualStyleBackColor = false;
        _Text1_0.AcceptsReturn = true;
        _Text1_0.BackColor = SystemColors.Window;
        _Text1_0.Cursor = Cursors.IBeam;
        _Text1_0.ForeColor = SystemColors.WindowText;
        _Text1_0.Location = new System.Drawing.Point(27, 29);
        _Text1_0.Margin = new System.Windows.Forms.Padding(4);
        _Text1_0.MaxLength = 0;
        _Text1_0.Name = "txtLicPart1";
        _Text1_0.RightToLeft = RightToLeft.No;
        _Text1_0.Size = new System.Drawing.Size(232, 25);
        _Text1_0.TabIndex = 1;
        _Text1_0.Tag = "0";
        List1.BackColor = SystemColors.Window;
        List1.Cursor = Cursors.Default;
        List1.Font = new System.Drawing.Font("Courier New", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        List1.ForeColor = SystemColors.WindowText;
        List1.ItemHeight = 17;
        List1.Location = new System.Drawing.Point(35, 62);
        List1.Margin = new System.Windows.Forms.Padding(4);
        List1.Name = "List1";
        List1.RightToLeft = RightToLeft.No;
        List1.Size = new System.Drawing.Size(744, 565);
        List1.TabIndex = 0;
        _Label2_1.BackColor = SystemColors.Control;
        _Label2_1.Cursor = Cursors.Default;
        _Label2_1.Font = new System.Drawing.Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_1.ForeColor = SystemColors.ControlText;
        _Label2_1.Location = new System.Drawing.Point(517, 3);
        _Label2_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        _Label2_1.Name = "lblSep1";
        _Label2_1.RightToLeft = RightToLeft.No;
        _Label2_1.Size = new System.Drawing.Size(228, 22);
        _Label2_1.TabIndex = 9;
        _Label2_1.Text = "oder Vorname der Frau";
        _Label2_0.BackColor = SystemColors.Control;
        _Label2_0.Cursor = Cursors.Default;
        _Label2_0.Font = new System.Drawing.Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_0.ForeColor = SystemColors.ControlText;
        _Label2_0.Location = new System.Drawing.Point(272, 3);
        _Label2_0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        _Label2_0.Name = "lblSep2";
        _Label2_0.RightToLeft = RightToLeft.No;
        _Label2_0.Size = new System.Drawing.Size(228, 22);
        _Label2_0.TabIndex = 5;
        _Label2_0.Text = "Familienname der Frau";
        Label1.BackColor = SystemColors.Control;
        Label1.Cursor = Cursors.Default;
        Label1.Font = new System.Drawing.Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label1.ForeColor = SystemColors.ControlText;
        Label1.Location = new System.Drawing.Point(36, 3);
        Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label1.Name = "Label1";
        Label1.RightToLeft = RightToLeft.No;
        Label1.Size = new System.Drawing.Size(228, 22);
        Label1.TabIndex = 4;
        Label1.Text = "Familienname des Mannes";
        CheckBox1.AutoSize = true;
        CheckBox1.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        CheckBox1.Location = new System.Drawing.Point(432, 659);
        CheckBox1.Margin = new System.Windows.Forms.Padding(4);
        CheckBox1.Name = "cbxIllegitRel";
        CheckBox1.Size = new System.Drawing.Size(160, 21);
        CheckBox1.TabIndex = 11;
        CheckBox1.Text = "Auswahl beibehalten";
        CheckBox1.UseVisualStyleBackColor = true;
        AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        ClientSize = new System.Drawing.Size(1008, 715);
        ControlBox = false;
        Controls.Add(CheckBox1);
        Controls.Add(_Text1_2);
        Controls.Add(Command3);
        Controls.Add(_Text1_1);
        Controls.Add(Command2);
        Controls.Add(List2);
        Controls.Add(Command1);
        Controls.Add(_Text1_0);
        Controls.Add(List1);
        Controls.Add(_Label2_1);
        Controls.Add(_Label2_0);
        Controls.Add(Label1);
        Cursor = Cursors.Default;
        Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Margin = new System.Windows.Forms.Padding(4);
        Name = "Partnerrecherche";
        RightToLeft = RightToLeft.No;
        StartPosition = FormStartPosition.Manual;
        Text = "Partnersuche";
        ResumeLayout(false);
        PerformLayout();
        Command3.Click += new EventHandler(Command3_Click);
        Command2.Click += new EventHandler(Command2_Click);
        List2.DoubleClick += new EventHandler(List2_DoubleClick);
        Command1.Click += new EventHandler(Command1_Click);
        _Text1_0.TextChanged += new EventHandler(_Text1_0_TextChanged);
        List1.DoubleClick += new EventHandler(List1_DoubleClick);
    }

    public ToolTip ToolTip1;
    public Button Command3;
    public Button Command2;
    public ListBox List2;
    public Button Command1;
    public TextBox _Text1_0;
    public ListBox List1;
    public ControlArray<TextBox> Text1;
    public TextBox _Text1_2;
    public TextBox _Text1_1;
    public Label _Label2_1;
    public Label _Label2_0;
    public Label Label1;

    public ControlArray<Label> Label2;



    internal CheckBox CheckBox1;

}