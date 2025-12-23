// Einfaches Demonstrationsprogramm
// Version 1.0 - Hello World Example
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Willkommen zum Demonstrationsprogramm!");
Console.WriteLine("======================================");
Console.WriteLine();
Console.WriteLine("Dieses Programm zeigt grundlegende C#-Konzepte:");
Console.WriteLine("  - Variablen und Datentypen");
Console.WriteLine("  - Schleifen und Bedingungen");
Console.WriteLine("  - Benutzereingaben");
Console.WriteLine();
Console.WriteLine("Druecken Sie eine beliebige Taste zum Fortfahren...");
Console.ReadKey(true);

// Ab hier wird es interessant...
Console.Clear();
Thread.Sleep(300);

string[] glitch = [
    "SYSTEM OVERRIDE DETECTED",
    "INITIALISIERE VERSTECKTES MODUL...",
    "*** LOADING ***"
];

foreach (var line in glitch)
{
    Console.ForegroundColor = ConsoleColor.Green;
    for (int i = 0; i < line.Length; i++)
    {
        Console.Write(line[i]);
        Thread.Sleep(20);
    }
    Console.WriteLine();
    Thread.Sleep(200);
}

Thread.Sleep(500);
Console.Clear();
Console.ResetColor();

// Das versteckte Spiel
int width = 40, height = 20;
int playerX = width / 2, playerY = height / 2;
int score = 0;
var treasures = new List<(int x, int y)>();
var rand = new Random();

for (int i = 0; i < 5; i++)
    treasures.Add((rand.Next(1, width - 1), rand.Next(1, height - 1)));

Console.CursorVisible = false;

void DrawGame()
{
    Console.SetCursorPosition(0, 0);
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"+{"".PadLeft(width, '-')}+  [GAME] UEBERRASCHUNG!");
    Console.ResetColor();
    
    for (int y = 0; y < height; y++)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("|");
        Console.ResetColor();
        
        for (int x = 0; x < width; x++)
        {
            if (x == playerX && y == playerY)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("@");
            }
            else if (treasures.Any(t => t.x == x && t.y == y))
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("*");
            }
            else
            {
                Console.Write(" ");
            }
            Console.ResetColor();
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("|");
        Console.ResetColor();
    }
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"+{"".PadLeft(width, '-')}+");
    Console.ResetColor();
    
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($"  Punkte: {score}   |   Sammle die * ein!");
    Console.WriteLine("  Steuerung: Pfeiltasten   |   ESC = Beenden");
    Console.ResetColor();
}

DrawGame();

while (true)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        
        int newX = playerX, newY = playerY;
        
        switch (key)
        {
            case ConsoleKey.UpArrow: newY = Math.Max(0, playerY - 1); break;
            case ConsoleKey.DownArrow: newY = Math.Min(height - 1, playerY + 1); break;
            case ConsoleKey.LeftArrow: newX = Math.Max(0, playerX - 1); break;
            case ConsoleKey.RightArrow: newX = Math.Min(width - 1, playerX + 1); break;
            case ConsoleKey.Escape:
                Console.Clear();
                Console.CursorVisible = true;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  Danke fuers Spielen! *");
                Console.WriteLine($"  Endpunktzahl: {score}");
                Console.WriteLine("\n  Das war das 'einfache Demonstrationsprogramm' ;)");
                Console.ResetColor();
                return;
        }
        
        playerX = newX;
        playerY = newY;
        
        var collected = treasures.FirstOrDefault(t => t.x == playerX && t.y == playerY);
        if (collected != default)
        {
            treasures.Remove(collected);
            score += 10;
            Console.Beep(800, 50);
            
            if (treasures.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                    treasures.Add((rand.Next(1, width - 1), rand.Next(1, height - 1)));
                score += 50;
                Console.Beep(1000, 100);
                Console.Beep(1200, 100);
            }
        }
        
        DrawGame();
    }
    
    Thread.Sleep(16);
}
