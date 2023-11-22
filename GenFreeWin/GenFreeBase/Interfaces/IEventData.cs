﻿using GenFree.Data;
using GenFree.Interfaces.DB;
using System;

namespace GenFree.Interfaces;

public interface IEventData
{
    DateTime dDatumB { get; }
    DateTime dDatumV { get; }
    EEventArt eArt { get; }
    int iAn { get; }
    int iArtText { get; }
    int iCausal { get; }
    int iDatumText { get; }
    int iHausNr { get; }
    int iKBem { get; }
    int iLfNr { get; }
    int iOrt { get; }
    int iPerFamNr { get; }
    int iPlatz { get; }
    int iPrivacy { get; }
    string sArtText { get; }
    string[] sBem { get; }
    string sCausal { get; }
    string sDatumB_S { get; }
    string sDatumText { get; }
    string sDatumV_S { get; }
    string sDeath { get; }
    string sHausNr { get; }
    string sKBem { get; }
    string sOrt_S { get; }
    string sPlatz { get; }
    string sReg { get; }
    string sVChr { get; }
    string sZusatz { get; }
    bool xIsDead { get; }

    void Delete();
    void FillDataFields(IRecordset dB_EventTable);
}