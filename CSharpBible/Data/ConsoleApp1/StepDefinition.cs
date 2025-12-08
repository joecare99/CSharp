using System.Collections.Generic;

namespace ConsoleApp1;

// 3. Schrittdefinition mit Aktionen und Prädikaten
public class StepDefinition
{
    public string Name { get; set; }
    public List<IAction> Actions { get; init; } = new();
    public List<IStatePredicate> Conditions { get; } = new();
}
