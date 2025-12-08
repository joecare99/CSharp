using MVVM.ViewModel;
using MVVM_DynamicShape.Models.Interfaces;

namespace MVVM_DynamicShape.Models;

public class DynamicShapeModel : IDynamicShapeModel
{
    public IObservableCollection<IShape> Shapes { get; set ; }
}
