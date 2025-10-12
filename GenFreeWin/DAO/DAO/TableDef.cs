using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000049-0000-0010-8000-00AA006D2EA4")]
    [CoClass(typeof(TableDefClass))]
    public interface TableDef : _TableDef
    {
    }
}
