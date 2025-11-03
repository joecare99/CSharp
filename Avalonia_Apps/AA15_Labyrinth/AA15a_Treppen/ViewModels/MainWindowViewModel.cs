namespace AA15a_Treppen.ViewModels
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Treppen.Base;

    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IHeightLabyrinth _engine;

        [ObservableProperty]
        private int _size = 21;

        [ObservableProperty]
        private int[,] _grid = new int[0, 0];

        public MainWindowViewModel(IHeightLabyrinth engine)
        {
            _engine = engine;
            _engine.Dimension = new Rectangle(0, 0, _size, _size);
            _engine.UpdateCell += (_, p) => { /* Hook for live updates */ };
        }

        [RelayCommand]
        private void Generate()
        {
            _engine.Dimension = new Rectangle(0, 0, _size, _size);
            _engine.Generate();
            var g = new int[_size, _size];
            for (int x = 0; x < _size; x++)
                for (int y = 0; y < _size; y++)
                    g[x, y] = _engine[x, y];
            Grid = g;
        }
    }
}
