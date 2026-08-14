// ***********************************************************************
// Assembly         : MVVM_42a_3DView
// Author           : Mir
// Created          : 03-09-2025
//
// Last Modified By : Mir
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="ISceneModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MVVM_42a_3DView.Models;

/// <summary>
/// Interface ISceneModel
/// </summary>
public interface ISceneModel
{
    MeshGeometry3D FloorGeometry { get; }
    MeshGeometry3D CeilingGeometry { get; }
    MeshGeometry3D WallGeometry { get; }
    Brush FloorBrush { get; }
    Brush CeilingBrush { get; }
    Brush WallBrush { get; }
    Vector3D LightDirection { get; }
    Vector3D RotationAxis { get; }
    double RotationAngle { get; }
    Point3D CameraPosition { get; }
    Vector3D CameraLookDirection { get; }
    Vector3D CameraUpDirection { get; }
    event PropertyChangedEventHandler PropertyChanged;
    void ResetCamera();
}
