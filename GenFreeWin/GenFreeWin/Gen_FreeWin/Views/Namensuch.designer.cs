using BaseLib.Helper;
using GenFreeWin.Data;
using GenFreeWin.Main;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

public partial class Namensuch 
{
    public ToolTip ToolTip1;
    public SaveFileDialog CommonDialog1Save;

    [Obsolete]
    public ControlArray<Button> Command1;

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.List7 = new System.Windows.Forms.ListBox();
        this.CommonDialog1Save = new System.Windows.Forms.SaveFileDialog();
        this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
        this.PictureBox1 = new System.Windows.Forms.PictureBox();
        this.btnReady = new System.Windows.Forms.Button();
        this.btnStartSearch = new System.Windows.Forms.Button();
        this.btnFamilySheet = new System.Windows.Forms.Button();
        this.btnPersonSheet = new System.Windows.Forms.Button();
        this.btnClose = new System.Windows.Forms.Button();
        this.btnPrintList = new System.Windows.Forms.Button();
        this.btnReqHint = new System.Windows.Forms.Button();
        this.btnRegisterSearch = new System.Windows.Forms.Button();
        this.chbMale = new System.Windows.Forms.CheckBox();
        this.chbFemales = new System.Windows.Forms.CheckBox();
        this.chbFamOnly = new System.Windows.Forms.CheckBox();
        this.chbSelection = new System.Windows.Forms.CheckBox();
        this.chbOmitSpouse = new System.Windows.Forms.CheckBox();
        this.chbFemale2 = new System.Windows.Forms.CheckBox();
        this.chbMale2 = new System.Windows.Forms.CheckBox();
        this.ComboBox1 = new System.Windows.Forms.ComboBox();
        this.ComboBox2 = new System.Windows.Forms.ComboBox();
        this.Label27 = new System.Windows.Forms.Label();
        this.Label25 = new System.Windows.Forms.Label();
        this.Label26 = new System.Windows.Forms.Label();
        this.Label17 = new System.Windows.Forms.Label();
        this.Label18 = new System.Windows.Forms.Label();
        this.Label16 = new System.Windows.Forms.Label();
        this.Label15 = new System.Windows.Forms.Label();
        this.lblMarr8 = new System.Windows.Forms.Label();
        this.lblMarr7 = new System.Windows.Forms.Label();
        this._Label6_5 = new System.Windows.Forms.Label();
        this._Label7_15 = new System.Windows.Forms.Label();
        this._Label7_14 = new System.Windows.Forms.Label();
        this._Label7_13 = new System.Windows.Forms.Label();
        this._Label7_12 = new System.Windows.Forms.Label();
        this._Label7_11 = new System.Windows.Forms.Label();
        this._Label7_10 = new System.Windows.Forms.Label();
        this._Label7_9 = new System.Windows.Forms.Label();
        this._Label7_8 = new System.Windows.Forms.Label();
        this._Label7_7 = new System.Windows.Forms.Label();
        this._Label7_6 = new System.Windows.Forms.Label();
        this._Label7_5 = new System.Windows.Forms.Label();
        this._Label7_4 = new System.Windows.Forms.Label();
        this._Label7_3 = new System.Windows.Forms.Label();
        this._Label7_2 = new System.Windows.Forms.Label();
        this._Label7_1 = new System.Windows.Forms.Label();
        this._Label7_0 = new System.Windows.Forms.Label();
        this._Line1_28 = new System.Windows.Forms.Label();
        this._Line1_23 = new System.Windows.Forms.Label();
        this._Label6_4 = new System.Windows.Forms.Label();
        this._Label6_3 = new System.Windows.Forms.Label();
        this.lblMarr2 = new System.Windows.Forms.Label();
        this._Label6_2 = new System.Windows.Forms.Label();
        this._Line1_27 = new System.Windows.Forms.Label();
        this._Line1_26 = new System.Windows.Forms.Label();
        this._Line1_25 = new System.Windows.Forms.Label();
        this._Line1_24 = new System.Windows.Forms.Label();
        this._Line1_22 = new System.Windows.Forms.Label();
        this._Line1_21 = new System.Windows.Forms.Label();
        this._Line1_20 = new System.Windows.Forms.Label();
        this._Line1_19 = new System.Windows.Forms.Label();
        this._Line1_17 = new System.Windows.Forms.Label();
        this.lblMarr1 = new System.Windows.Forms.Label();
        this._Line1_16 = new System.Windows.Forms.Label();
        this._Line1_15 = new System.Windows.Forms.Label();
        this._Line1_14 = new System.Windows.Forms.Label();
        this._Line1_13 = new System.Windows.Forms.Label();
        this._Line1_12 = new System.Windows.Forms.Label();
        this._Line1_11 = new System.Windows.Forms.Label();
        this._Line1_10 = new System.Windows.Forms.Label();
        this._Line1_9 = new System.Windows.Forms.Label();
        this._Line1_8 = new System.Windows.Forms.Label();
        this._Label5_14 = new System.Windows.Forms.Label();
        this._Label5_10 = new System.Windows.Forms.Label();
        this._Label5_15 = new System.Windows.Forms.Label();
        this._Line1_7 = new System.Windows.Forms.Label();
        this._Line1_6 = new System.Windows.Forms.Label();
        this._Line1_5 = new System.Windows.Forms.Label();
        this._Line1_4 = new System.Windows.Forms.Label();
        this._Line1_3 = new System.Windows.Forms.Label();
        this._Line1_2 = new System.Windows.Forms.Label();
        this._Line1_1 = new System.Windows.Forms.Label();
        this._Label5_8 = new System.Windows.Forms.Label();
        this._Label5_13 = new System.Windows.Forms.Label();
        this._Label5_7 = new System.Windows.Forms.Label();
        this._Label5_12 = new System.Windows.Forms.Label();
        this._Label5_9 = new System.Windows.Forms.Label();
        this._Label5_6 = new System.Windows.Forms.Label();
        this._Label5_5 = new System.Windows.Forms.Label();
        this._Label5_4 = new System.Windows.Forms.Label();
        this._Label5_3 = new System.Windows.Forms.Label();
        this._Label5_2 = new System.Windows.Forms.Label();
        this._Label5_11 = new System.Windows.Forms.Label();
        this._Label5_1 = new System.Windows.Forms.Label();
        this._Label5_0 = new System.Windows.Forms.Label();
        this._Line1_0 = new System.Windows.Forms.Label();
        this.Label20 = new System.Windows.Forms.Label();
        this.Label19 = new System.Windows.Forms.Label();
        this.Label21 = new System.Windows.Forms.Label();
        this.Label22 = new System.Windows.Forms.Label();
        this.Label23 = new System.Windows.Forms.Label();
        this.Label24 = new System.Windows.Forms.Label();
        this.Label10 = new System.Windows.Forms.Label();
        this.Label9 = new System.Windows.Forms.Label();
        this._Label8_7 = new System.Windows.Forms.Label();
        this._Label8_6 = new System.Windows.Forms.Label();
        this._Label8_5 = new System.Windows.Forms.Label();
        this._Label8_4 = new System.Windows.Forms.Label();
        this._Label8_3 = new System.Windows.Forms.Label();
        this._Label8_2 = new System.Windows.Forms.Label();
        this._Label8_1 = new System.Windows.Forms.Label();
        this._Label8_0 = new System.Windows.Forms.Label();
        this.Label4 = new System.Windows.Forms.Label();
        this.Label3 = new System.Windows.Forms.Label();
        this.lblFamNr = new System.Windows.Forms.Label();
        this.lblPersNr = new System.Windows.Forms.Label();
        this.lblPredicate = new System.Windows.Forms.Label();
        this.lblNickName = new System.Windows.Forms.Label();
        this.Text1 = new System.Windows.Forms.TextBox();
        this.Text2 = new System.Windows.Forms.TextBox();
        this.Timer1 = new System.Windows.Forms.Timer(this.components);
        this.Frame3 = new System.Windows.Forms.GroupBox();
        this.CheckBox19 = new System.Windows.Forms.CheckBox();
        this.List4 = new System.Windows.Forms.ListBox();
        this.List2 = new System.Windows.Forms.ListBox();
        this.List1 = new System.Windows.Forms.ListBox();
        this.ListBox1 = new System.Windows.Forms.ListBox();
        this.List3 = new System.Windows.Forms.ListBox();
        this.fraNameSrchSelection1 = new GenFreeWin.Views.FraNameSrchSelection();
        this.fraPreview1 = new GenFreeWin.Views.FraPreview();
        ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
        this.Frame3.SuspendLayout();
        this.SuspendLayout();
        // 
        // List7
        // 
        this.List7.BackColor = System.Drawing.SystemColors.Window;
        this.List7.Cursor = System.Windows.Forms.Cursors.Default;
        this.List7.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List7.ItemHeight = 19;
        this.List7.Location = new System.Drawing.Point(957, 396);
        this.List7.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.List7.Name = "List7";
        this.List7.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.List7.Size = new System.Drawing.Size(83, 99);
        this.List7.TabIndex = 94;
        this.List7.Visible = false;
        // 
        // OpenFileDialog1
        // 
        this.OpenFileDialog1.FileName = "OpenFileDialog1";
        // 
        // PictureBox1
        // 
        this.PictureBox1.BackColor = System.Drawing.Color.Red;
        this.PictureBox1.Location = new System.Drawing.Point(252, 110);
        this.PictureBox1.Name = "PictureBox1";
        this.PictureBox1.Size = new System.Drawing.Size(100, 50);
        this.PictureBox1.TabIndex = 113;
        this.PictureBox1.TabStop = false;
        this.PictureBox1.Visible = false;
        // 
        // btnReady
        // 
        this.btnReady.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this.btnReady.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnReady.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnReady.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnReady.Location = new System.Drawing.Point(1078, 48);
        this.btnReady.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnReady.Name = "btnReady";
        this.btnReady.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnReady.Size = new System.Drawing.Size(125, 27);
        this.btnReady.TabIndex = 90;
        this.btnReady.Text = "Fertig";
        this.btnReady.UseVisualStyleBackColor = false;
        this.btnReady.Visible = false;
        // 
        // btnStartSearch
        // 
        this.btnStartSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
        this.btnStartSearch.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnStartSearch.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnStartSearch.Location = new System.Drawing.Point(710, 43);
        this.btnStartSearch.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnStartSearch.Name = "btnStartSearch";
        this.btnStartSearch.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnStartSearch.Size = new System.Drawing.Size(125, 44);
        this.btnStartSearch.TabIndex = 16;
        this.btnStartSearch.Text = "S&uche starten";
        this.btnStartSearch.UseVisualStyleBackColor = false;
        // 
        // btnFamilySheet
        // 
        this.btnFamilySheet.BackColor = System.Drawing.SystemColors.Control;
        this.btnFamilySheet.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnFamilySheet.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnFamilySheet.Location = new System.Drawing.Point(878, 9);
        this.btnFamilySheet.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnFamilySheet.Name = "btnFamilySheet";
        this.btnFamilySheet.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnFamilySheet.Size = new System.Drawing.Size(137, 26);
        this.btnFamilySheet.TabIndex = 9;
        this.btnFamilySheet.Text = "&Familienblatt";
        this.btnFamilySheet.UseVisualStyleBackColor = false;
        this.btnFamilySheet.Visible = false;
        // 
        // btnPersonSheet
        // 
        this.btnPersonSheet.BackColor = System.Drawing.SystemColors.Control;
        this.btnPersonSheet.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnPersonSheet.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnPersonSheet.Location = new System.Drawing.Point(878, 43);
        this.btnPersonSheet.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnPersonSheet.Name = "btnPersonSheet";
        this.btnPersonSheet.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnPersonSheet.Size = new System.Drawing.Size(137, 26);
        this.btnPersonSheet.TabIndex = 8;
        this.btnPersonSheet.Text = "&Personenblatt";
        this.btnPersonSheet.UseVisualStyleBackColor = false;
        this.btnPersonSheet.Visible = false;
        // 
        // btnDuplClose
        // 
        this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this.btnClose.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnClose.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnClose.Location = new System.Drawing.Point(710, 9);
        this.btnClose.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnClose.Name = "btnClose";
        this.btnClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnClose.Size = new System.Drawing.Size(125, 27);
        this.btnClose.TabIndex = 33;
        this.btnClose.Text = "&Schlieﬂen";
        this.btnClose.UseVisualStyleBackColor = false;
        // 
        // btnPrintList
        // 
        this.btnPrintList.BackColor = System.Drawing.SystemColors.Control;
        this.btnPrintList.Cursor = System.Windows.Forms.Cursors.Default;
        this.btnPrintList.ForeColor = System.Drawing.SystemColors.ControlText;
        this.btnPrintList.Location = new System.Drawing.Point(878, 77);
        this.btnPrintList.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.btnPrintList.Name = "btnPrintList";
        this.btnPrintList.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.btnPrintList.Size = new System.Drawing.Size(137, 26);
        this.btnPrintList.TabIndex = 99;
        this.btnPrintList.Text = "Liste drucken";
        this.btnPrintList.UseVisualStyleBackColor = false;
        // 
        // btnReqHint
        // 
        this.btnReqHint.Location = new System.Drawing.Point(893, 187);
        this.btnReqHint.Name = "btnReqHint";
        this.btnReqHint.Size = new System.Drawing.Size(86, 24);
        this.btnReqHint.TabIndex = 101;
        this.btnReqHint.Text = "btnReqHint";
        this.btnReqHint.UseVisualStyleBackColor = true;
        // 
        // btnRegisterSearch
        // 
        this.btnRegisterSearch.Location = new System.Drawing.Point(893, 217);
        this.btnRegisterSearch.Name = "btnRegisterSearch";
        this.btnRegisterSearch.Size = new System.Drawing.Size(86, 24);
        this.btnRegisterSearch.TabIndex = 102;
        this.btnRegisterSearch.Text = "btnRegisterSearch";
        this.btnRegisterSearch.UseVisualStyleBackColor = true;
        // 
        // chbMale
        // 
        this.chbMale.BackColor = System.Drawing.SystemColors.Control;
        this.chbMale.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbMale.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbMale.Location = new System.Drawing.Point(303, 58);
        this.chbMale.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbMale.Name = "chbMale";
        this.chbMale.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbMale.Size = new System.Drawing.Size(80, 23);
        this.chbMale.TabIndex = 34;
        this.chbMale.Text = "M‰nner";
        this.chbMale.UseVisualStyleBackColor = false;
        this.chbMale.Visible = false;
        // 
        // chbFemales
        // 
        this.chbFemales.BackColor = System.Drawing.SystemColors.Control;
        this.chbFemales.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbFemales.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbFemales.Location = new System.Drawing.Point(303, 84);
        this.chbFemales.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbFemales.Name = "chbFemales";
        this.chbFemales.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbFemales.Size = new System.Drawing.Size(75, 23);
        this.chbFemales.TabIndex = 35;
        this.chbFemales.Text = "Frauen";
        this.chbFemales.TextAlign = System.Drawing.ContentAlignment.TopLeft;
        this.chbFemales.UseVisualStyleBackColor = false;
        this.chbFemales.Visible = false;
        // 
        // chbFamOnly
        // 
        this.chbFamOnly.BackColor = System.Drawing.SystemColors.Control;
        this.chbFamOnly.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbFamOnly.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbFamOnly.Location = new System.Drawing.Point(467, 116);
        this.chbFamOnly.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbFamOnly.Name = "chbFamOnly";
        this.chbFamOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbFamOnly.Size = new System.Drawing.Size(124, 23);
        this.chbFamOnly.TabIndex = 36;
        this.chbFamOnly.Text = "Nur Familien";
        this.chbFamOnly.UseVisualStyleBackColor = false;
        this.chbFamOnly.Visible = false;
        // 
        // chbSelection
        // 
        this.chbSelection.BackColor = System.Drawing.SystemColors.Control;
        this.chbSelection.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbSelection.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbSelection.Location = new System.Drawing.Point(840, 116);
        this.chbSelection.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbSelection.Name = "chbSelection";
        this.chbSelection.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbSelection.Size = new System.Drawing.Size(167, 23);
        this.chbSelection.TabIndex = 37;
        this.chbSelection.Text = "Auswahl beibehalten";
        this.chbSelection.UseVisualStyleBackColor = false;
        // 
        // chbOmitSpouse
        // 
        this.chbOmitSpouse.BackColor = System.Drawing.SystemColors.Control;
        this.chbOmitSpouse.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbOmitSpouse.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbOmitSpouse.Location = new System.Drawing.Point(601, 116);
        this.chbOmitSpouse.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbOmitSpouse.Name = "Check3";
        this.chbOmitSpouse.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbOmitSpouse.Size = new System.Drawing.Size(226, 23);
        this.chbOmitSpouse.TabIndex = 93;
        this.chbOmitSpouse.Text = "Ehepartner nicht anzeigen";
        this.chbOmitSpouse.UseVisualStyleBackColor = false;
        // 
        // chbFemale2
        // 
        this.chbFemale2.BackColor = System.Drawing.SystemColors.Control;
        this.chbFemale2.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbFemale2.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbFemale2.Location = new System.Drawing.Point(352, 84);
        this.chbFemale2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbFemale2.Name = "chbFemale2";
        this.chbFemale2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbFemale2.Size = new System.Drawing.Size(90, 23);
        this.chbFemale2.TabIndex = 92;
        this.chbFemale2.Text = "weiblich";
        this.chbFemale2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
        this.chbFemale2.UseVisualStyleBackColor = false;
        // 
        // chbMale2
        // 
        this.chbMale2.BackColor = System.Drawing.SystemColors.Control;
        this.chbMale2.Cursor = System.Windows.Forms.Cursors.Default;
        this.chbMale2.ForeColor = System.Drawing.SystemColors.ControlText;
        this.chbMale2.Location = new System.Drawing.Point(352, 58);
        this.chbMale2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.chbMale2.Name = "chbMale2";
        this.chbMale2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.chbMale2.Size = new System.Drawing.Size(86, 23);
        this.chbMale2.TabIndex = 91;
        this.chbMale2.Text = "m‰nnlich";
        this.chbMale2.UseVisualStyleBackColor = false;
        // 
        // cbxProperty
        // 
        this.ComboBox1.CausesValidation = false;
        this.ComboBox1.FormattingEnabled = true;
        this.ComboBox1.Location = new System.Drawing.Point(8, 30);
        this.ComboBox1.Name = "ComboBox1";
        this.ComboBox1.Size = new System.Drawing.Size(290, 27);
        this.ComboBox1.TabIndex = 100;
        // 
        // ComboBox2
        // 
        this.ComboBox2.FormattingEnabled = true;
        this.ComboBox2.Location = new System.Drawing.Point(298, 29);
        this.ComboBox2.Name = "ComboBox2";
        this.ComboBox2.Size = new System.Drawing.Size(399, 27);
        this.ComboBox2.TabIndex = 109;
        // 
        // Label27
        // 
        this.Label27.AutoEllipsis = true;
        this.Label27.BackColor = System.Drawing.SystemColors.Control;
        this.Label27.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label27.Location = new System.Drawing.Point(342, 91);
        this.Label27.Name = "Label27";
        this.Label27.Size = new System.Drawing.Size(300, 18);
        this.Label27.TabIndex = 128;
        this.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // Label25
        // 
        this.Label25.AutoEllipsis = true;
        this.Label25.BackColor = System.Drawing.SystemColors.Control;
        this.Label25.Location = new System.Drawing.Point(154, 180);
        this.Label25.Name = "Label25";
        this.Label25.Size = new System.Drawing.Size(150, 18);
        this.Label25.TabIndex = 126;
        // 
        // Label26
        // 
        this.Label26.AutoEllipsis = true;
        this.Label26.BackColor = System.Drawing.SystemColors.Control;
        this.Label26.Location = new System.Drawing.Point(0, 180);
        this.Label26.Name = "Label26";
        this.Label26.Size = new System.Drawing.Size(150, 18);
        this.Label26.TabIndex = 125;
        // 
        // Label17
        // 
        this.Label17.AutoEllipsis = true;
        this.Label17.BackColor = System.Drawing.SystemColors.Control;
        this.Label17.Location = new System.Drawing.Point(493, 157);
        this.Label17.Name = "Label17";
        this.Label17.Size = new System.Drawing.Size(150, 18);
        this.Label17.TabIndex = 118;
        // 
        // Label18
        // 
        this.Label18.AutoEllipsis = true;
        this.Label18.BackColor = System.Drawing.SystemColors.Control;
        this.Label18.Location = new System.Drawing.Point(339, 157);
        this.Label18.Name = "Label18";
        this.Label18.Size = new System.Drawing.Size(150, 18);
        this.Label18.TabIndex = 117;
        // 
        // Label16
        // 
        this.Label16.AutoEllipsis = true;
        this.Label16.BackColor = System.Drawing.SystemColors.Control;
        this.Label16.Location = new System.Drawing.Point(493, 61);
        this.Label16.Name = "Label16";
        this.Label16.Size = new System.Drawing.Size(150, 18);
        this.Label16.TabIndex = 116;
        // 
        // Label15
        // 
        this.Label15.AutoEllipsis = true;
        this.Label15.BackColor = System.Drawing.SystemColors.Control;
        this.Label15.Location = new System.Drawing.Point(339, 61);
        this.Label15.Name = "Label15";
        this.Label15.Size = new System.Drawing.Size(150, 18);
        this.Label15.TabIndex = 115;
        // 
        // lblMarr8
        // 
        this.lblMarr8.BackColor = System.Drawing.SystemColors.Control;
        this.lblMarr8.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblMarr8.ForeColor = System.Drawing.SystemColors.ControlText;
        this.lblMarr8.Location = new System.Drawing.Point(302, 37);
        this.lblMarr8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.lblMarr8.Name = "lblMarr8";
        this.lblMarr8.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblMarr8.Size = new System.Drawing.Size(29, 14);
        this.lblMarr8.TabIndex = 86;
        this.lblMarr8.Text = "oo";
        this.lblMarr8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        // 
        // lblMarr7
        // 
        this.lblMarr7.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.lblMarr7.BackColor = System.Drawing.SystemColors.Control;
        this.lblMarr7.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblMarr7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.lblMarr7.ForeColor = System.Drawing.SystemColors.ControlText;
        this.lblMarr7.Location = new System.Drawing.Point(302, 131);
        this.lblMarr7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.lblMarr7.Name = "lblMarr7";
        this.lblMarr7.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblMarr7.Size = new System.Drawing.Size(29, 19);
        this.lblMarr7.TabIndex = 85;
        this.lblMarr7.Text = "oo";
        this.lblMarr7.TextAlign = System.Drawing.ContentAlignment.TopRight;
        // 
        // _Label6_5
        // 
        this._Label6_5.BackColor = System.Drawing.SystemColors.Control;
        this._Label6_5.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label6_5.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label6_5.Location = new System.Drawing.Point(653, 92);
        this._Label6_5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label6_5.Name = "_Label6_5";
        this._Label6_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label6_5.Size = new System.Drawing.Size(29, 26);
        this._Label6_5.TabIndex = 84;
        this._Label6_5.Text = "oo";
        // 
        // _Label7_15
        // 
        this._Label7_15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_15.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_15.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_15.Location = new System.Drawing.Point(554, 199);
        this._Label7_15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_15.Name = "_Label7_15";
        this._Label7_15.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_15.Size = new System.Drawing.Size(69, 20);
        this._Label7_15.TabIndex = 75;
        // 
        // _Label7_14
        // 
        this._Label7_14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_14.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_14.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_14.Location = new System.Drawing.Point(509, 191);
        this._Label7_14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_14.Name = "_Label7_14";
        this._Label7_14.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_14.Size = new System.Drawing.Size(69, 20);
        this._Label7_14.TabIndex = 74;
        // 
        // _Label7_13
        // 
        this._Label7_13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_13.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_13.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_13.Location = new System.Drawing.Point(607, 200);
        this._Label7_13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_13.Name = "_Label7_13";
        this._Label7_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_13.Size = new System.Drawing.Size(69, 20);
        this._Label7_13.TabIndex = 73;
        // 
        // _Label7_12
        // 
        this._Label7_12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this._Label7_12.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_12.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_12.Location = new System.Drawing.Point(586, 199);
        this._Label7_12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_12.Name = "_Label7_12";
        this._Label7_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_12.Size = new System.Drawing.Size(69, 20);
        this._Label7_12.TabIndex = 72;
        // 
        // _Label7_11
        // 
        this._Label7_11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_11.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_11.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_11.Location = new System.Drawing.Point(492, 198);
        this._Label7_11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_11.Name = "_Label7_11";
        this._Label7_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_11.Size = new System.Drawing.Size(69, 20);
        this._Label7_11.TabIndex = 71;
        // 
        // _Label7_10
        // 
        this._Label7_10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_10.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_10.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_10.Location = new System.Drawing.Point(509, 192);
        this._Label7_10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_10.Name = "_Label7_10";
        this._Label7_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_10.Size = new System.Drawing.Size(69, 20);
        this._Label7_10.TabIndex = 70;
        // 
        // _Label7_9
        // 
        this._Label7_9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this._Label7_9.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_9.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_9.Location = new System.Drawing.Point(586, 192);
        this._Label7_9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_9.Name = "_Label7_9";
        this._Label7_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_9.Size = new System.Drawing.Size(69, 20);
        this._Label7_9.TabIndex = 69;
        // 
        // _Label7_8
        // 
        this._Label7_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_8.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_8.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_8.Location = new System.Drawing.Point(588, 191);
        this._Label7_8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_8.Name = "_Label7_8";
        this._Label7_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_8.Size = new System.Drawing.Size(69, 20);
        this._Label7_8.TabIndex = 68;
        // 
        // _Label7_7
        // 
        this._Label7_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_7.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_7.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_7.Location = new System.Drawing.Point(452, 200);
        this._Label7_7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_7.Name = "_Label7_7";
        this._Label7_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_7.Size = new System.Drawing.Size(69, 20);
        this._Label7_7.TabIndex = 67;
        // 
        // _Label7_6
        // 
        this._Label7_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_6.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_6.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_6.Location = new System.Drawing.Point(452, 203);
        this._Label7_6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_6.Name = "_Label7_6";
        this._Label7_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_6.Size = new System.Drawing.Size(69, 20);
        this._Label7_6.TabIndex = 66;
        // 
        // _Label7_5
        // 
        this._Label7_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_5.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_5.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_5.Location = new System.Drawing.Point(349, 198);
        this._Label7_5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_5.Name = "_Label7_5";
        this._Label7_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_5.Size = new System.Drawing.Size(69, 20);
        this._Label7_5.TabIndex = 65;
        // 
        // _Label7_4
        // 
        this._Label7_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_4.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_4.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_4.Location = new System.Drawing.Point(349, 203);
        this._Label7_4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_4.Name = "_Label7_4";
        this._Label7_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_4.Size = new System.Drawing.Size(69, 20);
        this._Label7_4.TabIndex = 64;
        // 
        // _Label7_3
        // 
        this._Label7_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_3.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_3.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_3.Location = new System.Drawing.Point(5, 178);
        this._Label7_3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_3.Name = "_Label7_3";
        this._Label7_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_3.Size = new System.Drawing.Size(69, 20);
        this._Label7_3.TabIndex = 63;
        // 
        // _Label7_2
        // 
        this._Label7_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_2.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_2.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_2.Location = new System.Drawing.Point(175, 203);
        this._Label7_2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_2.Name = "_Label7_2";
        this._Label7_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_2.Size = new System.Drawing.Size(69, 20);
        this._Label7_2.TabIndex = 62;
        // 
        // _Label7_1
        // 
        this._Label7_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_1.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_1.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_1.Location = new System.Drawing.Point(39, 203);
        this._Label7_1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_1.Name = "_Label7_1";
        this._Label7_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_1.Size = new System.Drawing.Size(69, 20);
        this._Label7_1.TabIndex = 61;
        // 
        // _Label7_0
        // 
        this._Label7_0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this._Label7_0.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label7_0.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label7_0.Location = new System.Drawing.Point(56, 203);
        this._Label7_0.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label7_0.Name = "_Label7_0";
        this._Label7_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label7_0.Size = new System.Drawing.Size(69, 20);
        this._Label7_0.TabIndex = 60;
        // 
        // _Line1_28
        // 
        this._Line1_28.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_28.Location = new System.Drawing.Point(651, 102);
        this._Line1_28.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_28.Name = "_Line1_28";
        this._Line1_28.Size = new System.Drawing.Size(25, 1);
        this._Line1_28.TabIndex = 87;
        // 
        // _Line1_23
        // 
        this._Line1_23.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_23.Location = new System.Drawing.Point(651, 50);
        this._Line1_23.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_23.Name = "_Line1_23";
        this._Line1_23.Size = new System.Drawing.Size(1, 97);
        this._Line1_23.TabIndex = 88;
        // 
        // _Label6_4
        // 
        this._Label6_4.BackColor = System.Drawing.SystemColors.Control;
        this._Label6_4.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label6_4.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label6_4.Location = new System.Drawing.Point(715, 191);
        this._Label6_4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label6_4.Name = "_Label6_4";
        this._Label6_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label6_4.Size = new System.Drawing.Size(29, 14);
        this._Label6_4.TabIndex = 59;
        this._Label6_4.Text = "oo";
        // 
        // _Label6_3
        // 
        this._Label6_3.BackColor = System.Drawing.SystemColors.Control;
        this._Label6_3.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label6_3.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label6_3.Location = new System.Drawing.Point(715, 151);
        this._Label6_3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label6_3.Name = "_Label6_3";
        this._Label6_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label6_3.Size = new System.Drawing.Size(29, 14);
        this._Label6_3.TabIndex = 58;
        this._Label6_3.Text = "oo";
        // 
        // lblMarr2
        // 
        this.lblMarr2.BackColor = System.Drawing.SystemColors.Control;
        this.lblMarr2.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblMarr2.ForeColor = System.Drawing.SystemColors.ControlText;
        this.lblMarr2.Location = new System.Drawing.Point(715, 112);
        this.lblMarr2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.lblMarr2.Name = "lblMarr2";
        this.lblMarr2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblMarr2.Size = new System.Drawing.Size(29, 14);
        this.lblMarr2.TabIndex = 56;
        this.lblMarr2.Text = "oo";
        // 
        // _Label6_2
        // 
        this._Label6_2.BackColor = System.Drawing.SystemColors.Control;
        this._Label6_2.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label6_2.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label6_2.Location = new System.Drawing.Point(715, 33);
        this._Label6_2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label6_2.Name = "_Label6_2";
        this._Label6_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label6_2.Size = new System.Drawing.Size(29, 14);
        this._Label6_2.TabIndex = 57;
        this._Label6_2.Text = "oo";
        // 
        // _Line1_27
        // 
        this._Line1_27.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_27.Location = new System.Drawing.Point(701, 42);
        this._Line1_27.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_27.Name = "_Line1_27";
        this._Line1_27.Size = new System.Drawing.Size(29, 1);
        this._Line1_27.TabIndex = 89;
        // 
        // _Line1_26
        // 
        this._Line1_26.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_26.Location = new System.Drawing.Point(707, 160);
        this._Line1_26.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_26.Name = "_Line1_26";
        this._Line1_26.Size = new System.Drawing.Size(29, 1);
        this._Line1_26.TabIndex = 90;
        // 
        // _Line1_25
        // 
        this._Line1_25.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_25.Location = new System.Drawing.Point(707, 199);
        this._Line1_25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_25.Name = "_Line1_25";
        this._Line1_25.Size = new System.Drawing.Size(29, 1);
        this._Line1_25.TabIndex = 91;
        // 
        // _Line1_24
        // 
        this._Line1_24.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_24.Location = new System.Drawing.Point(704, 120);
        this._Line1_24.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_24.Name = "_Line1_24";
        this._Line1_24.Size = new System.Drawing.Size(29, 1);
        this._Line1_24.TabIndex = 92;
        // 
        // _Line1_22
        // 
        this._Line1_22.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_22.Location = new System.Drawing.Point(701, 34);
        this._Line1_22.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_22.Name = "_Line1_22";
        this._Line1_22.Size = new System.Drawing.Size(1, 8);
        this._Line1_22.TabIndex = 93;
        // 
        // _Line1_21
        // 
        this._Line1_21.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_21.Location = new System.Drawing.Point(704, 112);
        this._Line1_21.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_21.Name = "_Line1_21";
        this._Line1_21.Size = new System.Drawing.Size(1, 8);
        this._Line1_21.TabIndex = 94;
        // 
        // _Line1_20
        // 
        this._Line1_20.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_20.Location = new System.Drawing.Point(707, 152);
        this._Line1_20.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_20.Name = "_Line1_20";
        this._Line1_20.Size = new System.Drawing.Size(1, 8);
        this._Line1_20.TabIndex = 95;
        // 
        // _Line1_19
        // 
        this._Line1_19.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_19.Location = new System.Drawing.Point(707, 191);
        this._Line1_19.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_19.Name = "_Line1_19";
        this._Line1_19.Size = new System.Drawing.Size(1, 8);
        this._Line1_19.TabIndex = 96;
        // 
        // _Line1_17
        // 
        this._Line1_17.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_17.Location = new System.Drawing.Point(704, 73);
        this._Line1_17.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_17.Name = "_Line1_17";
        this._Line1_17.Size = new System.Drawing.Size(1, 8);
        this._Line1_17.TabIndex = 97;
        // 
        // lblMarr1
        // 
        this.lblMarr1.BackColor = System.Drawing.SystemColors.Control;
        this.lblMarr1.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblMarr1.ForeColor = System.Drawing.SystemColors.ControlText;
        this.lblMarr1.Location = new System.Drawing.Point(715, 72);
        this.lblMarr1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.lblMarr1.Name = "lblMarr1";
        this.lblMarr1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblMarr1.Size = new System.Drawing.Size(29, 14);
        this.lblMarr1.TabIndex = 55;
        this.lblMarr1.Text = "oo";
        // 
        // _Line1_16
        // 
        this._Line1_16.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_16.Location = new System.Drawing.Point(704, 81);
        this._Line1_16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_16.Name = "_Line1_16";
        this._Line1_16.Size = new System.Drawing.Size(29, 1);
        this._Line1_16.TabIndex = 98;
        // 
        // _Line1_15
        // 
        this._Line1_15.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_15.Location = new System.Drawing.Point(675, 141);
        this._Line1_15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_15.Name = "_Line1_15";
        this._Line1_15.Size = new System.Drawing.Size(27, 1);
        this._Line1_15.TabIndex = 99;
        // 
        // _Line1_14
        // 
        this._Line1_14.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_14.Location = new System.Drawing.Point(645, 50);
        this._Line1_14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_14.Name = "_Line1_14";
        this._Line1_14.Size = new System.Drawing.Size(8, 1);
        this._Line1_14.TabIndex = 100;
        // 
        // _Line1_13
        // 
        this._Line1_13.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_13.Location = new System.Drawing.Point(645, 146);
        this._Line1_13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_13.Name = "_Line1_13";
        this._Line1_13.Size = new System.Drawing.Size(8, 1);
        this._Line1_13.TabIndex = 101;
        // 
        // _Line1_12
        // 
        this._Line1_12.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_12.Location = new System.Drawing.Point(677, 180);
        this._Line1_12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_12.Name = "_Line1_12";
        this._Line1_12.Size = new System.Drawing.Size(27, 1);
        this._Line1_12.TabIndex = 102;
        // 
        // _Line1_11
        // 
        this._Line1_11.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_11.Location = new System.Drawing.Point(677, 24);
        this._Line1_11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_11.Name = "_Line1_11";
        this._Line1_11.Size = new System.Drawing.Size(21, 1);
        this._Line1_11.TabIndex = 103;
        // 
        // _Line1_10
        // 
        this._Line1_10.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_10.Location = new System.Drawing.Point(675, 63);
        this._Line1_10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_10.Name = "_Line1_10";
        this._Line1_10.Size = new System.Drawing.Size(24, 1);
        this._Line1_10.TabIndex = 104;
        // 
        // _Line1_9
        // 
        this._Line1_9.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_9.Location = new System.Drawing.Point(675, 102);
        this._Line1_9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_9.Name = "_Line1_9";
        this._Line1_9.Size = new System.Drawing.Size(27, 1);
        this._Line1_9.TabIndex = 105;
        // 
        // _Line1_8
        // 
        this._Line1_8.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_8.Location = new System.Drawing.Point(675, 24);
        this._Line1_8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_8.Name = "_Line1_8";
        this._Line1_8.Size = new System.Drawing.Size(1, 158);
        this._Line1_8.TabIndex = 106;
        // 
        // _Label5_14
        // 
        this._Label5_14.AutoEllipsis = true;
        this._Label5_14.BackColor = System.Drawing.Color.White;
        this._Label5_14.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_14.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_14.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_14.Location = new System.Drawing.Point(744, 153);
        this._Label5_14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_14.Name = "_Label5_14";
        this._Label5_14.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_14.Size = new System.Drawing.Size(266, 17);
        this._Label5_14.TabIndex = 54;
        // 
        // _Label5_10
        // 
        this._Label5_10.AutoEllipsis = true;
        this._Label5_10.BackColor = System.Drawing.Color.White;
        this._Label5_10.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_10.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_10.Location = new System.Drawing.Point(705, 173);
        this._Label5_10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_10.Name = "_Label5_10";
        this._Label5_10.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_10.Size = new System.Drawing.Size(305, 17);
        this._Label5_10.TabIndex = 53;
        // 
        // _Label5_15
        // 
        this._Label5_15.AutoEllipsis = true;
        this._Label5_15.BackColor = System.Drawing.Color.White;
        this._Label5_15.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_15.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_15.Location = new System.Drawing.Point(744, 192);
        this._Label5_15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_15.Name = "_Label5_15";
        this._Label5_15.Size = new System.Drawing.Size(266, 17);
        this._Label5_15.TabIndex = 52;
        // 
        // _Line1_7
        // 
        this._Line1_7.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_7.Location = new System.Drawing.Point(325, 141);
        this._Line1_7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_7.Name = "_Line1_7";
        this._Line1_7.Size = new System.Drawing.Size(13, 1);
        this._Line1_7.TabIndex = 107;
        // 
        // _Line1_6
        // 
        this._Line1_6.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_6.Location = new System.Drawing.Point(325, 118);
        this._Line1_6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_6.Name = "_Line1_6";
        this._Line1_6.Size = new System.Drawing.Size(1, 50);
        this._Line1_6.TabIndex = 108;
        // 
        // _Line1_5
        // 
        this._Line1_5.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_5.Location = new System.Drawing.Point(325, 47);
        this._Line1_5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_5.Name = "_Line1_5";
        this._Line1_5.Size = new System.Drawing.Size(13, 1);
        this._Line1_5.TabIndex = 109;
        // 
        // _Line1_4
        // 
        this._Line1_4.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_4.Location = new System.Drawing.Point(312, 118);
        this._Line1_4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_4.Name = "_Line1_4";
        this._Line1_4.Size = new System.Drawing.Size(13, 1);
        this._Line1_4.TabIndex = 110;
        // 
        // _Line1_3
        // 
        this._Line1_3.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_3.Location = new System.Drawing.Point(312, 167);
        this._Line1_3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_3.Name = "_Line1_3";
        this._Line1_3.Size = new System.Drawing.Size(13, 1);
        this._Line1_3.TabIndex = 111;
        // 
        // _Line1_2
        // 
        this._Line1_2.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_2.Location = new System.Drawing.Point(325, 27);
        this._Line1_2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_2.Name = "_Line1_2";
        this._Line1_2.Size = new System.Drawing.Size(1, 42);
        this._Line1_2.TabIndex = 112;
        // 
        // _Line1_1
        // 
        this._Line1_1.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_1.Location = new System.Drawing.Point(312, 71);
        this._Line1_1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_1.Name = "_Line1_1";
        this._Line1_1.Size = new System.Drawing.Size(13, 1);
        this._Line1_1.TabIndex = 113;
        // 
        // _Label5_8
        // 
        this._Label5_8.AutoEllipsis = true;
        this._Label5_8.BackColor = System.Drawing.Color.White;
        this._Label5_8.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_8.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_8.Location = new System.Drawing.Point(705, 94);
        this._Label5_8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_8.Name = "_Label5_8";
        this._Label5_8.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_8.Size = new System.Drawing.Size(305, 17);
        this._Label5_8.TabIndex = 51;
        // 
        // _Label5_13
        // 
        this._Label5_13.AutoEllipsis = true;
        this._Label5_13.BackColor = System.Drawing.Color.White;
        this._Label5_13.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_13.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_13.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_13.Location = new System.Drawing.Point(744, 114);
        this._Label5_13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_13.Name = "_Label5_13";
        this._Label5_13.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_13.Size = new System.Drawing.Size(266, 17);
        this._Label5_13.TabIndex = 50;
        // 
        // _Label5_7
        // 
        this._Label5_7.AutoEllipsis = true;
        this._Label5_7.BackColor = System.Drawing.Color.White;
        this._Label5_7.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_7.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_7.Location = new System.Drawing.Point(705, 56);
        this._Label5_7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_7.Name = "_Label5_7";
        this._Label5_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_7.Size = new System.Drawing.Size(305, 17);
        this._Label5_7.TabIndex = 49;
        // 
        // _Label5_12
        // 
        this._Label5_12.AutoEllipsis = true;
        this._Label5_12.BackColor = System.Drawing.Color.White;
        this._Label5_12.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_12.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_12.Location = new System.Drawing.Point(744, 75);
        this._Label5_12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_12.Name = "_Label5_12";
        this._Label5_12.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_12.Size = new System.Drawing.Size(266, 17);
        this._Label5_12.TabIndex = 48;
        // 
        // _Label5_9
        // 
        this._Label5_9.AutoEllipsis = true;
        this._Label5_9.BackColor = System.Drawing.Color.White;
        this._Label5_9.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_9.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_9.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_9.Location = new System.Drawing.Point(705, 133);
        this._Label5_9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_9.Name = "_Label5_9";
        this._Label5_9.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_9.Size = new System.Drawing.Size(305, 17);
        this._Label5_9.TabIndex = 47;
        // 
        // _Label5_6
        // 
        this._Label5_6.AutoEllipsis = true;
        this._Label5_6.BackColor = System.Drawing.Color.White;
        this._Label5_6.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_6.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_6.Location = new System.Drawing.Point(705, 16);
        this._Label5_6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_6.Name = "_Label5_6";
        this._Label5_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_6.Size = new System.Drawing.Size(305, 17);
        this._Label5_6.TabIndex = 46;
        // 
        // _Label5_5
        // 
        this._Label5_5.AutoEllipsis = true;
        this._Label5_5.BackColor = System.Drawing.Color.White;
        this._Label5_5.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_5.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_5.Location = new System.Drawing.Point(339, 133);
        this._Label5_5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_5.Name = "_Label5_5";
        this._Label5_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_5.Size = new System.Drawing.Size(305, 18);
        this._Label5_5.TabIndex = 45;
        // 
        // _Label5_4
        // 
        this._Label5_4.AutoEllipsis = true;
        this._Label5_4.BackColor = System.Drawing.Color.White;
        this._Label5_4.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_4.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_4.Location = new System.Drawing.Point(339, 39);
        this._Label5_4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_4.Name = "_Label5_4";
        this._Label5_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_4.Size = new System.Drawing.Size(305, 18);
        this._Label5_4.TabIndex = 44;
        // 
        // _Label5_3
        // 
        this._Label5_3.AutoEllipsis = true;
        this._Label5_3.BackColor = System.Drawing.Color.White;
        this._Label5_3.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_3.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_3.Location = new System.Drawing.Point(5, 160);
        this._Label5_3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_3.Name = "_Label5_3";
        this._Label5_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_3.Size = new System.Drawing.Size(305, 18);
        this._Label5_3.TabIndex = 43;
        // 
        // _Label5_2
        // 
        this._Label5_2.AutoEllipsis = true;
        this._Label5_2.BackColor = System.Drawing.Color.White;
        this._Label5_2.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_2.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_2.Location = new System.Drawing.Point(5, 110);
        this._Label5_2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_2.Name = "_Label5_2";
        this._Label5_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_2.Size = new System.Drawing.Size(305, 18);
        this._Label5_2.TabIndex = 42;
        // 
        // _Label5_11
        // 
        this._Label5_11.AutoEllipsis = true;
        this._Label5_11.BackColor = System.Drawing.Color.White;
        this._Label5_11.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_11.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_11.Location = new System.Drawing.Point(744, 35);
        this._Label5_11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_11.Name = "_Label5_11";
        this._Label5_11.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_11.Size = new System.Drawing.Size(266, 19);
        this._Label5_11.TabIndex = 41;
        // 
        // _Label5_1
        // 
        this._Label5_1.AutoEllipsis = true;
        this._Label5_1.BackColor = System.Drawing.Color.White;
        this._Label5_1.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_1.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_1.Location = new System.Drawing.Point(5, 60);
        this._Label5_1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_1.Name = "_Label5_1";
        this._Label5_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_1.Size = new System.Drawing.Size(309, 18);
        this._Label5_1.TabIndex = 40;
        // 
        // _Label5_0
        // 
        this._Label5_0.AutoEllipsis = true;
        this._Label5_0.BackColor = System.Drawing.Color.White;
        this._Label5_0.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label5_0.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this._Label5_0.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label5_0.Location = new System.Drawing.Point(5, 16);
        this._Label5_0.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label5_0.Name = "_Label5_0";
        this._Label5_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label5_0.Size = new System.Drawing.Size(309, 18);
        this._Label5_0.TabIndex = 39;
        // 
        // _Line1_0
        // 
        this._Line1_0.BackColor = System.Drawing.SystemColors.WindowText;
        this._Line1_0.Location = new System.Drawing.Point(312, 26);
        this._Line1_0.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Line1_0.Name = "_Line1_0";
        this._Line1_0.Size = new System.Drawing.Size(13, 1);
        this._Line1_0.TabIndex = 114;
        // 
        // Label20
        // 
        this.Label20.AutoEllipsis = true;
        this.Label20.BackColor = System.Drawing.SystemColors.Control;
        this.Label20.Location = new System.Drawing.Point(5, 36);
        this.Label20.Name = "Label20";
        this.Label20.Size = new System.Drawing.Size(150, 18);
        this.Label20.TabIndex = 119;
        // 
        // Label19
        // 
        this.Label19.AutoEllipsis = true;
        this.Label19.BackColor = System.Drawing.SystemColors.Control;
        this.Label19.Location = new System.Drawing.Point(159, 36);
        this.Label19.Name = "Label19";
        this.Label19.Size = new System.Drawing.Size(150, 18);
        this.Label19.TabIndex = 120;
        // 
        // Label21
        // 
        this.Label21.AutoEllipsis = true;
        this.Label21.BackColor = System.Drawing.SystemColors.Control;
        this.Label21.Location = new System.Drawing.Point(159, 81);
        this.Label21.Name = "Label21";
        this.Label21.Size = new System.Drawing.Size(150, 18);
        this.Label21.TabIndex = 122;
        // 
        // Label22
        // 
        this.Label22.AutoEllipsis = true;
        this.Label22.BackColor = System.Drawing.SystemColors.Control;
        this.Label22.Location = new System.Drawing.Point(5, 81);
        this.Label22.Name = "Label22";
        this.Label22.Size = new System.Drawing.Size(150, 18);
        this.Label22.TabIndex = 121;
        // 
        // Label23
        // 
        this.Label23.AutoEllipsis = true;
        this.Label23.BackColor = System.Drawing.SystemColors.Control;
        this.Label23.Location = new System.Drawing.Point(159, 132);
        this.Label23.Name = "Label23";
        this.Label23.Size = new System.Drawing.Size(150, 18);
        this.Label23.TabIndex = 124;
        // 
        // Label24
        // 
        this.Label24.AutoEllipsis = true;
        this.Label24.BackColor = System.Drawing.SystemColors.Control;
        this.Label24.Location = new System.Drawing.Point(5, 132);
        this.Label24.Name = "Label24";
        this.Label24.Size = new System.Drawing.Size(150, 18);
        this.Label24.TabIndex = 123;
        // 
        // Label10
        // 
        this.Label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
        this.Label10.Cursor = System.Windows.Forms.Cursors.Default;
        this.Label10.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label10.Location = new System.Drawing.Point(465, 66);
        this.Label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.Label10.Name = "Label10";
        this.Label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Label10.Size = new System.Drawing.Size(191, 16);
        this.Label10.TabIndex = 96;
        this.Label10.Text = "Einschr‰nkung auf Namen";
        // 
        // lblOccubation
        // 
        this.Label9.BackColor = System.Drawing.SystemColors.Control;
        this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
        this.Label9.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label9.Location = new System.Drawing.Point(837, 116);
        this.Label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.Label9.Name = "Label9";
        this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Label9.Size = new System.Drawing.Size(167, 27);
        this.Label9.TabIndex = 89;
        this.Label9.Text = "Ausgew‰hlte Personen";
        this.Label9.Visible = false;
        // 
        // _Label8_7
        // 
        this._Label8_7.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_7.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_7.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_7.Location = new System.Drawing.Point(909, 282);
        this._Label8_7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_7.Name = "_Label8_7";
        this._Label8_7.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_7.Size = new System.Drawing.Size(95, 27);
        this._Label8_7.TabIndex = 83;
        this._Label8_7.Text = "Label8";
        this._Label8_7.Visible = false;
        // 
        // _Label8_6
        // 
        this._Label8_6.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_6.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_6.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_6.Location = new System.Drawing.Point(890, 214);
        this._Label8_6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_6.Name = "_Label8_6";
        this._Label8_6.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_6.Size = new System.Drawing.Size(95, 27);
        this._Label8_6.TabIndex = 82;
        this._Label8_6.Text = "Label8";
        this._Label8_6.Visible = false;
        // 
        // _Label8_5
        // 
        this._Label8_5.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_5.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_5.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_5.Location = new System.Drawing.Point(909, 175);
        this._Label8_5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_5.Name = "_Label8_5";
        this._Label8_5.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_5.Size = new System.Drawing.Size(95, 27);
        this._Label8_5.TabIndex = 81;
        this._Label8_5.Text = "Label8";
        this._Label8_5.Visible = false;
        // 
        // _Label8_4
        // 
        this._Label8_4.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_4.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_4.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_4.Location = new System.Drawing.Point(1041, 91);
        this._Label8_4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_4.Name = "_Label8_4";
        this._Label8_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_4.Size = new System.Drawing.Size(95, 27);
        this._Label8_4.TabIndex = 80;
        this._Label8_4.Text = "Label8";
        this._Label8_4.Visible = false;
        // 
        // _Label8_3
        // 
        this._Label8_3.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_3.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_3.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_3.Location = new System.Drawing.Point(1051, 20);
        this._Label8_3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_3.Name = "_Label8_3";
        this._Label8_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_3.Size = new System.Drawing.Size(95, 27);
        this._Label8_3.TabIndex = 79;
        this._Label8_3.Text = "Label8";
        this._Label8_3.Visible = false;
        // 
        // _Label8_2
        // 
        this._Label8_2.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_2.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_2.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_2.Location = new System.Drawing.Point(1074, -5);
        this._Label8_2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_2.Name = "_Label8_2";
        this._Label8_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_2.Size = new System.Drawing.Size(95, 27);
        this._Label8_2.TabIndex = 78;
        this._Label8_2.Text = "Label8";
        this._Label8_2.Visible = false;
        // 
        // _Label8_1
        // 
        this._Label8_1.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_1.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_1.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_1.Location = new System.Drawing.Point(1074, 120);
        this._Label8_1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_1.Name = "_Label8_1";
        this._Label8_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_1.Size = new System.Drawing.Size(95, 27);
        this._Label8_1.TabIndex = 77;
        this._Label8_1.Text = "Label8";
        this._Label8_1.Visible = false;
        // 
        // _Label8_0
        // 
        this._Label8_0.BackColor = System.Drawing.SystemColors.Control;
        this._Label8_0.Cursor = System.Windows.Forms.Cursors.Default;
        this._Label8_0.ForeColor = System.Drawing.SystemColors.ControlText;
        this._Label8_0.Location = new System.Drawing.Point(1023, -1);
        this._Label8_0.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this._Label8_0.Name = "_Label8_0";
        this._Label8_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this._Label8_0.Size = new System.Drawing.Size(95, 27);
        this._Label8_0.TabIndex = 76;
        this._Label8_0.Text = "Label8";
        this._Label8_0.Visible = false;
        // 
        // lblSearch
        // 
        this.Label4.BackColor = System.Drawing.SystemColors.Control;
        this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
        this.Label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
        this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label4.Location = new System.Drawing.Point(8, 10);
        this.Label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.Label4.Name = "Label4";
        this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Label4.Size = new System.Drawing.Size(117, 18);
        this.Label4.TabIndex = 23;
        this.Label4.Text = "JJJJMMTT";
        this.Label4.Visible = false;
        // 
        // lblDisplayHint
        // 
        this.Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
        this.Label3.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Label3.Location = new System.Drawing.Point(0, 151);
        this.Label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.Label3.Name = "Label3";
        this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Label3.Size = new System.Drawing.Size(1057, 17);
        this.Label3.TabIndex = 22;
        this.Label3.Text = "Name,Vorname";
        // 
        // lblFamNr
        // 
        this.lblFamNr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this.lblFamNr.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblFamNr.ForeColor = System.Drawing.SystemColors.ControlText;
        this.lblFamNr.Location = new System.Drawing.Point(750, 129);
        this.lblFamNr.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.lblFamNr.Name = "lblFamNr";
        this.lblFamNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblFamNr.Size = new System.Drawing.Size(69, 22);
        this.lblFamNr.TabIndex = 7;
        // 
        // lblPersNr
        // 
        this.lblPersNr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this.lblPersNr.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblPersNr.ForeColor = System.Drawing.SystemColors.ControlText;
        this.lblPersNr.Location = new System.Drawing.Point(701, 122);
        this.lblPersNr.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
        this.lblPersNr.Name = "lblPersNr";
        this.lblPersNr.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblPersNr.Size = new System.Drawing.Size(69, 22);
        this.lblPersNr.TabIndex = 6;
        this.lblPersNr.Text = "<PersNr>";
        // 
        // lblPredicate
        // 
        this.lblPredicate.AutoSize = true;
        this.lblPredicate.Location = new System.Drawing.Point(295, 9);
        this.lblPredicate.Name = "lblPredicate";
        this.lblPredicate.Size = new System.Drawing.Size(94, 19);
        this.lblPredicate.TabIndex = 110;
        this.lblPredicate.Text = "lblPredicate";
        // 
        // lblNickName
        // 
        this.lblNickName.AutoSize = true;
        this.lblNickName.Location = new System.Drawing.Point(8, 9);
        this.lblNickName.Name = "lblNickName";
        this.lblNickName.Size = new System.Drawing.Size(96, 19);
        this.lblNickName.TabIndex = 111;
        this.lblNickName.Text = "lblNickname";
        // 
        // Text1
        // 
        this.Text1.AcceptsReturn = true;
        this.Text1.BackColor = System.Drawing.SystemColors.Window;
        this.Text1.Cursor = System.Windows.Forms.Cursors.IBeam;
        this.Text1.ForeColor = System.Drawing.SystemColors.WindowText;
        this.Text1.Location = new System.Drawing.Point(8, 30);
        this.Text1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.Text1.MaxLength = 0;
        this.Text1.Name = "Text1";
        this.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Text1.Size = new System.Drawing.Size(272, 27);
        this.Text1.TabIndex = 1;
        this.Text1.Visible = false;
        // 
        // Text2
        // 
        this.Text2.AcceptsReturn = true;
        this.Text2.BackColor = System.Drawing.SystemColors.Window;
        this.Text2.Cursor = System.Windows.Forms.Cursors.IBeam;
        this.Text2.ForeColor = System.Drawing.SystemColors.WindowText;
        this.Text2.Location = new System.Drawing.Point(465, 87);
        this.Text2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.Text2.MaxLength = 0;
        this.Text2.Name = "Text2";
        this.Text2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Text2.Size = new System.Drawing.Size(237, 27);
        this.Text2.TabIndex = 95;
        // 
        // Timer1
        // 
        this.Timer1.Enabled = true;
        this.Timer1.Interval = 10;
        // 
        // Frame3
        // 
        this.Frame3.BackColor = System.Drawing.SystemColors.Control;
        this.Frame3.Controls.Add(this.Label27);
        this.Frame3.Controls.Add(this.Label25);
        this.Frame3.Controls.Add(this.Label26);
        this.Frame3.Controls.Add(this.Label17);
        this.Frame3.Controls.Add(this.Label18);
        this.Frame3.Controls.Add(this.Label16);
        this.Frame3.Controls.Add(this.Label15);
        this.Frame3.Controls.Add(this.lblMarr8);
        this.Frame3.Controls.Add(this.lblMarr7);
        this.Frame3.Controls.Add(this._Label6_5);
        this.Frame3.Controls.Add(this._Label7_15);
        this.Frame3.Controls.Add(this._Label7_14);
        this.Frame3.Controls.Add(this._Label7_13);
        this.Frame3.Controls.Add(this._Label7_12);
        this.Frame3.Controls.Add(this._Label7_11);
        this.Frame3.Controls.Add(this._Label7_10);
        this.Frame3.Controls.Add(this._Label7_9);
        this.Frame3.Controls.Add(this._Label7_8);
        this.Frame3.Controls.Add(this._Label7_7);
        this.Frame3.Controls.Add(this._Label7_6);
        this.Frame3.Controls.Add(this._Label7_5);
        this.Frame3.Controls.Add(this._Label7_4);
        this.Frame3.Controls.Add(this._Label7_3);
        this.Frame3.Controls.Add(this._Label7_2);
        this.Frame3.Controls.Add(this._Label7_1);
        this.Frame3.Controls.Add(this._Label7_0);
        this.Frame3.Controls.Add(this._Line1_28);
        this.Frame3.Controls.Add(this._Line1_23);
        this.Frame3.Controls.Add(this._Label6_4);
        this.Frame3.Controls.Add(this._Label6_3);
        this.Frame3.Controls.Add(this.lblMarr2);
        this.Frame3.Controls.Add(this._Label6_2);
        this.Frame3.Controls.Add(this._Line1_27);
        this.Frame3.Controls.Add(this._Line1_26);
        this.Frame3.Controls.Add(this._Line1_25);
        this.Frame3.Controls.Add(this._Line1_24);
        this.Frame3.Controls.Add(this._Line1_22);
        this.Frame3.Controls.Add(this._Line1_21);
        this.Frame3.Controls.Add(this._Line1_20);
        this.Frame3.Controls.Add(this._Line1_19);
        this.Frame3.Controls.Add(this._Line1_17);
        this.Frame3.Controls.Add(this.lblMarr1);
        this.Frame3.Controls.Add(this._Line1_16);
        this.Frame3.Controls.Add(this._Line1_15);
        this.Frame3.Controls.Add(this._Line1_14);
        this.Frame3.Controls.Add(this._Line1_13);
        this.Frame3.Controls.Add(this._Line1_12);
        this.Frame3.Controls.Add(this._Line1_11);
        this.Frame3.Controls.Add(this._Line1_10);
        this.Frame3.Controls.Add(this._Line1_9);
        this.Frame3.Controls.Add(this._Line1_8);
        this.Frame3.Controls.Add(this._Label5_14);
        this.Frame3.Controls.Add(this._Label5_10);
        this.Frame3.Controls.Add(this._Label5_15);
        this.Frame3.Controls.Add(this._Line1_7);
        this.Frame3.Controls.Add(this._Line1_6);
        this.Frame3.Controls.Add(this._Line1_5);
        this.Frame3.Controls.Add(this._Line1_4);
        this.Frame3.Controls.Add(this._Line1_3);
        this.Frame3.Controls.Add(this._Line1_2);
        this.Frame3.Controls.Add(this._Line1_1);
        this.Frame3.Controls.Add(this._Label5_8);
        this.Frame3.Controls.Add(this._Label5_13);
        this.Frame3.Controls.Add(this._Label5_7);
        this.Frame3.Controls.Add(this._Label5_12);
        this.Frame3.Controls.Add(this._Label5_9);
        this.Frame3.Controls.Add(this._Label5_6);
        this.Frame3.Controls.Add(this._Label5_5);
        this.Frame3.Controls.Add(this._Label5_4);
        this.Frame3.Controls.Add(this._Label5_3);
        this.Frame3.Controls.Add(this._Label5_2);
        this.Frame3.Controls.Add(this._Label5_11);
        this.Frame3.Controls.Add(this._Label5_1);
        this.Frame3.Controls.Add(this._Label5_0);
        this.Frame3.Controls.Add(this._Line1_0);
        this.Frame3.Controls.Add(this.Label20);
        this.Frame3.Controls.Add(this.Label19);
        this.Frame3.Controls.Add(this.Label21);
        this.Frame3.Controls.Add(this.Label22);
        this.Frame3.Controls.Add(this.Label23);
        this.Frame3.Controls.Add(this.Label24);
        this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
        this.Frame3.Location = new System.Drawing.Point(0, 505);
        this.Frame3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.Frame3.Name = "Frame3";
        this.Frame3.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.Frame3.Size = new System.Drawing.Size(1081, 223);
        this.Frame3.TabIndex = 38;
        this.Frame3.TabStop = false;
        // 
        // CheckBox19
        // 
        this.CheckBox19.AutoSize = true;
        this.CheckBox19.BackColor = System.Drawing.SystemColors.Control;
        this.CheckBox19.Location = new System.Drawing.Point(704, 89);
        this.CheckBox19.Name = "CheckBox19";
        this.CheckBox19.Size = new System.Drawing.Size(190, 23);
        this.CheckBox19.TabIndex = 112;
        this.CheckBox19.Text = "Aliasnamen anzeigen";
        this.CheckBox19.UseVisualStyleBackColor = false;
        // 
        // List4
        // 
        this.List4.BackColor = System.Drawing.SystemColors.Window;
        this.List4.Cursor = System.Windows.Forms.Cursors.Default;
        this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List4.ItemHeight = 19;
        this.List4.Location = new System.Drawing.Point(347, 277);
        this.List4.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.List4.Name = "List4";
        this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.List4.Size = new System.Drawing.Size(95, 23);
        this.List4.Sorted = true;
        this.List4.TabIndex = 31;
        this.List4.Visible = false;
        // 
        // List2
        // 
        this.List2.BackColor = System.Drawing.SystemColors.Window;
        this.List2.Cursor = System.Windows.Forms.Cursors.Default;
        this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List2.ItemHeight = 19;
        this.List2.Location = new System.Drawing.Point(520, 432);
        this.List2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.List2.Name = "List2";
        this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.List2.Size = new System.Drawing.Size(195, 4);
        this.List2.Sorted = true;
        this.List2.TabIndex = 10;
        this.List2.Visible = false;
        // 
        // List1
        // 
        this.List1.BackColor = System.Drawing.SystemColors.Window;
        this.List1.CausesValidation = false;
        this.List1.Cursor = System.Windows.Forms.Cursors.Default;
        this.List1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List1.ItemHeight = 22;
        this.List1.Location = new System.Drawing.Point(78, 182);
        this.List1.Name = "List1";
        this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.List1.Size = new System.Drawing.Size(1015, 312);
        this.List1.TabIndex = 2;
        // 
        // lstUsageList
        // 
        this.ListBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
        this.ListBox1.FormattingEnabled = true;
        this.ListBox1.ItemHeight = 19;
        this.ListBox1.Location = new System.Drawing.Point(1, 62);
        this.ListBox1.Name = "ListBox1";
        this.ListBox1.Size = new System.Drawing.Size(81, 612);
        this.ListBox1.TabIndex = 108;
        this.ListBox1.Visible = false;
        // 
        // List3
        // 
        this.List3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
        this.List3.Cursor = System.Windows.Forms.Cursors.Default;
        this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
        this.List3.ItemHeight = 19;
        this.List3.Location = new System.Drawing.Point(401, 341);
        this.List3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.List3.Name = "List3";
        this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.List3.Size = new System.Drawing.Size(213, 42);
        this.List3.Sorted = true;
        this.List3.TabIndex = 114;
        this.List3.Visible = false;
        // 
        // fraNameSrchSelection1
        // 
        this.fraNameSrchSelection1.BackColor = System.Drawing.SystemColors.Control;
        this.fraNameSrchSelection1.ForeColor = System.Drawing.SystemColors.ControlText;
        this.fraNameSrchSelection1.Location = new System.Drawing.Point(835, 12);
        this.fraNameSrchSelection1.Name = "fraNameSrchSelection1";
        this.fraNameSrchSelection1.Size = new System.Drawing.Size(784, 483);
        this.fraNameSrchSelection1.TabIndex = 115;
        // 
        // Document
        // 
        this.fraPreview1.Location = new System.Drawing.Point(147, 74);
        this.fraPreview1.Name = "fraPreview1";
        this.fraPreview1.Size = new System.Drawing.Size(816, 629);
        this.fraPreview1.TabIndex = 116;
        // 
        // Namensuch
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
        this.CancelButton = this.btnReady;
        this.ClientSize = new System.Drawing.Size(1014, 724);
        this.ControlBox = false;
        this.Controls.Add(this.fraPreview1);
        this.Controls.Add(this.fraNameSrchSelection1);
        this.Controls.Add(this.List3);
        this.Controls.Add(this.ListBox1);
        this.Controls.Add(this.PictureBox1);
        this.Controls.Add(this.List7);
        this.Controls.Add(this.btnReady);
        this.Controls.Add(this.Frame3);
        this.Controls.Add(this.List4);
        this.Controls.Add(this.btnStartSearch);
        this.Controls.Add(this.List2);
        this.Controls.Add(this.btnFamilySheet);
        this.Controls.Add(this.btnPersonSheet);
        this.Controls.Add(this.btnClose);
        this.Controls.Add(this.chbMale);
        this.Controls.Add(this.chbFemales);
        this.Controls.Add(this.chbFamOnly);
        this.Controls.Add(this.chbSelection);
        this.Controls.Add(this.chbOmitSpouse);
        this.Controls.Add(this.Text2);
        this.Controls.Add(this.btnPrintList);
        this.Controls.Add(this.chbFemale2);
        this.Controls.Add(this.chbMale2);
        this.Controls.Add(this.Label10);
        this.Controls.Add(this.Label9);
        this.Controls.Add(this._Label8_7);
        this.Controls.Add(this._Label8_5);
        this.Controls.Add(this._Label8_4);
        this.Controls.Add(this._Label8_3);
        this.Controls.Add(this._Label8_2);
        this.Controls.Add(this._Label8_1);
        this.Controls.Add(this._Label8_0);
        this.Controls.Add(this.Label4);
        this.Controls.Add(this.Label3);
        this.Controls.Add(this.lblFamNr);
        this.Controls.Add(this.lblPersNr);
        this.Controls.Add(this.ComboBox1);
        this.Controls.Add(this.Text1);
        this.Controls.Add(this.lblPredicate);
        this.Controls.Add(this.lblNickName);
        this.Controls.Add(this.ComboBox2);
        this.Controls.Add(this.CheckBox19);
        this.Controls.Add(this.List1);
        this.Controls.Add(this.btnReqHint);
        this.Controls.Add(this._Label8_6);
        this.Controls.Add(this.btnRegisterSearch);
        this.Cursor = System.Windows.Forms.Cursors.Default;
        this.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.KeyPreview = true;
        this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
        this.Name = "Namensuch";
        this.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.Text = "Namensuche";
        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
        this.Frame3.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    public ListBox List7;
    public Button btnReady;
    public Timer Timer1;
    public Label lblMarr8;
    public Label lblMarr7;
    public Label _Label6_5;
    public Label _Label7_15;
    public Label _Label7_14;
    public Label _Label7_13;
    public Label _Label7_12;
    public Label _Label7_11;
    public Label _Label7_10;
    public Label _Label7_9;
    public Label _Label7_8;
    public Label _Label7_7;
    public Label _Label7_6;
    public Label _Label7_5;
    public Label _Label7_4;
    public Label _Label7_3;
    public Label _Label7_2;
    public Label _Label7_1;
    public Label _Label7_0;
    public Label _Line1_28;
    public Label _Line1_23;
    public Label _Label6_4;
    public Label _Label6_3;
    public Label lblMarr2;
    public Label _Label6_2;
    public Label _Line1_27;
    public Label _Line1_26;
    public Label _Line1_25;
    public Label _Line1_24;
    public Label _Line1_22;
    public Label _Line1_21;
    public Label _Line1_20;
    public Label _Line1_19;
    public Label _Line1_17;
    public Label lblMarr1;
    public Label _Line1_16;
    public Label _Line1_15;
    public Label _Line1_14;
    public Label _Line1_13;
    public Label _Line1_12;
    public Label _Line1_11;
    public Label _Line1_10;
    public Label _Line1_9;
    public Label _Line1_8;
    public Label _Label5_14;
    public Label _Label5_15;
    public Label _Line1_7;
    public Label _Line1_6;
    public Label _Line1_5;
    public Label _Line1_4;
    public Label _Line1_3;
    public Label _Line1_2;
    public Label _Line1_1;
    public Label _Label5_8;
    public Label _Label5_13;
    public Label _Label5_7;
    public Label _Label5_12;
    public Label _Label5_9;
    public Label _Label5_6;
    public Label _Label5_5;
    public Label _Label5_4;
    public Label _Label5_3;
    public Label _Label5_2;
    public Label _Label5_11;
    public Label _Label5_1;
    public Label _Label5_0;
    public Label _Line1_0;
    public GroupBox Frame3;
    public ListBox List4;
    public Button btnStartSearch;
    public ListBox List2;

