using GenFree.ViewModels.Interfaces;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

public partial class Mand
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
            this.Label16 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.Laufwerk1 = new System.Windows.Forms.ListBox();
            this.List1 = new System.Windows.Forms.ListBox();
            this.BezMAND = new System.Windows.Forms.Label();
            this.cmdNewMandant = new System.Windows.Forms.Button();
            this.cmdDeleteMandant = new System.Windows.Forms.Button();
            this.Befehl2 = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Bez1 = new System.Windows.Forms.Label();
            this.Bez2 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.Command2 = new System.Windows.Forms.Button();
            this.Command1 = new System.Windows.Forms.Button();
            this.edtNewMandant = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.Frame1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label16
            // 
            this.Label16.BackColor = System.Drawing.Color.Red;
            this.Label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label16.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label16.ForeColor = System.Drawing.Color.Yellow;
            this.Label16.Location = new System.Drawing.Point(1, 20);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(940, 20);
            this.Label16.TabIndex = 40;
            this.Label16.Text = "Label16";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label17
            // 
            this.Label17.BackColor = System.Drawing.Color.Red;
            this.Label17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label17.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label17.ForeColor = System.Drawing.Color.Yellow;
            this.Label17.Location = new System.Drawing.Point(1, 40);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(940, 20);
            this.Label17.TabIndex = 39;
            this.Label17.Text = "Label17";
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label15
            // 
            this.Label15.BackColor = System.Drawing.Color.Red;
            this.Label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Label15.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label15.ForeColor = System.Drawing.Color.Yellow;
            this.Label15.Location = new System.Drawing.Point(1, 0);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(940, 20);
            this.Label15.TabIndex = 38;
            this.Label15.Text = "Gen_Plus das Genealogieprogramm mit den Pluspunkten";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Laufwerk1
            // 
            this.Laufwerk1.FormattingEnabled = true;
            this.Laufwerk1.Location = new System.Drawing.Point(22, 72);
            this.Laufwerk1.Name = "Laufwerk1";
            this.Laufwerk1.Size = new System.Drawing.Size(258, 27);
            this.Laufwerk1.TabIndex = 41;
            this.Laufwerk1.SelectedIndexChanged += new System.EventHandler(this.Laufwerk1_SelectedIndexChanged);
            // 
            // List1
            // 
            this.List1.FormattingEnabled = true;
            this.List1.ItemHeight = 19;
            this.List1.Location = new System.Drawing.Point(22, 139);
            this.List1.Name = "List1";
            this.List1.Size = new System.Drawing.Size(207, 517);
            this.List1.TabIndex = 43;
            this.List1.Click += new System.EventHandler(this.List1_Click);
            this.List1.DoubleClick += new System.EventHandler(this.List1_DoubleClick);
            // 
            // BezMAND
            // 
            this.BezMAND.AutoSize = true;
            this.BezMAND.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BezMAND.Location = new System.Drawing.Point(311, 503);
            this.BezMAND.Name = "BezMAND";
            this.BezMAND.Size = new System.Drawing.Size(0, 19);
            this.BezMAND.TabIndex = 44;
            // 
            // cmdNewMandant
            // 
            this.cmdNewMandant.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.cmdNewMandant.Location = new System.Drawing.Point(560, 72);
            this.cmdNewMandant.Name = "cmdNewMandant";
            this.cmdNewMandant.Size = new System.Drawing.Size(205, 36);
            this.cmdNewMandant.TabIndex = 45;
            this.cmdNewMandant.Text = "&Neuen Mandanten anlegen";
            this.cmdNewMandant.UseVisualStyleBackColor = false;
            this.cmdNewMandant.Click += new System.EventHandler(this._CmdNewMandant_Click);
            // 
            // cmdDeleteMandant
            // 
            this.cmdDeleteMandant.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.cmdDeleteMandant.Enabled = false;
            this.cmdDeleteMandant.Location = new System.Drawing.Point(560, 128);
            this.cmdDeleteMandant.Name = "cmdDeleteMandant";
            this.cmdDeleteMandant.Size = new System.Drawing.Size(205, 36);
            this.cmdDeleteMandant.TabIndex = 46;
            this.cmdDeleteMandant.Text = "&Mandanten löschen";
            this.cmdDeleteMandant.UseVisualStyleBackColor = false;
            this.cmdDeleteMandant.Click += new System.EventHandler(this._CmdDeleteMandant_Click);
            // 
            // Befehl2
            // 
            this.Befehl2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Befehl2.Location = new System.Drawing.Point(880, 673);
            this.Befehl2.Name = "Befehl2";
            this.Befehl2.Size = new System.Drawing.Size(116, 28);
            this.Befehl2.TabIndex = 47;
            this.Befehl2.Text = "Hauptmenü";
            this.Befehl2.UseVisualStyleBackColor = false;
            this.Befehl2.Click += new System.EventHandler(this.Befehl2_Click);
            // 
            // lblState
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(19, 118);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(0, 19);
            this.Label2.TabIndex = 48;
            // 
            // lblEnterLicence
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(19, 101);
            this.Label1.Name = "lblRepoName";
            this.Label1.Size = new System.Drawing.Size(112, 19);
            this.Label1.TabIndex = 49;
            this.Label1.Text = "Mandanten in:";
            // 
            // lblDisplayHint
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(74, 679);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(559, 19);
            this.Label3.TabIndex = 50;
            this.Label3.Text = "Klick auf Mandanten zeigt Dateinformationen - Doppelklick wählt Mandanten";
            // 
            // Bez1
            // 
            this.Bez1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Bez1.Location = new System.Drawing.Point(309, 138);
            this.Bez1.Name = "Bez1";
            this.Bez1.Size = new System.Drawing.Size(80, 18);
            this.Bez1.TabIndex = 51;
            this.Bez1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Bez2
            // 
            this.Bez2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Bez2.Location = new System.Drawing.Point(309, 161);
            this.Bez2.Name = "Bez2";
            this.Bez2.Size = new System.Drawing.Size(80, 17);
            this.Bez2.TabIndex = 52;
            this.Bez2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSearch
            // 
            this.Label4.AutoSize = true;
            this.Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label4.Location = new System.Drawing.Point(395, 161);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(70, 19);
            this.Label4.TabIndex = 53;
            this.Label4.Text = "Familien";
            // 
            // lblSorting
            // 
            this.Label5.AutoSize = true;
            this.Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label5.Location = new System.Drawing.Point(395, 139);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(79, 19);
            this.Label5.TabIndex = 54;
            this.Label5.Text = "Personen";
            // 
            // lblEMail
            // 
            this.Label6.AutoSize = true;
            this.Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label6.Location = new System.Drawing.Point(309, 118);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(128, 19);
            this.Label6.TabIndex = 55;
            this.Label6.Text = "Mandant enthält:";
            // 
            // lblURL
            // 
            this.Label7.AutoSize = true;
            this.Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.Label7.Location = new System.Drawing.Point(311, 475);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(143, 19);
            this.Label7.TabIndex = 56;
            this.Label7.Text = "Aktueller Mandant:";
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Frame1.Controls.Add(this.Label9);
            this.Frame1.Controls.Add(this.Command2);
            this.Frame1.Controls.Add(this.Command1);
            this.Frame1.Controls.Add(this.edtNewMandant);
            this.Frame1.Controls.Add(this.Label8);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1.Location = new System.Drawing.Point(337, 244);
            this.Frame1.Name = "Frame1";
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(310, 111);
            this.Frame1.TabIndex = 57;
            this.Frame1.TabStop = false;
            this.Frame1.Text = "Name der neuen Genealogie";
            this.Frame1.Visible = false;
            // 
            // lblOccubation
            // 
            this.Label9.Location = new System.Drawing.Point(6, 63);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(136, 17);
            this.Label9.TabIndex = 22;
            // 
            // btnEdit
            // 
            this.Command2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Command2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Command2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Command2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command2.Location = new System.Drawing.Point(175, 22);
            this.Command2.Name = "Command2";
            this.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Command2.Size = new System.Drawing.Size(81, 30);
            this.Command2.TabIndex = 21;
            this.Command2.Text = "Abbruch";
            this.Command2.UseVisualStyleBackColor = false;
            // 
            // btnEnterNew
            // 
            this.Command1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.Command1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Command1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Command1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Command1.Location = new System.Drawing.Point(19, 22);
            this.Command1.Name = "Command1";
            this.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Command1.Size = new System.Drawing.Size(86, 30);
            this.Command1.TabIndex = 20;
            this.Command1.Text = "OK";
            this.Command1.UseVisualStyleBackColor = false;
            this.Command1.Click += new System.EventHandler(this.Command1_Click);
            // 
            // edtNewMandant
            // 
            this.edtNewMandant.AcceptsReturn = true;
            this.edtNewMandant.BackColor = System.Drawing.Color.White;
            this.edtNewMandant.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtNewMandant.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.edtNewMandant.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.edtNewMandant.ForeColor = System.Drawing.SystemColors.WindowText;
            this.edtNewMandant.Location = new System.Drawing.Point(140, 60);
            this.edtNewMandant.MaxLength = 0;
            this.edtNewMandant.Multiline = true;
            this.edtNewMandant.Name = "edtNewMandant";
            this.edtNewMandant.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.edtNewMandant.Size = new System.Drawing.Size(149, 20);
            this.edtNewMandant.TabIndex = 19;
            this.edtNewMandant.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Text1_KeyPress);
            // 
            // lblResidence
            // 
            this.Label8.AutoSize = true;
            this.Label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label8.Location = new System.Drawing.Point(118, 66);
            this.Label8.Name = "Label8";
            this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label8.Size = new System.Drawing.Size(0, 19);
            this.Label8.TabIndex = 18;
            this.Label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lstUsageList
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.ItemHeight = 19;
            this.ListBox1.Location = new System.Drawing.Point(683, 178);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(258, 232);
            this.ListBox1.TabIndex = 58;
            this.ListBox1.Visible = false;
            // 
            // Mand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(1018, 725);
            this.ControlBox = false;
            this.Controls.Add(this.ListBox1);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Bez2);
            this.Controls.Add(this.Bez1);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Befehl2);
            this.Controls.Add(this.cmdDeleteMandant);
            this.Controls.Add(this.cmdNewMandant);
            this.Controls.Add(this.BezMAND);
            this.Controls.Add(this.List1);
            this.Controls.Add(this.Laufwerk1);
            this.Controls.Add(this.Label16);
            this.Controls.Add(this.Label17);
            this.Controls.Add(this.Label15);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Mand";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Mandantenverwaltung";
            this.Frame1.ResumeLayout(false);
            this.Frame1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }


    public Label Label1;
    public Label Label2;
    public Label Label3;
    public Label Label4;
    public Label Label5;
    public Label Label6;
    public Label Label7;
    public Label Label8;
    public Label Label9;
    public Label Label16;
    public Label Label17;
    public Label Label15;
    public Label Bez1;
    public Label Bez2;
    public Label BezMAND;
    public TextBox edtNewMandant;
#pragma warning disable CS0618 // Typ oder Element ist veraltet

    public ListBox Laufwerk1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet
    public ListBox List1;
    public Button cmdNewMandant;
    public Button cmdDeleteMandant;
    public Button Befehl2;
    public Button Command1;
    public Button Command2;
    public ListBox ListBox1;
    [VisibilityBinding(nameof(IMandViewModel.Frame1_Visible))]
    public GroupBox Frame1;

}
