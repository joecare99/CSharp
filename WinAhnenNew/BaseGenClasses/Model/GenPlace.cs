using BaseGenClasses.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model;

public class GenPlace : GenObject, IGenPlace
{
    #region Properties
    private WeakReference<IGenealogy>? _WLowner;
    public override EGenType eGenType => EGenType.GenPlace;

    public string? Name { get ; set ; }
    public string? Type { get ; set ; }
    public string? GOV_ID { get ; set ; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get ; set ; }
    public IGenPlace? Parent { get; set; }

    public IGenealogy? Owner => (_WLowner?.TryGetTarget(out var t) ?? false) ?t:null;
    #endregion

    [JsonConstructor]
    private GenPlace() { }
    public GenPlace(params string[] place)
    {
        if (place.Length <= 0) return;
        Name = place[0];
        if (place.Length <= 1) return;
        Type = place[1];
        if (place.Length <= 2) return;
        GOV_ID = place[2];
        if (place.Length <= 4) return;
        Latitude = double.Parse(place[3]);
        Longitude = double.Parse(place[4]);
        if (place.Length <= 5) return;
        Notes = place[5];
    }
    void IHasOwner<IGenealogy>.SetOwner(IGenealogy t)
    {
        if (t == null) return;
        _WLowner = new(t);
    }

}