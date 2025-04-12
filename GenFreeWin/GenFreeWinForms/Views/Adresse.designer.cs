using Gen_FreeWin.ViewModels.Interfaces;
using System;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;
public partial class Adresse
{

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Adresse));
            this.edtTitle = new System.Windows.Forms.TextBox();
            this.edtGivenname = new System.Windows.Forms.TextBox();
            this.edtSurname = new System.Windows.Forms.TextBox();
            this.edtStreet = new System.Windows.Forms.TextBox();
            this.edtZip = new System.Windows.Forms.TextBox();
            this.edtPlace = new System.Windows.Forms.TextBox();
            this.edtPhone = new System.Windows.Forms.TextBox();
            this.edtEMail = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblGivenname = new System.Windows.Forms.Label();
            this.lblSurname = new System.Windows.Forms.Label();
            this.lblStreet = new System.Windows.Forms.Label();
            this.lblZip = new System.Windows.Forms.Label();
            this.lblPlace = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblEMail = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblSpecial = new System.Windows.Forms.Label();
            this.edtSpecial = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.lblSubcaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // edtTitle
            // 
            resources.ApplyResources(this.edtTitle, "edtTitle");
            this.edtTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtTitle.Name = "edtTitle";
            // 
            // edtGivenname
            // 
            this.edtGivenname.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.edtGivenname.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.edtGivenname, "edtGivenname");
            this.edtGivenname.Name = "edtGivenname";
            // 
            // edtSurname
            // 
            this.edtSurname.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtSurname, "edtSurname");
            this.edtSurname.Name = "edtSurname";
            // 
            // edtStreet
            // 
            this.edtStreet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtStreet, "edtStreet");
            this.edtStreet.Name = "edtStreet";
            // 
            // edtZip
            // 
            this.edtZip.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtZip, "edtZip");
            this.edtZip.Name = "edtZip";
            // 
            // edtPlace
            // 
            this.edtPlace.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtPlace, "edtPlace");
            this.edtPlace.Name = "edtPlace";
            // 
            // edtPhone
            // 
            this.edtPhone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtPhone, "edtPhone");
            this.edtPhone.Name = "edtPhone";
            // 
            // edtEMail
            // 
            this.edtEMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtEMail, "edtEMail");
            this.edtEMail.Name = "edtEMail";
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.AutoEllipsis = true;
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTitle.Name = "lblTitle";
            // 
            // lblGivenname
            // 
            resources.ApplyResources(this.lblGivenname, "lblGivenname");
            this.lblGivenname.AutoEllipsis = true;
            this.lblGivenname.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblGivenname.Name = "lblGivenname";
            // 
            // lblSurname
            // 
            resources.ApplyResources(this.lblSurname, "lblSurname");
            this.lblSurname.AutoEllipsis = true;
            this.lblSurname.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblSurname.Name = "lblSurname";
            // 
            // lblStreet
            // 
            resources.ApplyResources(this.lblStreet, "lblStreet");
            this.lblStreet.AutoEllipsis = true;
            this.lblStreet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblStreet.Name = "lblStreet";
            // 
            // lblZip
            // 
            resources.ApplyResources(this.lblZip, "lblZip");
            this.lblZip.AutoEllipsis = true;
            this.lblZip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblZip.Name = "lblZip";
            // 
            // lblPlace
            // 
            resources.ApplyResources(this.lblPlace, "lblPlace");
            this.lblPlace.AutoEllipsis = true;
            this.lblPlace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblPlace.Name = "lblPlace";
            // 
            // lblPhone
            // 
            resources.ApplyResources(this.lblPhone, "lblPhone");
            this.lblPhone.AutoEllipsis = true;
            this.lblPhone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblPhone.Name = "lblPhone";
            // 
            // lblEMail
            // 
            resources.ApplyResources(this.lblEMail, "lblEMail");
            this.lblEMail.AutoEllipsis = true;
            this.lblEMail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblEMail.Name = "lblEMail";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // lblHint
            // 
            resources.ApplyResources(this.lblHint, "lblHint");
            this.lblHint.Name = "lblHint";
            // 
            // lblSpecial
            // 
            resources.ApplyResources(this.lblSpecial, "lblSpecial");
            this.lblSpecial.AutoEllipsis = true;
            this.lblSpecial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblSpecial.Name = "lblSpecial";
            // 
            // edtSpecial
            // 
            this.edtSpecial.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.edtSpecial, "edtSpecial");
            this.edtSpecial.Name = "edtSpecial";
            // 
            // Label11
            // 
            resources.ApplyResources(this.Label11, "Label11");
            this.Label11.Name = "Label11";
            // 
            // lblSubcaption
            // 
            resources.ApplyResources(this.lblSubcaption, "lblSubcaption");
            this.lblSubcaption.Name = "lblSubcaption";
            // 
            // Adresse
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.lblSubcaption);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.edtSpecial);
            this.Controls.Add(this.lblSpecial);
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblEMail);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.lblPlace);
            this.Controls.Add(this.lblZip);
            this.Controls.Add(this.lblStreet);
            this.Controls.Add(this.lblSurname);
            this.Controls.Add(this.lblGivenname);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.edtEMail);
            this.Controls.Add(this.edtPhone);
            this.Controls.Add(this.edtPlace);
            this.Controls.Add(this.edtZip);
            this.Controls.Add(this.edtStreet);
            this.Controls.Add(this.edtSurname);
            this.Controls.Add(this.edtGivenname);
            this.Controls.Add(this.edtTitle);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Adresse";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.Adresse_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    [TextBinding(nameof(IAdresseViewModel.Title))]
    public TextBox edtTitle;
    [TextBinding(nameof(IAdresseViewModel.Givenname))]
    internal TextBox edtGivenname;
    [TextBinding(nameof(IAdresseViewModel.Surname))]
    internal TextBox edtSurname;
    [TextBinding(nameof(IAdresseViewModel.Street))]
    internal TextBox edtStreet;
    [TextBinding(nameof(IAdresseViewModel.Zip))]
    internal TextBox edtZip;
    [TextBinding(nameof(IAdresseViewModel.Place))]
    internal TextBox edtPlace;
    [TextBinding(nameof(IAdresseViewModel.Phone))]
    internal TextBox edtPhone;
    [TextBinding(nameof(IAdresseViewModel.EMail))]
    internal TextBox edtEMail;
    [TextBinding(nameof(IAdresseViewModel.Special))]
    internal TextBox edtSpecial;
    internal Label lblTitle;
    internal Label lblGivenname;
    internal Label lblSurname;
    internal Label lblStreet;
    internal Label lblZip;
    internal Label lblPlace;
    internal Label lblPhone;
    internal Label lblEMail;
    internal Label lblHint;
    internal Label lblSpecial;
    internal Label Label11;
    internal Label lblSubcaption;
    [CommandBinding(nameof(IAdresseViewModel.SaveCommand))]
    internal Button btnSave;
}