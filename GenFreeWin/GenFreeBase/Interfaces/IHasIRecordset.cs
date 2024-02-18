//using DAO;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces
{
    public interface IHasIRecordset
    {
        void Delete();
        void FillData(IRecordset dB_Table);
        void SetDBValue(IRecordset dB_Table, Enum[]? asProps);
    }
}