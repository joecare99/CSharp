// ***********************************************************************
// Assembly         : MVVM_42b_3DView
// Author           : Mir
// Created          : 03-09-2025
//
// Last Modified By : Mir
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="SceneModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MVVM_42b_3DView.Models;

/// <summary>
/// Class SceneModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="ISceneModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="ISceneModel" />
public partial class SceneModel : ObservableObject, ISceneModel
{
    private const string csApplStart = "Application startet";
#if !NET5_0_OR_GREATER
    private const string csApplEnded = "Application ended";
#endif

    #region Properties
    private readonly Timer _timer;
    private readonly ISysTime _systime;
    private readonly ILog _log;
    private double _rotationAngle;
    private Point3D _cameraPosition;
    private Vector3D _cameraLookDirection;
    private Vector3D _cameraUpDirection;

    public MeshGeometry3D FloorGeometry { get; }
    public MeshGeometry3D CeilingGeometry { get; }
    public MeshGeometry3D NorthWallGeometry { get; }
    public MeshGeometry3D SouthWallGeometry { get; }
    public MeshGeometry3D EastWallGeometry { get; }
    public MeshGeometry3D WestWallGeometry { get; }
    public Brush FloorBrush { get; }
    public Brush CeilingBrush { get; }
    public Brush NorthWallBrush { get; }
    public Brush SouthWallBrush { get; }
    public Brush EastWallBrush { get; }
    public Brush WestWallBrush { get; }
    public Vector3D LightDirection { get; } = new Vector3D(-1, -1, 2);
    public Vector3D RotationAxis { get; } = new Vector3D(0, 1, 0);

    public double RotationAngle
    {
        get => _rotationAngle;
        private set => SetProperty(ref _rotationAngle, value);
    }

    public Point3D CameraPosition
    {
        get => _cameraPosition;
        private set => SetProperty(ref _cameraPosition, value);
    }

    public Vector3D CameraLookDirection
    {
        get => _cameraLookDirection;
        private set => SetProperty(ref _cameraLookDirection, value);
    }

    public Vector3D CameraUpDirection
    {
        get => _cameraUpDirection;
        private set => SetProperty(ref _cameraUpDirection, value);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneModel"/> class.
    /// </summary>
    public SceneModel(ISysTime sysTime, ILog log)
    {
        _systime = sysTime;
        _log = log;
        _log.Log(csApplStart);
        var roomSize = 8d;
        var roomHeight = 4d;
        FloorGeometry = CreateFloorGeometry(roomSize);
        CeilingGeometry = CreateCeilingGeometry(roomSize, roomHeight);
        NorthWallGeometry = CreateWallGeometry(
            new Point3D(-roomSize / 2d, 0,          roomSize / 2d),
            new Point3D(-roomSize / 2d, roomHeight, roomSize / 2d),
            new Point3D(roomSize / 2d, roomHeight,  roomSize / 2d),
            new Point3D(roomSize / 2d, 0,           roomSize / 2d));
        SouthWallGeometry = CreateWallGeometry(
            new Point3D(roomSize / 2d, 0,           -roomSize / 2d),
            new Point3D(roomSize / 2d,  roomHeight, -roomSize / 2d),
            new Point3D(-roomSize / 2d, roomHeight, -roomSize / 2d),
            new Point3D(-roomSize / 2d, 0,          -roomSize / 2d));
        EastWallGeometry = CreateWallGeometry(
            new Point3D(roomSize / 2d, 0,          roomSize / 2d),
            new Point3D(roomSize / 2d, roomHeight, roomSize / 2d),
            new Point3D(roomSize / 2d, roomHeight, -roomSize / 2d),
            new Point3D(roomSize / 2d, 0,          -roomSize / 2d));
        WestWallGeometry = CreateWallGeometry(
            new Point3D(-roomSize / 2d, 0,          -roomSize / 2d),
            new Point3D(-roomSize / 2d, roomHeight, -roomSize / 2d),
            new Point3D(-roomSize / 2d, roomHeight, roomSize / 2d),
            new Point3D(-roomSize / 2d, 0,          roomSize / 2d));
        FloorBrush = CreateNoiseBrush(
            256,
            0.06,
            d => SmoothStep(d),
            CreatePaletteSelector(Colors.SaddleBrown, Colors.Peru, Colors.BurlyWood),
            0.25);
        NorthWallBrush = CreateNoiseBrush(
            256,
            0.09,
            d => Ridge(d, 1.6),
            CreatePaletteSelector(Colors.DarkSlateGray, Colors.SlateGray, Colors.LightSlateGray),
            0.2);
        SouthWallBrush = CreateNoiseBrush(
            256,
            0.08,
            d => SmoothStep(d * 1.1),
            CreatePaletteSelector(Colors.DarkRed, Colors.Firebrick, Colors.IndianRed),
            0.2);
        EastWallBrush = CreateNoiseBrush(
            256,
            0.07,
            d => SmoothStep(d),
            CreatePaletteSelector(Colors.MidnightBlue, Colors.RoyalBlue, Colors.LightSteelBlue),
            1.0);
        WestWallBrush = CreateNoiseBrush(
            512,
            0.05,
            d => Math.Cos(d* Math.PI*8d )* 0.5 + 0.5,
            CreatePaletteSelector(Colors.PaleGreen, Colors.ForestGreen, Colors.DarkGreen),
//            CreatePaletteSelector(Colors.White, Colors.ForestGreen, Colors.White),
            1.0);
        CeilingBrush = CreateNoiseBrush(
            256,
            0.05,
            d => SmoothStep(d),
            CreatePaletteSelector(Colors.LightSteelBlue, Colors.WhiteSmoke, Colors.AliceBlue),
            1.0);
        ResetCamera();
        _timer = new(33d);
        _timer.Elapsed += (s, e) => UpdateRotation();
        _timer.Start();
    }

    public void ResetCamera()
    {
        CameraPosition = new Point3D(0, 1.6, -4);
        CameraLookDirection = new Vector3D(0, 0, 1);
        CameraUpDirection = new Vector3D(0, 1, 0);
    }

    private void UpdateRotation()
    {
        var angle = RotationAngle + 1d;
        if (angle >= 360d)
        {
            angle -= 360d;
        }

        RotationAngle = angle;
    }

    private static MeshGeometry3D CreateFloorGeometry(double size)
    {
        var half = size / 2d;
        return new MeshGeometry3D
        {
            Positions = new Point3DCollection
            {
                new Point3D(-half, 0, -half),
                new Point3D(half, 0, -half),
                new Point3D(half, 0, half),
                new Point3D(-half, 0, half)
            },
            TriangleIndices = new Int32Collection
            {
                0, 1, 2,
                0, 2, 3
            },
            TextureCoordinates = new PointCollection
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
                new Point(0, 0)
            }
        };
    }

