using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;

namespace BaseGenClasses.Model;

public class GenConnect : GenObject, IGenConnects
{
    private WeakReference<IGenEntity>? _wlEntity;
    public EGenConnectionType eGenConnectionType { get; init; }
    public IGenEntity? Entity { 
        get=> (_wlEntity?.TryGetTarget(out IGenEntity? entity) ?? false) ? entity : null; 
        init=>_wlEntity = value == null ? null : new WeakReference<IGenEntity>(value); }

    public override EGenType eGenType => EGenType.GenConnect;
}