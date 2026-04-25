namespace GenFree.Interfaces.DB
{
    public interface IDBWorkSpace
    {
        void Begin();
        void Commit();
        IDatabase CreateDatabase(string sDBName, string? sSQL = null);
        void Rollback();
    }
}