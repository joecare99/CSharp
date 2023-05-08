// ***********************************************************************
// Assembly         : MVVM_6_Converters_4
// Author           : Mir
// Created          : 07-03-2022
//
// Last Modified By : Mir
// Last Modified On : 07-04-2022
// ***********************************************************************
// <copyright file="CurrencyViewViewModel.cs" company="MVVM_6_Converters_4">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using MathLibrary.TwoDim;
using MVVM_6_Converters_4.Properties;

namespace MVVM_6_Converters_4.ViewModel
{
    /// <summary>
    /// Class CurrencyViewViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public class VehicleViewModel : BaseViewModel
    {
        /// <summary>
        /// The value
        /// </summary>
        private Math2d.Vector _vehicleDim;
        private Math2d.Vector _swivelKoor;
        private double _axisOffset;
        private double _swivel1Angle;
        private double _wheel1Velocity;
        private double _wheel2Velocity;

        private double _swivel2Angle;
        private double _wheel3Velocity;
        private double _wheel4Velocity;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double VehicleLength { get => _vehicleDim.x; set => ExecPropSetter((v)=>_vehicleDim.x=v, _vehicleDim.x, value); }
        public double VehicleWidth { get => _vehicleDim.y; set => ExecPropSetter((v) => _vehicleDim.y = v, _vehicleDim.y, value); }
        public double SwivelKoorX { get => _swivelKoor.x; set => ExecPropSetter((v) => _swivelKoor.x = v, _swivelKoor.x, value); }
        public double SwivelKoorY { get => _swivelKoor.y; set => ExecPropSetter((v) => _swivelKoor.y = v, _swivelKoor.y, value); }
        public double AxisOffset { get => _axisOffset; set => SetProperty(ref _axisOffset, value); }

        public double Swivel1Angle { get => _swivel1Angle; set => SetProperty(ref _swivel1Angle, value); }
        public double Wheel1Velocity { get => _wheel1Velocity; set => SetProperty(ref _wheel1Velocity, value); }
        public double Wheel2Velocity { get => _wheel2Velocity; set => SetProperty(ref _wheel2Velocity, value); }
        public double Swivel2Angle { get => _swivel2Angle; set => SetProperty(ref _swivel2Angle, value); }
        public double Wheel3Velocity { get => _wheel3Velocity; set => SetProperty(ref _wheel3Velocity, value); }
        public double Wheel4Velocity { get => _wheel4Velocity; set => SetProperty(ref _wheel4Velocity, value); }

        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleViewModel"/> class.
        /// </summary>
        public VehicleViewModel()
        {
            _vehicleDim = new Math2d.Vector(Settings.Default.Vehicle_Length, Settings.Default.Vehicle_Width);
            _swivelKoor = new Math2d.Vector(Settings.Default.SwivelKoor_X, Settings.Default.SwivelKoor_Y);
        }

        /// <summary>
        /// Gets a value indicating whether [value is not zero].
        /// </summary>
        /// <value><c>true</c> if [value is not zero]; otherwise, <c>false</c>.</value>
        public bool ValueIsNotZero => _vehicleDim.AsComplex != 0;
    }
}
