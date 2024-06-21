using System;
using MVVM.ViewModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

namespace CanvasWPF2_ItemTemplateSelector.ViewModel
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class MainWindowViewModel : BaseViewModel
	{
        #region Properties
        #region private Propeties
        private Random r = new Random();

		private enum SelectedShape { None, Circle, Rectangle }
		private SelectedShape Shape1 = SelectedShape.None;

		private Task? t;

		private object? _editShape = null; // Wird mit einem Leeren Shape initialisiert
		private ObservableCollection<IVisualObject> _shapes = new ObservableCollection<IVisualObject>();
		private bool xRunning;
		private string _dataText="";
		#endregion

		/// <summary>
		/// Gets or sets the data text.
		/// </summary>
		/// <value>The data text.</value>
		public string DataText { get => _dataText; set => SetProperty(ref _dataText, value); }
		/// <summary>
		/// Gets or sets the shapes.
		/// </summary>
		/// <value>The shapes.</value>
		public ObservableCollection<IVisualObject> Shapes { get => _shapes; set => SetProperty(ref _shapes, value); }
		/// <summary>
		/// Gets or sets the dc select shape.
		/// </summary>
		/// <value>The dc select shape.</value>
		public DelegateCommand SelectShapeCommand { get; set; }
		/// <summary>
		/// Gets or sets the dc create shape.
		/// </summary>
		/// <value>The dc create shape.</value>
		public DelegateCommand CreateShapeCommand { get; set; }
		/// <summary>
		/// Gets or sets the dc delete shape.
		/// </summary>
		/// <value>The dc delete shape.</value>
		public DelegateCommand DeleteShapeCommand { get; set; }

		/// <summary>
		/// Gets or sets the dc key down.
		/// </summary>
		/// <value>The dc key down.</value>
		public DelegateCommand KeyDownCommand { get; set; }
		/// <summary>
		/// Gets or sets the dc key up.
		/// </summary>
		/// <value>The dc key up.</value>
		public DelegateCommand KeyUpCommand { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
		/// </summary>
		public MainWindowViewModel()
		{
			CreateShapeCommand = new DelegateCommand(
				(o) => DoCreateShape(o),
				(o) => _editShape != null
			);

			SelectShapeCommand = new DelegateCommand(
				(o) => DoSelectShape(o),
				(o) => true
			);

			DeleteShapeCommand = new DelegateCommand(
				(o) => DoDeleteShape(o),
				(o) => true
			);

			KeyDownCommand = new DelegateCommand((o) => { DataText = $"Data: Down {o}"; });

            KeyUpCommand = new DelegateCommand((o) => { DataText = $"Data: Up {o}"; });

            for (int i = 0; i < 5; i++)
			{
				CreateRectangle();
				CreateCircle();
				CreateHexagon();
                CreateTorus();
            }
        }

#if NET5_0_OR_GREATER
		private void DoCreateShape(object? o) => throw new NotImplementedException();
		private void DoSelectShape(object? o)
#else
        		private void DoCreateShape(object o) => throw new NotImplementedException();
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
			lock (_shapes)
				_shapes.Add(new ShapeData(r.Next(0, 600), r.Next(0, 400)) { Dx = r.NextDouble() * 40 - 20, Dy = r.NextDouble() * 40 - 20, sType = 0, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
		}

		private void CreateCircle()
		{
			lock (_shapes)
				_shapes.Add(new ShapeData(r.Next(0, 600), r.Next(0, 400)) { Dx = r.NextDouble() * 40 - 20, Dy = r.NextDouble() * 40 - 20 , sType = 1, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
		}

        private void CreateHexagon()
        {
            lock (_shapes)
                _shapes.Add(new ShapeData(r.Next(0, 600), r.Next(0, 400)) { Dx = r.NextDouble() * 40 - 20, Dy = r.NextDouble() * 40 - 20, sType = 2, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
        }

        private void CreateTorus()
        {
            lock (_shapes)
                _shapes.Add(new ShapeData(r.Next(0, 600), r.Next(0, 400)) { Dx = r.NextDouble() * 40 - 20, Dy = r.NextDouble() * 40 - 20, sType = 3, MouseHover = new DelegateCommand<object>(Shape_MouseHover) });
        }


#if NET5_0_OR_GREATER
        private void DoDeleteShape(object? o) => throw new NotImplementedException();
#else
		private void DoDeleteShape(object o) => throw new NotImplementedException();
#endif

        private void Shape_MouseHover(object o)
		{
			if (o is ShapeData obj)
				DataText = $"{GetNameOfType(obj.sType)} at {obj.point}";
		}

		private static string GetNameOfType(int iType) 
			=> iType switch { 0 => "Rectangle", 1 => "Circle", 2 => "Hexagon", 3 => "Torus", _ => "Shape" };

		#endregion
	}
}
