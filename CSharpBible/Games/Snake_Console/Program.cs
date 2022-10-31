using Snake_Base.ViewModel;
using Snake_Console.View;

namespace Snake_Console
{
    public static class Program
    {
        private static Game _game;

        static Program()
        {
            _game = new Game();
            Visual.SetGame(_game);
        }
        public static void Main(string[] args)
        {
            while (_game.IsRunning)
                _game.GameStep();
        }
    }
}