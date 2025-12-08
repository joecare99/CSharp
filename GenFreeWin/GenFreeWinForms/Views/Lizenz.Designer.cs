using GenFree.ViewModels.Interfaces;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

[DesignerGenerated]
public partial class Lizenz
{
    [DebuggerNonUserCode]
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && _components != null)
        {
            _components.Dispose();
        }
        base.Dispose(Disposing);
    }

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
        _components = new System.ComponentModel.Container();
        ToolTip1 = new System.Windows.Forms.ToolTip(_components);
        btnCancel = new System.Windows.Forms.Button();
        btnVerify = new System.Windows.Forms.Button();
        txtLicPart3 = new System.Windows.Forms.TextBox();
        txtLicPart2 = new System.Windows.Forms.TextBox();
        txtLicPart1 = new System.Windows.Forms.TextBox();
        lblSep1 = new System.Windows.Forms.Label();
        lblSep2 = new System.Windows.Forms.Label();
        lblEnterLicence = new System.Windows.Forms.Label();
        lblDisplayHint = new System.Windows.Forms.Label();
        btnReqHint = new System.Windows.Forms.Button();
        SuspendLayout();
        btnCancel.Cursor = Cursors.Default;
        btnCancel.Font = new System.Drawing.Font("Arial", 8.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        btnCancel.Location = new System.Drawing.Point(206, 208);
        btnCancel.Name = "btnCancel";
        btnCancel.RightToLeft = RightToLeft.No;
        btnCancel.Size = new System.Drawing.Size(100, 29);
        btnCancel.TabIndex = 9;
        btnCancel.Text = "Abbruch";
        btnCancel.UseVisualStyleBackColor = false;
        btnVerify.Cursor = Cursors.Default;
        btnVerify.Enabled = false;
        btnVerify.Font = new System.Drawing.Font("Arial", 8.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        btnVerify.Location = new System.Drawing.Point(67, 208);
        btnVerify.Name = "btnVerify";
        btnVerify.RightToLeft = RightToLeft.No;
        btnVerify.Size = new System.Drawing.Size(92, 29);
        btnVerify.TabIndex = 5;
        btnVerify.Text = "Fertig";
        btnVerify.UseVisualStyleBackColor = false;
        txtLicPart3.AcceptsReturn = true;
        txtLicPart3.Cursor = Cursors.IBeam;
        txtLicPart3.Font = new System.Drawing.Font("Courier New", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        txtLicPart3.Location = new System.Drawing.Point(293, 138);
        txtLicPart3.MaxLength = 0;
        txtLicPart3.Name = "txtLicPart3";
        txtLicPart3.RightToLeft = RightToLeft.No;
        txtLicPart3.Tag = 2;
        txtLicPart3.Size = new System.Drawing.Size(73, 26);
        txtLicPart3.TabIndex = 2;
        txtLicPart2.AcceptsReturn = true;
        txtLicPart2.Cursor = Cursors.IBeam;
        txtLicPart2.Font = new System.Drawing.Font("Courier New", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        txtLicPart2.Location = new System.Drawing.Point(163, 138);
        txtLicPart2.MaxLength = 0;
        txtLicPart2.Name = "txtLicPart2";
        txtLicPart2.RightToLeft = RightToLeft.No;
        txtLicPart2.Tag = 1;
        txtLicPart2.Size = new System.Drawing.Size(121, 26);
        txtLicPart2.TabIndex = 1;
        txtLicPart1.AcceptsReturn = true;
        txtLicPart1.Cursor = Cursors.IBeam;
        txtLicPart1.Font = new System.Drawing.Font("Courier New", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        txtLicPart1.Location = new System.Drawing.Point(20, 138);
        txtLicPart1.MaxLength = 0;
        txtLicPart1.Tag = 0;
        txtLicPart1.Name = "txtLicPart1";
        txtLicPart1.RightToLeft = RightToLeft.No;
        txtLicPart1.Size = new System.Drawing.Size(124, 26);
        txtLicPart1.TabIndex = 0;
        lblSep1.BackColor = System.Drawing.Color.Silver;
        lblSep1.Cursor = Cursors.Default;
        lblSep1.Font = new System.Drawing.Font("Courier New", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        lblSep1.Location = new System.Drawing.Point(277, 138);
        lblSep1.Name = "lblSep1";
        lblSep1.RightToLeft = RightToLeft.No;
        lblSep1.Size = new System.Drawing.Size(13, 26);
        lblSep1.TabIndex = 7;
        lblSep1.Text = "-";
        lblSep2.BackColor = System.Drawing.Color.Silver;
        lblSep2.Cursor = Cursors.Default;
        lblSep2.Font = new System.Drawing.Font("Courier New", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        lblSep2.Location = new System.Drawing.Point(141, 140);
        lblSep2.Name = "lblSep2";
        lblSep2.RightToLeft = RightToLeft.No;
        lblSep2.Size = new System.Drawing.Size(20, 26);
        lblSep2.TabIndex = 6;
        lblSep2.Text = "-";
        lblEnterLicence.Cursor = Cursors.Default;
        lblEnterLicence.Font = new System.Drawing.Font("Arial", 8.5f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        lblEnterLicence.Location = new System.Drawing.Point(17, 34);
        lblEnterLicence.Name = "lblEnterLicence";
        lblEnterLicence.RightToLeft = RightToLeft.No;
        lblEnterLicence.Size = new System.Drawing.Size(335, 21);
        lblEnterLicence.TabIndex = 4;
        lblEnterLicence.Text = "Eingabe der Lizenznummer";
        lblEnterLicence.TextAlign = System.Drawing.ContentAlignment.TopCenter;
        lblDisplayHint.AutoSize = true;
        lblDisplayHint.Location = new System.Drawing.Point(124, 108);
        lblDisplayHint.Name = "lblDisplayHint";
        lblDisplayHint.Size = new System.Drawing.Size(145, 14);
        lblDisplayHint.TabIndex = 10;
        lblDisplayHint.Text = "Hinter der CD in der CD-Hülle";
        lblDisplayHint.Visible = false;
        btnReqHint.Location = new System.Drawing.Point(98, 72);
        btnReqHint.Name = "btnReqHint";
        btnReqHint.Size = new System.Drawing.Size(239, 22);
        btnReqHint.TabIndex = 11;
        btnReqHint.Text = "Wo finde ich die Lizenznummer?";
        btnReqHint.UseVisualStyleBackColor = true;
        AutoScaleDimensions = new System.Drawing.SizeF(6f, 14f);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = System.Drawing.Color.Silver;
        ClientSize = new System.Drawing.Size(378, 300);

        ControlBox = false;
        Controls.Add(btnReqHint);
        Controls.Add(lblDisplayHint);
        Controls.Add(btnCancel);
        Controls.Add(btnVerify);
        Controls.Add(txtLicPart3);
        Controls.Add(txtLicPart2);
        Controls.Add(txtLicPart1);
        Controls.Add(lblSep1);
        Controls.Add(lblSep2);
        Controls.Add(lblEnterLicence);
        Cursor = Cursors.Default;
        Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.Fixed3D;
        Location = new System.Drawing.Point(3, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "Lizenz";
        RightToLeft = RightToLeft.No;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        ResumeLayout(false);
        PerformLayout();
    }

    private IContainer _components;

    public ToolTip ToolTip1;
    [CommandBinding(nameof(ILizenzViewModel.CancelCommand))]
    public Button btnCancel;
    [CommandBinding(nameof(ILizenzViewModel.VerifyCommand))]
    public Button btnVerify;

    [TextBinding(nameof(ILizenzViewModel.LicText3))]
    public TextBox txtLicPart3;
    [TextBinding(nameof(ILizenzViewModel.LicText2))]
    public TextBox txtLicPart2;
    [TextBinding(nameof(ILizenzViewModel.LicText1))]
    public TextBox txtLicPart1;
    public Label lblSep1;
    public Label lblSep2;
    public Label lblEnterLicence;

    [VisibilityBinding(nameof(ILizenzViewModel.DisplayHintVisible))]
    internal Label lblDisplayHint;
    [CommandBinding(nameof(ILizenzViewModel.ReqHintCommand))]
    public Button btnReqHint;

}