// ***********************************************************************
// Assembly         : JCAMS
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-24-2022
// ***********************************************************************
// <copyright file="CProtocol.cs" company="JC-Soft">
//     Copyright © JC-Soft 2008-2015
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace JCAMS.Core.Logging
{
    public interface IProtocol
    {
        bool Write(int Intervall_mSec, string Text);
        bool IsEventLog { get; }
        bool IsFile { get; }
        string Filename { get; }
    }
}