using GenFree.Data;
using GenFree.ViewModels.Interfaces;
using Microsoft.VisualBasic.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Personen
{
    /// <summary> 
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(Disposing);
    }

    public ToolTip ToolTip1;
    public Timer Timer1;
    public PictureBox Picture1;

    [VisibilityBinding(nameof(IPersonenViewModel.Dublicates_Visible))]
    public GroupBox frmDublicates;
    public ListBox lstDuplicates;
    public Label lblDuplLabel6;
    public Label lblDuplDisplay;

    public ListBox Sortlist;
    public ListBox List4;


    public Button btnCancel1;

    [ListBinding(nameof(IPersonenViewModel.Occupation_Items), nameof(IPersonenViewModel.Occupation_SelectedItem))]
    [DblClickBinding(nameof(IPersonenViewModel.OccupationCommand))]
    public ComboBox cbxOccupation;
    [ListBinding(nameof(IPersonenViewModel.Title_Items), nameof(IPersonenViewModel.Title_SelectedItem))]
    [DblClickBinding(nameof(IPersonenViewModel.TitleCommand))]
    public ComboBox cbxTitle;
    [ListBinding(nameof(IPersonenViewModel.Residence_Items), nameof(IPersonenViewModel.Residence_SelectedItem))]
    [DblClickBinding(nameof(IPersonenViewModel.ResidenceCommand))]
    public ComboBox cbxResidence;
    [ListBinding(nameof(IPersonenViewModel.Home_Items), nameof(IPersonenViewModel.Home_SelectedItem))]
    public ComboBox cbxHome;
    [ListBinding(nameof(IPersonenViewModel.Additional_Items), nameof(IPersonenViewModel.Additional_SelectedItem))]
    [DblClickBinding(nameof(IPersonenViewModel.AdditionalCommand))]
    public ComboBox cbxAdditional;

    [TextBinding(nameof(IPersonenViewModel.Notes_Text))]
    public RichTextBox edtNotes;

    [CommandBinding(nameof(IPersonenViewModel.ReturnCommand))]
    public Button btnReturn; // Index:0
    [VisibilityBinding(nameof(IPersonenViewModel.SearchNumber_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.SearchNumberCommand))]
    public Button btnSearchNumber;// Index:1
    [VisibilityBinding(nameof(IPersonenViewModel.NewPerson_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.NewPersonCommand))]
    public Button btnNewPerson;// Index:2
    [VisibilityBinding(nameof(IPersonenViewModel.Next_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.NextCommand))]
    public Button btnNext;// Index:3
    [VisibilityBinding(nameof(IPersonenViewModel.Previous_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.PreviousCommand))]
    public Button btnPrevious;// Index:4
    [VisibilityBinding(nameof(IPersonenViewModel.Delete_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.DeleteCommand))]
    public Button btnDelete;// Index:5
    [VisibilityBinding(nameof(IPersonenViewModel.GodparentIfNo_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.GodparentIfNoCommand))]
    public Button btnGodparentIfNo;// Index:7
    [VisibilityBinding(nameof(IPersonenViewModel.NoGodparents_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.NoGodparentsCommand))]
    public Button btnNoGodparents;// Index:9
    [VisibilityBinding(nameof(IPersonenViewModel.EndTextInput_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.EndTextInputCommand))]
    public Button btnEndTextInput;// Index:10
    [VisibilityBinding(nameof(IPersonenViewModel.SaveNExit_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.SaveNExitCommand))]
    public Button btnSaveNExit;// Index:11
    [VisibilityBinding(nameof(IPersonenViewModel.SaveToFamily_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.SaveToFamilyCommand))]
    public Button btnSaveToFamily;// Index:12
    [VisibilityBinding(nameof(IPersonenViewModel.SearchAncestors_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.SearchAncestorsCommand))]
    public Button btnSearchAncestors;// Index:13
    [VisibilityBinding(nameof(IPersonenViewModel.NewFamily_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.NewFamilyCommand))]
    public Button btnNewFamily;// Index:14
    [VisibilityBinding(nameof(IPersonenViewModel.ShowPerson_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.ShowPersonCommand))]
    public Button btnShowPerson;// Index:15
    [VisibilityBinding(nameof(IPersonenViewModel.NoSources_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.NoSourcesCommand))]
    public Button btnNoSources;// Index:17
    [VisibilityBinding(nameof(IPersonenViewModel.LinkedPerson_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.LinkedPersonCommand))]
    public Button btnLinkedPerson;// Index:18
    [VisibilityBinding(nameof(IPersonenViewModel.LinkTo_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.LinkToCommand))]
    public Button btnLinkTo;// Index:19
    [VisibilityBinding(nameof(IPersonenViewModel.SearchName_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.SearchNameCommand))]
    public Button btnSearchName;// Index:20
    [VisibilityBinding(nameof(IPersonenViewModel.PrintScreen_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.PrintScreenCommand))]
    public Button btnPrintScreen;// Index:21
    [VisibilityBinding(nameof(IPersonenViewModel.NoWitnesses_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.NoWitnessesCommand))]
    public Button btnNoWitnesses;// Index:22
    [VisibilityBinding(nameof(IPersonenViewModel.WitnessIfNo_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.WitnessIfNoCommand))]
    public Button btnWitnessIfNo;// Index:23
    [VisibilityBinding(nameof(IPersonenViewModel.Resarch_Visible))]
    [CommandBinding(nameof(IPersonenViewModel.ResarchCommand))]
    public Button btnResarch;// Index:24

    [TextBinding(nameof(IPersonenViewModel.Sex_Text))]
    public TextBox edtSex;
    [TextBinding(nameof(IPersonenViewModel.Religion_Text))]
    public TextBox edtReligion;
    [TextBinding(nameof(IPersonenViewModel.Prefix_Text))]
    public TextBox edtPrefix;
    [TextBinding(nameof(IPersonenViewModel.Suffix_Text))]
    public TextBox edtSuffix;
    [TextBinding(nameof(IPersonenViewModel.Clan_Text))]
    public TextBox edtClan;
    [TextBinding(nameof(IPersonenViewModel.Status_Text))]
    public TextBox edtStatus;
    public ListBox List2;
    public ListBox List1;
    public GroupBox frmPicture;
    [TextBinding(nameof(IPersonenViewModel.Search_Text))]
    public TextBox edtSearch;
    [TextBinding(nameof(IPersonenViewModel.Search2_Text))]
    public TextBox edtSearch2;
    [TextBinding(nameof(IPersonenViewModel.Search3_Text))]
    public TextBox edtSearch3;

    public ComboBox cbxSorting;

    [CommandBinding(nameof(IPersonenViewModel.TitleCommand))]
    public Button btnTitle;
    [CommandBinding(nameof(IPersonenViewModel.OccupationCommand))]
    public Button btnOccupation;
    [CommandBinding(nameof(IPersonenViewModel.AdditionalCommand))]
    public Button btnAdditional;
    [CommandBinding(nameof(IPersonenViewModel.ResidenceCommand))]
    public Button btnResidence;
    [CommandBinding(nameof(IPersonenViewModel.HometownCommand))]
    public Button btnHometown;

    public Button btnAdoption;
    public Button btnFieldsOFB;
    [TextBinding(nameof(IPersonenViewModel.Givennames_Text))]
    [EnabledBinding(nameof(IPersonenViewModel.Givennames_Enabled))]
    public TextBox edtGivennames;

    public Label lblSearch;
    public Label lblSorting;
    [TextBinding(nameof(IPersonenViewModel.NachfNr))]
    public Label lblNachfNr;
    [TextBinding(nameof(IPersonenViewModel.PersonNr))]
    public Label lblPersonNr;
    [TextBinding(nameof(IPersonenViewModel.Marriages_Text))]
    public Label lblMarriages;
    public Label lblOther;
    public Label lblResidence;
    public Label lblSuffix;
    public Label lblOccupation;
    public Label lblClan;
    public Label lblPrefix;
    public Label lblAlias;
    public Label lblMandant;
    public Label lblGivennames;
    public Label lblReligion;
    [TextBinding(nameof(IPersonenViewModel.Age_Text))]
    public Label lblAge;

    public Label lblAncesterNr;
    public Label lblSearch2;
    public Label lblSex;
    public Label lblSurname;
    public Label lblState;
    [TextBinding(nameof(IPersonenViewModel.Family1_Text))]
    public Label lblFamily1;
    [TextBinding(nameof(IPersonenViewModel.CreationDate_Text))]
    public Label lblCreationDate;
    internal Label lblRemark;
    [TextBinding(nameof(IPersonenViewModel.FamNr))]
    internal Label lblFamPers;
    [TextBinding(nameof(IPersonenViewModel.Alias_Text))]
    internal TextBox edtAlias;

    public Button btnCancel2;
    internal Button btnRegisterSearch;
    internal Button btnDuplBttn3;
    internal Button btnDuplClose;
    [CommandBinding(nameof(IPersonenViewModel.ResidenceCommand))]
    internal Label lblResidenceDisp;
    [CommandBinding(nameof(IPersonenViewModel.OccupationCommand))]
    internal Label lblOccubation;
    [CommandBinding(nameof(IPersonenViewModel.TitleCommand))]
    internal Label lblTitelDisp;
    [CommandBinding(nameof(IPersonenViewModel.AdditionalCommand))]
    internal Label lblAdditDisp;
    [CommandBinding(nameof(IPersonenViewModel.HometownCommand))]
    internal Label lblHomeDisp;
    public Label lblPredicate;
    public Label lblNickname;
    internal Label Label15;

    [TextBinding(nameof(IPersonenViewModel.Predicate_Text))]
    public TextBox edtPredicate;
    [TextBinding(nameof(IPersonenViewModel.Nickname_Text))]
    public TextBox edtNickname;
    [TextBinding(nameof(IPersonenViewModel.Surnames_Text))]
    internal TextBox edtSurnames;
    public ComboBox cbxProperty;

    public GroupBox frmReenter;
    public Button btnReenter2;
    public Button btnEdit;

    [CommandBinding(nameof(IPersonenViewModel.PropertyCommand))]
    public Button btnProperty;
    public Button btnCancel;
    public Button btnCancel4;
    [CommandBinding(nameof(IPersonenViewModel.ShowPlacesCommand))]
    public Button btnShowPlaces;
    internal ListBox List3;

    internal ComboBox cbxWitnes;
    [VisibilityBinding(nameof(IPersonenViewModel.PersImpQuerry1_Visible))]
    public FraPersImpQuerry fraPersImpQuerry1;
    public FraEventShowEdit fraEventShowEdit1;
    public FraEventShowEdit fraEventShowEdit2;
    public FraEventShowEdit fraEventShowEdit3;
    public FraEventShowEdit fraEventShowEdit4;


    [DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.btnSearchAncestors = new System.Windows.Forms.Button();
            this.btnNewPerson = new System.Windows.Forms.Button();
            this.btnSearchNumber = new System.Windows.Forms.Button();
            this.btnSearchName = new System.Windows.Forms.Button();
            this.btnNoWitnesses = new System.Windows.Forms.Button();
            this.btnWitnessIfNo = new System.Windows.Forms.Button();
            this.btnNoSources = new System.Windows.Forms.Button();
            this.btnNewFamily = new System.Windows.Forms.Button();
            this.btnDuplClose = new System.Windows.Forms.Button();
            this.btnDuplBttn3 = new System.Windows.Forms.Button();
            this.btnReenter2 = new System.Windows.Forms.Button();
            this.btnCancel4 = new System.Windows.Forms.Button();
            this.btnPrintScreen = new System.Windows.Forms.Button();
            this.btnCancel1 = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnShowPerson = new System.Windows.Forms.Button();
            this.btnEndTextInput = new System.Windows.Forms.Button();
            this.btnReturn = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNoGodparents = new System.Windows.Forms.Button();
            this.btnGodparentIfNo = new System.Windows.Forms.Button();
            this.btnLinkTo = new System.Windows.Forms.Button();
            this.btnLinkedPerson = new System.Windows.Forms.Button();
            this.btnSaveNExit = new System.Windows.Forms.Button();
            this.btnSaveToFamily = new System.Windows.Forms.Button();
            this.btnTitle = new System.Windows.Forms.Button();
            this.btnOccupation = new System.Windows.Forms.Button();
            this.btnAdditional = new System.Windows.Forms.Button();
            this.btnResidence = new System.Windows.Forms.Button();
            this.btnAdoption = new System.Windows.Forms.Button();
            this.btnFieldsOFB = new System.Windows.Forms.Button();
            this.btnResarch = new System.Windows.Forms.Button();
            this.btnRegisterSearch = new System.Windows.Forms.Button();
            this.btnHometown = new System.Windows.Forms.Button();
            this.cbxHome = new System.Windows.Forms.ComboBox();
            this.cbxAdditional = new System.Windows.Forms.ComboBox();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnCancel2 = new System.Windows.Forms.Button();
            this.btnProperty = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnShowPlaces = new System.Windows.Forms.Button();

            this.cbxTitle = new System.Windows.Forms.ComboBox();
            this.cbxResidence = new System.Windows.Forms.ComboBox();
            this.cbxOccupation = new System.Windows.Forms.ComboBox();
            this.cbxSorting = new System.Windows.Forms.ComboBox();
            this.cbxProperty = new System.Windows.Forms.ComboBox();
            this.cbxWitnes = new System.Windows.Forms.ComboBox();

            this.edtStatus = new System.Windows.Forms.TextBox();
            this.edtNotes = new System.Windows.Forms.RichTextBox();
            this.edtSex = new System.Windows.Forms.TextBox();
            this.edtClan = new System.Windows.Forms.TextBox();
            this.edtPrefix = new System.Windows.Forms.TextBox();
            this.edtSuffix = new System.Windows.Forms.TextBox();
            this.edtGivennames = new System.Windows.Forms.TextBox();
            this.edtReligion = new System.Windows.Forms.TextBox();
            this.edtSearch = new System.Windows.Forms.TextBox();
            this.edtSearch2 = new System.Windows.Forms.TextBox();
            this.edtSearch3 = new System.Windows.Forms.TextBox();
            this.edtAlias = new System.Windows.Forms.TextBox();
            this.edtPredicate = new System.Windows.Forms.TextBox();
            this.edtNickname = new System.Windows.Forms.TextBox();
            this.edtSurnames = new System.Windows.Forms.TextBox();

            this.Picture1 = new System.Windows.Forms.PictureBox();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);

            this.lstDuplicates = new System.Windows.Forms.ListBox();
            this.List3 = new System.Windows.Forms.ListBox();
            this.Sortlist = new System.Windows.Forms.ListBox();
            this.List4 = new System.Windows.Forms.ListBox();
            this.List2 = new System.Windows.Forms.ListBox();
            this.List1 = new System.Windows.Forms.ListBox();

            this.frmDublicates = new System.Windows.Forms.GroupBox();
            this.frmReenter = new System.Windows.Forms.GroupBox();
            this.frmPicture = new System.Windows.Forms.GroupBox();

            this.lblDuplLabel6 = new System.Windows.Forms.Label();
            this.lblDuplDisplay = new System.Windows.Forms.Label();
            this.lblSorting = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.lblNachfNr = new System.Windows.Forms.Label();
            this.lblPersonNr = new System.Windows.Forms.Label();
            this.lblMarriages = new System.Windows.Forms.Label();
            this.lblOther = new System.Windows.Forms.Label();
            this.lblResidence = new System.Windows.Forms.Label();
            this.lblSuffix = new System.Windows.Forms.Label();
            this.lblOccupation = new System.Windows.Forms.Label();
            this.lblClan = new System.Windows.Forms.Label();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.lblAlias = new System.Windows.Forms.Label();
            this.lblMandant = new System.Windows.Forms.Label();
            this.lblGivennames = new System.Windows.Forms.Label();
            this.lblSex = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblAncesterNr = new System.Windows.Forms.Label();
            this.lblSearch2 = new System.Windows.Forms.Label();
            this.lblReligion = new System.Windows.Forms.Label();
            this.lblSurname = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lblFamily1 = new System.Windows.Forms.Label();
            this.lblCreationDate = new System.Windows.Forms.Label();
            this.lblRemark = new System.Windows.Forms.Label();
            this.lblFamPers = new System.Windows.Forms.Label();
            this.lblResidenceDisp = new System.Windows.Forms.Label();
            this.lblOccubation = new System.Windows.Forms.Label();
            this.lblTitelDisp = new System.Windows.Forms.Label();
            this.lblAdditDisp = new System.Windows.Forms.Label();
            this.lblHomeDisp = new System.Windows.Forms.Label();
            this.lblPredicate = new System.Windows.Forms.Label();
            this.lblNickname = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();

            this.fraEventShowEdit1 = new Gen_FreeWin.Views.FraEventShowEdit(_viewModel.BirthEVM, Modul1.IText);
            this.fraEventShowEdit2 = new Gen_FreeWin.Views.FraEventShowEdit(_viewModel.BaptismEVM, Modul1.IText);
            this.fraEventShowEdit3 = new Gen_FreeWin.Views.FraEventShowEdit(_viewModel.DeathEVM, Modul1.IText);
            this.fraEventShowEdit4 = new Gen_FreeWin.Views.FraEventShowEdit(_viewModel.BurialEVM, Modul1.IText);
            this.fraPersImpQuerry1 = new Gen_FreeWin.Views.FraPersImpQuerry(_viewModel.FraPersImpQuerryViewModel,Modul1.IText);
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).BeginInit();
            this.frmDublicates.SuspendLayout();
            this.frmReenter.SuspendLayout();
            this.frmPicture.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearchAncestors
            // 
            this.btnSearchAncestors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchAncestors.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSearchAncestors.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSearchAncestors.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSearchAncestors.Location = new System.Drawing.Point(2, 680);
            this.btnSearchAncestors.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnSearchAncestors.Name = "btnSearchAncestors";
            this.btnSearchAncestors.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSearchAncestors.Size = new System.Drawing.Size(151, 27);
            this.btnSearchAncestors.TabIndex = 64;
            this.btnSearchAncestors.Text = "&Ahnensuche";
            this.btnSearchAncestors.UseVisualStyleBackColor = false;
            // 
            // btnNewPerson
            // 
            this.btnNewPerson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNewPerson.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNewPerson.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNewPerson.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNewPerson.Location = new System.Drawing.Point(544, 680);
            this.btnNewPerson.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNewPerson.Name = "btnNewPerson";
            this.btnNewPerson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNewPerson.Size = new System.Drawing.Size(105, 27);
            this.btnNewPerson.TabIndex = 30;
            this.btnNewPerson.Text = "&neue Person eingeben";
            this.btnNewPerson.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNewPerson.UseVisualStyleBackColor = false;
            // 
            // btnSearchNumber
            // 
            this.btnSearchNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchNumber.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSearchNumber.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSearchNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSearchNumber.Location = new System.Drawing.Point(2, 650);
            this.btnSearchNumber.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnSearchNumber.Name = "btnSearchNumber";
            this.btnSearchNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSearchNumber.Size = new System.Drawing.Size(132, 27);
            this.btnSearchNumber.TabIndex = 29;
            this.btnSearchNumber.Text = "suchen N&ummer";
            this.btnSearchNumber.UseVisualStyleBackColor = false;
            // 
            // btnSearchName
            // 
            this.btnSearchName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchName.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSearchName.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSearchName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSearchName.Location = new System.Drawing.Point(147, 650);
            this.btnSearchName.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnSearchName.Name = "btnSearchName";
            this.btnSearchName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSearchName.Size = new System.Drawing.Size(133, 27);
            this.btnSearchName.TabIndex = 85;
            this.btnSearchName.Text = "su&chen Name";
            this.btnSearchName.UseVisualStyleBackColor = false;
            // 
            // btnNoWitnesses
            // 
            this.btnNoWitnesses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnNoWitnesses.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNoWitnesses.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNoWitnesses.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNoWitnesses.Location = new System.Drawing.Point(613, 148);
            this.btnNoWitnesses.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNoWitnesses.Name = "btnNoWitnesses";
            this.btnNoWitnesses.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNoWitnesses.Size = new System.Drawing.Size(95, 27);
            this.btnNoWitnesses.TabIndex = 97;
            this.btnNoWitnesses.Text = "&Zeugen nein";
            this.btnNoWitnesses.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNoWitnesses.UseVisualStyleBackColor = false;
            // 
            // btnWitnessIfNo
            // 
            this.btnWitnessIfNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnWitnessIfNo.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnWitnessIfNo.Font = new System.Drawing.Font("Arial", 9F);
            this.btnWitnessIfNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnWitnessIfNo.Location = new System.Drawing.Point(713, 147);
            this.btnWitnessIfNo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnWitnessIfNo.Name = "btnWitnessIfNo";
            this.btnWitnessIfNo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnWitnessIfNo.Size = new System.Drawing.Size(113, 27);
            this.btnWitnessIfNo.TabIndex = 98;
            this.btnWitnessIfNo.Text = "Zeuge bei nein";
            this.btnWitnessIfNo.UseVisualStyleBackColor = false;
            // 
            // edtStatus
            // 
            this.edtStatus.AcceptsReturn = true;
            this.edtStatus.BackColor = System.Drawing.Color.White;
            this.edtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtStatus.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtStatus.Font = new System.Drawing.Font("Arial", 9F);
            this.edtStatus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtStatus.Location = new System.Drawing.Point(821, 0);
            this.edtStatus.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtStatus.MaxLength = 0;
            this.edtStatus.Name = "edtStatus";
            this.edtStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtStatus.Size = new System.Drawing.Size(197, 21);
            this.edtStatus.TabIndex = 75;
            this.edtStatus.Text = "<Status>";
            // 
            // btnNoSources
            // 
            this.btnNoSources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnNoSources.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNoSources.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNoSources.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNoSources.Location = new System.Drawing.Point(690, 88);
            this.btnNoSources.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNoSources.Name = "btnNoSources";
            this.btnNoSources.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNoSources.Size = new System.Drawing.Size(136, 27);
            this.btnNoSources.TabIndex = 70;
            this.btnNoSources.Text = "Keine Quellle";
            this.btnNoSources.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNoSources.UseVisualStyleBackColor = false;
            // 
            // btnNewFamily
            // 
            this.btnNewFamily.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNewFamily.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNewFamily.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNewFamily.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNewFamily.Location = new System.Drawing.Point(384, 680);
            this.btnNewFamily.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNewFamily.Name = "btnNewFamily";
            this.btnNewFamily.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNewFamily.Size = new System.Drawing.Size(158, 27);
            this.btnNewFamily.TabIndex = 67;
            this.btnNewFamily.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNewFamily.UseVisualStyleBackColor = false;
            // 
            // Picture1
            // 
            this.Picture1.BackColor = System.Drawing.SystemColors.Control;
            this.Picture1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Picture1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Picture1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Picture1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Picture1.Location = new System.Drawing.Point(5, 30);
            this.Picture1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Picture1.Name = "Picture1";
            this.Picture1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Picture1.Size = new System.Drawing.Size(175, 205);
            this.Picture1.TabIndex = 110;
            this.Picture1.TabStop = false;
            // 
            // frmDublicates
            // 
            this.frmDublicates.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.frmDublicates.Controls.Add(this.lstDuplicates);
            this.frmDublicates.Controls.Add(this.btnDuplClose);
            this.frmDublicates.Controls.Add(this.btnDuplBttn3);
            this.frmDublicates.Controls.Add(this.lblDuplLabel6);
            this.frmDublicates.Controls.Add(this.lblDuplDisplay);
            this.frmDublicates.ForeColor = System.Drawing.SystemColors.ControlText;
            this.frmDublicates.Location = new System.Drawing.Point(157, 186);
            this.frmDublicates.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmDublicates.Name = "frmDublicates";
            this.frmDublicates.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmDublicates.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmDublicates.Size = new System.Drawing.Size(804, 484);
            this.frmDublicates.TabIndex = 99;
            this.frmDublicates.TabStop = false;
            this.frmDublicates.Text = "Dublettenkontrolle";
            this.frmDublicates.Visible = false;
            // 
            // lstDuplicates
            // 
            this.lstDuplicates.BackColor = System.Drawing.SystemColors.Window;
            this.lstDuplicates.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstDuplicates.Font = new System.Drawing.Font("Courier New", 8.5F);
            this.lstDuplicates.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstDuplicates.ItemHeight = 20;
            this.lstDuplicates.Location = new System.Drawing.Point(2, 70);
            this.lstDuplicates.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lstDuplicates.Name = "List5";
            this.lstDuplicates.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstDuplicates.Size = new System.Drawing.Size(792, 344);
            this.lstDuplicates.TabIndex = 100;
            // 
            // btnDuplClose
            // 
            this.btnDuplClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDuplClose.Location = new System.Drawing.Point(496, 10);
            this.btnDuplClose.Name = "btnDuplClose";
            this.btnDuplClose.Size = new System.Drawing.Size(97, 24);
            this.btnDuplClose.TabIndex = 110;
            this.btnDuplClose.Text = "Schließen";
            this.btnDuplClose.UseVisualStyleBackColor = false;
            // 
            // btnDuplBttn3
            // 
            this.btnDuplBttn3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDuplBttn3.Location = new System.Drawing.Point(13, 22);
            this.btnDuplBttn3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnDuplBttn3.Name = "btnDuplBttn3";
            this.btnDuplBttn3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDuplBttn3.Size = new System.Drawing.Size(138, 28);
            this.btnDuplBttn3.TabIndex = 58;
            this.btnDuplBttn3.Text = "&neu eingeben";
            this.btnDuplBttn3.UseVisualStyleBackColor = false;
            // 
            // lblEMail
            // 
            this.lblDuplLabel6.BackColor = System.Drawing.SystemColors.Control;
            this.lblDuplLabel6.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDuplLabel6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDuplLabel6.Location = new System.Drawing.Point(11, 43);
            this.lblDuplLabel6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblDuplLabel6.Name = "Label6";
            this.lblDuplLabel6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDuplLabel6.Size = new System.Drawing.Size(261, 23);
            this.lblDuplLabel6.TabIndex = 106;
            // 
            // lblDisplayHint
            // 
            this.lblDuplDisplay.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblDuplDisplay.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDuplDisplay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDuplDisplay.Location = new System.Drawing.Point(11, 18);
            this.lblDuplDisplay.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblDuplDisplay.Name = "lblDisplayHint";
            this.lblDuplDisplay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblDuplDisplay.Size = new System.Drawing.Size(261, 21);
            this.lblDuplDisplay.TabIndex = 105;
            this.lblDuplDisplay.Text = "Anzeige:";
            // 
            // frmReenter
            // 
            this.frmReenter.BackColor = System.Drawing.Color.Red;
            this.frmReenter.Controls.Add(this.btnReenter2);
            this.frmReenter.Controls.Add(this.btnEdit);
            this.frmReenter.ForeColor = System.Drawing.Color.White;
            this.frmReenter.Location = new System.Drawing.Point(161, 416);
            this.frmReenter.Name = "frmReenter";
            this.frmReenter.Size = new System.Drawing.Size(419, 136);
            this.frmReenter.TabIndex = 63;
            this.frmReenter.TabStop = false;
            // 
            // btnReenter2
            // 
            this.btnReenter2.BackColor = System.Drawing.SystemColors.Control;
            this.btnReenter2.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnReenter2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReenter2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReenter2.Location = new System.Drawing.Point(13, 22);
            this.btnReenter2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnReenter2.Name = "btnReenter2";
            this.btnReenter2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnReenter2.Size = new System.Drawing.Size(138, 28);
            this.btnReenter2.TabIndex = 58;
            this.btnReenter2.Text = "&neu eingeben";
            this.btnReenter2.UseVisualStyleBackColor = false;
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEdit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEdit.Location = new System.Drawing.Point(215, 22);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEdit.Size = new System.Drawing.Size(155, 28);
            this.btnEdit.TabIndex = 57;
            this.btnEdit.Text = "bearbeiten";
            this.btnEdit.UseVisualStyleBackColor = false;
            // 
            // btnCancel4
            // 
            this.btnCancel4.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel4.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel4.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCancel4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel4.Location = new System.Drawing.Point(123, 72);
            this.btnCancel4.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCancel4.Name = "btnCancel4";
            this.btnCancel4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel4.Size = new System.Drawing.Size(121, 28);
            this.btnCancel4.TabIndex = 61;
            this.btnCancel4.Text = "abbrechen";
            this.btnCancel4.UseVisualStyleBackColor = false;
            // 
            // List3
            // 
            this.List3.BackColor = System.Drawing.SystemColors.Window;
            this.List3.Cursor = System.Windows.Forms.Cursors.Default;
            this.List3.Font = new System.Drawing.Font("Arial", 8.5F);
            this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List3.ItemHeight = 19;
            this.List3.Location = new System.Drawing.Point(630, 427);
            this.List3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List3.Name = "List3";
            this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List3.Size = new System.Drawing.Size(241, 574);
            this.List3.TabIndex = 44;
            this.List3.Visible = false;
            // 
            // Sortlist
            // 
            this.Sortlist.BackColor = System.Drawing.SystemColors.Window;
            this.Sortlist.Cursor = System.Windows.Forms.Cursors.Default;
            this.Sortlist.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Sortlist.ItemHeight = 25;
            this.Sortlist.Location = new System.Drawing.Point(903, 755);
            this.Sortlist.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Sortlist.Name = "Sortlist";
            this.Sortlist.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Sortlist.Size = new System.Drawing.Size(77, 29);
            this.Sortlist.Sorted = true;
            this.Sortlist.TabIndex = 96;
            this.Sortlist.Visible = false;
            // 
            // List4
            // 
            this.List4.BackColor = System.Drawing.SystemColors.Window;
            this.List4.Cursor = System.Windows.Forms.Cursors.Default;
            this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List4.ItemHeight = 25;
            this.List4.Location = new System.Drawing.Point(278, 574);
            this.List4.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List4.Name = "List4";
            this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List4.Size = new System.Drawing.Size(245, 279);
            this.List4.TabIndex = 93;
            this.List4.Visible = false;
            // 
            // btnPrintScreen
            // 
            this.btnPrintScreen.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrintScreen.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnPrintScreen.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPrintScreen.Location = new System.Drawing.Point(677, 763);
            this.btnPrintScreen.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnPrintScreen.Name = "btnPrintScreen";
            this.btnPrintScreen.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnPrintScreen.Size = new System.Drawing.Size(136, 18);
            this.btnPrintScreen.TabIndex = 88;
            this.btnPrintScreen.Text = "Bildschirm drucken";
            this.btnPrintScreen.UseVisualStyleBackColor = false;
            this.btnPrintScreen.Visible = false;
            // 
            // btnCancel1
            // 
            this.btnCancel1.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel1.Location = new System.Drawing.Point(587, 816);
            this.btnCancel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCancel1.Name = "btnCancel1";
            this.btnCancel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel1.Size = new System.Drawing.Size(107, 36);
            this.btnCancel1.TabIndex = 86;
            this.btnCancel1.Text = "Abbrechen";
            this.btnCancel1.UseVisualStyleBackColor = false;
            this.btnCancel1.Visible = false;
            // 
            // edtNotes
            // 
            this.edtNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edtNotes.Font = new System.Drawing.Font("Arial", 9F);
            this.edtNotes.Location = new System.Drawing.Point(0, 533);
            this.edtNotes.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtNotes.Name = "edtNotes";
            this.edtNotes.Size = new System.Drawing.Size(1010, 109);
            this.edtNotes.TabIndex = 71;
            this.edtNotes.Text = "";
            // 
            // cbxHome
            // 
            this.cbxHome.BackColor = System.Drawing.SystemColors.Window;
            this.cbxHome.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxHome.Font = new System.Drawing.Font("Arial", 9F);
            this.cbxHome.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxHome.Location = new System.Drawing.Point(133, 312);
            this.cbxHome.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxHome.Name = "cbxHome";
            this.cbxHome.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxHome.Size = new System.Drawing.Size(516, 29);
            this.cbxHome.Sorted = true;
            this.cbxHome.TabIndex = 73;
            this.cbxHome.Text = "<Heimatort>";
            // 
            // cbxAdditional
            // 
            this.cbxAdditional.BackColor = System.Drawing.SystemColors.Window;
            this.cbxAdditional.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxAdditional.Font = new System.Drawing.Font("Arial", 9F);
            this.cbxAdditional.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxAdditional.Location = new System.Drawing.Point(102, 255);
            this.cbxAdditional.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxAdditional.Name = "cbxAdditional";
            this.cbxAdditional.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxAdditional.Size = new System.Drawing.Size(547, 29);
            this.cbxAdditional.Sorted = true;
            this.cbxAdditional.TabIndex = 72;
            this.cbxAdditional.Text = "<Sonstiges>";
            // 
            // btnShowPerson
            // 
            this.btnShowPerson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnShowPerson.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnShowPerson.Font = new System.Drawing.Font("Arial", 9F);
            this.btnShowPerson.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnShowPerson.Location = new System.Drawing.Point(651, 650);
            this.btnShowPerson.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnShowPerson.Name = "btnShowPerson";
            this.btnShowPerson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnShowPerson.Size = new System.Drawing.Size(128, 27);
            this.btnShowPerson.TabIndex = 68;
            this.btnShowPerson.Text = "Personenblat&t";
            this.btnShowPerson.UseVisualStyleBackColor = false;
            // 
            // cbxTitle
            // 
            this.cbxTitle.BackColor = System.Drawing.SystemColors.Window;
            this.cbxTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxTitle.Font = new System.Drawing.Font("Arial", 9F);
            this.cbxTitle.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxTitle.Location = new System.Drawing.Point(89, 227);
            this.cbxTitle.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxTitle.Name = "cbxTitle";
            this.cbxTitle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxTitle.Size = new System.Drawing.Size(560, 29);
            this.cbxTitle.Sorted = true;
            this.cbxTitle.TabIndex = 60;
            this.cbxTitle.Text = "<Titel>";
            // 
            // cbxResidence
            // 
            this.cbxResidence.BackColor = System.Drawing.SystemColors.Window;
            this.cbxResidence.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxResidence.Font = new System.Drawing.Font("Arial", 9F);
            this.cbxResidence.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxResidence.Location = new System.Drawing.Point(102, 283);
            this.cbxResidence.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxResidence.Name = "cbxResidence";
            this.cbxResidence.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxResidence.Size = new System.Drawing.Size(547, 29);
            this.cbxResidence.Sorted = true;
            this.cbxResidence.TabIndex = 59;
            this.cbxResidence.Text = "<Wohnort(e)>";
            // 
            // cbxOccupation
            // 
            this.cbxOccupation.BackColor = System.Drawing.Color.Gainsboro;
            this.cbxOccupation.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxOccupation.Font = new System.Drawing.Font("Arial", 9F);
            this.cbxOccupation.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxOccupation.Location = new System.Drawing.Point(89, 202);
            this.cbxOccupation.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxOccupation.Name = "cbxOccupation";
            this.cbxOccupation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxOccupation.Size = new System.Drawing.Size(560, 29);
            this.cbxOccupation.Sorted = true;
            this.cbxOccupation.TabIndex = 55;
            this.cbxOccupation.Text = "<Beruf>";
            // 
            // btnEndTextInput
            // 
            this.btnEndTextInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnEndTextInput.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEndTextInput.Font = new System.Drawing.Font("Arial", 9F);
            this.btnEndTextInput.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEndTextInput.Location = new System.Drawing.Point(14, 679);
            this.btnEndTextInput.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnEndTextInput.Name = "btnEndTextInput";
            this.btnEndTextInput.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEndTextInput.Size = new System.Drawing.Size(166, 27);
            this.btnEndTextInput.TabIndex = 52;
            this.btnEndTextInput.Text = "&Texteingabe beenden";
            this.btnEndTextInput.UseVisualStyleBackColor = false;
            this.btnEndTextInput.Visible = false;
            // 
            // btnReturn
            // 
            this.btnReturn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnReturn.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnReturn.Font = new System.Drawing.Font("Arial", 9F);
            this.btnReturn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReturn.Location = new System.Drawing.Point(909, 651);
            this.btnReturn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnReturn.Size = new System.Drawing.Size(109, 27);
            this.btnReturn.TabIndex = 51;
            this.btnReturn.Text = "&Hauptmenü";
            this.btnReturn.UseVisualStyleBackColor = false;
            // 
            // edtSex
            // 
            this.edtSex.AcceptsReturn = true;
            this.edtSex.BackColor = System.Drawing.SystemColors.Window;
            this.edtSex.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtSex.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtSex.Font = new System.Drawing.Font("Arial", 9F);
            this.edtSex.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtSex.Location = new System.Drawing.Point(393, 23);
            this.edtSex.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtSex.MaxLength = 0;
            this.edtSex.Name = "edtSex";
            this.edtSex.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtSex.Size = new System.Drawing.Size(15, 21);
            this.edtSex.TabIndex = 35;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNext.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNext.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNext.Location = new System.Drawing.Point(533, 652);
            this.btnNext.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNext.Name = "btnNext";
            this.btnNext.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNext.Size = new System.Drawing.Size(110, 27);
            this.btnNext.TabIndex = 33;
            this.btnNext.Text = "&vorblättern";
            this.btnNext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnNext.UseVisualStyleBackColor = false;
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 9F);
            this.btnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDelete.Location = new System.Drawing.Point(651, 680);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDelete.Size = new System.Drawing.Size(105, 27);
            this.btnDelete.TabIndex = 32;
            this.btnDelete.Text = "&Löschen";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnPrevious
            // 
            this.btnPrevious.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPrevious.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnPrevious.Font = new System.Drawing.Font("Arial", 9F);
            this.btnPrevious.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPrevious.Location = new System.Drawing.Point(430, 650);
            this.btnPrevious.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnPrevious.Size = new System.Drawing.Size(101, 27);
            this.btnPrevious.TabIndex = 31;
            this.btnPrevious.Text = "&rückblättern";
            this.btnPrevious.UseVisualStyleBackColor = false;
            // 
            // edtClan
            // 
            this.edtClan.AcceptsReturn = true;
            this.edtClan.BackColor = System.Drawing.SystemColors.Window;
            this.edtClan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtClan.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtClan.Font = new System.Drawing.Font("Arial", 9F);
            this.edtClan.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtClan.Location = new System.Drawing.Point(418, 91);
            this.edtClan.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtClan.MaxLength = 0;
            this.edtClan.Multiline = true;
            this.edtClan.Name = "edtClan";
            this.edtClan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtClan.Size = new System.Drawing.Size(267, 18);
            this.edtClan.TabIndex = 23;
            this.edtClan.Text = "<Sippe>";
            // 
            // edtPrefix
            // 
            this.edtPrefix.AcceptsReturn = true;
            this.edtPrefix.BackColor = System.Drawing.SystemColors.Window;
            this.edtPrefix.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtPrefix.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtPrefix.Font = new System.Drawing.Font("Arial", 9F);
            this.edtPrefix.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtPrefix.Location = new System.Drawing.Point(442, 48);
            this.edtPrefix.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtPrefix.MaxLength = 0;
            this.edtPrefix.Name = "edtPrefix";
            this.edtPrefix.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtPrefix.Size = new System.Drawing.Size(100, 21);
            this.edtPrefix.TabIndex = 22;
            this.edtPrefix.Text = "<Präfix>";
            // 
            // edtSuffix
            // 
            this.edtSuffix.AcceptsReturn = true;
            this.edtSuffix.BackColor = System.Drawing.SystemColors.Window;
            this.edtSuffix.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtSuffix.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtSuffix.Font = new System.Drawing.Font("Arial", 9F);
            this.edtSuffix.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtSuffix.Location = new System.Drawing.Point(613, 48);
            this.edtSuffix.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtSuffix.MaxLength = 0;
            this.edtSuffix.Name = "edtSuffix";
            this.edtSuffix.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtSuffix.Size = new System.Drawing.Size(193, 21);
            this.edtSuffix.TabIndex = 21;
            this.edtSuffix.Text = "<Suffix>";
            // 
            // List2
            // 
            this.List2.BackColor = System.Drawing.SystemColors.Window;
            this.List2.Cursor = System.Windows.Forms.Cursors.Default;
            this.List2.Font = new System.Drawing.Font("Arial", 11F);
            this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List2.ItemHeight = 25;
            this.List2.Location = new System.Drawing.Point(513, 368);
            this.List2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List2.Name = "List2";
            this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List2.Size = new System.Drawing.Size(500, 129);
            this.List2.TabIndex = 20;
            // 
            // List1
            // 
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.Font = new System.Drawing.Font("Arial", 9F);
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 21;
            this.List1.Location = new System.Drawing.Point(3, 368);
            this.List1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List1.Size = new System.Drawing.Size(500, 109);
            this.List1.TabIndex = 19;
            // 
            // edtReligion
            // 
            this.edtReligion.AcceptsReturn = true;
            this.edtReligion.BackColor = System.Drawing.SystemColors.Window;
            this.edtReligion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtReligion.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtReligion.Font = new System.Drawing.Font("Arial", 9F);
            this.edtReligion.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtReligion.Location = new System.Drawing.Point(460, 23);
            this.edtReligion.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtReligion.MaxLength = 0;
            this.edtReligion.Name = "edtReligion";
            this.edtReligion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtReligion.Size = new System.Drawing.Size(95, 21);
            this.edtReligion.TabIndex = 7;
            this.edtReligion.Text = "<Rel>";
            // 
            // btnNoGodparents
            // 
            this.btnNoGodparents.AutoEllipsis = true;
            this.btnNoGodparents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnNoGodparents.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNoGodparents.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNoGodparents.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNoGodparents.Location = new System.Drawing.Point(613, 119);
            this.btnNoGodparents.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNoGodparents.Name = "btnNoGodparents";
            this.btnNoGodparents.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNoGodparents.Size = new System.Drawing.Size(95, 27);
            this.btnNoGodparents.TabIndex = 45;
            this.btnNoGodparents.Text = "Pa&ten nein";
            this.btnNoGodparents.UseVisualStyleBackColor = false;
            // 
            // btnGodparentIfNo
            // 
            this.btnGodparentIfNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnGodparentIfNo.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnGodparentIfNo.Font = new System.Drawing.Font("Arial", 9F);
            this.btnGodparentIfNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnGodparentIfNo.Location = new System.Drawing.Point(713, 119);
            this.btnGodparentIfNo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnGodparentIfNo.Name = "btnGodparentIfNo";
            this.btnGodparentIfNo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnGodparentIfNo.Size = new System.Drawing.Size(113, 27);
            this.btnGodparentIfNo.TabIndex = 47;
            this.btnGodparentIfNo.Text = "Pate &bei nein";
            this.btnGodparentIfNo.UseVisualStyleBackColor = false;
            // 
            // btnLinkTo
            // 
            this.btnLinkTo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLinkTo.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnLinkTo.Font = new System.Drawing.Font("Arial", 9F);
            this.btnLinkTo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLinkTo.Location = new System.Drawing.Point(651, 233);
            this.btnLinkTo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnLinkTo.Name = "btnLinkTo";
            this.btnLinkTo.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnLinkTo.Size = new System.Drawing.Size(175, 27);
            this.btnLinkTo.TabIndex = 77;
            this.btnLinkTo.Text = "verb. mit";
            this.btnLinkTo.UseVisualStyleBackColor = false;
            // 
            // btnLinkedPerson
            // 
            this.btnLinkedPerson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLinkedPerson.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnLinkedPerson.Font = new System.Drawing.Font("Arial", 9F);
            this.btnLinkedPerson.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLinkedPerson.Location = new System.Drawing.Point(651, 202);
            this.btnLinkedPerson.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnLinkedPerson.Name = "btnLinkedPerson";
            this.btnLinkedPerson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnLinkedPerson.Size = new System.Drawing.Size(175, 27);
            this.btnLinkedPerson.TabIndex = 76;
            this.btnLinkedPerson.Text = "verb. Personen";
            this.btnLinkedPerson.UseVisualStyleBackColor = false;
            // 
            // frmPicture
            // 
            this.frmPicture.BackColor = System.Drawing.Color.Cyan;
            this.frmPicture.Controls.Add(this.Picture1);
            this.frmPicture.Controls.Add(this.frmDublicates);
            this.frmPicture.ForeColor = System.Drawing.SystemColors.ControlText;
            this.frmPicture.Location = new System.Drawing.Point(825, 69);
            this.frmPicture.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmPicture.Name = "frmPicture";
            this.frmPicture.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmPicture.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmPicture.Size = new System.Drawing.Size(185, 239);
            this.frmPicture.TabIndex = 78;
            this.frmPicture.TabStop = false;
            this.frmPicture.Text = "Personenbild";
            this.frmPicture.Visible = false;
            // 
            // edtSearch
            // 
            this.edtSearch.AcceptsReturn = true;
            this.edtSearch.BackColor = System.Drawing.SystemColors.Window;
            this.edtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtSearch.Font = new System.Drawing.Font("Arial", 9F);
            this.edtSearch.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtSearch.Location = new System.Drawing.Point(460, 339);
            this.edtSearch.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtSearch.MaxLength = 0;
            this.edtSearch.Name = "edtSearch";
            this.edtSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtSearch.Size = new System.Drawing.Size(180, 28);
            this.edtSearch.TabIndex = 79;
            this.edtSearch.Text = "<Such1>";
            // 
            // edtSearch2
            // 
            this.edtSearch2.AcceptsReturn = true;
            this.edtSearch2.BackColor = System.Drawing.SystemColors.Window;
            this.edtSearch2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edtSearch2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtSearch2.Font = new System.Drawing.Font("Arial", 9F);
            this.edtSearch2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtSearch2.Location = new System.Drawing.Point(646, 339);
            this.edtSearch2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtSearch2.MaxLength = 0;
            this.edtSearch2.Name = "edtSearch2";
            this.edtSearch2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtSearch2.Size = new System.Drawing.Size(180, 28);
            this.edtSearch2.TabIndex = 80;
            this.edtSearch2.Text = "<Such2>";
            // 
            // edtSearch3
            // 
            this.edtSearch3.AcceptsReturn = true;
            this.edtSearch3.BackColor = System.Drawing.SystemColors.Window;
            this.edtSearch3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edtSearch3.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtSearch3.Font = new System.Drawing.Font("Arial", 9F);
            this.edtSearch3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtSearch3.Location = new System.Drawing.Point(830, 339);
            this.edtSearch3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtSearch3.MaxLength = 0;
            this.edtSearch3.Name = "edtSearch3";
            this.edtSearch3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtSearch3.Size = new System.Drawing.Size(180, 28);
            this.edtSearch3.TabIndex = 81;
            this.edtSearch3.Text = "<Such3>";
            // 
            // btnSaveNExit
            // 
            this.btnSaveNExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSaveNExit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSaveNExit.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSaveNExit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSaveNExit.Location = new System.Drawing.Point(17, 678);
            this.btnSaveNExit.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnSaveNExit.Name = "btnSaveNExit";
            this.btnSaveNExit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSaveNExit.Size = new System.Drawing.Size(255, 27);
            this.btnSaveNExit.TabIndex = 53;
            this.btnSaveNExit.Text = "Fertig &Speichern und zurück";
            this.btnSaveNExit.UseVisualStyleBackColor = false;
            this.btnSaveNExit.Visible = false;
            // 
            // cbxSorting
            // 
            this.cbxSorting.BackColor = System.Drawing.Color.White;
            this.cbxSorting.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxSorting.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxSorting.Location = new System.Drawing.Point(881, 680);
            this.cbxSorting.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxSorting.Name = "cbxSorting";
            this.cbxSorting.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxSorting.Size = new System.Drawing.Size(91, 33);
            this.cbxSorting.TabIndex = 83;
            this.cbxSorting.Text = "<Sort>";
