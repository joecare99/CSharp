using Avalonia;
using Avalonia.Media;
using System.ComponentModel;

namespace AA15_Labyrinth.ViewModels;

public interface ILabyrinthViewModel : INotifyPropertyChanged
{
    int Seed { get; set; }
    double CellSize { get; set; }
    double LineThickness { get; set; }
    Thickness Padding { get; set; }
    bool DrawSolution { get; set; }

    Geometry? MazeGeometry { get; }
    Geometry? SolutionGeometry { get; }

    // progress in [0,1]
    double Progress { get; }
    bool IsGenerating { get; }

    void Build(Size bounds);
    void Regenerate();
    void Randomize();
}
