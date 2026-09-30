using BaseLib.Interfaces;
using System.IO;

namespace BaseLib.Models;

public class TextWriterProxy(TextWriter @out) : ITextWriter
{
    public void Write(string text) => @out.Write(text);
    public void WriteLine(string text) => @out.WriteLine(text);
}