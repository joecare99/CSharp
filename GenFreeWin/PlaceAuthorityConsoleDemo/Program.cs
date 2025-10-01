using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ConsoleLib;
using ConsoleLib.CommonControls;
using GenFreeBrowser.Places;
using GenFreeBrowser.Places.Interface;

namespace PlaceAuthorityConsoleDemo;

class Program
{
    private static TextBox _queryBox = null!;
    private static Label _status = null!;
    private static TextBox _resultBox = null!;
    private static IPlaceAuthority _nominatim = null!;
    private static IPlaceAuthority _gov = null!;
    private static HttpClient _http = null!;

    static async Task Main(string[] args)
    {
        Console.Title = "PlaceAuthority Console Demo";
        Console.Clear();
        _http = new HttpClient();
        _nominatim = new NominatimAuthority(_http);
        _gov = new GovAuthority(_http);

        // Root container
        var root = new Control { Position = new System.Drawing.Point(0,0), Dimension = new System.Drawing.Rectangle(0,0,Console.WindowWidth, Console.WindowHeight) };
        Control.MessageQueue = new();

        new Label { Parent = root, Position = new System.Drawing.Point(0,0), size = new System.Drawing.Size(20,1), Text = "Query:" };
        _queryBox = new TextBox { Parent = root, Position = new System.Drawing.Point(0,1), size = new System.Drawing.Size(Math.Max(20,Console.WindowWidth-2),1), MultiLine = false, BackColor = ConsoleColor.DarkBlue, ForeColor = ConsoleColor.White };
        _queryBox.Active = true;
        new Label { Parent = root, Position = new System.Drawing.Point(0,3), size = new System.Drawing.Size(20,1), Text = "Results:" };
        _resultBox = new TextBox { Parent = root, Position = new System.Drawing.Point(0,4), size = new System.Drawing.Size(Console.WindowWidth-2, Console.WindowHeight-6), MultiLine = true, BackColor = ConsoleColor.Black, ForeColor = ConsoleColor.Gray };
        _status = new Label { Parent = root, Position = new System.Drawing.Point(0, Console.WindowHeight-1), size = new System.Drawing.Size(Console.WindowWidth-1,1), BackColor = ConsoleColor.DarkGray, ForeColor = ConsoleColor.Black, Text = "Enter text and press ENTER to search (Nominatim + GOV). ESC to exit." };

        root.Invalidate();
        FlushQueue();

        while (true)
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept:true);
                if (key.Key == ConsoleKey.Escape) return;
                if (key.Key == ConsoleKey.Enter)
                {
                    var q = _queryBox.Text.Trim();
                    if (!string.IsNullOrEmpty(q))
                    {
                        _ = RunSearchAsync(q);
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (_queryBox.Text.Length>0)
                        _queryBox.SetText(_queryBox.Text.Substring(0,_queryBox.Text.Length-1));
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    _queryBox.SetText(_queryBox.Text + key.KeyChar);
                }
                FlushQueue();
            }
            await Task.Delay(30);
        }
    }

    private static async Task RunSearchAsync(string query)
    {
        try
        {
            _status.Text = "Searching ..."; _status.Invalidate(); FlushQueue();
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var nomTask = _nominatim.SearchAsync(new PlaceQuery(query), cts.Token);
            var govTask = _gov.SearchAsync(new PlaceQuery(query), cts.Token);
            await Task.WhenAll(nomTask, govTask);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("-- Nominatim --");
            foreach (var p in nomTask.Result) sb.AppendLine(p.ToString());
            sb.AppendLine("-- GOV --");
            foreach (var p in govTask.Result) sb.AppendLine(p.ToString());
            _resultBox.SetText(sb.ToString());
            _status.Text = $"Done ({nomTask.Result?.Count} + {govTask.Result?.Count})."; _status.Invalidate();
        }
        catch (Exception ex)
        {
            _resultBox.SetText(ex.Message);
            _status.Text = "Error."; _status.Invalidate();
        }
        FlushQueue();
    }

    private static void FlushQueue()
    {
        if (Control.MessageQueue == null) return;
        while (Control.MessageQueue.Count>0)
        {
            var (a,s,e)=Control.MessageQueue.Pop();
            a(s,e);
        }
    }
}
