// ***********************************************************************
// Assembly         : AA25_RichTextEdit
// ***********************************************************************
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.IO;
using System.Timers;

namespace AA25_RichTextEdit.Models;

public partial class RichTextEditModel : ObservableObject, IRichTextEditModel
{
    private readonly System.Timers.Timer _timer; // disambiguate Timer
    private readonly ISysTime _systime;
    private readonly ILog _log;

    public DateTime Now => _systime.Now;

    public string EmptyText => ""; // Plain text start for Avalonia

    public RichTextEditModel(ISysTime sysTime, ILog log)
    {
        _systime = sysTime;
        _log = log;
        _log.Log("Application startet");
        _timer = new System.Timers.Timer(250d);
        _timer.Elapsed += (_, _) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }

    public string DocumentFromStream(FileStream fs)
    {
        using var tr = new StreamReader(fs);
        return tr.ReadToEnd();
    }

    public void DocumentToStream(FileStream fs, string document)
    {
        using var tw = new StreamWriter(fs);
        tw.Write(document);
    }
}
