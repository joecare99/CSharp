using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace VectorGfx2.Models.Interfaces;

public interface IHasPoint
{
    Point P { get; set; }

    IRelayCommand MouseHover { get; set; }
}
