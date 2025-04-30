using GenFree.ViewModels.Interfaces;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Repo
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
        lblRepoName = new Label();
        lblState = new Label();
        lblDisplayHint = new Label();
        lblSearch = new Label();
        lblSorting = new Label();
        lblEMail = new Label();
        lblURL = new Label();
        edtPredicate = new TextBox();
        TextBox2 = new TextBox();
        TextBox3 = new TextBox();
        TextBox4 = new TextBox();
        TextBox5 = new TextBox();
        TextBox6 = new TextBox();
        RichTextBox1 = new RichTextBox();
        btnSave = new Button();
        btnSave2 = new Button();
        ListBox1 = new ListBox();
        Label8 = new Label();
        btnClose = new Button();
        btnNewEntry = new Button();
        RichTextBox2 = new RichTextBox();
        Button5 = new Button();
        ListBox2 = new ListBox();
        Label9 = new Label();
        SuspendLayout();
        lblRepoName.BackColor = Color.FromArgb(224, 224, 224);
        lblRepoName.Location = new Point(12, 10);
        lblRepoName.Name = "lblRepoName";
        lblRepoName.Size = new Size(100, 22);
        lblRepoName.TabIndex = 0;
        lblRepoName.Text = "Name:";
        lblRepoName.TextAlign = ContentAlignment.MiddleCenter;
        lblState.BackColor = Color.FromArgb(224, 224, 224);
        lblState.Location = new Point(12, 37);
        lblState.Name = "lblState";
        lblState.Size = new Size(100, 22);
        lblState.TabIndex = 1;
        lblState.Text = "Straße:";
        lblState.TextAlign = ContentAlignment.MiddleCenter;
        lblDisplayHint.BackColor = Color.FromArgb(224, 224, 224);
        lblDisplayHint.Location = new Point(12, 63);
        lblDisplayHint.Name = "lblDisplayHint";
        lblDisplayHint.Size = new Size(100, 22);
        lblDisplayHint.TabIndex = 2;
        lblDisplayHint.Text = "Ort:";
        lblDisplayHint.TextAlign = ContentAlignment.MiddleCenter;
        lblSearch.BackColor = Color.FromArgb(224, 224, 224);
        lblSearch.Location = new Point(12, 88);
        lblSearch.Name = "lblSearch";
        lblSearch.Size = new Size(100, 22);
        lblSearch.TabIndex = 3;
        lblSearch.Text = "PLZ";
        lblSearch.TextAlign = ContentAlignment.MiddleCenter;
        lblSorting.BackColor = Color.FromArgb(224, 224, 224);
        lblSorting.Location = new Point(12, 113);
        lblSorting.Name = "lblSorting";
        lblSorting.Size = new Size(100, 22);
        lblSorting.TabIndex = 4;
        lblSorting.Text = "Telefon";
        lblSorting.TextAlign = ContentAlignment.MiddleCenter;
        lblEMail.BackColor = Color.FromArgb(224, 224, 224);
        lblEMail.Location = new Point(12, 139);
        lblEMail.Name = "lblEMail";
        lblEMail.Size = new Size(100, 22);
        lblEMail.TabIndex = 5;
        lblEMail.Text = "E-Mail:";
        lblEMail.TextAlign = ContentAlignment.MiddleCenter;
        lblURL.BackColor = Color.FromArgb(224, 224, 224);
        lblURL.Location = new Point(12, 165);
        lblURL.Name = "lblURL";
        lblURL.Size = new Size(100, 22);
        lblURL.TabIndex = 6;
        lblURL.Text = "Internet:";
        lblURL.TextAlign = ContentAlignment.MiddleCenter;
        edtPredicate.BorderStyle = BorderStyle.None;
        edtPredicate.Location = new Point(118, 10);
        edtPredicate.Multiline = true;
        edtPredicate.Name = "edtPredicate";
        edtPredicate.Size = new Size(576, 22);
        edtPredicate.TabIndex = 8;
        TextBox2.BorderStyle = BorderStyle.None;
        TextBox2.Location = new Point(118, 37);
        TextBox2.Multiline = true;
        TextBox2.Name = "TextBox2";
        TextBox2.Size = new Size(576, 22);
        TextBox2.TabIndex = 9;
        TextBox3.BorderStyle = BorderStyle.None;
        TextBox3.Location = new Point(118, 63);
        TextBox3.Multiline = true;
        TextBox3.Name = "TextBox3";
        TextBox3.Size = new Size(576, 22);
        TextBox3.TabIndex = 10;
        TextBox4.BorderStyle = BorderStyle.None;
        TextBox4.Location = new Point(118, 88);
        TextBox4.Multiline = true;
        TextBox4.Name = "TextBox4";
        TextBox4.Size = new Size(576, 22);
        TextBox4.TabIndex = 11;
        TextBox5.BorderStyle = BorderStyle.None;
        TextBox5.Location = new Point(118, 113);
        TextBox5.Multiline = true;
        TextBox5.Name = "edtNameprefix";
        TextBox5.Size = new Size(576, 22);
        TextBox5.TabIndex = 12;
        TextBox6.BorderStyle = BorderStyle.None;
        TextBox6.Location = new Point(118, 139);
        TextBox6.Multiline = true;
        TextBox6.Name = "edtNameSuffix";
        TextBox6.Size = new Size(576, 22);
        TextBox6.TabIndex = 13;
        RichTextBox1.Location = new Point(12, 193);
        RichTextBox1.Name = "RichTextBox1";
        RichTextBox1.Size = new Size(682, 150);
        RichTextBox1.TabIndex = 15;
        RichTextBox1.Text = "";
        btnSave.BackColor = Color.FromArgb(128, 255, 255);
        btnSave.Location = new Point(20, 358);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(92, 23);
        btnSave.TabIndex = 16;
        btnSave.Text = "speichern";
        btnSave.UseVisualStyleBackColor = false;
        btnSave.Visible = false;
        btnSave2.BackColor = Color.FromArgb(128, 255, 255);
        btnSave2.Location = new Point(37, 358);
        btnSave2.Name = "btnSave2";
        btnSave2.Size = new Size(92, 23);
        btnSave2.TabIndex = 17;
        btnSave2.Text = "speichern";
        btnSave2.UseVisualStyleBackColor = false;
        btnSave2.Visible = false;
        ListBox1.FormattingEnabled = true;
        ListBox1.ItemHeight = 17;
        ListBox1.Location = new Point(700, 1);
        ListBox1.Name = "ListBox1";
        ListBox1.Size = new Size(446, 429);
        ListBox1.TabIndex = 18;
        Label8.AutoSize = true;
        Label8.Location = new Point(643, 361);
        Label8.Name = "Label8";
        Label8.Size = new Size(51, 17);
        Label8.TabIndex = 19;
        Label8.Text = "Label8";
        Label8.Visible = false;
        btnClose.BackColor = Color.FromArgb(128, 255, 255);
        btnClose.Location = new Point(888, 436);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(92, 23);
        btnClose.TabIndex = 20;
        btnClose.Text = "schließen";
        btnClose.UseVisualStyleBackColor = false;
        btnClose.Visible = false;
        btnNewEntry.BackColor = Color.FromArgb(128, 255, 255);
        btnNewEntry.Location = new Point(135, 358);
        btnNewEntry.Name = "btnNewEntry";
        btnNewEntry.Size = new Size(128, 23);
        btnNewEntry.TabIndex = 21;
        btnNewEntry.Text = "Neuer Eintrag";
        btnNewEntry.UseVisualStyleBackColor = false;
        btnNewEntry.Visible = false;
        RichTextBox2.BorderStyle = BorderStyle.None;
        RichTextBox2.Location = new Point(118, 167);
        RichTextBox2.Name = "RichTextBox2";
        RichTextBox2.ScrollBars = RichTextBoxScrollBars.None;
        RichTextBox2.Size = new Size(576, 22);
        RichTextBox2.TabIndex = 22;
        RichTextBox2.Text = "";
        Button5.BackColor = Color.FromArgb(128, 255, 255);
        Button5.Location = new Point(306, 358);
        Button5.Name = "btnHometown";
        Button5.Size = new Size(128, 23);
        Button5.TabIndex = 23;
        Button5.Text = "löschen";
        Button5.UseVisualStyleBackColor = false;
        Button5.Visible = false;
        ListBox2.FormattingEnabled = true;
        ListBox2.ItemHeight = 17;
        ListBox2.Location = new Point(9, 436);
        ListBox2.Name = "ListBox2";
        ListBox2.Size = new Size(685, 208);
        ListBox2.TabIndex = 25;
        Label9.AutoSize = true;
        Label9.Location = new Point(6, 416);
        Label9.Name = "Label9";
        Label9.Size = new Size(189, 17);
        Label9.TabIndex = 26;
        Label9.Text = "Quellen an diesem Standort";
        AutoScaleDimensions = new SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1016, 678);
        Controls.Add(Label9);
        Controls.Add(ListBox2);
        Controls.Add(Button5);
        Controls.Add(ListBox1);
        Controls.Add(RichTextBox2);
        Controls.Add(btnNewEntry);
        Controls.Add(btnClose);
        Controls.Add(Label8);
        Controls.Add(btnSave2);
        Controls.Add(btnSave);
        Controls.Add(RichTextBox1);
        Controls.Add(TextBox6);
        Controls.Add(TextBox5);
        Controls.Add(TextBox4);
        Controls.Add(TextBox3);
        Controls.Add(TextBox2);
        Controls.Add(edtPredicate);
        Controls.Add(lblURL);
        Controls.Add(lblEMail);
        Controls.Add(lblSorting);
        Controls.Add(lblSearch);
        Controls.Add(lblDisplayHint);
        Controls.Add(lblState);
        Controls.Add(lblRepoName);
        Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Margin = new Padding(4);
        Name = "Repo";
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.Manual;
        Text = "Quellenstandorte";
        ResumeLayout(false);
        PerformLayout();
    }


    [CommandBinding(nameof(IRepoViewModel.SaveCommand))]
    internal Button btnSave;
    [CommandBinding(nameof(IRepoViewModel.Save2Command))]
    private Button btnSave2;
    private ListBox ListBox1;
    [CommandBinding(nameof(IRepoViewModel.CloseCommand))]
    private Button btnClose;
    [CommandBinding(nameof(IRepoViewModel.NewEntryCommand))]
    private Button btnNewEntry;
    [CommandBinding(nameof(IRepoViewModel.DeleteCommand))]
    private Button Button5;
    private ListBox ListBox2;
    internal Label lblRepoName;
    internal Label lblState;
    internal Label lblDisplayHint;
    internal Label lblSearch;
    internal Label lblSorting;
    internal Label lblEMail;
    internal Label lblURL;
    [TextBinding(nameof(IRepoViewModel.SourceCount))]
    internal Label Label8;
    internal Label Label9;
    [TextBinding(nameof(IRepoViewModel.RepoName))]
    internal TextBox edtPredicate;
    internal TextBox TextBox2;
    internal TextBox TextBox3;
    internal TextBox TextBox4;
    internal TextBox TextBox5;
    internal TextBox TextBox6;
    internal RichTextBox RichTextBox1;
    private RichTextBox RichTextBox2;
}