using System;
using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface ICombatSystem
{
    void Attack(Creature attacker, Creature defender, Action<string> onMessage);
}
