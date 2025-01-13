// ***********************************************************************
// Assembly         : Snake_Base
// Author           : Mir
// Created          : 08-25-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="SnakeGameObject.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Snake_Base.Models.Interfaces;

public interface ISnakeGameObject
{
    Point OldPlace { get; }

    void ResetOldPlace();
}