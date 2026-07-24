using BaseLib.Helper;
using GenFreeWin.Main;
using GenFreeWin.ViewModels;
using GenFreeWin.Views;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.ViewModels.Interfaces;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;
namespace GenFreeWin;
public partial class Regsuch 
{
    private IContainer components;


    [DebuggerNonUserCode]
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(Disposing);
    }

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._Option1_7 = new System.Windows.Forms.RadioButton();
            this._Option1_6 = new System.Windows.Forms.RadioButton();
            this._Option1_5 = new System.Windows.Forms.RadioButton();
            this._Option1_4 = new System.Windows.Forms.RadioButton();
            this._Option1_3 = new System.Windows.Forms.RadioButton();
            this._Command1_3 = new System.Windows.Forms.Button();
            this.Combo1 = new System.Windows.Forms.ComboBox();
            this.List3 = new System.Windows.Forms.ListBox();
            this.Text1 = new System.Windows.Forms.TextBox();
            this._Option1_2 = new System.Windows.Forms.RadioButton();
            this._Option1_1 = new System.Windows.Forms.RadioButton();
            this._Option1_0 = new System.Windows.Forms.RadioButton();
            this._Command1_0 = new System.Windows.Forms.Button();
            this._Check2_3 = new System.Windows.Forms.CheckBox();
            this._Command1_7 = new System.Windows.Forms.Button();
            this._Option1_9 = new System.Windows.Forms.RadioButton();
            this._Option1_8 = new System.Windows.Forms.RadioButton();
            this._Option1_10 = new System.Windows.Forms.RadioButton();
            this._Option1_11 = new System.Windows.Forms.RadioButton();
            this._Option1_12 = new System.Windows.Forms.RadioButton();
            this._Option1_13 = new System.Windows.Forms.RadioButton();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this._Label1_1 = new System.Windows.Forms.Label();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.Label76 = new System.Windows.Forms.Label();
            this.Label75 = new System.Windows.Forms.Label();
            this.Label74 = new System.Windows.Forms.Label();
            this.lblChld01Info = new System.Windows.Forms.Label();
            this.Label72 = new System.Windows.Forms.Label();
            this.Label71 = new System.Windows.Forms.Label();
            this.Label70 = new System.Windows.Forms.Label();
            this.Label69 = new System.Windows.Forms.Label();
            this.lblChild01 = new System.Windows.Forms.Label();
            this.lblChild04 = new System.Windows.Forms.Label();
            this.lblChld02Info = new System.Windows.Forms.Label();
            this.lblChild02 = new System.Windows.Forms.Label();
            this.lblChld03Info = new System.Windows.Forms.Label();
            this.lblChild03 = new System.Windows.Forms.Label();
            this.Label62 = new System.Windows.Forms.Label();
            this.Label61 = new System.Windows.Forms.Label();
            this.Label60 = new System.Windows.Forms.Label();
            this.Label59 = new System.Windows.Forms.Label();
            this.Label58 = new System.Windows.Forms.Label();
            this.Label57 = new System.Windows.Forms.Label();
            this.Label56 = new System.Windows.Forms.Label();
            this.lblChld05Info = new System.Windows.Forms.Label();
            this.lblChild05 = new System.Windows.Forms.Label();
            this.lblChld04Info = new System.Windows.Forms.Label();
            this.Label52 = new System.Windows.Forms.Label();
            this.Label50 = new System.Windows.Forms.Label();
            this.Label49 = new System.Windows.Forms.Label();
            this.Label48 = new System.Windows.Forms.Label();
            this.Label47 = new System.Windows.Forms.Label();
            this.Label46 = new System.Windows.Forms.Label();
            this.Label45 = new System.Windows.Forms.Label();
            this.Label43 = new System.Windows.Forms.Label();
            this.Label33 = new System.Windows.Forms.Label();
            this.Label32 = new System.Windows.Forms.Label();
            this.Label31 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.Label29 = new System.Windows.Forms.Label();
            this.Label28 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _Option1_7
            // 
            this._Option1_7.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_7.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_7.Location = new System.Drawing.Point(112, 35);
            this._Option1_7.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_7.Name = "_Option1_7";
            this._Option1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_7.Size = new System.Drawing.Size(177, 27);
            this._Option1_7.TabIndex = 25;
            this._Option1_7.TabStop = true;
            this._Option1_7.Text = "Verlobungsregister";
            this._Option1_7.UseVisualStyleBackColor = false;
            // 
            // _Option1_6
            // 
            this._Option1_6.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_6.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_6.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_6.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_6.Location = new System.Drawing.Point(112, 5);
            this._Option1_6.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_6.Name = "_Option1_6";
            this._Option1_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_6.Size = new System.Drawing.Size(177, 26);
            this._Option1_6.TabIndex = 24;
            this._Option1_6.TabStop = true;
            this._Option1_6.Text = "Proklamationsregister";
            this._Option1_6.UseVisualStyleBackColor = false;
            // 
            // _Option1_5
            // 
            this._Option1_5.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_5.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_5.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_5.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_5.Location = new System.Drawing.Point(550, 5);
            this._Option1_5.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_5.Name = "_Option1_5";
            this._Option1_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_5.Size = new System.Drawing.Size(181, 23);
            this._Option1_5.TabIndex = 21;
            this._Option1_5.TabStop = true;
            this._Option1_5.Text = "Geburtsregister";
            this._Option1_5.UseVisualStyleBackColor = false;
            // 
            // _Option1_4
            // 
            this._Option1_4.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_4.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_4.Location = new System.Drawing.Point(550, 29);
            this._Option1_4.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_4.Name = "_Option1_4";
            this._Option1_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_4.Size = new System.Drawing.Size(181, 23);
            this._Option1_4.TabIndex = 20;
            this._Option1_4.TabStop = true;
            this._Option1_4.Text = "Taufregister";
            this._Option1_4.UseVisualStyleBackColor = false;
            // 
            // _Option1_3
            // 
            this._Option1_3.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_3.Location = new System.Drawing.Point(550, 53);
            this._Option1_3.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_3.Name = "_Option1_3";
            this._Option1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_3.Size = new System.Drawing.Size(181, 23);
            this._Option1_3.TabIndex = 19;
            this._Option1_3.TabStop = true;
            this._Option1_3.Text = "Sterberegister";
            this._Option1_3.UseVisualStyleBackColor = false;
            // 
            // _Command1_3
            // 
            this._Command1_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this._Command1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_3.Location = new System.Drawing.Point(762, 74);
            this._Command1_3.Margin = new System.Windows.Forms.Padding(5);
            this._Command1_3.Name = "_Command1_3";
            this._Command1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_3.Size = new System.Drawing.Size(125, 26);
            this._Command1_3.TabIndex = 16;
            this._Command1_3.Text = "S&uche starten";
            this._Command1_3.UseVisualStyleBackColor = false;
            // 
            // Combo1
            // 
            this.Combo1.BackColor = System.Drawing.SystemColors.Window;
            this.Combo1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Combo1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Combo1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Combo1.Location = new System.Drawing.Point(-1, 121);
            this.Combo1.Margin = new System.Windows.Forms.Padding(5);
            this.Combo1.Name = "Combo1";
            this.Combo1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Combo1.Size = new System.Drawing.Size(349, 31);
            this.Combo1.TabIndex = 0;
            this.Combo1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Combo1_KeyUp);
            // 
            // List3
            // 
            this.List3.BackColor = System.Drawing.SystemColors.Window;
            this.List3.Cursor = System.Windows.Forms.Cursors.Default;
            this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List3.ItemHeight = 19;
            this.List3.Location = new System.Drawing.Point(654, 158);
            this.List3.Margin = new System.Windows.Forms.Padding(5);
            this.List3.Name = "List3";
            this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List3.Size = new System.Drawing.Size(213, 4);
            this.List3.Sorted = true;
            this.List3.TabIndex = 11;
            this.List3.Visible = false;
            // 
            // Text1
            // 
            this.Text1.AcceptsReturn = true;
            this.Text1.BackColor = System.Drawing.SystemColors.Window;
            this.Text1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text1.Location = new System.Drawing.Point(24, 122);
            this.Text1.Margin = new System.Windows.Forms.Padding(5);
            this.Text1.MaxLength = 0;
            this.Text1.Name = "Text1";
            this.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text1.Size = new System.Drawing.Size(283, 27);
            this.Text1.TabIndex = 1;
            this.Text1.Visible = false;
            // 
            // _Option1_2
            // 
            this._Option1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_2.Location = new System.Drawing.Point(550, 77);
            this._Option1_2.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_2.Name = "_Option1_2";
            this._Option1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_2.Size = new System.Drawing.Size(181, 23);
            this._Option1_2.TabIndex = 5;
            this._Option1_2.TabStop = true;
            this._Option1_2.Text = "Begräbnisregister";
            this._Option1_2.UseVisualStyleBackColor = false;
            // 
            // _Option1_1
            // 
            this._Option1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_1.Location = new System.Drawing.Point(550, 125);
            this._Option1_1.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_1.Name = "_Option1_1";
            this._Option1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_1.Size = new System.Drawing.Size(181, 23);
            this._Option1_1.TabIndex = 4;
            this._Option1_1.TabStop = true;
            this._Option1_1.Text = "Alle Personenregister";
            this._Option1_1.UseVisualStyleBackColor = false;
            // 
            // _Option1_0
            // 
            this._Option1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_0.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_0.Location = new System.Drawing.Point(550, 101);
            this._Option1_0.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_0.Name = "_Option1_0";
            this._Option1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_0.Size = new System.Drawing.Size(181, 23);
            this._Option1_0.TabIndex = 3;
            this._Option1_0.TabStop = true;
            this._Option1_0.Tag = GenFree.Data.EEventArt.eA_105;
            this._Option1_0.Text = "sonst. Register";
            this._Option1_0.UseVisualStyleBackColor = false;
            // 
            // _Command1_0
            // 
            this._Command1_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_0.Location = new System.Drawing.Point(762, 38);
            this._Command1_0.Margin = new System.Windows.Forms.Padding(5);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_0.Size = new System.Drawing.Size(125, 26);
            this._Command1_0.TabIndex = 33;
            this._Command1_0.Text = "&Schließen";
            this._Command1_0.UseVisualStyleBackColor = false;
            // 
            // _Check2_3
            // 
            this._Check2_3.BackColor = System.Drawing.SystemColors.Control;
            this._Check2_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Check2_3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Check2_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Check2_3.Location = new System.Drawing.Point(752, 126);
            this._Check2_3.Margin = new System.Windows.Forms.Padding(5);
            this._Check2_3.Name = "_Check2_3";
            this._Check2_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Check2_3.Size = new System.Drawing.Size(207, 22);
            this._Check2_3.TabIndex = 34;
            this._Check2_3.Text = "Auswahl beibehalten";
            this._Check2_3.UseVisualStyleBackColor = false;
            // 
            // _Command1_7
            // 
            this._Command1_7.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_7.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_7.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Command1_7.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_7.Location = new System.Drawing.Point(221, 83);
            this._Command1_7.Margin = new System.Windows.Forms.Padding(5);
            this._Command1_7.Name = "_Command1_7";
            this._Command1_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_7.Size = new System.Drawing.Size(128, 35);
            this._Command1_7.TabIndex = 83;
            this._Command1_7.Text = "Liste drucken";
            this._Command1_7.UseVisualStyleBackColor = false;
            // 
            // _Option1_9
            // 
            this._Option1_9.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_9.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_9.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_9.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_9.Location = new System.Drawing.Point(382, 57);
            this._Option1_9.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_9.Name = "_Option1_9";
            this._Option1_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_9.Size = new System.Drawing.Size(177, 23);
            this._Option1_9.TabIndex = 85;
            this._Option1_9.TabStop = true;
            this._Option1_9.Text = "Alle Familienregister";
            this._Option1_9.UseVisualStyleBackColor = false;
            // 
            // _Option1_8
            // 
            this._Option1_8.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_8.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_8.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_8.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_8.Location = new System.Drawing.Point(382, 33);
            this._Option1_8.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_8.Name = "_Option1_8";
            this._Option1_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_8.Size = new System.Drawing.Size(177, 23);
            this._Option1_8.TabIndex = 84;
            this._Option1_8.TabStop = true;
            this._Option1_8.Text = "sonst.Register";
            this._Option1_8.UseVisualStyleBackColor = false;
            // 
            // _Option1_10
            // 
            this._Option1_10.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_10.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_10.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_10.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_10.Location = new System.Drawing.Point(382, -63);
            this._Option1_10.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_10.Name = "_Option1_10";
            this._Option1_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_10.Size = new System.Drawing.Size(177, 23);
            this._Option1_10.TabIndex = 86;
            this._Option1_10.TabStop = true;
            this._Option1_10.Text = "Heiratsregister";
            this._Option1_10.UseVisualStyleBackColor = false;
            // 
            // _Option1_11
            // 
            this._Option1_11.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_11.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_11.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_11.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_11.Location = new System.Drawing.Point(382, -39);
            this._Option1_11.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_11.Name = "_Option1_11";
            this._Option1_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_11.Size = new System.Drawing.Size(177, 23);
            this._Option1_11.TabIndex = 87;
            this._Option1_11.TabStop = true;
            this._Option1_11.Text = "kirchl. Heiratsregister";
            this._Option1_11.UseVisualStyleBackColor = false;
            // 
            // _Option1_12
            // 
            this._Option1_12.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_12.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_12.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_12.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_12.Location = new System.Drawing.Point(382, -15);
            this._Option1_12.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_12.Name = "_Option1_12";
            this._Option1_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_12.Size = new System.Drawing.Size(177, 23);
            this._Option1_12.TabIndex = 88;
            this._Option1_12.TabStop = true;
            this._Option1_12.Text = "Scheidungsregister";
            this._Option1_12.UseVisualStyleBackColor = false;
            // 
            // _Option1_13
            // 
            this._Option1_13.BackColor = System.Drawing.SystemColors.Control;
            this._Option1_13.Cursor = System.Windows.Forms.Cursors.Default;
            this._Option1_13.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Option1_13.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Option1_13.Location = new System.Drawing.Point(382, 9);
            this._Option1_13.Margin = new System.Windows.Forms.Padding(5);
            this._Option1_13.Name = "_Option1_13";
            this._Option1_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Option1_13.Size = new System.Drawing.Size(177, 23);
            this._Option1_13.TabIndex = 89;
            this._Option1_13.TabStop = true;
            this._Option1_13.Text = "Dim.-register";
            this._Option1_13.UseVisualStyleBackColor = false;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.SystemColors.Control;
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.Location = new System.Drawing.Point(33, 92);
            this.Label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label3.Size = new System.Drawing.Size(977, 18);
            this.Label3.TabIndex = 22;
            this.Label3.Text = "Name,Vorname";
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.Red;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.Color.White;
            this.Label2.Location = new System.Drawing.Point(741, 6);
            this.Label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(253, 26);
            this.Label2.TabIndex = 18;
            this.Label2.Text = "Suche nach Registernummern";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _Label1_1
            // 
            this._Label1_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_1.Location = new System.Drawing.Point(112, 97);
            this._Label1_1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_1.Size = new System.Drawing.Size(69, 22);
            this._Label1_1.TabIndex = 7;
            // 
            // lstUsageList
            // 
            this.ListBox1.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 20;
            this.ListBox1.Location = new System.Drawing.Point(24, 126);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(997, 244);
            this.ListBox1.TabIndex = 98;
            this.ListBox1.Click += new System.EventHandler(this.ListBox1_Click);
            this.ListBox1.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            // 
            // Label76
            // 
            this.Label76.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label76.Location = new System.Drawing.Point(265, 29);
            this.Label76.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label76.Name = "Label76";
            this.Label76.Size = new System.Drawing.Size(15, 1);
            this.Label76.TabIndex = 114;
            // 
            // Label75
            // 
            this.Label75.AutoEllipsis = true;
            this.Label75.BackColor = System.Drawing.Color.White;
            this.Label75.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label75.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label75.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label75.Location = new System.Drawing.Point(5, 18);
            this.Label75.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label75.Name = "Label75";
            this.Label75.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label75.Size = new System.Drawing.Size(260, 22);
            this.Label75.TabIndex = 39;
            // 
            // Label74
            // 
            this.Label74.AutoEllipsis = true;
            this.Label74.BackColor = System.Drawing.Color.White;
            this.Label74.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label74.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label74.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label74.Location = new System.Drawing.Point(5, 68);
            this.Label74.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label74.Name = "Label74";
            this.Label74.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label74.Size = new System.Drawing.Size(260, 22);
            this.Label74.TabIndex = 40;
            // 
            // lblChld01Info
            // 
            this.lblChld01Info.AutoEllipsis = true;
            this.lblChld01Info.BackColor = System.Drawing.Color.White;
            this.lblChld01Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChld01Info.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChld01Info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChld01Info.Location = new System.Drawing.Point(778, 40);
            this.lblChld01Info.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChld01Info.Name = "lblChld01Info";
            this.lblChld01Info.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChld01Info.Size = new System.Drawing.Size(320, 19);
            this.lblChld01Info.TabIndex = 41;
            // 
            // Label72
            // 
            this.Label72.AutoEllipsis = true;
            this.Label72.BackColor = System.Drawing.Color.White;
            this.Label72.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label72.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label72.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label72.Location = new System.Drawing.Point(5, 126);
            this.Label72.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label72.Name = "Label72";
            this.Label72.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label72.Size = new System.Drawing.Size(260, 22);
            this.Label72.TabIndex = 42;
            // 
            // Label71
            // 
            this.Label71.AutoEllipsis = true;
            this.Label71.BackColor = System.Drawing.Color.White;
            this.Label71.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label71.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label71.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label71.Location = new System.Drawing.Point(5, 182);
            this.Label71.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label71.Name = "Label71";
            this.Label71.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label71.Size = new System.Drawing.Size(260, 22);
            this.Label71.TabIndex = 43;
            // 
            // Label70
            // 
            this.Label70.AutoEllipsis = true;
            this.Label70.BackColor = System.Drawing.Color.White;
            this.Label70.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label70.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label70.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label70.Location = new System.Drawing.Point(312, 40);
            this.Label70.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label70.Name = "Label70";
            this.Label70.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label70.Size = new System.Drawing.Size(337, 22);
            this.Label70.TabIndex = 44;
            // 
            // Label69
            // 
            this.Label69.AutoEllipsis = true;
            this.Label69.BackColor = System.Drawing.Color.White;
            this.Label69.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label69.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label69.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label69.Location = new System.Drawing.Point(312, 152);
            this.Label69.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label69.Name = "Label69";
            this.Label69.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label69.Size = new System.Drawing.Size(337, 22);
            this.Label69.TabIndex = 45;
            // 
            // lblChild01
            // 
            this.lblChild01.AutoEllipsis = true;
            this.lblChild01.BackColor = System.Drawing.Color.White;
            this.lblChild01.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChild01.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChild01.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChild01.Location = new System.Drawing.Point(750, 18);
            this.lblChild01.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChild01.Name = "lblChild01";
            this.lblChild01.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChild01.Size = new System.Drawing.Size(337, 19);
            this.lblChild01.TabIndex = 46;
            // 
            // lblChild04
            // 
            this.lblChild04.AutoEllipsis = true;
            this.lblChild04.BackColor = System.Drawing.Color.White;
            this.lblChild04.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChild04.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChild04.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChild04.Location = new System.Drawing.Point(750, 152);
            this.lblChild04.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChild04.Name = "lblChild04";
            this.lblChild04.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChild04.Size = new System.Drawing.Size(337, 19);
            this.lblChild04.TabIndex = 47;
            // 
            // lblChld02Info
            // 
            this.lblChld02Info.AutoEllipsis = true;
            this.lblChld02Info.BackColor = System.Drawing.Color.White;
            this.lblChld02Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChld02Info.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChld02Info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChld02Info.Location = new System.Drawing.Point(778, 84);
            this.lblChld02Info.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChld02Info.Name = "lblChld02Info";
            this.lblChld02Info.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChld02Info.Size = new System.Drawing.Size(326, 19);
            this.lblChld02Info.TabIndex = 48;
            // 
            // lblChild02
            // 
            this.lblChild02.AutoEllipsis = true;
            this.lblChild02.BackColor = System.Drawing.Color.White;
            this.lblChild02.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChild02.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChild02.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChild02.Location = new System.Drawing.Point(750, 62);
            this.lblChild02.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChild02.Name = "lblChild02";
            this.lblChild02.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChild02.Size = new System.Drawing.Size(337, 19);
            this.lblChild02.TabIndex = 49;
            // 
            // lblChld03Info
            // 
            this.lblChld03Info.AutoEllipsis = true;
            this.lblChld03Info.BackColor = System.Drawing.Color.White;
            this.lblChld03Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChld03Info.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChld03Info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChld03Info.Location = new System.Drawing.Point(778, 127);
            this.lblChld03Info.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChld03Info.Name = "lblChld03Info";
            this.lblChld03Info.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChld03Info.Size = new System.Drawing.Size(320, 19);
            this.lblChld03Info.TabIndex = 50;
            // 
            // lblChild03
            // 
            this.lblChild03.AutoEllipsis = true;
            this.lblChild03.BackColor = System.Drawing.Color.White;
            this.lblChild03.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChild03.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChild03.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChild03.Location = new System.Drawing.Point(750, 107);
            this.lblChild03.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChild03.Name = "lblChild03";
            this.lblChild03.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChild03.Size = new System.Drawing.Size(337, 19);
            this.lblChild03.TabIndex = 51;
            // 
            // Label62
            // 
            this.Label62.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label62.Location = new System.Drawing.Point(265, 80);
            this.Label62.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label62.Name = "Label62";
            this.Label62.Size = new System.Drawing.Size(15, 1);
            this.Label62.TabIndex = 113;
            // 
            // Label61
            // 
            this.Label61.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label61.Location = new System.Drawing.Point(279, 29);
            this.Label61.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label61.Name = "Label61";
            this.Label61.Size = new System.Drawing.Size(1, 49);
            this.Label61.TabIndex = 112;
            // 
            // Label60
            // 
            this.Label60.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label60.Location = new System.Drawing.Point(265, 192);
            this.Label60.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label60.Name = "Label60";
            this.Label60.Size = new System.Drawing.Size(15, 1);
            this.Label60.TabIndex = 111;
            // 
            // Label59
            // 
            this.Label59.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label59.Location = new System.Drawing.Point(265, 135);
            this.Label59.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label59.Name = "Label59";
            this.Label59.Size = new System.Drawing.Size(15, 1);
            this.Label59.TabIndex = 110;
            // 
            // Label58
            // 
            this.Label58.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label58.Location = new System.Drawing.Point(300, 53);
            this.Label58.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label58.Name = "Label58";
            this.Label58.Size = new System.Drawing.Size(15, 1);
            this.Label58.TabIndex = 109;
            // 
            // Label57
            // 
            this.Label57.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label57.Location = new System.Drawing.Point(279, 137);
            this.Label57.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label57.Name = "Label57";
            this.Label57.Size = new System.Drawing.Size(1, 57);
            this.Label57.TabIndex = 108;
            // 
            // Label56
            // 
            this.Label56.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label56.Location = new System.Drawing.Point(300, 162);
            this.Label56.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label56.Name = "Label56";
            this.Label56.Size = new System.Drawing.Size(15, 1);
            this.Label56.TabIndex = 107;
            // 
            // lblChld05Info
            // 
            this.lblChld05Info.AutoEllipsis = true;
            this.lblChld05Info.BackColor = System.Drawing.Color.White;
            this.lblChld05Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChld05Info.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChld05Info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChld05Info.Location = new System.Drawing.Point(778, 217);
            this.lblChld05Info.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChld05Info.Name = "lblChld05Info";
            this.lblChld05Info.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChld05Info.Size = new System.Drawing.Size(320, 19);
            this.lblChld05Info.TabIndex = 52;
            // 
            // lblChild05
            // 
            this.lblChild05.AutoEllipsis = true;
            this.lblChild05.BackColor = System.Drawing.Color.White;
            this.lblChild05.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChild05.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChild05.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChild05.Location = new System.Drawing.Point(750, 194);
            this.lblChild05.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChild05.Name = "lblChild05";
            this.lblChild05.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChild05.Size = new System.Drawing.Size(337, 19);
            this.lblChild05.TabIndex = 53;
            // 
            // lblChld04Info
            // 
            this.lblChld04Info.AutoEllipsis = true;
            this.lblChld04Info.BackColor = System.Drawing.Color.White;
            this.lblChld04Info.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblChld04Info.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChld04Info.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChld04Info.Location = new System.Drawing.Point(778, 173);
            this.lblChld04Info.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblChld04Info.Name = "lblChld04Info";
            this.lblChld04Info.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblChld04Info.Size = new System.Drawing.Size(320, 19);
            this.lblChld04Info.TabIndex = 54;
            // 
            // Label52
            // 
            this.Label52.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label52.Location = new System.Drawing.Point(719, 26);
            this.Label52.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label52.Name = "Label52";
            this.Label52.Size = new System.Drawing.Size(1, 181);
            this.Label52.TabIndex = 106;
            // 
            // Label50
            // 
            this.Label50.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label50.Location = new System.Drawing.Point(720, 73);
            this.Label50.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label50.Name = "Label50";
            this.Label50.Size = new System.Drawing.Size(27, 1);
            this.Label50.TabIndex = 104;
            // 
            // Label49
            // 
            this.Label49.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label49.Location = new System.Drawing.Point(720, 28);
            this.Label49.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label49.Name = "Label49";
            this.Label49.Size = new System.Drawing.Size(24, 1);
            this.Label49.TabIndex = 103;
            // 
            // Label48
            // 
            this.Label48.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label48.Location = new System.Drawing.Point(720, 203);
            this.Label48.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label48.Name = "Label48";
            this.Label48.Size = new System.Drawing.Size(32, 1);
            this.Label48.TabIndex = 102;
            // 
            // Label47
            // 
            this.Label47.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label47.Location = new System.Drawing.Point(655, 162);
            this.Label47.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label47.Name = "Label47";
            this.Label47.Size = new System.Drawing.Size(9, 1);
            this.Label47.TabIndex = 101;
            // 
            // Label46
            // 
            this.Label46.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label46.Location = new System.Drawing.Point(655, 50);
            this.Label46.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label46.Name = "Label46";
            this.Label46.Size = new System.Drawing.Size(9, 1);
            this.Label46.TabIndex = 100;
            // 
            // Label45
            // 
            this.Label45.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label45.Location = new System.Drawing.Point(720, 162);
            this.Label45.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label45.Name = "Label45";
            this.Label45.Size = new System.Drawing.Size(32, 1);
            this.Label45.TabIndex = 99;
            // 
            // Label43
            // 
            this.Label43.BackColor = System.Drawing.SystemColors.Control;
            this.Label43.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label43.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label43.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label43.Location = new System.Drawing.Point(750, 84);
            this.Label43.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label43.Name = "Label43";
            this.Label43.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label43.Size = new System.Drawing.Size(33, 16);
            this.Label43.TabIndex = 55;
            this.Label43.Text = "oo";
            // 
            // Label33
            // 
            this.Label33.BackColor = System.Drawing.SystemColors.Control;
            this.Label33.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label33.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label33.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label33.Location = new System.Drawing.Point(750, 39);
            this.Label33.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label33.Name = "Label33";
            this.Label33.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label33.Size = new System.Drawing.Size(33, 16);
            this.Label33.TabIndex = 57;
            this.Label33.Text = "oo";
            // 
            // Label32
            // 
            this.Label32.BackColor = System.Drawing.SystemColors.Control;
            this.Label32.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label32.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label32.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label32.Location = new System.Drawing.Point(750, 128);
            this.Label32.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label32.Name = "Label32";
            this.Label32.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label32.Size = new System.Drawing.Size(33, 16);
            this.Label32.TabIndex = 56;
            this.Label32.Text = "oo";
            // 
            // Label31
            // 
            this.Label31.BackColor = System.Drawing.SystemColors.Control;
            this.Label31.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label31.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label31.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label31.Location = new System.Drawing.Point(750, 171);
            this.Label31.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label31.Name = "Label31";
            this.Label31.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label31.Size = new System.Drawing.Size(33, 16);
            this.Label31.TabIndex = 58;
            this.Label31.Text = "oo";
            // 
            // Label30
            // 
            this.Label30.BackColor = System.Drawing.SystemColors.Control;
            this.Label30.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label30.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label30.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label30.Location = new System.Drawing.Point(750, 219);
            this.Label30.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label30.Name = "Label30";
            this.Label30.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label30.Size = new System.Drawing.Size(33, 16);
            this.Label30.TabIndex = 59;
            this.Label30.Text = "oo";
            // 
            // Label29
            // 
            this.Label29.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label29.Location = new System.Drawing.Point(665, 50);
            this.Label29.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(1, 111);
            this.Label29.TabIndex = 88;
            // 
            // Label28
            // 
            this.Label28.BackColor = System.Drawing.SystemColors.WindowText;
            this.Label28.Location = new System.Drawing.Point(701, 116);
            this.Label28.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(43, 1);
            this.Label28.TabIndex = 87;
            // 
            // Label11
            // 
            this.Label11.BackColor = System.Drawing.SystemColors.Control;
            this.Label11.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label11.Location = new System.Drawing.Point(676, 107);
            this.Label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label11.Name = "Label11";
            this.Label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label11.Size = new System.Drawing.Size(33, 29);
            this.Label11.TabIndex = 84;
            this.Label11.Text = "oo";
            // 
            // Label10
            // 
            this.Label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Label10.BackColor = System.Drawing.SystemColors.Control;
            this.Label10.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label10.Location = new System.Drawing.Point(261, 153);
            this.Label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label10.Name = "Label10";
            this.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label10.Size = new System.Drawing.Size(29, 22);
            this.Label10.TabIndex = 85;
            this.Label10.Text = "oo";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Label9
            // 
            this.Label9.BackColor = System.Drawing.SystemColors.Control;
            this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label9.Location = new System.Drawing.Point(265, 47);
            this.Label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Label9.Name = "Label9";
            this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label9.Size = new System.Drawing.Size(33, 16);
            this.Label9.TabIndex = 86;
            this.Label9.Text = "oo";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // GroupBoxUsage
            // 
            this.GroupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.GroupBox1.Controls.Add(this.Label9);
            this.GroupBox1.Controls.Add(this.Label10);
            this.GroupBox1.Controls.Add(this.Label11);
            this.GroupBox1.Controls.Add(this.Label28);
            this.GroupBox1.Controls.Add(this.Label29);
            this.GroupBox1.Controls.Add(this.Label30);
            this.GroupBox1.Controls.Add(this.Label31);
            this.GroupBox1.Controls.Add(this.Label32);
            this.GroupBox1.Controls.Add(this.Label33);
            this.GroupBox1.Controls.Add(this.Label43);
            this.GroupBox1.Controls.Add(this.Label45);
            this.GroupBox1.Controls.Add(this.Label46);
            this.GroupBox1.Controls.Add(this.Label47);
            this.GroupBox1.Controls.Add(this.Label48);
            this.GroupBox1.Controls.Add(this.Label49);
            this.GroupBox1.Controls.Add(this.Label50);
            this.GroupBox1.Controls.Add(this.Label52);
            this.GroupBox1.Controls.Add(this.lblChld04Info);
            this.GroupBox1.Controls.Add(this.lblChild05);
            this.GroupBox1.Controls.Add(this.lblChld05Info);
            this.GroupBox1.Controls.Add(this.Label56);
            this.GroupBox1.Controls.Add(this.Label57);
            this.GroupBox1.Controls.Add(this.Label58);
            this.GroupBox1.Controls.Add(this.Label59);
            this.GroupBox1.Controls.Add(this.Label60);
            this.GroupBox1.Controls.Add(this.Label61);
            this.GroupBox1.Controls.Add(this.Label62);
            this.GroupBox1.Controls.Add(this.lblChild03);
            this.GroupBox1.Controls.Add(this.lblChld03Info);
            this.GroupBox1.Controls.Add(this.lblChild02);
            this.GroupBox1.Controls.Add(this.lblChld02Info);
            this.GroupBox1.Controls.Add(this.lblChild04);
            this.GroupBox1.Controls.Add(this.lblChild01);
            this.GroupBox1.Controls.Add(this.Label69);
            this.GroupBox1.Controls.Add(this.Label70);
            this.GroupBox1.Controls.Add(this.Label71);
            this.GroupBox1.Controls.Add(this.Label72);
            this.GroupBox1.Controls.Add(this.lblChld01Info);
            this.GroupBox1.Controls.Add(this.Label74);
            this.GroupBox1.Controls.Add(this.Label75);
            this.GroupBox1.Controls.Add(this.Label76);
            this.GroupBox1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.GroupBox1.Location = new System.Drawing.Point(-1, 456);
            this.GroupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.GroupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.GroupBox1.Size = new System.Drawing.Size(1004, 260);
            this.GroupBox1.TabIndex = 96;
            this.GroupBox1.TabStop = false;
            // 
            // Regsuch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1008, 715);
            this.ControlBox = false;
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this._Option1_7);
            this.Controls.Add(this._Option1_6);
            this.Controls.Add(this._Option1_5);
            this.Controls.Add(this._Option1_4);
            this.Controls.Add(this._Option1_3);
            this.Controls.Add(this._Command1_3);
            this.Controls.Add(this.Combo1);
            this.Controls.Add(this.List3);
            this.Controls.Add(this.Text1);
            this.Controls.Add(this._Option1_2);
            this.Controls.Add(this._Option1_1);
            this.Controls.Add(this._Option1_0);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this._Check2_3);
            this.Controls.Add(this._Command1_7);
            this.Controls.Add(this._Option1_9);
            this.Controls.Add(this._Option1_8);
            this.Controls.Add(this._Option1_10);
            this.Controls.Add(this._Option1_11);
            this.Controls.Add(this._Option1_12);
            this.Controls.Add(this._Option1_13);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this._Label1_1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Regsuch";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Registersuche";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.GroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    public ControlArray<CheckBox> ACheck2;
    public ControlArray<Button> ACommand1;
    public ControlArray<RadioButton> AOption1;
    public ControlArray<Label> ALabel1;
    public ControlArray<Label> ALabel5;
    public ControlArray<Label> ALabel6;
    public ControlArray<Label> ALabel7;
    public ControlArray<Label> ALabel8;
    public ControlArray<Label> ALine1;

    public ToolTip ToolTip1;

    public GroupBox GroupBox1;

    public ComboBox Combo1;

    public ListBox ListBox1;
    public ListBox List3;

    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label Label75;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label Label74;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChld01Info;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label Label72;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label Label71;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label Label70;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label Label69;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChild01;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChild04;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChld02Info;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChild02;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChld03Info;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChild03;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChld05Info;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChild05;
    [DblClickBinding(nameof(IRegSuchViewModel.Label70_DoubleClickCommand))]
    public Label lblChld04Info;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label43;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label33;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label32;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label31;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label30;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label11;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label10;
    [DblClickBinding(nameof(IRegSuchViewModel.Label9_DoubleClickCommand))]
    public Label Label9;
    public Label Label3;
    public Label Label2;
    public Label _Label1_1;
    public Label Label76;
    public Label Label62;
    public Label Label61;
    public Label Label60;
    public Label Label59;
    public Label Label58;
    public Label Label57;
    public Label Label56;
    public Label Label52;
    public Label Label50;
    public Label Label49;
    public Label Label48;
    public Label Label47;
    public Label Label46;
    public Label Label45;
    public Label Label29;
    public Label Label28;

    public RadioButton _Option1_0;
    public RadioButton _Option1_1;
    public RadioButton _Option1_2;
    public RadioButton _Option1_3;
    public RadioButton _Option1_4;
    public RadioButton _Option1_5;
    public RadioButton _Option1_6;
    public RadioButton _Option1_7;
    public RadioButton _Option1_8;
    public RadioButton _Option1_9;
    public RadioButton _Option1_10;
    public RadioButton _Option1_11;
    public RadioButton _Option1_12;
    public RadioButton _Option1_13;

    public Button _Command1_3;
    public Button _Command1_0;
    public Button _Command1_7;

    [TextBinding(nameof(IRegSuchViewModel.Text1_Text))]
    public TextBox Text1;

    public CheckBox _Check2_3;

}
