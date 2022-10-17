using MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Converter_ImgGrid.Model;
using System.Windows.Controls;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_ImgGrid.ViewModel
{
    public struct TileData
    {
        public int Idx;
        public PointF position;
        public Point destination;
        public int tileType;
    }

    public struct SWindowPort 
    {
        public TileData[] tiles; 
        public System.Windows.Size WindowSize;
    }

    public class PlotFrameViewModel : BaseViewModel
    {
        private SWindowPort _viewPort;

        public SWindowPort WindowPort { get => _viewPort; set => SetProperty(ref _viewPort, value); }
        
        public TileData[] Tiles { get => _viewPort.tiles; set => SetProperty(ref _viewPort.tiles, value,new string[] { nameof(WindowPort) }); } 
        public System.Windows.Size WindowSize { get=>_viewPort.WindowSize; 
            set => SetProperty(ref _viewPort.WindowSize, value, new string[] { nameof(WindowPort) }); }

        public PlotFrameViewModel()
        {
 //           VPWindow = new RectangleF(-300, 300, 9, 6);
 //           VPWindow = new RectangleF(-3, -3, 9, 6);
            //           VPWindow = new RectangleF(-3, -3, 900, 600);
            Tiles = new TileData[20];
            WindowSize = new System.Windows.Size(600, 400);
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
