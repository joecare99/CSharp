using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace VectorGfx.Models.Interfaces;

public interface IHasPoint
{
    Point P { get; set; }

    IRelayCommand MouseHover { get; set; }
}