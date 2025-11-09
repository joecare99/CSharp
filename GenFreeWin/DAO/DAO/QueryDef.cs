using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000079-0000-0010-8000-00AA006D2EA4")]
    [CoClass(typeof(QueryDefClass))]
    public interface QueryDef : _QueryDef
    {
    }
}
