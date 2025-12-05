using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BaseLib.Show;

/// <summary>
/// Ausstellungs-Software für die BaseLib Bibliothek
/// Demonstriert alle wichtigen Funktionen und deren Anwendung
/// </summary>
internal class Program
{
    private static readonly IConsole _console = new ConsoleProxy();

    static void Main(string[] args)
    {
        _console.Title = "BaseLib Showcase - Demonstration aller Funktionen";

        ShowWelcome();

        bool running = true;
        while (running)
        {
            ShowMainMenu();
            var key = _console.ReadKey();
            _console.WriteLine("");
            _console.WriteLine("");

            running = key?.KeyChar switch
            {
                '1' => RunDemo(DemoStringUtils),
                '2' => RunDemo(DemoMathUtilities),
                '3' => RunDemo(DemoByteeUtils),
                '4' => RunDemo(DemoObjectHelper),
                '5' => RunDemo(DemoListHelper),
                '6' => RunDemo(DemoTypeUtils),
                '7' => RunDemo(DemoConsoleProxy),
                '8' => RunDemo(DemoIoC),
                '9' => RunDemo(DemoSysTimeAndRandom),
                '0' => RunAllDemos(),
                'q' or 'Q' => false,
                _ => true
            };
        }

        ShowGoodbye();
    }

    static bool RunDemo(Action demo)
    {
        demo();
        WaitForKey();
        return true;
    }

    static bool RunAllDemos()
    {
        DemoStringUtils();
        DemoMathUtilities();
        DemoByteeUtils();
        DemoObjectHelper();
        DemoListHelper();
        DemoTypeUtils();
        DemoConsoleProxy();
        DemoIoC();
        DemoSysTimeAndRandom();
        WaitForKey();
        return true;
    }

    static void ShowWelcome()
    {
        _console.ForegroundColor = ConsoleColor.Cyan;
        _console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        _console.WriteLine("║                                                                ║");
        _console.WriteLine("║              B A S E L I B   S H O W C A S E                   ║");
        _console.WriteLine("║                                                                ║");
        _console.WriteLine("║     Demonstriert alle Funktionen der BaseLib Bibliothek        ║");
        _console.WriteLine("║                                                                ║");
        _console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.WriteLine("");
    }

    static void ShowMainMenu()
    {
        _console.ForegroundColor = ConsoleColor.Yellow;
        _console.WriteLine("══════════════════════════════════════════════════════════════════");
        _console.WriteLine("                         HAUPTMENÜ");
        _console.WriteLine("══════════════════════════════════════════════════════════════════");
        _console.ForegroundColor = ConsoleColor.White;
        _console.WriteLine("");
        _console.WriteLine("  [1] StringUtils     - String-Hilfsmethoden");
        _console.WriteLine("  [2] MathUtilities   - Mathematische Funktionen (PT1, Mean, Median)");
        _console.WriteLine("  [3] ByteUtils       - Bit-Operationen");
        _console.WriteLine("  [4] ObjectHelper    - Typ-Konvertierungen");
        _console.WriteLine("  [5] ListHelper      - Listen-Operationen");
        _console.WriteLine("  [6] TypeUtils       - Typ-Utilities");
        _console.WriteLine("  [7] ConsoleProxy    - Console-Abstraktion");
        _console.WriteLine("  [8] IoC             - Dependency Injection");
        _console.WriteLine("  [9] SysTime/CRandom - Zeit und Zufall");
        _console.WriteLine("");
        _console.WriteLine("  [0] Alle Demos ausführen");
        _console.WriteLine("  [Q] Beenden");
        _console.WriteLine("");
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.Write("Ihre Wahl: ");
    }

    static void ShowGoodbye()
    {
        _console.WriteLine("");
        _console.ForegroundColor = ConsoleColor.Green;
        _console.WriteLine("Vielen Dank für die Verwendung des BaseLib Showcases!");
        _console.ForegroundColor = ConsoleColor.Gray;
    }

