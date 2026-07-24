using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Rahmen
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

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.frmFrame1 = new System.Windows.Forms.GroupBox();
            this.frmRahmenSelect = new GenFreeWin.Views.FraRahSelect();
            this.RTB = new System.Windows.Forms.RichTextBox();
            this.List4 = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAppend = new System.Windows.Forms.Button();
            this.lblAsText = new System.Windows.Forms.Label();
            this.frmFrame1.SuspendLayout();
            this.SuspendLayout();
            // 
            // frmFrame1
            // 
            this.frmFrame1.BackColor = System.Drawing.Color.Red;
            this.frmFrame1.Controls.Add(this.frmRahmenSelect);
            this.frmFrame1.Controls.Add(this.RTB);
            this.frmFrame1.Controls.Add(this.List4);
            this.frmFrame1.Controls.Add(this.btnClose);
            this.frmFrame1.Controls.Add(this.btnDelete);
            this.frmFrame1.Controls.Add(this.btnAppend);
            this.frmFrame1.Controls.Add(this.lblAsText);
            this.frmFrame1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.frmFrame1.ForeColor = System.Drawing.Color.White;
            this.frmFrame1.Location = new System.Drawing.Point(2, 0);
            this.frmFrame1.Margin = new System.Windows.Forms.Padding(4);
            this.frmFrame1.Name = "frmFrame1";
            this.frmFrame1.Padding = new System.Windows.Forms.Padding(4);
            this.frmFrame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frmFrame1.Size = new System.Drawing.Size(711, 405);
            this.frmFrame1.TabIndex = 0;
            this.frmFrame1.TabStop = false;
            // 
            // frmRahmenSelect
            // 
            this.frmRahmenSelect.eText = GenFreeWin.EUserText.tMarrTo;
            this.frmRahmenSelect.Location = new System.Drawing.Point(33, 23);
            this.frmRahmenSelect.Margin = new System.Windows.Forms.Padding(4);
            this.frmRahmenSelect.Name = "frmRahmenSelect";
            this.frmRahmenSelect.Size = new System.Drawing.Size(632, 127);
            this.frmRahmenSelect.TabIndex = 23;
            this.frmRahmenSelect.xEnReenter = true;
            this.frmRahmenSelect.xVisReenter = true;
            this.frmRahmenSelect.Cancel += new System.EventHandler(this.btnSelCancel_Click);
            this.frmRahmenSelect.Reenter += new System.EventHandler(this.btnSelReenter_Click);
            this.frmRahmenSelect.EnterNumber += new System.EventHandler(this.btnEnterNumber_Click);
            this.frmRahmenSelect.FromFile += new System.EventHandler(this.btnSelFromFile_Click);
            // 
            // RTB
            // 
            this.RTB.Font = new System.Drawing.Font("Arial", 9F);
            this.RTB.Location = new System.Drawing.Point(11, 254);
            this.RTB.Margin = new System.Windows.Forms.Padding(4);
            this.RTB.Name = "RTB";
            this.RTB.RightMargin = 487;
            this.RTB.Size = new System.Drawing.Size(679, 129);
            this.RTB.TabIndex = 22;
            this.RTB.Text = "";
            // 
            // List4
            // 
            this.List4.BackColor = System.Drawing.SystemColors.Window;
            this.List4.Cursor = System.Windows.Forms.Cursors.Default;
            this.List4.Font = new System.Drawing.Font("Courier New", 11.25F);
            this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List4.ItemHeight = 25;
            this.List4.Location = new System.Drawing.Point(14, 13);
            this.List4.Margin = new System.Windows.Forms.Padding(4);
            this.List4.Name = "List4";
            this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List4.Size = new System.Drawing.Size(676, 154);
            this.List4.Sorted = true;
            this.List4.TabIndex = 4;
            // 
            // btnDuplClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F);
            this.btnClose.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClose.Location = new System.Drawing.Point(33, 187);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnClose.Size = new System.Drawing.Size(164, 36);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&schließen";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.SystemColors.Control;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnDelete.Enabled = false;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 9F);
            this.btnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDelete.Location = new System.Drawing.Point(490, 187);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnDelete.Size = new System.Drawing.Size(164, 36);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "&entfernen";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.Command1_Click);
            // 
            // btnAppend
            // 
            this.btnAppend.BackColor = System.Drawing.SystemColors.Control;
            this.btnAppend.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnAppend.Font = new System.Drawing.Font("Arial", 9F);
            this.btnAppend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnAppend.Location = new System.Drawing.Point(271, 187);
            this.btnAppend.Margin = new System.Windows.Forms.Padding(4);
            this.btnAppend.Name = "btnAppend";
            this.btnAppend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnAppend.Size = new System.Drawing.Size(164, 36);
            this.btnAppend.TabIndex = 1;
            this.btnAppend.Text = "&hinzufügen";
            this.btnAppend.UseVisualStyleBackColor = false;
            this.btnAppend.Click += new System.EventHandler(this.btnAppend_Click);
            // 
            // lblAsText
            // 
            this.lblAsText.BackColor = System.Drawing.Color.Red;
            this.lblAsText.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblAsText.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblAsText.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblAsText.Location = new System.Drawing.Point(8, 227);
            this.lblAsText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAsText.Name = "lblAsText";
            this.lblAsText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAsText.Size = new System.Drawing.Size(228, 23);
            this.lblAsText.TabIndex = 21;
            this.lblAsText.Text = "als Text";
            // 
            // Rahmen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(717, 418);
            this.ControlBox = false;
            this.Controls.Add(this.frmFrame1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(3, 22);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Rahmen";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = " ";
            this.frmFrame1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    public RichTextBox RTB;
    public ListBox List4;
    public Button btnClose;
    public Button btnDelete;
    public Button btnAppend;
    public Label lblAsText;
    public GroupBox frmFrame1;
    public ToolTip ToolTip1;
    public FraRahSelect frmRahmenSelect;
}