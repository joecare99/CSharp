using System;
using System.Runtime.InteropServices;

namespace WinAhnenCls.Model.GenBase
{
    [Guid("1197F8EE-0339-47CC-AFAB-F78B9FF280A8")]
    public interface IGenData
    {
        string Data { get; set; }

        // Management-Properies      
        DateTime? LastChange { get; }
        object This { get; }
    }
}
