using PluginBase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWithPlugin.Model;

public class Random : IRandom
{
    private System.Random _random;

    public Random()
    {
        _random = new System.Random();
    }
    public int Next()
    {
        return _random.Next();
    }

    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }

    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }

    public void NextBytes(byte[] buffer)
    {
        _random.NextBytes(buffer);
    }

    public double NextDouble()
    {
        return _random.NextDouble();
    }

    public void Seed(int seed)
    {
        _random = new System.Random(seed);
    }
}
