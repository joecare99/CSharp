namespace GenFreeWin.Views
{
    partial class FraPreview
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
            this.FraNameDocumentPreview = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnSaveText = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.List6 = new System.Windows.Forms.ListBox();
            this.edtText = new System.Windows.Forms.RichTextBox();
            this.List8 = new System.Windows.Forms.ListBox();
            this.List5 = new System.Windows.Forms.ListBox();
            this.CommonDialog1Save = new System.Windows.Forms.SaveFileDialog();
            this.FraNameDocumentPreview.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FraNameDocumentPreview
            // 
            this.FraNameDocumentPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.FraNameDocumentPreview.Controls.Add(this.tableLayoutPanel1);
            this.FraNameDocumentPreview.Controls.Add(this.List6);
            this.FraNameDocumentPreview.Controls.Add(this.edtText);
            this.FraNameDocumentPreview.Controls.Add(this.List8);
            this.FraNameDocumentPreview.Controls.Add(this.List5);
            this.FraNameDocumentPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FraNameDocumentPreview.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FraNameDocumentPreview.Location = new System.Drawing.Point(0, 0);
            this.FraNameDocumentPreview.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.FraNameDocumentPreview.Name = "FraNameDocumentPreview";
            this.FraNameDocumentPreview.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.FraNameDocumentPreview.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FraNameDocumentPreview.Size = new System.Drawing.Size(973, 761);
            this.FraNameDocumentPreview.TabIndex = 13;
            this.FraNameDocumentPreview.TabStop = false;
            this.FraNameDocumentPreview.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.btnNew, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSaveText, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 705);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(963, 52);
            this.tableLayoutPanel1.TabIndex = 101;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.SystemColors.Control;
            this.btnNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNew.Location = new System.Drawing.Point(5, 4);
            this.btnNew.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnNew.Name = "btnNew";
            this.btnNew.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnNew.Size = new System.Drawing.Size(230, 44);
            this.btnNew.TabIndex = 14;
            this.btnNew.Text = "Neu";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSaveText
            // 
            this.btnSaveText.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveText.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnSaveText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSaveText.Location = new System.Drawing.Point(245, 4);
            this.btnSaveText.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnSaveText.Name = "btnSaveText";
            this.btnSaveText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSaveText.Size = new System.Drawing.Size(230, 44);
            this.btnSaveText.TabIndex = 17;
            this.btnSaveText.Text = "Save";
            this.btnSaveText.UseVisualStyleBackColor = false;
            this.btnSaveText.Click += new System.EventHandler(this.btnSaveText_Click);
            // 
            // btnDuplClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClose.Location = new System.Drawing.Point(725, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(233, 44);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btn_Commend2_1_Click);
            // 
            // List6
            // 
            this.List6.BackColor = System.Drawing.SystemColors.Window;
            this.List6.Cursor = System.Windows.Forms.Cursors.Default;
            this.List6.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List6.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List6.ItemHeight = 20;
            this.List6.Location = new System.Drawing.Point(474, 8);
            this.List6.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List6.Name = "List6";
            this.List6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List6.Size = new System.Drawing.Size(188, 4);
            this.List6.TabIndex = 88;
            this.List6.Visible = false;
            // 
            // edtText
            // 
            this.edtText.Font = new System.Drawing.Font("Arial", 11F);
            this.edtText.Location = new System.Drawing.Point(106, 42);
            this.edtText.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.edtText.Name = "edtText";
            this.edtText.RightMargin = 300;
            this.edtText.Size = new System.Drawing.Size(729, 618);
            this.edtText.TabIndex = 13;
            this.edtText.Text = "RichText";
            // 
            // List8
            // 
            this.List8.BackColor = System.Drawing.SystemColors.Window;
            this.List8.Cursor = System.Windows.Forms.Cursors.Default;
            this.List8.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List8.ItemHeight = 20;
            this.List8.Location = new System.Drawing.Point(48, 42);
            this.List8.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List8.Name = "List8";
            this.List8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List8.Size = new System.Drawing.Size(59, 4);
            this.List8.Sorted = true;
            this.List8.TabIndex = 100;
            this.List8.Visible = false;
            // 
            // lstDuplicates
            // 
            this.List5.BackColor = System.Drawing.SystemColors.Window;
            this.List5.Cursor = System.Windows.Forms.Cursors.Default;
            this.List5.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List5.ItemHeight = 20;
            this.List5.Location = new System.Drawing.Point(35, 128);
            this.List5.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.List5.Name = "List5";
            this.List5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List5.Size = new System.Drawing.Size(41, 4);
            this.List5.Sorted = true;
            this.List5.TabIndex = 87;
            this.List5.Visible = false;
            // 
            // FraPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FraNameDocumentPreview);
            this.Name = "FraPreview";
            this.Size = new System.Drawing.Size(973, 761);
            this.Load += new System.EventHandler(this.FraPreview_Load);
            this.FraNameDocumentPreview.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox FraNameDocumentPreview;
        public System.Windows.Forms.Button btnNew;
        public System.Windows.Forms.Button btnSaveText;
        public System.Windows.Forms.ListBox List6;
        public System.Windows.Forms.RichTextBox edtText;
        public System.Windows.Forms.ListBox List8;
        public System.Windows.Forms.ListBox List5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.SaveFileDialog CommonDialog1Save;
    }
}
