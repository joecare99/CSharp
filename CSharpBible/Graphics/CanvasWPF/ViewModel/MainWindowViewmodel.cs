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

namespace CanvasWPF.ViewModel
{
	/// <summary>
	/// Class MainWindowViewModel.
	/// Implements the <see cref="BaseViewModel" />
	/// </summary>
	/// <seealso cref="BaseViewModel" />
	public class MainWindowViewModel : BaseViewModel
	{

		/// <summary>
		/// The r
		/// </summary>
		private Random r = new Random();

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
		private ObservableCollection<ObservablePoint> _rectangles = new ObservableCollection<ObservablePoint>();
		/// <summary>
		/// The circles
		/// </summary>
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
		/// Gets or sets the rectangles.
		/// </summary>
		/// <value>The rectangles.</value>
		public ObservableCollection<ObservablePoint> Rectangles
		{
			get => _rectangles; set
			{
				if (value == _rectangles) return;
				_rectangles = value;
				RaisePropertyChanged();
			}
		}
		/// <summary>
		/// Gets or sets the circles.
		/// </summary>
		/// <value>The circles.</value>
		public ObservableCollection<ObservablePoint> Circles
		{
			get => _circles; set
			{
				if (value == _circles) return;
				_circles = value;
				RaisePropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the dc select shape.
		/// </summary>
		/// <value>The dc select shape.</value>
		public DelegateCommand dcSelectShape { get; set; }
		/// <summary>
		/// Gets or sets the dc create shape.
		/// </summary>
		/// <value>The dc create shape.</value>
		public DelegateCommand dcCreateShape { get; set; }
		/// <summary>
		/// Gets or sets the dc delete shape.
		/// </summary>
		/// <value>The dc delete shape.</value>
		public DelegateCommand dcDeleteShape { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
		/// </summary>
		public MainWindowViewModel()
		{
			this.dcCreateShape = new DelegateCommand(
				(o) => DoCreateShape(o),
				(o) => _editShape != null
			);

			this.dcSelectShape = new DelegateCommand(
				(o) => DoSelectShape(o),
				(o) => true
			);

			this.dcDeleteShape = new DelegateCommand(
				(o) => DoDeleteShape(o),
				(o) => true
			);

			for (int i = 0; i < 5; i++)
			{
				CreateRectangle();
				CreateCircle();
			}
		}

#if NET5_0_OR_GREATER
		private void DoCreateShape(object? o) => throw new NotImplementedException();
		private void DoSelectShape(object? o)
#else
        		/// <summary>
        		/// Does the create shape.
        		/// </summary>
        		/// <param name="o">The o.</param>
        		/// <exception cref="System.NotImplementedException"></exception>
        		private void DoCreateShape(object o) => throw new NotImplementedException();
/// <summary>
/// Does the select shape.
/// </summary>
/// <param name="o">The o.</param>
/// <exception cref="System.NotImplementedException"></exception>
private void DoSelectShape(object o)
#endif
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
			lock (_rectangles)
				_rectangles.Add(new ObservablePoint(r.Next(0, 600), r.Next(0, 400)) { Dx = r.NextDouble() * 40 - 20, Dy = r.NextDouble() * 40 - 20 });
		}

		/// <summary>
		/// Creates the circle.
		/// </summary>
		private void CreateCircle()
		{
			lock (_circles)
				_circles.Add(new ObservablePoint(r.Next(0, 600), r.Next(0, 400)) { Dx = r.NextDouble() * 40 - 20, Dy = r.NextDouble() * 40 - 20 });
		}


#if NET5_0_OR_GREATER
		private void DoDeleteShape(object? o) => throw new NotImplementedException();
#else
		/// <summary>
		/// Does the delete shape.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		private void DoDeleteShape(object o) => throw new NotImplementedException();
#endif
	}
}
