using MathLibrary.TwoDim;
using System.Collections.Generic;
using System.ComponentModel;

namespace MVVM_06_Converters_4.Model;

public interface IAGVModel:INotifyPropertyChanged
{
    Math2d.Vector VehicleDim { get; set; }
    Math2d.Vector SwivelKoor { get; set; }
    double AxisOffset { get; set; }
    double Swivel1Angle { get; set; }
    double Wheel1Velocity { get; set; }
    double Wheel2Velocity { get; set; }
    double Swivel1Velocity { get; }

    double Swivel2Angle { get; set; }
    double Wheel3Velocity { get; set; }
    double Wheel4Velocity { get; set; }
    double Swivel2Velocity { get; }
    IEnumerable<(string Dest, string Src)> Dependencies { get; }
    Math2d.Vector AGVVelocity { get; }
    double VehicleRotation { get; }
    double Swivel1Rot { get; }
    double Swivel2Rot { get; }

    void Save();
}