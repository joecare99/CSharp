using System.Collections.ObjectModel;
using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_DynamicShape.Model;
using MVVM_DynamicShape.Models.Interfaces;

namespace MVVM_DynamicShape.ViewModels;

public partial class DynamicShapeViewModel(IDynamicShapeModel iModel) : BaseViewModelCT
{
    IDynamicShapeModel _iModel { get; } = iModel;


    #region Property

    /// <summary>
    /// The shape1
    /// </summary>
    [ObservableProperty]
    private EShapeType _Shape = EShapeType.None;

    [ObservableProperty]
    private string _sText = "";

    [ObservableProperty]
    private ObservableCollection<VisShape> _visShapes = new();
    #endregion

    #region Methode
    public DynamicShapeViewModel() : this(IoC.GetRequiredService<IDynamicShapeModel>())
    {
    }

    [RelayCommand]
    public void Shape1()
    {
        Shape = EShapeType.Rectangle;
    }
    [RelayCommand]
    public void Shape2()
    {
        Shape = EShapeType.Circle;
    }
    [RelayCommand]
    public void MouseLeftButtonDown(object? sender)
    {
        // Create the selected shape
    }
    [RelayCommand]
    public void MouseRightButtonDown(object? sender)
    {
        // Delete the selected shape
    }
    #endregion

}
