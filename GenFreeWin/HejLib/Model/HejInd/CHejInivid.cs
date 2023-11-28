using System;
using System.Collections.Generic;
using WinAhnenCls.Model.GenBase;

namespace WinAhnenCls.Model.HejInd
{
    public partial class CHejInivid : IDisposable, IGenIndividual, IGenListProvider<IGenIndividual>, IGenListProvider<IGenFamily>, IGenListProvider<IGenEvent>
    {
        private bool _disposedValue;

        public CHejInivid()
        {
            this.Indi = new CHejIndiData(); // init DataHolder
            Children = new CGenList<IGenIndividual>(this, EGenListType.glt_Children);
            Spouses = new CGenList<IGenIndividual>(this, EGenListType.glt_Spouses);
            Families = new CGenList<IGenFamily>(this, EGenListType.glt_Families);
            //this.IndiRedir = EHejIndRedir.hIRd_Ind;
            //this.IndiMeta = EIndMetaData.hInMeD_ParentCount;
            _disposedValue = false;
        }

        public CHejIndiData Indi { get; set; }
        public EHejIndRedir IndiRedir { get; set; }
        public EIndMetaData IndiMeta { get; set; }

        // Basic-Properies
        public string Name { get => $"{Indi.FamilyName}, {Indi.GivenName}"; set => throw new NotImplementedException(); }
        public string GivenName { get => Indi.GivenName; set => Indi.GivenName = value; }
        public string Surname { get => Indi.FamilyName; set => Indi.FamilyName = value; }
        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Sex { get => Indi.Sex; set => Indi.Sex = value; }
        public string IndRefID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // Relationship-Properties
        public IGenIndividual Father { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenIndividual Mother { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int ChildCount => throw new NotImplementedException();

        public IGenList<IGenIndividual> Children { get; }

        public IGenFamily ParentFamily { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int FamilyCount => throw new NotImplementedException();

        public IGenList<IGenFamily> Families { get; }

        public int SpouseCount => throw new NotImplementedException();

        public IGenList<IGenIndividual> Spouses { get; }

        // Vital-Properties
        public string BirthDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BirthPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenEvent Birth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BaptDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BaptPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenEvent Baptism { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DeathDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DeathPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenEvent Death { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BurialDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string BurialPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IGenEvent Burial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Religion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Occupation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string OccuPlace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Residence { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int EventCount => throw new NotImplementedException();

        public IGenEntity._IEvents Events { get; }

        public string Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        DateTime? IGenData.LastChange => throw new NotImplementedException();

        object IGenData.This => this;


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public int Count(EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public IGenIndividual GetGenList(object idx, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void SetGenList(object idx, EGenListType lType, IGenIndividual value)
        {
            throw new NotImplementedException();
        }

        public object IndexOf(IGenIndividual item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Insert(object index, IGenIndividual item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IGenIndividual> GetEnumerator(EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(object index, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Add(IGenIndividual item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Clear(EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IGenIndividual[] array, int arrayIndex, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IGenIndividual item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IGenIndividual item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        IGenFamily IGenListProvider<IGenFamily>.GetGenList(object idx, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void SetGenList(object idx, EGenListType lType, IGenFamily value)
        {
            throw new NotImplementedException();
        }

        public object IndexOf(IGenFamily item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Insert(object index, IGenFamily item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        IEnumerator<IGenFamily> IGenListProvider<IGenFamily>.GetEnumerator(EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Add(IGenFamily item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IGenFamily[] array, int arrayIndex, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IGenFamily item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IGenFamily item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        IGenEvent IGenListProvider<IGenEvent>.GetGenList(object idx, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void SetGenList(object idx, EGenListType lType, IGenEvent value)
        {
            throw new NotImplementedException();
        }

        public object IndexOf(IGenEvent item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Insert(object index, IGenEvent item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        IEnumerator<IGenEvent> IGenListProvider<IGenEvent>.GetEnumerator(EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void Add(IGenEvent item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IGenEvent[] array, int arrayIndex, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IGenEvent item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IGenEvent item, EGenListType lType)
        {
            throw new NotImplementedException();
        }

        /*
       function GetBaptDate: string;
       function GetBaptism: IGenEvent;
       function GetBaptPlace: string;
       function GetBirth: IGenEvent;
       function GetBirthDate: string;
       function GetBirthPlace: string;
       function GetBurial: IGenEvent;
       function GetBurialDate: string;
       function GetBurialPlace: string;
       function GetChildrenCount: integer;
       function GetChildren(Idx: Variant): IGenIndividual;
       function GetDeath: IGenEvent;
       function GetDeathDate: string;
       function GetDeathPlace: string;
       function GetFamilies(Idx: Variant): IGenFamily;
       function GetFamilyCount: integer;
       function GetFather: IGenIndividual;
       function GetGivenName: string;
       function GetIndRefID: string;
       function GetMother: IGenIndividual;
       function GetName: string;
       function GetOccupation: string;
       function GetOccuPlace: string;
       function GetParentFamily: IGenFamily;
       function GetReligion: string;
       function GetResidence: string;
       function GetSex: string;
       function GetSpouseCount: integer;
       function GetSpouses(Idx: Variant): IGenIndividual;
       function GetSurname: string;
       function GetTimeStamp: TDateTime;
       function GetTitle: string;
{    Todo:   Mathoden Implementieren                            }
{       function EnumSpouses:IGenIndEnumerator;
       function EnumChildren:IGenIndEnumerator;
       function EnumFamilies:IGenFamEnumerator;       }
procedure SetBaptDate(AValue: string);
procedure SetBaptism(AValue: IGenEvent);
procedure SetBaptPlace(AValue: string);
procedure SetBirth(AValue: IGenEvent);
procedure SetBirthDate(AValue: string);
procedure SetBirthPlace(AValue: string);
procedure SetBurial(AValue: IGenEvent);
procedure SetBurialDate(AValue: string);
procedure SetBurialPlace(AValue: string);
procedure SetChildren(Idx: Variant; AValue: IGenIndividual);
procedure SetDeath(AValue: IGenEvent);
procedure SetDeathDate(AValue: string);
procedure SetDeathPlace(AValue: string);
procedure SetFamilies(Idx: Variant; AValue: IGenFamily);
procedure SetFather(AValue: IGenIndividual);
procedure SetGivenName(AValue: string);
procedure SetIndRefID(AValue: string);
procedure SetMother(AValue: IGenIndividual);
procedure SetName(AValue: string);
procedure SetOccupation(AValue: string);
procedure SetOccuPlace(AValue: string);
procedure SetParentFamily(AValue: IGenFamily);
procedure SetReligion(AValue: string);
procedure SetResidence(AValue: string);
procedure SetSex(AValue: string);
procedure SetSpouses(Idx: Variant; AValue: IGenIndividual);
procedure SetSurname(AValue: string);
procedure SetTimeStamp(AValue: TDateTime);
procedure SetTitle(AValue: string);
*/

    }
}
