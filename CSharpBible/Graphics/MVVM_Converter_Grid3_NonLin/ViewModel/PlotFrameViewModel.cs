using MVVM.ViewModel;
using System.Drawing;

namespace MVVM_Converter_Grid3_NonLin.ViewModel
{
    public struct SWindowPort
    {
        public RectangleF port; 
        public System.Windows.Size WindowSize;
    }

    public struct DataSet
    {
        public PointF[] Datapoints;
        public string Name;
        public string Description;
        public System.Windows.Media.Pen Pen;
    }

    public class PlotFrameViewModel : BaseViewModel
    {
        private SWindowPort _viewPort;

        public SWindowPort WindowPort { get => _viewPort; set => SetProperty(ref _viewPort, value); }
        
        public RectangleF VPWindow { get => _viewPort.port; set => SetProperty(ref _viewPort.port, value,new string[] { nameof(WindowPort) }); } 
        public System.Windows.Size WindowSize { get=>_viewPort.WindowSize; 
            set => SetProperty(ref _viewPort.WindowSize, value, new string[] { nameof(WindowPort) }); }

        public PlotFrameViewModel()
        {
 //           VPWindow = new RectangleF(-300, 300, 9, 6);
            VPWindow = new RectangleF(-1, -1, 3, 2);
            //           VPWindow = new RectangleF(-3, -3, 900, 600);
 //           VPWindow = new RectangleF(-0.03f, -0.03f, 0.09f, 0.06f);
            WindowSize = new System.Windows.Size(600, 400);
        }
        

    }
}
