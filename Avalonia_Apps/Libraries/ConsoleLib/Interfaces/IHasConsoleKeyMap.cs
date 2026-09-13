namespace ConsoleLib.Interfaces;

public interface IHasConsoleKeyMap
{
    ushort KeyEnter { get; }
    ushort KeyEsc { get; }
    ushort KeyTab { get; }
    ushort KeyLeft { get; }
    ushort KeyUp { get; }
    ushort KeyRight { get; }
    ushort KeyDown { get; }
    ushort KeyHome { get; }
    ushort KeyEnd { get; }
    ushort KeyDelete { get; }
    ushort KeyPageUp { get; }
    ushort KeyPageDown { get; }
}
