using System;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RenderDemo.ViewModels
{
    public partial class Transform3DPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private double _depth = 200;
        
        [ObservableProperty]
        private double _centerX = 0;
        [ObservableProperty]
        private double _centerY = 0;
        [ObservableProperty]
        private double _centerZ = 0;
        [ObservableProperty]
        private double _angleX = 0;
        [ObservableProperty]
        private double _angleY = 0;
        [ObservableProperty]
        private double _angleZ = 0;
        
    }
}
