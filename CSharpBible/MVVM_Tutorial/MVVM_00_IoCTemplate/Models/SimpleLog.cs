// ***********************************************************************
// Assembly         : MVVM_00_IoCTemplate
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Interfaces;
using System;
using System.Diagnostics;
using System.Globalization;

namespace MVVM_00_IoCTemplate;

public class SimpleLog(ISysTime sysTime):ILog
{
    public static Action<string> LogAction { get; set; } = (message) => Debug.WriteLine(message);
    ISysTime _sysTime { get; } = sysTime;

    public void Log(string message) 
        => LogAction($"{_sysTime.Now.ToString(CultureInfo.InvariantCulture)}: Msg: {message}");
    public void Log(string message, Exception exception) 
        => LogAction($"{_sysTime.Now.ToString(CultureInfo.InvariantCulture)}: Err: {message}, {exception}");
}