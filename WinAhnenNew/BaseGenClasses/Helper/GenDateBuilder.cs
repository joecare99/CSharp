using BaseGenClasses.Helper;
using BaseGenClasses.Helper.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Model.Tests;

public class GenDateBuilder : IGenDateBuilder
{
    public IGenDate Emit(DateTime date, EDateModifier eDateModifier = EDateModifier.None, EDateType eDateType = EDateType.Full) 
        => new GenDate(eDateModifier, eDateType, date);
}