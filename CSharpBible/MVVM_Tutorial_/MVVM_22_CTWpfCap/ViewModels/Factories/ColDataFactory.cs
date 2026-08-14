using CommunityToolkit.Mvvm.Input;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;

namespace MVVM_22_CTWpfCap.ViewModels.Factories;

public class ColDataFactory : IColDataFactory
{
    public IColData Create(int colId, IWpfCapViewModel parent) => new ColData
    {
        ColId = colId,
        Parent = parent
    };
}