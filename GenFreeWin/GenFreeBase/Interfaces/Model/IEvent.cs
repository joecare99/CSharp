//using DAO;
using GenFree.Data;
using GenFree.Interfaces;
using GenFree.Interfaces.DB;
using GenFree.Model;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model;
#nullable enable
public interface IEvent : IHasDataItf<IEventData, (EEventArt eArt, int iLink, short iLfNr)>, IUsesRecordset<(EEventArt eArt, int iLink, short iLfNr)> , IHasRSIndex1<EventIndex, EventFields>
{
    void ChgEvent(EEventArt eArt, int iFamNr, EEventArt eArt2, int iFam2 = 0);
    void DeleteBeSu(int PersInArb, EEventArt num6b);
    void DeleteAll(int PersInArb, EEventArt num6b);
    bool DeleteEmptyFam(int ifamInArb, EEventArt eArt);
    bool Exists(EEventArt eArt, int iLink, int iLfNR = 0);
    void ForeachDo(EventIndex eIndex, EventFields eIndexField, object iIndexVal, Func<IEventData, bool> func);
    DateTime GetDate(int iFamPers, EEventArt eArt);
    DateTime GetDate(int iFamPers, EEventArt eArt, out string sDateV_S);
    DateTime GetDateB(int iFamPers, EEventArt eArt);
    DateTime GetDateB(int iFamPers, EEventArt eArt, out string sDateB_S);
    DateTime GetPersonBirthOrBapt(int persInArb, bool xPrefBap = false);
    DateTime[] GetPersonDates(int persInArb, out bool xBC, Action<EEventArt, int, string>? onPlace = null);
    void PersLebDatles(int PersInArb, IPersonData person);
    void PersonDat(int iPersNr, out DateTime down, out DateTime up);
    bool ReadData(int persInArb, EEventArt eEventArt, out IEventData? cEvt, short iLfNr = 0);
    IEventData? ReadDataPl(int persInArb, EEventArt eEventArt, out bool xBreak, short iLfNr = 0);
    IEnumerable<IEventData> ReadAll();
    IEnumerable<IEventData> ReadAll(EventIndex eIndex);
    IEnumerable<IEventData> ReadAll(EventIndex eIndex, object iPlace);
    bool ReadData(EventIndex eIndex, object iValue, out IEventData? cEvent);
    IEnumerable<IEventData> ReadEventsBeSu(int iFamPers, EEventArt iArt);
    DateTime[] ReadFamDates(int famInArb);
    void SetValues((EEventArt eArt, int iLink, short iLfNr) key, (EventFields, object)[] values);
    T GetValue<T>((EEventArt eArt, int iLink, short iLfNR) key, EventFields eDataField, T dDef);
}