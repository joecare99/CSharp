using AA22_AvlnCap2.ViewModels.Interfaces;

namespace AA22_AvlnCap2.ViewModels.Factories;

public interface IColDataFactory
{
    IColData Create(int colId, IWpfCapViewModel parent);
}