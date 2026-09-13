using System.Drawing;

namespace ConsoleLib.Interfaces;

public interface IMouseEvent
{
    Point MousePos { get; }
    bool MouseButtonLeft { get; }
    bool MouseButtonRight { get; }
    bool MouseButtonMiddle { get; }
    int MouseWheel { get; }
    bool MouseMoved { get; }
    bool ButtonEvent { get; }
    bool Handled { get; set; }
}
