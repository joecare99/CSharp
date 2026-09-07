using System;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Showcase.Desktop.Capabilities;
using ConsoleLib.Showcase.Desktop.Rendering;

namespace ConsoleLib.Showcase.Desktop.ViewModels;

/// <summary>Portable analog-clock presentation state.</summary>
public partial class ClockViewModel : ObservableObject, IDisposable
{
    private readonly Timer? _timer;
    private DateTime _currentTime;
    private readonly IShowcaseAlertService _alert;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AlarmText))]
    private bool alarmEnabled;

    public ClockViewModel(DateTime? currentTime = null, IShowcaseAlertService? alert = null)
    {
        var hasFixedTime = currentTime.HasValue;
        _currentTime = currentTime ?? DateTime.Now;
        _alert = alert ?? new UnavailableShowcaseAlertService();
        _timer = hasFixedTime
            ? null
            : new Timer(_ => CurrentTime = DateTime.Now, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public string TimeText => _currentTime.ToString("HH:mm:ss");

    public string AlarmText => AlarmEnabled ? "Alarm armed" : "Alarm off";

    public string BrailleFace => BrailleClockRenderer.Render(_currentTime);

    public string HalfBlockFace => HalfBlockClockRenderer.Render(_currentTime);
    public string PixelFrame => HalfBlockClockRenderer.RenderColorMap(_currentTime);

    public string SecondsIndicator => $"Seconds hand: {_currentTime.Second:00}";

    public string AlarmIndicator => AlarmEnabled ? "Alarm pointer: armed" : "Alarm pointer: off";

    public string Face
    {
        get
        {
            var rows = new[]
            {
                "          .--------.          ",
                "       .-'     12     '-.       ",
                "     .'    " + TimeText + "    '.     ",
                "    /    9      +      3   \\    ",
                $"   |        H{HourHand}  +  M{MinuteHand}        |    ",
                "    \\                    /     ",
                "     '.        6       .'      ",
                "       '-.          .-'        ",
                $"          H:{HourHand}  M:{MinuteHand}  S:{_currentTime.Second:00}"
            };
            return string.Join(
                Environment.NewLine,
                rows.Select(row => row.Length >= 32 ? row[..32] : row.PadRight(32)));
        }
    }

    public string HourHand => GetDirection((_currentTime.Hour % 12 + _currentTime.Minute / 60d) / 12d);

    public string MinuteHand => GetDirection(_currentTime.Minute / 60d);

    [RelayCommand]
    private void SetAlarm()
    {
        AlarmEnabled = true;
        if (_alert.IsAvailable)
            _alert.Alert();
    }

    [RelayCommand]
    private void StopAlarm() => AlarmEnabled = false;

    private DateTime CurrentTime
    {
        set
        {
            if (_currentTime == value)
                return;
            _currentTime = value;
            OnPropertyChanged(nameof(TimeText));
            OnPropertyChanged(nameof(Face));
            OnPropertyChanged(nameof(BrailleFace));
            OnPropertyChanged(nameof(HalfBlockFace));
            OnPropertyChanged(nameof(PixelFrame));
            OnPropertyChanged(nameof(SecondsIndicator));
        }
    }

    public void Dispose() => _timer?.Dispose();

    private static string GetDirection(double turn) =>
        (((int)Math.Round(turn * 8, MidpointRounding.AwayFromZero) + 8) % 8) switch
        {
            0 => "^",
            1 => "/",
            2 => ">",
            3 => "\\",
            4 => "v",
            5 => "/",
            6 => "<",
            _ => "\\"
        };
}
