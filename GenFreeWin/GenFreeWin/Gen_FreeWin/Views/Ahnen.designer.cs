using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace GenFreeWin.Views;

internal partial class Ahnen
{
    /*
    [AccessedThroughProperty(nameof(List1))]
    private ListBox _List1;

    [AccessedThroughProperty(nameof(_Label2_9))]
    private Label __Label2_9;

    [AccessedThroughProperty(nameof(_Label2_8))]
    private Label __Label2_8;

    [AccessedThroughProperty(nameof(_Label2_7))]
    private Label __Label2_7;

    [AccessedThroughProperty(nameof(_Frame2_1))]
    private GroupBox __Frame2_1;

    [AccessedThroughProperty(nameof(btnVerify))]
    private Button __Command1_0;

    [AccessedThroughProperty(nameof(_Label2_2))]
    private Label __Label2_2;

    [AccessedThroughProperty(nameof(lblSep1))]
    private Label __Label2_1;

    [AccessedThroughProperty(nameof(lblSep2))]
    private Label __Label2_0;

    [AccessedThroughProperty(nameof(_Frame2_0))]
    private GroupBox __Frame2_0;

    [AccessedThroughProperty(nameof(btnEdit))]
    private Button _Command3;

    [AccessedThroughProperty(nameof(btnNew))]
    private Button _Command2;

    [AccessedThroughProperty(nameof(_Option1_1))]
    private RadioButton __Option1_1;

    [AccessedThroughProperty(nameof(_Option1_0))]
    private RadioButton __Option1_0;

    [AccessedThroughProperty(nameof(frmFamilyresidence))]
    private GroupBox _Frame1;

    [AccessedThroughProperty(nameof(_Command1_2))]
    private Button __Command1_2;

    [AccessedThroughProperty(nameof(btnCancel))]
    private Button __Command1_1;

    [AccessedThroughProperty(nameof(lblDisplayHint))]
    private Label Label3;

    [AccessedThroughProperty(nameof(Label1_10))]
    private Label __Label1_10;

    [AccessedThroughProperty(nameof(_Label1_9))]
    private Label __Label1_9;

    [AccessedThroughProperty(nameof(_Label1_8))]
    private Label __Label1_8;

    [AccessedThroughProperty(nameof(_Label1_3))]
    private Label __Label1_3;

    [AccessedThroughProperty(nameof(_Label1_0))]
    private Label __Label1_0;

    [AccessedThroughProperty(nameof(Bezeichnung1))]
    private Label _Bezeichnung1;
    */
    public ListBox List1;
    public  Label _Label2_9;
    public  Label _Label2_8;
    public  Label _Label2_7;
    public  GroupBox _Frame2_1;
    public  Button _Command1_0;
   public  Label _Label2_2;
    public  Label _Label2_1;
    public  Label _Label2_0;
    public  GroupBox _Frame2_0;
    public Button Command3;
       public Button Command2;
    public RadioButton _Option1_1;
    public RadioButton _Option1_0;
   public  GroupBox Frame1;
     public Button _Command1_2;
    public Button _Command1_1;
    public Label Label3;
   public Label Label1_10;
    public Label _Label1_9;    
    public Label _Label1_8;
    public Label _Label1_3;
    public Label _Label1_0;
    public  Label Bezeichnung1;

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
        components = new Container();
        ToolTip1 = new ToolTip(components);
        Command3 = new Button();
        _Command1_2 = new Button();
        List1 = new ListBox();
        _Frame2_1 = new GroupBox();
        _Label2_9 = new Label();
        _Label2_8 = new Label();
        _Label2_7 = new Label();
        _Command1_0 = new Button();
        _Frame2_0 = new GroupBox();
        _Label2_2 = new Label();
        _Label2_1 = new Label();
        _Label2_0 = new Label();
        Frame1 = new GroupBox();
        Command2 = new Button();
        _Option1_1 = new RadioButton();
        _Option1_0 = new RadioButton();
        _Command1_1 = new Button();
        Label3 = new Label();
        Label1_10 = new Label();
        _Label1_9 = new Label();
        _Label1_8 = new Label();
        _Label1_3 = new Label();
        _Label1_0 = new Label();
        Bezeichnung1 = new Label();
        //btnCancel4 = new ControlArray<Button>(components);
        //Frame2 = new ControlArray<GroupBox>(components);
        //lblEnterLicence = new ControlArray<Label>(components);
        //lblState = new ControlArray<Label>(components);
        //Option1 = new ControlArray<RadioButton>(components);
        _Frame2_1.SuspendLayout();
        _Frame2_0.SuspendLayout();
        Frame1.SuspendLayout();
        /*
        ((ISupportInitialize)btnCancel4).BeginInit();
        ((ISupportInitialize)Frame2).BeginInit();
        ((ISupportInitialize)lblEnterLicence).BeginInit();
        ((ISupportInitialize)lblState).BeginInit();
        ((ISupportInitialize)Option1).BeginInit();*/
        SuspendLayout();
        Command3.BackColor = SystemColors.Control;
        Command3.Cursor = Cursors.Default;
        Command3.DialogResult = DialogResult.Cancel;
        Command3.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Command3.ForeColor = SystemColors.ControlText;
        Command3.Location = new Point(736, 389);
        Command3.Margin = new Padding(3, 4, 3, 4);
        Command3.Name = "btnEdit";
        Command3.RightToLeft = RightToLeft.No;
        Command3.Size = new Size(166, 50);
        Command3.TabIndex = 19;
        Command3.Text = "Beenden (Schnellausstieg)";
        ToolTip1.SetToolTip(Command3, "Beendet das Programm auch bei laufenden Berechnungen");
        Command3.UseVisualStyleBackColor = false;
        Command3.Click += Command3_Click;
        _Command1_2.BackColor = SystemColors.Control;
        _Command1_2.Cursor = Cursors.Default;
        _Command1_2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_2.ForeColor = SystemColors.ControlText;
        _Command1_2.Location = new Point(557, 413);
        //btnCancel4.SetIndex(_Command1_2, 2);
        _Command1_2.Margin = new Padding(3, 4, 3, 4);
        _Command1_2.Name = "_Command1_2";
        _Command1_2.RightToLeft = RightToLeft.No;
        _Command1_2.Size = new Size(142, 26);
        _Command1_2.TabIndex = 3;
        _Command1_2.Text = "Abbrechen";
        ToolTip1.SetToolTip(_Command1_2, "Zurück zum Hauptmenue");
        _Command1_2.UseVisualStyleBackColor = false;
        List1.BackColor = SystemColors.Window;
        List1.Cursor = Cursors.Default;
        List1.ForeColor = SystemColors.WindowText;
        List1.ItemHeight = 17;
        List1.Location = new Point(114, 374);
        List1.Margin = new Padding(3, 4, 3, 4);
        List1.Name = "List1";
        List1.RightToLeft = RightToLeft.No;
        List1.Size = new Size(131, 55);
        List1.Sorted = true;
        List1.TabIndex = 29;
        List1.Visible = false;
        _Frame2_1.BackColor = SystemColors.Control;
        _Frame2_1.Controls.Add(_Label2_9);
        _Frame2_1.Controls.Add(_Label2_8);
        _Frame2_1.Controls.Add(_Label2_7);
        _Frame2_1.ForeColor = SystemColors.ControlText;
        _Frame2_1.Location = new Point(5, 493);
        //Frame2.SetIndex(_Frame2_1, 1);
        _Frame2_1.Margin = new Padding(3, 4, 3, 4);
        _Frame2_1.Name = "_Frame2_1";
        _Frame2_1.Padding = new Padding(3, 4, 3, 4);
        _Frame2_1.RightToLeft = RightToLeft.No;
        _Frame2_1.Size = new Size(442, 113);
        _Frame2_1.TabIndex = 24;
        _Frame2_1.TabStop = false;
        _Label2_9.BackColor = Color.White;
        _Label2_9.Cursor = Cursors.Default;
        _Label2_9.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_9.ForeColor = SystemColors.ControlText;
        _Label2_9.Location = new Point(6, 24);
        //lblState.SetIndex(_Label2_9, 9);
        _Label2_9.Name = "_Label2_9";
        _Label2_9.RightToLeft = RightToLeft.No;
        _Label2_9.Size = new Size(431, 21);
        _Label2_9.TabIndex = 27;
        _Label2_8.BackColor = Color.White;
        _Label2_8.Cursor = Cursors.Default;
        _Label2_8.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_8.ForeColor = SystemColors.ControlText;
        _Label2_8.Location = new Point(6, 47);
        //lblState.SetIndex(_Label2_8, 8);
        _Label2_8.Name = "_Label2_8";
        _Label2_8.RightToLeft = RightToLeft.No;
        _Label2_8.Size = new Size(431, 21);
        _Label2_8.TabIndex = 26;
        _Label2_7.BackColor = Color.White;
        _Label2_7.Cursor = Cursors.Default;
        _Label2_7.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_7.ForeColor = SystemColors.ControlText;
        _Label2_7.Location = new Point(6, 70);
        //lblState.SetIndex(_Label2_7, 7);
        _Label2_7.Name = "_Label2_7";
        _Label2_7.RightToLeft = RightToLeft.No;
        _Label2_7.Size = new Size(431, 21);
        _Label2_7.TabIndex = 25;
        _Command1_0.BackColor = SystemColors.Control;
        _Command1_0.Cursor = Cursors.Default;
        _Command1_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_0.ForeColor = SystemColors.ControlText;
        _Command1_0.Location = new Point(7, 459);
        //btnCancel4.SetIndex(btnVerify, 0);
        _Command1_0.Margin = new Padding(3, 4, 3, 4);
        _Command1_0.Name = "btnVerify";
        _Command1_0.RightToLeft = RightToLeft.No;
        _Command1_0.Size = new Size(442, 26);
        _Command1_0.TabIndex = 1;
        _Command1_0.Text = "Ahnen";
        _Command1_0.UseVisualStyleBackColor = false;
        _Frame2_0.BackColor = SystemColors.Control;
        _Frame2_0.Controls.Add(_Label2_2);
        _Frame2_0.Controls.Add(_Label2_1);
        _Frame2_0.Controls.Add(_Label2_0);
        _Frame2_0.ForeColor = SystemColors.ControlText;
        _Frame2_0.Location = new Point(457, 493);
        //Frame2.SetIndex(_Frame2_0, 0);
        _Frame2_0.Margin = new Padding(3, 4, 3, 4);
        _Frame2_0.Name = "_Frame2_0";
        _Frame2_0.Padding = new Padding(3, 4, 3, 4);
        _Frame2_0.RightToLeft = RightToLeft.No;
        _Frame2_0.Size = new Size(442, 113);
        _Frame2_0.TabIndex = 20;
        _Frame2_0.TabStop = false;
        _Label2_2.BackColor = Color.White;
        _Label2_2.Cursor = Cursors.Default;
        _Label2_2.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_2.ForeColor = SystemColors.ControlText;
        _Label2_2.Location = new Point(5, 70);
        //lblState.SetIndex(_Label2_2, 2);
        _Label2_2.Name = "_Label2_2";
        _Label2_2.RightToLeft = RightToLeft.No;
        _Label2_2.Size = new Size(431, 21);
        _Label2_2.TabIndex = 23;
        _Label2_1.BackColor = Color.White;
        _Label2_1.Cursor = Cursors.Default;
        _Label2_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_1.ForeColor = SystemColors.ControlText;
        _Label2_1.Location = new Point(5, 47);
        //lblState.SetIndex(lblSep1, 1);
        _Label2_1.Name = "lblSep1";
        _Label2_1.RightToLeft = RightToLeft.No;
        _Label2_1.Size = new Size(431, 21);
        _Label2_1.TabIndex = 22;
        _Label2_0.BackColor = Color.White;
        _Label2_0.Cursor = Cursors.Default;
        _Label2_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label2_0.ForeColor = SystemColors.ControlText;
        _Label2_0.Location = new Point(5, 24);
        //lblState.SetIndex(lblSep2, 0);
        _Label2_0.Name = "lblSep2";
        _Label2_0.RightToLeft = RightToLeft.No;
        _Label2_0.Size = new Size(431, 21);
        _Label2_0.TabIndex = 21;
        Frame1.BackColor = SystemColors.Control;
        Frame1.Controls.Add(Command2);
        Frame1.Controls.Add(_Option1_1);
        Frame1.Controls.Add(_Option1_0);
        Frame1.ForeColor = SystemColors.ControlText;
        Frame1.Location = new Point(341, 228);
        Frame1.Margin = new Padding(3, 4, 3, 4);
        Frame1.Name = "frmFamilyresidence";
        Frame1.Padding = new Padding(3, 4, 3, 4);
        Frame1.RightToLeft = RightToLeft.No;
        Frame1.Size = new Size(285, 169);
        Frame1.TabIndex = 12;
        Frame1.TabStop = false;
        Frame1.Visible = false;
        Command2.BackColor = SystemColors.Control;
        Command2.Cursor = Cursors.Default;
        Command2.ForeColor = SystemColors.ControlText;
        Command2.Location = new Point(89, 126);
        Command2.Margin = new Padding(3, 4, 3, 4);
        Command2.Name = "btnNew";
        Command2.RightToLeft = RightToLeft.No;
        Command2.Size = new Size(97, 24);
        Command2.TabIndex = 15;
        Command2.Text = "Weiter";
        Command2.UseVisualStyleBackColor = false;
        Command2.Click += Command2_Click;
        _Option1_1.BackColor = Color.FromArgb(255, 128, 128);
        _Option1_1.Cursor = Cursors.Default;
        _Option1_1.ForeColor = SystemColors.ControlText;
        _Option1_1.Location = new Point(18, 80);
        //Option1.SetIndex(_Option1_1, 1);
        _Option1_1.Margin = new Padding(3, 4, 3, 4);
        _Option1_1.Name = "_Option1_1";
        _Option1_1.RightToLeft = RightToLeft.No;
        _Option1_1.Size = new Size(250, 26);
        _Option1_1.TabIndex = 14;
        _Option1_1.TabStop = true;
        _Option1_1.Text = "Proband hat Generationenziffer 1";
        _Option1_1.UseVisualStyleBackColor = false;
        _Option1_0.BackColor = Color.FromArgb(255, 128, 128);
        _Option1_0.Checked = true;
        _Option1_0.Cursor = Cursors.Default;
        _Option1_0.ForeColor = SystemColors.ControlText;
        _Option1_0.Location = new Point(18, 49);
        //Option1.SetIndex(_Option1_0, 0);
        _Option1_0.Margin = new Padding(3, 4, 3, 4);
        _Option1_0.Name = "_Option1_0";
        _Option1_0.RightToLeft = RightToLeft.No;
        _Option1_0.Size = new Size(250, 26);
        _Option1_0.TabIndex = 13;
        _Option1_0.TabStop = true;
        _Option1_0.Text = "Proband hat Generationenziffer 0";
        _Option1_0.UseVisualStyleBackColor = false;
        _Command1_1.BackColor = SystemColors.Control;
        _Command1_1.Cursor = Cursors.Default;
        _Command1_1.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Command1_1.ForeColor = SystemColors.ControlText;
        _Command1_1.Location = new Point(455, 459);
        //btnCancel4.SetIndex(btnCancel, 1);
        _Command1_1.Margin = new Padding(3, 4, 3, 4);
        _Command1_1.Name = "btnCancel";
        _Command1_1.RightToLeft = RightToLeft.No;
        _Command1_1.Size = new Size(447, 26);
        _Command1_1.TabIndex = 2;
        _Command1_1.Text = "Nachfahren";
        _Command1_1.UseVisualStyleBackColor = false;
        Label3.AutoSize = true;
        Label3.BackColor = Color.White;
        Label3.Cursor = Cursors.Default;
        Label3.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label3.ForeColor = SystemColors.ControlText;
        Label3.Location = new Point(21, 92);
        Label3.MinimumSize = new Size(20, 0);
        Label3.Name = "lblDisplayHint";
        Label3.RightToLeft = RightToLeft.No;
        Label3.Size = new Size(20, 16);
        Label3.TabIndex = 28;
        Label1_10.BackColor = Color.Red;
        Label1_10.Cursor = Cursors.Default;
        Label1_10.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label1_10.ForeColor = Color.Yellow;
        Label1_10.Location = new Point(0, 0);
        //lblEnterLicence.SetIndex(Label1_10, 10);
        Label1_10.Name = "_Label1_10";
        Label1_10.RightToLeft = RightToLeft.No;
        Label1_10.Size = new Size(921, 19);
        Label1_10.TabIndex = 18;
        Label1_10.TextAlign = ContentAlignment.TopCenter;
        _Label1_9.BackColor = Color.Red;
        _Label1_9.Cursor = Cursors.Default;
        _Label1_9.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_9.ForeColor = Color.Yellow;
        _Label1_9.Location = new Point(0, 22);
        //lblEnterLicence.SetIndex(_Label1_9, 9);
        _Label1_9.Name = "_Label1_9";
        _Label1_9.RightToLeft = RightToLeft.No;
        _Label1_9.Size = new Size(921, 19);
        _Label1_9.TabIndex = 17;
        _Label1_9.TextAlign = ContentAlignment.TopCenter;
        _Label1_8.BackColor = Color.Red;
        _Label1_8.Cursor = Cursors.Default;
        _Label1_8.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        _Label1_8.ForeColor = Color.Yellow;
        _Label1_8.Location = new Point(0, 44);
        //lblEnterLicence.SetIndex(_Label1_8, 8);
        _Label1_8.Name = "_Label1_8";
        _Label1_8.RightToLeft = RightToLeft.No;
        _Label1_8.Size = new Size(921, 19);
        _Label1_8.TabIndex = 16;
        _Label1_8.TextAlign = ContentAlignment.TopCenter;
        _Label1_3.BackColor = Color.White;
        _Label1_3.Cursor = Cursors.Default;
        _Label1_3.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_3.ForeColor = SystemColors.ControlText;
        _Label1_3.Location = new Point(158, 228);
        //lblEnterLicence.SetIndex(_Label1_3, 3);
        _Label1_3.Name = "_Label1_3";
        _Label1_3.RightToLeft = RightToLeft.No;
        _Label1_3.Size = new Size(250, 16);
        _Label1_3.TabIndex = 7;
        _Label1_3.Visible = false;
        _Label1_0.BackColor = SystemColors.Control;
        _Label1_0.Cursor = Cursors.Default;
        _Label1_0.Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        _Label1_0.ForeColor = SystemColors.ControlText;
        _Label1_0.Location = new Point(35, 225);
        //lblEnterLicence.SetIndex(_Label1_0, 0);
        _Label1_0.Name = "_Label1_0";
        _Label1_0.RightToLeft = RightToLeft.No;
        _Label1_0.Size = new Size(117, 19);
        _Label1_0.TabIndex = 4;
        _Label1_0.Text = "Person in Arbeit:";
        _Label1_0.TextAlign = ContentAlignment.TopRight;
        _Label1_0.Visible = false;
        Bezeichnung1.BackColor = Color.FromArgb(192, 192, 192);
        Bezeichnung1.Cursor = Cursors.Default;
        Bezeichnung1.ForeColor = Color.Black;
        Bezeichnung1.Location = new Point(2, 131);
        Bezeichnung1.Name = "Bezeichnung1";
        Bezeichnung1.RightToLeft = RightToLeft.No;
        Bezeichnung1.Size = new Size(914, 19);
        Bezeichnung1.TabIndex = 0;
        Bezeichnung1.Text = "Berechnungen";
        Bezeichnung1.TextAlign = ContentAlignment.TopCenter;
        AcceptButton = Command2;
        AutoScaleDimensions = new SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(192, 192, 192);
        CancelButton = Command3;
        ClientSize = new Size(1018, 725);
        Controls.Add(List1);
        Controls.Add(_Frame2_1);
        Controls.Add(_Command1_0);
        Controls.Add(_Frame2_0);
        Controls.Add(Command3);
        Controls.Add(Frame1);
        Controls.Add(_Command1_2);
        Controls.Add(_Command1_1);
        Controls.Add(Label3);
        Controls.Add(Label1_10);
        Controls.Add(_Label1_9);
        Controls.Add(_Label1_8);
        Controls.Add(_Label1_3);
        Controls.Add(_Label1_0);
        Controls.Add(Bezeichnung1);
        Cursor = Cursors.Default;
        Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        ForeColor = Color.Black;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Margin = new Padding(3, 4, 3, 4);
        Name = "Ahnen";
        RightToLeft = RightToLeft.No;
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.Manual;
        Text = "Ahnen- und Nachfahrenberechnung";
        _Frame2_1.ResumeLayout(false);
        _Frame2_0.ResumeLayout(false);
        Frame1.ResumeLayout(false);
        /*
        ((ISupportInitialize)btnCancel4).EndInit();
        ((ISupportInitialize)Frame2).EndInit();
        ((ISupportInitialize)lblEnterLicence).EndInit();
        ((ISupportInitialize)lblState).EndInit();
        ((ISupportInitialize)Option1).EndInit();*/
        ResumeLayout(false);
        PerformLayout();
    }

}