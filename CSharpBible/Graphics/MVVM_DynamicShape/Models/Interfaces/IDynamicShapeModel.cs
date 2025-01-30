using MVVM.ViewModel;

namespace MVVM_DynamicShape.Models.Interfaces;

public interface IDynamicShapeModel
{
    IObservableCollection<IShape> Shapes { get; set; }
}