//            this.cbxSorting.SelectedIndexChanged += new System.EventHandler(this.Combo2_SelectedIndexChanged);
            // 
            // btnSaveToFamily
            // 
            this.btnSaveToFamily.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSaveToFamily.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSaveToFamily.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSaveToFamily.Location = new System.Drawing.Point(16, 650);
            this.btnSaveToFamily.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnSaveToFamily.Name = "btnSaveToFamily";
            this.btnSaveToFamily.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSaveToFamily.Size = new System.Drawing.Size(241, 27);
            this.btnSaveToFamily.TabIndex = 54;
            this.btnSaveToFamily.Tag = "12";
            this.btnSaveToFamily.UseVisualStyleBackColor = false;
            this.btnSaveToFamily.Visible = false;
            // 
            // btnTitle
            // 
            this.btnTitle.BackColor = System.Drawing.SystemColors.Control;
            this.btnTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnTitle.Font = new System.Drawing.Font("Arial", 9F);
            this.btnTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnTitle.Location = new System.Drawing.Point(3, 228);
            this.btnTitle.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnTitle.Name = "btnTitle";
            this.btnTitle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnTitle.Size = new System.Drawing.Size(82, 26);
            this.btnTitle.TabIndex = 92;
            this.btnTitle.Text = "Titel:";
            this.btnTitle.UseVisualStyleBackColor = false;
            // 
            // btnOccupation
            // 
            this.btnOccupation.BackColor = System.Drawing.SystemColors.Control;
            this.btnOccupation.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnOccupation.Font = new System.Drawing.Font("Arial", 9F);
            this.btnOccupation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOccupation.Location = new System.Drawing.Point(3, 200);
            this.btnOccupation.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnOccupation.Name = "btnOccupation";
            this.btnOccupation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnOccupation.Size = new System.Drawing.Size(82, 26);
            this.btnOccupation.TabIndex = 89;
            this.btnOccupation.Text = "Beruf:";
            this.btnOccupation.UseVisualStyleBackColor = false;
            // 
            // btnAdditional
            // 
            this.btnAdditional.BackColor = System.Drawing.SystemColors.Control;
            this.btnAdditional.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnAdditional.Font = new System.Drawing.Font("Arial", 9F);
            this.btnAdditional.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAdditional.Location = new System.Drawing.Point(3, 256);
            this.btnAdditional.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnAdditional.Name = "btnAdditional";
            this.btnAdditional.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnAdditional.Size = new System.Drawing.Size(98, 26);
            this.btnAdditional.TabIndex = 91;
            this.btnAdditional.Text = "Sonst.Dat.:";
            this.btnAdditional.UseVisualStyleBackColor = false;
            // 
            // btnResidence
            // 
            this.btnResidence.BackColor = System.Drawing.SystemColors.Control;
            this.btnResidence.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnResidence.Font = new System.Drawing.Font("Arial", 9F);
            this.btnResidence.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnResidence.Location = new System.Drawing.Point(3, 283);
            this.btnResidence.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnResidence.Name = "btnResidence";
            this.btnResidence.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnResidence.Size = new System.Drawing.Size(98, 26);
            this.btnResidence.TabIndex = 90;
            this.btnResidence.Text = "Wohnort:";
            this.btnResidence.UseVisualStyleBackColor = false;
            // 
            // btnAdoption
            // 
            this.btnAdoption.BackColor = System.Drawing.Color.Yellow;
            this.btnAdoption.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnAdoption.Font = new System.Drawing.Font("Arial", 9F);
            this.btnAdoption.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAdoption.Location = new System.Drawing.Point(653, 260);
            this.btnAdoption.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnAdoption.Name = "btnAdoption";
            this.btnAdoption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnAdoption.Size = new System.Drawing.Size(159, 27);
            this.btnAdoption.TabIndex = 95;
            this.btnAdoption.Text = "Adoptiv-Eltern";
            this.btnAdoption.UseVisualStyleBackColor = false;
            this.btnAdoption.Visible = false;
            // 
            // btnFieldsOFB
            // 
            this.btnFieldsOFB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFieldsOFB.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnFieldsOFB.Font = new System.Drawing.Font("Arial", 9F);
            this.btnFieldsOFB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFieldsOFB.Location = new System.Drawing.Point(282, 650);
            this.btnFieldsOFB.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnFieldsOFB.Name = "btnFieldsOFB";
            this.btnFieldsOFB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFieldsOFB.Size = new System.Drawing.Size(145, 27);
            this.btnFieldsOFB.TabIndex = 94;
            this.btnFieldsOFB.Text = "Sonderfelder OFB";
            this.btnFieldsOFB.UseVisualStyleBackColor = false;
            // 
            // btnResarch
            // 
            this.btnResarch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnResarch.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnResarch.Font = new System.Drawing.Font("Arial", 9F);
            this.btnResarch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnResarch.Location = new System.Drawing.Point(282, 680);
            this.btnResarch.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnResarch.Name = "btnResarch";
            this.btnResarch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnResarch.Size = new System.Drawing.Size(96, 27);
            this.btnResarch.TabIndex = 108;
            this.btnResarch.Text = "R&echerche";
            this.btnResarch.UseVisualStyleBackColor = false;
            // 
            // edtGivennames
            // 
            this.edtGivennames.AcceptsReturn = true;
            this.edtGivennames.BackColor = System.Drawing.Color.White;
            this.edtGivennames.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtGivennames.Cursor = System.Windows.Forms.Cursors.Default;
            this.edtGivennames.Font = new System.Drawing.Font("Arial", 9F);
            this.edtGivennames.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtGivennames.Location = new System.Drawing.Point(88, 70);
            this.edtGivennames.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtGivennames.MaxLength = 0;
            this.edtGivennames.Name = "edtGivennames";
            this.edtGivennames.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtGivennames.Size = new System.Drawing.Size(507, 21);
            this.edtGivennames.TabIndex = 50;
            this.edtGivennames.Text = "<Vornamen>";
            // 
            // lblSorting
            // 
            this.lblSorting.BackColor = System.Drawing.SystemColors.Control;
            this.lblSorting.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSorting.Font = new System.Drawing.Font("Arial", 9F);
            this.lblSorting.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSorting.Location = new System.Drawing.Point(777, 680);
            this.lblSorting.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSorting.Name = "lblSorting";
            this.lblSorting.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSorting.Size = new System.Drawing.Size(94, 27);
            this.lblSorting.TabIndex = 84;
            this.lblSorting.Text = "Sortierung";
            this.lblSorting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSearch
            // 
            this.lblSearch.BackColor = System.Drawing.SystemColors.Control;
            this.lblSearch.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSearch.Location = new System.Drawing.Point(401, 339);
            this.lblSearch.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSearch.Size = new System.Drawing.Size(61, 23);
            this.lblSearch.TabIndex = 82;
            this.lblSearch.Text = "Such:";
            // 
            // lblNachfNr
            // 
            this.lblNachfNr.BackColor = System.Drawing.SystemColors.Control;
            this.lblNachfNr.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNachfNr.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNachfNr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNachfNr.Location = new System.Drawing.Point(654, 288);
            this.lblNachfNr.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblNachfNr.Name = "lblNachfNr";
            this.lblNachfNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNachfNr.Size = new System.Drawing.Size(172, 45);
            this.lblNachfNr.TabIndex = 63;
            this.lblNachfNr.Text = "Nachf-.Nr.:";
            // 
            // lblPersonNr
            // 
            this.lblPersonNr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblPersonNr.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPersonNr.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPersonNr.ForeColor = System.Drawing.Color.Black;
            this.lblPersonNr.Location = new System.Drawing.Point(89, 23);
            this.lblPersonNr.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPersonNr.Name = "lblPersonNr";
            this.lblPersonNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPersonNr.Size = new System.Drawing.Size(91, 18);
            this.lblPersonNr.TabIndex = 62;
            // 
            // lblMarriages
            // 
            this.lblMarriages.BackColor = System.Drawing.SystemColors.Control;
            this.lblMarriages.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblMarriages.Font = new System.Drawing.Font("Arial", 9F);
            this.lblMarriages.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMarriages.Location = new System.Drawing.Point(558, 23);
            this.lblMarriages.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblMarriages.Name = "lblMarriages";
            this.lblMarriages.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMarriages.Size = new System.Drawing.Size(244, 18);
            this.lblMarriages.TabIndex = 34;
            this.lblMarriages.Text = "<Partners>";
            // 
            // lblOther
            // 
            this.lblOther.BackColor = System.Drawing.SystemColors.Control;
            this.lblOther.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblOther.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblOther.Location = new System.Drawing.Point(602, 679);
            this.lblOther.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblOther.Name = "lblOther";
            this.lblOther.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOther.Size = new System.Drawing.Size(83, 26);
            this.lblOther.TabIndex = 27;
            this.lblOther.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblOther.Visible = false;
            // 
            // lblResidence
            // 
            this.lblResidence.BackColor = System.Drawing.SystemColors.Control;
            this.lblResidence.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblResidence.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblResidence.Location = new System.Drawing.Point(602, 680);
            this.lblResidence.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblResidence.Name = "lblResidence";
            this.lblResidence.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblResidence.Size = new System.Drawing.Size(83, 26);
            this.lblResidence.TabIndex = 26;
            this.lblResidence.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblResidence.Visible = false;
            // 
            // lblSuffix
            // 
            this.lblSuffix.BackColor = System.Drawing.SystemColors.Control;
            this.lblSuffix.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSuffix.Font = new System.Drawing.Font("Arial", 9F);
            this.lblSuffix.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSuffix.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSuffix.Location = new System.Drawing.Point(545, 48);
            this.lblSuffix.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSuffix.Size = new System.Drawing.Size(70, 18);
            this.lblSuffix.TabIndex = 25;
            this.lblSuffix.Text = " Suffix:";
            this.lblSuffix.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblOccupation
            // 
            this.lblOccupation.BackColor = System.Drawing.SystemColors.Control;
            this.lblOccupation.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblOccupation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblOccupation.Location = new System.Drawing.Point(71, 710);
            this.lblOccupation.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblOccupation.Name = "lblOccupation";
            this.lblOccupation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOccupation.Size = new System.Drawing.Size(43, 14);
            this.lblOccupation.TabIndex = 24;
            this.lblOccupation.Visible = false;
            // 
            // lblClan
            // 
            this.lblClan.BackColor = System.Drawing.SystemColors.Control;
            this.lblClan.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblClan.Font = new System.Drawing.Font("Arial", 9F);
            this.lblClan.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClan.Location = new System.Drawing.Point(357, 91);
            this.lblClan.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblClan.Name = "lblClan";
            this.lblClan.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblClan.Size = new System.Drawing.Size(61, 18);
            this.lblClan.TabIndex = 16;
            this.lblClan.Text = "Sippe:";
            this.lblClan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPrefix
            // 
            this.lblPrefix.BackColor = System.Drawing.SystemColors.Control;
            this.lblPrefix.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPrefix.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPrefix.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPrefix.Location = new System.Drawing.Point(386, 48);
            this.lblPrefix.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPrefix.Size = new System.Drawing.Size(53, 18);
            this.lblPrefix.TabIndex = 15;
            this.lblPrefix.Text = "Präfix:";
            this.lblPrefix.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAlias
            // 
            this.lblAlias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblAlias.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblAlias.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAlias.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAlias.Location = new System.Drawing.Point(3, 91);
            this.lblAlias.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAlias.Name = "lblAlias";
            this.lblAlias.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAlias.Size = new System.Drawing.Size(48, 18);
            this.lblAlias.TabIndex = 14;
            this.lblAlias.Text = "Alias:";
            this.lblAlias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMandant
            // 
            this.lblMandant.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblMandant.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblMandant.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMandant.Location = new System.Drawing.Point(0, 0);
            this.lblMandant.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblMandant.Name = "lblMandant";
            this.lblMandant.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMandant.Size = new System.Drawing.Size(352, 18);
            this.lblMandant.TabIndex = 12;
            this.lblMandant.Text = "Mandant";
            // 
            // lblGivennames
            // 
            this.lblGivennames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblGivennames.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblGivennames.Font = new System.Drawing.Font("Arial", 9F);
            this.lblGivennames.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblGivennames.Location = new System.Drawing.Point(3, 70);
            this.lblGivennames.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblGivennames.Name = "lblGivennames";
            this.lblGivennames.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblGivennames.Size = new System.Drawing.Size(82, 18);
            this.lblGivennames.TabIndex = 11;
            this.lblGivennames.Text = "Vornamen:";
            this.lblGivennames.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSex
            // 
            this.lblSex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblSex.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSex.Font = new System.Drawing.Font("Arial", 9F);
            this.lblSex.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSex.Location = new System.Drawing.Point(348, 23);
            this.lblSex.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSex.Name = "lblSex";
            this.lblSex.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSex.Size = new System.Drawing.Size(45, 18);
            this.lblSex.TabIndex = 1;
            this.lblSex.Text = "Sex:";
            // 
            // lblAge
            // 
            this.lblAge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblAge.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblAge.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAge.ForeColor = System.Drawing.Color.Black;
            this.lblAge.Location = new System.Drawing.Point(0, 23);
            this.lblAge.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAge.Name = "lblAge";
            this.lblAge.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAge.Size = new System.Drawing.Size(85, 18);
            this.lblAge.TabIndex = 4;
            this.lblAge.Text = "<Alter>";
            this.lblAge.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblAncesterNr
            // 
            this.lblAncesterNr.AutoEllipsis = true;
            this.lblAncesterNr.BackColor = System.Drawing.SystemColors.Control;
            this.lblAncesterNr.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblAncesterNr.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAncesterNr.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAncesterNr.Location = new System.Drawing.Point(804, 23);
            this.lblAncesterNr.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblAncesterNr.Name = "lblAncesterNr";
            this.lblAncesterNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAncesterNr.Size = new System.Drawing.Size(214, 18);
            this.lblAncesterNr.TabIndex = 3;
            this.lblAncesterNr.Text = "Ahnennummer";
            // 
            // lblSearch2
            // 
            this.lblSearch2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblSearch2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSearch2.Font = new System.Drawing.Font("Arial", 9F);
            this.lblSearch2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSearch2.Location = new System.Drawing.Point(190, 23);
            this.lblSearch2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSearch2.Name = "lblSearch2";
            this.lblSearch2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSearch2.Size = new System.Drawing.Size(155, 18);
            this.lblSearch2.TabIndex = 2;
            this.lblSearch2.Text = "Suche:";
            // 
            // lblReligion
            // 
            this.lblReligion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblReligion.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblReligion.Font = new System.Drawing.Font("Arial", 9F);
            this.lblReligion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblReligion.Location = new System.Drawing.Point(413, 23);
            this.lblReligion.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReligion.Name = "lblReligion";
            this.lblReligion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblReligion.Size = new System.Drawing.Size(43, 18);
            this.lblReligion.TabIndex = 6;
            this.lblReligion.Text = "Rel:";
            // 
            // lblSurname
            // 
            this.lblSurname.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblSurname.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSurname.Font = new System.Drawing.Font("Arial", 9F);
            this.lblSurname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSurname.Location = new System.Drawing.Point(5, 48);
            this.lblSurname.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblSurname.Name = "lblSurname";
            this.lblSurname.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblSurname.Size = new System.Drawing.Size(57, 18);
            this.lblSurname.TabIndex = 0;
            this.lblSurname.Text = "Name:";
            // 
            // lblState
            // 
            this.lblState.BackColor = System.Drawing.SystemColors.Control;
            this.lblState.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblState.Font = new System.Drawing.Font("Arial", 9F);
            this.lblState.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblState.Location = new System.Drawing.Point(746, 0);
            this.lblState.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblState.Name = "lblState";
            this.lblState.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblState.Size = new System.Drawing.Size(73, 20);
            this.lblState.TabIndex = 74;
            this.lblState.Text = "Status";
            // 
            // lblFamily1
            // 
            this.lblFamily1.BackColor = System.Drawing.SystemColors.Control;
            this.lblFamily1.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFamily1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblFamily1.Location = new System.Drawing.Point(0, 0);
            this.lblFamily1.Name = "lblFamily1";
            this.lblFamily1.Size = new System.Drawing.Size(100, 23);
            this.lblFamily1.TabIndex = 134;
            // 
            // lblCreationDate
            // 
            this.lblCreationDate.BackColor = System.Drawing.SystemColors.Control;
            this.lblCreationDate.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCreationDate.Font = new System.Drawing.Font("Arial", 9F);
            this.lblCreationDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblCreationDate.Location = new System.Drawing.Point(357, 0);
            this.lblCreationDate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblCreationDate.Name = "lblCreationDate";
            this.lblCreationDate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCreationDate.Size = new System.Drawing.Size(167, 20);
            this.lblCreationDate.TabIndex = 37;
            this.lblCreationDate.Text = "Anl.-Datum: 12.12.1997";
            // 
            // lblRemark
            // 
            this.lblRemark.AutoSize = true;
            this.lblRemark.Location = new System.Drawing.Point(19, 512);
            this.lblRemark.Name = "lblRemark";
            this.lblRemark.Size = new System.Drawing.Size(253, 25);
            this.lblRemark.TabIndex = 111;
            this.lblRemark.Text = "Bemerkungen zur Person";
            // 
            // lblFamPers
            // 
            this.lblFamPers.BackColor = System.Drawing.Color.Red;
            this.lblFamPers.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblFamPers.ForeColor = System.Drawing.Color.Yellow;
            this.lblFamPers.Location = new System.Drawing.Point(0, 0);
            this.lblFamPers.Name = "lblFamPers";
            this.lblFamPers.Size = new System.Drawing.Size(353, 22);
            this.lblFamPers.TabIndex = 112;
            this.lblFamPers.Text = "<Familie>";
            this.lblFamPers.Visible = false;
            // 
            // btnReqHint
            // 
            this.btnCancel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCancel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel2.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCancel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel2.Location = new System.Drawing.Point(662, 680);
            this.btnCancel2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCancel2.Name = "btnReqHint";
            this.btnCancel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel2.Size = new System.Drawing.Size(105, 27);
            this.btnCancel2.TabIndex = 113;
            this.btnCancel2.Text = "Abbrechen";
            this.btnCancel2.UseVisualStyleBackColor = false;
            this.btnCancel2.Visible = false;
            // 
            // edtAlias
            // 
            this.edtAlias.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtAlias.Font = new System.Drawing.Font("Arial", 9F);
            this.edtAlias.Location = new System.Drawing.Point(56, 91);
            this.edtAlias.Name = "edtAlias";
            this.edtAlias.Size = new System.Drawing.Size(296, 21);
            this.edtAlias.TabIndex = 114;
            this.edtAlias.Text = "<Alias>";
            // 
            // btnRegisterSearch
            // 
            this.btnRegisterSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnRegisterSearch.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRegisterSearch.Location = new System.Drawing.Point(161, 681);
            this.btnRegisterSearch.Name = "btnRegisterSearch";
            this.btnRegisterSearch.Size = new System.Drawing.Size(119, 27);
            this.btnRegisterSearch.TabIndex = 115;
            this.btnRegisterSearch.Text = "Re&gistersuche";
            this.btnRegisterSearch.UseVisualStyleBackColor = false;
            // 
            // lblResidenceDisp
            // 
            this.lblResidenceDisp.BackColor = System.Drawing.Color.White;
            this.lblResidenceDisp.Font = new System.Drawing.Font("Arial", 9F);
            this.lblResidenceDisp.Location = new System.Drawing.Point(96, 283);
            this.lblResidenceDisp.Name = "lblResidenceDisp";
            this.lblResidenceDisp.Size = new System.Drawing.Size(532, 26);
            this.lblResidenceDisp.TabIndex = 116;
            this.lblResidenceDisp.Text = "<Wohnort(e)>";
            this.lblResidenceDisp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblResidenceDisp.UseMnemonic = false;
            // 
            // lblOccubation
            // 
            this.lblOccubation.BackColor = System.Drawing.Color.White;
            this.lblOccubation.Font = new System.Drawing.Font("Arial", 9F);
            this.lblOccubation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblOccubation.Location = new System.Drawing.Point(86, 200);
            this.lblOccubation.Name = "lblOccubation";
            this.lblOccubation.Size = new System.Drawing.Size(540, 24);
            this.lblOccubation.TabIndex = 117;
            this.lblOccubation.Text = "<Beruf>";
            this.lblOccubation.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lblOccubation.UseMnemonic = false;
            // 
            // lblTitelDisp
            // 
            this.lblTitelDisp.BackColor = System.Drawing.Color.White;
            this.lblTitelDisp.Font = new System.Drawing.Font("Arial", 9F);
            this.lblTitelDisp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTitelDisp.Location = new System.Drawing.Point(86, 228);
            this.lblTitelDisp.Name = "lblTitelDisp";
            this.lblTitelDisp.Size = new System.Drawing.Size(540, 24);
            this.lblTitelDisp.TabIndex = 118;
            this.lblTitelDisp.Text = "<Titel>";
            this.lblTitelDisp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitelDisp.UseMnemonic = false;
            // 
            // lblAdditDisp
            // 
            this.lblAdditDisp.BackColor = System.Drawing.Color.White;
            this.lblAdditDisp.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAdditDisp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblAdditDisp.Location = new System.Drawing.Point(96, 256);
            this.lblAdditDisp.Name = "lblAdditDisp";
            this.lblAdditDisp.Size = new System.Drawing.Size(532, 24);
            this.lblAdditDisp.TabIndex = 119;
            this.lblAdditDisp.Text = "<Sonsiges>";
            this.lblAdditDisp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAdditDisp.UseMnemonic = false;
            // 
            // lblHomeDisp
            // 
            this.lblHomeDisp.BackColor = System.Drawing.Color.White;
            this.lblHomeDisp.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHomeDisp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHomeDisp.Location = new System.Drawing.Point(135, 312);
            this.lblHomeDisp.Name = "lblHomeDisp";
            this.lblHomeDisp.Size = new System.Drawing.Size(491, 23);
            this.lblHomeDisp.TabIndex = 120;
            this.lblHomeDisp.Text = "<Heimatort>";
            this.lblHomeDisp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblHomeDisp.UseMnemonic = false;
            // 
            // btnHometown
            // 
            this.btnHometown.BackColor = System.Drawing.SystemColors.Control;
            this.btnHometown.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnHometown.Font = new System.Drawing.Font("Arial", 9F);
            this.btnHometown.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnHometown.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHometown.Location = new System.Drawing.Point(3, 311);
            this.btnHometown.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnHometown.Name = "btnHometown";
            this.btnHometown.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnHometown.Size = new System.Drawing.Size(134, 26);
            this.btnHometown.TabIndex = 121;
            this.btnHometown.Text = "Heimatort/recht";
            this.btnHometown.UseVisualStyleBackColor = false;
            // 
            // lblPredicate
            // 
            this.lblPredicate.BackColor = System.Drawing.SystemColors.Control;
            this.lblPredicate.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPredicate.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPredicate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPredicate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPredicate.Location = new System.Drawing.Point(801, 48);
            this.lblPredicate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblPredicate.Name = "lblPredicate";
            this.lblPredicate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPredicate.Size = new System.Drawing.Size(70, 18);
            this.lblPredicate.TabIndex = 122;
            this.lblPredicate.Text = "Prädikat:";
            this.lblPredicate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // edtPredicate
            // 
            this.edtPredicate.AcceptsReturn = true;
            this.edtPredicate.BackColor = System.Drawing.SystemColors.Window;
            this.edtPredicate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtPredicate.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtPredicate.Font = new System.Drawing.Font("Arial", 9F);
            this.edtPredicate.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtPredicate.Location = new System.Drawing.Point(874, 48);
            this.edtPredicate.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtPredicate.MaxLength = 0;
            this.edtPredicate.Name = "edtPredicate";
            this.edtPredicate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtPredicate.Size = new System.Drawing.Size(144, 21);
            this.edtPredicate.TabIndex = 123;
            this.edtPredicate.Text = "<Prädicat>";
            // 
            // lblNickname
            // 
            this.lblNickname.BackColor = System.Drawing.SystemColors.Control;
            this.lblNickname.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNickname.Font = new System.Drawing.Font("Arial", 9F);
            this.lblNickname.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblNickname.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblNickname.Location = new System.Drawing.Point(602, 70);
            this.lblNickname.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNickname.Size = new System.Drawing.Size(83, 18);
            this.lblNickname.TabIndex = 124;
            this.lblNickname.Text = "Spitzname:";
            this.lblNickname.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblNickname.Visible = false;
            // 
            // edtNickname
            // 
            this.edtNickname.AcceptsReturn = true;
            this.edtNickname.BackColor = System.Drawing.SystemColors.Window;
            this.edtNickname.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtNickname.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtNickname.Font = new System.Drawing.Font("Arial", 9F);
            this.edtNickname.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtNickname.Location = new System.Drawing.Point(690, 71);
            this.edtNickname.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtNickname.MaxLength = 0;
            this.edtNickname.Multiline = true;
            this.edtNickname.Name = "edtNickname";
            this.edtNickname.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtNickname.Size = new System.Drawing.Size(328, 18);
            this.edtNickname.TabIndex = 125;
            this.edtNickname.Text = "<Spitzname>";
            this.edtNickname.Visible = false;
            // 
            // edtSurnames
            // 
            this.edtSurnames.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtSurnames.Font = new System.Drawing.Font("Arial", 9F);
            this.edtSurnames.Location = new System.Drawing.Point(64, 48);
            this.edtSurnames.Name = "edtSurnames";
            this.edtSurnames.Size = new System.Drawing.Size(318, 21);
            this.edtSurnames.TabIndex = 126;
            this.edtSurnames.Text = "<Name>";
            // 
            // btnProperty
            // 
            this.btnProperty.BackColor = System.Drawing.SystemColors.Control;
            this.btnProperty.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnProperty.Font = new System.Drawing.Font("Arial", 9F);
            this.btnProperty.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnProperty.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnProperty.Location = new System.Drawing.Point(2, 339);
            this.btnProperty.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnProperty.Name = "btnProperty";
            this.btnProperty.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnProperty.Size = new System.Drawing.Size(99, 26);
            this.btnProperty.TabIndex = 127;
            this.btnProperty.Text = "Grundbesitz";
            this.btnProperty.UseVisualStyleBackColor = false;
            // 
            // cbxProperty
            // 
            this.cbxProperty.BackColor = System.Drawing.SystemColors.Window;
            this.cbxProperty.Cursor = System.Windows.Forms.Cursors.Default;
            this.cbxProperty.Font = new System.Drawing.Font("Arial", 9F);
            this.cbxProperty.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxProperty.Location = new System.Drawing.Point(102, 339);
            this.cbxProperty.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cbxProperty.Name = "cbxProperty";
            this.cbxProperty.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbxProperty.Size = new System.Drawing.Size(294, 29);
            this.cbxProperty.Sorted = true;
            this.cbxProperty.TabIndex = 129;
            this.cbxProperty.Text = "<Grundbesitz>";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(190, 681);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel.Size = new System.Drawing.Size(166, 27);
            this.btnCancel.TabIndex = 131;
            this.btnCancel.Text = "abbrechen";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            // 
            // btnShowPlaces
            // 
            this.btnShowPlaces.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnShowPlaces.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnShowPlaces.Font = new System.Drawing.Font("Arial", 9F);
            this.btnShowPlaces.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnShowPlaces.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnShowPlaces.Location = new System.Drawing.Point(789, 649);
            this.btnShowPlaces.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnShowPlaces.Name = "btnShowPlaces";
            this.btnShowPlaces.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnShowPlaces.Size = new System.Drawing.Size(97, 27);
            this.btnShowPlaces.TabIndex = 132;
            this.btnShowPlaces.Text = "Lebensorte";
            this.btnShowPlaces.UseVisualStyleBackColor = false;
            this.btnShowPlaces.Visible = false;
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Label15.Location = new System.Drawing.Point(909, 316);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(88, 25);
            this.Label15.TabIndex = 133;
            this.Label15.Text = "Label15";
            this.Label15.Visible = false;
            // 
            // cbxWitnes
            // 
            this.cbxWitnes.FormattingEnabled = true;
            this.cbxWitnes.Location = new System.Drawing.Point(613, 176);
            this.cbxWitnes.Name = "cbxWitnes";
            this.cbxWitnes.Size = new System.Drawing.Size(214, 33);
            this.cbxWitnes.TabIndex = 134;
            this.cbxWitnes.Text = "<Zeuge>";
            // 
            // fraEventShowEdit2
            // 
            this.fraEventShowEdit2.Location = new System.Drawing.Point(2, 133);
            this.fraEventShowEdit2.Margin = new System.Windows.Forms.Padding(11, 9, 11, 9);
            this.fraEventShowEdit2.Name = "fraEventShowEdit2";
            this.fraEventShowEdit2.Size = new System.Drawing.Size(607, 18);
            this.fraEventShowEdit2.TabIndex = 137;
            // 
            // fraEventShowEdit1
            // 
            this.fraEventShowEdit1.Location = new System.Drawing.Point(3, 111);
            this.fraEventShowEdit1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.fraEventShowEdit1.Name = "fraEventShowEdit1";
            this.fraEventShowEdit1.Size = new System.Drawing.Size(606, 21);
            this.fraEventShowEdit1.TabIndex = 136;
            // 
            // fraPersImpQuerry1
            // 
            this.fraPersImpQuerry1.Location = new System.Drawing.Point(56, 391);
            this.fraPersImpQuerry1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.fraPersImpQuerry1.Name = "fraPersImpQuerry1";
            this.fraPersImpQuerry1.Size = new System.Drawing.Size(624, 232);
            this.fraPersImpQuerry1.TabIndex = 135;
            // 
            // fraEventShowEdit3
            // 
            this.fraEventShowEdit3.Location = new System.Drawing.Point(3, 153);
            this.fraEventShowEdit3.Margin = new System.Windows.Forms.Padding(29, 20, 29, 20);
            this.fraEventShowEdit3.Name = "fraEventShowEdit3";
            this.fraEventShowEdit3.Size = new System.Drawing.Size(606, 22);
            this.fraEventShowEdit3.TabIndex = 138;
            // 
            // fraEventShowEdit4
            // 
            this.fraEventShowEdit4.Location = new System.Drawing.Point(0, 176);
            this.fraEventShowEdit4.Margin = new System.Windows.Forms.Padding(73, 45, 73, 45);
            this.fraEventShowEdit4.Name = "fraEventShowEdit4";
            this.fraEventShowEdit4.Size = new System.Drawing.Size(609, 23);
            this.fraEventShowEdit4.TabIndex = 139;
            // 
            // Personen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(1018, 725);
            this.Controls.Add(this.fraEventShowEdit4);
            this.Controls.Add(this.fraEventShowEdit3);
            this.Controls.Add(this.fraEventShowEdit2);
            this.Controls.Add(this.fraEventShowEdit1);
            this.Controls.Add(this.fraPersImpQuerry1);
            this.Controls.Add(this.lblFamily1);
            this.Controls.Add(this.List3);
            this.Controls.Add(this.Label15);
            this.Controls.Add(this.List4);
            this.Controls.Add(this.frmReenter);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEndTextInput);
            this.Controls.Add(this.btnResarch);
            this.Controls.Add(this.edtNickname);
            this.Controls.Add(this.lblNickname);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.lblOccubation);
            this.Controls.Add(this.btnShowPerson);
            this.Controls.Add(this.btnHometown);
            this.Controls.Add(this.lblHomeDisp);
            this.Controls.Add(this.lblAdditDisp);
            this.Controls.Add(this.lblTitelDisp);
            this.Controls.Add(this.lblResidenceDisp);
            this.Controls.Add(this.btnNoSources);
            this.Controls.Add(this.btnRegisterSearch);
            this.Controls.Add(this.edtAlias);
            this.Controls.Add(this.btnCancel2);
            this.Controls.Add(this.lblFamPers);
            this.Controls.Add(this.Sortlist);
            this.Controls.Add(this.btnPrintScreen);
            this.Controls.Add(this.btnCancel4);
            this.Controls.Add(this.edtStatus);
            this.Controls.Add(this.cbxHome);
            this.Controls.Add(this.cbxAdditional);
            this.Controls.Add(this.btnNewFamily);
            this.Controls.Add(this.btnSearchAncestors);
            this.Controls.Add(this.cbxTitle);
            this.Controls.Add(this.cbxResidence);
            this.Controls.Add(this.cbxOccupation);
            this.Controls.Add(this.edtSex);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNewPerson);
            this.Controls.Add(this.btnSearchNumber);
            this.Controls.Add(this.edtClan);
            this.Controls.Add(this.edtPrefix);
            this.Controls.Add(this.edtSuffix);
            this.Controls.Add(this.edtReligion);
            this.Controls.Add(this.btnNoGodparents);
            this.Controls.Add(this.btnGodparentIfNo);
            this.Controls.Add(this.btnLinkTo);
            this.Controls.Add(this.btnLinkedPerson);
            this.Controls.Add(this.frmPicture);
            this.Controls.Add(this.edtSearch);
            this.Controls.Add(this.edtSearch2);
            this.Controls.Add(this.edtSearch3);
            this.Controls.Add(this.btnSaveNExit);
            this.Controls.Add(this.btnSearchName);
            this.Controls.Add(this.cbxSorting);
            this.Controls.Add(this.btnTitle);
            this.Controls.Add(this.btnOccupation);
            this.Controls.Add(this.btnAdditional);
            this.Controls.Add(this.btnResidence);
            this.Controls.Add(this.btnFieldsOFB);
            this.Controls.Add(this.btnNoWitnesses);
            this.Controls.Add(this.btnWitnessIfNo);
            this.Controls.Add(this.edtGivennames);
            this.Controls.Add(this.lblSorting);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.lblNachfNr);
            this.Controls.Add(this.lblPersonNr);
            this.Controls.Add(this.lblCreationDate);
            this.Controls.Add(this.lblMarriages);
            this.Controls.Add(this.lblOther);
            this.Controls.Add(this.lblResidence);
            this.Controls.Add(this.lblSuffix);
            this.Controls.Add(this.lblOccupation);
            this.Controls.Add(this.lblClan);
            this.Controls.Add(this.lblPrefix);
            this.Controls.Add(this.lblAlias);
            this.Controls.Add(this.lblMandant);
            this.Controls.Add(this.lblGivennames);
            this.Controls.Add(this.lblReligion);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.lblAncesterNr);
            this.Controls.Add(this.lblSearch2);
            this.Controls.Add(this.lblSex);
            this.Controls.Add(this.lblSurname);
            this.Controls.Add(this.btnAdoption);
            this.Controls.Add(this.edtPredicate);
            this.Controls.Add(this.lblPredicate);
            this.Controls.Add(this.edtSurnames);
            this.Controls.Add(this.cbxProperty);
            this.Controls.Add(this.btnProperty);
            this.Controls.Add(this.edtNotes);
            this.Controls.Add(this.List1);
            this.Controls.Add(this.List2);
            this.Controls.Add(this.lblRemark);
            this.Controls.Add(this.btnSaveToFamily);
            this.Controls.Add(this.btnShowPlaces);
            this.Controls.Add(this.cbxWitnes);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Personen";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Personenverwaltung";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).EndInit();
            this.frmDublicates.ResumeLayout(false);
            this.frmReenter.ResumeLayout(false);
            this.frmPicture.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

}