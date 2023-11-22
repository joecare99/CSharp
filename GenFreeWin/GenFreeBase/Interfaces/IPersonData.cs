﻿using System;
using System.Collections.Generic;

namespace GenFree.Interfaces;
public interface IPersonData
{
    string Alias { get; }
    string Baptised { get; }
    string Birthday { get; }
    string Burial { get; }
    List<string> Callname { get; }
    string Clan { get; }
    string Death { get; }
    DateTime dEditDat { get; }
    string FullName { get; }
    string FullSurName { get; }
    List<string> Givenname { get; }
    string Givennames { get; }
    Guid gUID { get; }
    int iPersNr { get; }
    int iReligi { get; }
    List<string> Nickname { get; }
    string Prae { get; }
    string Prefix { get; }
    string sAge { get; }
    string[] sBem { get; }
    string sBurried { get; }
    string sOFB { get; }
    string sPruefen { get; }
    string sSex { get; }
    string[] sSuch { get; }
    string Stat { get; }
    string Suffix { get; }
    string SurName { get; }
    bool xDead { get; }
    DateTime dBurial { get; }
    DateTime dDeath { get; }
    DateTime dBaptised { get; }
    DateTime dBirth { get; }

    void SetData(IEventData cEvt);
    void SetDates(string[] value, Func<string, string, string>? SetAge = null);
    void SetFull(string value);
    void SetFullSurname(string value);
    void SetPersonNames(int[] iName, (int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN);
    void SetPersonNr(int i);
}