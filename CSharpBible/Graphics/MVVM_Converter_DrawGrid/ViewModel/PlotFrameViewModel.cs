using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Converter_DrawGrid.Model;
using System.Windows.Controls;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_DrawGrid.ViewModel
{

    /// <summary>
    /// Class PlotFrameViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class PlotFrameViewModel : BaseViewModel
    {
        private SWindowPort _viewPort;

        /// <summary>
        /// Gets or sets the window port.
        /// </summary>
        /// <value>The window port.</value>
        public SWindowPort WindowPort { get => _viewPort; set => SetProperty(ref _viewPort, value); }

        /// <summary>
        /// Gets or sets the tiles.
        /// </summary>
        /// <value>The tiles.</value>
        public TileData[] Tiles { get => _viewPort.tiles; set => SetProperty(ref _viewPort.tiles, value,new string[] { nameof(WindowPort) }); }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotFrameViewModel"/> class.
        /// </summary>
        public PlotFrameViewModel()
        {
            //           VPWindow = new RectangleF(-300, 300, 9, 6);
            //           VPWindow = new RectangleF(-3, -3, 9, 6);
            //           VPWindow = new RectangleF(-3, -3, 900, 600);
            var _tiles = new TileData[20];
            for (int i = 0; i < 20; i++)
            {
                _tiles[i].Idx = i;
                _tiles[i].position =
                    _tiles[i].destination = new Point((i % 5) * 3, (i / 5) * 3);
                _tiles[i].tileType = i % 9;
            }
            Tiles = _tiles;
            Model.Model.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object? sender, (string, object, object) e)
        {
            if (e.Item3 is FieldDef[] fd)
            {
                TileData[] result = new TileData[fd.Length];
                for (int i = 0; i < fd.Length; i++)
                {
                    result[i].Idx = i;
                    result[i].position =
                        result[i].destination = new Point(i % 20, i / 20);
                    result[i].tileType = (int)fd[i];
                }
                Tiles = result;
            }
        }
    }
}
