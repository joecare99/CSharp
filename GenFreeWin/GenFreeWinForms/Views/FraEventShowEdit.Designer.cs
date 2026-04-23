using GenFree.ViewModels.Interfaces;
using GenFreeWin.Attributes;
using Views;

namespace Gen_FreeWin.Views;

partial class FraEventShowEdit
{
    /// <summary> 
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Vom Komponenten-Designer generierter Code

    /// <summary> 
    /// Erforderliche Methode für die Designerunterstützung. 
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
        this.lblBirthDisp = new System.Windows.Forms.Label();
        this.lblEvent = new System.Windows.Forms.Label();
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.tableLayoutPanel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // lblBirthDisp
        // 
        this.lblBirthDisp.AutoEllipsis = true;
        this.lblBirthDisp.BackColor = System.Drawing.Color.White;
        this.lblBirthDisp.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblBirthDisp.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblBirthDisp.Font = new System.Drawing.Font("Arial", 9F);
        this.lblBirthDisp.Location = new System.Drawing.Point(188, 0);
        this.lblBirthDisp.Margin = new System.Windows.Forms.Padding(9, 0, 9, 0);
        this.lblBirthDisp.Name = "lblBirthDisp";
        this.lblBirthDisp.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblBirthDisp.Size = new System.Drawing.Size(718, 40);
        this.lblBirthDisp.TabIndex = 42;
        this.lblBirthDisp.Text = "<Geburtstag>";
        // 
        // lblEvent
        // 
        this.lblEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
        this.lblEvent.Cursor = System.Windows.Forms.Cursors.Default;
        this.lblEvent.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblEvent.Font = new System.Drawing.Font("Arial", 9F);
        this.lblEvent.Location = new System.Drawing.Point(9, 0);
        this.lblEvent.Margin = new System.Windows.Forms.Padding(9, 0, 9, 0);
        this.lblEvent.Name = "lblBirth";
        this.lblEvent.RightToLeft = System.Windows.Forms.RightToLeft.No;
        this.lblEvent.Size = new System.Drawing.Size(161, 40);
        this.lblEvent.TabIndex = 41;
        this.lblEvent.Text = "Geb.:";
        this.lblEvent.TextAlign = System.Drawing.ContentAlignment.TopRight;
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.ColumnCount = 2;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.66568F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.33432F));
        this.tableLayoutPanel1.Controls.Add(this.lblEvent, 0, 0);
        this.tableLayoutPanel1.Controls.Add(this.lblBirthDisp, 1, 0);
        this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
        this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 1;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel1.Size = new System.Drawing.Size(915, 40);
        this.tableLayoutPanel1.TabIndex = 43;
        // 
        // FraEventShowEdit
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.tableLayoutPanel1);
        this.Name = "FraEventShowEdit";
        this.Size = new System.Drawing.Size(915, 40);
        this.tableLayoutPanel1.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion
    [CommandBinding(nameof(IEventShowEditViewModel.ClickCommand))]
    [TextBinding(nameof(IEventShowEditViewModel.Display_Text))]
    public System.Windows.Forms.Label lblBirthDisp;
    [ApplTextBinding(nameof(IEventShowEditViewModel.Display_Hdr))]
    [CommandBinding(nameof(IEventShowEditViewModel.ClickCommand))]
    public System.Windows.Forms.Label lblEvent;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
}
