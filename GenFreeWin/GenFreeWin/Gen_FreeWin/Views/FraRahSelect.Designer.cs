namespace GenFreeWin.Views
{
    partial class FraRahSelect
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
            this.frmRahmenSelect = new System.Windows.Forms.GroupBox();
            this.btnEnterNumber = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReenter = new System.Windows.Forms.Button();
            this.btnFromFile = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.frmRahmenSelect.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // frmRahmenSelect
            // 
            this.frmRahmenSelect.BackColor = System.Drawing.Color.Red;
            this.frmRahmenSelect.Controls.Add(this.tableLayoutPanel1);
            this.frmRahmenSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRahmenSelect.ForeColor = System.Drawing.Color.White;
            this.frmRahmenSelect.Location = new System.Drawing.Point(0, 0);
            this.frmRahmenSelect.Margin = new System.Windows.Forms.Padding(4);
            this.frmRahmenSelect.Name = "frmRahmenSelect";
            this.frmRahmenSelect.Padding = new System.Windows.Forms.Padding(4);
            this.frmRahmenSelect.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmRahmenSelect.Size = new System.Drawing.Size(685, 273);
            this.frmRahmenSelect.TabIndex = 6;
            this.frmRahmenSelect.TabStop = false;
            this.frmRahmenSelect.Text = "Frame2";
            // 
            // btnEnterNumber
            // 
            this.btnEnterNumber.BackColor = System.Drawing.SystemColors.Control;
            this.btnEnterNumber.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEnterNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEnterNumber.Font = new System.Drawing.Font("Arial", 9F);
            this.btnEnterNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEnterNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEnterNumber.Location = new System.Drawing.Point(229, 4);
            this.btnEnterNumber.Margin = new System.Windows.Forms.Padding(4);
            this.btnEnterNumber.Name = "btnEnterNumber";
            this.btnEnterNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEnterNumber.Size = new System.Drawing.Size(217, 115);
            this.btnEnterNumber.TabIndex = 23;
            this.btnEnterNumber.Text = "Nummer ein&geben";
            this.btnEnterNumber.UseVisualStyleBackColor = false;
            this.btnEnterNumber.Click += new System.EventHandler(this.btnEnterNumber_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(229, 127);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel.Size = new System.Drawing.Size(217, 115);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "a&bbrechen";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnReenter
            // 
            this.btnReenter.BackColor = System.Drawing.SystemColors.Control;
            this.btnReenter.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnReenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReenter.Font = new System.Drawing.Font("Arial", 9F);
            this.btnReenter.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnReenter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReenter.Location = new System.Drawing.Point(4, 4);
            this.btnReenter.Margin = new System.Windows.Forms.Padding(4);
            this.btnReenter.Name = "btnReenter";
            this.btnReenter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnReenter.Size = new System.Drawing.Size(217, 115);
            this.btnReenter.TabIndex = 7;
            this.btnReenter.Text = "&neu eingeben";
            this.btnReenter.UseVisualStyleBackColor = false;
            this.btnReenter.Click += new System.EventHandler(this.btnReenter_Click);
            // 
            // btnFromFile
            // 
            this.btnFromFile.BackColor = System.Drawing.SystemColors.Control;
            this.btnFromFile.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnFromFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFromFile.Font = new System.Drawing.Font("Arial", 9F);
            this.btnFromFile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFromFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnFromFile.Location = new System.Drawing.Point(454, 4);
            this.btnFromFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnFromFile.Name = "btnFromFile";
            this.btnFromFile.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnFromFile.Size = new System.Drawing.Size(219, 115);
            this.btnFromFile.TabIndex = 6;
            this.btnFromFile.Text = "&aus Datei wählen";
            this.btnFromFile.UseVisualStyleBackColor = false;
            this.btnFromFile.Click += new System.EventHandler(this.btnFromFile_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnFromFile, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnEnterNumber, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnReenter, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 23);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(677, 246);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // FraRahSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.frmRahmenSelect);
            this.Name = "FraRahSelect";
            this.Size = new System.Drawing.Size(685, 273);
            this.Load += new System.EventHandler(this.FraRahSelect_Load);
            this.frmRahmenSelect.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox frmRahmenSelect;
        public System.Windows.Forms.Button btnEnterNumber;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnReenter;
        public System.Windows.Forms.Button btnFromFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
