using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SyncAsyncParallel.Model;

public class SyncAsyncModel : ISyncAsyncModel
{
    /// <summary>
    /// The url's
    /// </summary>
    readonly string[] urls = new string[]{
        "https://www.stackoverflow.com",
        "https://www.microsoft.com",
        "https://www.youtube.com",
        "https://www.windows.com",
        "https://www.asm.net",
        "https://www.jc99.de",
        "https://www.google.com",
        "https://www.yahoo.com",
        "https://www.bing.com",
        "https://www.gmail.com"
    };

    public long Download_sync(Action<string> actS)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<DownloadResult> results = new(urls.Length);
        foreach (string url in urls)
        {
            results.Add(_DownloadUrl(url));
            ShowResults(results,actS);
            Application.Current?.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
        watch.Stop();
        return watch.ElapsedMilliseconds;
    }

    public async Task<long> Download_async(Action<string> actS)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        List<DownloadResult> results = new(urls.Length);
        foreach (string url in urls)
        {
            results.Add(await Task.Run(() => _DownloadUrl(url)));
            ShowResults(results, actS);
        }
        watch.Stop();
        return watch.ElapsedMilliseconds;
    }

    public async Task<long> Download_async_para(Action<string> actS)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        List<Task<DownloadResult>> tasklist = new(urls.Length);
        foreach (string url in urls)
        {
            tasklist.Add(Task.Run(() => _DownloadUrl(url)));
        }

        List<DownloadResult> results = new();
        while (tasklist.Count > 0)
        {
            var result = await Task.WhenAny(tasklist);
            tasklist.Remove(result);
            results.Add(result.Result);
            ShowResults(results,actS);
        }
        watch.Stop();
        return watch.ElapsedMilliseconds;
    }

    /// <summary>
    /// Downloads the URL.
    /// </summary>
    /// <param name="url">The URL.</param>
    /// <returns>DownloadResult.</returns>
    private DownloadResult _DownloadUrl(string url)
    {
        using HttpClient client = new();
        try
        {
            string html = client.GetStringAsync(url).Result;
            return new DownloadResult()
            {
                Html = html,
                Url = url
            };
        }
        catch (Exception e)
        {
            return new DownloadResult()
            {
                Html = e.Message,
                Url = url
            };

        }
    }
    /// <summary>
    /// Shows the results.
    /// </summary>
    /// <param name="results">The results.</param>
    private void ShowResults(List<DownloadResult> results, Action<string> actS)
    {
        StringBuilder text = new();
        foreach (var result in results)
        {
            text.Append(result.Url);
            text.Append('\t');
            text.Append(result.ContentLength);
            text.Append(Environment.NewLine);
        }
        actS(text.ToString());

    }
}
