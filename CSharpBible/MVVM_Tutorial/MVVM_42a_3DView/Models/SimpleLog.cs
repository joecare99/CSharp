// ***********************************************************************
// Assembly         : MVVM_42a_3DView
// Author           : Mir
// Created          : 03-09-2025
//
// Last Modified By : Mir
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="SimpleLog.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models.Interfaces;
using System;
using System.Diagnostics;
using System.Globalization;

namespace MVVM_42a_3DView;

public class SimpleLog(ISysTime sysTime) : ILog
{
    public static Action<string> LogAction { get; set; } = (message) => Debug.WriteLine(message);
    ISysTime _sysTime { get; } = sysTime;

    public void Log(string message)
        => LogAction($"{_sysTime.Now.ToString(CultureInfo.InvariantCulture)}: Msg: {message}");
    public void Log(string message, Exception exception)
        => LogAction($"{_sysTime.Now.ToString(CultureInfo.InvariantCulture)}: Err: {message}, {exception}");
}
