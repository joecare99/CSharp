using GenFree.Data;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces;
public interface IPersonData : IHasID<int>,IHasPropEnum<EPersonProp>, IHasIRecordset
{
    string Alias { get; }
    string Baptised { get; }
    string Birthday { get; }
    string Burial { get; }
    IList<string> Callname { get; }
    string Clan { get; }
    string Death { get; }
    DateTime dEditDat { get; }
    string FullSurName { get; }
    IList<string> Givenname { get; }
    string Givennames { get; }
    Guid gUid { get; }
    int iReligi { get; }
    IList<string> Nickname { get; }
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
    DateTime dAnlDatum { get; }
    bool isEmpty { get; }

    void SetData(IEventData cEvt);
    void SetDates(string[] value, Func<string, string, string>? SetAge = null);
    void SetFullSurname(string value);
    void SetPersonNames(int[] iName, (int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN);
    void SetPersonNr(int i);
}