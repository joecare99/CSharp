using Views;
using GenFree.ViewModels.Interfaces;
using Gen_FreeWin.Views;



#if !NET5_0_OR_GREATER
#endif
using System.Windows.Forms;


namespace GenFreeWin.Views;

public partial class Menue
{
    [CommandBinding(nameof(IMenu1ViewModel.OpenFamiliesCommand))]
    internal Button btnFamilies;
    [CommandBinding(nameof(IMenu1ViewModel.OpenSourcesCommand))]
    internal Button btnSources;
    [CommandBinding(nameof(IMenu1ViewModel.OpenPersonsCommand))]
    internal Button btnPersons;
    [CommandBinding(nameof(IMenu1ViewModel.OpenPlacesCommand))]
    internal Button btnPlaces;
    [CommandBinding(nameof(IMenu1ViewModel.OpenMandantsCommand))]
    public Button btnMandants;
    [CommandBinding(nameof(IMenu1ViewModel.OpenTextsCommand))]
    internal Button btnManageTexts;
    [CommandBinding(nameof(IMenu1ViewModel.OpenPrintCommand))]
    internal Button btnPrint;
    [CommandBinding(nameof(IMenu1ViewModel.OpenImportExportCommand))]
    internal Button btnImportExport; 
    [CommandBinding(nameof(IMenu1ViewModel.OpenAddressCommand))]
    public Button btnAddress;
    [CommandBinding(nameof(IMenu1ViewModel.EndProgramCommand))]
    internal Button btnEndProgram;
    [CommandBinding(nameof(IMenu1ViewModel.OpenCalculationsCommand))]
    internal Button btnCalculations;
    [CommandBinding(nameof(IMenu1ViewModel.OpenFunctionKeysCommand))]
    internal Button btnFunctionKeys;
    [CommandBinding(nameof(IMenu1ViewModel.OpenConfigCommand))]
    internal Button btnConfig;
    [CommandBinding(nameof(IMenu1ViewModel.OpenCheckFamiliesCommand))]
    internal Button btnCheckFamilies;
    [CommandBinding(nameof(IMenu1ViewModel.OpenCheckMissingCommand))]
    internal Button btnCheckMissing;
    [CommandBinding(nameof(IMenu1ViewModel.OpenCheckPersonsCommand))]
    internal Button btnCheckPersons;
    [CommandBinding(nameof(IMenu1ViewModel.OpenDuplettesCommand))]
    internal Button btnDuplettes;
    [CommandBinding(nameof(IMenu1ViewModel.OpenNotesCommand))]
    internal Button btnNotes;
    [CommandBinding(nameof(IMenu1ViewModel.OpenEnterLizenzCommand))]
    public Button btnEnterLizenz;
    [CommandBinding(nameof(IMenu1ViewModel.OpenReorgCommand))]
    internal Button btnReorg;
    [CommandBinding(nameof(IMenu1ViewModel.OpenPropertyCommand))]
    internal Button btnProperty;

    [TextBinding(nameof(IMenu1ViewModel.Mandant))]
    internal Label lblMandant;
    [TextBinding(nameof(IMenu1ViewModel.MandantPath))]
    internal Label lblMandantPath;
    [TextBinding(nameof(IMenu1ViewModel.Owner))]
    public Label lblOwner;
    [TextBinding(nameof(IMenu1ViewModel.Menue18))]
    internal Label lblMenue18;
    [TextBinding(nameof(IMenu1ViewModel.HdrOwner))]
    internal Label lblHdrOwner;
    [TextBinding(nameof(IMenu1ViewModel.HdrProgName))]
    internal Label lblHdrProgName;
    [TextBinding(nameof(IMenu1ViewModel.HdrAdt))]
    internal Label lblHdrAdt;
    [TextBinding(nameof(IMenu1ViewModel.HdrCopyright))]
    internal Label lblHdrCopyright;
    [TextBinding(nameof(IMenu1ViewModel.WarningText))]
    [VisibilityBinding(nameof(IMenu1ViewModel.WarningVisible))]
    internal Label lblWarning;

    internal Timer Timer1;
    internal ListBox ListBox1;
    internal Button btnCardMode;
    internal Button btnBachupRead;
    internal Button btnBackupWrite;

    public ListBox File2;
    public ListBox File1;
    public ListBox Dir1;

    [VisibilityBinding(nameof(IMenu1ViewModel.CheckUpdateVisible))]
    [CommandBinding(nameof(IMenu1ViewModel.OpenCheckUpdateCommand))]
    public Button btnCheckUpdate;
    [VisibilityBinding(nameof(IMenu1ViewModel.UpdateVisible))]
    internal Panel pnlUpdate;
    internal CheckBox chbDisableMsg;
    [CommandBinding(nameof(IMenu1ViewModel.UpdateNoCommand))]
    internal Button btnUpdNo;
    [CommandBinding(nameof(IMenu1ViewModel.UpdateYesCommand))]
    internal Button btnUpdYes;
    internal Label lblHdrCheckNow;
    internal Label lblHdrLastCheck;
    internal Label lblDateLastCheck;
    //            _

    public Button btnSendData;
    internal Button btnMerging;
    internal TrackBar TrackBar1;
    [VisibilityBinding(nameof(IMenu1ViewModel.FrmWindowSizeVisible))]
    internal GroupBox frmWindowSize;
    internal Label lblSmaller;
    internal Button btnHelpMain;

