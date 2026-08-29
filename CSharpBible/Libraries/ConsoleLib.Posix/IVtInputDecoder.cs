using System.Collections.Generic;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Posix;

/// <summary>Decodes VT terminal input into immutable, provider-neutral key values.</summary>
public interface IVtInputDecoder
{
    IReadOnlyList<KeyInput> Decode(string input);
    IReadOnlyList<KeyInput> Flush();
}
