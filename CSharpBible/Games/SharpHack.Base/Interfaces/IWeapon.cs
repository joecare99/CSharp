namespace SharpHack.Base.Interfaces;

public interface IWeapon : IItem
{
    int AttackBonus { get; set; }
}