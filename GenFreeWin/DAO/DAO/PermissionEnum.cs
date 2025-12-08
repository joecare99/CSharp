namespace DAO
{
    public enum PermissionEnum
    {
        dbSecNoAccess = 0,
        dbSecFullAccess = 1048575,
        dbSecDelete = 65536,
        dbSecReadSec = 131072,
        dbSecWriteSec = 262144,
        dbSecWriteOwner = 524288,
        dbSecDBCreate = 1,
        dbSecDBOpen = 2,
        dbSecDBExclusive = 4,
        dbSecDBAdmin = 8,
        dbSecCreate = 1,
        dbSecReadDef = 4,
        dbSecWriteDef = 65548,
        dbSecRetrieveData = 20,
        dbSecInsertData = 32,
        dbSecReplaceData = 64,
        dbSecDeleteData = 128
    }
}
