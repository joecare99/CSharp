// ***********************************************************************
// Assembly  : Avln_CustomAnimation
// Author           : Mir
// Created: 01-15-2025
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avln_CustomAnimation.Models;

public partial class AnimationModel : ObservableObject, IAnimationModel
{
   [ObservableProperty]
    private bool _isAnimating;
}
