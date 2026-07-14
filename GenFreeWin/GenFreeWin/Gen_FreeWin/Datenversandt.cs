using GenFree;
using GenFree.Data;
using GenFree.Interfaces.Sys;
using GenFreeWin.Views;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Gen_FreeWin;

[DesignerGenerated]
public class Datenversandt : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
    IModul1 Modul1 => _Modul1.Instance;

    [AccessedThroughProperty(nameof(Button1))]
    private Button _Button1;

    [AccessedThroughProperty(nameof(Button2))]
    private Button _Button2;

    internal Label Label1;
    internal Label Label2;
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

    internal Label Label3;
    [DebuggerNonUserCode]
    public Datenversandt()
    {
        Load += Datenversandt_Load;
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

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
        Label1 = new Label();
        Label2 = new Label();
        Button1 = new Button();
        Button2 = new Button();
        Label3 = new Label();
        SuspendLayout();
        Label1.AutoSize = true;
        Label1.Location = new System.Drawing.Point(24, 26);
        Label1.Margin = new Padding(4, 0, 4, 0);
        Label1.Name = "Label1";
        Label1.Size = new System.Drawing.Size(51, 17);
        Label1.TabIndex = 0;
        Label1.Text = "Label1";
        Label2.AutoSize = true;
        Label2.Location = new System.Drawing.Point(27, 232);
        Label2.Name = "lblState";
        Label2.Size = new System.Drawing.Size(51, 17);
        Label2.TabIndex = 1;
        Label2.Text = "lblState";
        Button1.Location = new System.Drawing.Point(123, 336);
        Button1.Name = "btnReqHint";
        Button1.Size = new System.Drawing.Size(75, 23);
        Button1.TabIndex = 2;
        Button1.Text = "Start";
        Button1.UseVisualStyleBackColor = true;
        Button2.Location = new System.Drawing.Point(282, 336);
        Button2.Name = "btnRegisterSearch";
        Button2.Size = new System.Drawing.Size(75, 23);
        Button2.TabIndex = 3;
        Button2.Text = Modul1.IText[EUserText.tNMBack];
        Button2.UseVisualStyleBackColor = true;
        Label3.AutoSize = true;
        Label3.Location = new System.Drawing.Point(123, 384);
        Label3.Name = "lblDisplayHint";
        Label3.Size = new System.Drawing.Size(43, 17);
        Label3.TabIndex = 4;
        Label3.Text = "Label";
        AutoScaleDimensions = new System.Drawing.SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1028, 752);
        Controls.Add(Label3);
        Controls.Add(Button2);
        Controls.Add(Button1);
        Controls.Add(Label2);
        Controls.Add(Label1);
        Font = new System.Drawing.Font("Arial", 8.5f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        Margin = new Padding(4);
        Name = "Datenversandt";
        Text = "Datenversandt";
        ResumeLayout(false);
        PerformLayout();
    }

    private void Datenversandt_Load(object sender, EventArgs e)
    {
        var aiPos = Modul1.Persistence.ReadIntsProg("maspos.dat", 2);
        Left = aiPos[0];
        Top = aiPos[1];
        string text = "Hiermit können Sie die Daten dieses Mandanten per Mail-Anhang an Gisbert Berwe schicken. Beachten Sie folgende Hinweise:";
        text += "\n1.) Es muss ein Internetzugang aktiv sein.";
        text += "\n2.) Die Daten werden nicht komprimiert, dadurch kann die Übertragung sehr lange dauern.";
        text += "\n3.) Während der Übertragung erscheint evt. systembedingt der Hinweis >Keine Rückmeldung<, das ist kein Fehlerhinweis.";
        text += $"\n4.) Nach der Übertragung wird {Modul1.AppName} beendet und muß neu gestartet werden.";
        text += "\n5.) Dateien über 20 MB (20.000 MB)sollten nur in Ausnahmefällen versendet werden, ";
        Label1.Text = text;
        FileInfo fileInfo = new FileInfo(Modul1.Verz + "Gen_Plusdaten.mdb");
        text = "Übertragen wird die Datei:  >" + fileInfo.FullName.ToUpper() + "<  mit " + Strings.Format(fileInfo.Length / 1000000.0, "0.000") + " MB";
        Label2.Text = text;
        Label3.Text = "";
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        FileInfo fileInfo = new FileInfo(Modul1.Verz + "Gen_Plusdaten.mdb");
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Append);
        FileSystem.FileClose(99);
        FileSystem.FileOpen(99, Path.Combine(Modul1.GenFreeDir, "Adresse"), OpenMode.Input);
        string[] array = new string[10];
        var M1_Iter = 1;
        checked
        {
            while (!FileSystem.EOF(99))
            {
                array[M1_Iter] = FileSystem.LineInput(99);
                M1_Iter++;
                int i = M1_Iter;
                int num = 9;
                if (i > num)
                {
                    break;
                }
            }
            if (array[8].Trim() == "")
            {
                array[8] = "Adresse@fehlt.de";
            }
            MailMessage mailMessage = new MailMessage(array[8], "support@Genpluswin.de");
            SmtpClient smtpClient = new SmtpClient();
            SmtpClient smtpClient2 = smtpClient;
            smtpClient2.Host = "post.strato.de";
            smtpClient2.Port = 25;
            smtpClient2.UseDefaultCredentials = false;
            smtpClient2.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient2.Credentials = new NetworkCredential("support@Genpluswin.de", "support1");
            MailMessage mailMessage2 = mailMessage;
            mailMessage2.Subject = "Datenbank";
            mailMessage2.IsBodyHtml = false;
            mailMessage2.Body = array[2] + " " + array[3] + " " + Modul1.Verz;
            DataModul.MandDB.Close();
            DataModul.TempDB.Close();
            DataModul.DOSB.Close();
            DataModul.DSB.Close();
            mailMessage2.Attachments.Add(new Attachment(fileInfo.FullName));
            mailMessage2.Priority = MailPriority.Normal;
            try
            {
                smtpClient.Timeout = 1800000;
                Cursor = Cursors.WaitCursor;
                Label3.Text = "Übertragung gestartet";
                Application.DoEvents();
                smtpClient.Send(mailMessage);
                Application.DoEvents();
                _ = Interaction.MsgBox("Datei wurde versandt.");
                ProjectData.EndApp();
                Menue.Default.Show();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                _ = Interaction.MsgBox("Fehler: " + ex2.Message.ToString());
                ProjectData.EndApp();
                ProjectData.ClearProjectError();
            }
        }
    }
}
