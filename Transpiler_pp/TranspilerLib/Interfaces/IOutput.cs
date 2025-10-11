using System;
using TranspilerLib.Interfaces;

namespace TranspilerLib.Interfaces;

public interface IOutput
{
    void Output(IReader reader, Action<string> write, Action<string> debug);
}