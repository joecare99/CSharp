using Pattern_00_Template.Views;
using System;
using System.Windows.Input;

namespace Pattern_00_Template
{
    public static class Program
    {
        private static ICommand? _view;

        public static Action<string[]> Run { get; set; } = DoRun;
        public static Func<string[],bool> Init { get; set; } = DoInit;

        public static void Main(params string[] args) {
            if (Init(args))
                Run(args);
        }

        private static void DoRun(string[] args)
        {
            _view?.Execute(args);
        }

        private static bool DoInit(string[] args)
        {
            // Init View 
            _view = new MainView();
                
            return _view.CanExecute(args);
        }
    }
}
