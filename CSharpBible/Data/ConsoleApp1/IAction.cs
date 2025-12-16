namespace ConsoleApp1;

// 1. Interfaces für Aktionen und Prädikate
public interface IAction
{
    string Name { get; }
    string Execute(StateContext context);
}
