using System.Collections.Generic;

namespace GenFree.Interfaces.UI;

public interface IApplUserTexts : IList<string>
{
    string this[object Idx] { get; set; }
}