    static void WaitForKey()
    {
        _console.WriteLine("");
        _console.ForegroundColor = ConsoleColor.DarkGray;
        _console.Write("Drücken Sie eine beliebige Taste, um fortzufahren...");
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.ReadKey();
        _console.WriteLine("");
        _console.Clear();
        ShowWelcome();
    }

    static void PrintHeader(string title)
    {
        _console.ForegroundColor = ConsoleColor.Green;
        _console.WriteLine($"══════════════════════════════════════════════════════════════════");
        _console.WriteLine($"  {title}");
        _console.WriteLine($"══════════════════════════════════════════════════════════════════");
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.WriteLine("");
    }

    static void PrintSubHeader(string subtitle)
    {
        _console.ForegroundColor = ConsoleColor.Cyan;
        _console.WriteLine($"  ── {subtitle} ──");
        _console.ForegroundColor = ConsoleColor.Gray;
    }

    static void PrintResult(string description, object? result)
    {
        _console.ForegroundColor = ConsoleColor.White;
        _console.Write($"    {description}: ");
        _console.ForegroundColor = ConsoleColor.Yellow;
        _console.WriteLine($"{result}");
        _console.ForegroundColor = ConsoleColor.Gray;
    }

    #region StringUtils Demo
    static void DemoStringUtils()
    {
        PrintHeader("StringUtils - String-Hilfsmethoden");

        // Quote / UnQuote
        PrintSubHeader("Quote / UnQuote");
        string original = "Zeile1\nZeile2\tTabulator";
        string quoted = original.Quote();
        string unquoted = quoted.UnQuote();
        PrintResult("Original", $"\"{original.Replace("\n", "↵").Replace("\t", "→")}\"");
        PrintResult("Quoted", $"\"{quoted}\"");
        PrintResult("UnQuoted", $"\"{unquoted.Replace("\n", "↵").Replace("\t", "→")}\"");
        _console.WriteLine("");

        // Format
        PrintSubHeader("Format Extension");
        string template = "Hallo {0}, heute ist {1}!";
        PrintResult("Template", template);
        PrintResult("Formatiert", template.Format("Welt", DateTime.Now.ToString("dddd")));
        _console.WriteLine("");

        // SFirst / SRest
        PrintSubHeader("SFirst / SRest (String-Splitting)");
        string sentence = "Dies ist ein Beispielsatz";
        PrintResult("Original", sentence);
        PrintResult("SFirst (erstes Wort)", sentence.SFirst());
        PrintResult("SRest (Rest)", sentence.SRest());
        _console.WriteLine("");

        // ToNormal
        PrintSubHeader("ToNormal (Namensformatierung)");
        string[] names = ["pETER", "MÜLLER", "schmidt", "McGregor"];
        foreach (var name in names)
        {
            PrintResult($"'{name}'", name.ToNormal());
        }
        _console.WriteLine("");

        // Left / Right
        PrintSubHeader("Left / Right (Substring-Funktionen)");
        string text = "BaseLib Showcase";
        PrintResult("Original", text);
        PrintResult("Left(7)", text.Left(7));
        PrintResult("Right(8)", text.Right(8));
        PrintResult("Left(-8)", text.Left(-8));
        PrintResult("Right(-7)", text.Right(-7));
        _console.WriteLine("");

        // QuotedSplit
        PrintSubHeader("QuotedSplit (CSV-Parsing)");
        string csv = "Name,\"Beschreibung, mit Komma\",Wert";
        PrintResult("CSV-Zeile", csv);
        var parts = csv.QuotedSplit();
        for (int i = 0; i < parts.Count; i++)
        {
            PrintResult($"  Teil {i + 1}", parts[i]);
        }
        _console.WriteLine("");

        // IsValidIdentifyer
        PrintSubHeader("IsValidIdentifyer");
        string[] identifiers = ["MyVariable", "123Invalid", "_underscore", "Valid_Name"];
        foreach (var id in identifiers)
        {
            PrintResult($"'{id}'", id.IsValidIdentifyer() ? "✓ gültig" : "✗ ungültig");
        }
        _console.WriteLine("");

        // PadTab
        PrintSubHeader("PadTab (Tab-Ausrichtung)");
        string tabText = "A\tB\tC";
        PrintResult("Original", tabText.Replace("\t", "→"));
        PrintResult("PadTab", tabText.PadTab());
        _console.WriteLine("");

        // ContainsAny / StartswithAny / EndswithAny
        PrintSubHeader("ContainsAny / StartswithAny / EndswithAny");
        string testStr = "HelloWorld.txt";
        PrintResult("String", testStr);
        PrintResult("ContainsAny('llo','xyz')", testStr.ContainsAny("llo", "xyz"));
        PrintResult("StartswithAny('Hi','Hello')", testStr.StartswithAny("Hi", "Hello"));
        PrintResult("EndswithAny('.cs','.txt')", testStr.EndswithAny(".cs", ".txt"));
    }
    #endregion

