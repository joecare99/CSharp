using Snake_Base.ViewModel;
using Snake_Console.View;
using System.Threading;

namespace Snake_Console
{
    public static class Program
    {
        private static Game _game;
		private static UserAction action;
		private static int iDelay;

        static Program()
        {
            _game = new Game();
            Visual.SetGame(_game);
        }
        public static void Main(string[] args)
        {
            _game.Setup(1);
            Visual.FullRedraw();
            while (_game.IsRunning) {
                iDelay = _game.GameStep();
				Thread.Sleep(iDelay);
				Visual.CheckUserAction(out action);
				_game.HandleUserAct(action);
            }
    	}
    }
}
