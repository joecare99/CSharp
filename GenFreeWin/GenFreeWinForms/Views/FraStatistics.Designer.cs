using Views;
using GenFree.ViewModels.Interfaces;

namespace Gen_FreeWin.Views
{
    partial class FraStatistics
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
            this.frmStatistics = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblTexts = new System.Windows.Forms.TextBox();
            this.lblDates = new System.Windows.Forms.TextBox();
            this.lblPlaces = new System.Windows.Forms.TextBox();
            this.lblFamilies = new System.Windows.Forms.TextBox();
            this.lblPersons = new System.Windows.Forms.TextBox();
            this.lblHdrTexts = new System.Windows.Forms.Label();
            this.lblHdrDates = new System.Windows.Forms.Label();
            this.lblHdrPlaces = new System.Windows.Forms.Label();
            this.lblHdrFamilies = new System.Windows.Forms.Label();
            this.lblHdrPersons = new System.Windows.Forms.Label();
            this.frmStatistics.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // frmStatistics
            // 
            this.frmStatistics.BackColor = System.Drawing.Color.Red;
            this.frmStatistics.Controls.Add(this.tableLayoutPanel1);
            this.frmStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmStatistics.Font = new System.Drawing.Font("Arial", 9F);
            this.frmStatistics.Location = new System.Drawing.Point(0, 0);
            this.frmStatistics.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmStatistics.Name = "frmStatistics";
            this.frmStatistics.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.frmStatistics.Size = new System.Drawing.Size(258, 183);
            this.frmStatistics.TabIndex = 30;
            this.frmStatistics.TabStop = false;
            this.frmStatistics.Text = "Datei-Statistik";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lblTexts, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblDates, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblPlaces, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblFamilies, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPersons, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblHdrTexts, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblHdrDates, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblHdrPlaces, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblHdrFamilies, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblHdrPersons, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(248, 154);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblTexts
            // 
            this.lblTexts.BackColor = System.Drawing.Color.White;
            this.lblTexts.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblTexts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTexts.Font = new System.Drawing.Font("Arial", 9F);
            this.lblTexts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTexts.Location = new System.Drawing.Point(2, 122);
            this.lblTexts.Margin = new System.Windows.Forms.Padding(2);
            this.lblTexts.Multiline = true;
            this.lblTexts.Name = "lblTexts";
            this.lblTexts.ReadOnly = true;
            this.lblTexts.Size = new System.Drawing.Size(120, 30);
            this.lblTexts.TabIndex = 19;
            this.lblTexts.Text = "<Texts>";
            this.lblTexts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDates
            // 
            this.lblDates.BackColor = System.Drawing.Color.White;
            this.lblDates.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblDates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDates.Font = new System.Drawing.Font("Arial", 9F);
            this.lblDates.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDates.Location = new System.Drawing.Point(2, 92);
            this.lblDates.Margin = new System.Windows.Forms.Padding(2);
            this.lblDates.Multiline = true;
            this.lblDates.Name = "lblDates";
            this.lblDates.ReadOnly = true;
            this.lblDates.Size = new System.Drawing.Size(120, 26);
            this.lblDates.TabIndex = 18;
            this.lblDates.Text = "<Dates>";
            this.lblDates.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPlaces
            // 
            this.lblPlaces.BackColor = System.Drawing.Color.White;
            this.lblPlaces.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPlaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPlaces.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPlaces.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPlaces.Location = new System.Drawing.Point(2, 62);
            this.lblPlaces.Margin = new System.Windows.Forms.Padding(2);
            this.lblPlaces.Multiline = true;
            this.lblPlaces.Name = "lblPlaces";
            this.lblPlaces.ReadOnly = true;
            this.lblPlaces.Size = new System.Drawing.Size(120, 26);
            this.lblPlaces.TabIndex = 17;
            this.lblPlaces.Text = "<Places>";
            this.lblPlaces.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblFamilies
            // 
            this.lblFamilies.BackColor = System.Drawing.Color.White;
            this.lblFamilies.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblFamilies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFamilies.Font = new System.Drawing.Font("Arial", 9F);
            this.lblFamilies.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFamilies.Location = new System.Drawing.Point(2, 32);
            this.lblFamilies.Margin = new System.Windows.Forms.Padding(2);
            this.lblFamilies.Multiline = true;
            this.lblFamilies.Name = "lblFamilies";
            this.lblFamilies.ReadOnly = true;
            this.lblFamilies.Size = new System.Drawing.Size(120, 26);
            this.lblFamilies.TabIndex = 16;
            this.lblFamilies.Text = "<Families>";
            this.lblFamilies.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblPersons
            // 
            this.lblPersons.BackColor = System.Drawing.Color.White;
            this.lblPersons.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPersons.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPersons.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPersons.Location = new System.Drawing.Point(2, 2);
            this.lblPersons.Margin = new System.Windows.Forms.Padding(2);
            this.lblPersons.Multiline = true;
            this.lblPersons.Name = "lblPersons";
            this.lblPersons.ReadOnly = true;
            this.lblPersons.Size = new System.Drawing.Size(120, 26);
            this.lblPersons.TabIndex = 15;
            this.lblPersons.Text = "<Persons>";
            this.lblPersons.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblHdrTexts
            // 
            this.lblHdrTexts.BackColor = System.Drawing.Color.White;
            this.lblHdrTexts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHdrTexts.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrTexts.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHdrTexts.Location = new System.Drawing.Point(126, 122);
            this.lblHdrTexts.Margin = new System.Windows.Forms.Padding(2);
            this.lblHdrTexts.Name = "lblHdrTexts";
            this.lblHdrTexts.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHdrTexts.Size = new System.Drawing.Size(120, 30);
            this.lblHdrTexts.TabIndex = 14;
            this.lblHdrTexts.Tag = "101";
            this.lblHdrTexts.Text = "Texte";
            // 
            // lblHdrDates
            // 
            this.lblHdrDates.BackColor = System.Drawing.Color.White;
            this.lblHdrDates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHdrDates.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrDates.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHdrDates.Location = new System.Drawing.Point(126, 92);
            this.lblHdrDates.Margin = new System.Windows.Forms.Padding(2);
            this.lblHdrDates.Name = "lblHdrDates";
            this.lblHdrDates.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHdrDates.Size = new System.Drawing.Size(120, 26);
            this.lblHdrDates.TabIndex = 13;
            this.lblHdrDates.Tag = "100";
            this.lblHdrDates.Text = "Datumssätze";
            // 
            // lblHdrPlaces
            // 
            this.lblHdrPlaces.BackColor = System.Drawing.Color.White;
            this.lblHdrPlaces.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHdrPlaces.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrPlaces.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHdrPlaces.Location = new System.Drawing.Point(126, 62);
            this.lblHdrPlaces.Margin = new System.Windows.Forms.Padding(2);
            this.lblHdrPlaces.Name = "lblHdrPlaces";
            this.lblHdrPlaces.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHdrPlaces.Size = new System.Drawing.Size(120, 26);
            this.lblHdrPlaces.TabIndex = 12;
            this.lblHdrPlaces.Tag = "99";
            this.lblHdrPlaces.Text = "Orte";
            // 
            // lblHdrFamilies
            // 
            this.lblHdrFamilies.BackColor = System.Drawing.Color.White;
            this.lblHdrFamilies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHdrFamilies.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrFamilies.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHdrFamilies.Location = new System.Drawing.Point(126, 32);
            this.lblHdrFamilies.Margin = new System.Windows.Forms.Padding(2);
            this.lblHdrFamilies.Name = "lblHdrFamilies";
            this.lblHdrFamilies.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHdrFamilies.Size = new System.Drawing.Size(120, 26);
            this.lblHdrFamilies.TabIndex = 11;
            this.lblHdrFamilies.Tag = "83";
            this.lblHdrFamilies.Text = "Familien";
            // 
            // lblHdrPersons
            // 
            this.lblHdrPersons.BackColor = System.Drawing.Color.White;
            this.lblHdrPersons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHdrPersons.Font = new System.Drawing.Font("Arial", 9F);
            this.lblHdrPersons.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblHdrPersons.Location = new System.Drawing.Point(126, 2);
            this.lblHdrPersons.Margin = new System.Windows.Forms.Padding(2);
            this.lblHdrPersons.Name = "lblHdrPersons";
            this.lblHdrPersons.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.lblHdrPersons.Size = new System.Drawing.Size(120, 26);
            this.lblHdrPersons.TabIndex = 10;
            this.lblHdrPersons.Tag = "84";
            this.lblHdrPersons.Text = "Personen";
            // 
            // FraStatictics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.frmStatistics);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "FraStatictics";
            this.Size = new System.Drawing.Size(258, 183);
            this.frmStatistics.ResumeLayout(false);
            this.frmStatistics.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox frmStatistics;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        [TextBinding(nameof(IFraStatisticsViewModel.Texts))]
        public System.Windows.Forms.TextBox lblTexts;
        [TextBinding(nameof(IFraStatisticsViewModel.Dates))]
        public System.Windows.Forms.TextBox lblDates;
        [TextBinding(nameof(IFraStatisticsViewModel.Places))]
        public System.Windows.Forms.TextBox lblPlaces;
        [TextBinding(nameof(IFraStatisticsViewModel.Families))]
        public System.Windows.Forms.TextBox lblFamilies;
        [TextBinding(nameof(IFraStatisticsViewModel.Persons))]
        public System.Windows.Forms.TextBox lblPersons;
        internal System.Windows.Forms.Label lblHdrTexts;
        internal System.Windows.Forms.Label lblHdrDates;
        internal System.Windows.Forms.Label lblHdrPlaces;
        internal System.Windows.Forms.Label lblHdrFamilies;
        internal System.Windows.Forms.Label lblHdrPersons;
    }
}