    /*

                 */
    public TextBox Text1;
    [CommandBinding(nameof(INamenSuchViewModel.FamilySheetCommand))]
    public Button btnFamilySheet;
    [CommandBinding(nameof(INamenSuchViewModel.PersonSheetCommand))]
    public Button btnPersonSheet;
    [CommandBinding(nameof(INamenSuchViewModel.CloseCommand))]
    public Button btnClose;
    [CommandBinding(nameof(INamenSuchViewModel.PrintListCommand))]
    public Button btnPrintList;

    [CheckedBinding(nameof(INamenSuchViewModel.Male_Checked))]
    public CheckBox chbMale;
    [CheckedBinding(nameof(INamenSuchViewModel.Females_Checked))]
    public CheckBox chbFemales;
    [CheckedBinding(nameof(INamenSuchViewModel.FamOnly_Checked))]
    public CheckBox chbFamOnly;
    [CheckedBinding(nameof(INamenSuchViewModel.Selection_Checked))]
    public CheckBox chbSelection;
    [CheckedBinding(nameof(INamenSuchViewModel.Female2_Checked))]
    public CheckBox chbFemale2;
    [CheckedBinding(nameof(INamenSuchViewModel.Male2_Checked))]
    public CheckBox chbMale2;
    [CheckedBinding(nameof(INamenSuchViewModel.OmitSpouse_Checked))]
    public CheckBox chbOmitSpouse;