    #region MathUtilities Demo
    static void DemoMathUtilities()
    {
        PrintHeader("MathUtilities - Mathematische Filter und Funktionen");

        // PT1 Filter
        PrintSubHeader("PT1 Filter (Tiefpass-Verhalten)");
        double actValue = 0;
        _console.WriteLine("    Simuliere Sprung von 0 auf 100:");
        for (int i = 0; i < 10; i++)
        {
            double newValue = 100.0;
            newValue.PT1(ref actValue, 0.3);
            PrintResult($"    Schritt {i + 1}", $"{actValue:F2}");
        }
        _console.WriteLine("");

        // Mean (Gleitender Mittelwert)
        PrintSubHeader("Mean (Gleitender Mittelwert)");
        double[] meanBuffer = new double[5];
        int meanIdx = 0;
        double[] values = [10, 20, 30, 40, 50, 60, 70];
        _console.WriteLine("    Puffergröße: 5");
        foreach (var v in values)
        {
            double mean = v.Mean(meanBuffer, ref meanIdx);
            PrintResult($"    Wert {v:F0}", $"Mittelwert = {mean:F2}");
        }
        _console.WriteLine("");

        // Median
        PrintSubHeader("Median Filter");
        double[] medianBuffer = new double[5];
        int medianIdx = 0;
        double[] noisyValues = [10, 100, 20, 15, 25, 200, 30];
        _console.WriteLine("    Puffergröße: 5 (entfernt Ausreißer)");
        foreach (var v in noisyValues)
        {
            double median = v.Median(medianBuffer, ref medianIdx);
            PrintResult($"    Wert {v:F0}", $"Median = {median:F2}");
        }
    }
    #endregion

    #region ByteUtils Demo
    static void DemoByteeUtils()
    {
        PrintHeader("ByteUtils - Bit-Operationen");

        // BitMask
        PrintSubHeader("BitMask32 / BitMask64");
        for (int i = 0; i < 8; i++)
        {
            PrintResult($"Bit {i}", $"Maske = {i.BitMask32():X8} (binär: {Convert.ToString(i.BitMask32(), 2).PadLeft(8, '0')})");
        }
        _console.WriteLine("");

        // SetBit / ClearBit / GetBit
        PrintSubHeader("SetBit / ClearBit / GetBit / SwitchBit");
        int bitArray = 0;
        PrintResult("Start", $"{bitArray:X8}");
        bitArray = bitArray.SetBit(0);
        PrintResult("SetBit(0)", $"{bitArray:X8}");
        bitArray = bitArray.SetBit(3);
        PrintResult("SetBit(3)", $"{bitArray:X8}");
        bitArray = bitArray.SetBit(7);
        PrintResult("SetBit(7)", $"{bitArray:X8}");
        PrintResult("GetBit(3)", bitArray.GetBit(3));
        PrintResult("GetBit(4)", bitArray.GetBit(4));
        bitArray = bitArray.ClearBit(3);
        PrintResult("ClearBit(3)", $"{bitArray:X8}");
        bitArray = bitArray.SwitchBit(0);
        PrintResult("SwitchBit(0)", $"{bitArray:X8}");
        bitArray = bitArray.SwitchBit(0);
        PrintResult("SwitchBit(0)", $"{bitArray:X8}");
        _console.WriteLine("");

        // BitCount
        PrintSubHeader("BitCount (Anzahl gesetzter Bits)");
        long[] testValues = [0b1010101, 0xFF, 0xFFFF, 0x12345678];
        foreach (var val in testValues)
        {
            PrintResult($"0x{val:X}", $"{val.BitCount()} Bits gesetzt");
        }
    }
    #endregion

