using System.Collections.Generic;

namespace AlternatingAppearanceOfItems.Models.Interfaces;

public interface IDivision
{
    string Name { get; }
    List<ITeam> Teams { get; }
}
