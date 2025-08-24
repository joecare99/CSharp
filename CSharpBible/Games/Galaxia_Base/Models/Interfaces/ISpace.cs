using System.Collections.Generic;

namespace Galaxia.Models.Interfaces;

public interface ISpace
{
    IDictionary<Point3d, ISector> Sectors { get; }
    int SpaceTime { get; }
}