using System;
using System.Collections.Generic;
using System.Drawing;

namespace GenFree.Interfaces;

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
    int ReadStringsInit(string sSection, IList<string> asValue);
    void WriteEnumInit(string sSection, Enum eValue);
    void WriteIntInit(string sSection, int iValue);
    void WriteStringInit(string sSection, string verz);
    void WriteStringsInit(string sSection, string[] asData);
    
    void AppendStringsTemp(IList<string> lines, string sSection);
    void WriteStringTemp(string sSection, string text);
    
    void GetEnumsMand<T>(string sSection, T[] enums) where T : Enum;
    int GetIntMand(string sSection, long lPos = 1);
    void PutEnumsMand<T>(string sSection, T[] enums) where T : Enum;
    void PutIntMand(string sSection, ValueType letzte, long lPos);
    void PutIntsMand(string sSection, int[] aiValues);
    int ReadIntMand(string sFilename);
    void WriteIntMand(string sFilename, int iValue);
    void WriteStringMand(string sSection, string sValue);
    void ReadSuchDat<T>(string DateiName, T[] aeValues) where T : Enum;

    string ReadStringMLProg(string sSection, int iMaxLine);
    string ReadStringProg(string sSection);
    void ReadStringsProg(string sSection, string[] aus);
    int[] ReadIntsProg(string sSection, int iCnt);
    void WriteIntsProg(string sSection, int[] aiValues);
    void WriteStringProg(string sFilename, string text);
    void WriteStringsProg(string sSection, string[] values);

}
