using System.Windows;

namespace BoxFlight.World;

public interface IGameWorld
{
    Obstacle[] Objects { get; }

    void Initialize();


    // Compute render entries for current time/frame
    void Step(int time, bool stereo, int scrollOffset, int focusX, int maxHeight, RenderEntry[] render,
              out Point cPoint2, out Point cPoint3);
}
