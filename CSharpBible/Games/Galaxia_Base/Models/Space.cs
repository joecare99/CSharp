using Galaxia.Helper;
using Galaxia.Models.Interfaces;
using System.Collections.Generic;

namespace Galaxia.Models;

public class Space: ISpace
{
    public IDictionary<Point3d, ISector> Sectors { get; } = new Dictionary<Point3d, ISector>();

    public int SpaceTime { get; private set; }

    public void Initialize()
    {
        // Initialization logic if needed
        Sectors.Clear();
        SpaceTime = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    var point = new Point3d(x, y, z);
                    Sectors[point] = new Sector(this,point);
                    for (int i = 0; i < 2; i++)
                    {
                        Sectors[point].Starsystems.Add(
                            new Starsystem(Sectors[point],Sectors[point].Name.GetStarSysName(),point,i % 2,(i + point.X+point.Y+point.Z) % 2));
                    }
                }
            }
        }
    }
}
