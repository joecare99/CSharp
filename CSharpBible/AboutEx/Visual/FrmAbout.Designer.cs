namespace CSharpBible.AboutEx.Visual
{
    partial class FrmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblComments = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblProductName = new System.Windows.Forms.Label();
            this.pctProgramIcon = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctProgramIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlTop.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlTop.Controls.Add(this.lblComments);
            this.pnlTop.Controls.Add(this.lblCopyright);
            this.pnlTop.Controls.Add(this.lblVersion);
            this.pnlTop.Controls.Add(this.lblProductName);
            this.pnlTop.Controls.Add(this.pctProgramIcon);
            this.pnlTop.Location = new System.Drawing.Point(9, 8);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this.pnlTop.Size = new System.Drawing.Size(694, 281);
            this.pnlTop.TabIndex = 0;
            // 
            // lblComments
            // 
            this.lblComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComments.Location = new System.Drawing.Point(13, 149);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(663, 114);
            this.lblComments.TabIndex = 4;
            this.lblComments.Text = "Beta Test Version.\n!!! CONFIDENTIAL !!!\n Do not copy. Do not trade. All rights re" +
    "served.\n This means you.\n Violators shot at dawn.";
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright.Location = new System.Drawing.Point(16, 110);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(331, 25);
            this.lblCopyright.TabIndex = 3;
            this.lblCopyright.Text = "Copyleft © 2001 by the Ugly Ducking";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.Location = new System.Drawing.Point(117, 59);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(334, 25);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Code name Siberia aka. DuckCalc98";
            // 
            // lblProductName
            // 
            this.lblProductName.AutoSize = true;
            this.lblProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductName.Location = new System.Drawing.Point(117, 14);
            this.lblProductName.Name = "lblProductName";
            this.lblProductName.Size = new System.Drawing.Size(145, 25);
            this.lblProductName.TabIndex = 1;
            this.lblProductName.Text = "About Example";
            // 
            // pctProgramIcon
            // 
            this.pctProgramIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pctProgramIcon.BackgroundImage")));
            this.pctProgramIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pctProgramIcon.Location = new System.Drawing.Point(16, 14);
            this.pctProgramIcon.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pctProgramIcon.Name = "pctProgramIcon";
            this.pctProgramIcon.Size = new System.Drawing.Size(85, 77);
            this.pctProgramIcon.TabIndex = 0;
            this.pctProgramIcon.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.BackgroundImage = global::CSharpBible.AboutEx.Properties.Resources.CheckMarkGreen;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(193, 294);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(214, 88);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "all20.png");
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 390);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FrmAbout";
            this.Padding = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.Text = "FrmAbout";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctProgramIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.PictureBox pctProgramIcon;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblComments;
    }
}