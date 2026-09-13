// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir (extended by AI)
// Created          : 09-26-2025
// ***********************************************************************
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleLib.Interfaces;

/// <summary>Interface for control containers that manage child layout and fixed anchoring.</summary>
public interface IGroupControl : IControl
{
    /// <summary>Summons a popup above all siblings in z-order.</summary>
    void BringToFront(IControl control);

    /// <summary>Adds a child anchored to the parent frame (drawn on/into parent canvas).</summary>
    IGroupControl AddChild(IControl control);

    Point Offset { get; }

}