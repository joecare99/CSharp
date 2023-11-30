//using DAO;
using GenFree.Data;
using GenFree.Interfaces;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model;
#nullable enable
public interface IEvent : IUsesRecordset<(EEventArt,int,short)>
{

    void ChgEvent(EEventArt eArt, int iFamNr, EEventArt eArt2, int iFam2 = 0);
    void DeleteBeSu(int PersInArb, EEventArt num6b);
    void DeleteAll(int PersInArb, EEventArt num6b);
    bool Exists(EEventArt eArt, object iLink, int iLfNR = 0);
    DateTime GetDate(int iFamPers, EEventArt eArt);
    DateTime GetDate(int iFamPers, EEventArt eArt, out string sDateV_S);
    DateTime GetDateB(int iFamPers, EEventArt eArt);
    DateTime GetDateB(int iFamPers, EEventArt eArt, out string sDateB_S);
    DateTime GetPersonBirthOrBapt(int persInArb);
    DateTime[] GetPersonDates(int persInArb, out bool xBC, Action<EEventArt, int, string>? onPlace = null);
    void PersonDat(int iPersNr, out DateTime down, out DateTime up);
    bool ReadData(int persInArb, EEventArt eEventArt, out IEventData? cEvt, short iLfNr = 0);
    IEventData? ReadDataPl(int persInArb, EEventArt eEventArt, out bool xBreak, short iLfNr = 0);
    IEnumerable<IEventData> ReadAll();
    IEnumerable<IEventData> ReadEventsBeSu(int iFamPers, EEventArt iArt);
    DateTime[] ReadFamDates(int famInArb);
    bool DeleteEmptyFam(int ifamInArb, EEventArt eArt);
    void PersLebDatles(int PersInArb, IPersonData person);
}