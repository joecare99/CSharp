using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Helper.Interfaces;

public interface IGenDateBuilder
{
    IGenDate Emit(DateTime date, EDateModifier eDateModifier = EDateModifier.None, EDateType eDateType = EDateType.Full);
}
