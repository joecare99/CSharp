using SharpHack.Base.Interfaces;

namespace SharpHack.Base.Model;

public class Weapon : Item, IWeapon
{
    public int AttackBonus { get; set; }
}
