using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;

namespace SharpHack.LevelGen;

public interface IMapGenerator
{
    IMap Generate(int width, int height, Point? startPos=null);
}
