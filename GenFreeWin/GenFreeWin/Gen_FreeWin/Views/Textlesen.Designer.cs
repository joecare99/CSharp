using BaseLib.Helper;
using GenFreeWin.Data;
using GenFreeWin.Main;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
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
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Textlesen
{
    /// <summary> 
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    [DebuggerNonUserCode]
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(Disposing);
    }

    public ToolTip ToolTip1;
    public ControlArray<CheckBox> Check2;
    public ControlArray<Label> Bezeichnung1;
    public ControlArray<Label> Label1;
    public ControlArray<Label> Bez;
    public ControlArray<Button> Bef;
    public ControlArray<Button> Command1;


/*
        btnCancel.Click += new EventHandler(Command3_Click);
        List3.DoubleClick += new EventHandler(List3_DoubleClick);
        List1.DoubleClick += new EventHandler(List1_DoubleClick);
        Liste1.DoubleClick += new EventHandler(Liste1_DoubleClick);
        Text1.TextChanged += new EventHandler(Text1_TextChanged);
        btnMoveNameToAlias.Click += new EventHandler(btnMoveNameToAlias_Click);
        Text3.KeyUp += new KeyEventHandler(Text3_KeyUp);
        Text2.KeyUp += new KeyEventHandler(Text2_KeyUp);
        Text4.KeyUp += new KeyEventHandler(Text4_KeyUp);
        Bezeichnung2.TextChanged += new EventHandler(Bezeichnung2_TextChanged);
       
        btnReqHint.Click += new EventHandler(btnShowAsocPeople_Click);
        btnDuplBttn3.Click += new EventHandler(btnReenter_Click);
        lblURL.Click += new EventHandler(Label7_Click);
        lblResidence.Click += new EventHandler(Label7_Click);
        lblOccubation.Click += new EventHandler(Label7_Click);
        Label10.Click += new EventHandler(Label7_Click);
        Label11.Click += new EventHandler(Label7_Click);
        Label12.Click += new EventHandler(Label7_Click);
        lblPredicate.Click += new EventHandler(Label7_Click);
        lblNickName.Click += new EventHandler(Label7_Click);
        Label15.Click += new EventHandler(Label7_Click);
        Label16.Click += new EventHandler(Label7_Click);
        Label17.Click += new EventHandler(Label7_Click);
        Label23.Click += new EventHandler(Label7_Click);
        Label18.Click += new EventHandler(Label7_Click);
        Label22.Click += new EventHandler(Label7_Click);
        Label21.Click += new EventHandler(Label7_Click);
        Label20.Click += new EventHandler(Label7_Click);
        Label19.Click += new EventHandler(Label7_Click);
        Label24.Click += new EventHandler(Label7_Click);
        Label25.Click += new EventHandler(Label7_Click);
        Label26.Click += new EventHandler(Label7_Click);
        Label27.Click += new EventHandler(Label7_Click);
        Label28.Click += new EventHandler(Label7_Click);
        btnMoveToCause.Click += new EventHandler(btnMoveToCause_Click);
        Label29.Click += new EventHandler(Label7_Click);
        btnChangeSexToF.Click += new EventHandler(btnChangeSexToF_Click);
        btnChangeSexToM.Click += new EventHandler(btnChangeSexToM_Click);
        btnMoveToChurchCemet.Click += new EventHandler(btnMoveToChurchCemet_Click);
        btnMoveToEntityAnot.Click += new EventHandler(btnMoveToEntityAnot_Click);
        btnMoveToLowerDateAnot.Click += new EventHandler(btnMoveToLowerDateAnot_Click);  
        btnDeleteEntry.Click += new EventHandler(frmSrch.btnDeleteEntry_Click);
        btnMoveToDateAnot.Click += new EventHandler(OpenCalculations);

         */
