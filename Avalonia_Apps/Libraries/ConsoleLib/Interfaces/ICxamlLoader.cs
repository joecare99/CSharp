using System.IO;

namespace ConsoleLib.Interfaces;

/// <summary>Loads a ConsoleLib control tree from CXAML markup.</summary>
public interface ICxamlLoader : ICxamlValidator
{
    IControl Load(TextReader markup);
    CxamlLoadResult Load(TextReader markup, CxamlLoadContext context);
}
