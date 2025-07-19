using System;
using System.Collections;
using System.Collections.Generic;

namespace GenFree.Interfaces.VB;

public interface IStrings
{
    short Asc(char keyChar);
    short Asc(string keystring);
    char Chr(int num);
    int InStr(string text, string v);
    int Len(string readOnlySpan);
    string Mid(string text, int num11, int length=-1);
    string RTrim(string v);
    string LTrim(string v);
    string Trim(string v);
    string Right(string v,int c);
    string Left(string v, int c);
    void MidStmtStr(ref string datu, int v1, int v2, string v3);
    string Format(object value, string format);

    IList<string> Split(string text, string delimiter, int limit = -1);
    string Space(int v);
    string UCase(string v);
    string Replace(string v1, string v2, string v3);
}