using BaseLib.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GenFreeWin.Views;

partial class RechText
{

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
            this._Command1_2 = new System.Windows.Forms.Button();
            this._Command1_1 = new System.Windows.Forms.Button();
            this._Command1_0 = new System.Windows.Forms.Button();
            this._Bef_4 = new System.Windows.Forms.Button();
            this.List4 = new System.Windows.Forms.ListBox();
            this.Text2 = new System.Windows.Forms.TextBox();
            this.List3 = new System.Windows.Forms.ListBox();
            this.List2 = new System.Windows.Forms.ListBox();
            this._Bef_0 = new System.Windows.Forms.Button();
            this.Liste1 = new System.Windows.Forms.ListBox();
            this.Text1 = new System.Windows.Forms.TextBox();
            this._Bef_1 = new System.Windows.Forms.Button();
            this._Bef_2 = new System.Windows.Forms.Button();
            this._Bef_3 = new System.Windows.Forms.Button();
            this.List1 = new System.Windows.Forms.ListBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Bezeichnung4 = new System.Windows.Forms.Label();
            this.Bezeichnung1 = new System.Windows.Forms.Label();
            this.Bezeichnung6 = new System.Windows.Forms.Label();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label23 = new System.Windows.Forms.Label();
            this.Label24 = new System.Windows.Forms.Label();
            this.Label25 = new System.Windows.Forms.Label();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _Command1_2
            // 
            this._Command1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_2.Location = new System.Drawing.Point(424, 689);
            this._Command1_2.Name = "_Command1_2";
            this._Command1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_2.Size = new System.Drawing.Size(237, 22);
            this._Command1_2.TabIndex = 39;
            this._Command1_2.Text = "Speichern für Gedcom-Ausgabe";
            this._Command1_2.UseVisualStyleBackColor = false;
            // 
            // _Command1_1
            // 
            this._Command1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_1.Location = new System.Drawing.Point(7, 688);
            this._Command1_1.Name = "_Command1_1";
            this._Command1_1.Size = new System.Drawing.Size(198, 22);
            this._Command1_1.TabIndex = 38;
            this._Command1_1.Text = "Einschränken auf männlich";
            this._Command1_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._Command1_1.UseVisualStyleBackColor = false;
            // 
            // _Command1_0
            // 
            this._Command1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Command1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Command1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Command1_0.Location = new System.Drawing.Point(211, 689);
            this._Command1_0.Name = "_Command1_0";
            this._Command1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Command1_0.Size = new System.Drawing.Size(207, 22);
            this._Command1_0.TabIndex = 37;
            this._Command1_0.Text = "Einschränken auf weiblich";
            this._Command1_0.UseVisualStyleBackColor = false;
            // 
            // _Bef_4
            // 
            this._Bef_4.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_4.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_4.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._Bef_4.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_4.Location = new System.Drawing.Point(327, 660);
            this._Bef_4.Name = "_Bef_4";
            this._Bef_4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_4.Size = new System.Drawing.Size(91, 22);
            this._Bef_4.TabIndex = 36;
            this._Bef_4.Text = "&Drucken";
            this._Bef_4.UseVisualStyleBackColor = false;
            // 
            // List4
            // 
            this.List4.BackColor = System.Drawing.SystemColors.Window;
            this.List4.Cursor = System.Windows.Forms.Cursors.Default;
            this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List4.ItemHeight = 19;
            this.List4.Location = new System.Drawing.Point(812, 79);
            this.List4.Name = "List4";
            this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List4.Size = new System.Drawing.Size(204, 536);
            this.List4.TabIndex = 34;
            // 
            // Text2
            // 
            this.Text2.AcceptsReturn = true;
            this.Text2.BackColor = System.Drawing.SystemColors.Window;
            this.Text2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text2.Location = new System.Drawing.Point(849, 28);
            this.Text2.MaxLength = 0;
            this.Text2.Name = "Text2";
            this.Text2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text2.Size = new System.Drawing.Size(131, 27);
            this.Text2.TabIndex = 30;
            // 
            // List3
            // 
            this.List3.BackColor = System.Drawing.SystemColors.Window;
            this.List3.Cursor = System.Windows.Forms.Cursors.Default;
            this.List3.Font = new System.Drawing.Font("Courier New", 8.5F);
            this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List3.ItemHeight = 20;
            this.List3.Location = new System.Drawing.Point(5, 80);
            this.List3.Name = "List3";
            this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List3.Size = new System.Drawing.Size(803, 544);
            this.List3.TabIndex = 29;
            this.List3.DoubleClick += new System.EventHandler(this.List3_DoubleClick);
            // 
            // List2
            // 
            this.List2.BackColor = System.Drawing.SystemColors.Window;
            this.List2.Cursor = System.Windows.Forms.Cursors.Default;
            this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List2.ItemHeight = 19;
            this.List2.Location = new System.Drawing.Point(462, 612);
            this.List2.Name = "List2";
            this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List2.Size = new System.Drawing.Size(497, 4);
            this.List2.TabIndex = 28;
            this.List2.Visible = false;
            // 
            // _Bef_0
            // 
            this._Bef_0.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_0.Enabled = false;
            this._Bef_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_0.Location = new System.Drawing.Point(8, 660);
            this._Bef_0.Name = "_Bef_0";
            this._Bef_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_0.Size = new System.Drawing.Size(94, 22);
            this._Bef_0.TabIndex = 25;
            this._Bef_0.Text = "neue Suche";
            this._Bef_0.UseVisualStyleBackColor = false;
            // 
            // Liste1
            // 
            this.Liste1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Liste1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Liste1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Liste1.ForeColor = System.Drawing.Color.Black;
            this.Liste1.ItemHeight = 19;
            this.Liste1.Location = new System.Drawing.Point(812, 79);
            this.Liste1.Name = "Liste1";
            this.Liste1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Liste1.Size = new System.Drawing.Size(192, 534);
            this.Liste1.Sorted = true;
            this.Liste1.TabIndex = 0;
            this.Liste1.DoubleClick += new System.EventHandler(this.Liste1_DoubleClick);
            // 
            // Text1
            // 
            this.Text1.AcceptsReturn = true;
            this.Text1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Text1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Text1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text1.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.Text1.ForeColor = System.Drawing.Color.Black;
            this.Text1.Location = new System.Drawing.Point(5, 632);
            this.Text1.MaxLength = 0;
            this.Text1.Name = "Text1";
            this.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text1.Size = new System.Drawing.Size(405, 30);
            this.Text1.TabIndex = 4;
            // 
            // _Bef_1
            // 
            this._Bef_1.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_1.Enabled = false;
            this._Bef_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_1.Location = new System.Drawing.Point(108, 660);
            this._Bef_1.Name = "_Bef_1";
            this._Bef_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_1.Size = new System.Drawing.Size(105, 22);
            this._Bef_1.TabIndex = 5;
            this._Bef_1.Text = "Einsch&ränken";
            this._Bef_1.UseVisualStyleBackColor = false;
            // 
            // _Bef_2
            // 
            this._Bef_2.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_2.Location = new System.Drawing.Point(764, 689);
            this._Bef_2.Name = "_Bef_2";
            this._Bef_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_2.Size = new System.Drawing.Size(167, 22);
            this._Bef_2.TabIndex = 9;
            this._Bef_2.Text = "&Menue";
            this._Bef_2.UseVisualStyleBackColor = false;
            // 
            // _Bef_3
            // 
            this._Bef_3.BackColor = System.Drawing.SystemColors.Control;
            this._Bef_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Bef_3.Enabled = false;
            this._Bef_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Bef_3.Location = new System.Drawing.Point(219, 660);
            this._Bef_3.Name = "_Bef_3";
            this._Bef_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Bef_3.Size = new System.Drawing.Size(78, 22);
            this._Bef_3.TabIndex = 12;
            this._Bef_3.Text = "&Erweitern";
            this._Bef_3.UseVisualStyleBackColor = false;
            this._Bef_3.Click += new System.EventHandler(this._Bef_3_Click);
            // 
            // List1
            // 
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 19;
            this.List1.Location = new System.Drawing.Point(539, 510);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List1.Size = new System.Drawing.Size(494, 42);
            this.List1.Sorted = true;
            this.List1.TabIndex = 27;
            this.List1.Visible = false;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(809, 582);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(104, 27);
            this.Label2.TabIndex = 40;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.Control;
            this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.Location = new System.Drawing.Point(780, 31);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label1.Size = new System.Drawing.Size(72, 18);
            this.Label1.TabIndex = 35;
            this.Label1.Text = "Start mit:";
            // 
            // Bezeichnung4
            // 
            this.Bezeichnung4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Bezeichnung4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung4.ForeColor = System.Drawing.Color.Black;
            this.Bezeichnung4.Location = new System.Drawing.Point(4, 60);
            this.Bezeichnung4.Name = "Bezeichnung4";
            this.Bezeichnung4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung4.Size = new System.Drawing.Size(267, 17);
            this.Bezeichnung4.TabIndex = 7;
            // 
            // Bezeichnung1
            // 
            this.Bezeichnung1.BackColor = System.Drawing.SystemColors.Control;
            this.Bezeichnung1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Bezeichnung1.Location = new System.Drawing.Point(281, 478);
            this.Bezeichnung1.Name = "Bezeichnung1";
            this.Bezeichnung1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung1.Size = new System.Drawing.Size(182, 18);
            this.Bezeichnung1.TabIndex = 26;
            this.Bezeichnung1.Visible = false;
            // 
            // Bezeichnung6
            // 
            this.Bezeichnung6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Bezeichnung6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Bezeichnung6.Font = new System.Drawing.Font("Arial", 8.5F);
            this.Bezeichnung6.ForeColor = System.Drawing.Color.Black;
            this.Bezeichnung6.Location = new System.Drawing.Point(817, 60);
            this.Bezeichnung6.Name = "Bezeichnung6";
            this.Bezeichnung6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Bezeichnung6.Size = new System.Drawing.Size(187, 16);
            this.Bezeichnung6.TabIndex = 11;
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckBox1.Location = new System.Drawing.Point(772, 648);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(183, 23);
            this.CheckBox1.TabIndex = 41;
            this.CheckBox1.Text = "Auswahl beibehalten";
            this.CheckBox1.UseVisualStyleBackColor = false;
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.Color.Yellow;
            this.Label5.ForeColor = System.Drawing.Color.Black;
            this.Label5.Location = new System.Drawing.Point(2, 9);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(69, 18);
            this.Label5.TabIndex = 43;
            this.Label5.Text = "Namen";
            this.Label5.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label6
            // 
            this.Label6.BackColor = System.Drawing.Color.Yellow;
            this.Label6.ForeColor = System.Drawing.Color.Black;
            this.Label6.Location = new System.Drawing.Point(79, 9);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(136, 18);
            this.Label6.TabIndex = 44;
            this.Label6.Text = "Vornamen weiblich";
            this.Label6.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.Color.Yellow;
            this.Label7.ForeColor = System.Drawing.Color.Black;
            this.Label7.Location = new System.Drawing.Point(223, 9);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(141, 18);
            this.Label7.TabIndex = 45;
            this.Label7.Text = "Vornamen männlich";
            this.Label7.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label8
            // 
            this.Label8.BackColor = System.Drawing.Color.Yellow;
            this.Label8.ForeColor = System.Drawing.Color.Black;
            this.Label8.Location = new System.Drawing.Point(372, 9);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(103, 18);
            this.Label8.TabIndex = 46;
            this.Label8.Text = "Namenspräfix";
            this.Label8.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label9
            // 
            this.Label9.BackColor = System.Drawing.Color.Yellow;
            this.Label9.ForeColor = System.Drawing.Color.Black;
            this.Label9.Location = new System.Drawing.Point(483, 9);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(103, 18);
            this.Label9.TabIndex = 47;
            this.Label9.Text = "Namenssuffix";
            this.Label9.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label10
            // 
            this.Label10.BackColor = System.Drawing.Color.Yellow;
            this.Label10.ForeColor = System.Drawing.Color.Black;
            this.Label10.Location = new System.Drawing.Point(594, 9);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(47, 18);
            this.Label10.TabIndex = 48;
            this.Label10.Text = "Alias";
            this.Label10.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label11
            // 
            this.Label11.BackColor = System.Drawing.Color.Yellow;
            this.Label11.ForeColor = System.Drawing.Color.Black;
            this.Label11.Location = new System.Drawing.Point(649, 9);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(61, 18);
            this.Label11.TabIndex = 49;
            this.Label11.Text = "Sippe";
            this.Label11.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label12
            // 
            this.Label12.BackColor = System.Drawing.Color.Yellow;
            this.Label12.ForeColor = System.Drawing.Color.Black;
            this.Label12.Location = new System.Drawing.Point(718, 9);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(58, 18);
            this.Label12.TabIndex = 50;
            this.Label12.Text = "Berufe";
            this.Label12.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label13
            // 
            this.Label13.BackColor = System.Drawing.Color.Yellow;
            this.Label13.ForeColor = System.Drawing.Color.Black;
            this.Label13.Location = new System.Drawing.Point(784, 9);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(50, 18);
            this.Label13.TabIndex = 51;
            this.Label13.Text = "Titel";
            this.Label13.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label14
            // 
            this.Label14.BackColor = System.Drawing.Color.Yellow;
            this.Label14.ForeColor = System.Drawing.Color.Black;
            this.Label14.Location = new System.Drawing.Point(842, 9);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(121, 18);
            this.Label14.TabIndex = 52;
            this.Label14.Text = "Kurzbemerkung";
            this.Label14.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label15
            // 
            this.Label15.BackColor = System.Drawing.Color.Yellow;
            this.Label15.ForeColor = System.Drawing.Color.Black;
            this.Label15.Location = new System.Drawing.Point(2, 35);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(109, 18);
            this.Label15.TabIndex = 53;
            this.Label15.Text = "Ereignisnamen";
            this.Label15.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label16
            // 
            this.Label16.BackColor = System.Drawing.Color.Yellow;
            this.Label16.ForeColor = System.Drawing.Color.Black;
            this.Label16.Location = new System.Drawing.Point(117, 35);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(59, 18);
            this.Label16.TabIndex = 54;
            this.Label16.Text = "Orte";
            this.Label16.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label17
            // 
            this.Label17.BackColor = System.Drawing.Color.Yellow;
            this.Label17.ForeColor = System.Drawing.Color.Black;
            this.Label17.Location = new System.Drawing.Point(182, 35);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(68, 18);
            this.Label17.TabIndex = 55;
            this.Label17.Text = "Ortsteile";
            this.Label17.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label18
            // 
            this.Label18.BackColor = System.Drawing.Color.Yellow;
            this.Label18.ForeColor = System.Drawing.Color.Black;
            this.Label18.Location = new System.Drawing.Point(256, 35);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(59, 18);
            this.Label18.TabIndex = 56;
            this.Label18.Text = "Kreise";
            this.Label18.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label19
            // 
            this.Label19.BackColor = System.Drawing.Color.Yellow;
            this.Label19.ForeColor = System.Drawing.Color.Black;
            this.Label19.Location = new System.Drawing.Point(321, 35);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(59, 18);
            this.Label19.TabIndex = 57;
            this.Label19.Text = "Länder";
            this.Label19.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label20
            // 
            this.Label20.BackColor = System.Drawing.Color.Yellow;
            this.Label20.ForeColor = System.Drawing.Color.Black;
            this.Label20.Location = new System.Drawing.Point(386, 35);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(59, 18);
            this.Label20.TabIndex = 58;
            this.Label20.Text = "Staaten";
            this.Label20.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label21
            // 
            this.Label21.BackColor = System.Drawing.Color.Yellow;
            this.Label21.ForeColor = System.Drawing.Color.Black;
            this.Label21.Location = new System.Drawing.Point(451, 35);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(68, 18);
            this.Label21.TabIndex = 59;
            this.Label21.Text = "Straßen";
            this.Label21.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label22
            // 
            this.Label22.BackColor = System.Drawing.Color.Yellow;
            this.Label22.ForeColor = System.Drawing.Color.Black;
            this.Label22.Location = new System.Drawing.Point(590, 35);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(48, 18);
            this.Label22.TabIndex = 60;
            this.Label22.Text = "Platz";
            this.Label22.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label23
            // 
            this.Label23.BackColor = System.Drawing.Color.Yellow;
            this.Label23.ForeColor = System.Drawing.Color.Black;
            this.Label23.Location = new System.Drawing.Point(644, 35);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(59, 18);
            this.Label23.TabIndex = 61;
            this.Label23.Text = "Status";
            this.Label23.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label24
            // 
            this.Label24.BackColor = System.Drawing.Color.Yellow;
            this.Label24.ForeColor = System.Drawing.Color.Black;
            this.Label24.Location = new System.Drawing.Point(709, 35);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(66, 18);
            this.Label24.TabIndex = 62;
            this.Label24.Text = "Religion";
            this.Label24.Click += new System.EventHandler(this.Label5_Click);
            // 
            // Label25
            // 
            this.Label25.BackColor = System.Drawing.Color.Yellow;
            this.Label25.ForeColor = System.Drawing.Color.Black;
            this.Label25.Location = new System.Drawing.Point(525, 35);
            this.Label25.Name = "Label25";
            this.Label25.Size = new System.Drawing.Size(59, 18);
            this.Label25.TabIndex = 63;
            this.Label25.Text = "Hausnr.";
            this.Label25.Click += new System.EventHandler(this.Label5_Click);
            // 
            // lstUsageList
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 19;
            this.ListBox1.Location = new System.Drawing.Point(12, 374);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(796, 251);
            this.ListBox1.TabIndex = 64;
            this.ListBox1.Visible = false;
            // 
            // RechText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1008, 715);
            this.ControlBox = false;
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.Label25);
            this.Controls.Add(this.Label24);
            this.Controls.Add(this.Label23);
            this.Controls.Add(this.Label22);
            this.Controls.Add(this.Label21);
            this.Controls.Add(this.Label20);
            this.Controls.Add(this.Label19);
            this.Controls.Add(this.Label18);
            this.Controls.Add(this.Label17);
            this.Controls.Add(this.Label16);
            this.Controls.Add(this.Label15);
            this.Controls.Add(this.Label14);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.CheckBox1);
            this.Controls.Add(this.List3);
            this.Controls.Add(this._Command1_2);
            this.Controls.Add(this._Command1_1);
            this.Controls.Add(this._Command1_0);
            this.Controls.Add(this._Bef_4);
            this.Controls.Add(this.List4);
            this.Controls.Add(this.Text2);
            this.Controls.Add(this.List2);
            this.Controls.Add(this._Bef_0);
            this.Controls.Add(this.Liste1);
            this.Controls.Add(this.Text1);
            this.Controls.Add(this._Bef_1);
            this.Controls.Add(this._Bef_2);
            this.Controls.Add(this._Bef_3);
            this.Controls.Add(this.List1);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Bezeichnung4);
            this.Controls.Add(this.Bezeichnung1);
            this.Controls.Add(this.Bezeichnung6);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 8.5F);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "RechText";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " Personenrecherche";
            this.ResumeLayout(false);
            this.PerformLayout();

    }


    private IContainer components;

    public ToolTip ToolTip1;

    public ListBox List3;

    public ListBox Liste1;

    public Button _Bef_3;
