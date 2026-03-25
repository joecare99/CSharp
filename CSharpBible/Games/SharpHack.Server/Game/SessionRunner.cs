using BaseLib.Models;
using SharpHack.AI;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using SharpHack.Combat;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Persist;
using SharpHack.Server.Ansi;
using SharpHack.Server.Net;
using SharpHack.ViewModel;

namespace SharpHack.Server.Game;

internal sealed class SessionRunner
{
    private const int ViewWidth = 80;
    private const int ViewHeight = 22;

    private readonly AnsiWriter _ansi;
    private readonly AnsiKeyReader _keys;

    private readonly GameViewModel _vm;
    private bool _autoDoorOpen = true;

    public SessionRunner(Stream stream)
    {
        _ansi = new AnsiWriter(stream);
        _keys = new AnsiKeyReader(stream);

        var random = new CRandom();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();
        var gamePersist = new InMemoryGamePersist();

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        _vm = new GameViewModel(session);
        _vm.SetViewSize(ViewWidth, ViewHeight);
        _vm.AutoDoorOpen = _autoDoorOpen;
    }

    public async Task RunAsync(CancellationToken ct)
    {
        _ansi.Clear();
        _ansi.HideCursor();

        try
        {
            var running = true;
            while (running && !ct.IsCancellationRequested)
            {
                Render();
                running = await HandleInputAsync(ct).ConfigureAwait(false);
            }
        }
        finally
        {
            _ansi.Reset();
            _ansi.ShowCursor();
        }
    }

    private void Render()
    {
        _ansi.MoveCursor(1, 1);

        int w = _vm.ViewWidth;
        int h = _vm.ViewHeight;
        var tiles = _vm.DisplayTiles;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var t = tiles[y * w + x];
                var (ch, fg) = TileGlyphs.Map(t);
                _ansi.SetFg256(fg);
                _ansi.Write(ch.ToString());
            }
            _ansi.Reset();
            _ansi.Write("\n");
        }

        _ansi.Reset();
        _ansi.SetFg256(15);
        _ansi.Write($"HP: {_vm.HP}/{_vm.MaxHP}  Lvl: {_vm.Level}  AutoDoorOpen: {(_autoDoorOpen ? "ON" : "OFF")}      \n");

        var msg = _vm.Messages.Count > 0 ? _vm.Messages[^1] : string.Empty;
        var hint = _vm.PrimaryActionHint;
        if (!string.IsNullOrWhiteSpace(hint) && hint != msg)
        {
            msg = hint;
        }
        _ansi.Write(msg);
        _ansi.Write("\n");

        _ansi.Reset();
        _ansi.Write("Keys: Arrows/Numpad, Enter=Interact, A=AutoDoor, O/C/T, G=Go dx,dy, Q=Quit\n");
    }

    private async Task<bool> HandleInputAsync(CancellationToken ct)
    {
        var key = await _keys.ReadAsync(ct).ConfigureAwait(false);

        if (key.Key == AnsiKey.Char)
        {
            switch (char.ToLowerInvariant(key.Char))
            {
                case 'q':
                    return false;
                case 'o':
                    _vm.OpenDoorNearby();
                    return true;
                case 'c':
                    _vm.CloseDoorNearby();
                    return true;
                case 't':
                    _vm.ToggleDoorNearby();
                    return true;
                case 'a':
                    _autoDoorOpen = !_autoDoorOpen;
                    _vm.AutoDoorOpen = _autoDoorOpen;
                    _vm.Messages.Add($"AutoDoorOpen: {(_autoDoorOpen ? "ON" : "OFF")}");
                    return true;
                case 'g':
                    await GoPromptAsync(ct).ConfigureAwait(false);
                    return true;

                // roguelike movement fallback
                case 'h': _vm.Move(Direction.West); return true;
                case 'l': _vm.Move(Direction.East); return true;
                case 'k': _vm.Move(Direction.North); return true;
                case 'j': _vm.Move(Direction.South); return true;

                case '.':
                    _vm.Wait();
                    return true;
            }
        }

        switch (key.Key)
        {
            case AnsiKey.Up: _vm.Move(Direction.North); return true;
            case AnsiKey.Down: _vm.Move(Direction.South); return true;
            case AnsiKey.Left: _vm.Move(Direction.West); return true;
            case AnsiKey.Right: _vm.Move(Direction.East); return true;
            case AnsiKey.Enter: _vm.ExecutePrimaryAction(); return true;
            case AnsiKey.Escape: return false;
            default: return true;
        }
    }

    private async Task GoPromptAsync(CancellationToken ct)
    {
        var pp = _vm.Player.Position;
        _ansi.Reset();
        _ansi.SetFg256(15);
        _ansi.Write($"\nGoTo (dx,dy) from [{pp.X},{pp.Y}]: ");
        _ansi.ShowCursor();

        try
        {
            var line = await ReadLineAsync(ct).ConfigureAwait(false);
            var parts = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0], out var dx) && int.TryParse(parts[1], out var dy))
            {
                var target = new Point(pp.X + dx, pp.Y + dy);
                if (!_vm.Map.IsValid(target))
                {
                    _vm.Messages.Add("Target is outside of the map.");
                    return;
                }

                await _vm.GoToWorldAsync(target).ConfigureAwait(false);
            }
        }
        finally
        {
            _ansi.HideCursor();
        }
    }

    private async Task<string> ReadLineAsync(CancellationToken ct)
    {
        var sb = new System.Text.StringBuilder();

        while (!ct.IsCancellationRequested)
        {
            var k = await _keys.ReadAsync(ct).ConfigureAwait(false);
            if (k.Key == AnsiKey.Enter)
            {
                _ansi.Write("\n");
                return sb.ToString();
            }

            if (k.Key == AnsiKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Length -= 1;
                    _ansi.Write("\b \b");
                }
                continue;
            }

            if (k.Key == AnsiKey.Char)
            {
                sb.Append(k.Char);
                _ansi.Write(k.Char.ToString());
            }
        }

        return sb.ToString();
    }
}
