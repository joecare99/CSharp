using MVVM_DynamicShape.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_DynamicShape.Models.Interfaces;

public interface IShape
{
    EShapeType ShapeType { get; }
    string Name { get; }
    public double X { get; set; }
    public double Y { get; set; }

}
