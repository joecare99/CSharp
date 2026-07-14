// ***********************************************************************
// Assembly         : ConsoleLib
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 07-21-2022
// ***********************************************************************
// <copyright file="Panel.cs" company="ConsoleLib">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ConsoleLib.Data;
using ConsoleLib.Interfaces;
using System;
using System.Drawing;

namespace ConsoleLib.CommonControls;

/// <summary>
/// Class Panel.
/// Implements the <see cref="ConsoleLib.Control" />
/// </summary>
/// <seealso cref="ConsoleLib.Control" />
public class Panel : Control, IGroupControl, IHasBorder
{
    /// <summary>
    /// The border
    /// </summary>
    [Obsolete("Use BorderStyle property instead.")]
    public char[] Border
    {
        get => BorderDefinition.CustomChars ?? Array.Empty<char>();
        set
        {
            var _border = value ?? Array.Empty<char>();
            BorderDefinition = new BorderDef
            {
                Style = ResolveBorderStyle(_border),
                CustomChars = _border.Length > 0 ? _border : null
            };
        }
    }

    public IBorderDefinition BorderDefinition { get; set; } = new BorderDef
    { Style = BorderStyle.None };

    public BorderStyle BorderStyle
    {
        get => BorderDefinition.Style;
        set => BorderDefinition = new BorderDef
        {
            Style = value,
            CustomChars = value == BorderStyle.Custom ? BorderDefinition.CustomChars : null
        };
    }
    /// <summary>
    /// The border color
    /// </summary>
    public ConsoleColor BorderColor;

    private static BorderStyle ResolveBorderStyle(char[] border)
    {
        if (border == null || border.Length == 0)
            return BorderStyle.None;

        if (border.Length >= 6 && border[0] == '─' && border[1] == '│' && border[2] == '┌' && border[3] == '┐' && border[4] == '└' && border[5] == '┘')
            return BorderStyle.Single;

        if (border.Length >= 6 && border[0] == '═' && border[1] == '║' && border[2] == '╔' && border[3] == '╗' && border[4] == '╚' && border[5] == '╝')
            return BorderStyle.Double;

        if (border.Length >= 6 && border[0] == '-' && border[1] == '|' && border[2] == ',' && border[3] == ',' && border[4] == '\'' && border[5] == '\'')
            return BorderStyle.Simple;

        return BorderStyle.Custom;
    }

    public void BringToFront(IControl menuPopup)
    {
        if (Children.Contains(menuPopup))
        {
            // Bring the specified menuPopup to the front
            Children.Remove(menuPopup);
            Children.Insert(0, menuPopup);
        }
    }

    /// <summary>
    /// Draws this instance.
    /// </summary>
    public override void Draw()
    {
        WidgetSet?.DrawPanel(this);
    }

    /// <summary>
    /// Res the draw.
    /// </summary>
    /// <param name="dimension">The Dimension.</param>
    public override void ReDraw(Rectangle dimension)
    {
        WidgetSet?.RedrawPanel(this, dimension);
    }

}
