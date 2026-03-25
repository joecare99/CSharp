using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BoxFlight.World;

namespace BoxFlight.ViewModels;

public partial class BoxFlightViewModel : ObservableObject, IBoxFlightViewModel
{
    private readonly IGameWorld _world;
    private RenderEntry[] _render = Array.Empty<RenderEntry>();
    private readonly DispatcherTimer _timer;

    private Point _cPoint2, _cPoint3;
    private int _time = 3440;
    private int _focusX; // mouse x

    [ObservableProperty] public partial bool Pause {  get; set; }
    [ObservableProperty] public partial bool Stereo { get; set; }
    [ObservableProperty] public partial bool ShowObjects { get; set; } = true;
    [ObservableProperty] public partial int ScrollOffset { get; set; } = 500;

    public WriteableBitmap Frame { get; }
    public WriteableBitmap MiniMap { get; }


    public BoxFlightViewModel() : this(new GameWorld()) { }

    public BoxFlightViewModel(IGameWorld world)
    {
        _world = world;

        int width = 1280, height = 720;
        Frame = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        MiniMap = new WriteableBitmap(400, 400, 96, 96, PixelFormats.Bgra32, null);
        _render = new RenderEntry[width];
        for (int i = 0; i < _render.Length; i++) _render[i] = new RenderEntry();
        _focusX = width / 2;

        _world.Initialize();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
        _timer.Tick += (_, _) => Tick();
        _timer.Start();
    }

    [RelayCommand]
    private void Start()
    {
        Pause = false; _timer.Start();
    }
    [RelayCommand]
    private void Stop()
    {
        Pause = true; _timer.Stop();
    }

    private static Point PathFunction(double omega)
    {
        return new Point(
            -Math.Cos(omega) * 200.0,
            (Math.Sin(omega) * Math.Sin(-Math.Cos(omega) * Math.PI * 0.5) * 5.5 + omega) / Math.PI / 2.0 * 200.0
        );
    }

    private void Tick()
    {
        if (!Pause) _time = (_time + 1) % 6000;
        _timer.Interval = TimeSpan.FromMilliseconds(Pause ? 250 : 20);

        int maxHeight = Frame.PixelHeight * 4;
        _world.Step(_time, Stereo, ScrollOffset, _focusX, maxHeight, _render, out _cPoint2, out _cPoint3);

        // Draw to frame
        DrawFrameBackground();
        DrawRenderEntries(Frame.PixelHeight / 2);
    }

    private void DrawFrameBackground()
    {
        int w = Frame.PixelWidth, h = Frame.PixelHeight;
        int stride = w * 4;
        byte[] buffer = new byte[h * stride];
        int bd2 = h / 2;
        for (int y = 0; y < bd2; y++)
        {
            byte c1 = (byte)(128 + (y * 127) / bd2);
            uint clrSky = 0xFF000000u | (uint)c1 << 16 | (uint)c1 << 8 | 0xFFu; // BGRA
            byte c2 = (byte)(128 - (y * 127) / bd2);
            uint clrGround = 0xFF000000u | ((uint)c2 / 2u) << 16 | (uint)c2 << 8 | ((uint)c2 / 2u);

            int yTopOffset = y * stride;
            int yBottom = h - 1 - y;
            int yBottomOffset = yBottom * stride;

            for (int x = 0; x < w; x++)
            {
                int offTop = yTopOffset + x * 4;
                buffer[offTop + 0] = (byte)(clrSky & 0xFF);
                buffer[offTop + 1] = (byte)((clrSky >> 8) & 0xFF);
                buffer[offTop + 2] = (byte)((clrSky >> 16) & 0xFF);
                buffer[offTop + 3] = (byte)((clrSky >> 24) & 0xFF);

                int offBottom = yBottomOffset + x * 4;
                buffer[offBottom + 0] = (byte)(clrGround & 0xFF);
                buffer[offBottom + 1] = (byte)((clrGround >> 8) & 0xFF);
                buffer[offBottom + 2] = (byte)((clrGround >> 16) & 0xFF);
                buffer[offBottom + 3] = (byte)((clrGround >> 24) & 0xFF);
            }
        }

        Frame.WritePixels(new Int32Rect(0, 0, w, h), buffer, stride, 0);
    }

    private void DrawRenderEntries(int bd2)
    {
        int w = Frame.PixelWidth, h = Frame.PixelHeight;
        int stride = w * 4;
        byte[] buffer = new byte[h * stride];
        Frame.CopyPixels(buffer, stride, 0);

        for (int x = 0; x < _render.Length; x++)
        {
            var re = _render[x];
            if (re.Shad !=0 && re.Height > re.Shad / 2)
                DrawVLine(buffer, stride, x, bd2 + re.Shad, bd2 + re.Shad / 2, Colors.Black);

            int c1 = Math.Clamp(re.Light, 0, 255);
            var col1 = ScaleColor(re.Color, c1);
            DrawVLine(buffer, stride, x, bd2 - re.Height, bd2 + re.Height, col1);

            int c2 = Math.Clamp(re.Light2, 0, 255);
            var col2 = ScaleColor(re.Color2, c2);
            DrawVLine(buffer, stride, x, bd2 - re.Height2, bd2 + re.Height2, col2);
        }

        Frame.WritePixels(new Int32Rect(0, 0, w, h), buffer, stride, 0);
    }

    private static Color ScaleColor(Color baseColor, int light)
    {
        double f = light / 255.0;
        return Color.FromRgb(
            (byte)Math.Clamp(baseColor.R * f, 0, 255),
            (byte)Math.Clamp(baseColor.G * f, 0, 255),
            (byte)Math.Clamp(baseColor.B * f, 0, 255));
    }

    private static void DrawVLine(byte[] buffer, int stride, int x, int y1, int y2, Color c)
    {
        if (y1 > y2) (y1, y2) = (y2, y1);
        int h = buffer.Length / stride;
        y1 = Math.Clamp(y1, 0, h - 1);
        y2 = Math.Clamp(y2, 0, h - 1);
        for (int y = y1; y <= y2; y++)
        {
            int off = y * stride + x * 4;
            buffer[off + 0] = c.B;
            buffer[off + 1] = c.G;
            buffer[off + 2] = c.R;
            buffer[off + 3] = 255;
        }
    }
}
