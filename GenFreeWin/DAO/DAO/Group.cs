using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [CoClass(typeof(GroupClass))]
    [Guid("00000061-0000-0010-8000-00AA006D2EA4")]
    public interface Group : _Group
    {
    }
}
