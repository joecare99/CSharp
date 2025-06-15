using BaseGenClasses.Helper.Interfaces;
using BaseGenClasses.Model;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BaseGenClasses.Helper;

public class GenConnectBuilder : IGenConnectBuilder
{
    public IGenConnects Emit(EGenConnectionType type, IGenEntity entity,Guid? uid = null) 
        => new GenConnect() { eGenConnectionType = type, Entity=entity, UId = uid ?? new Guid()  };

    public IGenConnects? Emit(EGenConnectionType type, IList<IGenConnects?> connects) 
        => connects.FirstOrDefault(c => c?.eGenConnectionType == type);
}