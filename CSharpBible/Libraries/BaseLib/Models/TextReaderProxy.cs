using BaseLib.Interfaces;
using System.IO;

namespace BaseLib.Models;

internal class TextReaderProxy(TextReader @in) : ITextReader
{
    public string ReadLine() => @in.ReadLine() ?? string.Empty;
    public string ReadToEnd() => @in.ReadToEnd();
}