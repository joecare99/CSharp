// ***********************************************************************
// Assembly      : AA06_Converters_4
// Author      : Mir
// Created : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CurrencyViewViewModel.cs" company="AA06_Converters_4">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MathLibrary.TwoDim;
using AA06_Converters_4.Model;
using System.ComponentModel;
using System;
using CommunityToolkit.Mvvm.Input;

namespace AA06_Converters_4.ViewModels;

/// <summary>
/// Class CurrencyViewViewModel.
/// Implements the <see cref="ObservableObject" />
/// </summary>
/// <seealso cref="ObservableObject" />
public partial class VehicleViewModel : ObservableObject
{
    /// <summary>
    /// The value
    /// </summary>
    private IAGVModel _agv_Model;
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public double VehicleLength { get => _agv_Model.VehicleDim.x; set => _agv_Model.VehicleDim = new(value, _agv_Model.VehicleDim.y); }
    public double VehicleWidth { get => _agv_Model.VehicleDim.y; set => _agv_Model.VehicleDim = new(_agv_Model.VehicleDim.x, value); }
    public double SwivelKoorX { get => _agv_Model.SwivelKoor.x; set => _agv_Model.SwivelKoor = new(value, _agv_Model.SwivelKoor.y); }
    public double SwivelKoorY { get => _agv_Model.SwivelKoor.y; set => _agv_Model.SwivelKoor = new(_agv_Model.SwivelKoor.x, value); }
    public double AxisOffset { get => _agv_Model.AxisOffset; set => _agv_Model.AxisOffset = value; }

    public double Swivel1Angle { get => _agv_Model.Swivel1Angle; set => _agv_Model.Swivel1Angle = value; }
    public double Wheel1Velocity { get => _agv_Model.Wheel1Velocity; set => _agv_Model.Wheel1Velocity = value; }
    public double Wheel2Velocity { get => _agv_Model.Wheel2Velocity; set => _agv_Model.Wheel2Velocity = value; }
    public double Swivel2Angle { get => _agv_Model.Swivel2Angle; set => _agv_Model.Swivel2Angle = value; }
    public double Wheel3Velocity { get => _agv_Model.Wheel3Velocity; set => _agv_Model.Wheel3Velocity = value; }
    public double Wheel4Velocity { get => _agv_Model.Wheel4Velocity; set => _agv_Model.Wheel4Velocity = value; }

    public double Swivel1Velocity { get => _agv_Model.Swivel1Velocity; }
    public double Swivel2Velocity { get => _agv_Model.Swivel2Velocity; }
    public double Swivel1Rot { get => _agv_Model.Swivel1Rot; }
    public double Swivel2Rot { get => _agv_Model.Swivel2Rot; }
    public double VehicleVelocityX { get => _agv_Model.AGVVelocity.x; }
    public double VehicleVelocityY { get => _agv_Model.AGVVelocity.y; }
    public double VehicleRotation { get => _agv_Model.VehicleRotation; }
    public Math2d.Vector AGVVelocity { get => _agv_Model.AGVVelocity; }

    [RelayCommand]
    private void Save() => _agv_Model.Save();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="VehicleViewModel"/> class.
    /// </summary>
    public VehicleViewModel(IAGVModel model)
 {
        _agv_Model = model;
    _agv_Model.PropertyChanged += OnModelPropChanged;
    }

    ~VehicleViewModel()
    {
 _agv_Model.Save();
    }
    
    private void OnModelPropChanged(object? sender, PropertyChangedEventArgs e) => (_ = e.PropertyName switch
    {
        nameof(IAGVModel.VehicleDim)
 => () => { OnPropertyChanged(nameof(VehicleLength)); OnPropertyChanged(nameof(VehicleWidth)); },
        nameof(IAGVModel.SwivelKoor)
=> () => { OnPropertyChanged(nameof(SwivelKoorX)); OnPropertyChanged(nameof(SwivelKoorY)); },
        string s => () => OnPropertyChanged(s),
    _ => (Action)(() => { })
    })();

    /// <summary>
    /// Gets a value indicating whether [value is not zero].
    /// </summary>
    /// <value><c>true</c> if [value is not zero]; otherwise, <c>false</c>.</value>
    public bool ValueIsNotZero => _agv_Model.VehicleDim.AsComplex != 0;
}
