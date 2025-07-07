using GenFree.Data;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Data;
public interface IPersonData : IEntityData, IHasID<int>,IHasPropEnum<EPersonProp>, IHasIRecordset
{
    /// <summary>
    /// Gets the alias or AKA.
    /// </summary>
    /// <value>The alias.</value>
    string Alias { get; }
    /// <summary>
    /// Gets the baptised.
    /// </summary>
    /// <value>The baptised.</value>
    string Baptised { get; }
    /// <summary>
    /// Gets the birthday.
    /// </summary>
    /// <value>The birthday.</value>
    string Birthday { get; }
    /// <summary>
    /// Gets the burial.
    /// </summary>
    /// <value>The burial.</value>
    string Burial { get; }
    /// <summary>
    /// Gets the callname.
    /// </summary>
    /// <value>The callname.</value>
    IList<string> Callname { get; }
    /// <summary>
    /// Gets the clan.
    /// </summary>
    /// <value>The clan.</value>
    string Clan { get; }
    /// <summary>
    /// Gets the death.
    /// </summary>
    /// <value>The death.</value>
    string Death { get; }
    string FullSurName { get; }
    IList<string> Givenname { get; }
    string Givennames { get; }
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
    /// <summary>
    /// Gets or sets a value indicating whether [x v character].
    /// </summary>
    /// <value><c>true</c> if [x v character]; otherwise, <c>false</c>.</value>
    bool xVChr { get; set; }

    void SetData(IEventData cEvt);
    void SetDates(string[] value, Func<string, string, string>? SetAge = null);
    void SetFullSurname(string value);
    void SetPersonNames(int[] iName, (int iName, bool xRuf, bool xNick)[] aiVorns, bool xInclLN);
    void SetPersonNr(int i);
    void SetSex(string sSex);
}