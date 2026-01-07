using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces
{
    public interface ICreature: IGameObject
    {
        int Attack { get; }
        int BaseAttack { get; set; }
        int BaseDefense { get; set; }
        Armor? Body { get; set; }
        int Defense { get; }
        int HP { get; set; }
        List<Item> Inventory { get; }
        Weapon? MainHand { get; set; }
        int MaxHP { get; set; }
        int Speed { get; set; }
        Point OldPosition { get; }
    }
}