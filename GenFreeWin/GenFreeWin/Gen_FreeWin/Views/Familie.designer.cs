using GenFree;
using GenFree.ViewModels.Interfaces;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Familie
{
    /*
                btnSearchNumber.Click += new System.EventHandler(btnSearchNumber_Click);
                btnSearchName.Click += new System.EventHandler(btnSearchName_Click);
                btnReenter.Click += new System.EventHandler(btnReenter_Click);
                btnNext.Click += new System.EventHandler(btnNext_Click);
                btnPrevious.Click += new System.EventHandler(btnPrevious_Click);
                btnEnableCheck.Click += new System.EventHandler(btnEnableCheck_Click);
                btnEndTextinput.Click += new System.EventHandler(btnEndTextinput_Click);
                btnSearchPartner.Click += new System.EventHandler(btnSearchPartner_Click);
                btnSearchRegister.Click += new System.EventHandler(btnSearchRegister_Click);
                btnMainmenue.Click += new System.EventHandler(btnMainmenue_Click);
                btnDelete.Click += new System.EventHandler(DeleteFamily);
                btnFamilysheet.Click += new System.EventHandler(btnFamilysheet_Click);
                btnResarch.Click += new System.EventHandler(btnResarch_Click);
                btnChildren.Click += new System.EventHandler(btnChildren_Click);
                lblGrandfatherF.Click += new System.EventHandler(Label5_Click);
                lblGrandmotherF.Click += new System.EventHandler(Label5_Click);
                lblNickName.Click += new System.EventHandler(Label14_Click);
                lblPredicate.Click += new System.EventHandler(Label13_Click);
                Label12.Click += new System.EventHandler(lblFatherName_Click);
                Label11.Click += new System.EventHandler(lblFatherName_Click);
                Label10.Click += new System.EventHandler(lblFatherName_Click);
                lblOccubation.Click += new System.EventHandler(lblFatherName_Click);
                lblResidence.Click += new System.EventHandler(lblFatherName_Click);
                lblURL.Click += new System.EventHandler(lblFatherName_Click);
                lblEMail.Click += new System.EventHandler(lblFatherName_Click);
                edtFatherPNr.KeyUp += new System.EventHandler(TextBox1_KeyUp);
                edtFatherPNr.KeyPress += new System.EventHandler(TextBox1_KeyPress);
                edtSuburb.KeyUp += new System.EventHandler(TextBox1_KeyUp);
                edtSuburb.KeyPress += new System.EventHandler(TextBox1_KeyPress);
                Label15.Click += new System.EventHandler(Label15_Click);
                Label16.Click += new System.EventHandler(Label16_Click);
                Label17.Click += new System.EventHandler(lblMotherName_Click);
                Label18.Click += new System.EventHandler(lblMotherName_Click);
                Label19.Click += new System.EventHandler(lblMotherName_Click);
                Label20.Click += new System.EventHandler(lblMotherName_Click);
                Label21.Click += new System.EventHandler(lblMotherName_Click);
                Label22.Click += new System.EventHandler(lblMotherName_Click);
                Label23.Click += new System.EventHandler(lblMotherName_Click);
                lblGrandfatherM.Click += new System.EventHandler(Label27_Click);
                Label28.Click += new System.EventHandler(Label27_Click);
                lblProklamation.Click += new System.EventHandler(Label29_Click);
                lblMarriage.Click += new System.EventHandler(Label30_Click);
                lblDivorce.Click += new System.EventHandler(Label31_Click);
                lblDimissorale.Click += new System.EventHandler(Label32_Click);
                lblPictures.Click += new System.EventHandler(Label35_Click);
                lblSources.Click += new System.EventHandler(Label36_Click);
                Label37.Click += new System.EventHandler(Label37_Click);
                lblReligMarr.Click += new System.EventHandler(Label38_Click);
                lblPartnership.Click += new System.EventHandler(Label39_Click);
                lblEstimatedMarr.Click += new System.EventHandler(Label40_Click);
                lstUsageList.DoubleClick += new System.EventHandler(frmSrch.ListBox1_DoubleClick);
                cbxIllegitRel.Click += new System.EventHandler(CheckBox1_Click);
                cbxIllegitRel.CheckedChanged += new System.EventHandler(CheckBox1_CheckedChanged);
                cbxProperty.SelectedIndexChanged += new System.EventHandler(ComboBox1_SelectedIndexChanged);
                Button16.Click += new System.EventHandler(OpenCheckPersons);
                ComboBox2.SelectedIndexChanged += new System.EventHandler(ComboBox2_SelectedIndexChanged);
                ComboBox2.TextChanged += new System.EventHandler(ComboBox2_TextChanged);
                Button17.Click += new System.EventHandler(OpenDuplettes);
                edtCounty.KeyPress += new System.EventHandler(TextBox1_KeyPress);
                Button25.Click += new System.EventHandler(Button25_Click);
                ListBox2.DoubleClick += new System.EventHandler(ListBox2_DoubleClick);
                edtCountry.KeyUp += new System.EventHandler(TextBox4_KeyUp);
                edtCountry.KeyPress += new System.EventHandler(TextBox4_KeyPress);
                RichTextBox1.LinkClicked += new System.EventHandler(RichTextBox1_LinkClicked);
                RichTextBox1.KeyDown += new System.EventHandler(RichTextBox1_KeyDown);
                RichTextBox1.Click += new System.EventHandler(RichTextBox1_Click);
                btnNew.Click += new System.EventHandler(Command2_Click);
                btnEdit.Click += new System.EventHandler(Command3_Click);
                btnEnterNew.Click += new System.EventHandler(Command1_1_Click);
                Button20.Click += new System.EventHandler(Button18_Click_1);
                Button19.Click += new System.EventHandler(Button18_Click_1);
                Button18.Click += new System.EventHandler(Button18_Click_1);
                Button21.Click += new System.EventHandler(Button21_Click);
                Button22.Click += new System.EventHandler(Button18_Click_1);
                ListBox3.Click += new System.EventHandler(Listbox3_Click);
                ListBox3.DoubleClick += new System.EventHandler(Listbox3_DoubleClick);
                Label45.Click += new System.EventHandler(Label46_Click);
                Label46.Click += new System.EventHandler(Label46_Click);
                Label47.Click += new System.EventHandler(Label46_Click);
                Button23.Click += new System.EventHandler(Button23_Click);
                Button24.Click += new System.EventHandler(Button24_Click);
                Button26.Click += new System.EventHandler(Button26_Click);
                edtNameprefix.KeyUp += new System.EventHandler(TextBox4_KeyUp);
                edtNameSuffix.KeyUp += new System.EventHandler(TextBox4_KeyUp);
         */
    public Button btnSearchNumber;
    public Button btnSearchName;
    public Button btnReenter;
    public Button btnNext;
    public Button btnPrevious;
    public Button btnEnableCheck;
    public Button btnEndTextinput;
    public Button btnSearchPartner;
    public Button btnSearchRegister;
    public Button btnMainmenue;
    [CommandBinding(nameof(IFamilieViewModel.DeleteFamilyCommand))]
    public Button btnDelete;
    public Button btnFamilysheet;
    public Button btnResarch;
    public Button btnChildren;
    public Button btnResidence;
    public Button btnConfirmation;
    public Button btnAppCancel;
    public Button btnAppFromFile;
    public Button btnAppNewPerson;
    public Button btnAppDelete;
    public Button btnAppRechPers;
    public Button btnMarrClose;
    public Button btnNew;
    public Button btnEdit;
    public Button Command1;
    public Button Button23;
    public Button Button24;
    public Button Button26;
    public Button btnSearchNumber2;
    [CommandBinding(nameof(IFamilieViewModel.AddProklamationCommand))]
    public Label lblProklamation;
    [CommandBinding(nameof(IFamilieViewModel.AddEngagementCommand))]
    public Label lblEngagement;
    [CommandBinding(nameof(IFamilieViewModel.AddMarriageCommand))]
    public Label lblMarriage;
    [CommandBinding(nameof(IFamilieViewModel.AddReligMarrCommand))]
    public Label lblReligMarr;
    [CommandBinding(nameof(IFamilieViewModel.AddDivorceCommand))]
    public Label lblDivorce;
    [CommandBinding(nameof(IFamilieViewModel.AddPartnershipCommand))]
    public Label lblPartnership;
    [CommandBinding(nameof(IFamilieViewModel.AddDimissoraleCommand))]
    public Label lblDimissorale;
    [CommandBinding(nameof(IFamilieViewModel.AddEstimatedMarrCommand))]
    public Label lblEstimatedMarr;
    
    public Label lblPictures;
    public Label lblSources;
    public Label lblFamilynotes;
    public Label lblMandant;
    public Label lblFamName;
    public Label lblFamNr;
    public Label Label24;
    public Label Label25;
    public Label lblCommonFamName;
    public Label lblChildCount;
    public Label lblHdrList;
    public Label lblHdrBorn_Name;
    public Label lblAppLabel44;
    public Label Label45;
    public Label Label46;
    public Label Label47;

    public CheckBox cbxIllegitRel;
    public CheckBox chbAppAsAdopted;
    public CheckBox CheckBox2;
    public ComboBox ComboBox1;
    public ComboBox ComboBox2;
    public ToolTip ToolTip1;
    public FraParentView fraFather;

    public TextBox edtNameprefix;
    public TextBox edtNameSuffix;
    public TextBox TextBox3;
    public TextBox TextBox4;

    public ListBox lstChildren;
    public ListBox List2;
    public ListBox lstMarriages;
    public ListBox ListBox3;
    public ListBox Famsortlist;
    public RichTextBox RichTextBox1;
    public GroupBox frmMarriage;
    public GroupBox frmFamilyresidence;
    public GroupBox frmAppendPerson;

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.btnSearchNumber = new System.Windows.Forms.Button();
            this.btnSearchName = new System.Windows.Forms.Button();
            this.btnReenter = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnEnableCheck = new System.Windows.Forms.Button();
            this.btnEndTextinput = new System.Windows.Forms.Button();
            this.btnSearchPartner = new System.Windows.Forms.Button();
            this.btnSearchRegister = new System.Windows.Forms.Button();
            this.btnMainmenue = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnFamilysheet = new System.Windows.Forms.Button();
            this.btnSearchNumber2 = new System.Windows.Forms.Button();
            this.btnResarch = new System.Windows.Forms.Button();
            this.btnChildren = new System.Windows.Forms.Button();
            this.lblMandant = new System.Windows.Forms.Label();
            this.lblFamName = new System.Windows.Forms.Label();
            this.lblFamNr = new System.Windows.Forms.Label();
            this.frmMarriage = new System.Windows.Forms.GroupBox();
            this.btnMarrClose = new System.Windows.Forms.Button();
            this.lstMarriages = new System.Windows.Forms.ListBox();
            this.Label24 = new System.Windows.Forms.Label();
            this.Label25 = new System.Windows.Forms.Label();
            this.List2 = new System.Windows.Forms.ListBox();
            this.lblProklamation = new System.Windows.Forms.Label();
            this.lblMarriage = new System.Windows.Forms.Label();
            this.lblDivorce = new System.Windows.Forms.Label();
            this.lblDimissorale = new System.Windows.Forms.Label();
            this.lblCommonFamName = new System.Windows.Forms.Label();
            this.lblChildCount = new System.Windows.Forms.Label();
            this.lblPictures = new System.Windows.Forms.Label();
            this.lblSources = new System.Windows.Forms.Label();
            this.lblEngagement = new System.Windows.Forms.Label();
            this.lblReligMarr = new System.Windows.Forms.Label();
            this.lblPartnership = new System.Windows.Forms.Label();
            this.lblEstimatedMarr = new System.Windows.Forms.Label();
            this.lblHdrList = new System.Windows.Forms.Label();
            this.lblHdrBorn_Name = new System.Windows.Forms.Label();
            this.lstChildren = new System.Windows.Forms.ListBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.edtNameprefix = new System.Windows.Forms.TextBox();
            this.edtNameSuffix = new System.Windows.Forms.TextBox();
            this.cbxIllegitRel = new System.Windows.Forms.CheckBox();
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.btnResidence = new System.Windows.Forms.Button();
            this.ComboBox2 = new System.Windows.Forms.ComboBox();
            this.btnConfirmation = new System.Windows.Forms.Button();
            this.TextBox3 = new System.Windows.Forms.TextBox();
            this.TextBox4 = new System.Windows.Forms.TextBox();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.lblFamilynotes = new System.Windows.Forms.Label();
            this.frmFamilyresidence = new System.Windows.Forms.GroupBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.Command1 = new System.Windows.Forms.Button();
            this.frmAppendPerson = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAppRechPers = new System.Windows.Forms.Button();
            this.btnAppDelete = new System.Windows.Forms.Button();
            this.btnAppNewPerson = new System.Windows.Forms.Button();
            this.btnAppFromFile = new System.Windows.Forms.Button();
            this.btnAppCancel = new System.Windows.Forms.Button();
            this.chbAppAsAdopted = new System.Windows.Forms.CheckBox();
            this.lblAppLabel44 = new System.Windows.Forms.Label();
            this.Famsortlist = new System.Windows.Forms.ListBox();
            this.ListBox3 = new System.Windows.Forms.ListBox();
            this.Label45 = new System.Windows.Forms.Label();
            this.Label46 = new System.Windows.Forms.Label();
            this.Label47 = new System.Windows.Forms.Label();
            this.Button23 = new System.Windows.Forms.Button();
            this.Button24 = new System.Windows.Forms.Button();
            this.Button26 = new System.Windows.Forms.Button();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.fraFather = new GenFreeWin.Views.FraParentView();
            this.fraMother = new GenFreeWin.Views.FraParentView();
            this.frmMarriage.SuspendLayout();
            this.frmFamilyresidence.SuspendLayout();
            this.frmAppendPerson.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearchNumber
            // 
            this.btnSearchNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSearchNumber.Location = new System.Drawing.Point(14, 630);
            this.btnSearchNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchNumber.Name = "btnSearchNumber";
            this.btnSearchNumber.Size = new System.Drawing.Size(129, 24);
            this.btnSearchNumber.TabIndex = 0;
            this.btnSearchNumber.Text = "suchen Nu&mmer";
            this.btnSearchNumber.UseVisualStyleBackColor = false;
            this.btnSearchNumber.Click += new System.EventHandler(this.btnSearchNumber_Click);
            // 
            // btnSearchName
            // 
            this.btnSearchName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSearchName.Location = new System.Drawing.Point(147, 630);
            this.btnSearchName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchName.Name = "btnSearchName";
            this.btnSearchName.Size = new System.Drawing.Size(107, 24);
            this.btnSearchName.TabIndex = 1;
            this.btnSearchName.Text = "suchen N&ame";
            this.btnSearchName.UseVisualStyleBackColor = false;
            this.btnSearchName.Click += new System.EventHandler(this.btnSearchName_Click);
            // 
            // btnReenter
            // 
            this.btnReenter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnReenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnReenter.Location = new System.Drawing.Point(260, 630);
            this.btnReenter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnReenter.Name = "btnReenter";
            this.btnReenter.Size = new System.Drawing.Size(108, 24);
            this.btnReenter.TabIndex = 2;
            this.btnReenter.Text = "&neu eingeben";
            this.btnReenter.UseVisualStyleBackColor = false;
            this.btnReenter.Click += new System.EventHandler(this.btnReenter_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnNext.Location = new System.Drawing.Point(527, 630);
            this.btnNext.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(147, 24);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "&vorblättern";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPrevious.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnPrevious.Location = new System.Drawing.Point(376, 630);
            this.btnPrevious.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(147, 24);
            this.btnPrevious.TabIndex = 4;
            this.btnPrevious.Text = "&rückblättern";
            this.btnPrevious.UseVisualStyleBackColor = false;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnEnableCheck
            // 
            this.btnEnableCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnEnableCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnEnableCheck.Location = new System.Drawing.Point(822, 630);
            this.btnEnableCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEnableCheck.Name = "btnEnableCheck";
            this.btnEnableCheck.Size = new System.Drawing.Size(129, 24);
            this.btnEnableCheck.TabIndex = 5;
            this.btnEnableCheck.Text = "&prüfen ein";
            this.btnEnableCheck.UseVisualStyleBackColor = false;
            this.btnEnableCheck.Click += new System.EventHandler(this.btnEnableCheck_Click);
            // 
            // btnEndTextinput
            // 
            this.btnEndTextinput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnEndTextinput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnEndTextinput.Location = new System.Drawing.Point(14, 659);
            this.btnEndTextinput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEndTextinput.Name = "btnEndTextinput";
            this.btnEndTextinput.Size = new System.Drawing.Size(185, 24);
            this.btnEndTextinput.TabIndex = 6;
            this.btnEndTextinput.Text = "Te&xteingabe beenden";
            this.btnEndTextinput.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEndTextinput.UseCompatibleTextRendering = true;
            this.btnEndTextinput.UseVisualStyleBackColor = false;
            this.btnEndTextinput.Visible = false;
            this.btnEndTextinput.Click += new System.EventHandler(this.btnEndTextinput_Click);
            // 
            // btnSearchPartner
            // 
            this.btnSearchPartner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchPartner.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSearchPartner.Location = new System.Drawing.Point(14, 659);
            this.btnSearchPartner.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchPartner.Name = "btnSearchPartner";
            this.btnSearchPartner.Size = new System.Drawing.Size(129, 24);
            this.btnSearchPartner.TabIndex = 7;
            this.btnSearchPartner.Text = "Par&tnersuche";
            this.btnSearchPartner.UseVisualStyleBackColor = false;
            this.btnSearchPartner.Click += new System.EventHandler(this.btnSearchPartner_Click);
            // 
            // btnSearchRegister
            // 
            this.btnSearchRegister.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSearchRegister.Location = new System.Drawing.Point(149, 659);
            this.btnSearchRegister.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchRegister.Name = "btnSearchRegister";
            this.btnSearchRegister.Size = new System.Drawing.Size(113, 24);
            this.btnSearchRegister.TabIndex = 8;
            this.btnSearchRegister.Text = "Re&gistersuche";
            this.btnSearchRegister.UseVisualStyleBackColor = false;
            this.btnSearchRegister.Click += new System.EventHandler(this.btnSearchRegister_Click);
            // 
            // btnMainmenue
            // 
            this.btnMainmenue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnMainmenue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnMainmenue.Location = new System.Drawing.Point(874, 659);
            this.btnMainmenue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMainmenue.Name = "btnMainmenue";
            this.btnMainmenue.Size = new System.Drawing.Size(129, 24);
            this.btnMainmenue.TabIndex = 9;
            this.btnMainmenue.Text = "Hauptmenue";
            this.btnMainmenue.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnMainmenue.UseVisualStyleBackColor = false;
            this.btnMainmenue.Click += new System.EventHandler(this.btnMainmenue_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnDelete.Location = new System.Drawing.Point(480, 659);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(79, 24);
            this.btnDelete.TabIndex = 10;
            this.btnDelete.Text = "&löschen";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnFamilysheet
            // 
            this.btnFamilysheet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFamilysheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnFamilysheet.Location = new System.Drawing.Point(688, 630);
            this.btnFamilysheet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFamilysheet.Name = "btnFamilysheet";
            this.btnFamilysheet.Size = new System.Drawing.Size(129, 24);
            this.btnFamilysheet.TabIndex = 11;
            this.btnFamilysheet.Text = "&Familienblatt";
            this.btnFamilysheet.UseVisualStyleBackColor = false;
            this.btnFamilysheet.Click += new System.EventHandler(this.btnFamilysheet_Click);
            // 
            // btnSearchNumber2
            // 
            this.btnSearchNumber2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSearchNumber2.Location = new System.Drawing.Point(732, 668);
            this.btnSearchNumber2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchNumber2.Name = "btnSearchNumber2";
            this.btnSearchNumber2.Size = new System.Drawing.Size(129, 24);
            this.btnSearchNumber2.TabIndex = 12;
            this.btnSearchNumber2.Text = "suchen Nummer";
            this.btnSearchNumber2.UseVisualStyleBackColor = true;
            this.btnSearchNumber2.Visible = false;
            // 
            // btnResarch
            // 
            this.btnResarch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnResarch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnResarch.Location = new System.Drawing.Point(273, 661);
            this.btnResarch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnResarch.Name = "btnResarch";
            this.btnResarch.Size = new System.Drawing.Size(95, 24);
            this.btnResarch.TabIndex = 13;
            this.btnResarch.Text = "R&echerche";
            this.btnResarch.UseVisualStyleBackColor = false;
            this.btnResarch.Click += new System.EventHandler(this.btnResarch_Click);
            // 
            // btnChildren
            // 
            this.btnChildren.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnChildren.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnChildren.Location = new System.Drawing.Point(186, 357);
            this.btnChildren.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChildren.Name = "btnChildren";
            this.btnChildren.Size = new System.Drawing.Size(87, 25);
            this.btnChildren.TabIndex = 14;
            this.btnChildren.Text = "&Kind +/-";
            this.ToolTip1.SetToolTip(this.btnChildren, "Kind zufügen oder entfernen");
            this.btnChildren.UseVisualStyleBackColor = false;
            this.btnChildren.Click += new System.EventHandler(this.btnChildren_Click);
            // 
            // lblMandant
            // 
            this.lblMandant.BackColor = System.Drawing.Color.Yellow;
            this.lblMandant.Font = new System.Drawing.Font("Arial", 8.25F);
            this.lblMandant.Location = new System.Drawing.Point(0, 0);
            this.lblMandant.Name = "lblMandant";
            this.lblMandant.Size = new System.Drawing.Size(303, 17);
            this.lblMandant.TabIndex = 15;
            this.lblMandant.Text = "<Mandant>";
            // 
            // lblFamName
            // 
            this.lblFamName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblFamName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblFamName.Location = new System.Drawing.Point(530, 459);
            this.lblFamName.Name = "lblFamName";
            this.lblFamName.Size = new System.Drawing.Size(165, 16);
            this.lblFamName.TabIndex = 16;
            this.lblFamName.Text = "<Family>";
            // 
            // lblFamNr
            // 
            this.lblFamNr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblFamNr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblFamNr.Location = new System.Drawing.Point(306, 0);
            this.lblFamNr.Name = "lblFamNr";
            this.lblFamNr.Size = new System.Drawing.Size(165, 16);
            this.lblFamNr.TabIndex = 18;
            this.lblFamNr.Text = "<FamNr>";
            // 
            // frmMarriage
            // 
            this.frmMarriage.BackColor = System.Drawing.Color.Red;
            this.frmMarriage.Controls.Add(this.btnMarrClose);
            this.frmMarriage.Controls.Add(this.lstMarriages);
            this.frmMarriage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frmMarriage.Location = new System.Drawing.Point(211, 435);
            this.frmMarriage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.frmMarriage.Name = "frmMarriage";
            this.frmMarriage.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.frmMarriage.Size = new System.Drawing.Size(553, 187);
            this.frmMarriage.TabIndex = 57;
            this.frmMarriage.TabStop = false;
            this.frmMarriage.Text = "Ehe mit";
            this.frmMarriage.Visible = false;
            // 
            // btnMarrClose
            // 
            this.btnMarrClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnMarrClose.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnMarrClose.Location = new System.Drawing.Point(248, 159);
            this.btnMarrClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMarrClose.Name = "btnMarrClose";
            this.btnMarrClose.Size = new System.Drawing.Size(84, 22);
            this.btnMarrClose.TabIndex = 1;
            this.btnMarrClose.Text = "schließen";
            this.btnMarrClose.UseVisualStyleBackColor = false;
            this.btnMarrClose.Click += new System.EventHandler(this.btnMarrClose_Click);
            // 
            // lstMarriages
            // 
            this.lstMarriages.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lstMarriages.FormattingEnabled = true;
            this.lstMarriages.ItemHeight = 20;
            this.lstMarriages.Location = new System.Drawing.Point(12, 22);
            this.lstMarriages.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstMarriages.Name = "lstMarriages";
            this.lstMarriages.Size = new System.Drawing.Size(528, 104);
            this.lstMarriages.Sorted = true;
            this.lstMarriages.TabIndex = 0;
            this.lstMarriages.DoubleClick += new System.EventHandler(this.lstMarriages_DoubleClick);
            // 
            // Label24
            // 
            this.Label24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Label24.Location = new System.Drawing.Point(477, -1);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(202, 16);
            this.Label24.TabIndex = 23;
            this.Label24.Text = "Label24";
            // 
            // Label25
            // 
            this.Label25.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Label25.Location = new System.Drawing.Point(685, 0);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(211, 16);
            this.Label25.TabIndex = 24;
            this.Label25.Text = "Label25";
            // 
            // List2
            // 
            this.List2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.List2.FormattingEnabled = true;
            this.List2.ItemHeight = 20;
            this.List2.Location = new System.Drawing.Point(186, 526);
            this.List2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.List2.Name = "List2";
            this.List2.Size = new System.Drawing.Size(86, 24);
            this.List2.Sorted = true;
            this.List2.TabIndex = 28;
            this.List2.Visible = false;
            // 
            // lblProklamation
            // 
            this.lblProklamation.AutoEllipsis = true;
            this.lblProklamation.BackColor = System.Drawing.Color.White;
            this.lblProklamation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblProklamation.Location = new System.Drawing.Point(511, 221);
            this.lblProklamation.Name = "lblProklamation";
            this.lblProklamation.Size = new System.Drawing.Size(484, 16);
            this.lblProklamation.TabIndex = 29;
            this.lblProklamation.Text = "Proklamation";
            this.lblProklamation.UseMnemonic = false;
            // 
            // lblMarriage
            // 
            this.lblMarriage.AutoEllipsis = true;
            this.lblMarriage.BackColor = System.Drawing.Color.White;
            this.lblMarriage.Font = new System.Drawing.Font("Arial", 8.25F);
            this.lblMarriage.Location = new System.Drawing.Point(12, 239);
            this.lblMarriage.Name = "lblMarriage";
            this.lblMarriage.Size = new System.Drawing.Size(484, 16);
            this.lblMarriage.TabIndex = 30;
            this.lblMarriage.Text = "Heirat";
            this.lblMarriage.UseMnemonic = false;
            // 
            // lblDivorce
            // 
            this.lblDivorce.AutoEllipsis = true;
            this.lblDivorce.BackColor = System.Drawing.Color.White;
            this.lblDivorce.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblDivorce.Location = new System.Drawing.Point(12, 258);
            this.lblDivorce.Name = "lblDivorce";
            this.lblDivorce.Size = new System.Drawing.Size(484, 16);
            this.lblDivorce.TabIndex = 31;
            this.lblDivorce.Text = "Scheidung";
            this.lblDivorce.UseMnemonic = false;
            // 
            // lblDimissorale
            // 
            this.lblDimissorale.AutoEllipsis = true;
            this.lblDimissorale.BackColor = System.Drawing.Color.White;
            this.lblDimissorale.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblDimissorale.Location = new System.Drawing.Point(12, 276);
            this.lblDimissorale.Name = "lblDimissorale";
            this.lblDimissorale.Size = new System.Drawing.Size(484, 16);
            this.lblDimissorale.TabIndex = 32;
            this.lblDimissorale.Text = "Dimissiorale";
            this.lblDimissorale.UseMnemonic = false;
            // 
            // lblCommonFamName
            // 
            this.lblCommonFamName.AutoEllipsis = true;
            this.lblCommonFamName.BackColor = System.Drawing.Color.White;
            this.lblCommonFamName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblCommonFamName.Location = new System.Drawing.Point(9, 303);
            this.lblCommonFamName.Name = "lblCommonFamName";
            this.lblCommonFamName.Size = new System.Drawing.Size(196, 22);
            this.lblCommonFamName.TabIndex = 33;
            // 
            // lblChildCount
            // 
            this.lblChildCount.BackColor = System.Drawing.Color.White;
            this.lblChildCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblChildCount.Location = new System.Drawing.Point(337, 358);
            this.lblChildCount.Name = "lblChildCount";
            this.lblChildCount.Size = new System.Drawing.Size(130, 24);
            this.lblChildCount.TabIndex = 34;
            // 
            // lblPictures
            // 
            this.lblPictures.BackColor = System.Drawing.Color.White;
            this.lblPictures.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblPictures.Location = new System.Drawing.Point(851, 303);
            this.lblPictures.Name = "lblPictures";
            this.lblPictures.Size = new System.Drawing.Size(55, 22);
            this.lblPictures.TabIndex = 35;
            this.lblPictures.Text = "Bilder";
            this.lblPictures.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPictures.Click += new System.EventHandler(this.lblPicures_Click);
            // 
            // lblSources
            // 
            this.lblSources.BackColor = System.Drawing.Color.White;
            this.lblSources.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblSources.Location = new System.Drawing.Point(909, 303);
            this.lblSources.Name = "lblSources";
            this.lblSources.Size = new System.Drawing.Size(88, 22);
            this.lblSources.TabIndex = 36;
            this.lblSources.Text = "Quellen";
            this.lblSources.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSources.Click += new System.EventHandler(this.lblSources_Click);
            // 
            // lblEngagement
            // 
            this.lblEngagement.AutoEllipsis = true;
            this.lblEngagement.BackColor = System.Drawing.Color.White;
            this.lblEngagement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblEngagement.Location = new System.Drawing.Point(12, 221);
            this.lblEngagement.Name = "lblEngagement";
            this.lblEngagement.Size = new System.Drawing.Size(484, 16);
            this.lblEngagement.TabIndex = 37;
            this.lblEngagement.Text = "Verlobung";
            this.lblEngagement.UseMnemonic = false;
            // 
            // lblReligMarr
            // 
            this.lblReligMarr.AutoEllipsis = true;
            this.lblReligMarr.BackColor = System.Drawing.Color.White;
            this.lblReligMarr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblReligMarr.Location = new System.Drawing.Point(511, 239);
            this.lblReligMarr.Name = "lblReligMarr";
            this.lblReligMarr.Size = new System.Drawing.Size(484, 16);
            this.lblReligMarr.TabIndex = 38;
            this.lblReligMarr.Text = "Kirchl. Heirat";
            this.lblReligMarr.UseMnemonic = false;
            // 
            // lblPartnership
            // 
            this.lblPartnership.AutoEllipsis = true;
            this.lblPartnership.BackColor = System.Drawing.Color.White;
            this.lblPartnership.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblPartnership.Location = new System.Drawing.Point(511, 258);
            this.lblPartnership.Name = "lblPartnership";
            this.lblPartnership.Size = new System.Drawing.Size(484, 16);
            this.lblPartnership.TabIndex = 39;
            this.lblPartnership.Text = "Eheähnl. Beziehung";
            this.lblPartnership.UseMnemonic = false;
            // 
            // lblEstimatedMarr
            // 
            this.lblEstimatedMarr.AutoEllipsis = true;
            this.lblEstimatedMarr.BackColor = System.Drawing.Color.White;
            this.lblEstimatedMarr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblEstimatedMarr.Location = new System.Drawing.Point(511, 276);
            this.lblEstimatedMarr.Name = "lblEstimatedMarr";
            this.lblEstimatedMarr.Size = new System.Drawing.Size(263, 16);
            this.lblEstimatedMarr.TabIndex = 40;
            this.lblEstimatedMarr.Text = "fiktiv. Heiratsdatum";
            this.lblEstimatedMarr.UseMnemonic = false;
            // 
            // lblHdrList
            // 
            this.lblHdrList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblHdrList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblHdrList.Location = new System.Drawing.Point(472, 358);
            this.lblHdrList.Name = "lblHdrList";
            this.lblHdrList.Size = new System.Drawing.Size(503, 24);
            this.lblHdrList.TabIndex = 42;
            // 
            // lblHdrBorn_Name
            // 
            this.lblHdrBorn_Name.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblHdrBorn_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblHdrBorn_Name.Location = new System.Drawing.Point(9, 358);
            this.lblHdrBorn_Name.Name = "lblHdrBorn_Name";
            this.lblHdrBorn_Name.Size = new System.Drawing.Size(170, 24);
            this.lblHdrBorn_Name.TabIndex = 43;
            // 
            // lstChildren
            // 
            this.lstChildren.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lstChildren.FormattingEnabled = true;
            this.lstChildren.ItemHeight = 20;
            this.lstChildren.Location = new System.Drawing.Point(4, 384);
            this.lstChildren.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstChildren.Name = "lstChildren";
            this.lstChildren.Size = new System.Drawing.Size(992, 64);
            this.lstChildren.TabIndex = 48;
            this.lstChildren.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            // 
            // edtNameprefix
            // 
            this.edtNameprefix.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.edtNameprefix.Location = new System.Drawing.Point(488, 303);
            this.edtNameprefix.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.edtNameprefix.Name = "edtNameprefix";
            this.edtNameprefix.Size = new System.Drawing.Size(151, 26);
            this.edtNameprefix.TabIndex = 94;
            this.ToolTip1.SetToolTip(this.edtNameprefix, "Namenspräfix");
            this.edtNameprefix.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtNamePS_KeyUp);
            // 
            // edtNameSuffix
            // 
            this.edtNameSuffix.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.edtNameSuffix.Location = new System.Drawing.Point(646, 303);
            this.edtNameSuffix.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.edtNameSuffix.Name = "edtNameSuffix";
            this.edtNameSuffix.Size = new System.Drawing.Size(203, 26);
            this.edtNameSuffix.TabIndex = 95;
            this.ToolTip1.SetToolTip(this.edtNameSuffix, "Namenssuffix");
            this.edtNameSuffix.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtNamePS_KeyUp);
            // 
            // cbxIllegitRel
            // 
            this.cbxIllegitRel.BackColor = System.Drawing.Color.White;
            this.cbxIllegitRel.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbxIllegitRel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.cbxIllegitRel.Location = new System.Drawing.Point(783, 276);
            this.cbxIllegitRel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbxIllegitRel.Name = "cbxIllegitRel";
            this.cbxIllegitRel.Size = new System.Drawing.Size(212, 16);
            this.cbxIllegitRel.TabIndex = 49;
            this.cbxIllegitRel.Text = "Aussereheliche Verbindung";
            this.cbxIllegitRel.UseCompatibleTextRendering = true;
            this.cbxIllegitRel.UseVisualStyleBackColor = false;
            this.cbxIllegitRel.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            this.cbxIllegitRel.Click += new System.EventHandler(this.cbxIllegitRel_Click);
            // 
            // cbxProperty
            // 
            this.ComboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ComboBox1.FormattingEnabled = true;
            this.ComboBox1.Location = new System.Drawing.Point(123, 330);
            this.ComboBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(268, 28);
            this.ComboBox1.Sorted = true;
            this.ComboBox1.TabIndex = 50;
            this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // btnResidence
            // 
            this.btnResidence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnResidence.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnResidence.Location = new System.Drawing.Point(4, 330);
            this.btnResidence.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnResidence.Name = "btnResidence";
            this.btnResidence.Size = new System.Drawing.Size(116, 24);
            this.btnResidence.TabIndex = 51;
            this.btnResidence.Text = "Wohnort:";
            this.btnResidence.UseVisualStyleBackColor = false;
            this.btnResidence.Click += new System.EventHandler(this.btnResidence_Click);
            // 
            // ComboBox2
            // 
            this.ComboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ComboBox2.FormattingEnabled = true;
            this.ComboBox2.Location = new System.Drawing.Point(670, 329);
            this.ComboBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ComboBox2.Name = "ComboBox2";
            this.ComboBox2.Size = new System.Drawing.Size(326, 28);
            this.ComboBox2.Sorted = true;
            this.ComboBox2.TabIndex = 52;
            this.ComboBox2.SelectedIndexChanged += new System.EventHandler(this.ComboBox2_SelectedIndexChanged);
            this.ComboBox2.TextChanged += new System.EventHandler(this.ComboBox2_TextChanged);
            // 
            // btnConfirmation
            // 
            this.btnConfirmation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnConfirmation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnConfirmation.Location = new System.Drawing.Point(581, 328);
            this.btnConfirmation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfirmation.Name = "btnConfirmation";
            this.btnConfirmation.Size = new System.Drawing.Size(87, 24);
            this.btnConfirmation.TabIndex = 53;
            this.btnConfirmation.UseVisualStyleBackColor = false;
            this.btnConfirmation.Click += new System.EventHandler(this.btnConfirmation_Click);
            // 
            // edtCounty
            // 
            this.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.TextBox3.Location = new System.Drawing.Point(274, 358);
            this.TextBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TextBox3.Multiline = true;
            this.TextBox3.Name = "edtCounty";
            this.TextBox3.Size = new System.Drawing.Size(56, 24);
            this.TextBox3.TabIndex = 54;
            this.TextBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            // 
            // edtCountry
            // 
            this.TextBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.TextBox4.Location = new System.Drawing.Point(211, 303);
            this.TextBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TextBox4.Name = "edtCountry";
            this.TextBox4.Size = new System.Drawing.Size(271, 26);
            this.TextBox4.TabIndex = 58;
            this.TextBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox4_KeyPress);
            this.TextBox4.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtNamePS_KeyUp);
            // 
            // RichTextBox1
            // 
            this.RichTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.RichTextBox1.Location = new System.Drawing.Point(4, 538);
            this.RichTextBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.Size = new System.Drawing.Size(992, 82);
            this.RichTextBox1.TabIndex = 59;
            this.RichTextBox1.Text = "";
            this.RichTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.RichTextBox1_LinkClicked);
            this.RichTextBox1.Click += new System.EventHandler(this.RichTextBox1_Click);
            this.RichTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox1_KeyDown);
            // 
            // lblFamilynotes
            // 
            this.lblFamilynotes.AutoSize = true;
            this.lblFamilynotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblFamilynotes.Location = new System.Drawing.Point(4, 520);
            this.lblFamilynotes.Name = "lblFamilynotes";
            this.lblFamilynotes.Size = new System.Drawing.Size(189, 20);
            this.lblFamilynotes.TabIndex = 60;
            this.lblFamilynotes.Text = "Bemerkungen zur Familie";
            // 
            // frmFamilyresidence
            // 
            this.frmFamilyresidence.BackColor = System.Drawing.Color.Red;
            this.frmFamilyresidence.Controls.Add(this.btnNew);
            this.frmFamilyresidence.Controls.Add(this.btnEdit);
            this.frmFamilyresidence.Controls.Add(this.Command1);
            this.frmFamilyresidence.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frmFamilyresidence.ForeColor = System.Drawing.Color.White;
            this.frmFamilyresidence.Location = new System.Drawing.Point(376, 86);
            this.frmFamilyresidence.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.frmFamilyresidence.Name = "frmFamilyresidence";
            this.frmFamilyresidence.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.frmFamilyresidence.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmFamilyresidence.Size = new System.Drawing.Size(264, 97);
            this.frmFamilyresidence.TabIndex = 84;
            this.frmFamilyresidence.TabStop = false;
            this.frmFamilyresidence.Text = "Wohnort der Familie";
            this.frmFamilyresidence.Visible = false;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.SystemColors.Control;
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNew.Location = new System.Drawing.Point(136, 16);
            this.btnNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNew.Name = "btnNew";
            this.btnNew.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNew.Size = new System.Drawing.Size(117, 30);
            this.btnNew.TabIndex = 86;
            this.btnNew.Text = "neu e&ingeben";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEdit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEdit.Location = new System.Drawing.Point(10, 17);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEdit.Size = new System.Drawing.Size(108, 29);
            this.btnEdit.TabIndex = 85;
            this.btnEdit.Text = "&Bearbeiten";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnEnterNew
            // 
            this.Command1.BackColor = System.Drawing.SystemColors.Control;
            this.Command1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Command1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.Location = new System.Drawing.Point(79, 51);
            this.Command1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Command1.Name = "Command1";
            this.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Command1.Size = new System.Drawing.Size(91, 28);
            this.Command1.TabIndex = 84;
            this.Command1.Text = "abbrechen";
            this.Command1.UseVisualStyleBackColor = false;
            this.Command1.Click += new System.EventHandler(this.Command1_Click);
            // 
            // frmAppendPerson
            // 
            this.frmAppendPerson.BackColor = System.Drawing.Color.Red;
            this.frmAppendPerson.Controls.Add(this.panel1);
            this.frmAppendPerson.Controls.Add(this.btnAppRechPers);
            this.frmAppendPerson.Controls.Add(this.btnAppDelete);
            this.frmAppendPerson.Controls.Add(this.btnAppNewPerson);
            this.frmAppendPerson.Controls.Add(this.btnAppFromFile);
            this.frmAppendPerson.Controls.Add(this.btnAppCancel);
            this.frmAppendPerson.Controls.Add(this.chbAppAsAdopted);
            this.frmAppendPerson.Controls.Add(this.lblAppLabel44);
            this.frmAppendPerson.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frmAppendPerson.ForeColor = System.Drawing.SystemColors.ControlText;
            this.frmAppendPerson.Location = new System.Drawing.Point(319, 312);
            this.frmAppendPerson.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.frmAppendPerson.Name = "frmAppendPerson";
            this.frmAppendPerson.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.frmAppendPerson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmAppendPerson.Size = new System.Drawing.Size(279, 274);
            this.frmAppendPerson.TabIndex = 85;
            this.frmAppendPerson.TabStop = false;
            this.frmAppendPerson.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(7, 198);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(268, 8);
            this.panel1.TabIndex = 88;
            // 
            // btnAppRechPers
            // 
            this.btnAppRechPers.BackColor = System.Drawing.Color.Lime;
            this.btnAppRechPers.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnAppRechPers.Location = new System.Drawing.Point(20, 138);
            this.btnAppRechPers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAppRechPers.Name = "btnAppRechPers";
            this.btnAppRechPers.Size = new System.Drawing.Size(247, 25);
            this.btnAppRechPers.TabIndex = 87;
            this.btnAppRechPers.Text = "na&ch der Person recherchieren";
            this.btnAppRechPers.UseVisualStyleBackColor = false;
            this.btnAppRechPers.Click += new System.EventHandler(this.Button18_Click_1);
            // 
            // btnAppDelete
            // 
            this.btnAppDelete.BackColor = System.Drawing.Color.Lime;
            this.btnAppDelete.Enabled = false;
            this.btnAppDelete.Location = new System.Drawing.Point(19, 208);
            this.btnAppDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAppDelete.Name = "btnAppDelete";
            this.btnAppDelete.Size = new System.Drawing.Size(246, 49);
            this.btnAppDelete.TabIndex = 86;
            this.btnAppDelete.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAppDelete.UseVisualStyleBackColor = false;
            this.btnAppDelete.Click += new System.EventHandler(this.Button21_Click);
            // 
            // btnAppNewPerson
            // 
            this.btnAppNewPerson.BackColor = System.Drawing.Color.Lime;
            this.btnAppNewPerson.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnAppNewPerson.Location = new System.Drawing.Point(19, 80);
            this.btnAppNewPerson.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAppNewPerson.Name = "btnAppNewPerson";
            this.btnAppNewPerson.Size = new System.Drawing.Size(248, 25);
            this.btnAppNewPerson.TabIndex = 84;
            this.btnAppNewPerson.Text = "Pers&on neu eingeben";
            this.btnAppNewPerson.UseVisualStyleBackColor = false;
            this.btnAppNewPerson.Click += new System.EventHandler(this.Button18_Click_1);
            // 
            // btnAppFromFile
            // 
            this.btnAppFromFile.BackColor = System.Drawing.Color.Lime;
            this.btnAppFromFile.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnAppFromFile.Location = new System.Drawing.Point(19, 111);
            this.btnAppFromFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAppFromFile.Name = "btnAppFromFile";
            this.btnAppFromFile.Size = new System.Drawing.Size(248, 25);
            this.btnAppFromFile.TabIndex = 83;
            this.btnAppFromFile.Text = "Person aus &Datei wählen";
            this.btnAppFromFile.UseVisualStyleBackColor = false;
            this.btnAppFromFile.Click += new System.EventHandler(this.Button18_Click_1);
            // 
            // btnAppCancel
            // 
            this.btnAppCancel.BackColor = System.Drawing.Color.Lime;
            this.btnAppCancel.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnAppCancel.Location = new System.Drawing.Point(19, 167);
            this.btnAppCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAppCancel.Name = "btnAppCancel";
            this.btnAppCancel.Size = new System.Drawing.Size(248, 25);
            this.btnAppCancel.TabIndex = 82;
            this.btnAppCancel.Text = "Abbrechen";
            this.btnAppCancel.UseVisualStyleBackColor = false;
            this.btnAppCancel.Click += new System.EventHandler(this.Button18_Click_1);
            // 
            // chbAppAsAdopted
            // 
            this.chbAppAsAdopted.BackColor = System.Drawing.SystemColors.Control;
            this.chbAppAsAdopted.Cursor = System.Windows.Forms.Cursors.Default;
            this.chbAppAsAdopted.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chbAppAsAdopted.Location = new System.Drawing.Point(19, 50);
            this.chbAppAsAdopted.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chbAppAsAdopted.Name = "chbAppAsAdopted";
            this.chbAppAsAdopted.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chbAppAsAdopted.Size = new System.Drawing.Size(142, 24);
            this.chbAppAsAdopted.TabIndex = 81;
            this.chbAppAsAdopted.Text = "Als Adoptivkind";
            this.chbAppAsAdopted.UseVisualStyleBackColor = false;
            this.chbAppAsAdopted.Visible = false;
            // 
            // lblAppLabel44
            // 
            this.lblAppLabel44.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblAppLabel44.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblAppLabel44.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAppLabel44.Location = new System.Drawing.Point(16, 20);
            this.lblAppLabel44.Name = "lblAppLabel44";
            this.lblAppLabel44.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAppLabel44.Size = new System.Drawing.Size(248, 24);
            this.lblAppLabel44.TabIndex = 69;
            // 
            // Famsortlist
            // 
            this.Famsortlist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Famsortlist.FormattingEnabled = true;
            this.Famsortlist.ItemHeight = 20;
            this.Famsortlist.Location = new System.Drawing.Point(636, 470);
            this.Famsortlist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Famsortlist.Name = "Famsortlist";
            this.Famsortlist.Size = new System.Drawing.Size(81, 224);
            this.Famsortlist.Sorted = true;
            this.Famsortlist.TabIndex = 86;
            this.Famsortlist.Visible = false;
            // 
            // ListBox3
            // 
            this.ListBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.ListBox3.FormattingEnabled = true;
            this.ListBox3.ItemHeight = 20;
            this.ListBox3.Location = new System.Drawing.Point(4, 384);
            this.ListBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ListBox3.Name = "ListBox3";
            this.ListBox3.Size = new System.Drawing.Size(294, 224);
            this.ListBox3.Sorted = true;
            this.ListBox3.TabIndex = 87;
            this.ListBox3.Visible = false;
            this.ListBox3.DoubleClick += new System.EventHandler(this.Listbox3_DoubleClick);
            // 
            // Label45
            // 
            this.Label45.BackColor = System.Drawing.Color.Red;
            this.Label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Label45.Location = new System.Drawing.Point(233, 520);
            this.Label45.Name = "Label45";
            this.Label45.Size = new System.Drawing.Size(178, 17);
            this.Label45.TabIndex = 88;
            this.Label45.Text = "Label45";
            this.Label45.Click += new System.EventHandler(this.Label46_Click);
            // 
            // Label46
            // 
            this.Label46.BackColor = System.Drawing.Color.Red;
            this.Label46.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Label46.Location = new System.Drawing.Point(426, 520);
            this.Label46.Name = "Label46";
            this.Label46.Size = new System.Drawing.Size(143, 17);
            this.Label46.TabIndex = 89;
            this.Label46.Text = "Label46";
            this.Label46.Click += new System.EventHandler(this.Label46_Click);
            // 
            // Label47
            // 
            this.Label47.BackColor = System.Drawing.Color.Red;
            this.Label47.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Label47.Location = new System.Drawing.Point(633, 520);
            this.Label47.Name = "Label47";
            this.Label47.Size = new System.Drawing.Size(129, 17);
            this.Label47.TabIndex = 90;
            this.Label47.Text = "Label47";
            this.Label47.Click += new System.EventHandler(this.Label46_Click);
            // 
            // Button23
            // 
            this.Button23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Button23.Location = new System.Drawing.Point(563, 659);
            this.Button23.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button23.Name = "Button23";
            this.Button23.Size = new System.Drawing.Size(298, 24);
            this.Button23.TabIndex = 91;
            this.Button23.Text = "Heiratseintrag zur kirchl. Heirat verschieben";
            this.Button23.UseVisualStyleBackColor = false;
            this.Button23.Visible = false;
            this.Button23.Click += new System.EventHandler(this.Button23_Click);
            // 
            // Button24
            // 
            this.Button24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button24.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Button24.Location = new System.Drawing.Point(956, 630);
            this.Button24.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button24.Name = "Button24";
            this.Button24.Size = new System.Drawing.Size(48, 24);
            this.Button24.TabIndex = 92;
            this.Button24.Text = "Heiratseintrag zur kirchl. Heirat verschieben";
            this.Button24.UseVisualStyleBackColor = false;
            this.Button24.Visible = false;
            this.Button24.Click += new System.EventHandler(this.Button24_Click);
            // 
            // Button26
            // 
            this.Button26.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.Button26.Location = new System.Drawing.Point(207, 659);
            this.Button26.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button26.Name = "Button26";
            this.Button26.Size = new System.Drawing.Size(185, 24);
            this.Button26.TabIndex = 93;
            this.Button26.Text = "abbrechen";
            this.Button26.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Button26.UseCompatibleTextRendering = true;
            this.Button26.UseVisualStyleBackColor = false;
            this.Button26.Visible = false;
            this.Button26.Click += new System.EventHandler(this.Button26_Click);
            // 
            // CheckBox2
            // 
            this.CheckBox2.AutoSize = true;
            this.CheckBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.CheckBox2.Location = new System.Drawing.Point(397, 329);
            this.CheckBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CheckBox2.Name = "CheckBox2";
            this.CheckBox2.Size = new System.Drawing.Size(213, 24);
            this.CheckBox2.TabIndex = 96;
            this.CheckBox2.Text = "Gleichgesch. Verbindung";
            this.CheckBox2.UseVisualStyleBackColor = true;
            // 
            // fraFather
            // 
            this.fraFather.iPNr = 0;
  //          this.fraFather.iText = GenFreeWin.EUserText.tNone;
            this.fraFather.Location = new System.Drawing.Point(4, 17);
            this.fraFather.Name = "fraFather";
            this.fraFather.PersAka = "<AKA>";
            this.fraFather.PersGivn = "<väterl. Vorname>";
            this.fraFather.PersName = "<väterl. Nachname>";
            this.fraFather.PersNrMarr = "Anz. Ehen: <##>";
            this.fraFather.PersText10 = "";
            this.fraFather.PersText12 = "";
            this.fraFather.PersText8 = "<Zusatz>";
            this.fraFather.PersTitle = "<Titel>";
            this.fraFather.PNr_Visible = true;
            this.fraFather.Size = new System.Drawing.Size(500, 206);
            this.fraFather.TabIndex = 97;
            // 
            // fraMother
            // 
            this.fraMother.iPNr = 0;
   //         this.fraMother.iText = GenFreeWin.EUserText.tNone;
            this.fraMother.Location = new System.Drawing.Point(504, 17);
            this.fraMother.Margin = new System.Windows.Forms.Padding(4);
            this.fraMother.Name = "fraMother";
            this.fraMother.PersAka = "<AKA>";
            this.fraMother.PersGivn = "<mütterl. Vorname>";
            this.fraMother.PersName = "<mütterl. Nachname>";
            this.fraMother.PersNrMarr = "Anz. Ehen: <##>";
            this.fraMother.PersText10 = "";
            this.fraMother.PersText12 = "";
            this.fraMother.PersText8 = "<Zusatz>";
            this.fraMother.PersTitle = "<Titel>";
            this.fraMother.PNr_Visible = true;
            this.fraMother.Size = new System.Drawing.Size(500, 206);
            this.fraMother.TabIndex = 98;
            // 
            // Familie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(1006, 696);
            this.Controls.Add(this.ListBox3);
            this.Controls.Add(this.frmMarriage);
            this.Controls.Add(this.frmAppendPerson);
            this.Controls.Add(this.Button26);
            this.Controls.Add(this.btnEndTextinput);
            this.Controls.Add(this.Button24);
            this.Controls.Add(this.Button23);
            this.Controls.Add(this.RichTextBox1);
            this.Controls.Add(this.Label47);
            this.Controls.Add(this.frmFamilyresidence);
            this.Controls.Add(this.Label46);
            this.Controls.Add(this.Label45);
            this.Controls.Add(this.lblFamilynotes);
            this.Controls.Add(this.TextBox4);
            this.Controls.Add(this.TextBox3);
            this.Controls.Add(this.List2);
            this.Controls.Add(this.btnConfirmation);
            this.Controls.Add(this.ComboBox2);
            this.Controls.Add(this.btnResidence);
            this.Controls.Add(this.ComboBox1);
            this.Controls.Add(this.cbxIllegitRel);
            this.Controls.Add(this.lstChildren);
            this.Controls.Add(this.lblHdrBorn_Name);
            this.Controls.Add(this.lblHdrList);
            this.Controls.Add(this.lblEstimatedMarr);
            this.Controls.Add(this.lblPartnership);
            this.Controls.Add(this.lblReligMarr);
            this.Controls.Add(this.lblEngagement);
            this.Controls.Add(this.lblSources);
            this.Controls.Add(this.lblPictures);
            this.Controls.Add(this.lblChildCount);
            this.Controls.Add(this.lblCommonFamName);
            this.Controls.Add(this.lblDimissorale);
            this.Controls.Add(this.lblDivorce);
            this.Controls.Add(this.lblMarriage);
            this.Controls.Add(this.lblProklamation);
            this.Controls.Add(this.Label25);
            this.Controls.Add(this.Label24);
            this.Controls.Add(this.lblFamNr);
            this.Controls.Add(this.lblFamName);
            this.Controls.Add(this.lblMandant);
            this.Controls.Add(this.btnChildren);
            this.Controls.Add(this.btnResarch);
            this.Controls.Add(this.btnSearchNumber2);
            this.Controls.Add(this.btnFamilysheet);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnMainmenue);
            this.Controls.Add(this.btnSearchRegister);
            this.Controls.Add(this.btnSearchPartner);
            this.Controls.Add(this.btnEnableCheck);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnReenter);
            this.Controls.Add(this.btnSearchName);
            this.Controls.Add(this.btnSearchNumber);
            this.Controls.Add(this.Famsortlist);
            this.Controls.Add(this.edtNameprefix);
            this.Controls.Add(this.edtNameSuffix);
            this.Controls.Add(this.CheckBox2);
            this.Controls.Add(this.fraFather);
            this.Controls.Add(this.fraMother);
            this.Font = new System.Drawing.Font("Arial", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Familie";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.frmMarriage.ResumeLayout(false);
            this.frmFamilyresidence.ResumeLayout(false);
            this.frmAppendPerson.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }


    private Panel panel1;
    public FraParentView fraMother;
}