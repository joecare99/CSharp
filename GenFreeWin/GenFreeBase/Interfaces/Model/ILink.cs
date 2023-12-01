//using DAO;

using GenFree.Data;
using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Model;

public interface ILink : IUsesRecordset<(int iFamily, int iPerson, ELinkKennz iKennz)>, IUsesID<(int iFamily, int iPerson, ELinkKennz iKennz)>
{
    void Append(int iFamily, int iPerson, ELinkKennz iKennz);
    bool AppendE(int iFamNr, int iPerson, ELinkKennz eKennz);
    int AppendFamilyParent(int famInArb, int persInArb, ELinkKennz kennz, Func<int, bool, ELinkKennz, int> CheckPerson, bool xIgnoreSex = false);
    bool Delete(int iFamNr, int iPersNr, ELinkKennz iKennz);
    bool DeleteAllE(int iPersNr, ELinkKennz iKennz);
    bool DeleteAllF(int iFamNr, ELinkKennz iKennz);
    T DeleteChildren<T>(int famInArb, ELinkKennz iKennz, int persInArb, T mrOK, Func<int, T> func) where T : struct;
    void DeleteFam(int famInArb, ELinkKennz iKennz);
    void DeleteFamWhere(int famInArb, Predicate<ILinkData> pWhere);
    void DeleteInvalidPerson();
    bool DeleteQ<T>(int iFamNr, int iPersNr, ELinkKennz iKennz, T okVal, Func<int, int, T> func);
    bool Exist(int famInArb, int persInArb, ELinkKennz eKennz);
    bool ExistE(int persInArb, ELinkKennz eKennz);
    bool ExistF(int famInArb, ELinkKennz b2);
    bool ExistFam(int famInArb, ELinkKennz[] eLinkKennzs);
    bool ExistP(int iPersNr);
    bool GetFamPerson(int famInArb, ELinkKennz eLKennz, out int Link_iPerNr);
    bool GetPersonFam(int persInArb, ELinkKennz eLKennz, out int iFamNr);
    IList<int> GetPersonFams(int persInArb, ELinkKennz eLKnz);
    void ReadFamily(int FamNr, IFamilyPersons Family, Action<ELinkKennz, int>? action = null);
    bool SetEQ<T>(int iFamNr, int iPersNr, ELinkKennz iKennz, T okVal, Func<int, int, T> func);
    bool SetVerknQ<T>(int iFamNr, int iPersNr, ELinkKennz iKennz, T okVal, Func<int, int, T> func);
}