// ***********************************************************************
// Assembly         : Snake_Console
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Visual.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace Snake_Console.View
{
    public interface IVisual
    {
        bool CheckUserAction();
        void FullRedraw(object? sender = null, EventArgs? e = null);
    }
}