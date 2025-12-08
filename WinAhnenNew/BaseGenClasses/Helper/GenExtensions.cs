using BaseGenClasses.Helper.Interfaces;
using BaseGenClasses.Model;
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
    /// <summary>
    /// Fügt eine Person mit dem angegebenen Verbindungstyp zur Liste der Verbindungen hinzu.
    /// Erstellt dazu eine neue Verbindung über den IGenConnectBuilder.
    /// Wirft eine ArgumentNullException, wenn die Person null ist.
    /// </summary>
    /// <param name="connects">Die Liste der Verbindungen.</param>
    /// <param name="type">Der Verbindungstyp.</param>
    /// <param name="person">Die hinzuzufügende Person.</param>
    /// <exception cref="System.ArgumentNullException">person - Person cannot be null.</exception>
    public static void AddPerson(this IList<IGenConnects?> connects, EGenConnectionType type, IGenPerson? person)
    {
        if (person == null)
            throw new ArgumentNullException(nameof(person), "Person cannot be null.");
        IGenConnectBuilder conBuilder = IoC.GetRequiredService<IGenConnectBuilder>();
        var connect = conBuilder.Emit(type,person);

        connects.Add(connect);
    }

    /// <summary>
    /// Setzt eine Person mit dem angegebenen Verbindungstyp in der Liste der Verbindungen.
    /// Erstellt dazu eine neue Verbindung über den IGenConnectBuilder.
    /// Wenn die Person null ist, wird die bestehende Verbindung entfernt.
    /// </summary>
    /// <param name="connects">Die Liste der Verbindungen.</param>
    /// <param name="type">Der Verbindungstyp.</param>
    /// <param name="person">Die hinzuzufügende Person.</param>

    public static void SetPerson(this IList<IGenConnects?> connects, EGenConnectionType type, IGenPerson? person)
    {
        IGenConnectBuilder conBuilder = IoC.GetRequiredService<IGenConnectBuilder>();
        if (person != null)
        {
            var connect = conBuilder.Emit(type, person);
            connects.Add(connect);
        }
        else
        {
            var connect = conBuilder.Emit(type, connects);
            if (connect != null) connects.Remove(connect);
        }
    }

    public static void AddFamily(this IList<IGenConnects?> connects, EGenConnectionType type, IGenFamily? family)
    {
        if (family == null)
            throw new ArgumentNullException(nameof(family), "Family cannot be null.");
        IGenConnectBuilder conBuilder = IoC.GetRequiredService<IGenConnectBuilder>();
        var connect = conBuilder.Emit(type, family);

        connects.Add(connect);
    }

    public static IGenFact AddFact(this IGenEntity mainEnt, EFactType type, string? data, Guid? Uid = null) 
        => mainEnt.Facts.AddFact(mainEnt, type, data, Uid);
    public static IGenFact AddFact(this IList<IGenFact?> facts,IGenEntity mainEnt, EFactType type, string? data, Guid? Uid=null)
    {
        IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
        var fact = conBuilder.Emit(type,mainEnt, data, Uid);

        facts.Add(fact);
        return fact;
    }

    public static IGenFact AddEvent(this IList<IGenFact?> facts, IGenEntity mainEnt, EFactType type,IGenDate date, string data, Guid? Uid = null)
    {
        IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
        var evnt = conBuilder.Emit(type, mainEnt,date, data, Uid);

        facts.Add(evnt);
        return evnt;
    }

    public static IGenFact AddEvent(this IGenEntity mainEnt, EFactType type, IGenDate genDate, string data, Guid? Uid = null) 
        => mainEnt.Facts.AddEvent(mainEnt, type, genDate, data, Uid);
    public static IGenFact AddEvent(this IGenEntity mainEnt, EFactType type, DateTime date, string data, Guid? Uid = null)
    {
        IGenDateBuilder dateBuilder = IoC.GetRequiredService<IGenDateBuilder>();
        return AddEvent(mainEnt, type, dateBuilder.Emit(date), data, Uid);
    }

    public static IGenFact AddEvent(this IList<IGenFact?> facts, IGenEntity mainEnt, EFactType type, IGenDate date, IGenPlace place, string data, Guid? Uid = null)
    {
        IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
        var evnt = conBuilder.Emit(type, mainEnt, date, place, data, Uid);
        facts.Add(evnt);
        return evnt;
    }

    public static IGenFact AddEvent(this IGenEntity mainEnt, EFactType type, IGenDate date, IGenPlace place, string data, Guid? Uid = null) 
        => mainEnt.Facts.AddEvent(mainEnt, type, date,place, data, Uid);

    public static T? GetFact<T>(this IList<IGenFact?> facts, EFactType type,Func<IGenFact,T> selFct)
    {
        return facts.Where(f => f?.eFactType == type).Select(selFct!).FirstOrDefault();
    }

    public static IGenFact? GetFact(this IList<IGenFact?> facts, EFactType type)
    {
        return facts.Where(f => f?.eFactType == type).FirstOrDefault();
    }

    public static void SetFact(this IList<IGenFact?> facts, EFactType type,IGenEntity genEntity, string? value)
    {
        (facts.FirstOrDefault(f => f?.eFactType == type) ?? genEntity.AddFact( type, value)).Data = value;       
    }

    public static IIndexedList<T> ToIndexedList<T,T2>(this IEnumerable<T?> list,Func<T,T2> getIdx) where T : class where T2 : notnull
    {
        IGenILBuilder conBuilder = IoC.GetRequiredService<IGenILBuilder>();
        var result = conBuilder.NewList(getIdx);
        foreach (var item in list)
        {
            if (item == null) continue;
            result.Add(item,getIdx(item));
        }
        return result;
    }
}
