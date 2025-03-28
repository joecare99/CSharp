using BaseGenClasses.Helper.Interfaces;
using BaseLib.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseGenClasses.Helper;

public static class GenExtensions
{
    public static void AddPerson(this IList<IGenConnects> connects, EGenConnectionType type, IGenPerson person)
    {
        IGenConnectBuilder conBuilder = IoC.GetRequiredService<IGenConnectBuilder>();
        var connect = conBuilder.Emit(type,person);

        connects.Add(connect);
    }

    public static void AddFamily(this IList<IGenConnects> connects, EGenConnectionType type, IGenFamily family)
    {
        IGenConnectBuilder conBuilder = IoC.GetRequiredService<IGenConnectBuilder>();
        var connect = conBuilder.Emit(type, family);

        connects.Add(connect);
    }

    public static void AddFact(this IList<IGenFact> facts,IGenEntity mainEnt, EFactType type, string data, Guid? Uid=null)
    {
        IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
        var fact = conBuilder.Emit(type,mainEnt, data, Uid);

        facts.Add(fact);
    }
    public static void AddFact(this IGenEntity mainEnt, EFactType type, string data, Guid? Uid = null) 
        => mainEnt.Facts.AddFact(mainEnt, type, data, Uid);

    public static void AddEvent(this IList<IGenFact> facts, IGenEntity mainEnt, EFactType type,IGenDate date, string data, Guid? Uid = null)
    {
        IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
        var evnt = conBuilder.Emit(type, mainEnt,date, data, Uid);

        facts.Add(evnt);
    }

    public static void AddEvent(this IGenEntity mainEnt, EFactType type, IGenDate date, string data, Guid? Uid = null) 
        => mainEnt.Facts.AddEvent(mainEnt, type, date, data, Uid);

    public static void AddEvent(this IList<IGenFact> facts, IGenEntity mainEnt, EFactType type, IGenDate date, IGenPlace place, string data, Guid? Uid = null)
    {
        IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
        var evnt = conBuilder.Emit(type, mainEnt, date, place, data, Uid);
        facts.Add(evnt);
    }

    public static void AddEvent(this IGenEntity mainEnt, EFactType type, IGenDate date, IGenPlace place, string data, Guid? Uid = null) 
        => mainEnt.Facts.AddEvent(mainEnt, type, date,place, data, Uid);

    public static T? GetFact<T>(this IList<IGenFact> facts, EFactType type,Func<IGenFact,T> selFct)
    {
        return facts.Where(f => f.eFactType == type).Select(selFct).FirstOrDefault();
    }

    public static IGenFact GetFact(this IList<IGenFact> facts, EFactType type)
    {
        return facts.Where(f => f.eFactType == type).FirstOrDefault();
    }

    public static IIndexedList<T> ToIndexedList<T,T2>(this IEnumerable<T> list,Func<T,T2> getIdx) where T : class where T2 : notnull
    {
        IGenILBuilder conBuilder = IoC.GetRequiredService<IGenILBuilder>();
        var result = conBuilder.NewList(getIdx);
        foreach (var item in list)
        {
            result.Add(item,getIdx(item));
        }
        return result;
    }
}
