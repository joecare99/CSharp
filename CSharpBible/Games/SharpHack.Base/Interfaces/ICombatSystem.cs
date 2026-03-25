using System;
using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface ICombatSystem
{
    void Attack(ICreature attacker, ICreature defender, Action<string> onMessage);
}
