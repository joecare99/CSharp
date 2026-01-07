using SharpHack.Base.Interfaces;
using System.Collections.Generic;

namespace SharpHack.Base.Model;

public class Creature : GameObject, ICreature
{
    public int HP { get; set; }
    public int MaxHP { get; set; }

    public int BaseAttack { get; set; }
    public int BaseDefense { get; set; }

    public int Attack => BaseAttack + (MainHand?.AttackBonus ?? 0);
    public int Defense => BaseDefense + (Body?.DefenseBonus ?? 0);

    public int Speed { get; set; }

    public List<Item> Inventory { get; } = new();
    public Weapon? MainHand { get; set; }
    public Armor? Body { get; set; }

    public Point OldPosition => _oldPosition;
}
