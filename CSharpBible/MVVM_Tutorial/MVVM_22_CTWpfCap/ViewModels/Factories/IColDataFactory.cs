using MVVM_22_CTWpfCap.ViewModels.Interfaces;

namespace MVVM_22_CTWpfCap.ViewModels.Factories;

public interface IColDataFactory
{
    IColData Create(int colId, IWpfCapViewModel parent);
}