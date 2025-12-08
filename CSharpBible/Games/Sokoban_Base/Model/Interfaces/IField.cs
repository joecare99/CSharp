// ***********************************************************************
// Assembly         : Sokoban_Base
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Field.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace Sokoban.Model.Interfaces
{
    public interface IField
    {
        FieldDef fieldDef { get; }
        IPlayObject? Item { get; set; }
        IPlayfield? Parent { get; }
        Point Position { get; }
    }
}