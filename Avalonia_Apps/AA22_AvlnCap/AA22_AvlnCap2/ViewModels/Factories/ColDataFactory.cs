using CommunityToolkit.Mvvm.Input;
using AA22_AvlnCap2.ViewModels.Interfaces;

namespace AA22_AvlnCap2.ViewModels.Factories;

public class ColDataFactory : IColDataFactory
{
    public IColData Create(int colId, IWpfCapViewModel parent) => new ColData
    {
        ColId = colId,
        Parent = parent
    };
}