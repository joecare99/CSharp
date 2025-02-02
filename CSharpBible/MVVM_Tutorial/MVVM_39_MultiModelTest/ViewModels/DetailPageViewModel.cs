using Microsoft.Extensions.DependencyInjection;
using MVVM.ViewModel;
using BaseLib.Helper;
using System;
using MVVM_39_MultiModelTest.Models;
//using NSubstitute;

namespace MVVM_39_MultiModelTest.ViewModels;

public partial class DetailPageViewModel :BaseViewModelCT
{
    public string Name { get; }
    public string Description { get; }
    public Guid Id { get; }
    public IServiceScope Scope { get; }

    private readonly IScopedModel _model;

    public int ICommonValue { get => _model?.ICommonValue ?? 0; set => _model.ICommonValue = value; }

    public DetailPageViewModel():this(IoC.GetRequiredService<IScopedModel>())
    {           
     /*   Name = "<Name>";
        Description = "<Description>";
        Id = Guid.Empty;
        Scope = null!;
        _model = Substitute.For<IScopedModel>(); */
    }

    public DetailPageViewModel(IScopedModel model)
    {
        Name = model.Name;
        Description = model.Description;
        Id = model.Id;
        Scope = model.Scope;
        _model = model;
        model.PropertyChanged += (o, e) => OnPropertyChanged(e);
    }
}
