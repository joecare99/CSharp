using System;

namespace Terminal.Wpf.Host;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new App
        {
            ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose
        };

        app.Run(new MainWindow());
    }
}
