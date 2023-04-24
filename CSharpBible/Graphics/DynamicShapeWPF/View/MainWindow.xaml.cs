// ***********************************************************************
// Assembly         : DynamicShapes
// Author           : Mir
// Created          : 06-16-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

/// <summary>
/// The Dynamic Shape - View namespace.
/// </summary>
namespace DynamicShapeWPF.View {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		#region Property
		/// <summary>
		/// Enum SelectedShape
		/// </summary>
		private enum SelectedShape { None, Circle, Rectangle }

		/// <summary>
		/// The shape1
		/// </summary>
		private SelectedShape Shape1 = SelectedShape.None;
		#endregion

		#region Methode
		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow" /> class.
		/// </summary>
		public MainWindow() {
			InitializeComponent();
		}

		/// <summary>
		/// Handles the Click event of the CreateShape1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CreateShape1_Click(object sender, RoutedEventArgs e) {
			Shape1 = SelectedShape.Rectangle;
		}

		/// <summary>
		/// Handles the Click event of the CreateShape2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
		private void CreateShape2_Click(object sender, RoutedEventArgs e) {
			Shape1 = SelectedShape.Circle;
		}

		/// <summary>
		/// Handles the MouseLeftButtonDown event of the canvasArea control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void canvasArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

            Shape? Rendershape;
            switch (Shape1) {

			case SelectedShape.Circle:

				Rendershape = new Ellipse() { Height = 40, Width = 40 };

				RadialGradientBrush brush = new RadialGradientBrush();

				brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#00FF00"), 0.80));

				brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#7F0000FF"), 0.60));
				brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#00FF00"), 0.400));
				brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#0000FF"), 0.200));

				brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF7689"), 1));

				Rendershape.Fill = brush;

				break;

			case SelectedShape.Rectangle:

				Rendershape = new Rectangle() { Fill = Brushes.Blue, Height = 45, Width = 45, RadiusX = 12, RadiusY = 12 };

				break;

			default:

				return;


			}



			Canvas.SetLeft(Rendershape, e.GetPosition(canvasArea).X-Rendershape.Width / 2);

			Canvas.SetTop(Rendershape, e.GetPosition(canvasArea).Y - Rendershape.Height / 2);



			canvasArea.Children.Add(Rendershape);

		}



		/// <summary>
		/// Handles the MouseRightButtonDown event of the canvasArea control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
		private void canvasArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {

			Point pt = e.GetPosition((Canvas)sender);

			HitTestResult result = VisualTreeHelper.HitTest(canvasArea, pt);



			if (result != null) {

				canvasArea.Children.Remove(result.VisualHit as Shape);

			}

		}
        #endregion
    }

}
