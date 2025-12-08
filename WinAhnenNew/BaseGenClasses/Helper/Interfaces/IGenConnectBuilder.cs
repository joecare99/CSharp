using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;

namespace BaseGenClasses.Helper.Interfaces;

public interface IGenConnectBuilder
{
    IGenConnects Emit(EGenConnectionType type, IGenEntity entity, Guid? uid = null);
    IGenConnects? Emit(EGenConnectionType type, IList<IGenConnects?> connects);
}