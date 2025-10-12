using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BoxFlight.ViewModels;

public interface IBoxFlightViewModel
{
    bool Pause { get; set; }
    bool Stereo { get; set; }
    bool ShowObjects { get; set; }
    int ScrollOffset { get; set; }
    WriteableBitmap Frame { get; }
    WriteableBitmap MiniMap { get; }
    IRelayCommand StartCommand { get; }
    IRelayCommand StopCommand { get; }
}