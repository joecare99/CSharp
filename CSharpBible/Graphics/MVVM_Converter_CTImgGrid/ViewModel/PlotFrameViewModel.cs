using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Converter_CTImgGrid.Model;
using System.Windows.Controls;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_CTImgGrid.ViewModel
{
    public struct TileData
    {
        public int Idx;
        public PointF position;
        public Point destination;
        public int tileType;
    }

    public class PlotFrameViewModel : BaseViewModel
    {
        private TileData[] _tiles;        
        public TileData[] Tiles { get => _tiles; set => SetProperty(ref _tiles, value); } 

        public PlotFrameViewModel()
        {
 //           VPWindow = new RectangleF(-300, 300, 9, 6);
 //           VPWindow = new RectangleF(-3, -3, 9, 6);
            //           VPWindow = new RectangleF(-3, -3, 900, 600);
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
