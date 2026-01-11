using SharpHack.Base.Interfaces;

namespace SharpHack.Base.Model;

public class Armor : Item, IArmor
{
    public int DefenseBonus { get; set; }
}
