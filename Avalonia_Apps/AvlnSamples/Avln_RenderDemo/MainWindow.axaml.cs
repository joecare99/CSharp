using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;
using RenderDemo.ViewModels;

namespace RenderDemo
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            this.AttachDevTools();

            _vm = vm;

            void BindOverlay(string propertyName, RendererDebugOverlays overlay)
            {
                bool GetValue()
                    => (bool)(typeof(MainWindowViewModel).GetProperty(propertyName)!
                        .GetValue(_vm)!);

                void Apply(bool x)
                {
                    var diagnostics = RendererDiagnostics;
                    diagnostics.DebugOverlays = x
                        ? diagnostics.DebugOverlays | overlay
                        : diagnostics.DebugOverlays & ~overlay;
                }

                // initial apply
                Apply(GetValue());

                // subscribe using INotifyPropertyChanged
                if (_vm is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged += (_, e) =>
                    {
                        if (e.PropertyName == propertyName)
                        {
                            Apply(GetValue());
                        }
                    };
                }
            }

            BindOverlay("DrawDirtyRects", RendererDebugOverlays.DirtyRects);
            BindOverlay("DrawFps", RendererDebugOverlays.Fps);
            BindOverlay("DrawLayoutTimeGraph", RendererDebugOverlays.LayoutTimeGraph);
            BindOverlay("DrawRenderTimeGraph", RendererDebugOverlays.RenderTimeGraph);

            DataContext = _vm;
        }

        private void InitializeComponent()
        {
        }
    }
}
