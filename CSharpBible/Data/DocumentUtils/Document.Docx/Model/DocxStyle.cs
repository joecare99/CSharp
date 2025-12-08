using System.Collections.Generic;
using Document.Base.Models.Interfaces;

namespace Document.Docx.Model;

public sealed class DocxStyle : IDocStyleStyle
{
    public DocxStyle(string? name)
    {
        Name = name;
        Properties = new Dictionary<string, string>();
    }

    public string? Name { get; }
    public IDictionary<string, string> Properties { get; }
}
