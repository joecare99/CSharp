using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RenderDemo.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _drawDirtyRects;
        [ObservableProperty]
        private bool _drawFps = true;
        [ObservableProperty]
        private bool _drawLayoutTimeGraph;
        [ObservableProperty]
        private bool _drawRenderTimeGraph;
        [ObservableProperty]
        private double _width = 800;
        [ObservableProperty]
        private double _height = 600;

        public MainWindowViewModel()
        {
            ToggleDrawDirtyRects = new RelayCommand(() => DrawDirtyRects = !DrawDirtyRects);
            ToggleDrawFps = new RelayCommand(() => DrawFps = !DrawFps);
            ToggleDrawLayoutTimeGraph = new RelayCommand(() => DrawLayoutTimeGraph = !DrawLayoutTimeGraph);
            ToggleDrawRenderTimeGraph = new RelayCommand(() => DrawRenderTimeGraph = !DrawRenderTimeGraph);
            ResizeWindow = new RelayCommand(async () => await ResizeWindowAsync());
        }


        public RelayCommand ToggleDrawDirtyRects { get; }
        public RelayCommand ToggleDrawFps { get; }
        public RelayCommand ToggleDrawLayoutTimeGraph { get; }
        public RelayCommand ToggleDrawRenderTimeGraph { get; }
        public RelayCommand ResizeWindow { get; }

        private async Task ResizeWindowAsync()
        {
            for (int i = 0; i < 30; i++)
            {
                Width += 10;
                Height += 5;
                await Task.Delay(10);
            }

            await Task.Delay(10);

            for (int i = 0; i < 30; i++)
            {
                Width -= 10;
                Height -= 5;
                await Task.Delay(10);
            }
        }
    }
}