    #region ObjectHelper Demo
    static void DemoObjectHelper()
    {
        PrintHeader("ObjectHelper - Typ-Konvertierungen");

        // AsInt
        PrintSubHeader("AsInt (zu Integer konvertieren)");
        object[] intTests = ["42", 3.14, "abc", null!, 100L];
        foreach (var test in intTests)
        {
            PrintResult($"'{test ?? "null"}' ({test?.GetType().Name ?? "null"})", test.AsInt(-1));
        }
        _console.WriteLine("");

        // AsDouble
        PrintSubHeader("AsDouble (zu Double konvertieren)");
        object[] doubleTests = ["3.14", 42, "1.5e10", "ungültig"];
        foreach (var test in doubleTests)
        {
            PrintResult($"'{test}'", test.AsDouble());
        }
        _console.WriteLine("");

        // AsBool
        PrintSubHeader("AsBool (zu Boolean konvertieren)");
        object[] boolTests = ["true", "false", 1, 0, "1", 'T'];
        foreach (var test in boolTests)
        {
            PrintResult($"'{test}'", test.AsBool());
        }
        _console.WriteLine("");

        // AsDate
        PrintSubHeader("AsDate (zu DateTime konvertieren)");
        object[] dateTests = ["2024-01-15", 20240115, "15.01.2024"];
        foreach (var test in dateTests)
        {
            var result = test.AsDate();
            PrintResult($"'{test}'", result == default ? "ungültig" : result.ToString("dd.MM.yyyy"));
        }
        _console.WriteLine("");

        // AsEnum
        PrintSubHeader("AsEnum<T> (zu Enum konvertieren)");
        PrintResult("'Red' als ConsoleColor", "Red".AsEnum<ConsoleColor>());
        PrintResult("1 als ConsoleColor", 1.AsEnum<ConsoleColor>());
        PrintResult("'DarkBlue' als ConsoleColor", "DarkBlue".AsEnum<ConsoleColor>());
        _console.WriteLine("");

        // AsString
        PrintSubHeader("AsString (zu String konvertieren)");
        object[] stringTests = [123, 3.14, DateTime.Now, null!];
        foreach (var test in stringTests)
        {
            PrintResult($"{test?.GetType().Name ?? "null"}", (test?.AsString() ?? "(null)"));
        }
    }
    #endregion

    #region ListHelper Demo
    static void DemoListHelper()
    {
        PrintHeader("ListHelper - Listen-Operationen");

        // Swap
        PrintSubHeader("Swap (Elemente tauschen)");
        List<string> list1 = ["A", "B", "C", "D", "E"];
        PrintResult("Original", string.Join(", ", list1));
        list1.Swap(1, 3);
        PrintResult("Swap(1, 3)", string.Join(", ", list1));
        _console.WriteLine("");

        // MoveItem
        PrintSubHeader("MoveItem (Element verschieben)");
        List<string> list2 = ["A", "B", "C", "D", "E"];
        PrintResult("Original", string.Join(", ", list2));
        list2.MoveItem(0, 3);
        PrintResult("MoveItem(0, 3)", string.Join(", ", list2));
        list2.MoveItem(4, 1);
        PrintResult("MoveItem(4, 1)", string.Join(", ", list2));
        _console.WriteLine("");

        // To (Range erstellen)
        PrintSubHeader("To (Bereich erstellen)");
        var range1 = 1.To(10);
        PrintResult("1.To(10)", string.Join(", ", range1));
        var range2 = 5.To(8);
        PrintResult("5.To(8)", string.Join(", ", range2));
    }
    #endregion

    #region TypeUtils Demo
    static void DemoTypeUtils()
    {
        PrintHeader("TypeUtils - Typ-Utilities");

        // TC (TypeCode)
        PrintSubHeader("TC (TypeCode ermitteln)");
        Type[] types = [typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime)];
        foreach (var type in types)
        {
            PrintResult(type.Name, type.TC());
        }
        _console.WriteLine("");

