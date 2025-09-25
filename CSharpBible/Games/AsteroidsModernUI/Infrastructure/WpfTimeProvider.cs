using System.Diagnostics;
using AsteroidsModern.Engine.Abstractions;

namespace AsteroidsModern.UI;

public sealed class WpfTimeProvider : ITimeProvider
{
    private readonly Stopwatch _sw = Stopwatch.StartNew();
    private double _last;

    public double TotalTime => _sw.Elapsed.TotalSeconds;
    public double DeltaTime { get; private set; }

    public void Tick()
    {
        var now = _sw.Elapsed.TotalSeconds;
        DeltaTime = Math.Max(0, now - _last);
        _last = now;
    }
}
