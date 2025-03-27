using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Helper.Interfaces;

public interface IGenConnectBuilder
{
    IGenConnects Emit(EGenConnectionType type, IGenEntity entity, Guid? uid = null);
}