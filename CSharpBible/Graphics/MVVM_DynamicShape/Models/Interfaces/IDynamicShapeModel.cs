using MVVM.ViewModel;
using System.Collections;
using System.Collections.Generic;

namespace MVVM_DynamicShape.Models.Interfaces;

public interface IDynamicShapeModel
{
    IObservableCollection<IShape> Shapes { get; set; }
}