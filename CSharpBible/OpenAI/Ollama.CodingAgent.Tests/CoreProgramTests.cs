using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.Tests;

[TestClass]
public sealed class CoreProgramTests
{
    [TestMethod]
    public async Task Main_ExecutesHelpRuntimeBaselineAndDelegationModes()
    {
        using LocalOllamaServer server = new();
        using TestWorkspace workspace = new();

        Assert.AreEqual(0, await InvokeMain(["--help"]));
        Assert.AreEqual(0, await InvokeMain(["--endpoint", server.Endpoint.AbsoluteUri, "--model", "model", "--prompt", "Answer."]));
        Assert.AreEqual(0, await InvokeMain(["--endpoint", server.Endpoint.AbsoluteUri, "--model", "model", "--preflight"]));
        Assert.AreEqual(0, await InvokeMain(["--endpoint", server.Endpoint.AbsoluteUri, "--model", "model", "--baseline-smoke", "--prompt", "Answer."]));
        using LocalOllamaServer thinkingServer = new(includeThinking: true);
        Assert.AreEqual(0, await InvokeMain(["--endpoint", thinkingServer.Endpoint.AbsoluteUri, "--model", "model", "--show-thinking", "--prompt", "Answer."]));
        using LocalOllamaServer unavailableServer = new(modelAvailable: false);
        Assert.AreEqual(1, await InvokeMain(["--endpoint", unavailableServer.Endpoint.AbsoluteUri, "--model", "model", "--preflight"]));
        Assert.AreEqual(0, await InvokeMain(
        [
            "--endpoint", server.Endpoint.AbsoluteUri,
            "--model", "model",
            "--workspace-root", workspace.RootPath,
            "--delegate",
            "--prompt", "Inspect files.",
        ]));

        int unusedPort = LocalOllamaServer.GetUnusedPort();
        Assert.AreEqual(1, await InvokeMain(["--endpoint", $"http://127.0.0.1:{unusedPort}/", "--model", "model", "--prompt", "Answer."]));
    }

    private static Task<int> InvokeMain(string[] arguments)
    {
        MethodInfo main = typeof(OllamaAgentCliOptions).Assembly
            .GetType("Ollama.CodingAgent.Program", throwOnError: true)!
            .GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (Task<int>)main.Invoke(null, [arguments])!;
    }

    private sealed class LocalOllamaServer : IDisposable
    {
        private readonly CancellationTokenSource _cancellation = new();
        private readonly HttpListener _listener = new();
        private readonly Task _acceptingTask;

        public LocalOllamaServer(bool modelAvailable = true, bool includeThinking = false)
        {
            int port = GetUnusedPort();
            Endpoint = new Uri($"http://127.0.0.1:{port}/");
            ModelAvailable = modelAvailable;
            IncludeThinking = includeThinking;
            _listener.Prefixes.Add(Endpoint.AbsoluteUri);
            _listener.Start();
            _acceptingTask = AcceptAsync(_cancellation.Token);
        }

        public Uri Endpoint { get; }

        public bool ModelAvailable { get; }

        public bool IncludeThinking { get; }

        public static int GetUnusedPort()
        {
            using TcpListener reservation = new(IPAddress.Loopback, 0);
            reservation.Start();
            return ((IPEndPoint)reservation.LocalEndpoint).Port;
        }

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

                string body = context.Request.Url!.AbsolutePath == "/api/tags"
                    ? ModelAvailable ? """{"models":[{"name":"model"}]}""" : """{"models":[{"name":"other"}]}"""
                    : IncludeThinking
                        ? """{"message":{"role":"assistant","content":"answer"},"thinking":"reasoning","done":true}""" + Environment.NewLine
                        : """{"message":{"role":"assistant","content":"answer"},"done":true}""" + Environment.NewLine;
                byte[] payload = Encoding.UTF8.GetBytes(body);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = context.Request.Url.AbsolutePath == "/api/tags" ? "application/json" : "application/x-ndjson";
                context.Response.ContentLength64 = payload.Length;
                await context.Response.OutputStream.WriteAsync(payload, cancellationToken);
                context.Response.Close();
            }
        }
    }
}
