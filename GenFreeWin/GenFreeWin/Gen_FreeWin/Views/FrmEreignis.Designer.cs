using Gen_FreeWin.Main;
using GenFree;
using GenFree.Data;
using GenFree.Helper;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class FrmEreignis
{

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {
        try
        {

        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.Button1 = new System.Windows.Forms.Button();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.Button4 = new System.Windows.Forms.Button();
            this.Button5 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.Button7 = new System.Windows.Forms.Button();
            this.Button8 = new System.Windows.Forms.Button();
            this.Button9 = new System.Windows.Forms.Button();
            this.Button10 = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.TextBox3 = new System.Windows.Forms.TextBox();
            this.TextBox4 = new System.Windows.Forms.TextBox();
            this.TextBox5 = new System.Windows.Forms.TextBox();
            this.TextBox6 = new System.Windows.Forms.TextBox();
            this.TextBox7 = new System.Windows.Forms.TextBox();
            this.TextBox8 = new System.Windows.Forms.TextBox();
            this.TextBox9 = new System.Windows.Forms.TextBox();
            this.TextBox10 = new System.Windows.Forms.TextBox();
            this.TextBox11 = new System.Windows.Forms.TextBox();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.RichTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.Button15 = new System.Windows.Forms.Button();
            this.TextBox15 = new System.Windows.Forms.TextBox();
            this.TextBox14 = new System.Windows.Forms.TextBox();
            this.TextBox13 = new System.Windows.Forms.TextBox();
            this.TextBox12 = new System.Windows.Forms.TextBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.ListBox2 = new System.Windows.Forms.ListBox();
            this.ListBox3 = new System.Windows.Forms.ListBox();
            this.Button11 = new System.Windows.Forms.Button();
            this.Label13 = new System.Windows.Forms.Label();
            this.Button12 = new System.Windows.Forms.Button();
            this.Label8 = new System.Windows.Forms.Label();
            this.Check1 = new System.Windows.Forms.CheckBox();
            this.Button13 = new System.Windows.Forms.Button();
            this.Button14 = new System.Windows.Forms.Button();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.TextBox16 = new System.Windows.Forms.TextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.TextBox17 = new System.Windows.Forms.TextBox();
            this.TextBox18 = new System.Windows.Forms.TextBox();
            this.Label22 = new System.Windows.Forms.Label();
            this.TextBox19 = new System.Windows.Forms.TextBox();
            this.Button16 = new System.Windows.Forms.Button();
            this.Box1 = new System.Windows.Forms.GroupBox();
            this.RadioButton3 = new System.Windows.Forms.RadioButton();
            this.RadioButton2 = new System.Windows.Forms.RadioButton();
            this.RadioButton1 = new System.Windows.Forms.RadioButton();
            this.ListBox4 = new System.Windows.Forms.ListBox();
            this.edtGrabNr = new System.Windows.Forms.TextBox();
            this.Button17 = new System.Windows.Forms.Button();
            this.Label23 = new System.Windows.Forms.Label();
            this.Button18 = new System.Windows.Forms.Button();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.Frame1.SuspendLayout();
            this.Box1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReqHint
            // 
            this.Button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button1.Location = new System.Drawing.Point(813, 176);
            this.Button1.Name = "btnNext";
            this.Button1.Size = new System.Drawing.Size(137, 24);
            this.Button1.TabIndex = 0;
            this.Button1.Text = "&Quellen";
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // btnRegisterSearch
            // 
            this.Button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button2.Location = new System.Drawing.Point(772, 145);
            this.Button2.Name = "btnPrev";
            this.Button2.Size = new System.Drawing.Size(69, 24);
            this.Button2.TabIndex = 1;
            this.Button2.Text = "&Zeugen";
            this.Button2.UseVisualStyleBackColor = false;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // btnDuplBttn3
            // 
            this.Button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button3.Location = new System.Drawing.Point(586, 412);
            this.Button3.Name = "btnShowPlaceGE";
            this.Button3.Size = new System.Drawing.Size(101, 26);
            this.Button3.TabIndex = 2;
            this.Button3.Text = "Z";
            this.Button3.UseVisualStyleBackColor = false;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // btnMoveToCause
            // 
            this.Button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button4.Location = new System.Drawing.Point(8, 156);
            this.Button4.Name = "btnShowPlaceGM";
            this.Button4.Size = new System.Drawing.Size(88, 24);
            this.Button4.TabIndex = 3;
            this.Button4.Text = "abbrechen";
            this.Button4.UseVisualStyleBackColor = false;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // btnChangeSexToM
            // 
            this.Button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button5.Location = new System.Drawing.Point(693, 412);
            this.Button5.Name = "btnLinkGOV";
            this.Button5.Size = new System.Drawing.Size(101, 26);
            this.Button5.TabIndex = 4;
            this.Button5.Text = "&Fertig";
            this.Button5.UseVisualStyleBackColor = false;
            this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // btnChangeSexToF
            // 
            this.Button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button6.Location = new System.Drawing.Point(375, 414);
            this.Button6.Name = "btnSearchGOV";
            this.Button6.Size = new System.Drawing.Size(101, 26);
            this.Button6.TabIndex = 5;
            this.Button6.Text = "Löschen";
            this.Button6.UseVisualStyleBackColor = false;
            this.Button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // btnMoveToChurchCemet
            // 
            this.Button7.Location = new System.Drawing.Point(26, 389);
            this.Button7.Name = "btnConvertKoords";
            this.Button7.Size = new System.Drawing.Size(155, 24);
            this.Button7.TabIndex = 6;
            this.Button7.Text = "Texteingabe beenden";
            this.Button7.UseVisualStyleBackColor = true;
            this.Button7.Visible = false;
            this.Button7.Click += new System.EventHandler(this.Button7_Click);
            // 
            // btnMoveToEntityAnot
            // 
            this.Button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button8.Location = new System.Drawing.Point(813, 206);
            this.Button8.Name = "btnSearchName";
            this.Button8.Size = new System.Drawing.Size(137, 23);
            this.Button8.TabIndex = 7;
            this.Button8.Text = "Geburt berechnen";
            this.Button8.UseVisualStyleBackColor = false;
            this.Button8.Visible = false;
            this.Button8.Click += new System.EventHandler(this.Button8_Click);
            // 
            // btnMoveToLowerDateAnot
            // 
            this.Button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button9.Enabled = false;
            this.Button9.Location = new System.Drawing.Point(11, 126);
            this.Button9.Name = "btnSearchNumber";
            this.Button9.Size = new System.Drawing.Size(85, 24);
            this.Button9.TabIndex = 8;
            this.Button9.Text = "Rechnen";
            this.Button9.UseVisualStyleBackColor = false;
            this.Button9.Click += new System.EventHandler(this.Button9_Click);
            // 
            // btnDeleteEntry
            // 
            this.Button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button10.Enabled = false;
            this.Button10.Location = new System.Drawing.Point(105, 126);
            this.Button10.Name = "Button10";
            this.Button10.Size = new System.Drawing.Size(95, 24);
            this.Button10.TabIndex = 9;
            this.Button10.Text = "übernehmen";
            this.Button10.UseVisualStyleBackColor = false;
            this.Button10.Click += new System.EventHandler(this.Button10_Click);
            // 
            // lblEnterLicence
            // 
            this.Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label1.Location = new System.Drawing.Point(0, 26);
            this.Label1.Name = "lblRepoName";
            this.Label1.Size = new System.Drawing.Size(147, 18);
            this.Label1.TabIndex = 10;
            this.Label1.Text = "Art des Ereignisses:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblState
            // 
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label2.Location = new System.Drawing.Point(0, 46);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(147, 17);
            this.Label2.TabIndex = 11;
            this.Label2.Text = "Datum:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDisplayHint
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(254, 172);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(209, 18);
            this.Label3.TabIndex = 12;
            this.Label3.Text = "Obere Ereignisbemerkungen";
            this.Label3.Click += new System.EventHandler(this.Label3_Click);
            // 
            // lblSearch
            // 
            this.Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label4.Location = new System.Drawing.Point(0, 86);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(147, 17);
            this.Label4.TabIndex = 13;
            this.Label4.Text = "Text:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSorting
            // 
            this.Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label5.Location = new System.Drawing.Point(0, 106);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(147, 17);
            this.Label5.TabIndex = 14;
            this.Label5.Text = "Ort:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblEMail
            // 
            this.Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label6.Location = new System.Drawing.Point(0, 126);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(147, 18);
            this.Label6.TabIndex = 15;
            this.Label6.Text = "Kirche/Friedhof/etc.:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblURL
            // 
            this.Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Label7.Location = new System.Drawing.Point(6, 98);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(209, 23);
            this.Label7.TabIndex = 16;
            // 
            // Label11
            // 
            this.Label11.Location = new System.Drawing.Point(0, 443);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(982, 21);
            this.Label11.TabIndex = 20;
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Location = new System.Drawing.Point(254, 296);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(208, 18);
            this.Label12.TabIndex = 21;
            this.Label12.Text = "untere Ereignisbemerkungen";
            this.Label12.Click += new System.EventHandler(this.Label12_Click);
            // 
            // lblNickName
            // 
            this.Label14.BackColor = System.Drawing.Color.White;
            this.Label14.Location = new System.Drawing.Point(810, 290);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(113, 17);
            this.Label14.TabIndex = 23;
            this.Label14.Text = "Ausgabezusatz:";
            // 
            // Label15
            // 
            this.Label15.BackColor = System.Drawing.Color.White;
            this.Label15.Location = new System.Drawing.Point(254, 46);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(15, 18);
            this.Label15.TabIndex = 24;
            this.Label15.Text = "/";
            // 
            // edtPredicate
            // 
            this.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox1.Location = new System.Drawing.Point(153, 26);
            this.TextBox1.Name = "edtPlace";
            this.TextBox1.Size = new System.Drawing.Size(423, 19);
            this.TextBox1.TabIndex = 25;
            this.TextBox1.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtSuburb
            // 
            this.TextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox2.Location = new System.Drawing.Point(153, 46);
            this.TextBox2.Name = "edtSuburb";
            this.TextBox2.Size = new System.Drawing.Size(77, 19);
            this.TextBox2.TabIndex = 26;
            this.TextBox2.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            this.TextBox2.LostFocus += new System.EventHandler(this.TextBox2_LostFocus);
            // 
            // edtCounty
            // 
            this.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox3.Location = new System.Drawing.Point(233, 46);
            this.TextBox3.Name = "edtCounty";
            this.TextBox3.Size = new System.Drawing.Size(20, 19);
            this.TextBox3.TabIndex = 27;
            this.TextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBox3.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtCountry
            // 
            this.TextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox4.Location = new System.Drawing.Point(153, 86);
            this.TextBox4.Name = "edtCountry";
            this.TextBox4.Size = new System.Drawing.Size(423, 19);
            this.TextBox4.TabIndex = 30;
            this.TextBox4.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox4.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtState
            // 
            this.TextBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox5.Location = new System.Drawing.Point(153, 106);
            this.TextBox5.Name = "edtState";
            this.TextBox5.Size = new System.Drawing.Size(423, 19);
            this.TextBox5.TabIndex = 32;
            this.TextBox5.GotFocus += new System.EventHandler(this.TextBox5_GotFocus);
            this.TextBox5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox5.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtLocator
            // 
            this.TextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox6.Location = new System.Drawing.Point(153, 126);
            this.TextBox6.Name = "edtLocator";
            this.TextBox6.Size = new System.Drawing.Size(423, 19);
            this.TextBox6.TabIndex = 34;
            this.TextBox6.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox6.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox6.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtLat1
            // 
            this.TextBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox7.Location = new System.Drawing.Point(929, 290);
            this.TextBox7.Name = "edtLat1";
            this.TextBox7.Size = new System.Drawing.Size(69, 19);
            this.TextBox7.TabIndex = 39;
            this.TextBox7.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox7.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox7.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox7.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtLong1
            // 
            this.TextBox8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox8.Location = new System.Drawing.Point(813, 266);
            this.TextBox8.Name = "edtLong1";
            this.TextBox8.Size = new System.Drawing.Size(148, 19);
            this.TextBox8.TabIndex = 37;
            this.TextBox8.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox8.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox8.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtGOV
            // 
            this.TextBox9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox9.Location = new System.Drawing.Point(270, 46);
            this.TextBox9.Name = "edtGOV";
            this.TextBox9.Size = new System.Drawing.Size(77, 19);
            this.TextBox9.TabIndex = 28;
            this.TextBox9.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox9.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox9.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox9.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            this.TextBox9.LostFocus += new System.EventHandler(this.TextBox2_LostFocus);
            // 
            // edtLat2
            // 
            this.TextBox10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox10.Location = new System.Drawing.Point(349, 46);
            this.TextBox10.Name = "edtLat2";
            this.TextBox10.Size = new System.Drawing.Size(20, 19);
            this.TextBox10.TabIndex = 29;
            this.TextBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBox10.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox10.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox10.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // edtLat3
            // 
            this.TextBox11.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox11.Location = new System.Drawing.Point(582, 106);
            this.TextBox11.Name = "edtLat3";
            this.TextBox11.Size = new System.Drawing.Size(20, 19);
            this.TextBox11.TabIndex = 33;
            this.TextBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TextBox11.GotFocus += new System.EventHandler(this.TextBox1_GotFocus);
            this.TextBox11.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox2_KeyDown);
            this.TextBox11.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox11_KeyPress);
            this.TextBox11.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // RichTextBox1
            // 
            this.RichTextBox1.Location = new System.Drawing.Point(3, 197);
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.Size = new System.Drawing.Size(791, 96);
            this.RichTextBox1.TabIndex = 38;
            this.RichTextBox1.Text = "";
            this.RichTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox1_KeyDown);
            this.RichTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RichTextBox1_KeyUp);
            // 
            // RichTextBox2
            // 
            this.RichTextBox2.Location = new System.Drawing.Point(3, 316);
            this.RichTextBox2.Name = "RichTextBox2";
            this.RichTextBox2.Size = new System.Drawing.Size(791, 90);
            this.RichTextBox2.TabIndex = 41;
            this.RichTextBox2.Text = "";
            this.RichTextBox2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RichTextBox2_KeyDown);
            this.RichTextBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RichTextBox2_KeyUp);
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.Color.Silver;
            this.Frame1.Controls.Add(this.Button15);
            this.Frame1.Controls.Add(this.TextBox15);
            this.Frame1.Controls.Add(this.TextBox14);
            this.Frame1.Controls.Add(this.TextBox13);
            this.Frame1.Controls.Add(this.TextBox12);
            this.Frame1.Controls.Add(this.Label16);
            this.Frame1.Controls.Add(this.Label9);
            this.Frame1.Controls.Add(this.Label7);
            this.Frame1.Controls.Add(this.Button9);
            this.Frame1.Controls.Add(this.Button10);
            this.Frame1.Controls.Add(this.Button4);
            this.Frame1.Controls.Add(this.Label17);
            this.Frame1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame1.Location = new System.Drawing.Point(513, 197);
            this.Frame1.Name = "Frame1";
            this.Frame1.Size = new System.Drawing.Size(237, 197);
            this.Frame1.TabIndex = 42;
            this.Frame1.TabStop = false;
            this.Frame1.Text = "Geburtsdatum errechen";
            this.Frame1.Visible = false;
            // 
            // Button15
            // 
            this.Button15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button15.Location = new System.Drawing.Point(105, 153);
            this.Button15.Name = "Button15";
            this.Button15.Size = new System.Drawing.Size(95, 24);
            this.Button15.TabIndex = 50;
            this.Button15.Text = "kopieren";
            this.Button15.UseVisualStyleBackColor = false;
            this.Button15.Click += new System.EventHandler(this.Button15_Click);
            // 
            // edtAdditional
            // 
            this.TextBox15.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox15.Location = new System.Drawing.Point(177, 57);
            this.TextBox15.Name = "edtAdditional";
            this.TextBox15.Size = new System.Drawing.Size(38, 27);
            this.TextBox15.TabIndex = 49;
            this.TextBox15.TextChanged += new System.EventHandler(this.TextBox12_TextChanged);
            this.TextBox15.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox12_KeyPress);
            // 
            // edtZIP
            // 
            this.TextBox14.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox14.Location = new System.Drawing.Point(124, 57);
            this.TextBox14.Name = "edtZIP";
            this.TextBox14.Size = new System.Drawing.Size(38, 27);
            this.TextBox14.TabIndex = 48;
            this.TextBox14.TextChanged += new System.EventHandler(this.TextBox12_TextChanged);
            this.TextBox14.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox12_KeyPress);
            // 
            // edtLong3
            // 
            this.TextBox13.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox13.Location = new System.Drawing.Point(61, 57);
            this.TextBox13.Name = "edtLong3";
            this.TextBox13.Size = new System.Drawing.Size(38, 27);
            this.TextBox13.TabIndex = 47;
            this.TextBox13.TextChanged += new System.EventHandler(this.TextBox12_TextChanged);
            this.TextBox13.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox12_KeyPress);
            // 
            // edtLong2
            // 
            this.TextBox12.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox12.Location = new System.Drawing.Point(6, 57);
            this.TextBox12.Name = "edtLong2";
            this.TextBox12.Size = new System.Drawing.Size(38, 27);
            this.TextBox12.TabIndex = 46;
            this.TextBox12.TextChanged += new System.EventHandler(this.TextBox12_TextChanged);
            this.TextBox12.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox12_KeyPress);
            // 
            // Label16
            // 
            this.Label16.Location = new System.Drawing.Point(21, 20);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(194, 17);
            this.Label16.TabIndex = 44;
            this.Label16.Text = "bekanntes Alter";
            // 
            // lblOccubation
            // 
            this.Label9.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(6, 37);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(209, 17);
            this.Label9.TabIndex = 36;
            this.Label9.Text = "Jahre   Monate (Wochen) Tage";
            // 
            // Label17
            // 
            this.Label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Label17.Location = new System.Drawing.Point(8, 153);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(207, 23);
            this.Label17.TabIndex = 45;
            this.Label17.Visible = false;
            // 
            // Label10
            // 
            this.Label10.BackColor = System.Drawing.Color.Yellow;
            this.Label10.Location = new System.Drawing.Point(0, 0);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(1021, 24);
            this.Label10.TabIndex = 43;
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lstUsageList
            // 
            this.ListBox1.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 20;
            this.ListBox1.Location = new System.Drawing.Point(311, 170);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(301, 224);
            this.ListBox1.TabIndex = 44;
            this.ListBox1.Visible = false;
            this.ListBox1.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            // 
            // ListBox2
            // 
            this.ListBox2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBox2.FormattingEnabled = true;
            this.ListBox2.ItemHeight = 19;
            this.ListBox2.Location = new System.Drawing.Point(3, 169);
            this.ListBox2.Name = "ListBox2";
            this.ListBox2.Size = new System.Drawing.Size(301, 270);
            this.ListBox2.TabIndex = 45;
            this.ListBox2.DoubleClick += new System.EventHandler(this.ListBox2_DoubleClick);
            // 
            // ListBox3
            // 
            this.ListBox3.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBox3.FormattingEnabled = true;
            this.ListBox3.ItemHeight = 20;
            this.ListBox3.Location = new System.Drawing.Point(772, 28);
            this.ListBox3.Name = "ListBox3";
            this.ListBox3.Size = new System.Drawing.Size(238, 104);
            this.ListBox3.TabIndex = 46;
            this.ListBox3.Visible = false;
            this.ListBox3.DoubleClick += new System.EventHandler(this.ListBox3_DoubleClick);
            // 
            // btnMoveToDateAnot
            // 
            this.Button11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button11.Location = new System.Drawing.Point(615, 170);
            this.Button11.Name = "Button11";
            this.Button11.Size = new System.Drawing.Size(179, 24);
            this.Button11.TabIndex = 47;
            this.Button11.Text = "&Kinddatum übernehmen";
            this.Button11.UseVisualStyleBackColor = false;
            this.Button11.Visible = false;
            this.Button11.Click += new System.EventHandler(this.Button11_Click);
            // 
            // lblPredicate
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(923, 446);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(91, 18);
            this.Label13.TabIndex = 48;
            this.Label13.Text = "lblPredicate";
            this.Label13.Visible = false;
            // 
            // Button12
            // 
            this.Button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button12.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button12.Location = new System.Drawing.Point(813, 236);
            this.Button12.Name = "Button12";
            this.Button12.Size = new System.Drawing.Size(137, 24);
            this.Button12.TabIndex = 49;
            this.Button12.Text = "&Urkunden-Nr.";
            this.Button12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Button12.UseVisualStyleBackColor = false;
            this.Button12.Click += new System.EventHandler(this.Button12_Click);
            // 
            // lblResidence
            // 
            this.Label8.Location = new System.Drawing.Point(582, 53);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(184, 33);
            this.Label8.TabIndex = 50;
            // 
            // Check1
            // 
            this.Check1.Location = new System.Drawing.Point(375, 46);
            this.Check1.Name = "Check1";
            this.Check1.Size = new System.Drawing.Size(182, 20);
            this.Check1.TabIndex = 51;
            this.Check1.Text = "Check1";
            this.Check1.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.Check1.UseVisualStyleBackColor = true;
            this.Check1.Visible = false;
            // 
            // Button13
            // 
            this.Button13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button13.Location = new System.Drawing.Point(629, 106);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(137, 24);
            this.Button13.TabIndex = 52;
            this.Button13.Text = "&neuer Ort";
            this.Button13.UseVisualStyleBackColor = false;
            this.Button13.Click += new System.EventHandler(this.Button13_Click);
            // 
            // Button14
            // 
            this.Button14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button14.Location = new System.Drawing.Point(582, 26);
            this.Button14.Name = "Button14";
            this.Button14.Size = new System.Drawing.Size(184, 24);
            this.Button14.TabIndex = 53;
            this.Button14.Text = "&Letzten Ort wiederholen";
            this.Button14.UseVisualStyleBackColor = false;
            this.Button14.Click += new System.EventHandler(this.Button14_Click_1);
            // 
            // Label18
            // 
            this.Label18.BackColor = System.Drawing.Color.Red;
            this.Label18.Location = new System.Drawing.Point(638, 365);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(84, 29);
            this.Label18.TabIndex = 54;
            this.Label18.Text = "Label18";
            this.Label18.Visible = false;
            // 
            // Label19
            // 
            this.Label19.BackColor = System.Drawing.Color.Red;
            this.Label19.Location = new System.Drawing.Point(942, 385);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(68, 37);
            this.Label19.TabIndex = 55;
            this.Label19.Visible = false;
            // 
            // CheckBox1
            // 
            this.CheckBox1.Location = new System.Drawing.Point(375, 46);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(150, 18);
            this.CheckBox1.TabIndex = 56;
            this.CheckBox1.Text = "Datum vor Chr.";
            this.CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Label20
            // 
            this.Label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label20.Location = new System.Drawing.Point(0, 66);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(147, 17);
            this.Label20.TabIndex = 57;
            this.Label20.Text = "Datumsphrase:";
            this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // edtPolName
            // 
            this.TextBox16.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox16.Location = new System.Drawing.Point(153, 66);
            this.TextBox16.Name = "edtPolName";
            this.TextBox16.Size = new System.Drawing.Size(216, 19);
            this.TextBox16.TabIndex = 58;
            this.TextBox16.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox16.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox16.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // Label21
            // 
            this.Label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label21.Location = new System.Drawing.Point(0, 146);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(147, 18);
            this.Label21.TabIndex = 59;
            this.Label21.Text = "Todesursache:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label21.Visible = false;
            // 
            // TextBox17
            // 
            this.TextBox17.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox17.Location = new System.Drawing.Point(153, 146);
            this.TextBox17.Name = "TextBox17";
            this.TextBox17.Size = new System.Drawing.Size(423, 19);
            this.TextBox17.TabIndex = 35;
            this.TextBox17.Visible = false;
            this.TextBox17.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox17.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox17.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp1);
            // 
            // TextBox18
            // 
            this.TextBox18.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox18.Enabled = false;
            this.TextBox18.Location = new System.Drawing.Point(615, 86);
            this.TextBox18.Name = "TextBox18";
            this.TextBox18.Size = new System.Drawing.Size(151, 19);
            this.TextBox18.TabIndex = 31;
            this.TextBox18.Visible = false;
            this.TextBox18.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox17_KeyUp);
            // 
            // Label22
            // 
            this.Label22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label22.Location = new System.Drawing.Point(579, 86);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(33, 18);
            this.Label22.TabIndex = 62;
            this.Label22.Text = "Nr:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label22.Visible = false;
            // 
            // TextBox19
            // 
            this.TextBox19.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox19.Location = new System.Drawing.Point(586, 146);
            this.TextBox19.Name = "TextBox19";
            this.TextBox19.Size = new System.Drawing.Size(180, 19);
            this.TextBox19.TabIndex = 36;
            this.TextBox19.Visible = false;
            // 
            // Button16
            // 
            this.Button16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button16.Location = new System.Drawing.Point(12, 414);
            this.Button16.Name = "Button16";
            this.Button16.Size = new System.Drawing.Size(313, 26);
            this.Button16.TabIndex = 63;
            this.Button16.Text = "Texteintrag zur Todesursache verschieben";
            this.Button16.UseVisualStyleBackColor = false;
            this.Button16.Visible = false;
            this.Button16.Click += new System.EventHandler(this.Button16_Click);
            // 
            // Box1
            // 
            this.Box1.Controls.Add(this.RadioButton3);
            this.Box1.Controls.Add(this.RadioButton2);
            this.Box1.Controls.Add(this.RadioButton1);
            this.Box1.Location = new System.Drawing.Point(807, 323);
            this.Box1.Name = "Box1";
            this.Box1.Size = new System.Drawing.Size(185, 115);
            this.Box1.TabIndex = 64;
            this.Box1.TabStop = false;
            this.Box1.Text = "Vertraulichkeit der Information";
            // 
            // RadioButton3
            // 
            this.RadioButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.RadioButton3.Checked = true;
            this.RadioButton3.Location = new System.Drawing.Point(6, 87);
            this.RadioButton3.Name = "RadioButton3";
            this.RadioButton3.Size = new System.Drawing.Size(90, 22);
            this.RadioButton3.TabIndex = 2;
            this.RadioButton3.TabStop = true;
            this.RadioButton3.Text = "frei";
            this.RadioButton3.UseVisualStyleBackColor = false;
            // 
            // RadioButton2
            // 
            this.RadioButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.RadioButton2.Location = new System.Drawing.Point(6, 63);
            this.RadioButton2.Name = "RadioButton2";
            this.RadioButton2.Size = new System.Drawing.Size(90, 22);
            this.RadioButton2.TabIndex = 1;
            this.RadioButton2.Text = "privat";
            this.RadioButton2.UseVisualStyleBackColor = false;
            // 
            // RadioButton1
            // 
            this.RadioButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.RadioButton1.Location = new System.Drawing.Point(6, 39);
            this.RadioButton1.Name = "RadioButton1";
            this.RadioButton1.Size = new System.Drawing.Size(90, 22);
            this.RadioButton1.TabIndex = 0;
            this.RadioButton1.Text = "gesperrt";
            this.RadioButton1.UseVisualStyleBackColor = false;
            // 
            // ListBox4
            // 
            this.ListBox4.BackColor = System.Drawing.Color.White;
            this.ListBox4.Font = new System.Drawing.Font("Courier New", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBox4.FormattingEnabled = true;
            this.ListBox4.ItemHeight = 20;
            this.ListBox4.Location = new System.Drawing.Point(615, 172);
            this.ListBox4.Name = "ListBox4";
            this.ListBox4.Size = new System.Drawing.Size(355, 224);
            this.ListBox4.TabIndex = 65;
            this.ListBox4.Visible = false;
            this.ListBox4.DoubleClick += new System.EventHandler(this.ListBox4_DoubleClick);
            // 
            // edtGrabNr
            // 
            this.edtGrabNr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtGrabNr.Location = new System.Drawing.Point(654, 126);
            this.edtGrabNr.Name = "TextBox20";
            this.edtGrabNr.Size = new System.Drawing.Size(96, 19);
            this.edtGrabNr.TabIndex = 66;
            this.edtGrabNr.Visible = false;
            // 
            // Button17
            // 
            this.Button17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button17.Location = new System.Drawing.Point(761, 120);
            this.Button17.Name = "Button17";
            this.Button17.Size = new System.Drawing.Size(46, 24);
            this.Button17.TabIndex = 67;
            this.Button17.Text = "http";
            this.Button17.UseVisualStyleBackColor = false;
            this.Button17.Visible = false;
            this.Button17.Click += new System.EventHandler(this.Button17_Click);
            // 
            // Label23
            // 
            this.Label23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label23.Location = new System.Drawing.Point(579, 126);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(69, 18);
            this.Label23.TabIndex = 68;
            this.Label23.Text = "Grab-Nr.";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Label23.Visible = false;
            // 
            // Button18
            // 
            this.Button18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Button18.Location = new System.Drawing.Point(847, 145);
            this.Button18.Name = "Button18";
            this.Button18.Size = new System.Drawing.Size(151, 24);
            this.Button18.TabIndex = 69;
            this.Button18.Text = "Beteiligte Personen";
            this.Button18.UseVisualStyleBackColor = false;
            this.Button18.Visible = false;
            // 
            // CheckBox2
            // 
            this.CheckBox2.AutoSize = true;
            this.CheckBox2.Location = new System.Drawing.Point(375, 65);
            this.CheckBox2.Name = "CheckBox2";
            this.CheckBox2.Size = new System.Drawing.Size(107, 22);
            this.CheckBox2.TabIndex = 70;
            this.CheckBox2.Text = "verstorben";
            this.CheckBox2.UseVisualStyleBackColor = true;
            this.CheckBox2.Visible = false;
            // 
            // FrmEreignis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(1016, 472);
            this.ControlBox = false;
            this.Controls.Add(this.Button18);
            this.Controls.Add(this.Label23);
            this.Controls.Add(this.Button17);
            this.Controls.Add(this.ListBox4);
            this.Controls.Add(this.edtGrabNr);
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.Box1);
            this.Controls.Add(this.Label19);
            this.Controls.Add(this.Label18);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Button12);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.ListBox2);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.RichTextBox2);
            this.Controls.Add(this.RichTextBox1);
            this.Controls.Add(this.TextBox11);
            this.Controls.Add(this.TextBox10);
            this.Controls.Add(this.TextBox9);
            this.Controls.Add(this.TextBox8);
            this.Controls.Add(this.TextBox7);
            this.Controls.Add(this.TextBox6);
            this.Controls.Add(this.TextBox5);
            this.Controls.Add(this.TextBox4);
            this.Controls.Add(this.TextBox3);
            this.Controls.Add(this.TextBox2);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.Label15);
            this.Controls.Add(this.Label14);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Button8);
            this.Controls.Add(this.Button7);
            this.Controls.Add(this.Button6);
            this.Controls.Add(this.Button5);
            this.Controls.Add(this.Button3);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.Button13);
            this.Controls.Add(this.ListBox3);
            this.Controls.Add(this.Check1);
            this.Controls.Add(this.Label21);
            this.Controls.Add(this.CheckBox1);
            this.Controls.Add(this.Button16);
            this.Controls.Add(this.TextBox17);
            this.Controls.Add(this.TextBox19);
            this.Controls.Add(this.Label22);
            this.Controls.Add(this.TextBox18);
            this.Controls.Add(this.Button14);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Button11);
            this.Controls.Add(this.CheckBox2);
            this.Controls.Add(this.Label20);
            this.Controls.Add(this.TextBox16);
            this.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmEreignis";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " ";
            this.Frame1.ResumeLayout(false);
            this.Frame1.PerformLayout();
            this.Box1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    private static List<WeakReference> __ENCList = new();

    [AccessedThroughProperty(nameof(Button1))]
    private Button _Button1;

    [AccessedThroughProperty(nameof(Button2))]
    private Button _Button2;

    [AccessedThroughProperty(nameof(Button3))]
    private Button _Button3;

    [AccessedThroughProperty(nameof(Button4))]
    private Button _Button4;

    [AccessedThroughProperty(nameof(Button5))]
    private Button _Button5;

    [AccessedThroughProperty(nameof(Button6))]
    private Button _Button6;

    [AccessedThroughProperty(nameof(Button7))]
    private Button _Button7;

    [AccessedThroughProperty(nameof(Button8))]
    private Button _Button8;

    [AccessedThroughProperty(nameof(Button9))]
    private Button _Button9;

    [AccessedThroughProperty(nameof(Button10))]
    private Button _Button10;

    [AccessedThroughProperty(nameof(Label3))]
    private Label _Label3;

    [AccessedThroughProperty(nameof(Label12))]
    private Label _Label12;

    [AccessedThroughProperty(nameof(TextBox1))]
    private TextBox _TextBox1;

    [AccessedThroughProperty(nameof(TextBox2))]
    private TextBox _TextBox2;

    [AccessedThroughProperty(nameof(TextBox3))]
    private TextBox _TextBox3;

    [AccessedThroughProperty(nameof(TextBox4))]
    private TextBox _TextBox4;

    [AccessedThroughProperty(nameof(TextBox5))]
    private TextBox _TextBox5;

    [AccessedThroughProperty(nameof(TextBox6))]
    private TextBox _TextBox6;

    [AccessedThroughProperty(nameof(TextBox7))]
    private TextBox _TextBox7;

    [AccessedThroughProperty(nameof(TextBox8))]
    private TextBox _TextBox8;

    [AccessedThroughProperty(nameof(TextBox9))]
    private TextBox _TextBox9;

    [AccessedThroughProperty(nameof(TextBox10))]
    private TextBox _TextBox10;

    [AccessedThroughProperty(nameof(TextBox11))]
    private TextBox _TextBox11;

    [AccessedThroughProperty(nameof(RichTextBox1))]
    private RichTextBox _RichTextBox1;

    [AccessedThroughProperty(nameof(RichTextBox2))]
    private RichTextBox _RichTextBox2;

    [AccessedThroughProperty(nameof(ListBox1))]
    private ListBox _ListBox1;

    [AccessedThroughProperty(nameof(ListBox2))]
    private ListBox _ListBox2;

    [AccessedThroughProperty(nameof(ListBox3))]
    private ListBox _ListBox3;

    [AccessedThroughProperty(nameof(Button11))]
    private Button _Button11;

    [AccessedThroughProperty(nameof(Button12))]
    private Button _Button12;

    [AccessedThroughProperty(nameof(Button13))]
    private Button _Button13;

    [AccessedThroughProperty(nameof(Button14))]
    private Button _Button14;

    [AccessedThroughProperty(nameof(TextBox12))]
    private TextBox _TextBox12;

    [AccessedThroughProperty(nameof(TextBox13))]
    private TextBox _TextBox13;

    [AccessedThroughProperty(nameof(TextBox15))]
    private TextBox _TextBox15;

    [AccessedThroughProperty(nameof(TextBox14))]
    private TextBox _TextBox14;

    [AccessedThroughProperty(nameof(Button15))]
    private Button _Button15;

    [AccessedThroughProperty(nameof(TextBox16))]
    private TextBox _TextBox16;

    [AccessedThroughProperty(nameof(TextBox17))]
    private TextBox _TextBox17;

    [AccessedThroughProperty(nameof(TextBox18))]
    private TextBox _TextBox18;

    [AccessedThroughProperty(nameof(Button16))]
    private Button _Button16;

    [AccessedThroughProperty(nameof(ListBox4))]
    private ListBox _ListBox4;

    [AccessedThroughProperty(nameof(Button17))]
    private Button _Button17;

    /*
         btnReqHint.Click += new EventHandler(btnShowAsocPeople_Click);
         btnRegisterSearch.Click += new EventHandler(btnNewEntry_Click);
         btnDuplBttn3.Click += new EventHandler(btnReenter_Click);
         btnMoveToCause.Click += new EventHandler(btnMoveToCause_Click);
         btnChangeSexToM.Click += new EventHandler(btnChangeSexToM_Click);
         btnChangeSexToF.Click += new EventHandler(btnChangeSexToF_Click);
         btnMoveToChurchCemet.Click += new EventHandler(btnMoveToChurchCemet_Click);
         btnMoveToEntityAnot.Click += new EventHandler(btnMoveToEntityAnot_Click);
         btnMoveToLowerDateAnot.Click += new EventHandler(btnMoveToLowerDateAnot_Click);
         btnDeleteEntry.Click += new EventHandler(frmSrch.btnDeleteEntry_Click);
         lblDisplayHint.Click += new EventHandler(Label3_Click);
         Label12.Click += new EventHandler(Label12_Click);
         edtPredicate.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtPredicate.GotFocus += new EventHandler(TextBox1_GotFocus);
         edtPredicate.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
         edtPredicate.KeyUp +=new KeyEventHandler(TextBox1_KeyUp1);
         
         edtSuburb.LostFocus += new EventHandler(TextBox2_LostFocus);
         
         edtSuburb.GotFocus += new EventHandler(TextBox1_GotFocus);
         edtSuburb.KeyDown +=new KeyEventHandler(TextBox1_KeyDown);
         edtSuburb.KeyUp +=new KeyEventHandler(TextBox1_KeyUp1);
         edtSuburb.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtCounty.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         
         edtCounty.GotFocus += new EventHandler(TextBox1_GotFocus);
         edtCounty.KeyDown +=new KeyEventHandler(TextBox1_KeyDown);
         edtCounty.KeyUp += new KeyEventHandler(TextBox1_KeyUp1);
         
         edtCountry.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         
         edtCountry.GotFocus += new EventHandler(TextBox1_GotFocus);
         edtCountry.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
         edtCountry.KeyUp += new KeyEventHandler(TextBox1_KeyUp1);
         
         edtState.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         
         edtState.GotFocus += new EventHandler(TextBox5_GotFocus);
         edtState.KeyDown +=  new KeyEventHandler(TextBox1_KeyDown);
         edtState.KeyUp += new KeyEventHandler(TextBox1_KeyUp1);
         
         edtLocator.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtLocator.GotFocus += new EventHandler(TextBox1_GotFocus);
         edtLocator.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
         edtLocator.KeyUp +=  new KeyEventHandler(TextBox1_KeyUp1);
         
         edtLat1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtLat1.GotFocus += new EventHandler(TextBox1_GotFocus);
         edtLat1.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
         edtLat1.KeyUp += new KeyEventHandler(TextBox1_KeyUp1);
         
         edtLong1.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtLong1.KeyDown +=  new KeyEventHandler (TextBox1_KeyDown);
         edtLong1.KeyUp +=   new KeyEventHandler (TextBox1_KeyUp1);
         edtGOV.LostFocus += new EventHandler(TextBox2_LostFocus);
         
         edtGOV.GotFocus +=   new  EventHandler(TextBox1_GotFocus);
         edtGOV.KeyDown +=  new  KeyEventHandler(TextBox1_KeyDown);
         edtGOV.KeyUp +=  new  KeyEventHandler(TextBox1_KeyUp1);
         edtGOV.KeyPress +=   new  KeyPressEventHandler(TextBox1_KeyPress);
         
         edtLat2.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtLat2.GotFocus += new  EventHandler (TextBox1_GotFocus);
         edtLat2.KeyDown +=  new KeyEventHandler ( TextBox1_KeyDown);
         edtLat2.KeyUp += new KeyEventHandler ( TextBox1_KeyUp1);
         edtLat3.KeyUp += new KeyEventHandler(TextBox1_KeyUp1);   
         edtLat3.KeyPress +=  new  KeyPressEventHandler (TextBox11_KeyPress);
         edtLat3.KeyDown +=      new   KeyEventHandler (TextBox2_KeyDown);
         edtLat3.GotFocus +=   new EventHandler (TextBox1_GotFocus);
         RichTextBox1.KeyDown += new KeyEventHandler(RichTextBox1_KeyDown);
         RichTextBox1.KeyUp += new KeyEventHandler ( RichTextBox1_KeyUp);
         RichTextBox2.KeyDown += new KeyEventHandler(RichTextBox2_KeyDown);
         RichTextBox2.KeyUp += new KeyEventHandler (RichTextBox2_KeyUp);
         lstUsageList.DoubleClick += new EventHandler(frmSrch.ListBox1_DoubleClick);
         ListBox2.DoubleClick += new EventHandler(ListBox2_DoubleClick);
         ListBox3.DoubleClick += new EventHandler(ListBox3_DoubleClick);
         btnMoveToDateAnot.Click += new EventHandler(OpenCalculations);
         Button12.Click += new EventHandler(OpenFunctionKeys);
         Button13.Click += new EventHandler(OpenConfig);
         Button14.Click += new EventHandler(Button14_Click_1);
         edtLong2.KeyPress += new KeyPressEventHandler(TextBox12_KeyPress);
         edtLong2.TextChanged += new EventHandler (TextBox12_TextChanged);
         edtLong3.KeyPress += new KeyPressEventHandler(TextBox12_KeyPress);
         edtLong3.TextChanged += new EventHandler(TextBox12_TextChanged);
         edtAdditional.KeyPress += new KeyPressEventHandler(TextBox12_KeyPress);
         edtAdditional.TextChanged += new EventHandler (TextBox12_TextChanged);
         edtZIP.KeyPress += new KeyPressEventHandler(TextBox12_KeyPress);
         edtZIP.TextChanged += new EventHandler (TextBox12_TextChanged);
         Button15.Click += new EventHandler(OpenCheckMissing);
         edtPolName.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         edtPolName.KeyDown +=  new KeyEventHandler (TextBox1_KeyDown);
         edtPolName.KeyUp +=  new  KeyEventHandler (TextBox1_KeyUp1);
         TextBox17.KeyPress += new KeyPressEventHandler(TextBox1_KeyPress);
         TextBox17.KeyDown +=  new KeyEventHandler(TextBox1_KeyDown);
         TextBox17.KeyUp += new KeyEventHandler (TextBox17_KeyUp);
         TextBox17.KeyUp +=   new KeyEventHandler (TextBox1_KeyUp1);
         TextBox18.KeyUp += new KeyEventHandler(TextBox18_KeyUp);
         Button16.Click += new EventHandler(OpenCheckPersons);
         ListBox4.DoubleClick += new EventHandler(ListBox4_DoubleClick);
         Button17.Click += new EventHandler(OpenDuplettes);
            */

    public Button Button1;


    public Button Button2;

    

    public Button Button3;

    

    public Button Button4;

    

    public Button Button5;

    

    public Button Button6;

    

    public Button Button7;

    

    public Button Button8;

    

    public Button Button9;

    internal Label Label1;
    internal Label Label2;



    public Button Button10;
    internal Label Label4;
    internal Label Label5;
    internal Label Label6;
    internal Label Label7;
    internal Label Label11;


    public Label Label3;
    internal Label Label14;
    internal Label Label15;


    public Label Label12;
    
    public TextBox TextBox1;

    

    public TextBox TextBox2;

    

    public TextBox TextBox3;

    

    public TextBox TextBox4;



    public TextBox TextBox5;



    public TextBox TextBox6;



    public TextBox TextBox7;



    public TextBox TextBox8;

    

    public TextBox TextBox9;

    

    public TextBox TextBox10;

    

    public TextBox TextBox11;

    

    public RichTextBox RichTextBox1;

    internal GroupBox Frame1;
    internal Label Label9;
    internal Label Label10;



    public RichTextBox RichTextBox2;
    
    public ListBox ListBox1;

    

    public ListBox ListBox2;

    

    public ListBox ListBox3;

    internal Label Label13;



    public Button Button11;
    internal Label Label8;
    internal CheckBox Check1;
    internal Label Label16;


    public Button Button12;
    
    public Button Button13;

    internal Label Label18;
    internal Label Label19;



    public Button Button14;
    
    public TextBox TextBox12;

    

    public TextBox TextBox13;

    

    public TextBox TextBox15;

    internal Label Label17;
    internal CheckBox CheckBox1;

    

    public TextBox TextBox14;
    internal Label Label20;


    public Button Button15;
    internal Label Label21;

    
    public TextBox TextBox16;

    public TextBox TextBox17;

    internal Label Label22;
    internal TextBox TextBox19;



    public TextBox TextBox18;
    internal GroupBox Box1;
    internal RadioButton RadioButton3;
    internal RadioButton RadioButton2;
    internal RadioButton RadioButton1;


    public Button Button16;
    internal TextBox edtGrabNr;


    public ListBox ListBox4;
    internal Label Label23;
    internal Button Button18;
    internal CheckBox CheckBox2;


    public Button Button17;
}