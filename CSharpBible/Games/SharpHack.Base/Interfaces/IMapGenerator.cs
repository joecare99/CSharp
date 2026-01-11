using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface IMapGenerator
{
    IMap Generate(int width, int height, Point? startPos=null);
}
