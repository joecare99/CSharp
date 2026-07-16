using GenFree.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public partial class Hinter
{

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {
        try
        {

        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    public SaveFileDialog CommonDialog1Save;

    internal GroupBox Frame4;
    internal Label Label2;
    internal Label Label1;

    /*
             btnBack.Click += new EventHandler(btnNewEntry_Click);
             btnMainmenue.Click += new EventHandler(Button1_Click_1);
             _Button15.Click += new EventHandler(OpenCheckMissing);
             _Button14.Click += new EventHandler(OpenCheckFamilies);
             _Button13.Click += new EventHandler(OpenConfig);
             _Button12.Click += new EventHandler(Button12_Click_1);
             _DirListBox1.DoubleClick += new EventHandler(DirListBox1_DoubleClick1);
             _DirListBox1.Change += new EventHandler(DirListBox1_Change);
            _DriveListBox1.SelectedIndexChanged += new EventHandler(DriveListBox1_SelectedIndexChanged);
            _FileListBox1.DoubleClick += new EventHandler(FileListBox1_DoubleClick1);
            btnNextEntry.Click += new EventHandler(btnMoveToCause_Click);
            btnPrevEntry.Click += new EventHandler(btnChangeSexToM_Click);
            _List1.DoubleClick += new EventHandler(List1_DoubleClick);
            btnSearch.Click += new EventHandler(btnChangeSexToF_Click);
            btnSearchNumber.Click += new EventHandler(btnMoveToLowerDateAnot_Click);
            btnSearchName.Click += new EventHandler(btnMoveToEntityAnot_Click);
            btnEnterNew2.Click += new EventHandler(btnMoveToChurchCemet_Click);
            btnShowUsage.Click += new EventHandler(btnReenter_Click);
            _CheckBox10.Click += new EventHandler(CheckBox10_Click);
            _Button10.Click += new EventHandler(Button10_Click_1);
            _RadioButton6.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton5.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton4.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton7.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _Button18.Click += new EventHandler(OpenNotes);
            _RadioButton9.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton8.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton12.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton11.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton10.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton22.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton23.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton24.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton25.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton26.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton27.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton28.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton29.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton30.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton13.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton14.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton15.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton16.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton17.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton18.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton19.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton20.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
            _RadioButton21.CheckedChanged += new EventHandler(RadioButton4_CheckedChanged);
             */

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.Frame4 = new System.Windows.Forms.GroupBox();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.RadioButton32 = new System.Windows.Forms.RadioButton();
            this.RadioButton31 = new System.Windows.Forms.RadioButton();
            this.CheckBox4 = new System.Windows.Forms.CheckBox();
            this.Frame2 = new System.Windows.Forms.GroupBox();
            this.Button4 = new System.Windows.Forms.Button();
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.DirListBox1 = new System.Windows.Forms.ListBox();
        this.FileListBox1 = new System.Windows.Forms.ListBox();
            this.DriveListBox1 = new System.Windows.Forms.ListBox();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.WebBrowser1 = new System.Windows.Forms.WebBrowser();
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.Button10 = new System.Windows.Forms.Button();
            this.CheckBox10 = new System.Windows.Forms.CheckBox();
            this.Sortlist = new System.Windows.Forms.ListBox();
            this.Button5 = new System.Windows.Forms.Button();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.Button15 = new System.Windows.Forms.Button();
            this.Button14 = new System.Windows.Forms.Button();
            this.Button13 = new System.Windows.Forms.Button();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.Button12 = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Button3 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.Frame3 = new System.Windows.Forms.GroupBox();
            this.CheckBox3 = new System.Windows.Forms.CheckBox();
            this.TextBox5 = new System.Windows.Forms.TextBox();
            this.TextBox4 = new System.Windows.Forms.TextBox();
            this.TextBox3 = new System.Windows.Forms.TextBox();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Frame6 = new System.Windows.Forms.GroupBox();
            this.CheckBox5 = new System.Windows.Forms.CheckBox();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.TextBox7 = new System.Windows.Forms.TextBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.Option2 = new System.Windows.Forms.RadioButton();
            this.Option1 = new System.Windows.Forms.RadioButton();
            this.TextBox6 = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.CheckBox9 = new System.Windows.Forms.CheckBox();
            this.CheckBox8 = new System.Windows.Forms.CheckBox();
            this.CheckBox7 = new System.Windows.Forms.CheckBox();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.Button9 = new System.Windows.Forms.Button();
            this.Button8 = new System.Windows.Forms.Button();
            this.Button7 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.List1 = new System.Windows.Forms.ListBox();
            this.RTB = new System.Windows.Forms.RichTextBox();
            this.Label53 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.RadioButton2 = new System.Windows.Forms.RadioButton();
            this.RadioButton3 = new System.Windows.Forms.RadioButton();
            this.Button11 = new System.Windows.Forms.Button();
            this.Button16 = new System.Windows.Forms.Button();
            this.Button17 = new System.Windows.Forms.Button();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.fraHntSearchFields1 = new GenFreeWin.Views.FraHntSearchFields();
            this.Frame4.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.Frame2.SuspendLayout();
            this.Frame1.SuspendLayout();
            this.Frame3.SuspendLayout();
            this.Frame6.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Frame4
            // 
            this.Frame4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Frame4.Controls.Add(this.GroupBox7);
            this.Frame4.Controls.Add(this.CheckBox4);
            this.Frame4.Controls.Add(this.Frame2);
            this.Frame4.Controls.Add(this.CheckBox1);
            this.Frame4.Controls.Add(this.Label18);
            this.Frame4.Controls.Add(this.Label3);
            this.Frame4.Controls.Add(this.WebBrowser1);
            this.Frame4.Controls.Add(this.ProgressBar1);
            this.Frame4.Controls.Add(this.Button10);
            this.Frame4.Controls.Add(this.CheckBox10);
            this.Frame4.Controls.Add(this.Sortlist);
            this.Frame4.Controls.Add(this.Button5);
            this.Frame4.Controls.Add(this.Frame1);
            this.Frame4.Controls.Add(this.Label2);
            this.Frame4.Controls.Add(this.Label1);
            this.Frame4.Controls.Add(this.Button3);
            this.Frame4.Controls.Add(this.Button2);
            this.Frame4.Controls.Add(this.Button1);
            this.Frame4.Controls.Add(this.Frame3);
            this.Frame4.Controls.Add(this.Frame6);
            this.Frame4.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame4.Location = new System.Drawing.Point(119, 191);
            this.Frame4.Name = "Frame4";
            this.Frame4.Size = new System.Drawing.Size(1024, 666);
            this.Frame4.TabIndex = 21;
            this.Frame4.TabStop = false;
            this.Frame4.Visible = false;
            // 
            // GroupBox7
            // 
            this.GroupBox7.Controls.Add(this.RadioButton32);
            this.GroupBox7.Controls.Add(this.RadioButton31);
            this.GroupBox7.Location = new System.Drawing.Point(9, 409);
            this.GroupBox7.Name = "GroupBox7";
            this.GroupBox7.Size = new System.Drawing.Size(200, 81);
            this.GroupBox7.TabIndex = 49;
            this.GroupBox7.TabStop = false;
            this.GroupBox7.Text = "Sortierung sonst. Datum";
            // 
            // RadioButton32
            // 
            this.RadioButton32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton32.Location = new System.Drawing.Point(6, 47);
            this.RadioButton32.Name = "RadioButton32";
            this.RadioButton32.Size = new System.Drawing.Size(181, 21);
            this.RadioButton32.TabIndex = 1;
            this.RadioButton32.Text = "Nach Art und Datum";
            this.RadioButton32.UseVisualStyleBackColor = false;
            // 
            // RadioButton31
            // 
            this.RadioButton31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton31.Checked = true;
            this.RadioButton31.Location = new System.Drawing.Point(6, 20);
            this.RadioButton31.Name = "RadioButton31";
            this.RadioButton31.Size = new System.Drawing.Size(180, 23);
            this.RadioButton31.TabIndex = 0;
            this.RadioButton31.TabStop = true;
            this.RadioButton31.Text = "Nach Datum";
            this.RadioButton31.UseVisualStyleBackColor = false;
            // 
            // CheckBox4
            // 
            this.CheckBox4.BackColor = System.Drawing.Color.Lime;
            this.CheckBox4.Location = new System.Drawing.Point(9, 153);
            this.CheckBox4.Name = "CheckBox4";
            this.CheckBox4.Size = new System.Drawing.Size(268, 42);
            this.CheckBox4.TabIndex = 47;
            this.CheckBox4.Text = "Quelle automatisch um >Seite:< ergänzen";
            this.CheckBox4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckBox4.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.CheckBox4.UseVisualStyleBackColor = false;
            // 
            // Frame2
            // 
            this.Frame2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Frame2.Controls.Add(this.Button4);
            this.Frame2.Controls.Add(this.DirListBox1);
            this.Frame2.Controls.Add(this.FileListBox1);
            this.Frame2.Controls.Add(this.DriveListBox1);
            this.Frame2.Location = new System.Drawing.Point(704, 0);
            this.Frame2.Name = "Frame2";
            this.Frame2.Size = new System.Drawing.Size(314, 528);
            this.Frame2.TabIndex = 31;
            this.Frame2.TabStop = false;
            this.Frame2.Visible = false;
            // 
            // btnMoveToCause
            // 
            this.Button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button4.Location = new System.Drawing.Point(31, 484);
            this.Button4.Name = "btnShowPlaceGM";
            this.Button4.Size = new System.Drawing.Size(95, 25);
            this.Button4.TabIndex = 37;
            this.Button4.Text = "Abbruch";
            this.Button4.UseVisualStyleBackColor = false;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // DirListBox1
            // 
            this.DirListBox1.FormattingEnabled = true;
            this.DirListBox1.HorizontalScrollbar = true;
            this.DirListBox1.IntegralHeight = false;
            this.DirListBox1.Location = new System.Drawing.Point(12, 50);
            this.DirListBox1.Name = "DirListBox1";
            this.DirListBox1.Size = new System.Drawing.Size(296, 199);
            this.DirListBox1.TabIndex = 32;
            this.DirListBox1.SelectedValueChanged += new System.EventHandler(this.DirListBox1_Change);
            this.DirListBox1.DoubleClick += new System.EventHandler(this.DirListBox1_DoubleClick1);
            // 
            // FileListBox1
            // 
            this.FileListBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FileListBox1.FormattingEnabled = true;
            this.FileListBox1.HorizontalScrollbar = true;
            this.FileListBox1.Location = new System.Drawing.Point(12, 255);
            this.FileListBox1.Name = "FileListBox1";
            this.FileListBox1.Size = new System.Drawing.Size(296, 192);
            this.FileListBox1.TabIndex = 36;
            this.FileListBox1.Tag = "*.EXE";
            this.FileListBox1.DoubleClick += new System.EventHandler(this.FileListBox1_DoubleClick1);
            // 
            // DriveListBox1
            // 
            this.DriveListBox1.FormattingEnabled = true;
            this.DriveListBox1.Location = new System.Drawing.Point(12, 21);
            this.DriveListBox1.Name = "DriveListBox1";
            this.DriveListBox1.Size = new System.Drawing.Size(296, 28);
            this.DriveListBox1.TabIndex = 35;
            this.DriveListBox1.SelectedIndexChanged += new System.EventHandler(this.DriveListBox1_SelectedIndexChanged);
            // 
            // CheckBox1
            // 
            this.CheckBox1.BackColor = System.Drawing.Color.Lime;
            this.CheckBox1.Location = new System.Drawing.Point(6, 68);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(268, 21);
            this.CheckBox1.TabIndex = 45;
            this.CheckBox1.Text = "automatische Aktualitätskontrolle ein";
            this.CheckBox1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.CheckBox1.UseVisualStyleBackColor = false;
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Location = new System.Drawing.Point(473, 509);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(66, 19);
            this.Label18.TabIndex = 44;
            this.Label18.Text = "Label18";
            // 
            // lblDisplayHint
            // 
            this.Label3.Location = new System.Drawing.Point(298, 509);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(162, 17);
            this.Label3.TabIndex = 43;
            this.Label3.Text = "lblDisplayHint";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WebBrowser1
            // 
            this.WebBrowser1.Location = new System.Drawing.Point(6, 547);
            this.WebBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser1.Name = "WebBrowser1";
            this.WebBrowser1.Size = new System.Drawing.Size(195, 112);
            this.WebBrowser1.TabIndex = 42;
            this.WebBrowser1.Visible = false;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(30, 534);
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(919, 23);
            this.ProgressBar1.TabIndex = 41;
            // 
            // btnDeleteEntry
            // 
            this.Button10.BackColor = System.Drawing.Color.Lime;
            this.Button10.Location = new System.Drawing.Point(266, 441);
            this.Button10.Name = "Button10";
            this.Button10.Size = new System.Drawing.Size(435, 65);
            this.Button10.TabIndex = 40;
            this.Button10.Text = "Interne Liste wiederbelegbarer Datensätze schreiben.\r\nIst erforderlich, um gelösc" +
    "hte Personen- und Familiennummern neu zu belegen.";
            this.Button10.UseVisualStyleBackColor = false;
            this.Button10.Click += new System.EventHandler(this.Button10_Click_1);
            // 
            // CheckBox10
            // 
            this.CheckBox10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.CheckBox10.Location = new System.Drawing.Point(266, 415);
            this.CheckBox10.Name = "CheckBox10";
            this.CheckBox10.Size = new System.Drawing.Size(435, 20);
            this.CheckBox10.TabIndex = 39;
            this.CheckBox10.Text = "Personen und Familiennummern wieder neu belegen";
            this.CheckBox10.UseVisualStyleBackColor = false;
            this.CheckBox10.Click += new System.EventHandler(this.CheckBox10_Click);
            // 
            // Sortlist
            // 
            this.Sortlist.ColumnWidth = 50;
            this.Sortlist.FormattingEnabled = true;
            this.Sortlist.ItemHeight = 19;
            this.Sortlist.Location = new System.Drawing.Point(6, 534);
            this.Sortlist.MultiColumn = true;
            this.Sortlist.Name = "Sortlist";
            this.Sortlist.Size = new System.Drawing.Size(271, 42);
            this.Sortlist.TabIndex = 25;
            this.Sortlist.Visible = false;
            // 
            // btnChangeSexToM
            // 
            this.Button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button5.Location = new System.Drawing.Point(827, 563);
            this.Button5.Name = "btnLinkGOV";
            this.Button5.Size = new System.Drawing.Size(141, 27);
            this.Button5.TabIndex = 33;
            this.Button5.UseVisualStyleBackColor = false;
            this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Frame1.Controls.Add(this.Button15);
            this.Frame1.Controls.Add(this.Button14);
            this.Frame1.Controls.Add(this.Button13);
            this.Frame1.Controls.Add(this.ListBox1);
            this.Frame1.Controls.Add(this.Button12);
            this.Frame1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame1.Location = new System.Drawing.Point(9, 201);
            this.Frame1.Name = "Frame1";
            this.Frame1.Size = new System.Drawing.Size(251, 145);
            this.Frame1.TabIndex = 26;
            this.Frame1.TabStop = false;
            this.Frame1.Text = "Farbeinstellungen";
            // 
            // Button15
            // 
            this.Button15.BackColor = System.Drawing.Color.Lime;
            this.Button15.Location = new System.Drawing.Point(11, 107);
            this.Button15.Name = "Button15";
            this.Button15.Size = new System.Drawing.Size(234, 25);
            this.Button15.TabIndex = 3;
            this.Button15.Text = "Standard wieder herstellen";
            this.Button15.UseVisualStyleBackColor = false;
            this.Button15.Click += new System.EventHandler(this.Button15_Click);
            // 
            // Button14
            // 
            this.Button14.BackColor = System.Drawing.Color.Lime;
            this.Button14.Location = new System.Drawing.Point(11, 79);
            this.Button14.Name = "Button14";
            this.Button14.Size = new System.Drawing.Size(234, 25);
            this.Button14.TabIndex = 2;
            this.Button14.Text = "Eingabe- und Anzeigefenster";
            this.Button14.UseVisualStyleBackColor = false;
            this.Button14.Click += new System.EventHandler(this.Button14_Click);
            // 
            // Button13
            // 
            this.Button13.BackColor = System.Drawing.Color.Lime;
            this.Button13.Location = new System.Drawing.Point(11, 51);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(234, 25);
            this.Button13.TabIndex = 1;
            this.Button13.Text = "Ereignismaske";
            this.Button13.UseVisualStyleBackColor = false;
            this.Button13.Click += new System.EventHandler(this.Button13_Click);
            // 
            // lstUsageList
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 19;
            this.ListBox1.Location = new System.Drawing.Point(144, 21);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(260, 346);
            this.ListBox1.Sorted = true;
            this.ListBox1.TabIndex = 27;
            this.ListBox1.Visible = false;
            // 
            // Button12
            // 
            this.Button12.BackColor = System.Drawing.Color.Lime;
            this.Button12.Location = new System.Drawing.Point(11, 23);
            this.Button12.Name = "Button12";
            this.Button12.Size = new System.Drawing.Size(234, 25);
            this.Button12.TabIndex = 0;
            this.Button12.Text = "Hintergrund";
            this.Button12.UseVisualStyleBackColor = false;
            this.Button12.Click += new System.EventHandler(this.Button12_Click_1);
            // 
            // lblState
            // 
            this.Label2.AutoEllipsis = true;
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(9, 47);
            this.Label2.MinimumSize = new System.Drawing.Size(24, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(268, 18);
            this.Label2.TabIndex = 24;
            // 
            // lblEnterLicence
            // 
            this.Label1.AutoEllipsis = true;
            this.Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label1.Location = new System.Drawing.Point(3, 382);
            this.Label1.MinimumSize = new System.Drawing.Size(24, 0);
            this.Label1.Name = "lblRepoName";
            this.Label1.Size = new System.Drawing.Size(271, 19);
            this.Label1.TabIndex = 23;
            this.Label1.Visible = false;
            // 
            // btnDuplBttn3
            // 
            this.Button3.BackColor = System.Drawing.Color.Lime;
            this.Button3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button3.Location = new System.Drawing.Point(5, 352);
            this.Button3.Name = "btnShowPlaceGE";
            this.Button3.Size = new System.Drawing.Size(261, 27);
            this.Button3.TabIndex = 22;
            this.Button3.Text = "Word als Textverarbeitung einstellen";
            this.Button3.UseVisualStyleBackColor = false;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // btnRegisterSearch
            // 
            this.Button2.BackColor = System.Drawing.Color.Lime;
            this.Button2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button2.Location = new System.Drawing.Point(6, 90);
            this.Button2.Name = "btnPrev";
            this.Button2.Size = new System.Drawing.Size(271, 57);
            this.Button2.TabIndex = 21;
            this.Button2.Text = "Mandantenspezifische Einstellungen der Suchfelder";
            this.Button2.UseVisualStyleBackColor = false;
            this.Button2.Visible = false;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // btnReqHint
            // 
            this.Button1.BackColor = System.Drawing.Color.Lime;
            this.Button1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button1.Location = new System.Drawing.Point(6, 17);
            this.Button1.Name = "btnNext";
            this.Button1.Size = new System.Drawing.Size(271, 27);
            this.Button1.TabIndex = 20;
            this.Button1.Text = "Standardtextverarbeitung einstellen";
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // Frame3
            // 
            this.Frame3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Frame3.Controls.Add(this.CheckBox3);
            this.Frame3.Controls.Add(this.TextBox5);
            this.Frame3.Controls.Add(this.TextBox4);
            this.Frame3.Controls.Add(this.TextBox3);
            this.Frame3.Controls.Add(this.TextBox2);
            this.Frame3.Controls.Add(this.Label13);
            this.Frame3.Controls.Add(this.Label12);
            this.Frame3.Controls.Add(this.Label11);
            this.Frame3.Controls.Add(this.Label10);
            this.Frame3.Controls.Add(this.Label9);
            this.Frame3.Controls.Add(this.Label8);
            this.Frame3.Controls.Add(this.TextBox1);
            this.Frame3.Controls.Add(this.Label7);
            this.Frame3.Location = new System.Drawing.Point(283, 21);
            this.Frame3.Name = "Frame3";
            this.Frame3.Size = new System.Drawing.Size(415, 164);
            this.Frame3.TabIndex = 32;
            this.Frame3.TabStop = false;
            this.Frame3.Text = "Prüfkriterien Familienprüfung";
            // 
            // CheckBox3
            // 
            this.CheckBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox3.Location = new System.Drawing.Point(6, 138);
            this.CheckBox3.Name = "CheckBox3";
            this.CheckBox3.Size = new System.Drawing.Size(403, 20);
            this.CheckBox3.TabIndex = 12;
            this.CheckBox3.Text = "kein Hinweis auf Familien mit ausgeschalteter Prüfung";
            this.CheckBox3.UseVisualStyleBackColor = false;
            // 
            // edtState
            // 
            this.TextBox5.Location = new System.Drawing.Point(125, 113);
            this.TextBox5.Name = "edtState";
            this.TextBox5.Size = new System.Drawing.Size(36, 27);
            this.TextBox5.TabIndex = 11;
            // 
            // edtCountry
            // 
            this.TextBox4.Location = new System.Drawing.Point(6, 90);
            this.TextBox4.Name = "edtCountry";
            this.TextBox4.Size = new System.Drawing.Size(36, 27);
            this.TextBox4.TabIndex = 10;
            // 
            // edtCounty
            // 
            this.TextBox3.Location = new System.Drawing.Point(6, 65);
            this.TextBox3.Name = "edtCounty";
            this.TextBox3.Size = new System.Drawing.Size(36, 27);
            this.TextBox3.TabIndex = 9;
            // 
            // edtSuburb
            // 
            this.TextBox2.Location = new System.Drawing.Point(204, 40);
            this.TextBox2.Name = "edtSuburb";
            this.TextBox2.Size = new System.Drawing.Size(36, 27);
            this.TextBox2.TabIndex = 8;
            // 
            // lblPredicate
            // 
            this.Label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label13.Location = new System.Drawing.Point(6, 115);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(113, 20);
            this.Label13.TabIndex = 7;
            this.Label13.Text = "Kind max.";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label12
            // 
            this.Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label12.Location = new System.Drawing.Point(167, 113);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(242, 20);
            this.Label12.TabIndex = 6;
            this.Label12.Text = "Jahre vor der Heirat";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label11
            // 
            this.Label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label11.Location = new System.Drawing.Point(45, 91);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(364, 20);
            this.Label11.TabIndex = 5;
            this.Label11.Text = "Jahre max. Alter der Frau beim letzten Kind";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label10
            // 
            this.Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label10.Location = new System.Drawing.Point(48, 67);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(361, 20);
            this.Label10.TabIndex = 4;
            this.Label10.Text = "Jahre max. Altersdifferenz Mann / Frau";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOccubation
            // 
            this.Label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label9.Location = new System.Drawing.Point(246, 42);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(163, 20);
            this.Label9.TabIndex = 3;
            this.Label9.Text = "Jahre Frau";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblResidence
            // 
            this.Label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label8.Location = new System.Drawing.Point(45, 42);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(142, 20);
            this.Label8.TabIndex = 2;
            this.Label8.Text = "Jahre Mann";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // edtPredicate
            // 
            this.TextBox1.Location = new System.Drawing.Point(6, 41);
            this.TextBox1.Name = "edtPlace";
            this.TextBox1.Size = new System.Drawing.Size(36, 27);
            this.TextBox1.TabIndex = 1;
            // 
            // lblURL
            // 
            this.Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label7.Location = new System.Drawing.Point(6, 18);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(403, 20);
            this.Label7.TabIndex = 0;
            this.Label7.Text = "Mindestalter bei der Heirat";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Frame6
            // 
            this.Frame6.BackColor = System.Drawing.Color.Silver;
            this.Frame6.Controls.Add(this.CheckBox5);
            this.Frame6.Controls.Add(this.CheckBox2);
            this.Frame6.Controls.Add(this.TextBox7);
            this.Frame6.Controls.Add(this.Label19);
            this.Frame6.Controls.Add(this.Option2);
            this.Frame6.Controls.Add(this.Option1);
            this.Frame6.Controls.Add(this.TextBox6);
            this.Frame6.Controls.Add(this.Label15);
            this.Frame6.Controls.Add(this.Label14);
            this.Frame6.Controls.Add(this.CheckBox9);
            this.Frame6.Controls.Add(this.CheckBox8);
            this.Frame6.Controls.Add(this.CheckBox7);
            this.Frame6.Location = new System.Drawing.Point(266, 185);
            this.Frame6.Name = "Frame6";
            this.Frame6.Size = new System.Drawing.Size(435, 224);
            this.Frame6.TabIndex = 36;
            this.Frame6.TabStop = false;
            // 
            // CheckBox5
            // 
            this.CheckBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox5.Location = new System.Drawing.Point(6, 198);
            this.CheckBox5.Name = "CheckBox5";
            this.CheckBox5.Size = new System.Drawing.Size(429, 20);
            this.CheckBox5.TabIndex = 11;
            this.CheckBox5.Text = "Nur Personenbild in der Personenmaske";
            this.CheckBox5.UseVisualStyleBackColor = false;
            // 
            // CheckBox2
            // 
            this.CheckBox2.AutoSize = true;
            this.CheckBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox2.Location = new System.Drawing.Point(5, 120);
            this.CheckBox2.Name = "CheckBox2";
            this.CheckBox2.Size = new System.Drawing.Size(356, 23);
            this.CheckBox2.TabIndex = 10;
            this.CheckBox2.Text = "Bei Suchliste Namenauswahlliste einblenden";
            this.CheckBox2.UseVisualStyleBackColor = false;
            // 
            // edtLat1
            // 
            this.TextBox7.Location = new System.Drawing.Point(193, 173);
            this.TextBox7.Multiline = true;
            this.TextBox7.Name = "edtLat1";
            this.TextBox7.Size = new System.Drawing.Size(60, 20);
            this.TextBox7.TabIndex = 9;
            // 
            // Label19
            // 
            this.Label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label19.Location = new System.Drawing.Point(6, 173);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(181, 20);
            this.Label19.TabIndex = 8;
            this.Label19.Text = "Wochentage anzeigen ab:";
            // 
            // Option2
            // 
            this.Option2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Option2.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Option2.Location = new System.Drawing.Point(215, 147);
            this.Option2.Name = "Option2";
            this.Option2.Size = new System.Drawing.Size(118, 20);
            this.Option2.TabIndex = 7;
            this.Option2.TabStop = true;
            this.Option2.Text = "mit Datum";
            this.Option2.UseVisualStyleBackColor = false;
            // 
            // Option1
            // 
            this.Option1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Option1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Option1.Location = new System.Drawing.Point(6, 147);
            this.Option1.Name = "Option1";
            this.Option1.Size = new System.Drawing.Size(207, 20);
            this.Option1.TabIndex = 6;
            this.Option1.TabStop = true;
            this.Option1.Text = "Wohnorte starten mit Ort";
            this.Option1.UseVisualStyleBackColor = false;
            // 
            // edtLocator
            // 
            this.TextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox6.Location = new System.Drawing.Point(259, 96);
            this.TextBox6.Name = "edtLocator";
            this.TextBox6.Size = new System.Drawing.Size(43, 20);
            this.TextBox6.TabIndex = 5;
            // 
            // Label15
            // 
            this.Label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label15.Location = new System.Drawing.Point(308, 96);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(104, 20);
            this.Label15.TabIndex = 4;
            this.Label15.Text = "Einträge";
            // 
            // lblNickName
            // 
            this.Label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label14.Location = new System.Drawing.Point(6, 96);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(247, 20);
            this.Label14.TabIndex = 3;
            this.Label14.Text = "Bildschirmanzeige Suchliste max.";
            // 
            // CheckBox9
            // 
            this.CheckBox9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox9.Location = new System.Drawing.Point(6, 70);
            this.CheckBox9.Name = "CheckBox9";
            this.CheckBox9.Size = new System.Drawing.Size(429, 20);
            this.CheckBox9.TabIndex = 2;
            this.CheckBox9.Text = "Alter ausdrucken bei fehlendem Sterbedatum (bis 120 Jahre)";
            this.CheckBox9.UseVisualStyleBackColor = false;
            // 
            // CheckBox8
            // 
            this.CheckBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox8.Location = new System.Drawing.Point(6, 47);
            this.CheckBox8.Name = "CheckBox8";
            this.CheckBox8.Size = new System.Drawing.Size(429, 20);
            this.CheckBox8.TabIndex = 1;
            this.CheckBox8.Text = "Leitnamen ausgeben";
            this.CheckBox8.UseVisualStyleBackColor = false;
            // 
            // CheckBox7
            // 
            this.CheckBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox7.Location = new System.Drawing.Point(6, 21);
            this.CheckBox7.Name = "CheckBox7";
            this.CheckBox7.Size = new System.Drawing.Size(429, 20);
            this.CheckBox7.TabIndex = 0;
            this.CheckBox7.Text = "Dublettenkontrolle";
            this.CheckBox7.UseVisualStyleBackColor = false;
            // 
            // GroupBoxUsage
            // 
            this.GroupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.GroupBox1.Controls.Add(this.Label16);
            this.GroupBox1.Controls.Add(this.Button9);
            this.GroupBox1.Controls.Add(this.Button8);
            this.GroupBox1.Controls.Add(this.Button7);
            this.GroupBox1.Controls.Add(this.Button6);
            this.GroupBox1.Controls.Add(this.List1);
            this.GroupBox1.Controls.Add(this.RTB);
            this.GroupBox1.Controls.Add(this.Label53);
            this.GroupBox1.Controls.Add(this.Label17);
            this.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.GroupBox1.Location = new System.Drawing.Point(322, 65);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.GroupBox1.Size = new System.Drawing.Size(413, 588);
            this.GroupBox1.TabIndex = 26;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Visible = false;
            // 
            // Label16
            // 
            this.Label16.BackColor = System.Drawing.Color.White;
            this.Label16.Location = new System.Drawing.Point(12, 86);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(378, 24);
            this.Label16.TabIndex = 81;
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnMoveToLowerDateAnot
            // 
            this.Button9.BackColor = System.Drawing.Color.Lime;
            this.Button9.Location = new System.Drawing.Point(287, 539);
            this.Button9.Name = "btnSearchNumber";
            this.Button9.Size = new System.Drawing.Size(105, 28);
            this.Button9.TabIndex = 80;
            this.Button9.UseVisualStyleBackColor = false;
            this.Button9.Click += new System.EventHandler(this.Button9_Click);
            // 
            // btnMoveToEntityAnot
            // 
            this.Button8.BackColor = System.Drawing.Color.Lime;
            this.Button8.Location = new System.Drawing.Point(185, 539);
            this.Button8.Name = "btnSearchName";
            this.Button8.Size = new System.Drawing.Size(85, 28);
            this.Button8.TabIndex = 79;
            this.Button8.Text = "Speichern";
            this.Button8.UseVisualStyleBackColor = false;
            this.Button8.Click += new System.EventHandler(this.Button8_Click);
            // 
            // btnMoveToChurchCemet
            // 
            this.Button7.BackColor = System.Drawing.Color.Lime;
            this.Button7.Location = new System.Drawing.Point(86, 539);
            this.Button7.Name = "btnConvertKoords";
            this.Button7.Size = new System.Drawing.Size(71, 28);
            this.Button7.TabIndex = 78;
            this.Button7.Text = "Drucken";
            this.Button7.UseVisualStyleBackColor = false;
            this.Button7.Click += new System.EventHandler(this.Button7_Click);
            // 
            // btnChangeSexToF
            // 
            this.Button6.BackColor = System.Drawing.Color.Lime;
            this.Button6.Location = new System.Drawing.Point(8, 539);
            this.Button6.Name = "btnSearchGOV";
            this.Button6.Size = new System.Drawing.Size(67, 28);
            this.Button6.TabIndex = 77;
            this.Button6.Text = "Start";
            this.Button6.UseVisualStyleBackColor = false;
            this.Button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // List1
            // 
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 19;
            this.List1.Location = new System.Drawing.Point(12, 118);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List1.Size = new System.Drawing.Size(378, 403);
            this.List1.TabIndex = 76;
            this.List1.DoubleClick += new System.EventHandler(this.List1_DoubleClick);
            // 
            // RTB
            // 
            this.RTB.BackColor = System.Drawing.Color.White;
            this.RTB.Location = new System.Drawing.Point(12, 126);
            this.RTB.Name = "RTB";
            this.RTB.Size = new System.Drawing.Size(378, 325);
            this.RTB.TabIndex = 43;
            this.RTB.Text = "";
            // 
            // lblChld04Info
            // 
            this.Label53.BackColor = System.Drawing.Color.White;
            this.Label53.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label53.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label53.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label53.Location = new System.Drawing.Point(12, 18);
            this.Label53.Name = "Label53";
            this.Label53.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label53.Size = new System.Drawing.Size(380, 24);
            this.Label53.TabIndex = 21;
            this.Label53.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Label17
            // 
            this.Label17.BackColor = System.Drawing.Color.White;
            this.Label17.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label17.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label17.Location = new System.Drawing.Point(12, 54);
            this.Label17.Name = "Label17";
            this.Label17.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label17.Size = new System.Drawing.Size(381, 24);
            this.Label17.TabIndex = 18;
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GroupBox2
            // 
            this.GroupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.GroupBox2.Controls.Add(this.RadioButton1);
            this.GroupBox2.Controls.Add(this.RadioButton2);
            this.GroupBox2.Controls.Add(this.RadioButton3);
            this.GroupBox2.Controls.Add(this.Button11);
            this.GroupBox2.Controls.Add(this.Button16);
            this.GroupBox2.Controls.Add(this.Button17);
            this.GroupBox2.Controls.Add(this.RichTextBox1);
            this.GroupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.GroupBox2.Location = new System.Drawing.Point(852, 13);
            this.GroupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.GroupBox2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.GroupBox2.Size = new System.Drawing.Size(795, 726);
            this.GroupBox2.TabIndex = 49;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "frmPicture";
            this.GroupBox2.Visible = false;
            // 
            // RadioButton1
            // 
            this.RadioButton1.BackColor = System.Drawing.SystemColors.Control;
            this.RadioButton1.Cursor = System.Windows.Forms.Cursors.Default;
            this.RadioButton1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton1.Location = new System.Drawing.Point(714, 665);
            this.RadioButton1.Margin = new System.Windows.Forms.Padding(4);
            this.RadioButton1.Name = "RadioButton1";
            this.RadioButton1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RadioButton1.Size = new System.Drawing.Size(111, 22);
            this.RadioButton1.TabIndex = 57;
            this.RadioButton1.TabStop = true;
            this.RadioButton1.Text = "Nach Autor";
            this.RadioButton1.UseVisualStyleBackColor = false;
            // 
            // RadioButton2
            // 
            this.RadioButton2.BackColor = System.Drawing.SystemColors.Control;
            this.RadioButton2.Cursor = System.Windows.Forms.Cursors.Default;
            this.RadioButton2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton2.Location = new System.Drawing.Point(409, 665);
            this.RadioButton2.Margin = new System.Windows.Forms.Padding(4);
            this.RadioButton2.Name = "RadioButton2";
            this.RadioButton2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RadioButton2.Size = new System.Drawing.Size(137, 22);
            this.RadioButton2.TabIndex = 56;
            this.RadioButton2.TabStop = true;
            this.RadioButton2.Text = "Nach \"Zitiert als\"";
            this.RadioButton2.UseVisualStyleBackColor = false;
            // 
            // RadioButton3
            // 
            this.RadioButton3.BackColor = System.Drawing.SystemColors.Control;
            this.RadioButton3.Cursor = System.Windows.Forms.Cursors.Default;
            this.RadioButton3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RadioButton3.Location = new System.Drawing.Point(579, 665);
            this.RadioButton3.Margin = new System.Windows.Forms.Padding(4);
            this.RadioButton3.Name = "RadioButton3";
            this.RadioButton3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RadioButton3.Size = new System.Drawing.Size(97, 22);
            this.RadioButton3.TabIndex = 55;
            this.RadioButton3.TabStop = true;
            this.RadioButton3.Text = "Nach Titel";
            this.RadioButton3.UseVisualStyleBackColor = false;
            // 
            // btnMoveToDateAnot
            // 
            this.Button11.BackColor = System.Drawing.SystemColors.Control;
            this.Button11.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Button11.Location = new System.Drawing.Point(298, 661);
            this.Button11.Margin = new System.Windows.Forms.Padding(4);
            this.Button11.Name = "Button11";
            this.Button11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Button11.Size = new System.Drawing.Size(65, 30);
            this.Button11.TabIndex = 54;
            this.Button11.Text = "Start";
            this.Button11.UseVisualStyleBackColor = false;
            // 
            // Button16
            // 
            this.Button16.BackColor = System.Drawing.SystemColors.Control;
            this.Button16.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button16.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Button16.Location = new System.Drawing.Point(199, 661);
            this.Button16.Margin = new System.Windows.Forms.Padding(4);
            this.Button16.Name = "Button16";
            this.Button16.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Button16.Size = new System.Drawing.Size(71, 30);
            this.Button16.TabIndex = 53;
            this.Button16.UseVisualStyleBackColor = false;
            // 
            // Button17
            // 
            this.Button17.BackColor = System.Drawing.SystemColors.Control;
            this.Button17.Cursor = System.Windows.Forms.Cursors.Default;
            this.Button17.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Button17.Location = new System.Drawing.Point(66, 661);
            this.Button17.Margin = new System.Windows.Forms.Padding(4);
            this.Button17.Name = "Button17";
            this.Button17.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Button17.Size = new System.Drawing.Size(100, 30);
            this.Button17.TabIndex = 52;
            this.Button17.Text = "Ausdrucken";
            this.Button17.UseVisualStyleBackColor = false;
            // 
            // RichTextBox1
            // 
            this.RichTextBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RichTextBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.RichTextBox1.Location = new System.Drawing.Point(18, 24);
            this.RichTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.Size = new System.Drawing.Size(916, 629);
            this.RichTextBox1.TabIndex = 50;
            this.RichTextBox1.Text = "";
            // 
            // lblSearch
            // 
            this.Label4.BackColor = System.Drawing.Color.Red;
            this.Label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label4.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.ForeColor = System.Drawing.Color.Yellow;
            this.Label4.Location = new System.Drawing.Point(0, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(404, 20);
            this.Label4.TabIndex = 22;
            this.Label4.Text = "lblSearch";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSorting
            // 
            this.Label5.BackColor = System.Drawing.Color.Red;
            this.Label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label5.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.ForeColor = System.Drawing.Color.Yellow;
            this.Label5.Location = new System.Drawing.Point(0, 21);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(404, 20);
            this.Label5.TabIndex = 23;
            this.Label5.Text = "lblSorting";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEMail
            // 
            this.Label6.BackColor = System.Drawing.Color.Red;
            this.Label6.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.ForeColor = System.Drawing.Color.Yellow;
            this.Label6.Location = new System.Drawing.Point(0, 42);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(404, 20);
            this.Label6.TabIndex = 24;
            this.Label6.Text = "Label6";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fraHntSearchFields1
            // 

            this.fraHntSearchFields1.eSearchSelection1 = GenFree.Data.ESearchSelection.eManual;
            this.fraHntSearchFields1.eSearchSelection2 = GenFree.Data.ESearchSelection.eManual;
            this.fraHntSearchFields1.eSearchSelection3 = GenFree.Data.ESearchSelection.eManual;
            this.fraHntSearchFields1.Location = new System.Drawing.Point(826, 41);
            this.fraHntSearchFields1.eSearchSelection1 = ESearchSelection.eManual;
            this.fraHntSearchFields1.eSearchSelection2 = ESearchSelection.eManual;
            this.fraHntSearchFields1.eSearchSelection3 = ESearchSelection.eManual;
            this.fraHntSearchFields1.Location = new System.Drawing.Point(826, 41);
            this.fraHntSearchFields1.Name = "fraHntSearchFields1";
            this.fraHntSearchFields1.Size = new System.Drawing.Size(1036, 612);
            this.fraHntSearchFields1.TabIndex = 50;
            this.fraHntSearchFields1.SaveAndBack += new System.EventHandler(this.Button18_Click);
            // 
            // Hinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 744);
            this.ControlBox = false;
            this.Controls.Add(this.fraHntSearchFields1);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Frame4);
            this.Controls.Add(this.GroupBox1);
            this.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "Hinter";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Frame4.ResumeLayout(false);
            this.Frame4.PerformLayout();
            this.GroupBox7.ResumeLayout(false);
            this.Frame2.ResumeLayout(false);
            this.Frame1.ResumeLayout(false);
            this.Frame3.ResumeLayout(false);
            this.Frame3.PerformLayout();
            this.Frame6.ResumeLayout(false);
            this.Frame6.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

    }


    public Button Button2;
    internal GroupBox Frame1;

    public Button Button1;
    public Button Button15;
    public Button Button14;
    public Button Button13;
    internal GroupBox Frame2;
    internal ColorDialog ColorDialog1;

    public Button Button12;
#pragma warning disable CS0618 // Typ oder Element ist veraltet
    public ListBox DirListBox1;
    public ListBox DriveListBox1;
    public ListBox FileListBox1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    internal Label Label4;
    internal Label Label5;
    internal Label Label6;
    internal GroupBox Frame3;
    internal Label Label7;
    internal Label Label13;
    internal Label Label12;
    internal Label Label11;
    internal Label Label10;
    internal Label Label9;
    internal Label Label8;
    internal TextBox TextBox1;
    internal TextBox TextBox4;
    internal TextBox TextBox3;
    internal TextBox TextBox2;
    internal TextBox TextBox5;

    public Button Button4;
    internal GroupBox Frame6;
    internal CheckBox CheckBox9;
    internal CheckBox CheckBox8;
    internal CheckBox CheckBox7;
    internal Label Label14;
    internal Label Label15;
    internal TextBox TextBox6;
    internal RadioButton Option2;
    internal RadioButton Option1;
    internal ListBox Sortlist;
    public GroupBox GroupBox1;

    public Button Button5;
    public RichTextBox RTB;
    public Label Label53;

    public ListBox List1;
    public Button Button6;
    public Button Button9;
    public Button Button8;
    internal Label Label16;
    internal Label Label17;
    internal SaveFileDialog SaveFileDialog1;

    public Button Button7;
    public Button Button3;
    public CheckBox CheckBox10;
    internal ProgressBar ProgressBar1;
    internal WebBrowser WebBrowser1;
    internal Label Label18;
    internal Label Label3;
    internal TextBox TextBox7;
    internal Label Label19;
    internal CheckBox CheckBox1;
    internal CheckBox CheckBox2;
    internal CheckBox CheckBox3;
    internal ListBox ListBox1;
    internal RadioButton RadioButton1;
    internal RadioButton RadioButton2;
    internal RadioButton RadioButton3;
    internal Button Button11;
    internal Button Button16;
    internal Button Button17;
    internal RichTextBox RichTextBox1;
    internal GroupBox GroupBox2;

    public Button Button10;

    internal CheckBox CheckBox4;
    internal RadioButton RadioButton32;
    internal RadioButton RadioButton31;
    internal GroupBox GroupBox7;
    internal CheckBox CheckBox5;
    private FraHntSearchFields fraHntSearchFields1;
}