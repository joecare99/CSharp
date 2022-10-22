using MVVM_TiledDisplay.View.Converter;
using MVVM_TiledDisplay.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                (this.Resources["positionConverter"] as PositionConverter).WindowSize = evt.NewSize;
            };
        }

        private void Storyboard_Completed(object sender, EventArgs e) => (this.DataContext as TileViewViewModel).Storyboard_Completed(sender, e);

    }
}
