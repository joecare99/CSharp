using MVVM_22_CTWpfCap.ViewModels.Interfaces;

namespace MVVM_22_CTWpfCap.ViewModels.Factories
{
    public interface IRowDataFactory
    {
        IRowData Create(int rowId, IWpfCapViewModel parent);
    }
}