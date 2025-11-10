using System.Collections.Generic;

namespace AlternatingAppearanceOfItems.Models.Interfaces;

public interface ILeague
{
    string Name { get; }
    List<IDivision> Divisions { get; }
}