        // ToType (String zu Type)
        PrintSubHeader("ToType (String zu Type konvertieren)");
        string[] typeNames = ["Int32", "String", "Double", "Boolean"];
        foreach (var name in typeNames)
        {
            var type = name.ToType();
            PrintResult(name, type.FullName);
        }
        _console.WriteLine("");

        // IsBetweenIncl / IsBetweenExcl
        PrintSubHeader("IsBetweenIncl / IsBetweenExcl");
        int testValue = 5;
        PrintResult($"{testValue}.IsBetweenIncl(1, 5)", testValue.IsBetweenIncl(1, 5));
        PrintResult($"{testValue}.IsBetweenExcl(1, 5)", testValue.IsBetweenExcl(1, 5));
        PrintResult($"{testValue}.IsBetweenIncl(5, 10)", testValue.IsBetweenIncl(5, 10));
        PrintResult($"{testValue}.IsBetweenExcl(5, 10)", testValue.IsBetweenExcl(5, 10));
        _console.WriteLine("");

        // CheckLimit
        PrintSubHeader("CheckLimit");
        PrintResult("5.CheckLimit(1, 10)", 5.CheckLimit(1, 10));
        PrintResult("15.CheckLimit(1, 10)", 15.CheckLimit(1, 10));
        PrintResult("0.CheckLimit(1, 10)", 0.CheckLimit(1, 10));
        _console.WriteLine("");

