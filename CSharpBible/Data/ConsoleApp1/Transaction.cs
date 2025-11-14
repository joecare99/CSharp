using System.Collections.Generic;

namespace ConsoleApp1;

// 4. Transaktionsklasse für Ablauflogik
public class Transaction
{
    public string Name { get; set; }
    public StepDefinition StartStep { get; set; }
    public List<string> Steps { get; init; } = new();
    public List<Transaction> Dependencies { get; init; } = new();
}
