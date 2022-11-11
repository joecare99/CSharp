// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="FrmCalc64Main.Designer.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Calc64WF.Properties;

namespace Calc64WF.Visual
{
    /// <summary>
    /// Class FrmCalc64Main.
    /// Implements the <see cref="System.Windows.Forms.Form" />
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    partial class FrmCalc64Main
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
            this.btnNum1 = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnNum2 = new System.Windows.Forms.Button();
            this.btnNum3 = new System.Windows.Forms.Button();
            this.btnNum4 = new System.Windows.Forms.Button();
            this.btnNum5 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.btnHexA = new System.Windows.Forms.Button();
            this.btnNum0 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.lblOperation = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnHexB = new System.Windows.Forms.Button();
            this.btnHexC = new System.Windows.Forms.Button();
            this.btnHexF = new System.Windows.Forms.Button();
            this.btnHexE = new System.Windows.Forms.Button();
            this.btnHexD = new System.Windows.Forms.Button();
            this.btnMult = new System.Windows.Forms.Button();
            this.btnPlus = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnResult = new System.Windows.Forms.Button();
            this.btnOpAnd = new System.Windows.Forms.Button();
            this.btnOpOR = new System.Windows.Forms.Button();
            this.btnOpXOR = new System.Windows.Forms.Button();
            this.btnOpNOT = new System.Windows.Forms.Button();
            this.btnMemory = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlMaster = new System.Windows.Forms.Panel();
            this.btnDivide = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNum1
            // 
            this.btnNum1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNum1.AutoEllipsis = true;
            this.btnNum1.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnNum1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnNum1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNum1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNum1.Location = new System.Drawing.Point(546, 492);
            this.btnNum1.Name = "btnNum1";
            this.btnNum1.Padding = new System.Windows.Forms.Padding(3);
            this.btnNum1.Size = new System.Drawing.Size(60, 60);
            this.btnNum1.TabIndex = 0;
            this.btnNum1.Tag = "1";
            this.btnNum1.Text = "&1";
            this.btnNum1.UseVisualStyleBackColor = true;
            this.btnNum1.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnNum1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnNum1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // lblResult
            // 
            this.lblResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.lblResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(186, 72);
            this.lblResult.Name = "lblResult";
            this.lblResult.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblResult.Size = new System.Drawing.Size(699, 57);
            this.lblResult.TabIndex = 1;
            this.lblResult.Text = "0";
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblResult.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::Calc64WF.Properties.Resources.Glow_White;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(940, 589);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(195, 208);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnNum2
            // 
            this.btnNum2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNum2.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnNum2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnNum2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNum2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNum2.Location = new System.Drawing.Point(610, 492);
            this.btnNum2.Name = "btnNum2";
            this.btnNum2.Padding = new System.Windows.Forms.Padding(3);
            this.btnNum2.Size = new System.Drawing.Size(60, 60);
            this.btnNum2.TabIndex = 4;
            this.btnNum2.Tag = "2";
            this.btnNum2.Text = "&2";
            this.btnNum2.UseVisualStyleBackColor = true;
            this.btnNum2.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnNum2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnNum2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnNum3
            // 
            this.btnNum3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNum3.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnNum3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnNum3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNum3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNum3.Location = new System.Drawing.Point(671, 492);
            this.btnNum3.Name = "btnNum3";
            this.btnNum3.Padding = new System.Windows.Forms.Padding(3);
            this.btnNum3.Size = new System.Drawing.Size(60, 60);
            this.btnNum3.TabIndex = 5;
            this.btnNum3.Tag = "3";
            this.btnNum3.Text = "&3";
            this.btnNum3.UseVisualStyleBackColor = true;
            this.btnNum3.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnNum3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnNum3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnNum4
            // 
            this.btnNum4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNum4.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnNum4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnNum4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNum4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNum4.Location = new System.Drawing.Point(546, 427);
            this.btnNum4.Name = "btnNum4";
            this.btnNum4.Padding = new System.Windows.Forms.Padding(3);
            this.btnNum4.Size = new System.Drawing.Size(60, 60);
            this.btnNum4.TabIndex = 6;
            this.btnNum4.Tag = "4";
            this.btnNum4.Text = "&4";
            this.btnNum4.UseVisualStyleBackColor = true;
            this.btnNum4.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnNum4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnNum4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnNum5
            // 
            this.btnNum5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNum5.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnNum5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnNum5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNum5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNum5.Location = new System.Drawing.Point(610, 427);
            this.btnNum5.Name = "btnNum5";
            this.btnNum5.Padding = new System.Windows.Forms.Padding(3);
            this.btnNum5.Size = new System.Drawing.Size(60, 60);
            this.btnNum5.TabIndex = 7;
            this.btnNum5.Tag = "5";
            this.btnNum5.Text = "5";
            this.btnNum5.UseVisualStyleBackColor = true;
            this.btnNum5.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnNum5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnNum5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(671, 427);
            this.button5.Name = "button5";
            this.button5.Padding = new System.Windows.Forms.Padding(3);
            this.button5.Size = new System.Drawing.Size(60, 60);
            this.button5.TabIndex = 8;
            this.button5.Tag = "6";
            this.button5.Text = "6";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.btnNummber_Click);
            this.button5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(546, 330);
            this.button6.Name = "button6";
            this.button6.Padding = new System.Windows.Forms.Padding(3);
            this.button6.Size = new System.Drawing.Size(60, 60);
            this.button6.TabIndex = 9;
            this.button6.Tag = "7";
            this.button6.Text = "7";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.btnNummber_Click);
            this.button6.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button6.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(610, 330);
            this.button7.Name = "button7";
            this.button7.Padding = new System.Windows.Forms.Padding(3);
            this.button7.Size = new System.Drawing.Size(60, 60);
            this.button7.TabIndex = 10;
            this.button7.Tag = "8";
            this.button7.Text = "8";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.btnNummber_Click);
            this.button7.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button7.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button8.Location = new System.Drawing.Point(671, 330);
            this.button8.Name = "button8";
            this.button8.Padding = new System.Windows.Forms.Padding(3);
            this.button8.Size = new System.Drawing.Size(60, 60);
            this.button8.TabIndex = 11;
            this.button8.Tag = "9";
            this.button8.Text = "9";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.btnNummber_Click);
            this.button8.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button8.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnHexA
            // 
            this.btnHexA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHexA.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnHexA.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnHexA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHexA.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHexA.Location = new System.Drawing.Point(546, 265);
            this.btnHexA.Name = "btnHexA";
            this.btnHexA.Padding = new System.Windows.Forms.Padding(3);
            this.btnHexA.Size = new System.Drawing.Size(60, 60);
            this.btnHexA.TabIndex = 12;
            this.btnHexA.Tag = "10";
            this.btnHexA.Text = "A";
            this.btnHexA.UseVisualStyleBackColor = true;
            this.btnHexA.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnHexA.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnNum0
            // 
            this.btnNum0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNum0.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnNum0.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnNum0.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNum0.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNum0.Location = new System.Drawing.Point(546, 557);
            this.btnNum0.Name = "btnNum0";
            this.btnNum0.Padding = new System.Windows.Forms.Padding(3);
            this.btnNum0.Size = new System.Drawing.Size(120, 60);
            this.btnNum0.TabIndex = 13;
            this.btnNum0.Tag = "0";
            this.btnNum0.Text = "0";
            this.btnNum0.UseVisualStyleBackColor = true;
            this.btnNum0.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnNum0.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnNum0.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button11
            // 
            this.button11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button11.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button11.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button11.Location = new System.Drawing.Point(671, 557);
            this.button11.Name = "button11";
            this.button11.Padding = new System.Windows.Forms.Padding(3);
            this.button11.Size = new System.Drawing.Size(60, 60);
            this.button11.TabIndex = 14;
            this.button11.Tag = "+/-";
            this.button11.Text = "+/-";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.btnOperator_Click);
            this.button11.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button11.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // lblOperation
            // 
            this.lblOperation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOperation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.lblOperation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOperation.Location = new System.Drawing.Point(779, 9);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOperation.Size = new System.Drawing.Size(106, 57);
            this.lblOperation.TabIndex = 15;
            this.lblOperation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblOperation.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // lblMemory
            // 
            this.lblMemory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMemory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.lblMemory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemory.Location = new System.Drawing.Point(186, 9);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMemory.Size = new System.Drawing.Size(587, 57);
            this.lblMemory.TabIndex = 16;
            this.lblMemory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblMemory.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(124, 200);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(118, 58);
            this.btnBack.TabIndex = 17;
            this.btnBack.Tag = "+2";
            this.btnBack.Text = "<==";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.btnBack.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(332, 200);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(3);
            this.btnClear.Size = new System.Drawing.Size(120, 60);
            this.btnClear.TabIndex = 18;
            this.btnClear.Tag = "+1";
            this.btnClear.Text = "C";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnClear.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearAll.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnClearAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearAll.Location = new System.Drawing.Point(332, 265);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Padding = new System.Windows.Forms.Padding(3);
            this.btnClearAll.Size = new System.Drawing.Size(120, 60);
            this.btnClearAll.TabIndex = 19;
            this.btnClearAll.Tag = "+3";
            this.btnClearAll.Text = "&CE";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnClearAll.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnHexB
            // 
            this.btnHexB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHexB.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnHexB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnHexB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHexB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHexB.Location = new System.Drawing.Point(610, 265);
            this.btnHexB.Name = "btnHexB";
            this.btnHexB.Padding = new System.Windows.Forms.Padding(3);
            this.btnHexB.Size = new System.Drawing.Size(60, 60);
            this.btnHexB.TabIndex = 20;
            this.btnHexB.Tag = "11";
            this.btnHexB.Text = "B";
            this.btnHexB.UseVisualStyleBackColor = true;
            this.btnHexB.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnHexC
            // 
            this.btnHexC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHexC.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnHexC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnHexC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHexC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHexC.Location = new System.Drawing.Point(671, 265);
            this.btnHexC.Name = "btnHexC";
            this.btnHexC.Padding = new System.Windows.Forms.Padding(3);
            this.btnHexC.Size = new System.Drawing.Size(60, 60);
            this.btnHexC.TabIndex = 21;
            this.btnHexC.Tag = "12";
            this.btnHexC.Text = "C";
            this.btnHexC.UseVisualStyleBackColor = true;
            this.btnHexC.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexC.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnHexF
            // 
            this.btnHexF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHexF.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnHexF.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnHexF.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHexF.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHexF.Location = new System.Drawing.Point(674, 200);
            this.btnHexF.Name = "btnHexF";
            this.btnHexF.Padding = new System.Windows.Forms.Padding(3);
            this.btnHexF.Size = new System.Drawing.Size(60, 60);
            this.btnHexF.TabIndex = 24;
            this.btnHexF.Tag = "15";
            this.btnHexF.Text = "F";
            this.btnHexF.UseVisualStyleBackColor = true;
            this.btnHexF.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexF.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnHexE
            // 
            this.btnHexE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHexE.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnHexE.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnHexE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHexE.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHexE.Location = new System.Drawing.Point(610, 200);
            this.btnHexE.Name = "btnHexE";
            this.btnHexE.Padding = new System.Windows.Forms.Padding(3);
            this.btnHexE.Size = new System.Drawing.Size(60, 60);
            this.btnHexE.TabIndex = 23;
            this.btnHexE.Tag = "14";
            this.btnHexE.Text = "E";
            this.btnHexE.UseVisualStyleBackColor = true;
            this.btnHexE.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexE.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnHexD
            // 
            this.btnHexD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHexD.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnHexD.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnHexD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHexD.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHexD.Location = new System.Drawing.Point(546, 200);
            this.btnHexD.Name = "btnHexD";
            this.btnHexD.Padding = new System.Windows.Forms.Padding(3);
            this.btnHexD.Size = new System.Drawing.Size(60, 60);
            this.btnHexD.TabIndex = 22;
            this.btnHexD.Tag = "13";
            this.btnHexD.Text = "D";
            this.btnHexD.UseVisualStyleBackColor = true;
            this.btnHexD.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexD.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnMult
            // 
            this.btnMult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMult.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnMult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnMult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMult.Location = new System.Drawing.Point(482, 200);
            this.btnMult.Name = "btnMult";
            this.btnMult.Padding = new System.Windows.Forms.Padding(3);
            this.btnMult.Size = new System.Drawing.Size(60, 60);
            this.btnMult.TabIndex = 29;
            this.btnMult.Tag = "-4";
            this.btnMult.Text = "*";
            this.btnMult.UseVisualStyleBackColor = true;
            this.btnMult.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnMult.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnPlus
            // 
            this.btnPlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlus.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnPlus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlus.Location = new System.Drawing.Point(485, 330);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Padding = new System.Windows.Forms.Padding(3);
            this.btnPlus.Size = new System.Drawing.Size(60, 60);
            this.btnPlus.TabIndex = 27;
            this.btnPlus.Tag = "-2";
            this.btnPlus.Text = "+";
            this.btnPlus.UseVisualStyleBackColor = true;
            this.btnPlus.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnPlus.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnMinus
            // 
            this.btnMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinus.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnMinus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMinus.Location = new System.Drawing.Point(485, 427);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Padding = new System.Windows.Forms.Padding(3);
            this.btnMinus.Size = new System.Drawing.Size(60, 60);
            this.btnMinus.TabIndex = 26;
            this.btnMinus.Tag = "-3";
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnMinus.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnResult
            // 
            this.btnResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResult.AutoEllipsis = true;
            this.btnResult.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResult.Location = new System.Drawing.Point(485, 492);
            this.btnResult.Name = "btnResult";
            this.btnResult.Padding = new System.Windows.Forms.Padding(3);
            this.btnResult.Size = new System.Drawing.Size(56, 125);
            this.btnResult.TabIndex = 25;
            this.btnResult.Tag = "-1";
            this.btnResult.Text = "=";
            this.btnResult.UseVisualStyleBackColor = true;
            this.btnResult.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnResult.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnResult.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnOpAnd
            // 
            this.btnOpAnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpAnd.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnOpAnd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnOpAnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpAnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpAnd.Location = new System.Drawing.Point(757, 200);
            this.btnOpAnd.Name = "btnOpAnd";
            this.btnOpAnd.Padding = new System.Windows.Forms.Padding(3);
            this.btnOpAnd.Size = new System.Drawing.Size(120, 90);
            this.btnOpAnd.TabIndex = 33;
            this.btnOpAnd.Tag = "-6";
            this.btnOpAnd.Text = "AND";
            this.btnOpAnd.UseVisualStyleBackColor = true;
            this.btnOpAnd.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnOpAnd.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnOpOR
            // 
            this.btnOpOR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpOR.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnOpOR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnOpOR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpOR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpOR.Location = new System.Drawing.Point(751, 295);
            this.btnOpOR.Name = "btnOpOR";
            this.btnOpOR.Padding = new System.Windows.Forms.Padding(3);
            this.btnOpOR.Size = new System.Drawing.Size(120, 90);
            this.btnOpOR.TabIndex = 32;
            this.btnOpOR.Tag = "-7";
            this.btnOpOR.Text = "OR";
            this.btnOpOR.UseVisualStyleBackColor = true;
            this.btnOpOR.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnOpOR.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnOpXOR
            // 
            this.btnOpXOR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpXOR.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnOpXOR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnOpXOR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpXOR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpXOR.Location = new System.Drawing.Point(751, 427);
            this.btnOpXOR.Name = "btnOpXOR";
            this.btnOpXOR.Padding = new System.Windows.Forms.Padding(3);
            this.btnOpXOR.Size = new System.Drawing.Size(120, 90);
            this.btnOpXOR.TabIndex = 31;
            this.btnOpXOR.Tag = "-8";
            this.btnOpXOR.Text = "XOR";
            this.btnOpXOR.UseVisualStyleBackColor = true;
            this.btnOpXOR.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnOpXOR.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnOpNOT
            // 
            this.btnOpNOT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpNOT.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnOpNOT.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnOpNOT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpNOT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpNOT.Location = new System.Drawing.Point(751, 522);
            this.btnOpNOT.Name = "btnOpNOT";
            this.btnOpNOT.Padding = new System.Windows.Forms.Padding(3);
            this.btnOpNOT.Size = new System.Drawing.Size(120, 90);
            this.btnOpNOT.TabIndex = 30;
            this.btnOpNOT.Tag = "-9";
            this.btnOpNOT.Text = "NOT";
            this.btnOpNOT.UseVisualStyleBackColor = true;
            this.btnOpNOT.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnOpNOT.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnMemory
            // 
            this.btnMemory.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnMemory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnMemory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMemory.Location = new System.Drawing.Point(124, 8);
            this.btnMemory.Name = "btnMemory";
            this.btnMemory.Size = new System.Drawing.Size(56, 58);
            this.btnMemory.TabIndex = 36;
            this.btnMemory.Tag = "MR";
            this.btnMemory.Text = "MR";
            this.btnMemory.UseVisualStyleBackColor = true;
            this.btnMemory.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnMemory.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button12
            // 
            this.button12.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button12.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button12.Location = new System.Drawing.Point(124, 72);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(56, 58);
            this.button12.TabIndex = 37;
            this.button12.Tag = "MS";
            this.button12.Text = "MS";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.btnOperator_Click);
            this.button12.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = global::Calc64WF.Properties.Resources.Exit;
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(42, 522);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(3);
            this.btnClose.Size = new System.Drawing.Size(392, 142);
            this.btnClose.TabIndex = 39;
            this.btnClose.Tag = "+0";
            this.btnClose.Text = "Schliessen";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // pnlMaster
            // 
            this.pnlMaster.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pnlMaster.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnlMaster.Location = new System.Drawing.Point(11, 233);
            this.pnlMaster.Name = "pnlMaster";
            this.pnlMaster.Size = new System.Drawing.Size(66, 112);
            this.pnlMaster.TabIndex = 40;
            this.pnlMaster.SizeChanged += new System.EventHandler(this.pnlMaster_SizeChanged);
            this.pnlMaster.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnDivide
            // 
            this.btnDivide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDivide.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnDivide.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnDivide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDivide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDivide.Location = new System.Drawing.Point(485, 265);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Padding = new System.Windows.Forms.Padding(3);
            this.btnDivide.Size = new System.Drawing.Size(60, 60);
            this.btnDivide.TabIndex = 28;
            this.btnDivide.Tag = "-5";
            this.btnDivide.Text = "/";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnDivide.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // FrmCalc64Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(978, 694);
            this.Controls.Add(this.pnlMaster);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.btnMemory);
            this.Controls.Add(this.btnOpAnd);
            this.Controls.Add(this.btnOpOR);
            this.Controls.Add(this.btnOpXOR);
            this.Controls.Add(this.btnOpNOT);
            this.Controls.Add(this.btnMult);
            this.Controls.Add(this.btnDivide);
            this.Controls.Add(this.btnPlus);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.btnResult);
            this.Controls.Add(this.btnHexF);
            this.Controls.Add(this.btnHexE);
            this.Controls.Add(this.btnHexD);
            this.Controls.Add(this.btnHexC);
            this.Controls.Add(this.btnHexB);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblMemory);
            this.Controls.Add(this.lblOperation);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.btnNum0);
            this.Controls.Add(this.btnHexA);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btnNum5);
            this.Controls.Add(this.btnNum4);
            this.Controls.Add(this.btnNum3);
            this.Controls.Add(this.btnNum2);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnNum1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FrmCalc64Main";
            this.Text = "Calc64-Winform";
            this.Click += new System.EventHandler(this.btnOperator_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The BTN one
        /// </summary>
        private System.Windows.Forms.Button btnNum1;
        /// <summary>
        /// The label result
        /// </summary>
        private System.Windows.Forms.Label lblResult;
        /// <summary>
        /// The picture box1
        /// </summary>
        private System.Windows.Forms.PictureBox pictureBox1;
        /// <summary>
        /// The button1
        /// </summary>
        private System.Windows.Forms.Button btnNum2;
        /// <summary>
        /// The button2
        /// </summary>
        private System.Windows.Forms.Button btnNum3;
        /// <summary>
        /// The button3
        /// </summary>
        private System.Windows.Forms.Button btnNum4;
        /// <summary>
        /// The button4
        /// </summary>
        private System.Windows.Forms.Button btnNum5;
        /// <summary>
        /// The button5
        /// </summary>
        private System.Windows.Forms.Button button5;
        /// <summary>
        /// The button6
        /// </summary>
        private System.Windows.Forms.Button button6;
        /// <summary>
        /// The button7
        /// </summary>
        private System.Windows.Forms.Button button7;
        /// <summary>
        /// The button8
        /// </summary>
        private System.Windows.Forms.Button button8;
        /// <summary>
        /// The BTN hexadecimal a
        /// </summary>
        private System.Windows.Forms.Button btnHexA;
        /// <summary>
        /// The button10
        /// </summary>
        private System.Windows.Forms.Button btnNum0;
        /// <summary>
        /// The button11
        /// </summary>
        private System.Windows.Forms.Button button11;
        /// <summary>
        /// The label operation
        /// </summary>
        private System.Windows.Forms.Label lblOperation;
        /// <summary>
        /// The label memory
        /// </summary>
        private System.Windows.Forms.Label lblMemory;
        /// <summary>
        /// The BTN back
        /// </summary>
        private System.Windows.Forms.Button btnBack;
        /// <summary>
        /// The BTN clear
        /// </summary>
        private System.Windows.Forms.Button btnClear;
        /// <summary>
        /// The BTN clear all
        /// </summary>
        private System.Windows.Forms.Button btnClearAll;
        /// <summary>
        /// The BTN hexadecimal b
        /// </summary>
        private System.Windows.Forms.Button btnHexB;
        /// <summary>
        /// The BTN hexadecimal c
        /// </summary>
        private System.Windows.Forms.Button btnHexC;
        /// <summary>
        /// The BTN hexadecimal f
        /// </summary>
        private System.Windows.Forms.Button btnHexF;
        /// <summary>
        /// The BTN hexadecimal e
        /// </summary>
        private System.Windows.Forms.Button btnHexE;
        /// <summary>
        /// The BTN hexadecimal d
        /// </summary>
        private System.Windows.Forms.Button btnHexD;
        /// <summary>
        /// The BTN mult
        /// </summary>
        private System.Windows.Forms.Button btnMult;
        /// <summary>
        /// The BTN plus
        /// </summary>
        private System.Windows.Forms.Button btnPlus;
        /// <summary>
        /// The BTN minus
        /// </summary>
        private System.Windows.Forms.Button btnMinus;
        /// <summary>
        /// The BTN result
        /// </summary>
        private System.Windows.Forms.Button btnResult;
        /// <summary>
        /// The BTN op and
        /// </summary>
        private System.Windows.Forms.Button btnOpAnd;
        /// <summary>
        /// The BTN op or
        /// </summary>
        private System.Windows.Forms.Button btnOpOR;
        /// <summary>
        /// The BTN op xor
        /// </summary>
        private System.Windows.Forms.Button btnOpXOR;
        /// <summary>
        /// The BTN op not
        /// </summary>
        private System.Windows.Forms.Button btnOpNOT;
        /// <summary>
        /// The BTN memory
        /// </summary>
        private System.Windows.Forms.Button btnMemory;
        /// <summary>
        /// The button12
        /// </summary>
        private System.Windows.Forms.Button button12;
        /// <summary>
        /// The BTN close
        /// </summary>
        private System.Windows.Forms.Button btnClose;
        /// <summary>
        /// The PNL master
        /// </summary>
        private System.Windows.Forms.Panel pnlMaster;
        /// <summary>
        /// The BTN divide
        /// </summary>
        private System.Windows.Forms.Button btnDivide;
    }
}