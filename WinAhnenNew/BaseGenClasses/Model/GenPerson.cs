using BaseGenClasses.Helper;
//using GenFree2Base.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BaseGenClasses.Model;

[JsonDerivedType(typeof(GenPerson), typeDiscriminator: nameof(GenPerson))]
public class GenPerson : GenEntity, IGenPerson
{
    #region Properties
    public override EGenType eGenType => EGenType.GenPerson;

    [JsonIgnore]
    public string Name { get => BuildFullname(Facts); set => ParseName(value,Facts); }
    [JsonIgnore]
    public string GivenName { get => Facts.Where(f=>f.eGenType== EGenType.GenFact && f.eFactType==EFactType.Givenname).Select(g=>g.Data).FirstOrDefault(); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public string Surname { get => Facts.GetFact(EFactType.Surname,f=>f.Data); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public string Title { get => Facts.GetFact(EFactType.Title, f => f.Data); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public string Sex { get => Facts.GetFact(EFactType.Sex, f => f.Data); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public string IndRefID { get => Facts.GetFact(EFactType.Reference, f => f.Data); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPerson Father { get => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Parent && c.Entity is IGenPerson p && p.Sex == "M").Select(p=>p.Entity as IGenPerson).FirstOrDefault(); 
        set => Connects.AddPerson(EGenConnectionType.Parent,value); }
    [JsonIgnore]
    public IGenPerson Mother { get => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Parent && c.Entity is IGenPerson p && p.Sex == "F").Select(p => p.Entity as IGenPerson).FirstOrDefault(); 
        set => Connects.AddPerson(EGenConnectionType.Parent, value); }

    [JsonIgnore]
    public int ChildCount => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Child && c.Entity is IGenPerson).Count();

    [JsonIgnore]
    public IIndexedList<IGenPerson> Children => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Child && c.Entity is IGenPerson).Select(f => f.Entity as IGenPerson).ToIndexedList(i=>i.IndRefID );

    [JsonIgnore]
    public IGenFamily ParentFamily { get => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.ParentFamily && c.Entity is IGenFamily).Select(f => f.Entity as IGenFamily).FirstOrDefault(); set => throw new NotImplementedException(); }

    [JsonIgnore]
    public int FamilyCount => Connects.Where(c => c.Entity is IGenFamily).Count();

    [JsonIgnore]
    public IIndexedList<IGenFamily> Families => Connects.Where(c => c.Entity is IGenFamily).Select(f=>f.Entity as IGenFamily).ToIndexedList(f=>f.UId );

    [JsonIgnore]
    public int SpouseCount => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Spouse && c.Entity is IGenPerson).Count();

    [JsonIgnore]
    public IIndexedList<IGenPerson> Spouses => Connects.Where(c => c.Entity is IGenPerson && c.eGenConnectionType == EGenConnectionType.Spouse).Select(p => p.Entity as IGenPerson).ToIndexedList(f => f.UId);

    [JsonIgnore]
    public IGenDate BirthDate { get => Facts.GetFact(EFactType.Birth,t=>t.Date); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPlace BirthPlace { get => Facts.GetFact(EFactType.Birth, t => t.Place); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenFact Birth { get => Facts.GetFact(EFactType.Birth); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenDate BaptDate { get => Facts.GetFact(EFactType.Baptism, t => t.Date); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPlace BaptPlace { get => Facts.GetFact(EFactType.Baptism, t => t.Place); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenFact Baptism { get => Facts.GetFact(EFactType.Baptism); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenDate DeathDate { get => Facts.GetFact(EFactType.Death, t => t.Date); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPlace DeathPlace { get => Facts.GetFact(EFactType.Death, t => t.Place); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenFact Death { get => Facts.GetFact(EFactType.Death); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenDate BurialDate { get => Facts.GetFact(EFactType.Burial, t => t.Date); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPlace BurialPlace { get => Facts.GetFact(EFactType.Burial, t => t.Place); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenFact Burial { get => Facts.GetFact(EFactType.Burial); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public string? Religion { get => Facts.GetFact(EFactType.Religion, t => t.Data); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public string? Occupation { get => Facts.GetFact(EFactType.Occupation, t => t.Data); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPlace OccuPlace { get => Facts.GetFact(EFactType.Occupation, t => t.Place); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IGenPlace Residence { get => Facts.GetFact(EFactType.Residence, t => t.Place); set => throw new NotImplementedException(); }
    [JsonIgnore]
    public IIndexedList<IGenFamily> Marriages => Connects.Where(c => c.Entity is IGenFamily && c.eGenConnectionType == EGenConnectionType.ChildFamily).Select(p => p.Entity as IGenFamily).ToIndexedList(f => f.UId);

    #endregion

    protected override IGenFact? GetEndFactOfEntity() 
        => Facts.FirstOrDefault(f => f.eGenType == EGenType.GenFact && f.eFactType is EFactType.Death or EFactType.Burial);

    protected override IGenFact? GetStartFactOfEntity()
        => Facts.FirstOrDefault(f => f.eGenType == EGenType.GenFact && f.eFactType is EFactType.Birth or EFactType.Baptism);

    private void ParseName(string value, IList<IGenFact> facts)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;
        if (value.StartsWith("Prof."))
        {
            facts.AddFact(this, EFactType.Title, "Prof.");
            value = value.Substring(5).Trim();
        }
        if (value.StartsWith("Dr."))
        {
            facts.AddFact(this, EFactType.Title, "Dr.");
            value = value.Substring(3).Trim();
        }
        if (value.Contains(" "))
        {
            var parts = value.Split(' ');
            if (parts.Length > 1)
            {
                facts.AddFact(this, EFactType.Givenname, parts[0]);
                facts.AddFact(this, EFactType.Surname, parts[1]);
            }
        }
        else
        {
            facts.AddFact(this, EFactType.Givenname, value);
        }
    }

    private string BuildFullname(IList<IGenFact> facts)
    {
       return $"{facts.GetFact(EFactType.Title, f => f.Data)} {facts.GetFact(EFactType.Givenname, f => f.Data)} {facts.GetFact(EFactType.Surname, f => f.Data)}".Trim();
    }

}
