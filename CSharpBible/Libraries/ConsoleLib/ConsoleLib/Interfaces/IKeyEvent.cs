using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
