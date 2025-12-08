using AsteroidsModern.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidsModern.Engine.Model;

public class CRandom: IRandom
{
    private readonly Random _rand = new();
    public int Next(int minValue, int maxValue) => _rand.Next(minValue, maxValue);
    public float NextSingle() => _rand.NextSingle();
}
