using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [CoClass(typeof(RelationClass))]
    [Guid("00000089-0000-0010-8000-00AA006D2EA4")]
    public interface Relation : _Relation
    {
    }
}
