using SharpHack.Base.Model;

namespace SharpHack.LevelGen;

public interface IMapGenerator
{
    Map Generate(int width, int height, Point? startPos=null);
}
