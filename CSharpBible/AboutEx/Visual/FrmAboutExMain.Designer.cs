// ***********************************************************************
// Assembly         : AboutEx
// Author           : Mir
// Created          : 11-11-2022
//
// Last Modified By : Mir
// Last Modified On : 11-08-2022
// ***********************************************************************
// <copyright file="FrmAboutExMain.Designer.cs" company="HP Inc.">
//     Copyright (c) HP Inc.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Visual namespace.
/// </summary>
namespace CSharpBible.AboutEx.Visual
{
    /// <summary>
    /// Class FrmAboutExMain.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    partial class FrmAboutExMain
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClickMe = new System.Windows.Forms.Button();
            this.btnClickMe2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClickMe
            // 
            this.btnClickMe.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnClickMe.FlatAppearance.BorderSize = 3;
            this.btnClickMe.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClickMe.Location = new System.Drawing.Point(68, 31);
            this.btnClickMe.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClickMe.Name = "btnClickMe";
            this.btnClickMe.Size = new System.Drawing.Size(574, 125);
            this.btnClickMe.TabIndex = 0;
            this.btnClickMe.Text = "Click Me! ...";
            this.btnClickMe.UseVisualStyleBackColor = true;
            this.btnClickMe.Click += new System.EventHandler(this.btnClickMe_Click);
            // 
            // btnClickMe2
            // 
            this.btnClickMe2.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnClickMe2.FlatAppearance.BorderSize = 3;
            this.btnClickMe2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClickMe2.Location = new System.Drawing.Point(68, 172);
            this.btnClickMe2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClickMe2.Name = "btnClickMe2";
            this.btnClickMe2.Size = new System.Drawing.Size(574, 125);
            this.btnClickMe2.TabIndex = 1;
            this.btnClickMe2.Text = "Click Me! ...";
            this.btnClickMe2.UseVisualStyleBackColor = true;
            this.btnClickMe2.Click += new System.EventHandler(this.btnClickMe2_Click);
            // 
            // FrmAboutExMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 360);
            this.Controls.Add(this.btnClickMe2);
            this.Controls.Add(this.btnClickMe);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FrmAboutExMain";
            this.Text = "About Example";
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// The BTN click me
        /// </summary>
        private System.Windows.Forms.Button btnClickMe;
        /// <summary>
        /// The BTN click me2
        /// </summary>
        private System.Windows.Forms.Button btnClickMe2;
    }
}

