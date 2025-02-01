﻿// ***********************************************************************
// Assembly         : MVVM_25_RichTextEdit
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="RichTextEditModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.IO;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_25_RichTextEdit.Models;

/// <summary>
/// Class RichTextEditModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="MVVM_25_RichTextEdit.Models.IRichTextEditModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="MVVM_25_RichTextEdit.Models.IRichTextEditModel" />
/// <autogeneratedoc />
public partial class RichTextEditModel : ObservableObject, IRichTextEditModel
{
    private const string csApplStart = "Application startet";
#if !NET5_0_OR_GREATER
    private const string csApplEnded = "Application ended";
#endif
    #region Properties
    /// <summary>
    /// The timer
    /// </summary>
    /// <autogeneratedoc />
    private readonly Timer _timer;
    private readonly ISysTime _systime;
    private readonly ILog _log;

    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    /// <autogeneratedoc />
    public DateTime Now { get => _systime.Now; }

    public string EmptyText => @"<FlowDocument PagePadding=""5,0,5,0"" AllowDrop=""True"" NumberSubstitution.CultureSource=""User""
xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""></FlowDocument>";
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextEditModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public RichTextEditModel(ISysTime sysTime, ILog log)
    {
        _systime = sysTime;
        _log = log;
        _log.Log(csApplStart);
        _timer = new(250d);
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();
    }

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    ~RichTextEditModel()
    {
        _timer.Stop();
        _log.Log(csApplEnded);
        return;
    }
#endif

    public string DocumentFromStream(FileStream fs)
    {
        using var tr = new StreamReader(fs);
        var t = tr.ReadToEnd();
        if (t.StartsWith("<FlowDocument"))
        {
            return t;
        }
        else
        {
            return $"<FlowDocument PagePadding=\"5,0,5,0\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\"\r\n" +
                $"xml:lang=\"{CultureInfo.CurrentCulture.Name}\"\r\n" +
                $"xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n" +
                $"xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"><Paragraph>{t.Replace("\r\n\r\n", "</Paragraph>\r\n<Paragraph>").Replace("\r\n","<LineBreak/>")}</Paragraph></FlowDocument>";
        }
    }

    public void DocumentToStream(FileStream fs, string document)
    {
        using var tw = new StreamWriter(fs);
        tw.Write(document);
    }
    #endregion
}
