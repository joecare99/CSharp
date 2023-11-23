using GenFree.Data;
using System.Data;
using System.Data.Common;

namespace GenFree.Interfaces.DB
{
    public interface IDatabase
    {
        DbConnection Connection { get; }

        void Close();
        void Execute(string v, object? value = null);
        void Execute(string sql);
        DataTable GetSchema(string v, params string?[] strings);
        IRecordset OpenRecordset(string v, object value, object missing, object missing1);
        IRecordset OpenRecordset(string v, RecordsetTypeEnum dbOpenTable = RecordsetTypeEnum.dbOpenQuery);
    }
}
