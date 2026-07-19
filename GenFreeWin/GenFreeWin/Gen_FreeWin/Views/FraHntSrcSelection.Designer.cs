namespace GenFreeWin.Views
{
    partial class FraHntSrcSelection
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
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.RadioButton4 = new System.Windows.Forms.RadioButton();
            this.RadioButton5 = new System.Windows.Forms.RadioButton();
            this.RadioButton6 = new System.Windows.Forms.RadioButton();
            this.RadioButton7 = new System.Windows.Forms.RadioButton();
            this.RadioButton8 = new System.Windows.Forms.RadioButton();
            this.RadioButton9 = new System.Windows.Forms.RadioButton();
            this.RadioButton10 = new System.Windows.Forms.RadioButton();
            this.RadioButton11 = new System.Windows.Forms.RadioButton();
            this.RadioButton12 = new System.Windows.Forms.RadioButton();
            this.GroupBox4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox4
            // 
            this.GroupBox4.AutoSize = true;
            this.GroupBox4.Controls.Add(this.flowLayoutPanel1);
            this.GroupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupBox4.Location = new System.Drawing.Point(0, 0);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(350, 383);
            this.GroupBox4.TabIndex = 2;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Suchfeld 1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.RadioButton4);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton5);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton6);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton7);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton8);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton9);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton10);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton11);
            this.flowLayoutPanel1.Controls.Add(this.RadioButton12);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(344, 358);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // RadioButton4
            // 
            this.RadioButton4.AutoSize = true;
            this.RadioButton4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton4.Checked = true;
            this.RadioButton4.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton4.Location = new System.Drawing.Point(8, 8);
            this.RadioButton4.Name = "RadioButton4";
            this.RadioButton4.Size = new System.Drawing.Size(310, 24);
            this.RadioButton4.TabIndex = 0;
            this.RadioButton4.TabStop = true;
            this.RadioButton4.Text = "manuelle Eingabe";
            this.RadioButton4.UseVisualStyleBackColor = false;
            this.RadioButton4.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton5
            // 
            this.RadioButton5.AutoSize = true;
            this.RadioButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton5.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton5.Location = new System.Drawing.Point(8, 38);
            this.RadioButton5.Name = "RadioButton5";
            this.RadioButton5.Size = new System.Drawing.Size(310, 24);
            this.RadioButton5.TabIndex = 1;
            this.RadioButton5.TabStop = true;
            this.RadioButton5.Text = "Name,Vorname";
            this.RadioButton5.UseVisualStyleBackColor = false;
            this.RadioButton5.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton6
            // 
            this.RadioButton6.AutoSize = true;
            this.RadioButton6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton6.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton6.Location = new System.Drawing.Point(8, 68);
            this.RadioButton6.Name = "RadioButton6";
            this.RadioButton6.Size = new System.Drawing.Size(310, 24);
            this.RadioButton6.TabIndex = 2;
            this.RadioButton6.Text = "Name,Vorname */~ Datum (JJJJMMTT)";
            this.RadioButton6.UseVisualStyleBackColor = false;
            this.RadioButton6.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton7
            // 
            this.RadioButton7.AutoSize = true;
            this.RadioButton7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton7.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton7.Location = new System.Drawing.Point(8, 98);
            this.RadioButton7.Name = "RadioButton7";
            this.RadioButton7.Size = new System.Drawing.Size(310, 24);
            this.RadioButton7.TabIndex = 3;
            this.RadioButton7.Text = "*/~ Datum (JJJMMTT Name,Vorname";
            this.RadioButton7.UseVisualStyleBackColor = false;
            this.RadioButton7.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton8
            // 
            this.RadioButton8.AutoSize = true;
            this.RadioButton8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton8.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton8.Location = new System.Drawing.Point(8, 128);
            this.RadioButton8.Name = "RadioButton8";
            this.RadioButton8.Size = new System.Drawing.Size(310, 24);
            this.RadioButton8.TabIndex = 4;
            this.RadioButton8.Text = "Ahnennummern";
            this.RadioButton8.UseVisualStyleBackColor = false;
            this.RadioButton8.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton9
            // 
            this.RadioButton9.AutoSize = true;
            this.RadioButton9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton9.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton9.Location = new System.Drawing.Point(8, 158);
            this.RadioButton9.Name = "RadioButton9";
            this.RadioButton9.Size = new System.Drawing.Size(310, 24);
            this.RadioButton9.TabIndex = 5;
            this.RadioButton9.Text = "Geburtsregister";
            this.RadioButton9.UseVisualStyleBackColor = false;
            this.RadioButton9.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton10
            // 
            this.RadioButton10.AutoSize = true;
            this.RadioButton10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton10.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton10.Location = new System.Drawing.Point(8, 188);
            this.RadioButton10.Name = "RadioButton10";
            this.RadioButton10.Size = new System.Drawing.Size(310, 24);
            this.RadioButton10.TabIndex = 6;
            this.RadioButton10.Text = "Taufregister";
            this.RadioButton10.UseVisualStyleBackColor = false;
            this.RadioButton10.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton11
            // 
            this.RadioButton11.AutoSize = true;
            this.RadioButton11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton11.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton11.Location = new System.Drawing.Point(8, 218);
            this.RadioButton11.Name = "RadioButton11";
            this.RadioButton11.Size = new System.Drawing.Size(310, 24);
            this.RadioButton11.TabIndex = 7;
            this.RadioButton11.Text = "Sterberegister";
            this.RadioButton11.UseVisualStyleBackColor = false;
            this.RadioButton11.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // RadioButton12
            // 
            this.RadioButton12.AutoSize = true;
            this.RadioButton12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.RadioButton12.Dock = System.Windows.Forms.DockStyle.Top;
            this.RadioButton12.Location = new System.Drawing.Point(8, 248);
            this.RadioButton12.Name = "RadioButton12";
            this.RadioButton12.Size = new System.Drawing.Size(310, 24);
            this.RadioButton12.TabIndex = 8;
            this.RadioButton12.Text = "Begräbnisregister";
            this.RadioButton12.UseVisualStyleBackColor = false;
            this.RadioButton12.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // FraHntSrcSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBox4);
            this.Name = "FraHntSrcSelection";
            this.Size = new System.Drawing.Size(350, 383);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.RadioButton RadioButton12;
        public System.Windows.Forms.RadioButton RadioButton11;
        public System.Windows.Forms.RadioButton RadioButton10;
        public System.Windows.Forms.RadioButton RadioButton9;
        public System.Windows.Forms.RadioButton RadioButton8;
        public System.Windows.Forms.RadioButton RadioButton7;
        public System.Windows.Forms.RadioButton RadioButton6;
        public System.Windows.Forms.RadioButton RadioButton5;
        public System.Windows.Forms.RadioButton RadioButton4;
    }
}
