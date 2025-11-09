namespace DAO
{
    public enum CursorDriverEnum
    {
        dbUseDefaultCursor = -1,
        dbUseODBCCursor = 1,
        dbUseServerCursor = 2,
        dbUseClientBatchCursor = 3,
        dbUseNoCursor = 4
    }
}
