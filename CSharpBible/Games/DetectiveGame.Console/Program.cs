using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Cards;

var setup = new GameService();
var state = setup.CreateNew(new []{"Alice","Bob","Carol"});
Console.WriteLine("Detektivspiel gestartet.");
while (!state.Finished)
{
    var current = state.Players[state.CurrentPlayerIndex];
    if (!current.Active) { new GameService().NextTurn(state); continue; }
    Console.WriteLine($"Spieler {current.Name} ist am Zug.");
    Console.Write("Verdacht ‰uﬂern? (j/n): ");
    var key = Console.ReadLine();
    if (key?.StartsWith("j", StringComparison.OrdinalIgnoreCase) == true)
    {
        var person = Prompt("Person", GameData.Persons);
        var weapon = Prompt("Waffe", GameData.Weapons);
        var room = Prompt("Raum", GameData.Rooms);
        var svc = new GameService();
        var sug = svc.MakeSuggestion(state, current.Id, person, weapon, room);
        if (sug.RefutingPlayerId is int pid)
            Console.WriteLine($"Widerlegt von Spieler {pid} mit Karte {sug.RevealedCard}");
        else
            Console.WriteLine("Niemand konnte widerlegen.");
    }
    Console.Write("Anklage? (j/n): ");
    var acc = Console.ReadLine();
    if (acc?.StartsWith("j", StringComparison.OrdinalIgnoreCase) == true)
    {
        var person = Prompt("Person", GameData.Persons);
        var weapon = Prompt("Waffe", GameData.Weapons);
        var room = Prompt("Raum", GameData.Rooms);
        var svc = new GameService();
        var ok = svc.MakeAccusation(state, current.Id, person, weapon, room);
        Console.WriteLine(ok ? "Richtig! Spiel Ende." : "Falsch! Spieler inaktiv.");
    }
    new GameService().NextTurn(state);
}
Console.WriteLine($"Sieger: {state.WinnerPlayerId}");

static Card Prompt(string label, IReadOnlyList<Card> options)
{
    Console.WriteLine(label + ":");
    for (int i=0;i<options.Count;i++) Console.WriteLine($"  {i}: {options[i].Name}");
    int idx;
    while(!int.TryParse(Console.ReadLine(), out idx) || idx<0 || idx>=options.Count) ;
    return options[idx];
}