using BaseLib.Helper;
using Druck.My;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Compatibility.VB6;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Druck.Views
{
    [DesignerGenerated]
    public partial class AhneneST : Form
    {

        [AccessedThroughProperty("List4")]
        private ListBox _List4;

        [AccessedThroughProperty("List3")]
        private ListBox _List3;

        [AccessedThroughProperty("_Command2_3")]
        private Button __Command2_3;

        [AccessedThroughProperty("_Command2_0")]
        private Button __Command2_0;

        [AccessedThroughProperty("_Command2_1")]
        private Button __Command2_1;

        [AccessedThroughProperty("_Command2_2")]
        private Button __Command2_2;

        [AccessedThroughProperty("Frame2")]
        private GroupBox _Frame2;

        [AccessedThroughProperty("_RichTextBox1_1")]
        private RichTextBox __RichTextBox1_1;

        [AccessedThroughProperty("_Befehl_6")]
        private Button __Befehl_6;

        [AccessedThroughProperty("_Befehl_5")]
        private Button __Befehl_5;

        [AccessedThroughProperty("_Befehl_4")]
        private Button __Befehl_4;

        [AccessedThroughProperty("_Befehl_0")]
        private Button __Befehl_0;

        [AccessedThroughProperty("_RichTextBox1_0")]
        private RichTextBox __RichTextBox1_0;

        [AccessedThroughProperty("_Command_1")]
        private Button __Command_1;

        [AccessedThroughProperty("_Command_0")]
        private Button __Command_0;

        [AccessedThroughProperty("_Label1_5")]
        private Label __Label1_5;

        [AccessedThroughProperty("_Label1_4")]
        private Label __Label1_4;

        [AccessedThroughProperty("_Label1_3")]
        private Label __Label1_3;

        [AccessedThroughProperty("_Label1_2")]
        private Label __Label1_2;

        [AccessedThroughProperty("_Label1_1")]
        private Label __Label1_1;

        [AccessedThroughProperty("_Label1_0")]
        private Label __Label1_0;

        [AccessedThroughProperty("Frame1")]
        private GroupBox _Frame1;

        [AccessedThroughProperty("_Befehl_1")]
        private Button __Befehl_1;

        [AccessedThroughProperty("_Befehl_2")]
        private Button __Befehl_2;

        [AccessedThroughProperty("_Befehl_3")]
        private Button __Befehl_3;

        [AccessedThroughProperty("_RichTextBox1_2")]
        private RichTextBox __RichTextBox1_2;

        [AccessedThroughProperty("_RichTextBox1_3")]
        private RichTextBox __RichTextBox1_3;

        [AccessedThroughProperty("List1")]
        private ListBox _List1;

        [AccessedThroughProperty("List2")]
        private ListBox _List2;

        [AccessedThroughProperty("_Bezeichnung1_2")]
        private Label __Bezeichnung1_2;

        [AccessedThroughProperty("_Bezeichnung1_1")]
        private Label __Bezeichnung1_1;

        [AccessedThroughProperty("_Bezeichnung1_0")]
        private Label __Bezeichnung1_0;

        [AccessedThroughProperty("Befehl")]
        private ControlArray<Button> _Befehl;

        [AccessedThroughProperty("Bezeichnung1")]
        private ControlArray<Label> _Bezeichnung1;

        [AccessedThroughProperty("Command_Renamed")]
        private ControlArray<Button> _Command_Renamed;

        [AccessedThroughProperty("Command2")]
        private ControlArray<Button> _Command2;

        [AccessedThroughProperty("Label1")]
        private ControlArray<Label> _Label1;

        [AccessedThroughProperty("RichTextBox1")]
        private RichTextBoxArray _RichTextBox1;

        private string Ahnes;

        private string Ahne;

        private long IZahl;

        private string Job;

        private string Dat;

        private string Job1;

        [SpecialName]
        private int _Kinder_I;

        [SpecialName]
        private byte _0024STATIC_0024Kinder_00242001_0024K;

        [SpecialName]
        private int _0024STATIC_0024Kinder_00242001_0024F;

        [SpecialName]
        private byte _0024STATIC_0024Berufe_00242001_0024I1;

        [SpecialName]
        private int _0024STATIC_0024Kinder_00242001_0024Fa1;

        [SpecialName]
        private int _0024STATIC_0024Kinder_00242001_0024Fam;
        private string M_Namen;
        private byte M_KontLen;

        [DebuggerNonUserCode]
        public AhneneST()
        {
            Load += AhneneST_Load;
            FormClosing += AhneneST_FormClosing;
            InitializeComponent();
            this.Command2.SetIndex(this._Command2_0, 0);
            this.Command2.SetIndex(this._Command2_1, 1);
            this.Command2.SetIndex(this._Command2_2, 2);
            this.Command2.SetIndex(this._Command2_3, 3);
            Command2.AddClick(Command2_Click);

            this.Befehl.SetIndex(this._Befehl_0, 0);
            this.Befehl.SetIndex(this._Befehl_1, 1);
            this.Befehl.SetIndex(this._Befehl_2, 2);
            this.Befehl.SetIndex(this._Befehl_3, 3);
            this.Befehl.SetIndex(this._Befehl_4, 4);
            this.Befehl.SetIndex(this._Befehl_5, 5);
            this.Befehl.SetIndex(this._Befehl_6, 6);
            Befehl.AddClick(Befehl_Click);

            this.Command_Renamed.SetIndex(this._Command_0, 0);
            this.Command_Renamed.SetIndex(this._Command_1, 1);
            Command_Renamed.AddClick(Command_Renamed_Click);
        }


        //[DebuggerStepThrough]
        //private void InitializeComponent()
        //{
        //    this.components = new Container();
        //    this.ToolTip1 = new ToolTip(this.components);
        //    this.List5 = new ListBox();
        //    this.List4 = new ListBox();
        //    this.List3 = new ListBox();
        //    this.Frame2 = new GroupBox();
        //    this._Command2_3 = new Button();
        //    this._Command2_0 = new Button();
        //    this._Command2_1 = new Button();
        //    this._Command2_2 = new Button();
        //    this._RichTextBox1_1 = new RichTextBox();
        //    this._Befehl_6 = new Button();
        //    this._Befehl_5 = new Button();
        //    this._Befehl_4 = new Button();
        //    this._Befehl_0 = new Button();
        //    this._RichTextBox1_0 = new RichTextBox();
        //    this.Frame1 = new GroupBox();
        //    this._Command_1 = new Button();
        //    this._Command_0 = new Button();
        //    this._Label1_5 = new Label();
        //    this._Label1_4 = new Label();
        //    this._Label1_3 = new Label();
        //    this._Label1_2 = new Label();
        //    this._Label1_1 = new Label();
        //    this._Label1_0 = new Label();
        //    this._Befehl_1 = new Button();
        //    this._Befehl_2 = new Button();
        //    this._Befehl_3 = new Button();
        //    this._RichTextBox1_2 = new RichTextBox();
        //    this._RichTextBox1_3 = new RichTextBox();
        //    this.List1 = new ListBox();
        //    this.List2 = new ListBox();
        //    this._Bezeichnung1_2 = new Label();
        //    this._Bezeichnung1_1 = new Label();
        //    this._Bezeichnung1_0 = new Label();
        //    this.Befehl = new ControlArray<Button>();
        //    this.Bezeichnung1 = new ControlArray<Label>();
        //    this.Command_Renamed = new ControlArray<Button>();
        //    this.Command2 = new ControlArray<Button>();
        //    this.Label1 = new ControlArray<Label>();
        //    this.RichTextBox1 = new RichTextBoxArray(this.components);
        //    this.Frame2.SuspendLayout();
        //    this.Frame1.SuspendLayout();
        //    ((ISupportInitialize)this.Befehl).BeginInit();
        //    ((ISupportInitialize)this.Bezeichnung1).BeginInit();
        //    ((ISupportInitialize)this.Command_Renamed).BeginInit();
        //    ((ISupportInitialize)this.Command2).BeginInit();
        //    ((ISupportInitialize)this.Label1).BeginInit();
        //    ((ISupportInitialize)this.RichTextBox1).BeginInit();
        //    SuspendLayout();
        //    this.List5.BackColor = SystemColors.Window;
        //    this.List5.Cursor = Cursors.Default;
        //    this.List5.ForeColor = SystemColors.WindowText;
        //    this.List5.ItemHeight = 17;
        //    Point point2 = this.List5.Location = new Point(64, 264);
        //    Padding padding2 = this.List5.Margin = new Padding(4);
        //    this.List5.Name = "List5";
        //    this.List5.RightToLeft = RightToLeft.No;
        //    Size size2 = this.List5.Size = new Size(117, 140);
        //    this.List5.TabIndex = 32;
        //    this.List5.Visible = false;
        //    this.List4.BackColor = SystemColors.Window;
        //    this.List4.Cursor = Cursors.Default;
        //    this.List4.ForeColor = SystemColors.WindowText;
        //    this.List4.ItemHeight = 17;
        //    point2 = this.List4.Location = new Point(187, 84);
        //    padding2 = this.List4.Margin = new Padding(4);
        //    this.List4.Name = "List4";
        //    this.List4.RightToLeft = RightToLeft.No;
        //    size2 = this.List4.Size = new Size(56, 21);
        //    this.List4.TabIndex = 31;
        //    this.List4.Visible = false;
        //    this.List3.BackColor = SystemColors.Window;
        //    this.List3.Cursor = Cursors.Default;
        //    this.List3.ForeColor = SystemColors.WindowText;
        //    this.List3.ItemHeight = 17;
        //    point2 = this.List3.Location = new Point(83, 146);
        //    padding2 = this.List3.Margin = new Padding(4);
        //    this.List3.Name = "List3";
        //    this.List3.RightToLeft = RightToLeft.No;
        //    size2 = this.List3.Size = new Size(77, 21);
        //    this.List3.Sorted = true;
        //    this.List3.TabIndex = 30;
        //    this.List3.Visible = false;
        //    this.Frame2.BackColor = Color.FromArgb(255, 255, 192);
        //    this.Frame2.Controls.Add(this._Command2_3);
        //    this.Frame2.Controls.Add(this._Command2_0);
        //    this.Frame2.Controls.Add(this._Command2_1);
        //    this.Frame2.Controls.Add(this._Command2_2);
        //    this.Frame2.ForeColor = SystemColors.ControlText;
        //    point2 = this.Frame2.Location = new Point(333, 178);
        //    padding2 = this.Frame2.Margin = new Padding(4);
        //    this.Frame2.Name = "Frame2";
        //    padding2 = this.Frame2.Padding = new Padding(4);
        //    this.Frame2.RightToLeft = RightToLeft.No;
        //    size2 = this.Frame2.Size = new Size(231, 197);
        //    this.Frame2.TabIndex = 24;
        //    this.Frame2.TabStop = false;
        //    this.Frame2.Visible = false;
        //    this._Command2_3.BackColor = SystemColors.Control;
        //    this._Command2_3.Cursor = Cursors.Default;
        //    this._Command2_3.ForeColor = SystemColors.ControlText;
        //    point2 = this._Command2_3.Location = new Point(32, 136);
        //    padding2 = this._Command2_3.Margin = new Padding(4);
        //    this._Command2_3.Name = "_Command2_3";
        //    this._Command2_3.RightToLeft = RightToLeft.No;
        //    size2 = this._Command2_3.Size = new Size(169, 48);
        //    this._Command2_3.TabIndex = 28;
        //    this._Command2_3.Text = "Indizierungsdatei Orte für WinWord";
        //    this._Command2_3.UseVisualStyleBackColor = false;
        //    this._Command2_3.Visible = false;
        //    this._Command2_0.BackColor = SystemColors.Control;
        //    this._Command2_0.Cursor = Cursors.Default;
        //    this._Command2_0.ForeColor = SystemColors.ControlText;
        //    point2 = this._Command2_0.Location = new Point(32, 34);
        //    padding2 = this._Command2_0.Margin = new Padding(4);
        //    this._Command2_0.Name = "_Command2_0";
        //    this._Command2_0.RightToLeft = RightToLeft.No;
        //    size2 = this._Command2_0.Size = new Size(169, 25);
        //    this._Command2_0.TabIndex = 27;
        //    this._Command2_0.Text = "Ortsindex";
        //    this._Command2_0.UseVisualStyleBackColor = false;
        //    this._Command2_1.BackColor = SystemColors.Control;
        //    this._Command2_1.Cursor = Cursors.Default;
        //    this._Command2_1.ForeColor = SystemColors.ControlText;
        //    point2 = this._Command2_1.Location = new Point(32, 102);
        //    padding2 = this._Command2_1.Margin = new Padding(4);
        //    this._Command2_1.Name = "_Command2_1";
        //    this._Command2_1.RightToLeft = RightToLeft.No;
        //    size2 = this._Command2_1.Size = new Size(169, 25);
        //    this._Command2_1.TabIndex = 26;
        //    this._Command2_1.Text = "Index Orte-Namen";
        //    this._Command2_1.UseVisualStyleBackColor = false;
        //    this._Command2_2.BackColor = SystemColors.Control;
        //    this._Command2_2.Cursor = Cursors.Default;
        //    this._Command2_2.ForeColor = SystemColors.ControlText;
        //    point2 = this._Command2_2.Location = new Point(32, 68);
        //    padding2 = this._Command2_2.Margin = new Padding(4);
        //    this._Command2_2.Name = "_Command2_2";
        //    this._Command2_2.RightToLeft = RightToLeft.No;
        //    size2 = this._Command2_2.Size = new Size(169, 25);
        //    this._Command2_2.TabIndex = 25;
        //    this._Command2_2.Text = "Index Namen-Orte";
        //    this._Command2_2.UseVisualStyleBackColor = false;
        //    this.RichTextBox1.SetIndex(this._RichTextBox1_1, 1);
        //    point2 = this._RichTextBox1_1.Location = new Point(9, 49);
        //    padding2 = this._RichTextBox1_1.Margin = new Padding(4);
        //    this._RichTextBox1_1.Name = "_RichTextBox1_1";
        //    this._RichTextBox1_1.RightMargin = 627;
        //    size2 = this._RichTextBox1_1.Size = new Size(981, 599);
        //    this._RichTextBox1_1.TabIndex = 21;
        //    this._RichTextBox1_1.Text = "";
        //    this._RichTextBox1_1.Visible = false;
        //    this._Befehl_6.BackColor = SystemColors.Control;
        //    this._Befehl_6.Cursor = Cursors.Default;
        //    this._Befehl_6.DialogResult = DialogResult.Cancel;
        //    this._Befehl_6.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_6.Location = new Point(733, 690);
        //    padding2 = this._Befehl_6.Margin = new Padding(4);
        //    this._Befehl_6.Name = "_Befehl_6";
        //    this._Befehl_6.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_6.Size = new Size(91, 21);
        //    this._Befehl_6.TabIndex = 20;
        //    this._Befehl_6.Text = "Ortsindex";
        //    this._Befehl_6.UseVisualStyleBackColor = false;
        //    this._Befehl_5.BackColor = SystemColors.Control;
        //    this._Befehl_5.Cursor = Cursors.Default;
        //    this._Befehl_5.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_5.Location = new Point(539, 690);
        //    padding2 = this._Befehl_5.Margin = new Padding(4);
        //    this._Befehl_5.Name = "_Befehl_5";
        //    this._Befehl_5.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_5.Size = new Size(157, 21);
        //    this._Befehl_5.TabIndex = 19;
        //    this._Befehl_5.Text = "Namenkurzindex";
        //    this._Befehl_5.UseVisualStyleBackColor = false;
        //    this._Befehl_4.BackColor = SystemColors.Control;
        //    this._Befehl_4.Cursor = Cursors.Default;
        //    this._Befehl_4.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_4.Location = new Point(243, 690);
        //    padding2 = this._Befehl_4.Margin = new Padding(4);
        //    this._Befehl_4.Name = "_Befehl_4";
        //    this._Befehl_4.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_4.Size = new Size(93, 21);
        //    this._Befehl_4.TabIndex = 18;
        //    this._Befehl_4.Text = "Ahnenliste";
        //    this._Befehl_4.UseVisualStyleBackColor = false;
        //    this._Befehl_0.BackColor = SystemColors.Control;
        //    this._Befehl_0.Cursor = Cursors.Default;
        //    this._Befehl_0.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_0.Location = new Point(363, 690);
        //    padding2 = this._Befehl_0.Margin = new Padding(4);
        //    this._Befehl_0.Name = "_Befehl_0";
        //    this._Befehl_0.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_0.Size = new Size(163, 21);
        //    this._Befehl_0.TabIndex = 17;
        //    this._Befehl_0.Text = "Namenlangindex";
        //    this._Befehl_0.UseVisualStyleBackColor = false;
        //    this.RichTextBox1.SetIndex(this._RichTextBox1_0, 0);
        //    point2 = this._RichTextBox1_0.Location = new Point(9, 49);
        //    padding2 = this._RichTextBox1_0.Margin = new Padding(4);
        //    this._RichTextBox1_0.Name = "_RichTextBox1_0";
        //    this._RichTextBox1_0.RightMargin = 634;
        //    size2 = this._RichTextBox1_0.Size = new Size(977, 599);
        //    this._RichTextBox1_0.TabIndex = 13;
        //    this._RichTextBox1_0.Text = "";
        //    this._RichTextBox1_0.Visible = false;
        //    this.Frame1.BackColor = Color.Red;
        //    this.Frame1.Controls.Add(this._Command_1);
        //    this.Frame1.Controls.Add(this._Command_0);
        //    this.Frame1.Controls.Add(this._Label1_5);
        //    this.Frame1.Controls.Add(this._Label1_4);
        //    this.Frame1.Controls.Add(this._Label1_3);
        //    this.Frame1.Controls.Add(this._Label1_2);
        //    this.Frame1.Controls.Add(this._Label1_1);
        //    this.Frame1.Controls.Add(this._Label1_0);
        //    this.Frame1.ForeColor = SystemColors.ControlText;
        //    point2 = this.Frame1.Location = new Point(197, 160);
        //    padding2 = this.Frame1.Margin = new Padding(4);
        //    this.Frame1.Name = "Frame1";
        //    padding2 = this.Frame1.Padding = new Padding(4);
        //    this.Frame1.RightToLeft = RightToLeft.No;
        //    size2 = this.Frame1.Size = new Size(543, 230);
        //    this.Frame1.TabIndex = 4;
        //    this.Frame1.TabStop = false;
        //    this._Command_1.BackColor = SystemColors.Control;
        //    this._Command_1.Cursor = Cursors.Default;
        //    this._Command_1.ForeColor = SystemColors.ControlText;
        //    point2 = this._Command_1.Location = new Point(327, 192);
        //    padding2 = this._Command_1.Margin = new Padding(4);
        //    this._Command_1.Name = "_Command_1";
        //    this._Command_1.RightToLeft = RightToLeft.No;
        //    size2 = this._Command_1.Size = new Size(141, 26);
        //    this._Command_1.TabIndex = 12;
        //    this._Command_1.Text = "OK";
        //    this._Command_1.UseVisualStyleBackColor = false;
        //    this._Command_0.BackColor = SystemColors.Control;
        //    this._Command_0.Cursor = Cursors.Default;
        //    this._Command_0.ForeColor = SystemColors.ControlText;
        //    point2 = this._Command_0.Location = new Point(47, 192);
        //    padding2 = this._Command_0.Margin = new Padding(4);
        //    this._Command_0.Name = "_Command_0";
        //    this._Command_0.RightToLeft = RightToLeft.No;
        //    size2 = this._Command_0.Size = new Size(141, 26);
        //    this._Command_0.TabIndex = 11;
        //    this._Command_0.Text = "Abbrechen";
        //    this._Command_0.UseVisualStyleBackColor = false;
        //    this._Label1_5.BackColor = Color.White;
        //    this._Label1_5.Cursor = Cursors.Default;
        //    this._Label1_5.ForeColor = SystemColors.ControlText;
        //    this.Label1.SetIndex(this._Label1_5, 5);
        //    point2 = this._Label1_5.Location = new Point(9, 73);
        //    padding2 = this._Label1_5.Margin = new Padding(4, 0, 4, 0);
        //    this._Label1_5.Name = "_Label1_5";
        //    this._Label1_5.RightToLeft = RightToLeft.No;
        //    size2 = this._Label1_5.Size = new Size(524, 20);
        //    this._Label1_5.TabIndex = 10;
        //    this._Label1_4.BackColor = Color.White;
        //    this._Label1_4.Cursor = Cursors.Default;
        //    this._Label1_4.ForeColor = SystemColors.ControlText;
        //    this.Label1.SetIndex(this._Label1_4, 4);
        //    point2 = this._Label1_4.Location = new Point(9, 156);
        //    padding2 = this._Label1_4.Margin = new Padding(4, 0, 4, 0);
        //    this._Label1_4.Name = "_Label1_4";
        //    this._Label1_4.RightToLeft = RightToLeft.No;
        //    size2 = this._Label1_4.Size = new Size(524, 20);
        //    this._Label1_4.TabIndex = 9;
        //    this._Label1_3.BackColor = Color.White;
        //    this._Label1_3.Cursor = Cursors.Default;
        //    this._Label1_3.ForeColor = SystemColors.ControlText;
        //    this.Label1.SetIndex(this._Label1_3, 3);
        //    point2 = this._Label1_3.Location = new Point(9, 18);
        //    padding2 = this._Label1_3.Margin = new Padding(4, 0, 4, 0);
        //    this._Label1_3.Name = "_Label1_3";
        //    this._Label1_3.RightToLeft = RightToLeft.No;
        //    size2 = this._Label1_3.Size = new Size(524, 20);
        //    this._Label1_3.TabIndex = 8;
        //    this._Label1_3.TextAlign = ContentAlignment.TopCenter;
        //    this._Label1_2.BackColor = Color.White;
        //    this._Label1_2.Cursor = Cursors.Default;
        //    this._Label1_2.ForeColor = SystemColors.ControlText;
        //    this.Label1.SetIndex(this._Label1_2, 2);
        //    point2 = this._Label1_2.Location = new Point(9, 128);
        //    padding2 = this._Label1_2.Margin = new Padding(4, 0, 4, 0);
        //    this._Label1_2.Name = "_Label1_2";
        //    this._Label1_2.RightToLeft = RightToLeft.No;
        //    size2 = this._Label1_2.Size = new Size(524, 20);
        //    this._Label1_2.TabIndex = 7;
        //    this._Label1_1.BackColor = Color.White;
        //    this._Label1_1.Cursor = Cursors.Default;
        //    this._Label1_1.ForeColor = SystemColors.ControlText;
        //    this.Label1.SetIndex(this._Label1_1, 1);
        //    point2 = this._Label1_1.Location = new Point(9, 101);
        //    padding2 = this._Label1_1.Margin = new Padding(4, 0, 4, 0);
        //    this._Label1_1.Name = "_Label1_1";
        //    this._Label1_1.RightToLeft = RightToLeft.No;
        //    size2 = this._Label1_1.Size = new Size(524, 20);
        //    this._Label1_1.TabIndex = 6;
        //    this._Label1_0.BackColor = Color.White;
        //    this._Label1_0.Cursor = Cursors.Default;
        //    this._Label1_0.ForeColor = SystemColors.ControlText;
        //    this.Label1.SetIndex(this._Label1_0, 0);
        //    point2 = this._Label1_0.Location = new Point(9, 46);
        //    padding2 = this._Label1_0.Margin = new Padding(4, 0, 4, 0);
        //    this._Label1_0.Name = "_Label1_0";
        //    this._Label1_0.RightToLeft = RightToLeft.No;
        //    size2 = this._Label1_0.Size = new Size(524, 20);
        //    this._Label1_0.TabIndex = 5;
        //    this._Befehl_1.BackColor = SystemColors.Control;
        //    this._Befehl_1.Cursor = Cursors.Default;
        //    this._Befehl_1.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_1.Location = new Point(11, 690);
        //    padding2 = this._Befehl_1.Margin = new Padding(4);
        //    this._Befehl_1.Name = "_Befehl_1";
        //    this._Befehl_1.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_1.Size = new Size(96, 21);
        //    this._Befehl_1.TabIndex = 0;
        //    this._Befehl_1.Text = "&Drucken";
        //    this._Befehl_1.UseVisualStyleBackColor = false;
        //    this._Befehl_2.BackColor = SystemColors.Control;
        //    this._Befehl_2.Cursor = Cursors.Default;
        //    this._Befehl_2.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_2.Location = new Point(883, 690);
        //    padding2 = this._Befehl_2.Margin = new Padding(4);
        //    this._Befehl_2.Name = "_Befehl_2";
        //    this._Befehl_2.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_2.Size = new Size(125, 21);
        //    this._Befehl_2.TabIndex = 2;
        //    this._Befehl_2.Text = "Druck&menue";
        //    this._Befehl_2.UseVisualStyleBackColor = false;
        //    this._Befehl_3.BackColor = SystemColors.Control;
        //    this._Befehl_3.Cursor = Cursors.Default;
        //    this._Befehl_3.ForeColor = SystemColors.ControlText;
        //    point2 = this._Befehl_3.Location = new Point(120, 690);
        //    padding2 = this._Befehl_3.Margin = new Padding(4);
        //    this._Befehl_3.Name = "_Befehl_3";
        //    this._Befehl_3.RightToLeft = RightToLeft.No;
        //    size2 = this._Befehl_3.Size = new Size(107, 21);
        //    this._Befehl_3.TabIndex = 1;
        //    this._Befehl_3.Text = "&In Datei";
        //    this._Befehl_3.UseVisualStyleBackColor = false;
        //    this.RichTextBox1.SetIndex(this._RichTextBox1_2, 2);
        //    point2 = this._RichTextBox1_2.Location = new Point(9, 47);
        //    padding2 = this._RichTextBox1_2.Margin = new Padding(4);
        //    this._RichTextBox1_2.Name = "_RichTextBox1_2";
        //    size2 = this._RichTextBox1_2.Size = new Size(977, 598);
        //    this._RichTextBox1_2.TabIndex = 22;
        //    this._RichTextBox1_2.Text = "";
        //    this._RichTextBox1_2.Visible = false;
        //    this.RichTextBox1.SetIndex(this._RichTextBox1_3, 3);
        //    point2 = this._RichTextBox1_3.Location = new Point(9, 47);
        //    padding2 = this._RichTextBox1_3.Margin = new Padding(4);
        //    this._RichTextBox1_3.Name = "_RichTextBox1_3";
        //    this._RichTextBox1_3.RightMargin = 627;
        //    size2 = this._RichTextBox1_3.Size = new Size(977, 598);
        //    this._RichTextBox1_3.TabIndex = 23;
        //    this._RichTextBox1_3.Text = "";
        //    this._RichTextBox1_3.Visible = false;
        //    this.List1.BackColor = SystemColors.Window;
        //    this.List1.Cursor = Cursors.Default;
        //    this.List1.ForeColor = SystemColors.WindowText;
        //    this.List1.ItemHeight = 17;
        //    point2 = this.List1.Location = new Point(84, 9);
        //    padding2 = this.List1.Margin = new Padding(4);
        //    this.List1.Name = "List1";
        //    this.List1.RightToLeft = RightToLeft.No;
        //    size2 = this.List1.Size = new Size(267, 21);
        //    this.List1.Sorted = true;
        //    this.List1.TabIndex = 14;
        //    this.List1.Visible = false;
        //    this.List2.BackColor = SystemColors.Window;
        //    this.List2.Cursor = Cursors.Default;
        //    this.List2.ForeColor = SystemColors.WindowText;
        //    this.List2.ItemHeight = 17;
        //    point2 = this.List2.Location = new Point(21, 136);
        //    padding2 = this.List2.Margin = new Padding(4);
        //    this.List2.Name = "List2";
        //    this.List2.RightToLeft = RightToLeft.No;
        //    size2 = this.List2.Size = new Size(53, 21);
        //    this.List2.Sorted = true;
        //    this.List2.TabIndex = 29;
        //    this.List2.Visible = false;
        //    this._Bezeichnung1_2.BackColor = Color.Cyan;
        //    this._Bezeichnung1_2.Cursor = Cursors.Default;
        //    this._Bezeichnung1_2.ForeColor = Color.Black;
        //    this.Bezeichnung1.SetIndex(this._Bezeichnung1_2, 2);
        //    point2 = this._Bezeichnung1_2.Location = new Point(327, 24);
        //    padding2 = this._Bezeichnung1_2.Margin = new Padding(4, 0, 4, 0);
        //    this._Bezeichnung1_2.Name = "_Bezeichnung1_2";
        //    this._Bezeichnung1_2.RightToLeft = RightToLeft.No;
        //    size2 = this._Bezeichnung1_2.Size = new Size(740, 21);
        //    this._Bezeichnung1_2.TabIndex = 16;
        //    this._Bezeichnung1_2.TextAlign = ContentAlignment.TopCenter;
        //    this._Bezeichnung1_1.BackColor = Color.Cyan;
        //    this._Bezeichnung1_1.Cursor = Cursors.Default;
        //    this._Bezeichnung1_1.ForeColor = Color.Black;
        //    this.Bezeichnung1.SetIndex(this._Bezeichnung1_1, 1);
        //    point2 = this._Bezeichnung1_1.Location = new Point(0, 24);
        //    padding2 = this._Bezeichnung1_1.Margin = new Padding(4, 0, 4, 0);
        //    this._Bezeichnung1_1.Name = "_Bezeichnung1_1";
        //    this._Bezeichnung1_1.RightToLeft = RightToLeft.No;
        //    size2 = this._Bezeichnung1_1.Size = new Size(316, 21);
        //    this._Bezeichnung1_1.TabIndex = 15;
        //    this._Bezeichnung1_1.TextAlign = ContentAlignment.TopCenter;
        //    this._Bezeichnung1_0.BackColor = Color.Cyan;
        //    this._Bezeichnung1_0.Cursor = Cursors.Default;
        //    this._Bezeichnung1_0.ForeColor = Color.Black;
        //    this.Bezeichnung1.SetIndex(this._Bezeichnung1_0, 0);
        //    point2 = this._Bezeichnung1_0.Location = new Point(0, 0);
        //    padding2 = this._Bezeichnung1_0.Margin = new Padding(4, 0, 4, 0);
        //    this._Bezeichnung1_0.Name = "_Bezeichnung1_0";
        //    this._Bezeichnung1_0.RightToLeft = RightToLeft.No;
        //    size2 = this._Bezeichnung1_0.Size = new Size(1067, 21);
        //    this._Bezeichnung1_0.TabIndex = 3;
        //    this._Bezeichnung1_0.TextAlign = ContentAlignment.TopCenter;
        //    SizeF sizeF2 = AutoScaleDimensions = new SizeF(8f, 17f);
        //    AutoScaleMode = AutoScaleMode.Font;
        //    BackColor = Color.FromArgb(192, 192, 192);
        //    CancelButton = this._Befehl_6;
        //    size2 = ClientSize = new Size(1018, 725);
        //    Controls.Add(this.Frame1);
        //    Controls.Add(this.List5);
        //    Controls.Add(this.List4);
        //    Controls.Add(this.List3);
        //    Controls.Add(this.Frame2);
        //    Controls.Add(this._RichTextBox1_1);
        //    Controls.Add(this._Befehl_6);
        //    Controls.Add(this._Befehl_5);
        //    Controls.Add(this._Befehl_4);
        //    Controls.Add(this._Befehl_0);
        //    Controls.Add(this._RichTextBox1_0);
        //    Controls.Add(this._Befehl_1);
        //    Controls.Add(this._Befehl_2);
        //    Controls.Add(this._Befehl_3);
        //    Controls.Add(this._RichTextBox1_2);
        //    Controls.Add(this._RichTextBox1_3);
        //    Controls.Add(this.List1);
        //    Controls.Add(this.List2);
        //    Controls.Add(this._Bezeichnung1_2);
        //    Controls.Add(this._Bezeichnung1_1);
        //    Controls.Add(this._Bezeichnung1_0);
        //    Cursor = Cursors.Default;
        //    Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        //    ForeColor = Color.Black;
        //    FormBorderStyle = FormBorderStyle.FixedSingle;
        //    padding2 = Margin = new Padding(4);
        //    MaximizeBox = false;
        //    Name = "AhneneST";
        //    RightToLeft = RightToLeft.No;
        //    StartPosition = FormStartPosition.Manual;
        //    Text = "Ahnenstammliste";
        //    this.Frame2.ResumeLayout(false);
        //    this.Frame1.ResumeLayout(false);
        //    ((ISupportInitialize)this.Befehl).EndInit();
        //    ((ISupportInitialize)this.Bezeichnung1).EndInit();
        //    ((ISupportInitialize)this.Command_Renamed).EndInit();
        //    ((ISupportInitialize)this.Command2).EndInit();
        //    ((ISupportInitialize)this.Label1).EndInit();
        //    ((ISupportInitialize)this.RichTextBox1).EndInit();
        //    ResumeLayout(false);
        //}

        private void Befehl_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            int number = default;
            string right = default;
            string text = default;
            string text2 = default;
            string text3 = default;
            object CounterResult = default;
            object LoopForResult = default;
            object CounterResult2 = default;
            object LoopForResult2 = default;
            object LoopForResult3 = default;
            object LoopForResult4 = default;
            object LoopForResult5 = default;
            object LoopForResult6 = default;
            object LoopForResult7 = default;
            object LoopForResult8 = default;
            object LoopForResult9 = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                    string text4;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            index = Befehl.GetIndex((Button)eventSender);
                            goto IL_0015;
                        case 4739:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0f89;
                                    default:
                                        goto end_IL_0000;
                                }
                                number = Information.Err().Number;
                                if (number == 55)
                                {
                                    FileSystem.FileClose();
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0f85;
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
                                goto IL_0f85;
                            }
                        end_IL_0000:
                            break;
                        IL_0015:
                            num = 2;
                            text2 = "";
                            right = "";
                            text = "";
                            text3 = "";
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            text4 = "Datum " + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Today.Year.ToString();
                            switch (index)
                            {
                                case 0:
                                    break;
                                case 1:
                                    goto IL_0689;
                                case 2:
                                    goto IL_075f;
                                case 3:
                                    goto IL_0781;
                                case 4:
                                    goto IL_08df;
                                case 5:
                                    goto IL_09ad;
                                case 6:
                                    goto IL_0e48;
                                default:
                                    goto IL_0ecf;
                            }
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 6, 1, ref LoopForResult8, ref CounterResult2))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult8, ref CounterResult2))
                                {
                                    Befehl[(short)CounterResult2.AsInt()].Visible = true;
                                }
                            }
                            Befehl[0].Visible = false;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 3, 1, ref LoopForResult9, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult9, ref CounterResult))
                                {
                                    RichTextBox1[(short)CounterResult.AsInt()].Visible = false;
                                }
                            }
                            RichTextBox1[2].Visible = true;
                            if (RichTextBox1[2].Text != "")
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.DSB_NamIdxTable.Index = "Langi";
                            RichTextBox1[2].SelectionIndent = 50;
                            RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[2].SelectedText = "Namen-Index (Langform)\n\n";
                            RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DSB_NamIdxTable.Seek(">=", " ", " ", 0);
                            M_Namen = "";
                            goto IL_05f1;
                        IL_047d:
                            num = 47;
                            if (text3 == "")
                            {
                                text3 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                            }
                            goto IL_04b5;
                        IL_04b5:
                            num = 50;
                            if (text3 != DataModul.DSB_NamIdxTable.Fields["Name"].AsString())
                            {
                                RichTextBox1[2].SelectionIndent = 70;
                                RichTextBox1[2].SelectedText = text3 + ": " + Strings.Mid(text, 2, text.Length) + ".\n";
                                text = "";
                                right = 0.AsString();
                                text3 = DataModul.DSB_NamIdxTable.Fields["Name"].AsString();
                            }
                            goto IL_0565;
                        IL_0565:
                            num = 57;
                            if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                            {
                                text = text + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                                right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                            }
                            goto IL_05e3;
                        IL_05e3:
                            num = 61;
                            DataModul.DSB_NamIdxTable.MoveNext();
                            goto IL_05f1;
                        IL_05f1: // <========== 3
                            num = 29;
                            if (!DataModul.DSB_NamIdxTable.EOF)
                            {
                                RichTextBox1[2].SelectionIndent = 50;
                                if (M_Namen != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                                {
                                    if (text3 != "")
                                    {
                                        RichTextBox1[2].SelectionIndent = 70;
                                        RichTextBox1[2].SelectedText = text3 + "; " + Strings.Mid(text, 2, text.Length) + ".\n";
                                        text = "";
                                        right = "";
                                        text3 = "";
                                    }
                                    RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                    RichTextBox1[2].SelectionIndent = 50;
                                    RichTextBox1[2].SelectedText = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString() + '\n';
                                    RichTextBox1[2].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    text = "";
                                    right = "";
                                    M_Namen = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                                }
                                goto IL_047d;
                            }
                            if (text3 == "")
                            {
                            
                            }
                            else
                            {
                                RichTextBox1[2].SelectionIndent = 70;
                            RichTextBox1[2].SelectedText = text3 + "; " + Strings.Mid(text, 2, text.Length) + ".\n";
                            text = "";
                            right = "";
                            text3 = "";
                            }
                            goto end_IL_0000_2;
                        IL_0689:
                            num = 73;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 2, 1, ref LoopForResult7, ref CounterResult))
                            {
                                goto IL_06ac;
                            }
                            goto IL_06dc;
                        IL_06ac: // <========== 3
                            num = 74;
                            if (!RichTextBox1[(short)CounterResult.AsInt()].Visible)
                            {
                                if (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult7, ref CounterResult))
                                {
                                    goto IL_06ac;
                                }
                            }
                            goto IL_06dc;
                        IL_06dc: // <========== 3
                            num = 78;
                            RichTextBox1[(short)CounterResult.AsInt()].SaveFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            RichTextBox1[(short)CounterResult.AsInt()].LoadFile(_Modul1.Instance.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                            Interaction.Shell(_Modul1.Instance.Aus[7] + " " + _Modul1.Instance.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
                            goto end_IL_0000_2;
                        IL_075f:
                            num = 83;
                            Close();
                            MyProject.Forms.Druck.Show();
                            goto end_IL_0000_2;
                        IL_0781:
                            num = 87;
                            MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 2, 1, ref LoopForResult6, ref CounterResult))
                            {
                                goto IL_07c1;
                            }
                            goto IL_07f1;
                        IL_07c1: // <========== 3
                            num = 89;
                            if (!RichTextBox1[(short)CounterResult.AsInt()].Visible)
                            {
                                if (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult6, ref CounterResult))
                                {
                                    goto IL_07c1;
                                }
                            }
                            goto IL_07f1;
                        IL_07f1: // <========== 3
                            num = 93;
                            MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = _Modul1.Instance.GenFreeDir + "list\\";
                            MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                            MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                            switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                            {
                                case 1:
                            RichTextBox1[(short)CounterResult.AsInt()].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                                    break;
                                case 2:
                                    goto IL_08a5;
                                default:
                                    break;
                            }
                            goto end_IL_0000_2;
                        IL_08a5:
                            num = 103;
                            RichTextBox1[(short)CounterResult.AsInt()].SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                            goto end_IL_0000_2;
                        IL_08df:
                            num = 108;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 6, 1, ref LoopForResult4, ref CounterResult2))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult4, ref CounterResult2))
                                {
                                    Befehl[(short)CounterResult2.AsInt()].Visible = true;
                                }
                            }
                            Befehl[4].Visible = false;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 3, 1, ref LoopForResult5, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult5, ref CounterResult))
                                {
                                    RichTextBox1[(short)CounterResult.AsInt()].Visible = false;
                                }
                            }
                            goto IL_0992;
                        IL_0992:
                            num = 115;
                            RichTextBox1[0].Visible = true;
                            goto end_IL_0000_2;
                        IL_09ad:
                            num = 118;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult2, 0, 6, 1, ref LoopForResult2, ref CounterResult2))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult2, LoopForResult2, ref CounterResult2))
                                {
                                    Befehl[(short)CounterResult2.AsInt()].Visible = true;
                                }
                            }
                            Befehl[5].Visible = false;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 3, 1, ref LoopForResult3, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult3, ref CounterResult))
                                {
                                    RichTextBox1[(short)CounterResult.AsInt()].Visible = false;
                                }
                            }
                            RichTextBox1[1].Visible = true;
                            if (RichTextBox1[1].Text != "")
                            {
                                goto end_IL_0000_2;
                            }
                            DataModul.DSB_NamIdxTable.Index = "Kurzname";
                            RichTextBox1[1].SelectionIndent = 50;
                            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[1].SelectedText = "Namen-Index (Kurzform)\n\n";
                            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            DataModul.DSB_NamIdxTable.Seek(">=", " ", 0);
                            goto IL_0d6e;
                        IL_0caf:
                            num = 144;
                            text2 = DataModul.DSB_NamIdxTable.Fields["Name1"].AsString();
                            goto IL_0cd6;
                        IL_0cd6: // <========== 3
                            num = 146;
                            if (DataModul.DSB_NamIdxTable.Fields["Nr"].AsInt() != right.AsInt())
                            {
                                text = text + ", " + DataModul.DSB_NamIdxTable.Fields["Nr"].AsString().Trim();
                                right = DataModul.DSB_NamIdxTable.Fields["Nr"].AsString();
                            }
                            goto IL_0d5d;
                        IL_0d5d:
                            num = 150;
                            DataModul.DSB_NamIdxTable.MoveNext();
                            goto IL_0d6e;
                        IL_0d6e: // <========== 3
                            num = 134;
                            if (!DataModul.DSB_NamIdxTable.EOF)
                            {
                                if (text2 != DataModul.DSB_NamIdxTable.Fields["Name1"].AsString())
                                {
                                    if (text2 != "")
                                    {
                                        RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                        RichTextBox1[1].SelectedText = text2;
                                        RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        RichTextBox1[1].SelectedText = text + "\n\n";
                                        text = "";
                                        right = "";
                                    }
                                    goto IL_0caf;
                                }
                                goto IL_0cd6;
                            }
                            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            RichTextBox1[1].SelectedText = text2;
                            RichTextBox1[1].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            RichTextBox1[1].SelectedText = text + "\n\n";
                            text = "";
                            right = "";
                            goto end_IL_0000_2;
                        IL_0e48:
                            num = 161;
                            if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 0, 3, 1, ref LoopForResult, ref CounterResult))
                            {
                                while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult, ref CounterResult))
                                {
                                    RichTextBox1[(short)CounterResult.AsInt()].Visible = false;
                                }
                            }
                            RichTextBox1[3].Visible = true;
                            Frame2.Visible = true;
                            goto end_IL_0000_2;
                        IL_0ecf:
                            num = 168;
                            Interaction.MsgBox(index);
                            goto end_IL_0000_2;
                        IL_0f85:
                            num4 = num2;
                            goto IL_0f8d;
                        IL_0f89:
                            num4 = num2 + 1;
                            goto IL_0f8d;
                        IL_0f8d:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 46:
                                case 47:
                                    goto IL_047d;
                                case 49:
                                case 50:
                                    goto IL_04b5;
                                case 56:
                                case 57:
                                    goto IL_0565;
                                case 60:
                                case 61:
                                    goto IL_05e3;
                                case 28:
                                case 29:
                                case 62:
                                    goto IL_05f1;
                                case 74:
                                    goto IL_06ac;
                                case 75:
                                case 78:
                                    goto IL_06dc;
                                case 89:
                                    goto IL_07c1;
                                case 90:
                                case 93:
                                    goto IL_07f1;
                                case 115:
                                    goto IL_0992;
                                case 143:
                                case 144:
                                    goto IL_0caf;
                                case 145:
                                case 146:
                                    goto IL_0cd6;
                                case 149:
                                case 150:
                                    goto IL_0d5d;
                                case 133:
                                case 134:
                                case 151:
                                    goto IL_0d6e;
                                case 9:
                                case 69:
                                case 70:
                                case 71:
                                case 81:
                                case 85:
                                case 97:
                                case 101:
                                case 104:
                                case 105:
                                case 106:
                                case 116:
                                case 158:
                                case 159:
                                case 166:
                                case 169:
                                case 170:
                                case 172:
                                case 177:
                                case 178:
                                case 184:
                                case 185:
                                case 186:
                                    goto end_IL_0000_2;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj);
                    try0000_dispatch = 4739;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 14
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Command_Renamed_Click(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int index = default;
            int num2 = default;
            int num3 = default;
            short num5 = default;
            byte b = default;
            string left = default;
            string text = default;
            short num6 = default;
            long num7 = default;
            int lErl = default;
            string text2 = default;
            byte b2 = default;
            string text4 = default;
            int persInArb = default;
            short num9 = default;
            string text5 = default;
            byte b3 = default;
            byte b4 = default;
            int num10 = default;
            short num11 = default;
            object right = default;
            byte b6 = default;
            byte b7 = default;
            object CounterResult = default;
            object LoopForResult2 = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    checked
                    {
                        int num4;
                        string text3;
                        object objectValue;
                        string text6;
                        switch (try0000_dispatch)
                        {
                            default:
                                num = 1;
                                index = Command_Renamed.GetIndex((Button)eventSender);
                                goto IL_0016;
                            case 8061:
                                {
                                    num2 = num;
                                    switch (num3)
                                    {
                                        case 2:
                                            break;
                                        case 1:
                                            goto IL_1ac3;
                                        default:
                                            goto end_IL_0000;
                                    }
                                    if (Information.Err().Number == 3021)
                                    {
                                        Frame1.Visible = false;
                                        Kinder();
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
                                    goto IL_1ac7;
                                }
                            end_IL_0000:
                                break;
                            IL_0016:
                                num = 2;
                                b = 7;
                                left = "";
                                text = "";
                                ProjectData.ClearProjectError();
                                num3 = 2;
                                _Modul1.Instance.DAus[101] = _Modul1.Instance.Font1;
                                _Modul1.Instance.DAus[102] = "10";
                                switch (index)
                                {
                                    case 0:
                                Close();
                                MyProject.Forms.Druck.Show();
                                        break;
                                    case 1:
                                        goto IL_0096;
                                    default:
                                        break;
                                }
                                goto end_IL_0000_2;
                            IL_0096:
                                num = 17;
                                Frame1.Visible = false;
                                if (Strings.Mid(Label1[3].Text, 16, 10).AsDouble() < 1.0)
                                {
                                    Close();
                                    MyProject.Forms.Druck.Show();
                                    goto end_IL_0000_2;
                                }
                                num5 = 0;
                                while (num5 <= 3)
                                {
                                    RichTextBox1[num5].Font = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    num5 = (short)unchecked(num5 + 1);
                                }
                                RichTextBox1[0].Visible = true;
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.MoveLast();
                                M_KontLen = (byte)Strings.Len(DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim());
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                if (_Modul1.Instance.Druck_Tast == 0)
                                {
                                    RichTextBox1[0].SelectedText = _Modul1.Instance.IText[80];
                                }
                                else
                                {
                                    RichTextBox1[0].SelectedText = "Ahnenstammliste für";
                                }
                                goto IL_0221;
                            IL_0221: // <========== 3
                                num = 37;
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                RichTextBox1[0].SelectionAlignment = HorizontalAlignment.Center;
                                Bezeichnung1[0].Text = "Ahnenstammliste " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen für " + _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0].ToUpper();
                                Bezeichnung1[0].Refresh();
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3].TrimEnd() + " ";
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1].Trim() + " ";
                                }
                               M_Namen = _Modul1.Instance.Kont[0];
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[0].SelectedText =M_Namen.ToUpper() + "\n";
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[0].SelectedText = "Erstellt am " + DateTime.Today.AsString();
                                RichTextBox1[0].SelectedText = " von " + _Modul1.Instance.User.Owner.Trim() + " mit Gen_Plus aus Mandant: " + _Modul1.Instance.Verz + "\n";
                                RichTextBox1[0].SelectionAlignment = HorizontalAlignment.Left;
                                RichTextBox1[0].SelectedText = "\n";
                                RichTextBox1[0].SelectionHangingIndent = unchecked(M_KontLen) * 15;
                                DataModul.DT_AncesterTable.Index = "Namen";
                                DataModul.DT_AncesterTable.MoveFirst();
                                num6 = 1;
                                num7 = (long)Math.Round(Interaction.InputBox("Start mit Ahnennummer", " ", "1").AsDouble());
                                goto IL_0532;
                            IL_0532: // <========== 3
                                num = 59;
                                lErl = 2;
                                text2 = num7.AsString();
                                DataModul.DT_AncesterTable.Index = "Ahnen";
                                DataModul.DT_AncesterTable.Seek("=", new string(' ', 40) + text2.Right(40));
                                lErl = 1;
                                if (DataModul.DT_AncesterTable.EOF)
                                {
                                    goto end_IL_0000_2;
                                }
                                text3 = Strings.Right("00000000" + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), 8);
                                b2 = (byte)DataModul.DT_AncesterTable.Fields["Gen"].AsInt();
                                num6 = (short)(num6 + 1);
                                if (num6 == 12)
                                {
                                    num6 = 10;
                                }
                                goto IL_065f;
                            IL_065f:
                                num = 74;
                                if (!DataModul.DT_AncesterTable.NoMatch)
                                {
                                    text += ">";
                                    Bezeichnung1[2].Text = text;
                                    Bezeichnung1[2].Refresh();
                                    if (text.Length == 50)
                                    {
                                        text = "";
                                    }
                                    goto IL_06cc;
                                }
                                goto IL_1a15;
                            IL_06cc:
                                num = 83;
                                if (_Modul1.Instance.Schalt == 0)
                                {
                                    if (DataModul.DT_AncesterTable.Fields["Ahn"].AsInt() == num7 + 1)
                                    {
                                        if (unchecked(left == "M" && num7 != 1))
                                        {
                                            text4 = "";
                                            _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                            var aiFams2 = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                            List4.Items.AddRange(aiFams2.Select((i) => new ListItem($"{i}", i)).ToArray());
                                            if (aiFams2.Count > 1)
                                            {
                                                text4 = _Modul1.Instance.UbgT;
                                                _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                                persInArb = _Modul1.Instance.PersInArb;
                                                _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                                aiFams2 = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                                List4.Items.AddRange(aiFams2.Select((i) => new ListItem($"{i}", i)).ToArray());
                                                if (aiFams2.Count > 1)
                                                {
                                                    num9 = (short)aiFams2.Count;
                                                    num5 = 1;
                                                    goto IL_08cd;
                                                }
                                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                            
                                            }
                                            else
                                            {
                                                _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.AsDouble());
                                            }
                                            goto IL_0914;
                                        }

                                    }
                                }
                                goto IL_0ba1;
                            IL_08b4: // <========== 3
                                num = 107;
                                if (_Modul1.Instance.Schalt != 1)
                                {
                                    num5 = (short)unchecked(num5 + 1);
                                    goto IL_08cd;
                                }
                                goto IL_0914;
                            IL_08cd:
                                if (num5 <= num9)
                                {
                                    text5 = Strings.Mid(text4, num5 * 10 - 9, 10);
                                    b3 = (byte)Math.Round(_Modul1.Instance.UbgT.Length / 10.0);
                                    b4 = 1;
                                    while (unchecked(b4 <= (uint)b3))
                                    {
                                        if (Operators.CompareString(text5, Strings.Mid(_Modul1.Instance.UbgT, unchecked(b4) * 10 - 9, 10), TextCompare: false) == 0)
                                        {
                                            _Modul1.Instance.FamInArb = text5.AsInt();
                                            _Modul1.Instance.Schalt = 1;
                                            goto IL_08b4;
                                        }
                                        b4 = (byte)unchecked((uint)(b4 + 1));
                                    }
                                    goto IL_08b4;
                                }
                                goto IL_0914;
                            IL_0914: // <========== 5
                                num = 119;
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                _Modul1.Instance.Schalt = 0;
                                _Modul1.Instance.Famdatles2();
                                if (_Modul1.Instance.Kont[0].Trim() != "")
                                {
                                    RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[5];
                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[0] + "\n";
                                }
                                goto IL_09c6;
                            IL_09c6:
                                num = 126;
                                if (_Modul1.Instance.Kont[1].Trim() != "")
                                {
                                    RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[6];
                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[1] + "\n";
                                }
                                goto IL_0a38;
                            IL_0a38:
                                num = 130;
                                if (_Modul1.Instance.Kont[2].Trim() != "")
                                {
                                    RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[7];
                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2] + "\n";
                                }
                                goto IL_0ab0;
                            IL_0ab0:
                                num = 134;
                                if (_Modul1.Instance.Kont[3].Trim() != "")
                                {
                                    RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[8];
                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + "\n";
                                }
                                goto IL_0b28;
                            IL_0b28:
                                num = 138;
                                if (_Modul1.Instance.Kont[4].Trim() != "")
                                {
                                    RichTextBox1[0].SelectedText = "                      " + _Modul1.Instance.DTxt[9];
                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[4] + "\n";
                                }
                                goto IL_0ba1;
                            IL_0ba1: // <========== 4
                                num = 145;
                                if (DataModul.DT_AncesterTable.Fields["Ehe"].AsInt() != 0)
                                {
                                    objectValue = DataModul.DT_AncesterTable.Fields["Ehe"].Value;
                                }
                                _Modul1.Instance.Schalt = 0;
                                num7 = DataModul.DT_AncesterTable.Fields["Ahn"].AsLong();
                                _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                persInArb = _Modul1.Instance.PersInArb;
                                if (_Modul1.Instance.PersInArb == 0)
                                {
                                    _Modul1.Instance.FamInArb = DataModul.DT_AncesterTable.Fields["Ehe"].AsInt();
                                    goto IL_1a15;
                                }
                                DataModul.DB_PersonTable.Seek("=", _Modul1.Instance.PersInArb.AsString());
                                left = DataModul.DB_PersonTable.Fields[PersonFields.Sex].AsString();
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                if (unchecked(b2 > (uint)b))
                                {
                                    RichTextBox1[0].SelectedText = "\n";
                                    RichTextBox1[0].Visible = true;
                                    Bezeichnung1[1].Text = "Bearbeite Stamm " + _Modul1.Instance.AltName;
                                    Bezeichnung1[1].Refresh();
                                    text6 = "Generation " + b2.AsString() + "\n";
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                lErl = 3;
                                HPerschreib();
                                List4.Items.Add(_Modul1.Instance.PersInArb.AsString());
                                if (left == "M")
                                {
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkFather;
                                }
                                else
                                {
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                }
                                goto IL_0ec3;
                            IL_0ec3: // <========== 3
                                num = 177;
                                var aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                List1.Items.Clear();
                                num10 = aiFams.Count;
                                num5 = 1;
                                goto IL_10ea;
                            IL_10ba: // <========== 3
                                num = 192;
                                _Modul1.Instance.UbgT = Strings.Mid(_Modul1.Instance.UbgT, 11, _Modul1.Instance.UbgT.Length);
                                num5 = (short)unchecked(num5 + 1);
                                goto IL_10ea;
                            IL_10ea:
                                if (num5 <= num10)
                                {
                                    _Modul1.Instance.FamInArb = (int)Math.Round(_Modul1.Instance.UbgT.Left(10).AsDouble());
                                    if (ObjectFlowControl.ForLoopControl.ForLoopInitObj(CounterResult, 500, 508, 1, ref LoopForResult2, ref CounterResult))
                                    {
                                        while (ObjectFlowControl.ForLoopControl.ForNextCheckObj(CounterResult, LoopForResult2, ref CounterResult))
                                        {
                                            _Modul1.Instance.Ubg = CounterResult.AsInt();
                                            _Modul1.Instance.Art = (EEventArt)_Modul1.Instance.Ubg;
                                            DataModul.DB_EventTable.Index = "ArtNr";
                                            DataModul.DB_EventTable.Seek("=", _Modul1.Instance.Ubg.AsString(), _Modul1.Instance.FamInArb.AsString(), "0");
                                            if (!DataModul.DB_EventTable.NoMatch)
                                            {
                                                _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                                List1.Items.Add(Strings.Right("        " + _Modul1.Instance.sDatu.AsDouble().AsString(), 8) + "          " + _Modul1.Instance.FamInArb.AsString());

                                            }
                                        }
                                    }
                                    goto IL_10ba;
                                }
                                num11 = (short)(List1.Items.Count - 1);
                                num5 = 0;
                                goto IL_1a0c;
                            IL_1327: // <========== 4
                                num = 213;
                                _Modul1.Instance.Famles();
                                if (_Modul1.Instance.Family.Frau == persInArb)
                                {
                                    if (_Modul1.Instance.Family.Mann > 0)
                                    {
                                        _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                        DataModul.DT_AncesterTable.Index = "Pernr";
                                        DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.Family.Mann);
                                        _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                        HPerschreib();
                                        Eltern();
                                    }
                                    goto IL_19fe;
                                }
                                if (_Modul1.Instance.Family.Frau > 0)
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    DataModul.DT_AncesterTable.Index = "Pernr";
                                    DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.Family.Frau);
                                    HPerschreib();
                                    _Modul1.Instance.eLKennz = ELinkKennz.lkMother;
                                    aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                                    if (aiFams.Count > 1)
                                    {
                                        right = _Modul1.Instance.FamInArb;
                                        List3.Items.Clear();
                                        foreach (var iFam in aiFams)
                                        {
                                            _Modul1.Instance.FamInArb = iFam;
                                            for (_Modul1.Instance.Art = EEventArt.eA_500; _Modul1.Instance.Art <= EEventArt.eA_507; _Modul1.Instance.Art++)
                                            {
                                                DataModul.DB_EventTable.Index = "ArtNr";
                                                DataModul.DB_EventTable.Seek("=", _Modul1.Instance.Art, _Modul1.Instance.FamInArb.AsString(), "0");
                                                if (!DataModul.DB_EventTable.NoMatch)
                                                {
                                                    _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                                                    List3.Items.Add(Strings.Right("        " + _Modul1.Instance.sDatu.AsDouble().AsString(), 8) + "          " + _Modul1.Instance.FamInArb.AsString());
                                                }
                                            }
                                        }
                                        _Modul1.Instance.DAus[102] = 8.AsString();
                                        b7 = (byte)(List3.Items.Count - 1);
                                        b6 = 0;
                                        goto IL_19e8;
                                    }
                                }
                                goto IL_19f1;
                            IL_199a: // <========== 4
                                num = 271;
                                _Modul1.Instance.Famles();
                                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                Perschreib1(_Modul1.Instance.PersInArb);
                                _Modul1.Instance.DAus[102] = 10.AsString();
                                goto IL_19da;
                            IL_19da: // <========== 3
                                num = 276;
                                b6 = (byte)unchecked((uint)(b6 + 1));
                                goto IL_19e8;
                            IL_19e8:
                                if (unchecked(b6 <= (uint)b7))
                                {
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List3.Items[b6].AsString(), 9, 15)));
                                    RichTextBox1[0].SelectionIndent = unchecked(M_KontLen) * 20;
                                    if (_Modul1.Instance.FamInArb != right.AsInt())
                                    {
                                        _Modul1.Instance.Famdatles2();
                                        RichTextBox1[0].SelectedText = "\n";
                                        RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                        if (List3.Items.Count > 1)
                                        {
                                            if (b6 == 0)
                                            {
                                                RichTextBox1[0].SelectedText = " oo " + Strings.Trim(unchecked(b6) + 1.AsString()) + ". Ehe " + _Modul1.Instance.Kont[2] + " mit \n";
                                            }
                                            else
                                            {
                                                RichTextBox1[0].SelectedText = Ahne.Trim() + " oo " + Strings.Trim(unchecked(b6) + 1.AsString()) + ". Ehe " + _Modul1.Instance.Kont[2] + " mit \n";
                                            }
                                        
                                        }
                                        else
                                        {
                                            RichTextBox1[0].SelectedText = " oo " + _Modul1.Instance.Kont[2] + " mit \n";
                                        }
                                        goto IL_199a;
                                    }
                                    goto IL_19da;
                                }
                                goto IL_19f1;
                            IL_19f1: // <========== 3
                                num = 279;
                                Eltern();
                                goto IL_19fe;
                            IL_19fe: // <========== 3
                                num = 281;
                                num5 = (short)unchecked(num5 + 1);
                                goto IL_1a0c;
                            IL_1a0c:
                                if (num5 <= num11)
                                {
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    _Modul1.Instance.FamInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[num5].AsString(), 9, 20)));
                                    RichTextBox1[0].SelectionIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5);
                                    _Modul1.Instance.Famdatles2();
                                    if (_Modul1.Instance.Kont[2].Trim() == "")
                                    {
                                        _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[3];
                                    }
                                    if (List1.Items.Count > 1)
                                    {
                                        if (num5 == 0)
                                        {
                                            RichTextBox1[0].SelectedText = " oo" + (num5 + 1.AsString()).Trim() + " " + _Modul1.Instance.Kont[2] + " mit \n";
                                        }
                                        else
                                        {
                                            RichTextBox1[0].SelectedText = num7.AsString() + " oo" + (num5 + 1.AsString()).Trim() + " " + _Modul1.Instance.Kont[2] + " mit \n";
                                        }
                                    
                                    }
                                    else
                                    {
                                        RichTextBox1[0].SelectedText = " oo " + _Modul1.Instance.Kont[2] + " mit \n";
                                    }
                                    goto IL_1327;
                                }
                                goto IL_1a15;
                            IL_1a15: // <========== 4
                                num = 282;
                                lErl = 4;
                                num7 *= 2;
                                goto IL_0532;
                            IL_1ac3:
                                num4 = unchecked(num2 + 1);
                                goto IL_1ac7;
                            IL_1ac7:
                                num2 = 0;
                                switch (num4)
                                {
                                    case 1:
                                        break;
                                    case 33:
                                    case 36:
                                    case 37:
                                        goto IL_0221;
                                    case 59:
                                    case 284:
                                        goto IL_0532;
                                    case 73:
                                    case 74:
                                        goto IL_065f;
                                    case 82:
                                    case 83:
                                        goto IL_06cc;
                                    case 104:
                                    case 107:
                                        goto IL_08b4;
                                    case 108:
                                    case 111:
                                    case 114:
                                    case 115:
                                    case 118:
                                    case 119:
                                        goto IL_0914;
                                    case 125:
                                    case 126:
                                        goto IL_09c6;
                                    case 129:
                                    case 130:
                                        goto IL_0a38;
                                    case 133:
                                    case 134:
                                        goto IL_0ab0;
                                    case 137:
                                    case 138:
                                        goto IL_0b28;
                                    case 141:
                                    case 142:
                                    case 143:
                                    case 144:
                                    case 145:
                                        goto IL_0ba1;
                                    case 173:
                                    case 176:
                                    case 177:
                                        goto IL_0ec3;
                                    case 189:
                                    case 192:
                                        goto IL_10ba;
                                    case 205:
                                    case 208:
                                    case 209:
                                    case 212:
                                    case 213:
                                        goto IL_1327;
                                    case 263:
                                    case 266:
                                    case 267:
                                    case 270:
                                    case 271:
                                        goto IL_199a;
                                    case 275:
                                    case 276:
                                        goto IL_19da;
                                    case 277:
                                    case 278:
                                    case 279:
                                        goto IL_19f1;
                                    case 222:
                                    case 223:
                                    case 280:
                                    case 281:
                                        goto IL_19fe;
                                    case 75:
                                    case 154:
                                    case 282:
                                        goto IL_1a15;
                                    case 9:
                                    case 14:
                                    case 15:
                                    case 21:
                                    case 66:
                                    case 285:
                                    case 286:
                                    case 287:
                                    case 288:
                                    case 292:
                                    case 298:
                                        goto end_IL_0000_2;
                                }
                                goto default;
                        }
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 8061;
                    continue;
                }
                throw ProjectData.CreateProjectError(-2146828237);
            end_IL_0000_2: // <========== 6
                break;
            }
            if (num2 != 0)
            {
                ProjectData.ClearProjectError();
            }
        }
        private void Command2_Click(object eventSender, EventArgs eventArgs)
        {
            int index = Command2.GetIndex((Button)eventSender);
            Frame2.Visible = false;
            _Modul1.Instance.Ind1 = "";
            GenFree.Interfaces.DB.IRecordset dT_OrtIdxTable = DataModul.DSB_OrtIdxTable;
            RichTextBox richTextBox = RichTextBox1[3];

            richTextBox.Text = "";
            richTextBox.SelectionHangingIndent = 0;
            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            switch (index)
            {
                case 0:
                    richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                    richTextBox.SelectedText = "Ortsindex";
                    richTextBox.SelectedText = "\n";
                    dT_OrtIdxTable.Index = "Ort";
                    dT_OrtIdxTable.Seek(">=", " ");
                    while (!dT_OrtIdxTable.EOF)
                    {
                        if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionIndent = 0;
                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles(dT_OrtIdxTable.Fields["OrtNr"].AsInt(), 0);
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            richTextBox.SelectedText = _Modul1.Instance.UbgT;
                            _Modul1.Instance.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            richTextBox.SelectionIndent = 40;
                        }
                        richTextBox.SelectedText = dT_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                        dT_OrtIdxTable.MoveNext();
                    }
                    Befehl[4].Visible = true;
                    break;
                case 1:
                    richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                    richTextBox.SelectedText = "Index Orte-Namen";
                    richTextBox.SelectedText = "\n";
                    richTextBox.SelectionHangingIndent = checked((int)Math.Round(0.0 * 1440.0 / DeviceDpi));
                    dT_OrtIdxTable.Index = "ortnam";
                    dT_OrtIdxTable.Seek(">=", " ");
                    while (!dT_OrtIdxTable.EOF)
                    {
                        if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                            richTextBox.SelectionIndent = 0;
                            _Modul1.Instance.AltName = "";
                            _Modul1.Instance.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            _Modul1.Instance.UbgT = _Modul1.Instance.ortles(dT_OrtIdxTable.Fields["OrtNr"].AsInt(), 0);
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            richTextBox.SelectedText = _Modul1.Instance.UbgT;
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            richTextBox.SelectionIndent = 40;
                        }
                        if (Operators.CompareString(dT_OrtIdxTable.Fields["Name"].AsString().Trim(), _Modul1.Instance.AltName.Trim(), TextCompare: false) != 0)
                        {
                            if (_Modul1.Instance.AltName != "")
                            {
                                richTextBox.SelectedText = "\n";
                            }
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            richTextBox.SelectedText = (dT_OrtIdxTable.Fields["Name"].Value +  "  ").AsString();
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            _Modul1.Instance.AltName = dT_OrtIdxTable.Fields["Name"].AsString();
                        }
                        richTextBox.SelectedText = dT_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                        dT_OrtIdxTable.MoveNext();
                    }
                    Befehl[4].Visible = true;
                    break;
                case 2:
                    richTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                    richTextBox.SelectedText = "Index Namen-Orte";
                    richTextBox.SelectedText = "\n";
                    dT_OrtIdxTable.Index = "NameOrt";
                    dT_OrtIdxTable.Seek(">=", " ");
                    while (!dT_OrtIdxTable.EOF)
                    {
                        if (Operators.CompareString(dT_OrtIdxTable.Fields["Name"].AsString().Trim(), _Modul1.Instance.AltName.Trim(), TextCompare: false) != 0)
                        {
                            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionIndent = 0;
                            _Modul1.Instance.AltNr = 0;
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            richTextBox.SelectedText = dT_OrtIdxTable.Fields["Name"].AsString();
                            _Modul1.Instance.AltName = dT_OrtIdxTable.Fields["Name"].AsString();
                            richTextBox.SelectedText = "\n";
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            richTextBox.SelectionIndent = 40;
                        }
                        if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            if (_Modul1.Instance.AltNr > 0)
                            {
                                richTextBox.SelectedText = "\n";
                            }
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                            richTextBox.SelectedText = (dT_OrtIdxTable.Fields["Ort"].Value +  "  ").AsString();
                            richTextBox.SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            _Modul1.Instance.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                        }
                        richTextBox.SelectedText = dT_OrtIdxTable.Fields["Ind"].AsString().Trim() + "; ";
                        dT_OrtIdxTable.MoveNext();
                    }
                    Befehl[4].Visible = true;
                    break;
                case 3:
                    _Modul1.Instance.PrintDat.Flagsch = 1;
                    dT_OrtIdxTable.Index = "Ort";
                    dT_OrtIdxTable.Seek(">=", " ");
                    while (!dT_OrtIdxTable.EOF)
                    {
                        if (dT_OrtIdxTable.Fields["OrtNr"].AsInt() != _Modul1.Instance.AltNr)
                        {
                            richTextBox.SelectionIndent = 0;
                            _Modul1.Instance.ortles(dT_OrtIdxTable.Fields["OrtNr"].AsInt(), 2);
                            richTextBox.SelectedText = _Modul1.Instance.UbgT;
                            _Modul1.Instance.AltNr = dT_OrtIdxTable.Fields["OrtNr"].AsInt();
                            richTextBox.SelectedText = "\n";
                        }
                        dT_OrtIdxTable.MoveNext();
                    }
                    break;
                default:
                    Interaction.MsgBox(index);
                    break;
            }
        }

        private void AhneneST_Load(object eventSender, EventArgs eventArgs)
        {
            int try0000_dispatch = -1;
            int num = default;
            int num2 = default;
            int num3 = default;
            int lErl = default;
            int num6 = default;
            while (true)
            {
                try
                {
                    /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                    ;
                    int num4;
                    short Listart;
                    string Ahne;
                    bool neb;
                    int num7;
                    switch (try0000_dispatch)
                    {
                        default:
                            num = 1;
                            BackColor = _Modul1.Instance.HintFarb;
                            goto IL_0013;
                        case 1830:
                            {
                                num2 = num;
                                switch (num3)
                                {
                                    case 2:
                                        break;
                                    case 1:
                                        goto IL_0594;
                                    default:
                                        goto end_IL_0000;
                                }
                                int number = Information.Err().Number;
                                if (number == 55)
                                {
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    goto IL_0594;
                                }
                                if (number == 3021)
                                {
                                    short num5 = 11;
                                    while (num5 <= 14)
                                    {
                                        _Modul1.Instance.Kont[num5] = "";
                                        num5 = checked((short)unchecked(num5 + 1));
                                    }
                                    ProjectData.ClearProjectError();
                                    if (num2 == 0)
                                    {
                                        throw ProjectData.CreateProjectError(-2146828268);
                                    }
                                    num2 = 0;
                                    goto IL_03f7;
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
                                goto IL_0598;
                            }
                        end_IL_0000:
                            break;
                        IL_0013:
                            num = 2;
                            Befehl[3].Text = _Modul1.Instance.IText[47];
                            _Modul1.Instance.Dateienopen();
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
                                _ => _Modul1.Instance.Fs,
                            };
                            goto IL_01c1;
                        IL_01c1: // <========== 12
                            num = 44;
                            Font = new Font("Arial", _Modul1.Instance.Fs, FontStyle.Regular);
                            Show();
                            ProjectData.ClearProjectError();
                            num3 = 2;
                            num7 = num6;
                            num6 = 0;
                            FileSystem.FileClose(30);
                            Label1[3].Text = "Keine Berechnung vorhanden";
                            Label1[0].Text = "Sie müssen erst die Ahnen berechnen.";
                            DataModul.DT_AncesterTable.Index = "Ahnen";
                            DataModul.DT_AncesterTable.Seek("=", 1);
                            DataModul.DT_AncesterTable.MoveFirst();
                            if (!DataModul.DT_AncesterTable.EOF)
                            {
                                if (!DataModul.DT_AncesterTable.NoMatch)
                                {
                                    string text = DataModul.DT_AncesterTable.Fields["Ahn"].AsString();
                                    DataModul.DT_AncesterTable.MoveLast();
                                    Label1[3].Text = "Ahnenberechnung " + DataModul.DT_AncesterTable.Fields["Gen"].AsString() + " Generationen vorhanden für Ahnenziffer " + text;
                                    DataModul.DT_AncesterTable.MoveFirst();
                                    _Modul1.Instance.PersInArb = DataModul.DT_AncesterTable.Fields["PerNr"].AsInt();
                                    _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                    Label1[0].Text = _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0].ToUpper();
                                    Listart = 0;
                                    Ahne = 0.AsString();
                                    neb = false;
                                    _Modul1.Instance.Datles3(Listart, default, default, ref neb);

                                }
                            }
                            goto IL_03f7;
                        IL_03f7: // <========== 4
                            num = 69;
                            lErl = 1;
                            Label1[5].Text = _Modul1.Instance.IText[3] + " " + _Modul1.Instance.Kont[11];
                            Label1[2].Text = _Modul1.Instance.IText[5] + " " + _Modul1.Instance.Kont[13];
                            Label1[1].Text = _Modul1.Instance.IText[4] + " " + _Modul1.Instance.Kont[12];
                            Label1[4].Text = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                            goto end_IL_0000_2;
                        IL_0594:
                            num4 = num2 + 1;
                            goto IL_0598;
                        IL_0598:
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 12:
                                case 16:
                                case 19:
                                case 22:
                                case 25:
                                case 28:
                                case 31:
                                case 34:
                                case 37:
                                case 40:
                                case 43:
                                case 44:
                                    goto IL_01c1;
                                case 56:
                                case 59:
                                case 69:
                                case 87:
                                    goto IL_03f7;
                            }
                            goto default;
                    }
                }
                catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
                {
                    ProjectData.SetProjectError(obj, lErl);
                    try0000_dispatch = 1830;
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
        private void AhneneST_FormClosing(object eventSender, FormClosingEventArgs eventArgs)
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

        public void HPerschreib()
        {
            checked
            {
                if (!DataModul.DT_AncesterTable.NoMatch)
                {
                    Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                    Ahnes = Strings.Right("00" + DataModul.DT_AncesterTable.Fields["Gen"].AsString().Trim(), 2) + Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                }
                else
                {
                    Ahnes = "                ".Right(unchecked(M_KontLen) + 3);
                }
                RichTextBox1[0].SelectionIndent = 0;
                RichTextBox1[0].SelectionHangingIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5);
                RichTextBox1[0].SelectionFont = new Font("Courier", 10f, RichTextBox1[0].SelectionFont.Style);
                RichTextBox1[0].SelectedText = "\n" + Ahnes;
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                Namensindex();
                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + " ";
                if (_Modul1.Instance.Kont[1] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1] + " ";
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[0].ToUpper();
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[2].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2].TrimEnd();
                }
                if (_Modul1.Instance.Kont[5].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5].TrimEnd();
                }
                if (_Modul1.Instance.Kont[4].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = " (" + _Modul1.Instance.Kont[4].ToUpper() + ")";
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.Ubg = 300;
                Berufe((EEventArt)_Modul1.Instance.Ubg);
                _Modul1.Instance.Ubg = 301;
                Berufe((EEventArt)_Modul1.Instance.Ubg);
                RichTextBox1[0].SelectedText = "\n";
                RichTextBox1[0].SelectionIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5);
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                string sBem1 = DataModul.Person.Seek(_Modul1.Instance.PersInArb)?.Fields["Bem1"].AsString();
                if (Operators.CompareString(sBem1.Trim(), "", TextCompare: false) != 0)
                {
                    RichTextBox1[0].SelectedText = sBem1;
                    RichTextBox1[0].SelectedText = "\n";
                }
                RichTextBox1[0].SelectionIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5);
                _Modul1.Instance.Datles4();
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[11] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11] + " ";
                }
                if (_Modul1.Instance.Kont[12] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12];
                }
                if (_Modul1.Instance.Kont[11] != "" | _Modul1.Instance.Kont[12] != "")
                {
                    RichTextBox1[0].SelectedText = "\n";
                }
                if (_Modul1.Instance.Kont[13] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13] + " ";
                }
                if (_Modul1.Instance.Kont[14] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                }
                if (_Modul1.Instance.Kont[13] != "" | _Modul1.Instance.Kont[14] != "")
                {
                    RichTextBox1[0].SelectedText = "\n";
                }
            }
        }

        public void Namensindex()
        {
            string Kont0 = _Modul1.Instance.Kont[0];
            string ahne = Ahne;
            string Kont3 = _Modul1.Instance.Kont[3];

            if (Kont0 != "" && Kont0 != "NN" && Kont0 != "N.N.")
            {
               M_Namen = Kont0;
                _Modul1.Instance.Ind1 = ahne.AsLong().AsString().PadLeft(20);

                DataModul.DSB_NamIdxTable.AddNew();
                DataModul.DSB_NamIdxTable.Fields["Name"].Value = Kont3 == "" ? "?" : Kont3;
                DataModul.DSB_NamIdxTable.Fields["Name1"].Value = Kont0;
                DataModul.DSB_NamIdxTable.Fields["Nr"].Value = ahne;
                DataModul.DSB_NamIdxTable.Update();
            }
        }

        public void Berufe(EEventArt eArt)
        {
            List2.Items.Clear();
            int M1_J;
            checked
            {
                DataModul.DB_EventTable.Index = "Besu";
                DataModul.DB_EventTable.Seek("=", eArt.AsString(), _Modul1.Instance.PersInArb.AsString());
                if (DataModul.DB_EventTable.NoMatch)
                {
                    DataModul.DB_EventTable.Index = "ArtNr";
                    return;
                }
                Job1 = "";
                _0024STATIC_0024Berufe_00242001_0024I1 = 1;
                while (!DataModul.DB_EventTable.EOF)
                {
                    if (!Operators.ConditionalCompareObjectLess(DataModul.DB_EventTable.Fields[EventFields.LfNr].Value, 1, TextCompare: false))
                    {
                        M1_J = 0;
                        do
                        {
                            _Modul1.Instance.Kont[M1_J] = "";
                            M1_J = (byte)unchecked((uint)(M1_J + 1));
                        }
                        while (unchecked(M1_J) <= 15u);
                        _Modul1.Instance.Ubg = _0024STATIC_0024Berufe_00242001_0024I1;
                        Dat = "        ";
                        if (Conversions.ToBoolean(DataModul.DB_EventTable.NoMatch
                            | DataModul.DB_EventTable.Fields[EventFields.PerFamNr].AsInt() != _Modul1.Instance.PersInArb
                            | DataModul.DB_EventTable.Fields[EventFields.Art].AsEnum<EEventArt>() != eArt))
                        {
                            DataModul.DB_EventTable.Index = "ArtNr";
                            break;
                        }
                        _Modul1.Instance.UbgT = "";
                        _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumV].AsString();
                        if (_Modul1.Instance.sDatu.AsDouble() == 0.0)
                        {
                            _Modul1.Instance.sDatu = DataModul.DB_EventTable.Fields[EventFields.DatumB].AsString();
                        }
                        Dat = Strings.Right("        " + _Modul1.Instance.sDatu.AsDouble().AsString(), 8);
                        if (DataModul.DB_EventTable.Fields[EventFields.KBem].Value.AsDouble() > 0.0)
                        {
                            int AAA = DataModul.DB_EventTable.Fields[EventFields.KBem].AsInt();
                            string LD;
                            _Modul1.Instance.Kont[0] = DataModul.TextLese1(AAA);
                            if (_Modul1.Instance.Kont[0] != "")
                            {
                                _Modul1.Instance.Kont[8] = " " + _Modul1.Instance.Kont[0].Trim() + " ";
                            }
                        }
                        if (_Modul1.Instance.Kont[3] != "" | _Modul1.Instance.Kont[4] != "")
                        {
                            _Modul1.Instance.Kont[2] = " {" + _Modul1.Instance.Kont[3].Trim();
                            if (_Modul1.Instance.Kont[3] == "")
                            {
                                _Modul1.Instance.Kont[2] = " {" + _Modul1.Instance.Kont[4].Trim();
                            }
                            else
                            {
                                _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[2] + " " + _Modul1.Instance.Kont[4].Trim();
                            }
                            _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[2] + "} ";
                        }
                        if (DataModul.DB_EventTable.Fields[EventFields.Reg].AsString() != " ")
                        {
                            Job1 = _Modul1.Instance.Kont[8].Trim();
                            List2.Items.Add("");
                        }
                        else
                        {
                            List2.Items.Add(Dat + _Modul1.Instance.Kont[8].Trim());
                        }
                    }
                    DataModul.DB_EventTable.MoveNext();
                    _0024STATIC_0024Berufe_00242001_0024I1 = (byte)unchecked((uint)(_0024STATIC_0024Berufe_00242001_0024I1 + 1));
                    if (unchecked(_0024STATIC_0024Berufe_00242001_0024I1) > 70u)
                    {
                        break;
                    }
                }
                switch (eArt)
                {
                    case EEventArt.eA_300:
                        if (List2.Items.Count == 0)
                        {
                            return;
                        }
                        if (List2.Items.Count == 1)
                        {
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[7] + " ";
                        }
                        if (List2.Items.Count > 1)
                        {
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[111] + " ";
                        }
                        break;
                    case EEventArt.eA_301:
                        if (List2.Items.Count == 0)
                        {
                            return;
                        }
                        if (List2.Items.Count > 0)
                        {
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[70] + " ";
                        }
                        break;
                    case EEventArt.eA_302:
                        if (List2.Items.Count == 0)
                        {
                            return;
                        }
                        if (List2.Items.Count == 1)
                        {
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[8];
                        }
                        if (List2.Items.Count > 1)
                        {
                            RichTextBox1[0].SelectedText = " " + _Modul1.Instance.IText[8];
                        }
                        break;
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                Job = "";
                byte b = (byte)(List2.Items.Count - 1);
                _0024STATIC_0024Berufe_00242001_0024I1 = 0;
                while (unchecked(_0024STATIC_0024Berufe_00242001_0024I1 <= (uint)b))
                {
                    if (Operators.CompareString(List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString(), List2.Items[unchecked(_0024STATIC_0024Berufe_00242001_0024I1) + 1].AsString(), TextCompare: false) != 0 & List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString() != Job1 && List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString() != "")
                    {
                        if (Job == "" & Job1 == "")
                        {
                            Job = Strings.Mid(List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString(), 9, List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString().Length);
                        }
                        else
                        {
                            Job = Job + "; " + Strings.Mid(List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString(), 9, List2.Items[_0024STATIC_0024Berufe_00242001_0024I1].AsString().Length);
                        }
                    }
                    _0024STATIC_0024Berufe_00242001_0024I1 = (byte)unchecked((uint)(_0024STATIC_0024Berufe_00242001_0024I1 + 1));
                }
                Job = Job1.Trim() + Job + ".";
                if (Job.Trim() != "")
                {
                    RichTextBox1[0].SelectedText = Job.Trim();
                }
                List2.Items.Clear();
            }
        }

        public void Eltern()
        {
            _Modul1.Instance.DAus[102] = 8.AsString();
            DataModul.Link.GetPersonFam(_Modul1.Instance.PersInArb, ELinkKennz.lkChild, out var iFamNr); _Modul1.Instance.Ubg = iFamNr;
            _Modul1.Instance.FamInArb = iFamNr;
            _Modul1.Instance.Famles(iFamNr);
            _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
            Perschreib();
            _Modul1.Instance.Famdatles2();
            if (_Modul1.Instance.Kont[2].Trim() != "")
            {
                RichTextBox1[0].SelectedText = "\n";
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                RichTextBox1[0].SelectedText = "  oo " + _Modul1.Instance.Kont[2] + " mit \n";
            }
            checked
            {
                RichTextBox1[0].SelectionIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5);
                _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                Perschreib();
                _Modul1.Instance.DAus[102] = 10.AsString();
            }
        }

        public void Perschreib()
        {
            if (_Modul1.Instance.PersInArb == 0)
            {
                return;
            }
            DataModul.DT_AncesterTable.Index = "Pernr";
            DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.PersInArb);
            checked
            {
                if (!DataModul.DT_AncesterTable.NoMatch)
                {
                    this.Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                    Ahnes = Strings.Right("00" + DataModul.DT_AncesterTable.Fields["Gen"].AsString().Trim(), 2) + Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                }
                else
                {
                    Ahnes = DataModul.Person.GetSex(_Modul1.Instance.PersInArb) == "F" ? "Mutter:" : "Vater:";
                }
                RichTextBox1[0].SelectionFont = new Font("Courier", 8f, RichTextBox1[0].SelectionFont.Style);
                RichTextBox1[0].SelectedText = "\n" + Ahnes;
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                Namensindex();
                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + " ";
                if (_Modul1.Instance.Kont[1] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1] + " ";
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[0].ToUpper();
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[2].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2].TrimEnd();
                }
                if (_Modul1.Instance.Kont[5].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5].TrimEnd();
                }
                if (_Modul1.Instance.Kont[4].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = " (" + _Modul1.Instance.Kont[4].ToUpper() + ")";
                }
                _Modul1.Instance.Ubg = 300;
                Berufe((EEventArt)_Modul1.Instance.Ubg);
                _Modul1.Instance.Ubg = 301;
                Berufe((EEventArt)_Modul1.Instance.Ubg);
                short Listart = 0;
                string Ahne = 0.AsString();
                bool neb = false;
                _Modul1.Instance.Datles3(Listart, default, default, ref neb);
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                RichTextBox1[0].SelectionIndent = unchecked(M_KontLen) * 30;
                RichTextBox1[0].SelectedText = "\n";
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[11] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11] + " ";
                }
                if (_Modul1.Instance.Kont[12] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12];
                }
                if (_Modul1.Instance.Kont[13] != "" | _Modul1.Instance.Kont[14] != "")
                {
                    RichTextBox1[0].SelectedText = "\n";
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[13] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13] + " ";
                }
                if (_Modul1.Instance.Kont[14] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                }
                RichTextBox1[0].SelectedText = "\n";
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }

        public void Perschreib1(int persInArb)
        {
            if (persInArb != 0)
            {
                DataModul.DT_AncesterTable.Index = "Pernr";
                DataModul.DT_AncesterTable.Seek("=", persInArb);
                if (!DataModul.DT_AncesterTable.NoMatch)
                {
                    this.Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                    Ahnes = Strings.Right("00" + DataModul.DT_AncesterTable.Fields["Gen"].AsString().Trim(), 2) + Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                }
                else
                {
                    Ahnes = "";
                }
                RichTextBox1[0].SelectionFont = new Font("Courier", 8f, RichTextBox1[0].SelectionFont.Style);
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                _Modul1.Instance.Person_ReadNames(persInArb, _Modul1.Instance.Person);
                Namensindex();
                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + " ";
                if (_Modul1.Instance.Kont[1] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1] + " ";
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[0].ToUpper();
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[2].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[2].TrimEnd();
                }
                if (_Modul1.Instance.Kont[5].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = ", Sippe " + _Modul1.Instance.Kont[5].TrimEnd();
                }
                if (_Modul1.Instance.Kont[4].Trim() != "")
                {
                    RichTextBox1[0].SelectedText = " (" + _Modul1.Instance.Kont[4].ToUpper() + ")";
                }
                _Modul1.Instance.Ubg = 300;
                Berufe((EEventArt)_Modul1.Instance.Ubg);
                _Modul1.Instance.Ubg = 301;
                Berufe((EEventArt)_Modul1.Instance.Ubg);
                short Listart = 0;
                string Ahne = 0.AsString();
                EEventArt Art = default;
                bool neb = false;
                _Modul1.Instance.Datles3(Listart, default, Art, ref neb);
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[11] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11] + " ";
                }
                if (_Modul1.Instance.Kont[12] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12];
                }
                if (_Modul1.Instance.Kont[13] != "" | _Modul1.Instance.Kont[14] != "")
                {
                    RichTextBox1[0].SelectedText = "\n";
                }
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                if (_Modul1.Instance.Kont[13] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13] + " ";
                }
                if (_Modul1.Instance.Kont[14] != "")
                {
                    RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                }
                RichTextBox1[0].SelectedText = "\n";
                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
            }
        }

        public void Kinder()
        {
            string text = "";
            List5.Items.Clear();
            int count = List4.Items.Count;
            checked
            {
                for (_Kinder_I = 0; _Kinder_I <= count; _Kinder_I++)
                {
                    byte b = 0;
                    _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(List4.Items[_Kinder_I].AsString()));
                    _Modul1.Instance.eLKennz = DataModul.Person.GetSex(_Modul1.Instance.PersInArb) == "F" ? ELinkKennz.lkMother : ELinkKennz.lkFather;
                    var aiFams = DataModul.Link.GetPersonFams(_Modul1.Instance.PersInArb, _Modul1.Instance.eLKennz);
                    RichTextBox1[0].SelectedText = "\n";
                    List2.Items.Clear();
                    int num = aiFams.Count;
                    foreach (var iFam in aiFams)
                    {
                        _Modul1.Instance.FamInArb = iFam;
                        _Modul1.Instance.Schalt = 1;
                        _Modul1.Instance.Famdatles(_Modul1.Instance.FamInArb, out var asFamDates);
                        _Modul1.Instance.Schalt = 0;
                        if (_Modul1.Instance.Kont[2].Trim() == "")
                        {
                            _Modul1.Instance.Kont[2] = _Modul1.Instance.Kont[3];
                        }
                        if (_Modul1.Instance.Kont[2].Trim() == "")
                        {
                            _Modul1.Instance.Kont[2] = "         ";
                        }
                        List2.Items.Add(_Modul1.Instance.Kont[2] + "          " + _Modul1.Instance.FamInArb.AsString());
                    }
                    int count2 = List2.Items.Count;
                    _0024STATIC_0024Kinder_00242001_0024Fa1 = 0;
                    while (_0024STATIC_0024Kinder_00242001_0024Fa1 <= count2)
                    {
                        _0024STATIC_0024Kinder_00242001_0024Fam = (int)Math.Round(Conversion.Val(List2.Items[_0024STATIC_0024Kinder_00242001_0024Fa1].AsString().Right(10)));
                        int num2 = List5.Items.Count - 1;
                        _0024STATIC_0024Kinder_00242001_0024F = 0;
                        while (_0024STATIC_0024Kinder_00242001_0024F <= num2 && _0024STATIC_0024Kinder_00242001_0024F <= List5.Items.Count)
                        {
                            if (Conversion.Val(List5.Items[_0024STATIC_0024Kinder_00242001_0024F].AsString()) == _0024STATIC_0024Kinder_00242001_0024Fam)
                            {
                                goto end_IL_1094;
                            }
                            _0024STATIC_0024Kinder_00242001_0024F++;
                        }
                        List5.Items.Add(_0024STATIC_0024Kinder_00242001_0024Fam.AsString());
                        _Modul1.Instance.FamInArb = (int)Math.Round(_0024STATIC_0024Kinder_00242001_0024Fam.AsDouble());
                        _Modul1.Instance.Famles();
                        List1.Items.Clear();
                        _0024STATIC_0024Kinder_00242001_0024K = 1;
                        while (_Modul1.Instance.Family.Kind[_0024STATIC_0024Kinder_00242001_0024K] != 0)
                        {
                            _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(_Modul1.Instance.Family.Kind[_0024STATIC_0024Kinder_00242001_0024K].AsString()));
                            _Modul1.Instance.Datschalt = 10;
                            short Listart = 0;
                            bool neb = false;
                            _Modul1.Instance.Datles3(Listart, default, default, ref neb);
                            if (_Modul1.Instance.Kont[12].Trim() == "")
                            {
                                _Modul1.Instance.Kont[12] = "        ";
                            }
                            if (_Modul1.Instance.Kont[11].AsDouble() > 0.0)
                            {
                                List1.Items.Add(_Modul1.Instance.Kont[11] + "          " + _Modul1.Instance.PersInArb.AsString());
                            }
                            else
                            {
                                List1.Items.Add(_Modul1.Instance.Kont[12] + "          " + _Modul1.Instance.PersInArb.AsString());
                            }
                            _0024STATIC_0024Kinder_00242001_0024K = (byte)unchecked((uint)(_0024STATIC_0024Kinder_00242001_0024K + 1));
                            if (unchecked(_0024STATIC_0024Kinder_00242001_0024K) > 99u)
                            {
                                break;
                            }
                        }
                        if (_0024STATIC_0024Kinder_00242001_0024K > 1)
                        {
                            _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(List4.Items[_Kinder_I].AsString()));
                            _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                            DataModul.DT_AncesterTable.Index = "Pernr";
                            DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.PersInArb);
                            if (!DataModul.DT_AncesterTable.NoMatch)
                            {
                                Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                                Ahnes = Strings.Right("00" + DataModul.DT_AncesterTable.Fields["Gen"].AsString().Trim(), 2) + Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                                RichTextBox1[0].SelectionIndent = 0;
                                RichTextBox1[0].SelectedText = "Kinder von \n";
                                _Modul1.Instance.Ind1 = "";
                                RichTextBox1[0].SelectionIndent = (int)Math.Round(0.0 * 1440.0 / DeviceDpi);
                                RichTextBox1[0].SelectionHangingIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5);
                                RichTextBox1[0].SelectionFont = RichTextBox1[0].SelectionFont.ChangeFName("Courier");
                                RichTextBox1[0].SelectionFont = RichTextBox1[0].SelectionFont.ChangeFSize(10f);
                                RichTextBox1[0].SelectedText = "\n" + Ahnes;
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.Kont[3];
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[1] + " ";
                                }
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                text = Ahne;
                                if (_Modul1.Instance.PersInArb == _Modul1.Instance.Family.Mann)
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Frau;
                                }
                                else
                                {
                                    _Modul1.Instance.PersInArb = _Modul1.Instance.Family.Mann;
                                }
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                DataModul.DT_AncesterTable.Index = "Pernr";
                                DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.PersInArb);
                                if (!DataModul.DT_AncesterTable.NoMatch)
                                {
                                    Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                                    Ahnes = Strings.Right("00" + DataModul.DT_AncesterTable.Fields["Gen"].AsString().Trim(), 2) + Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                                }
                                else
                                {
                                    Ahnes = "          ";
                                }
                                RichTextBox1[0].SelectedText = " und ";
                                RichTextBox1[0].SelectionIndent = 0;
                                RichTextBox1[0].SelectionHangingIndent = (int)Math.Round(M_KontLen * 175 * 1440.0 / DeviceDpi);
                                RichTextBox1[0].SelectionFont = RichTextBox1[0].SelectionFont.ChangeFName("Courier");
                                RichTextBox1[0].SelectionFont = RichTextBox1[0].SelectionFont.ChangeFSize(10f);
                                RichTextBox1[0].SelectedText = "\n" + Ahnes;
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[0].SelectedText = "  " + _Modul1.Instance.Kont[3];
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    RichTextBox1[0].SelectedText = _Modul1.Instance.Kont[1] + " ";
                                }
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Bold);
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[0];
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                RichTextBox1[0].SelectedText = "\n";
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                            }
                            byte b2 = (byte)(List1.Items.Count - 1);
                            _0024STATIC_0024Kinder_00242001_0024K = 0;
                            while (unchecked(_0024STATIC_0024Kinder_00242001_0024K <= (uint)b2))
                            {
                                _Modul1.Instance.PersInArb = (int)Math.Round(Conversion.Val(Strings.Mid(List1.Items[_0024STATIC_0024Kinder_00242001_0024K].AsString(), 10, List1.Items[_0024STATIC_0024Kinder_00242001_0024K].AsString().Length)));
                                _Modul1.Instance.Person_ReadNames(_Modul1.Instance.PersInArb, _Modul1.Instance.Person);
                                b = (byte)(unchecked(b) + 1);
                                RichTextBox1[0].SelectionIndent = 0;
                                RichTextBox1[0].SelectionHangingIndent = (int)Math.Round(M_KontLen * 175 * 1440.0 / DeviceDpi);
                                RichTextBox1[0].SelectionFont = RichTextBox1[0].SelectionFont.ChangeFName("Courier");
                                RichTextBox1[0].SelectionFont = RichTextBox1[0].SelectionFont.ChangeFSize(10f);
                                RichTextBox1[0].SelectedText = "\n  " + text + "." + b.AsString().Trim();
                                _Modul1.Instance.Ind1 = text;
                                RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                               M_Namen = _Modul1.Instance.Kont[0];
                                if (_Modul1.Instance.Kont[1] != "")
                                {
                                    _Modul1.Instance.Kont[0] = _Modul1.Instance.Kont[1] + " " + _Modul1.Instance.Kont[0];
                                }
                                RichTextBox1[0].SelectedText = " " + _Modul1.Instance.Kont[3] + " " + _Modul1.Instance.Kont[0];
                                DataModul.DT_AncesterTable.Index = "Pernr";
                                DataModul.DT_AncesterTable.Seek("=", _Modul1.Instance.PersInArb);
                                if (!DataModul.DT_AncesterTable.NoMatch)
                                {
                                    Ahne = Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                                    Ahnes = Strings.Right("00" + DataModul.DT_AncesterTable.Fields["Gen"].AsString().Trim(), 2) + Strings.Right(new string(' ', 40) + DataModul.DT_AncesterTable.Fields["Ahn"].AsString().Trim(), M_KontLen);
                                    RichTextBox1[0].SelectedText = " siehe " + Ahnes + "\n";
                                }
                                else
                                {
                                    RichTextBox1[0].SelectedText = "\n";
                                    RichTextBox1[0].SelectionIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5 + 1.0);
                                    _Modul1.Instance.Datschalt = 0;
                                    _Modul1.Instance.Datles4();
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                    RichTextBox1[0].SelectionIndent = (int)Math.Round(unchecked(M_KontLen) * 17.5 + 1.0);
                                    if (_Modul1.Instance.Kont[11] != "")
                                    {
                                        RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[1] + " " + _Modul1.Instance.Kont[11] + " ";
                                    }
                                    if (_Modul1.Instance.Kont[12] != "")
                                    {
                                        RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[2] + " " + _Modul1.Instance.Kont[12];
                                    }
                                    if (_Modul1.Instance.Kont[13] != "" | _Modul1.Instance.Kont[14] != "")
                                    {
                                        RichTextBox1[0].SelectedText = "\n";
                                    }
                                    if (_Modul1.Instance.Kont[13] != "")
                                    {
                                        RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[3] + " " + _Modul1.Instance.Kont[13] + " ";
                                    }
                                    if (_Modul1.Instance.Kont[14] != "")
                                    {
                                        RichTextBox1[0].SelectedText = _Modul1.Instance.DTxt[4] + " " + _Modul1.Instance.Kont[14];
                                    }
                                    RichTextBox1[0].SelectedText = "\n";
                                    RichTextBox1[0].SelectionFont = new Font(_Modul1.Instance.DAus[101], (float)_Modul1.Instance.DAus[102].AsDouble(), FontStyle.Regular);
                                }
                                _0024STATIC_0024Kinder_00242001_0024K = (byte)unchecked((uint)(_0024STATIC_0024Kinder_00242001_0024K + 1));
                            }
                        }
                        _0024STATIC_0024Kinder_00242001_0024Fa1++;
                        continue;
                    end_IL_1094:
                        break;
                    }
                }
            }
        }

        private void _Command_1_Click(object sender, EventArgs e)
        {
        }
    }
}
