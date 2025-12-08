using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [CoClass(typeof(UserClass))]
    [Guid("00000069-0000-0010-8000-00AA006D2EA4")]
    public interface User : _User
    {
    }
}
