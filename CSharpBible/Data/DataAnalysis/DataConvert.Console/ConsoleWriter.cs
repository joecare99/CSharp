namespace DataConvert.Console;

public class ConsoleWriter : IConsoleWriter
{
    public void Write(string v)
    {
       System.Console.Write(v);
    }

    public void WriteLine(string v)
    {
        System.Console.WriteLine(v);
    }
}