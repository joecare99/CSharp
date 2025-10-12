using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [CoClass(typeof(DBEngineClass))]
    [Guid("00000021-0000-0010-8000-00AA006D2EA4")]
    public interface DBEngine : _DBEngine
    {
    }
}
