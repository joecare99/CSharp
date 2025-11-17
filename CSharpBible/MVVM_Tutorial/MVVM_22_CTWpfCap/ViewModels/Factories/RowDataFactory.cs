using CommunityToolkit.Mvvm.Input;
using MVVM_22_CTWpfCap.ViewModels.Interfaces;

namespace MVVM_22_CTWpfCap.ViewModels.Factories;

public class RowDataFactory : IRowDataFactory
{
    public IRowData Create(int rowId, IWpfCapViewModel parent) => new RowData
    {
        RowId = rowId,
        Parent = parent
    };
}