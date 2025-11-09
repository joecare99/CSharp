using System;
using System.Collections.Generic;

namespace WinAhnenCls.Utils;

// Minimal EventType that matches Pascal etWarning/etError usage in cls_GenHelperBase
public enum EventType
{
    Warning,
    Error
}

// Delegate equivalent to TTMessageEvent/THlprMsgEvent
public delegate void THlprMsgEvent(object sender, EventType type, string text, string reference, int mode);

public abstract class GenHelperBase
{
    private THlprMsgEvent? _onHlpMessage;
    private IList<string>? _citation;
    private string _citTitle = string.Empty;
    private string _osbHdr = string.Empty;

    // Protected fields (to mirror Pascal's protected members)
    protected IList<string>? FCitation => _citation; // Compatibility alias
    protected string FCitRefn = string.Empty;
    protected string FCitTitle => _citTitle; // Compatibility alias
    protected string FOsbHdr => _osbHdr; // Compatibility alias

    public THlprMsgEvent? OnHlpMessage
    {
        get => _onHlpMessage;
        set
        {
            if (_onHlpMessage == value) return;
            _onHlpMessage = value;
        }
    }

    public IList<string>? Citation
    {
        get => _citation;
        set
        {
            if (ReferenceEquals(_citation, value)) return;
            _citation = value;
            FCitRefn = string.Empty;
        }
    }

    public string CitTitle
    {
        get => _citTitle;
        set
        {
            if (_citTitle == value) return;
            _citTitle = value ?? string.Empty;
        }
    }

    public string OsbHdr
    {
        get => _osbHdr;
        set
        {
            if (_osbHdr == value) return;
            _osbHdr = value ?? string.Empty;
        }
    }

    // Helper methods translated from Pascal
    protected string NormalCitRef(string aText)
    {
        if (string.IsNullOrEmpty(aText))
            return string.Empty;

        string lText = (aText.StartsWith("F", StringComparison.Ordinal) || aText.StartsWith("I", StringComparison.Ordinal))
            ? aText.Substring(1)
            : aText;

        // Find first digit
        int lp1 = IndexOfAsciiDigit(lText);
        if (lp1 < 0)
            return lText;

        int i = lp1 + 1;
        while (i < lText.Length && IsAsciiDigit(lText[i]))
            i++;

        int seqLen = i - lp1;
        int pad = 4 - seqLen;
        if (pad > 0)
        {
            return lText.Insert(lp1, new string('0', pad));
        }
        return lText;
    }

    protected void Warning(string aText, string Ref, int aMode)
    {
        _onHlpMessage?.Invoke(this, EventType.Warning, aText ?? string.Empty, Ref ?? string.Empty, aMode);
    }

    protected void Error(string aText, string Ref, int aMode)
    {
        _onHlpMessage?.Invoke(this, EventType.Error, aText ?? string.Empty, Ref ?? string.Empty, aMode);
    }

    public void FireEvent(object sender, string[] aSTa)
    {
        if (aSTa == null || aSTa.Length != 4) return;
        if (!int.TryParse(aSTa[3], out var lInt)) return;

        switch (aSTa[0])
        {
            case "ParserStartIndiv":
                StartIndiv(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserStartFamily":
                StartFamily(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserFamilyType":
                FamilyType(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserFamilyDate":
                FamilyDate(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserFamilyData":
                FamilyData(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserFamilyIndiv":
                FamilyIndiv(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserFamilyPlace":
                FamilyPlace(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiData":
                IndiData(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiDate":
                IndiDate(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiName":
                IndiName(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiOccu":
                IndiOccu(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiPlace":
                IndiPlace(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiRef":
                IndiRef(sender, aSTa[1], aSTa[2], lInt);
                break;
            case "ParserIndiRel":
                IndiRel(sender, aSTa[1], aSTa[2], lInt);
                break;
        }
    }

    // Abstract API matching Pascal class
    public abstract void Clear();
    public abstract void StartFamily(object sender, string aText, string aRef, int subType);
    public abstract void StartIndiv(object sender, string aText, string aRef, int subType);
    public abstract void FamilyIndiv(object sender, string aText, string aRef, int subType);
    public abstract void FamilyType(object sender, string aText, string aRef, int subType);
    public abstract void FamilyDate(object sender, string aText, string aRef, int subType);
    public abstract void FamilyData(object sender, string aText, string aRef, int subType);
    public abstract void FamilyPlace(object sender, string aText, string aRef, int subType);
    public abstract void IndiData(object sender, string aText, string aRef, int subType);
    public abstract void IndiDate(object sender, string aText, string aRef, int subType);
    public abstract void IndiName(object sender, string aText, string aRef, int subType);
    public abstract void IndiPlace(object sender, string aText, string aRef, int subType);
    public abstract void IndiRef(object sender, string aText, string aRef, int subType);
    public abstract void IndiOccu(object sender, string aText, string aRef, int subType);
    public abstract void IndiRel(object sender, string aText, string aRef, int subType);
    public abstract void EndOfEntry(object sender, string aText, string aRef, int subType);
    public abstract void CreateNewHeader(string filename);
    public abstract void SaveToFile(string filename);

    // Local helpers
    private static int IndexOfAsciiDigit(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (IsAsciiDigit(s[i])) return i;
        }
        return -1;
    }

    private static bool IsAsciiDigit(char c) => c >= '0' && c <= '9';
}