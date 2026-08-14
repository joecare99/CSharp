// ***********************************************************************
// Assembly         : MVVM_42a_3DView
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

namespace MVVM_42a_3DView.Models;

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
    public MeshGeometry3D WallGeometry { get; }
    public Brush FloorBrush { get; }
    public Brush CeilingBrush { get; }
    public Brush WallBrush { get; }
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
        var roomHeight = 3d;
        FloorGeometry = CreateFloorGeometry(roomSize);
        CeilingGeometry = CreateCeilingGeometry(roomSize, roomHeight);
        WallGeometry = CreateWallGeometry(roomSize, roomHeight);
        FloorBrush = CreateNoiseBrush(256, 0.06, Colors.SaddleBrown, Colors.BurlyWood, 0.25);
        WallBrush = CreateNoiseBrush(256, 0.08, Colors.DimGray, Colors.LightGray, 0.2);
        CeilingBrush = CreateNoiseBrush(256, 0.05, Colors.LightSteelBlue, Colors.WhiteSmoke, 0.35);
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

    private static MeshGeometry3D CreateWallGeometry(double size, double height)
    {
        var half = size / 2d;
        var mesh = new MeshGeometry3D
        {
            Positions = new Point3DCollection(),
            TextureCoordinates = new PointCollection(),
            TriangleIndices = new Int32Collection()
        };

        AddQuad(mesh,
            new Point3D(-half, 0, half),
            new Point3D(-half, height, half),
            new Point3D(half, height, half),
            new Point3D(half, 0, half));

        AddQuad(mesh,
            new Point3D(-half, 0, -half),
            new Point3D(half, 0, -half),
            new Point3D(half, height, -half),
            new Point3D(-half, height, -half));

        AddQuad(mesh,
            new Point3D(-half, 0, -half),
            new Point3D(-half, height, -half),
            new Point3D(-half, height, half),
            new Point3D(-half, 0, half));

        AddQuad(mesh,
            new Point3D(half, 0, -half),
            new Point3D(half, 0, half),
            new Point3D(half, height, half),
            new Point3D(half, height, -half));

        return mesh;
    }

    private static void AddQuad(MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2, Point3D p3)
    {
        var index = mesh.Positions.Count;
        mesh.Positions.Add(p0);
        mesh.Positions.Add(p1);
        mesh.Positions.Add(p2);
        mesh.Positions.Add(p3);
        mesh.TextureCoordinates.Add(new Point(0, 1));
        mesh.TextureCoordinates.Add(new Point(0, 0));
        mesh.TextureCoordinates.Add(new Point(1, 0));
        mesh.TextureCoordinates.Add(new Point(1, 1));
        mesh.TriangleIndices.Add(index);
        mesh.TriangleIndices.Add(index + 1);
        mesh.TriangleIndices.Add(index + 2);
        mesh.TriangleIndices.Add(index);
        mesh.TriangleIndices.Add(index + 2);
        mesh.TriangleIndices.Add(index + 3);
    }

    private static Brush CreateNoiseBrush(int size, double scale, Color baseColor, Color accentColor, double tileSize)
    {
        var bitmap = new WriteableBitmap(size, size, 96, 96, PixelFormats.Bgra32, null);
        var pixels = new byte[size * size * 4];

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var noise = (SimplexNoise.Noise(x * scale, y * scale) + 1d) / 2d;
                var color = Lerp(baseColor, accentColor, noise);
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

    private static Color Lerp(Color from, Color to, double t)
    {
        var clamped = Math.Max(0d, Math.Min(1d, t));
        return Color.FromArgb(
            (byte)(from.A + (to.A - from.A) * clamped),
            (byte)(from.R + (to.R - from.R) * clamped),
            (byte)(from.G + (to.G - from.G) * clamped),
            (byte)(from.B + (to.B - from.B) * clamped));
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
