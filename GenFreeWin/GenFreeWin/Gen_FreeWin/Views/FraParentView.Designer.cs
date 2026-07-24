using GenFree.ViewModels.Interfaces;
using Views;

namespace GenFreeWin.Views
{
    partial class FraParentView
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
            this.lblGrandmother = new System.Windows.Forms.Label();
            this.lblGrandfather = new System.Windows.Forms.Label();
            this.frmParent = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.edtParentPNr = new System.Windows.Forms.TextBox();
            this.lblParentNrMarr = new System.Windows.Forms.Label();
            this.lblDeleteParent = new System.Windows.Forms.Label();
            this.lblParentName = new System.Windows.Forms.Label();
            this.lblParentGivn = new System.Windows.Forms.Label();
            this.lblParent_12 = new System.Windows.Forms.Label();
            this.lblParentResidence = new System.Windows.Forms.Label();
            this.lblParent_8 = new System.Windows.Forms.Label();
            this.lblParentAka = new System.Windows.Forms.Label();
            this.lblParentTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.frmParent.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGrandmother
            // 
            this.lblGrandmother.AutoEllipsis = true;
            this.lblGrandmother.BackColor = System.Drawing.Color.White;
            this.lblGrandmother.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGrandmother.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblGrandmother.Location = new System.Drawing.Point(2, 18);
            this.lblGrandmother.Margin = new System.Windows.Forms.Padding(2, 0, 2, 1);
            this.lblGrandmother.Name = "lblGrandmother";
            this.lblGrandmother.Size = new System.Drawing.Size(496, 17);
            this.lblGrandmother.TabIndex = 22;
            this.lblGrandmother.Text = "<Großmutter>";
            this.lblGrandmother.UseMnemonic = false;
            // 
            // lblGrandfather
            // 
            this.lblGrandfather.AutoEllipsis = true;
            this.lblGrandfather.BackColor = System.Drawing.Color.White;
            this.lblGrandfather.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGrandfather.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.lblGrandfather.Location = new System.Drawing.Point(2, 0);
            this.lblGrandfather.Margin = new System.Windows.Forms.Padding(2, 0, 2, 1);
            this.lblGrandfather.Name = "lblGrandfather";
            this.lblGrandfather.Size = new System.Drawing.Size(496, 17);
            this.lblGrandfather.TabIndex = 21;
            this.lblGrandfather.Text = "<Großvater>";
            this.lblGrandfather.UseMnemonic = false;
            this.lblGrandfather.Click += new System.EventHandler(this.lblGrandparent_Click);
            // 
            // frmParent
            // 
            this.frmParent.BackColor = System.Drawing.Color.Red;
            this.frmParent.Controls.Add(this.tableLayoutPanel2);
            this.frmParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmParent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.frmParent.Location = new System.Drawing.Point(0, 36);
            this.frmParent.Margin = new System.Windows.Forms.Padding(0);
            this.frmParent.Name = "frmParent";
            this.frmParent.Padding = new System.Windows.Forms.Padding(2);
            this.frmParent.Size = new System.Drawing.Size(500, 173);
            this.frmParent.TabIndex = 23;
            this.frmParent.TabStop = false;
            this.frmParent.Text = "<Relation>";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.edtParentPNr, 2, 7);
            this.tableLayoutPanel2.Controls.Add(this.lblParentNrMarr, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.lblDeleteParent, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.lblParentName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblParentGivn, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblParent_12, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.lblParentResidence, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.lblParent_8, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.lblParentAka, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.lblParentTitle, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 21);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.49953F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.50329F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(496, 150);
            this.tableLayoutPanel2.TabIndex = 29;
            // 
            // edtParentPNr
            // 
            this.edtParentPNr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtParentPNr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edtParentPNr.Location = new System.Drawing.Point(372, 126);
            this.edtParentPNr.Margin = new System.Windows.Forms.Padding(0);
            this.edtParentPNr.Multiline = true;
            this.edtParentPNr.Name = "edtParentPNr";
            this.edtParentPNr.Size = new System.Drawing.Size(124, 22);
            this.edtParentPNr.TabIndex = 28;
            this.edtParentPNr.Text = "<väterl. ID>";
            this.edtParentPNr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.edtParentPNr_KeyPress);
            this.edtParentPNr.KeyUp += new System.Windows.Forms.KeyEventHandler(this.edtParentPNr_KeyUp);
            // 
            // lblParentNrMarr
            // 
            this.lblParentNrMarr.AutoEllipsis = true;
            this.lblParentNrMarr.BackColor = System.Drawing.Color.White;
            this.lblParentNrMarr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParentNrMarr.Font = new System.Drawing.Font("Arial", 8.25F);
            this.lblParentNrMarr.Location = new System.Drawing.Point(1, 126);
            this.lblParentNrMarr.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParentNrMarr.Name = "lblParentNrMarr";
            this.lblParentNrMarr.Size = new System.Drawing.Size(246, 21);
            this.lblParentNrMarr.TabIndex = 26;
            this.lblParentNrMarr.Text = "Anz. Ehen: <##>";
            this.lblParentNrMarr.UseMnemonic = false;
            this.lblParentNrMarr.Click += new System.EventHandler(this.lblParentNrMarr_Click);
            // 
            // lblDeleteParent
            // 
            this.lblDeleteParent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblDeleteParent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDeleteParent.Font = new System.Drawing.Font("Arial", 8.25F);
            this.lblDeleteParent.Location = new System.Drawing.Point(250, 128);
            this.lblDeleteParent.Margin = new System.Windows.Forms.Padding(2);
            this.lblDeleteParent.Name = "lblDeleteParent";
            this.lblDeleteParent.Size = new System.Drawing.Size(120, 18);
            this.lblDeleteParent.TabIndex = 27;
            this.lblDeleteParent.Text = "Personen-Nr.:";
            this.lblDeleteParent.Click += new System.EventHandler(this.lblDeleteParent_Click);
            // 
            // lblParentName
            // 
            this.lblParentName.AutoEllipsis = true;
            this.lblParentName.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParentName, 3);
            this.lblParentName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParentName.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentName.Location = new System.Drawing.Point(1, 0);
            this.lblParentName.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParentName.Name = "lblParentName";
            this.lblParentName.Size = new System.Drawing.Size(494, 17);
            this.lblParentName.TabIndex = 19;
            this.lblParentName.Text = "<väterl. Nachname>";
            this.lblParentName.UseMnemonic = false;
            this.lblParentName.Click += new System.EventHandler(this.Label_Click);
            // 
            // lblParentGivn
            // 
            this.lblParentGivn.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParentGivn, 3);
            this.lblParentGivn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParentGivn.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentGivn.Location = new System.Drawing.Point(1, 18);
            this.lblParentGivn.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParentGivn.Name = "lblParentGivn";
            this.lblParentGivn.Size = new System.Drawing.Size(494, 17);
            this.lblParentGivn.TabIndex = 20;
            this.lblParentGivn.Text = "<väterl. Vorname>";
            this.lblParentGivn.UseMnemonic = false;
            // 
            // lblParent_12
            // 
            this.lblParent_12.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParent_12, 3);
            this.lblParent_12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParent_12.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParent_12.Location = new System.Drawing.Point(1, 108);
            this.lblParent_12.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParent_12.Name = "lblParent_12";
            this.lblParent_12.Size = new System.Drawing.Size(494, 17);
            this.lblParent_12.TabIndex = 25;
            this.lblParent_12.UseMnemonic = false;
            // 
            // lblParentResidence
            // 
            this.lblParentResidence.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParentResidence, 3);
            this.lblParentResidence.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParentResidence.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentResidence.Location = new System.Drawing.Point(1, 90);
            this.lblParentResidence.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParentResidence.Name = "lblParentResidence";
            this.lblParentResidence.Size = new System.Drawing.Size(494, 17);
            this.lblParentResidence.TabIndex = 23;
            this.lblParentResidence.UseMnemonic = false;
            // 
            // lblParent_8
            // 
            this.lblParent_8.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParent_8, 3);
            this.lblParent_8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParent_8.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParent_8.Location = new System.Drawing.Point(1, 36);
            this.lblParent_8.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParent_8.Name = "lblParent_8";
            this.lblParent_8.Size = new System.Drawing.Size(494, 17);
            this.lblParent_8.TabIndex = 21;
            this.lblParent_8.Text = "<Zusatz>";
            this.lblParent_8.UseMnemonic = false;
            // 
            // lblParentAka
            // 
            this.lblParentAka.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParentAka, 3);
            this.lblParentAka.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParentAka.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentAka.Location = new System.Drawing.Point(1, 54);
            this.lblParentAka.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParentAka.Name = "lblParentAka";
            this.lblParentAka.Size = new System.Drawing.Size(494, 17);
            this.lblParentAka.TabIndex = 22;
            this.lblParentAka.Text = "<AKA>";
            this.lblParentAka.UseMnemonic = false;
            // 
            // lblParentTitle
            // 
            this.lblParentTitle.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel2.SetColumnSpan(this.lblParentTitle, 3);
            this.lblParentTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblParentTitle.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParentTitle.Location = new System.Drawing.Point(1, 72);
            this.lblParentTitle.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.lblParentTitle.Name = "lblParentTitle";
            this.lblParentTitle.Size = new System.Drawing.Size(494, 17);
            this.lblParentTitle.TabIndex = 24;
            this.lblParentTitle.Text = "<Titel>";
            this.lblParentTitle.UseMnemonic = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblGrandmother, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblGrandfather, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.frmParent, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 209);
            this.tableLayoutPanel1.TabIndex = 24;
            // 
            // FraParentView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FraParentView";
            this.Size = new System.Drawing.Size(500, 209);
            this.frmParent.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        [TextBinding(nameof(IPersonRedViewModel.Mother_Text))]
        [CommandBinding(nameof(IPersonRedViewModel.GrandparentCommand))]
        public System.Windows.Forms.Label lblGrandmother;
        [TextBinding(nameof(IPersonRedViewModel.Father_Text))]
        [CommandBinding(nameof(IPersonRedViewModel.GrandparentCommand))]
        public System.Windows.Forms.Label lblGrandfather;
        public System.Windows.Forms.GroupBox frmParent;
        [TextBinding(nameof(IPersonRedViewModel.PersonId))]
        public System.Windows.Forms.TextBox edtParentPNr;
        [CommandBinding(nameof(IPersonRedViewModel.DeletePersonCommand))]
        public System.Windows.Forms.Label lblDeleteParent;
        [TextBinding(nameof(IPersonRedViewModel.Marriages))]
        public System.Windows.Forms.Label lblParentNrMarr;
        [TextBinding(nameof(IPersonRedViewModel.PersonAdditional))]
        public System.Windows.Forms.Label lblParent_12;
        [TextBinding(nameof(IPersonRedViewModel.PersonTitle))]
        public System.Windows.Forms.Label lblParentTitle;
        [TextBinding(nameof(IPersonRedViewModel.PersonResidence))]
        public System.Windows.Forms.Label lblParentResidence;
        [TextBinding(nameof(IPersonRedViewModel.PersonGivenName))]
        public System.Windows.Forms.Label lblParentGivn;
        [TextBinding(nameof(IPersonRedViewModel.PersonSurname))]
        [CommandBinding(nameof(IPersonRedViewModel.PersonNameCommand))]
        public System.Windows.Forms.Label lblParentName;
        [TextBinding(nameof(IPersonRedViewModel.PersonAKA))]
        public System.Windows.Forms.Label lblParentAka;
        [TextBinding(nameof(IPersonRedViewModel.PersonNotes))]
        public System.Windows.Forms.Label lblParent_8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
