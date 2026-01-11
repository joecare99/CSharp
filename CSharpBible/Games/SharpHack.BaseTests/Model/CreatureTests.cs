using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using NSubstitute;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class CreatureTests
{
    [TestMethod]
    public void Attack_IncludesWeaponBonus()
    {
        var creature = new Creature { BaseAttack = 10 };
        var weapon = Substitute.For<IWeapon>();
            weapon.AttackBonus.Returns(5);

        creature.MainHand = weapon;

        Assert.AreEqual(15, creature.Attack);
    }

    [TestMethod]
    public void Defense_IncludesArmorBonus()
    {
        var creature = new Creature { BaseDefense = 2 };
        var armor = Substitute.For<IArmor>(); 
        armor.DefenseBonus.Returns(3);

        creature.Body = armor;

        Assert.AreEqual(5, creature.Defense);
    }
}