    [VisibilityBinding(nameof(IMenu1ViewModel.CodeOfArmsVisible))]
    public PictureBox pbxCodeOfArms;
    internal Label lblAutoUpdState;
    public Button btnRemoteDiag;
    [VisibilityBinding(nameof(IMenu1ViewModel.PbxLanguage1Visible))]
    internal PictureBox pbxLanguage1;
    [VisibilityBinding(nameof(IMenu1ViewModel.PbxLanguage2Visible))]
    internal PictureBox pbxLanguage2;
    [VisibilityBinding(nameof(IMenu1ViewModel.PbxLanguage3Visible))]
    internal PictureBox pbxLanguage3;
    internal OpenFileDialog OpenFileDialog1;
    public Button btnTodayBirth;
    [VisibilityBinding(nameof(IMenu1ViewModel.ListBox2Visible))]
    internal ListBox ListBox2;
    [VisibilityBinding(nameof(IMenu1ViewModel.CreationDateVisible))]
    internal Label lblCreationDate;
    [VisibilityBinding(nameof(IMenu1ViewModel.MarkedVisible))]
    internal Label lblMarked;
    public Button btnTodayDeath;
    public Button btnTodayMarriage;
    public Button btnTodayMarrRel;
    public Button btnTodayBapt;
    public Button btnTodayBurial;
    [VisibilityBinding(nameof(IMenu1ViewModel.NotesVisible))]
    internal Label lblNotes;
    internal Label lblBigger;
    [VisibilityBinding(nameof(IMenu1ViewModel.List3Visible))]
    internal ListBox lstList3;
    internal DateTimePicker DateTimePicker1;
    [VisibilityBinding(nameof(IMenu1ViewModel.SetDateVisible))]
    internal Label lblSetDate;
    internal Button btnLoadTestPrint;

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.btnFamilies = new System.Windows.Forms.Button();
            this.btnSources = new System.Windows.Forms.Button();
            this.btnPersons = new System.Windows.Forms.Button();
            this.btnPlaces = new System.Windows.Forms.Button();
            this.btnMandants = new System.Windows.Forms.Button();
            this.btnManageTexts = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnImportExport = new System.Windows.Forms.Button();
            this.btnAddress = new System.Windows.Forms.Button();
            this.btnEndProgram = new System.Windows.Forms.Button();
            this.btnCalculations = new System.Windows.Forms.Button();
            this.btnFunctionKeys = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.btnCheckFamilies = new System.Windows.Forms.Button();
            this.btnCheckMissing = new System.Windows.Forms.Button();
            this.btnCheckPersons = new System.Windows.Forms.Button();
            this.btnDuplettes = new System.Windows.Forms.Button();
            this.btnNotes = new System.Windows.Forms.Button();
            this.btnEnterLizenz = new System.Windows.Forms.Button();
            this.btnReorg = new System.Windows.Forms.Button();
            this.lblMandant = new System.Windows.Forms.Label();
            this.lblMandantPath = new System.Windows.Forms.Label();
            this.lblHdrOwner = new System.Windows.Forms.Label();
            this.lblOwner = new System.Windows.Forms.Label();
            this.lblMenue18 = new System.Windows.Forms.Label();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblHdrProgName = new System.Windows.Forms.Label();
            this.lblHdrAdt = new System.Windows.Forms.Label();
            this.lblHdrCopyright = new System.Windows.Forms.Label();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.btnCardMode = new System.Windows.Forms.Button();
            this.btnBachupRead = new System.Windows.Forms.Button();
            this.btnBackupWrite = new System.Windows.Forms.Button();

#pragma warning disable CS0618 // Typ oder Element ist veraltet
        this.File2 = new System.Windows.Forms.ListBox();
        this.File1 = new System.Windows.Forms.ListBox();
            this.Dir1 = new System.Windows.Forms.ListBox();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        
            this.btnCheckUpdate = new System.Windows.Forms.Button();
            this.pnlUpdate = new System.Windows.Forms.Panel();
            this.lblAutoUpdState = new System.Windows.Forms.Label();
            this.lblDateLastCheck = new System.Windows.Forms.Label();
            this.lblHdrCheckNow = new System.Windows.Forms.Label();
            this.lblHdrLastCheck = new System.Windows.Forms.Label();
            this.btnUpdNo = new System.Windows.Forms.Button();
            this.btnUpdYes = new System.Windows.Forms.Button();
            this.chbDisableMsg = new System.Windows.Forms.CheckBox();
            this.btnSendData = new System.Windows.Forms.Button();
            this.btnMerging = new System.Windows.Forms.Button();
            this.TrackBar1 = new System.Windows.Forms.TrackBar();
            this.frmWindowSize = new System.Windows.Forms.GroupBox();
            this.lblBigger = new System.Windows.Forms.Label();
            this.lblSmaller = new System.Windows.Forms.Label();
            this.btnHelpMain = new System.Windows.Forms.Button();
            this.btnRemoteDiag = new System.Windows.Forms.Button();
            this.pbxCodeOfArms = new System.Windows.Forms.PictureBox();
            this.pbxLanguage1 = new System.Windows.Forms.PictureBox();
            this.pbxLanguage2 = new System.Windows.Forms.PictureBox();
            this.pbxLanguage3 = new System.Windows.Forms.PictureBox();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnTodayBirth = new System.Windows.Forms.Button();
            this.ListBox2 = new System.Windows.Forms.ListBox();
            this.lblCreationDate = new System.Windows.Forms.Label();
            this.lblMarked = new System.Windows.Forms.Label();
            this.btnTodayDeath = new System.Windows.Forms.Button();
            this.btnTodayMarriage = new System.Windows.Forms.Button();
            this.btnTodayMarrRel = new System.Windows.Forms.Button();
            this.lblNotes = new System.Windows.Forms.Label();
            this.btnProperty = new System.Windows.Forms.Button();
            this.lstList3 = new System.Windows.Forms.ListBox();
            this.DateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.lblSetDate = new System.Windows.Forms.Label();
            this.btnTodayBapt = new System.Windows.Forms.Button();
            this.btnTodayBurial = new System.Windows.Forms.Button();
            this.btnLoadTestPrint = new System.Windows.Forms.Button();
            this.frmStatictics1 = new Gen_FreeWin.Views.FraStatistics(_menu1ViewModel.Statistics,Modul1_IText);
            this.pnlUpdate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar1)).BeginInit();
            this.frmWindowSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxCodeOfArms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLanguage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLanguage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLanguage3)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFamilies
            // 
            this.btnFamilies.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFamilies.Font = new System.Drawing.Font("Arial", 9F);
            this.btnFamilies.Location = new System.Drawing.Point(12, 73);
            this.btnFamilies.Name = "btnFamilies";
            this.btnFamilies.Size = new System.Drawing.Size(203, 27);
            this.btnFamilies.TabIndex = 0;
            this.btnFamilies.Text = "&Familien";
            this.btnFamilies.UseVisualStyleBackColor = false;
            // 
            // btnSources
            // 
            this.btnSources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSources.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSources.Location = new System.Drawing.Point(12, 160);
            this.btnSources.Name = "btnSources";
            this.btnSources.Size = new System.Drawing.Size(203, 27);
            this.btnSources.TabIndex = 1;
            this.btnSources.Tag = "2";
            this.btnSources.Text = "Quellenverwaltung";
            this.btnSources.UseVisualStyleBackColor = false;
            // 
            // btnPersons
            // 
            this.btnPersons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPersons.Font = new System.Drawing.Font("Arial", 9F);
            this.btnPersons.Location = new System.Drawing.Point(12, 102);
            this.btnPersons.Name = "btnPersons";
            this.btnPersons.Size = new System.Drawing.Size(203, 27);
            this.btnPersons.TabIndex = 5;
            this.btnPersons.Tag = "3";
            this.btnPersons.Text = "&Personen";
            this.btnPersons.UseVisualStyleBackColor = false;
            // 
            // btnPlaces
            // 
            this.btnPlaces.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPlaces.Font = new System.Drawing.Font("Arial", 9F);
            this.btnPlaces.Location = new System.Drawing.Point(12, 132);
            this.btnPlaces.Name = "btnPlaces";
            this.btnPlaces.Size = new System.Drawing.Size(203, 27);
            this.btnPlaces.TabIndex = 6;
            this.btnPlaces.Tag = "4";
            this.btnPlaces.Text = "&Ortsverwaltung";
            this.btnPlaces.UseVisualStyleBackColor = false;
            // 
            // btnMandants
            // 
            this.btnMandants.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnMandants.Font = new System.Drawing.Font("Arial", 9F);
            this.btnMandants.Location = new System.Drawing.Point(12, 189);
            this.btnMandants.Name = "btnMandants";
            this.btnMandants.Size = new System.Drawing.Size(203, 27);
            this.btnMandants.TabIndex = 7;
            this.btnMandants.Tag = "5";
            this.btnMandants.Text = "&Mandanten";
            this.btnMandants.UseVisualStyleBackColor = false;
            // 
            // btnManageTexts
            // 
            this.btnManageTexts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnManageTexts.Font = new System.Drawing.Font("Arial", 9F);
            this.btnManageTexts.Location = new System.Drawing.Point(12, 218);
            this.btnManageTexts.Name = "btnManageTexts";
            this.btnManageTexts.Size = new System.Drawing.Size(203, 27);
            this.btnManageTexts.TabIndex = 8;
            this.btnManageTexts.Text = "&Texte bearbeiten";
            this.btnManageTexts.UseVisualStyleBackColor = false;
            // 
            // btnPrint
            // 
            this.btnPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPrint.Font = new System.Drawing.Font("Arial", 9F);
            this.btnPrint.Location = new System.Drawing.Point(12, 247);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(203, 27);
            this.btnPrint.TabIndex = 9;
            this.btnPrint.Text = "&Druckausgaben";
            this.btnPrint.UseVisualStyleBackColor = false;
            // 
            // btnImportExport
            // 
            this.btnImportExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnImportExport.Font = new System.Drawing.Font("Arial", 9F);
            this.btnImportExport.Location = new System.Drawing.Point(12, 276);
            this.btnImportExport.Name = "btnImportExport";
            this.btnImportExport.Size = new System.Drawing.Size(203, 27);
            this.btnImportExport.TabIndex = 10;
            this.btnImportExport.Text = "&Im- und Export";
            this.btnImportExport.UseVisualStyleBackColor = false;
            // 
            // btnAddress
            // 
            this.btnAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnAddress.Font = new System.Drawing.Font("Arial", 9F);
            this.btnAddress.Location = new System.Drawing.Point(12, 305);
            this.btnAddress.Name = "btnAddress";
            this.btnAddress.Size = new System.Drawing.Size(203, 27);
            this.btnAddress.TabIndex = 11;
            this.btnAddress.Text = "&Adresse";
            this.btnAddress.UseVisualStyleBackColor = false;
            // 
            // btnEndProgram
            // 
            this.btnEndProgram.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEndProgram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnEndProgram.Font = new System.Drawing.Font("Arial", 9F);
            this.btnEndProgram.Location = new System.Drawing.Point(12, 598);
            this.btnEndProgram.Name = "btnEndProgram";
            this.btnEndProgram.Size = new System.Drawing.Size(203, 27);
            this.btnEndProgram.TabIndex = 12;
            this.btnEndProgram.Text = "Ende";
            this.btnEndProgram.UseVisualStyleBackColor = false;
            // 
            // btnCalculations
            // 
            this.btnCalculations.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCalculations.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCalculations.Location = new System.Drawing.Point(236, 73);
            this.btnCalculations.Name = "btnCalculations";
            this.btnCalculations.Size = new System.Drawing.Size(217, 27);
            this.btnCalculations.TabIndex = 13;
            this.btnCalculations.Text = "Berechnungen";
            this.btnCalculations.UseVisualStyleBackColor = false;
            // 
            // btnFunctionKeys
            // 
            this.btnFunctionKeys.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFunctionKeys.Font = new System.Drawing.Font("Arial", 9F);
            this.btnFunctionKeys.Location = new System.Drawing.Point(236, 102);
            this.btnFunctionKeys.Name = "btnFunctionKeys";
            this.btnFunctionKeys.Size = new System.Drawing.Size(217, 27);
            this.btnFunctionKeys.TabIndex = 14;
            this.btnFunctionKeys.Text = "Funktionstastenbelegung";
            this.btnFunctionKeys.UseVisualStyleBackColor = false;
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnConfig.Font = new System.Drawing.Font("Arial", 9F);
            this.btnConfig.Location = new System.Drawing.Point(236, 131);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(217, 27);
            this.btnConfig.TabIndex = 15;
            this.btnConfig.Text = "&Einstellungen";
            this.btnConfig.UseVisualStyleBackColor = false;
            // 
            // btnCheckFamilies
            // 
            this.btnCheckFamilies.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCheckFamilies.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCheckFamilies.Location = new System.Drawing.Point(236, 160);
            this.btnCheckFamilies.Name = "btnCheckFamilies";
            this.btnCheckFamilies.Size = new System.Drawing.Size(217, 27);
            this.btnCheckFamilies.TabIndex = 16;
            this.btnCheckFamilies.Text = "Datenprüfung Familien";
            this.btnCheckFamilies.UseVisualStyleBackColor = false;
            // 
            // btnCheckMissing
            // 
            this.btnCheckMissing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCheckMissing.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCheckMissing.Location = new System.Drawing.Point(236, 218);
            this.btnCheckMissing.Name = "btnCheckMissing";
            this.btnCheckMissing.Size = new System.Drawing.Size(217, 27);
            this.btnCheckMissing.TabIndex = 17;
            this.btnCheckMissing.Text = "Datenfehlliste";
            this.btnCheckMissing.UseVisualStyleBackColor = false;
            // 
            // btnCheckPersons
            // 
            this.btnCheckPersons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCheckPersons.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCheckPersons.Location = new System.Drawing.Point(236, 189);
            this.btnCheckPersons.Name = "btnCheckPersons";
            this.btnCheckPersons.Size = new System.Drawing.Size(217, 27);
            this.btnCheckPersons.TabIndex = 18;
            this.btnCheckPersons.Text = "Datenprüfung Personen";
            this.btnCheckPersons.UseVisualStyleBackColor = false;
            // 
            // btnDuplettes
            // 
            this.btnDuplettes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDuplettes.Font = new System.Drawing.Font("Arial", 9F);
            this.btnDuplettes.Location = new System.Drawing.Point(236, 247);
            this.btnDuplettes.Name = "btnDuplettes";
            this.btnDuplettes.Size = new System.Drawing.Size(217, 27);
            this.btnDuplettes.TabIndex = 19;
            this.btnDuplettes.Text = "Dubletten";
            this.btnDuplettes.UseVisualStyleBackColor = false;
            // 
            // btnNotes
            // 
            this.btnNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNotes.Font = new System.Drawing.Font("Arial", 9F);
            this.btnNotes.Location = new System.Drawing.Point(236, 276);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(217, 27);
            this.btnNotes.TabIndex = 20;
            this.btnNotes.Text = "Bemerkungen durchsuch.";
            this.btnNotes.UseVisualStyleBackColor = false;
            // 
            // btnEnterLizenz
            // 
            this.btnEnterLizenz.BackColor = System.Drawing.Color.Red;
            this.btnEnterLizenz.Font = new System.Drawing.Font("Arial", 9F);
            this.btnEnterLizenz.Location = new System.Drawing.Point(236, 335);
            this.btnEnterLizenz.Name = "btnEnterLizenz";
            this.btnEnterLizenz.Size = new System.Drawing.Size(217, 30);
            this.btnEnterLizenz.TabIndex = 22;
            this.btnEnterLizenz.Text = "Lizenznummer eingeben";
            this.btnEnterLizenz.UseVisualStyleBackColor = false;
            this.btnEnterLizenz.Visible = false;
            // 
            // btnReorg
            // 
            this.btnReorg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnReorg.Font = new System.Drawing.Font("Arial", 9F);
            this.btnReorg.Location = new System.Drawing.Point(12, 425);
            this.btnReorg.Name = "btnReorg";
            this.btnReorg.Size = new System.Drawing.Size(203, 30);
            this.btnReorg.TabIndex = 23;
            this.btnReorg.TabStop = false;
            this.btnReorg.Text = "Datei reorganisieren";
            this.btnReorg.UseVisualStyleBackColor = false;
            // 
            // lblMandant
            // 
            this.lblMandant.AutoSize = true;
            this.lblMandant.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblMandant.Location = new System.Drawing.Point(456, 114);
            this.lblMandant.Name = "lblMandant";
            this.lblMandant.Size = new System.Drawing.Size(91, 21);
            this.lblMandant.TabIndex = 27;
            this.lblMandant.Text = "Mandant:";
            // 
            // lblMandantPath
            // 
            this.lblMandantPath.AutoSize = true;
            this.lblMandantPath.BackColor = System.Drawing.Color.Silver;
            this.lblMandantPath.Font = new System.Drawing.Font("Arial", 9F);
            this.lblMandantPath.Location = new System.Drawing.Point(456, 140);
            this.lblMandantPath.Name = "lblMandantPath";
            this.lblMandantPath.Size = new System.Drawing.Size(52, 21);
            this.lblMandantPath.TabIndex = 30;
            this.lblMandantPath.Text = "<12>";
            // 
            // lblHdrOwner
            // 
            this.lblHdrOwner.AutoSize = true;
            this.lblHdrOwner.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrOwner.Location = new System.Drawing.Point(456, 73);
            this.lblHdrOwner.Name = "lblHdrOwner";
            this.lblHdrOwner.Size = new System.Drawing.Size(147, 21);
            this.lblHdrOwner.TabIndex = 31;
            this.lblHdrOwner.Text = "Gen_Pluswin für:";
            // 
            // lblOwner
            // 
            this.lblOwner.AutoSize = true;
            this.lblOwner.Font = new System.Drawing.Font("Arial", 9F);
            this.lblOwner.Location = new System.Drawing.Point(456, 97);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(0, 21);
            this.lblOwner.TabIndex = 32;
            // 
            // lblMenue18
            // 
            this.lblMenue18.AutoSize = true;
            this.lblMenue18.Location = new System.Drawing.Point(802, 101);
            this.lblMenue18.Name = "lblMenue18";
            this.lblMenue18.Size = new System.Drawing.Size(52, 21);
            this.lblMenue18.TabIndex = 34;
            this.lblMenue18.Text = "<18>";
            // 
            // lblHdrProgName
            // 
            this.lblHdrProgName.BackColor = System.Drawing.Color.Red;
            this.lblHdrProgName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblHdrProgName.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrProgName.ForeColor = System.Drawing.Color.Yellow;
            this.lblHdrProgName.Location = new System.Drawing.Point(0, 9);
            this.lblHdrProgName.Name = "lblHdrProgName";
            this.lblHdrProgName.Size = new System.Drawing.Size(940, 20);
            this.lblHdrProgName.TabIndex = 35;
            this.lblHdrProgName.Text = "Gen_Plus das Genealogieprogramm mit den Pluspunkten";
            this.lblHdrProgName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHdrAdt
            // 
            this.lblHdrAdt.BackColor = System.Drawing.Color.Red;
            this.lblHdrAdt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblHdrAdt.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrAdt.ForeColor = System.Drawing.Color.Yellow;
            this.lblHdrAdt.Location = new System.Drawing.Point(0, 50);
            this.lblHdrAdt.Name = "lblHdrAdt";
            this.lblHdrAdt.Size = new System.Drawing.Size(940, 20);
            this.lblHdrAdt.TabIndex = 36;
            this.lblHdrAdt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHdrCopyright
            // 
            this.lblHdrCopyright.BackColor = System.Drawing.Color.Red;
            this.lblHdrCopyright.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblHdrCopyright.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrCopyright.ForeColor = System.Drawing.Color.Yellow;
            this.lblHdrCopyright.Location = new System.Drawing.Point(0, 29);
            this.lblHdrCopyright.Name = "lblHdrCopyright";
            this.lblHdrCopyright.Size = new System.Drawing.Size(940, 20);
            this.lblHdrCopyright.TabIndex = 37;
            this.lblHdrCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHdrCopyright.UseCompatibleTextRendering = true;
            // 
            // ListBox1
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 21;
            this.ListBox1.Location = new System.Drawing.Point(890, 682);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(93, 4);
            this.ListBox1.TabIndex = 38;
            this.ListBox1.Visible = false;
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.BackColor = System.Drawing.Color.Red;
            this.lblWarning.Font = new System.Drawing.Font("Arial", 9F);
            this.lblWarning.Location = new System.Drawing.Point(233, 611);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(98, 21);
            this.lblWarning.TabIndex = 39;
            this.lblWarning.Text = "<Warning>";
            this.lblWarning.Visible = false;
            // 
            // btnCardMode
            // 
            this.btnCardMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCardMode.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCardMode.Location = new System.Drawing.Point(521, 643);
            this.btnCardMode.Name = "btnCardMode";
            this.btnCardMode.Size = new System.Drawing.Size(179, 25);
            this.btnCardMode.TabIndex = 40;
            this.btnCardMode.Text = "&Verkartmodus";
            this.btnCardMode.UseVisualStyleBackColor = false;
            this.btnCardMode.Visible = false;
            // 
            // btnBachupRead
            // 
            this.btnBachupRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnBachupRead.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBachupRead.Location = new System.Drawing.Point(12, 489);
            this.btnBachupRead.Name = "btnBachupRead";
            this.btnBachupRead.Size = new System.Drawing.Size(203, 30);
            this.btnBachupRead.TabIndex = 41;
            this.btnBachupRead.TabStop = false;
            this.btnBachupRead.Text = "Sicherung zurücklesen";
            this.btnBachupRead.UseVisualStyleBackColor = false;
            // 
            // btnBackupWrite
            // 
            this.btnBackupWrite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnBackupWrite.Font = new System.Drawing.Font("Arial", 9F);
            this.btnBackupWrite.Location = new System.Drawing.Point(12, 458);
            this.btnBackupWrite.Name = "btnBackupWrite";
            this.btnBackupWrite.Size = new System.Drawing.Size(203, 30);
            this.btnBackupWrite.TabIndex = 42;
            this.btnBackupWrite.TabStop = false;
            this.btnBackupWrite.Text = "Datensicherung";
            this.btnBackupWrite.UseVisualStyleBackColor = false;
            // 
            // File2
            // 
            this.File2.Cursor = System.Windows.Forms.Cursors.Default;
            this.File2.FormattingEnabled = true;
            this.File2.Location = new System.Drawing.Point(786, 146);
            this.File2.Name = "File2";
            this.File2.Size = new System.Drawing.Size(197, 4);
            this.File2.TabIndex = 49;
            this.File2.Visible = false;
            // 
            // File1
            // 
            this.File1.Cursor = System.Windows.Forms.Cursors.Default;
            this.File1.FormattingEnabled = true;
            this.File1.Location = new System.Drawing.Point(786, 556);
            this.File1.Name = "File1";
            this.File1.Size = new System.Drawing.Size(197, 88);
            this.File1.TabIndex = 50;
            this.File1.Visible = false;
            // 
            // Dir1
            // 
            this.Dir1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Dir1.Font = new System.Drawing.Font("Arial", 9F);
            this.Dir1.FormattingEnabled = true;
            this.Dir1.IntegralHeight = false;
            this.Dir1.Location = new System.Drawing.Point(854, 9);
            this.Dir1.Name = "Dir1";
            this.Dir1.Size = new System.Drawing.Size(141, 43);
            this.Dir1.TabIndex = 51;
            this.Dir1.Visible = false;
            // 
            // btnCheckUpdate
            // 
            this.btnCheckUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCheckUpdate.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCheckUpdate.Location = new System.Drawing.Point(236, 392);
            this.btnCheckUpdate.Name = "btnCheckUpdate";
            this.btnCheckUpdate.Size = new System.Drawing.Size(217, 30);
            this.btnCheckUpdate.TabIndex = 58;
            this.btnCheckUpdate.Text = "Aktualitätskontrolle";
            this.btnCheckUpdate.UseVisualStyleBackColor = false;
            // 
            // pnlUpdate
            // 
            this.pnlUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlUpdate.Controls.Add(this.lblAutoUpdState);
            this.pnlUpdate.Controls.Add(this.lblDateLastCheck);
            this.pnlUpdate.Controls.Add(this.lblHdrCheckNow);
            this.pnlUpdate.Controls.Add(this.lblHdrLastCheck);
            this.pnlUpdate.Controls.Add(this.btnUpdNo);
            this.pnlUpdate.Controls.Add(this.btnUpdYes);
            this.pnlUpdate.Controls.Add(this.chbDisableMsg);
            this.pnlUpdate.Font = new System.Drawing.Font("Arial", 9F);
            this.pnlUpdate.Location = new System.Drawing.Point(236, 428);
            this.pnlUpdate.Name = "pnlUpdate";
            this.pnlUpdate.Size = new System.Drawing.Size(217, 173);
            this.pnlUpdate.TabIndex = 59;
            this.pnlUpdate.Visible = false;
            // 
            // lblAutoUpdState
            // 
            this.lblAutoUpdState.BackColor = System.Drawing.Color.Red;
            this.lblAutoUpdState.Location = new System.Drawing.Point(3, 125);
            this.lblAutoUpdState.Name = "lblAutoUpdState";
            this.lblAutoUpdState.Size = new System.Drawing.Size(214, 47);
            this.lblAutoUpdState.TabIndex = 65;
            this.lblAutoUpdState.Text = "Automatsche Aktualitätskontrolle ist Aus!";
            this.lblAutoUpdState.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblDateLastCheck
            // 
            this.lblDateLastCheck.Location = new System.Drawing.Point(6, 29);
            this.lblDateLastCheck.Name = "lblDateLastCheck";
            this.lblDateLastCheck.Size = new System.Drawing.Size(207, 23);
            this.lblDateLastCheck.TabIndex = 64;
            this.lblDateLastCheck.Text = "<Last Check>";
            this.lblDateLastCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHdrCheckNow
            // 
            this.lblHdrCheckNow.Location = new System.Drawing.Point(6, 52);
            this.lblHdrCheckNow.Name = "lblHdrCheckNow";
            this.lblHdrCheckNow.Size = new System.Drawing.Size(207, 19);
            this.lblHdrCheckNow.TabIndex = 63;
            this.lblHdrCheckNow.Text = "Jetzt Aktualität prüfen?";
            this.lblHdrCheckNow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHdrLastCheck
            // 
            this.lblHdrLastCheck.Location = new System.Drawing.Point(3, 6);
            this.lblHdrLastCheck.Name = "lblHdrLastCheck";
            this.lblHdrLastCheck.Size = new System.Drawing.Size(210, 23);
            this.lblHdrLastCheck.TabIndex = 62;
            this.lblHdrLastCheck.Text = "Letzte Prüfung";
            this.lblHdrLastCheck.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnUpdNo
            // 
            this.btnUpdNo.Location = new System.Drawing.Point(129, 74);
            this.btnUpdNo.Name = "btnUpdNo";
            this.btnUpdNo.Size = new System.Drawing.Size(75, 23);
            this.btnUpdNo.TabIndex = 60;
            this.btnUpdNo.UseVisualStyleBackColor = true;
            // 
            // btnUpdYes
            // 
            this.btnUpdYes.Location = new System.Drawing.Point(6, 74);
            this.btnUpdYes.Name = "btnUpdYes";
            this.btnUpdYes.Size = new System.Drawing.Size(75, 23);
            this.btnUpdYes.TabIndex = 61;
            this.btnUpdYes.Text = "Ja";
            this.btnUpdYes.UseVisualStyleBackColor = true;
            // 
            // chbDisableMsg
            // 
            this.chbDisableMsg.AutoSize = true;
            this.chbDisableMsg.Location = new System.Drawing.Point(4, 103);
            this.chbDisableMsg.Name = "chbDisableMsg";
            this.chbDisableMsg.Size = new System.Drawing.Size(263, 25);
            this.chbDisableMsg.TabIndex = 0;
            this.chbDisableMsg.Text = "Meldung nicht wieder zeigen";
            this.chbDisableMsg.UseVisualStyleBackColor = true;
            // 
            // btnSendData
            // 
            this.btnSendData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSendData.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSendData.Location = new System.Drawing.Point(12, 521);
            this.btnSendData.Name = "btnSendData";
            this.btnSendData.Size = new System.Drawing.Size(203, 30);
            this.btnSendData.TabIndex = 61;
            this.btnSendData.TabStop = false;
            this.btnSendData.Text = "Daten verschicken";
            this.btnSendData.UseVisualStyleBackColor = false;
            this.btnSendData.Visible = false;
            // 
            // btnMerging
            // 
            this.btnMerging.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnMerging.Font = new System.Drawing.Font("Arial", 9F);
            this.btnMerging.Location = new System.Drawing.Point(236, 305);
            this.btnMerging.Name = "btnMerging";
            this.btnMerging.Size = new System.Drawing.Size(217, 27);
            this.btnMerging.TabIndex = 62;
            this.btnMerging.Text = "Verschmelzen";
            this.btnMerging.UseVisualStyleBackColor = false;
            this.btnMerging.Visible = false;
            // 
            // TrackBar1
            // 
            this.TrackBar1.AutoSize = false;
            this.TrackBar1.Location = new System.Drawing.Point(62, 20);
            this.TrackBar1.Name = "TrackBar1";
            this.TrackBar1.Size = new System.Drawing.Size(474, 25);
            this.TrackBar1.TabIndex = 65;
            // 
            // frmWindowSize
            // 
            this.frmWindowSize.Controls.Add(this.lblBigger);
            this.frmWindowSize.Controls.Add(this.TrackBar1);
            this.frmWindowSize.Controls.Add(this.lblSmaller);
            this.frmWindowSize.Font = new System.Drawing.Font("Arial", 8.25F);
            this.frmWindowSize.Location = new System.Drawing.Point(15, 631);
            this.frmWindowSize.Name = "frmWindowSize";
            this.frmWindowSize.Size = new System.Drawing.Size(617, 51);
            this.frmWindowSize.TabIndex = 66;
            this.frmWindowSize.TabStop = false;
            this.frmWindowSize.Text = "Fenstergröße";
            // 
            // lblBigger
            // 
            this.lblBigger.AutoSize = true;
            this.lblBigger.Location = new System.Drawing.Point(533, 20);
            this.lblBigger.Name = "lblBigger";
            this.lblBigger.Size = new System.Drawing.Size(58, 19);
            this.lblBigger.TabIndex = 67;
            this.lblBigger.Text = "größer";
            // 
            // lblSmaller
            // 
            this.lblSmaller.AutoSize = true;
            this.lblSmaller.Location = new System.Drawing.Point(6, 20);
            this.lblSmaller.Name = "lblSmaller";
            this.lblSmaller.Size = new System.Drawing.Size(57, 19);
            this.lblSmaller.TabIndex = 66;
            this.lblSmaller.Text = "kleiner";
            // 
            // btnHelpMain
            // 
            this.btnHelpMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnHelpMain.Font = new System.Drawing.Font("Arial", 9F);
            this.btnHelpMain.Location = new System.Drawing.Point(12, 553);
            this.btnHelpMain.Name = "btnHelpMain";
            this.btnHelpMain.Size = new System.Drawing.Size(203, 30);
            this.btnHelpMain.TabIndex = 67;
            this.btnHelpMain.Text = "Hilfetext Hauptmenü";
            this.btnHelpMain.UseVisualStyleBackColor = false;
            // 
            // btnRemoteDiag
            // 
            this.btnRemoteDiag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnRemoteDiag.Font = new System.Drawing.Font("Arial", 9F);
            this.btnRemoteDiag.Location = new System.Drawing.Point(13, 392);
            this.btnRemoteDiag.Name = "btnRemoteDiag";
            this.btnRemoteDiag.Size = new System.Drawing.Size(202, 30);
            this.btnRemoteDiag.TabIndex = 69;
            this.btnRemoteDiag.Text = "Fernwartung";
            this.btnRemoteDiag.UseVisualStyleBackColor = false;
            // 
            // pbxCodeOfArms
            // 
            this.pbxCodeOfArms.Location = new System.Drawing.Point(712, 189);
            this.pbxCodeOfArms.Name = "pbxCodeOfArms";
            this.pbxCodeOfArms.Size = new System.Drawing.Size(283, 514);
            this.pbxCodeOfArms.TabIndex = 68;
            this.pbxCodeOfArms.TabStop = false;
            // 
            // pbxLanguage1
            // 
            this.pbxLanguage1.Location = new System.Drawing.Point(588, 73);
            this.pbxLanguage1.Name = "pbxLanguage1";
            this.pbxLanguage1.Size = new System.Drawing.Size(44, 27);
            this.pbxLanguage1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxLanguage1.TabIndex = 71;
            this.pbxLanguage1.TabStop = false;
            this.pbxLanguage1.Visible = false;
            // 
            // pbxLanguage2
            // 
            this.pbxLanguage2.Location = new System.Drawing.Point(638, 73);
            this.pbxLanguage2.Name = "pbxLanguage2";
            this.pbxLanguage2.Size = new System.Drawing.Size(58, 27);
            this.pbxLanguage2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxLanguage2.TabIndex = 72;
            this.pbxLanguage2.TabStop = false;
            this.pbxLanguage2.Visible = false;
            // 
            // pbxLanguage3
            // 
            this.pbxLanguage3.Location = new System.Drawing.Point(702, 73);
            this.pbxLanguage3.Name = "pbxLanguage3";
            this.pbxLanguage3.Size = new System.Drawing.Size(60, 27);
            this.pbxLanguage3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxLanguage3.TabIndex = 73;
            this.pbxLanguage3.TabStop = false;
            this.pbxLanguage3.Visible = false;
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // btnTodayBirth
            // 
            this.btnTodayBirth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTodayBirth.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnTodayBirth.Location = new System.Drawing.Point(533, 466);
            this.btnTodayBirth.Name = "btnTodayBirth";
            this.btnTodayBirth.Size = new System.Drawing.Size(167, 23);
            this.btnTodayBirth.TabIndex = 75;
            this.btnTodayBirth.Text = "Heute Geburtstag";
            this.btnTodayBirth.UseVisualStyleBackColor = false;
            // 
            // ListBox2
            // 
            this.ListBox2.Font = new System.Drawing.Font("Arial", 8.25F);
            this.ListBox2.FormattingEnabled = true;
            this.ListBox2.ItemHeight = 19;
            this.ListBox2.Location = new System.Drawing.Point(706, 219);
            this.ListBox2.Name = "ListBox2";
            this.ListBox2.Size = new System.Drawing.Size(493, 441);
            this.ListBox2.Sorted = true;
            this.ListBox2.TabIndex = 76;
            this.ListBox2.Visible = false;
            // 
            // lblCreationDate
            // 
            this.lblCreationDate.BackColor = System.Drawing.Color.White;
            this.lblCreationDate.Font = new System.Drawing.Font("Arial", 9F);
            this.lblCreationDate.Location = new System.Drawing.Point(709, 196);
            this.lblCreationDate.Name = "lblCreationDate";
            this.lblCreationDate.Size = new System.Drawing.Size(270, 20);
            this.lblCreationDate.TabIndex = 77;
            this.lblCreationDate.Text = "<CreDate>";
            this.lblCreationDate.Visible = false;
            // 
            // lblMarked
            // 
            this.lblMarked.BackColor = System.Drawing.Color.White;
            this.lblMarked.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblMarked.ForeColor = System.Drawing.Color.Red;
            this.lblMarked.Location = new System.Drawing.Point(978, 196);
            this.lblMarked.Name = "lblMarked";
            this.lblMarked.Size = new System.Drawing.Size(17, 20);
            this.lblMarked.TabIndex = 78;
            this.lblMarked.Text = "X";
            this.lblMarked.Visible = false;
            // 
            // btnTodayDeath
            // 
            this.btnTodayDeath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTodayDeath.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnTodayDeath.Location = new System.Drawing.Point(533, 512);
            this.btnTodayDeath.Name = "btnTodayDeath";
            this.btnTodayDeath.Size = new System.Drawing.Size(167, 23);
            this.btnTodayDeath.TabIndex = 79;
            this.btnTodayDeath.Text = "Heute Todestag";
            this.btnTodayDeath.UseVisualStyleBackColor = false;
            // 
            // btnTodayMarriage
            // 
            this.btnTodayMarriage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTodayMarriage.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnTodayMarriage.Location = new System.Drawing.Point(533, 559);
            this.btnTodayMarriage.Name = "btnTodayMarriage";
            this.btnTodayMarriage.Size = new System.Drawing.Size(167, 24);
            this.btnTodayMarriage.TabIndex = 80;
            this.btnTodayMarriage.Text = "Heute Hochzeitstag";
            this.btnTodayMarriage.UseVisualStyleBackColor = false;
            // 
            // btnTodayMarrRel
            // 
            this.btnTodayMarrRel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTodayMarrRel.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnTodayMarrRel.Location = new System.Drawing.Point(533, 586);
            this.btnTodayMarrRel.Name = "btnTodayMarrRel";
            this.btnTodayMarrRel.Size = new System.Drawing.Size(167, 48);
            this.btnTodayMarrRel.TabIndex = 81;
            this.btnTodayMarrRel.Text = "Heute kirchl. Hochzeitstag";
            this.btnTodayMarrRel.UseVisualStyleBackColor = false;
            // 
            // lblNotes
            // 
            this.lblNotes.Font = new System.Drawing.Font("Arial", 8.25F);
            this.lblNotes.Location = new System.Drawing.Point(471, 305);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(236, 113);
            this.lblNotes.TabIndex = 82;
            this.lblNotes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNotes.Visible = false;
            // 
            // btnProperty
            // 
            this.btnProperty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnProperty.Font = new System.Drawing.Font("Arial", 9F);
            this.btnProperty.Location = new System.Drawing.Point(12, 338);
            this.btnProperty.Name = "btnProperty";
            this.btnProperty.Size = new System.Drawing.Size(203, 27);
            this.btnProperty.TabIndex = 83;
            this.btnProperty.Text = "Hof- und Grundaktenverw.";
            this.btnProperty.UseVisualStyleBackColor = false;
            // 
            // lstList3
            // 
            this.lstList3.Font = new System.Drawing.Font("Arial", 8.25F);
            this.lstList3.FormattingEnabled = true;
            this.lstList3.ItemHeight = 19;
            this.lstList3.Location = new System.Drawing.Point(706, 219);
            this.lstList3.Name = "lstList3";
            this.lstList3.Size = new System.Drawing.Size(305, 441);
            this.lstList3.TabIndex = 84;
            this.lstList3.Visible = false;
            // 
            // DateTimePicker1
            // 
            this.DateTimePicker1.CustomFormat = "dd MM";
            this.DateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.DateTimePicker1.Location = new System.Drawing.Point(870, 170);
            this.DateTimePicker1.Name = "DateTimePicker1";
            this.DateTimePicker1.RightToLeftLayout = true;
            this.DateTimePicker1.Size = new System.Drawing.Size(113, 28);
            this.DateTimePicker1.TabIndex = 86;
            this.DateTimePicker1.Visible = false;
            // 
            // lblSetDate
            // 
            this.lblSetDate.AutoSize = true;
            this.lblSetDate.Font = new System.Drawing.Font("Arial", 9F);
            this.lblSetDate.Location = new System.Drawing.Point(709, 175);
            this.lblSetDate.Name = "lblSetDate";
            this.lblSetDate.Size = new System.Drawing.Size(176, 21);
            this.lblSetDate.TabIndex = 87;
            this.lblSetDate.Text = "Gewünschtes Datum";
            this.lblSetDate.Visible = false;
            // 
            // btnTodayBapt
            // 
            this.btnTodayBapt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTodayBapt.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnTodayBapt.Location = new System.Drawing.Point(533, 489);
            this.btnTodayBapt.Name = "btnTodayBapt";
            this.btnTodayBapt.Size = new System.Drawing.Size(167, 23);
            this.btnTodayBapt.TabIndex = 88;
            this.btnTodayBapt.Text = "Heute Tauftag";
            this.btnTodayBapt.UseVisualStyleBackColor = false;
            // 
            // btnTodayBurial
            // 
            this.btnTodayBurial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTodayBurial.Font = new System.Drawing.Font("Arial", 8.25F);
            this.btnTodayBurial.Location = new System.Drawing.Point(533, 535);
            this.btnTodayBurial.Name = "btnTodayBurial";
            this.btnTodayBurial.Size = new System.Drawing.Size(167, 23);
            this.btnTodayBurial.TabIndex = 89;
            this.btnTodayBurial.Text = "Heute Begräbnistag";
            this.btnTodayBurial.UseVisualStyleBackColor = false;
            // 
            // btnLoadTestPrint
            // 
            this.btnLoadTestPrint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnLoadTestPrint.Font = new System.Drawing.Font("Arial", 9F);
            this.btnLoadTestPrint.Location = new System.Drawing.Point(13, 366);
            this.btnLoadTestPrint.Name = "btnLoadTestPrint";
            this.btnLoadTestPrint.Size = new System.Drawing.Size(440, 25);
            this.btnLoadTestPrint.TabIndex = 90;
            this.btnLoadTestPrint.Text = "Testversion Druckmodul laden";
            this.btnLoadTestPrint.UseVisualStyleBackColor = false;
            this.btnLoadTestPrint.Visible = false;
            // 
            // frmStatictics1
            // 
            this.frmStatictics1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.frmStatictics1.Location = new System.Drawing.Point(475, 163);
            this.frmStatictics1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmStatictics1.Name = "frmStatictics1";
            this.frmStatictics1.Size = new System.Drawing.Size(232, 169);
            this.frmStatictics1.TabIndex = 91;
            // 
            // Menue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 715);
            this.Controls.Add(this.frmStatictics1);
            this.Controls.Add(this.btnLoadTestPrint);
            this.Controls.Add(this.btnTodayBurial);
            this.Controls.Add(this.btnTodayBapt);
            this.Controls.Add(this.lblSetDate);
            this.Controls.Add(this.DateTimePicker1);
            this.Controls.Add(this.lstList3);
            this.Controls.Add(this.btnProperty);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.btnTodayMarrRel);
            this.Controls.Add(this.btnTodayMarriage);
            this.Controls.Add(this.btnTodayDeath);
            this.Controls.Add(this.lblMarked);
            this.Controls.Add(this.lblCreationDate);
            this.Controls.Add(this.ListBox2);
            this.Controls.Add(this.btnTodayBirth);
            this.Controls.Add(this.pbxLanguage3);
            this.Controls.Add(this.pbxLanguage2);
            this.Controls.Add(this.pbxLanguage1);
            this.Controls.Add(this.btnRemoteDiag);
            this.Controls.Add(this.pbxCodeOfArms);
            this.Controls.Add(this.btnHelpMain);
            this.Controls.Add(this.frmWindowSize);
            this.Controls.Add(this.btnMerging);
            this.Controls.Add(this.btnSendData);
            this.Controls.Add(this.pnlUpdate);
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.btnCheckUpdate);
            this.Controls.Add(this.Dir1);
            this.Controls.Add(this.File1);
            this.Controls.Add(this.File2);
            this.Controls.Add(this.btnBackupWrite);
            this.Controls.Add(this.btnBachupRead);
            this.Controls.Add(this.btnCardMode);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.lblHdrCopyright);
            this.Controls.Add(this.lblHdrAdt);
            this.Controls.Add(this.lblHdrProgName);
            this.Controls.Add(this.lblMenue18);
            this.Controls.Add(this.lblOwner);
            this.Controls.Add(this.lblHdrOwner);
            this.Controls.Add(this.lblMandantPath);
            this.Controls.Add(this.lblMandant);
            this.Controls.Add(this.btnReorg);
            this.Controls.Add(this.btnEnterLizenz);
            this.Controls.Add(this.btnNotes);
            this.Controls.Add(this.btnDuplettes);
            this.Controls.Add(this.btnCheckPersons);
            this.Controls.Add(this.btnCheckMissing);
            this.Controls.Add(this.btnCheckFamilies);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnFunctionKeys);
            this.Controls.Add(this.btnCalculations);
            this.Controls.Add(this.btnEndProgram);
            this.Controls.Add(this.btnAddress);
            this.Controls.Add(this.btnImportExport);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnManageTexts);
            this.Controls.Add(this.btnMandants);
            this.Controls.Add(this.btnPlaces);
            this.Controls.Add(this.btnPersons);
            this.Controls.Add(this.btnSources);
            this.Controls.Add(this.btnFamilies);
            this.Font = new System.Drawing.Font("Arial", 9F);
    //        this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Menue";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Menue";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlUpdate.ResumeLayout(false);
            this.pnlUpdate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar1)).EndInit();
            this.frmWindowSize.ResumeLayout(false);
            this.frmWindowSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxCodeOfArms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLanguage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLanguage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLanguage3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private System.ComponentModel.IContainer components;
    private FraStatistics frmStatictics1;
}