using BaseGenClasses.Helper.Interfaces;
using BaseGenClasses.Model;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Helper
{
    public class GenFactBuilder : IGenFactBuilder
    {

        public IGenFact Emit(EFactType type, IGenEntity mainEnt, string data, Guid? Uid, bool xReplace = true) => new GenFact(mainEnt)
        {
            eFactType = type,
            UId = Uid ?? new Guid(),
            Data = data
        };

        public IGenFact Emit(EFactType type, IGenEntity mainEnt, IGenDate date, string data, Guid? Uid, bool xReplace = true) => new GenFact(mainEnt)
        {
            eFactType = type,
            UId = Uid ?? new Guid(),
            Date = date,
            Data = data
        };

        public IGenFact Emit(EFactType type, IGenEntity mainEnt, IGenDate date, IGenPlace place, string data, Guid? Uid, bool xReplace = true) => new GenFact(mainEnt)
        {
            eFactType = type,
            UId = Uid ?? new Guid(),
            Date = date,
            Place = place,
            Data = data
        };
    }
}