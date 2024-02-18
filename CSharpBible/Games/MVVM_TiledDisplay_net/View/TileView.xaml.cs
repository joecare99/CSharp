using MVVM_TiledDisplay.View.Converter;
using MVVM_TiledDisplay.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MVVM_TiledDisplay.View
{
    /// <summary>
    /// Interaktionslogik für TileView.xaml
    /// </summary>
    public partial class TileView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileView"/> class.
        /// </summary>
        public TileView()
        {
            InitializeComponent();
            this.SizeChanged += (object sender, SizeChangedEventArgs evt) =>
            {
                if (this.Resources["positionConverter"] is PositionConverter pc)
                    pc.WindowSize = evt.NewSize;
            };
        }

        private void Storyboard_Completed(object sender, EventArgs e) => (this.DataContext as TileViewViewModel)?.Storyboard_Completed(sender, e);

    }
}
