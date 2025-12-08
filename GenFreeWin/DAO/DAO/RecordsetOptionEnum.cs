namespace DAO
{
    public enum RecordsetOptionEnum
    {
        dbDenyWrite = 1,
        dbDenyRead = 2,
        dbReadOnly = 4,
        dbAppendOnly = 8,
        dbInconsistent = 0x10,
        dbConsistent = 0x20,
        dbSQLPassThrough = 0x40,
        dbFailOnError = 0x80,
        dbForwardOnly = 0x100,
        dbSeeChanges = 0x200,
        dbRunAsync = 0x400,
        dbExecDirect = 0x800
    }
}
