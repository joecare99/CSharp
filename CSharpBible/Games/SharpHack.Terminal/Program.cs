using BaseLib.Interfaces;
using BaseLib.Models;
using SharpHack.AI;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using SharpHack.Combat;
using SharpHack.Engine;
using SharpHack.LevelGen.BSP;
using SharpHack.Persist;
using SharpHack.ViewModel;

namespace SharpHack.Terminal;

public static class Program
{
    private const int HudY = 22;

    public static void Main(string[] args)
    {
        IConsole console = new ConsoleProxy();

        var random = new CRandom();
        var mapGenerator = new BSPMapGenerator(random);
        var combatSystem = new SimpleCombatSystem();
        var enemyAI = new SimpleEnemyAI();
        var gamePersist = new InMemoryGamePersist();

        var session = new GameSession(mapGenerator, gamePersist, random, combatSystem, enemyAI);
        var vm = new GameViewModel(session);

        // Terminal-friendly: fixed view; ASCII output.
        vm.SetViewSize(80, 22);

        bool autoDoorOpen = true;
        vm.AutoDoorOpen = autoDoorOpen;

        console.CursorVisible = false;
        console.Title = "SharpHack (Terminal)";

        var running = true;
        while (running)
        {
            Render(console, vm, autoDoorOpen);

            var key = console.ReadKey()?.Key ?? ConsoleKey.NoName;
            running = HandleInput(console, vm, ref autoDoorOpen, key);
        }
    }

    private static bool HandleInput(IConsole console, GameViewModel vm, ref bool autoDoorOpen, ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow: vm.Move(Direction.North); break;
            case ConsoleKey.DownArrow: vm.Move(Direction.South); break;
            case ConsoleKey.LeftArrow: vm.Move(Direction.West); break;
            case ConsoleKey.RightArrow: vm.Move(Direction.East); break;
            case ConsoleKey.NumPad7: vm.Move(Direction.NorthWest); break;
            case ConsoleKey.NumPad9: vm.Move(Direction.NorthEast); break;
            case ConsoleKey.NumPad1: vm.Move(Direction.SouthWest); break;
            case ConsoleKey.NumPad3: vm.Move(Direction.SouthEast); break;
            case ConsoleKey.NumPad5:
            case ConsoleKey.OemPeriod:
                vm.Wait();
                break;

            case ConsoleKey.O: vm.OpenDoorNearby(); break;
            case ConsoleKey.C: vm.CloseDoorNearby(); break;
            case ConsoleKey.T: vm.ToggleDoorNearby(); break;

            case ConsoleKey.A:
                autoDoorOpen = !autoDoorOpen;
                vm.AutoDoorOpen = autoDoorOpen;
                vm.Messages.Add($"AutoDoorOpen: {(autoDoorOpen ? "ON" : "OFF")}");
                break;

            case ConsoleKey.Enter:
                vm.ExecutePrimaryAction();
                break;

            case ConsoleKey.G:
                GoRelative(console, vm);
                break;

            case ConsoleKey.Escape:
            case ConsoleKey.Q:
                return false;
        }

        return true;
    }

    private static void GoRelative(IConsole console, GameViewModel vm)
    {
        var pp = vm.Player.Position;
        var (dx, dy) = ReadIntPairAtHud(console, $"GoTo (dx,dy) from [{pp.X},{pp.Y}]: ");
        var target = new Point(pp.X + dx, pp.Y + dy);

        if (!vm.Map.IsValid(target))
        {
            vm.Messages.Add("Target is outside of the map.");
            return;
        }

        vm.GoToWorldAsync(target).GetAwaiter().GetResult();
    }

    private static (int dx, int dy) ReadIntPairAtHud(IConsole console, string prompt)
    {
        var prevCursorVisible = console.CursorVisible;
        var prevFore = console.ForegroundColor;
        var prevBack = console.BackgroundColor;

        try
        {
            console.CursorVisible = true;
            console.ForegroundColor = ConsoleColor.White;
            console.BackgroundColor = ConsoleColor.Black;

            while (true)
            {
                console.SetCursorPosition(0, HudY);
                console.Write(new string(' ', Math.Max(1, console.BufferWidth - 1)));
                console.SetCursorPosition(0, HudY);
                console.Write(prompt);

                var inputX = Math.Min(console.BufferWidth - 1, prompt.Length);
                console.SetCursorPosition(inputX, HudY);

                var s = console.ReadLine();
                var parts = s.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && int.TryParse(parts[0], out var dx) && int.TryParse(parts[1], out var dy))
                {
                    return (dx, dy);
                }
            }
        }
        finally
        {
            console.ForegroundColor = prevFore;
            console.BackgroundColor = prevBack;
            console.CursorVisible = prevCursorVisible;
        }
    }

    private static void Render(IConsole console, GameViewModel vm, bool autoDoorOpen)
    {
        // Clear + draw full frame (simple and robust for SSH terminals)
        console.SetCursorPosition(0, 0);

        int w = vm.ViewWidth;
        int h = vm.ViewHeight;

        var tiles = vm.DisplayTiles;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                var t = tiles[y * w + x];
                console.Write(ToChar(t));
            }
            console.WriteLine();
        }

        console.ForegroundColor = ConsoleColor.White;
        console.SetCursorPosition(0, h);
        console.Write($"HP: {vm.HP}/{vm.MaxHP}  Lvl: {vm.Level}  AutoDoorOpen: {(autoDoorOpen ? "ON" : "OFF")}");
        console.Write(new string(' ', 20));

        console.SetCursorPosition(0, h + 1);
        var msg = vm.Messages.Count > 0 ? vm.Messages[^1] : string.Empty;
        var hint = vm.PrimaryActionHint;
        if (!string.IsNullOrWhiteSpace(hint) && hint != msg)
        {
            msg = hint;
        }
        console.Write(msg);
        console.Write(new string(' ', 20));
    }

    private static char ToChar(DisplayTile t) => t switch
    {
        DisplayTile.Archaeologist => '@',
        DisplayTile.Goblin => 'g',
        DisplayTile.Sword => '/',
        DisplayTile.Armor => '[',

        DisplayTile.Floor_Lit or DisplayTile.Floor_Dark => '.',

        DisplayTile.Wall_NS or DisplayTile.Wall_EW or DisplayTile.Wall_EN or DisplayTile.Wall_NW or DisplayTile.Wall_ES or DisplayTile.Wall_WS or DisplayTile.Wall_ENWS or DisplayTile.Wall_ENW or DisplayTile.Wall_EWS or DisplayTile.Wall_NWS or DisplayTile.Wall_ENS => '#',

        DisplayTile.Door_Closed_EW or DisplayTile.Door_Closed_NS => '+',
        DisplayTile.Door_Open_EW or DisplayTile.Door_Open_NS => '/',

        DisplayTile.Stairs_Up => '<',
        DisplayTile.Stairs_Down => '>',

        _ => ' '
    };
}
