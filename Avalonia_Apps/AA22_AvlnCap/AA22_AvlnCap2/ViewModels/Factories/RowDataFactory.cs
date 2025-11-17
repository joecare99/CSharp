using CommunityToolkit.Mvvm.Input;
using AA22_AvlnCap2.ViewModels.Interfaces;

namespace AA22_AvlnCap2.ViewModels.Factories;

public class RowDataFactory : IRowDataFactory
{
    public IRowData Create(int rowId, IWpfCapViewModel parent) => new RowData
    {
        RowId = rowId,
        Parent = parent
    };
}