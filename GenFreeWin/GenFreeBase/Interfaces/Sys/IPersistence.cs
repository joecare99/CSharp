using System;
using System.Collections.Generic;
using System.Drawing;

namespace GenFree.Interfaces.Sys;

public interface IPersistence
{
    T GetEnumInit<T>(string sSection) where T : Enum;
    int GetIntInit(string sSection, long lPos);
    void ReadEnumsInit<T>(string sSection, out T eVal) where T : Enum;
    Color[] ReadFarbenInit(string name, int iCnt);
    int[] ReadIntsInit(string sSection, int iCnt);
    int ReadIntInit(string sSection);
    string ReadStringInit(string sSection);
    void ReadStringsInit(string sSection, string[] aus);
    void ReadStringsInit(string sSection, int anz, IList<string> txT);
    int ReadStringsInit(string sSection, IList<string> asValue);
    void WriteEnumInit(string sSection, Enum eValue);
    void WriteIntInit(string sSection, int iValue);
    void WriteStringInit(string sSection, string verz);
    void WriteStringsInit(string sSection, IList<string> asData);
    IList<T> ReadEnumsInit<T>(string v);
    void ReadBoolsInit(string sSection, IList<bool> axOption);
    void WriteBoolsInit(string sSection, IList<bool> axOption);
    bool ExistFileInit(string v);
    void PutColorsInit(string v, IList<Color> value);
    void PutColorInit(string v, Color value, int iCnt);
    
    void AppendStringsTemp(IList<string> lines, string sSection);
    void WriteStringTemp(string sSection, string text);
    
    void GetEnumsMand<T>(string sSection, T[] enums) where T : Enum;
    void GetEnumsMand<T>(string sSection, IList<T> enums) where T : Enum;
    int GetIntMand(string sSection, long lPos = 1);
    void PutEnumsMand<T>(string sSection, IList<T> enums) where T : Enum;
    void PutIntMand(string sSection, ValueType letzte, long lPos);
    void PutIntsMand(string sSection, int[] aiValues);
    int ReadIntMand(string sFilename);
    void WriteIntMand(string sFilename, int iValue);
    void WriteStringMand(string sSection, string sValue);
    void ReadSuchDatMand<T>(string DateiName, IList<T> aeValues) where T : Enum;

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
}
