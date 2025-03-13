using BaseLib.Helper;
using GenFree2Base.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseGenClasses.Helper
{
   public static class GenExtensions
    {
        public static void AddPerson(this IList<IGenConnects> connects, EGenConnectionType type, IGenPerson person)
        {
            IGenConnectBuilder conBuilder = IoC.GetRequiredService<IGenConnectBuilder>();
            var connect = conBuilder.Emit(type,person);

            connects.Add(connect);
        }

        public static void AddFact(this IList<IGenFact> facts,IGenEntity mainEnt, EFactType type, string data)
        {
            IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
            var fact = conBuilder.Emit(type,mainEnt, data);

            facts.Add(fact);
        }
        public static void AddEvent(this IList<IGenFact> facts, IGenEntity mainEnt, EFactType type,IGenDate date, string data)
        {
            IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
            var evnt = conBuilder.Emit(type, mainEnt,date, data);

            facts.Add(evnt);
        }

        public static void AddEvent(this IList<IGenFact> facts, IGenEntity mainEnt, EFactType type, IGenDate date, IGenPlace place, string data)
        {
            IGenFactBuilder conBuilder = IoC.GetRequiredService<IGenFactBuilder>();
            var evnt = conBuilder.Emit(type, mainEnt, date, place, data);
            facts.Add(evnt);
        }

        public static T? GetFact<T>(this IList<IGenFact> facts, EFactType type,Func<IGenFact,T> selFct)
        {
            return facts.Where(f => f.eFactType == type).Select(selFct).FirstOrDefault();
        }

        public static IGenFact GetFact(this IList<IGenFact> facts, EFactType type)
        {
            return facts.Where(f => f.eFactType == type).FirstOrDefault();
        }

        public static IIndexedList<T> ToIndexedList<T,T2>(this IEnumerable<T> list,Func<T,T2> getIdx)
        {
            IGenILBuilder conBuilder = IoC.GetRequiredService<IGenILBuilder>();
            var result = conBuilder.NewList<T,T2>();
            foreach (var item in list)
            {
                result.Add(item,getIdx(item));
            }
            return result;
        }
    }
}
