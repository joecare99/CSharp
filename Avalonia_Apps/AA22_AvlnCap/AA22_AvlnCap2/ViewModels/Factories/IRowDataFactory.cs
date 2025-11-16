using AA22_AvlnCap2.ViewModels.Interfaces;

namespace AA22_AvlnCap2.ViewModels.Factories
{
    public interface IRowDataFactory
    {
        IRowData Create(int rowId, IWpfCapViewModel parent);
    }
}