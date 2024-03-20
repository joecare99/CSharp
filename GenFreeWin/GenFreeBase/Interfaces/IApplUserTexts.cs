using System.Collections.Generic;

namespace GenFree.Interfaces;

public interface IApplUserTexts : IList<string>
{
    string this[object Idx] { get; set; }
}