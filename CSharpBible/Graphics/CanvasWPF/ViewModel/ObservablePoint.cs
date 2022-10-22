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
using MVVM.ViewModel;
using System.Windows;

namespace CanvasWPF.ViewModel
{
	/// <summary>
	/// Class ObservablePoint.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public class ObservablePoint : BaseViewModel        {
		/// <summary>
		/// The x
		/// </summary>
		private double _x;
		/// <summary>
		/// The y
		/// </summary>
		private double _y;

		/// <summary>
		/// Gets or sets the dx.
		/// </summary>
		/// <value>The dx.</value>
		public double Dx { get; set; }
		/// <summary>
		/// Gets or sets the dy.
		/// </summary>
		/// <value>The dy.</value>
		public double Dy { get; set; }

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
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
		public double X { get => _x; set { if (value == _x) return; _x = value; RaisePropertyChanged(); } }
		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public double Y { get => _y; set { if (value == _y) return; _y = value; RaisePropertyChanged(); } }

		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <value>The point.</value>
		Point point => new Point(X, Y);

		/// <summary>
		/// Steps this instance.
		/// </summary>
		public void Step()
		{
			if (X + Dx * 0.1 < 0 || X + Dx > maxX) { Dx = -Dx; RaisePropertyChanged(nameof(Dx)); }
			if (Y + Dy * 0.1 < 0 || Y + Dy > maxY) { Dy = -Dy; RaisePropertyChanged(nameof(Dy)); }
			(X, Y) = (X + Dx * 0.1, Y + Dy * 0.1);
		}
        }
}
