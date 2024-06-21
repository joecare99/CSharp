// ***********************************************************************
// Assembly         : MVVM_Converter_DrawGrid2
// Author           : Mir
// Created          : 08-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-22-2022
// ***********************************************************************
// <copyright file="PlotFrameViewModel.cs" company="JC-Soft">
//     (c) by Joe Care 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System.Drawing;
using MVVM_Converter_DrawGrid2.Model;
using Sokoban_Base.Model;

namespace MVVM_Converter_DrawGrid2.ViewModel
{
    /// <summary>
    /// Struct TileData
    /// </summary>
    public struct TileData
    {
        /// <summary>
        /// The index
        /// </summary>
        public int Idx;
        /// <summary>
        /// The position
        /// </summary>
        public PointF position;
        /// <summary>
        /// The destination
        /// </summary>
        public Point destination;
        /// <summary>
        /// The tile type
        /// </summary>
        public int tileType;
    }

    /// <summary>
    /// Struct SWindowPort
    /// </summary>
    public struct SWindowPort 
    {
        /// <summary>
        /// The tiles
        /// </summary>
        public TileData[] tiles;
        /// <summary>
        /// The window size
        /// </summary>
        public System.Windows.Size WindowSize;
    }

    /// <summary>
    /// Class PlotFrameViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class PlotFrameViewModel : BaseViewModel
    {
        /// <summary>
        /// The Views port
        /// </summary>
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
        /// Gets or sets the size of the window.
        /// </summary>
        /// <value>The size of the window.</value>
        public System.Windows.Size WindowSize { get=>_viewPort.WindowSize; 
            set => SetProperty(ref _viewPort.WindowSize, value, new string[] { nameof(WindowPort) }); }

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
                    _tiles[i].destination = new Point((i % 5)*3, (i / 5)*3);
                _tiles[i].tileType = i % 9;
            }
            Tiles = _tiles;
            WindowSize = new System.Windows.Size(600, 400);
            Model.Model.PropertyChanged += ModelPropertyChanged;
        }

        /// <summary>
        /// Models the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ModelPropertyChanged(object? sender, (string, object, object) e)
        {
            if (e.Item3 is LabDefData ld)
            {
                TileData[] result = new TileData[ld.fields.Length];
                for (int i = 0; i < ld.fields.Length; i++)
                {
                    result[i].Idx = i;
                    result[i].position =
                        result[i].destination = new Point(i % ld.lSize.Width, i / ld.lSize.Width);
                    result[i].tileType = (ld.fields[i])switch {
                        FieldDef.Player =>1,
                        FieldDef.Stone => 5,
                        FieldDef.StoneInDest => 6,
                        FieldDef.Destination =>7,
                        FieldDef.Wall =>8,
                        FieldDef.Empty =>0,
                        _ =>0
                    };
                }
                Tiles = result;
            }
        }
    }
}
