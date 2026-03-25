using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;

namespace SharpHack.BaseItems.Model;

public class Armor : Item, IArmor
{
    public int DefenseBonus { get; set; }
}
