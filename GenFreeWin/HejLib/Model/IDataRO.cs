using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace WinAhnenCls.Model
{
    [Guid("735E17C0-B53F-4999-B668-B11B77EE34BB")]
    /// <INFO>GetData liefert einen (den aktuellen) kompletten Datensatz.</INFO>
    public interface IDataRO<T> where T : class
    {
        T GetData();
        void First(object? sender = null);
        void Last(object? sender = null);
        void Next(object? sender = null);
        void Previous(object? sender = null);
        void Seek(int Id);
        bool EOF();
        bool BOF();
        int GetActID();

        T Data { get; }
        NotifyCollectionChangedEventHandler OnUpdate { get; set; }
    }

    [Guid("7E8B9348-4B2E-4A6F-B540-F0E9F3717EFC")]
    public interface IDataRO : IDataRO<object> { }

}