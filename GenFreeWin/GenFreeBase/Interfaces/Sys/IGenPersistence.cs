using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace GenFree.Interfaces.Sys;

public interface IGenPersistence
{
    int GetIntInit(string sSection, long lPos);
    Color[] ReadFarbenInit(string name, int iCnt);
    int[] ReadIntsInit(string sSection, int iCnt=-1);
    int ReadIntInit(string sSection);
    string ReadStringInit(string sSection);
    void ReadStringsInit(string sSection, string[] aus);
    void ReadStringsInit(string sSection, int anz, IList<string> txT);
    int ReadStringsInit(string sSection, IList<string> asValue);
    void WriteEnumInit(string sSection, Enum eValue);
    void WriteIntInit(string sSection, int iValue);
    void WriteStringInit(string sSection, string verz);
    void WriteStringsInit(string sSection, IList<string> asData);
    void ReadBoolsInit(string sSection, IList<bool> axOption);
    void WriteBoolsInit(string sSection, IList<bool> axOption);
    bool ExistFileInit(string v);
    void PutColorsInit(string v, IList<Color> value);
    void PutColorInit(string v, Color value, int iCnt);
    
    void AppendStringsTemp(string sSection, IList<string> lines);
    void WriteStringTemp(string sSection, string text);
    
    int GetIntMand(string sSection, long lPos = 1);
    void PutIntMand(string sSection, ValueType letzte, long lPos);
    void PutIntsMand(string sSection, int[] aiValues);
    int ReadIntMand(string sFilename);
    void WriteIntMand(string sFilename, int iValue);
    void WriteStringMand(string sSection, string sValue);

    string ReadStringMLProg(string sSection, int iMaxLine);
    string ReadStringProg(string sSection);
    void ReadStringsProg(string sSection, string[] aus);
    int[] ReadIntsProg(string sSection, int iCnt);
    void WriteIntsProg(string sSection, int[] aiValues);
    void WriteStringProg(string sFilename, string text);
    void WriteStringsProg(string sSection, string[] values);

    void CopyDirectory(string v, string backupDir);
    
    void CreateTempFile(string v);
    void WriteBoolsTemp(string sSection, IList<bool> Option);
    bool ReadBoolInit(string v);
    void ReadBoolsTemp( string v, IList<bool> Option);
    void ReadStringsOutput(string sSection, IList<string> asValue, int iCount);
    void WriteStringsOutput(string sSection, IList<string> asValue, int iCount);
    void DeleteTempFile(string v);
    bool ExistFileTemp(string v);
    int FileLengthTemp(string v);
    void WriteStringsTemp(string sSection, IList<string> asValue);
    int[] ReadIntsMand(string sSection, int cCnt);
    IList<string> ReadStringsMand(string v1, int v2);
    void ReadStringsTemp(string v, IList<string> asOption);
    string CreateTempFilefromInit(string v);
    bool ExistFile(string v);
    bool ExistFileMand(string dateiName);
}

public static class PersistenceHelper {
    public static T ReadEnumInit<T>(this IGenPersistence _p, string sSection) where T : Enum
        => (T)(object)_p.ReadIntInit(sSection);
    public static void ReadEnumsMand<T>(this IGenPersistence _p, string sSection, T[] enums) where T : Enum
    {
        var ai = _p.ReadIntsMand(sSection, enums.Length);
        for (var i = 0; i < Math.Min(ai.Length, enums.Length) - 1; i++)
            enums[i] = (T)(object)ai[i];
    }
    public static void ReadEnumsMand<T>(this IGenPersistence _p, string sSection, IList<T> enums) where T : Enum
    {
        var ai = _p.ReadIntsMand(sSection, enums.Count);
        for (var i = 0; i<Math.Min(ai.Length, enums.Count) - 1; i++)
            enums[i] = (T) (object) ai[i]; 
    }
    public static void PutEnumsMand<T>(this IGenPersistence _p, string sSection, IList<T> enums) where T : Enum
    {
        _p.PutIntsMand(sSection, enums.Select(e=>e.AsInt()).ToArray());
    }
    public static IList<T> ReadEnumsInit<T>(this IGenPersistence _p, string v) where T: Enum
    {
        return _p.ReadIntsInit(v).Select(i=>(T)(object)i).ToList();         
    }
    public static void ReadSuchDatMand<T>(this IGenPersistence _p, string DateiName, IList<T> aeValues) where T : Enum
    {
        _p.ReadEnumsMand(DateiName, aeValues);
    }
    public static void ReadEnumsInit<T>(this IGenPersistence _p, string sSection, out T eVal) where T : Enum
    { 
        eVal = _p.ReadEnumInit<T>(sSection);
    }

}