//    [ListBinding(nameof(ITextLesenViewModel.List1_List))]
    public ListBox List1;
    public ListBox List2;
    public ListBox List3;
    public ListBox List4;
    public ListBox ListBox1;
    public ListBox Liste1;
    internal ListBox Sortbox1;

    [VisibilityBinding(nameof(ITextLesenViewModel.Frame1_Visible))]
    public GroupBox Frame1;
    public GroupBox Frame2;

    public Button btnShowAsocPeople;
    internal Button Button2;
    public Button btnReenter;
    public Button Command3;
    public Button _Command1_1;
    public Button _Command1_0;
    public Button btnMoveNameToAlias;
    public Button _Bef_0;
    public Button _Bef_1;
    public Button _Bef_2;
    public Button _Bef_3;
    public Button _Bef_4;
    public Button btnMoveToCause;
    [VisibilityBinding(nameof(ITextLesenViewModel.ChangeSex_Visibility))]
    [CommandBinding(nameof(ITextLesenViewModel.ChangeSexToFCommand))]
    public Button btnChangeSexToF;
    [VisibilityBinding(nameof(ITextLesenViewModel.ChangeSex_Visibility))]
    [CommandBinding(nameof(ITextLesenViewModel.ChangeSexToMCommand))]
    public Button btnChangeSexToM;
    [CommandBinding(nameof(ITextLesenViewModel.MoveToChurchCemetCommand))]
    public Button btnMoveToChurchCemet;
    [CommandBinding(nameof(ITextLesenViewModel.MoveToEntityAnotCommand))]
    public Button btnMoveToEntityAnot;
    public Button btnMoveToLowerDateAnot;
    public Button btnDeleteEntry;
    public Button btnMoveToDateAnot;

    public CheckBox _Check2_0;
    public CheckBox _Check2_1;
    [CheckedBinding(nameof(ITextLesenViewModel.Check1_Checked))]
    public CheckBox Check1;
    [CheckedBinding(nameof(ITextLesenViewModel.Check3_Checked))]
    public CheckBox Check3;

    public RichTextBox RichTextBox1;
    public RichTextBox RTB;

    [TextBinding(nameof(ITextLesenViewModel.Text1_Text))]
    public TextBox Text1;
    [TextBinding(nameof(ITextLesenViewModel.Text2_Text))]
    public TextBox Text2;
    [TextBinding(nameof(ITextLesenViewModel.Text3_Text))]
    public TextBox Text3;
    [TextBinding(nameof(ITextLesenViewModel.Text4_Text))]
    public TextBox Text4;

    public Label Label4;
    public Label Label2;
    public Label _Bezeichnung1_1;
    public Label Label3;
    public Label _Label1_4;
    public Label _Label1_3;
    public Label _Label1_2;
    public Label _Label1_1;
    public Label _Label1_0;
    public Label Bezeichnung5;
    public Label _Bezeichnung1_0;
    public Label Bezeichnung2;

    public Label Bezeichnung3;
    public Label Bezeichnung6;
    internal Label Label5;
    internal Label Label6;
    public Label Label7;
    public Label Label8;
    public Label Label9;
    public Label Label10;
    public Label Label11;
    public Label Label12;
    public Label Label13;
    public Label Label14;
    public Label Label15;
    public Label Label16;
    public Label Label17;
    public Label Label23;
    public Label Label18;
    public Label Label22;
    public Label Label21;
    public Label Label20;
    public Label Label19;
    public Label Label24;
    public Label Label25;
    public Label Label26;
    public Label Label27;
    public Label Label28;
    public Label Label29;

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Command3 = new System.Windows.Forms.Button();
            this.List4 = new System.Windows.Forms.ListBox();
            this.List3 = new System.Windows.Forms.ListBox();
            this.List2 = new System.Windows.Forms.ListBox();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.btnChangeSexToF = new System.Windows.Forms.Button();
            this.btnChangeSexToM = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.Sortbox1 = new System.Windows.Forms.ListBox();
            this.btnShowAsocPeople = new System.Windows.Forms.Button();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this.List1 = new System.Windows.Forms.ListBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Liste1 = new System.Windows.Forms.ListBox();
            this.Text1 = new System.Windows.Forms.TextBox();
            this._Bef_1 = new System.Windows.Forms.Button();
            this._Bef_2 = new System.Windows.Forms.Button();
            this._Bef_3 = new System.Windows.Forms.Button();
            this._Bef_4 = new System.Windows.Forms.Button();
            this.btnMoveNameToAlias = new System.Windows.Forms.Button();
            this._Check2_0 = new System.Windows.Forms.CheckBox();
            this.Text3 = new System.Windows.Forms.TextBox();
            this._Check2_1 = new System.Windows.Forms.CheckBox();
            this._Bef_0 = new System.Windows.Forms.Button();
            this.RTB = new System.Windows.Forms.RichTextBox();
            this.Frame2 = new System.Windows.Forms.GroupBox();
            this.Text2 = new System.Windows.Forms.TextBox();
            this.Check1 = new System.Windows.Forms.CheckBox();
            this.Text4 = new System.Windows.Forms.TextBox();
            this.Check3 = new System.Windows.Forms.CheckBox();
            this._Bezeichnung1_1 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label23 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label24 = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this._Label1_4 = new System.Windows.Forms.Label();
            this._Label1_3 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.Bezeichnung5 = new System.Windows.Forms.Label();
            this._Bezeichnung1_0 = new System.Windows.Forms.Label();
            this.Bezeichnung2 = new System.Windows.Forms.Label();
            this.Bezeichnung3 = new System.Windows.Forms.Label();
            this.Bezeichnung6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.btnReenter = new System.Windows.Forms.Button();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label25 = new System.Windows.Forms.Label();
            this.Label26 = new System.Windows.Forms.Label();
            this.Label27 = new System.Windows.Forms.Label();
            this.Label28 = new System.Windows.Forms.Label();
            this.btnMoveToCause = new System.Windows.Forms.Button();
            this.Label29 = new System.Windows.Forms.Label();
            this.btnMoveToChurchCemet = new System.Windows.Forms.Button();
            this.btnMoveToEntityAnot = new System.Windows.Forms.Button();
            this.btnMoveToLowerDateAnot = new System.Windows.Forms.Button();
            this.btnDeleteEntry = new System.Windows.Forms.Button();
            this.btnMoveToDateAnot = new System.Windows.Forms.Button();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.Frame1.SuspendLayout();
            this.Frame2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.Command3.BackColor = System.Drawing.SystemColors.Control;
            this.Command3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Command3.Enabled = false;
            this.Command3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command3.Location = new System.Drawing.Point(685, 626);
            this.Command3.Name = "Command3";
            this.Command3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Command3.Size = new System.Drawing.Size(234, 23);
            this.Command3.TabIndex = 54;
            this.Command3.Text = "Nichtverwendete Texte löschen";
            this.Command3.UseVisualStyleBackColor = false;
            this.Command3.Visible = false;
            this.Command3.Click += new System.EventHandler(this.Command3_Click);
            // 
            // List4
            // 
            this.List4.BackColor = System.Drawing.SystemColors.Window;
            this.List4.Cursor = System.Windows.Forms.Cursors.Default;
            this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List4.ItemHeight = 19;
            this.List4.Location = new System.Drawing.Point(695, 514);
            this.List4.Name = "List4";
            this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List4.Size = new System.Drawing.Size(170, 80);
            this.List4.TabIndex = 48;
            this.List4.Visible = false;
            // 
            // List3
            // 
            this.List3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.List3.Cursor = System.Windows.Forms.Cursors.Default;
            this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List3.ItemHeight = 19;
            this.List3.Location = new System.Drawing.Point(1, 427);
            this.List3.Name = "List3";
            this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List3.Size = new System.Drawing.Size(421, 175);
            this.List3.Sorted = true;
            this.List3.TabIndex = 41;
            this.List3.Visible = false;
            this.List3.DoubleClick += new System.EventHandler(this.List3_DoubleClick);
            // 
            // List2
            // 
            this.List2.BackColor = System.Drawing.SystemColors.Window;
            this.List2.Cursor = System.Windows.Forms.Cursors.Default;
            this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List2.ItemHeight = 19;
            this.List2.Location = new System.Drawing.Point(560, 570);
            this.List2.Name = "List2";
            this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List2.Size = new System.Drawing.Size(106, 4);
            this.List2.TabIndex = 39;
            this.List2.Visible = false;
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Frame1.Controls.Add(this.btnChangeSexToF);
            this.Frame1.Controls.Add(this.btnChangeSexToM);
            this.Frame1.Controls.Add(this.Button2);
            this.Frame1.Controls.Add(this.Sortbox1);
            this.Frame1.Controls.Add(this.btnShowAsocPeople);
            this.Frame1.Controls.Add(this.RichTextBox1);
            this.Frame1.Controls.Add(this._Command1_1);
            this.Frame1.Controls.Add(this._Command1_0);
            this.Frame1.Controls.Add(this.List1);
            this.Frame1.Controls.Add(this.Label4);
            this.Frame1.Controls.Add(this.Label2);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1.Location = new System.Drawing.Point(843, 225);
            this.Frame1.Name = "Frame1";
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(144, 162);
            this.Frame1.TabIndex = 18;
            this.Frame1.TabStop = false;
            this.Frame1.Visible = false;
            // 
            // btnChangeSexToF
            // 
            this.btnChangeSexToF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnChangeSexToF.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnChangeSexToF.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnChangeSexToF.Location = new System.Drawing.Point(394, 583);
            this.btnChangeSexToF.Name = "btnChangeSexToF";
            this.btnChangeSexToF.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnChangeSexToF.Size = new System.Drawing.Size(359, 31);
            this.btnChangeSexToF.TabIndex = 60;
            this.btnChangeSexToF.Text = "Geschlecht aller angezeigten Personen ändern in: F";
            this.btnChangeSexToF.UseVisualStyleBackColor = false;
            this.btnChangeSexToF.Visible = false;
            // 
            // btnChangeSexToM
            // 
            this.btnChangeSexToM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnChangeSexToM.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnChangeSexToM.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnChangeSexToM.Location = new System.Drawing.Point(409, 583);
            this.btnChangeSexToM.Name = "btnChangeSexToM";
            this.btnChangeSexToM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnChangeSexToM.Size = new System.Drawing.Size(359, 31);
            this.btnChangeSexToM.TabIndex = 59;
            this.btnChangeSexToM.Text = "Geschlecht aller angezeigten Personen ändern in: M";
            this.btnChangeSexToM.UseVisualStyleBackColor = false;
            this.btnChangeSexToM.Visible = false;
            // 
            // btnBack
            // 
            this.Button2.Location = new System.Drawing.Point(732, 429);
            this.Button2.Name = "btnPrev";
            this.Button2.Size = new System.Drawing.Size(106, 31);
            this.Button2.TabIndex = 58;
            this.Button2.Text = "Für Gedcom";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Visible = false;
            // 
            // Sortbox1
            // 
            this.Sortbox1.FormattingEnabled = true;
            this.Sortbox1.ItemHeight = 19;
            this.Sortbox1.Location = new System.Drawing.Point(876, 589);
            this.Sortbox1.Name = "Sortbox1";
            this.Sortbox1.Size = new System.Drawing.Size(120, 42);
            this.Sortbox1.Sorted = true;
            this.Sortbox1.TabIndex = 57;
            this.Sortbox1.Visible = false;
            // 
            // btnShowAsocPeople
            // 
            this.btnShowAsocPeople.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnShowAsocPeople.Location = new System.Drawing.Point(368, 589);
            this.btnShowAsocPeople.Name = "btnShowAsocPeople";
            this.btnShowAsocPeople.Size = new System.Drawing.Size(183, 42);
            this.btnShowAsocPeople.TabIndex = 53;
            this.btnShowAsocPeople.Text = "Zugehörige Personen anzeigen";
            this.btnShowAsocPeople.UseVisualStyleBackColor = false;
            this.btnShowAsocPeople.Visible = false;
            this.btnShowAsocPeople.Click += new System.EventHandler(this.btnShowAsocPeople_Click);
            // 
            // RichTextBox1
            // 
            this.RichTextBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextBox1.Location = new System.Drawing.Point(6, 28);
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.Size = new System.Drawing.Size(721, 548);
            this.RichTextBox1.TabIndex = 50;
            this.RichTextBox1.Text = "RichText";
            // 
            // _Command1_1
            // 
            this._Command1_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_1.Location = new System.Drawing.Point(208, 589);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_1.Size = new System.Drawing.Size(154, 31);
            this._Command1_1.TabIndex = 47;
            this._Command1_1.Text = "Drucken";
            this._Command1_1.UseVisualStyleBackColor = false;
            // 
            // _Command1_0
            // 
            this._Command1_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_0.Location = new System.Drawing.Point(46, 589);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_0.Size = new System.Drawing.Size(154, 31);
            this._Command1_0.TabIndex = 35;
            this._Command1_0.Text = "btnCancel4";
            this._Command1_0.UseVisualStyleBackColor = false;
            // 
            // List1
            // 
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 22;
            this.List1.Location = new System.Drawing.Point(3, 24);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List1.Size = new System.Drawing.Size(705, 532);
            this.List1.Sorted = true;
            this.List1.TabIndex = 31;
            this.List1.DoubleClick += new System.EventHandler(this.List1_DoubleClick);
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.SystemColors.Control;
            this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label4.Location = new System.Drawing.Point(746, 30);
            this.Label4.Name = "Label4";
            this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label4.Size = new System.Drawing.Size(230, 63);
            this.Label4.TabIndex = 52;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(729, 485);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(127, 29);
            this.Label2.TabIndex = 46;
            // 
            // Liste1
            // 
            this.Liste1.BackColor = System.Drawing.Color.White;
            this.Liste1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Liste1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Liste1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.Liste1.ForeColor = System.Drawing.Color.Black;
            this.Liste1.IntegralHeight = false;
            this.Liste1.ItemHeight = 23;
            this.Liste1.Location = new System.Drawing.Point(525, 162);
            this.Liste1.Name = "Liste1";
            this.Liste1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Liste1.Size = new System.Drawing.Size(379, 456);
            this.Liste1.Sorted = true;
            this.Liste1.TabIndex = 0;
            this.Liste1.DoubleClick += new System.EventHandler(this.Liste1_DoubleClick);
            // 
            // Text1
            // 
            this.Text1.AcceptsReturn = true;
            this.Text1.BackColor = System.Drawing.Color.White;
            this.Text1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text1.ForeColor = System.Drawing.Color.Black;
            this.Text1.Location = new System.Drawing.Point(45, 258);
            this.Text1.MaxLength = 0;
            this.Text1.Name = "Text1";
            this.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text1.Size = new System.Drawing.Size(454, 20);
            this.Text1.TabIndex = 4;
            this.Text1.TextChanged += new System.EventHandler(this.Text1_TextChanged);
            // 
            // _Bef_1
            // 
            this._Bef_1.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_1.Location = new System.Drawing.Point(7, 653);
            this._Bef_1.Name = "_Bef_1";
            this._Bef_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_1.Size = new System.Drawing.Size(87, 31);
            this._Bef_1.TabIndex = 5;
            this._Bef_1.Text = "&Speichern";
            this._Bef_1.UseVisualStyleBackColor = false;
            // 
            // _Bef_2
            // 
            this._Bef_2.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._Bef_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_2.Location = new System.Drawing.Point(346, 655);
            this._Bef_2.Name = "_Bef_2";
            this._Bef_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_2.Size = new System.Drawing.Size(107, 31);
            this._Bef_2.TabIndex = 12;
            this._Bef_2.Text = "&Menue";
            this._Bef_2.UseVisualStyleBackColor = false;
            // 
            // _Bef_3
            // 
            this._Bef_3.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_3.Location = new System.Drawing.Point(210, 655);
            this._Bef_3.Name = "_Bef_3";
            this._Bef_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_3.Size = new System.Drawing.Size(121, 31);
            this._Bef_3.TabIndex = 11;
            this._Bef_3.UseVisualStyleBackColor = false;
            // 
            // _Bef_4
            // 
            this._Bef_4.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_4.Location = new System.Drawing.Point(102, 655);
            this._Bef_4.Name = "_Bef_4";
            this._Bef_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_4.Size = new System.Drawing.Size(101, 31);
            this._Bef_4.TabIndex = 16;
            this._Bef_4.Text = "Verwendung";
            this._Bef_4.UseVisualStyleBackColor = false;
            // 
            // btnMoveNameToAlias
            // 
            this.btnMoveNameToAlias.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoveNameToAlias.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnMoveNameToAlias.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnMoveNameToAlias.Location = new System.Drawing.Point(3, 304);
            this.btnMoveNameToAlias.Name = "btnMoveNameToAlias";
            this.btnMoveNameToAlias.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnMoveNameToAlias.Size = new System.Drawing.Size(295, 22);
            this.btnMoveNameToAlias.TabIndex = 17;
            this.btnMoveNameToAlias.Text = "&Alten Namen zum Alias verschieben";
            this.btnMoveNameToAlias.UseVisualStyleBackColor = false;
            this.btnMoveNameToAlias.Visible = false;
            this.btnMoveNameToAlias.Click += new System.EventHandler(this.btnMoveNameToAlias_Click);
            // 
            // _Check2_0
            // 
            this._Check2_0.BackColor = System.Drawing.SystemColors.Control;
            this._Check2_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check2_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Check2_0.Location = new System.Drawing.Point(239, 692);
            this._Check2_0.Name = "_Check2_0";
            this._Check2_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Check2_0.Size = new System.Drawing.Size(161, 21);
            this._Check2_0.TabIndex = 33;
            this._Check2_0.Text = "Auswahl umkehren";
            this._Check2_0.UseVisualStyleBackColor = false;
            // 
            // Text3
            // 
            this.Text3.AcceptsReturn = true;
            this.Text3.BackColor = System.Drawing.SystemColors.Window;
            this.Text3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text3.Location = new System.Drawing.Point(85, 280);
            this.Text3.MaxLength = 0;
            this.Text3.Name = "Text3";
            this.Text3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text3.Size = new System.Drawing.Size(422, 20);
            this.Text3.TabIndex = 36;
            this.Text3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Text3_KeyUp);
            // 
            // _Check2_1
            // 
            this._Check2_1.BackColor = System.Drawing.SystemColors.Control;
            this._Check2_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check2_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Check2_1.Location = new System.Drawing.Point(8, 692);
            this._Check2_1.Name = "_Check2_1";
            this._Check2_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Check2_1.Size = new System.Drawing.Size(161, 21);
            this._Check2_1.TabIndex = 40;
            this._Check2_1.Text = "Nach Leitname";
            this._Check2_1.UseVisualStyleBackColor = false;
            this._Check2_1.Visible = false;
            // 
            // _Bef_0
            // 
            this._Bef_0.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_0.Location = new System.Drawing.Point(525, 626);
            this._Bef_0.Name = "_Bef_0";
            this._Bef_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_0.Size = new System.Drawing.Size(151, 23);
            this._Bef_0.TabIndex = 49;
            this._Bef_0.Text = "Diese Liste  drucken";
            this._Bef_0.UseVisualStyleBackColor = false;
            // 
            // RTB
            // 
            this.RTB.BackColor = System.Drawing.Color.White;
            this.RTB.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTB.Location = new System.Drawing.Point(1, 395);
            this.RTB.Name = "RTB";
            this.RTB.RightMargin = 430;
            this.RTB.Size = new System.Drawing.Size(499, 228);
            this.RTB.TabIndex = 55;
            this.RTB.Text = "";
            // 
            // Frame2
            // 
            this.Frame2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Frame2.Controls.Add(this.Text2);
            this.Frame2.Controls.Add(this.Check1);
            this.Frame2.Controls.Add(this.Text4);
            this.Frame2.Controls.Add(this.Check3);
            this.Frame2.Controls.Add(this._Bezeichnung1_1);
            this.Frame2.Controls.Add(this.Label3);
            this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame2.Location = new System.Drawing.Point(3, 137);
            this.Frame2.Name = "Frame2";
            this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame2.Size = new System.Drawing.Size(504, 98);
            this.Frame2.TabIndex = 56;
            this.Frame2.TabStop = false;
            // 
            // Text2
            // 
            this.Text2.AcceptsReturn = true;
            this.Text2.BackColor = System.Drawing.SystemColors.Window;
            this.Text2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text2.Location = new System.Drawing.Point(131, 65);
            this.Text2.MaxLength = 0;
            this.Text2.Multiline = true;
            this.Text2.Name = "Text2";
            this.Text2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text2.Size = new System.Drawing.Size(118, 22);
            this.Text2.TabIndex = 61;
            this.Text2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Text2_KeyUp);
            // 
            // Check1
            // 
            this.Check1.BackColor = System.Drawing.SystemColors.Control;
            this.Check1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Check1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check1.Location = new System.Drawing.Point(279, 65);
            this.Check1.Name = "Check1";
            this.Check1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Check1.Size = new System.Drawing.Size(189, 22);
            this.Check1.TabIndex = 60;
            this.Check1.Text = "Vorauswahl beibehalten";
            this.Check1.UseVisualStyleBackColor = false;
            // 
            // Text4
            // 
            this.Text4.AcceptsReturn = true;
            this.Text4.BackColor = System.Drawing.SystemColors.Window;
            this.Text4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text4.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text4.Location = new System.Drawing.Point(131, 38);
            this.Text4.MaxLength = 0;
            this.Text4.Multiline = true;
            this.Text4.Name = "Text4";
            this.Text4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text4.Size = new System.Drawing.Size(367, 22);
            this.Text4.TabIndex = 58;
            this.Text4.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Text4_KeyUp);
            // 
            // Check3
            // 
            this.Check3.BackColor = System.Drawing.SystemColors.Control;
            this.Check3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Check3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check3.Location = new System.Drawing.Point(19, 12);
            this.Check3.Name = "Check3";
            this.Check3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Check3.Size = new System.Drawing.Size(289, 22);
            this.Check3.TabIndex = 57;
            this.Check3.Text = "Groß- und Kleinschreibung beachten";
            this.Check3.UseVisualStyleBackColor = false;
            // 
            // _Bezeichnung1_1
            // 
            this._Bezeichnung1_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._Bezeichnung1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bezeichnung1_1.ForeColor = System.Drawing.Color.Black;
            this._Bezeichnung1_1.Location = new System.Drawing.Point(16, 65);
            this._Bezeichnung1_1.Name = "_Bezeichnung1_1";
            this._Bezeichnung1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bezeichnung1_1.Size = new System.Drawing.Size(109, 22);
            this._Bezeichnung1_1.TabIndex = 62;
            this._Bezeichnung1_1.Text = "Start mit:";
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.SystemColors.Control;
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.Location = new System.Drawing.Point(16, 38);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label3.Size = new System.Drawing.Size(109, 22);
            this.Label3.TabIndex = 59;
            this.Label3.Text = "Text enthält:";
            // 
            // Label23
            // 
            this.Label23.AutoSize = true;
            this.Label23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label23.Location = new System.Drawing.Point(395, 112);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(64, 19);
            this.Label23.TabIndex = 75;
            this.Label23.Text = "Staaten";
            this.Label23.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label18.Location = new System.Drawing.Point(7, 112);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(118, 19);
            this.Label18.TabIndex = 71;
            this.Label18.Text = "Ereignisnamen";
            this.Label18.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label22
            // 
            this.Label22.AutoSize = true;
            this.Label22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label22.Location = new System.Drawing.Point(337, 112);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(60, 19);
            this.Label22.TabIndex = 74;
            this.Label22.Text = "Länder";
            this.Label22.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label24
            // 
            this.Label24.AutoSize = true;
            this.Label24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label24.Location = new System.Drawing.Point(459, 112);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(58, 19);
            this.Label24.TabIndex = 71;
            this.Label24.Text = "Straße";
            this.Label24.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label21.Location = new System.Drawing.Point(280, 112);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(56, 19);
            this.Label21.TabIndex = 73;
            this.Label21.Text = "Kreise";
            this.Label21.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label20
            // 
            this.Label20.AutoEllipsis = true;
            this.Label20.AutoSize = true;
            this.Label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label20.Location = new System.Drawing.Point(198, 112);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(68, 19);
            this.Label20.TabIndex = 72;
            this.Label20.Text = "Ortsteile";
            this.Label20.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label19.Location = new System.Drawing.Point(154, 112);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(40, 19);
            this.Label19.TabIndex = 71;
            this.Label19.Text = "Orte";
            this.Label19.Click += new System.EventHandler(this.Label7_Click);
            // 
            // _Label1_4
            // 
            this._Label1_4.BackColor = System.Drawing.Color.Red;
            this._Label1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_4.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._Label1_4.Location = new System.Drawing.Point(0, 0);
            this._Label1_4.Name = "_Label1_4";
            this._Label1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_4.Size = new System.Drawing.Size(1024, 20);
            this._Label1_4.TabIndex = 44;
            this._Label1_4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _Label1_3
            // 
            this._Label1_3.BackColor = System.Drawing.Color.Red;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._Label1_3.Location = new System.Drawing.Point(0, 21);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_3.Size = new System.Drawing.Size(1024, 20);
            this._Label1_3.TabIndex = 43;
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _Label1_2
            // 
            this._Label1_2.BackColor = System.Drawing.Color.Red;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this._Label1_2.Location = new System.Drawing.Point(0, 43);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_2.Size = new System.Drawing.Size(1024, 20);
            this._Label1_2.TabIndex = 42;
            this._Label1_2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _Label1_1
            // 
            this._Label1_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_1.Location = new System.Drawing.Point(4, 280);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_1.Size = new System.Drawing.Size(77, 20);
            this._Label1_1.TabIndex = 38;
            this._Label1_1.Text = "Leitname ";
            this._Label1_1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _Label1_0
            // 
            this._Label1_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_0.Location = new System.Drawing.Point(4, 258);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_0.Size = new System.Drawing.Size(38, 20);
            this._Label1_0.TabIndex = 37;
            this._Label1_0.Text = "Text: ";
            this._Label1_0.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Bezeichnung5
            // 
            this.Bezeichnung5.BackColor = System.Drawing.Color.White;
            this.Bezeichnung5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bezeichnung5.ForeColor = System.Drawing.Color.Red;
            this.Bezeichnung5.Location = new System.Drawing.Point(4, 626);
            this.Bezeichnung5.Name = "Bezeichnung5";
            this.Bezeichnung5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung5.Size = new System.Drawing.Size(384, 23);
            this.Bezeichnung5.TabIndex = 13;
            this.Bezeichnung5.Text = "Änderungen wirken auf alle Einträge";
            // 
            // _Bezeichnung1_0
            // 
            this._Bezeichnung1_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._Bezeichnung1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bezeichnung1_0.ForeColor = System.Drawing.Color.Black;
            this._Bezeichnung1_0.Location = new System.Drawing.Point(456, 669);
            this._Bezeichnung1_0.Name = "_Bezeichnung1_0";
            this._Bezeichnung1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bezeichnung1_0.Size = new System.Drawing.Size(110, 17);
            this._Bezeichnung1_0.TabIndex = 8;
            // 
            // Bezeichnung2
            // 
            this.Bezeichnung2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Bezeichnung2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung2.ForeColor = System.Drawing.Color.Black;
            this.Bezeichnung2.Location = new System.Drawing.Point(1, 238);
            this.Bezeichnung2.Name = "Bezeichnung2";
            this.Bezeichnung2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung2.Size = new System.Drawing.Size(377, 20);
            this.Bezeichnung2.TabIndex = 9;
            this.Bezeichnung2.TextChanged += new System.EventHandler(this.Bezeichnung2_TextChanged);
            // 
            // Bezeichnung3
            // 
            this.Bezeichnung3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Bezeichnung3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bezeichnung3.ForeColor = System.Drawing.Color.Black;
            this.Bezeichnung3.Location = new System.Drawing.Point(-2, 62);
            this.Bezeichnung3.Name = "Bezeichnung3";
            this.Bezeichnung3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung3.Size = new System.Drawing.Size(921, 17);
            this.Bezeichnung3.TabIndex = 10;
            this.Bezeichnung3.Text = "Verwalten und Bearbeiten der Texte für";
            this.Bezeichnung3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Bezeichnung6
            // 
            this.Bezeichnung6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Bezeichnung6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bezeichnung6.ForeColor = System.Drawing.Color.Black;
            this.Bezeichnung6.Location = new System.Drawing.Point(525, 137);
            this.Bezeichnung6.Name = "Bezeichnung6";
            this.Bezeichnung6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung6.Size = new System.Drawing.Size(379, 22);
            this.Bezeichnung6.TabIndex = 15;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(4, 238);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(76, 19);
            this.Label5.TabIndex = 57;
            this.Label5.Text = "lblSorting";
            this.Label5.Visible = false;
            // 
            // btnReenter
            // 
            this.btnReenter.Location = new System.Drawing.Point(411, 235);
            this.btnReenter.Name = "btnReenter";
            this.btnReenter.Size = new System.Drawing.Size(108, 23);
            this.btnReenter.TabIndex = 58;
            this.btnReenter.Text = "btnReenter";
            this.btnReenter.UseVisualStyleBackColor = true;
            this.btnReenter.Visible = false;
            this.btnReenter.Click += new System.EventHandler(this.btnReenter_Click);
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(525, 181);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(558, 82);
            this.Label6.TabIndex = 59;
            this.Label6.Text = "Liste der Religionen wird erstellet. \r\nBitte Geduld!";
            this.Label6.Visible = false;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label7.Location = new System.Drawing.Point(7, 85);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(60, 19);
            this.Label7.TabIndex = 60;
            this.Label7.Text = "Namen";
            this.Label7.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label8.Location = new System.Drawing.Point(73, 85);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(146, 19);
            this.Label8.TabIndex = 61;
            this.Label8.Text = "Vornamen weiblich";
            this.Label8.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label9.Location = new System.Drawing.Point(216, 85);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(153, 19);
            this.Label9.TabIndex = 62;
            this.Label9.Text = "Vornamen männlich";
            this.Label9.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label10.Location = new System.Drawing.Point(359, 85);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(108, 19);
            this.Label10.TabIndex = 63;
            this.Label10.Text = "Namenspräfix";
            this.Label10.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label11.Location = new System.Drawing.Point(469, 85);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(106, 19);
            this.Label11.TabIndex = 64;
            this.Label11.Text = "Namenssuffix";
            this.Label11.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label12.Location = new System.Drawing.Point(581, 85);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(44, 19);
            this.Label12.TabIndex = 65;
            this.Label12.Text = "Alias";
            this.Label12.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label13.Location = new System.Drawing.Point(628, 85);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(51, 19);
            this.Label13.TabIndex = 66;
            this.Label13.Text = "Sippe";
            this.Label13.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label14.Location = new System.Drawing.Point(682, 85);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(58, 19);
            this.Label14.TabIndex = 67;
            this.Label14.Text = "Berufe";
            this.Label14.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label15.Location = new System.Drawing.Point(740, 85);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(37, 19);
            this.Label15.TabIndex = 68;
            this.Label15.Text = "Titel";
            this.Label15.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label16.Location = new System.Drawing.Point(783, 85);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(79, 19);
            this.Label16.TabIndex = 69;
            this.Label16.Text = "Kurzbem.";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Label16.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label17.Location = new System.Drawing.Point(941, 85);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(69, 19);
            this.Label17.TabIndex = 70;
            this.Label17.Text = "Prädikat";
            this.Label17.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label25
            // 
            this.Label25.AutoSize = true;
            this.Label25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label25.Location = new System.Drawing.Point(594, 112);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(170, 19);
            this.Label25.TabIndex = 72;
            this.Label25.Text = "Kirche/Friedhof/Firma";
            this.Label25.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label26
            // 
            this.Label26.AutoSize = true;
            this.Label26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label26.Location = new System.Drawing.Point(749, 112);
            this.Label26.Name = "Label26";
            this.Label26.Size = new System.Drawing.Size(54, 19);
            this.Label26.TabIndex = 73;
            this.Label26.Text = "Status";
            this.Label26.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label27
            // 
            this.Label27.AutoSize = true;
            this.Label27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label27.Location = new System.Drawing.Point(820, 112);
            this.Label27.Name = "Label27";
            this.Label27.Size = new System.Drawing.Size(85, 19);
            this.Label27.TabIndex = 74;
            this.Label27.Text = "Religionen";
            this.Label27.Click += new System.EventHandler(this.Label7_Click);
            // 
            // Label28
            // 
            this.Label28.AutoSize = true;
            this.Label28.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label28.Location = new System.Drawing.Point(902, 112);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(110, 19);
            this.Label28.TabIndex = 75;
            this.Label28.Text = "Todesursache";
            this.Label28.Click += new System.EventHandler(this.Label7_Click);
            // 
            // btnMoveToCause
            // 
            this.btnMoveToCause.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoveToCause.Location = new System.Drawing.Point(259, 303);
            this.btnMoveToCause.Name = "btnMoveToCause";
            this.btnMoveToCause.Size = new System.Drawing.Size(228, 22);
            this.btnMoveToCause.TabIndex = 76;
            this.btnMoveToCause.Text = "Zur Todesursache verschieben";
            this.btnMoveToCause.UseVisualStyleBackColor = false;
            this.btnMoveToCause.Visible = false;
            this.btnMoveToCause.Click += new System.EventHandler(this.btnMoveToCause_Click);
            // 
            // Label29
            // 
            this.Label29.AutoSize = true;
            this.Label29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label29.Location = new System.Drawing.Point(523, 112);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(73, 19);
            this.Label29.TabIndex = 77;
            this.Label29.Text = "Haus-Nr.";
            this.Label29.Click += new System.EventHandler(this.Label7_Click);
            // 
            // btnMoveToChurchCemet
            // 
            this.btnMoveToChurchCemet.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoveToChurchCemet.Location = new System.Drawing.Point(4, 303);
            this.btnMoveToChurchCemet.Name = "btnMoveToChurchCemet";
            this.btnMoveToChurchCemet.Size = new System.Drawing.Size(228, 23);
            this.btnMoveToChurchCemet.TabIndex = 78;
            this.btnMoveToChurchCemet.Text = "Zu Kirche/Friedhof  verschieben";
            this.btnMoveToChurchCemet.UseVisualStyleBackColor = false;
            this.btnMoveToChurchCemet.Visible = false;
            // 
            // btnMoveToEntityAnot
            // 
            this.btnMoveToEntityAnot.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoveToEntityAnot.Location = new System.Drawing.Point(3, 374);
            this.btnMoveToEntityAnot.Name = "btnMoveToEntityAnot";
            this.btnMoveToEntityAnot.Size = new System.Drawing.Size(364, 22);
            this.btnMoveToEntityAnot.TabIndex = 79;
            this.btnMoveToEntityAnot.Text = "zu den  Personen/Familienbemerkungen verschieben";
            this.btnMoveToEntityAnot.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMoveToEntityAnot.UseVisualStyleBackColor = true;
            this.btnMoveToEntityAnot.Visible = false;
            // 
            // btnMoveToLowerDateAnot
            // 
            this.btnMoveToLowerDateAnot.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoveToLowerDateAnot.Location = new System.Drawing.Point(3, 350);
            this.btnMoveToLowerDateAnot.Name = "btnMoveToLowerDateAnot";
            this.btnMoveToLowerDateAnot.Size = new System.Drawing.Size(364, 22);
            this.btnMoveToLowerDateAnot.TabIndex = 80;
            this.btnMoveToLowerDateAnot.Text = "zu den unteren Datumsbemerkungen verschieben";
            this.btnMoveToLowerDateAnot.UseVisualStyleBackColor = false;
            this.btnMoveToLowerDateAnot.Visible = false;
            this.btnMoveToLowerDateAnot.Click += new System.EventHandler(this.btnMoveToLowerDateAnot_Click);
            // 
            // btnDeleteEntry
            // 
            this.btnDeleteEntry.BackColor = System.Drawing.Color.Red;
            this.btnDeleteEntry.Location = new System.Drawing.Point(459, 655);
            this.btnDeleteEntry.Name = "btnDeleteEntry";
            this.btnDeleteEntry.Size = new System.Drawing.Size(186, 31);
            this.btnDeleteEntry.TabIndex = 81;
            this.btnDeleteEntry.Text = "Eintrag komplett löschen";
            this.btnDeleteEntry.UseVisualStyleBackColor = false;
            this.btnDeleteEntry.Visible = false;
            this.btnDeleteEntry.Click += new System.EventHandler(this.btnDeleteEntry_Click);
            // 
            // btnMoveToDateAnot
            // 
            this.btnMoveToDateAnot.BackColor = System.Drawing.SystemColors.Control;
            this.btnMoveToDateAnot.Location = new System.Drawing.Point(3, 327);
            this.btnMoveToDateAnot.Name = "btnMoveToDateAnot";
            this.btnMoveToDateAnot.Size = new System.Drawing.Size(364, 22);
            this.btnMoveToDateAnot.TabIndex = 82;
            this.btnMoveToDateAnot.Text = "zu den oberen Datumsbemerkungen verschieben";
            this.btnMoveToDateAnot.UseVisualStyleBackColor = false;
            this.btnMoveToDateAnot.Visible = false;
            this.btnMoveToDateAnot.Click += new System.EventHandler(this.btnMoveToDateAnot_Click);
            // 
            // lstUsageList
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 19;
            this.ListBox1.Location = new System.Drawing.Point(495, 547);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(59, 23);
            this.ListBox1.TabIndex = 83;
            this.ListBox1.Visible = false;
            // 
            // Textlesen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1018, 725);
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.btnDeleteEntry);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Command3);
            this.Controls.Add(this.List4);
            this.Controls.Add(this.List3);
            this.Controls.Add(this.List2);
            this.Controls.Add(this.Liste1);
            this.Controls.Add(this.Text1);
            this.Controls.Add(this._Bef_1);
            this.Controls.Add(this._Bef_2);
            this.Controls.Add(this._Bef_3);
            this.Controls.Add(this._Bef_4);
            this.Controls.Add(this.btnMoveNameToAlias);
            this.Controls.Add(this._Check2_0);
            this.Controls.Add(this.Text3);
            this.Controls.Add(this._Check2_1);
            this.Controls.Add(this._Bef_0);
            this.Controls.Add(this.RTB);
            this.Controls.Add(this.Frame2);
            this.Controls.Add(this._Label1_4);
            this.Controls.Add(this._Label1_3);
            this.Controls.Add(this._Label1_2);
            this.Controls.Add(this._Label1_1);
            this.Controls.Add(this._Label1_0);
            this.Controls.Add(this.Bezeichnung5);
            this.Controls.Add(this._Bezeichnung1_0);
            this.Controls.Add(this.Bezeichnung2);
            this.Controls.Add(this.Bezeichnung3);
            this.Controls.Add(this.Bezeichnung6);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Label18);
            this.Controls.Add(this.Label21);
            this.Controls.Add(this.Label20);
            this.Controls.Add(this.Label19);
            this.Controls.Add(this.Label17);
            this.Controls.Add(this.Label16);
            this.Controls.Add(this.Label15);
            this.Controls.Add(this.Label14);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Label24);
            this.Controls.Add(this.Label22);
            this.Controls.Add(this.Label23);
            this.Controls.Add(this.Label28);
            this.Controls.Add(this.Label27);
            this.Controls.Add(this.Label26);
            this.Controls.Add(this.Label25);
            this.Controls.Add(this.btnMoveToCause);
            this.Controls.Add(this.btnReenter);
            this.Controls.Add(this.Label29);
            this.Controls.Add(this.btnMoveToChurchCemet);
            this.Controls.Add(this.btnMoveToLowerDateAnot);
            this.Controls.Add(this.btnMoveToEntityAnot);
            this.Controls.Add(this.btnMoveToDateAnot);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Textlesen";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Gen_Plus Texte bearbeiten";
            this.Frame1.ResumeLayout(false);
            this.Frame2.ResumeLayout(false);
            this.Frame2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }


}