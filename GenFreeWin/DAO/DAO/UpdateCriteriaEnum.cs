namespace DAO
{
    public enum UpdateCriteriaEnum
    {
        dbCriteriaKey = 1,
        dbCriteriaModValues = 2,
        dbCriteriaAllCols = 4,
        dbCriteriaTimestamp = 8,
        dbCriteriaDeleteInsert = 0x10,
        dbCriteriaUpdate = 0x20
    }
}
