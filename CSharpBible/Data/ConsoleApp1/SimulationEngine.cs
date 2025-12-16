using System;
using System.Timers;

namespace ConsoleApp1;

// 7. Beispielklasse für die Simulation
public class SimulationEngine
{
    private StateMachine _stateMachine = new();
    private System.Timers.Timer _monitorTimer;

    public SimulationEngine()
    {
        // Beispielaktionen registrieren
        _stateMachine.AddTransaction(new Transaction
        {
            Name = "InitialSetup",
            Steps = new() { "InitialStep" },
            StartStep = new() { Name = "InitialStep" },
            Dependencies = new()
        });

        // Schrittdefinitionen hinzufügen
        var initialStep = new StepDefinition
        {
            Name = "InitialStep",
            Actions = new()
            {
                new ExampleActions.ActionExample(),
                new ExampleActions.SaveStateAction()
            }
        };

        _stateMachine.AddStepDefinition(initialStep);
        _stateMachine.AddTransaction(new Transaction
        {
            Name = "InitialSetup",
            Steps = new() { "InitialStep" },
            StartStep = new() { Name = "InitialStep" },
        });

        // Timer für Überwachung und Interaktion
        _monitorTimer = new Timer(5000); // Jede 5 Sekunden
        _monitorTimer.Elapsed += MonitorSimulation;
    }

    public void Start()
    {
        _monitorTimer.Start();
        Console.WriteLine("Simulation gestartet. Verwende 'help' für Hilfe.");
        MonitorSimulation(null, null);
    }

    private void MonitorSimulation(object sender, ElapsedEventArgs e)
    {
        Console.WriteLine("\n=== Aktueller Zustand ===");
        Console.WriteLine($"Zustand: {_stateMachine.CurrentState}");

        Console.WriteLine("\nVerfügbare Transaktionen:");
        foreach (var tx in _stateMachine.Transactions)
        {
            Console.WriteLine($" - {tx.Key}");
        }

        Console.Write("\n> ");
        var input = Console.ReadLine()?.ToLower();

        if (input == "exit")
        {
            _monitorTimer.Stop();
            Environment.Exit(0);
        }
        else if (input == "help")
        {
            Console.WriteLine("\nVerfügbare Befehle:");
            Console.WriteLine(" - help: Zeige diese Hilfe");
            Console.WriteLine(" - exit: Beende die Simulation");
            Console.WriteLine(" - [Transaktionsname]: Führe eine Transaktion aus");
            Console.WriteLine(" - save [dateiname]: Speichere den aktuellen Zustand");
            Console.WriteLine(" - load [dateiname]: Lade einen Zustand");
        }
        else if (input?.StartsWith("save") == true)
        {
            var filename = input.Substring(5).Trim();
            if (!string.IsNullOrEmpty(filename))
            {
                _stateMachine.SaveState(filename);
                Console.WriteLine($"Zustand wurde gespeichert in {filename}");
            }
            else
            {
                Console.WriteLine("Bitte einen Dateinamen angeben");
            }
        }
        else if (input?.StartsWith("load") == true)
        {
            var filename = input.Substring(5).Trim();
            if (!string.IsNullOrEmpty(filename))
            {
                _stateMachine.LoadState(filename);
                Console.WriteLine($"Zustand wurde geladen aus {filename}");
            }
            else
            {
                Console.WriteLine("Bitte einen Dateinamen angeben");
            }
        }
        else if (!string.IsNullOrEmpty(input))
        {
            // Führe eine Transaktion aus
            _stateMachine.ExecuteTransaction(input);
        }
    }
}
