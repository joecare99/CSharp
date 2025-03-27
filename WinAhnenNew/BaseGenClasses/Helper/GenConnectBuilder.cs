using BaseGenClasses.Helper.Interfaces;
using BaseGenClasses.Model;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
namespace BaseGenClasses.Helper;

public class GenConnectBuilder : IGenConnectBuilder
{
    public IGenConnects Emit(EGenConnectionType type, IGenEntity entity,Guid? uid = null) 
        => new GenConnect() { eGenConnectionType = type, Entity=entity, UId = uid ?? new Guid()  };
}