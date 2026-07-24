using GenFreeWin.ViewModels.Interfaces;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;
using Views;
using static System.Net.Mime.MediaTypeNames;

namespace GenFreeWin.Views;

public partial class HGakte
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

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.edtNumber = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.edtPlace = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.edtUnion = new System.Windows.Forms.TextBox();
            this.edtClass = new System.Windows.Forms.TextBox();
            this.edtFireInsureance = new System.Windows.Forms.TextBox();
            this.edtAdditional = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.btnMainmenue = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnShowUsage = new System.Windows.Forms.Button();
            this.btnNextEntry = new System.Windows.Forms.Button();
            this.btnPrevEntry = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.Label8 = new System.Windows.Forms.Label();
            this.btnEnterNew2 = new System.Windows.Forms.Button();
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEnterNew = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lstUsageList = new System.Windows.Forms.ListBox();
            this.GroupBoxUsage = new System.Windows.Forms.GroupBox();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnClose2 = new System.Windows.Forms.Button();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.edtFlurstueck = new System.Windows.Forms.TextBox();
            this.edtParzelle = new System.Windows.Forms.TextBox();
            this.Frame1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.GroupBoxUsage.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // edtNumber
            // 
            this.edtNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtNumber.Enabled = false;
            this.edtNumber.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.edtNumber.Location = new System.Drawing.Point(123, 12);
            this.edtNumber.Name = "edtNumber";
            this.edtNumber.Size = new System.Drawing.Size(72, 33);
            this.edtNumber.TabIndex = 0;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label1.Location = new System.Drawing.Point(12, 12);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(105, 22);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Hof/Grundakte";
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label2.Location = new System.Drawing.Point(201, 12);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(108, 22);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Verwaltungsort";
            // 
            // edtPlace
            // 
            this.edtPlace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtPlace.Location = new System.Drawing.Point(315, 12);
            this.edtPlace.Name = "edtPlace";
            this.edtPlace.Size = new System.Drawing.Size(178, 20);
            this.edtPlace.TabIndex = 3;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label3.Location = new System.Drawing.Point(499, 12);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(124, 22);
            this.Label3.TabIndex = 4;
            this.Label3.Text = "Bauernschaft/Ortsteil";
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label4.Location = new System.Drawing.Point(779, 12);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(74, 18);
            this.Label4.TabIndex = 5;
            this.Label4.Text = "Hofklasse";
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label5.Location = new System.Drawing.Point(24, 70);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(480, 18);
            this.Label5.TabIndex = 6;
            this.Label5.Text = "Brandkasse";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // edtUnion
            // 
            this.edtUnion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtUnion.Location = new System.Drawing.Point(629, 12);
            this.edtUnion.Name = "edtUnion";
            this.edtUnion.Size = new System.Drawing.Size(144, 20);
            this.edtUnion.TabIndex = 7;
            // 
            // edtClass
            // 
            this.edtClass.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtClass.Location = new System.Drawing.Point(859, 12);
            this.edtClass.Name = "edtClass";
            this.edtClass.Size = new System.Drawing.Size(144, 20);
            this.edtClass.TabIndex = 8;
            // 
            // edtFireInsureance
            // 
            this.edtFireInsureance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtFireInsureance.Location = new System.Drawing.Point(24, 93);
            this.edtFireInsureance.Multiline = true;
            this.edtFireInsureance.Name = "edtFireInsureance";
            this.edtFireInsureance.Size = new System.Drawing.Size(480, 200);
            this.edtFireInsureance.TabIndex = 9;
            // 
            // edtAdditional
            // 
            this.edtAdditional.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtAdditional.Location = new System.Drawing.Point(528, 93);
            this.edtAdditional.Multiline = true;
            this.edtAdditional.Name = "edtAdditional";
            this.edtAdditional.Size = new System.Drawing.Size(480, 200);
            this.edtAdditional.TabIndex = 10;
            // 
            // Label6
            // 
            this.Label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label6.Location = new System.Drawing.Point(528, 70);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(480, 18);
            this.Label6.TabIndex = 11;
            this.Label6.Text = "sonstiges";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ComboBox1
            // 
            this.ComboBox1.FormattingEnabled = true;
            this.ComboBox1.Location = new System.Drawing.Point(137, 322);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(823, 27);
            this.ComboBox1.Sorted = true;
            this.ComboBox1.TabIndex = 12;
            this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label7.Location = new System.Drawing.Point(21, 322);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(110, 25);
            this.Label7.TabIndex = 13;
            this.Label7.Text = "Besitzwechsel";
            this.Label7.Click += new System.EventHandler(this.Label7_Click);
            // 
            // btnMainmenue
            // 
            this.btnMainmenue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnMainmenue.Location = new System.Drawing.Point(859, 677);
            this.btnMainmenue.Name = "btnMainmenue";
            this.btnMainmenue.Size = new System.Drawing.Size(101, 34);
            this.btnMainmenue.TabIndex = 14;
            this.btnMainmenue.Text = "Hauptmenü";
            this.btnMainmenue.UseVisualStyleBackColor = false;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnBack.Location = new System.Drawing.Point(778, 677);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 34);
            this.btnBack.TabIndex = 15;
            this.btnBack.Text = "Zurück";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // btnShowUsage
            // 
            this.btnShowUsage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnShowUsage.Location = new System.Drawing.Point(15, 677);
            this.btnShowUsage.Name = "btnShowUsage";
            this.btnShowUsage.Size = new System.Drawing.Size(102, 34);
            this.btnShowUsage.TabIndex = 16;
            this.btnShowUsage.Text = "Verwendung";
            this.btnShowUsage.UseVisualStyleBackColor = false;
            // 
            // btnNextEntry
            // 
            this.btnNextEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnNextEntry.Location = new System.Drawing.Point(146, 677);
            this.btnNextEntry.Name = "btnNextEntry";
            this.btnNextEntry.Size = new System.Drawing.Size(98, 34);
            this.btnNextEntry.TabIndex = 17;
            this.btnNextEntry.Text = "vor blättern";
            this.btnNextEntry.UseVisualStyleBackColor = false;
            // 
            // btnPrevEntry
            // 
            this.btnPrevEntry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnPrevEntry.Location = new System.Drawing.Point(250, 677);
            this.btnPrevEntry.Name = "btnPrevEntry";
            this.btnPrevEntry.Size = new System.Drawing.Size(125, 34);
            this.btnPrevEntry.TabIndex = 18;
            this.btnPrevEntry.Text = "zurück blättern";
            this.btnPrevEntry.UseVisualStyleBackColor = false;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnSearch.Location = new System.Drawing.Point(516, 677);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 34);
            this.btnSearch.TabIndex = 19;
            this.btnSearch.Text = "suchen";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // Label8
            // 
            this.Label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label8.Location = new System.Drawing.Point(873, 450);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(131, 22);
            this.Label8.TabIndex = 20;
            // 
            // btnEnterNew2
            // 
            this.btnEnterNew2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnEnterNew2.Location = new System.Drawing.Point(381, 677);
            this.btnEnterNew2.Name = "btnEnterNew2";
            this.btnEnterNew2.Size = new System.Drawing.Size(129, 34);
            this.btnEnterNew2.TabIndex = 21;
            this.btnEnterNew2.Text = "neu eingeben";
            this.btnEnterNew2.UseVisualStyleBackColor = false;
            // 
            // Frame1
            // 
            this.Frame1.AutoSize = true;
            this.Frame1.BackColor = System.Drawing.Color.Red;
            this.Frame1.Controls.Add(this.tableLayoutPanel1);
            this.Frame1.Font = new System.Drawing.Font("Arial", 8.5F);
            this.Frame1.ForeColor = System.Drawing.Color.White;
            this.Frame1.Location = new System.Drawing.Point(273, 236);
            this.Frame1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Frame1.Name = "Frame1";
            this.Frame1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(500, 298);
            this.Frame1.TabIndex = 57;
            this.Frame1.TabStop = false;
            this.Frame1.Text = "Grundbucheintrag";
            this.Frame1.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.btnEdit, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnEnterNew, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(490, 270);
            this.tableLayoutPanel1.TabIndex = 62;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.btnCancel, 2);
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.5F);
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(132, 145);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnCancel.Size = new System.Drawing.Size(224, 115);
            this.btnCancel.TabIndex = 61;
            this.btnCancel.Text = "abbrechen";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnEnterNew
            // 
            this.btnEnterNew.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.btnEnterNew, 2);
            this.btnEnterNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEnterNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEnterNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEnterNew.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEnterNew.Location = new System.Drawing.Point(254, 10);
            this.btnEnterNew.Margin = new System.Windows.Forms.Padding(10);
            this.btnEnterNew.Name = "btnEnterNew";
            this.btnEnterNew.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEnterNew.Size = new System.Drawing.Size(226, 115);
            this.btnEnterNew.TabIndex = 58;
            this.btnEnterNew.Text = "&neu eingeben";
            this.btnEnterNew.UseVisualStyleBackColor = false;
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.btnEdit, 2);
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEdit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnEdit.Location = new System.Drawing.Point(10, 10);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(10);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnEdit.Size = new System.Drawing.Size(224, 115);
            this.btnEdit.TabIndex = 57;
            this.btnEdit.Text = "bearbeiten";
            this.btnEdit.UseVisualStyleBackColor = false;
            // 
            // lstUsageList
            // 
            this.lstUsageList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsageList.FormattingEnabled = true;
            this.lstUsageList.ItemHeight = 19;
            this.lstUsageList.Location = new System.Drawing.Point(3, 23);
            this.lstUsageList.Name = "lstUsageList";
            this.lstUsageList.Size = new System.Drawing.Size(471, 266);
            this.lstUsageList.TabIndex = 58;
            this.lstUsageList.DoubleClick += new System.EventHandler(this.ListBox1_DoubleClick);
            // 
            // GroupBoxUsage
            // 
            this.GroupBoxUsage.Controls.Add(this.pnlBottom);
            this.GroupBoxUsage.Controls.Add(this.lstUsageList);
            this.GroupBoxUsage.Location = new System.Drawing.Point(607, 584);
            this.GroupBoxUsage.Name = "GroupBoxUsage";
            this.GroupBoxUsage.Size = new System.Drawing.Size(477, 292);
            this.GroupBoxUsage.TabIndex = 59;
            this.GroupBoxUsage.TabStop = false;
            this.GroupBoxUsage.Text = "Verwendung:";
            this.GroupBoxUsage.Visible = false;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnClose2);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(3, 253);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(471, 36);
            this.pnlBottom.TabIndex = 60;
            // 
            // btnClose2
            // 
            this.btnClose2.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose2.Location = new System.Drawing.Point(245, 0);
            this.btnClose2.Name = "btnClose2";
            this.btnClose2.Size = new System.Drawing.Size(226, 36);
            this.btnClose2.TabIndex = 59;
            this.btnClose2.Text = "schließen";
            this.btnClose2.UseVisualStyleBackColor = true;
            // 
            // Label11
            // 
            this.Label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label11.Location = new System.Drawing.Point(525, 40);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(86, 18);
            this.Label11.TabIndex = 62;
            this.Label11.Text = "Parzelle";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label12
            // 
            this.Label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Label12.Location = new System.Drawing.Point(24, 39);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(58, 18);
            this.Label12.TabIndex = 63;
            this.Label12.Text = "Flur";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // edtFlurstueck
            // 
            this.edtFlurstueck.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtFlurstueck.Location = new System.Drawing.Point(88, 39);
            this.edtFlurstueck.Name = "edtFlurstueck";
            this.edtFlurstueck.Size = new System.Drawing.Size(189, 20);
            this.edtFlurstueck.TabIndex = 64;
            // 
            // edtParzelle
            // 
            this.edtParzelle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtParzelle.Location = new System.Drawing.Point(607, 40);
            this.edtParzelle.Name = "edtParzelle";
            this.edtParzelle.Size = new System.Drawing.Size(195, 20);
            this.edtParzelle.TabIndex = 65;
            // 
            // HGakte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 723);
            this.Controls.Add(this.GroupBoxUsage);
            this.Controls.Add(this.edtParzelle);
            this.Controls.Add(this.edtFlurstueck);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.btnEnterNew2);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnPrevEntry);
            this.Controls.Add(this.btnNextEntry);
            this.Controls.Add(this.btnShowUsage);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnMainmenue);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.ComboBox1);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.edtAdditional);
            this.Controls.Add(this.edtFireInsureance);
            this.Controls.Add(this.edtClass);
            this.Controls.Add(this.edtUnion);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.edtPlace);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.edtNumber);
            this.Font = new System.Drawing.Font("Arial", 8.5F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "HGakte";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Hof_Grundakte: Testversion, noch unter Entwicklung";
            this.Frame1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.GroupBoxUsage.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }


    internal ComboBox ComboBox1;
    internal Label Label7;
    [CommandBinding(nameof(IHGAkteViewModel.MainMenueCommand))]
    internal Button btnMainmenue;
    [CommandBinding(nameof(IHGAkteViewModel.BackCommand))]
    internal Button btnBack;
    [CommandBinding(nameof(IHGAkteViewModel.ShowUsageCommand))]
    internal Button btnShowUsage;
    [CommandBinding(nameof(IHGAkteViewModel.NextEntryCommand))]
    internal Button btnNextEntry;
    [CommandBinding(nameof(IHGAkteViewModel.PrevEntryCommand))]
    internal Button btnPrevEntry;
    [CommandBinding(nameof(IHGAkteViewModel.SearchCommand))]
    internal Button btnSearch;
    [CommandBinding(nameof(IHGAkteViewModel.EnterNew2Command))]
    internal Button btnEnterNew2;
    [CommandBinding(nameof(IHGAkteViewModel.CancelEntryCommand))]
    public Button btnCancel;
    [CommandBinding(nameof(IHGAkteViewModel.NewEntryCommand))]
    public Button btnEnterNew;
    [CommandBinding(nameof(IHGAkteViewModel.EditEntryCommand))]
    public Button btnEdit;
    [VisibilityBinding(nameof(IHGAkteViewModel.Usage_Visible))]
    internal GroupBox GroupBoxUsage;
    internal ListBox lstUsageList;
    [CommandBinding(nameof(IHGAkteViewModel.CloseUsageCommand))]
    internal Button btnClose2;
    internal Label Label1;
    internal Label Label2;
    internal Label Label3;
    internal Label Label4;
    internal Label Label5;
    internal Label Label6;
    internal Label Label8;
    [VisibilityBinding(nameof(IHGAkteViewModel.Frame1_Visible))]
    public GroupBox Frame1;
    internal Label Label11;
    internal Label Label12;
    [TextBinding(nameof(IHGAkteViewModel.Number_Text))]
    internal TextBox edtNumber;
    [TextBinding(nameof(IHGAkteViewModel.Union_Text))]
    internal TextBox edtUnion;
    [TextBinding(nameof(IHGAkteViewModel.Place_Text))]
    internal TextBox edtPlace;
    [TextBinding(nameof(IHGAkteViewModel.Class_Text))]
    internal TextBox edtClass;
    [TextBinding(nameof(IHGAkteViewModel.Flurstueck_Text))]
    internal TextBox edtFlurstueck;
    [TextBinding(nameof(IHGAkteViewModel.Parzelle_Text))]
    internal TextBox edtParzelle;
    [TextBinding(nameof(IHGAkteViewModel.FireInsurance_Text))]
    internal TextBox edtFireInsureance;
    [TextBinding(nameof(IHGAkteViewModel.Additional_Text))]
    internal TextBox edtAdditional;
    private Panel pnlBottom;
    private TableLayoutPanel tableLayoutPanel1;
}