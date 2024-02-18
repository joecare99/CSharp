// ***********************************************************************
// Assembly         : GenFreeBase
// Author           : Mir
// Created          : 11-19-2023
//
// Last Modified By : Mir
// Last Modified On : 01-10-2024
// ***********************************************************************
// <copyright file="IEvent.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenFree.Data;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;

/// <summary>
/// The Model namespace.
/// </summary>
namespace GenFree.Interfaces.Model;
#nullable enable
/// <summary>
/// Interface IEvent
/// Extends the <see cref="Model.IHasDataItf{IEventData, (EEventArt eArt, int iLink, short iLfNr)}" />
/// Extends the <see cref="Model.IUsesRecordset{(EEventArt eArt, int iLink, short iLfNr)}" />
/// Extends the <see cref="IHasRSIndex1{EventIndex, EventFields}" />
/// Extends the <see cref="Model.IHasIxDataItf{EventIndex, IEventData, (EEventArt eArt, int iLink, short iLfNr)}" />
/// </summary>
/// <seealso cref="Model.IHasDataItf{IEventData, (EEventArt eArt, int iLink, short iLfNr)}" />
/// <seealso cref="Model.IUsesRecordset{(EEventArt eArt, int iLink, short iLfNr)}" />
/// <seealso cref="IHasRSIndex1{EventIndex, EventFields}" />
/// <seealso cref="Model.IHasIxDataItf{EventIndex, IEventData, (EEventArt eArt, int iLink, short iLfNr)}" />
public interface IEvent : 
    IUsesRecordset<(EEventArt eArt, int iLink, short iLfNr)> , 
    IHasRSIndex1<EventIndex, EventFields>,
    IHasIxDataItf<EventIndex, IEventData, (EEventArt eArt, int iLink, short iLfNr)>
{
    /// <summary>
    /// Replace/Change the event with another.
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iFamNr">The i fam nr.</param>
    /// <param name="eArt2">The e art2.</param>
    /// <param name="iFam2">The i fam2.</param>
    void ChgEvent(EEventArt eArt, int iFamNr, EEventArt eArt2, int iFam2 = 0);

    /// <summary>Deletes the Event using the "BeSu"-Index.</summary>
    /// <param name="eArt">The event-type</param>
    /// <param name="iLink">The linked person/family</param>
    /// <returns>
    ///   <c>true</c> if event found/deleted, <c>false</c> otherwise.</returns>
    bool DeleteBeSu(EEventArt eArt, int iLink);
    /// <summary>
    /// Deletes all Events (with given Type &amp; Person.
    /// </summary>
    /// <param name="num6b">The num6b.</param>
    /// <param name="PersInArb">The pers in arb.</param>
    void DeleteAll(EEventArt num6b, int PersInArb);
    /// <summary>
    /// Deletes the empty events of the family.
    /// </summary>
    /// <param name="ifamInArb">The ifam in arb.</param>
    /// <param name="eArt">The e art.</param>
    /// <returns><c>true</c> if exists, <c>false</c> otherwise.</returns>
    bool DeleteEmptyFam(int ifamInArb, EEventArt eArt);

    /// <summary>
    /// Checks if specified event exists.
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iLink">The i link.</param>
    /// <param name="iLfNR">The i lf nr.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool Exists(EEventArt eArt, int iLink, int iLfNR = 0);
    /// <summary>
    /// Checks if the specified event exists with predicate.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iTndexVal">The i tndex value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns><c>true</c> if exists, <c>false</c> otherwise.</returns>
    bool ExistsPred(EventIndex eIndex, EventFields eIndexField, int iTndexVal, Predicate<IEventData> predicate);
    /// <summary>
    /// Checks if the specified event exists (using "BeSu"-index).
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iLink">The i link.</param>
    /// <returns><c>true</c> if exists, <c>false</c> otherwise.</returns>
    bool ExistsBeSu(EEventArt eArt, int iLink);

    /// <summary>
    /// Executes the function for each specified event.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iIndexVal">The i index value.</param>
    /// <param name="func">The function.</param>
    void ForEachDo(EventIndex eIndex, EventFields eIndexField, object iIndexVal, Func<IEventData, bool> func);

    /// <summary>
    /// Gets the (start-) date.
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iFamPers">The i fam pers.</param>
    /// <returns>DateTime.</returns>
    DateTime GetDate(EEventArt eArt, int iFamPers);
    /// <summary>
    /// Gets the (start-) date.
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iFamPers">The i fam pers.</param>
    /// <param name="sDateV_S">The s date v s.</param>
    /// <returns>DateTime.</returns>
    DateTime GetDate(EEventArt eArt, int iFamPers, out string sDateV_S);
    /// <summary>
    /// Gets the end-date.
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iFamPers">The i fam pers.</param>
    /// <returns>DateTime.</returns>
    DateTime GetDateB(EEventArt eArt, int iFamPers);
    /// <summary>
    /// Gets the end-date.
    /// </summary>
    /// <param name="eArt">The e art.</param>
    /// <param name="iFamPers">The i fam pers.</param>
    /// <param name="sDateB_S">The s date b s.</param>
    /// <returns>DateTime.</returns>
    DateTime GetDateB(EEventArt eArt, int iFamPers, out string sDateB_S);
    /// <summary>
    /// Gets the person birth or baptism-date.
    /// </summary>
    /// <param name="iPersNr">The person.</param>
    /// <param name="xPrefBap">if set to <c>true</c>, baptism is preferred.</param>
    /// <returns>DateTime.</returns>
    DateTime GetPersonBirthOrBapt(int iPersNr, bool xPrefBap = false);
    /// <summary>
    /// Gets the person dates.
    /// </summary>
    /// <param name="iPersNr">The person.</param>
    /// <param name="xBC">if set to <c>true</c> date is b.c.</param>
    /// <param name="onPlace">The on place.</param>
    /// <returns>DateTime[].</returns>
    DateTime[] GetPersonDates(int iPersNr, out bool xBC, Action<EEventArt, int, string>? onPlace = null);
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">The key.</param>
    /// <param name="eDataField">The data field.</param>
    /// <param name="dDef">The definition.</param>
    /// <returns>T.</returns>
    T GetValue<T>((EEventArt eArt, int iLink, short iLfNR) key, EventFields eDataField, T dDef);

    /// <summary>
    /// Perses the leb datles.
    /// </summary>
    /// <param name="PersInArb">The pers in arb.</param>
    /// <param name="person">The person.</param>
    void PersLebDatles(int PersInArb, IPersonData person);
    /// <summary>
    /// Persons the dat.
    /// </summary>
    /// <param name="iPersNr">The i pers nr.</param>
    /// <param name="down">Down.</param>
    /// <param name="up">Up.</param>
    void PersonDat(int iPersNr, out DateTime down, out DateTime up);

    /// <summary>
    /// Reads the data.
    /// </summary>
    /// <param name="eEventArt">The e event art.</param>
    /// <param name="persInArb">The pers in arb.</param>
    /// <param name="cEvt">The c evt.</param>
    /// <param name="iLfNr">The i lf nr.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool ReadData(EEventArt eEventArt, int persInArb, out IEventData? cEvt, short iLfNr = 0);
    /// <summary>
    /// Reads the data pl.
    /// </summary>
    /// <param name="eEventArt">The e event art.</param>
    /// <param name="persInArb">The pers in arb.</param>
    /// <param name="xBreak">if set to <c>true</c> [x break].</param>
    /// <param name="iLfNr">The i lf nr.</param>
    /// <returns>System.Nullable&lt;IEventData&gt;.</returns>
    IEventData? ReadDataPl(EEventArt eEventArt, int persInArb, out bool xBreak, short iLfNr = 0);
    /// <summary>
    /// Reads the data.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="iValue">The i value.</param>
    /// <param name="cEvent">The c event.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool ReadData(EventIndex eIndex, object iValue, out IEventData? cEvent);
    /// <summary>Reads the data with BeSu-Index.</summary>
    /// <param name="eArt">The event art.</param>
    /// <param name="iLink">The linked family/person.</param>
    /// <param name="cEv">The data.</param>
    /// <returns>
    ///   <c>true</c> if event found, <c>false</c> otherwise.</returns>
    bool ReadBeSu(EEventArt eArt, int iLink, out IEventData? cEv);
    /// <summary>
    /// Reads the events be su.
    /// </summary>
    /// <param name="iFamPers">The i fam pers.</param>
    /// <param name="iArt">The i art.</param>
    /// <returns>IEnumerable&lt;IEventData&gt;.</returns>
    IEnumerable<IEventData> ReadEventsBeSu(int iFamPers, EEventArt iArt);
    /// <summary>
    /// Reads the fam dates.
    /// </summary>
    /// <param name="famInArb">The fam in arb.</param>
    /// <returns>DateTime[].</returns>
    DateTime[] ReadFamDates(int famInArb);
    /// <summary>
    /// Sets the values.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="values">The values.</param>
    void SetValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values);
    /// <summary>Seeks the specified event with BeSu-index </summary>
    /// <param name="eArt">The event art.</param>
    /// <param name="iPerFamnr">The linked person/family.</param>
    /// <param name="xBreak">if set to <c>true</c> event not found.</param>
    /// <returns>System.Nullable&lt;IRecordset&gt;.</returns>
    IRecordset? SeekBeSu(EEventArt eArt, int iPerFamnr, out bool xBreak);

    /// <summary>
    /// Updates the clear pred.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iIndexVal">The i index value.</param>
    /// <param name="predicate">The predicate.</param>
    void UpdateClearPred(EventIndex eIndex, EventFields eIndexField, int iIndexVal, Predicate<IEventData> predicate);
    /// <summary>
    /// Updates all set value.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iIndexVal">The i index value.</param>
    /// <param name="iNewVal">The i new value.</param>
    void UpdateAllSetVal(EventIndex eIndex, EventFields eIndexField, int iIndexVal, int iNewVal);
    /// <summary>
    /// Updates the values.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="values">The values.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool UpdateValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values);
    /// <summary>
    /// Updates all mv append.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iIndexVal">The i index value.</param>
    /// <param name="eModField">The e mod field.</param>
    /// <param name="sNewText">The s new text.</param>
    void UpdateAllMvAppend(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, string sNewText);
    /// <summary>
    /// Updates all mv value.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iIndexVal">The i index value.</param>
    /// <param name="eModField">The e mod field.</param>
    /// <param name="iClearVal">The i clear value.</param>
    void UpdateAllMvVal(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, int iClearVal = 0);
    /// <summary>
    /// Updates all set value pred.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIndexField">The e index field.</param>
    /// <param name="iIndexVal">The i index value.</param>
    /// <param name="eModField">The e mod field.</param>
    /// <param name="iNewVal">The i new value.</param>
    /// <param name="predicate">The predicate.</param>
    /// <param name="iClearVal">The i clear value.</param>
    void UpdateAllSetValPred(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, int iNewVal, Predicate<IEventData> predicate, int iClearVal = 0);
    /// <summary>
    /// Clears all rem text.
    /// </summary>
    /// <param name="eIndex">Index of the e.</param>
    /// <param name="eIdxField">The e index field.</param>
    /// <param name="iIdxVal">The i index value.</param>
    void ClearAllRemText(EventIndex eIndex, EventFields eIdxField, int iIdxVal);
    IRecordset AppendRaw((EEventArt eArt, int iLink, short iLfNr) key);
    void SetValAppend((EEventArt eArt, int iLink, short iLfNr) key, EventFields eSetField, string sNewVal);
    T GetValue<T>(int persInArb, EEventArt iEventType, EventFields eGetField, Func<IField, T> conv);
    void UpdateReplFams(int Fam1, int Fam2, EEventArt eArt);
    void DeleteAllNonVitalE(int num18);
    void DeleteAllVitalE(int num18);
    IEnumerable<IEventData> ReadAllGt(EventIndex eIndex, int iIndexVal);
}