    public ListBox List1;

    public TextBox Text2;
    public Label Label10;
    public Label Label9;
    public Label _Label8_7;
    public Label _Label8_6;
    public Label _Label8_5;
    public Label _Label8_4;
    public Label _Label8_3;
    public Label _Label8_2;
    public Label _Label8_1;
    public Label _Label8_0;
    public Label Label4;
    public Label Label3;
    public Label lblFamNr;
    public Label lblPersNr;

    public ControlArray<Label> Label1;
    public ControlArray<Label> Label5;
    public ControlArray<Label> Label6;
    public ControlArray<Label> Label7;
    public ControlArray<Label> Label8;
    public ControlArray<Label> Line1;


    [CommandBinding(nameof(INamenSuchViewModel.ReqHintCommand))]
    public Button btnReqHint;
    public Button btnRegisterSearch;
    public ComboBox ComboBox1;

    public ListBox ListBox1;
    internal ComboBox ComboBox2;


    internal Label lblPredicate;
    internal Label lblNickName;
    internal Label Label16;
    internal Label Label15;
    internal Label Label17;
    internal Label Label18;
    internal Label Label25;
    internal Label Label26;
    internal Label Label20;
    internal Label Label19;
    internal Label Label21;
    internal Label Label22;
    internal Label Label23;
    internal Label Label24;
    internal Label Label27;
    protected Label _Label5_10;
    internal CheckBox CheckBox19;
    internal OpenFileDialog OpenFileDialog1;
    internal PictureBox PictureBox1;
    public ListBox List3;
    public FraNameSrchSelection fraNameSrchSelection1;
    public FraPreview fraPreview1;
}