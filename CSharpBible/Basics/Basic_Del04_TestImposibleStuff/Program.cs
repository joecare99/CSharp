using Basic_Del04_TestImposibleStuff.Views;
using System;
using System.Windows.Input;

namespace Basic_Del04_TestImposibleStuff
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

        public static void DoRun(string[] args)
        {
            _view?.Execute(args);
        }

        public static bool DoInit(string[] args)
        {
            // Init View 
            SetView( new MainView());                 
            return _view.CanExecute(args);
        }

        public static void SetView(ICommand view)=>_view = view;
    }
}
