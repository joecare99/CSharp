using MVVM_DynamicShape.Model;

namespace MVVM_DynamicShape.Models.Interfaces;

public interface IShape
{
    EShapeType ShapeType { get; }
    string Name { get; }
    public double X { get; set; }
    public double Y { get; set; }

}