        // Get (Type-basierte Konvertierung)
        PrintSubHeader("Get (Type-basierte Konvertierung)");
        PrintResult("typeof(int).Get(\"42\")", typeof(int).Get("42"));
        PrintResult("typeof(double).Get(\"3.14\")", typeof(double).Get("3.14"));
        PrintResult("typeof(bool).Get(\"true\")", typeof(bool).Get("true"));
    }
    #endregion

    #region ConsoleProxy Demo
    static void DemoConsoleProxy()
    {
        PrintHeader("ConsoleProxy - Console-Abstraktion");

        PrintSubHeader("Aktuelle Console-Eigenschaften");
        PrintResult("WindowWidth", _console.WindowWidth);
        PrintResult("WindowHeight", _console.WindowHeight);
        PrintResult("Title", _console.Title);
        PrintResult("IsOutputRedirected", _console.IsOutputRedirected);
        PrintResult("LargestWindowHeight", _console.LargestWindowHeight);
        _console.WriteLine("");

        PrintSubHeader("Cursor-Position");
        var pos = _console.GetCursorPosition();
        PrintResult("Aktuelle Position", $"Left={pos.Left}, Top={pos.Top}");
        _console.WriteLine("");

        PrintSubHeader("Farben Demo");
        ConsoleColor[] colors = [ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, 
                                  ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.Cyan];
        _console.Write("    Farbpalette: ");
        foreach (var color in colors)
        {
            _console.ForegroundColor = color;
            _console.Write("■ ");
        }
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.WriteLine("");
        _console.WriteLine("");

        PrintSubHeader("Verwendungszweck");
        _console.WriteLine("    Die ConsoleProxy-Klasse ermöglicht:");
        _console.WriteLine("    • Abstraktion der Console für Unit-Tests");
        _console.WriteLine("    • Dependency Injection der Console");
        _console.WriteLine("    • Mockbares Console-Interface (IConsole)");
    }
    #endregion

    #region IoC Demo
    static void DemoIoC()
    {
        PrintHeader("IoC - Dependency Injection Container");

        PrintSubHeader("ServiceCollection konfigurieren");
        _console.WriteLine("    Code-Beispiel:");
        _console.ForegroundColor = ConsoleColor.DarkGray;
        _console.WriteLine("    var builder = new ServiceCollection();");
        _console.WriteLine("    builder.AddSingleton<IConsole, ConsoleProxy>();");
        _console.WriteLine("    builder.AddTransient<IRandom, CRandom>();");
        _console.WriteLine("    IoC.Configure(builder.BuildServiceProvider());");
        _console.ForegroundColor = ConsoleColor.Gray;
        _console.WriteLine("");

        // Tatsächliche Konfiguration
        var builder = new ServiceCollection();
        builder.AddSingleton<IConsole, ConsoleProxy>();
        builder.AddTransient<Models.Interfaces.IRandom, CRandom>();
        builder.AddSingleton<Models.Interfaces.ISysTime, SysTime>();
        IoC.Configure(builder.BuildServiceProvider());

        PrintSubHeader("Services abrufen");
        var console = IoC.GetRequiredService<IConsole>();
        PrintResult("GetRequiredService<IConsole>()", console.GetType().Name);

        var random = IoC.GetRequiredService<Models.Interfaces.IRandom>();
        PrintResult("GetRequiredService<IRandom>()", random.GetType().Name);

        var sysTime = IoC.GetService<Models.Interfaces.ISysTime>();
        PrintResult("GetService<ISysTime>()", sysTime?.GetType().Name ?? "null");

        var notRegistered = IoC.GetService<System.Text.StringBuilder>();
        PrintResult("GetService<StringBuilder>() (nicht registriert)", notRegistered?.GetType().Name ?? "null");
        _console.WriteLine("");

        PrintSubHeader("Scopes");
        _console.WriteLine("    IoC unterstützt auch Scopes für zeitlich begrenzte Dienste:");
        _console.ForegroundColor = ConsoleColor.DarkGray;
        _console.WriteLine("    var scope = IoC.GetNewScope();");
        _console.WriteLine("    // Arbeiten mit scope-spezifischen Services");
        _console.WriteLine("    IoC.SetCurrentScope(scope);");
        _console.ForegroundColor = ConsoleColor.Gray;
    }
    #endregion

    #region SysTime and CRandom Demo
    static void DemoSysTimeAndRandom()
    {
        PrintHeader("SysTime & CRandom - Zeit und Zufallszahlen");

        // SysTime
        PrintSubHeader("SysTime (Systemzeit-Abstraktion)");
        var sysTime = new SysTime();
        PrintResult("Now", sysTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        PrintResult("Today", sysTime.Today.ToString("dd.MM.yyyy"));
        _console.WriteLine("");

        _console.WriteLine("    Testbare Zeit (Mock):");
        _console.ForegroundColor = ConsoleColor.DarkGray;
        _console.WriteLine("    SysTime.GetNow = () => new DateTime(2020, 1, 1);");
        _console.ForegroundColor = ConsoleColor.Gray;
        var originalGetNow = SysTime.GetNow;
        SysTime.GetNow = () => new DateTime(2020, 1, 1, 12, 0, 0);
        PrintResult("Now (gemockt)", sysTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        SysTime.GetNow = originalGetNow; // Zurücksetzen
        PrintResult("Now (wiederhergestellt)", sysTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
        _console.WriteLine("");

        // CRandom
        PrintSubHeader("CRandom (Zufallszahlen-Generator)");
        var random = new CRandom();
        
        _console.WriteLine("    Zufällige Integer:");
        _console.Write("    ");
        for (int i = 0; i < 10; i++)
        {
            _console.Write($"{random.Next(1, 100),4}");
        }
        _console.WriteLine("");
        _console.WriteLine("");

        _console.WriteLine("    Zufällige Doubles (0.0 - 1.0):");
        _console.Write("    ");
        for (int i = 0; i < 5; i++)
        {
            _console.Write($"{random.NextDouble():F3}  ");
        }
        _console.WriteLine("");
        _console.WriteLine("");

        _console.WriteLine("    Reproduzierbare Sequenz (Seed = 42):");
        random.Seed(42);
        _console.Write("    Sequenz 1: ");
        for (int i = 0; i < 5; i++)
            _console.Write($"{random.Next(1, 100),4}");
        _console.WriteLine("");
        
        random.Seed(42);
        _console.Write("    Sequenz 2: ");
        for (int i = 0; i < 5; i++)
            _console.Write($"{random.Next(1, 100),4}");
        _console.WriteLine("");
        _console.WriteLine("    → Beide Sequenzen sind identisch!");
    }
    #endregion
}
