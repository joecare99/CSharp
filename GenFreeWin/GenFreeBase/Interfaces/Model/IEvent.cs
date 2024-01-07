//using DAO;
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Model;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model;
#nullable enable
public interface IEvent : 
    IHasDataItf<IEventData, (EEventArt eArt, int iLink, short iLfNr)>,
    IUsesRecordset<(EEventArt eArt, int iLink, short iLfNr)> , 
    IHasRSIndex1<EventIndex, EventFields>,
    IHasIxDataItf<EventIndex, IEventData, (EEventArt eArt, int iLink, short iLfNr)>
{
    void ChgEvent(EEventArt eArt, int iFamNr, EEventArt eArt2, int iFam2 = 0);
    void DeleteBeSu(EEventArt num6b, int PersInArb);
    void DeleteAll(EEventArt num6b, int PersInArb);
    bool DeleteEmptyFam(int ifamInArb, EEventArt eArt);
    bool Exists(EEventArt eArt, int iLink, int iLfNR = 0);
    void ForEachDo(EventIndex eIndex, EventFields eIndexField, object iIndexVal, Func<IEventData, bool> func);
    DateTime GetDate(EEventArt eArt, int iFamPers);
    DateTime GetDate(EEventArt eArt, int iFamPers, out string sDateV_S);
    DateTime GetDateB(EEventArt eArt, int iFamPers);
    DateTime GetDateB(EEventArt eArt, int iFamPers, out string sDateB_S);
    DateTime GetPersonBirthOrBapt(int persInArb, bool xPrefBap = false);
    DateTime[] GetPersonDates(int persInArb, out bool xBC, Action<EEventArt, int, string>? onPlace = null);
    void PersLebDatles(int PersInArb, IPersonData person);
    void PersonDat(int iPersNr, out DateTime down, out DateTime up);
    bool ReadData(EEventArt eEventArt, int persInArb, out IEventData? cEvt, short iLfNr = 0);
    IEventData? ReadDataPl(EEventArt eEventArt, int persInArb, out bool xBreak, short iLfNr = 0);
    IEnumerable<IEventData> ReadAll(EventIndex eIndex, object iPlace);
    bool ReadData(EventIndex eIndex, object iValue, out IEventData? cEvent);
    IEnumerable<IEventData> ReadEventsBeSu(int iFamPers, EEventArt iArt);
    DateTime[] ReadFamDates(int famInArb);
    void SetValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values);
    T GetValue<T>((EEventArt eArt, int iLink, short iLfNR) key, EventFields eDataField, T dDef);
    void UpdateClearPred(EventIndex eIndex, EventFields eIndexField, int iIndexVal, Predicate<IEventData> predicate);
    void UpdateAllSetVal(EventIndex eIndex, EventFields eIndexField, int iIndexVal, int iNewVal);
    bool UpdateValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values);
    void UpdateAllMvAppend(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, string sNewText);
    void UpdateAllMvVal(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, int iClearVal = 0);
    void UpdateAllSetValPred(EventIndex eIndex, EventFields eIndexField, int iIndexVal, EventFields eModField, int iNewVal, Predicate<IEventData> predicate, int iClearVal = 0);
    bool ExistsPred(EventIndex eIndex, EventFields eIndexField, int iTndexVal, Predicate<IEventData> predicate);
    void ClearAllRemText(EventIndex eIndex, EventFields eIdxField, int iIdxVal);
    bool ExistsBeSu(EEventArt eArt, int iLink);
}