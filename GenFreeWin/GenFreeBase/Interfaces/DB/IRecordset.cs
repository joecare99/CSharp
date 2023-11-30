//using DAO;

using GenFree.Data;

namespace GenFree.Interfaces.DB;
public interface IRecordset
{
    RecordsetTypeEnum _Type { get; }
    bool BOF { get; }
    int EditMode { get; }
    bool EOF { get; }
    IFieldsCollection Fields { get; }
    string Index { get; set; }
    string Name { get; }
    bool NoMatch { get; }
    int RecordCount { get; }

    void AddNew();
    void Close();
    void Delete();
    void Edit();
    void FindFirst(string v);
    void MoveFirst();
    void MoveLast();
    void MoveNext();
    void MovePrevious();
    void Seek(string v, params object[] param);
    void Update();
}