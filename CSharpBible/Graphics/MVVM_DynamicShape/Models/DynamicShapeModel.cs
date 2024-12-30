using MVVM.ViewModel;
using MVVM_DynamicShape.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_DynamicShape.Models;

public class DynamicShapeModel : IDynamicShapeModel
{
    public IObservableCollection<IShape> Shapes { get; set ; }
}
