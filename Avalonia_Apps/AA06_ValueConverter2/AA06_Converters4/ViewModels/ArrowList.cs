// ***********************************************************************
// Assembly  : AA06_Converters_4
// Author           : Mir
// Created    : 08-28-2022
//
// Last Modified By : Mir
// Last Modified On : 08-28-2022
// ***********************************************************************
// <copyright file="ArrowList.cs" company="JC-Soft">
// (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;

namespace AA06_Converters_4.ViewModels;

/// <summary>
/// Struct DataSet
/// </summary>
public class ArrowList : List<ArrowData>
{
    /// <summary>
    /// The name
    /// </summary>
    public string? Name;
    /// <summary>
    /// The description
  /// </summary>
 public string? Description;
    /// <summary>
    /// The pen
    /// </summary>
    public (Avalonia.Media.IBrush, double)? PenData;
}
