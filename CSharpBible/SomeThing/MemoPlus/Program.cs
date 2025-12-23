// Program.cs
// Kompaktes Konsolenprogramm: Notiz-Tool mit verstecktem Text-Adventure
// Benötigt: .NET 6+
// Kompilieren / Ausführen: dotnet new console; Datei ersetzen; dotnet run

using System.Text.Json;

class Note
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Created { get; set; } = "";
}

class GameState
{
    public bool Unlocked { get; set; } = false;
    public string Room { get; set; } = "entrance";
    public List<string> Inventory { get; set; } = new();
}

class Program
{
    const string DataDir = "data";
    static readonly string NotesFile = Path.Combine(DataDir, "notes.json");
    static readonly string StateFile = Path.Combine(DataDir, "game_state.json");
    const string SecretTrigger = "unlock 42";

    static JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    static void Main()
    {
        try
        {
            EnsureData();
            var state = LoadState();
            ShowHeader();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Menü: 1 List  2 Add  3 Status  4 Exit");
                Console.Write("Eingabe: ");
                var cmd = Console.ReadLine()?.Trim() ?? "";

                if (string.Equals(cmd, SecretTrigger, StringComparison.OrdinalIgnoreCase))
                {
                    state.Unlocked = true;
                    SaveState(state);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nGeheimer Modus aktiviert.");
                    Console.ResetColor();
                    PlayGame(state);
                    Console.WriteLine("\nZurück im Tool.");
                    continue;
                }

                if (cmd == "1" || cmd.Equals("list", StringComparison.OrdinalIgnoreCase) || cmd.Equals("l", StringComparison.OrdinalIgnoreCase))
                {
                    ListNotes();
                }
                else if (cmd == "2" || cmd.Equals("add", StringComparison.OrdinalIgnoreCase) || cmd.Equals("a", StringComparison.OrdinalIgnoreCase))
                {
                    AddNote();
                }
                else if (cmd == "3" || cmd.Equals("status", StringComparison.OrdinalIgnoreCase) || cmd.Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    ShowStatus();
                }
                else if (cmd == "4" || cmd.Equals("exit", StringComparison.OrdinalIgnoreCase) || cmd.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Auf Wiedersehen.");
                    break;
                }
                else
                {
                    if (cmd.ToLower().Contains("unlock") || cmd.Contains("42"))
                    {
                        Console.WriteLine("Unbekannter Befehl. Tipp: Versuche 'unlock 42'.");
                    }
                    else
                    {
                        Console.WriteLine("Unbekannter Befehl. Bitte wähle eine Menüoption.");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nBeendet.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nFehler: {ex.Message}");
        }
    }

    static void EnsureData()
    {
        Directory.CreateDirectory(DataDir);
        if (!File.Exists(NotesFile))
            File.WriteAllText(NotesFile, JsonSerializer.Serialize(new List<Note>(), jsonOptions));
        if (!File.Exists(StateFile))
            File.WriteAllText(StateFile, JsonSerializer.Serialize(new GameState(), jsonOptions));
    }

    static List<Note> LoadNotes()
    {
        try
        {
            var txt = File.ReadAllText(NotesFile);
            return JsonSerializer.Deserialize<List<Note>>(txt) ?? new List<Note>();
        }
        catch { return new List<Note>(); }
    }

    static void SaveNotes(List<Note> notes)
    {
        File.WriteAllText(NotesFile, JsonSerializer.Serialize(notes, jsonOptions));
    }

    static GameState LoadState()
    {
        try
        {
            var txt = File.ReadAllText(StateFile);
            return JsonSerializer.Deserialize<GameState>(txt) ?? new GameState();
        }
        catch { return new GameState(); }
    }

    static void SaveState(GameState state)
    {
        File.WriteAllText(StateFile, JsonSerializer.Serialize(state, jsonOptions));
    }

    static void ShowHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('=', 40));
        Console.WriteLine(" Mini-Tool  v0.1  — Notizen und Status");
        Console.WriteLine(new string('=', 40));
        Console.ResetColor();
    }

    static void ListNotes()
    {
        var notes = LoadNotes();
        if (notes.Count == 0)
        {
            Console.WriteLine("Keine Notizen vorhanden.");
            return;
        }
        for (int i = 0; i < notes.Count; i++)
        {
            var n = notes[i];
            Console.WriteLine($"{i + 1}. {n.Title} — {n.Created}");
        }
    }

