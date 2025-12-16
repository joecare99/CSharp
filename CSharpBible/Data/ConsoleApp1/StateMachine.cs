using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ConsoleApp1;

// 5. Dynamische Zustandsmaschine
public class StateMachine
{
    private Dictionary<string, StepDefinition> _stepDefinitions = new();
    private Dictionary<string, Transaction> _transactions = new();
    private string _currentState = "Initial";
    public string CurrentState => _currentState;
    public Dictionary<string, Transaction> Transactions => _transactions;
    private StateContext _context = new();

    public void AddStepDefinition(StepDefinition step)
    {
        _stepDefinitions[step.Name] = step;
    }

    public void AddTransaction(Transaction tx)
    {
        _transactions[tx.Name] = tx;
    }

    public void ExecuteTransaction(string transactionName)
    {
        if (!_transactions.ContainsKey(transactionName))
        {
            throw new ArgumentException($"Transaction '{transactionName}' not found");
        }

        var tx = _transactions[transactionName];
        ExecuteTransaction(tx);
    }

    private void ExecuteTransaction(Transaction tx)
    {
        // Prüfe vorab auf Abhängigkeiten
        foreach (var dependency in tx.Dependencies)
        {
            if (!_transactions.ContainsKey(dependency.Name) ||
                !_transactions[dependency.Name].Steps.Contains(tx.Name))
            {
                throw new InvalidOperationException($"Dependency '{dependency}' not satisfied for transaction '{tx.Name}'");
            }
        }

        // Führe Schritte der Transaktion aus
        foreach (var stepName in tx.Steps)
        {
            var step = _stepDefinitions[stepName];
            foreach (var action in step.Actions)
            {
                var result = action.Execute(_context);
                HandleActionResult(result);
            }
        }

        // Nach Ausführung des Transaktionsablaufes den Zustand aktualisieren
        _currentState = tx.Name;
    }

    private void HandleActionResult(string result)
    {
        // Beispiel: Ergebnisse verarbeiten, die Zustände beeinflussen
        switch (result)
        {
            case "ERROR":
                _context.AddData("LastError", "Ablaufabbruch");
                break;
            case "TRANSITION":
                // Hier könnte eine Zustandsänderung initiiert werden
                break;
            case "SAVE_NEEDED":
                // Speichervorschlag
                break;
        }
    }

    public void SaveState(string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            var serializer = new XmlSerializer(typeof(StateContext));
            serializer.Serialize(writer, _context);
        }
    }

    public void LoadState(string filename)
    {
        if (!File.Exists(filename)) return;

        using (var reader = new StreamReader(filename))
        {
            var serializer = new XmlSerializer(typeof(StateContext));
            _context = (StateContext)serializer.Deserialize(reader);
        }
    }
}
