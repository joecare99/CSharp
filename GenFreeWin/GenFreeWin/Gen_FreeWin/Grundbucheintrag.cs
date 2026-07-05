using BaseLib.Helper;
using Gen_FreeWin.Main;
using GenFree;
using GenFree.Data;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.ComponentModel;
using GenFree.Interfaces.Sys;
using GenFreeWin.Views;

namespace Gen_FreeWin;

[DesignerGenerated]
public class Grundbucheintrag : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
    IModul1 Modul1 => _Modul1.Instance;

    [AccessedThroughProperty(nameof(TextBox3))]
    private TextBox _TextBox3;

    [AccessedThroughProperty(nameof(Button1))]
    private Button _Button1;

    [AccessedThroughProperty(nameof(Button2))]
    private Button _Button2;

    [AccessedThroughProperty(nameof(Button3))]
    private Button _Button3;

    internal Label Label1;
    internal Label Label2;
    internal Label Label3;
    internal Label Label4;
    internal Label Label5;
    internal TextBox TextBox1;
    internal TextBox TextBox2;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual TextBox TextBox3
    {
        [DebuggerNonUserCode]
        get => _TextBox3;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = TextBox3_TextChanged;
            if (_TextBox3 != null)
            {
                _TextBox3.TextChanged -= value2;
            }
            _TextBox3 = value;
            if (_TextBox3 != null)
            {
                _TextBox3.TextChanged += value2;
            }
        }
    }

    internal TextBox TextBox4;
    internal TextBox TextBox5;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual Button Button1
    {
        [DebuggerNonUserCode]
        get => _Button1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Button1_Click;
            if (_Button1 != null)
            {
                _Button1.Click -= value2;
            }
            _Button1 = value;
            if (_Button1 != null)
            {
                _Button1.Click += value2;
            }
        }
    }

    internal virtual Button Button2
    {
        [DebuggerNonUserCode]
        get => _Button2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Button2_Click;
            if (_Button2 != null)
            {
                _Button2.Click -= value2;
            }
            _Button2 = value;
            if (_Button2 != null)
            {
                _Button2.Click += value2;
            }
        }
    }

    internal virtual Button Button3
    {
        [DebuggerNonUserCode]
        get => _Button3;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Button3_Click;
            if (_Button3 != null)
            {
                _Button3.Click -= value2;
            }
            _Button3 = value;
            if (_Button3 != null)
            {
                _Button3.Click += value2;
            }
        }
    }

    internal Label Label7;
    [DebuggerNonUserCode]
    public Grundbucheintrag()
    {
        Load += Grundbucheintrag_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
    }

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
        Label1 = new System.Windows.Forms.Label();
        Label2 = new System.Windows.Forms.Label();
        Label3 = new System.Windows.Forms.Label();
        Label4 = new System.Windows.Forms.Label();
        Label5 = new System.Windows.Forms.Label();
        TextBox1 = new System.Windows.Forms.TextBox();
        TextBox2 = new System.Windows.Forms.TextBox();
        TextBox3 = new System.Windows.Forms.TextBox();
        TextBox4 = new System.Windows.Forms.TextBox();
        TextBox5 = new System.Windows.Forms.TextBox();
        Button1 = new System.Windows.Forms.Button();
        Button2 = new System.Windows.Forms.Button();
        Button3 = new System.Windows.Forms.Button();
        Label7 = new System.Windows.Forms.Label();
        SuspendLayout();
        Label1.BackColor = Color.FromArgb(224, 224, 224);
        Label1.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label1.Location = new System.Drawing.Point(16, 12);
        Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label1.Name = "Label1";
        Label1.Size = new System.Drawing.Size(64, 22);
        Label1.TabIndex = 0;
        Label1.Text = "Jahr";
        Label1.TextAlign = ContentAlignment.MiddleCenter;
        Label2.BackColor = Color.FromArgb(224, 224, 224);
        Label2.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label2.Location = new System.Drawing.Point(88, 12);
        Label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label2.Name = "lblState";
        Label2.Size = new System.Drawing.Size(400, 22);
        Label2.TabIndex = 1;
        Label2.Text = "Name ";
        Label2.TextAlign = ContentAlignment.MiddleCenter;
        Label3.BackColor = Color.FromArgb(224, 224, 224);
        Label3.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label3.Location = new System.Drawing.Point(501, 12);
        Label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label3.Name = "lblDisplayHint";
        Label3.Size = new System.Drawing.Size(405, 22);
        Label3.TabIndex = 2;
        Label3.Text = "Art des Gebäudes";
        Label3.TextAlign = ContentAlignment.MiddleCenter;
        Label4.BackColor = Color.FromArgb(224, 224, 224);
        Label4.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label4.Location = new System.Drawing.Point(13, 156);
        Label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label4.Name = "lblSearch";
        Label4.Size = new System.Drawing.Size(68, 22);
        Label4.TabIndex = 3;
        Label4.Text = "erbaut";
        Label4.TextAlign = ContentAlignment.MiddleCenter;
        Label5.BackColor = Color.FromArgb(224, 224, 224);
        Label5.Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Label5.Location = new System.Drawing.Point(5, 221);
        Label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label5.Name = "lblSorting";
        Label5.Size = new System.Drawing.Size(81, 22);
        Label5.TabIndex = 4;
        Label5.Text = "abgängig";
        Label5.TextAlign = ContentAlignment.MiddleCenter;
        TextBox1.Location = new System.Drawing.Point(16, 38);
        TextBox1.Margin = new System.Windows.Forms.Padding(4);
        TextBox1.Name = "edtPredicate";
        TextBox1.Size = new System.Drawing.Size(63, 25);
        TextBox1.TabIndex = 5;
        TextBox2.Location = new System.Drawing.Point(88, 38);
        TextBox2.Margin = new System.Windows.Forms.Padding(4);
        TextBox2.Multiline = true;
        TextBox2.Name = "edtSuburb";
        TextBox2.Size = new System.Drawing.Size(400, 234);
        TextBox2.TabIndex = 6;
        TextBox3.Location = new System.Drawing.Point(504, 38);
        TextBox3.Margin = new System.Windows.Forms.Padding(4);
        TextBox3.Multiline = true;
        TextBox3.Name = "edtCounty";
        TextBox3.Size = new System.Drawing.Size(400, 234);
        TextBox3.TabIndex = 7;
        TextBox4.Location = new System.Drawing.Point(13, 247);
        TextBox4.Margin = new System.Windows.Forms.Padding(4);
        TextBox4.Name = "edtCountry";
        TextBox4.Size = new System.Drawing.Size(73, 25);
        TextBox4.TabIndex = 8;
        TextBox5.Location = new System.Drawing.Point(16, 182);
        TextBox5.Margin = new System.Windows.Forms.Padding(4);
        TextBox5.Name = "edtNameprefix";
        TextBox5.Size = new System.Drawing.Size(73, 25);
        TextBox5.TabIndex = 9;
        Button1.Location = new System.Drawing.Point(423, 309);
        Button1.Margin = new System.Windows.Forms.Padding(4);
        Button1.Name = "btnReqHint";
        Button1.Size = new System.Drawing.Size(141, 35);
        Button1.TabIndex = 10;
        Button1.Text = "btnReqHint";
        Button1.UseVisualStyleBackColor = true;
        Button2.Location = new System.Drawing.Point(583, 309);
        Button2.Margin = new System.Windows.Forms.Padding(4);
        Button2.Name = "btnRegisterSearch";
        Button2.Size = new System.Drawing.Size(141, 35);
        Button2.TabIndex = 11;
        Button2.Text = "btnRegisterSearch";
        Button2.UseVisualStyleBackColor = true;
        Button2.Visible = false;
        Button3.Location = new System.Drawing.Point(765, 309);
        Button3.Margin = new System.Windows.Forms.Padding(4);
        Button3.Name = "btnReenter";
        Button3.Size = new System.Drawing.Size(141, 35);
        Button3.TabIndex = 12;
        Button3.Text = "btnReenter";
        Button3.UseVisualStyleBackColor = true;
        Label7.AutoSize = true;
        Label7.Location = new System.Drawing.Point(346, 327);
        Label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        Label7.Name = "lblRemark";
        Label7.Size = new System.Drawing.Size(51, 17);
        Label7.TabIndex = 14;
        Label7.Text = "lblRemark";
        AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(928, 357);
        ControlBox = false;
        Controls.Add(Label7);
        Controls.Add(Button3);
        Controls.Add(Button2);
        Controls.Add(Button1);
        Controls.Add(TextBox5);
        Controls.Add(TextBox4);
        Controls.Add(TextBox3);
        Controls.Add(TextBox2);
        Controls.Add(TextBox1);
        Controls.Add(Label5);
        Controls.Add(Label4);
        Controls.Add(Label3);
        Controls.Add(Label2);
        Controls.Add(Label1);
        Font = new System.Drawing.Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        Margin = new System.Windows.Forms.Padding(4);
        MaximizeBox = false;
        Name = "Grundbucheintrag";
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Grundbucheintrag";
        ResumeLayout(false);
        PerformLayout();
    }

    private void TextBox3_TextChanged(object sender, EventArgs e)
    {
    }

    private void Grundbucheintrag_Load(object sender, EventArgs e)
    {
        if (Modul1.FontSize > 0f)
        {
            Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Label1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Label2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Label3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Label4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
            Label5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        }
        BackColor = Menue.Default.BackColor;
        Top = checked(MainProject.Forms.HGakte.Label7.Top - MainProject.Forms.HGakte.Label7.Height + MainProject.Forms.HGakte.Label7.Height);
        Left = MainProject.Forms.HGakte.Label7.Left;
        Button1.Text = Modul1.IText[EUserText.tDelete];
        Button2.Text = Modul1.IText[EUserText.tNMCancel];
        Button3.Text = Modul1.IText[EUserText.t113];
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
    }
    private void Button3_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_035e, IL_039a
        int try0001_dispatch = -1;
        int num3 = default;
        int num2 = default;
        int num = default;
        int lErl = default;
        bool flag = default;
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                switch (try0001_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_0008;
                    case 1105:
                        {
                            num2 = num;
                            switch ((num3 <= -2) ? 1 : num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_039d;
                                default:
                                    goto end_IL_0001;
                            }
                            if (Information.Err().Number != 3022)
                            {
                                goto end_IL_0001_2;
                            }
                            ProjectData.ClearProjectError();
                            if (num2 == 0)
                            {
                                throw ProjectData.CreateProjectError(-2146828268);
                            }
                            num2 = 0;
                            goto IL_0348;
                        }
                    end_IL_0001:
                        break;
                    IL_0008:
                        num = 2;
                        DataModul.DB_GbeTable.Index = "Nr";
                        DataModul.DB_GbeTable.Seek("=", Label7.Tag.AsInt());
                        if ((TextBox2.Text + TextBox3.Text + TextBox4.Text + TextBox5.Text).Trim() == "")
                        {
                            DataModul.DB_GbeTable.Delete();
                            Close();
                            goto end_IL_0001_2;
                        }
                        else
                        {
                            flag = false;
                            if (DataModul.DB_GbeTable.Fields[GBEFields.Jahr].AsString().Trim() != TextBox1.Text)
                            {
                                flag = true;
                            }
                            if (DataModul.DB_GbeTable.Fields[GBEFields.Name].AsString().Trim() != TextBox2.Text)
                            {
                                flag = true;
                            }
                            if (DataModul.DB_GbeTable.Fields[GBEFields.Geb].AsString().Trim() != TextBox3.Text)
                            {
                                flag = true;
                            }
                            if (DataModul.DB_GbeTable.Fields[GBEFields.Erb].AsString().Trim() != TextBox5.Text)
                            {
                                flag = true;
                            }
                            if (DataModul.DB_GbeTable.Fields[GBEFields.Abg].AsString().Trim() != TextBox4.Text)
                            {
                                flag = true;
                            }
                            if (flag)
                            {
                                DataModul.DB_GbeTable.Edit();
                                DataModul.DB_GbeTable.Fields[GBEFields.Jahr].Value = TextBox1.Text;
                                DataModul.DB_GbeTable.Fields[GBEFields.Name].Value = TextBox2.Text;
                                DataModul.DB_GbeTable.Fields[GBEFields.Geb].Value = TextBox3.Text;
                                DataModul.DB_GbeTable.Fields["erb"].Value = TextBox5.Text;
                                DataModul.DB_GbeTable.Fields[GBEFields.Abg].Value = TextBox4.Text;
                                DataModul.DB_GbeTable.Update();
                            }
                        }
                        goto IL_0348;
                    IL_0348: // <========== 3
                        num = 34;
                        lErl = 3;
                        Close();
                        goto end_IL_0001_2;
                    IL_039d:
                        num4 = num2 + 1;
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 33:
                            case 34:
                            case 39:
                                goto IL_0348;
                            case 7:
                            case 36:
                            case 40:
                            case 41:
                                goto end_IL_0001_2;
                        }
                        goto default;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj, lErl);
                try0001_dispatch = 1105;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
        end_IL_0001_2: // <========== 4
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }

    internal DialogResult ShowDialog(int iGBENr, string sJahr, string sName, string sGeb, string sErb, string sAbg)
    {
        Label7.Text = iGBENr.ToString();
        Label7.Tag = iGBENr;
        TextBox1.Text =  sJahr;
        TextBox2.Text =  sName;
        TextBox3.Text =  sGeb ;
        TextBox5.Text =  sErb ;
        TextBox4.Text = sAbg;
        return base.ShowDialog();
    }
}
