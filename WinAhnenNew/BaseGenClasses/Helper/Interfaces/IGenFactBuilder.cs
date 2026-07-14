using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Helper.Interfaces;

public interface IGenFactBuilder
{
    IGenFact Emit(EFactType type, IGenEntity mainEnt, string? data, Guid? Uid, bool xReplace = true);
    IGenFact Emit(EFactType type, IGenEntity mainEnt, IGenDate date, string? data, Guid? Uid, bool xReplace = true);
    IGenFact Emit(EFactType type, IGenEntity mainEnt, IGenDate date, IGenPlace place, string? data, Guid? Uid, bool xReplace = true);
}
