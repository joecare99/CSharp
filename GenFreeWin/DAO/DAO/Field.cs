using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000051-0000-0010-8000-00AA006D2EA4")]
    [CoClass(typeof(FieldClass))]
    public interface Field : _Field
    {
    }
}
