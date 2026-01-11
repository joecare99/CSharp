using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SharpHack.AI;
using SharpHack.Base.Interfaces;
using SharpHack.Combat;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Persist;
using SharpHack.ViewModel;
using SharpHack.Wpf.Services;
using BaseLib.Models; // CRandom
using BaseLib.Models.Interfaces; // IRandom

namespace SharpHack.MapExport;

public static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            var options = ExportOptions.Parse(args);

            var services = new ServiceCollection();

            // Core services
            services.AddSingleton<IRandom, CRandom>();
            services.AddSingleton<ICombatSystem, SimpleCombatSystem>();
            services.AddSingleton<IEnemyAI, SimpleEnemyAI>();
            services.AddSingleton<IGamePersist, InMemoryGamePersist>();

            // Map generator
            services.AddSingleton<IMapGenerator, BSPMapGenerator>();

            // WPF tile service
            services.AddSingleton<ITileService, TileService>();
           
            //GameSession
            services.AddSingleton<GameSession>();

            var sp = services.BuildServiceProvider();

            var tileService = sp.GetRequiredService<ITileService>();
            tileService.LoadTileset(options.TileSheetPath, options.TileSize);

            var session = sp.GetRequiredService<GameSession>();

            session.RevealAll(visible: true);

            var vm = new GameViewModel(session, viewWidth: session.Map.Width, viewHeight: session.Map.Height);
            vm.SetViewSize(session.Map.Width, session.Map.Height);

            var exporter = new WpfMapExporter(tileService);
            exporter.Export(vm.DisplayTiles, session.Map.Width, session.Map.Height, options.TileSize, options.OutputPath);

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private sealed record ExportOptions(string TileSheetPath, int TileSize, string OutputPath)
    {
        public static ExportOptions Parse(string[] args)
        {
            // Defaults: try to read SharpHack.Wpf\tiles.tileset.json if present.
            string? tryJson = TryFindTilesetJson();
            string defaultTileSheet = "tiles2.png"; // matches TileService.EnsureLoaded default
            int defaultTileSize = 96; // matches TileService.EnsureLoaded default

            if (tryJson != null)
            {
                TryReadTilesetJson(tryJson, ref defaultTileSheet, ref defaultTileSize);
            }

            string tileSheetPath = defaultTileSheet;
            int tileSize = defaultTileSize;
            string output = Path.Combine(Environment.CurrentDirectory, "map.png");

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--tiles":
                    case "-t":
                        tileSheetPath = args[++i];
                        break;
                    case "--tileSize":
                    case "-s":
                        tileSize = int.Parse(args[++i]);
                        break;
                    case "--out":
                    case "-o":
                        output = args[++i];
                        break;
                }
            }

            // Allow relative tiles path next to the executable
            if (!Path.IsPathRooted(tileSheetPath))
            {
                var local = Path.Combine(AppContext.BaseDirectory, tileSheetPath);
                if (File.Exists(local))
                {
                    tileSheetPath = local;
                }
            }

            return new ExportOptions(tileSheetPath, tileSize, output);
        }

        private static string? TryFindTilesetJson()
        {
            // Look next to executable
            var local = Path.Combine(AppContext.BaseDirectory, "tiles2.tileset.json");
            if (File.Exists(local)) return local;

            // Look in repo layout (best-effort)
            var repo = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var inRepo = Path.Combine(repo, "SharpHack.Wpf", "tiles2.tileset.json");
            if (File.Exists(inRepo)) return inRepo;

            return null;
        }

        private static void TryReadTilesetJson(string tilesetJsonPath, ref string tileSheetPath, ref int tileSize)
        {
            try
            {
                using var fs = File.OpenRead(tilesetJsonPath);
                using var doc = JsonDocument.Parse(fs);
                if (doc.RootElement.TryGetProperty("TileSheetPath", out var p))
                    tileSheetPath = p.GetString() ?? tileSheetPath;
                if (doc.RootElement.TryGetProperty("Grid", out var g) && g.TryGetProperty("TileWidth", out var tw))
                    tileSize = tw.GetInt32();
            }
            catch
            {
                // ignore
            }
        }
    }
}
