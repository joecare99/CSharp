namespace GenFreeWin.Views;

partial class FraNameSrchSelection
{
    /// <summary> 
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Vom Komponenten-Designer generierter Code

    /// <summary> 
    /// Erforderliche Methode für die Designerunterstützung. 
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
        this.panel1 = new System.Windows.Forms.Panel();
        this.pnlDataOrder = new System.Windows.Forms.Panel();
        this.lblSelHdrDataorder = new System.Windows.Forms.Label();
        this.rbtBirthOccuEtcDeath = new System.Windows.Forms.RadioButton();
        this.rbtBirthDeathOccuEtc = new System.Windows.Forms.RadioButton();
        this.grpNameSrchSelection = new System.Windows.Forms.GroupBox();
        this.pnlLinkedPeople = new System.Windows.Forms.Panel();
        this.chbGodparentOf = new System.Windows.Forms.CheckBox();
        this.chbWitnessOf = new System.Windows.Forms.CheckBox();
        this.chbWitnWithoutData = new System.Windows.Forms.CheckBox();
        this.chbWitnesses = new System.Windows.Forms.CheckBox();
        this.chbGodpWithoutData = new System.Windows.Forms.CheckBox();
        this.chbGodparents = new System.Windows.Forms.CheckBox();
        this.pnlButtons = new System.Windows.Forms.Panel();
        this.btnSelStart2 = new System.Windows.Forms.Button();
        this.btnSelStart1 = new System.Windows.Forms.Button();
        this.fraSelPrintPrivacy1 = new GenFreeWin.Views.FraSelPrintPrivacy();
        this.fraSelPrintBem1 = new GenFreeWin.Views.FraSelPrintBem();
        this.lblHelp = new System.Windows.Forms.Label();
        this.chbPictOrginalSize = new System.Windows.Forms.CheckBox();
        this.chbPersonPictOnly = new System.Windows.Forms.CheckBox();
        this.chbEmitPictures = new System.Windows.Forms.CheckBox();
        this.chbNoCauseOfDeath = new System.Windows.Forms.CheckBox();
        this.lblSelHdrNumEnt = new System.Windows.Forms.Label();
        this.btnSelHelp = new System.Windows.Forms.Button();
        this.TextBox1 = new System.Windows.Forms.TextBox();
        this.chbPersonBaseDatesOnly = new System.Windows.Forms.CheckBox();
        this.chbEmitSources = new System.Windows.Forms.CheckBox();
        this.chbShortenPlaces = new System.Windows.Forms.CheckBox();
        this.chbEmitDocumentNo = new System.Windows.Forms.CheckBox();
        this.chbStructured = new System.Windows.Forms.CheckBox();
        this.chbPicturePath = new System.Windows.Forms.CheckBox();
        this.chbSelEmitDescNo = new System.Windows.Forms.CheckBox();
        this.chbSelEmitAncestNo = new System.Windows.Forms.CheckBox();
        this.chbSelEmitIDs = new System.Windows.Forms.CheckBox();
        this.panel1.SuspendLayout();
        this.pnlDataOrder.SuspendLayout();
        this.grpNameSrchSelection.SuspendLayout();
        this.pnlLinkedPeople.SuspendLayout();
        this.pnlButtons.SuspendLayout();
        this.SuspendLayout();
        // 
        // panel1
        // 
        this.panel1.Controls.Add(this.pnlDataOrder);
        this.panel1.Controls.Add(this.grpNameSrchSelection);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Location = new System.Drawing.Point(0, 0);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(784, 483);
        this.panel1.TabIndex = 0;
        // 
        // pnlDataOrder
        // 
        this.pnlDataOrder.Controls.Add(this.lblSelHdrDataorder);
        this.pnlDataOrder.Controls.Add(this.rbtBirthOccuEtcDeath);
        this.pnlDataOrder.Controls.Add(this.rbtBirthDeathOccuEtc);
        this.pnlDataOrder.Location = new System.Drawing.Point(410, 0);
        this.pnlDataOrder.Name = "pnlDataOrder";
        this.pnlDataOrder.Size = new System.Drawing.Size(353, 75);
        this.pnlDataOrder.TabIndex = 133;
        // 
        // lblSelHdrDataorder
        // 
        this.lblSelHdrDataorder.AutoSize = true;
        this.lblSelHdrDataorder.Location = new System.Drawing.Point(-1, 4);
        this.lblSelHdrDataorder.Name = "lblSelHdrDataorder";
        this.lblSelHdrDataorder.Size = new System.Drawing.Size(170, 20);
        this.lblSelHdrDataorder.TabIndex = 118;
        this.lblSelHdrDataorder.Text = "Reihenfolge der Daten";
        // 
        // rbtBirthOccuEtcDeath
        // 
        this.rbtBirthOccuEtcDeath.AutoSize = true;
        this.rbtBirthOccuEtcDeath.Location = new System.Drawing.Point(3, 49);
        this.rbtBirthOccuEtcDeath.Name = "rbtBirthOccuEtcDeath";
        this.rbtBirthOccuEtcDeath.Size = new System.Drawing.Size(261, 24);
        this.rbtBirthOccuEtcDeath.TabIndex = 117;
        this.rbtBirthOccuEtcDeath.TabStop = true;
        this.rbtBirthOccuEtcDeath.Text = "Geburt, Beruf/sonst. Daten, Tod";
        this.rbtBirthOccuEtcDeath.UseVisualStyleBackColor = true;
        // 
        // rbtBirthDeathOccuEtc
        // 
        this.rbtBirthDeathOccuEtc.AutoSize = true;
        this.rbtBirthDeathOccuEtc.Location = new System.Drawing.Point(3, 23);
        this.rbtBirthDeathOccuEtc.Name = "rbtBirthDeathOccuEtc";
        this.rbtBirthDeathOccuEtc.Size = new System.Drawing.Size(261, 24);
        this.rbtBirthDeathOccuEtc.TabIndex = 116;
        this.rbtBirthDeathOccuEtc.TabStop = true;
        this.rbtBirthDeathOccuEtc.Text = "Geburt, Tod, Beruf/sonst. Daten";
        this.rbtBirthDeathOccuEtc.UseVisualStyleBackColor = true;
        // 
        // grpNameSrchSelection
        // 
        this.grpNameSrchSelection.BackColor = System.Drawing.SystemColors.Control;
        this.grpNameSrchSelection.Controls.Add(this.pnlLinkedPeople);
        this.grpNameSrchSelection.Controls.Add(this.pnlButtons);
        this.grpNameSrchSelection.Controls.Add(this.fraSelPrintPrivacy1);
        this.grpNameSrchSelection.Controls.Add(this.fraSelPrintBem1);
        this.grpNameSrchSelection.Controls.Add(this.lblHelp);
        this.grpNameSrchSelection.Controls.Add(this.chbPictOrginalSize);
        this.grpNameSrchSelection.Controls.Add(this.chbPersonPictOnly);
        this.grpNameSrchSelection.Controls.Add(this.chbEmitPictures);
        this.grpNameSrchSelection.Controls.Add(this.chbNoCauseOfDeath);
        this.grpNameSrchSelection.Controls.Add(this.lblSelHdrNumEnt);
        this.grpNameSrchSelection.Controls.Add(this.btnSelHelp);
        this.grpNameSrchSelection.Controls.Add(this.TextBox1);
        this.grpNameSrchSelection.Controls.Add(this.chbPersonBaseDatesOnly);
        this.grpNameSrchSelection.Controls.Add(this.chbEmitSources);
        this.grpNameSrchSelection.Controls.Add(this.chbShortenPlaces);
        this.grpNameSrchSelection.Controls.Add(this.chbEmitDocumentNo);
        this.grpNameSrchSelection.Controls.Add(this.chbStructured);
        this.grpNameSrchSelection.Controls.Add(this.chbPicturePath);
        this.grpNameSrchSelection.Controls.Add(this.chbSelEmitDescNo);
        this.grpNameSrchSelection.Controls.Add(this.chbSelEmitAncestNo);
        this.grpNameSrchSelection.Controls.Add(this.chbSelEmitIDs);
        this.grpNameSrchSelection.ForeColor = System.Drawing.SystemColors.ControlText;
        this.grpNameSrchSelection.Location = new System.Drawing.Point(3, 4);
        this.grpNameSrchSelection.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.grpNameSrchSelection.Name = "grpNameSrchSelection";
        this.grpNameSrchSelection.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.grpNameSrchSelection.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.grpNameSrchSelection.Size = new System.Drawing.Size(778, 475);
        this.grpNameSrchSelection.TabIndex = 28;
        this.grpNameSrchSelection.TabStop = false;
        this.grpNameSrchSelection.Visible = false;
        // 
        // pnlLinkedPeople
        // 
        this.pnlLinkedPeople.Controls.Add(this.chbGodparentOf);
        this.pnlLinkedPeople.Controls.Add(this.chbWitnessOf);
        this.pnlLinkedPeople.Controls.Add(this.chbWitnWithoutData);
        this.pnlLinkedPeople.Controls.Add(this.chbWitnesses);
        this.pnlLinkedPeople.Controls.Add(this.chbGodpWithoutData);
        this.pnlLinkedPeople.Controls.Add(this.chbGodparents);
        this.pnlLinkedPeople.Location = new System.Drawing.Point(6, 302);
        this.pnlLinkedPeople.Name = "pnlLinkedPeople";
        this.pnlLinkedPeople.Size = new System.Drawing.Size(395, 110);
        this.pnlLinkedPeople.TabIndex = 132;
        // 
        // chbGodparentOf
        // 
        this.chbGodparentOf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.chbGodparentOf.Location = new System.Drawing.Point(207, 72);
        this.chbGodparentOf.Name = "chbGodparentOf";
        this.chbGodparentOf.Size = new System.Drawing.Size(166, 21);
        this.chbGodparentOf.TabIndex = 114;
        this.chbGodparentOf.Text = "Pate bei";
        this.chbGodparentOf.UseVisualStyleBackColor = false;
        // 
        // chbWitnessOf
        // 
        this.chbWitnessOf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.chbWitnessOf.Location = new System.Drawing.Point(3, 72);
        this.chbWitnessOf.Name = "chbWitnessOf";
        this.chbWitnessOf.Size = new System.Drawing.Size(168, 21);
        this.chbWitnessOf.TabIndex = 113;
        this.chbWitnessOf.Text = "Zeugen bei";
        this.chbWitnessOf.UseVisualStyleBackColor = false;
        // 
        // chbWitnWithoutData
        // 
        this.chbWitnWithoutData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.chbWitnWithoutData.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.chbWitnWithoutData.Location = new System.Drawing.Point(207, 44);
        this.chbWitnWithoutData.Name = "chbWitnWithoutData";
        this.chbWitnWithoutData.Size = new System.Drawing.Size(166, 21);
        this.chbWitnWithoutData.TabIndex = 109;
        this.chbWitnWithoutData.Text = "ohne Daten";
        this.chbWitnWithoutData.UseVisualStyleBackColor = false;
        // 
        // chbWitnesses
        // 
        this.chbWitnesses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.chbWitnesses.Location = new System.Drawing.Point(3, 44);
        this.chbWitnesses.Name = "chbWitnesses";
        this.chbWitnesses.Size = new System.Drawing.Size(168, 21);
        this.chbWitnesses.TabIndex = 108;
        this.chbWitnesses.Text = "Zeugen";
        this.chbWitnesses.UseVisualStyleBackColor = false;
        // 
        // chbGodpWithoutData
        // 
        this.chbGodpWithoutData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.chbGodpWithoutData.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.chbGodpWithoutData.Location = new System.Drawing.Point(207, 16);
        this.chbGodpWithoutData.Name = "chbGodpWithoutData";
        this.chbGodpWithoutData.Size = new System.Drawing.Size(166, 21);
        this.chbGodpWithoutData.TabIndex = 107;
        this.chbGodpWithoutData.Text = "ohne Daten";
        this.chbGodpWithoutData.UseVisualStyleBackColor = false;
        // 
        // chbGodparents
        // 
        this.chbGodparents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.chbGodparents.Location = new System.Drawing.Point(3, 16);
        this.chbGodparents.Name = "chbGodparents";
        this.chbGodparents.Size = new System.Drawing.Size(168, 21);
        this.chbGodparents.TabIndex = 106;
        this.chbGodparents.Text = "Paten";
        this.chbGodparents.UseVisualStyleBackColor = false;
        // 
        // pnlButtons
        // 
        this.pnlButtons.Controls.Add(this.btnSelStart2);
        this.pnlButtons.Controls.Add(this.btnSelStart1);
        this.pnlButtons.Location = new System.Drawing.Point(12, 418);
        this.pnlButtons.Name = "pnlButtons";
        this.pnlButtons.Size = new System.Drawing.Size(375, 56);
        this.pnlButtons.TabIndex = 131;
        // 
        // btnSelStart2
        // 
        this.btnSelStart2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.btnSelStart2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
        this.btnSelStart2.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnSelStart2.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnSelStart2.Location = new System.Drawing.Point(200, 13);
        this.btnSelStart2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnSelStart2.Name = "btnSelStart2";
        this.btnSelStart2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnSelStart2.Size = new System.Drawing.Size(172, 33);
        this.btnSelStart2.TabIndex = 34;
        this.btnSelStart2.Text = "Start";
        this.btnSelStart2.UseVisualStyleBackColor = false;
        this.btnSelStart2.Visible = false;
        // 
        // btnSelStart1
        // 
        this.btnSelStart1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.btnSelStart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
        this.btnSelStart1.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnSelStart1.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnSelStart1.Location = new System.Drawing.Point(2, 13);
        this.btnSelStart1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnSelStart1.Name = "btnSelStart1";
        this.btnSelStart1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnSelStart1.Size = new System.Drawing.Size(176, 33);
        this.btnSelStart1.TabIndex = 33;
        this.btnSelStart1.Text = "Start";
        this.btnSelStart1.UseVisualStyleBackColor = false;
        this.btnSelStart1.Visible = false;
        // 
        // fraSelPrintPrivacy1
        // 
        this.fraSelPrintPrivacy1.Location = new System.Drawing.Point(407, 343);
        this.fraSelPrintPrivacy1.Name = "fraSelPrintPrivacy1";
        this.fraSelPrintPrivacy1.Size = new System.Drawing.Size(344, 57);
        this.fraSelPrintPrivacy1.TabIndex = 129;
        // 
        // fraSelPrintBem1
        // 
        this.fraSelPrintBem1.Location = new System.Drawing.Point(407, 96);
        this.fraSelPrintBem1.Name = "fraSelPrintBem1";
        this.fraSelPrintBem1.Size = new System.Drawing.Size(344, 255);
        this.fraSelPrintBem1.TabIndex = 130;
        // 
        // lblHelp
        // 
        this.lblHelp.AutoSize = true;
        this.lblHelp.BackColor = System.Drawing.Color.Red;
        this.lblHelp.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.lblHelp.Location = new System.Drawing.Point(714, 76);
        this.lblHelp.Name = "lblHelp";
        this.lblHelp.Size = new System.Drawing.Size(58, 19);
        this.lblHelp.TabIndex = 128;
        this.lblHelp.Text = "Hilfe ?";
        this.lblHelp.Visible = false;
        this.lblHelp.Click += new System.EventHandler(this.lblHelp_Click);
        // 
        // chbPictOrginalSize
        // 
        this.chbPictOrginalSize.AutoSize = true;
        this.chbPictOrginalSize.Location = new System.Drawing.Point(531, 74);
        this.chbPictOrginalSize.Name = "chbPictOrginalSize";
        this.chbPictOrginalSize.Size = new System.Drawing.Size(190, 24);
        this.chbPictOrginalSize.TabIndex = 127;
        this.chbPictOrginalSize.Text = "Bilder in Originalgröße";
        this.chbPictOrginalSize.UseVisualStyleBackColor = true;
        this.chbPictOrginalSize.CheckedChanged += new System.EventHandler(this.chbPictOrginalSize_CheckedChanged);
        // 
        // chbPersonPictOnly
        // 
        this.chbPersonPictOnly.BackColor = System.Drawing.SystemColors.Control;
        this.chbPersonPictOnly.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbPersonPictOnly.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbPersonPictOnly.Location = new System.Drawing.Point(320, 73);
        this.chbPersonPictOnly.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbPersonPictOnly.Name = "chbPersonPictOnly";
        this.chbPersonPictOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbPersonPictOnly.Size = new System.Drawing.Size(211, 22);
        this.chbPersonPictOnly.TabIndex = 126;
        this.chbPersonPictOnly.Text = "Nur Personenbild ausgeben";
        this.chbPersonPictOnly.UseVisualStyleBackColor = false;
        this.chbPersonPictOnly.CheckedChanged += new System.EventHandler(this.chbPersonPictOnly_CheckedChanged);
        // 
        // chbEmitPictures
        // 
        this.chbEmitPictures.BackColor = System.Drawing.SystemColors.Control;
        this.chbEmitPictures.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbEmitPictures.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbEmitPictures.Location = new System.Drawing.Point(188, 73);
        this.chbEmitPictures.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbEmitPictures.Name = "chbEmitPictures";
        this.chbEmitPictures.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbEmitPictures.Size = new System.Drawing.Size(139, 22);
        this.chbEmitPictures.TabIndex = 125;
        this.chbEmitPictures.Text = "Bilder ausgeben";
        this.chbEmitPictures.UseVisualStyleBackColor = false;
        this.chbEmitPictures.CheckedChanged += new System.EventHandler(this.chbEmitPictures_CheckStateChanged);
        // 
        // chbNoCauseOfDeath
        // 
        this.chbNoCauseOfDeath.BackColor = System.Drawing.SystemColors.Control;
        this.chbNoCauseOfDeath.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbNoCauseOfDeath.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbNoCauseOfDeath.Location = new System.Drawing.Point(14, 210);
        this.chbNoCauseOfDeath.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbNoCauseOfDeath.Name = "chbNoCauseOfDeath";
        this.chbNoCauseOfDeath.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbNoCauseOfDeath.Size = new System.Drawing.Size(277, 22);
        this.chbNoCauseOfDeath.TabIndex = 123;
        this.chbNoCauseOfDeath.Text = "Todesursache nicht ausgeben";
        this.chbNoCauseOfDeath.UseVisualStyleBackColor = false;
        // 
        // lblSelHdrNumEnt
        // 
        this.lblSelHdrNumEnt.AutoSize = true;
        this.lblSelHdrNumEnt.Location = new System.Drawing.Point(423, 403);
        this.lblSelHdrNumEnt.Name = "lblSelHdrNumEnt";
        this.lblSelHdrNumEnt.Size = new System.Drawing.Size(268, 20);
        this.lblSelHdrNumEnt.TabIndex = 122;
        this.lblSelHdrNumEnt.Text = "Nummerneingabe für Mehrfachdruck";
        // 
        // btnSelHelp
        // 
        this.btnSelHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        this.btnSelHelp.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.btnSelHelp.Location = new System.Drawing.Point(387, 426);
        this.btnSelHelp.Name = "btnSelHelp";
        this.btnSelHelp.Size = new System.Drawing.Size(29, 23);
        this.btnSelHelp.TabIndex = 121;
        this.btnSelHelp.Text = "?";
        this.btnSelHelp.UseVisualStyleBackColor = false;
        // 
        // edtPredicate
        // 
        this.TextBox1.Location = new System.Drawing.Point(418, 424);
        this.TextBox1.Name = "edtPlace";
        this.TextBox1.Size = new System.Drawing.Size(320, 26);
        this.TextBox1.TabIndex = 120;
        // 
        // chbPersonBaseDatesOnly
        // 
        this.chbPersonBaseDatesOnly.BackColor = System.Drawing.SystemColors.Control;
        this.chbPersonBaseDatesOnly.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbPersonBaseDatesOnly.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbPersonBaseDatesOnly.Location = new System.Drawing.Point(14, 188);
        this.chbPersonBaseDatesOnly.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbPersonBaseDatesOnly.Name = "chbPersonBaseDatesOnly";
        this.chbPersonBaseDatesOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbPersonBaseDatesOnly.Size = new System.Drawing.Size(277, 22);
        this.chbPersonBaseDatesOnly.TabIndex = 119;
        this.chbPersonBaseDatesOnly.Text = "Personen nur Grunddaten";
        this.chbPersonBaseDatesOnly.UseVisualStyleBackColor = false;
        // 
        // chbEmitSources
        // 
        this.chbEmitSources.BackColor = System.Drawing.SystemColors.Control;
        this.chbEmitSources.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbEmitSources.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbEmitSources.Location = new System.Drawing.Point(14, 96);
        this.chbEmitSources.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbEmitSources.Name = "chbEmitSources";
        this.chbEmitSources.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbEmitSources.Size = new System.Drawing.Size(277, 22);
        this.chbEmitSources.TabIndex = 115;
        this.chbEmitSources.Text = "Quellen Ausgeben";
        this.chbEmitSources.UseVisualStyleBackColor = false;
        // 
        // chbShortenPlaces
        // 
        this.chbShortenPlaces.BackColor = System.Drawing.SystemColors.Control;
        this.chbShortenPlaces.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbShortenPlaces.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbShortenPlaces.Location = new System.Drawing.Point(14, 165);
        this.chbShortenPlaces.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbShortenPlaces.Name = "chbShortenPlaces";
        this.chbShortenPlaces.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbShortenPlaces.Size = new System.Drawing.Size(277, 22);
        this.chbShortenPlaces.TabIndex = 111;
        this.chbShortenPlaces.Text = "Ortsbezeichnung verkürzt";
        this.chbShortenPlaces.UseVisualStyleBackColor = false;
        // 
        // chbEmitDocumentNo
        // 
        this.chbEmitDocumentNo.BackColor = System.Drawing.SystemColors.Control;
        this.chbEmitDocumentNo.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbEmitDocumentNo.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbEmitDocumentNo.Location = new System.Drawing.Point(14, 119);
        this.chbEmitDocumentNo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbEmitDocumentNo.Name = "chbEmitDocumentNo";
        this.chbEmitDocumentNo.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbEmitDocumentNo.Size = new System.Drawing.Size(277, 22);
        this.chbEmitDocumentNo.TabIndex = 110;
        this.chbEmitDocumentNo.Text = "Urkundennummer";
        this.chbEmitDocumentNo.UseVisualStyleBackColor = false;
        // 
        // chbStructured
        // 
        this.chbStructured.BackColor = System.Drawing.SystemColors.Control;
        this.chbStructured.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbStructured.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbStructured.Location = new System.Drawing.Point(14, 142);
        this.chbStructured.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbStructured.Name = "chbStructured";
        this.chbStructured.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbStructured.Size = new System.Drawing.Size(277, 22);
        this.chbStructured.TabIndex = 98;
        this.chbStructured.Text = "Strukturierte Ausgabe";
        this.chbStructured.UseVisualStyleBackColor = false;
        // 
        // chbPicturePath
        // 
        this.chbPicturePath.BackColor = System.Drawing.SystemColors.Control;
        this.chbPicturePath.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbPicturePath.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbPicturePath.Location = new System.Drawing.Point(14, 73);
        this.chbPicturePath.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbPicturePath.Name = "chbPicturePath";
        this.chbPicturePath.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbPicturePath.Size = new System.Drawing.Size(176, 22);
        this.chbPicturePath.TabIndex = 97;
        this.chbPicturePath.Text = "Bilder-Pfad ausgeben";
        this.chbPicturePath.UseVisualStyleBackColor = false;
        // 
        // chbSelEmitDescNo
        // 
        this.chbSelEmitDescNo.BackColor = System.Drawing.SystemColors.Control;
        this.chbSelEmitDescNo.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbSelEmitDescNo.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbSelEmitDescNo.Location = new System.Drawing.Point(14, 50);
        this.chbSelEmitDescNo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbSelEmitDescNo.Name = "chbSelEmitDescNo";
        this.chbSelEmitDescNo.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbSelEmitDescNo.Size = new System.Drawing.Size(277, 21);
        this.chbSelEmitDescNo.TabIndex = 29;
        this.chbSelEmitDescNo.Text = "Nachfahren-Nr. ausgeben";
        this.chbSelEmitDescNo.UseVisualStyleBackColor = false;
        // 
        // chbSelEmitAncestNo
        // 
        this.chbSelEmitAncestNo.BackColor = System.Drawing.SystemColors.Control;
        this.chbSelEmitAncestNo.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbSelEmitAncestNo.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbSelEmitAncestNo.Location = new System.Drawing.Point(14, 27);
        this.chbSelEmitAncestNo.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbSelEmitAncestNo.Name = "chbSelEmitAncestNo";
        this.chbSelEmitAncestNo.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbSelEmitAncestNo.Size = new System.Drawing.Size(277, 22);
        this.chbSelEmitAncestNo.TabIndex = 28;
        this.chbSelEmitAncestNo.Text = "Ahnen-Nr. ausgeben";
        this.chbSelEmitAncestNo.UseVisualStyleBackColor = false;
        // 
        // chbSelEmitIDs
        // 
        this.chbSelEmitIDs.BackColor = System.Drawing.SystemColors.Control;
        this.chbSelEmitIDs.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbSelEmitIDs.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbSelEmitIDs.Location = new System.Drawing.Point(14, 4);
        this.chbSelEmitIDs.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbSelEmitIDs.Name = "chbSelEmitIDs";
        this.chbSelEmitIDs.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbSelEmitIDs.Size = new System.Drawing.Size(341, 23);
        this.chbSelEmitIDs.TabIndex = 27;
        this.chbSelEmitIDs.Text = "Familien und Personen-Nr. ausgeben";
        this.chbSelEmitIDs.UseVisualStyleBackColor = false;
        // 
        // FraNameSrchSelection
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.panel1);
        this.DoubleBuffered = true;
        this.Name = "FraNameSrchSelection";
        this.Size = new System.Drawing.Size(784, 483);
        this.Load += new System.EventHandler(this.FraNameSrchSelection_Load);
        this.panel1.ResumeLayout(false);
        this.pnlDataOrder.ResumeLayout(false);
        this.pnlDataOrder.PerformLayout();
        this.grpNameSrchSelection.ResumeLayout(false);
        this.grpNameSrchSelection.PerformLayout();
        this.pnlLinkedPeople.ResumeLayout(false);
        this.pnlButtons.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    public System.Windows.Forms.GroupBox grpNameSrchSelection;
    private System.Windows.Forms.Panel pnlButtons;
    public System.Windows.Forms.Button btnSelStart2;
    public System.Windows.Forms.Button btnSelStart1;
    private FraSelPrintPrivacy fraSelPrintPrivacy1;
    private FraSelPrintBem fraSelPrintBem1;
    public System.Windows.Forms.Label lblHelp;
    public System.Windows.Forms.CheckBox chbPictOrginalSize;
    public System.Windows.Forms.CheckBox chbPersonPictOnly;
    public System.Windows.Forms.CheckBox chbEmitPictures;
    public System.Windows.Forms.CheckBox chbNoCauseOfDeath;
    internal System.Windows.Forms.Label lblSelHdrNumEnt;
    public System.Windows.Forms.Button btnSelHelp;
    internal System.Windows.Forms.TextBox TextBox1;
    public System.Windows.Forms.CheckBox chbPersonBaseDatesOnly;
    internal System.Windows.Forms.Label lblSelHdrDataorder;
    internal System.Windows.Forms.RadioButton rbtBirthOccuEtcDeath;
    internal System.Windows.Forms.RadioButton rbtBirthDeathOccuEtc;
    public System.Windows.Forms.CheckBox chbEmitSources;
    internal System.Windows.Forms.CheckBox chbGodparentOf;
    internal System.Windows.Forms.CheckBox chbWitnessOf;
    public System.Windows.Forms.CheckBox chbShortenPlaces;
    public System.Windows.Forms.CheckBox chbEmitDocumentNo;
    internal System.Windows.Forms.CheckBox chbWitnWithoutData;
    internal System.Windows.Forms.CheckBox chbWitnesses;
    internal System.Windows.Forms.CheckBox chbGodpWithoutData;
    internal System.Windows.Forms.CheckBox chbGodparents;
    public System.Windows.Forms.CheckBox chbStructured;
    public System.Windows.Forms.CheckBox chbPicturePath;
    public System.Windows.Forms.CheckBox chbSelEmitDescNo;
    public System.Windows.Forms.CheckBox chbSelEmitAncestNo;
    public System.Windows.Forms.CheckBox chbSelEmitIDs;
    private System.Windows.Forms.Panel pnlDataOrder;
    private System.Windows.Forms.Panel pnlLinkedPeople;
}
