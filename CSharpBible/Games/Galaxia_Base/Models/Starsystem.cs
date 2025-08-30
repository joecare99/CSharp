using BaseLib.Helper;
using Galaxia.Models.Interfaces;
using System.Linq;

namespace Galaxia.Models;

public class Starsystem(ISector sector,string name,Point3d position,float population,float resources) : IStarsystem
{
    public ISector Sector => sector;
    public string Name => name;

    public Point3d Position => position;

    public float Population => population;

    public float Resources => resources;

    public IFleet? Fleet { get; private set; }

    public bool IsOpen => true;

    public bool SetFleet(IFleet? fleet)
    {
        if (fleet != null && Fleet != null)
        {
            if (Fleet.Owner == fleet.Owner)
            {
                // Wenn die Flotten dem gleichen Besitzer gehören, fusioniere sie
                Fleet.Join(fleet);
                return true;
            }
            else
            {
            // Wenn die Flotten unterschiedlichen Besitzern gehören => Raumkampf
            fleet = IoC.GetRequiredService<IBattleService>().StartBattle(Fleet, fleet);
            if (fleet == Fleet) // Die angreifende Flotte wurde zerstört
                return true;
            }
        }
        
        if (Fleet != null)
        {
            if (!Fleet.Owner.Stars.Contains(this) && Fleet.Owner.Home != this)
                fleet.Owner.Stars.Remove(this);
        }
        
        Fleet = fleet;
        
        if (fleet != null && fleet.Container != this)
        {
            if (!fleet.Owner.Stars.Contains(this))
                fleet.Owner.Stars.Add(this);

            fleet.MoveTo(this);
        }
        return true;
    }
}