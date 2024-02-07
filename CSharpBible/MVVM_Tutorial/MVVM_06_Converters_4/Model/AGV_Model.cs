using CommunityToolkit.Mvvm.ComponentModel;
using MathLibrary.TwoDim;
using MVVM.ViewModel;
using MVVM_06_Converters_4.Properties;
using System;
using System.Collections.Generic;

namespace MVVM_06_Converters_4.Model
{
    public partial class AGV_Model : NotificationObjectCT, IAGVModel 
    {
        //  private static AGV_Model? _instance;
        [ObservableProperty]
        private Math2d.Vector _vehicleDim;
        [ObservableProperty]
        private Math2d.Vector _swivelKoor;
        [ObservableProperty]
        private double _axisOffset;
        [ObservableProperty]
        private double _swivel1Angle;
        [ObservableProperty]
        private double _swivel2Angle;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Swivel1Velocity))]
        [NotifyPropertyChangedFor(nameof(Swivel1Rot))]
        [NotifyPropertyChangedFor(nameof(AGVVelocity))]
        [NotifyPropertyChangedFor(nameof(VehicleRotation))]
        private double _wheel1Velocity;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Swivel1Velocity))]
        [NotifyPropertyChangedFor(nameof(Swivel1Rot))]
        [NotifyPropertyChangedFor(nameof(AGVVelocity))]
        [NotifyPropertyChangedFor(nameof(VehicleRotation))]
        private double _wheel2Velocity;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Swivel2Velocity))]
        [NotifyPropertyChangedFor(nameof(Swivel2Rot))]
        [NotifyPropertyChangedFor(nameof(AGVVelocity))]
        [NotifyPropertyChangedFor(nameof(VehicleRotation))]
        private double _wheel3Velocity;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Swivel2Velocity))]
        [NotifyPropertyChangedFor(nameof(Swivel2Rot))]
        [NotifyPropertyChangedFor(nameof(AGVVelocity))]
        [NotifyPropertyChangedFor(nameof(VehicleRotation))]
        private double _wheel4Velocity;

//        public static AGV_Model Instance => _instance ??= new();

        public double Swivel1Velocity => (Wheel1Velocity + Wheel2Velocity) * 0.5d;
        public double Swivel2Velocity => (Wheel3Velocity + Wheel4Velocity) * 0.5d;
        public double Swivel1Rot => (Wheel2Velocity - Wheel1Velocity) / AxisOffset;
        public double Swivel2Rot => (Wheel4Velocity - Wheel3Velocity) / AxisOffset;
        public Math2d.Vector AGVVelocity { get => Math2d.ByLengthAngle(Swivel1Velocity,Swivel1Angle)
                .Add(Math2d.ByLengthAngle(Swivel2Velocity, Swivel2Angle))
                .Mult(0.5d); }

        public double VehicleRotation => Math2d.ByLengthAngle(Swivel1Velocity, Swivel1Angle)
            .Subtract(Math2d.ByLengthAngle(Swivel2Velocity, Swivel2Angle))
            .Mult(SwivelKoor.Rot90())
            / (SwivelKoor.Length()*SwivelKoor.Length()*2);

        public IEnumerable<(string, string)> Dependencies => new[]{
            (nameof(Swivel1Velocity),nameof(Wheel1Velocity)),
            //(nameof(Swivel1Velocity),nameof(Wheel2Velocity)),
            //(nameof(Swivel2Velocity),nameof(Wheel3Velocity)),
            //(nameof(Swivel2Velocity),nameof(Wheel4Velocity)),
            //(nameof(Swivel1Rot),nameof(Wheel1Velocity)),
            //(nameof(Swivel1Rot),nameof(Wheel2Velocity)),
            //(nameof(Swivel2Rot),nameof(Wheel3Velocity)),
            //(nameof(Swivel2Rot),nameof(Wheel4Velocity)),
            //(nameof(AGVVelocity),nameof(Swivel1Velocity)),
            //(nameof(AGVVelocity),nameof(Swivel2Velocity)),
            //(nameof(AGVVelocity),nameof(Swivel1Angle)),
            //(nameof(AGVVelocity),nameof(Swivel2Angle)),
            //(nameof(VehicleRotation),nameof(Swivel1Velocity)),
            //(nameof(VehicleRotation),nameof(Swivel2Velocity)),
            //(nameof(VehicleRotation),nameof(Swivel1Angle)),
            //(nameof(VehicleRotation),nameof(Swivel2Angle)),
        };

        public AGV_Model()
        {
            _vehicleDim = new Math2d.Vector(Settings.Default.Vehicle_Length, Settings.Default.Vehicle_Width);
            _swivelKoor = new Math2d.Vector(Settings.Default.SwivelKoor_X, Settings.Default.SwivelKoor_Y);
            _axisOffset = Settings.Default.AxisOffset; 
        }
#if NET5_0_OR_GREATER
#else
        ~AGV_Model()
        {
            Save();
        }
#endif
        public void Save()
        {
            Settings.Default.Vehicle_Length = VehicleDim.x;
            Settings.Default.Vehicle_Width = VehicleDim.y;
            Settings.Default.SwivelKoor_X = SwivelKoor.x;
            Settings.Default.SwivelKoor_Y = SwivelKoor.y;
            Settings.Default.AxisOffset = AxisOffset;
            Settings.Default.Save();
            
        }
    }
}