#pragma warning disable CS0618 // Typ oder Element ist veraltet

    public ControlArray<Button> ABef;

    public ControlArray<Button> ACommand1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet

    internal Label Label5;

    internal Label Label6;

    internal Label Label7;

    internal Label Label8;

    internal Label Label9;

    internal Label Label10;

    internal Label Label11;

    internal Label Label12;

    internal Label Label13;

    internal Label Label14;

    internal Label Label15;

    internal Label Label16;

    internal Label Label17;

    internal Label Label18;

    internal Label Label19;

    internal Label Label20;

    internal Label Label21;

    internal Label Label22;

    internal Label Label23;

    internal Label Label24;

    internal Label Label25;

    public Button _Command1_2;
    public Button _Command1_1;
    public Button _Command1_0;
    public Button _Bef_4;
    public ListBox List4;
    public TextBox Text2;


    public ListBox List2;
    public Button _Bef_0;


    public TextBox Text1;
    public Button _Bef_1;
    public Button _Bef_2;


    public ListBox List1;
    public Label Label2;
    public Label Label1;
    public Label Bezeichnung4;
    public Label Bezeichnung1;
    public Label Bezeichnung6;
    public ControlArray<Label> ABez;


    internal CheckBox CheckBox1;

    internal ListBox ListBox1;

}