    private static MeshGeometry3D CreateCeilingGeometry(double size, double height)
    {
        var half = size / 2d;
        return new MeshGeometry3D
        {
            Positions = new Point3DCollection
            {
                new Point3D(-half, height, -half),
                new Point3D(-half, height, half),
                new Point3D(half, height, half),
                new Point3D(half, height, -half)
            },
            TriangleIndices = new Int32Collection
            {
                0, 1, 2,
                0, 2, 3
            },
            TextureCoordinates = new PointCollection
            {
                new Point(0, 1),
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 1)
            }
        };
    }

    private static MeshGeometry3D CreateWallGeometry(Point3D p0, Point3D p1, Point3D p2, Point3D p3)
    {
        return new MeshGeometry3D
        {
            Positions = new Point3DCollection { p0, p1, p2, p3 },
            TriangleIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 },
            TextureCoordinates = new PointCollection
            {
                new Point(0, 0.4),
                new Point(0, 0),
                new Point(1, 0),
                new Point(1, 0.4)
            }
        };
    }

    private static Brush CreateNoiseBrush(
        int size,
        double scale,
        Func<double, double> noiseFilter,
        Func<double, Color> colorSelector,
        double tileSize)
    {
        var bitmap = new WriteableBitmap(size, size, 96, 96, PixelFormats.Bgra32, null);
        var pixels = new byte[size * size * 4];

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var xf = (double)x / (double)(size);
                var yf = (double)y / (double)(size);
                var noise = 
                    SimplexNoise.Noise((xf + 0.5) * scale * 256, yf * scale *256 ) * xf * yf +
                    SimplexNoise.Noise((xf - 0.5) * scale * 256, yf * scale * 256) * (1-xf) * yf +
                    SimplexNoise.Noise((xf+0.5) * scale * 256, (yf-1) * scale * 256) * xf *(1- yf) +
                    SimplexNoise.Noise((xf-0.5) * scale * 256, (yf-1) * scale * 256) * (1-xf) * (1-yf) ;
                noise = noiseFilter(Clamp(noise));
                var color = colorSelector(Clamp(noise));
                var offset = (y * size + x) * 4;
                pixels[offset] = color.B;
                pixels[offset + 1] = color.G;
                pixels[offset + 2] = color.R;
                pixels[offset + 3] = color.A;
            }
        }

        bitmap.WritePixels(new Int32Rect(0, 0, size, size), pixels, size * 4, 0);

        var brush = new ImageBrush(bitmap)
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, tileSize, tileSize),
            ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
            Stretch = Stretch.Fill
        };
        if (brush.CanFreeze)
        {
            brush.Freeze();
        }

        return brush;
    }

    private static Func<double, Color> CreatePaletteSelector(params Color[] colors)
    {
        if (colors is null || colors.Length == 0)
        {
            return _ => Colors.White;
        }

        if (colors.Length == 1)
        {
            return _ => colors[0];
        }

        return value =>
        {
            var scaled = Clamp(value) * (colors.Length - 1);
            var index = (int)Math.Floor(scaled);
            var next = Math.Min(colors.Length - 1, index + 1);
            var localT = scaled - index;
            return Lerp(colors[index], colors[next], localT);
        };
    }

    private static double SmoothStep(double value)
    {
        var t = Clamp(value);
        return t * t * (3d - 2d * t);
    }

    private static double Ridge(double value, double sharpness)
    {
        var t = 1d - Math.Abs(Clamp(value) * 2d - 1d);
        return Math.Pow(t, sharpness);
    }

    private static double Frequency(double value, double factor)
    {
        return 0.5 - Math.Cos(value * Math.PI * factor) * 0.5;
    }

    private static Color Lerp(Color from, Color to, double t)
    {
        var clamped = Clamp(t);
        return Color.FromArgb(
            (byte)(from.A + (to.A - from.A) * clamped),
            (byte)(from.R + (to.R - from.R) * clamped),
            (byte)(from.G + (to.G - from.G) * clamped),
            (byte)(from.B + (to.B - from.B) * clamped));
    }

    private static double Clamp(double value)
    {
        if (value < 0d)
        {
            return 0d;
        }

        if (value > 1d)
        {
            return 1d;
        }

        return value;
    }

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="SceneModel" /> class.
    /// </summary>
    ~SceneModel()
    {
        _timer.Stop();
        _log.Log(csApplEnded);
        return;
    }
#endif
    #endregion
}
