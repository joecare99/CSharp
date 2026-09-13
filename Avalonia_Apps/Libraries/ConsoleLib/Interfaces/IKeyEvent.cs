namespace ConsoleLib.Interfaces;

public interface IKeyEvent
{
    bool bKeyDown { get; }
    char KeyChar { get; }
    ushort usKeyCode { get; }
    ushort usScanCode { get; }
    uint dwControlKeyState { get; }
    bool Handled { get; set; }
}
