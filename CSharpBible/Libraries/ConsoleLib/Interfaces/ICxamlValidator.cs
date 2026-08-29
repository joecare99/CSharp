using System.Collections.Generic;
using System.IO;

namespace ConsoleLib.Interfaces;

/// <summary>Reports unsupported CXAML elements and attributes without constructing controls.</summary>
public interface ICxamlValidator
{
    IReadOnlyList<CxamlDiagnostic> Validate(TextReader markup);
}
