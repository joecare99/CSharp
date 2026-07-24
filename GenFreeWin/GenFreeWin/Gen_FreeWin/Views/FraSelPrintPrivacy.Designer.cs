namespace GenFreeWin.Views
{
    partial class FraSelPrintPrivacy
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
            this.grpSelPrivacy = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rbtSelPrvFree = new System.Windows.Forms.RadioButton();
            this.rbtSelPrvPrivate = new System.Windows.Forms.RadioButton();
            this.rbtSelPrvLocked = new System.Windows.Forms.RadioButton();
            this.grpSelPrivacy.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSelPrivacy
            // 
            this.grpSelPrivacy.Controls.Add(this.flowLayoutPanel1);
            this.grpSelPrivacy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelPrivacy.Location = new System.Drawing.Point(0, 0);
            this.grpSelPrivacy.Name = "grpSelPrivacy";
            this.grpSelPrivacy.Size = new System.Drawing.Size(336, 96);
            this.grpSelPrivacy.TabIndex = 125;
            this.grpSelPrivacy.TabStop = false;
            this.grpSelPrivacy.Text = "Vertraulichkeit des Ausdrucks";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.rbtSelPrvFree);
            this.flowLayoutPanel1.Controls.Add(this.rbtSelPrvPrivate);
            this.flowLayoutPanel1.Controls.Add(this.rbtSelPrvLocked);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(330, 71);
            this.flowLayoutPanel1.TabIndex = 3;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // rbtSelPrvFree
            // 
            this.rbtSelPrvFree.AutoSize = true;
            this.rbtSelPrvFree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbtSelPrvFree.Checked = true;
            this.rbtSelPrvFree.Location = new System.Drawing.Point(15, 8);
            this.rbtSelPrvFree.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rbtSelPrvFree.MinimumSize = new System.Drawing.Size(80, 0);
            this.rbtSelPrvFree.Name = "rbtSelPrvFree";
            this.rbtSelPrvFree.Size = new System.Drawing.Size(80, 24);
            this.rbtSelPrvFree.TabIndex = 0;
            this.rbtSelPrvFree.TabStop = true;
            this.rbtSelPrvFree.Text = "frei";
            this.rbtSelPrvFree.UseVisualStyleBackColor = false;
            this.rbtSelPrvFree.CheckedChanged += new System.EventHandler(this.rbtSeletion_CheckedChanged);
            // 
            // rbtSelPrvPrivate
            // 
            this.rbtSelPrvPrivate.AutoSize = true;
            this.rbtSelPrvPrivate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbtSelPrvPrivate.Location = new System.Drawing.Point(105, 8);
            this.rbtSelPrvPrivate.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rbtSelPrvPrivate.MinimumSize = new System.Drawing.Size(80, 0);
            this.rbtSelPrvPrivate.Name = "rbtSelPrvPrivate";
            this.rbtSelPrvPrivate.Size = new System.Drawing.Size(80, 24);
            this.rbtSelPrvPrivate.TabIndex = 1;
            this.rbtSelPrvPrivate.TabStop = true;
            this.rbtSelPrvPrivate.Text = "privat";
            this.rbtSelPrvPrivate.UseVisualStyleBackColor = false;
            // 
            // rbtSelPrvLocked
            // 
            this.rbtSelPrvLocked.AutoSize = true;
            this.rbtSelPrvLocked.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.rbtSelPrvLocked.Location = new System.Drawing.Point(195, 8);
            this.rbtSelPrvLocked.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.rbtSelPrvLocked.MinimumSize = new System.Drawing.Size(80, 0);
            this.rbtSelPrvLocked.Name = "rbtSelPrvLocked";
            this.rbtSelPrvLocked.Size = new System.Drawing.Size(93, 24);
            this.rbtSelPrvLocked.TabIndex = 2;
            this.rbtSelPrvLocked.TabStop = true;
            this.rbtSelPrvLocked.Text = "gesperrt";
            this.rbtSelPrvLocked.UseVisualStyleBackColor = false;
            // 
            // FraSelPrintPrivacy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSelPrivacy);
            this.Name = "FraSelPrintPrivacy";
            this.Size = new System.Drawing.Size(336, 96);
            this.Load += new System.EventHandler(this.FraSelPrintPrivacy_Load);
            this.grpSelPrivacy.ResumeLayout(false);
            this.grpSelPrivacy.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox grpSelPrivacy;
        internal System.Windows.Forms.RadioButton rbtSelPrvLocked;
        internal System.Windows.Forms.RadioButton rbtSelPrvPrivate;
        internal System.Windows.Forms.RadioButton rbtSelPrvFree;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
