using MVVM.ViewModel;
using System.Drawing;
using Werner_Flaschbier_Base.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MVVM_Converter_CTImgGrid.ViewModel
{
    public struct TileData
    {
        public int Idx;
        public PointF position;
        public Point destination;
        public int tileType;
    }

    public partial class PlotFrameViewModel : BaseViewModelCT
    {
        [ObservableProperty]
        private TileData[] _tiles;        

        public PlotFrameViewModel()
        {
            _tiles = new TileData[20];
            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i].Idx = i;
                _tiles[i].position =
                    _tiles[i].destination = new Point((i % 5) * 3, (i / 5) * 3);
                _tiles[i].tileType = i % 9;
            }
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
