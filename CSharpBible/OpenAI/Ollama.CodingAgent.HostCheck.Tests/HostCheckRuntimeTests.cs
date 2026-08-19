using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.HostCheck.Tests;

[TestClass]
public sealed class HostCheckRuntimeTests
{
    [TestMethod]
    public async Task Main_RunsRegularAndDelegatedScenariosAgainstLoopbackOllama()
    {
        using LocalOllamaServer server = new();
        using TestWorkspace workspace = new();

        Assert.AreEqual(0, await InvokeMain(
        [
            "--endpoint", server.Endpoint.AbsoluteUri,
            "--model", "model",
            "--prompt", "Summarize the change.",
        ]));
        Assert.AreEqual(0, await InvokeMain(
        [
            "--endpoint", server.Endpoint.AbsoluteUri,
            "--model", "model",
            "--workspace-root", workspace.RootPath,
            "--delegate",
            "--prompt", "Inspect source files.",
        ]));
        Assert.AreEqual(0, await InvokeMain(
        [
            "--endpoint", server.Endpoint.AbsoluteUri,
            "--model", "model",
        ]));
        Assert.AreEqual(0, await InvokeMain(
        [
            "--endpoint", server.Endpoint.AbsoluteUri,
            "--model", "model",
            "--prompt", " ",
        ]));
        await Assert.ThrowsExactlyAsync<ArgumentException>(() => InvokeMain(["--unknown"]));
    }

    private static Task<int> InvokeMain(string[] arguments)
    {
        MethodInfo main = Assembly.Load("Ollama.CodingAgent.HostCheck")
            .GetType("Ollama.CodingAgent.HostCheck.Program", throwOnError: true)!
            .GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (Task<int>)main.Invoke(null, [arguments])!;
    }

    private sealed class TestWorkspace : IDisposable
    {
        public TestWorkspace()
        {
            RootPath = Path.Combine(AppContext.BaseDirectory, "CoverageWorkspaces", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(RootPath);
        }

        public string RootPath { get; }

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, recursive: true);
            }
        }
    }

    private sealed class LocalOllamaServer : IDisposable
    {
        private readonly CancellationTokenSource _cancellation = new();
        private readonly HttpListener _listener = new();
        private readonly Task _acceptingTask;

        public LocalOllamaServer()
        {
            using TcpListener reservation = new(System.Net.IPAddress.Loopback, 0);
            reservation.Start();
            int port = ((IPEndPoint)reservation.LocalEndpoint).Port;
            reservation.Stop();
            Endpoint = new Uri($"http://127.0.0.1:{port}/");
            _listener.Prefixes.Add(Endpoint.AbsoluteUri);
            _listener.Start();
            _acceptingTask = AcceptAsync(_cancellation.Token);
        }

        public Uri Endpoint { get; }

        public void Dispose()
        {
            _cancellation.Cancel();
            _listener.Close();
            try
            {
                _acceptingTask.GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }
            catch (HttpListenerException)
            {
            }

            _cancellation.Dispose();
        }

        private async Task AcceptAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync().WaitAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                byte[] payload = Encoding.UTF8.GetBytes("""{"message":{"role":"assistant","content":"answer"},"done":true}""" + Environment.NewLine);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/x-ndjson";
                context.Response.ContentLength64 = payload.Length;
                await context.Response.OutputStream.WriteAsync(payload, cancellationToken);
                context.Response.Close();
            }
        }
    }
}
