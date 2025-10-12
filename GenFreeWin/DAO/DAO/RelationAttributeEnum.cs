namespace DAO
{
    public enum RelationAttributeEnum
    {
        dbRelationUnique = 1,
        dbRelationDontEnforce = 2,
        dbRelationInherited = 4,
        dbRelationUpdateCascade = 0x100,
        dbRelationDeleteCascade = 0x1000,
        dbRelationLeft = 0x1000000,
        dbRelationRight = 0x2000000
    }
}
