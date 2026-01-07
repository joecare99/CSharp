using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpHack.Persist;

public class GamePersist : IGamePersist
{
    public bool LoadLevel(int level, out IMap map, out IList<ICreature> enemies)
    {
        throw new NotImplementedException();
    }

    public void SaveLevel(int level, IMap map, ICreature player, IList<ICreature> enemies)
    {
        throw new NotImplementedException();
    }
}
