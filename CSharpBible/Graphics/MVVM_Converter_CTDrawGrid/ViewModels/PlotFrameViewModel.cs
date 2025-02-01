using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using MVVM_Converter_CTDrawGrid.Models.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_CTDrawGrid.ViewModel
{

    /// <summary>
    /// Class PlotFrameViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class PlotFrameViewModel : BaseViewModelCT
    {
        [ObservableProperty]
        private TileData[] _tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotFrameViewModel"/> class.
        /// </summary>
        public PlotFrameViewModel(IDrawGridModel drawGridModel)
        {
            var _tiles = new TileData[20];
            for (int i = 0; i < 20; i++)
            {
                _tiles[i].Idx = i;
                _tiles[i].position =
                    _tiles[i].destination = new Point((i % 5) * 3, (i / 5) * 3);
                _tiles[i].tileType = i % 9;
            }
            Tiles = _tiles;
            drawGridModel.PropertyChanged += ModelPropertyChanged;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ( e.PropertyName == nameof(IDrawGridModel.LevelData) &&
               sender.GetType().GetProperty(e.PropertyName).GetValue(sender) is FieldDef[] fd)
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
