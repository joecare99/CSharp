// ***********************************************************************
// Assembly         : Sokoban
// Author           : Mir
// Created          : 07-09-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Playfield.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Drawing;

namespace Sokoban.Models.Interfaces
{
    public interface IPlayfield
    {
        IField? this[Point p] { get; set; }

        bool GameSolved { get; }
        int StonesInDest { get; }
        IPlayer? player { get; }
        Size fieldSize { get; }
        IList<IPlayObject> Stones { get; }

        void Clear();
        void Setup((FieldDef[], Size) SFDef);
        void Setup(string[] SFDef);
        FieldDef VField(Point p, IList<Move> moves);
    }
}