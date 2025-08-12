using Galaxia.Models.Interfaces;

namespace Galaxia.Models;

public class Starsystem(ISector sector,string name,Point3d position,float population,float resources) : IStarsystem
{
    public string Name => name;

    public Point3d Position => position;

    public float Population => population;

    public float Resources => resources;
}