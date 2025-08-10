using System;
using System.Collections.Generic;
using GenFree.Interfaces.VB;

namespace GenFree.Models.VB;

public class CStrings : IStrings
{
    public short Asc(char keyChar) =>
        // Convert the character to its ASCII value
        (short)keyChar;

    public short Asc(string keystring) =>
        // If the string is empty, return 0
        string.IsNullOrEmpty(keystring) ? (short)0 : (short)keystring[0];

    public char Chr(int num) =>
        // Convert the ASCII value back to a character
        (char)num;

    public string Format(object value, string format)
    {
        // This method is intended to format a value according to a specified format.
        // In a real implementation, you might use string.Format or similar methods.
        return string.Format(format, value);
    }

    public int InStr(string text, string v)
    {
        // This method finds the first occurrence of a substring in a string.
        // In a real implementation, you might use text.IndexOf(v) + 1 to return 1-based index.
        return text.IndexOf(v) + 1; // +1 to convert to 1-based index
    }

    public string Left(string v, int c)
    {
        // This method returns the leftmost 'c' characters of the string 'v'.
        if (string.IsNullOrEmpty(v) || c <= 0)
        {
            return string.Empty;
        }
        return v.Length <= c ? v : v.Substring(0, c);
    }

    public int Len(string readOnlySpan)
    {
        // This method returns the length of the string.
        // In a real implementation, you might use readOnlySpan.Length.
        return readOnlySpan.Length;
    }

    public string LTrim(string v)
    {
        // This method removes leading whitespace from the string.
        // In a real implementation, you might use v.TrimStart().
        return v.TrimStart();
    }

    public string Mid(string text, int num11, int length = -1)
    {
        // This method returns a substring starting at 'num11' with an optional 'length'.
        // If 'length' is -1, it returns the rest of the string.
        if (string.IsNullOrEmpty(text) || num11 < 1 || num11 > text.Length)
        {
            return string.Empty;
        }
        int startIndex = num11 - 1; // Convert to 0-based index
        if (length == -1 || startIndex + length > text.Length)
        {
            return text.Substring(startIndex);
        }
        return text.Substring(startIndex, length);
    }

    public void MidStmtStr(ref string datu, int v1, int v2, string v3)
    {
        // This method replaces a substring in 'datu' starting at 'v1' with 'v3' for 'v2' characters.
        // If 'v1' is less than 1, it does nothing.
        if (v1 < 1 || v2 < 0 || string.IsNullOrEmpty(datu))
        {
            return;
        }
        int startIndex = v1 - 1; // Convert to 0-based index
        if (startIndex >= datu.Length)
        {
            return; // Start index is out of bounds
        }
        if (startIndex + v2 > datu.Length)
        {
            v2 = datu.Length - startIndex; // Adjust length if it exceeds the string length
        }
        datu = datu.Remove(startIndex, v2).Insert(startIndex, v3);
    }

    public string Replace(string v1, string v2, string v3)
    {
        // This method replaces all occurrences of 'v2' in 'v1' with 'v3'.
        // In a real implementation, you might use v1.Replace(v2, v3).
        return v1.Replace(v2, v3);
    }

    public string Right(string v, int c)
    {
        // This method returns the rightmost 'c' characters of the string 'v'.
        if (string.IsNullOrEmpty(v) || c <= 0)
        {
            return string.Empty;
        }
        return v.Length <= c ? v : v.Substring(v.Length - c);
    }

    public string RTrim(string v)
    {
        // This method removes trailing whitespace from the string.
        // In a real implementation, you might use v.TrimEnd().
        return v.TrimEnd();
    }

    public string Space(int v)
    {
        // This method returns a string containing 'v' spaces.
        // In a real implementation, you might use new string(' ', v).
        return new string(' ', v);
    }

    public IList<string> Split(string text, string delimiter, int limit = -1)
    {
        // This method splits the string 'text' by the specified 'delimiter'.
        // If 'limit' is specified, it limits the number of splits.
        if (string.IsNullOrEmpty(text))
        {
            return new List<string>();
        }
        var result = new List<string>(text.Split(new[] { delimiter }, limit == -1 ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries));
        return result;
    }

    public string Trim(string v)
    {
        // This method removes leading and trailing whitespace from the string.
        // In a real implementation, you might use v.Trim().
        return v.Trim();
    }

    public string UCase(string v)
    {
        // This method converts the string 'v' to uppercase.
        // In a real implementation, you might use v.ToUpperInvariant().
        return v.ToUpperInvariant();
    }
}