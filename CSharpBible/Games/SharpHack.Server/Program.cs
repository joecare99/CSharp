using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using SharpHack.Server.Net;
using SharpHack.Server.Remote;

namespace SharpHack.Server;

public static class Program
{
    public static async Task Main(string[] args)
    {
        int port = 2323;
        bool useTls = false;
        string? certPath = null;
        string? certPassword = null;
        bool requireClientCert = false;

        ParseArgs(args, ref port, ref useTls, ref certPath, ref certPassword, ref requireClientCert);

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        System.Console.WriteLine($"SharpHack server listening on 0.0.0.0:{port}");
        if (useTls)
        {
            System.Console.WriteLine("TLS enabled: connect with an SSL/TLS capable client, then use TELNET/RAW over the secure channel.");
            System.Console.WriteLine($"Certificate: {(string.IsNullOrWhiteSpace(certPath) ? "<from env SHARPHACK_TLS_CERT_PATH>" : certPath)}");
        }
        else
        {
            System.Console.WriteLine("PuTTY recommended: Connection type TELNET (preferred) or RAW");
        }
        System.Console.WriteLine("PuTTY Terminal settings: Local echo OFF, Local line editing OFF");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
            _ = Task.Run(() => HandleClient(client, useTls, certPath, certPassword, requireClientCert));
        }
    }

    private static void HandleClient(TcpClient client, bool useTls, string? certPath, string? certPassword, bool requireClientCert)
    {
        using (client)
        {
            var remote = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
            System.Console.WriteLine($"Client connected: {remote}");

            client.ReceiveTimeout = 60_000;
            client.SendTimeout = 60_000;
            client.NoDelay = true;

            using var rawStream = client.GetStream();

            Stream stream = rawStream;
            if (useTls)
            {
                try
                {
                    var cert = LoadCertificate(certPath, certPassword);
                    var ssl = new SslStream(
                        rawStream,
                        leaveInnerStreamOpen: false,
                        userCertificateValidationCallback: requireClientCert ? null : static (_, __, ___, ____) => true);

                    ssl.AuthenticateAsServer(
                        cert,
                        clientCertificateRequired: requireClientCert,
                        enabledSslProtocols: SslProtocols.Tls12 | SslProtocols.Tls13,
                        checkCertificateRevocation: false);

                    stream = ssl;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"[{remote}] TLS handshake failed: {ex.GetType().Name}: {ex.Message}");
                    return;
                }
            }

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

    private static void ParseArgs(string[] args, ref int port, ref bool useTls, ref string? certPath, ref string? certPassword, ref bool requireClientCert)
    {
        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a.Equals("--port", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && int.TryParse(args[i + 1], out var p))
            {
                port = p;
                i++;
                continue;
            }

            if (a.Equals("--tls", StringComparison.OrdinalIgnoreCase) || a.Equals("--ssl", StringComparison.OrdinalIgnoreCase))
            {
                useTls = true;
                continue;
            }

            if (a.Equals("--cert", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                certPath = args[i + 1];
                i++;
                continue;
            }

            if (a.Equals("--certpass", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                certPassword = args[i + 1];
                i++;
                continue;
            }

            if (a.Equals("--require-client-cert", StringComparison.OrdinalIgnoreCase))
            {
                requireClientCert = true;
                continue;
            }

            // Backward compatible: single numeric arg = port
            if (args.Length == 1 && int.TryParse(a, out var parsed))
            {
                port = parsed;
            }
        }

        if (!useTls)
        {
            var envTls = Environment.GetEnvironmentVariable("SHARPHACK_TLS") ?? Environment.GetEnvironmentVariable("SHARPHACK_SSL");
            if (!string.IsNullOrWhiteSpace(envTls) && (envTls.Equals("1") || envTls.Equals("true", StringComparison.OrdinalIgnoreCase) || envTls.Equals("yes", StringComparison.OrdinalIgnoreCase)))
            {
                useTls = true;
            }
        }

        certPath ??= Environment.GetEnvironmentVariable("SHARPHACK_TLS_CERT_PATH");
        certPassword ??= Environment.GetEnvironmentVariable("SHARPHACK_TLS_CERT_PASSWORD");

        if (useTls && string.IsNullOrWhiteSpace(certPath))
        {
            throw new InvalidOperationException("TLS is enabled but no certificate was provided. Use --cert <path> or set SHARPHACK_TLS_CERT_PATH.");
        }
    }

    private static X509Certificate2 LoadCertificate(string? certPath, string? certPassword)
    {
        if (string.IsNullOrWhiteSpace(certPath))
        {
            throw new InvalidOperationException("Certificate path is missing.");
        }

        if (!File.Exists(certPath))
        {
            throw new FileNotFoundException("Certificate file not found.", certPath);
        }

        return string.IsNullOrEmpty(certPassword)
            ? new X509Certificate2(certPath)
            : new X509Certificate2(certPath, certPassword);
    }
}
