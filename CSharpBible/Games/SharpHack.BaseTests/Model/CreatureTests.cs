using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Model;

namespace SharpHack.BaseTests.Model;

[TestClass]
public class CreatureTests
{
    [TestMethod]
    public void Attack_IncludesWeaponBonus()
    {
        var creature = new Creature { BaseAttack = 10 };
        var weapon = new Weapon { AttackBonus = 5 };
        
        creature.MainHand = weapon;

        Assert.AreEqual(15, creature.Attack);
    }

    [TestMethod]
    public void Defense_IncludesArmorBonus()
    {
        var creature = new Creature { BaseDefense = 2 };
        var armor = new Armor { DefenseBonus = 3 };
        
        creature.Body = armor;

        Assert.AreEqual(5, creature.Defense);
    }
}
