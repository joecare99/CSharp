// ***********************************************************************
// Assembly         : Werner_Flaschbier_Base
// Author           : Mir
// Created          : 08-02-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="WernerGame.cs" company="Werner_Flaschbier_Base">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;

namespace Werner_Flaschbier_Base.Model
{
    public interface IWernerGame
    {
        Size size { get; }
        int Level { get; }
        int Lives { get; }
        float TimeLeft { get; }
        bool isRunning { get; }
        int Score { get; }
        int MaxLives { get; }

        Enum this[Point p] { get; }

        event EventHandler<bool>? visUpdate;
        event EventHandler? visFullRedraw;
        event EventHandler? visShowHelp;

        void Setup(int? newLevel = null);
        void ReqQuit();
        void ReqHelp();
        void ReqRestart();
        void NextLvl();
        void PrevLvl();
        void MovePlayer(Direction action);
        void HandleGameLogic();
        int GameStep();
        Point OldPos(Point p);
    }
}