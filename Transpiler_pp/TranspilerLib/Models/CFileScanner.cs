using System.Reflection;

namespace TranspilerLib.Models;

public class CFileScanner 
{
    public delegate void TResourceHandler(object? Sender, string aFileName, TStrings aOptions);

    public struct TResourceHandlerRecord
    { 
        string Ext;
        TResourceHandler Handler;
    }
    public struct TWarnMsgNumberState
    {
        int Number;
      //  TWarnMsgState State;
    }
    

}