// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-20-2022
// ***********************************************************************
// <copyright file="StopWatchExample.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace TestStatements.Diagnostics
{
    public interface IStopwatch
    {
        TimeSpan Elapsed { get; }
        long ElapsedTicks { get; }
        long ElapsedMilliseconds { get; }

        void Reset();
        void Start();
        void Stop();
    }
}