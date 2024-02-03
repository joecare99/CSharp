// ***********************************************************************
// Assembly         : MVVM_38_CTDependencyInjection
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="ITimer.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Timers;

namespace MVVM_38_CTDependencyInjection.Models.Interfaces
{
    public interface ITimer
    {
        double Interval { get; set; }
        bool Enabled { get; }

        event ElapsedEventHandler Elapsed; 

        void Start();
        void Stop();
    }
}