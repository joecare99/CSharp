using System;

namespace ConsoleLib;

/// <summary>Reports invalid or unsupported CXAML markup.</summary>
public sealed class CxamlParseException : Exception
{
    public CxamlParseException(string message) : base(message) { }
    public CxamlParseException(string message, Exception innerException) : base(message, innerException) { }
}
