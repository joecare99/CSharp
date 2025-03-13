using System.Collections.Generic;
using System.IO;

namespace WinAhnenCls.Model.GenBase;

public interface IReader<T>
{
    IEnumerable<T> Read(string path);
    IEnumerable<T> Read(Stream stream);
    IEnumerable<T> Read(StreamReader reader);
}