using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class HttpClientProxyTests
{
    [TestMethod]
    public async Task GetAsync_Returns_Response_From_Local_Server()
    {
        using var xListener = CreateListener();
        var iPort = ((IPEndPoint)xListener.LocalEndpoint).Port;
        var xServerTask = RunServerAsync(xListener, "HTTP/1.1 200 OK\r\nContent-Length: 2\r\nConnection: close\r\n\r\nok");

        using var xProxy = new HttpClientProxy();
        using var xResponse = await xProxy.GetAsync($"http://127.0.0.1:{iPort}/test");
        var sBody = await xResponse.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.OK, xResponse.StatusCode);
        Assert.AreEqual("ok", sBody);
        await xServerTask;
    }

    [TestMethod]
    public void Dispose_Can_Be_Called_Multiple_Times()
    {
        var xProxy = new HttpClientProxy();

        xProxy.Dispose();
        xProxy.Dispose();

        Assert.IsTrue(true);
    }

    private static TcpListener CreateListener()
    {
        var xListener = new TcpListener(IPAddress.Loopback, 0);
        xListener.Start();
        return xListener;
    }

    private static async Task RunServerAsync(TcpListener xListener, string sResponse)
    {
        using var xClient = await xListener.AcceptTcpClientAsync();
        await using var xStream = xClient.GetStream();
        var arrBuffer = new byte[4096];
        _ = await xStream.ReadAsync(arrBuffer);
        var arrResponse = Encoding.ASCII.GetBytes(sResponse);
        await xStream.WriteAsync(arrResponse);
        await xStream.FlushAsync();
        xListener.Stop();
    }
}
