using MathLibrary.TwoDim;
using Microsoft.Win32;
using MVVM.ViewModel;
using MVVM_06_Converters_4.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_06_Converters_4.Model
{
    public class AGV_Model : NotificationObject, IAGVModel 
    {
        private static AGV_Model? _instance;
        private Math2d.Vector _vehicleDim;
        private Math2d.Vector _swivelKoor;
        private double _axisOffset;
        private double _swivel1Angle;
        private double _swivel2Angle;
        private double _wheel1Velocity;
        private double _wheel2Velocity;
        private double _wheel3Velocity;
        private double _wheel4Velocity;

        public static AGV_Model Instance => _instance ??= new();

        public Math2d.Vector VehicleDim { get => _vehicleDim; set => SetProperty(ref _vehicleDim,value); }
        public Math2d.Vector SwivelKoor { get => _swivelKoor; set => SetProperty(ref _swivelKoor, value); }
        public double AxisOffset { get => _axisOffset; set => SetProperty(ref _axisOffset, value); }
        public double Swivel1Angle { get => _swivel1Angle; set => SetProperty(ref _swivel1Angle, value); }
        public double Wheel1Velocity { get => _wheel1Velocity; set => SetProperty(ref _wheel1Velocity, value); }
        public double Wheel2Velocity { get => _wheel2Velocity; set => SetProperty(ref _wheel2Velocity, value); }
        public double Swivel2Angle { get => _swivel2Angle; set => SetProperty(ref _swivel2Angle, value); }
        public double Wheel3Velocity { get => _wheel3Velocity; set => SetProperty(ref _wheel3Velocity, value); }
        public double Wheel4Velocity { get => _wheel4Velocity; set => SetProperty(ref _wheel4Velocity, value); }
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
            (nameof(Swivel1Velocity),nameof(Wheel2Velocity)),
            (nameof(Swivel2Velocity),nameof(Wheel3Velocity)),
            (nameof(Swivel2Velocity),nameof(Wheel4Velocity)),
            (nameof(Swivel1Rot),nameof(Wheel1Velocity)),
            (nameof(Swivel1Rot),nameof(Wheel2Velocity)),
            (nameof(Swivel2Rot),nameof(Wheel3Velocity)),
            (nameof(Swivel2Rot),nameof(Wheel4Velocity)),
            (nameof(AGVVelocity),nameof(Swivel1Velocity)),
            (nameof(AGVVelocity),nameof(Swivel2Velocity)),
            (nameof(AGVVelocity),nameof(Swivel1Angle)),
            (nameof(AGVVelocity),nameof(Swivel2Angle)),
            (nameof(VehicleRotation),nameof(Swivel1Velocity)),
            (nameof(VehicleRotation),nameof(Swivel2Velocity)),
            (nameof(VehicleRotation),nameof(Swivel1Angle)),
            (nameof(VehicleRotation),nameof(Swivel2Angle)),
        };

        public AGV_Model()
        {
            _vehicleDim = new Math2d.Vector(Settings.Default.Vehicle_Length, Settings.Default.Vehicle_Width);
            _swivelKoor = new Math2d.Vector(Settings.Default.SwivelKoor_X, Settings.Default.SwivelKoor_Y);
            _axisOffset = Settings.Default.AxisOffset; 
        }
        ~AGV_Model()
        {
            Save();
        }

        public void Save()
        {
            Settings.Default.Vehicle_Length = _vehicleDim.x;
            Settings.Default.Vehicle_Width = _vehicleDim.y;
            Settings.Default.SwivelKoor_X = _swivelKoor.x;
            Settings.Default.SwivelKoor_Y = _swivelKoor.y;
            Settings.Default.AxisOffset = _axisOffset;
            Settings.Default.Save();
        }
    }
}
