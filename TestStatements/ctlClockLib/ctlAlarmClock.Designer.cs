namespace ctlClockLib
{
    partial class ctlAlarmClock
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
            this.lblAlarm = new System.Windows.Forms.Label();
            this.btnAlarmOff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAlarm
            // 
            this.lblAlarm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblAlarm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlarm.Location = new System.Drawing.Point(0, 414);
            this.lblAlarm.Name = "lblAlarm";
            this.lblAlarm.Size = new System.Drawing.Size(800, 36);
            this.lblAlarm.TabIndex = 1;
            this.lblAlarm.Text = "Alarm!";
            this.lblAlarm.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblAlarm.Visible = false;
            // 
            // btnAlarmOff
            // 
            this.btnAlarmOff.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAlarmOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAlarmOff.Location = new System.Drawing.Point(0, 0);
            this.btnAlarmOff.Name = "btnAlarmOff";
            this.btnAlarmOff.Size = new System.Drawing.Size(800, 43);
            this.btnAlarmOff.TabIndex = 2;
            this.btnAlarmOff.Text = "Wecker deaktivieren";
            this.btnAlarmOff.UseVisualStyleBackColor = true;
            this.btnAlarmOff.Click += new System.EventHandler(this.btnAlarmOff_Click);
            // 
            // ctlAlarmClock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAlarmOff);
            this.Controls.Add(this.lblAlarm);
            this.Name = "ctlAlarmClock";
            this.Controls.SetChildIndex(this.lblAlarm, 0);
            this.Controls.SetChildIndex(this.btnAlarmOff, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAlarm;
        private System.Windows.Forms.Button btnAlarmOff;
    }
}
