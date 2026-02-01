// ***********************************************************************
// Assembly         : MVVM_42_3DView
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
using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MVVM_42_3DView.Models;

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

    public MeshGeometry3D Geometry { get; }
    public Brush ModelBrush { get; }
    public Vector3D LightDirection { get; } = new Vector3D(-1, -1, -2);
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
        Geometry = CreateCubeGeometry(1d);
        ModelBrush = CreateBrush();
        ResetCamera();
        _timer = new(33d);
        _timer.Elapsed += (s, e) => UpdateRotation();
        _timer.Start();
    }

    public void ResetCamera()
    {
        CameraPosition = new Point3D(3, 3, 5);
        CameraLookDirection = new Vector3D(-3, -3, -5);
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

    private static MeshGeometry3D CreateCubeGeometry(double size)
    {
        var half = size / 2d;
        return new MeshGeometry3D
        {
            Positions = new Point3DCollection
            {
                new Point3D(-half, -half, -half),
                new Point3D(half, -half, -half),
                new Point3D(half, half, -half),
                new Point3D(-half, half, -half),
                new Point3D(-half, -half, half),
                new Point3D(half, -half, half),
                new Point3D(half, half, half),
                new Point3D(-half, half, half)
            },
            TriangleIndices = new Int32Collection
            {
                4, 5, 6, 4, 6, 7,
                0, 2, 1, 0, 3, 2,
                0, 7, 3, 0, 4, 7,
                1, 2, 6, 1, 6, 5,
                3, 7, 6, 3, 6, 2,
                0, 1, 5, 0, 5, 4
            }
        };
    }

    private static Brush CreateBrush()
    {
        var brush = new SolidColorBrush(Colors.CornflowerBlue);
        if (brush.CanFreeze)
        {
            brush.Freeze();
        }

        return brush;
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
