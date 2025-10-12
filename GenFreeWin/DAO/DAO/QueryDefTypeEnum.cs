namespace DAO
{
    public enum QueryDefTypeEnum
    {
        dbQSelect = 0,
        dbQProcedure = 224,
        dbQAction = 240,
        dbQCrosstab = 16,
        dbQDelete = 32,
        dbQUpdate = 48,
        dbQAppend = 64,
        dbQMakeTable = 80,
        dbQDDL = 96,
        dbQSQLPassThrough = 112,
        dbQSetOperation = 128,
        dbQSPTBulk = 144,
        dbQCompound = 160
    }
}
