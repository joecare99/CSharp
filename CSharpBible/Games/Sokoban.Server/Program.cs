using BaseLib.Interfaces;
using RemoteTerminal.Remote;
using Sokoban.Models;
using Sokoban.View;
using Sokoban.ViewModels;
using System.Net;
using System.Net.Sockets;

namespace Sokoban.Server;

/// <summary>
/// Remote server host for Sokoban using a simple ANSI terminal protocol.
/// </summary>
public static class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args)
    {
        int port = TryGetPort(args, defaultPort: 2323);
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        while (true)
        {
            using TcpClient client = listener.AcceptTcpClient();
            using NetworkStream stream = client.GetStream();

            IConsole console = new RemoteAnsiConsole(stream);

            SetupVisuals(console);

            var game = new Game();
            Visuals.SokobanGame = game;
            game.Init();
            game.visSetMessage = (s) => Visuals.Message = s;
            game.visShow = Visuals.Show;
            game.visUpdate = Visuals.Update;
            game.visGetUserAction = Visuals.WaitforUser;

            while (LabDefs.SLevels.Length > game.level)
            {
                var action = game.Run();
                if (action == UserAction.Quit)
                {
                    break;
                }
            }

            game.Cleanup();
        }
    }

    private static void SetupVisuals(IConsole console)
    {
        Visuals.myConsole = console;
    }

    private static int TryGetPort(string[] args, int defaultPort)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (!string.Equals(args[i], "--port", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (int.TryParse(args[i + 1], out int port))
            {
                return port;
            }
        }

        return defaultPort;
    }
}
