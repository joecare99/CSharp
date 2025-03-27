using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Model;

public class GenConnect : GenObject, IGenConnects
{

    public EGenConnectionType eGenConnectionType { get; init; }
    public IGenEntity Entity { get; init; }

    public override EGenType eGenType => EGenType.GenConnect;
}