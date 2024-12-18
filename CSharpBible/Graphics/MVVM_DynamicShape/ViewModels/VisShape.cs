using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModel;
using MVVM_DynamicShape.Model;

namespace MVVM_DynamicShape.ViewModels;

public partial class VisShape : BaseViewModelCT
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private EShapeType _sType;
}
