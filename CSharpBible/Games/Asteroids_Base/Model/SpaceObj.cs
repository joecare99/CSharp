using Asteroids.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Asteroids.Model;

public class SpaceObj : ISpaceObj
{
    private IGame _Game;
    public bool ShowPos { get; set; }

    public Point Position { get; private set; }

    public Point Speed { get; protected set; }

    public float Turned { get; protected set; }

    public float TurnSpeed { get; protected set; }

    public IVertex[] Edges { get; private set; }

    public Point[] DataPoints { get; private set; }

    public float fSize { get; private set; }

    public Color color { get; private set; }

    public Point[] Points { get; private set; }

    public bool xWrap { get; set; }

    public bool xOutline { get; protected set; }

    public void Draw()
    {
        throw new NotImplementedException();
    }

    public SpaceObj(IGame game, Point position, Point speed, float turned, float turnSpeed, 
        IVertex[] edges, float fSize, Color color)
    {
        _Game = game;
        Position = position;
        Speed = speed;
        Turned = turned;
        TurnSpeed = turnSpeed;
        Edges = edges;
        this.fSize = fSize;
        this.color = color;
        ProcessPoints();
    }

    public void ProcessPoints()
    {
        var tmpx = fSize * _Game.ScreenFactor.X;
        var tmpy = fSize * _Game.ScreenFactor.Y;
        var tp = new List<Point>();
        foreach(var edge in Edges)
        {
          var tmp = (edge.Angle + Turned);
            tp.Add(new Point((Position.X * _Game.Screen.Width) + (Math.Sin(tmp) * edge.Length * tmpx),
               (Position.X * _Game.Screen.Height) - (Math.Cos(tmp) * edge.Length * tmpy)));
        }
        Points = tp.ToArray();
    }

    public void Update()
    {
        throw new NotImplementedException();
    }
}
