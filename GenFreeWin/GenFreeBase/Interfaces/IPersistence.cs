using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces
{
    public interface IPersistence
    {
        void AppendStringsTemp(IList<string> lines, string sSection);
        void GetEnumsMand<T>(string sSection, T[] enums) where T : Enum;
        int GetIntInit(string sSection, long lPos);
        int GetIntMand(string sSection, long lPos = 1);
        void PutEnumsMand<T>(string sSection, T[] enums) where T : Enum;
        void PutIntMand(string sSection, ValueType letzte, long lPos);
        void PutIntsMand(string sSection, int[] aiValues);
        void ReadEnumsInit<T>(string sSection, out T eVal) where T : Enum;
        Color[] ReadFarbenInit(string name, int iCnt);
        int ReadIntInit(string sSection);
        int ReadIntMand(string sFilename);
        int[] ReadIntsInit(string sSection, int iCnt);
        string ReadStringInit(string sSection);
        string ReadStringMLProg(string sSection, int iMaxLine);
        string ReadStringProg(string sSection);
        void ReadStringsInit(string sSection, string[] aus);
        int ReadStringsInit(string sSection, IList<string> asValue);
        void ReadSuchDat<T>(string DateiName, T[] aeValues) where T : Enum;
        void WriteEnumInit(string sSection, object eValue);
        void WriteIntInit(string sSection, int iValue);
        void WriteIntMand(string sFilename, int iValue);
        void WriteIntsProg(string sSection, int[] aiValues);
        void WriteStringInit(string sSection, string verz);
        void WriteStringMand(string sSection, string sValue);
        void WriteStringProg(string sFilename, string text);
        void WriteStringsInit(string sSection, string[] asData);
        void WriteStringTemp(string sSection, string text);
    }
}
