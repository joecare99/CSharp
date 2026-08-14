// ***********************************************************************
// Assembly         : MVVM_42a_3DView
// Author           : Mir
// Created          : 03-09-2025
//
// Last Modified By : Mir
// Last Modified On : 03-09-2025
// ***********************************************************************
// <copyright file="ThreeDViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_42a_3DView.Models;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MVVM_42a_3DView.ViewModels;

/// <summary>
/// Class ThreeDViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class ThreeDViewModel : BaseViewModelCT
{
    #region Properties
    private readonly ISceneModel _model;

    public MeshGeometry3D FloorGeometry => _model.FloorGeometry;
    public MeshGeometry3D CeilingGeometry => _model.CeilingGeometry;
    public MeshGeometry3D WallGeometry => _model.WallGeometry;
    public Brush FloorBrush => _model.FloorBrush;
    public Brush CeilingBrush => _model.CeilingBrush;
    public Brush WallBrush => _model.WallBrush;
    public Vector3D LightDirection => _model.LightDirection;
    public Vector3D RotationAxis => _model.RotationAxis;
    public double RotationAngle => _model.RotationAngle;
    public Point3D CameraPosition => _model.CameraPosition;
    public Vector3D CameraLookDirection => _model.CameraLookDirection;
    public Vector3D CameraUpDirection => _model.CameraUpDirection;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreeDViewModel"/> class.
    /// </summary>
    public ThreeDViewModel() : this(IoC.GetRequiredService<ISceneModel>())
    {
    }

    public ThreeDViewModel(ISceneModel model)
    {
        _model = model;
        _model.PropertyChanged += OnModelPropertyChanged;
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    [RelayCommand]
    private void ResetCamera()
    {
        _model.ResetCamera();
    }

    #endregion
}
