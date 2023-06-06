using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using System.Windows;
using System.Windows.Interop;

namespace CanvasWPF2_CTItemTemplateSelector.ViewModel
{
	/// <summary>
	/// Class ObservablePoint.
	/// Implements the <see cref="NotificationObject" />
	/// </summary>
	/// <seealso cref="NotificationObject" />
	public partial class ShapeData : NotificationObjectCT
	{
		#region Properties
		/// <summary>
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
		[ObservableProperty]
        private double _x;
		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
        [ObservableProperty]
        private double _y;
        [ObservableProperty]
		/// <summary>
		/// Gets or sets the dx.
		/// </summary>
		/// <value>The dx.</value>
        private double _dx;
        [ObservableProperty]
		/// <summary>
		/// Gets or sets the dy.
		/// </summary>
		/// <value>The dy.</value>
        private double _dy;
        [ObservableProperty]
		/// <summary>
		/// Gets or sets the type of the s.
		/// </summary>
		/// <value>The type of the s.</value>
        private int _sType;

		/// <summary>
		/// Gets the this.
		/// </summary>
		/// <value>The this.</value>
		public ShapeData This => this;

		/// <summary>
		/// The maximum x
		/// </summary>
		static public double maxX = 640;
		/// <summary>
		/// The maximum y
		/// </summary>
		static public double maxY = 400;
		/// <summary>
		/// Gets or sets the maximum size.
		/// </summary>
		/// <value>The maximum size.</value>
		public static Size maxSize { get => new Size(maxX, maxY); set => (maxX, maxY) = (value.Width, value.Height); }
		/// <summary>
		/// Gets or sets the point.
		/// </summary>
		/// <value>The point.</value>
		public Point point { get => new Point(X, Y); set => (X, Y) = (value.X, value.Y); }

		/// <summary>
		/// Gets or sets the mouse left button down.
		/// </summary>
		/// <value>The mouse left button down.</value>
		public DelegateCommand<object> MouseLeftButtonDown { get; set; } = default;
		/// <summary>
		/// Gets or sets the mouse hover.
		/// </summary>
		/// <value>The mouse hover.</value>
		public DelegateCommand<object> MouseHover { get; set; } = default;
		#endregion

		#region Methods
		/// <summary>
		/// Initializes a new instance of the <see cref="ShapeData"/> class.
		/// </summary>
		public ShapeData():this(new Point()) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ShapeData"/> class.
		/// </summary>
		/// <param name="p">The p.</param>
		public ShapeData(Point p):this(p.X, p.Y) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ShapeData"/> class.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		public ShapeData(double x, double y) { 
			(X, Y) = (x, y);
			MouseLeftButtonDown = new DelegateCommand<object>((o) => { });
		}

		const int iMargin = 20;
		/// <summary>
		/// Steps this instance.
		/// </summary>
		public void Step()
		{
			if (((X + Dx * 0.1 < iMargin ) && (Dx<0)) || ((X + Dx*0.1 > maxX- iMargin) && (Dx>0))) Dx = -Dx; 
			if (((Y + Dy * 0.1 < iMargin) && (Dy < 0)) || ((Y + Dy*0.1 > maxY- iMargin) && (Dy > 0))) Dy = -Dy; 
			(X, Y) = (X + Dx * 0.1, Y + Dy * 0.1);
		}
        #endregion
    }
}
