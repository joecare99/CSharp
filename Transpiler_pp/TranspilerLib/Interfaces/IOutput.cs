using System;
using TranspilerLib.Interfaces;

namespace TranspilerConsTest.Model
{
    public interface IOutput
    {
        void Output(IReader reader, Action<string> write, Action<string> debug);
    }
}