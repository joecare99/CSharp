using System;
using Avalonia.Animation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RenderDemo.ViewModels;

public partial class AnimationsPageViewModel : ObservableObject
{
    private bool _isPlaying = true;
    [ObservableProperty]
    private string _playStateText = "Pause animations on this page";

    public void TogglePlayState()
    {
        PlayStateText = _isPlaying
            ? "Resume animations on this page" : "Pause animations on this page";
        _isPlaying = !_isPlaying;
    }
}
