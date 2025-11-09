using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using AsteroidsModern.Engine.Abstractions;
using AsteroidsModern.Engine.Services;
using AsteroidsModern.Engine.Services.Interfaces;

namespace AsteroidsModern.UI;

public partial class MainViewModel : ObservableObject
{
    private readonly IGameWorld _world;
    private readonly WpfTimeProvider _time;
    private readonly ISound _sound;

    public MainViewModel(IGameWorld world, ITimeProvider time, ISound sound)
    {
        _world = world;
        _time = (WpfTimeProvider)time;
        _sound = sound;
        _world.ShowTitle();       
    }

    public void SetSize(float width, float height)
    {
        if (_world is GameWorld gw)
            gw.WorldSize = new Vector2(width, height);
    }

    public void Tick(IGameInput input)
    {
        _time.Tick();
        _world.Update(input, _time, _sound);
    }

    public void Render(IRenderContext ctx)
    {
        _world.Render(ctx);
    }
}
