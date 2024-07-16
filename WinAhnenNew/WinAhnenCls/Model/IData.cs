using System;
using System.Runtime.InteropServices;

namespace WinAhnenCls.Model
{
    [Guid("7E8B9348-4B2E-4A6F-B540-F0E9F3717EFC")]
    public interface IData<T> : IDataRO<T> where T : class
    {
        /// <INFO>Fügt einen neuen, leeren Datensatz an.</INFO>
        void Append(object? sender = null);
        void Edit(object? sender = null);
        void Post(object? sender = null);
        void Cancel(object? sender = null);
        void Delete(object? sender = null);
        new T Data { get; set; }
    }

    [Guid("735E17C0-B53F-4999-B668-B11B77EE34BC")]
    public interface IData : IData<object> { }
}