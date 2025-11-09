namespace DAO
{
    public enum RecordsetTypeEnum
    {
        dbOpenTable = 1,
        dbOpenDynaset = 2,
        dbOpenSnapshot = 4,
        dbOpenForwardOnly = 8,
        dbOpenDynamic = 0x10
    }
}
