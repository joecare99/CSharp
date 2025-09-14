using GenFree.Data;
using System;
using System.Data;
using System.Data.Common;

namespace GenFree.Interfaces.DB
{
    public interface IDatabase
    {
        DbConnection Connection { get; }
        bool IsOpen { get; }

        void Close();
        int Execute(string v, object? value = null);
        int Execute(string sql);
        DataTable GetSchema(string v, params string?[] strings);
        IRecordset OpenRecordset(string v, object value, object missing, object missing1);
        IRecordset OpenRecordset(string v, RecordsetTypeEnum dbOpenTable = RecordsetTypeEnum.dbOpenQuery);
        IRecordset OpenRecordset(Enum eTable, RecordsetTypeEnum dbOpenTable = RecordsetTypeEnum.dbOpenTable);
    }
}
