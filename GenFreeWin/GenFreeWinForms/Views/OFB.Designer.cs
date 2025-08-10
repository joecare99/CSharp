using BaseLib.Helper;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace Gen_FreeWin.Views;
partial class OFB
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

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Check1 = new System.Windows.Forms.CheckBox();
            this._Label1_3 = new System.Windows.Forms.Label();
            this.Text1 = new System.Windows.Forms.TextBox();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_2 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.List1 = new System.Windows.Forms.ListBox();
            this._List5_2 = new System.Windows.Forms.ListBox();
            this._List5_1 = new System.Windows.Forms.ListBox();
            this._Text2_2 = new System.Windows.Forms.TextBox();
            this._Text2_1 = new System.Windows.Forms.TextBox();
            this._Text2_0 = new System.Windows.Forms.TextBox();
            this._List5_0 = new System.Windows.Forms.ListBox();
            this.List4 = new System.Windows.Forms.ListBox();
            this.List3 = new System.Windows.Forms.ListBox();
            this.List2 = new System.Windows.Forms.ListBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.btnList52_Add = new System.Windows.Forms.Button();
            this.btnList51_Add = new System.Windows.Forms.Button();
            this.btnList50_Add = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // Check1
            // 
            this.Check1.BackColor = System.Drawing.SystemColors.Control;
            this.Check1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Check1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Check1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Check1.Location = new System.Drawing.Point(3, 3);
            this.Check1.Name = "Check1";
            this.Check1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Check1.Size = new System.Drawing.Size(253, 30);
            this.Check1.TabIndex = 21;
            this.Check1.Text = "Person für OFB sperren";
            this.ToolTip1.SetToolTip(this.Check1, "Person wird im OFB nicht berücksichtigt");
            this.Check1.UseVisualStyleBackColor = false;
            // 
            // _Label1_3
            // 
            this._Label1_3.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_3.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_3.Dock = System.Windows.Forms.DockStyle.Top;
            this._Label1_3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_3.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._Label1_3.Location = new System.Drawing.Point(276, 0);
            this._Label1_3.Name = "_Label1_3";
            this._Label1_3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_3.Size = new System.Drawing.Size(267, 30);
            this._Label1_3.TabIndex = 22;
            this._Label1_3.Text = "Sortiername:";
            this._Label1_3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ToolTip1.SetToolTip(this._Label1_3, "Unter diesem Namen wird die Person im OFB einsortiert, ohne Eintrag erfolgt die S" +
        "ortierung nach dem Namen");
            // 
            // Text1
            // 
            this.Text1.AcceptsReturn = true;
            this.Text1.BackColor = System.Drawing.SystemColors.Window;
            this.Text1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Text1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Text1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Text1.Location = new System.Drawing.Point(549, 3);
            this.Text1.MaxLength = 0;
            this.Text1.Name = "Text1";
            this.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text1.Size = new System.Drawing.Size(267, 27);
            this.Text1.TabIndex = 22;
            this.ToolTip1.SetToolTip(this.Text1, "Unter diesem Namen wird die Person im OFB einsortiert, ohne Eintrag erfolgt die S" +
        "ortierung nach dem Namen");
            // 
            // _Label1_1
            // 
            this._Label1_1.AutoSize = true;
            this._Label1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.Dock = System.Windows.Forms.DockStyle.Top;
            this._Label1_1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_1.Location = new System.Drawing.Point(0, 0);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_1.Size = new System.Drawing.Size(129, 19);
            this._Label1_1.TabIndex = 3;
            this._Label1_1.Text = "Namen für Index";
            this.ToolTip1.SetToolTip(this._Label1_1, "In diese Felder können Orte, Namen und Berufe z.B. aus den Bemerkungen, eingetrag" +
        "en werden, um sie in den Indizis zu berücksichtigen.");
            // 
            // _Label1_2
            // 
            this._Label1_2.AutoSize = true;
            this._Label1_2.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_2.Dock = System.Windows.Forms.DockStyle.Top;
            this._Label1_2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_2.Location = new System.Drawing.Point(0, 0);
            this._Label1_2.Name = "_Label1_2";
            this._Label1_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_2.Size = new System.Drawing.Size(127, 19);
            this._Label1_2.TabIndex = 20;
            this._Label1_2.Text = "Berufe für Index";
            this.ToolTip1.SetToolTip(this._Label1_2, "In diese Felder können Orte, Namen und Berufe z.B. aus den Bemerkungen, eingetrag" +
        "en werden, um sie in den Indizis zu berücksichtigen.");
            // 
            // _Label1_0
            // 
            this._Label1_0.AutoSize = true;
            this._Label1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.Dock = System.Windows.Forms.DockStyle.Top;
            this._Label1_0.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_0.Location = new System.Drawing.Point(0, 0);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_0.Size = new System.Drawing.Size(109, 19);
            this._Label1_0.TabIndex = 21;
            this._Label1_0.Text = "Orte für Index";
            this.ToolTip1.SetToolTip(this._Label1_0, "In diese Felder können Orte, Namen und Berufe z.B. aus den Bemerkungen, eingetrag" +
        "en werden, um sie in den Indizis zu berücksichtigen.");
            // 
            // List1
            // 
            this.List1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.List1.BackColor = System.Drawing.SystemColors.Window;
            this.List1.Cursor = System.Windows.Forms.Cursors.Default;
            this.List1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List1.ItemHeight = 23;
            this.List1.Location = new System.Drawing.Point(0, 3);
            this.List1.Name = "List1";
            this.List1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List1.Size = new System.Drawing.Size(214, 280);
            this.List1.TabIndex = 7;
            this.List1.Visible = false;
            // 
            // _List5_2
            // 
            this._List5_2.BackColor = System.Drawing.SystemColors.Window;
            this._List5_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._List5_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._List5_2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._List5_2.ForeColor = System.Drawing.SystemColors.WindowText;
            this._List5_2.ItemHeight = 19;
            this._List5_2.Location = new System.Drawing.Point(0, 19);
            this._List5_2.Name = "_List5_2";
            this._List5_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._List5_2.Size = new System.Drawing.Size(267, 275);
            this._List5_2.Sorted = true;
            this._List5_2.TabIndex = 23;
            // 
            // _List5_1
            // 
            this._List5_1.BackColor = System.Drawing.SystemColors.Window;
            this._List5_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._List5_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._List5_1.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._List5_1.ForeColor = System.Drawing.SystemColors.WindowText;
            this._List5_1.ItemHeight = 19;
            this._List5_1.Location = new System.Drawing.Point(0, 19);
            this._List5_1.Name = "_List5_1";
            this._List5_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._List5_1.Size = new System.Drawing.Size(267, 275);
            this._List5_1.Sorted = true;
            this._List5_1.TabIndex = 22;
            // 
            // _Text2_2
            // 
            this._Text2_2.AcceptsReturn = true;
            this._Text2_2.BackColor = System.Drawing.SystemColors.Window;
            this._Text2_2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text2_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Text2_2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Text2_2.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text2_2.Location = new System.Drawing.Point(0, 0);
            this._Text2_2.MaxLength = 0;
            this._Text2_2.Name = "_Text2_2";
            this._Text2_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text2_2.Size = new System.Drawing.Size(236, 30);
            this._Text2_2.TabIndex = 25;
            // 
            // _Text2_1
            // 
            this._Text2_1.AcceptsReturn = true;
            this._Text2_1.BackColor = System.Drawing.SystemColors.Window;
            this._Text2_1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text2_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Text2_1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Text2_1.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text2_1.Location = new System.Drawing.Point(0, 0);
            this._Text2_1.MaxLength = 0;
            this._Text2_1.Name = "_Text2_1";
            this._Text2_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text2_1.Size = new System.Drawing.Size(236, 30);
            this._Text2_1.TabIndex = 22;
            // 
            // _Text2_0
            // 
            this._Text2_0.AcceptsReturn = true;
            this._Text2_0.BackColor = System.Drawing.SystemColors.Window;
            this._Text2_0.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._Text2_0.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Text2_0.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._Text2_0.ForeColor = System.Drawing.SystemColors.WindowText;
            this._Text2_0.Location = new System.Drawing.Point(0, 0);
            this._Text2_0.MaxLength = 0;
            this._Text2_0.Name = "_Text2_0";
            this._Text2_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Text2_0.Size = new System.Drawing.Size(236, 30);
            this._Text2_0.TabIndex = 22;
            // 
            // _List5_0
            // 
            this._List5_0.BackColor = System.Drawing.SystemColors.Window;
            this._List5_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._List5_0.Dock = System.Windows.Forms.DockStyle.Fill;
            this._List5_0.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._List5_0.ForeColor = System.Drawing.SystemColors.WindowText;
            this._List5_0.ItemHeight = 19;
            this._List5_0.Location = new System.Drawing.Point(0, 0);
            this._List5_0.Name = "_List5_0";
            this._List5_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._List5_0.Size = new System.Drawing.Size(267, 294);
            this._List5_0.Sorted = true;
            this._List5_0.TabIndex = 20;
            // 
            // List4
            // 
            this.List4.BackColor = System.Drawing.SystemColors.Window;
            this.List4.Cursor = System.Windows.Forms.Cursors.Default;
            this.List4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.List4.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List4.ItemHeight = 19;
            this.List4.Location = new System.Drawing.Point(549, 353);
            this.List4.Name = "List4";
            this.List4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List4.Size = new System.Drawing.Size(267, 294);
            this.List4.TabIndex = 26;
            this.List4.Visible = false;
            // 
            // List3
            // 
            this.List3.BackColor = System.Drawing.SystemColors.Window;
            this.List3.Cursor = System.Windows.Forms.Cursors.Default;
            this.List3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.List3.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List3.ItemHeight = 19;
            this.List3.Location = new System.Drawing.Point(276, 353);
            this.List3.Name = "List3";
            this.List3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List3.Size = new System.Drawing.Size(267, 294);
            this.List3.TabIndex = 25;
            this.List3.Visible = false;
            // 
            // List2
            // 
            this.List2.BackColor = System.Drawing.SystemColors.Window;
            this.List2.Cursor = System.Windows.Forms.Cursors.Default;
            this.List2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.List2.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.List2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.List2.ItemHeight = 19;
            this.List2.Location = new System.Drawing.Point(3, 353);
            this.List2.Name = "List2";
            this.List2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.List2.Size = new System.Drawing.Size(267, 294);
            this.List2.TabIndex = 24;
            this.List2.Visible = false;
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.SystemColors.Control;
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnApply.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnApply.Font = new System.Drawing.Font("Arial", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnApply.Location = new System.Drawing.Point(822, 353);
            this.btnApply.Name = "btnApply";
            this.btnApply.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnApply.Size = new System.Drawing.Size(268, 102);
            this.btnApply.TabIndex = 19;
            this.btnApply.Text = "&Fertig";
            this.btnApply.UseVisualStyleBackColor = false;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(822, 50);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(268, 111);
            this.Label2.TabIndex = 18;
            this.Label2.Text = "Einträge entfernen mit Doppelklick auf den Eintrag";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnList52_Add
            // 
            this.btnList52_Add.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnList52_Add.Location = new System.Drawing.Point(236, 0);
            this.btnList52_Add.Name = "btnList52_Add";
            this.btnList52_Add.Size = new System.Drawing.Size(31, 39);
            this.btnList52_Add.TabIndex = 24;
            this.btnList52_Add.Text = "+";
            this.btnList52_Add.UseVisualStyleBackColor = true;
            // 
            // btnList51_Add
            // 
            this.btnList51_Add.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnList51_Add.Location = new System.Drawing.Point(236, 0);
            this.btnList51_Add.Name = "btnList51_Add";
            this.btnList51_Add.Size = new System.Drawing.Size(31, 39);
            this.btnList51_Add.TabIndex = 24;
            this.btnList51_Add.Text = "+";
            this.btnList51_Add.UseVisualStyleBackColor = true;
            // 
            // btnList50_Add
            // 
            this.btnList50_Add.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnList50_Add.Location = new System.Drawing.Point(236, 0);
            this.btnList50_Add.Name = "btnList50_Add";
            this.btnList50_Add.Size = new System.Drawing.Size(31, 39);
            this.btnList50_Add.TabIndex = 24;
            this.btnList50_Add.Text = "+";
            this.btnList50_Add.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.Text1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._Label1_3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Check1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Label2, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnApply, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.List2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.List3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.List4, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 2, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1093, 650);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this._Label1_1);
            this.panel1.Controls.Add(this._List5_0);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(267, 294);
            this.panel1.TabIndex = 23;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this._Text2_0);
            this.panel5.Controls.Add(this.btnList50_Add);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 19);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(267, 39);
            this.panel5.TabIndex = 25;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this._List5_1);
            this.panel2.Controls.Add(this._Label1_2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(276, 53);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(267, 294);
            this.panel2.TabIndex = 27;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this._Text2_1);
            this.panel4.Controls.Add(this.btnList51_Add);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 19);
            this.panel4.Margin = new System.Windows.Forms.Padding(0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(267, 39);
            this.panel4.TabIndex = 24;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.List1);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this._List5_2);
            this.panel3.Controls.Add(this._Label1_0);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(549, 53);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(267, 294);
            this.panel3.TabIndex = 28;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this._Text2_2);
            this.panel6.Controls.Add(this.btnList52_Add);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 19);
            this.panel6.Margin = new System.Windows.Forms.Padding(0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(267, 39);
            this.panel6.TabIndex = 25;
            // 
            // OFB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1093, 650);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Arial", 11F);
            this.Location = new System.Drawing.Point(4, 23);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OFB";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Tag = "Farbwechsel";
            this.Text = "Sonderfelder Für Ortsfamilienbuch";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

    }


    public ToolTip ToolTip1;
    private TableLayoutPanel tableLayoutPanel1;
    private Panel panel1;
    private Panel panel2;
    private Panel panel3;
    private Panel panel4;
    private Panel panel5;
    private Panel panel6;    
    [VisibilityBinding(nameof(IOFBViewModel.List1_Visible))]
    [DblClickBinding(nameof(IOFBViewModel.List1_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List1_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List1_Items), nameof(IOFBViewModel.List1_SelectedItem))]
    public ListBox List1;
    [VisibilityBinding(nameof(IOFBViewModel.List2_Visible))]
    [DblClickBinding(nameof(IOFBViewModel.List2_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List2_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List2_Items), nameof(IOFBViewModel.List2_SelectedItem))]
    public ListBox List2;
    [VisibilityBinding(nameof(IOFBViewModel.List3_Visible))]
    [DblClickBinding(nameof(IOFBViewModel.List3_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List3_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List3_Items), nameof(IOFBViewModel.List3_SelectedItem))]
    public ListBox List3;
    [VisibilityBinding(nameof(IOFBViewModel.List4_Visible))]
    [DblClickBinding(nameof(IOFBViewModel.List4_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List4_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List4_Items), nameof(IOFBViewModel.List4_SelectedItem))]
    public ListBox List4;
    [DblClickBinding(nameof(IOFBViewModel.List5_0_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List5_0_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List50_Items), nameof(IOFBViewModel.List50_SelectedItem))]
    public ListBox _List5_0;
    [DblClickBinding(nameof(IOFBViewModel.List5_1_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List5_1_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List51_Items), nameof(IOFBViewModel.List51_SelectedItem))]
    public ListBox _List5_1;
    [DblClickBinding(nameof(IOFBViewModel.List5_2_DblClickCommand))]
    [KeyBinding('\x8', nameof(IOFBViewModel.List5_2_DblClickCommand))]
    [ListBinding(nameof(IOFBViewModel.List52_Items), nameof(IOFBViewModel.List52_SelectedItem))]
    public ListBox _List5_2;

    [TextBinding(nameof(IOFBViewModel.Text1_Text))]
    [KeyBinding('\r',nameof(IOFBViewModel.Text1_KeyEnterCommand))]
    public TextBox Text1;
    [TextBinding(nameof(IOFBViewModel.Text2_2_Text))]
    [KeyBinding('\r', nameof(IOFBViewModel.List52_AddCommand))]
    public TextBox _Text2_2;
    [TextBinding(nameof(IOFBViewModel.Text2_1_Text))]
    [KeyBinding('\r', nameof(IOFBViewModel.List51_AddCommand))]
    public TextBox _Text2_1;
    [TextBinding(nameof(IOFBViewModel.Text2_0_Text))]
    [KeyBinding('\r', nameof(IOFBViewModel.List50_AddCommand))]
    public TextBox _Text2_0;

    [CommandBinding(nameof(IOFBViewModel.ApplyCommand))]
    public Button btnApply;
    [CommandBinding(nameof(IOFBViewModel.List50_AddCommand))]
    public Button btnList50_Add;
    [CommandBinding(nameof(IOFBViewModel.List51_AddCommand))]
    public Button btnList51_Add;
    [CommandBinding(nameof(IOFBViewModel.List52_AddCommand))]
    public Button btnList52_Add;

    [CheckedBinding(nameof(IOFBViewModel.Check1_Checked))]
    public CheckBox Check1;

    public Label Label2;
    public Label _Label1_3;
    public Label _Label1_2;
    public Label _Label1_1;
    public Label _Label1_0;

}