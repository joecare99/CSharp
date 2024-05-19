// ***********************************************************************
// Assembly         : PolySpline_net
// Author           : Mir
// Created          : 06-20-2022
//
// Last Modified By : Mir
// Last Modified On : 06-22-2022
// ***********************************************************************
// <copyright file="PolySplineViewModel.cs" company="PolySpline_net">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
namespace PolySpline.ViewModels;

/// <summary>
/// Class PolySplineViewModel.
/// </summary>
public partial class PolySplineViewModel : BaseViewModel
{
    /// <summary>
    /// Gets or sets the pl collection.
    /// </summary>
    /// <value>The pl collection.</value>
    public PolySplineCollection plCollection { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PolySplineViewModel"/> class.
    /// </summary>
    public PolySplineViewModel()
    {
        plCollection = new PolySplineCollection();
        var p = new PolySpline();
        p.Add(new Coordinate() { X = 100, Y = 100 });
        p.Add(new Coordinate() { X = 200, Y = 150 });
        p.Add(new Coordinate() { X = 250, Y = 200 });
        p.Add(new Coordinate() { X = 150, Y = 250 });
        p.Add(new Coordinate() { X = 250, Y = 300 });
        plCollection.Add(p);
    }   
}
