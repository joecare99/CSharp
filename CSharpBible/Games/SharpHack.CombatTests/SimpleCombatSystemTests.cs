using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpHack.Base.Model;
using SharpHack.Combat;
using System.Collections.Generic;

namespace SharpHack.CombatTests;

[TestClass]
public class SimpleCombatSystemTests
{
    [TestMethod]
    public void Attack_DealsCorrectDamage()
    {
        var system = new SimpleCombatSystem();
        var attacker = new Creature { Name = "Attacker", BaseAttack = 10 }; // Changed Attack to BaseAttack
        var defender = new Creature { Name = "Defender", BaseDefense = 2, HP = 20 }; // Changed Defense to BaseDefense
        var messages = new List<string>();

        system.Attack(attacker, defender, msg => messages.Add(msg));

        Assert.AreEqual(12, defender.HP); // 20 - (10 - 2) = 12
        Assert.IsNotEmpty(messages);
        Assert.Contains("8 damage", messages[0]);
    }

    [TestMethod]
    public void Attack_ReportsDeath()
    {
        var system = new SimpleCombatSystem();
        var attacker = new Creature { Name = "Attacker", BaseAttack = 10 }; // Changed Attack to BaseAttack
        var defender = new Creature { Name = "Defender", BaseDefense = 0, HP = 5 }; // Changed Defense to BaseDefense
        var messages = new List<string>();

        system.Attack(attacker, defender, msg => messages.Add(msg));

        Assert.IsLessThanOrEqualTo(0, defender.HP);
        Assert.IsTrue(messages.Exists(m => m.Contains("dies")));
    }
}
