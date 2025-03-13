using BaseGenClasses.Helper;
using GenFree2Base.Interfaces;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System.Runtime.Serialization;

namespace BaseGenClasses.Model;

public class GenPerson : GenEntity, IGenPerson
{
    #region Properties
    public string Name { get => BuildFullname(Facts); set => ParseName(value,Facts); }

    public string GivenName { get => Facts.Where(f=>f.eGenType== EGenType.GenFact && f.eFactType==EFactType.Givenname).Select(g=>g.Data).FirstOrDefault(); set => throw new NotImplementedException(); }
    public string Surname { get => Facts.GetFact(EFactType.Surname,f=>f.Data); set => throw new NotImplementedException(); }
    public string Title { get => Facts.GetFact(EFactType.Title, f => f.Data); set => throw new NotImplementedException(); }
    public string Sex { get => Facts.GetFact(EFactType.Sex, f => f.Data); set => throw new NotImplementedException(); }
    public string IndRefID { get => Facts.GetFact(EFactType.Reference, f => f.Data); set => throw new NotImplementedException(); }
    public IGenPerson Father { get => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Parent && c.Entity is IGenPerson p && p.Sex == "M").Select(p=>p.Entity as IGenPerson).FirstOrDefault(); 
        set => Connects.AddPerson(EGenConnectionType.Parent,value); }
    public IGenPerson Mother { get => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Parent && c.Entity is IGenPerson p && p.Sex == "F").Select(p => p.Entity as IGenPerson).FirstOrDefault(); 
        set => Connects.AddPerson(EGenConnectionType.Parent, value); }

    public int ChildCount => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Child && c.Entity is IGenPerson).Count();

    public IIndexedList<IGenPerson> Children => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Child && c.Entity is IGenPerson).Select(f => f.Entity as IGenPerson).ToIndexedList(i=>i.IndRefID );

    public IGenFamily ParentFamily { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int FamilyCount => Connects.Where(c => c.Entity is IGenFamily).Count();

    public IIndexedList<IGenFamily> Families => Connects.Where(c => c.Entity is IGenFamily).Select(f=>f.Entity as IGenFamily).ToIndexedList();

    public int SpouseCount => Connects.Where(c => c.eGenConnectionType == EGenConnectionType.Spouse && c.Entity is IGenPerson).Count();

    public IIndexedList<IGenPerson> Spouses => throw new NotImplementedException();

    public IGenDate BirthDate { get => Facts.GetFact(EFactType.Birth,t=>t.Date); set => throw new NotImplementedException(); }
    public IGenPlace BirthPlace { get => Facts.GetFact(EFactType.Birth, t => t.Place); set => throw new NotImplementedException(); }
    public IGenFact Birth { get => Facts.GetFact(EFactType.Birth); set => throw new NotImplementedException(); }
    public IGenDate BaptDate { get => Facts.GetFact(EFactType.Baptism, t => t.Date); set => throw new NotImplementedException(); }
    public IGenPlace BaptPlace { get => Facts.GetFact(EFactType.Baptism, t => t.Place); set => throw new NotImplementedException(); }
    public IGenFact Baptism { get => Facts.GetFact(EFactType.Baptism); set => throw new NotImplementedException(); }
    public IGenDate DeathDate { get => Facts.GetFact(EFactType.Death, t => t.Date); set => throw new NotImplementedException(); }
    public IGenPlace DeathPlace { get => Facts.GetFact(EFactType.Death, t => t.Place); set => throw new NotImplementedException(); }
    public IGenFact Death { get => Facts.GetFact(EFactType.Death); set => throw new NotImplementedException(); }
    public IGenDate BurialDate { get => Facts.GetFact(EFactType.Burial, t => t.Date); set => throw new NotImplementedException(); }
    public IGenPlace BurialPlace { get => Facts.GetFact(EFactType.Burial, t => t.Place); set => throw new NotImplementedException(); }
    public IGenFact Burial { get => Facts.GetFact(EFactType.Burial); set => throw new NotImplementedException(); }
    public string? Religion { get => Facts.GetFact(EFactType.Religion, t => t.Data); set => throw new NotImplementedException(); }
    public string? Occupation { get => Facts.GetFact(EFactType.Occupation, t => t.Data); set => throw new NotImplementedException(); }
    public IGenPlace OccuPlace { get => Facts.GetFact(EFactType.Occupation, t => t.Place); set => throw new NotImplementedException(); }
    public IGenPlace Residence { get => Facts.GetFact(EFactType.Residence, t => t.Place); set => throw new NotImplementedException(); }
    #endregion
    protected override IGenFact? GetEndFactOfEntity()
    {
        throw new NotImplementedException();
    }

    protected override IGenFact? GetStartFactOfEntity()
    {
        throw new NotImplementedException();
    }

    private void ParseName(string value, IList<IGenFact> facts)
    {
        throw new NotImplementedException();
    }

    private string BuildFullname(IList<IGenFact> facts)
    {
        throw new NotImplementedException();
    }

}
