// ***********************************************************************
// Assembly         : CanvasWPF
// Author           : Mir
// Created          : 06-22-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="ObservablePoint.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.Windows;

namespace CanvasWPF_CT.ViewModel
{
	/// <summary>
	/// Class ObservablePoint.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public partial class ObservablePoint : BaseViewModelCT
	{
		/// <summary>
		/// The x
		/// </summary>
		[ObservableProperty]
		//		[NotifyPropertyChangedFor(nameof())]
		private double _x;
		/// <summary>
		/// The y
		/// </summary>
		[ObservableProperty]
		private double _y;

		/// <summary>
		/// Gets or sets the dx.
		/// </summary>
		/// <value>The dx.</value>
		[ObservableProperty]
		private double _dx;

		/// <summary>
		/// Gets or sets the dy.
		/// </summary>
		/// <value>The dy.</value>
		[ObservableProperty]
		private double _dy;

		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <value>The point.</value>
		Point point => new Point(X, Y);

		/// <summary>
		/// The maximum x
		/// </summary>
		static public double maxX = 640;
		/// <summary>
		/// The maximum y
		/// </summary>
		static public double maxY = 400;
		/// <summary>
		/// Initializes a new instance of the <see cref="ObservablePoint" /> class.
		/// </summary>
		public ObservablePoint() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ObservablePoint" /> class.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		public ObservablePoint(double x, double y) { (X, Y) = (x, y); }
		/// <summary>
		/// Initializes a new instance of the <see cref="ObservablePoint" /> class.
		/// </summary>
		/// <param name="p">The p.</param>
		public ObservablePoint(Point p) { (X, Y) = (p.X, p.Y); }

		/// <summary>
		/// Steps this instance.
		/// </summary>
		public void Step()
		{
			if (X + Dx * 0.1 < 0 || X + Dx > maxX) { Dx = -Dx; }
			if (Y + Dy * 0.1 < 0 || Y + Dy > maxY) { Dy = -Dy; }
			(X, Y) = (X + Dx * 0.1, Y + Dy * 0.1);
		}
        }
}
