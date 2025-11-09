// ***********************************************************************
// Assembly    : Avln_CustomAnimation
// Author  : Mir
// Created          : 01-15-2025
// ***********************************************************************
using System.ComponentModel;

namespace Avln_CustomAnimation.Models;

public interface IAnimationModel : INotifyPropertyChanged
{
  bool IsAnimating { get; }
}
