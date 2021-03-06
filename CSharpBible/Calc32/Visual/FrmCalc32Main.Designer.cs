﻿using System;
using CSharpBible.Calc32.NonVisual;

namespace CSharpBible.Calc32.Visual
{
    partial class FrmCalc32Main
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
            this.btnOne = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.calculatorClass1 = new CSharpBible.Calc32.NonVisual.CalculatorClass();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.btnHexA = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
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
            this.btnDivide = new System.Windows.Forms.Button();
            this.btnPlus = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnResult = new System.Windows.Forms.Button();
            this.btnOpAnd = new System.Windows.Forms.Button();
            this.btnOpOR = new System.Windows.Forms.Button();
            this.btnOpXOR = new System.Windows.Forms.Button();
            this.btnOpNOT = new System.Windows.Forms.Button();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.button9 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOne
            // 
            this.btnOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOne.AutoEllipsis = true;
            this.btnOne.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnOne.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOne.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOne.Location = new System.Drawing.Point(363, 299);
            this.btnOne.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOne.Name = "btnOne";
            this.btnOne.Size = new System.Drawing.Size(37, 38);
            this.btnOne.TabIndex = 0;
            this.btnOne.Tag = "1";
            this.btnOne.Text = "1";
            this.btnOne.UseVisualStyleBackColor = true;
            this.btnOne.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnOne.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnOne.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // lblResult
            // 
            this.lblResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            this.lblResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult.Location = new System.Drawing.Point(124, 47);
            this.lblResult.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblResult.Size = new System.Drawing.Size(444, 37);
            this.lblResult.TabIndex = 1;
            this.lblResult.Text = "0";
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblResult.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // calculatorClass1
            // 
            this.calculatorClass1.Akkumulator = 0;
            this.calculatorClass1.Memory = 0;
            this.calculatorClass1.OnChange += new System.EventHandler(this.calculatorClassChange);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::CSharpBible.Calc32.Properties.Resources.Glow_White;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(627, 383);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(130, 135);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(405, 299);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(37, 38);
            this.button1.TabIndex = 4;
            this.button1.Tag = "2";
            this.button1.Text = "2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnNummber_Click);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(446, 299);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(37, 38);
            this.button2.TabIndex = 5;
            this.button2.Tag = "3";
            this.button2.Text = "3";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnNummber_Click);
            this.button2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(363, 257);
            this.button3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(37, 38);
            this.button3.TabIndex = 6;
            this.button3.Tag = "4";
            this.button3.Text = "4";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnNummber_Click);
            this.button3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(405, 257);
            this.button4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(37, 38);
            this.button4.TabIndex = 7;
            this.button4.Tag = "5";
            this.button4.Text = "5";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnNummber_Click);
            this.button4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(446, 257);
            this.button5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(37, 38);
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
            this.button6.Location = new System.Drawing.Point(363, 214);
            this.button6.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(37, 38);
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
            this.button7.Location = new System.Drawing.Point(405, 214);
            this.button7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(37, 38);
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
            this.button8.Location = new System.Drawing.Point(446, 214);
            this.button8.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(37, 38);
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
            this.btnHexA.Location = new System.Drawing.Point(363, 172);
            this.btnHexA.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHexA.Name = "btnHexA";
            this.btnHexA.Size = new System.Drawing.Size(37, 38);
            this.btnHexA.TabIndex = 12;
            this.btnHexA.Tag = "10";
            this.btnHexA.Text = "A";
            this.btnHexA.UseVisualStyleBackColor = true;
            this.btnHexA.Click += new System.EventHandler(this.btnNummber_Click);
            this.btnHexA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.btnHexA.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button10
            // 
            this.button10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button10.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button10.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button10.Location = new System.Drawing.Point(363, 341);
            this.button10.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(79, 38);
            this.button10.TabIndex = 13;
            this.button10.Tag = "0";
            this.button10.Text = "0";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.btnNummber_Click);
            this.button10.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.button10.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button11
            // 
            this.button11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button11.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button11.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button11.Location = new System.Drawing.Point(446, 341);
            this.button11.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(37, 38);
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
            this.lblOperation.Location = new System.Drawing.Point(497, 6);
            this.lblOperation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOperation.Name = "lblOperation";
            this.lblOperation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOperation.Size = new System.Drawing.Size(71, 37);
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
            this.lblMemory.Location = new System.Drawing.Point(124, 6);
            this.lblMemory.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMemory.Size = new System.Drawing.Size(369, 37);
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
            this.btnBack.Location = new System.Drawing.Point(83, 130);
            this.btnBack.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(79, 38);
            this.btnBack.TabIndex = 17;
            this.btnBack.Tag = "2";
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
            this.btnClear.Location = new System.Drawing.Point(239, 130);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(79, 38);
            this.btnClear.TabIndex = 18;
            this.btnClear.Tag = "1";
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
            this.btnClearAll.Location = new System.Drawing.Point(239, 172);
            this.btnClearAll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(79, 38);
            this.btnClearAll.TabIndex = 19;
            this.btnClearAll.Tag = "3";
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
            this.btnHexB.Location = new System.Drawing.Point(405, 172);
            this.btnHexB.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHexB.Name = "btnHexB";
            this.btnHexB.Size = new System.Drawing.Size(37, 38);
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
            this.btnHexC.Location = new System.Drawing.Point(446, 172);
            this.btnHexC.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHexC.Name = "btnHexC";
            this.btnHexC.Size = new System.Drawing.Size(37, 38);
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
            this.btnHexF.Location = new System.Drawing.Point(446, 130);
            this.btnHexF.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHexF.Name = "btnHexF";
            this.btnHexF.Size = new System.Drawing.Size(37, 38);
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
            this.btnHexE.Location = new System.Drawing.Point(405, 130);
            this.btnHexE.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHexE.Name = "btnHexE";
            this.btnHexE.Size = new System.Drawing.Size(37, 38);
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
            this.btnHexD.Location = new System.Drawing.Point(363, 130);
            this.btnHexD.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnHexD.Name = "btnHexD";
            this.btnHexD.Size = new System.Drawing.Size(37, 38);
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
            this.btnMult.Location = new System.Drawing.Point(322, 130);
            this.btnMult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnMult.Name = "btnMult";
            this.btnMult.Size = new System.Drawing.Size(37, 38);
            this.btnMult.TabIndex = 29;
            this.btnMult.Tag = "-4";
            this.btnMult.Text = "*";
            this.btnMult.UseVisualStyleBackColor = true;
            this.btnMult.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnMult.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnDivide
            // 
            this.btnDivide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDivide.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnDivide.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnDivide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDivide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDivide.Location = new System.Drawing.Point(322, 172);
            this.btnDivide.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(37, 38);
            this.btnDivide.TabIndex = 28;
            this.btnDivide.Tag = "-5";
            this.btnDivide.Text = "/";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnDivide.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnPlus
            // 
            this.btnPlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlus.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnPlus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlus.Location = new System.Drawing.Point(322, 214);
            this.btnPlus.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(37, 38);
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
            this.btnMinus.Location = new System.Drawing.Point(322, 257);
            this.btnMinus.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(37, 38);
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
            this.btnResult.Location = new System.Drawing.Point(322, 299);
            this.btnResult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnResult.Name = "btnResult";
            this.btnResult.Size = new System.Drawing.Size(37, 81);
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
            this.btnOpAnd.Location = new System.Drawing.Point(487, 130);
            this.btnOpAnd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOpAnd.Name = "btnOpAnd";
            this.btnOpAnd.Size = new System.Drawing.Size(80, 59);
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
            this.btnOpOR.Location = new System.Drawing.Point(487, 194);
            this.btnOpOR.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOpOR.Name = "btnOpOR";
            this.btnOpOR.Size = new System.Drawing.Size(80, 59);
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
            this.btnOpXOR.Location = new System.Drawing.Point(487, 257);
            this.btnOpXOR.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOpXOR.Name = "btnOpXOR";
            this.btnOpXOR.Size = new System.Drawing.Size(80, 59);
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
            this.btnOpNOT.Location = new System.Drawing.Point(487, 321);
            this.btnOpNOT.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOpNOT.Name = "btnOpNOT";
            this.btnOpNOT.Size = new System.Drawing.Size(80, 59);
            this.btnOpNOT.TabIndex = 30;
            this.btnOpNOT.Tag = "-9";
            this.btnOpNOT.Text = "NOT";
            this.btnOpNOT.UseVisualStyleBackColor = true;
            this.btnOpNOT.Click += new System.EventHandler(this.btnOperator_Click);
            this.btnOpNOT.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.panel2);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(79, 451);
            this.pnlLeft.TabIndex = 34;
            this.pnlLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(79, 172);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(156, 147);
            this.panel2.TabIndex = 41;
            // 
            // pnlRight
            // 
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(572, 0);
            this.pnlRight.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(79, 451);
            this.pnlRight.TabIndex = 35;
            this.pnlRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button9
            // 
            this.button9.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button9.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button9.Location = new System.Drawing.Point(83, 5);
            this.button9.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(37, 38);
            this.button9.TabIndex = 36;
            this.button9.Tag = "-4";
            this.button9.Text = "*";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.btnOperator_Click);
            this.button9.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // button12
            // 
            this.button12.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.button12.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button12.Location = new System.Drawing.Point(83, 47);
            this.button12.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(37, 38);
            this.button12.TabIndex = 37;
            this.button12.Tag = "-4";
            this.button12.Text = "*";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.btnOperator_Click);
            this.button12.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(79, 386);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(493, 65);
            this.pnlBottom.TabIndex = 38;
            this.pnlBottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.BackgroundImage = global::CSharpBible.Calc32.Properties.Resources.Exit;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Lavender;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(83, 323);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(138, 59);
            this.btnClose.TabIndex = 39;
            this.btnClose.Tag = "0";
            this.btnClose.Text = "Schliessen";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(225, 215);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(93, 177);
            this.panel1.TabIndex = 40;
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Location = new System.Drawing.Point(79, 172);
            this.panel3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(157, 146);
            this.panel3.TabIndex = 41;
            this.panel3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Location = new System.Drawing.Point(78, 89);
            this.panel4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(495, 36);
            this.panel4.TabIndex = 45;
            this.panel4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.Location = new System.Drawing.Point(165, 125);
            this.panel5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(70, 51);
            this.panel5.TabIndex = 46;
            this.panel5.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            // 
            // FrmCalc32Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(651, 451);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
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
            this.Controls.Add(this.button10);
            this.Controls.Add(this.btnHexA);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnOne);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FrmCalc32Main";
            this.Text = "FrmCalc32Main";
            this.Click += new System.EventHandler(this.btnOperator_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCalc32Main_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmCalc32Main_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button btnOne;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.PictureBox pictureBox1;
        private NonVisual.CalculatorClass calculatorClass1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button btnHexA;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnHexB;
        private System.Windows.Forms.Button btnHexC;
        private System.Windows.Forms.Button btnHexF;
        private System.Windows.Forms.Button btnHexE;
        private System.Windows.Forms.Button btnHexD;
        private System.Windows.Forms.Button btnMult;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnResult;
        private System.Windows.Forms.Button btnOpAnd;
        private System.Windows.Forms.Button btnOpOR;
        private System.Windows.Forms.Button btnOpXOR;
        private System.Windows.Forms.Button btnOpNOT;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
    }
}