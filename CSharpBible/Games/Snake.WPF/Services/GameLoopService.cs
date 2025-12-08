using System;
using System.Threading;
using System.Windows.Threading;
using Snake_Base.Models.Interfaces;
using Snake_Base.ViewModels;

namespace Snake.WPF.Services
{
    public class GameLoopService
    {
        private readonly ISnakeViewModel _game;
        private readonly Dispatcher _dispatcher;
        private Timer? _timer;

        public GameLoopService(ISnakeViewModel game, Dispatcher dispatcher)
        {
            _game = game;
            _dispatcher = dispatcher;
        }

        public void Start()
        {
            _game.ResetCommand?.Execute(null); 
            _timer = new Timer(Tick, null, 0, Timeout.Infinite);
        }

        public void Stop()
        {
            _timer?.Dispose();
            _game.PauseCommand?.Execute(null);
        }

        private void Tick(object? state)
        {
            if (!_game.IsRunning)
            {
                _timer?.Dispose();
                return;
            }
            var delay = _game.GameTick();
            _dispatcher.BeginInvoke(() =>
            {
                // UI can react if needed; rendering via bindings
            });
            _timer?.Change(delay, Timeout.Infinite);
        }
    }
}
