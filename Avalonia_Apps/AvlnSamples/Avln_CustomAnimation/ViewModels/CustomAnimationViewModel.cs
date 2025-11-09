// ***********************************************************************
// Assembly  : Avln_CustomAnimation
// Author  : Mir
// Created          : 01-15-2025
// ***********************************************************************
using Avln_CustomAnimation.Models;
using Avalonia.ViewModels;
using System;
using System.ComponentModel;

namespace Avln_CustomAnimation.ViewModels;

public partial class CustomAnimationViewModel : BaseViewModelCT
{
    public static Func<IAnimationModel> GetModel { get; set; } = () => new AnimationModel();

    private readonly IAnimationModel _model;

    public bool IsAnimating => _model.IsAnimating;

    public CustomAnimationViewModel() : this(GetModel())
    {
    }

    public CustomAnimationViewModel(IAnimationModel model)
    {
        _model = model;
  _model.PropertyChanged += OnModelPropertyChanged;
    }

    private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      OnPropertyChanged(e.PropertyName);
  }
}
