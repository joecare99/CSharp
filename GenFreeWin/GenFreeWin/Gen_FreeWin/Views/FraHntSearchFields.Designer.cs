using GenFree.Data;

namespace GenFreeWin.Views
{
    partial class FraHntSearchFields
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
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSaveAndBack = new System.Windows.Forms.Button();
            this.fraHntSrcSelection3 = new GenFreeWin.Views.FraHntSrcSelection();
            this.fraHntSrcSelection1 = new GenFreeWin.Views.FraHntSrcSelection();
            this.fraHntSrcSelection2 = new GenFreeWin.Views.FraHntSrcSelection();
            this.GroupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.tableLayoutPanel1);
            this.GroupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox3.Location = new System.Drawing.Point(0, 0);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(1258, 751);
            this.GroupBox3.TabIndex = 47;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Belegung der Suchfelder";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.btnSaveAndBack, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.fraHntSrcSelection3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.fraHntSrcSelection1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.fraHntSrcSelection2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 22);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1252, 726);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // btnSaveAndBack
            // 
            this.btnSaveAndBack.BackColor = System.Drawing.Color.Red;
            this.btnSaveAndBack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveAndBack.Location = new System.Drawing.Point(420, 510);
            this.btnSaveAndBack.Name = "btnSaveAndBack";
            this.btnSaveAndBack.Size = new System.Drawing.Size(411, 139);
            this.btnSaveAndBack.TabIndex = 2;
            this.btnSaveAndBack.Text = "Speichern und zurück";
            this.btnSaveAndBack.UseVisualStyleBackColor = false;
            this.btnSaveAndBack.Click += new System.EventHandler(this.btnSaveAndBack_Click);
            // 
            // fraHntSrcSelection3
            // 
            this.fraHntSrcSelection3.Caption = "Suchfeld 3";
            this.fraHntSrcSelection3.Dock = System.Windows.Forms.DockStyle.Fill;

            this.fraHntSrcSelection3.eSearchSelection = GenFree.Data.ESearchSelection.eManual;
            this.fraHntSrcSelection3.Location = new System.Drawing.Point(837, 3);
            this.fraHntSrcSelection3.eSearchSelection = ESearchSelection.eManual;
            this.fraHntSrcSelection3.Location = new System.Drawing.Point(837, 3);
            this.fraHntSrcSelection3.Name = "fraHntSrcSelection3";
            this.fraHntSrcSelection3.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.fraHntSrcSelection3.Size = new System.Drawing.Size(412, 429);
            this.fraHntSrcSelection3.TabIndex = 7;
            this.fraHntSrcSelection3.UnSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));

            this.fraHntSrcSelection3.eSearchSelectionChanged += new System.EventHandler<GenFree.Data.ESearchSelection>(this.fraHntSrcSelection_eSearchSelectionChanged);
            // 
            this.fraHntSrcSelection3.eSearchSelectionChanged += new System.EventHandler<ESearchSelection>(this.fraHntSrcSelection_eSearchSelectionChanged);
            // 
            // fraHntSrcSelection1
            // 
            this.fraHntSrcSelection1.Caption = "Suchfeld 1";
            this.fraHntSrcSelection1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fraHntSrcSelection1.eSearchSelection = GenFree.Data.ESearchSelection.eManual;
            this.fraHntSrcSelection1.Location = new System.Drawing.Point(3, 3);
            this.fraHntSrcSelection1.eSearchSelection = ESearchSelection.eManual;
            this.fraHntSrcSelection1.Location = new System.Drawing.Point(3, 3);
            this.fraHntSrcSelection1.Name = "fraHntSrcSelection1";
            this.fraHntSrcSelection1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.fraHntSrcSelection1.Size = new System.Drawing.Size(411, 429);
            this.fraHntSrcSelection1.TabIndex = 5;
            this.fraHntSrcSelection1.UnSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));

            this.fraHntSrcSelection1.eSearchSelectionChanged += new System.EventHandler<GenFree.Data.ESearchSelection>(this.fraHntSrcSelection_eSearchSelectionChanged);
            this.fraHntSrcSelection1.eSearchSelectionChanged += new System.EventHandler<ESearchSelection>(this.fraHntSrcSelection_eSearchSelectionChanged);
            // 
            // fraHntSrcSelection2
            // 
            this.fraHntSrcSelection2.Caption = "Suchfeld 2";
            this.fraHntSrcSelection2.Dock = System.Windows.Forms.DockStyle.Fill;

            this.fraHntSrcSelection2.eSearchSelection = GenFree.Data.ESearchSelection.eManual;
            this.fraHntSrcSelection2.Location = new System.Drawing.Point(420, 3);
            this.fraHntSrcSelection2.eSearchSelection = ESearchSelection.eManual;
            this.fraHntSrcSelection2.Location = new System.Drawing.Point(420, 3);
            this.fraHntSrcSelection2.Name = "fraHntSrcSelection2";
            this.fraHntSrcSelection2.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.fraHntSrcSelection2.Size = new System.Drawing.Size(411, 429);
            this.fraHntSrcSelection2.TabIndex = 6;
            this.fraHntSrcSelection2.UnSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));

            this.fraHntSrcSelection2.eSearchSelectionChanged += new System.EventHandler<GenFree.Data.ESearchSelection>(this.fraHntSrcSelection_eSearchSelectionChanged);
            this.fraHntSrcSelection2.eSearchSelectionChanged += new System.EventHandler<ESearchSelection>(this.fraHntSrcSelection_eSearchSelectionChanged);
            // 
            // FraHntSearchFields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBox3);
            this.Name = "FraHntSearchFields";
            this.Size = new System.Drawing.Size(1258, 751);
            this.GroupBox3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox3;
        public System.Windows.Forms.Button btnSaveAndBack;
        private FraHntSrcSelection fraHntSrcSelection1;
        private FraHntSrcSelection fraHntSrcSelection3;
        private FraHntSrcSelection fraHntSrcSelection2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
