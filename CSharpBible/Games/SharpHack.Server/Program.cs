using System.Net;
using System.Net.Sockets;
using SharpHack.Server.Net;
using SharpHack.Server.Remote;

namespace SharpHack.Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        int port = 2323;
        if (args.Length == 1 && int.TryParse(args[0], out var parsed))
        {
            port = parsed;
        }

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        System.Console.WriteLine($"SharpHack server listening on 0.0.0.0:{port}");
        System.Console.WriteLine("PuTTY recommended: Connection type TELNET (preferred) or RAW");
        System.Console.WriteLine("PuTTY Terminal settings: Local echo OFF, Local line editing OFF");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
            _ = Task.Run(() => HandleClient(client));
        }
    }

    private static void HandleClient(TcpClient client)
    {
        using (client)
        {
            var remote = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
            System.Console.WriteLine($"Client connected: {remote}");

            client.ReceiveTimeout = 60_000;
            client.SendTimeout = 60_000;
            client.NoDelay = true;

            using var stream = client.GetStream();

            try
            {
                // Best-effort: if user connects via PuTTY TELNET, this helps to disable line mode / echo.
                TelnetNegotiation.SendBasicServerNegotiation(stream);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[{remote}] Telnet negotiation failed: {ex.Message}");
            }

            void Log(string msg) => System.Console.WriteLine($"[{remote}] {msg}");

            var console = new RemoteAnsiConsole(stream, log: Log);

            console.ForegroundColor = ConsoleColor.Yellow;
            console.BackgroundColor = ConsoleColor.Black;
            console.SetCursorPosition(0, 0);
            console.Write("SharpHack remote console connected. (If no keys: use PuTTY TELNET)\n");
            console.ForegroundColor = ConsoleColor.Gray;

            var app = new SharpHack.Console.ConsoleGameApp(console);

            try
            {
                app.Run();
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(ex);
            }
            finally
            {
                System.Console.WriteLine($"Client disconnected: {remote}");
            }
        }
    }
}
