namespace GenFreeWin;

partial class EinzelQuelle
{
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Button btnFinished;
    private System.Windows.Forms.Button btnSources;
    public System.Windows.Forms.Label Label3;
    public System.Windows.Forms.Label lblTitel;
    internal System.Windows.Forms.Label lbl__PerFamNr;
    internal System.Windows.Forms.Label lbl__QuellNr;
    public System.Windows.Forms.RichTextBox edtComment;
    public System.Windows.Forms.Label Label2;

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

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// </summary>
    private void InitializeComponent()
    {
            this.edtAus = new System.Windows.Forms.TextBox();
            this.btnFinished = new System.Windows.Forms.Button();
            this.edtEntry = new System.Windows.Forms.TextBox();
            this.edtOriginalText = new System.Windows.Forms.RichTextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.lblTitel = new System.Windows.Forms.Label();
            this.lbl__QKenn = new System.Windows.Forms.Label();
            this.lbl__PerFamNr = new System.Windows.Forms.Label();
            this.lbl__QuellNr = new System.Windows.Forms.Label();
            this.btnSources = new System.Windows.Forms.Button();
            this.edtComment = new System.Windows.Forms.RichTextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // edtAus
            // 
            this.edtAus.AcceptsReturn = true;
            this.edtAus.BackColor = System.Drawing.SystemColors.Window;
            this.edtAus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edtAus.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtAus.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtAus.Location = new System.Drawing.Point(4, 33);
            this.edtAus.MaxLength = 0;
            this.edtAus.Name = "edtAus";
            this.edtAus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtAus.Size = new System.Drawing.Size(170, 33);
            this.edtAus.TabIndex = 1;
            this.edtAus.Text = "<Aus?>";
            this.edtAus.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.edtAus.GotFocus += new System.EventHandler(this.Text2_GotFocus);
            this.edtAus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text2_KeyDown);
            // 
            // btnFinished
            // 
            this.btnFinished.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnFinished.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnFinished.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFinished.Location = new System.Drawing.Point(760, 36);
            this.btnFinished.Name = "btnFinished";
            this.btnFinished.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFinished.Size = new System.Drawing.Size(83, 31);
            this.btnFinished.TabIndex = 5;
            this.btnFinished.Text = "&Fertig";
            this.btnFinished.UseVisualStyleBackColor = false;
            this.btnFinished.Click += new System.EventHandler(this.Command1_Click);
            // 
            // edtEntry
            // 
            this.edtEntry.AcceptsReturn = true;
            this.edtEntry.BackColor = System.Drawing.Color.White;
            this.edtEntry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.edtEntry.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtEntry.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtEntry.Location = new System.Drawing.Point(184, 33);
            this.edtEntry.MaxLength = 0;
            this.edtEntry.Name = "edtEntry";
            this.edtEntry.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtEntry.Size = new System.Drawing.Size(376, 33);
            this.edtEntry.TabIndex = 2;
            this.edtEntry.Text = "<Eintrag>";
            this.edtEntry.TextChanged += new System.EventHandler(this.Text1_TextChanged);
            this.edtEntry.GotFocus += new System.EventHandler(this.Text1_GotFocus);
            this.edtEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Text1_KeyDown);
            // 
            // edtOriginalText
            // 
            this.edtOriginalText.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edtOriginalText.Location = new System.Drawing.Point(4, 90);
            this.edtOriginalText.Name = "edtOriginalText";
            this.edtOriginalText.RightMargin = 800;
            this.edtOriginalText.Size = new System.Drawing.Size(848, 110);
            this.edtOriginalText.TabIndex = 3;
            this.edtOriginalText.Text = "<Orginaltext>";
            this.edtOriginalText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox1_KeyDown);
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.Location = new System.Drawing.Point(4, 67);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label3.Size = new System.Drawing.Size(848, 20);
            this.Label3.TabIndex = 4;
            this.Label3.Text = "Quelle Originaltext";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblTitel
            // 
            this.lblTitel.BackColor = System.Drawing.Color.White;
            this.lblTitel.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTitel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTitel.Location = new System.Drawing.Point(4, 6);
            this.lblTitel.Name = "lblTitel";
            this.lblTitel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTitel.Size = new System.Drawing.Size(848, 24);
            this.lblTitel.TabIndex = 1;
            this.lblTitel.Text = "<Titel>";
            // 
            // lbl__QKenn
            // 
            this.lbl__QKenn.AutoSize = true;
            this.lbl__QKenn.BackColor = System.Drawing.Color.Red;
            this.lbl__QKenn.Location = new System.Drawing.Point(258, 67);
            this.lbl__QKenn.Name = "lbl__QKenn";
            this.lbl__QKenn.Size = new System.Drawing.Size(83, 25);
            this.lbl__QKenn.TabIndex = 6;
            this.lbl__QKenn.Text = "lblState";
            this.lbl__QKenn.Visible = false;
            // 
            // lbl__PerFamNr
            // 
            this.lbl__PerFamNr.AutoSize = true;
            this.lbl__PerFamNr.BackColor = System.Drawing.Color.Red;
            this.lbl__PerFamNr.Location = new System.Drawing.Point(60, 67);
            this.lbl__PerFamNr.Name = "lbl__PerFamNr";
            this.lbl__PerFamNr.Size = new System.Drawing.Size(109, 25);
            this.lbl__PerFamNr.TabIndex = 7;
            this.lbl__PerFamNr.Text = "lblPerFam";
            this.lbl__PerFamNr.Visible = false;
            // 
            // lbl__QuellNr
            // 
            this.lbl__QuellNr.AutoSize = true;
            this.lbl__QuellNr.BackColor = System.Drawing.Color.Red;
            this.lbl__QuellNr.Location = new System.Drawing.Point(135, 67);
            this.lbl__QuellNr.Name = "lbl__QuellNr";
            this.lbl__QuellNr.Size = new System.Drawing.Size(101, 25);
            this.lbl__QuellNr.TabIndex = 8;
            this.lbl__QuellNr.Text = "lblSorting";
            this.lbl__QuellNr.Visible = false;
            // 
            // btnSources
            // 
            this.btnSources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSources.Location = new System.Drawing.Point(586, 36);
            this.btnSources.Name = "btnSources";
            this.btnSources.Size = new System.Drawing.Size(144, 30);
            this.btnSources.TabIndex = 9;
            this.btnSources.Text = "&Quellen Fertig";
            this.btnSources.UseVisualStyleBackColor = false;
            this.btnSources.Click += new System.EventHandler(this.Button1_Click);
            // 
            // edtComment
            // 
            this.edtComment.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edtComment.Location = new System.Drawing.Point(4, 234);
            this.edtComment.Name = "edtComment";
            this.edtComment.RightMargin = 800;
            this.edtComment.Size = new System.Drawing.Size(848, 110);
            this.edtComment.TabIndex = 10;
            this.edtComment.Text = "<Kommentar>";
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(4, 203);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(848, 24);
            this.Label2.TabIndex = 11;
            this.Label2.Text = "Kommentar zur Quelle";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // EinzelQuelle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(864, 379);
            this.ControlBox = false;
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.edtComment);
            this.Controls.Add(this.btnSources);
            this.Controls.Add(this.lbl__QuellNr);
            this.Controls.Add(this.lbl__PerFamNr);
            this.Controls.Add(this.lbl__QKenn);
            this.Controls.Add(this.btnFinished);
            this.Controls.Add(this.edtAus);
            this.Controls.Add(this.edtEntry);
            this.Controls.Add(this.edtOriginalText);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.lblTitel);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11F);
            this.Location = new System.Drawing.Point(4, 4);
            this.Name = "EinzelQuelle";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    public System.Windows.Forms.RichTextBox edtOriginalText;
    public System.Windows.Forms.TextBox edtAus;
    public System.Windows.Forms.TextBox edtEntry;
    public System.Windows.Forms.Label lbl__QKenn;
}
