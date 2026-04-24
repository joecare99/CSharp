using System;
using TranspilerLib.Interfaces;

namespace TranspilerLib.Interfaces;
/// <summary>
/// Interface for output handlers that process reader data and produce output.
/// </summary>
public interface IOutput
{
    /// <summary>
    /// Outputs the specified reader.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="write">The write.</param>
    /// <param name="debug">The debug.</param>
    void Output(IReader reader, Action<string> write, Action<string> debug);
}