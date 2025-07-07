using GenFree.ViewModels.Interfaces;
using GenFreeWin.Attributes;
using Views;

namespace Gen_FreeWin.Views
{
    partial class FraPersImpQuerry
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
            this.frmImport = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel3 = new System.Windows.Forms.Button();
            this.btnLoadFromFile = new System.Windows.Forms.Button();
            this.btnDeleteQuiet = new System.Windows.Forms.Button();
            this.btnReenter = new System.Windows.Forms.Button();
            this.frmImport.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // frmImport
            // 
            this.frmImport.AutoSize = true;
            this.frmImport.BackColor = System.Drawing.Color.Red;
            this.frmImport.Controls.Add(this.tableLayoutPanel1);
            this.frmImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmImport.Font = new System.Drawing.Font("Arial", 9F);
            this.frmImport.ForeColor = System.Drawing.Color.White;
            this.frmImport.Location = new System.Drawing.Point(0, 0);
            this.frmImport.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmImport.Name = "frmImport";
            this.frmImport.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmImport.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmImport.Size = new System.Drawing.Size(432, 186);
            this.frmImport.TabIndex = 57;
            this.frmImport.TabStop = false;
            this.frmImport.Text = "Daten Import";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnCancel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnLoadFromFile, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDeleteQuiet, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnReenter, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(422, 157);
            this.tableLayoutPanel1.TabIndex = 63;
            // 
            // btnCancel3
            // 
            this.btnCancel3.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel3.Font = new System.Drawing.Font("Arial", 9F);
            this.btnCancel3.Location = new System.Drawing.Point(5, 83);
            this.btnCancel3.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel3.Name = "btnCancel3";
            this.btnCancel3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel3.Size = new System.Drawing.Size(201, 69);
            this.btnCancel3.TabIndex = 61;
            this.btnCancel3.Text = "abbrechen";
            this.btnCancel3.UseVisualStyleBackColor = false;
            // 
            // btnLoadFromFile
            // 
            this.btnLoadFromFile.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnLoadFromFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadFromFile.Location = new System.Drawing.Point(216, 5);
            this.btnLoadFromFile.Margin = new System.Windows.Forms.Padding(5);
            this.btnLoadFromFile.Name = "btnLoadFromFile";
            this.btnLoadFromFile.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnLoadFromFile.Size = new System.Drawing.Size(201, 68);
            this.btnLoadFromFile.TabIndex = 57;
            this.btnLoadFromFile.Text = "aus &Datei wählen";
            this.btnLoadFromFile.UseVisualStyleBackColor = false;
            // 
            // btnDeleteQuiet
            // 
            this.btnDeleteQuiet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDeleteQuiet.ForeColor = System.Drawing.Color.Black;
            this.btnDeleteQuiet.Location = new System.Drawing.Point(216, 83);
            this.btnDeleteQuiet.Margin = new System.Windows.Forms.Padding(5);
            this.btnDeleteQuiet.Name = "btnDeleteQuiet";
            this.btnDeleteQuiet.Size = new System.Drawing.Size(201, 69);
            this.btnDeleteQuiet.TabIndex = 62;
            this.btnDeleteQuiet.Text = "Ohne Rückfrage löschen";
            this.btnDeleteQuiet.UseVisualStyleBackColor = false;
            // 
            // btnReenter
            // 
            this.btnReenter.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnReenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReenter.Location = new System.Drawing.Point(5, 5);
            this.btnReenter.Margin = new System.Windows.Forms.Padding(5);
            this.btnReenter.Name = "btnReenter";
            this.btnReenter.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnReenter.Size = new System.Drawing.Size(201, 68);
            this.btnReenter.TabIndex = 58;
            this.btnReenter.Text = "&neu eingeben";
            this.btnReenter.UseVisualStyleBackColor = false;
            // 
            // FraPersImpQuerry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.frmImport);
            this.Name = "FraPersImpQuerry";
            this.Size = new System.Drawing.Size(432, 186);
            this.frmImport.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        [ApplTextBinding(nameof(IFraPersImpQueryViewModel.IText))]
        public System.Windows.Forms.GroupBox frmImport;
        [CommandBinding(nameof(IFraPersImpQueryViewModel.DeleteQuietCommand))]
        [ApplTextBinding(nameof(IFraPersImpQueryViewModel.IDelete))]
        internal System.Windows.Forms.Button btnDeleteQuiet;
        [CommandBinding(nameof(IFraPersImpQueryViewModel.CancelCommand))]
        [ApplTextBinding(nameof(IFraPersImpQueryViewModel.ICancel))]
        public System.Windows.Forms.Button btnCancel3;
        [CommandBinding(nameof(IFraPersImpQueryViewModel.ReenterCommand))]
        [ApplTextBinding(nameof(IFraPersImpQueryViewModel.IReenter))]
        public System.Windows.Forms.Button btnReenter;
        [CommandBinding(nameof(IFraPersImpQueryViewModel.LoadFromFileCommand))]
        [ApplTextBinding(nameof(IFraPersImpQueryViewModel.ILoadFromFile))]
        public System.Windows.Forms.Button btnLoadFromFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
