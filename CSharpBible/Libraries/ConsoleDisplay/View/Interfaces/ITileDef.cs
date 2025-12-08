// ***********************************************************************
// Assembly         : ConsoleDisplay
// Author           : Mir
// Created          : 08-19-2022
//
// Last Modified By : Mir
// Last Modified On : 08-27-2022
// ***********************************************************************
// <copyright file="TileDisplay.cs" company="ConsoleDisplay">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace ConsoleDisplay.View;

public interface ITileDef
{
    Size TileSize { get; }

    (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile);
}