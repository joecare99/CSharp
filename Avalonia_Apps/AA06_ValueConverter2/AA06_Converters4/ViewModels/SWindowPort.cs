// ***********************************************************************
// Assembly  : AA06_Converters_4
// Author           : Mir
// Created    : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="SWindowPort.cs" company="JC-Soft">
// (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Drawing;

namespace AA06_Converters_4.ViewModels;

/// <summary>
/// Struct SWindowPort
/// </summary>
public struct SWindowPort
{
    /// <summary>
    /// The port
    /// </summary>
    public RectangleF port;
    /// <summary>
    /// The window size
    /// </summary>
    public Avalonia.Size WindowSize;
    /// <summary>
    /// The parent
    /// </summary>
  public PlotFrameViewModel Parent;
}
