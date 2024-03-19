using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Asteroids.Model.Interfaces;

public interface ISpaceObj : IScreenObj
{
    bool ShowPos { get; set; }
    Point Position { get; }
    Point Speed { get; }
    float Turned { get; }
    float TurnSpeed { get; }
    IVertex[] Edges { get; }
    Point[] DataPoints { get; }
    void ProcessPoints();
    void Draw();
    void Update();
}
// Path: Asteroids/Model/Interfaces/IScreenObj.cs

