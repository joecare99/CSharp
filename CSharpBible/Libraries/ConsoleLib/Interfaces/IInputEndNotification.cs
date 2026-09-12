using System;

namespace ConsoleLib.Interfaces;

public interface IInputEndNotification
{
    event EventHandler? EndOfInput;
}
