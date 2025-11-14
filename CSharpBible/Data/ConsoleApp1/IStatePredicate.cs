namespace ConsoleApp1;

public interface IStatePredicate
{
    bool Check(StateContext context);
    string Message { get; }
}
