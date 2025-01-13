namespace GenFree.Interfaces.DB
{
    public interface IDBWorkSpace
    {
        void Begin();
        void Commit();
        IDatabase CreateDatabase(string v1, string v2);
        void Rollback();
    }
}