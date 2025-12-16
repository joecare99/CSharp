using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MidiSwing.MVVM.Models;

namespace MidiSwing.MVVM.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly MusicGenerator _musicGenerator = new MusicGenerator();

    public IAsyncRelayCommand PlayCommand { get; }

    [ObservableProperty]
    private bool isPlaying;

    public MainWindowViewModel()
    {
        PlayCommand = new AsyncRelayCommand(ExecutePlayAsync, CanExecutePlay);
    }

    private bool CanExecutePlay() => !IsPlaying;

    private async Task ExecutePlayAsync()
    {
        IsPlaying = true;
        await _musicGenerator.PlaySwingMelody();
        IsPlaying = false;
    }

    partial void OnIsPlayingChanged(bool value)
    {
        PlayCommand.NotifyCanExecuteChanged();
    }
}