    static void AddNote()
    {
        Console.Write("Titel: ");
        var title = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Text: ");
        var body = Console.ReadLine()?.Trim() ?? "";
        var notes = LoadNotes();
        notes.Add(new Note { Title = title, Body = body, Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm") });
        SaveNotes(notes);
        Console.WriteLine("Notiz gespeichert.");
    }

    static void ShowStatus()
    {
        Console.WriteLine("Systemstatus: Alles in Ordnung.");
        Console.WriteLine("Letzte Prüfung: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
    }

    // --- Verstecktes Spiel
    static void PlayGame(GameState state)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("\nDu betrittst eine andere Ebene...");
        Console.ResetColor();

        while (true)
        {
            var room = state.Room ?? "entrance";
            var inv = state.Inventory ?? new List<string>();

            if (room == "entrance")
            {
                Console.WriteLine("\nDu stehst in einem dunklen Flur. Türen nach Norden und Osten.");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (cmd == "n" || cmd == "north")
                    state.Room = "library";
                else if (cmd == "e" || cmd == "east")
                    state.Room = "workshop";
                else if (cmd == "look")
                    Console.WriteLine("An der Wand hängt ein altes Schild mit einer Zahl: 7.");
                else if (cmd == "inventory")
                    Console.WriteLine("Inventar: " + (inv.Count == 0 ? "leer" : string.Join(", ", inv)));
                else if (cmd == "quit")
                {
                    Console.WriteLine("Du verlässt die Ebene.");
                    break;
                }
                else
                    Console.WriteLine("Unbekannte Aktion.");
            }
            else if (room == "library")
            {
                Console.WriteLine("\nDie Bibliothek riecht nach altem Papier. Ein Buch liegt offen auf einem Pult.");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (cmd == "take book" || cmd == "nimm buch" || cmd == "take")
                {
                    if (!inv.Contains("mysterious book"))
                    {
                        inv.Add("mysterious book");
                        Console.WriteLine("Du nimmst das Buch. Eine Seite ist markiert: 'Der Schlüssel ist 3+4'.");
                    }
                    else
                        Console.WriteLine("Du hast das Buch bereits.");
                }
                else if (cmd == "s" || cmd == "south")
                    state.Room = "entrance";
                else if (cmd == "read")
                    Console.WriteLine("Die markierte Seite zeigt eine einfache Rechenaufgabe: 3 + 4 = ?");
                else if (cmd == "quit")
                {
                    Console.WriteLine("Du verlässt die Ebene.");
                    break;
                }
                else
                    Console.WriteLine("Nichts passiert.");
            }
            else if (room == "workshop")
            {
                Console.WriteLine("\nWerkstatt mit einem verschlossenen Kasten. Ein Zahlenfeld daneben.");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (cmd.StartsWith("enter ") || cmd.StartsWith("eingabe "))
                {
                    var parts = cmd.Split(' ', 2);
                    var code = parts.Length > 1 ? parts[1].Trim() : "";
                    if (code == "7")
                    {
                        if (!inv.Contains("rusty key"))
                        {
                            inv.Add("rusty key");
                            Console.WriteLine("Das Schloss klickt. Du findest einen rostigen Schlüssel.");
                        }
                        else
                            Console.WriteLine("Der Kasten ist leer.");
                    }
                    else
                        Console.WriteLine("Falscher Code.");
                }
                else if (cmd == "w" || cmd == "west")
                    state.Room = "entrance";
                else if (cmd == "use key")
                {
                    if (inv.Contains("rusty key"))
                    {
                        Console.WriteLine("Du öffnest eine verborgene Luke und findest eine kleine Truhe mit einer Nachricht: 'Gut gemacht.'");
                        state.Room = "treasure";
                    }
                    else
                        Console.WriteLine("Kein Schlüssel vorhanden.");
                }
                else if (cmd == "quit")
                {
                    Console.WriteLine("Du verlässt die Ebene.");
                    break;
                }
                else
                    Console.WriteLine("Unbekannte Aktion.");
            }
            else if (room == "treasure")
            {
                Console.WriteLine("\nDie Truhe enthält ein altes Medaillon. Auf der Rückseite steht: 'Das Spiel endet, wenn du es willst.'");
                Console.Write("> ");
                var cmd = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (cmd == "take medallion" || cmd == "nimm medaillon" || cmd == "take")
                {
                    if (!inv.Contains("medallion"))
                    {
                        inv.Add("medallion");
                        Console.WriteLine("Du nimmst das Medaillon. Ein warmes Gefühl durchströmt dich.");
                    }
                    else
                        Console.WriteLine("Du hast es bereits.");
                }
                else if (cmd == "quit")
                {
                    Console.WriteLine("Du verlässt die Ebene.");
                    break;
                }
                else
                    Console.WriteLine("Nichts weiter zu tun.");
            }

            state.Inventory = inv;
            SaveState(state);
        }
    }
}
