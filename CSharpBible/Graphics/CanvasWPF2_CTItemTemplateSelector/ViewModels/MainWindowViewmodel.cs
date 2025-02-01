using System;
using MVVM.ViewModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BaseLib.Interfaces;
using BaseLib.Models.Interfaces;
using BaseLib.Models;

namespace CanvasWPF2_CTItemTemplateSelector.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class MainWindowViewModel : BaseViewModelCT
	{
        #region Properties
        #region private Propeties
        private IRandom _rnd = new CRandom();

		private enum SelectedShape { None, Circle, Rectangle }
		private SelectedShape Shape1 = SelectedShape.None;

		private Task? t;

		private object? _editShape = null; // Wird mit einem Leeren Shape initialisiert
        
		/// <summary>
		/// Gets or sets the shapes.
		/// </summary>
		/// <value>The shapes.</value>
		[ObservableProperty]
        private ObservableCollection<ShapeData> _shapes = new ObservableCollection<ShapeData>();
		private bool xRunning;

		/// <summary>
		/// Gets or sets the data text.
		/// </summary>
		/// <value>The data text.</value>
		[ObservableProperty]
		private string _dataText="";
		#endregion

		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
		/// </summary>
		public MainWindowViewModel()
		{

            for (int i = 0; i < 5; i++)
			{
				CreateRectangle();
				CreateCircle();
				CreateHexagon();
                CreateTorus();
            }
        }

		[RelayCommand]
        private void KeyUp(object? o)
        {
            DataText = $"Data: Up {o}";
        }

        [RelayCommand]
        private void KeyDown(object? o)
        {
            DataText = $"Data: Down {o}";
        }

        [RelayCommand]
        private void CreateShape(object? o) => throw new NotImplementedException();

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
								lock (_shapes)
									foreach (var point in _shapes) { point.Step(); }
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

		private void CreateRectangle()
		{
            lock (Shapes)
				Shapes.Add(new ShapeData(_rnd.Next(0, 600), _rnd.Next(0, 400)) { Dx = _rnd.NextDouble() * 40 - 20, Dy = _rnd.NextDouble() * 40 - 20, SType = 0, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
		}

		private void CreateCircle()
		{
			lock (Shapes)
				Shapes.Add(new ShapeData(_rnd.Next(0, 600), _rnd.Next(0, 400)) { Dx = _rnd.NextDouble() * 40 - 20, Dy = _rnd.NextDouble() * 40 - 20 , SType = 1, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
		}

        private void CreateHexagon()
        {
            lock (Shapes)
                Shapes.Add(new ShapeData(_rnd.Next(0, 600), _rnd.Next(0, 400)) { Dx = _rnd.NextDouble() * 40 - 20, Dy = _rnd.NextDouble() * 40 - 20, SType = 2, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
        }

        private void CreateTorus()
        {
            lock (Shapes)
                Shapes.Add(new ShapeData(_rnd.Next(0, 600), _rnd.Next(0, 400)) { Dx = _rnd.NextDouble() * 40 - 20, Dy = _rnd.NextDouble() * 40 - 20, SType = 3, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
        }

        [RelayCommand]
        private void DeleteShape(object? o) => throw new NotImplementedException();

        private void Shape_MouseHover(object o)
		{
			if (o is ShapeData obj)
				DataText = $"{GetNameOfType(obj.SType)} at {obj.point}";
		}

		private static string GetNameOfType(int iType) 
			=> iType switch { 0 => "Rectangle", 1 => "Circle", 2 => "Hexagon", 3 => "Torus", _ => "Shape" };

		#endregion
	}
}
