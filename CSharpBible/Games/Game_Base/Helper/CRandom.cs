using BaseLib.Interfaces;
using System;

namespace BaseLib.Helper;

public class CRandom :IRandom
{
    private Random _random;

    public CRandom() 
    { 
        _random = new Random();
    }

    public int Next(int v1, int v2) => v2 !=-1 || v1<v2? _random.Next(v1, v2): _random.Next(v1);

    public double NextDouble() => _random.NextDouble();

    public int NextInt() => _random.Next();

    public void Seed(int value) => _random = new Random(value);
}
