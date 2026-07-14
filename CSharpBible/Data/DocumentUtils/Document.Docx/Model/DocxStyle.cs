using Document.Base.Models.Interfaces;
using System.Collections.Generic;

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
