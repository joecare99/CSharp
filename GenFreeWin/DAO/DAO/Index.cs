using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [CoClass(typeof(IndexClass))]
    [Guid("00000059-0000-0010-8000-00AA006D2EA4")]
    public interface Index : _Index
    {
    }
}
