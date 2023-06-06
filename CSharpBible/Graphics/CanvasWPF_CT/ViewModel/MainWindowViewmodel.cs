// ***********************************************************************
// Assembly         : CanvasWPF
// Author           : Mir
// Created          : 07-19-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="MainWindowViewmodel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using MVVM.ViewModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using BaseLib.Helper.MVVM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CanvasWPF_CT.ViewModel
{
	/// <summary>
	/// Class MainWindowViewModel.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public partial class MainWindowViewModel : BaseViewModelCT
	{

		/// <summary>
		/// The r
		/// </summary>
		private IRandom _rnd = new CRandom();

		/// <summary>
		/// Enum SelectedShape
		/// </summary>
		private enum SelectedShape { None, Circle, Rectangle }
		/// <summary>
		/// The shape1
		/// </summary>
		private SelectedShape Shape1 = SelectedShape.None;

		/// <summary>
		/// The t
		/// </summary>
		private Task? t;

		/// <summary>
		/// The edit shape
		/// </summary>
		private object? _editShape = null; // Wird mit einem Leeren Shape initialisiert
		/// <summary>
		/// The rectangles
		/// </summary>
		[ObservableProperty]
		private ObservableCollection<ObservablePoint> _rectangles = new ObservableCollection<ObservablePoint>();
        /// <summary>
        /// The circles
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ObservablePoint> _circles = new ObservableCollection<ObservablePoint>();
		/// <summary>
		/// The x running
		/// </summary>
		private bool xRunning;

		/// <summary>
		/// Occurs when [missing data].
		/// </summary>
		public event EventHandler? MissingData;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
		/// </summary>
		public MainWindowViewModel()
		{
			for (int i = 0; i < 5; i++)
			{
				CreateRectangle();
				CreateCircle();
			}
		}

		/// <summary>
		/// Does the create shape.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		[RelayCommand]
		private void CreateShape(object? o) 
			=> throw new NotImplementedException();

        /// <summary>
        /// Does the select shape.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        [RelayCommand]
        private void SelectShape(object? o)
		{
			if (o == null) return;
			switch ((string)o)
			{
				case "Rectangle":
					CreateRectangle();
					break;
				case "Circle":
					CreateCircle();
					break;
				case "Step":
					if (t == null)
					{
						xRunning = true;
						t = Task.Run(() =>
						{
							while (xRunning)
							{
								lock (_rectangles)
									foreach (var point in _rectangles) { point.Step(); }
								lock (_circles)
									foreach (var point in _circles) { point.Step(); }
								Thread.Sleep(5);
							}
						});
					}
					else
					{
						var _t = t;
						xRunning = false;
						_t.Wait();
						t = null;

					}
					break;
				default: throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Creates the rectangle.
		/// </summary>
		private void CreateRectangle()
		{
			lock (Rectangles)
				Rectangles.Add(new ObservablePoint(_rnd.Next(0, 600), _rnd.Next(0, 400)) { Dx = _rnd.NextDouble() * 40 - 20, Dy = _rnd.NextDouble() * 40 - 20 });
		}

		/// <summary>
		/// Creates the circle.
		/// </summary>
		private void CreateCircle()
		{
			lock (Circles)
				Circles.Add(new ObservablePoint(_rnd.Next(0, 600), _rnd.Next(0, 400)) { Dx = _rnd.NextDouble() * 40 - 20, Dy = _rnd.NextDouble() * 40 - 20 });
		}


		/// <summary>
		/// Does the delete shape.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		[RelayCommand]
		private void DeleteShape(object? o) => throw new NotImplementedException();
	}
}
