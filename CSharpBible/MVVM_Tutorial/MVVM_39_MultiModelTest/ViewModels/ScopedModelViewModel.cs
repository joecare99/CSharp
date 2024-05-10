using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MVVM.View.Extension;
using MVVM.ViewModel;
using MVVM_39_MultiModelTest.Models;


namespace MVVM_39_MultiModelTest.ViewModels;

public partial class ScopedModelViewModel: BaseViewModelCT
{
    public string Name { get; }
    public string Description { get; }
    public Guid Id { get; }
    public IServiceScope Scope { get; }

    private readonly IScopedModel _model;

    public int ICommonValue { get => _model?.ICommonValue ?? 0; set => _model.ICommonValue = value; }

    [ObservableProperty]
    private string _FrameName = new Uri($"/{nameof(MVVM_39_MultiModelTest)};component/views/DetailPage1.xaml", UriKind.Relative).ToString();

    public Action<object> DoClose { get; set; } = _ => { };

    public ScopedModelViewModel():this(IoC.GetRequiredService<IScopedModel>())
    {
        /*
        Name = "<Name>";
        Description = "<Description>";
        Id = Guid.Empty;
        Scope = null!;
        */
    }

    public ScopedModelViewModel(IScopedModel model)
    {
        Name = model.Name;
        Description = model.Description;
        Id = model.Id;
        Scope = model.Scope;
        _model= model;
        model.PropertyChanged += (o,e)=>OnPropertyChanged(e);
    }

    [RelayCommand]
    private void Close()
    {
        DoClose.Invoke(this);
    }

    [RelayCommand]
    private void Detail1()
    {
        IoC.SetCurrentScope(Scope);
        Uri resourceLocater = new Uri($"/{nameof(MVVM_39_MultiModelTest)};component/views/DetailPage1.xaml", UriKind.Relative);
        FrameName = resourceLocater.ToString();
    }
    [RelayCommand]
    private void Detail2()
    {
        IoC.SetCurrentScope(Scope);
        Uri resourceLocater = new Uri($"/{nameof(MVVM_39_MultiModelTest)};component/views/DetailPage2.xaml", UriKind.Relative);
        FrameName = resourceLocater.ToString();
    }
    [RelayCommand]
    private void Detail3()
    {
        IoC.SetCurrentScope(Scope);
        Uri resourceLocater = new Uri($"/{nameof(MVVM_39_MultiModelTest)};component/views/DetailPage3.xaml", UriKind.Relative);
        FrameName = resourceLocater.ToString();
    }

}
