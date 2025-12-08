using AsteroidsModern.Engine.Abstractions;

namespace AsteroidsModern.UI;

public sealed class SilentSound : ISound
{
    public void PlayThrust() { }
    public void PlayShoot() { }
    public void PlayBang() { }
}
