using System;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;

namespace SharpHack.Combat;

public class SimpleCombatSystem : ICombatSystem
{
    public void Attack(Creature attacker, Creature defender, Action<string> onMessage)
    {
        int damage = Math.Max(0, attacker.Attack - defender.Defense);
        defender.HP -= damage;

        onMessage?.Invoke($"{attacker.Name} attacks {defender.Name} for {damage} damage.");

        if (defender.HP <= 0)
        {
            onMessage?.Invoke($"{defender.Name} dies!");
        }
    }
}
