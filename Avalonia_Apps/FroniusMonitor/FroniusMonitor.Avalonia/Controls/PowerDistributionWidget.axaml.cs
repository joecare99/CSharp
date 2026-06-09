using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using FroniusMonitor.Avalonia.Converters;
using FroniusMonitor.Core.ViewModels;

namespace FroniusMonitor.Avalonia.Controls;

/// <summary>
/// Displays the animated graphical power-distribution view for the Fronius dashboard.
/// </summary>
public partial class PowerDistributionWidget : UserControl
{
    private static readonly TimeSpan PacketLaunchInterval = TimeSpan.FromSeconds(0.5);
    private const double PacketSpeedPixelsPerSecond = 155d;
    private static readonly AbsolutePowerToLogSizeConverter PacketSizeConverter = new();

    private readonly List<CancellationTokenSource> _packetLoopCancellationTokenSources = [];
    private bool _packetLoopsStarted;

    public PowerDistributionWidget()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (_packetLoopsStarted)
        {
            return;
        }

        StartPacketLoop(nameof(FroniusDashboardViewModel.PvToHomeWatts), GetPath("pthPvToHome"), "PvNodeBrush");
        StartPacketLoop(nameof(FroniusDashboardViewModel.PvToBatteryWatts), GetPath("pthPvToBattery"), "BatteryNodeBrush");
        StartPacketLoop(nameof(FroniusDashboardViewModel.BatteryToHomeWatts), GetPath("pthBatteryToHome"), "HomeNodeBrush");
        StartPacketLoop(nameof(FroniusDashboardViewModel.BatteryToGridWatts), GetPath("pthBatteryToGrid"), "BatteryNodeBrush");
        StartPacketLoop(nameof(FroniusDashboardViewModel.GridToHomeWatts), GetPath("pthGridToHome"), "HomeNodeBrush");
        StartPacketLoop(nameof(FroniusDashboardViewModel.PvToGridWatts), GetPath("pthPvToGrid"), "PvNodeBrush");

        _packetLoopsStarted = true;
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        foreach (CancellationTokenSource cancellationTokenSource in _packetLoopCancellationTokenSources)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        _packetLoopCancellationTokenSources.Clear();
        _packetLoopsStarted = false;
    }

    private Path GetPath(string sName)
    {
        return this.FindControl<Path>(sName) ?? throw new InvalidOperationException($"Path '{sName}' not found.");
    }

    private Canvas PacketLayer => this.FindControl<Canvas>("cnvPacketLayer") ?? throw new InvalidOperationException("Packet layer canvas not found.");

    private void StartPacketLoop(string sFlowPropertyName, Path path, string sFillBrushKey)
    {
        CancellationTokenSource cancellationTokenSource = new();
        _packetLoopCancellationTokenSources.Add(cancellationTokenSource);
        _ = RunPacketLoopAsync(sFlowPropertyName, path, sFillBrushKey, cancellationTokenSource.Token);
    }

    private async Task RunPacketLoopAsync(string sFlowPropertyName, Path path, string sFillBrushKey, CancellationToken cancellationToken)
    {
        double fPathLength = ApproximatePathLength(path.Data);
        TimeSpan duration = TimeSpan.FromSeconds(Math.Max(2d, fPathLength / PacketSpeedPixelsPerSecond));

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (GetCurrentFlowValue(sFlowPropertyName) > 0d)
                    {
                        LaunchPacket(path.Data, duration, sFlowPropertyName, sFillBrushKey);
                    }
                }, DispatcherPriority.Background);

                await Task.Delay(PacketLaunchInterval, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void LaunchPacket(Geometry? geometry, TimeSpan duration, string sFlowPropertyName, string sFillBrushKey)
    {
        if (geometry is null)
        {
            return;
        }

        double fCurrentFlowValue = GetCurrentFlowValue(sFlowPropertyName);
        if (fCurrentFlowValue <= 0d)
        {
            return;
        }

        Ellipse packet = CreatePacket(fCurrentFlowValue, sFillBrushKey);
        PacketLayer.Children.Add(packet);

        DateTimeOffset start = DateTimeOffset.UtcNow;
        DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(16),
        };

        timer.Tick += (_, _) =>
        {
            double fElapsed = (DateTimeOffset.UtcNow - start).TotalMilliseconds;
            double fProgress = Math.Clamp(fElapsed / duration.TotalMilliseconds, 0d, 1d);
            Point point = SampleQuadraticBezier(geometry, fProgress);

            Canvas.SetLeft(packet, point.X - (packet.Width * 0.5d));
            Canvas.SetTop(packet, point.Y - (packet.Height * 0.5d));
            packet.Opacity = fProgress is > 0.1 and < 0.9 ? 1d : 0.5d;

            if (fProgress >= 1d)
            {
                timer.Stop();
                PacketLayer.Children.Remove(packet);
            }
        };

        timer.Start();
    }

    private Ellipse CreatePacket(double fFlowWatts, string sFillBrushKey)
    {
        double fPacketSize = System.Convert.ToDouble(PacketSizeConverter.Convert(fFlowWatts, typeof(double), null, CultureInfo.CurrentCulture));
        return new Ellipse
        {
            Width = fPacketSize,
            Height = fPacketSize,
            Fill = (IBrush)(Application.Current?.FindResource(sFillBrushKey) ?? Brushes.White),
            Opacity = 0d,
            IsHitTestVisible = false,
        };
    }

    private double GetCurrentFlowValue(string sFlowPropertyName)
    {
        return DataContext is not FroniusDashboardViewModel viewModel
            ? 0d
            : sFlowPropertyName switch
            {
                nameof(FroniusDashboardViewModel.PvToHomeWatts) when viewModel.ShowPvToHomePath => viewModel.PvToHomeWatts,
                nameof(FroniusDashboardViewModel.PvToBatteryWatts) when viewModel.ShowPvToBatteryPath => viewModel.PvToBatteryWatts,
                nameof(FroniusDashboardViewModel.BatteryToHomeWatts) when viewModel.ShowBatteryToHomePath => viewModel.BatteryToHomeWatts,
                nameof(FroniusDashboardViewModel.BatteryToGridWatts) when viewModel.ShowBatteryToGridPath => viewModel.BatteryToGridWatts,
                nameof(FroniusDashboardViewModel.GridToHomeWatts) when viewModel.ShowGridToHomePath => viewModel.GridToHomeWatts,
                nameof(FroniusDashboardViewModel.PvToGridWatts) when viewModel.ShowPvToGridPath => viewModel.PvToGridWatts,
                _ => 0d,
            };
    }

    private static double ApproximatePathLength(Geometry? geometry)
    {
        if (geometry is null)
        {
            return 0d;
        }

        Rect bounds = geometry.Bounds;
        return Math.Sqrt((bounds.Width * bounds.Width) + (bounds.Height * bounds.Height)) * 1.35d;
    }

    private static Point SampleQuadraticBezier(Geometry geometry, double fT)
    {
        Rect bounds = geometry.Bounds;

        Point p0 = new(bounds.Left + 6, bounds.Top + bounds.Height - 6);
        Point p1 = new(bounds.Left + (bounds.Width / 2), bounds.Top - (bounds.Height * 0.35));
        Point p2 = new(bounds.Right - 6, bounds.Top + bounds.Height - 6);

        double x = ((1 - fT) * (1 - fT) * p0.X) + (2 * (1 - fT) * fT * p1.X) + (fT * fT * p2.X);
        double y = ((1 - fT) * (1 - fT) * p0.Y) + (2 * (1 - fT) * fT * p1.Y) + (fT * fT * p2.Y);

        return new Point(x, y);
    }
}
