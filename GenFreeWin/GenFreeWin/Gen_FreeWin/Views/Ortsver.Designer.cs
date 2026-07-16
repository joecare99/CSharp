using GenFreeWin.Attributes;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using Views;
using static System.Net.Mime.MediaTypeNames;

namespace GenFreeWin.Views;
partial class Ortsver
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

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.edtLat1 = new System.Windows.Forms.TextBox();
            this.edtLong1 = new System.Windows.Forms.TextBox();
            this.edtLat2 = new System.Windows.Forms.TextBox();
            this.edtLat3 = new System.Windows.Forms.TextBox();
            this.edtLong2 = new System.Windows.Forms.TextBox();
            this.edtLong3 = new System.Windows.Forms.TextBox();
            this.edtZIP = new System.Windows.Forms.TextBox();
            this.edtAdditional = new System.Windows.Forms.TextBox();
            this.edtGOV = new System.Windows.Forms.TextBox();
            this.edtLocator = new System.Windows.Forms.TextBox();
            this.edtState = new System.Windows.Forms.TextBox();
            this.edtCountry = new System.Windows.Forms.TextBox();
            this.edtCounty = new System.Windows.Forms.TextBox();
            this.edtSuburb = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.edtPlace = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.edtPolName = new System.Windows.Forms.TextBox();
            this.TextBox17 = new System.Windows.Forms.TextBox();
            this.TextBox18 = new System.Windows.Forms.TextBox();
            this.TextBox21 = new System.Windows.Forms.TextBox();
            this.RTB1 = new System.Windows.Forms.RichTextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnShowPlaceGE = new System.Windows.Forms.Button();
            this.btnShowPlaceGM = new System.Windows.Forms.Button();
            this.btnLinkGOV = new System.Windows.Forms.Button();
            this.btnSearchGOV = new System.Windows.Forms.Button();
            this.btnConvertKoords = new System.Windows.Forms.Button();
            this.btnSearchName = new System.Windows.Forms.Button();
            this.btnSearchNumber = new System.Windows.Forms.Button();
            this.Button10 = new System.Windows.Forms.Button();
            this.Button11 = new System.Windows.Forms.Button();
            this.Button12 = new System.Windows.Forms.Button();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.Button15 = new System.Windows.Forms.Button();
            this.Button14 = new System.Windows.Forms.Button();
            this.Button13 = new System.Windows.Forms.Button();
            this.TextBox29 = new System.Windows.Forms.TextBox();
            this.TextBox28 = new System.Windows.Forms.TextBox();
            this.TextBox27 = new System.Windows.Forms.TextBox();
            this.TextBox26 = new System.Windows.Forms.TextBox();
            this.TextBox25 = new System.Windows.Forms.TextBox();
            this.TextBox24 = new System.Windows.Forms.TextBox();
            this.TextBox23 = new System.Windows.Forms.TextBox();
            this.TextBox22 = new System.Windows.Forms.TextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.Button16 = new System.Windows.Forms.Button();
            this.ListBox2 = new System.Windows.Forms.ListBox();
            this.TextBox30 = new System.Windows.Forms.TextBox();
            this.Button17 = new System.Windows.Forms.Button();
            this.Button18 = new System.Windows.Forms.Button();
            this.Button19 = new System.Windows.Forms.Button();
            this.Label22 = new System.Windows.Forms.Label();
            this.Button20 = new System.Windows.Forms.Button();
            this.Button21 = new System.Windows.Forms.Button();
            this.Button22 = new System.Windows.Forms.Button();
            this.Button23 = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Label35 = new System.Windows.Forms.Label();
            this.Label34 = new System.Windows.Forms.Label();
            this.Label33 = new System.Windows.Forms.Label();
            this.Label32 = new System.Windows.Forms.Label();
            this.Label31 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.Button24 = new System.Windows.Forms.Button();
            this.Label29 = new System.Windows.Forms.Label();
            this.Label28 = new System.Windows.Forms.Label();
            this.Label27 = new System.Windows.Forms.Label();
            this.Label25 = new System.Windows.Forms.Label();
            this.ListBox4 = new System.Windows.Forms.ListBox();
            this.ListBox3 = new System.Windows.Forms.ListBox();
            this.Label24 = new System.Windows.Forms.Label();
            this.Label23 = new System.Windows.Forms.Label();
            this.TextBox32 = new System.Windows.Forms.TextBox();
            this.TextBox31 = new System.Windows.Forms.TextBox();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Frame1.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.Color.Yellow;
            this.Label7.Location = new System.Drawing.Point(-2, 164);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(115, 18);
            this.Label7.TabIndex = 6;
            this.Label7.Text = "Geog.Breite:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label8
            // 
            this.Label8.BackColor = System.Drawing.Color.Yellow;
            this.Label8.Location = new System.Drawing.Point(-2, 186);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(115, 18);
            this.Label8.TabIndex = 7;
            this.Label8.Text = "Geog.LÃ¤nge:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label9
            // 
            this.Label9.BackColor = System.Drawing.Color.Yellow;
            this.Label9.Location = new System.Drawing.Point(2, 215);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(115, 18);
            this.Label9.TabIndex = 8;
            this.Label9.Text = "Ausgabezusatz:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // edtLat1
            // 
            this.edtLat1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLat1.Location = new System.Drawing.Point(115, 164);
            this.edtLat1.Name = "edtLat1";
            this.edtLat1.Size = new System.Drawing.Size(40, 17);
            this.edtLat1.TabIndex = 8;
            this.edtLat1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.edtLat1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLat1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtLong1
            // 
            this.edtLong1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLong1.Location = new System.Drawing.Point(115, 186);
            this.edtLong1.Name = "edtLong1";
            this.edtLong1.Size = new System.Drawing.Size(40, 17);
            this.edtLong1.TabIndex = 11;
            this.edtLong1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.edtLong1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLong1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtLat2
            // 
            this.edtLat2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLat2.Location = new System.Drawing.Point(161, 164);
            this.edtLat2.Name = "edtLat2";
            this.edtLat2.Size = new System.Drawing.Size(30, 17);
            this.edtLat2.TabIndex = 9;
            this.edtLat2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLat2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtLat3
            // 
            this.edtLat3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLat3.Location = new System.Drawing.Point(223, 164);
            this.edtLat3.Name = "edtLat3";
            this.edtLat3.Size = new System.Drawing.Size(30, 17);
            this.edtLat3.TabIndex = 10;
            this.edtLat3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLat3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtLong2
            // 
            this.edtLong2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLong2.Location = new System.Drawing.Point(161, 186);
            this.edtLong2.Name = "edtLong2";
            this.edtLong2.Size = new System.Drawing.Size(30, 17);
            this.edtLong2.TabIndex = 12;
            this.edtLong2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLong2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtLong3
            // 
            this.edtLong3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLong3.Location = new System.Drawing.Point(223, 186);
            this.edtLong3.Name = "edtLong3";
            this.edtLong3.Size = new System.Drawing.Size(30, 17);
            this.edtLong3.TabIndex = 13;
            this.edtLong3.TextChanged += new System.EventHandler(this.TextBox13_TextChanged);
            this.edtLong3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLong3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtZIP
            // 
            this.edtZIP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtZIP.Location = new System.Drawing.Point(47, 341);
            this.edtZIP.Name = "edtZIP";
            this.edtZIP.Size = new System.Drawing.Size(105, 17);
            this.edtZIP.TabIndex = 17;
            this.edtZIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtZIP.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtAdditional
            // 
            this.edtAdditional.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtAdditional.Location = new System.Drawing.Point(122, 215);
            this.edtAdditional.Name = "edtAdditional";
            this.edtAdditional.Size = new System.Drawing.Size(138, 17);
            this.edtAdditional.TabIndex = 15;
            this.edtAdditional.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtAdditional.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtGOV
            // 
            this.edtGOV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtGOV.Location = new System.Drawing.Point(303, 118);
            this.edtGOV.Name = "edtGOV";
            this.edtGOV.Size = new System.Drawing.Size(118, 17);
            this.edtGOV.TabIndex = 7;
            this.edtGOV.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtGOV.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtLocator
            // 
            this.edtLocator.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtLocator.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.edtLocator.Location = new System.Drawing.Point(87, 118);
            this.edtLocator.Name = "edtLocator";
            this.edtLocator.Size = new System.Drawing.Size(77, 17);
            this.edtLocator.TabIndex = 6;
            this.edtLocator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtLocator.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtState
            // 
            this.edtState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtState.Location = new System.Drawing.Point(87, 97);
            this.edtState.Name = "edtState";
            this.edtState.Size = new System.Drawing.Size(334, 17);
            this.edtState.TabIndex = 5;
            this.edtState.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtState.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtCountry
            // 
            this.edtCountry.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtCountry.Location = new System.Drawing.Point(87, 75);
            this.edtCountry.Name = "edtCountry";
            this.edtCountry.Size = new System.Drawing.Size(334, 17);
            this.edtCountry.TabIndex = 4;
            this.edtCountry.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtCountry.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtCounty
            // 
            this.edtCounty.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtCounty.Location = new System.Drawing.Point(87, 53);
            this.edtCounty.Name = "edtCounty";
            this.edtCounty.Size = new System.Drawing.Size(334, 17);
            this.edtCounty.TabIndex = 3;
            this.edtCounty.WordWrap = false;
            this.edtCounty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtCounty.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // edtSuburb
            // 
            this.edtSuburb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtSuburb.Location = new System.Drawing.Point(87, 31);
            this.edtSuburb.MaxLength = 327;
            this.edtSuburb.Name = "edtSuburb";
            this.edtSuburb.Size = new System.Drawing.Size(334, 17);
            this.edtSuburb.TabIndex = 2;
            this.edtSuburb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtSuburb.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // Label10
            // 
            this.Label10.BackColor = System.Drawing.Color.Yellow;
            this.Label10.Location = new System.Drawing.Point(163, 118);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(141, 17);
            this.Label10.TabIndex = 37;
            this.Label10.Text = "GOV-Ortskennung:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label6
            // 
            this.Label6.BackColor = System.Drawing.Color.Yellow;
            this.Label6.Location = new System.Drawing.Point(2, 118);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(87, 17);
            this.Label6.TabIndex = 36;
            this.Label6.Text = "Locator:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.Color.Yellow;
            this.Label5.Location = new System.Drawing.Point(2, 97);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(87, 17);
            this.Label5.TabIndex = 35;
            this.Label5.Text = "Staat:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.Color.Yellow;
            this.Label4.Location = new System.Drawing.Point(2, 75);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(87, 17);
            this.Label4.TabIndex = 34;
            this.Label4.Text = "Land:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.Color.Yellow;
            this.Label3.Location = new System.Drawing.Point(2, 53);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(87, 17);
            this.Label3.TabIndex = 33;
            this.Label3.Text = "Kreis:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.Yellow;
            this.Label2.Location = new System.Drawing.Point(2, 31);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(87, 17);
            this.Label2.TabIndex = 32;
            this.Label2.Text = "Ortsteil:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.Color.Yellow;
            this.Label1.Location = new System.Drawing.Point(2, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(87, 17);
            this.Label1.TabIndex = 31;
            this.Label1.Text = "Ort:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // edtPlace
            // 
            this.edtPlace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtPlace.Location = new System.Drawing.Point(87, 9);
            this.edtPlace.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.edtPlace.Name = "edtPlace";
            this.edtPlace.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtPlace.Size = new System.Drawing.Size(334, 17);
            this.edtPlace.TabIndex = 1;
            this.edtPlace.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtPlace.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // Label15
            // 
            this.Label15.Location = new System.Drawing.Point(112, 144);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(186, 20);
            this.Label15.TabIndex = 46;
            this.Label15.Text = "Grad Minuten Sekunden";
            // 
            // Label11
            // 
            this.Label11.BackColor = System.Drawing.Color.Yellow;
            this.Label11.Location = new System.Drawing.Point(310, 341);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(137, 20);
            this.Label11.TabIndex = 47;
            this.Label11.Text = "Staatskennzeichen:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label13
            // 
            this.Label13.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Label13.Location = new System.Drawing.Point(340, 315);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(115, 20);
            this.Label13.TabIndex = 49;
            this.Label13.Text = "Ausgabezusatz:";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label14
            // 
            this.Label14.BackColor = System.Drawing.Color.Yellow;
            this.Label14.Location = new System.Drawing.Point(158, 341);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(84, 20);
            this.Label14.TabIndex = 50;
            this.Label14.Text = "Territorium:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label16
            // 
            this.Label16.BackColor = System.Drawing.Color.Yellow;
            this.Label16.Location = new System.Drawing.Point(2, 341);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(39, 20);
            this.Label16.TabIndex = 51;
            this.Label16.Text = "PLZ:";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.BackColor = System.Drawing.Color.Yellow;
            this.Label17.Location = new System.Drawing.Point(12, 321);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(0, 17);
            this.Label17.TabIndex = 52;
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label18
            // 
            this.Label18.BackColor = System.Drawing.Color.Yellow;
            this.Label18.Location = new System.Drawing.Point(2, 238);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(351, 20);
            this.Label18.TabIndex = 53;
            this.Label18.Text = "Heutiger Ortsname in ehemals deutschen Gebieten:";
            this.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // edtPolName
            // 
            this.edtPolName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtPolName.Location = new System.Drawing.Point(10, 261);
            this.edtPolName.Name = "edtPolName";
            this.edtPolName.Size = new System.Drawing.Size(420, 17);
            this.edtPolName.TabIndex = 16;
            this.edtPolName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.edtPolName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // TextBox17
            // 
            this.TextBox17.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox17.Location = new System.Drawing.Point(248, 341);
            this.TextBox17.Name = "TextBox17";
            this.TextBox17.Size = new System.Drawing.Size(56, 17);
            this.TextBox17.TabIndex = 18;
            this.TextBox17.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.TextBox17.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // TextBox18
            // 
            this.TextBox18.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox18.Location = new System.Drawing.Point(453, 341);
            this.TextBox18.Name = "TextBox18";
            this.TextBox18.Size = new System.Drawing.Size(41, 17);
            this.TextBox18.TabIndex = 19;
            this.TextBox18.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Textbox1_KeyPress);
            this.TextBox18.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            // 
            // TextBox21
            // 
            this.TextBox21.BackColor = System.Drawing.Color.Yellow;
            this.TextBox21.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox21.Location = new System.Drawing.Point(1, 298);
            this.TextBox21.Multiline = true;
            this.TextBox21.Name = "TextBox21";
            this.TextBox21.ReadOnly = true;
            this.TextBox21.Size = new System.Drawing.Size(333, 37);
            this.TextBox21.TabIndex = 59;
            this.TextBox21.Text = "Angaben fÃ¼r Forscherkontakte der DAGV, kÃ¶nnen Entfallen wenn die GOV-Kennung vorh" +
    "anden ist";
            this.TextBox21.Visible = false;
            // 
            // RTB1
            // 
            this.RTB1.Location = new System.Drawing.Point(47, 376);
            this.RTB1.Name = "RTB1";
            this.RTB1.Size = new System.Drawing.Size(968, 250);
            this.RTB1.TabIndex = 60;
            this.RTB1.Text = "";
            this.RTB1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RTB1_KeyUp);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNext.Location = new System.Drawing.Point(178, 629);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(165, 25);
            this.btnNext.TabIndex = 61;
            this.btnNext.Text = "&vorblÃ¤ttern";
            this.btnNext.UseVisualStyleBackColor = false;
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPrev.Location = new System.Drawing.Point(176, 658);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(165, 25);
            this.btnPrev.TabIndex = 62;
            this.btnPrev.Text = "&rÃ¼ckblÃ¤ttern";
            this.btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnShowPlaceGE
            // 
            this.btnShowPlaceGE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnShowPlaceGE.Location = new System.Drawing.Point(546, 629);
            this.btnShowPlaceGE.Name = "btnShowPlaceGE";
            this.btnShowPlaceGE.Size = new System.Drawing.Size(215, 25);
            this.btnShowPlaceGE.TabIndex = 63;
            this.btnShowPlaceGE.Text = "Ort anzeigen mit Google-&Earth";
            this.btnShowPlaceGE.UseVisualStyleBackColor = false;
            // 
            // btnShowPlaceGM
            // 
            this.btnShowPlaceGM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnShowPlaceGM.Location = new System.Drawing.Point(547, 658);
            this.btnShowPlaceGM.Name = "btnShowPlaceGM";
            this.btnShowPlaceGM.Size = new System.Drawing.Size(214, 25);
            this.btnShowPlaceGM.TabIndex = 64;
            this.btnShowPlaceGM.Text = "&Ort anzeigen mit Google-Maps";
            this.btnShowPlaceGM.UseVisualStyleBackColor = false;
            // 
            // btnLinkGOV
            // 
            this.btnLinkGOV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnLinkGOV.Location = new System.Drawing.Point(767, 629);
            this.btnLinkGOV.Name = "btnLinkGOV";
            this.btnLinkGOV.Size = new System.Drawing.Size(175, 25);
            this.btnLinkGOV.TabIndex = 65;
            this.btnLinkGOV.Text = "Verbindung zum &GOV";
            this.btnLinkGOV.UseVisualStyleBackColor = false;
            // 
            // btnSearchGOV
            // 
            this.btnSearchGOV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchGOV.Location = new System.Drawing.Point(767, 658);
            this.btnSearchGOV.Name = "btnSearchGOV";
            this.btnSearchGOV.Size = new System.Drawing.Size(175, 25);
            this.btnSearchGOV.TabIndex = 66;
            this.btnSearchGOV.Text = "Or&tssuche im GOV";
            this.btnSearchGOV.UseVisualStyleBackColor = false;
            // 
            // btnConvertKoords
            // 
            this.btnConvertKoords.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnConvertKoords.Location = new System.Drawing.Point(547, 685);
            this.btnConvertKoords.Name = "btnConvertKoords";
            this.btnConvertKoords.Size = new System.Drawing.Size(214, 25);
            this.btnConvertKoords.TabIndex = 67;
            this.btnConvertKoords.Text = "&Koordinaten umrechnen";
            this.btnConvertKoords.UseVisualStyleBackColor = false;
            // 
            // btnSearchName
            // 
            this.btnSearchName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchName.Location = new System.Drawing.Point(343, 629);
            this.btnSearchName.Name = "btnSearchName";
            this.btnSearchName.Size = new System.Drawing.Size(197, 25);
            this.btnSearchName.TabIndex = 68;
            this.btnSearchName.Text = "Suche nach Na&men";
            this.btnSearchName.UseVisualStyleBackColor = false;
            // 
            // btnSearchNumber
            // 
            this.btnSearchNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSearchNumber.Location = new System.Drawing.Point(343, 658);
            this.btnSearchNumber.Name = "btnSearchNumber";
            this.btnSearchNumber.Size = new System.Drawing.Size(198, 25);
            this.btnSearchNumber.TabIndex = 69;
            this.btnSearchNumber.Text = "Suche n&ach Nummer";
            this.btnSearchNumber.UseVisualStyleBackColor = false;
            // 
            // Button10
            // 
            this.Button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button10.Location = new System.Drawing.Point(767, 685);
            this.Button10.Name = "Button10";
            this.Button10.Size = new System.Drawing.Size(175, 25);
            this.Button10.TabIndex = 70;
            this.Button10.UseVisualStyleBackColor = false;
            // 
            // Button11
            // 
            this.Button11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button11.Location = new System.Drawing.Point(13, 629);
            this.Button11.Name = "Button11";
            this.Button11.Size = new System.Drawing.Size(142, 25);
            this.Button11.TabIndex = 71;
            this.Button11.Text = "&neu Eingeben";
            this.Button11.UseVisualStyleBackColor = false;
            // 
            // Button12
            // 
            this.Button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button12.Location = new System.Drawing.Point(13, 660);
            this.Button12.Name = "Button12";
            this.Button12.Size = new System.Drawing.Size(142, 25);
            this.Button12.TabIndex = 72;
            this.Button12.Text = "&speichern";
            this.Button12.UseVisualStyleBackColor = false;
            this.Button12.Visible = false;
            // 
            // Frame1
            // 
            this.Frame1.Controls.Add(this.Button15);
            this.Frame1.Controls.Add(this.Button14);
            this.Frame1.Controls.Add(this.Button13);
            this.Frame1.Controls.Add(this.TextBox29);
            this.Frame1.Controls.Add(this.TextBox28);
            this.Frame1.Controls.Add(this.TextBox27);
            this.Frame1.Controls.Add(this.TextBox26);
            this.Frame1.Controls.Add(this.TextBox25);
            this.Frame1.Controls.Add(this.TextBox24);
            this.Frame1.Controls.Add(this.TextBox23);
            this.Frame1.Controls.Add(this.TextBox22);
            this.Frame1.Controls.Add(this.Label21);
            this.Frame1.Controls.Add(this.Label20);
            this.Frame1.Controls.Add(this.Label19);
            this.Frame1.Font = new System.Drawing.Font("Arial", 9F);
            this.Frame1.Location = new System.Drawing.Point(458, 477);
            this.Frame1.Name = "Frame1";
            this.Frame1.Size = new System.Drawing.Size(377, 139);
            this.Frame1.TabIndex = 73;
            this.Frame1.TabStop = false;
            this.Frame1.Text = "Koordinatenrechner";
            this.Frame1.Visible = false;
            // 
            // Button15
            // 
            this.Button15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button15.Location = new System.Drawing.Point(108, 105);
            this.Button15.Name = "Button15";
            this.Button15.Size = new System.Drawing.Size(89, 22);
            this.Button15.TabIndex = 13;
            this.Button15.Text = "Ã¼bernehmen";
            this.Button15.UseVisualStyleBackColor = false;
            // 
            // Button14
            // 
            this.Button14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button14.Location = new System.Drawing.Point(228, 105);
            this.Button14.Name = "Button14";
            this.Button14.Size = new System.Drawing.Size(74, 22);
            this.Button14.TabIndex = 12;
            this.Button14.Text = "SchlieÃŸen";
            this.Button14.UseVisualStyleBackColor = false;
            // 
            // Button13
            // 
            this.Button13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button13.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Button13.Location = new System.Drawing.Point(13, 105);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(73, 22);
            this.Button13.TabIndex = 11;
            this.Button13.Text = "Rechnen";
            this.Button13.UseVisualStyleBackColor = false;
            // 
            // TextBox29
            // 
            this.TextBox29.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox29.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox29.Location = new System.Drawing.Point(299, 63);
            this.TextBox29.Multiline = true;
            this.TextBox29.Name = "TextBox29";
            this.TextBox29.Size = new System.Drawing.Size(23, 16);
            this.TextBox29.TabIndex = 10;
            // 
            // TextBox28
            // 
            this.TextBox28.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox28.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox28.Location = new System.Drawing.Point(244, 63);
            this.TextBox28.Multiline = true;
            this.TextBox28.Name = "TextBox28";
            this.TextBox28.Size = new System.Drawing.Size(21, 16);
            this.TextBox28.TabIndex = 9;
            // 
            // TextBox27
            // 
            this.TextBox27.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox27.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox27.Location = new System.Drawing.Point(92, 63);
            this.TextBox27.Name = "TextBox27";
            this.TextBox27.Size = new System.Drawing.Size(87, 15);
            this.TextBox27.TabIndex = 8;
            // 
            // TextBox26
            // 
            this.TextBox26.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox26.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox26.Location = new System.Drawing.Point(188, 63);
            this.TextBox26.Multiline = true;
            this.TextBox26.Name = "TextBox26";
            this.TextBox26.Size = new System.Drawing.Size(38, 16);
            this.TextBox26.TabIndex = 7;
            // 
            // TextBox25
            // 
            this.TextBox25.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox25.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox25.Location = new System.Drawing.Point(299, 41);
            this.TextBox25.Multiline = true;
            this.TextBox25.Name = "TextBox25";
            this.TextBox25.Size = new System.Drawing.Size(23, 16);
            this.TextBox25.TabIndex = 6;
            // 
            // TextBox24
            // 
            this.TextBox24.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox24.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox24.Location = new System.Drawing.Point(244, 41);
            this.TextBox24.Multiline = true;
            this.TextBox24.Name = "TextBox24";
            this.TextBox24.Size = new System.Drawing.Size(21, 16);
            this.TextBox24.TabIndex = 5;
            // 
            // TextBox23
            // 
            this.TextBox23.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox23.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox23.Location = new System.Drawing.Point(188, 41);
            this.TextBox23.Multiline = true;
            this.TextBox23.Name = "TextBox23";
            this.TextBox23.Size = new System.Drawing.Size(38, 16);
            this.TextBox23.TabIndex = 4;
            // 
            // TextBox22
            // 
            this.TextBox22.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox22.Font = new System.Drawing.Font("Arial", 9.75F);
            this.TextBox22.Location = new System.Drawing.Point(92, 41);
            this.TextBox22.Name = "TextBox22";
            this.TextBox22.Size = new System.Drawing.Size(87, 15);
            this.TextBox22.TabIndex = 3;
            // 
            // Label21
            // 
            this.Label21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.Label21.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Label21.Location = new System.Drawing.Point(10, 41);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(76, 16);
            this.Label21.TabIndex = 2;
            this.Label21.Text = "Breite";
            // 
            // Label20
            // 
            this.Label20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.Label20.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Label20.Location = new System.Drawing.Point(10, 63);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(75, 16);
            this.Label20.TabIndex = 1;
            this.Label20.Text = "LÃ¤nge";
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.Label19.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Label19.Location = new System.Drawing.Point(88, 17);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(230, 16);
            this.Label19.TabIndex = 0;
            this.Label19.Text = "Dezimal        Grad Minuten  Sekunden";
            // 
            // lstUsageList
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 17;
            this.ListBox1.Location = new System.Drawing.Point(458, 11);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(406, 242);
            this.ListBox1.TabIndex = 74;
            this.ListBox1.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            // 
            // Button16
            // 
            this.Button16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button16.Location = new System.Drawing.Point(515, 335);
            this.Button16.Name = "Button16";
            this.Button16.Size = new System.Drawing.Size(95, 25);
            this.Button16.TabIndex = 75;
            this.Button16.Text = "Bilder nein";
            this.Button16.UseVisualStyleBackColor = false;
            // 
            // ListBox2
            // 
            this.ListBox2.Font = new System.Drawing.Font("Courier New", 8.5F);
            this.ListBox2.FormattingEnabled = true;
            this.ListBox2.ItemHeight = 14;
            this.ListBox2.Location = new System.Drawing.Point(458, 11);
            this.ListBox2.Name = "ListBox2";
            this.ListBox2.Size = new System.Drawing.Size(504, 256);
            this.ListBox2.TabIndex = 76;
            this.ListBox2.Visible = false;
            this.ListBox2.DoubleClick += new System.EventHandler(this.ListBox2_DoubleClick);
            // 
            // TextBox30
            // 
            this.TextBox30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.TextBox30.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox30.Location = new System.Drawing.Point(113, 9);
            this.TextBox30.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.TextBox30.Name = "TextBox30";
            this.TextBox30.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.TextBox30.Size = new System.Drawing.Size(334, 17);
            this.TextBox30.TabIndex = 77;
            this.TextBox30.Visible = false;
            this.TextBox30.TextChanged += new System.EventHandler(this.TextBox30_TextChanged);
            // 
            // Button17
            // 
            this.Button17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button17.Location = new System.Drawing.Point(354, 660);
            this.Button17.Name = "Button17";
            this.Button17.Size = new System.Drawing.Size(187, 25);
            this.Button17.TabIndex = 78;
            this.Button17.Text = "Abbruch ohne speichern";
            this.Button17.UseVisualStyleBackColor = false;
            this.Button17.Visible = false;
            // 
            // Button18
            // 
            this.Button18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button18.Location = new System.Drawing.Point(652, 335);
            this.Button18.Name = "Button18";
            this.Button18.Size = new System.Drawing.Size(130, 25);
            this.Button18.TabIndex = 79;
            this.Button18.Text = "Ort lÃ¶schen";
            this.Button18.UseVisualStyleBackColor = false;
            this.Button18.Visible = false;
            // 
            // Button19
            // 
            this.Button19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button19.Location = new System.Drawing.Point(13, 660);
            this.Button19.Name = "Button19";
            this.Button19.Size = new System.Drawing.Size(291, 25);
            this.Button19.TabIndex = 80;
            this.Button19.Text = "Speichern und &ZurÃ¼ck zur Datumsmaske";
            this.Button19.UseVisualStyleBackColor = false;
            this.Button19.Visible = false;
            // 
            // Label22
            // 
            this.Label22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Label22.Location = new System.Drawing.Point(1, 9);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(99, 17);
            this.Label22.TabIndex = 81;
            this.Label22.Text = "Ortsname? :";
            this.Label22.Visible = false;
            // 
            // Button20
            // 
            this.Button20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button20.Location = new System.Drawing.Point(799, 337);
            this.Button20.Name = "Button20";
            this.Button20.Size = new System.Drawing.Size(163, 25);
            this.Button20.TabIndex = 82;
            this.Button20.Text = "Nichtverwendete Orte";
            this.Button20.UseVisualStyleBackColor = false;
            // 
            // Button21
            // 
            this.Button21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button21.Location = new System.Drawing.Point(343, 685);
            this.Button21.Name = "Button21";
            this.Button21.Size = new System.Drawing.Size(198, 25);
            this.Button21.TabIndex = 83;
            this.Button21.Text = "&Koordinaten ermitteln";
            this.Button21.UseVisualStyleBackColor = false;
            this.Button21.Visible = false;
            // 
            // Button22
            // 
            this.Button22.Location = new System.Drawing.Point(867, 477);
            this.Button22.Name = "Button22";
            this.Button22.Size = new System.Drawing.Size(75, 23);
            this.Button22.TabIndex = 84;
            this.Button22.Text = "Button22";
            this.Button22.UseVisualStyleBackColor = true;
            // 
            // Button23
            // 
            this.Button23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button23.Location = new System.Drawing.Point(166, 685);
            this.Button23.Name = "Button23";
            this.Button23.Size = new System.Drawing.Size(175, 25);
            this.Button23.TabIndex = 85;
            this.Button23.Text = "Entfernung berechnen";
            this.Button23.UseVisualStyleBackColor = false;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.Label35);
            this.Panel1.Controls.Add(this.Label34);
            this.Panel1.Controls.Add(this.Label33);
            this.Panel1.Controls.Add(this.Label32);
            this.Panel1.Controls.Add(this.Label31);
            this.Panel1.Controls.Add(this.Label30);
            this.Panel1.Controls.Add(this.Button24);
            this.Panel1.Controls.Add(this.Label29);
            this.Panel1.Controls.Add(this.Label28);
            this.Panel1.Controls.Add(this.Label27);
            this.Panel1.Controls.Add(this.Label25);
            this.Panel1.Controls.Add(this.ListBox4);
            this.Panel1.Controls.Add(this.ListBox3);
            this.Panel1.Controls.Add(this.Label24);
            this.Panel1.Controls.Add(this.Label23);
            this.Panel1.Controls.Add(this.TextBox32);
            this.Panel1.Controls.Add(this.TextBox31);
            this.Panel1.Location = new System.Drawing.Point(870, 164);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(1034, 715);
            this.Panel1.TabIndex = 86;
            this.Panel1.Visible = false;
            // 
            // Label35
            // 
            this.Label35.Location = new System.Drawing.Point(597, 326);
            this.Label35.Name = "Label35";
            this.Label35.Size = new System.Drawing.Size(380, 17);
            this.Label35.TabIndex = 18;
            // 
            // Label34
            // 
            this.Label34.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Label34.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Bold);
            this.Label34.Location = new System.Drawing.Point(6, 15);
            this.Label34.Name = "Label34";
            this.Label34.Size = new System.Drawing.Size(997, 30);
            this.Label34.TabIndex = 17;
            this.Label34.Text = "Entfernungsberechnung zwischen zwei Orten (Luftlinie)";
            this.Label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label33
            // 
            this.Label33.Location = new System.Drawing.Point(6, 413);
            this.Label33.Name = "Label33";
            this.Label33.Size = new System.Drawing.Size(997, 76);
            this.Label33.TabIndex = 16;
            this.Label33.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Label32
            // 
            this.Label32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Label32.Location = new System.Drawing.Point(625, 350);
            this.Label32.Name = "Label32";
            this.Label32.Size = new System.Drawing.Size(351, 59);
            this.Label32.TabIndex = 14;
            // 
            // Label31
            // 
            this.Label31.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Label31.Location = new System.Drawing.Point(531, 160);
            this.Label31.Name = "Label31";
            this.Label31.Size = new System.Drawing.Size(91, 33);
            this.Label31.TabIndex = 13;
            this.Label31.Visible = false;
            // 
            // Label30
            // 
            this.Label30.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.Label30.Location = new System.Drawing.Point(530, 107);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(92, 31);
            this.Label30.TabIndex = 12;
            this.Label30.Visible = false;
            // 
            // Button24
            // 
            this.Button24.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Button24.ForeColor = System.Drawing.Color.Black;
            this.Button24.Location = new System.Drawing.Point(898, 689);
            this.Button24.Name = "Button24";
            this.Button24.Size = new System.Drawing.Size(110, 23);
            this.Button24.TabIndex = 11;
            this.Button24.Text = "SchlieÃŸen";
            this.Button24.UseVisualStyleBackColor = false;
            // 
            // Label29
            // 
            this.Label29.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Label29.Location = new System.Drawing.Point(360, 154);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(106, 36);
            this.Label29.TabIndex = 10;
            this.Label29.Visible = false;
            // 
            // Label28
            // 
            this.Label28.BackColor = System.Drawing.Color.White;
            this.Label28.Location = new System.Drawing.Point(360, 98);
            this.Label28.Name = "Label28";
            this.Label28.Size = new System.Drawing.Size(123, 36);
            this.Label28.TabIndex = 9;
            this.Label28.Visible = false;
            // 
            // Label27
            // 
            this.Label27.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Label27.Location = new System.Drawing.Point(3, 350);
            this.Label27.Name = "Label27";
            this.Label27.Size = new System.Drawing.Size(345, 59);
            this.Label27.TabIndex = 8;
            // 
            // Label25
            // 
            this.Label25.Location = new System.Drawing.Point(14, 332);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(380, 17);
            this.Label25.TabIndex = 6;
            // 
            // ListBox4
            // 
            this.ListBox4.FormattingEnabled = true;
            this.ListBox4.ItemHeight = 17;
            this.ListBox4.Location = new System.Drawing.Point(628, 98);
            this.ListBox4.Name = "ListBox4";
            this.ListBox4.Size = new System.Drawing.Size(346, 191);
            this.ListBox4.TabIndex = 5;
            this.ListBox4.DoubleClick += new System.EventHandler(this.ListBox4_DoubleClick);
            // 
            // ListBox3
            // 
            this.ListBox3.FormattingEnabled = true;
            this.ListBox3.ItemHeight = 17;
            this.ListBox3.Location = new System.Drawing.Point(8, 98);
            this.ListBox3.Name = "ListBox3";
            this.ListBox3.Size = new System.Drawing.Size(346, 191);
            this.ListBox3.TabIndex = 4;
            this.ListBox3.DoubleClick += new System.EventHandler(this.ListBox3_DoubleClick);
            // 
            // Label24
            // 
            this.Label24.Location = new System.Drawing.Point(701, 50);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(100, 18);
            this.Label24.TabIndex = 3;
            this.Label24.Text = "Auswahl Ort 2";
            // 
            // Label23
            // 
            this.Label23.AutoSize = true;
            this.Label23.Location = new System.Drawing.Point(59, 50);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(100, 17);
            this.Label23.TabIndex = 2;
            this.Label23.Text = "Auswahl Ort 1";
            // 
            // TextBox32
            // 
            this.TextBox32.Location = new System.Drawing.Point(628, 70);
            this.TextBox32.Name = "TextBox32";
            this.TextBox32.Size = new System.Drawing.Size(283, 24);
            this.TextBox32.TabIndex = 1;
            this.TextBox32.TextChanged += new System.EventHandler(this.TextBox32_TextChanged);
            // 
            // TextBox31
            // 
            this.TextBox31.Location = new System.Drawing.Point(8, 70);
            this.TextBox31.Name = "TextBox31";
            this.TextBox31.Size = new System.Drawing.Size(283, 24);
            this.TextBox31.TabIndex = 0;
            this.TextBox31.TextChanged += new System.EventHandler(this.TextBox31_TextChanged);
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.Location = new System.Drawing.Point(724, 298);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(261, 21);
            this.CheckBox1.TabIndex = 87;
            this.CheckBox1.Text = "Beim VorblÃ¤ttern Koordinaten prÃ¼fen";
            this.CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Ortsver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 721);
            this.ControlBox = false;
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Button23);
            this.Controls.Add(this.Button21);
            this.Controls.Add(this.Button20);
            this.Controls.Add(this.Label22);
            this.Controls.Add(this.Button19);
            this.Controls.Add(this.Button18);
            this.Controls.Add(this.Button17);
            this.Controls.Add(this.edtPlace);
            this.Controls.Add(this.TextBox30);
            this.Controls.Add(this.ListBox2);
            this.Controls.Add(this.Button16);
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Button12);
            this.Controls.Add(this.Button11);
            this.Controls.Add(this.Button10);
            this.Controls.Add(this.btnSearchNumber);
            this.Controls.Add(this.btnSearchName);
            this.Controls.Add(this.btnConvertKoords);
            this.Controls.Add(this.btnSearchGOV);
            this.Controls.Add(this.btnLinkGOV);
            this.Controls.Add(this.btnShowPlaceGM);
            this.Controls.Add(this.btnShowPlaceGE);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.RTB1);
            this.Controls.Add(this.TextBox21);
            this.Controls.Add(this.TextBox18);
            this.Controls.Add(this.TextBox17);
            this.Controls.Add(this.edtPolName);
            this.Controls.Add(this.Label18);
            this.Controls.Add(this.Label17);
            this.Controls.Add(this.Label16);
            this.Controls.Add(this.Label14);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.Label15);
            this.Controls.Add(this.edtGOV);
            this.Controls.Add(this.edtLocator);
            this.Controls.Add(this.edtState);
            this.Controls.Add(this.edtCountry);
            this.Controls.Add(this.edtCounty);
            this.Controls.Add(this.edtSuburb);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.edtAdditional);
            this.Controls.Add(this.edtZIP);
            this.Controls.Add(this.edtLong3);
            this.Controls.Add(this.edtLong2);
            this.Controls.Add(this.edtLat3);
            this.Controls.Add(this.edtLat2);
            this.Controls.Add(this.edtLong1);
            this.Controls.Add(this.edtLat1);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Button22);
            this.Controls.Add(this.CheckBox1);
            this.Font = new System.Drawing.Font("Arial", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Ortsver";
            this.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Ortsverwaltung";
            this.Frame1.ResumeLayout(false);
            this.Frame1.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    [TextBinding(nameof(IOrtsVerViewModel.edtPlace_Text))]
    internal TextBox edtPlace;
    [TextBinding(nameof(IOrtsVerViewModel.edtSuburb_Text))]
    internal TextBox edtSuburb;
    [TextBinding(nameof(IOrtsVerViewModel.edtCounty_Text))]
    internal TextBox edtCounty;
    [TextBinding(nameof(IOrtsVerViewModel.edtCountry_Text))]
    internal TextBox edtCountry;
    [TextBinding(nameof(IOrtsVerViewModel.edtState_Text))]
    internal TextBox edtState;
    [TextBinding(nameof(IOrtsVerViewModel.edtLocator_Text))]
    internal TextBox edtLocator;
    [TextBinding(nameof(IOrtsVerViewModel.edtLat1_Text))]
    internal TextBox edtLat1;
    [TextBinding(nameof(IOrtsVerViewModel.edtLong1_Text))]
    internal TextBox edtLong1;
    [TextBinding(nameof(IOrtsVerViewModel.edtGOV_Text))]
    internal TextBox edtGOV;
    [TextBinding(nameof(IOrtsVerViewModel.edtLat2_Text))]
    internal TextBox edtLat2;
    [TextBinding(nameof(IOrtsVerViewModel.edtLat3_Text))]
    internal TextBox edtLat3;
    [TextBinding(nameof(IOrtsVerViewModel.edtLong2_Text))]
    internal TextBox edtLong2;
    [TextBinding(nameof(IOrtsVerViewModel.edtLong3_Text))]
    internal TextBox edtLong3;
    [TextBinding(nameof(IOrtsVerViewModel.edtZIP_Text))]
    internal TextBox edtZIP;
    [TextBinding(nameof(IOrtsVerViewModel.edtAdditional_Text))]
    internal TextBox edtAdditional;
    [TextBinding(nameof(IOrtsVerViewModel.edtPolName_Text))]
    internal TextBox edtPolName;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox17_Text))]
    internal TextBox TextBox17;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox18_Text))]
    internal TextBox TextBox18;

    [ApplTextBinding(nameof(IOrtsVerViewModel.CoordinatesDecimalHintText))]
    internal TextBox TextBox21;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox22_Text))]
    internal TextBox TextBox22;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox23_Text))]
    internal TextBox TextBox23;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox24_Text))]
    internal TextBox TextBox24;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox25_Text))]
    internal TextBox TextBox25;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox26_Text))]
    internal TextBox TextBox26;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox27_Text))]
    internal TextBox TextBox27;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox28_Text))]
    internal TextBox TextBox28;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox29_Text))]
    internal TextBox TextBox29;

    [VisibilityBinding(nameof(IOrtsVerViewModel.TextBox30_Visible))]
    [TextBinding(nameof(IOrtsVerViewModel.TextBox30_Text))]
    internal TextBox TextBox30;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox31_Text))]
    internal TextBox TextBox31;
    [TextBinding(nameof(IOrtsVerViewModel.TextBox32_Text))]
    internal TextBox TextBox32;
    [TextBinding(nameof(IOrtsVerViewModel.RTB1_Text))]
    internal RichTextBox RTB1;
    [CommandBinding(nameof(IOrtsVerViewModel.NextCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.NextButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnNext_Visible))]
    internal Button btnNext;
    [CommandBinding(nameof(IOrtsVerViewModel.PrevCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.PrevButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnPrev_Visible))]
    internal Button btnPrev;
    [CommandBinding(nameof(IOrtsVerViewModel.ShowPlaceGECommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.ShowPlaceGEButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnShowPlaceGE_Visible))]
    internal Button btnShowPlaceGE;
    [CommandBinding(nameof(IOrtsVerViewModel.ShowPlaceGMCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.ShowPlaceGMButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnShowPlaceGM_Visible))]
    internal Button btnShowPlaceGM;
    [CommandBinding(nameof(IOrtsVerViewModel.LinkGOVCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.LinkGovButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnLinkGOV_Visible))]
    internal Button btnLinkGOV;
    [CommandBinding(nameof(IOrtsVerViewModel.SearchGOVCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.SearchGovButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnSearchGOV_Visible))]
    internal Button btnSearchGOV;
    [CommandBinding(nameof(IOrtsVerViewModel.ConvertKoordsCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.ConvertCoordinatesButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnConvertKoords_Visible))]
    internal Button btnConvertKoords;
    [CommandBinding(nameof(IOrtsVerViewModel.SearchNameCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.SearchByNameButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnSearchName_Visible))]
    internal Button btnSearchName;
    [CommandBinding(nameof(IOrtsVerViewModel.SearchNumberCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.SearchByNumberButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.btnSearchNumber_Visible))]
    internal Button btnSearchNumber;
    [CommandBinding(nameof(IOrtsVerViewModel.CloseViewCommand))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button10_Visible))]
    [TextBinding(nameof(IOrtsVerViewModel.Button10_Text))]
    internal Button Button10;
    [CommandBinding(nameof(IOrtsVerViewModel.BeginEditCommand))]
    [ApplTextBinding<EUserText>(EUserText.t73)]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button11_Visible))]
    internal Button Button11;
    [CommandBinding(nameof(IOrtsVerViewModel.SavePlaceCommand))]
    [ApplTextBinding<EUserText>(EUserText.tNMSave)]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button12_Visible))]
    internal Button Button12;
    [CommandBinding(nameof(IOrtsVerViewModel.CalculateCoordinatesCommand))]
    [ApplTextBinding<EUserText>(EUserText.t296)]
    internal Button Button13;
    [CommandBinding(nameof(IOrtsVerViewModel.ApplyCoordinatesCommand))]
    [ApplTextBinding<EUserText>(EUserText.t297)]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button15_Visible))]
    internal Button Button15;
    [CommandBinding(nameof(IOrtsVerViewModel.CloseCoordinateConverterCommand))]
    [ApplTextBinding<EUserText>(EUserText.t67)]
    internal Button Button14;
    [VisibilityBinding(nameof(IOrtsVerViewModel.ListBox1_Visible))]
    internal ListBox ListBox1;
    [CommandBinding(nameof(IOrtsVerViewModel.OpenPicturesCommand))]
    [TextBinding(nameof(IOrtsVerViewModel.Button16_Text))]
    internal Button Button16;
    [VisibilityBinding(nameof(IOrtsVerViewModel.ListBox2_Visible))]
    internal ListBox ListBox2;
    [CommandBinding(nameof(IOrtsVerViewModel.CancelEditCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.CancelEditButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button17_Visible))]
    internal Button Button17;
    [CommandBinding(nameof(IOrtsVerViewModel.DeletePlaceCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.DeletePlaceButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button18_Visible))]
    internal Button Button18;
    [CommandBinding(nameof(IOrtsVerViewModel.SaveAndReturnCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.SaveAndReturnButtonText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button19_Visible))]
    internal Button Button19;
    [CommandBinding(nameof(IOrtsVerViewModel.FindUnusedPlacesCommand))]
    [ApplTextBinding(nameof(IOrtsVerViewModel.FindUnusedPlacesButtonText))]
    internal Button Button20;
    [CommandBinding(nameof(IOrtsVerViewModel.DetectCoordinatesCommand))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button21_Visible))]
    internal Button Button21;
    [CommandBinding(nameof(IOrtsVerViewModel.ResetSearchViewCommand))]
    internal Button Button22;
    [CommandBinding(nameof(IOrtsVerViewModel.OpenDistancePanelCommand))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Button23_Visible))]
    internal Button Button23;
    internal ListBox ListBox4;
    internal ListBox ListBox3;
    [TextBinding(nameof(IOrtsVerViewModel.Label27_Text))]
    internal Label Label27;
    [CommandBinding(nameof(IOrtsVerViewModel.CloseDistancePanelCommand))]
    internal Button Button24;
    [TextBinding(nameof(IOrtsVerViewModel.Label32_Text))]
    internal Label Label32;
    [ApplTextBinding(nameof(IOrtsVerViewModel.LatitudeLabelText))]
    internal Label Label7;
    [ApplTextBinding(nameof(IOrtsVerViewModel.LongitudeLabelText))]
    internal Label Label8;
    [ApplTextBinding(nameof(IOrtsVerViewModel.AdditionalOutputLabelText))]
    internal Label Label9;
    [ApplTextBinding(nameof(IOrtsVerViewModel.GovLabelText))]
    internal Label Label10;
    internal Label Label6;
    [ApplTextBinding(nameof(IOrtsVerViewModel.StateLabelText))]
    internal Label Label5;
    [ApplTextBinding(nameof(IOrtsVerViewModel.CountryLabelText))]
    internal Label Label4;
    [ApplTextBinding(nameof(IOrtsVerViewModel.CountyLabelText))]
    internal Label Label3;
    [ApplTextBinding(nameof(IOrtsVerViewModel.SuburbLabelText))]
    internal Label Label2;
    [ApplTextBinding(nameof(IOrtsVerViewModel.PlaceLabelText))]
    [TextBinding(nameof(IOrtsVerViewModel.Label1_Text))]
    internal Label Label1;
    internal Label Label15;
    [ApplTextBinding(nameof(IOrtsVerViewModel.LocatorLabelText))]
    internal Label Label11;
    [TextBinding(nameof(IOrtsVerViewModel.Label13_Text))]
    internal Label Label13;
    [ApplTextBinding(nameof(IOrtsVerViewModel.PostalCodeLabelText))]
    internal Label Label14;
    [ApplTextBinding(nameof(IOrtsVerViewModel.AdditionalLabelText))]
    internal Label Label16;
    internal Label Label17;
    [ApplTextBinding(nameof(IOrtsVerViewModel.PoliticalNameLabelText))]
    internal Label Label18;
    [VisibilityBinding(nameof(IOrtsVerViewModel.Frame1_Visible))]
    internal GroupBox Frame1;
    [ApplTextBinding(nameof(IOrtsVerViewModel.ConverterLatitudeLabelText))]
    internal Label Label21;
    [ApplTextBinding(nameof(IOrtsVerViewModel.ConverterLongitudeLabelText))]
    internal Label Label20;
    [ApplTextBinding(nameof(IOrtsVerViewModel.ConverterHeaderLabelText))]
    internal Label Label19;
    [ApplTextBinding(nameof(IOrtsVerViewModel.SearchPromptLabelText))]
    [VisibilityBinding(nameof(IOrtsVerViewModel.Label22_Visible))]
    internal Label Label22;
    [VisibilityBinding(nameof(IOrtsVerViewModel.Panel1_Visible))]
    internal Panel Panel1;
    internal Label Label24;
    internal Label Label23;
    [TextBinding(nameof(IOrtsVerViewModel.Label25_Text))]
    internal Label Label25;
    [TextBinding(nameof(IOrtsVerViewModel.Label28_Text))]
    internal Label Label28;
    [TextBinding(nameof(IOrtsVerViewModel.Label29_Text))]
    internal Label Label29;
    [TextBinding(nameof(IOrtsVerViewModel.Label31_Text))]
    internal Label Label31;
    [TextBinding(nameof(IOrtsVerViewModel.Label30_Text))]
    internal Label Label30;
    [TextBinding(nameof(IOrtsVerViewModel.Label33_Text))]
    internal Label Label33;
    internal Label Label34;
    [TextBinding(nameof(IOrtsVerViewModel.Label35_Text))]
    internal Label Label35;
    internal CheckBox CheckBox1;


}


