using System.Collections.Generic;

namespace Document.Base.Models.Interfaces
{
    public interface IDocStyleStyle
    {
        string? Name { get; }
        IDictionary<string, string> Properties { get; }
    }
}